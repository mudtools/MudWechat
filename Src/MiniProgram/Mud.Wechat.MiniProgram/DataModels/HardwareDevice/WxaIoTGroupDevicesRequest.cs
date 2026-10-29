// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

/// <summary>设备组添加 / 删除设备请求体（<c>POST /wxa/business/group/{adddevice,removedevice}</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>hardware-device/api_addiotgroupdevice.html</c>、<c>api_removeiotgroupdevice.html</c>。</para>
/// <para>同一请求体两端点复用；删除时仅以 <c>sn</c> 定位亦可（<c>mac</c> 选填）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTGroupDevicesRequest
{
    /// <summary>设备组 ID（<c>group_id</c>，必填）。</summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>待操作的设备列表（<c>device_list</c>，必填），见 <see cref="WxaIoTDeviceInfo"/>。</summary>
    [JsonPropertyName("device_list")]
    public List<WxaIoTDeviceInfo>? DeviceList { get; set; }
}
