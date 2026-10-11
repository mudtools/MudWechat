// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「库存子域」（4 端点：stock/get、stock/update、stock/batchget、stock/getflow）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>区域库存（官方 <c>warehouse_stocks</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsWarehouseStock
{
    /// <summary>获取或设置区域库存外部 id（官方 <c>out_warehouse_id</c>）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string? OutWarehouseId { get; set; }

    /// <summary>获取或设置区域库存数量（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }

    /// <summary>获取或设置区域库存的锁定库存（已下单未支付的库存，官方 <c>lock_stock</c>）数量。</summary>
    [JsonPropertyName("lock_stock")]
    public long? LockStock { get; set; }
}

/// <summary>获取库存（<c>product/stock/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetStockRequest
{
    /// <summary>获取或设置内部商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置内部 sku_id（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;
}

/// <summary>库存数据（官方 <c>data</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductStockData
{
    /// <summary>获取或设置限时抢购库存数量（官方 <c>limited_discount_stock_num</c>，已废弃，默认返回 0）。</summary>
    [JsonPropertyName("limited_discount_stock_num")]
    public long? LimitedDiscountStockNum { get; set; }

    /// <summary>获取或设置通用库存数量（官方 <c>normal_stock_num</c>）。</summary>
    [JsonPropertyName("normal_stock_num")]
    public long? NormalStockNum { get; set; }

    /// <summary>获取或设置库存总量（官方 <c>total_stock_num</c>：通用库存数量 + 区域库存总量）。</summary>
    [JsonPropertyName("total_stock_num")]
    public long? TotalStockNum { get; set; }

    /// <summary>获取或设置区域库存（官方 <c>warehouse_stocks</c>）。</summary>
    [JsonPropertyName("warehouse_stocks")]
    public List<ChannelsWarehouseStock>? WarehouseStocks { get; set; }
}

/// <summary>获取库存（<c>product/stock/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetStockResponse : ChannelsResponse
{
    /// <summary>获取或设置库存数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsProductStockData? Data { get; set; }
}

/// <summary>
/// 更新库存（<c>product/stock/update</c>）请求体（<c>product/gift/stock/update</c> 字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateStockRequest
{
    /// <summary>获取或设置内部商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置内部 sku_id（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置修改类型（官方 <c>diff_type</c>，必填：1 增加，2 减少，3 设置；
    /// 建议使用 1 或 2，不建议使用 3 —— 高并发场景可能出现预期外表现）。
    /// </summary>
    [JsonPropertyName("diff_type")]
    public int DiffType { get; set; }

    /// <summary>获取或设置增加、减少或者设置的库存值（官方 <c>num</c>，必填）。</summary>
    [JsonPropertyName("num")]
    public long Num { get; set; }
}

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

/// <summary>SPU 库存（官方 <c>spu_stock_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSpuStock
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置 SKU 库存（官方 <c>sku_stock</c>）。</summary>
    [JsonPropertyName("sku_stock")]
    public List<ChannelsSkuStock>? SkuStock { get; set; }
}

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

/// <summary>批量库存数据（官方 <c>data</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsBatchStockData
{
    /// <summary>获取或设置 SPU 库存（官方 <c>spu_stock_list</c>）。</summary>
    [JsonPropertyName("spu_stock_list")]
    public List<ChannelsSpuStock>? SpuStockList { get; set; }
}

/// <summary>批量获取库存（<c>product/stock/batchget</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsBatchGetStockResponse : ChannelsResponse
{
    /// <summary>获取或设置批量库存数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsBatchStockData? Data { get; set; }
}

/// <summary>获取库存流水（<c>product/stock/getflow</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetStockFlowRequest
{
    /// <summary>获取或设置内部商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置内部 sku_id（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public long SkuId { get; set; }

    /// <summary>获取或设置库存类型（官方 <c>stock_type</c>，必填；0 普通，1 达人专属计划营销库存）。</summary>
    [JsonPropertyName("stock_type")]
    public int StockType { get; set; }

    /// <summary>获取或设置达人的视频号 finder_id（官方 <c>finder_id</c>，<c>StockType==1</c> 时必填）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置时间范围开始时间戳（官方 <c>begin_time</c>，必填，秒级）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>获取或设置时间范围结束时间戳（官方 <c>end_time</c>，必填，秒级）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置库存事件类型（官方 <c>op_type_list</c>，选填）。</summary>
    [JsonPropertyName("op_type_list")]
    public List<int>? OpTypeList { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// 获取或设置翻页上下文（官方 <c>next_key</c>，由上次请求返回；传入时会从上次返回的结果往后翻一页，
    /// 不传默认获取第一页数据）。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置库存类型 id（官方 <c>stock_type_id</c>，<c>StockType</c> 不为 0 且不为 1 时必填）。</summary>
    [JsonPropertyName("stock_type_id")]
    public string? StockTypeId { get; set; }
}

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

/// <summary>库存流水数据（官方 <c>data</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsStockFlowData
{
    /// <summary>获取或设置本次翻页的上下文（官方 <c>next_key</c>，用于请求下一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置库存流水（官方 <c>stock_flow_info_list</c>）。</summary>
    [JsonPropertyName("stock_flow_info_list")]
    public List<ChannelsStockFlowInfo>? StockFlowInfoList { get; set; }
}

/// <summary>获取库存流水（<c>product/stock/getflow</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetStockFlowResponse : ChannelsResponse
{
    /// <summary>获取或设置库存流水数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsStockFlowData? Data { get; set; }
}