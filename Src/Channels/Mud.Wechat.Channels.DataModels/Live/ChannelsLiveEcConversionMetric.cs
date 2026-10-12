// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>电商转化力数据（官方 <c>live_ec_conversion_metric</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveEcConversionMetric
{
    /// <summary>获取或设置近 10 分钟转化率数据（官方 <c>recent_10_min_conversion</c>）。</summary>
    [JsonPropertyName("recent_10_min_conversion")]
    public ChannelsLiveConversionMetric? Recent10MinConversion { get; set; }

    /// <summary>获取或设置整场直播转化率数据（官方 <c>whole_live_conversion</c>）。</summary>
    [JsonPropertyName("whole_live_conversion")]
    public ChannelsLiveConversionMetric? WholeLiveConversion { get; set; }
}
