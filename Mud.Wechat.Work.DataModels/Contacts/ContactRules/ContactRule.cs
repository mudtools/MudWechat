// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.ContactRules;

/// <summary>
/// 通讯录隐藏规则（创建 / 修改规则的 <c>rules[]</c> 元素，读取规则列表的响应元素）。
/// </summary>
/// <remarks>
/// <para><see cref="RuleType"/>：1 - 隐藏部门/成员；2 - 限制查看外部门；3 - 限制查看所有人。
/// 修改规则时不能更新规则类型。</para>
/// <para>创建时 <see cref="RuleId"/> 不填（由官方生成）；修改时必填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ContactRules")]
public class ContactRule
{
    /// <summary>
    /// 获取或设置规则唯一标识（创建时不填；修改时必填；读取时返回）。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public int? RuleId { get; set; }

    /// <summary>
    /// 获取或设置规则类型：1 - 隐藏部门/成员；2 - 限制查看外部门；3 - 限制查看所有人。
    /// </summary>
    [JsonPropertyName("rule_type")]
    public int RuleType { get; set; }

    /// <summary>
    /// 获取或设置规则目标范围（被隐藏或受限的部门、成员、标签）。
    /// </summary>
    [JsonPropertyName("range")]
    public ContactRuleRange? Range { get; set; }

    /// <summary>
    /// 获取或设置白名单范围（允许查看的部门、成员、标签）。
    /// </summary>
    [JsonPropertyName("whitelist")]
    public ContactRuleRange? Whitelist { get; set; }

    /// <summary>
    /// 获取或设置排除名单范围（不受该规则限制的部门、成员、标签；限制查看外部门/所有人时使用）。
    /// </summary>
    [JsonPropertyName("exclude")]
    public ContactRuleRange? Exclude { get; set; }

    /// <summary>
    /// 获取或设置是否允许搜索（限制查看外部门/所有人时使用）。
    /// </summary>
    [JsonPropertyName("is_allowed_search")]
    public bool? IsAllowedSearch { get; set; }

    /// <summary>
    /// 获取或设置是否允许会话（限制查看外部门/所有人时使用）。
    /// </summary>
    [JsonPropertyName("is_allowed_conversation")]
    public bool? IsAllowedConversation { get; set; }
}
