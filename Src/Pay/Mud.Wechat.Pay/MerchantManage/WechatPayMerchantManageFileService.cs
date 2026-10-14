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

namespace Mud.Wechat.Pay.MerchantManage;

/// <summary>
/// 商户管理记录文件上传服务实现（multipart/form-data 手工通道）。
/// </summary>
/// <remarks>
/// 参照 Fapiao 文件上传与 BillDownload 二进制流通道形态；表单字段名为 <c>submission_file</c> + <c>meta</c>。
/// </remarks>
internal sealed class WechatPayMerchantManageFileService : IWechatPayMerchantManageFileService
{
    private const string PrimaryAccessPoint = "https://api.mch.weixin.qq.com";

    private const string UploadPathTemplate = "/v3/mch-manage/submission-files/sub-mchid/{0}/upload";

    private const string FileFieldName = "submission_file";

    private const string MetaFieldName = "meta";

    private readonly IWechatPayHttpClient _httpClient;

    public WechatPayMerchantManageFileService(IWechatPayHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> UploadSubmissionFileAsync(
        string subMchId,
        Stream content,
        string fileName,
        string itemId,
        long? manageRecordId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(subMchId))
        {
            throw new ArgumentException("子商户号不能为空", nameof(subMchId));
        }

        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("文件名不能为空", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("资料项 ID 不能为空", nameof(itemId));
        }

        // 读取文件内容并计算 SHA-256
        using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        var fileBytes = memoryStream.ToArray();

        var sha256Hash = ComputeSha256(fileBytes);

        // 构建 meta JSON
        var metaJson = BuildMetaJson(fileName, sha256Hash, itemId, manageRecordId, subMchId);

        // 构建 multipart/form-data
        using var form = new MultipartFormDataContent("boundary-" + Guid.NewGuid().ToString("N"));

        // meta 部分
        var metaPart = new StringContent(metaJson, Encoding.UTF8, "application/json");
        metaPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{MetaFieldName}\""
        };
        form.Add(metaPart);

        // 文件部分
        var filePart = new ByteArrayContent(fileBytes);
        filePart.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        filePart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{FileFieldName}\"",
            FileName = $"\"{fileName}\""
        };
        form.Add(filePart);

        // 发送请求
        var path = string.Format(UploadPathTemplate, subMchId);
        var requestUri = new Uri(new Uri(PrimaryAccessPoint), path);

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Content = form;

        using var response = await _httpClient.SendRawAsync(request, cancellationToken).ConfigureAwait(false);

        // 解析响应（官方返回 JSON，含 file_id）
        var body = response.Content != null
            ? await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
            : string.Empty;

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"上传管理记录文件失败：HTTP {(int)response.StatusCode}");
        }

        // 提取 file_id（简单解析，假设响应为 {"file_id": "..."}）
        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.TryGetProperty("file_id", out var fileIdElement))
        {
            return fileIdElement.GetString() ?? string.Empty;
        }

        throw new InvalidOperationException("官方应答中未包含 file_id");
    }

    private static string ComputeSha256(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static string BuildMetaJson(string fileName, string fileDigest, string itemId, long? manageRecordId, string subMchId)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });

        writer.WriteStartObject();
        writer.WriteString("file_name", fileName);
        writer.WriteString("file_digest", fileDigest);
        writer.WriteString("item_id", itemId);

        if (manageRecordId.HasValue)
        {
            writer.WriteNumber("manage_record_id", manageRecordId.Value);
        }

        writer.WriteString("sub_mchid", subMchId);
        writer.WriteEndObject();
        writer.Flush();

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
