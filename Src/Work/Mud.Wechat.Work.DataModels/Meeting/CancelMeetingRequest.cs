// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 取消预约会议请求体（<c>/cgi-bin/meeting/cancel</c>；三种应用类型请求形态一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CancelMeetingRequest
{
    /// <summary>获取或设置会议 ID（官方必填；仅允许取消预约状态下的会议）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置周期性子会议 ID（仅预约会议高级管理文档页声明该参数）。
    /// <para>如果取消周期性会议且该字段不传，则会取消该系列的周期性会议。</para>
    /// </summary>
    [JsonPropertyName("sub_meetingid")]
    public string? SubMeetingid { get; set; }
}
