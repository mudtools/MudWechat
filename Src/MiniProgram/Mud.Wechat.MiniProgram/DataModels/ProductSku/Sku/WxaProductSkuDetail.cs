// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>SKU 详情（<c>sku/get</c> 的 <c>data</c> 与 <c>sku/get_list</c> 的 <c>skus[]</c> 元素同形）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSkuDetail
{
    /// <summary>小商店内部商品 ID。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义商品 ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>小商店内部 SKU ID。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>商家自定义 SKU ID（字符串）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>SKU 小图（须为换取后的 <c>temp_img_url</c>，不接受外链）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>售卖价格，以分为单位。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>市场价格，以分为单位。</summary>
    [JsonPropertyName("market_price")]
    public long? MarketPrice { get; set; }

    /// <summary>库存。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }

    /// <summary>商品编码。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>条形码。</summary>
    [JsonPropertyName("barcode")]
    public string? Barcode { get; set; }

    /// <summary>SKU 状态（5 上架中 / 21 假删除；<c>get_list</c> 侧另有 0 未上架）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>销售属性列表。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<WxaProductSkuAttr>? SkuAttrs { get; set; }
}
