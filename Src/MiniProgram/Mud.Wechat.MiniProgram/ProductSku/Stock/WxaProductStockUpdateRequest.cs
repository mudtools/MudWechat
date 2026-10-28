// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>更新库存请求（<c>POST /product/stock/update</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductStockUpdateRequest
{
    /// <summary>小商店内部商品 ID，与 <c>out_product_id</c> 二选一。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义商品 ID，与 <c>product_id</c> 二选一。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>小商店内部 SKU ID，与 <c>out_sku_id</c> 二选一。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>商家自定义 SKU ID，与 <c>sku_id</c> 二选一。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>更新方式（必填）：1 全量更新；2 增量更新，取值见 <see cref="WxaProductStockUpdateTypes"/>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>库存数量（必填）：全量更新时须大于 0；增量更新时正数增加、负数减少。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }
}
