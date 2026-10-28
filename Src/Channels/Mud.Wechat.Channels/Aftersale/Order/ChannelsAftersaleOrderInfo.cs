// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后单主文档」DTO（aftersale/getaftersaleorder、aftersale/getaftersalelist、
// aftersale/reason/get、aftersale/rejectreason/get）+ 售后单状态 / 原因 / 类型常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>售后单结构（官方 <c>after_sale_order</c> 对象，<c>getaftersaleorder</c> 响应主载荷）。</summary>
/// <remarks>
/// 售后单状态枚举见 <see cref="ChannelsAftersaleStatuses"/>、退款原因枚举见 <see cref="ChannelsAftersaleReasons"/>、
/// 售后类型见 <see cref="ChannelsAftersaleTypes"/>、售后子类型见 <see cref="ChannelsAftersaleSubTypes"/>。
/// 嵌套对象内部字段以官方售后文档为准（reason 后续新增原因不再有字面含义，请参考 reason_text）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleOrderInfo
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string? AfterSaleOrderId { get; set; }

    /// <summary>获取或设置售后单当前状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsAftersaleStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置订单归属人身份标识（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置订单归属人在开放平台的唯一标识符（官方 <c>unionid</c>）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置礼物订单赠送者 openid（官方 <c>present_giver_openid</c>，仅送礼订单返回）。</summary>
    [JsonPropertyName("present_giver_openid")]
    public string? PresentGiverOpenId { get; set; }

    /// <summary>获取或设置礼物订单赠送者在开放平台的唯一标识符（官方 <c>present_giver_unionid</c>）。</summary>
    [JsonPropertyName("present_giver_unionid")]
    public string? PresentGiverUnionId { get; set; }

    /// <summary>获取或设置售后相关商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsAftersaleProductInfo? ProductInfo { get; set; }

    /// <summary>获取或设置退款详情（官方 <c>refund_info</c>）。</summary>
    [JsonPropertyName("refund_info")]
    public ChannelsAftersaleRefundInfo? RefundInfo { get; set; }

    /// <summary>获取或设置用户退货信息（官方 <c>return_info</c>）。</summary>
    [JsonPropertyName("return_info")]
    public ChannelsAftersaleReturnInfo? ReturnInfo { get; set; }

    /// <summary>获取或设置商家上传的信息（官方 <c>merchant_upload_info</c>）。</summary>
    [JsonPropertyName("merchant_upload_info")]
    public ChannelsAftersaleMerchantUploadInfo? MerchantUploadInfo { get; set; }

    /// <summary>获取或设置售后单创建时间戳（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置售后单更新时间戳（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置退款原因（官方 <c>reason</c>，后续新增的原因将不再有字面含义，请参考 reason_text）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置退款原因解释（官方 <c>reason_text</c>，全部定义见 获取全量售后原因）。</summary>
    [JsonPropertyName("reason_text")]
    public string? ReasonText { get; set; }

    /// <summary>获取或设置售后类型（官方 <c>type</c>，REFUND:退款；RETURN:退货退款；EXCHANGE:换货；RESHIP:补寄）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置纠纷 id（官方 <c>complaint_id</c>，可用于获取纠纷信息）。</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>，可用于获取订单）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置微信支付退款的响应（官方 <c>refund_resp</c>）。</summary>
    [JsonPropertyName("refund_resp")]
    public ChannelsAftersaleRefundResp? RefundResp { get; set; }

    /// <summary>获取或设置当前状态截止时间（官方 <c>deadline</c>，仅在待商家审核退款退货申请或收货期间返回，秒级时间戳）。</summary>
    [JsonPropertyName("deadline")]
    public long? Deadline { get; set; }

    /// <summary>获取或设置换货相关商品信息（官方 <c>exchange_product_info</c>）。</summary>
    [JsonPropertyName("exchange_product_info")]
    public ChannelsExchangeProductInfo? ExchangeProductInfo { get; set; }

    /// <summary>获取或设置换货相关物流信息（官方 <c>exchange_delivery_info</c>）。</summary>
    [JsonPropertyName("exchange_delivery_info")]
    public ChannelsExchangeDeliveryInfo? ExchangeDeliveryInfo { get; set; }

    /// <summary>获取或设置虚拟号码信息（官方 <c>virtual_tel_num_info</c>）。</summary>
    [JsonPropertyName("virtual_tel_num_info")]
    public ChannelsVirtualTelNumInfo? VirtualTelNumInfo { get; set; }

    /// <summary>获取或设置商责额外赔付（官方 <c>compensation_liability_amount</c>，单位为分）。</summary>
    [JsonPropertyName("compensation_liability_amount")]
    public long? CompensationLiabilityAmount { get; set; }

    /// <summary>获取或设置售后详情（官方 <c>details</c>）。</summary>
    [JsonPropertyName("details")]
    public List<ChannelsAftersaleDetail>? Details { get; set; }

    /// <summary>获取或设置售后子类型（官方 <c>sub_type</c>，DEFAULT:普通售后；REFUND_PRICE_DIFF:退差价售后）。</summary>
    [JsonPropertyName("sub_type")]
    public string? SubType { get; set; }

    /// <summary>获取或设置商家发起协商信息（官方 <c>merchant_update_detail</c>，仅在 [待用户处理商家协商] 状态返回）。</summary>
    [JsonPropertyName("merchant_update_detail")]
    public ChannelsAftersaleMerchantUpdateDetail? MerchantUpdateDetail { get; set; }

    /// <summary>获取或设置售后完结秒级时间戳（官方 <c>complete_time</c>，售后完结后有效）。</summary>
    [JsonPropertyName("complete_time")]
    public long? CompleteTime { get; set; }

    /// <summary>获取或设置是否需要线下退款（官方 <c>need_offline_refund</c>，仅在售后状态为 [PLATFORM_REFUND_FAIL] 有效）。</summary>
    [JsonPropertyName("need_offline_refund")]
    public bool? NeedOfflineRefund { get; set; }

    /// <summary>获取或设置换货字段（官方 <c>exchange_info</c>）。</summary>
    [JsonPropertyName("exchange_info")]
    public ChannelsAftersaleExchangeInfo? ExchangeInfo { get; set; }

    /// <summary>获取或设置售后单本地生活类型（官方 <c>aftersale_voucher_type</c>：1=未核销 2=已预约 3=已核销；非券售后不设置）。</summary>
    [JsonPropertyName("aftersale_voucher_type")]
    public int? AftersaleVoucherType { get; set; }
}
