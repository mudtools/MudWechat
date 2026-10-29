// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>飞机票（查询方向，官方 <c>card.boarding_pass</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryBoardingPass
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置出发地（官方 <c>from</c>）。</summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>获取或设置目的地（官方 <c>to</c>）。</summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>获取或设置航班号（官方 <c>flight</c>）。</summary>
    [JsonPropertyName("flight")]
    public string? Flight { get; set; }

    /// <summary>获取或设置登机口（官方 <c>gate</c>）。</summary>
    [JsonPropertyName("gate")]
    public string? Gate { get; set; }

    /// <summary>获取或设置在线值机外链（官方 <c>check_in_url</c>）。</summary>
    [JsonPropertyName("check_in_url")]
    public string? CheckInUrl { get; set; }

    /// <summary>获取或设置机型（官方 <c>air_model</c>）。</summary>
    [JsonPropertyName("air_model")]
    public string? AirModel { get; set; }

    /// <summary>获取或设置起飞时间戳（官方 <c>departure_time</c>，秒）。</summary>
    [JsonPropertyName("departure_time")]
    public long DepartureTime { get; set; }

    /// <summary>获取或设置降落时间戳（官方 <c>landing_time</c>，秒）。</summary>
    [JsonPropertyName("landing_time")]
    public long LandingTime { get; set; }
}
