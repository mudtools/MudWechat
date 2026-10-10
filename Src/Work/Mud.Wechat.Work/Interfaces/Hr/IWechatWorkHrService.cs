// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「人事助手」模块花名册域公共 SDK（获取员工字段配置 / 获取员工花名册信息 / 更新员工花名册信息）。
/// <para>
/// 官方仅向<b>企业自建应用</b>开放本域端点（代开发应用与第三方应用均暂不支持），
/// 因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
/// 全部 3 个端点声明于 <see cref="IWechatWorkInternalHrService"/>
/// （形态对齐 <see cref="IWechatWorkMsgAuditPermitUserService"/> 零端点父接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkContactRulesService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由。本模块全部接口的调用应用须配置到「人事助手 - 可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkHrService
{
}
