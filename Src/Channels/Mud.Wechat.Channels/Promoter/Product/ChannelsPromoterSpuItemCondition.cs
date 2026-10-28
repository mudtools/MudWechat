// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>商品查询条件（官方 <c>spu_item_condition</c> 对象，<c>promoter/get_promote_product_list</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterSpuItemCondition
{
    /// <summary>获取或设置售卖价区间（官方 <c>selling_price_range</c>，单位：分）。</summary>
    [JsonPropertyName("selling_price_range")]
    public ChannelsPromoterPromotionRange? SellingPriceRange { get; set; }

    /// <summary>获取或设置月销量区间（官方 <c>monthly_sales_range</c>）。</summary>
    [JsonPropertyName("monthly_sales_range")]
    public ChannelsPromoterPromotionRange? MonthlySalesRange { get; set; }

    /// <summary>获取或设置保障标（官方 <c>flags</c>，取值见 <see cref="ChannelsPromoterProductFlags"/>）。</summary>
    [JsonPropertyName("flags")]
    public List<int>? Flags { get; set; }

    /// <summary>获取或设置服务费率区间（官方 <c>service_fee_rate_range</c>，单位：10 万分之）。</summary>
    [JsonPropertyName("service_fee_rate_range")]
    public ChannelsPromoterPromotionRange? ServiceFeeRateRange { get; set; }

    /// <summary>获取或设置佣金率区间（官方 <c>commission_rate_range</c>，单位：10 万分之）。</summary>
    [JsonPropertyName("commission_rate_range")]
    public ChannelsPromoterPromotionRange? CommissionRateRange { get; set; }

    /// <summary>获取或设置推广时间范围（官方 <c>promote_time_range</c>）。</summary>
    [JsonPropertyName("promote_time_range")]
    public ChannelsPromoterPromotionRange? PromoteTimeRange { get; set; }

    /// <summary>获取或设置店铺分范围（官方 <c>shop_score_range</c>）。</summary>
    [JsonPropertyName("shop_score_range")]
    public ChannelsPromoterPromotionRange? ShopScoreRange { get; set; }

    /// <summary>获取或设置好评率范围（官方 <c>good_evaluation_ratio_range</c>）。</summary>
    [JsonPropertyName("good_evaluation_ratio_range")]
    public ChannelsPromoterPromotionRange? GoodEvaluationRatioRange { get; set; }
}
