// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信服务商代开发/第三方应用的授权企业令牌获取接口（由 <c>CorpTokenManager</c> 直调）。
/// <para>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91132"/></para>
/// </summary>
/// <remarks>
/// 令牌签发接口公共约定：不带 [Token]、不硬编码 BaseAddress（见
/// <see cref="IWechatWorkInternalAppAuthentication"/> 的说明）。
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "Authentication")]
public interface IWechatWorkCorpTokenAuthentication
{
    /// <summary>
    /// 凭企业授权信息（auth_corpid + permanent_code）换取授权企业的 access_token
    /// （携带 suite_access_token 于 Query，由调用方经套件令牌管理器取得后传入 URL）。
    /// </summary>
    /// <param name="suiteAccessToken">套件访问令牌（suite_access_token，Query 注入）。</param>
    /// <param name="request">获取授权企业令牌请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>获取授权企业 access_token 响应。</returns>
    [Post("/cgi-bin/service/get_corp_token")]
    Task<GetCorpTokenResponse?> GetCorpTokenAsync(
        [Query("suite_access_token")] string suiteAccessToken,
        [Body] GetCorpTokenRequest request,
        CancellationToken cancellationToken = default);
}
