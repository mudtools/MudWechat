// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 设备健康信息对象（获取设备列表响应 <c>device_info_list.device_monitor_info</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsDeviceMonitorInfo
{
    /// <summary>获取或设置摄像头健康状态。</summary>
    [JsonPropertyName("camera_status")]
    public bool? CameraStatus { get; set; }

    /// <summary>获取或设置麦克风健康状态。</summary>
    [JsonPropertyName("microphone_status")]
    public bool? MicrophoneStatus { get; set; }

    /// <summary>获取或设置扬声器健康状态。</summary>
    [JsonPropertyName("speaker_status")]
    public bool? SpeakerStatus { get; set; }
}
