using Mud.Wechat.Work.DataModels.InternalAppAuthentication;
using Mud.Wechat.Work.Options;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信的企业内部应用开发授权。
/// 请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90597"/>
/// </summary>
[HttpClientApi(RegistryGroupName = "Authentication")]
public interface IWechatWorkInternalAppAuthentication
{
    /// <summary>
    /// 获取调用企业微信企业内部应用开发接口的access_token。
    /// <para>access_token是调用企业微信API接口的前提，相当于创建了一个登录凭证，其它的业务API接口，都需要依赖于access_token来鉴权调用者身份。</para>
    /// </summary>
    /// <param name="corpid">企业ID，对应 <see cref="WechatWorkClientOptions.CorpId"/> 参数。</param>
    /// <param name="corpsecret">应用的凭证密钥，注意应用需要是启用状态，对应 <see cref="WechatWorkClientOptions.AgentSecret"/> 参数。</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取access_token响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91039"/></remarks>
    [Get("/cgi-bin/gettoken")]
    Task<GetTokenResponse> GetTokenAsync(
        [Query("corpid")] string corpid,
        [Query("corpsecret")] string corpsecret,
        CancellationToken cancellationToken = default);

}
