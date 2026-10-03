// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

/// <summary>
/// 修改敏感词规则请求体（<c>/cgi-bin/externalcontact/update_intercept_rule</c>）。
/// <para><see cref="RuleId"/> 为官方必填，除 rule_id 外仅需更新的字段才填；
/// 使用范围通过 add / remove 两侧增删（非整体覆盖）；应用只可修改自己创建的规则。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class UpdateInterceptRuleRequest
{
    /// <summary>
    /// 获取或设置规则 id（官方必填）。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public string? RuleId { get; set; }

    /// <summary>
    /// 获取或设置规则名称（1 ~ 20 个 UTF-8 字符）。
    /// </summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>
    /// 获取或设置敏感词列表（每个词 1 ~ 32 个 UTF-8 字符，最多 300 个；为空则忽略该字段）。
    /// </summary>
    [JsonPropertyName("word_list")]
    public List<string>? WordList { get; set; }

    /// <summary>
    /// 获取或设置额外的规则（<see cref="InterceptRuleExtraRule.SemanticsList"/> 为空表示清除所有语义规则）。
    /// </summary>
    [JsonPropertyName("extra_rule")]
    public InterceptRuleExtraRule? ExtraRule { get; set; }

    /// <summary>
    /// 获取或设置拦截方式：1 - 警告并拦截发送，2 - 仅发警告。
    /// </summary>
    [JsonPropertyName("intercept_type")]
    public int? InterceptType { get; set; }

    /// <summary>
    /// 获取或设置需新增的使用范围（须在应用可见范围内；单次最多 1000 个节点）。
    /// </summary>
    [JsonPropertyName("add_applicable_range")]
    public InterceptRuleRange? AddApplicableRange { get; set; }

    /// <summary>
    /// 获取或设置需删除的使用范围（须在应用可见范围内；单次最多 1000 个节点）。
    /// </summary>
    [JsonPropertyName("remove_applicable_range")]
    public InterceptRuleRange? RemoveApplicableRange { get; set; }
}
