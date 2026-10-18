// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 订单商品（官方 <c>ProductInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>金额单位分</b>；<c>sale_price</c> 与 <c>market_price</c> 为单价，<c>sku_cnt</c> 为购买数量。</para>
/// <para><b>售后计数</b>：<c>on_aftersale_sku_cnt</c> 为售后中数量、<c>finish_aftersale_sku_cnt</c> 为已完成售后数量。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderProductInfo
{
    /// <summary>商品 SPU ID（微信侧）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>外部商品 SPU ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>商品 SKU ID（微信侧）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>外部商品 SKU ID。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>SKU 小图。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>SKU 数量。</summary>
    [JsonPropertyName("sku_cnt")]
    public long? SkuCnt { get; set; }

    /// <summary>售卖价格，单位<b>分</b>。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>商品标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>SKU 属性列表。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<WxaProductOrderAttrInfo>? SkuAttrs { get; set; }

    /// <summary>正在售后 / 退款流程中的 SKU 数量。</summary>
    [JsonPropertyName("on_aftersale_sku_cnt")]
    public long? OnAftersaleSkuCnt { get; set; }

    /// <summary>完成售后 / 退款的 SKU 数量。</summary>
    [JsonPropertyName("finish_aftersale_sku_cnt")]
    public long? FinishAftersaleSkuCnt { get; set; }

    /// <summary>商品编码。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>市场价格，单位<b>分</b>。</summary>
    [JsonPropertyName("market_price")]
    public long? MarketPrice { get; set; }
}
