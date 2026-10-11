// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net.Http.Headers;
using System.Text;
using Mud.Wechat.Ads.Abstractions.Exceptions;
using Mud.Wechat.Ads.Abstractions.Transport;
using Mud.Wechat.Ads.DataModels.Images;
using Mud.Wechat.Ads.DataModels.Videos;

namespace Mud.Wechat.Ads.Material;

/// <summary>
/// 素材上传通道实现（图片 <c>images/add</c> + 视频 <c>videos/add</c>，两支 <c>multipart/form-data</c> 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>形态照支付线 <c>WechatPayFapiaoFileService</c></b>：手写 <see cref="MultipartFormDataContent"/> +
/// 组件 <see cref="IEnhancedHttpClient.SendAsync{TResponse}"/>（反序列化走组件管线，上下文已并入
/// <c>AdsJsonResolverExtensions</c>），注入的 <see cref="IAdsHttpClient"/> 自带令牌 Handler
/// —— <c>access_token</c> / <c>timestamp</c> / <c>nonce</c> 的成组注入语义与声明式面完全一致（守卫 ADS-B1）。
/// </para>
/// <para>
/// <b>不缓冲大文件</b>：文件流由调用方提供并挂到 multipart 的文件部分，本类不整体读入内存、
/// 也<b>不</b>关闭调用方的流。<b>敏感面</b>：<c>signature</c> 是内容摘要（非凭据），
/// 异常消息只带端点路径与信封码（守卫 ADS-B5 的凭据面为 <c>access_token</c> 等 Query 参数，经 Handler 注入、
/// 不落本类字符串）。
/// </para>
/// </remarks>
public sealed class AdsMaterialUploadService : IWechatAdsImageUploadService, IWechatAdsVideoUploadService
{
    /// <summary>官方 <c>upload_type</c>：本地文件上传（配合图片文件流）。值照官方枚举原文。</summary>
    public const string UploadTypeFile = "UPLOAD_TYPE_FILE";

    /// <summary>官方 <c>upload_type</c>：base64 字节串上传。值照官方枚举原文。</summary>
    public const string UploadTypeBytes = "UPLOAD_TYPE_BYTES";

    private const string ImagesAddPath = "/v3.0/images/add";
    private const string VideosAddPath = "/v3.0/videos/add";

    private readonly IAdsHttpClient _httpClient;

    /// <summary>创建素材上传通道。</summary>
    /// <param name="httpClient">广告业务客户端（带令牌 Handler）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> 为 <c>null</c>。</exception>
    public AdsMaterialUploadService(IAdsHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<AdsImageUploadResponse> UploadAsync(
        long? accountId,
        string uploadType,
        string signature,
        Stream? file = null,
        string? fileName = null,
        string? bytes = null,
        string? imageUsage = null,
        string? description = null,
        long? organizationId = null,
        long? resizeWidth = null,
        long? resizeHeight = null,
        long? resizeFileSize = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(uploadType))
        {
            throw new ArgumentException("上传方式不能为空。", nameof(uploadType));
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            throw new ArgumentException("图片签名不能为空。", nameof(signature));
        }

        if (uploadType == UploadTypeFile)
        {
            if (file is null)
            {
                throw new InvalidOperationException(
                    $"upload_type={UploadTypeFile} 时必须提供文件流。");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("文件名不能为空。", nameof(fileName));
            }
        }
        else if (uploadType == UploadTypeBytes && string.IsNullOrWhiteSpace(bytes))
        {
            throw new InvalidOperationException(
                $"upload_type={UploadTypeBytes} 时必须提供 base64 字节串。");
        }

        using var form = new MultipartFormDataContent();
        AddText(form, "account_id", accountId);
        AddText(form, "organization_id", organizationId);
        AddText(form, "upload_type", uploadType);
        AddText(form, "signature", signature);
        AddText(form, "bytes", bytes);
        AddText(form, "image_usage", imageUsage);
        AddText(form, "description", description);
        AddText(form, "resize_width", resizeWidth);
        AddText(form, "resize_height", resizeHeight);
        AddText(form, "resize_file_size", resizeFileSize);

        if (file is not null)
        {
            var fileContent = new StreamContent(file);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            form.Add(fileContent, "file", fileName);
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, ImagesAddPath) { Content = form };
        var response = await _httpClient.SendAsync<AdsImageUploadResponse>(request, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        WechatAdsException.ThrowIfFailed(response, ImagesAddPath);
        return response!;
    }

    /// <inheritdoc />
    public async Task<AdsVideoUploadResponse> UploadAsync(
        long? accountId,
        string signature,
        Stream videoFile,
        string fileName,
        string? description = null,
        long? adcreativeTemplateId = null,
        long? organizationId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            throw new ArgumentException("视频签名不能为空。", nameof(signature));
        }

        if (videoFile is null)
        {
            throw new ArgumentNullException(nameof(videoFile));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("文件名不能为空。", nameof(fileName));
        }

        using var form = new MultipartFormDataContent();
        AddText(form, "account_id", accountId);
        AddText(form, "organization_id", organizationId);
        AddText(form, "signature", signature);
        AddText(form, "description", description);
        AddText(form, "adcreative_template_id", adcreativeTemplateId);

        var fileContent = new StreamContent(videoFile);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "video_file", fileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, VideosAddPath) { Content = form };
        var response = await _httpClient.SendAsync<AdsVideoUploadResponse>(request, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        WechatAdsException.ThrowIfFailed(response, VideosAddPath);
        return response!;
    }

    /// <summary>加一个非空文本表单字段（空值一律不上送，对齐官方「选填不送空串」口径）。</summary>
    private static void AddText(MultipartFormDataContent form, string name, long? value)
    {
        if (value is not null)
        {
            AddText(form, name, value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }

    private static void AddText(MultipartFormDataContent form, string name, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        form.Add(new StringContent(value, Encoding.UTF8), name);
    }
}
