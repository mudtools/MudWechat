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

namespace Mud.Wechat.Pay.Media;

/// <summary>
/// 商户媒体文件上传服务实现（multipart/form-data 手工通道）。
/// </summary>
/// <remarks>
/// 参照 ComplaintMediaService 形态；表单字段名为 <c>file</c> + <c>meta{filename, sha256}</c>。
/// </remarks>
internal sealed class WechatPayMerchantMediaService : IWechatPayMerchantMediaService
{
    private const string PrimaryAccessPoint = "https://api.mch.weixin.qq.com";

    private const string UploadPath = "/v3/merchant/media/upload";

    private const string VideoUploadPath = "/v3/merchant/media/video_upload";

    private const string FileFieldName = "file";

    private const string MetaFieldName = "meta";

    private readonly IWechatPayHttpClient _httpClient;

    public WechatPayMerchantMediaService(IWechatPayHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> UploadMediaAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return await UploadFileInternalAsync(content, fileName, UploadPath, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> UploadVideoAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return await UploadFileInternalAsync(content, fileName, VideoUploadPath, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string> UploadFileInternalAsync(
        Stream content,
        string fileName,
        string path,
        CancellationToken cancellationToken)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("文件名不能为空", nameof(fileName));
        }

        // 读取文件内容并计算 SHA-256
        using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        var fileBytes = memoryStream.ToArray();

        var sha256Hash = ComputeSha256(fileBytes);

        // 构建 meta JSON
        var metaJson = BuildMetaJson(fileName, sha256Hash);

        // 重置流位置
        if (content.CanSeek)
        {
            content.Position = 0;
        }

        // 构建 multipart/form-data
        using var form = new MultipartFormDataContent("boundary-" + Guid.NewGuid().ToString("N"));

        // 文件部分
        var filePart = new StreamContent(content);
        filePart.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        filePart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{FileFieldName}\"",
            FileName = $"\"{fileName}\""
        };
        form.Add(filePart);

        // meta 部分
        var metaContent = new StringContent(metaJson, Encoding.UTF8, "application/json");
        metaContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{MetaFieldName}\""
        };
        form.Add(metaContent);

        // 发送请求
        var requestUri = new Uri(new Uri(PrimaryAccessPoint), path);

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Content = form;

        using var response = await _httpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);

        // 解析响应（官方返回 JSON，含 media_id）
        var body = response.Content != null
            ? await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
            : string.Empty;

        if (!response.IsSuccessStatusCode)
        {
            var (code, message) = ExtractError(body);
            throw new InvalidOperationException(
                $"上传媒体文件失败：code={code}, message={message}。");
        }

        // 提取 media_id
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

    private static string ComputeSha256(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static string BuildMetaJson(string fileName, string sha256)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });

        writer.WriteStartObject();
        writer.WriteString("filename", fileName);
        writer.WriteString("sha256", sha256);
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
        catch
        {
            return (null, body);
        }
    }
}
