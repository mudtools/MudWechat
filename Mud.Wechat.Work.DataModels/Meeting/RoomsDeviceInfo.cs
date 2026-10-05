// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 设备信息对象（获取设备列表响应 <c>device_info_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsDeviceInfo
{
    /// <summary>获取或设置 Rooms 会议室 ID。</summary>
    [JsonPropertyName("meeting_room_id")]
    public string? MeetingRoomId { get; set; }

    /// <summary>获取或设置 Rooms ID。</summary>
    [JsonPropertyName("rooms_id")]
    public string? RoomsId { get; set; }

    /// <summary>获取或设置 Rooms 会议室名称。</summary>
    [JsonPropertyName("meeting_room_name")]
    public string? MeetingRoomName { get; set; }

    /// <summary>获取或设置 Rooms 会议室地址。</summary>
    [JsonPropertyName("meeting_room_location")]
    public string? MeetingRoomLocation { get; set; }

    /// <summary>获取或设置设备型号。</summary>
    [JsonPropertyName("device_model")]
    public string? DeviceModel { get; set; }

    /// <summary>获取或设置应用程序版本（文档页说明误写为「激活码」，官方示例为版本号形态，本模型按版本号语义承载）。</summary>
    [JsonPropertyName("app_version")]
    public string? AppVersion { get; set; }

    /// <summary>获取或设置 Rooms 会议室状态：0 - 未激活；1 - 未绑定；2 - 空闲；3 - 使用中；4 - 离线。</summary>
    [JsonPropertyName("meeting_room_status")]
    public int? MeetingRoomStatus { get; set; }

    /// <summary>获取或设置设备健康信息（详见 <see cref="RoomsDeviceMonitorInfo"/>）。</summary>
    [JsonPropertyName("device_monitor_info")]
    public RoomsDeviceMonitorInfo? DeviceMonitorInfo { get; set; }
}
