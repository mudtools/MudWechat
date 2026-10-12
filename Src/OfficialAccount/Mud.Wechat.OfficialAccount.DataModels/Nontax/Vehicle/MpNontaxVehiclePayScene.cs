// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 车辆出场场景信息（官方 <c>scene_info</c>；payapply 形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehiclePayScene
{
    /// <summary>获取或设置入场时间（官方 <c>start_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置出场时间（官方 <c>end_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置充电时长（官方 <c>charging_time</c>，可选）。</summary>
    [JsonPropertyName("charging_time")]
    public int? ChargingTime { get; set; }

    /// <summary>获取或设置车牌号（官方 <c>plate_number</c>，必填）。</summary>
    [JsonPropertyName("plate_number")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>获取或设置车型（官方 <c>car_type</c>，可选）。</summary>
    [JsonPropertyName("car_type")]
    public string? CarType { get; set; }

    /// <summary>获取或设置停车场名称（官方 <c>parking_name</c>，可选）。</summary>
    [JsonPropertyName("parking_name")]
    public string? ParkingName { get; set; }

    /// <summary>获取或设置车位号（官方 <c>space_number</c>，可选）。</summary>
    [JsonPropertyName("space_number")]
    public string? SpaceNumber { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置入口名称（官方 <c>entrance_name</c>，可选）。</summary>
    [JsonPropertyName("entrance_name")]
    public string? EntranceName { get; set; }

    /// <summary>获取或设置出口名称（官方 <c>exit_name</c>，可选）。</summary>
    [JsonPropertyName("exit_name")]
    public string? ExitName { get; set; }

    /// <summary>获取或设置承载量（官方 <c>carrying_capacity</c>，可选）。</summary>
    [JsonPropertyName("carrying_capacity")]
    public string? CarryingCapacity { get; set; }

    /// <summary>获取或设置承载范围（官方 <c>carrying_range</c>，可选）。</summary>
    [JsonPropertyName("carrying_range")]
    public string? CarryingRange { get; set; }

    /// <summary>获取或设置渠道类型（官方 <c>channel_type</c>，可选）。</summary>
    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; set; }
}
