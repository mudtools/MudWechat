// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「应用管理」模块「自定义菜单」域公共 SDK（创建菜单 + 获取菜单 + 删除菜单）。
/// <para>
/// 官方仅向企业自建应用开放（自定义菜单三个端点的权限说明均为「仅企业可调用；第三方不可调用」，
/// 服务商代开发章节亦无对应 API），故本父接口零端点，
/// 全部 3 个端点由唯一自建子接口 <see cref="IWechatWorkInternalAgentMenuService"/> 承载。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：自建应用消费应用自身 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAgentMenuService
{
}
