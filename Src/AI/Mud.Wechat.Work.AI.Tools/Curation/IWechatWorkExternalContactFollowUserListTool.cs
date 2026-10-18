// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.follow_user.list</c>：拉取配置了「客户联系」功能的成员列表。
/// </summary>
/// <remarks>
/// <para>
/// <b>它是客户联系域的入口</b>：客户列表按「成员」维度取（<c>externalcontact.contact.list</c> 需要 userid），
/// 而模型不知道企业里谁开了客户联系——先列成员，再按成员列客户，两跳自举，不需要用户提供任何 ID。
/// </para>
/// <para>
/// <b>只覆盖应用可见范围</b>：官方返回的是「本应用可操作的」配置成员，未配置客户联系功能的成员不在其中。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.follow_user.list",
    Description = "拉取企业微信中配置了客户联系功能的成员列表（零参数）。返回 follow_user（成员 userid 数组）与 count。用它作为客户查询的入口：先拿到成员 userid，再用 externalcontact.contact.list 列出该成员添加的客户。仅返回本应用可见范围内的成员。",
    Source = "IWechatWorkExternalContactFollowUserService.GetFollowUserListAsync",
    RequiredScopes = ["externalcontact.follow_user.read"])]
public interface IWechatWorkExternalContactFollowUserListTool
{
    /// <summary>拉取配置了客户联系功能的成员列表。</summary>
    void List();
}
