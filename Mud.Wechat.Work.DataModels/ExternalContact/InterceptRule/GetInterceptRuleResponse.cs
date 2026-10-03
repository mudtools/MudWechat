// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

/// <summary>
/// 获取敏感词规则详情响应体（<c>/cgi-bin/externalcontact/get_intercept_rule</c>）。
/// <para>使用范围字段只返回应用可见范围内的成员和部门。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class GetInterceptRuleResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置敏感词规则详情。
    /// </summary>
    [JsonPropertyName("rule")]
    public InterceptRuleDetail? Rule { get; set; }
}

/// <summary>
/// 敏感词规则详情（<c>rule</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class InterceptRuleDetail
{
    /// <summary>
    /// 获取或设置规则 id。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public string? RuleId { get; set; }

    /// <summary>
    /// 获取或设置规则名称（上限 20 字符）。
    /// </summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>
    /// 获取或设置敏感词列表（每个词不超过 30 字符，列表不超过 300 个）。
    /// </summary>
    [JsonPropertyName("word_list")]
    public List<string>? WordList { get; set; }

    /// <summary>
    /// 获取或设置额外的规则（语义规则）。
    /// </summary>
    [JsonPropertyName("extra_rule")]
    public InterceptRuleExtraRule? ExtraRule { get; set; }

    /// <summary>
    /// 获取或设置语义规则列表：1 - 手机号，2 - 邮箱地址，3 - 红包。
    /// </summary>
    [JsonPropertyName("semantics_list")]
    public List<int>? SemanticsList { get; set; }

    /// <summary>
    /// 获取或设置拦截方式：1 - 警告并拦截发送，2 - 仅发警告。
    /// </summary>
    [JsonPropertyName("intercept_type")]
    public int? InterceptType { get; set; }

    /// <summary>
    /// 获取或设置规则的使用范围（仅返回应用可见范围内的成员和部门）。
    /// </summary>
    [JsonPropertyName("applicable_range")]
    public InterceptRuleRange? ApplicableRange { get; set; }

    /// <summary>
    /// 获取或设置规则的创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}
