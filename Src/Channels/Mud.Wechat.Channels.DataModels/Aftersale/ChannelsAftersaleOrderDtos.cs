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

/// <summary>售后单当前状态（官方 <c>after_sale_order.status</c> 枚举）。</summary>
public static class ChannelsAftersaleStatuses
{
    /// <summary>用户取消申请。</summary>
    public const string UserCanceld = "USER_CANCELD";

    /// <summary>商家受理中。</summary>
    public const string MerchantProcessing = "MERCHANT_PROCESSING";

    /// <summary>商家拒绝退款。</summary>
    public const string MerchantRejectRefund = "MERCHANT_REJECT_REFUND";

    /// <summary>商家拒绝退货退款。</summary>
    public const string MerchantRejectReturn = "MERCHANT_REJECT_RETURN";

    /// <summary>待买家退货。</summary>
    public const string UserWaitReturn = "USER_WAIT_RETURN";

    /// <summary>退货退款关闭。</summary>
    public const string ReturnClosed = "RETURN_CLOSED";

    /// <summary>待商家收货。</summary>
    public const string MerchantWaitReceipt = "MERCHANT_WAIT_RECEIPT";

    /// <summary>商家逾期未退款。</summary>
    public const string MerchantOverdueRefund = "MERCHANT_OVERDUE_REFUND";

    /// <summary>退款完成。</summary>
    public const string MerchantRefundSuccess = "MERCHANT_REFUND_SUCCESS";

    /// <summary>退货退款完成。</summary>
    public const string MerchantReturnSuccess = "MERCHANT_RETURN_SUCCESS";

    /// <summary>平台退款中。</summary>
    public const string PlatformRefunding = "PLATFORM_REFUNDING";

    /// <summary>平台退款失败。</summary>
    public const string PlatformRefundFail = "PLATFORM_REFUND_FAIL";

    /// <summary>待用户确认。</summary>
    public const string UserWaitConfirm = "USER_WAIT_CONFIRM";

    /// <summary>商家打款失败，客服关闭售后。</summary>
    public const string MerchantRefundRetryFail = "MERCHANT_REFUND_RETRY_FAIL";

    /// <summary>售后关闭。</summary>
    public const string MerchantFail = "MERCHANT_FAIL";

    /// <summary>待用户处理商家协商。</summary>
    public const string UserWaitConfirmUpdate = "USER_WAIT_CONFIRM_UPDATE";

    /// <summary>待用户处理商家代发起的售后申请。</summary>
    public const string UserWaitHandleMerchantAfterSale = "USER_WAIT_HANDLE_MERCHANT_AFTER_SALE";

    /// <summary>物流线上拦截中。</summary>
    public const string WaitPackageIntercept = "WAIT_PACKAGE_INTERCEPT";

    /// <summary>商家拒绝换货。</summary>
    public const string MerchantRejectExchange = "MERCHANT_REJECT_EXCHANGE";

    /// <summary>商家拒绝发货。</summary>
    public const string MerchantRejectReship = "MERCHANT_REJECT_RESHIP";

    /// <summary>待用户收货。</summary>
    public const string UserWaitReceipt = "USER_WAIT_RECEIPT";

    /// <summary>换货完成。</summary>
    public const string MerchantExchangeSuccess = "MERCHANT_EXCHANGE_SUCCESS";
}

/// <summary>售后原因（官方 <c>reason</c> / <c>reason_text</c> 枚举，全部定义见 获取全量售后原因）。</summary>
public static class ChannelsAftersaleReasons
{
    /// <summary>拍错/多拍。</summary>
    public const string IncorrectSelection = "INCORRECT_SELECTION";

    /// <summary>不想要了。</summary>
    public const string NoLongerWant = "NO_LONGER_WANT";

    /// <summary>无快递信息。</summary>
    public const string NoExpressInfo = "NO_EXPRESS_INFO";

    /// <summary>包裹为空。</summary>
    public const string EmptyPackage = "EMPTY_PACKAGE";

    /// <summary>已拒签包裹。</summary>
    public const string RejectReceivePackage = "REJECT_RECEIVE_PACKAGE";

    /// <summary>快递长时间未送达。</summary>
    public const string NotDeliveredTooLong = "NOT_DELIVERED_TOO_LONG";

    /// <summary>与商品描述不符。</summary>
    public const string NotMatchProductDesc = "NOT_MATCH_PRODUCT_DESC";

    /// <summary>质量问题。</summary>
    public const string QualityIssue = "QUALITY_ISSUE";

    /// <summary>卖家发错货。</summary>
    public const string SendWrongGoods = "SEND_WRONG_GOODS";

    /// <summary>三无产品。</summary>
    public const string ThreeNoProduct = "THREE_NO_PRODUCT";

    /// <summary>假冒产品。</summary>
    public const string FakeProduct = "FAKE_PRODUCT";

    /// <summary>七天无理由。</summary>
    public const string NoReason7Days = "NO_REASON_7_DAYS";

    /// <summary>平台代发起。</summary>
    public const string InitiateByPlatform = "INITIATE_BY_PLATFORM";

    /// <summary>其它。</summary>
    public const string Others = "OTHERS";
}

/// <summary>售后类型（官方 <c>type</c> 枚举）。</summary>
public static class ChannelsAftersaleTypes
{
    /// <summary>退款。</summary>
    public const string Refund = "REFUND";

    /// <summary>退货退款。</summary>
    public const string Return = "RETURN";

    /// <summary>换货。</summary>
    public const string Exchange = "EXCHANGE";

    /// <summary>补寄。</summary>
    public const string Reship = "RESHIP";
}

/// <summary>售后子类型（官方 <c>sub_type</c> 枚举）。</summary>
public static class ChannelsAftersaleSubTypes
{
    /// <summary>普通售后。</summary>
    public const string Default = "DEFAULT";

    /// <summary>退差价售后。</summary>
    public const string RefundPriceDiff = "REFUND_PRICE_DIFF";
}

/// <summary>获取售后单（<c>aftersale/getaftersaleorder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetAftersaleOrderRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;
}

/// <summary>获取售后单（<c>aftersale/getaftersaleorder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetAftersaleOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置售后单（官方 <c>after_sale_order</c>）。</summary>
    [JsonPropertyName("after_sale_order")]
    public ChannelsAftersaleOrderInfo? AfterSaleOrder { get; set; }
}

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

/// <summary>售后相关商品信息（官方 <c>after_sale_order.product_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleProductInfo
{
    /// <summary>获取或设置商品 spu id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置售后数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>获取或设置是否极速退款（官方 <c>fast_refund</c>）。</summary>
    [JsonPropertyName("fast_refund")]
    public bool? FastRefund { get; set; }

    /// <summary>获取或设置赠品信息（官方 <c>gift_product_list</c>）。</summary>
    [JsonPropertyName("gift_product_list")]
    public List<ChannelsAftersaleGiftProduct>? GiftProductList { get; set; }

    /// <summary>获取或设置商品 sku code（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置本地生活券退款明细（官方 <c>voucher_list</c>）。</summary>
    [JsonPropertyName("voucher_list")]
    public List<ChannelsAftersaleVoucher>? VoucherList { get; set; }
}

/// <summary>赠品信息（官方 <c>product_info.gift_product_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleGiftProduct
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置售后数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }
}

/// <summary>本地生活券退款明细（官方 <c>product_info.voucher_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleVoucher
{
    /// <summary>获取或设置本地券券码（官方 <c>vourcher_code</c>，券级别 key）。</summary>
    [JsonPropertyName("vourcher_code")]
    public string? VourcherCode { get; set; }

    /// <summary>获取或设置券退款金额（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置次卡次数明细（官方 <c>times_list</c>，仅次卡券有值，普通券为空）。</summary>
    [JsonPropertyName("times_list")]
    public List<ChannelsAftersaleVoucherTimes>? TimesList { get; set; }
}

/// <summary>次卡次数明细（官方 <c>voucher_list[].times_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleVoucherTimes
{
    /// <summary>获取或设置次卡 id（官方 <c>serial_id</c>，次级别 key）。</summary>
    [JsonPropertyName("serial_id")]
    public string? SerialId { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>退款详情（官方 <c>after_sale_order.refund_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleRefundInfo
{
    /// <summary>获取或设置退款金额（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置标明售后单退款直接原因（官方 <c>refund_reason</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("refund_reason")]
    public int? RefundReason { get; set; }

    /// <summary>获取或设置平台优惠退款金额（官方 <c>platform_discount_return_amount</c>，单位分）。</summary>
    [JsonPropertyName("platform_discount_return_amount")]
    public long? PlatformDiscountReturnAmount { get; set; }

    /// <summary>获取或设置是否使用运费险小额保障退款（官方 <c>is_low_price_insurance_refund</c>）。</summary>
    [JsonPropertyName("is_low_price_insurance_refund")]
    public bool? IsLowPriceInsuranceRefund { get; set; }

    /// <summary>获取或设置是否最终由运费险出资（官方 <c>is_final_refund_by_insurance</c>）。</summary>
    [JsonPropertyName("is_final_refund_by_insurance")]
    public bool? IsFinalRefundByInsurance { get; set; }
}

/// <summary>用户退货信息（官方 <c>after_sale_order.return_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleReturnInfo
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置物流公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置物流公司名称（官方 <c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }

    /// <summary>获取或设置退回方式（官方 <c>return_type</c>：0-无物流 1-自行寄回 2-上门取件）。</summary>
    [JsonPropertyName("return_type")]
    public int? ReturnType { get; set; }

    /// <summary>获取或设置退货地址信息（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsAftersaleAddressInfo? AddressInfo { get; set; }
}

/// <summary>退货地址信息（官方 <c>return_info.address_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleAddressInfo
{
    /// <summary>获取或设置收货人姓名（官方 <c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置邮编（官方 <c>postal_code</c>）。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>获取或设置国标收货地址第一级地址（官方 <c>province_name</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置国标收货地址第二级地址（官方 <c>city_name</c>）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置国标收货地址第三级地址（官方 <c>county_name</c>）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>获取或设置详细收货地址信息（官方 <c>detail_info</c>）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>获取或设置收货地址国家码（官方 <c>national_code</c>）。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>获取或设置收货人手机号码（官方 <c>tel_number</c>）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置门牌号（官方 <c>house_number</c>）。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }
}

/// <summary>商家上传的信息（官方 <c>after_sale_order.merchant_upload_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleMerchantUploadInfo
{
    /// <summary>获取或设置拒绝原因（官方 <c>reject_reason</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>获取或设置退款凭证（官方 <c>refund_certificates</c>）。</summary>
    [JsonPropertyName("refund_certificates")]
    public List<string>? RefundCertificates { get; set; }
}

/// <summary>微信支付退款的响应（官方 <c>after_sale_order.refund_resp</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleRefundResp
{
    /// <summary>获取或设置错误码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置状态码（官方 <c>ret</c>）。</summary>
    [JsonPropertyName("ret")]
    public int? Ret { get; set; }

    /// <summary>获取或设置描述（官方 <c>message</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

/// <summary>换货相关商品信息（官方 <c>after_sale_order.exchange_product_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsExchangeProductInfo
{
    /// <summary>获取或设置商品 spu id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }
}

/// <summary>换货相关物流信息（官方 <c>after_sale_order.exchange_delivery_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsExchangeDeliveryInfo
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置物流公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置物流公司名称（官方 <c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}

/// <summary>虚拟号码信息（官方 <c>after_sale_order.virtual_tel_num_info</c> 与纠纷单共用字段集）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsVirtualTelNumInfo
{
    /// <summary>获取或设置虚拟号号码（官方 <c>virtual_tel_number</c>）。</summary>
    [JsonPropertyName("virtual_tel_number")]
    public string? VirtualTelNumber { get; set; }

    /// <summary>获取或设置虚拟号有效期（官方 <c>virtual_tel_expire_time</c>）。</summary>
    [JsonPropertyName("virtual_tel_expire_time")]
    public long? VirtualTelExpireTime { get; set; }
}

/// <summary>售后详情（官方 <c>after_sale_order.details</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleDetail
{
    /// <summary>获取或设置售后类型（官方 <c>type</c>：1、2 见 获取全量售后原因）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置金额（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>商家发起协商信息（官方 <c>after_sale_order.merchant_update_detail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleMerchantUpdateDetail
{
    /// <summary>获取或设置协商修改后的售后类型（官方 <c>type</c>：1-退款；2-退货退款；3-换货；4-补寄）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置金额（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置协商描述（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}

/// <summary>换货字段（官方 <c>after_sale_order.exchange_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleExchangeInfo
{
    /// <summary>获取或设置商家指定新换的 sku（官方 <c>new_sku_id</c>）。</summary>
    [JsonPropertyName("new_sku_id")]
    public long? NewSkuId { get; set; }

    /// <summary>获取或设置商家指定新换的数量（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}

/// <summary>获取售后单列表（<c>aftersale/getaftersalelist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetAftersaleListRequest
{
    /// <summary>获取或设置售后单创建启始时间（官方 <c>begin_create_time</c>，与 end_create_time 成对，必选其一）。</summary>
    [JsonPropertyName("begin_create_time")]
    public long? BeginCreateTime { get; set; }

    /// <summary>获取或设置售后单创建结束时间（官方 <c>end_create_time</c>，减去 begin_create_time 不得大于 24 小时）。</summary>
    [JsonPropertyName("end_create_time")]
    public long? EndCreateTime { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>，从第二页开始传，来源于上一页的返回值）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置售后单更新起始时间（官方 <c>begin_update_time</c>，与 end_update_time 成对，必选其一）。</summary>
    [JsonPropertyName("begin_update_time")]
    public long? BeginUpdateTime { get; set; }

    /// <summary>获取或设置售后单更新结束时间（官方 <c>end_update_time</c>，减去 begin_update_time 不得大于 24 小时）。</summary>
    [JsonPropertyName("end_update_time")]
    public long? EndUpdateTime { get; set; }
}

/// <summary>获取售后单列表（<c>aftersale/getaftersalelist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetAftersaleListResponse : ChannelsResponse
{
    /// <summary>获取或设置售后单号列表（官方 <c>after_sale_order_id_list</c>）。</summary>
    [JsonPropertyName("after_sale_order_id_list")]
    public List<string>? AfterSaleOrderIdList { get; set; }

    /// <summary>获取或设置是否还有数据（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>售后原因（官方 <c>aftersale/reason/get</c> 的 <c>reason_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleReason
{
    /// <summary>获取或设置售后原因枚举（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置售后原因说明（官方 <c>reason_text</c>）。</summary>
    [JsonPropertyName("reason_text")]
    public string? ReasonText { get; set; }
}

/// <summary>获取全量售后原因（<c>aftersale/reason/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetAftersaleReasonResponse : ChannelsResponse
{
    /// <summary>获取或设置售后原因列表（官方 <c>reason_list</c>）。</summary>
    [JsonPropertyName("reason_list")]
    public List<ChannelsAftersaleReason>? ReasonList { get; set; }
}

/// <summary>售后拒绝原因（官方 <c>aftersale/rejectreason/get</c> 的 <c>reason_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleRejectReason
{
    /// <summary>获取或设置售后拒绝原因枚举（官方 <c>reject_reason_type</c>）。</summary>
    [JsonPropertyName("reject_reason_type")]
    public int? RejectReasonType { get; set; }

    /// <summary>获取或设置售后拒绝原因说明（官方 <c>reject_reason_type_text</c>）。</summary>
    [JsonPropertyName("reject_reason_type_text")]
    public string? RejectReasonTypeText { get; set; }

    /// <summary>获取或设置售后拒绝原因默认描述（官方 <c>reject_reason</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>获取或设置售后拒绝原因适用场景（官方 <c>reject_scene</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("reject_scene")]
    public int? RejectScene { get; set; }
}

/// <summary>获取拒绝售后原因（<c>aftersale/rejectreason/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetAftersaleRejectReasonResponse : ChannelsResponse
{
    /// <summary>获取或设置售后拒绝原因列表（官方 <c>reason_list</c>）。</summary>
    [JsonPropertyName("reason_list")]
    public List<ChannelsAftersaleRejectReason>? ReasonList { get; set; }
}