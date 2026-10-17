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
using Mud.Wechat.Ads.DataModels.CustomAudiences;

namespace Mud.Wechat.Ads.CustomAudiences;

/// <summary>
/// 人群数据文件上传通道实现（<c>POST /v3.0/custom_audience_files/add</c>，<c>multipart/form-data</c>）。
/// </summary>
/// <remarks>
/// 形态照 <c>AdsMaterialUploadService</c>：手写 <see cref="MultipartFormDataContent"/> +
/// 组件 <see cref="Mud.HttpUtils.Clients.IEnhancedHttpClient.SendAsync{TResponse}"/>，注入的
/// <see cref="IAdsHttpClient"/> 自带令牌 Handler（成组注入语义与声明式面一致，守卫 ADS-B1）。
/// 文件流由调用方提供并挂到 <c>file</c> 部分，本类不整体读入内存、也不关闭调用方的流。
/// </remarks>
public sealed class AdsCustomAudienceFileUploadService : IWechatAdsCustomAudienceFileUploadService
{
    private const string FilesAddPath = "/v3.0/custom_audience_files/add";

    private readonly IAdsHttpClient _httpClient;

    /// <summary>创建人群数据文件上传通道。</summary>
    /// <param name="httpClient">广告业务客户端（带令牌 Handler）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> 为 <c>null</c>。</exception>
    public AdsCustomAudienceFileUploadService(IAdsHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<AdsCustomAudienceFileUploadResponse> UploadAsync(
        long accountId,
        long audienceId,
        string userIdType,
        Stream file,
        string fileName,
        string? operationType = null,
        string? openAppId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userIdType))
        {
            throw new ArgumentException("号码包用户 id 类型不能为空。", nameof(userIdType));
        }

        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("文件名不能为空。", nameof(fileName));
        }

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(accountId.ToString(System.Globalization.CultureInfo.InvariantCulture), Encoding.UTF8), "account_id");
        form.Add(new StringContent(audienceId.ToString(System.Globalization.CultureInfo.InvariantCulture), Encoding.UTF8), "audience_id");
        form.Add(new StringContent(userIdType, Encoding.UTF8), "user_id_type");
        if (!string.IsNullOrEmpty(operationType))
        {
            form.Add(new StringContent(operationType, Encoding.UTF8), "operation_type");
        }

        if (!string.IsNullOrEmpty(openAppId))
        {
            form.Add(new StringContent(openAppId, Encoding.UTF8), "open_app_id");
        }

        var fileContent = new StreamContent(file);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "file", fileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, FilesAddPath) { Content = form };
        var response = await _httpClient.SendAsync<AdsCustomAudienceFileUploadResponse>(request, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        WechatAdsException.ThrowIfFailed(response, FilesAddPath);
        return response!;
    }
}
