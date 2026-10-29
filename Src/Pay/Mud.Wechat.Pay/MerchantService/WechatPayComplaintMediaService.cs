// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mud.Wechat.Pay.Abstractions.Exceptions;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.Download;

namespace Mud.Wechat.Pay.MerchantService;

/// <summary>
/// 消费者投诉图片通道实现（<see cref="IWechatPayComplaintMediaService"/>）。
/// </summary>
/// <remarks>
/// <para><b>形态与既有先例一致</b>：经支付专用 <see cref="IWechatPayHttpClient"/> 发原始请求，
/// 由传输层自动补 <c>Authorization</c>；非 2xx 时按官方 JSON 错误体取 <c>code</c> / <c>message</c> 抛业务异常。</para>
/// <para><b>上传支自行计算 SHA-256</b>（BCL 内置，不同于 Fapiao 的 SM3）并校验 ≤2M 上限。</para>
/// </remarks>
public sealed class WechatPayComplaintMediaService : IWechatPayComplaintMediaService
{
    /// <summary>主接入点（官方就近接入点）。</summary>
    public const string PrimaryAccessPoint = "https://api.mch.weixin.qq.com";

    /// <summary>上传图片的请求路径（官方原文）。</summary>
    public const string UploadPath = "/v3/merchant-service/images/upload";

    /// <summary>获取图片的请求路径模板（官方原文）。</summary>
    public const string GetPathTemplate = "/v3/merchant-service/images/{media_id}";

    /// <summary>表单 <c>file</c> 部分的字段名（官方原文）。</summary>
    public const string FileFieldName = "file";

    /// <summary>表单 <c>meta</c> 部分的字段名（官方原文）。</summary>
    public const string MetaFieldName = "meta";

    /// <summary>文件类型取值集合（官方原文）。</summary>
    public static readonly string[] AllowedFileTypes = { ".jpg", ".bmp", ".png" };

    /// <summary>文件大小上限（官方：不超过 2M）。</summary>
    public const long MaxFileSizeBytes = 2 * 1024 * 1024;

    private readonly IWechatPayHttpClient _httpClient;
    private readonly ILogger<WechatPayComplaintMediaService> _logger;

    /// <summary>创建投诉图片通道。</summary>
    /// <param name="httpClient">支付专用 HTTP 客户端（携带传输层签名 Handler）。</param>
    /// <param name="logger">日志器。</param>
    public WechatPayComplaintMediaService(
        IWechatPayHttpClient httpClient,
        ILogger<WechatPayComplaintMediaService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<MerchantServiceUploadMediaResponse> UploadComplaintImageAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("文件名不能为空。", nameof(fileName));
        }

        // 官方限制：不超过 2M。SDK 做前置闸，避免把超大流推给官方后只得到一个难以定位的 INVALID_REQUEST。
        if (content.CanSeek && content.Length > MaxFileSizeBytes)
        {
            throw new ArgumentException(
                $"图片大小超过官方上限（{MaxFileSizeBytes} 字节 / 2M）。",
                nameof(content));
        }

        var sha256Hex = await ComputeSha256HexAsync(content, cancellationToken).ConfigureAwait(false);
        var metaJson = BuildMetaJson(fileName, sha256Hex);

        // 重置流位置（如果可寻址），确保 multipart 从头读。
        if (content.CanSeek)
        {
            content.Position = 0;
        }

        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // 官方不校验此头，统一设 jpeg
        form.Add(fileContent, FileFieldName, fileName);

        using var metaContent = new StringContent(metaJson, Encoding.UTF8, "application/json");
        form.Add(metaContent, MetaFieldName);

        using var request = new HttpRequestMessage(HttpMethod.Post, PrimaryAccessPoint + UploadPath)
        {
            Content = form,
        };

        using var response = await _httpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);

        var body = response.Content != null
            ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            : string.Empty;

        if (!response.IsSuccessStatusCode)
        {
            var (code, message) = ExtractError(body);
            throw new WechatPayException(
                code,
                $"微信支付投诉图片上传失败：code={code}, message={message}。",
                request.RequestUri?.ToString());
        }

        var mediaId = ExtractMediaId(body);

        if (string.IsNullOrWhiteSpace(mediaId))
        {
            throw new WechatPayException(
                null,
                "微信支付投诉图片上传：官方应答中未包含 media_id。",
                request.RequestUri?.ToString());
        }

        _logger.LogDebug("微信支付投诉图片上传完成（media_id: {MediaId}）。", mediaId);

        return new MerchantServiceUploadMediaResponse { MediaId = mediaId };
    }

    /// <inheritdoc />
    public async Task<WechatPayBillDownloadResult> GetComplaintImageAsync(
        string mediaId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
        {
            throw new ArgumentException("媒体 ID 不能为空。", nameof(mediaId));
        }

        // Replace(string, string, StringComparison) 自 netcoreapp2.1 起才提供：模板占位符为 ASCII，默认重载即序数语义。
        var path = GetPathTemplate.Replace("{media_id}", mediaId);
        var uri = new Uri(PrimaryAccessPoint + path);

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.ParseAdd("*/*"); // 官方要求 Accept: */*

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
                $"微信支付投诉图片获取失败：code={code}, message={message}。",
                request.RequestUri?.ToString());
        }

        if (!response.IsSuccessStatusCode)
        {
            var status = (int)response.StatusCode;
            response.Dispose();
            throw new WechatPayException(
                null,
                $"微信支付投诉图片获取失败：HTTP {status}（非 JSON 错误体，无法取得官方 code）。",
                request.RequestUri?.ToString());
        }

        var stream = response.Content != null
            ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
            : Stream.Null;

        var fileName = ExtractFileNameFromContentDisposition(response);

        return new WechatPayBillDownloadResult(contentType, fileName, stream, response);
    }

    /// <summary>计算流的 SHA-256 摘要（16 进制小写）。</summary>
    private static async Task<string> ComputeSha256HexAsync(Stream content, CancellationToken cancellationToken)
    {
        // 如果流可寻址，先归零再计算（确保从头部开始）。
        if (content.CanSeek)
        {
            content.Position = 0;
        }

#if NETSTANDARD2_0
        // ns2.0 无 HashAlgorithm.ComputeHashAsync：先落到内存流再摘要（流本身已限长）。
        byte[] hash;
        using (var sha256 = SHA256.Create())
        using (var buffer = new MemoryStream())
        {
            await content.CopyToAsync(buffer).ConfigureAwait(false);
            hash = sha256.ComputeHash(buffer.ToArray());
        }
#else
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(content, cancellationToken).ConfigureAwait(false);
#endif

        // 重置流位置，供后续 multipart 读取。
        if (content.CanSeek)
        {
            content.Position = 0;
        }

        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }

    /// <summary>组装表单元信息 JSON。</summary>
    private static string BuildMetaJson(string fileName, string sha256Hex)
    {
        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            writer.WriteString("filename", fileName);
            writer.WriteString("sha256", sha256Hex);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    /// <summary>从上传应答中取 <c>media_id</c>（解析失败返回 <c>null</c>，不抛出）。</summary>
    private static string? ExtractMediaId(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.ValueKind == JsonValueKind.Object
                && doc.RootElement.TryGetProperty("media_id", out var prop)
                && prop.ValueKind == JsonValueKind.String
                    ? prop.GetString()
                    : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>从响应头提取文件名（Content-Disposition）。</summary>
    private static string? ExtractFileNameFromContentDisposition(HttpResponseMessage response)
    {
        var disposition = response.Content?.Headers.ContentDisposition;
        if (disposition != null && !string.IsNullOrWhiteSpace(disposition.FileName))
        {
            return disposition.FileName.Trim('"');
        }

        return null;
    }

    /// <summary>判断 Content-Type 是否为 JSON。</summary>
    private static bool IsJson(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <summary>从官方错误体中提取 <c>code</c> / <c>message</c>。</summary>
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
            var code = root.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.String
                ? c.GetString()
                : null;
            var message = root.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String
                ? m.GetString()
                : null;
            return (code, message);
        }
        catch
        {
            return (null, body.Length > 200 ? body.Substring(0, 200) : body);
        }
    }
}

/// <summary>
/// 投诉图片上传应答（官方 <c>media_id</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467250"/>，2026-10-14 核验。</para>
/// <para><b>应答只有 <c>media_id</c> 一个字段</b>：无 <c>upload_time</c>、无 <c>file_size</c> ⇒
/// 本型是纯壳，不是通用文件元数据载体。</para>
/// </remarks>
public sealed class MerchantServiceUploadMediaResponse
{
    /// <summary>媒体文件标识 ID（<c>media_id</c>，必填 string(512)）：用于回复 / 举证时引用。</summary>
    public string? MediaId { get; set; }
}
