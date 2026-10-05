// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议发起记录请求体（<c>/cgi-bin/meeting/statistics/get_start_list</c>；官方仅向自建应用开放）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingStartListRequest
{
    /// <summary>
    /// 获取或设置查询的会议发起记录类型（官方必填）：1 - 发起成功的会议记录；2 - 发起失败的会议
    /// （企业同时发起的会议数已达上限，员工无法发起）。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置查询范围起始时间的 Unix 时间戳（秒，官方必填；与结束时间跨度不超过 30 天，查询区间左闭右开）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置查询范围结束时间的 Unix 时间戳（秒，官方必填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置每次拉取的数据量（默认 200，最大 1000）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置分页游标（首次调用可不填）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
