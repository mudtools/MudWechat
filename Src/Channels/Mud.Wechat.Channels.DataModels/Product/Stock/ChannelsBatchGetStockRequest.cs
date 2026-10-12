// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「库存子域」（4 端点：stock/get、stock/update、stock/batchget、stock/getflow）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>批量获取库存（<c>product/stock/batchget</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsBatchGetStockRequest
{
    /// <summary>获取或设置商品 ID 列表（官方 <c>product_id</c>，必填，上限为 50）。</summary>
    [JsonPropertyName("product_id")]
    public List<string> ProductId { get; set; } = new();

    /// <summary>获取或设置库存类型（官方 <c>stock_type</c>，不填默认为 0；1=达人专属计划营销库存）。</summary>
    [JsonPropertyName("stock_type")]
    public int? StockType { get; set; }

    /// <summary>获取或设置达人的视频号 finder_id（官方 <c>finder_id</c>，<c>StockType==1</c> 时必填）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置库存类型 id（官方 <c>stock_type_id</c>，<c>StockType</c> 不为 0 且不为 1 时必填）。</summary>
    [JsonPropertyName("stock_type_id")]
    public string? StockTypeId { get; set; }
}
