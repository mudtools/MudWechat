// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>添加商品时的内嵌 SKU（<c>skus[]</c> 元素；官方允许不传，再经 <c>sku/batch_add</c> 单独补充）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSpuAddSku
{
    /// <summary>商家自定义商品 ID（<c>skus</c> 非空时必填），最多 32 字符。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>商家自定义 SKU ID（<c>skus</c> 非空时必填），最多 32 字符。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>SKU 小图。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>售卖价格，单位分，不超过 1000000000（<c>skus</c> 非空时必填）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>市场价格，单位分，须比 <c>sale_price</c> 大。</summary>
    [JsonPropertyName("market_price")]
    public long? MarketPrice { get; set; }

    /// <summary>库存（<c>skus</c> 非空时必填）。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }

    /// <summary>条形码。</summary>
    [JsonPropertyName("barcode")]
    public string? Barcode { get; set; }

    /// <summary>商品编码，最多 20 字符。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>销售属性列表（<c>skus</c> 非空时必填）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<WxaProductSkuAttr>? SkuAttrs { get; set; }
}
