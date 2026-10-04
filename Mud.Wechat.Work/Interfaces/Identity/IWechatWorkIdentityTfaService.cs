// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「身份验证」模块二次验证域公共 SDK（获取用户二次验证信息 + 使用二次验证）。
/// <para>
/// 官方仅向「通讯录同步」或<b>企业自建应用</b>开放本域端点，第三方应用与服务商代开发均无对应文档，
/// 因此本父接口没有公共端点；全部 2 个端点声明于
/// <see cref="IWechatWorkInternalIdentityTfaService"/>（唯一的应用类型子接口，
/// 落位形态对齐 <see cref="IWechatWorkSecurityVipService"/> 零端点父接口 + 唯一子接口承载）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 登录二次验证（<c>/cgi-bin/user/authsucc</c>，官方文档 99521）已由通讯录成员域
/// <see cref="IWechatWorkUsersService.CompleteSecondaryAuthAsync"/> 承载，本域不重复提供。
/// </para>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>，应用自身凭证），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkIdentityTfaService
{
}
