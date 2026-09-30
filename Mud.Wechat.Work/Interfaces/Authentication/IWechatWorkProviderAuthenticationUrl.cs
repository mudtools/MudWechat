// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 服务商代开发带参授权链接接口（get_customized_auth_url，消费 provider_access_token，属业务面）。
/// <para>用于为代开发自建应用生成安装二维码链接（企业管理员扫码后完成安装授权）。</para>
/// <para>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98744"/></para>
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐 <see cref="Abstractions.Authentication.IWechatWorkCorpTokenAuthentication"/>：
/// <b>显式传令牌参数</b>（<c>[Query("provider_access_token")]</c>），不带 <c>[Token]</c>——
/// 服务商令牌是 per-app 的，须先经 <c>IWechatAppContextSwitcher.UseApp(appKey)</c> 切到目标应用后，
/// 由编排服务从该应用的 <c>IWechatProviderTokenManager</c> 取值再显式传入。
/// </para>
/// <para>
/// 因此本接口不进入契约守卫 G5（Query 令牌注入白名单），白名单保持仅
/// <see cref="IWechatWorkProviderAuthenticationService"/>。
/// </para>
/// <para>注册复用 <c>AddAuthenticationWebApiHttpClient()</c>（RegistryGroupName = "Authentication"），不新增独立注册项。</para>
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "Authentication")]
public interface IWechatWorkProviderAuthenticationUrl
{
    /// <summary>
    /// 获取代开发自建应用的带参授权链接（携带 <c>provider_access_token</c> 于 Query）。
    /// </summary>
    /// <param name="providerAccessToken">服务商访问令牌（provider_access_token，Query 注入）。</param>
    /// <param name="request">获取带参授权链接请求体（state + templateid_list）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带参授权链接响应（含 qrcode_url）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98744"/></remarks>
    [Post("/cgi-bin/service/get_customized_auth_url")]
    Task<GetCustomizedAuthUrlResponse?> GetCustomizedAuthUrlAsync(
        [Query("provider_access_token")] string providerAccessToken,
        [Body] GetCustomizedAuthUrlRequest request,
        CancellationToken cancellationToken = default);
}