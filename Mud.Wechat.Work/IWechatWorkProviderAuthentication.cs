using Mud.Wechat.Work.DataModels.ProviderAuthentication;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信服务商认证授权相关的API
/// <para>接口详细文档请参见：<see href="https://developer.work.weixin.qq.com/document/path/97201"/></para>
/// </summary>
[HttpClientApi(RegistryGroupName = "Authentication")]
public interface IWechatWorkProviderAuthentication
{
    /// <summary>
    /// 获取服务商凭证，用于后续接口的调用（注意：不能频繁调用get_provider_token接口，否则会受到频率拦截）。当provider_access_token失效或过期时，需要重新获取。
    /// <para>provider_access_token的有效期通过返回的expires_in来传达，正常情况下为7200秒（2小时），有效期内重复获取返回相同结果，过期后获取会返回新的provider_access_token。</para>
    /// <para>provider_access_token至少保留512字节的存储空间。</para>
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
