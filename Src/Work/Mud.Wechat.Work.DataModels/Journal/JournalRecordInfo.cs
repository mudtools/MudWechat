// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 汇报记录详情（官方 info）。
/// <para>
/// 提交人与接收人侧复用审批域的 userid 引用结构 <see cref="ApprovalUserRef"/>；
/// 汇报内容与审批申请的 apply_data 同为一棵「控件 + 控件值」树，本域按汇报详情的 value 参数表建模为
/// <see cref="JournalControlValue"/>。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalRecordInfo
{
    /// <summary>获取或设置汇报 UUID（官方 journal_uuid）。</summary>
    [JsonPropertyName("journal_uuid")]
    public string JournalId { get; set; } = string.Empty;

    /// <summary>获取或设置汇报表单 ID（官方 template_id）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置汇报表单名称（官方 template_name）。</summary>
    [JsonPropertyName("template_name")]
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>获取或设置提交人信息（官方 submitter）。</summary>
    [JsonPropertyName("submitter")]
    public ApprovalUserRef? Submitter { get; set; }

    /// <summary>获取或设置接收人列表（官方 receivers）。</summary>
    [JsonPropertyName("receivers")]
    public List<ApprovalUserRef>? ReceiverList { get; set; }

    /// <summary>获取或设置已读接收人列表（官方 readed_receivers，官方拼写如此）。</summary>
    [JsonPropertyName("readed_receivers")]
    public List<ApprovalUserRef>? ReadedReceiverList { get; set; }

    /// <summary>获取或设置汇报内容（官方 apply_data）。</summary>
    [JsonPropertyName("apply_data")]
    public JournalApplyData? ApplyData { get; set; }

    /// <summary>获取或设置「汇报」模板数据（官方 sys_journal_data，内容为富文本）。
    /// <para>官方口径：「汇报」模板是一个特殊模板，其表单不在 <c>apply_data</c> 中返回，故该控件形态的内容只能从此字段取。</para></summary>
    [JsonPropertyName("sys_journal_data")]
    public string? SysJournalData { get; set; }

    /// <summary>获取或设置评论列表（官方 comments）。</summary>
    [JsonPropertyName("comments")]
    public List<JournalComment>? CommentList { get; set; }

    /// <summary>获取或设置汇报时间戳（官方 report_time，秒级）。</summary>
    [JsonPropertyName("report_time")]
    public long ReportTimestamp { get; set; }
}
