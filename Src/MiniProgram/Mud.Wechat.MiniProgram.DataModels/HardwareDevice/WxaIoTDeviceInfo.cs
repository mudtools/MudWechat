// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

/// <summary>设备组设备信息（<c>device_list[]</c>）。</summary>
/// <remarks>硬件设备以 <c>sn</c>（设备序列号）与 <c>mac</c>（设备 MAC）标识；字段以官方页面为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTDeviceInfo
{
    /// <summary>设备序列号（<c>sn</c>）。</summary>
    [JsonPropertyName("sn")]
    public string? Sn { get; set; }

    /// <summary>设备 MAC 地址（<c>mac</c>）。</summary>
    [JsonPropertyName("mac")]
    public string? Mac { get; set; }
}
