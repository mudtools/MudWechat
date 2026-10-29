// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>飞机票（修改方向，官方 <c>boarding_pass</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateBoardingPass
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置出发地（官方 <c>from</c>，选填）。</summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>获取或设置目的地（官方 <c>to</c>，选填）。</summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>获取或设置航班号（官方 <c>flight</c>，选填）。</summary>
    [JsonPropertyName("flight")]
    public string? Flight { get; set; }

    /// <summary>获取或设置登机口（官方 <c>gate</c>，选填）。</summary>
    [JsonPropertyName("gate")]
    public string? Gate { get; set; }

    /// <summary>获取或设置在线值机外链（官方 <c>check_in_url</c>，选填）。</summary>
    [JsonPropertyName("check_in_url")]
    public string? CheckInUrl { get; set; }

    /// <summary>获取或设置机型（官方 <c>air_model</c>，选填）。</summary>
    [JsonPropertyName("air_model")]
    public string? AirModel { get; set; }

    /// <summary>获取或设置起飞时间戳（官方 <c>departure_time</c>，秒，选填）。</summary>
    [JsonPropertyName("departure_time")]
    public long? DepartureTime { get; set; }

    /// <summary>获取或设置降落时间戳（官方 <c>landing_time</c>，秒，选填）。</summary>
    [JsonPropertyName("landing_time")]
    public long? LandingTime { get; set; }
}
