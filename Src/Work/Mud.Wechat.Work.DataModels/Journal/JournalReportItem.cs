// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 汇报人员的单条汇报条目（官方 itemlist 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalReportItem
{
    /// <summary>获取或设置汇报 UUID（官方 journaluuid，无汇报时不出现）。</summary>
    [JsonPropertyName("journaluuid")]
    public string? JournalId { get; set; }

    /// <summary>获取或设置汇报时间戳（官方 reporttime，秒级）。</summary>
    [JsonPropertyName("reporttime")]
    public long? ReportTimestamp { get; set; }

    /// <summary>获取或设置迟交标识（官方 flag；<b>官方以数字 0/1 承载布尔语义</b>，本仓无数值-布尔转换器，故原样以 <c>int</c> 承载）。</summary>
    [JsonPropertyName("flag")]
    public int? LateFlag { get; set; }
}
