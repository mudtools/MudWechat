// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.IO;
using System.Text.Json;
using Mud.Wechat.OpenPlatform.Authentication;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 体验版二维码图片通道实现（<see cref="IOpenPlatformMpTrialQrCodeService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 走 <c>SendRawAsync</c> 原始响应直读，按 Content-Type 分支：JSON → 判错（命中令牌失效码时失效令牌并重试一次）；
/// 其余 → 图片流。<b>令牌手工注入</b>（官方契约 Query <c>access_token</c>，值取当前授权方作用域的
/// <c>authorizer_access_token</c>）——平台自身上下文取授权方令牌时由上下文自身 fail-fast。
/// </para>
/// <para>
/// <b>零反射</b>：错误体只经 <see cref="JsonDocument"/> 取 <c>errcode</c>/<c>errmsg</c> 两键，
/// 不做泛型 <c>Deserialize&lt;T&gt;</c>，故本类在 Native AOT 下无 IL 诊断。
/// </para>
/// </remarks>
public sealed class OpenPlatformMpTrialQrCodeService : IOpenPlatformMpTrialQrCodeService
{
    /// <summary>体验版二维码路由。</summary>
    public const string TrialQrCodePath = "/wxa/get_qrcode";

    /// <summary>官方令牌失效码：invalid credential / access_token is invalid or not latest。</summary>
    internal const int InvalidCredentialErrcode = 40001;

    /// <summary>官方令牌失效码：不合法的 access_token。</summary>
    internal const int InvalidAccessTokenErrcode = 40014;

    private readonly IAppContextHolder _appContextHolder;
    private readonly ILogger<OpenPlatformMpTrialQrCodeService> _logger;

    /// <summary>创建体验版二维码通道服务。</summary>
    /// <param name="appContextHolder">应用上下文持有器（与声明式客户端同源）。</param>
    /// <param name="logger">日志器。</param>
    /// <exception cref="ArgumentNullException">任一必填依赖为 <c>null</c>。</exception>
    public OpenPlatformMpTrialQrCodeService(
        IAppContextHolder appContextHolder,
        ILogger<OpenPlatformMpTrialQrCodeService> logger)
    {
        _appContextHolder = appContextHolder ?? throw new ArgumentNullException(nameof(appContextHolder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<OpenPlatformMpTrialQrCodeResult> GetTrialQrCodeAsync(
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var app = ResolveAppContext();

        // 官方注意事项：path 须「进行一次 urlencode」⇒ 编码由本通道承担，恰好一次。
        var pathQuery = string.IsNullOrWhiteSpace(path)
            ? string.Empty
            : "&path=" + Uri.EscapeDataString(path);

        for (var attempt = 1; ; attempt++)
        {
            var token = await app.GetTokenManager(OpenPlatformTokenTypes.AuthorizerAccessToken)
                .GetTokenAsync(cancellationToken).ConfigureAwait(false);

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{TrialQrCodePath}?access_token={Uri.EscapeDataString(token)}{pathQuery}");

            var response = await app.HttpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);
            var outcome = await ClassifyAsync(response).ConfigureAwait(false);

            if (outcome.IsTokenInvalid && attempt == 1)
            {
                // 令牌失效自愈：失效本授权方令牌 → 重新取用 → 重试一次（仍失败按业务异常上抛）。
                _logger.LogWarning(
                    "获取体验版二维码收到令牌失效码 {Errcode}，失效授权方令牌后重试一次。AppKey: {AppKey}, AuthorizerAppId: {AuthorizerAppId}",
                    outcome.Errcode, app.AppKey, app.AuthorizerAppId);
                response.Dispose();
                await InvalidateAuthorizerTokenAsync(app, cancellationToken).ConfigureAwait(false);
                continue;
            }

            // 非 2xx 或 JSON 错误体都必须判错：否则会把网关错误页当图片流交出（静默错误）。
            if (!response.IsSuccessStatusCode || outcome.Errcode is { } errcode && errcode != 0)
            {
                var message = $"获取体验版二维码失败：HTTP {(int)response.StatusCode}"
                              + (outcome.Errcode is null ? "。" : $", errcode={outcome.Errcode}, errmsg={outcome.Errmsg}。");
                response.Dispose();
                throw new WechatOpenPlatformException(message, outcome.Errcode?.ToString(CultureInfo.InvariantCulture));
            }

            var stream = response.Content != null
                ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
                : Stream.Null;

            return new OpenPlatformMpTrialQrCodeResult(
                response.Content?.Headers.ContentType?.MediaType,
                ExtractFileName(response),
                stream,
                response);
        }
    }

    /// <summary>解析当前开放平台应用上下文（授权方作用域，见类型注释）。</summary>
    private IOpenPlatformAppContext ResolveAppContext()
        => _appContextHolder.Current as IOpenPlatformAppContext
           ?? throw new InvalidOperationException(
               "无法找到开放平台应用上下文（IOpenPlatformAppContext）。" +
               "请先经 IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId) 进入授权方作用域。");

    /// <summary>失效当前授权方令牌（授权方令牌管理器为本程序集内类型，直接探测无需反射）。</summary>
    private static async Task InvalidateAuthorizerTokenAsync(
        IOpenPlatformAppContext app, CancellationToken cancellationToken)
    {
        if (app.GetTokenManager(OpenPlatformTokenTypes.AuthorizerAccessToken) is AuthorizerTokenManager manager)
        {
            await manager.InvalidateTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>分类一次响应（图片流 / JSON 错误体）。</summary>
    private static async Task<QrCodeOutcome> ClassifyAsync(HttpResponseMessage response)
    {
        var contentType = response.Content?.Headers.ContentType?.MediaType;
        var isJson = contentType != null
                     && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isJson)
        {
            return new QrCodeOutcome(contentType);
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
            // 声明为 JSON 却解析失败（如网关错误页）：不参与判错，交由状态码分支拒收（不交出假图片）。
        }

        var isTokenInvalid = errcode is InvalidCredentialErrcode or InvalidAccessTokenErrcode;

        return new QrCodeOutcome(contentType, errcode, errmsg, isTokenInvalid);
    }

    /// <summary>从 Content-Disposition 提取文件名（官方携带 <c>filename="QRCode.jpg"</c>；剥除引号）。</summary>
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

    /// <summary>单次响应的分类结果（图片流 / JSON 错误体两形态）。</summary>
    private sealed class QrCodeOutcome
    {
        /// <summary>创建图片流形态结果。</summary>
        internal QrCodeOutcome(string? contentType) => ContentType = contentType;

        /// <summary>创建 JSON 错误体形态结果。</summary>
        internal QrCodeOutcome(string? contentType, long? errcode, string? errmsg, bool isTokenInvalid)
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
