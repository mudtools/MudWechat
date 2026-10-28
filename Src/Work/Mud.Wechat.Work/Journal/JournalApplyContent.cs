// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 汇报内容控件项（官方 contents 元素：control + id + title + value）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalApplyContent
{
    /// <summary>获取或设置控件类型（官方 control，取值以官方页面为准）。</summary>
    [JsonPropertyName("control")]
    public string Control { get; set; } = string.Empty;

    /// <summary>获取或设置控件 ID（官方 id）。</summary>
    [JsonPropertyName("id")]
    public string ControlId { get; set; } = string.Empty;

    /// <summary>获取或设置控件名称（官方 title，多语言列表，复用审批域的 text + lang 结构 <see cref="ApprovalLangTextItem"/>）。</summary>
    [JsonPropertyName("title")]
    public List<ApprovalLangTextItem>? Title { get; set; }

    /// <summary>获取或设置控件值（官方 value，超集形态，见 <see cref="JournalControlValue"/>）。</summary>
    [JsonPropertyName("value")]
    public JournalControlValue? Value { get; set; }
}
