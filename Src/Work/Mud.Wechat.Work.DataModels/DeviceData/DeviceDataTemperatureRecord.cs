// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DeviceData;

/// <summary>
/// 温度检测数据条目（官方 temperature_data 的 items）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DeviceData")]
public class DeviceDataTemperatureRecord
{
    /// <summary>获取或设置成员类型（官方 user_type，数字枚举）。</summary>
    [JsonPropertyName("user_type")]
    public int UserType { get; set; }

    /// <summary>获取或设置成员的 OpenUserId（官方 open_userid）。</summary>
    [JsonPropertyName("open_userid")]
    public string OpenUserId { get; set; } = string.Empty;

    /// <summary>获取或设置设备的序列号（官方 device_sn）。</summary>
    [JsonPropertyName("device_sn")]
    public string DeviceSerialNumber { get; set; } = string.Empty;

    /// <summary>获取或设置上报时间戳（官方 timestamp，打卡侧同义字段名为 checkin_time）。</summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>获取或设置温度值（官方 temperature，官方以字符串承载而非数字）。</summary>
    [JsonPropertyName("temperature")]
    public string TemperatureString { get; set; } = string.Empty;

    /// <summary>获取或设置温度状态（官方 status，数字枚举）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}
