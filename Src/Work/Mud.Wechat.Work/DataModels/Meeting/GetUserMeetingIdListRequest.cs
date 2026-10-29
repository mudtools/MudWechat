// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取成员会议 ID 列表请求体（<c>/cgi-bin/meeting/get_user_meetingid</c>；三种应用类型请求形态一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetUserMeetingIdListRequest
{
    /// <summary>获取或设置企业成员的 userid（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置分页游标（上一次调用返回的 <c>next_cursor</c>，初次调用可填 "0"）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>获取或设置每次拉取的数据量（默认值和最大值均为 100）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置查询起始时间的 Unix 时间戳。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>
    /// 获取或设置查询结束时间的 Unix 时间戳（<see cref="BeginTime"/> 与 <see cref="EndTime"/> 时间跨度不超过 180 天；
    /// 两者都没填时，默认 <see cref="EndTime"/> 为当前时间）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
