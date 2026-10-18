// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Ads;
using Mud.Wechat.Ads.Abstractions.Auth;
using Mud.Wechat.Ads.Abstractions.Configuration;
using Mud.Wechat.Ads.Abstractions.Exceptions;
using Mud.Wechat.Ads.Abstractions.Extensions;
using Mud.Wechat.Ads.DataModels.Advertiser;
using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.Reports;
using Mud.Wechat.Ads.Demo;
using Mud.Wechat.Ads.Extensions;
using Mud.Wechat.Demo.Shared;

// 腾讯广告 Marketing API v3.0 Demo（DM-13）：跑通「授权引导 → 授权码换令牌 → 令牌状态 → 业务只读」。
// 读代码前先记住九件事：
//   1) 本线**不是微信的 access_token 体系**：OAuth2 双令牌（access + refresh），且官方要求
//      access_token / timestamp / nonce **成组、每请求现取** ⇒ 不走声明式 [Token]，注入在传输层
//      （AdsAuthorizationHandler）。故这里没有「取令牌」端点，令牌由 SDK 在每次业务调用时自行装配。
//   2) 授权与业务**两台主机**：授权页在 developers.e.qq.com，令牌与业务接口在 api.e.qq.com。
//      SDK 只管后者（AddAdsApp 登记 e.qq.com 后缀白名单）⇒ 授权 URL 由**宿主自行拼装**，不是 SDK 缺面。
//   3) authorization_code 与 refresh_token 都是**一次性**凭据：换码成功即失效；刷新成功即作废旧的两支。
//      因此「刷新」被写闸管住（不可逆动作），而「换码」不受 —— 码本来就只能用一次。
//   4) account_id **不是配置项**：官方把它放在 oauth/token 应答的 authorizer_info 里，随令牌落库。
//      做成配置就允许「配置说 A 账号、令牌其实属于 B 账号」的非法状态 ⇒ 业务请求的 account_id 由调用方显式传。
//   5) 错误信封是 {code, message, message_cn, data}，code 是整数但**语义与微信 errcode 完全不同**：
//      令牌失效码、判定器都不得跨线套用；中文支 message_cn 由本 Demo 的错误闸单独回显。
//   6) 复合 Query 参数（fields / group_by / date_range / filtering / order_by）**只能收 JSON 字面量字符串**：
//      组件对数组走「重复同名参数」、对复杂类型走「逐属性展平」，两种都不是官方形态 ⇒ 一律经 AdsQueryJson 构造。
//   7) 官方 redirect_uri **不支持指定端口号** ⇒ 本机 http://localhost:56568 收不到官方回调，
//      真实授权须把回调域配成已登记的公网地址；本 Demo 的换码入口接受**手工粘贴**的 code（走请求体，不走 URL 参数）。
//   8) state 的 CSRF 校验**不由 SDK 承担**（它只把 code 换令牌）⇒ 本宿主自己发、自己比、自己作废。
//   9) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀）；client_secret / access_token / refresh_token /
//      authorization_code 的值不进日志、不进响应体、不进 URL 查询串。

var env = DemoEnv.RequireAll("ADS_CLIENT_ID", "ADS_CLIENT_SECRET");

var clientId = env["ADS_CLIENT_ID"];
var clientSecret = env["ADS_CLIENT_SECRET"];
var configuredRedirect = DemoEnv.Optional("ADS_REDIRECT_URI") ?? string.Empty;
var appKey = DemoEnv.Optional("ADS_APP_KEY") ?? "default";
var defaultScope = DemoEnv.Optional("ADS_SCOPE") ?? "ADS_MANAGEMENT";
var defaultAccountType = DemoEnv.Optional("ADS_ACCOUNT_TYPE");

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,

    // 内容根钉在程序集目录：门禁脚本会直接执行 bin 下的 dll，默认内容根（当前工作目录）会找不到 wwwroot。
    ContentRootPath = AppContext.BaseDirectory,
});

// ① 授权与传输底座：只装 OAuth 编排 + 两个命名客户端 + 应用注册表，**不**拉业务接口。
//    注册期即构造 AdsAppManager ⇒ 凭据缺失 / AppKey 重复 / 多应用未标默认都钉在启动，而不是第一次真实请求。
builder.Services.AddAdsApp(c =>
{
    c.AppKey = appKey;
    c.ClientId = clientId;
    c.ClientSecret = clientSecret;
    c.RedirectUri = configuredRedirect;
});

// ② 业务接口按模块注册（本线 13 个枚举值；AddAllApis() 会把 48 支端点全登记，Demo 只取两域）。
builder.Services.AddWechatAdsApi(b => b.AddModules(AdsModule.Advertiser, AdsModule.Reports));

var app = builder.Build();

// 通用闸在前、本线闸在后（app.Use 越晚注册越靠内、越先捕获）：顺序写反不报错，
// 只会把「需重新授权」的 401 退化成 502、把 message_cn 吞进一段英文消息里。
app.UseWechatApiErrorGate();
app.UseWechatAdsErrorGate();

app.MapDemoHealth("ads", new
{
    appKey,
    clientId,
    apiBaseUrl = "https://api.e.qq.com（令牌与业务接口）",
    authorizeHost = "https://developers.e.qq.com/oauth/authorize（授权页在另一台主机，由宿主自行拼装）",
    tokenType = "OAuth2 双令牌；官方要求 access_token/timestamp/nonce 成组每请求现取 ⇒ 不走声明式 [Token]",
    modules = new[] { "Advertiser", "Reports" },
    redirectUriConfigured = configuredRedirect.Length > 0 ? configuredRedirect : "(未配置，换码时须显式传参)",
    // 密钥只回显「是否配置」：client_secret 不进响应体，令牌只回长度与指纹。
    clientSecretConfigured = clientSecret.Length > 0,
    writeGate = DemoWriteGate.Enabled ? "已启用（ALLOW_WRITE=1）" : "已关闭",
});

DemoWriteGate.Announce(app.Logger);

// —— 辅助：把令牌/密钥压成「长度 + sha256 前 4 字节」，既能证明「换到/换了新的」，又不外泄原文。
static string Mask(string? value)
{
    if (string.IsNullOrEmpty(value))
    {
        return "<空>";
    }

    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
    return $"{value.Length}位/sha256:{Convert.ToHexString(hash.AsSpan(0, 4)).ToLowerInvariant()}";
}

// —— 辅助：授权状态 → 非敏感摘要（三个端点共用同一形状，便于对比「刷新前后确实换了一对令牌」）。
static object Describe(string appKeyText, AdsAuthorizationState? state)
{
    if (state == null)
    {
        return new { authorized = false, hint = "存储里没有本应用的授权状态：先走 GET /api/authorize-url 引导授权，再 POST /api/auth/exchange。" };
    }

    var now = DateTimeOffset.UtcNow;
    return new
    {
        authorized = true,
        appKey = appKeyText,
        // 令牌原文一律不回显：只给长度 + 指纹，指纹变化即「确实换了新令牌」的证据。
        accessToken = Mask(state.AccessToken),
        refreshToken = Mask(state.RefreshToken),
        accessTokenUsable = state.IsAccessTokenUsable(now, 300),
        refreshTokenUsable = state.IsRefreshTokenUsable(now),
        accountId = state.AccountId,
        accountRoleType = state.AccountRoleType,
        scopeList = state.ScopeList,
        hint = "accountId 来自 oauth/token 应答的 authorizer_info，随令牌落库 ⇒ 业务请求的 account_id 用它，不要去配环境变量。",
    };
}

// ① 授权引导：SDK 不提供授权页 URL 构造（授权页不在 api.e.qq.com，也不在 SDK 的客户端面上），
//    由宿主自行拼装并**自己发 state**。官方参数表（2026-10 核验）：client_id/redirect_uri 必填，
//    state/scope/account_type 选填。
var issuedStates = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

app.MapGet("/api/authorize-url", (string? scope, string? accountType, string? redirectUri) =>
{
    var target = redirectUri ?? (configuredRedirect.Length > 0 ? configuredRedirect : null);
    if (target == null)
    {
        return Results.BadRequest(new
        {
            error = "缺少 redirect_uri",
            hint = $"既没传 ?redirectUri=，也没配 {DemoEnv.FullName("ADS_REDIRECT_URI")}。"
                + "官方 redirect_uri **不支持指定端口号**，必须是应用登记回调主域名下的地址。",
        });
    }

    var state = Guid.NewGuid().ToString("N");
    issuedStates[state] = 1;

    var url = new StringBuilder("https://developers.e.qq.com/oauth/authorize")
        .Append("?client_id=").Append(Uri.EscapeDataString(clientId))
        .Append("&redirect_uri=").Append(Uri.EscapeDataString(target))
        .Append("&state=").Append(state)
        .Append("&scope=").Append(Uri.EscapeDataString(scope ?? defaultScope));

    var accountTypeValue = accountType ?? defaultAccountType;
    if (!string.IsNullOrEmpty(accountTypeValue))
    {
        url.Append("&account_type=").Append(Uri.EscapeDataString(accountTypeValue!));
    }

    return Results.Json(new
    {
        authorizeUrl = url.ToString(),
        state,
        redirectUri = target,
        hint = "浏览器打开 authorizeUrl 完成授权后，官方会把 authorization_code 与 state 回跳到 redirect_uri。"
            + "本机收不到公网回调 ⇒ 从浏览器地址栏复制 code，用 POST /api/auth/exchange 手工换取令牌对。"
            + "state 的 CSRF 校验由本宿主负责（SDK 只换码），故这里同时回显 state 供比对。",
    });
});

// ② 授权码换令牌：code 走**请求体**而非 URL 参数 —— ASP.NET Core 的请求日志会把完整 URL（含查询串）落盘，
//    而 authorization_code 是一次性凭据，进日志等于把它公开。
app.MapPost("/api/auth/exchange", async (
    ExchangeRequest request,
    IAdsAuthorizationService authorization,
    IAdsAppManager apps,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("Mud.Wechat.Ads.Demo");

    if (string.IsNullOrWhiteSpace(request?.Code))
    {
        return Results.BadRequest(new { error = "请求体缺 code 字段（官方 authorization_code，≤64 字节、一次性）" });
    }

    // state 校验：给了就必须对得上，且**用过即作废**（同一 state 二次换码属重放）。
    if (!string.IsNullOrEmpty(request!.State) && !issuedStates.TryRemove(request.State!, out _))
    {
        return Results.Json(new
        {
            error = "state 未登记或已被消费",
            hint = "先走 GET /api/authorize-url 取一个新 state；不传 state 字段则跳过本道校验（演示态，生产必须校验）。",
        }, statusCode: StatusCodes.Status400BadRequest);
    }

    var targetAppKey = string.IsNullOrWhiteSpace(request.AppKey) ? apps.DefaultAppKey ?? appKey : request.AppKey!;

    try
    {
        // 单飞 + 结果记忆：并发换同一个码时只放行一个真实请求，其余复用结果（官方对第二次必拒）。
        var state = await authorization.ExchangeAuthorizationCodeAsync(
            targetAppKey, request.Code, request.RedirectUri, cancellationToken).ConfigureAwait(false);

        // 日志只写指纹：authorization_code 与两支令牌都不得进日志（AGENTS §8）。
        logger.LogInformation(
            "广告线换码成功：AppKey={AppKey}，accountId={AccountId}，access={Access}",
            targetAppKey,
            state.AccountId?.ToString() ?? "-",
            Mask(state.AccessToken));

        return Results.Json(Describe(targetAppKey, state));
    }
    catch (WechatAdsException ex)
    {
        // 换码失败不映射 502：授权码失效是最常见的**用户侧**原因（重放、超时 5 分钟、被别的进程消费过）。
        return Results.Json(new
        {
            errorCode = ex.ErrorCode,
            errorMessage = ex.Message,
            messageCn = ex.MessageCn,
            hint = "换码失败时授权码**未被消费**的可能性存在（官方一次性）⇒ 别按重试成功来设计；"
                + "若 code 已用过，只能回授权页重新拿一个。",
        }, statusCode: StatusCodes.Status502BadGateway);
    }
});

// ③ 令牌状态：整条链的诊断入口（是否已授权、两支令牌各自是否还在有效期、账号 id 是多少）。
app.MapGet("/api/auth/state", async (
    IAdsAuthorizationService authorization,
    IAdsAppManager apps,
    CancellationToken cancellationToken) =>
    Results.Json(Describe(apps.DefaultAppKey ?? appKey, await authorization.GetStateAsync(apps.DefaultAppKey ?? appKey, cancellationToken).ConfigureAwait(false))));

// ④ 主动刷新：包在写闸内 —— 官方 oauth/refresh_token 会**作废旧的 access 与 refresh 两支**，
//    是不可逆动作（且多实例部署下进程锁消除不了跨实例并发刷新），不是「读一下状态」那么轻。
if (DemoWriteGate.Enabled)
{
    app.MapPost("/api/auth/refresh", async (IAdsAppManager apps, IAdsAuthorizationService authorization, CancellationToken cancellationToken) =>
    {
        var targetAppKey = apps.DefaultAppKey ?? appKey;
        var state = await authorization.RefreshAsync(targetAppKey, cancellationToken).ConfigureAwait(false);
        return Results.Json(Describe(targetAppKey, state));
    });
}

// ⑤ 客户（广告主）列表：官方 GET /v3.0/advertiser/get，翻页两种模式互斥。
//    fields 是**必填**的 JSON 数组字面量 ⇒ 经 AdsQueryJson 构造，绝不能传逗号分隔串。
app.MapGet("/api/advertisers", async (
    IWechatAdsAdvertiserService advertisers,
    IAdsAppManager apps,
    long? accountId,
    IAdsAuthorizationService authorization,
    string? pagination,
    long? page,
    long? cursor,
    long? pageSize,
    CancellationToken cancellationToken) =>
{
    var mode = string.Equals(pagination, "cursor", StringComparison.OrdinalIgnoreCase)
        ? "PAGINATION_MODE_CURSOR"
        : "PAGINATION_MODE_NORMAL";

    // account_id 缺省时用令牌里落库的那一个 —— 这正是「account_id 不属于配置面」的落地形态。
    var effectiveAccountId = accountId;
    if (effectiveAccountId == null)
    {
        var authState = await authorization.GetStateAsync(apps.DefaultAppKey ?? appKey, cancellationToken).ConfigureAwait(false);
        effectiveAccountId = authState?.AccountId;
    }

    var size = Math.Clamp(pageSize ?? 10, 1, 100);

    var response = await advertisers.GetAsync(
        effectiveAccountId,
        AdsQueryJson.Fields("account_id", "corporation_name", "system_status", "reasons"),
        mode,
        size,
        page: mode == "PAGINATION_MODE_NORMAL" ? page ?? 1 : null,
        cursor: mode == "PAGINATION_MODE_CURSOR" ? cursor : null,
        cancellationToken: cancellationToken).ConfigureAwait(false);

    if (!response.IsSuccess)
    {
        return Results.Json(new
        {
            errorCode = response.Code,
            errorMessage = response.Message,
            messageCn = response.MessageCn,
            hint = "官方信封 code != 0；本线 code 是整数但语义与微信 errcode 完全不同（失效码集合与判定器不得跨线套用）。",
        });
    }

    return Results.Json(new
    {
        paginationMode = mode,
        accountId = effectiveAccountId,
        count = response.Data?.List?.Count ?? 0,
        list = response.Data?.List,
        pageInfo = response.Data?.PageInfo,
        cursorPageInfo = response.Data?.CursorPageInfo,
        hint = "游标模式：首次不传 cursor，之后回传上一次应答里的 cursor；游标有效期 24 小时，"
            + "且除分页参数外的请求条件须与上一次**完全一致**。has_more=false 时务必停止拉取。",
    });
});

// ⑥ 日报：官方 GET /v3.0/daily_reports/get。应答行的键集**由请求的 fields 决定**
//    （AdsReportListData.List 是 List<Dictionary<string, JsonElement>> 透传形态）⇒ 未请求的指标不会出现在应答里。
app.MapGet("/api/report/daily", async (
    IWechatAdsReportService reports,
    IAdsAppManager apps,
    long? accountId,
    string? beginDate,
    string? endDate,
    string? level,
    IAdsAuthorizationService authorization,
    long? pageSize,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrEmpty(beginDate) || string.IsNullOrEmpty(endDate))
    {
        return Results.BadRequest(new
        {
            error = "缺 beginDate / endDate（官方格式 yyyy-MM-dd）",
            example = "/api/report/daily?beginDate=2026-10-01&endDate=2026-10-07",
        });
    }

    var effectiveAccountId = accountId;
    if (effectiveAccountId == null)
    {
        var authState = await authorization.GetStateAsync(apps.DefaultAppKey ?? appKey, cancellationToken).ConfigureAwait(false);
        effectiveAccountId = authState?.AccountId;
    }

    if (effectiveAccountId == null)
    {
        return Results.BadRequest(new
        {
            error = "缺 account_id，且授权状态里没有 authorizer_info.account_id",
            hint = "官方把 account_id 标为非必填、实际必填（本页四处自相矛盾之一，SDK 不预判）⇒ "
                + "先完成一次授权（令牌落库时带 account_id），或用 ?accountId= 显式给出。",
        });
    }

    var response = await reports.GetDailyAsync(
        effectiveAccountId.Value,
        level ?? "REPORT_LEVEL_ADVERTISER",
        AdsQueryJson.DateRange(new AdsDateRange { StartDate = beginDate!, EndDate = endDate! }),
        AdsQueryJson.Fields("date", "cost", "view_count", "valid_click_count"),
        AdsQueryJson.GroupBy("date"),
        pageSize: Math.Clamp(pageSize ?? 100, 1, 2000),
        cancellationToken: cancellationToken).ConfigureAwait(false);

    if (!response.IsSuccess)
    {
        return Results.Json(new
        {
            errorCode = response.Code,
            errorMessage = response.Message,
            messageCn = response.MessageCn,
            hint = "报表域常见失败：权限不足、level 与 group_by 不配套、page × pageSize 乘积超 20000（须改走异步报表）。",
        });
    }

    return Results.Json(new
    {
        accountId = effectiveAccountId,
        level = level ?? "REPORT_LEVEL_ADVERTISER",
        rowCount = response.Data?.List?.Count ?? 0,
        rows = response.Data?.List,
        pageInfo = response.Data?.PageInfo,
        hint = "行的键集等于请求的 fields（本 Demo 取 date/cost/view_count/valid_click_count）；"
            + "time_line 与 filtering/order_by 未在此暴露，形态同样是 AdsQueryJson 编码的 JSON 字面量。",
    });
});

app.Logger.LogInformation(
    "腾讯广告 Demo 就绪：AppKey={AppKey}，clientId={ClientId}，业务模块 Advertiser + Reports，写闸 {WriteGate}。",
    appKey,
    clientId,
    DemoWriteGate.Enabled ? "已启用" : "已关闭（刷新端点未映射）");

app.Run();

/// <summary>换码请求体（<c>code</c> 与 <c>state</c> 都不进 URL 查询串，避免落进请求日志）。</summary>
internal sealed record ExchangeRequest(string? Code, string? State, string? AppKey, string? RedirectUri);
