// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「身份验证」模块第三方网页授权登录域公共 SDK
/// （获取访问用户身份 + 获取访问用户敏感信息；企业微信 Web 登录复用同两端点换取登录用户身份）。
/// <para>
/// 官方仅向<b>第三方应用</b>开放本族端点（路由在 <c>/cgi-bin/service/auth/</c> 下、
/// 明确不允许代开发自建应用调用，须改用 <see cref="IWechatWorkIdentityService"/> 公共面），
/// 因此本父接口没有公共端点；全部 2 个端点声明于
/// <see cref="IWechatWorkThirdPartyIdentitySuiteService"/>（唯一的应用类型子接口，
/// 落位形态对齐 <see cref="IWechatWorkSecurityVipService"/> 零端点父接口 + 唯一子接口承载）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 与自建/代开发公共面（<see cref="IWechatWorkIdentityService"/>）路由、令牌均不同：
/// 令牌路由键为 <see cref="WechatTokenTypes.SuiteAccessToken"/>（Query 注入 <c>suite_access_token</c>，
/// 服务商套件级凭证），官方对同一令牌路由键独立成族（对齐获客助手组件代支付流水族落位）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkIdentitySuiteService
{
}
