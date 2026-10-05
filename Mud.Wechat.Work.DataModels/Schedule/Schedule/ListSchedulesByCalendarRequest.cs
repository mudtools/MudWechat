// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 获取日历下的日程列表请求体（<c>/cgi-bin/oa/schedule/get_by_calendar</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：仅可获取应用自己创建的日历下的日程。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ListSchedulesByCalendarRequest
{
    /// <summary>获取或设置日历 ID（官方必填）。</summary>
    [JsonPropertyName("cal_id")]
    public string? CalId { get; set; }

    /// <summary>获取或设置分页偏移量（默认为 0，以 0 为起点）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置分页预期请求的数据量（默认 500，取值范围 1 ~ 1000）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
