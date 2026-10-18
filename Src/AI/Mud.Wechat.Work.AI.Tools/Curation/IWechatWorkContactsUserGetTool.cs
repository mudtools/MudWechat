// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.user.get</c>：按 userid 读取单个成员的基本信息。
/// </summary>
/// <remarks>
/// <b>不投影手机号 / 邮箱 / 头像</b>：2022-06-20 起新应用不再返回这些字段（需 oauth2 手工授权），
/// 且它们属于个人敏感信息——即使上游偶发返回，执行器也不外发（见 <c>ContactsUserToolHandlers</c>）。
/// </remarks>
[AgentTool(
    name: "contacts.user.get",
    Description = "按 userid 读取企业微信成员信息。参数 userid 为成员 UserID（企业内唯一，不区分大小写，1~64 字节）。返回 userid、姓名、别名、职务、所属部门 ID、主部门、启用状态与直属上级。仅能读取应用可见范围内的成员。",
    Source = "IWechatWorkUsersService.GetUserAsync",
    RequiredScopes = ["contacts.user.read"],
    IsWrite = false)]
public interface IWechatWorkContactsUserGetTool
{
    /// <summary>读取成员。</summary>
    /// <param name="userid">成员 UserID（企业内唯一）。</param>
    void User(string userid);
}
