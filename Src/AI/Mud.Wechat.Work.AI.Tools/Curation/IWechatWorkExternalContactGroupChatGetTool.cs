// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.groupchat.get</c>：按 chat_id 读取客户群详情（群名、群主、公告、成员与管理员）。
/// </summary>
/// <remarks>
/// <para>
/// <b>姓名是可选成本</b>：<c>name</c> 只有 <c>need_name = 1</c> 时才由官方返回，默认不返回（少一次跨端取昵称）。
/// 工具把它做成显式开关而不是恒开：查清「谁在群里」通常只需 <c>userid</c> 与入群事实。
/// </para>
/// <para>
/// <b>unionid 不外发</b>：它是客户在微信开放平台的跨应用身份关联键，与客户域同一条边界——群成员列表里同样剔除。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.groupchat.get",
    Description = "按客户群 ID 获取企业微信客户群详情。参数 chat_id 为客户群 ID（先用 externalcontact.groupchat.list 获取）；need_name 可选（true 时额外返回群成员的姓名，默认 false 只给 userid）。返回 chat_id、name（群名）、owner（群主 userid）、create_time（建群时间戳）、notice（群公告）、member_count、members（userid、type 1 企业成员 / 2 外部联系人、join_time、join_scene 1 直接邀请 / 2 邀请链接 / 3 扫码入群、invitor 邀请人 userid、group_nickname 群内昵称、name 姓名（需 need_name=true））、admin_list（管理员 userid）与 member_version。不含 unionid（跨应用身份关联键不外发）。群成员变动在官方侧为异步处理，刚发生变更时详情可能滞后。",
    Source = "IWechatWorkExternalContactGroupChatService.GetGroupChatDetailAsync",
    RequiredScopes = ["externalcontact.groupchat.read"])]
public interface IWechatWorkExternalContactGroupChatGetTool
{
    /// <summary>读取客户群详情。</summary>
    /// <param name="chat_id">客户群 ID（先用 externalcontact.groupchat.list 获取）。</param>
    /// <param name="need_name">是否返回群成员姓名（true 返回；可空 ⇒ false）。</param>
    void Get([ToolParameter("chat_id", "客户群 ID（先用 externalcontact.groupchat.list 获取）。", Required = true)] string chat_id, [ToolParameter("need_name", "是否返回群成员姓名（true 返回；可空 ⇒ false）。")] bool? need_name);
}
