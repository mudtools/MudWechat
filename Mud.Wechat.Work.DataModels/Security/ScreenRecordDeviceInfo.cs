// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 截屏/录屏记录的设备信息（<see cref="ScreenOperRecordItem.DeviceInfo"/>；windows/mac/mobile 按平台三选一返回）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class ScreenRecordDeviceInfo
{
    /// <summary>
    /// 获取或设置设备的平台类型：Windows / Mac / Mobile。
    /// </summary>
    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    /// <summary>
    /// 获取或设置设备唯一标识。
    /// </summary>
    [JsonPropertyName("device_code")]
    public string? DeviceCode { get; set; }

    /// <summary>
    /// 获取或设置设备型号。
    /// </summary>
    [JsonPropertyName("device_model")]
    public string? DeviceModel { get; set; }

    /// <summary>
    /// 获取或设置 Windows 设备信息（system 为 Windows 时返回）。
    /// </summary>
    [JsonPropertyName("windows")]
    public ScreenWindowsDevice? Windows { get; set; }

    /// <summary>
    /// 获取或设置 Mac 设备信息（system 为 Mac 时返回）。
    /// </summary>
    [JsonPropertyName("mac")]
    public ScreenMacDevice? Mac { get; set; }

    /// <summary>
    /// 获取或设置移动设备信息（system 为 Mobile 时返回）。
    /// </summary>
    [JsonPropertyName("mobile")]
    public ScreenMobileDevice? Mobile { get; set; }
}
