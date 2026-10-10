// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

/// <summary>
/// 获取敏感词规则列表响应体（<c>/cgi-bin/externalcontact/get_intercept_rule_list</c>，官方契约本端点为 GET 且无业务参数）。
/// <para>可获取企业所有敏感词规则。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class GetInterceptRuleListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置敏感词规则摘要列表。
    /// </summary>
    [JsonPropertyName("rule_list")]
    public List<InterceptRuleSummary>? RuleList { get; set; }
}

/// <summary>
/// 敏感词规则摘要（获取敏感词规则列表响应中 <c>rule_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class InterceptRuleSummary
{
    /// <summary>
    /// 获取或设置规则 id。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public string? RuleId { get; set; }

    /// <summary>
    /// 获取或设置规则名称（长度上限 20 字符）。
    /// </summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>
    /// 获取或设置规则的创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}
