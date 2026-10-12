// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>订单流水条目（官方 <c>data_list</c> 数组元素）。</summary>
/// <remarks>金额字段单位均为<b>分</b>；结算/时间字段带「(预计)」前缀的为预估口径（官方原文照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowItem
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>order_state</c>）。</summary>
    [JsonPropertyName("order_state")]
    public int? OrderState { get; set; }

    /// <summary>获取或设置订单结算状态（官方 <c>order_settle_state</c>）。</summary>
    [JsonPropertyName("order_settle_state")]
    public int? OrderSettleState { get; set; }

    /// <summary>获取或设置订单创建时间（官方 <c>order_create_time</c>，单位：秒）。</summary>
    [JsonPropertyName("order_create_time")]
    public long? OrderCreateTime { get; set; }

    /// <summary>获取或设置订单支付时间（官方 <c>order_paid_time</c>，单位：秒）。</summary>
    [JsonPropertyName("order_paid_time")]
    public long? OrderPaidTime { get; set; }

    /// <summary>获取或设置订单支付方式（官方 <c>order_pay_method</c>）。</summary>
    [JsonPropertyName("order_pay_method")]
    public int? OrderPayMethod { get; set; }

    /// <summary>获取或设置订单类型（官方 <c>order_type</c>）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置商户实收金额（官方 <c>mch_received_amount</c>，单位：分）。</summary>
    [JsonPropertyName("mch_received_amount")]
    public long? MchReceivedAmount { get; set; }

    /// <summary>获取或设置支出金额（官方 <c>expense_amount</c>，单位：分）。</summary>
    [JsonPropertyName("expense_amount")]
    public long? ExpenseAmount { get; set; }

    /// <summary>获取或设置（预计）结算金额（官方 <c>mch_settle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("mch_settle_amount")]
    public long? MchSettleAmount { get; set; }

    /// <summary>获取或设置（预计）商家货款结算时间（官方 <c>mch_settle_time</c>）。</summary>
    [JsonPropertyName("mch_settle_time")]
    public long? MchSettleTime { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>product_list</c>）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsOrderFlowProduct>? ProductList { get; set; }

    /// <summary>获取或设置商品总金额（官方 <c>product_total_amount</c>，单位：分）。</summary>
    [JsonPropertyName("product_total_amount")]
    public long? ProductTotalAmount { get; set; }

    /// <summary>获取或设置运费金额（官方 <c>freight_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_amount")]
    public long? FreightAmount { get; set; }

    /// <summary>获取或设置改价金额（官方 <c>change_down_price</c>，单位：分）。</summary>
    [JsonPropertyName("change_down_price")]
    public long? ChangeDownPrice { get; set; }

    /// <summary>获取或设置商户优惠金额（官方 <c>mch_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("mch_discount_amount")]
    public long? MchDiscountAmount { get; set; }

    /// <summary>获取或设置积分抵扣金额（官方 <c>score_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("score_discount_amount")]
    public long? ScoreDiscountAmount { get; set; }

    /// <summary>获取或设置用户实付金额（官方 <c>buyer_paid_amount</c>，单位：分）。</summary>
    [JsonPropertyName("buyer_paid_amount")]
    public long? BuyerPaidAmount { get; set; }

    /// <summary>获取或设置达人优惠金额（官方 <c>promoter_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_discount_amount")]
    public long? PromoterDiscountAmount { get; set; }

    /// <summary>获取或设置平台优惠金额（官方 <c>platform_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_discount_amount")]
    public long? PlatformDiscountAmount { get; set; }

    /// <summary>获取或设置国家补贴金额（官方 <c>national_subsidy_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("national_subsidy_discount_amount")]
    public long? NationalSubsidyDiscountAmount { get; set; }

    /// <summary>获取或设置补交运费（官方 <c>freight_make_up_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_make_up_amount")]
    public long? FreightMakeUpAmount { get; set; }

    /// <summary>获取或设置跨店优惠金额（官方 <c>cross_shop_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("cross_shop_discount_amount")]
    public long? CrossShopDiscountAmount { get; set; }

    /// <summary>获取或设置用户退款金额（官方 <c>buyer_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("buyer_refund_amount")]
    public long? BuyerRefundAmount { get; set; }

    /// <summary>获取或设置（预计）平台优惠退款金额（官方 <c>platform_discount_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_discount_refund_amount")]
    public long? PlatformDiscountRefundAmount { get; set; }

    /// <summary>获取或设置达人优惠退款金额（官方 <c>promoter_discount_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_discount_refund_amount")]
    public long? PromoterDiscountRefundAmount { get; set; }

    /// <summary>获取或设置原技术服务费（官方 <c>original_platform_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("original_platform_commission_amount")]
    public long? OriginalPlatformCommissionAmount { get; set; }

    /// <summary>获取或设置（预计）技术服务费（官方 <c>platform_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_commission_amount")]
    public long? PlatformCommissionAmount { get; set; }

    /// <summary>获取或设置运费险补贴减免技术服务费（官方 <c>freight_insurance_subsidy_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_subsidy_amount")]
    public long? FreightInsuranceSubsidyAmount { get; set; }

    /// <summary>获取或设置（预计）机构服务费（官方 <c>supplier_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("supplier_commission_amount")]
    public long? SupplierCommissionAmount { get; set; }

    /// <summary>获取或设置机构服务费结算状态（官方 <c>supplier_commission_settle_state</c>）。</summary>
    [JsonPropertyName("supplier_commission_settle_state")]
    public int? SupplierCommissionSettleState { get; set; }

    /// <summary>获取或设置（预计）达人服务费（官方 <c>promoter_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_commission_amount")]
    public long? PromoterCommissionAmount { get; set; }

    /// <summary>获取或设置达人服务费结算状态（官方 <c>promoter_commission_settle_state</c>）。</summary>
    [JsonPropertyName("promoter_commission_settle_state")]
    public int? PromoterCommissionSettleState { get; set; }

    /// <summary>获取或设置（预计）运费险金额（官方 <c>freight_insurance_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_amount")]
    public long? FreightInsuranceAmount { get; set; }

    /// <summary>获取或设置运费险结算状态（官方 <c>freight_insurance_settle_state</c>）。</summary>
    [JsonPropertyName("freight_insurance_settle_state")]
    public int? FreightInsuranceSettleState { get; set; }

    /// <summary>获取或设置运费险补缴他单金额（官方 <c>freight_insurance_make_up_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_make_up_amount")]
    public long? FreightInsuranceMakeUpAmount { get; set; }

    /// <summary>获取或设置运费险补缴他单订单 id 列表（官方 <c>freight_insurance_make_up_order_id_list</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_order_id_list")]
    public List<string>? FreightInsuranceMakeUpOrderIdList { get; set; }

    /// <summary>获取或设置（预计）技术服务费结算时间（官方 <c>platform_commission_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("platform_commission_settle_time")]
    public long? PlatformCommissionSettleTime { get; set; }

    /// <summary>获取或设置（预计）达人服务费结算时间（官方 <c>promoter_commission_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("promoter_commission_settle_time")]
    public long? PromoterCommissionSettleTime { get; set; }

    /// <summary>获取或设置（预计）机构服务费结算时间（官方 <c>supplier_commission_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("supplier_commission_settle_time")]
    public long? SupplierCommissionSettleTime { get; set; }

    /// <summary>获取或设置（预计）运费险结算时间（官方 <c>freight_insurance_settle_time</c>）。</summary>
    [JsonPropertyName("freight_insurance_settle_time")]
    public long? FreightInsuranceSettleTime { get; set; }

    /// <summary>获取或设置（预计）运费险补缴他单结算时间（官方 <c>freight_insurance_make_up_settle_time</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_settle_time")]
    public long? FreightInsuranceMakeUpSettleTime { get; set; }

    /// <summary>获取或设置预付运费退回金额（官方 <c>pre_freight_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("pre_freight_refund_amount")]
    public long? PreFreightRefundAmount { get; set; }

    /// <summary>获取或设置结算后支出（官方 <c>post_settlement_expense</c>）。</summary>
    [JsonPropertyName("post_settlement_expense")]
    public ChannelsOrderFlowPostSettlementExpense? PostSettlementExpense { get; set; }

    /// <summary>获取或设置结算前退款（官方 <c>refund_before_settlement</c>，单位：分）。</summary>
    [JsonPropertyName("refund_before_settlement")]
    public long? RefundBeforeSettlement { get; set; }

    /// <summary>获取或设置其他支出（官方 <c>other_expense_amount</c>，单位：分）。</summary>
    [JsonPropertyName("other_expense_amount")]
    public long? OtherExpenseAmount { get; set; }

    /// <summary>获取或设置平台服务费结算状态（官方 <c>platform_commission_settle_state</c>）。</summary>
    [JsonPropertyName("platform_commission_settle_state")]
    public int? PlatformCommissionSettleState { get; set; }

    /// <summary>获取或设置运费险补缴他单结算状态（官方 <c>freight_insurance_make_up_settle_state</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_settle_state")]
    public int? FreightInsuranceMakeUpSettleState { get; set; }

    /// <summary>获取或设置同城配送门店 id（官方 <c>intra_city_shop_id</c>）。</summary>
    [JsonPropertyName("intra_city_shop_id")]
    public long? IntraCityShopId { get; set; }

    /// <summary>获取或设置供货商平台结算信息（官方 <c>supplier_platform_commission</c>）。</summary>
    [JsonPropertyName("supplier_platform_commission")]
    public ChannelsOrderFlowSupplierPlatformCommission? SupplierPlatformCommission { get; set; }

    /// <summary>获取或设置首次商家货款结算时间（官方 <c>first_mch_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("first_mch_settle_time")]
    public long? FirstMchSettleTime { get; set; }
}
