// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>罗盘达人版商品明细数据（官方 <c>data</c> 对象，官方类型均为 number）。</summary>
/// <remarks>金额字段单位为<b>分</b>；比率字段为转化率口径。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderProductData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }

    /// <summary>获取或设置下单金额（官方 <c>create_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("create_gmv")]
    public long? CreateGmv { get; set; }

    /// <summary>获取或设置下单订单数（官方 <c>create_cnt</c>）。</summary>
    [JsonPropertyName("create_cnt")]
    public int? CreateCnt { get; set; }

    /// <summary>获取或设置下单人数（官方 <c>create_uv</c>）。</summary>
    [JsonPropertyName("create_uv")]
    public int? CreateUv { get; set; }

    /// <summary>获取或设置下单件数（官方 <c>create_product_cnt</c>）。</summary>
    [JsonPropertyName("create_product_cnt")]
    public int? CreateProductCnt { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>pay_cnt</c>）。</summary>
    [JsonPropertyName("pay_cnt")]
    public int? PayCnt { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>）。</summary>
    [JsonPropertyName("pay_uv")]
    public int? PayUv { get; set; }

    /// <summary>获取或设置成交件数（官方 <c>pay_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_cnt")]
    public int? PayProductCnt { get; set; }

    /// <summary>获取或设置成交金额（剔除退款）（官方 <c>pure_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pure_pay_gmv")]
    public long? PurePayGmv { get; set; }

    /// <summary>获取或设置成交客单价（官方 <c>pay_gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv_per_uv")]
    public long? PayGmvPerUv { get; set; }

    /// <summary>获取或设置实际结算金额（官方 <c>actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("actual_commission")]
    public long? ActualCommission { get; set; }

    /// <summary>获取或设置预估佣金金额（官方 <c>predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("predict_commission")]
    public long? PredictCommission { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>product_click_uv</c>）。</summary>
    [JsonPropertyName("product_click_uv")]
    public int? ProductClickUv { get; set; }

    /// <summary>获取或设置商品点击次数（官方 <c>product_click_cnt</c>）。</summary>
    [JsonPropertyName("product_click_cnt")]
    public int? ProductClickCnt { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public long? PayRefundGmv { get; set; }

    /// <summary>获取或设置成交退款人数（官方 <c>pay_refund_uv</c>）。</summary>
    [JsonPropertyName("pay_refund_uv")]
    public int? PayRefundUv { get; set; }

    /// <summary>获取或设置成交退款率（官方 <c>pay_refund_ratio</c>）。</summary>
    [JsonPropertyName("pay_refund_ratio")]
    public double? PayRefundRatio { get; set; }

    /// <summary>获取或设置发货后成交退款率（官方 <c>pay_refund_after_send_ratio</c>）。</summary>
    [JsonPropertyName("pay_refund_after_send_ratio")]
    public double? PayRefundAfterSendRatio { get; set; }

    /// <summary>获取或设置成交退款订单数（官方 <c>pay_refund_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_cnt")]
    public int? PayRefundCnt { get; set; }

    /// <summary>获取或设置成交退款件数（官方 <c>pay_refund_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_product_cnt")]
    public int? PayRefundProductCnt { get; set; }

    /// <summary>获取或设置发货前成交退款率（官方 <c>pay_refund_before_send_ratio</c>）。</summary>
    [JsonPropertyName("pay_refund_before_send_ratio")]
    public double? PayRefundBeforeSendRatio { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("refund_gmv")]
    public long? RefundGmv { get; set; }

    /// <summary>获取或设置退款件数（官方 <c>refund_product_cnt</c>）。</summary>
    [JsonPropertyName("refund_product_cnt")]
    public int? RefundProductCnt { get; set; }

    /// <summary>获取或设置退款订单数（官方 <c>refund_cnt</c>）。</summary>
    [JsonPropertyName("refund_cnt")]
    public int? RefundCnt { get; set; }

    /// <summary>获取或设置退款人数（官方 <c>refund_uv</c>）。</summary>
    [JsonPropertyName("refund_uv")]
    public int? RefundUv { get; set; }
}
