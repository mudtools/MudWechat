// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.MiniProgram.Abstractions;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Enums;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 客服临时素材通道实现（<see cref="IWxaKfMediaService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 走 <c>SendRawAsync</c> 原始响应直读，按 Content-Type 分支：JSON → 判错（并做令牌失效自愈重试一次，
/// 无 <c>errcode</c> 则视为<b>视频素材的 <c>video_url</c> 应答</b>原样交回）；其余 → 素材二进制流。
/// 令牌手工注入（官方契约 Query <c>access_token</c>），故本类不进 Query 白名单。
/// </para>
/// <para>
/// <b>与 <see cref="WxaFeedbackMediaService"/> 的差异</b>：本端点官方为 <b>GET</b> 且<b>无请求体</b>
/// （<c>media_id</c> 走 Query），故无需请求体序列化 / AOT JsonTypeInfo 快车道。
/// </para>
/// </remarks>
public sealed class WxaKfMediaService : IWxaKfMediaService
{
    /// <summary>获取临时素材路由（公众号线无此端点，MP-X1 断言）。</summary>
    public const string MediaPath = "/cgi-bin/media/get";

    private readonly IAppContextHolder _appContextHolder;
    private readonly IMpAppManager? _appManager;
    private readonly ILogger<WxaKfMediaService> _logger;

    /// <summary>创建客服临时素材通道服务。</summary>
    /// <param name="appContextHolder">应用上下文持有器（与声明式客户端同源）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="appManager">应用注册表（可选；用于「无环境上下文时回退默认应用」，与生成客户端同一口径）。</param>
    public WxaKfMediaService(
        IAppContextHolder appContextHolder,
        ILogger<WxaKfMediaService> logger,
        IMpAppManager? appManager = null)
    {
        _appContextHolder = appContextHolder ?? throw new ArgumentNullException(nameof(appContextHolder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appManager = appManager;
    }

    /// <inheritdoc />
    public Task<WxaCodeResult> GetMediaAsync(string mediaId, CancellationToken cancellationToken = default)
        => SendAsync(mediaId, cancellationToken);

    /// <summary>共享发送核心：取令牌 → GET 发送 → 分类 →（令牌失效时）失效重试一次 → 业务判错 / 交回素材流。</summary>
    private async Task<WxaCodeResult> SendAsync(string mediaId, CancellationToken cancellationToken)
    {
        if (mediaId is null)
        {
            throw new ArgumentNullException(nameof(mediaId));
        }

        if (mediaId.Length == 0)
        {
            throw new ArgumentException("媒体文件 ID（media_id）不能为空。", nameof(mediaId));
        }

        var app = ResolveAppContext();

        for (var attempt = 1; ; attempt++)
        {
            var token = await app.AccessTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"{MediaPath}?access_token={Uri.EscapeDataString(token)}&media_id={Uri.EscapeDataString(mediaId)}";
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await app.HttpClient.SendRawAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var outcome = await ClassifyAsync(response, cancellationToken).ConfigureAwait(false);

            if (outcome.IsTokenInvalid && attempt == 1)
            {
                // 令牌失效自愈（同 WxaCodeService / WxaFeedbackMediaService）：失效缓存 → 重试一次。
                _logger.LogWarning(
                    "客服临时素材请求收到令牌失效码 {Errcode}，失效本应用令牌后重试一次。AppKey: {AppKey}, Path: {Path}",
                    outcome.Errcode, app.AppKey, MediaPath);
                response.Dispose();
                await InvalidateAccessTokenAsync(app, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (outcome.Errcode is { } errcode && errcode != WxaErrorCodes.Success)
            {
                response.Dispose();
                throw new WxaException((int)errcode, outcome.Errmsg ?? "微信小程序客服临时素材获取失败。", MediaPath);
            }

            var stream = response.Content != null
                ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
                : Stream.Null;

            return new WxaCodeResult(
                response.Content?.Headers.ContentType?.MediaType,
                ExtractFileName(response),
                stream,
                response);
        }
    }

    // 与生成客户端同一口径：环境上下文缺失时回退默认应用（单应用宿主是最常见形态，
    // 只认 ambient 会让「已配置默认应用」的宿主在本通道恒报错，与本方法原先的提示语自相矛盾）。
    private IMpAppContext ResolveAppContext()
        => _appContextHolder.Current as IMpAppContext
           ?? _appManager?.GetDefaultApp()
           ?? throw new InvalidOperationException(
               "无法找到当前小程序（公众号）应用上下文（IMpAppContext）。" +
               "请先进入应用作用域，或经 AddMpApp 配置默认应用。");

    /// <summary>分类一次响应（素材流 / 视频 JSON 应答 / JSON 错误体）。</summary>
    private static async Task<CodeOutcome> ClassifyAsync(
        HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var contentType = response.Content?.Headers.ContentType?.MediaType;
        // ns2.0 无 Contains(string, StringComparison) 重载 ⇒ 用 IndexOf（与公众号线同款）。
        var isJson = contentType != null
                     && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isJson)
        {
            return new CodeOutcome(contentType);
        }

        var body = response.Content != null
            ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            : string.Empty;

        long? errcode = null;
        string? errmsg = null;
        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            if (root.TryGetProperty("errcode", out var errcodeElement) && errcodeElement.TryGetInt64(out var code))
            {
                errcode = code;
            }

            if (root.TryGetProperty("errmsg", out var errmsgElement) && errmsgElement.ValueKind == JsonValueKind.String)
            {
                errmsg = errmsgElement.GetString();
            }
        }
        catch (JsonException)
        {
            // 声明为 JSON 却解析失败（如网关错误页）：不参与判错，按「未知 JSON 形态」处理。
        }

        var isTokenInvalid = errcode is MpErrorCodes.InvalidCredential
                             or MpErrorCodes.InvalidAccessToken
                             or MpErrorCodes.ExpiredAccessToken;

        return new CodeOutcome(contentType, errcode, errmsg, isTokenInvalid);
    }

    /// <summary>失效本应用 access_token（按契约探测组件的失效能力，与公众号线同形）。</summary>
    private static async Task InvalidateAccessTokenAsync(IMpAppContext app, CancellationToken cancellationToken)
    {
        if (app.AccessTokenManager is TokenManagerBase manager)
        {
            await manager.InvalidateTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>从 Content-Disposition 提取文件名（未携带则不推断；剥除引号）。</summary>
    private static string? ExtractFileName(HttpResponseMessage response)
    {
        var disposition = response.Content?.Headers.ContentDisposition;
        if (disposition == null)
        {
            return null;
        }

        var name = disposition.FileNameStar ?? disposition.FileName;
        if (name is { Length: > 1 } quoted
            && quoted.StartsWith("\"", StringComparison.Ordinal)
            && quoted.EndsWith("\"", StringComparison.Ordinal))
        {
            name = quoted.Substring(1, quoted.Length - 2);
        }

        return name;
    }

    /// <summary>单次响应的分类结果（素材流 / 视频 JSON 应答 / JSON 错误体三形态）。</summary>
    private sealed class CodeOutcome
    {
        /// <summary>创建素材流形态结果。</summary>
        internal CodeOutcome(string? contentType) => ContentType = contentType;

        /// <summary>创建 JSON 形态结果（视频 <c>video_url</c> 应答或错误体）。</summary>
        internal CodeOutcome(string? contentType, long? errcode, string? errmsg, bool isTokenInvalid)
        {
            ContentType = contentType;
            Errcode = errcode;
            Errmsg = errmsg;
            IsTokenInvalid = isTokenInvalid;
        }

        /// <summary>响应内容类型。</summary>
        internal string? ContentType { get; }

        /// <summary>JSON 体中的 errcode。</summary>
        internal long? Errcode { get; }

        /// <summary>JSON 体中的 errmsg。</summary>
        internal string? Errmsg { get; }

        /// <summary>是否命中令牌失效码集合。</summary>
        internal bool IsTokenInvalid { get; }
    }
}
