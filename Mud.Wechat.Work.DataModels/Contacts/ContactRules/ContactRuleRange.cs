// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.ContactRules;

/// <summary>
/// 通讯录隐藏规则的目标范围（<c>range</c> / <c>whitelist</c> / <c>exclude</c> 三处共用结构）。
/// </summary>
/// <remarks>
/// <para><c>range</c> 为规则目标范围（被隐藏或受限的部门、成员、标签）；<c>whitelist</c> 为白名单范围
/// （允许查看的部门、成员、标签）；<c>exclude</c> 为排除名单（不受该规则限制，限制查看外部门/所有人时使用）。</para>
/// <para>上限：成员 ≤ 1000、部门 ≤ 100、标签 ≤ 100。</para>
/// </remarks>
public class ContactRuleRange
{
    /// <summary>
    /// 获取或设置成员 ID 列表（≤ 1000 个）。
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? UserIds { get; set; }

    /// <summary>
    /// 获取或设置部门 ID 列表（≤ 100 个）。
    /// </summary>
    [JsonPropertyName("partyid")]
    public List<int>? PartyIds { get; set; }

    /// <summary>
    /// 获取或设置标签 ID 列表（≤ 100 个）。
    /// </summary>
    [JsonPropertyName("tagid")]
    public List<int>? TagIds { get; set; }
}
