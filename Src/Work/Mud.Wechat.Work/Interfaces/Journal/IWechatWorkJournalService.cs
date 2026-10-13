// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「汇报」（日志）模块公共 SDK（汇报记录号列表、汇报记录详情、汇报统计数据）。
/// <para>
/// 官方「汇报」文档树（分类 93392）只在<b>企业自建应用</b>开发根下存在页面
/// （获取汇报记录号列表 93393 / 获取汇报记录详情 93394 / 获取汇报统计数据 93395），
/// 因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
/// 全部 3 个端点声明于 <see cref="IWechatWorkInternalJournalService"/>
/// （形态对齐 <see cref="IWechatWorkDialService"/> 零端点父接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkContactRulesService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由。
/// </para>
/// <para>
/// 与「审批」域（<see cref="IWechatWorkApprovalService"/>）分属官方 OA 的两棵子树；本域端点统一落在
/// <c>/cgi-bin/oa/journal/</c> 前缀下，与审批域的 <c>/cgi-bin/oa/applyevent</c>、<c>/cgi-bin/oa/getapprovalinfo</c>
/// 等路由互不承载。
/// 官方「汇报导出」两页（96108）不在应用开发目录树内，本域不声明其端点（见留档零声明口径）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkJournalService
{
}
