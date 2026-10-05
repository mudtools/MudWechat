// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议发起记录响应体（<c>/cgi-bin/meeting/statistics/get_start_list</c>；官方仅向自建应用开放）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingStartListResponse : WechatWorkResponse
{
    /// <summary>获取或设置分页游标（当前数据最后一个 key 值，下次调用带上该值则从该 key 值往后拉取）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置是否还有数据待拉取。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>
    /// 获取或设置发起成功或失败的会议记录列表（取决于请求的 <c>type</c> 参数，详见 <see cref="MeetingStartRecord"/>）。
    /// <para>记录按时间从大到小排序；会过滤会议发起者不在应用可见范围中的记录，故返回记录数可能小于 <c>limit</c>。</para>
    /// </summary>
    [JsonPropertyName("meeting_list")]
    public List<MeetingStartRecord>? MeetingList { get; set; }
}
