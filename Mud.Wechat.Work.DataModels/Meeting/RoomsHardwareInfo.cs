// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室硬件信息对象（获取 Rooms 会议室详情响应 <c>hardware_info</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>官方文档陷阱：<see cref="MonitorFrequency"/> 参数表标注为 string，官方示例为数字 0，本模型以参数表为准（字符串承载）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsHardwareInfo
{
    /// <summary>获取或设置厂家。</summary>
    [JsonPropertyName("factory")]
    public string? Factory { get; set; }

    /// <summary>获取或设置设备型号。</summary>
    [JsonPropertyName("device_model")]
    public string? DeviceModel { get; set; }

    /// <summary>获取或设置序列号。</summary>
    [JsonPropertyName("sn")]
    public string? Sn { get; set; }

    /// <summary>获取或设置 IP 地址。</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>获取或设置 MAC 地址。</summary>
    [JsonPropertyName("mac")]
    public string? Mac { get; set; }

    /// <summary>获取或设置 Rooms 版本。</summary>
    [JsonPropertyName("rooms_version")]
    public string? RoomsVersion { get; set; }

    /// <summary>获取或设置固件版本。</summary>
    [JsonPropertyName("firmware_version")]
    public string? FirmwareVersion { get; set; }

    /// <summary>获取或设置健康状况。</summary>
    [JsonPropertyName("health_status")]
    public string? HealthStatus { get; set; }

    /// <summary>获取或设置设备系统。</summary>
    [JsonPropertyName("system_type")]
    public string? SystemType { get; set; }

    /// <summary>获取或设置 Rooms 会议室状态：0 - 未激活；1 - 未绑定；2 - 空闲；3 - 使用中；4 - 离线。</summary>
    [JsonPropertyName("meeting_room_status")]
    public int? MeetingRoomStatus { get; set; }

    /// <summary>获取或设置激活时间。</summary>
    [JsonPropertyName("active_time")]
    public string? ActiveTime { get; set; }

    /// <summary>获取或设置 CPU 信息。</summary>
    [JsonPropertyName("cpu_info")]
    public string? CpuInfo { get; set; }

    /// <summary>获取或设置 CPU 最大占用率。</summary>
    [JsonPropertyName("cpu_usage")]
    public string? CpuUsage { get; set; }

    /// <summary>获取或设置 GPU 信息。</summary>
    [JsonPropertyName("gpu_info")]
    public string? GpuInfo { get; set; }

    /// <summary>获取或设置网络类型。</summary>
    [JsonPropertyName("net_type")]
    public string? NetType { get; set; }

    /// <summary>获取或设置内存信息。</summary>
    [JsonPropertyName("memory_info")]
    public string? MemoryInfo { get; set; }

    /// <summary>获取或设置显示器刷新率（参数表标注 string，官方示例为数字，本模型以参数表为准）。</summary>
    [JsonPropertyName("monitor_frequency")]
    public string? MonitorFrequency { get; set; }

    /// <summary>获取或设置摄像头型号。</summary>
    [JsonPropertyName("camera_model")]
    public string? CameraModel { get; set; }

    /// <summary>获取或设置是否开启视频镜像：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_video_mirror")]
    public bool? EnableVideoMirror { get; set; }

    /// <summary>获取或设置麦克风信息。</summary>
    [JsonPropertyName("microphone_info")]
    public string? MicrophoneInfo { get; set; }

    /// <summary>获取或设置扬声器信息。</summary>
    [JsonPropertyName("speaker_info")]
    public string? SpeakerInfo { get; set; }
}
