// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品 SKU（官方 <c>skus</c> 数组元素，添加商品形态：<c>out_sku_id</c> / <c>thumb_img</c> / <c>sale_price</c> / <c>stock_num</c> / …）。
/// </summary>
/// <remarks>SKU 长度最少 1、最多 500；价格单位：分，不超过 1000000000（1000 万元）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductSku
{
    /// <summary>获取或设置商家自定义 sku_id（官方 <c>out_sku_id</c>，最多 128 字符；添加成功后无法修改）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分，不超过 1000 万元）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置库存（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public int StockNum { get; set; }

    /// <summary>获取或设置 sku 编码（官方 <c>sku_code</c>，商家自定义编码，最多 100 字符）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置 sku 条形码（官方 <c>bar_code</c>）。</summary>
    [JsonPropertyName("bar_code")]
    public string? BarCode { get; set; }

    /// <summary>获取或设置规格列表（官方 <c>sku_attrs</c>）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<ChannelsSkuAttr>? SkuAttrs { get; set; }

    /// <summary>获取或设置 sku 预售信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }
}
