// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.OfficialAccount.Abstractions.Enums;
using Mud.Wechat.OfficialAccount.Abstractions.Exceptions;
using Mud.Wechat.OfficialAccount.DataModels.Media;

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 素材下载通道实现（<see cref="IMpMediaDownloadService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 走 <see cref="IBaseHttpClient.SendRawAsync"/> 原始响应直读（I3 裁决的独立请求形态），
/// 按 Content-Type 分支：JSON+errcode → 判错 / 令牌失效自愈重试；JSON 成功体 → 按端点解释
/// （临时素材 video_url / 永久素材 news_item·down_url）；其余 → 文件流。
/// JSON 成功体的结构化解释一律经 <c>MediaJsonContext</c> 源生成反序列化（零手写解析、AOT 净零）。
/// </para>
/// <para>
/// <b>令牌失效自愈</b>：镜像票据管理器先例（<c>MpTicketManagerBase</c> 的 40001 处置）——
/// errcode 命中 <c>{40001, 40014, 42001}</c> 时失效本应用 access_token 后<b>重试一次</b>，
/// 重试仍失败按业务异常上抛（避免对确定失效的场景空转）。
/// </para>
/// </remarks>
public sealed class MpMediaDownloadService : IMpMediaDownloadService
{
    /// <summary>官方「获取临时素材」路由（GET）。</summary>
    public const string TemporaryMediaPath = "/cgi-bin/media/get";

    /// <summary>官方「获取高清语音素材」路由（GET）。</summary>
    public const string JssdkVoicePath = "/cgi-bin/media/get/jssdk";

    /// <summary>官方「获取永久素材」路由（POST；响应按素材类型三形态分流）。</summary>
    public const string PermanentMaterialPath = "/cgi-bin/material/get_material";

    private readonly IAppContextHolder _appContextHolder;
    private readonly IHttpContentSerializer _contentSerializer;
    private readonly ILogger<MpMediaDownloadService> _logger;

    /// <summary>创建素材下载服务。</summary>
    /// <param name="appContextHolder">应用上下文持有器（与声明式客户端同源）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="contentSerializer">HTTP 内容序列化器（可选，未注入时使用组件默认——与生成客户端同源）。</param>
    public MpMediaDownloadService(
        IAppContextHolder appContextHolder,
        ILogger<MpMediaDownloadService> logger,
        IHttpContentSerializer? contentSerializer = null)
    {
        _appContextHolder = appContextHolder ?? throw new ArgumentNullException(nameof(appContextHolder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _contentSerializer = contentSerializer ?? HttpContentSerializerFactory.CreateDefault();
    }

    /// <inheritdoc />
    public Task<MpMediaDownloadResult> DownloadTemporaryMediaAsync(
        string mediaId,
        CancellationToken cancellationToken = default)
        => SendAndInterpretAsync(
            TemporaryMediaPath,
            mediaId,
            InterpretTemporaryMediaAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<MpMediaDownloadResult> DownloadJssdkVoiceAsync(
        string mediaId,
        CancellationToken cancellationToken = default)
        => SendAndInterpretAsync(
            JssdkVoicePath,
            mediaId,
            InterpretTemporaryMediaAsync,
            cancellationToken);

    /// <inheritdoc />
    public Task<MpPermanentMaterialResult> GetPermanentMaterialAsync(
        string mediaId,
        CancellationToken cancellationToken = default)
        => SendAndInterpretAsync(
            PermanentMaterialPath,
            mediaId,
            InterpretPermanentMaterialAsync,
            cancellationToken);

    /// <summary>
    /// 共享发送核心：取令牌 → 发送（GET 直连 / POST + <c>{"media_id"}</c> 体）→ 分类 →
    /// （令牌失效时）失效重试一次 → 业务判错；JSON 成功体与文件流的解释交给端点专属委托。
    /// </summary>
    private async Task<TResult> SendAndInterpretAsync<TResult>(
        string path,
        string mediaId,
        Func<HttpResponseMessage, DownloadOutcome, CancellationToken, Task<TResult>> interpret,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
        {
            throw new ArgumentException("mediaId 不能为空。", nameof(mediaId));
        }

        var app = ResolveAppContext();
        var method = path == PermanentMaterialPath ? HttpMethod.Post : HttpMethod.Get;

        for (var attempt = 1; ; attempt++)
        {
            var token = await app.AccessTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            var requestUri = BuildRequestUri(path, token, mediaId);

            using var request = new HttpRequestMessage(method, requestUri);
            if (method == HttpMethod.Post)
            {
                // 官方契约：请求体 {"media_id":…}（经组件序列化器——与生成客户端同一通路，net8+ 由
                // 消费方 JsonTypeInfoResolver 保证 AOT 安全）。
                request.Content = _contentSerializer.ToHttpContent(new MpMediaIdRequest { MediaId = mediaId });
            }

            var response = await app.HttpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);
            var outcome = await ClassifyAsync(response, cancellationToken).ConfigureAwait(false);

            // 令牌失效码：失效缓存 → 强制刷新 → 重试一次（镜像 MpTicketManagerBase 的 40001 自愈先例）。
            if (outcome.IsTokenInvalid && attempt == 1)
            {
                _logger.LogWarning(
                    "素材请求收到令牌失效码 {Errcode}，失效本应用令牌后重试一次。AppKey: {AppKey}, Path: {Path}",
                    outcome.Errcode, app.AppKey, path);
                response.Dispose();
                await InvalidateAccessTokenAsync(app, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (outcome.Errcode is { } errcode && errcode != MpErrorCodes.Success)
            {
                response.Dispose();
                throw new MpException((int)errcode, outcome.Errmsg ?? "微信公众号素材请求失败。", requestUri);
            }

            return await interpret(response, outcome, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>临时素材双端点的 JSON / 文件流解释（video_url 形态为视频素材独有）。</summary>
    private static async Task<MpMediaDownloadResult> InterpretTemporaryMediaAsync(
        HttpResponseMessage response,
        DownloadOutcome outcome,
        CancellationToken cancellationToken)
    {
        if (!outcome.IsJson)
        {
            return await ToStreamResultAsync(response, outcome).ConfigureAwait(false);
        }

        response.Dispose();
        if (outcome.VideoUrl is { Length: > 0 } videoUrl)
        {
            return new MpMediaDownloadResult(outcome.ContentType, videoUrl);
        }

        // JSON 成功体但无 video_url：官方未定义形态，防御性拒绝（不把错误体当流交出）。
        throw new InvalidOperationException(
            $"微信公众号素材下载返回了未定义的 JSON 形态（无 errcode / video_url）。" +
            $"Body 前缀: {TruncateBody(outcome.JsonBody)}");
    }

    /// <summary>永久素材端点的 JSON / 文件流解释（图文 / 视频 / 文件流三形态）。</summary>
    private async Task<MpPermanentMaterialResult> InterpretPermanentMaterialAsync(
        HttpResponseMessage response,
        DownloadOutcome outcome,
        CancellationToken cancellationToken)
    {
        if (!outcome.IsJson)
        {
            return await ToPermanentStreamResultAsync(response, outcome).ConfigureAwait(false);
        }

        response.Dispose();

        MpPermanentMaterialResponse? payload;
        try
        {
            payload = _contentSerializer.Deserialize<MpPermanentMaterialResponse>(outcome.JsonBody ?? string.Empty, null);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"微信公众号永久素材返回了无法解析的 JSON 形态。Body 前缀: {TruncateBody(outcome.JsonBody)}",
                exception);
        }

        if (payload?.NewsItems is { Count: > 0 } newsItems)
        {
            return new MpPermanentMaterialResult(newsItems);
        }

        if (payload?.DownUrl is { Length: > 0 } downUrl)
        {
            return new MpPermanentMaterialResult(
                outcome.ContentType, payload.Title, payload.Description, downUrl);
        }

        // JSON 成功体但无 news_item / down_url：官方未定义形态，防御性拒绝。
        throw new InvalidOperationException(
            $"微信公众号永久素材返回了未定义的 JSON 形态（无 news_item / down_url）。" +
            $"Body 前缀: {TruncateBody(outcome.JsonBody)}");
    }

    /// <summary>临时素材端点的文件流解释。</summary>
    private static async Task<MpMediaDownloadResult> ToStreamResultAsync(
        HttpResponseMessage response,
        DownloadOutcome outcome)
    {
        var stream = response.Content != null
            ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
            : Stream.Null;
        return new MpMediaDownloadResult(
            outcome.ContentType,
            ExtractFileName(response),
            stream,
            response);
    }

    /// <summary>永久素材端点的文件流解释。</summary>
    private static async Task<MpPermanentMaterialResult> ToPermanentStreamResultAsync(
        HttpResponseMessage response,
        DownloadOutcome outcome)
    {
        var stream = response.Content != null
            ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
            : Stream.Null;
        return new MpPermanentMaterialResult(
            outcome.ContentType,
            ExtractFileName(response),
            stream,
            response);
    }

    /// <summary>诊断用 JSON 体截断（错误消息里只留前缀；官方错误体不含敏感凭据）。</summary>
    private static string TruncateBody(string? body)
    {
        if (string.IsNullOrEmpty(body))
        {
            return "<empty>";
        }

        return body.Length <= 120 ? body : body.Substring(0, 120) + "…";
    }

    private IMpAppContext ResolveAppContext()
        => _appContextHolder.Current as IMpAppContext
           ?? throw new InvalidOperationException(
               "无法找到当前公众号应用上下文（IMpAppContext）。" +
               "请先进入应用作用域（SwitchTo / UseAppScope）或配置默认应用。");

    private static string BuildRequestUri(string path, string token, string mediaId)
        => $"{path}?access_token={Uri.EscapeDataString(token)}&media_id={Uri.EscapeDataString(mediaId)}";

    /// <summary>失效本应用 access_token（按契约探测组件的失效能力，与票据管理器同形）。</summary>
    private static async Task InvalidateAccessTokenAsync(IMpAppContext app, CancellationToken cancellationToken)
    {
        if (app.AccessTokenManager is TokenManagerBase manager)
        {
            await manager.InvalidateTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>从 Content-Disposition 提取官方携带的文件名（未携带时不推断；剥除 quoted-string 包裹引号）。</summary>
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

    /// <summary>分类一次响应（文件流 / JSON 成功体 / 业务错误 / 令牌失效）。</summary>
    private static async Task<DownloadOutcome> ClassifyAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var contentType = response.Content?.Headers.ContentType?.MediaType;
        // ns2.0 无 Contains(string, StringComparison) 重载 ⇒ 用 IndexOf。
        var isJson = contentType != null
                     && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isJson)
        {
            return new DownloadOutcome(contentType);
        }

        var body = response.Content != null
            ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            : string.Empty;

        long? errcode = null;
        string? errmsg = null;
        string? videoUrl = null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.TryGetProperty("errcode", out var errcodeElement) && errcodeElement.TryGetInt64(out var code))
            {
                errcode = code;
            }

            if (root.TryGetProperty("errmsg", out var errmsgElement) && errmsgElement.ValueKind == JsonValueKind.String)
            {
                errmsg = errmsgElement.GetString();
            }

            if (root.TryGetProperty("video_url", out var videoUrlElement) && videoUrlElement.ValueKind == JsonValueKind.String)
            {
                videoUrl = videoUrlElement.GetString();
            }
        }
        catch (JsonException)
        {
            // 声明为 JSON 但解析失败（如网关错误页）：不参与判错，按「未定义 JSON 形态」防御处理。
        }

        var isTokenInvalid = errcode is MpErrorCodes.InvalidCredential
                             or MpErrorCodes.InvalidAccessToken
                             or MpErrorCodes.ExpiredAccessToken;
        return new DownloadOutcome(contentType, errcode, errmsg, videoUrl, isTokenInvalid)
        {
            JsonBody = body,
        };
    }

    /// <summary>
    /// 单次响应的分类结果（文件流 / JSON 成功体 / 业务错误 / 令牌失效四分支的统一载体；
    /// ns2.0 禁 record/init ⇒ 普通类）。
    /// </summary>
    private sealed class DownloadOutcome
    {
        /// <summary>创建文件流形态分类结果（无 JSON 体）。</summary>
        internal DownloadOutcome(string? contentType)
        {
            ContentType = contentType;
        }

        /// <summary>创建 JSON 形态分类结果。</summary>
        internal DownloadOutcome(
            string? contentType,
            long? errcode,
            string? errmsg,
            string? videoUrl,
            bool isTokenInvalid)
        {
            ContentType = contentType;
            Errcode = errcode;
            Errmsg = errmsg;
            VideoUrl = videoUrl;
            IsTokenInvalid = isTokenInvalid;
            IsJson = true;
        }

        /// <summary>响应体媒体类型。</summary>
        internal string? ContentType { get; }

        /// <summary>JSON 体中的 errcode（无该字段或非 JSON 体为 <c>null</c>）。</summary>
        internal long? Errcode { get; }

        /// <summary>JSON 体中的 errmsg。</summary>
        internal string? Errmsg { get; }

        /// <summary>JSON 体中的 video_url（临时视频素材形态）。</summary>
        internal string? VideoUrl { get; }

        /// <summary>errcode 是否命中令牌失效码集合（{40001, 40014, 42001}）。</summary>
        internal bool IsTokenInvalid { get; }

        /// <summary>响应是否为 JSON 体。</summary>
        internal bool IsJson { get; }

        /// <summary>JSON 体原文（结构化解释用；仅 JSON 形态非 null）。</summary>
        internal string? JsonBody { get; set; }
    }
}
