// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>电商渠道分布（官方 <c>live_distribution_channel</c> 对象）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：官方示例中 <c>dist_flow_type_stats</c> 为数组而参数表类型列为 object——
/// 容器形态按示例建模为列表、元素字段按参数表子表建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistributionChannel
{
    /// <summary>获取或设置按场景划分的渠道分析统计（官方 <c>live_dist_channel_source_by_scene_stats</c>）。</summary>
    [JsonPropertyName("live_dist_channel_source_by_scene_stats")]
    public ChannelsLiveDistSceneStats? LiveDistChannelSourceBySceneStats { get; set; }

    /// <summary>获取或设置按流量类型、渠道层级划分的渠道分析统计（官方 <c>live_dist_channel_source_stats</c>；与 <c>dist_flow_type_stats</c> 子表字段集一致 ⇒ 共用 <see cref="ChannelsLiveDistFlowTypeStats"/>）。</summary>
    [JsonPropertyName("live_dist_channel_source_stats")]
    public ChannelsLiveDistFlowTypeStats? LiveDistChannelSourceStats { get; set; }
}
