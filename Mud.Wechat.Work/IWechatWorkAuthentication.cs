using Mud.Wechat.Work.DataModels.Authentication;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信认证授权相关的API
/// <para>接口详细文档请参见：<see href="https://developer.work.weixin.qq.com/document/path/97201"/></para>
/// </summary>
[HttpClientApi(RegistryGroupName = "Authentication")]
public interface IWechatWorkAuthentication
{
    /// <summary>
    /// 获取服务商凭证
    /// </summary>
    /// <param name="request">获取服务商凭证请求体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取服务商凭证响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91200"/></remarks>
    [Post("/cgi-bin/service/get_provider_token")]
    Task<GetProviderTokenResponse> GetProviderTokenAsync([Body] GetProviderTokenRequest request, CancellationToken cancellationToken = default);


    /// <summary>
    /// 获取第三方应用凭证
    /// </summary>
    /// <param name="request">获取第三方应用凭证请求体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取第三方应用凭证响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90600"/></remarks>
    [Post("/cgi-bin/service/get_suite_token")]
    Task<GetSuiteTokenResponse> GetSuiteTokenAsync([Body] GetSuiteTokenRequest request, CancellationToken cancellationToken = default);

}
