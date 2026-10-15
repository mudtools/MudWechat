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
using Mud.Wechat.Pay.Abstractions.Transport;

namespace Mud.Wechat.Pay.Ecommerce;

/// <summary>
/// 平台收付通注销申请资料图片上传实现（multipart/form-data 手工通道）。
/// </summary>
/// <remarks>
/// 参照 <c>WechatPayMerchantMediaService</c> 形态；表单字段名为 <c>file</c> + <c>meta{file_name, file_digest}</c>
/// —— 与商户媒体上传的 <c>meta{filename, sha256}</c> 键名不同，逐字照抄官方参数表。
/// </remarks>
internal sealed class WechatPayEcommerceCancelMediaService : IWechatPayEcommerceCancelMediaService
{
    private const string PrimaryAccessPoint = "https://api.mch.weixin.qq.com";

    private const string UploadPath = "/v3/ecommerce/account/cancel-applications/media";

    private const string FileFieldName = "file";

    private const string MetaFieldName = "meta";

    // 官方：媒体图片只支持 JPG、BMP、PNG，文件大小不能超过 2M。
    private const long MaxImageBytes = 2 * 1024 * 1024;

    private readonly IWechatPayHttpClient _httpClient;

    public WechatPayEcommerceCancelMediaService(IWechatPayHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> UploadCancelProofImageAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("文件名称不能为空", nameof(fileName));
        }

        var contentType = ResolveImageContentType(fileName);

        using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        var fileBytes = memoryStream.ToArray();

        if (fileBytes.Length == 0)
        {
            throw new ArgumentException("图片内容为空", nameof(content));
        }

        if (fileBytes.LongLength > MaxImageBytes)
        {
            throw new ArgumentException(
                $"图片大小 {fileBytes.LongLength} 字节超出官方 2M 上限", nameof(content));
        }

        var metaJson = BuildMetaJson(fileName, ComputeSha256(fileBytes));

        using var form = new MultipartFormDataContent("boundary-" + Guid.NewGuid().ToString("N"));

        var filePart = new ByteArrayContent(fileBytes);
        filePart.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        filePart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{FileFieldName}\"",
            FileName = $"\"{fileName}\""
        };
        form.Add(filePart);

        var metaPart = new StringContent(metaJson, Encoding.UTF8, "application/json");
        metaPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{MetaFieldName}\""
        };
        form.Add(metaPart);

        var requestUri = new Uri(new Uri(PrimaryAccessPoint), UploadPath);
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Content = form;

        using var response = await _httpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);

        var body = response.Content != null
            ? await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
            : string.Empty;

        if (!response.IsSuccessStatusCode)
        {
            var (code, message) = ExtractError(body);
            throw new InvalidOperationException(
                $"上传注销申请资料图片失败：code={code}, message={message}。");
        }

        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.TryGetProperty("media_id", out var mediaIdElement))
        {
            var mediaId = mediaIdElement.GetString();
            if (!string.IsNullOrWhiteSpace(mediaId))
            {
                return mediaId;
            }
        }

        throw new InvalidOperationException("官方应答中未包含 media_id");
    }

    /// <summary>
    /// 按官方后缀白名单（JPG/JPEG/BMP/PNG）取文件部分的 Content-Type —— 官方对非法 Content-Type
    /// 回 400 <c>INVALID_REQUEST</c>，本地先判可避免把不支持的格式传上去。
    /// </summary>
    private static string ResolveImageContentType(string fileName)
    {
        var extension = System.IO.Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentException(
                "图片文件名必须以 JPG、BMP、PNG 为后缀（官方要求）", nameof(fileName));
        }

        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".bmp" => "image/bmp",
            _ => throw new ArgumentException(
                $"图片后缀 {extension} 不在官方白名单（JPG、BMP、PNG）内", nameof(fileName)),
        };
    }

    private static string ComputeSha256(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static string BuildMetaJson(string fileName, string fileDigest)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });

        writer.WriteStartObject();
        writer.WriteString("file_name", fileName);
        writer.WriteString("file_digest", fileDigest);
        writer.WriteEndObject();
        writer.Flush();

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static (string? Code, string? Message) ExtractError(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var code = root.TryGetProperty("code", out var codeEl) ? codeEl.GetString() : null;
            var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() : null;
            return (code, message);
        }
        catch (JsonException)
        {
            return (null, body);
        }
    }
}
