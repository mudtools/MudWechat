// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// SKU 更新条目（官方 <c>skus</c> 数组元素，更新商品免审形态：带 <c>sku_id</c> 与 <c>is_delete</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductSkuUpdate
{
    /// <summary>获取或设置 sku_id（官方 <c>sku_id</c>，更新时必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 sku 编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置库存信息（官方 <c>stock_info</c>；供货商铺货商品（<c>supply_source == 1</c>）不支持修改库存）。</summary>
    [JsonPropertyName("stock_info")]
    public ChannelsStockInfo? StockInfo { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分，不超过 1000 万元）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置 sku 预售信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }

    /// <summary>获取或设置是否删除当前 sku（官方 <c>is_delete</c>）。</summary>
    [JsonPropertyName("is_delete")]
    public int? IsDelete { get; set; }

    /// <summary>获取或设置更新 sku 状态（官方 <c>status</c>：0 默认值，5 上架，11 下架）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
