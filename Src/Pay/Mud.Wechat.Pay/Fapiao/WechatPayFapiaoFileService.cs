// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Mud.Wechat.Pay.Abstractions.Exceptions;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Fapiao;

namespace Mud.Wechat.Pay.Fapiao;

/// <summary>
/// 电子发票文件通道实现（<see cref="IWechatPayFapiaoFileService"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>形态与账单下载通道一致</b>：经支付专用 <see cref="IWechatPayHttpClient"/> 发原始请求，
/// 由传输层自动补 <c>Authorization</c>；非 2xx 时按官方 JSON 错误体取 <c>code</c> / <c>message</c> 抛业务异常。
/// </para>
/// <para>
/// <b>不缓冲大文件</b>：文件流由调用方提供（<see cref="Stream"/>），本类只把它挂到 <c>multipart</c> 的
/// <c>file</c> 部分，<b>不</b>整体读入内存。
/// </para>
/// </remarks>
public sealed class WechatPayFapiaoFileService : IWechatPayFapiaoFileService
{
    /// <summary>主接入点（官方就近接入点）。</summary>
    public const string PrimaryAccessPoint = "https://api.mch.weixin.qq.com";

    /// <summary>上传发票文件的请求路径（官方原文）。</summary>
    public const string UploadPath = "/v3/new-tax-control-fapiao/fapiao-applications/upload-fapiao-file";

    /// <summary>表单 <c>file</c> 部分的字段名（官方原文）。</summary>
    public const string FileFieldName = "file";

    /// <summary>表单 <c>meta</c> 部分的字段名（官方原文）。</summary>
    public const string MetaFieldName = "meta";

    /// <summary>文件类型取值（官方 <c>meta.file_type</c> 该字段表给出的唯一可选值）。</summary>
    public const string PdfFileType = "PDF";

    /// <summary>摘要算法取值（官方 <c>meta.digest_alogrithm</c> 的唯一可选值）。</summary>
    public const string Sm3DigestAlgorithm = "SM3";

    /// <summary>表单 <c>meta</c> 中文件类型字段名（官方原文）。</summary>
    public const string FileTypeFieldName = "file_type";

    /// <summary>
    /// 表单 <c>meta</c> 中摘要算法字段名（官方原文）——<b>官方拼写少一个 r</b>
    /// （正确拼写是 <c>algorithm</c>）。<b>照录</b>：写对了才算合规，擅自「纠正」官方会当作未知字段。
    /// </summary>
    public const string DigestAlgorithmFieldName = "digest_alogrithm";

    /// <summary>表单 <c>meta</c> 中摘要值字段名（官方原文）。</summary>
    public const string DigestFieldName = "digest";

    /// <summary>上传应答中文件 ID 的字段名（官方原文）。</summary>
    public const string MediaIdFieldName = "fapiao_media_id";

    /// <summary>发票文件下载域名（官方《下载发票文件》示例 URL 的主机）——<b>与 API 接入点不同</b>。</summary>
    /// <remarks>
    /// <b>本通道不请求该主机</b>：它不在进程级白名单内（详见
    /// <see cref="IWechatPayFapiaoFileService"/> 的类注释）。本常量用于<b>留档与测试断言</b>，
    /// 让「下载为何不在此实现」成为可校验的事实而非口头说明。
    /// </remarks>
    public const string FileDownloadHost = "pay.wechatpay.cn";

    private readonly IWechatPayHttpClient _httpClient;
    private readonly ILogger<WechatPayFapiaoFileService> _logger;

    /// <summary>创建发票文件通道。</summary>
    /// <param name="httpClient">支付专用 HTTP 客户端（携带传输层签名 Handler）。</param>
    /// <param name="logger">日志器。</param>
    public WechatPayFapiaoFileService(
        IWechatPayHttpClient httpClient,
        ILogger<WechatPayFapiaoFileService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<FapiaoUploadFileResponse> UploadFapiaoFileAsync(
        Stream content,
        string fileName,
        string sm3DigestHex,
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

        // 摘要必须由调用方给出（SM3 不在 BCL 内，见接口 remarks）——空摘要一律拒绝，
        // 否则会发一个官方必然判错的请求，错误还被埋在 multipart 里难以定位。
        if (string.IsNullOrWhiteSpace(sm3DigestHex))
        {
            throw new ArgumentException(
                "SM3 摘要（16 进制）不能为空；SM3 不在 .NET BCL 内，须由调用方计算后传入。",
                nameof(sm3DigestHex));
        }

        var metaJson = BuildMetaJson(sm3DigestHex);

        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
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
                $"微信支付电子发票文件上传失败：code={code}, message={message}。",
                request.RequestUri?.ToString());
        }

        var mediaId = ExtractMediaId(body);

        if (string.IsNullOrWhiteSpace(mediaId))
        {
            // 2xx 但拿不到文件 ID：这是「看起来成功却不可用」的形态，必须显式失败而不是返回空壳。
            throw new WechatPayException(
                null,
                "微信支付电子发票文件上传：官方应答中未包含 fapiao_media_id。",
                request.RequestUri?.ToString());
        }

        // 只记非敏感过程信息：文件内容与摘要不入日志（与 PAY-B7 同口径）。
        _logger.LogDebug("微信支付电子发票文件上传完成（文件 ID 有效期为三天）。");

        return new FapiaoUploadFileResponse { FapiaoMediaId = mediaId };
    }

    /// <summary>
    /// 组装表单元信息 JSON。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何手写而不走源生成上下文</b>：本仓生成的 <c>*JsonContext</c> 统一包在
    /// <c>#if NET8_0_OR_GREATER</c> 内，而本包最低 TFM 是 <b>net6.0</b> ⇒ 直接引用会在 net6.0 分支
    /// <b>编译失败（CS0103，本轮实测踩到）</b>。回调包当初正是因此手写了一份上下文。
    /// 本处只有 <b>3</b> 个字段，故改用 <see cref="Utf8JsonWriter"/>（<b>零反射</b>、全 TFM 可用、
    /// AOT 安全），比再维护一份手写上下文更小更直白。
    /// </para>
    /// <para><b>不得</b>改用反射版 <c>JsonSerializer.Serialize(object)</c>：那会引入 IL2026/IL3050，AOT 门禁必红。</para>
    /// </remarks>
    private static string BuildMetaJson(string sm3DigestHex)
    {
        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            writer.WriteString(FileTypeFieldName, PdfFileType);
            writer.WriteString(DigestAlgorithmFieldName, Sm3DigestAlgorithm);
            writer.WriteString(DigestFieldName, sm3DigestHex);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    /// <summary>从上传应答中取 <c>fapiao_media_id</c>（解析失败返回 <c>null</c>，不抛出）。</summary>
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
                   && doc.RootElement.TryGetProperty(MediaIdFieldName, out var element)
                   && element.ValueKind == JsonValueKind.String
                ? element.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>从官方错误体提取 <c>code</c> / <c>message</c>（解析失败时返回 null，不抛出）。</summary>
    /// <remarks>走 <see cref="JsonDocument"/>（结构化只读解析，非反射反序列化，AOT 安全）。</remarks>
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
}
