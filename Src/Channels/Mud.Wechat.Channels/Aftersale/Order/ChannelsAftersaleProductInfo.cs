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
