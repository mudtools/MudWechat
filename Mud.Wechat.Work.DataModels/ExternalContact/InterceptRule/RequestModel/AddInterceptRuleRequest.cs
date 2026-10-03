// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

/// <summary>
/// 新建敏感词规则请求体（<c>/cgi-bin/externalcontact/add_intercept_rule</c>）。
/// <para>
/// <see cref="RuleName"/> / <see cref="WordList"/> / <see cref="InterceptType"/> / <see cref="ApplicableRange"/> 为官方必填；
/// 企业敏感词规则条数上限为 100 个。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class AddInterceptRuleRequest
{
    /// <summary>
    /// 获取或设置规则名称（官方必填；1 ~ 20 个 UTF-8 字符）。
    /// </summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>
    /// 获取或设置敏感词列表（官方必填；每个词 1 ~ 32 个 UTF-8 字符，列表不超过 300 个）。
    /// </summary>
    [JsonPropertyName("word_list")]
    public List<string>? WordList { get; set; }

    /// <summary>
    /// 获取或设置语义规则列表：1 - 手机号，2 - 邮箱地址，3 - 红包。
    /// </summary>
    [JsonPropertyName("semantics_list")]
    public List<int>? SemanticsList { get; set; }

    /// <summary>
    /// 获取或设置拦截方式（官方必填）：1 - 警告并拦截发送，2 - 仅发警告。
    /// </summary>
    [JsonPropertyName("intercept_type")]
    public int? InterceptType { get; set; }

    /// <summary>
    /// 获取或设置规则的使用范围（官方必填；成员与部门不可同时为空，须在应用可见范围内）。
    /// </summary>
    [JsonPropertyName("applicable_range")]
    public InterceptRuleRange? ApplicableRange { get; set; }
}
