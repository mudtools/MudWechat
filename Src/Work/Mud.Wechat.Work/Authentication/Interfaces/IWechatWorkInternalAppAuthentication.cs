// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业内部自建应用认证接口（令牌签发，由 <c>InternalAppTokenManager</c> 直调）。
/// <para>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91039"/></para>
/// </summary>
/// <remarks>
/// 公共约定（对齐 <c>IFeishuAuthentication</c>）：令牌签发接口位于 Abstractions、
/// 不带 [Token]（令牌管理器直调，自递归约束——刷新请求自身不得进入恢复链路）、
/// 不硬编码 BaseAddress（由 per-app <c>WechatAppConfig.BaseUrl</c> 经
/// <c>WechatHttpClientFactory</c> 运行时解析）。
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "Authentication")]
public interface IWechatWorkInternalAppAuthentication
{
    /// <summary>
    /// 获取调用企业微信企业内部应用开发接口的access_token。
    /// <para>access_token是调用企业微信API接口的前提，相当于创建了一个登录凭证，其它的业务API接口，都需要依赖于access_token来鉴权调用者身份。</para>
    /// </summary>
    /// <param name="corpid">企业ID，对应 <see cref="Configuration.WechatAppConfig.CorpId"/> 参数。</param>
    /// <param name="corpsecret">应用的凭证密钥，注意应用需要是启用状态，对应 <see cref="Configuration.WechatAppConfig.AgentSecret"/> 参数。</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取access_token响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91039"/></remarks>
    [Get("/cgi-bin/gettoken")]
    Task<GetTokenResponse?> GetTokenAsync(
        [Query("corpid")] string corpid,
        [Query("corpsecret")] string corpsecret,
        CancellationToken cancellationToken = default);
}
