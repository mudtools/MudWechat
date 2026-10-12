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
