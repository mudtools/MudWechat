// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>按场景划分的渠道分析统计（官方 <c>live_dist_channel_source_by_scene_stats</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistSceneStats
{
    /// <summary>获取或设置场景类型（官方 <c>scene_type</c>，见 <see cref="ChannelsLiveDistributionSceneTypes"/>）。</summary>
    [JsonPropertyName("scene_type")]
    public int? SceneType { get; set; }

    /// <summary>获取或设置该场景下的渠道分析统计（官方 <c>dist_flow_type_stats</c>；参数表类型列为 object、官方示例为数组——按数组容器建模，矛盾照录）。</summary>
    [JsonPropertyName("dist_flow_type_stats")]
    public List<ChannelsLiveDistFlowTypeStats>? DistFlowTypeStats { get; set; }

    /// <summary>获取或设置指标值总数（官方 <c>metric_value_total</c>）。</summary>
    [JsonPropertyName("metric_value_total")]
    public long? MetricValueTotal { get; set; }

    /// <summary>获取或设置 GMV 总值（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置 UV（官方 <c>uv</c>）。</summary>
    [JsonPropertyName("uv")]
    public long? Uv { get; set; }

    /// <summary>获取或设置千次看播成交 GPV（官方 <c>gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv_per_uv")]
    public long? GmvPerUv { get; set; }

    /// <summary>获取或设置指标值（官方 <c>metric_value</c>）。</summary>
    [JsonPropertyName("metric_value")]
    public long? MetricValue { get; set; }

    /// <summary>获取或设置指标值占比（官方 <c>metric_value_ratio</c>）。</summary>
    [JsonPropertyName("metric_value_ratio")]
    public double? MetricValueRatio { get; set; }

    /// <summary>获取或设置 PV（官方 <c>pv</c>）。</summary>
    [JsonPropertyName("pv")]
    public long? Pv { get; set; }
}
