// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ContactRules;

/// <summary>
/// 修改通讯录隐藏规则请求体（<c>/cgi-bin/contactrule/update</c>；仅通讯录同步应用可调用）。
/// </summary>
/// <remarks><see cref="Rules"/> 一次最多修改 100 条规则；每条规则必须携带
/// <see cref="ContactRule.RuleId"/>，且不能更新规则类型（<see cref="ContactRule.RuleType"/>）。</remarks>
public class UpdateContactRulesRequest
{
    /// <summary>
    /// 获取或设置待修改的规则列表（≤ 100 条；每条规则 <see cref="ContactRule.RuleId"/> 必填）。
    /// </summary>
    [JsonPropertyName("rules")]
    public List<ContactRule>? Rules { get; set; }
}
