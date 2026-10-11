// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后操作类」DTO（aftersale/acceptapply、aftersale/rejectapply、
// aftersale/acceptexchangereship、aftersale/rejectexchangereship、aftersale/merchantreship、
// aftersale/merchantupdatereshipexpress、aftersale/merchantupdateaftersale、aftersale/genaftersaleorder、
// aftersale/refundpricediff、aftersale/getexchangeableskulist、aftersale/applyvirtualtelnum、
// aftersale/handlefastexchangereceipt、aftersale/uploadrefundcertificate）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>同意售后（<c>aftersale/acceptapply</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAcceptAftersaleRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置同意退货时传入地址 id（官方 <c>address_id</c>，否则后续接口处理缺地址请求将报错）。</summary>
    [JsonPropertyName("address_id")]
    public string? AddressId { get; set; }

    /// <summary>
    /// 获取或设置同意类型（官方 <c>accept_type</c>）：1 用于退货退款或换货场景表示同意退货（不能用于仅退款或收到货同意退款）；
    /// 2 用于收到货或仅退款场景表示同意退款（不能用于换货或同意用户退货）；不填则按当前售后单状态及类型自动选择。
    /// </summary>
    [JsonPropertyName("accept_type")]
    public int? AcceptType { get; set; }
}

/// <summary>拒绝售后（<c>aftersale/rejectapply</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsRejectAftersaleRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置拒绝原因具体描述（官方 <c>reject_reason</c>，可使用默认描述或自定义）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>获取或设置拒绝原因枚举值（官方 <c>reject_reason_type</c>，必填）。</summary>
    [JsonPropertyName("reject_reason_type")]
    public int RejectReasonType { get; set; }
}

/// <summary>换货 / 补寄发货请求共用字段形态（<c>acceptexchangereship</c> / <c>merchantreship</c> / <c>merchantupdatereshipexpress</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleReshipRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置快递公司 id（官方 <c>delivery_id</c>，必填，经 获取快递公司列表 接口获得，非主流快递可填 OTHER）。
    /// </summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;
}

/// <summary>拒绝换货发货（<c>aftersale/rejectexchangereship</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsRejectExchangeReshipRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置拒绝原因具体描述（官方 <c>reject_reason</c>，可使用默认描述或自定义）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>获取或设置拒绝原因枚举值（官方 <c>reject_reason_type</c>）。</summary>
    [JsonPropertyName("reject_reason_type")]
    public int? RejectReasonType { get; set; }

    /// <summary>获取或设置退款凭证（官方 <c>reject_certificates</c>，经 图片上传接口 获取 media_id）。</summary>
    [JsonPropertyName("reject_certificates")]
    public List<string>? RejectCertificates { get; set; }
}

/// <summary>商家协商（<c>aftersale/merchantupdateaftersale</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMerchantUpdateAftersaleRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置协商修改后的售后类型（官方 <c>type</c>，必填）：1 退款；2 退货退款；3 换货；4 补寄。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置金额（官方 <c>amount</c>，单位分；换货 / 补寄时不填）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置协商描述（官方 <c>merchant_update_desc</c>，必填）。</summary>
    [JsonPropertyName("merchant_update_desc")]
    public string MerchantUpdateDesc { get; set; } = string.Empty;

    /// <summary>获取或设置协商原因（官方 <c>update_reason_type</c>，必填，枚举值见官方文档）。</summary>
    [JsonPropertyName("update_reason_type")]
    public int UpdateReasonType { get; set; }

    /// <summary>
    /// 获取或设置协商类型（官方 <c>merchant_update_type</c>，必填）：1 已协商一致邀请买家取消售后；2 邀请买家核实与补充凭证；3 修改买家售后申请。
    /// </summary>
    [JsonPropertyName("merchant_update_type")]
    public int MerchantUpdateType { get; set; }

    /// <summary>
    /// 获取或设置协商凭证 id 列表（官方 <c>media_ids</c>，必填；当 <c>update_reason_type</c> 对应 need_image 为 1 时必填）。
    /// </summary>
    [JsonPropertyName("media_ids")]
    public List<string> MediaIds { get; set; } = new();

    /// <summary>获取或设置商家指定新换的 sku（官方 <c>new_sku_id</c>，type=EXCHANGE 时必填）。</summary>
    [JsonPropertyName("new_sku_id")]
    public long? NewSkuId { get; set; }

    /// <summary>获取或设置商家指定新换的数量（官方 <c>product_cnt</c>，type=EXCHANGE 时必填，协商换货场景必须填 1）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}

/// <summary>换货信息（官方 <c>genaftersaleorder.exchange_sku_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleExchangeSkuInfo
{
    /// <summary>
    /// 获取或设置要换成的新 SKU ID（官方 <c>new_sku_id</c>，必须来自 <c>getexchangeableskulist</c> 返回的可换 SKU 列表）。
    /// </summary>
    [JsonPropertyName("new_sku_id")]
    public string NewSkuId { get; set; } = string.Empty;
}

/// <summary>商家代发起售后（<c>aftersale/genaftersaleorder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGenAftersaleOrderRequest
{
    /// <summary>获取或设置请求唯一 ID（官方 <c>request_id</c>，必填，失败可用相同 ID 重试避免重复发起）。</summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;

    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 SKU ID（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置发起售后数量（官方 <c>count</c>，必填，换货场景仅支持填 1）。</summary>
    [JsonPropertyName("count")]
    public long Count { get; set; }

    /// <summary>获取或设置售后退款金额（官方 <c>amount</c>，单位分；非换货场景必填）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置售后原因（官方 <c>reason</c>，必填，枚举值见 <see cref="ChannelsAftersaleReasons"/>）。</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>获取或设置售后类型（官方 <c>type</c>，必填，REFUND / RETURN / EXCHANGE / RESHIP）。</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>获取或设置退货地址 ID（官方 <c>address_id</c>，售后类型为退货退款时填，参考 获取地址列表）。</summary>
    [JsonPropertyName("address_id")]
    public string? AddressId { get; set; }

    /// <summary>获取或设置代发起售后补充说明（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>获取或设置换货信息（官方 <c>exchange_sku_info</c>，type=EXCHANGE 时必填）。</summary>
    [JsonPropertyName("exchange_sku_info")]
    public ChannelsAftersaleExchangeSkuInfo? ExchangeSkuInfo { get; set; }
}

/// <summary>商家代发起售后（<c>aftersale/genaftersaleorder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGenAftersaleOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置售后 ID（官方 <c>aftersale_id</c>）。</summary>
    [JsonPropertyName("aftersale_id")]
    public long? AftersaleId { get; set; }
}

/// <summary>代用户发起退差价（<c>aftersale/refundpricediff</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsRefundPriceDiffRequest
{
    /// <summary>获取或设置请求唯一 ID（官方 <c>request_id</c>，必填，失败可用相同 ID 重试避免重复发起）。</summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;

    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 SKU ID（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置售后退款金额（官方 <c>amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>获取或设置售后原因（官方 <c>reason</c>，必填，枚举值见 <see cref="ChannelsAftersaleReasons"/>）。</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>获取或设置代发起售后补充说明（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}

/// <summary>代用户发起退差价（<c>aftersale/refundpricediff</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsRefundPriceDiffResponse : ChannelsResponse
{
    /// <summary>获取或设置售后 ID（官方 <c>aftersale_id</c>）。</summary>
    [JsonPropertyName("aftersale_id")]
    public string? AftersaleId { get; set; }
}

/// <summary>获取可换 SKU 列表（<c>aftersale/getexchangeableskulist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetExchangeableSkuListRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 SKU ID（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;
}

/// <summary>获取可换 SKU 列表（<c>aftersale/getexchangeableskulist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetExchangeableSkuListResponse : ChannelsResponse
{
    /// <summary>获取或设置可换 SKU ID 列表（官方 <c>sku_id_list</c>）。</summary>
    [JsonPropertyName("sku_id_list")]
    public List<string>? SkuIdList { get; set; }
}

/// <summary>售后单兑换虚拟号（<c>aftersale/applyvirtualtelnum</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsApplyVirtualTelNumRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;
}

/// <summary>售后单兑换虚拟号（<c>aftersale/applyvirtualtelnum</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsApplyVirtualTelNumResponse : ChannelsResponse
{
    /// <summary>获取或设置虚拟号号码（官方 <c>virtual_tel_number</c>）。</summary>
    [JsonPropertyName("virtual_tel_number")]
    public string? VirtualTelNumber { get; set; }

    /// <summary>获取或设置虚拟号有效期（官方 <c>virtual_tel_expire_time</c>）。</summary>
    [JsonPropertyName("virtual_tel_expire_time")]
    public long? VirtualTelExpireTime { get; set; }
}

/// <summary>极速换货收货确认（<c>aftersale/handlefastexchangereceipt</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsHandleFastExchangeReceiptRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置处理动作（官方 <c>act</c>，必填）：1 同意；2 拒绝。</summary>
    [JsonPropertyName("act")]
    public int Act { get; set; }

    /// <summary>获取或设置拒绝原因具体描述（官方 <c>reject_reason</c>，可使用默认描述或自定义）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>获取或设置拒绝原因枚举值（官方 <c>reject_reason_type</c>，选择 reject_scene 为 7 的场景）。</summary>
    [JsonPropertyName("reject_reason_type")]
    public int? RejectReasonType { get; set; }

    /// <summary>获取或设置补充描述（官方 <c>merchant_text</c>）。</summary>
    [JsonPropertyName("merchant_text")]
    public string? MerchantText { get; set; }

    /// <summary>获取或设置举证材料（官方 <c>reject_confirm_exchange</c>）。</summary>
    [JsonPropertyName("reject_confirm_exchange")]
    public List<string>? RejectConfirmExchange { get; set; }
}

/// <summary>上传退款凭证（<c>aftersale/uploadrefundcertificate</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsUploadRefundCertificateRequest
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>，必填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string AfterSaleOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置退款凭证（官方 <c>refund_certificates</c>，必填，经 图片上传接口 获取 media_id）。</summary>
    [JsonPropertyName("refund_certificates")]
    public List<string> RefundCertificates { get; set; } = new();

    /// <summary>获取或设置描述（官方 <c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;
}
