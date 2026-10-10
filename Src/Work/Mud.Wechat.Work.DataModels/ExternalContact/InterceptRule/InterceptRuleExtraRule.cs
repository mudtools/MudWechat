// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

/// <summary>
/// 敏感词规则的额外规则（<c>extra_rule</c>）。
/// <para>修改规则时 <see cref="SemanticsList"/> 为空表示清除所有语义规则。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class InterceptRuleExtraRule
{
    /// <summary>
    /// 获取或设置语义规则列表：1 - 手机号，2 - 邮箱地址，3 - 红包。
    /// </summary>
    [JsonPropertyName("semantics_list")]
    public List<int>? SemanticsList { get; set; }
}
