// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「库存子域」（4 端点：stock/get、stock/update、stock/batchget、stock/getflow）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>库存流水（官方 <c>stock_flow_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsStockFlowInfo
{
    /// <summary>获取或设置操作数量（官方 <c>amount</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置开始数量（官方 <c>beginning_amount</c>）。</summary>
    [JsonPropertyName("beginning_amount")]
    public long? BeginningAmount { get; set; }

    /// <summary>获取或设置结束数量（官方 <c>ending_amount</c>）。</summary>
    [JsonPropertyName("ending_amount")]
    public long? EndingAmount { get; set; }

    /// <summary>获取或设置本次开始结束数量的库存子类型（官方 <c>stock_sub_type</c>）。</summary>
    [JsonPropertyName("stock_sub_type")]
    public int? StockSubType { get; set; }

    /// <summary>获取或设置库存事件类型（官方 <c>op_type</c>）。</summary>
    [JsonPropertyName("op_type")]
    public int? OpType { get; set; }

    /// <summary>获取或设置流水发生时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置归还的源库存子类型（官方 <c>unmove_from_stock_sub_type</c>，仅对 <c>op_type=7</c> 生效）。</summary>
    [JsonPropertyName("unmove_from_stock_sub_type")]
    public int? UnmoveFromStockSubType { get; set; }

    /// <summary>获取或设置分配的目标库存子类型（官方 <c>move_to_stock_sub_type</c>，仅对 <c>op_type=6</c> 生效）。</summary>
    [JsonPropertyName("move_to_stock_sub_type")]
    public int? MoveToStockSubType { get; set; }

    /// <summary>
    /// 获取或设置操作来源（官方 <c>upload_source</c>，仅对 <c>op_type=1/2/3</c> 生效：
    /// 2=API（开发者调用），3=API（服务商代调用），5=手机端，6=web 端）。
    /// </summary>
    [JsonPropertyName("upload_source")]
    public int? UploadSource { get; set; }

    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，仅对 <c>op_type=4/5</c> 生效）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置区域仓库 id（官方 <c>out_warehouse_id</c>，仅对 <c>stock_sub_type=3</c> 生效）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string? OutWarehouseId { get; set; }

    /// <summary>获取或设置限时抢购任务 id（官方 <c>limited_discount_id</c>，仅对 <c>stock_sub_type=2</c> 生效）。</summary>
    [JsonPropertyName("limited_discount_id")]
    public long? LimitedDiscountId { get; set; }

    /// <summary>
    /// 获取或设置达人的视频号 finder_id（官方 <c>finder_id</c>，仅对 <c>move_to_stock_sub_type=4</c>
    /// 和 <c>unmove_from_stock_sub_type=4</c> 生效）。
    /// </summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }
}
