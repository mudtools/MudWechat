// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 选品广场商品属性筛选条件（官方 <c>spu_item_condition</c> 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 六个区间字段官方字段集完全同形（均为 <c>min</c> / <c>max</c> 两个 number）⇒ 全部复用
/// <see cref="ChannelsLeagueHeadSupplierNumberRange"/>，不为每支单独建区间 DTO。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierSelectionProductCondition
{
    /// <summary>获取或设置售价区间（官方 <c>selling_price_range</c>，单位：分）。</summary>
    [JsonPropertyName("selling_price_range")]
    public ChannelsLeagueHeadSupplierNumberRange? SellingPriceRange { get; set; }

    /// <summary>获取或设置月销量区间（官方 <c>monthly_sales_range</c>，区间端点取值口径见 <see cref="ChannelsLeagueHeadSupplierMonthlySalesRanges"/>）。</summary>
    [JsonPropertyName("monthly_sales_range")]
    public ChannelsLeagueHeadSupplierNumberRange? MonthlySalesRange { get; set; }

    /// <summary>获取或设置保障标列表（官方 <c>flags</c>，取值见 <see cref="ChannelsLeagueHeadSupplierGuaranteeFlags"/>）。</summary>
    [JsonPropertyName("flags")]
    public List<int>? Flags { get; set; }

    /// <summary>获取或设置服务费率区间（官方 <c>service_fee_rate_range</c>，单位：10 万分之）。</summary>
    [JsonPropertyName("service_fee_rate_range")]
    public ChannelsLeagueHeadSupplierNumberRange? ServiceFeeRateRange { get; set; }

    /// <summary>获取或设置佣金率区间（官方 <c>commission_rate_range</c>，单位：10 万分之）。</summary>
    [JsonPropertyName("commission_rate_range")]
    public ChannelsLeagueHeadSupplierNumberRange? CommissionRateRange { get; set; }

    /// <summary>获取或设置推广时间范围（官方 <c>promote_time_range</c>，unix 时间戳）。</summary>
    [JsonPropertyName("promote_time_range")]
    public ChannelsLeagueHeadSupplierNumberRange? PromoteTimeRange { get; set; }

    /// <summary>获取或设置创建时间范围（官方 <c>create_time_range</c>，unix 时间戳；<b>与顶层 <c>keyword</c> 不能共用</b>）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsLeagueHeadSupplierNumberRange? CreateTimeRange { get; set; }
}
