// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.WebDemo;

// 企业微信「浏览器侧」Demo：网页授权（OAuth）+ JS-SDK 签名 + 应用发消息，与 ContactDemo（API）、
// ContactCallbackDemo（回调）构成 Work 线三件套。
// 说明：
//   1) SDK 不提供授权 URL 拼接，也不提供 JS-SDK 签名 —— 两者是官方强制的宿主侧职责，本 Demo 完整给出实现；
//   2) SDK 提供的是 code 换身份（IWechatWorkInternalIdentityService.GetUserInfoAsync）与两类 jsapi_ticket；
//   3) ticket 有「单应用 100 次/小时」的硬配额，必须宿主缓存（见 JsApiTicketCache），否则签名恒失败被误判为算法写错；
//   4) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md），仓库内不保留任何真实密钥。

// ① 网页授权与发消息都需要 AgentId（授权链接用 CorpId，消息归属用 AgentId），故本 Demo 的凭据比 ContactDemo 多一项。
var env = DemoEnv.RequireAll("WORK_CORP_ID", "WORK_AGENT_ID", "WORK_AGENT_SECRET");
var corpId = env["WORK_CORP_ID"];
var agentId = env["WORK_AGENT_ID"];

// 内容根钉在程序集目录：Demo 既可能 `dotnet run`（工作目录=工程目录），也可能被门禁脚本直接执行 bin 下的 dll，
// 用默认内容根会让后者找不到 wwwroot（表现为「接口全通、页面 404」）。
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "default";
    o.AppType = WechatAppType.Internal;
    o.CorpId = corpId;
    o.AgentId = agentId;
    o.AgentSecret = env["WORK_AGENT_SECRET"];
});

// ② 三个模块各自独立注册：身份识别（OAuth 换码）、JS-SDK（取 ticket）、消息（发送应用消息）。
builder.Services.AddWechatWorkServices(b => b
    .AddIdentityApi()
    .AddJsSdkApi()
    .AddMessageApi());

builder.Services.AddSingleton<JsApiTicketCache>();

var app = builder.Build();

// 令牌链（票据管理器）等业务错误以异常承载 ⇒ 统一映射 502 + 错误码，不把 SDK 堆栈交给浏览器。
app.UseWechatApiErrorGate();

// 页面走 wwwroot 静态文件（index.html 入口 + jssdk-demo.html 自检页）：
// 待签名 URL 必须是浏览器地址栏里的真实页面地址，用静态页最不容易在这一步出错。
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapDemoHealth("work-web", new
{
    appKey = "default",
    appType = WechatAppType.Internal.ToString(),
    modules = new[] { "Identity", "JsSdk", "Message" },
    routes = new[] { "/oauth/start", "/oauth/callback", "/jssdk/config", "/jssdk/agent-config", "/api/messages/text" },
    pages = new[] { "/", "/jssdk-demo.html" },
});

// ③ 构造网页授权链接并 302。官方规则：appid 填 CorpId（不是 AppId）、redirect_uri 需 URL encode、
//    scope 只支持 snsapi_base / snsapi_privateinfo，其余值官方拒绝。
app.MapGet("/oauth/start", (HttpContext context, string? scope) =>
{
    var effectiveScope = string.IsNullOrWhiteSpace(scope) ? "snsapi_base" : scope;
    if (!string.Equals(effectiveScope, "snsapi_base", StringComparison.Ordinal)
        && !string.Equals(effectiveScope, "snsapi_privateinfo", StringComparison.Ordinal))
    {
        return Results.BadRequest(new { error = $"scope 只支持 snsapi_base / snsapi_privateinfo，收到 {effectiveScope}" });
    }

    var redirectUri = BuildCallbackUrl(context.Request, "/oauth/callback");
    var state = JsSdkSignature.NewNonce();
    var authorizeUrl = "https://open.weixin.qq.com/connect/oauth2/authorize"
        + $"?appid={Uri.EscapeDataString(corpId)}"
        + $"&redirect_uri={Uri.EscapeDataString(redirectUri)}"
        + "&response_type=code"
        + $"&scope={effectiveScope}"
        + $"&state={state}#wechat_redirect";   // #wechat_redirect 官方必须保留，缺失则不会跳转

    return Results.Redirect(authorizeUrl);
});

// ④ code 换用户身份：成员返回 userid，非成员返回 openid，客户且跟进人可见时返回 external_userid。
//    code 最大 512 字节、仅可使用一次、5 分钟过期 —— 刷新页面重复用同一 code 必然失败，属预期行为。
app.MapGet("/oauth/callback",
    async (IWechatWorkInternalIdentityService identity, string? code, string? state, CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Results.BadRequest(new { error = "缺少 code（请从 /oauth/start 进入，不要直接访问本路由）" });
        }

        var info = await identity.GetUserInfoAsync(code, cancellationToken);

        // 声明式客户端不替调用方判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
        // 不判就把 40029（code 已用过）当成「拿到身份」，前端只会看到一个 userid 为空的正常响应。
        if (!info.IsSuccess)
        {
            return Results.Json(new { errorCode = info.ErrorCode, errorMessage = info.ErrorMessage },
                statusCode: StatusCodes.Status502BadGateway);
        }

        // user_ticket 是「可换敏感信息」的凭证（1800s 有效），只回显是否存在，不回显值。
        return Results.Json(new
        {
            state,
            userid = info.Userid,
            openid = info.Openid,
            externalUserid = info.ExternalUserid,
            hasUserTicket = !string.IsNullOrEmpty(info.UserTicket),
            hint = string.IsNullOrEmpty(info.Userid)
                ? "未返回 userid：授权 scope 为 snsapi_base 且用户非本企业内部成员（或不在应用可见范围）"
                : "可继续用 userid 调 /api/members/{userid} 拉取通讯录资料",
        });
    });

// ⑤ wx.config 签名：企业 jsapi_ticket（官方 getConfigSignature 语义）。
//    url 必须是「当前页面的完整地址、去掉 # 及之后」，且与浏览器实际地址逐字符一致 —— 由前端显式传入，
//    本端点无法反推（它就是被前端调用的，自己的 URL 不是待签名页面）。
app.MapGet("/jssdk/config",
    async (IWechatWorkInternalJsSdkService jsSdk, JsApiTicketCache cache, string? url, CancellationToken cancellationToken) =>
    {
        var target = NormalizeSignatureUrl(url);
        if (target == null)
        {
            return Results.BadRequest(new { error = "缺少 url 参数：须传待签名页面的完整 URL（不含 # 及之后部分）" });
        }

        var (ticket, expiresAt) = await cache.GetOrCreateAsync(
            "corp",
            async ct =>
            {
                var response = await jsSdk.GetCorpJsapiTicketAsync(ct);

                // ticket 取空会让签名「看起来成功、浏览器恒报 invalid signature」，故在缓存入口判错并上抛。
                WechatWorkException.ThrowIfFailed(response);

                return (response.Ticket, response.ExpiresIn);
            },
            cancellationToken);

        var nonceStr = JsSdkSignature.NewNonce();
        var timestamp = JsSdkSignature.CurrentTimestamp();

        return Results.Json(new
        {
            appId = corpId,
            timestamp,
            nonceStr,
            signature = JsSdkSignature.Compute(ticket, nonceStr, timestamp, target),
            jsApiList = new[] { "selectExternalContact", "shareWechatMessage", "agentConfig" },
            signedUrl = target,
            ticketExpiresAt = expiresAt,
        });
    });

// ⑥ wx.agentConfig 签名：应用 jsapi_ticket（与 corp ticket 分槽、有效期各自独立）。
//    agentConfig 除签名四元组外还需前端带上 corpid 与 agentid，故两者一并返回。
app.MapGet("/jssdk/agent-config",
    async (IWechatWorkInternalJsSdkService jsSdk, JsApiTicketCache cache, string? url, CancellationToken cancellationToken) =>
    {
        var target = NormalizeSignatureUrl(url);
        if (target == null)
        {
            return Results.BadRequest(new { error = "缺少 url 参数：须传待签名页面的完整 URL（不含 # 及之后部分）" });
        }

        var (ticket, expiresAt) = await cache.GetOrCreateAsync(
            "agent",
            async ct =>
            {
                var response = await jsSdk.GetAgentJsapiTicketAsync(ct);

                // 同企业 ticket：取空即签名无效，宁可抛错也不要把空 ticket 缓存 2 小时。
                WechatWorkException.ThrowIfFailed(response);

                return (response.Ticket, response.ExpiresIn);
            },
            cancellationToken);

        var nonceStr = JsSdkSignature.NewNonce();
        var timestamp = JsSdkSignature.CurrentTimestamp();

        return Results.Json(new
        {
            corpId,
            agentId = agentId,
            timestamp,
            nonceStr,
            signature = JsSdkSignature.Compute(ticket, nonceStr, timestamp, target),
            signedUrl = target,
            ticketExpiresAt = expiresAt,
        });
    });

// ⑦ 发送应用文本消息（写操作，默认不映射 —— 请求得到 404；MUDWECHAT_DEMO_ALLOW_WRITE=1 后启用）。
if (DemoWriteGate.Enabled)
{
    app.MapPost("/api/messages/text",
        async (IWechatWorkInternalMessageService messages, SendTextBody body, CancellationToken cancellationToken) =>
        {
            var response = await messages.SendTextMessageAsync(new MessageSendTextRequest
            {
                ToUser = body.ToUser,
                ToParty = body.ToParty,
                AgentId = int.TryParse(agentId, out var parsedAgentId) ? parsedAgentId : null,
                MsgType = "text",
                Text = new MessageTextBody { Content = body.Content },

                // 官方：safe=1 不能分享且显示水印；enable_id_trans=1 允许把 userid 等转译为可读名称。
                Safe = body.Safe,
                EnableIdTrans = 1,
            }, cancellationToken);

            // errcode != 0 是「整条消息未被受理」（如 81013 不在可见范围），
            // 与 errcode = 0 + invaliduser 非空（部分接收者无效）是两种结论，必须分开回传。
            if (!response.IsSuccess)
            {
                return Results.Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage },
                    statusCode: StatusCodes.Status502BadGateway);
            }

            return Results.Json(new
            {
                msgId = response.MsgId,
                invalidUser = response.InvalidUser,
                invalidParty = response.InvalidParty,
                unlicensedUser = response.UnlicensedUser,
            });
        });
}

DemoWriteGate.Announce(app.Logger);

app.Run();

// 反向代理下 Scheme/Host 需由转发头还原（README 说明 UseForwardedHeaders 的接法），此处按当前请求构造。
static string BuildCallbackUrl(HttpRequest request, string path)
    => $"{request.Scheme}://{request.Host}{path}";

// 官方签名串里的 url 不参与 URL encode，且须去掉 # 及之后部分；除此之外不得做任何「规整」。
static string? NormalizeSignatureUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url))
    {
        return null;
    }

    var fragmentIndex = url.IndexOf('#', StringComparison.Ordinal);
    return fragmentIndex >= 0 ? url[..fragmentIndex] : url;
}

/// <summary>发消息请求体（Demo 自用的最小入参，避免让调用方拼官方全量信封）。</summary>
internal sealed class SendTextBody
{
    public string? ToUser { get; set; }

    public string? ToParty { get; set; }

    public string Content { get; set; } = string.Empty;

    public int? Safe { get; set; }
}
