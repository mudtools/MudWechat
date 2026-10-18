// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>kf.account.list</c>：列出企业的微信客服账号（open_kfid 的入口）。
/// </summary>
/// <remarks>
/// <b>它是客服域的入口</b>：接待人员、会话状态、发消息都要求 <c>open_kfid</c>，而模型不知道客服账号有哪些；
/// 先列账号拿到 <c>open_kfid</c>，后续三个工具才有合法入参。
/// </remarks>
[AgentTool(
    name: "kf.account.list",
    Description = "列出企业的微信客服账号。参数 offset 为分页偏移（从 0 开始，可空）、limit 为单页条数（1~100，可空 ⇒ 100）。返回 account_list（每项 open_kfid、name 客服名称、manage_privilege 是否为该账号的主体管理者）与 count。用 open_kfid 作为后续 kf.servicer.list / kf.session.get / kf.message.send 的入参。",
    Source = "IWechatWorkKfAccountService.GetAccountListAsync",
    RequiredScopes = ["kf.account.read"])]
public interface IWechatWorkKfAccountListTool
{
    /// <summary>列出微信客服账号。</summary>
    /// <param name="offset">分页偏移（可空）。</param>
    /// <param name="limit">单页条数（1~100，可空 ⇒ 100）。</param>
    void List([ToolParameter("offset", "分页偏移（可空）。")] int? offset, [ToolParameter("limit", "单页条数（1~100，可空 ⇒ 100）。")] int? limit);
}
