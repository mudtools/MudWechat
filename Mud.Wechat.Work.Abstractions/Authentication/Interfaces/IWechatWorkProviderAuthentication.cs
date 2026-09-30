// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信服务商（provider）认证接口：服务商令牌 + 套件令牌（由令牌管理器直调）。
/// <para>接口详细文档请参见：<see href="https://developer.work.weixin.qq.com/document/path/97201"/></para>
/// </summary>
/// <remarks>
/// 令牌签发接口公共约定：不带 [Token]、不硬编码 BaseAddress（见
/// <see cref="IWechatWorkInternalAppAuthentication"/> 的说明）。
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "Authentication")]
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
    Task<GetProviderTokenResponse?> GetProviderTokenAsync([Body] GetProviderTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取第三方应用凭证（suite_access_token，依赖 suite_ticket）。
    /// <para>suite_ticket由腾讯后台每隔十分钟定时推送，不能主动获取。有效期内重复获取返回相同结果。</para>
    /// </summary>
    /// <param name="request">获取第三方应用凭证请求体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取第三方应用凭证响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90600"/></remarks>
    [Post("/cgi-bin/service/get_suite_token")]
    Task<GetSuiteTokenResponse?> GetSuiteTokenAsync([Body] GetSuiteTokenRequest request, CancellationToken cancellationToken = default);
}
