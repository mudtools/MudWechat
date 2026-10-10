// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议录制列表响应体（<c>/cgi-bin/meeting/record/list</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档陷阱：官方响应示例将录制列表字段误写为 <c>record_meetings</c>，参数表为 <c>record_list</c>，本模型以参数表为准。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ListMeetingRecordsResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否还有未拉取的会议录制列表。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置分页游标（<c>has_more</c> = true 时有值，表示当前数据最后一个 key 值，下次调用带上该值则从该 key 值往后拉取）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置会议录制列表（详见 <see cref="MeetingRecord"/>）。</summary>
    [JsonPropertyName("record_list")]
    public List<MeetingRecord>? RecordList { get; set; }
}
