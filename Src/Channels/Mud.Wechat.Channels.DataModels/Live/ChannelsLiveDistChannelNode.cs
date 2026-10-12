// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>一级渠道来源节点（官方 <c>j</c> 对象，<c>dist_flow_type_stats</c> / <c>live_dist_channel_source_stats</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistChannelNode
{
    /// <summary>获取或设置渠道层级（官方 <c>level</c>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置渠道来源 id（官方 <c>source_channel_id</c>）。</summary>
    [JsonPropertyName("source_channel_id")]
    public long? SourceChannelId { get; set; }

    /// <summary>获取或设置二级渠道来源统计（官方 <c>sub_channel_source_stats</c>；参数表类型列为 object、官方示例为数组——按数组容器建模，矛盾照录）。</summary>
    [JsonPropertyName("sub_channel_source_stats")]
    public List<ChannelsLiveDistSubChannelNode>? SubChannelSourceStats { get; set; }

    /// <summary>获取或设置 GMV（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置 UV（官方 <c>uv</c>）。</summary>
    [JsonPropertyName("uv")]
    public long? Uv { get; set; }

    /// <summary>获取或设置 GPV（官方 <c>gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv_per_uv")]
    public long? GmvPerUv { get; set; }

    /// <summary>获取或设置 GMV 占比（官方 <c>gmv_ratio</c>）。</summary>
    [JsonPropertyName("gmv_ratio")]
    public double? GmvRatio { get; set; }

    /// <summary>获取或设置 UV 占比（官方 <c>uv_ratio</c>）。</summary>
    [JsonPropertyName("uv_ratio")]
    public double? UvRatio { get; set; }

    /// <summary>获取或设置渠道来源名称（官方 <c>source_channel_name</c>）。</summary>
    [JsonPropertyName("source_channel_name")]
    public string? SourceChannelName { get; set; }

    /// <summary>获取或设置 PV 占比（官方 <c>pv_ratio</c>）。</summary>
    [JsonPropertyName("pv_ratio")]
    public double? PvRatio { get; set; }
}
