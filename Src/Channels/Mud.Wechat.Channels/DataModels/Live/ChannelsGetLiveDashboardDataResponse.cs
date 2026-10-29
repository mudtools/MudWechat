// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>获取直播大屏数据（<c>getlivedata</c>）响应。</summary>
/// <remarks>
/// 官方大屏 JSON 结构层级深、部分实体官方未展开（仅给实体名或示例片段）；
/// 未展开处 SDK 以 <see cref="System.Text.Json.JsonElement"/> 保真透传，已展开处逐一建型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetLiveDashboardDataResponse : ChannelsResponse
{
    /// <summary>获取或设置追踪 ID（官方 <c>trace_id</c>，报 bug 时携带）。</summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    /// <summary>获取或设置直播大屏基础数据（官方 <c>live_dashboard_data</c>，对应实体 LiveDashboardData）。</summary>
    [JsonPropertyName("live_dashboard_data")]
    public ChannelsLiveDashboardData? LiveDashboardData { get; set; }

    /// <summary>获取或设置内容力数据（官方 <c>live_comparison_index</c>；在播时 ended 为空、关播后 on_air 为空）。</summary>
    [JsonPropertyName("live_comparison_index")]
    public ChannelsLiveComparisonIndex? LiveComparisonIndex { get; set; }

    /// <summary>获取或设置电商数据概要（官方 <c>live_ec_data_summary</c>；未列出的指标为未定义）。</summary>
    [JsonPropertyName("live_ec_data_summary")]
    public ChannelsLiveEcDataSummary? LiveEcDataSummary { get; set; }

    /// <summary>获取或设置电商转化力数据（官方 <c>live_ec_conversion_metric</c>）。</summary>
    [JsonPropertyName("live_ec_conversion_metric")]
    public ChannelsLiveEcConversionMetric? LiveEcConversionMetric { get; set; }

    /// <summary>获取或设置电商画像数据（官方 <c>live_ec_profile</c>）。</summary>
    [JsonPropertyName("live_ec_profile")]
    public ChannelsLiveEcProfile? LiveEcProfile { get; set; }

    /// <summary>获取或设置电商渠道分布（官方 <c>live_distribution_channel</c>）。</summary>
    [JsonPropertyName("live_distribution_channel")]
    public ChannelsLiveDistributionChannel? LiveDistributionChannel { get; set; }

    /// <summary>获取或设置电商商品数据（官方 <c>single_live_ec_spu_data_page_v2</c>）。</summary>
    [JsonPropertyName("single_live_ec_spu_data_page_v2")]
    public ChannelsLiveSpuDataPage? SingleLiveEcSpuDataPageV2 { get; set; }
}
