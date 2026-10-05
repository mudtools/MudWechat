// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 录制访问统计对象（获取录制文件访问统计响应 <c>summaries</c> 嵌套对象；按天维度返回）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RecordStatisticsSummary
{
    /// <summary>获取或设置统计时间（格式：yyyy-MM-dd）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>获取或设置观看次数（当天数据，默认 0）。</summary>
    [JsonPropertyName("view_count")]
    public int? ViewCount { get; set; }

    /// <summary>获取或设置下载次数（当天数据，默认 0）。</summary>
    [JsonPropertyName("download_count")]
    public int? DownloadCount { get; set; }
}
