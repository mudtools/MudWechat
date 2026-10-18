// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Demo.Shared;
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.QrCodeLink;
using Mud.Wechat.MiniProgram.DataModels.SubscribeMessage;
using Mud.Wechat.MiniProgram.Extensions;
using Mud.Wechat.OfficialAccount.Abstractions;

// 微信小程序 Demo（DM-05）：服务端最高频的三件事 —— 登录换会话、生成小程序码、发送订阅消息。
// 说明：
//   1) 小程序与公众号同属微信公众平台、同一令牌域 ⇒ 底座仍是 AddMpApp，本线只装业务模块；
//   2) code2Session 免应用级令牌（方法级 [Token(Disabled = true)]），拿到的 session_key 是会话密钥，
//      既不下发前端也不入日志（红线 MP-X7）；
//   3) 小程序码「成功是图片流、失败才是 JSON」，SDK 已按 Content-Type 分支判错，宿主只处理流；
//   4) 小程序线没有回调工程：消息与事件接收属公众号 XML 通道（Mud.Wechat.OfficialAccount.Callback）；
//   5) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md）。

var env = DemoEnv.RequireAll("WXA_APP_ID", "WXA_APP_SECRET");
var appId = env["WXA_APP_ID"];
var appSecret = env["WXA_APP_SECRET"];

// 订阅消息另需模板 ID（小程序后台「订阅消息」选用模板 + 端上 wx.requestSubscribeMessage 授权），
// 留空则该端点直接给出可执行的下一步提示。
var templateId = DemoEnv.Optional("WXA_TEMPLATE_ID");

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

// ① 令牌底座来自公众号 Abstractions（per-app 令牌管理器 + 上下文切换器）。
builder.Services.AddMpApp(o =>
{
    o.AppKey = "default";
    o.AppId = appId;
    o.AppSecret = appSecret;
});

// ② 本线模块：Build() 会校验 IMpAppManager 已注册 ⇒ 两段装配顺序颠倒即注册期 fail-fast，
//    不会静默少装模块。
builder.Services.AddMiniProgramServices(b => b
    .AddAuthApi()                // 登录与用户（8 端点，含免令牌的 jscode2session）
    .AddQrCodeLinkApi()          // 小程序码（图片流手工通道）+ URL Link / Scheme
    .AddSubscribeMessageApi());  // 订阅消息发送与服务卡片

var app = builder.Build();

// 令牌链与小程序码二进制通道以异常承载业务错误（与生成 JSON 端点的「HTTP 200 + errcode」不同形）
// ⇒ 统一映射为 502 + 错误码，而不是把一段 SDK 内部堆栈交给浏览器。
app.UseWechatApiErrorGate();

app.MapDemoHealth("mini-program", new
{
    appKey = "default",
    tokenBase = "Mud.Wechat.OfficialAccount.Abstractions（与公众号同令牌域）",
    modules = new[] { "Auth", "QrCodeLink", "SubscribeMessage" },
    callback = "本线无回调包：消息与事件接收走公众号 XML 通道",
    templateConfigured = templateId != null,
});

// ③ 登录凭证校验：js_code 换 openid + session_key。
//    该端点以 appid + secret 换会话、不消费应用级 access_token，故免令牌；
//    js_code 一次性且 5 分钟过期 ⇒ 重试同一 code 必得 40029，属官方契约而非 SDK 缺陷。
app.MapPost("/api/session",
    async (IWxaAuthService auth, SessionBody body, CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(body.JsCode))
        {
            return Results.BadRequest(new { error = "缺少 jsCode：须由小程序端 wx.login 现场产生（一次性、5 分钟有效）" });
        }

        var session = await auth.Code2SessionAsync(
            appId, appSecret, body.JsCode, WxaGrantTypes.AuthorizationCode, cancellationToken);

        // 声明式客户端不替调用方判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功，
        // 判错契约（IWechatApiResponse）由调用方消费 —— 不判就把「invalid code」当成「拿到 openid」。
        // 常见码：40013 invalid appid / 40029 invalid code（已用过或超 5 分钟）/ 45011 频率限制 / 40226 高风险用户。
        if (!session.IsSuccess)
        {
            return Results.Json(new { errorCode = session.ErrorCode, errorMessage = session.ErrorMessage },
                statusCode: StatusCodes.Status502BadGateway);
        }

        // session_key 只回显「是否拿到」：下发前端等于把会话密钥交给客户端，
        // 记日志则违反 MP-X7；生产应把它与 openid 一并按用户持久化，用于解密端上加密数据与校验登录态。
        return Results.Json(new
        {
            openId = session.OpenId,
            unionId = session.UnionId,
            hasSessionKey = !string.IsNullOrEmpty(session.SessionKey),
            hint = session.UnionId == null
                ? "未返回 unionid：须小程序已绑定微信开放平台，且该用户已关注同主体公众号"
                : "已拿到 unionid，可用于跨应用识别同一用户",
        });
    });

// ④ 不限量小程序码：数量不受 10 万限制，代价是 scene 最长 32 个可见字符。
app.MapGet("/api/qrcode",
    async (IWxaCodeService codes, string? scene, string? page, CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(scene))
        {
            return Results.BadRequest(new { error = "缺少 scene（官方必填，最长 32 个可见字符）" });
        }

        if (scene.Length > 32)
        {
            // 本地预检只为把原因说清（官方对超长 scene 只回笼统错误码），不改变契约、不放宽任何校验。
            return Results.BadRequest(new { error = $"scene 最长 32 个可见字符，收到 {scene.Length} 个" });
        }

        using var result = await codes.GetUnlimitedCodeAsync(new WxaCodeUnlimitRequest
        {
            Scene = scene,
            Page = page,

            // 开发/体验期页面常未发布，置 false 放行；正式发布前应回默认 true（官方据此校验页面存在）。
            CheckPath = false,
        }, cancellationToken);

        // 先缓冲再一次交回：IResult 在委托返回之后才写响应，而 using 到点即释放底层响应流
        // —— 直接 Results.Stream(result.Content) 会拿到已关闭的流；小程序码只有几十 KB，缓冲更简单。
        using var buffer = new MemoryStream();
        await result.Content.CopyToAsync(buffer, cancellationToken);

        // 非透明底出 JPEG、is_hyaline=true 出透明 PNG ⇒ 一律以 SDK 回传的 Content-Type 为准。
        return Results.Bytes(buffer.ToArray(), result.ContentType ?? "image/jpeg");
    });

// ⑤ 发送订阅消息（写操作，默认不映射 —— 请求得到 404；MUDWECHAT_DEMO_ALLOW_WRITE=1 后启用）。
//    一次调用只下发一条，且须用户先在小程序端订阅过该模板（未订阅报 43101）。
if (DemoWriteGate.Enabled)
{
    app.MapPost("/api/subscribe/send",
        async (IWxaSubscribeMessageService subscriptions, SubscribeBody body, CancellationToken cancellationToken) =>
        {
            if (templateId == null)
            {
                return Results.BadRequest(new { error = "未配置 MUDWECHAT_DEMO_WXA_TEMPLATE_ID（须先在小程序后台订阅消息处选用模板）" });
            }

            if (string.IsNullOrWhiteSpace(body.ToUser))
            {
                return Results.BadRequest(new { error = "toUser 不能为空：取 /api/session 返回的 openid" });
            }

            if (body.Data == null || body.Data.Count == 0)
            {
                return Results.BadRequest(new { error = "data 不能为空：键名须与所选模板的关键词编号（thing1/character_string2/...）逐项一致，否则报 47003" });
            }

            var data = new Dictionary<string, WxaSubscribeMessageDataValue>();
            foreach (var (key, value) in body.Data)
            {
                data[key] = new WxaSubscribeMessageDataValue { Value = value };
            }

            var response = await subscriptions.SendSubscribeMessageAsync(new WxaSubscribeMessageSendRequest
            {
                Touser = body.ToUser,
                TemplateId = templateId,
                Page = body.Page,

                // 体验版/开发版模板内容可能与正式版不同（官方环境差异类错误），默认按正式版发送。
                MiniprogramState = body.MiniprogramState ?? "formal",
                Data = data,
            }, cancellationToken);

            // 与登录端点同一口径：43101（用户未订阅）/ 47003（关键词与模板不匹配）都是 HTTP 200 + 非零 errcode，
            // 判错必须由调用方做，否则「没送到」会被当成「发送成功」。
            if (!response.IsSuccess)
            {
                return Results.Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage },
                    statusCode: StatusCodes.Status502BadGateway);
            }

            return Results.Json(new { accepted = true, errMsg = response.ErrorMessage });
        });
}

DemoWriteGate.Announce(app.Logger);

app.Run();

/// <summary>登录请求体（Demo 自用扁平入参：只接小程序端 wx.login 的 code）。</summary>
internal sealed class SessionBody
{
    public string? JsCode { get; set; }
}

/// <summary>订阅消息请求体（Demo 自用扁平入参：关键词编号 → 值，避免调用方拼 SDK 字典类型）。</summary>
internal sealed class SubscribeBody
{
    public string? ToUser { get; set; }

    public string? Page { get; set; }

    public string? MiniprogramState { get; set; }

    public Dictionary<string, string>? Data { get; set; }
}
