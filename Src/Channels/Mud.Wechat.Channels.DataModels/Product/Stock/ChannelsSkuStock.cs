// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「库存子域」（4 端点：stock/get、stock/update、stock/batchget、stock/getflow）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>SKU 库存（官方 <c>sku_stock</c> 数组元素，<c>stock/batchget</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSkuStock
{
    /// <summary>获取或设置 skuID（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置普通/通用库存数量（官方 <c>normal_stock_num</c>）。</summary>
    [JsonPropertyName("normal_stock_num")]
    public long? NormalStockNum { get; set; }

    /// <summary>获取或设置限时抢购库存数量（官方 <c>limited_discount_stock_num</c>，已废弃，默认返回 0）。</summary>
    [JsonPropertyName("limited_discount_stock_num")]
    public long? LimitedDiscountStockNum { get; set; }

    /// <summary>获取或设置区域库存（官方 <c>warehouse_stocks</c>）。</summary>
    [JsonPropertyName("warehouse_stocks")]
    public List<ChannelsWarehouseStock>? WarehouseStocks { get; set; }

    /// <summary>获取或设置达人专属计划营销库存数量（官方 <c>finder_total_num</c>）。</summary>
    [JsonPropertyName("finder_total_num")]
    public long? FinderTotalNum { get; set; }

    /// <summary>
    /// 获取或设置库存总量（官方 <c>total_stock_num</c>：普通/通用库存数量 + 区域库存总量 + 直播预热/专享库存）。
    /// </summary>
    [JsonPropertyName("total_stock_num")]
    public long? TotalStockNum { get; set; }

    /// <summary>获取或设置直播预热专属库存（官方 <c>exclusive_num</c>）。</summary>
    [JsonPropertyName("exclusive_num")]
    public long? ExclusiveNum { get; set; }
}
