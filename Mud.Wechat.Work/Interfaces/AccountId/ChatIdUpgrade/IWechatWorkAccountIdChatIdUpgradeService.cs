// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「群 ID 升级（对所有新授权企业）」接口族公共 SDK。
/// <para>
/// 对代开发应用模板进行群 ID 升级：调用后该模板的所有<b>新增授权企业</b>都会升级为服务商主体的群 ID，
/// 无需逐个企业调用「申请群ID的升级」接口（已授权企业不受影响，逐企业申请与群 ID 转换见
/// <see cref="IWechatWorkProviderAccountIdService"/>）。
/// 本端点以 <c>suite_access_token</c>（<b>代开发模板的接口调用凭证</b>）鉴权 —— 与企业级
/// <c>access_token</c>、服务商 <c>provider_access_token</c> 端点分属不同令牌路由键，
/// 一接口族一令牌路由键，故独立成族；官方仅向代开发模板开放，本父接口没有公共端点，
/// 唯一的 1 个端点声明于 <see cref="IWechatWorkProviderAccountIdChatIdUpgradeService"/>。
/// </para>
/// <para>
/// 注意：<c>suite_access_token</c> 为套件级凭证、无企业 scope，不经由令牌作用域机制表达。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键对齐 Authentication 域第三方授权端点的 <see cref="WechatTokenTypes.SuiteAccessToken"/>
/// 形态（Query 注入 <c>suite_access_token</c>，参数名已在组件 <c>SensitiveUrlRedactor</c> 词表内，
/// 无 G7 豁免负担）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkAccountIdChatIdUpgradeService
{
}
