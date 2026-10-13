// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 单个汇报表单的统计信息（官方 stat_list 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalStat
{
    /// <summary>获取或设置汇报表单 ID（官方 template_id）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置汇报表单名称（官方 template_name）。</summary>
    [JsonPropertyName("template_name")]
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>获取或设置汇报方式（官方 report_type，数值枚举，取值语义以官方页面为准）。</summary>
    [JsonPropertyName("report_type")]
    public int ReportType { get; set; }

    /// <summary>获取或设置汇报周期的开始时间戳（官方 cycle_begin_time，秒级）。</summary>
    [JsonPropertyName("cycle_begin_time")]
    public long CycleBeginTimestamp { get; set; }

    /// <summary>获取或设置汇报周期的结束时间戳（官方 cycle_end_time，秒级）。</summary>
    [JsonPropertyName("cycle_end_time")]
    public long CycleEndTimestamp { get; set; }

    /// <summary>获取或设置统计范围开始时间戳（官方 stat_begin_time，秒级）。</summary>
    [JsonPropertyName("stat_begin_time")]
    public long StatisticsBeginTimestamp { get; set; }

    /// <summary>获取或设置统计范围结束时间戳（官方 stat_end_time，秒级）。</summary>
    [JsonPropertyName("stat_end_time")]
    public long StatisticsEndTimestamp { get; set; }

    /// <summary>获取或设置汇报人员范围信息（官方 report_range）。</summary>
    [JsonPropertyName("report_range")]
    public JournalReporterRange? ReporterRange { get; set; }

    /// <summary>获取或设置白名单范围信息（官方 white_range，与 report_range 同形）。</summary>
    [JsonPropertyName("white_range")]
    public JournalReporterRange? ReporterWhiteListRange { get; set; }

    /// <summary>获取或设置接收人员范围信息（官方 receivers）。</summary>
    [JsonPropertyName("receivers")]
    public JournalReceiverRange? ReceiverRange { get; set; }

    /// <summary>获取或设置已汇报人员列表（官方 report_list）。</summary>
    [JsonPropertyName("report_list")]
    public List<JournalReporter>? ReporterList { get; set; }

    /// <summary>获取或设置未汇报人员列表（官方 unreport_list，与 report_list 同为 <see cref="JournalReporter"/> 形态）。</summary>
    [JsonPropertyName("unreport_list")]
    public List<JournalReporter>? UnreportedReporterList { get; set; }
}
