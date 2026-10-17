// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>佣金单的分佣信息（官方 <c>order_detail.commission_info</c>）。金额单位分、费率区间 <c>[0, 1000000]</c>（十万分比）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierOrderCommissionInfo
{
    /// <summary>获取或设置视频号达人信息（官方 <c>finder_info</c>）。</summary>
    [JsonPropertyName("finder_info")]
    public ChannelsLeagueHeadSupplierOrderFinderInfo? FinderInfo { get; set; }

    /// <summary>获取或设置服务费率（官方 <c>service_ratio</c>，区间 <c>[0, 1000000]</c>）。</summary>
    [JsonPropertyName("service_ratio")]
    public long? ServiceRatio { get; set; }

    /// <summary>获取或设置实际结算服务费（官方 <c>service_amount</c>，单位分；2026-04-23 修订描述）。</summary>
    [JsonPropertyName("service_amount")]
    public long? ServiceAmount { get; set; }

    /// <summary>获取或设置服务费结算时间（官方 <c>profit_sharding_suc_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("profit_sharding_suc_time")]
    public long? ProfitShardingSucTime { get; set; }

    /// <summary>获取或设置推广渠道（官方 <c>promotion_channel</c>，见 <see cref="ChannelsLeagueHeadSupplierPromotionChannels"/>）。</summary>
    [JsonPropertyName("promotion_channel")]
    public int? PromotionChannel { get; set; }

    /// <summary>获取或设置推客分佣信息（官方 <c>sharer_info</c>）。</summary>
    [JsonPropertyName("sharer_info")]
    public ChannelsLeagueHeadSupplierOrderSharerInfo? SharerInfo { get; set; }

    /// <summary>获取或设置新带货达人信息（官方 <c>talent_info</c>）。</summary>
    [JsonPropertyName("talent_info")]
    public ChannelsLeagueHeadSupplierOrderTalentInfo? TalentInfo { get; set; }

    /// <summary>获取或设置推客推广参数（官方 <c>promotion_info</c>，作为推客时的推广信息）。</summary>
    [JsonPropertyName("promotion_info")]
    public ChannelsLeagueHeadSupplierOrderPromotionInfo? PromotionInfo { get; set; }

    /// <summary>获取或设置帮卖机构分佣（官方 <c>sub_agency_info</c>）。</summary>
    [JsonPropertyName("sub_agency_info")]
    public ChannelsLeagueHeadSupplierOrderAgencyInfo? SubAgencyInfo { get; set; }

    /// <summary>获取或设置推客机构分佣（官方 <c>promoter_agency_info</c>）。</summary>
    [JsonPropertyName("promoter_agency_info")]
    public ChannelsLeagueHeadSupplierOrderAgencyInfo? PromoterAgencyInfo { get; set; }

    /// <summary>获取或设置供货机构分佣（官方 <c>head_agency_info</c>）。</summary>
    [JsonPropertyName("head_agency_info")]
    public ChannelsLeagueHeadSupplierOrderAgencyInfo? HeadAgencyInfo { get; set; }

    /// <summary>获取或设置联盟技术服务费金额（官方 <c>platform_amount</c>，单位分，2026-03-06 新增）。</summary>
    [JsonPropertyName("platform_amount")]
    public long? PlatformAmount { get; set; }

    /// <summary>获取或设置联盟技术服务费计算状态（官方 <c>platform_commission_status</c>，见 <see cref="ChannelsLeagueHeadSupplierPlatformCommissionStatuses"/>，2026-04-23 新增）。</summary>
    [JsonPropertyName("platform_commission_status")]
    public int? PlatformCommissionStatus { get; set; }

    /// <summary>获取或设置服务费总额（官方 <c>service_total_amount</c>，含技术服务费，2026-04-23 新增）。</summary>
    [JsonPropertyName("service_total_amount")]
    public long? ServiceTotalAmount { get; set; }
}
