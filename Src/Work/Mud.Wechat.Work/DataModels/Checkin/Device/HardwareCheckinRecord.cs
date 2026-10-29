// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 设备打卡原始记录（获取设备打卡数据响应 <c>checkindata</c> 元素；仅 4 个字段，比获取打卡记录数据精简得多）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class HardwareCheckinRecord
{
    /// <summary>获取或设置用户 id。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置打卡时间（Unix 时间戳）。</summary>
    [JsonPropertyName("checkin_time")]
    public long? CheckinTime { get; set; }

    /// <summary>获取或设置打卡设备的 sn。</summary>
    [JsonPropertyName("device_sn")]
    public string? DeviceSn { get; set; }

    /// <summary>获取或设置打卡设备名。</summary>
    [JsonPropertyName("device_name")]
    public string? DeviceName { get; set; }
}
