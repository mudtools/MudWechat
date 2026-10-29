// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>
/// 商品详细数据（官方 <c>data</c> 对象，<c>getshopproductdata</c>，官方类型 string）。
/// </summary>
/// <remarks>
/// 金额字段单位均为<b>分</b>。<b>官方文档矛盾照录</b>：<c>pay_refund_ratio</c>/
/// <c>pay_refund_after_send_ratio</c>/<c>pay_refund_before_send_ratio</c> 与 <c>refund_gmv</c>
/// 官方类型列标注为 null（无类型）——比率按示例数值建模为 double、<c>refund_gmv</c> 按示例字符串建模为 string。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductDetailData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public string? PayGmv { get; set; }

    /// <summary>获取或设置下单金额（官方 <c>create_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("create_gmv")]
    public string? CreateGmv { get; set; }

    /// <summary>获取或设置下单订单数（官方 <c>create_cnt</c>）。</summary>
    [JsonPropertyName("create_cnt")]
    public string? CreateCnt { get; set; }

    /// <summary>获取或设置下单人数（官方 <c>create_uv</c>）。</summary>
    [JsonPropertyName("create_uv")]
    public string? CreateUv { get; set; }

    /// <summary>获取或设置下单件数（官方 <c>create_product_cnt</c>）。</summary>
    [JsonPropertyName("create_product_cnt")]
    public string? CreateProductCnt { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>pay_cnt</c>）。</summary>
    [JsonPropertyName("pay_cnt")]
    public string? PayCnt { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>）。</summary>
    [JsonPropertyName("pay_uv")]
    public string? PayUv { get; set; }

    /// <summary>获取或设置成交件数（官方 <c>pay_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_cnt")]
    public string? PayProductCnt { get; set; }

    /// <summary>获取或设置成交金额（剔除退款）（官方 <c>pure_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pure_pay_gmv")]
    public string? PurePayGmv { get; set; }

    /// <summary>获取或设置成交客单价（剔除退款）（官方 <c>pay_gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv_per_uv")]
    public string? PayGmvPerUv { get; set; }

    /// <summary>获取或设置实际结算金额（官方 <c>seller_actual_settle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("seller_actual_settle_amount")]
    public string? SellerActualSettleAmount { get; set; }

    /// <summary>获取或设置实际服务费金额（官方 <c>platform_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("platform_actual_commission")]
    public string? PlatformActualCommission { get; set; }

    /// <summary>获取或设置实际达人佣金支出（官方 <c>finderuin_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("finderuin_actual_commission")]
    public string? FinderuinActualCommission { get; set; }

    /// <summary>获取或设置实际团长佣金支出（官方 <c>captain_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("captain_actual_commission")]
    public string? CaptainActualCommission { get; set; }

    /// <summary>获取或设置预估结算金额（官方 <c>seller_predict_settle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("seller_predict_settle_amount")]
    public string? SellerPredictSettleAmount { get; set; }

    /// <summary>获取或设置预估服务费金额（官方 <c>platform_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("platform_predict_commission")]
    public string? PlatformPredictCommission { get; set; }

    /// <summary>获取或设置预估达人佣金支出（官方 <c>finderuin_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("finderuin_predict_commission")]
    public string? FinderuinPredictCommission { get; set; }

    /// <summary>获取或设置预估团长佣金支出（官方 <c>captain_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("captain_predict_commission")]
    public string? CaptainPredictCommission { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>product_click_uv</c>）。</summary>
    [JsonPropertyName("product_click_uv")]
    public string? ProductClickUv { get; set; }

    /// <summary>获取或设置商品点击次数（官方 <c>product_click_cnt</c>）。</summary>
    [JsonPropertyName("product_click_cnt")]
    public string? ProductClickCnt { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public string? PayRefundGmv { get; set; }

    /// <summary>获取或设置成交退款人数（官方 <c>pay_refund_uv</c>）。</summary>
    [JsonPropertyName("pay_refund_uv")]
    public string? PayRefundUv { get; set; }

    /// <summary>获取或设置成交退款率（官方 <c>pay_refund_ratio</c>，官方类型列标注 null、示例为数值——按 double 建模）。</summary>
    [JsonPropertyName("pay_refund_ratio")]
    public double? PayRefundRatio { get; set; }

    /// <summary>获取或设置发货后成交退款率（官方 <c>pay_refund_after_send_ratio</c>，官方类型列标注 null、示例为数值——按 double 建模）。</summary>
    [JsonPropertyName("pay_refund_after_send_ratio")]
    public double? PayRefundAfterSendRatio { get; set; }

    /// <summary>获取或设置成交退款订单数（官方 <c>pay_refund_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_cnt")]
    public string? PayRefundCnt { get; set; }

    /// <summary>获取或设置成交退款件数（官方 <c>pay_refund_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_product_cnt")]
    public string? PayRefundProductCnt { get; set; }

    /// <summary>获取或设置发货前成交退款率（官方 <c>pay_refund_before_send_ratio</c>，官方类型列标注 null、示例为数值——按 double 建模）。</summary>
    [JsonPropertyName("pay_refund_before_send_ratio")]
    public double? PayRefundBeforeSendRatio { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_gmv</c>，单位：分，官方类型列标注 null、示例为字符串——按 string 建模）。</summary>
    [JsonPropertyName("refund_gmv")]
    public string? RefundGmv { get; set; }

    /// <summary>获取或设置退款件数（官方 <c>refund_product_cnt</c>）。</summary>
    [JsonPropertyName("refund_product_cnt")]
    public string? RefundProductCnt { get; set; }

    /// <summary>获取或设置退款订单数（官方 <c>refund_cnt</c>）。</summary>
    [JsonPropertyName("refund_cnt")]
    public string? RefundCnt { get; set; }

    /// <summary>获取或设置退款人数（官方 <c>refund_uv</c>）。</summary>
    [JsonPropertyName("refund_uv")]
    public string? RefundUv { get; set; }
}
