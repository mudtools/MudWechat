// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DeviceData;

/// <summary>
/// 授权设备条目（官方 device_list 的 item）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DeviceData")]
public class DeviceDataAuthDevice
{
    /// <summary>获取或设置设备类型（官方 device_type，数字枚举）。</summary>
    [JsonPropertyName("device_type")]
    public int DeviceType { get; set; }

    /// <summary>获取或设置设备序列号（官方 device_sn，本域各端点定位设备的主键）。</summary>
    [JsonPropertyName("device_sn")]
    public string DeviceSerialNumber { get; set; } = string.Empty;

    /// <summary>获取或设置设备出厂型号（官方 model_name）。</summary>
    [JsonPropertyName("model_name")]
    public string? ModelName { get; set; }

    /// <summary>获取或设置设备出厂名（官方 default_name）。</summary>
    [JsonPropertyName("default_name")]
    public string? DefaultName { get; set; }

    /// <summary>获取或设置设备备注名（官方 remark_name）。</summary>
    [JsonPropertyName("remark_name")]
    public string? RemarkName { get; set; }
}
