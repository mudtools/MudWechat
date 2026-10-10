// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 删除指定成员额外权限请求体（<c>/cgi-bin/wedoc/smartsheet/content_priv/delete_rule</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class DeleteWedocSheetPrivRuleRequest
{
    /// <summary>获取或设置智能表 id（官方 <c>docid</c>，必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置需要删除的规则 id 列表（官方 <c>rule_id_list</c>，必填）。</summary>
    [JsonPropertyName("rule_id_list")]
    public List<uint>? RuleIdList { get; set; }
}
