// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 控制器信息对象（获取控制器列表响应 <c>controller_info_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsControllerInfo
{
    /// <summary>获取或设置 Rooms ID。</summary>
    [JsonPropertyName("rooms_id")]
    public string? RoomsId { get; set; }

    /// <summary>获取或设置 Rooms 会议室名称。</summary>
    [JsonPropertyName("meeting_room_name")]
    public string? MeetingRoomName { get; set; }

    /// <summary>获取或设置 Rooms 会议室地址。</summary>
    [JsonPropertyName("meeting_room_location")]
    public string? MeetingRoomLocation { get; set; }

    /// <summary>获取或设置控制器名称。</summary>
    [JsonPropertyName("controller_name")]
    public string? ControllerName { get; set; }

    /// <summary>获取或设置厂商。</summary>
    [JsonPropertyName("manufacture_name")]
    public string? ManufactureName { get; set; }

    /// <summary>获取或设置控制器型号。</summary>
    [JsonPropertyName("controller_model")]
    public string? ControllerModel { get; set; }

    /// <summary>获取或设置应用程序版本。</summary>
    [JsonPropertyName("app_version")]
    public string? AppVersion { get; set; }

    /// <summary>获取或设置设备状态（官方为字符串形态）："0" - 离线；"1" - 在线。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置固件版本。</summary>
    [JsonPropertyName("framework_version")]
    public string? FrameworkVersion { get; set; }

    /// <summary>获取或设置 IP 地址。</summary>
    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; }

    /// <summary>获取或设置 MAC 地址。</summary>
    [JsonPropertyName("mac_address")]
    public string? MacAddress { get; set; }

    /// <summary>获取或设置 CPU 类型。</summary>
    [JsonPropertyName("cpu_type")]
    public string? CpuType { get; set; }

    /// <summary>获取或设置 CPU 当前占有率。</summary>
    [JsonPropertyName("cpu_usage")]
    public string? CpuUsage { get; set; }

    /// <summary>获取或设置网络类型。</summary>
    [JsonPropertyName("network_type")]
    public string? NetworkType { get; set; }

    /// <summary>获取或设置内存使用大小。</summary>
    [JsonPropertyName("mem_usage")]
    public string? MemUsage { get; set; }
}
