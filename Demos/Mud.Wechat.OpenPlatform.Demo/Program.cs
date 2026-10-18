// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels.Component;
using Mud.Wechat.OpenPlatform.DataModels.Embedded;
using Mud.Wechat.OpenPlatform.Demo;
using Mud.Wechat.OpenPlatform.Extensions;

// 微信开放平台第三方平台 Demo（DM-09）：跑通「票据 → 平台令牌 → 预授权 → 用户授权 → 换令牌 → 代授权方调用」。
// 读代码前先记住七件事：
//   1) 本线**没有 Callback 包，也没有中间件**：「授权事件接收 URL」由宿主读原始请求体转交
//      ComponentVerifyTicketReceiver —— 这是官方形态，不是 SDK 缺面（勿与公众号/企微线的 XML 通道类比）；
//   2) 凭证链的**起点在外部**：component_verify_ticket 只能由微信后台每 10 分钟推送，SDK 无法请求获取。
//      没收到过票据时，除引导推票外的端点全部失败，这是链路真相而非缺陷 —— 启动后先看 /api/ticket；
//   3) 只有 Accepted / AuthorizerEvent 两类回 "success"，其余必须回非 success（让微信重试、失败在监控上可见）；
//   4) 默认存储是**进程内**：多实例部署必须先 TryAdd 预注册分布式实现，否则推送只打到一台、其余取不到令牌；
//   5) 令牌 / 票据 / 授权码 / 刷新令牌 / component_secret **一律不回显原文**（只回长度与指纹前缀）；
//      授权码走请求体而非 URL 参数 —— ASP.NET Core 的请求日志会把完整 URL（含查询串）写进日志；
//   6) 声明式 JSON 端点是「HTTP 200 + errcode」语义，判错由调用方做（IsSuccess）；
//      凭证链则以 WechatOpenPlatformException 承载失败，且它**不派生自** WechatApiException ⇒ 需要本线专属错误闸；
//   7) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md）。

var env = DemoEnv.RequireAll(
    "OPENPLATFORM_COMPONENT_APP_ID",
    "OPENPLATFORM_COMPONENT_APP_SECRET",
    "OPENPLATFORM_TOKEN",
    "OPENPLATFORM_AES_KEY");

var componentAppId = env["OPENPLATFORM_COMPONENT_APP_ID"];
var componentAppSecret = env["OPENPLATFORM_COMPONENT_APP_SECRET"];
var pushToken = env["OPENPLATFORM_TOKEN"];
var encodingAesKey = env["OPENPLATFORM_AES_KEY"];

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

// ① 凭证链底座：四个凭据缺任一即**注册期** fail-fast（EnsureValid 里含「AESKey 必须 43 位」硬校验）。
//    TryAdd 语义 ⇒ 多实例部署时宿主的分布式存储预注册必须在 AddOpenPlatform 之前。
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = componentAppId;
    c.ComponentAppSecret = componentAppSecret;
    c.Token = pushToken;
    c.EncodingAesKey = encodingAesKey;
});

// ② 业务客户端按模块注册（本线只有 AddModules/AddAllApis，没有各线那种 Add{域}Api() 扩展方法）。
//    Component = 平台管理面 + 免令牌推票引导（同一注册组，一次覆盖两支接口）；
//    Embedded  = 半屏小程序列表，令牌值是 authorizer_access_token ⇒ 本 Demo 用它演示授权方作用域。
builder.Services.AddOpenPlatformApis(b => b.AddModules(
    OpenPlatformModule.Component,
    OpenPlatformModule.Embedded));

builder.Services.AddSingleton<OpenPlatformLedger>();

var app = builder.Build();

// 通用闸在前、本线闸在后：app.Use 越晚注册越靠内、越先捕获异常（顺序写反不报错，只会把 502 退化成 500 + 堆栈）。
app.UseWechatApiErrorGate();
app.UseWechatOpenPlatformErrorGate();

app.MapDemoHealth("open-platform", new
{
    componentAppId,
    tokenType = "Wechat.OpenPlatform.ComponentAccessToken / AuthorizerAccessToken（与企微、公众号令牌域天然隔离）",
    modules = new[] { "Component", "Embedded" },
    pushRoute = "/wechat/component",
    // 原文不回显：这里只给「是否配置」，AESKey 长度校验已由注册期 EnsureValid 承担。
    pushCredentialsConfigured = true,
    storage = "默认进程内（IComponentVerifyTicketStore / IAuthorizerTokenStore）；多实例必须换分布式实现",
    writeGate = DemoWriteGate.Enabled ? "已启用（ALLOW_WRITE=1）" : "已关闭",
});

// —— 辅助：把凭据压成「长度 + 指纹前缀」，既能证明「拿到了/换了新的」，又不外泄原文。
static string Mask(string? value)
{
    if (string.IsNullOrEmpty(value))
    {
        return "<空>";
    }

    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
    return $"{value.Length}位/sha256:{Convert.ToHexString(hash.AsSpan(0, 4)).ToLowerInvariant()}";
}

// —— 辅助：授权码 → 令牌 → 落库，两个入口（人工粘贴 / 从推送自动取）共用同一段流程。
static async Task<IResult> ExchangeAuthCodeAsync(
    string authCode,
    IComponentAuthorizationService authorization,
    IAuthorizerTokenProvider tokenProvider,
    IAuthorizerTokenStore tokenStore,
    OpenPlatformLedger ledger,
    CancellationToken cancellationToken)
{
    var tokens = await authorization.QueryAuthorizationAsync(authCode, cancellationToken).ConfigureAwait(false);

    // 刷新令牌是长期凭据：拿到即落库，否则进程重启就要用户重新授权。
    tokenProvider.AcceptAuthorization(tokens);
    var stored = tokenStore.TryGet(tokens.AuthorizerAppId, out var snapshot);

    ledger.RecordAuthorization(tokens, stored, DateTimeOffset.UtcNow);

    return Results.Json(new
    {
        authorizerAppId = tokens.AuthorizerAppId,
        hasApiScope = tokens.HasApiScope,
        accessToken = Mask(tokens.AccessToken),
        refreshTokenStored = stored,
        refreshTokenLength = snapshot?.RefreshToken?.Length ?? 0,
        accessTokenExpiresAt = tokens.AccessTokenExpiresAt,
        hint = tokens.HasApiScope
            ? "已拿到接口调用令牌（原文不回显）；刷新令牌已按 authorizer_appid 落库"
            : "AccessToken 为空属**正常形态**：官方只在授权含 API 权限时返回 authorizer_access_token（不是换取失败）",
    });
}

// ① 授权事件接收 URL：官方形态是「同一个 URL 收两类推送」——票据 + 授权变更事件。
//    请求体只能读一次，读完把原文交给接收器；日志只写处置分类与 InfoType（非敏感）。
app.MapPost("/wechat/component", async (
    HttpRequest request,
    ComponentVerifyTicketReceiver receiver,
    OpenPlatformLedger ledger,
    CancellationToken cancellationToken) =>
{
    string body;
    using (var reader = new StreamReader(request.Body))
    {
        body = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }

    var result = receiver.Receive(
        request.Query["msg_signature"].ToString(),
        request.Query["timestamp"].ToString(),
        request.Query["nonce"].ToString(),
        body);

    ledger.Record(result, DateTimeOffset.UtcNow);

    // 取消授权后，那条授权码不该再被换令牌（官方也会让它失效，但本地留着只会误导演示者）。
    if (result.AuthorizerEvent?.IsUnauthorized == true)
    {
        ledger.ClearPendingAuthCode(result.AuthorizerEvent.AuthorizerAppId);
    }

    app.Logger.LogInformation(
        "开放平台推送处置：{Outcome} / InfoType={InfoType} / 授权方={AuthorizerAppId}",
        result.Outcome,
        result.InfoType ?? "-",
        result.AuthorizerEvent?.AuthorizerAppId ?? "-");

    return result.ShouldReturnSuccess
        ? Results.Text(ComponentVerifyTicketReceiver.SuccessResponse)
        : Results.Text("fail");
});

// ② 票据快照：整条凭证链的第一块 Domino。只回长度与指纹，绝不回票据原文。
app.MapGet("/api/ticket", (IComponentVerifyTicketStore store) =>
{
    var hit = store.TryGet(out var snapshot);
    return Results.Json(new
    {
        hasTicket = hit,
        receivedAt = snapshot?.ReceivedAt,
        ticketLength = snapshot?.Ticket?.Length ?? 0,
        fingerprint = Mask(snapshot?.Ticket),
        hint = hit
            ? "微信后台每 10 分钟推一次；空白推送被存储层**忽略**而非覆盖（旧票据多撑一轮好过清成空）"
            : "尚无票据：等推送到达，或在 ALLOW_WRITE=1 下调用 POST /api/start-push-ticket 引导微信开始推送",
    });
});

// ③ 平台令牌：验证「票据 → component_access_token」这一段真的通（缓存/提前刷新/并发单飞由 SDK 负责）。
app.MapGet("/api/component-token", async (
    IComponentTokenProvider provider,
    CancellationToken cancellationToken) =>
{
    var token = await provider.GetComponentAccessTokenAsync(cancellationToken).ConfigureAwait(false);
    return Results.Json(new
    {
        acquired = true,
        length = token.Length,
        fingerprint = Mask(token),
        hint = "不回显令牌原文：官方强制 Query 传令牌，进日志/回显即凭据外泄（AGENTS §8）。"
            + "取不到时由本线错误闸给出 502 + errorCode。",
    });
});

// ④ 手动失效平台令牌：演示 errcode 恢复路径的同一入口（只清缓存，不打官方接口）。
app.MapPost("/api/component-token/invalidate", (IComponentTokenProvider provider) =>
{
    provider.Invalidate();
    return Results.Json(new { invalidated = true, hint = "下次取用将强制重取；票据与授权关系不受影响" });
});

// ⑤ 预授权码（1800 秒）：授权页 URL 的拼接**不由 SDK 提供**（官方参数表未逐字核验，
//    按「未核验不臆造」纪律留给宿主）⇒ 本端点只给码与有效期，README 给出官方流程页链接。
app.MapGet("/api/preauth", async (
    IComponentAuthorizationService authorization,
    CancellationToken cancellationToken) =>
{
    var pre = await authorization.CreatePreAuthCodeAsync(cancellationToken).ConfigureAwait(false);
    return Results.Json(new
    {
        preAuthCode = pre.PreAuthCode,
        expiresAt = pre.ExpiresAt,
        hint = "用 pre_auth_code 按官方《授权流程》页拼出授权页 URL 让管理员扫码/点击授权；"
            + "授权完成后微信会带 auth_code 回调 redirect_uri，同时向接收 URL 推 authorized 事件。"
            + "本 Demo 不代拼 URL（SDK 未提供拼接方法，臆造参数表比缺方法更坏）。",
    });
});

// ⑥ 授权码换令牌（人工粘贴）：**POST + 请求体**，不是规划里的 GET /api/auth/query?authCode= ——
//    授权码是一次性凭据，放 URL 会被 ASP.NET Core 的请求日志整条写进日志文件（AGENTS §8 红线）。
app.MapPost("/api/auth/query", async (
    AuthCodeBody body,
    IComponentAuthorizationService authorization,
    IAuthorizerTokenProvider tokenProvider,
    IAuthorizerTokenStore tokenStore,
    OpenPlatformLedger ledger,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(body?.AuthCode))
    {
        return Results.BadRequest(new { error = "缺少 authCode：来自授权回调 URI 的 auth_code 参数（一次性、10 分钟有效）" });
    }

    return await ExchangeAuthCodeAsync(
        body!.AuthCode, authorization, tokenProvider, tokenStore, ledger, cancellationToken)
        .ConfigureAwait(false);
});

// ⑦ 授权码换令牌（自动取推送里的那条）：让整条链在一条命令链里走完，且授权码全程不回显、不落日志。
app.MapPost("/api/auth/exchange-latest", async (
    IComponentAuthorizationService authorization,
    IAuthorizerTokenProvider tokenProvider,
    IAuthorizerTokenStore tokenStore,
    OpenPlatformLedger ledger,
    CancellationToken cancellationToken) =>
{
    if (!ledger.TryTakePendingAuthCode(out var authorizerAppId, out var authCode, out var receivedAt))
    {
        return Results.NotFound(new
        {
            error = "台账里没有未消费的授权码",
            hint = "先完成一次用户授权（authorized / updateauthorized 事件会带 authorization_code），"
                + "或改用 POST /api/auth/query 手工传入；授权码一次性，重复消费必得 40029。",
        });
    }

    return await ExchangeAuthCodeAsync(
        authCode!, authorization, tokenProvider, tokenStore, ledger, cancellationToken)
        .ConfigureAwait(false);
});

// ⑧ 推送处置留痕：证明「合法 200 success / 篡改签名 fail / 超时 fail」三态各归各位。
app.MapGet("/api/push-events", (OpenPlatformLedger ledger) => Results.Json(new
{
    count = ledger.RecentPushes().Count,
    events = ledger.RecentPushes(),
    pendingAuthCodeAvailable = ledger.HasPendingAuthCode,
    hint = "pendingAuthCodeAvailable 只说「有没有」，授权码原文既不回显也不落日志。",
}));

// ⑨ 本地换取台账（与官方列表对照用）。
app.MapGet("/api/authorizations", (OpenPlatformLedger ledger) => Results.Json(new
{
    count = ledger.RecentAuthorizations().Count,
    authorizations = ledger.RecentAuthorizations(),
}));

// ⑩ 官方授权方列表（分页读取）。响应里带 refresh_token —— 属长期凭据，**只回 appid 与授权时刻**。
app.MapGet("/api/authorizers", async (
    int? offset,
    int? count,
    IWechatOpenPlatformComponentService component,
    OpenPlatformAppConfig config,
    CancellationToken cancellationToken) =>
{
    var response = await component.GetAuthorizerListAsync(new OpenPlatformGetAuthorizerListRequest
    {
        ComponentAppId = config.ComponentAppId,
        Offset = offset ?? 0,
        Count = Math.Clamp(count ?? 10, 1, 500),
    }, cancellationToken).ConfigureAwait(false);

    // 声明式客户端不替调用方判错：HTTP 200 且 errcode 非零是官方形态。
    if (!response.IsSuccess)
    {
        return Results.Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage },
            statusCode: StatusCodes.Status502BadGateway);
    }

    return Results.Json(new
    {
        totalCount = response.TotalCount,
        authorizers = (response.List ?? Array.Empty<OpenPlatformAuthorizerListItem>())
            .Select(static item => new { authorizerAppId = item.AuthorizerAppId, authTime = item.AuthTime }),
        hint = "官方应答含 authorizer_refresh_token（长期凭据）⇒ 本端点刻意丢弃该字段，只回 appid 与授权时刻。",
    });
});

// ⑪ 授权方账号信息：代授权方读取的公开面（昵称/主体/权限集数量），令牌走平台侧 component_access_token。
app.MapGet("/api/authorizer/{authorizerAppId}/info", async (
    string authorizerAppId,
    IWechatOpenPlatformComponentService component,
    OpenPlatformAppConfig config,
    CancellationToken cancellationToken) =>
{
    var response = await component.GetAuthorizerInfoAsync(new OpenPlatformGetAuthorizerInfoRequest
    {
        ComponentAppId = config.ComponentAppId,
        AuthorizerAppId = authorizerAppId,
    }, cancellationToken).ConfigureAwait(false);

    if (!response.IsSuccess)
    {
        return Results.Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage },
            statusCode: StatusCodes.Status502BadGateway);
    }

    return Results.Json(new
    {
        nickName = response.AuthorizerInfo?.NickName,
        userName = response.AuthorizerInfo?.UserName,
        principalName = response.AuthorizerInfo?.PrincipalName,
        alias = response.AuthorizerInfo?.Alias,
        serviceType = response.AuthorizerInfo?.ServiceTypeInfo?.Name,
        verifyType = response.AuthorizerInfo?.VerifyTypeInfo?.Name,
        funcScopeCount = response.AuthorizationInfo?.FuncInfo?.Length ?? 0,
        hint = "应答的 authorization_info 里带 authorizer_access_token / refresh_token —— 均不回显。",
    });
});

// ⑫ 授权方令牌：由提供者按需刷新（缓存 + 按 appid 分槽的单飞闸都在 SDK 侧）。
app.MapGet("/api/authorizer/{authorizerAppId}/token", async (
    string authorizerAppId,
    IAuthorizerTokenProvider provider,
    CancellationToken cancellationToken) =>
{
    var token = await provider.GetAuthorizerAccessTokenAsync(authorizerAppId, cancellationToken).ConfigureAwait(false);
    return Results.Json(new
    {
        authorizerAppId,
        length = token.Length,
        fingerprint = Mask(token),
        hint = "官方对「获取/刷新接口调用令牌」有每日限额 ⇒ 每次都经本端口取、由 SDK 决定是否真的刷新，"
            + "不要在业务代码里先调刷新接口再取令牌。",
    });
});

// ⑬ 代授权方调用业务端点：演示**授权方作用域**。
//     注册表分流规则：平台键任何作用域都可达，授权方键无作用域直接 fail-fast（静默回退即取错令牌）。
app.MapGet("/api/authorizer/{authorizerAppId}/embedded", async (
    string authorizerAppId,
    IComponentAppContextSwitcher switcher,
    IWechatOpenPlatformEmbeddedService embedded,
    CancellationToken cancellationToken) =>
{
    // using 内才是「平台 + 授权方」两级作用域；生成客户端在**发送时**解析令牌，故调用必须在作用域内 await。
    using (switcher.UseAuthorizerScope(authorizerAppId))
    {
        var response = await embedded.GetOwnEmbeddedListAsync(0, 100, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            return Results.Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage },
                statusCode: StatusCodes.Status502BadGateway);
        }

        return Results.Json(new
        {
            authorizerAppId,
            embeddedFlag = response.EmbeddedFlag,
            listCount = response.EmbeddedList?.Count ?? 0,
            hint = "同一支端点换个作用域就是另一把令牌：出作用域后再调用会 fail-fast，不会静默用平台令牌代调用。",
        });
    }
});

// ⑭ 引导微信开始推送票据：凭证链的冷启动开关。它改变**真实平台侧的推送配置** ⇒ 归写操作闸。
if (DemoWriteGate.Enabled)
{
    app.MapPost("/api/start-push-ticket", async (
        IWechatOpenPlatformComponentTicketFreeService ticketFree,
        OpenPlatformAppConfig config,
        CancellationToken cancellationToken) =>
    {
        // 免令牌端点：凭据（component_secret）经**请求体**提交，官方形态如此，也不进 URL ⇒ 不会落请求日志。
        var response = await ticketFree.StartPushTicketAsync(new OpenPlatformStartPushTicketRequest
        {
            ComponentAppId = config.ComponentAppId,
            ComponentSecret = config.ComponentAppSecret,
        }, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            return Results.Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage },
                statusCode: StatusCodes.Status502BadGateway);
        }

        return Results.Json(new
        {
            accepted = true,
            hint = "调用成功后微信后台开始每 10 分钟推送 component_verify_ticket 到「授权事件接收 URL」；"
                + "URL 未配置或与本机不通时会一直取不到票据（表现为 /api/component-token 持续 502）。",
        });
    });
}

DemoWriteGate.Announce(app.Logger);
app.Run();

/// <summary>授权码换令牌的请求体（授权码走请求体，不走 URL 参数 —— 理由见 ⑥ 的注释）。</summary>
internal sealed class AuthCodeBody
{
    /// <summary>授权回调 URI 上的 <c>auth_code</c>。</summary>
    public string? AuthCode { get; set; }
}
