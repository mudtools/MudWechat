// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Pay.Abstractions.Exceptions;
using Mud.Wechat.Pay.Abstractions.Transport;

namespace Mud.Wechat.Pay.Download;

/// <summary>
/// 账单文件下载通道实现（<see cref="IWechatPayBillDownloadService"/>）。
/// </summary>
/// <remarks>
/// <para>发送 → Content-Type 分流：JSON（错误体）→ 解析 <c>code</c>/<c>message</c> 抛业务异常；
/// 其余 → 文件流结果。非 2xx 且非 JSON 时按 <c>code</c> 缺省消息抛业务异常。</para>
/// <para><b>不消费令牌、不做自愈重试</b>（支付无 <c>access_token</c>，见接口 remarks）。</para>
/// </remarks>
public sealed class WechatPayBillDownloadService : IWechatPayBillDownloadService
{
    /// <summary>官方主接入点域名（SSRF 前置白名单）。</summary>
    public const string PrimaryHost = "api.mch.weixin.qq.com";

    /// <summary>官方备用（异地）接入点域名（SSRF 前置白名单）。</summary>
    public const string SecondaryHost = "api2.mch.weixin.qq.com";

    private readonly IWechatPayHttpClient _httpClient;
    private readonly ILogger<WechatPayBillDownloadService> _logger;

    /// <summary>创建账单下载服务。</summary>
    /// <param name="httpClient">支付专用 HTTP 客户端（携带传输层签名 Handler）。</param>
    /// <param name="logger">日志器。</param>
    public WechatPayBillDownloadService(
        IWechatPayHttpClient httpClient,
        ILogger<WechatPayBillDownloadService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<WechatPayBillDownloadResult> DownloadBillAsync(
        string downloadUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(downloadUrl))
        {
            throw new ArgumentException("账单下载地址不能为空。", nameof(downloadUrl));
        }

        if (!Uri.TryCreate(downloadUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
        {
            throw new ArgumentException($"账单下载地址非法：{downloadUrl}", nameof(downloadUrl));
        }

        // SSRF 前置闸（纵深防御）：download_url 由官方返回，但它是**可被篡改的字符串**
        // （配置 / 中间件 / 日志重放都可能替换），故此处只放行官方两个接入点。
        // 组件连接期的白名单校验是第二道闸，两者不互替。
        if (!IsOfficialHost(uri.Host))
        {
            throw new InvalidOperationException(
                $"账单下载地址主机不在官方白名单内：{uri.Host}。" +
                $"仅允许 {PrimaryHost} / {SecondaryHost}（SSRF 纵深防御）。");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await _httpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);

        var contentType = response.Content?.Headers.ContentType?.MediaType;
        if (IsJson(contentType))
        {
            var body = response.Content != null
                ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                : string.Empty;
            response.Dispose();

            var (code, message) = ExtractError(body);
            throw new WechatPayException(
                code,
                $"微信支付账单下载失败：code={code}, message={message}。",
                request.RequestUri?.ToString());
        }

        if (!response.IsSuccessStatusCode)
        {
            var status = (int)response.StatusCode;
            response.Dispose();
            throw new WechatPayException(
                null,
                $"微信支付账单下载失败：HTTP {status}（非 JSON 错误体，无法取得官方 code）。",
                request.RequestUri?.ToString());
        }

        var stream = response.Content != null
            ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
            : Stream.Null;

        _logger.LogDebug("微信支付账单下载开始，ContentType: {ContentType}", contentType);

        return new WechatPayBillDownloadResult(contentType, ExtractFileName(response), stream, response);
    }

    /// <summary>主机是否为官方接入点（精确匹配，不做子域后缀放宽）。</summary>
    private static bool IsOfficialHost(string host)
        => string.Equals(host, PrimaryHost, StringComparison.OrdinalIgnoreCase)
           || string.Equals(host, SecondaryHost, StringComparison.OrdinalIgnoreCase);

    /// <summary>内容类型是否 JSON（ns2.0 无 <c>Contains(string, StringComparison)</c> 重载 ⇒ 用 IndexOf）。</summary>
    private static bool IsJson(string? contentType)
        => contentType != null && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;

    /// <summary>从官方错误体提取 <c>code</c> / <c>message</c>（解析失败时返回 null，不抛出）。</summary>
    /// <remarks>
    /// 走 <see cref="JsonDocument"/>（结构化只读解析，<b>非反射反序列化</b>，AOT 安全）。
    /// </remarks>
    private static (string? Code, string? Message) ExtractError(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return (null, null);
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            string? code = root.TryGetProperty("code", out var codeElement)
                           && codeElement.ValueKind == JsonValueKind.String
                ? codeElement.GetString()
                : null;

            string? message = root.TryGetProperty("message", out var messageElement)
                              && messageElement.ValueKind == JsonValueKind.String
                ? messageElement.GetString()
                : null;

            return (code, message);
        }
        catch (JsonException)
        {
            // 声明为 JSON 但解析失败（如网关错误页）：不参与分类，交由调用方按「无 code」处理。
            return (null, null);
        }
    }

    /// <summary>从 <c>Content-Disposition</c> 提取文件名（未携带时不推断；剥除 quoted-string 引号）。</summary>
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
}
