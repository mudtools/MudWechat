// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 更新待办状态请求体（<c>/cgi-bin/todo/update</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class UpdateScheduleTodoRequest
{
    /// <summary>获取或设置待办 ID（官方必填）。</summary>
    [JsonPropertyName("todo_id")]
    public string? TodoId { get; set; }

    /// <summary>获取或设置待办整体状态：0 - 完成；1 - 进行中（官方选填；不传不修改整体状态）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置待办参与人列表（官方选填；最多支持 20 个参与人，可不传或传空数组；不传或为空时不修改参与人列表）。</summary>
    [JsonPropertyName("attendees")]
    public List<ScheduleTodoAttendee>? Attendees { get; set; }
}
