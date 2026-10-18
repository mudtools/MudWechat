// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.tag.members</c>：读取标签内的成员与部门。
/// </summary>
/// <remarks>
/// 成员 <c>name</c> 字段按官方停返口径可能为空：执行器原样透传缺失（不猜名字），
/// 需要姓名时由模型继续调 <c>contacts.user.get</c>。
/// </remarks>
[AgentTool(
    name: "contacts.tag.members",
    Description = "读取企业微信标签内的成员与部门。参数 tag_id 为整数标签 ID（先经 contacts.tag.list 获取）。返回 tagname、userlist（userid/name，name 可能缺省）与 partylist（部门 ID）。仅包含应用可见范围内的成员。",
    Source = "IWechatWorkTagsService.GetTagMembersAsync",
    RequiredScopes = ["contacts.tag.read"],
    IsWrite = false)]
public interface IWechatWorkContactsTagMembersTool
{
    /// <summary>读取标签成员。</summary>
    /// <param name="tag_id">标签 ID（整数）。</param>
    void Members(int tag_id);
}
