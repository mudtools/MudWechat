using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Authentication;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信的系统管理员可以授权安装第三方应用，安装后企业微信后台会将授权凭证、授权信息等推送给服务商后台。
/// 请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90597"/>
/// </summary>
[HttpClientApi("https://api.weixin.qq.com", Timeout = 30, TokenManage = nameof(IWechatAppManager), RegistryGroupName = "Weixin")]
[Token(TokenType = TokenType.TenantAccessToken, InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkAuthenticationService
{
    /// <summary>
    /// 获取预授权码
    /// <para>用于获取预授权码。预授权码用于企业授权时的第三方服务商安全验证。</para>
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取预授权码响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90601"/></remarks>
    [Get("/cgi-bin/service/get_pre_auth_code")]
    Task<GetPreAuthCodeResponse> GetPreAuthCodeAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// 设置授权配置
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设置授权配置响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90602"/></remarks>
    [Post("/cgi-bin/service/set_session_info")]
    Task<WechatWorkResponse> SetSessionInfoAsync([Body] SetSessionInfoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业永久授权码
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取企业永久授权码响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90603"/></remarks>
    [Post("/cgi-bin/service/get_permanent_code")]
    Task<GetPermanentCodeResponse> GetPermanentCodeAsync([Body] GetPermanentCodeRequest request, CancellationToken cancellationToken = default);
}
