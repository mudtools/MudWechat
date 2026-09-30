// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions;

namespace Mud.Wechat.Work;

/// <summary>
/// 第三方/服务商套件级授权流程接口（消费 suite_access_token，属业务面）。
/// <para>企业微信的系统管理员可以授权安装第三方应用，安装后企业微信后台会将授权凭证、授权信息等推送给服务商后台。</para>
/// <para>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90597"/></para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 Feishu：BaseAddress 交由运行时解析（per-app <c>WechatAppConfig.BaseUrl</c>，
/// 默认 <c>https://qyapi.weixin.qq.com</c>），令牌由框架经 [Token] 自动注入
/// （TokenManagerKey 路由键 <see cref="WechatTokenTypes.SuiteAccessToken"/>，Query 注入参数名
/// <c>suite_access_token</c>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制套件令牌走 Query 参数，无法改用 Header，
/// 该诊断属预期且不可规避（详见详细设计 §1）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Authentication",
    TokenManage = nameof(IWechatAppManager))]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkProviderAuthenticationService
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
    /// <para>用于通过临时授权码换取企业微信的永久授权码与授权信息。</para>
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取企业永久授权码响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90603"/></remarks>
    [Post("/cgi-bin/service/get_permanent_code")]
    Task<GetPermanentCodeResponse> GetPermanentCodeAsync([Body] GetPermanentCodeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业授权信息
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取企业授权信息响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91912"/></remarks>
    [Post("/cgi-bin/service/get_auth_info")]
    Task<GetAuthInfoResponse> GetAuthInfoAsync([Body] GetAuthInfoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业永久授权码（v2）
    /// <para>官方推荐版本：耗时更短，适合在授权回调中同步换取永久授权码（旧版易超时）。</para>
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取企业永久授权码响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100776"/></remarks>
    [Post("/cgi-bin/service/v2/get_permanent_code")]
    Task<GetPermanentCodeResponse> GetPermanentCodeV2Async([Body] GetPermanentCodeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业授权信息（v2）
    /// <para>官方推荐版本：不返回插件关注二维码、性能更好。</para>
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取企业授权信息响应</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100795"/></remarks>
    [Post("/cgi-bin/service/v2/get_auth_info")]
    Task<GetAuthInfoResponse> GetAuthInfoV2Async([Body] GetAuthInfoRequest request, CancellationToken cancellationToken = default);
}
