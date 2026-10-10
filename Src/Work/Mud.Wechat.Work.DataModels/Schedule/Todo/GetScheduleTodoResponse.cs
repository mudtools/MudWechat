// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 获取待办详情响应体（<c>/cgi-bin/todo/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class GetScheduleTodoResponse : WechatWorkResponse
{
    /// <summary>获取或设置待办内容。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置待办创建人 ID。</summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>获取或设置待办状态：0 - 已完成；1 - 进行中。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置待办创建时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置待办参与人列表。</summary>
    [JsonPropertyName("attendees")]
    public List<ScheduleTodoAttendee>? Attendees { get; set; }

    /// <summary>获取或设置待办截止时间戳。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置提醒列表。</summary>
    [JsonPropertyName("reminders")]
    public List<ScheduleTodoReminder>? Reminders { get; set; }
}
