// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Demo.Shared;
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.Abstractions;
using Mud.Wechat.OfficialAccount.Callback;
using Mud.Wechat.OfficialAccount.DataModels.Template;
using Mud.Wechat.OfficialAccount.Demo.Handlers;
using Mud.Wechat.OfficialAccount.Demo.Services;
using Mud.Wechat.OfficialAccount.Extensions;
using Mud.Wechat.OfficialAccount.Web;

// 公众号 / 服务号 Demo（DM-04）：一条工程覆盖 OAuth 网页授权、JS-SDK 签名、模板消息、
// 消息与事件回调 + 被动回复 —— 对应 Senparc Samples 里 OAuth2Controller / JSSDKController /
// TemplateApi / WeixinController 四个样例的合并面。
// 说明：
//   1) sns 四端点「整域免应用令牌」：授权码换的是**用户级** access_token，参数须显式带 appid+secret，
//      与应用级 access_token（业务接口用）是两套凭据，不可混用；
//   2) JS-SDK 签名 SDK 已内置（AddMpJsApiSignature），与企微线「宿主自己算签名」形成对照 —— 见 DM-03；
//   3) 回调路由 /mp/{AppKey}，安全模式（密文）为默认；两道 fail-closed 闸（时效 + 一次性指纹）不可为跑通而削弱；
//   4) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md）。

var env = DemoEnv.RequireAll("MP_APP_ID", "MP_APP_SECRET", "MP_CALLBACK_TOKEN", "MP_CALLBACK_AES_KEY");
var appId = env["MP_APP_ID"];
var appSecret = env["MP_APP_SECRET"];

// 模板消息另需模板 ID（服务号 + 已认证 + 模板库选取），留空则该端点直接给出可执行的下一步提示。
var templateId = DemoEnv.Optional("MP_TEMPLATE_ID");

// 内容根钉在程序集目录：门禁脚本会直接执行 bin 下的 dll，默认内容根（当前工作目录）会找不到 wwwroot。
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

// ① 公众号底座：AppId + AppSecret 换应用级 access_token（UseStableToken 默认 true，走稳定令牌）。
builder.Services.AddMpApp(o =>
{
    o.AppKey = "default";
    o.AppId = appId;
    o.AppSecret = appSecret;
});

builder.Services.AddMpServices(b => b
    .AddSnsApi()        // 网页授权（免应用令牌四端点）
    .AddUserApi()       // 用户信息（换取用户资料、备注等）
    .AddTemplateApi()); // 模板消息（服务号专属）

// ② JS-SDK 签名服务：须在 AddMpApp 之后（依赖票据管理器），产出 wx.config 五字段且**不返回 ticket**。
builder.Services.AddMpJsApiSignature();

builder.Services.AddSingleton<MpReplyDeduper>();

// ③ 回调凭据的唯一来源是 MpCallbackOptions.Apps（MpAppConfig 不提供推送属性）。
builder.Services.AddMpCallback(o =>
{
    o.GlobalRoutePrefix = "mp";
    o.Apps["default"] = new MpAppCallbackOptions
    {
        PushToken = env["MP_CALLBACK_TOKEN"],
        PushEncodingAESKey = env["MP_CALLBACK_AES_KEY"],

        // AppId 参与解密后 ToUserName 交叉校验：不填则失去「推给了别的应用」的识别能力。
        AppId = appId,
    };
})
    .AddHandler<SubscribeEventHandler>()     // 事件族：关注/扫码
    .AddHandler<TextMessageHandler>()        // 消息族：文本
    .AddReplyHandler<WelcomeReplyHandler>(); // 被动回复（返回式）

var app = builder.Build();

// 令牌链等业务错误以异常承载（区别于生成 JSON 端点的「HTTP 200 + errcode」）⇒ 统一映射 502 + 错误码。
app.UseWechatApiErrorGate();

app.UseDefaultFiles();
app.UseStaticFiles();

// ④ 回调中间件：GET echostr 验证 + POST 消息/事件接收（放在端点映射之前即可，路由前缀独立）。
app.UseMpCallback();

app.MapDemoHealth("official-account", new
{
    appKey = "default",
    securityMode = "Safe（默认，密文）",
    callbackRoute = "/mp/default",
    modules = new[] { "Sns", "User", "Template" },
    handlers = new[] { "subscribe", "text", "reply:welcome" },
    templateConfigured = templateId != null,
});

// ⑤ 网页授权入口：scope 只支持 snsapi_base（静默）/ snsapi_userinfo（需用户同意）。
//    把 scope 放进 state 回传 —— 微信回调只带 code + state，不带 scope。
app.MapGet("/oauth/start", (HttpRequest request, string? scope) =>
{
    var effectiveScope = string.IsNullOrWhiteSpace(scope) ? "snsapi_base" : scope;
    if (!string.Equals(effectiveScope, "snsapi_base", StringComparison.Ordinal)
        && !string.Equals(effectiveScope, "snsapi_userinfo", StringComparison.Ordinal))
    {
        return Results.BadRequest(new { error = $"scope 只支持 snsapi_base / snsapi_userinfo，收到 {effectiveScope}" });
    }

    var redirectUri = $"{request.Scheme}://{request.Host}/oauth/callback";
    var authorizeUrl = "https://open.weixin.qq.com/connect/oauth2/authorize"
        + $"?appid={Uri.EscapeDataString(appId)}"
        + $"&redirect_uri={Uri.EscapeDataString(redirectUri)}"
        + "&response_type=code"
        + $"&scope={effectiveScope}"
        + $"&state={effectiveScope}#wechat_redirect";

    return Results.Redirect(authorizeUrl);
});

// ⑥ code 换用户身份：用户级令牌与 openid 一次拿到；snsapi_userinfo 再拉昵称/头像。
//    code 仅可使用一次、5 分钟过期；用户级 access_token（30 天 refresh_token 生命周期）归宿主保管，
//    本 Demo 只回显「是否拿到」，不回显值 —— 令牌出现在响应里等于把它交给浏览器。
app.MapGet("/oauth/callback",
    async (IMpSnsService sns, string? code, string? state, CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Results.BadRequest(new { error = "缺少 code（请从 /oauth/start 进入）" });
        }

        var token = await sns.GetAccessTokenAsync(
            appId, appSecret, code, MpSnsGrantTypes.AuthorizationCode, cancellationToken);

        // 声明式客户端不替调用方判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
        // sns 域最常见的 40029（code 已用过/超 5 分钟）若不经判，会被当成「换到了身份」。
        if (!token.IsSuccess)
        {
            return Results.Json(new { errorCode = token.ErrorCode, errorMessage = token.ErrorMessage },
                statusCode: StatusCodes.Status502BadGateway);
        }

        var isUserInfo = string.Equals(state, "snsapi_userinfo", StringComparison.Ordinal);
        object? profile = null;
        if (isUserInfo && !string.IsNullOrEmpty(token.AccessToken) && !string.IsNullOrEmpty(token.OpenId))
        {
            var info = await sns.GetUserInfoAsync(token.AccessToken, token.OpenId, "zh_CN", cancellationToken);
            if (!info.IsSuccess)
            {
                return Results.Json(new { errorCode = info.ErrorCode, errorMessage = info.ErrorMessage },
                    statusCode: StatusCodes.Status502BadGateway);
            }

            profile = new { nickname = info.Nickname, sex = info.Sex, city = info.City, headImgUrl = info.HeadImgUrl };
        }

        return Results.Json(new
        {
            openId = token.OpenId,
            unionId = token.UnionId,
            scope = state,
            hasUserAccessToken = !string.IsNullOrEmpty(token.AccessToken),
            hasRefreshToken = !string.IsNullOrEmpty(token.RefreshToken),
            profile,
            hint = isUserInfo
                ? "snsapi_userinfo 已拉取用户资料；生产须把用户级令牌按 openId 持久化并有 refresh_token 续期"
                : "snsapi_base 只给 openid；需要昵称头像请走 snsapi_userinfo（用户需同意）",
        });
    });

// ⑦ JS-SDK 签名：SDK 内置算法与 ticket 缓存，宿主只需把当前页面 URL 交给 SignAsync。
//    与 DM-03（企微线要求宿主自己算签名）对照 —— 两条线的边界不同，别互相套用。
app.MapGet("/jssdk/config",
    async (IMpJsApiSignatureService signature, string? url, CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Results.BadRequest(new { error = "缺少 url 参数：须传待签名页面的完整 URL（# 片段 SDK 会自动去除）" });
        }

        var result = await signature.SignAsync(url, cancellationToken);

        // 直接透传 SDK 结果：AppId / Url / NonceStr / TimeStamp / Signature 五字段，刻意不含 jsapi_ticket。
        return Results.Json(result);
    });

// ⑧ 发送模板消息（写操作，默认不映射 —— 请求得到 404；MUDWECHAT_DEMO_ALLOW_WRITE=1 后启用）。
if (DemoWriteGate.Enabled)
{
    app.MapPost("/api/templates/send",
        async (IMpTemplateService templates, SendTemplateBody body, CancellationToken cancellationToken) =>
        {
            if (templateId == null)
            {
                return Results.BadRequest(new { error = "未配置 MUDWECHAT_DEMO_MP_TEMPLATE_ID（模板消息需先在模板库选取模板）" });
            }

            if (body.Data == null || body.Data.Count == 0)
            {
                return Results.BadRequest(new { error = "data 不能为空：键名须与所选模板的字段（first/keyword1/.../remark）逐项一致" });
            }

            var data = new Dictionary<string, MpTemplateDataValue>();
            foreach (var (key, value) in body.Data)
            {
                data[key] = new MpTemplateDataValue { Value = value };
            }

            var response = await templates.SendTemplateMessageAsync(new MpSendTemplateMessageRequest
            {
                ToUser = body.ToUser,
                TemplateId = templateId,
                Url = body.Url,
                Data = data,
            }, cancellationToken);

            // 模板消息的 errcode 同样不经 SDK 抛出（40013/40036/41010 等一律 HTTP 200 + 非零 errcode）；
            // 真正送达还要看 TEMPLATESENDJOBFINISH 回调事件，本端点只保证「受理结论如实回传」。
            if (!response.IsSuccess)
            {
                return Results.Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage },
                    statusCode: StatusCodes.Status502BadGateway);
            }

            return Results.Json(new { msgId = response.MsgId });
        });
}

DemoWriteGate.Announce(app.Logger);

app.Run();

/// <summary>模板消息请求体（Demo 自用的扁平入参：字段名 → 值，避免调用方拼 SDK 字典类型）。</summary>
internal sealed class SendTemplateBody
{
    public string ToUser { get; set; } = string.Empty;

    public string? Url { get; set; }

    public Dictionary<string, string> Data { get; set; } = new();
}
