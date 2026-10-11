// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「营销子域」（8 端点：买赠活动 activity/add、activity/del、activity/stop +
// 限时抢购 limiteddiscounttask/add、update、delete、stop、list/get）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。
//
// 官方文档矛盾照录：activity/del 的 activity_id 为 number、activity/stop 的 activity_id 为 string
// （原文即如此），SDK 分别建模为 long / string，不做统一。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>单套赠品内的赠品（官方 <c>gift_items</c> 数组元素，<c>product/activity/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityGiftItem
{
    /// <summary>获取或设置单套赠品内的赠品 id（官方 <c>gift_id</c>，必填）。</summary>
    [JsonPropertyName("gift_id")]
    public long GiftId { get; set; }

    /// <summary>获取或设置单套赠品内的赠品件数（官方 <c>give_num</c>，必填）。</summary>
    [JsonPropertyName("give_num")]
    public int GiveNum { get; set; }
}

/// <summary>赠品详情（官方 <c>gift_set</c> 对象，<c>product/activity/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityGiftSet
{
    /// <summary>获取或设置买赠活动的赠品总套数（官方 <c>gift_set_num</c>，必填）。</summary>
    [JsonPropertyName("gift_set_num")]
    public int GiftSetNum { get; set; }

    /// <summary>获取或设置单套赠品内的赠品（官方 <c>gift_items</c>，必填，至少 1 个）。</summary>
    [JsonPropertyName("gift_items")]
    public List<ChannelsGiftActivityGiftItem> GiftItems { get; set; } = new();
}

/// <summary>买赠活动主商品（官方 <c>main_products</c> 数组元素，<c>product/activity/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityMainProduct
{
    /// <summary>获取或设置买赠活动的主品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }
}

/// <summary>限领规则（官方 <c>receive_limit</c> 对象，<c>product/activity/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityReceiveLimit
{
    /// <summary>获取或设置是否限领（官方 <c>is_limited</c>，必填）。</summary>
    [JsonPropertyName("is_limited")]
    public bool IsLimited { get; set; }

    /// <summary>获取或设置限领套数（官方 <c>limit_num</c>，必填）。</summary>
    [JsonPropertyName("limit_num")]
    public int LimitNum { get; set; }
}

/// <summary>买赠活动详情（官方 <c>detail</c> 对象，<c>product/activity/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityDetail
{
    /// <summary>
    /// 获取或设置买赠活动生效场景（官方 <c>show_scene</c>，必填：0 全场景展示，1 仅直播间展示；
    /// 当前 Open API 仅支持传 0（全场景），传 1 可能无法生效）。
    /// </summary>
    [JsonPropertyName("show_scene")]
    public int ShowScene { get; set; }

    /// <summary>
    /// 获取或设置活动类型（官方 <c>activity_type</c>：0 赠品独享（默认），1 赠品分享，2 满赠门槛；不传时按 0 处理）。
    /// </summary>
    [JsonPropertyName("activity_type")]
    public int? ActivityType { get; set; }

    /// <summary>获取或设置满赠门槛类型（官方 <c>threshold_type</c>，<c>activity_type=2</c> 时必填：1 满金额，2 满件数）。</summary>
    [JsonPropertyName("threshold_type")]
    public int? ThresholdType { get; set; }

    /// <summary>
    /// 获取或设置满赠门槛金额（官方 <c>threshold_money</c>，单位：分；<c>threshold_type=1</c> 时必填；
    /// 须为 100 的整数倍（即整元），大于 0 且不超过 10000000（10 万元））。
    /// </summary>
    [JsonPropertyName("threshold_money")]
    public long? ThresholdMoney { get; set; }

    /// <summary>获取或设置满赠门槛件数（官方 <c>threshold_product_num</c>，<c>threshold_type=2</c> 时必填；须大于 1 且不超过 1000）。</summary>
    [JsonPropertyName("threshold_product_num")]
    public int? ThresholdProductNum { get; set; }

    /// <summary>
    /// 获取或设置限领规则（官方 <c>receive_limit</c>；赠品分享活动（<c>activity_type=1</c>）时建议传入；
    /// 赠品独享/满赠门槛活动可不传或传空对象）。
    /// </summary>
    [JsonPropertyName("receive_limit")]
    public ChannelsGiftActivityReceiveLimit? ReceiveLimit { get; set; }

    /// <summary>获取或设置买赠活动主商品列表（官方 <c>main_products</c>，必填：至少 1 个、最多 1000 个）。</summary>
    [JsonPropertyName("main_products")]
    public List<ChannelsGiftActivityMainProduct> MainProducts { get; set; } = new();

    /// <summary>
    /// 获取或设置赠品详情（官方 <c>gift_set</c>，必填：至少包含 1 个赠品；
    /// 赠品分享活动（<c>activity_type=1</c>）时只能配置 1 种赠品）。
    /// </summary>
    [JsonPropertyName("gift_set")]
    public ChannelsGiftActivityGiftSet GiftSet { get; set; } = new();
}

/// <summary>创建买赠活动（<c>product/activity/add</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + 请求体。</para>
/// <para>官方业务限制：① <c>start_time</c> 只能取大于等于当前时间的值，且距离当前时间不得超过 30 天；</para>
/// <para>② <c>end_time</c> 必须大于当前时间以及 <c>start_time</c>，且活动持续时间（<c>end_time - start_time</c>）
/// 需大于等于 10 分钟、小于等于 30 天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddGiftActivityRequest
{
    /// <summary>获取或设置买赠活动 id（官方 <c>activity_id</c>；如果不填，则自动生成）。</summary>
    [JsonPropertyName("activity_id")]
    public long? ActivityId { get; set; }

    /// <summary>获取或设置买赠活动标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置买赠活动开始时间（官方 <c>start_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置买赠活动结束时间（官方 <c>end_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置活动详情（官方 <c>detail</c>，必填）。</summary>
    [JsonPropertyName("detail")]
    public ChannelsGiftActivityDetail Detail { get; set; } = new();
}

/// <summary>创建买赠活动（<c>product/activity/add</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddGiftActivityResponse : ChannelsResponse
{
    /// <summary>获取或设置买赠活动 ID（官方 <c>activity_id</c>，创建成功后返回）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }
}

/// <summary>
/// 删除买赠活动（<c>product/activity/del</c>）请求体（<c>activity_id</c> 官方类型为 number）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityDelRequest
{
    /// <summary>获取或设置买赠活动 ID（官方 <c>activity_id</c>，必填）。</summary>
    [JsonPropertyName("activity_id")]
    public long ActivityId { get; set; }
}

/// <summary>
/// 停止买赠活动（<c>product/activity/stop</c>）请求体（<c>activity_id</c> 官方类型为 string，
/// 与 <c>activity/del</c> 的 number 不同，官方文档原文如此，SDK 不统一）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftActivityStopRequest
{
    /// <summary>获取或设置买赠活动 ID（官方 <c>activity_id</c>，必填）。</summary>
    [JsonPropertyName("activity_id")]
    public string ActivityId { get; set; } = string.Empty;
}

/// <summary>限时抢购 SKU（官方 <c>limited_discount_skus</c> 数组元素，<c>limiteddiscounttask/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsLimitedDiscountSku
{
    /// <summary>获取或设置参与抢购的商品 ID 下，不同规格（SKU）的商品信息（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 SKU 的抢购价格（官方 <c>sale_price</c>，必填，必须小于原价；原价为 1 分钱的商品无法创建抢购任务）。
    /// </summary>
    [JsonPropertyName("sale_price")]
    public long SalePrice { get; set; }

    /// <summary>获取或设置参与抢购的商品库存（官方 <c>sale_stock</c>，必填，必须小于等于现有库存）。</summary>
    [JsonPropertyName("sale_stock")]
    public long SaleStock { get; set; }
}

/// <summary>创建限时抢购任务（<c>product/limiteddiscounttask/add</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + 请求体。</para>
/// <para>
/// 官方业务限制：① <c>start_time</c> 只能取大于等于当前时间的值（允许有最多十分钟的误差），
/// 且距离当前时间不得超过一年（365 天）；② <c>end_time</c> 必须大于当前时间以及 <c>start_time</c>，
/// 且距离当前时间不得超过一年（365 天）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddLimitedDiscountTaskRequest
{
    /// <summary>获取或设置参与抢购的商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置限时抢购任务开始时间（官方 <c>start_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置限时抢购任务结束时间（官方 <c>end_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置限时抢购 SKU 列表（官方 <c>limited_discount_skus</c>，必填）。</summary>
    [JsonPropertyName("limited_discount_skus")]
    public List<ChannelsLimitedDiscountSku> LimitedDiscountSkus { get; set; } = new();

    /// <summary>获取或设置活动名称（官方 <c>title</c>，仅商家可见，最长 50 个字符）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置是否开启限购（官方 <c>is_limit_purchase</c>）。</summary>
    [JsonPropertyName("is_limit_purchase")]
    public bool? IsLimitPurchase { get; set; }

    /// <summary>获取或设置限购数量（官方 <c>limit_purchase_num</c>，<c>IsLimitPurchase</c> 为 true 时有效，需大于 0）。</summary>
    [JsonPropertyName("limit_purchase_num")]
    public long? LimitPurchaseNum { get; set; }
}

/// <summary>创建限时抢购任务（<c>product/limiteddiscounttask/add</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddLimitedDiscountTaskResponse : ChannelsResponse
{
    /// <summary>获取或设置限时抢购任务 ID（官方 <c>task_id</c>，创建成功后返回）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }
}

/// <summary>
/// 限时抢购任务 ID 请求体（官方 <c>limiteddiscounttask/delete</c> 与 <c>limiteddiscounttask/stop</c>
/// 请求体字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsLimitedDiscountTaskIdRequest
{
    /// <summary>获取或设置限时抢购任务 ID（官方 <c>task_id</c>，必填）。</summary>
    [JsonPropertyName("task_id")]
    public string TaskId { get; set; } = string.Empty;
}

/// <summary>更新限时抢购任务（<c>product/limiteddiscounttask/update</c>）的 SKU（官方 <c>limited_discount_skus</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsLimitedDiscountTaskSkuUpdate
{
    /// <summary>获取或设置 SKU 所属商品 ID（官方 <c>product_id</c>，必填，修改 SKU 时必传）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置 SKU ID（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置 SKU 的抢购价格（官方 <c>sale_price</c>，必填）。</summary>
    [JsonPropertyName("sale_price")]
    public long SalePrice { get; set; }

    /// <summary>获取或设置参与抢购的商品库存（官方 <c>sale_stock</c>，必填）。</summary>
    [JsonPropertyName("sale_stock")]
    public long SaleStock { get; set; }
}

/// <summary>更新限时抢购任务（<c>product/limiteddiscounttask/update</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；全字段必填（乐观锁校验语义：status 为当前活动状态快照）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateLimitedDiscountTaskRequest
{
    /// <summary>获取或设置限时抢购任务 ID（官方 <c>task_id</c>，必填）。</summary>
    [JsonPropertyName("task_id")]
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置当前活动状态（官方 <c>status</c>，必填，乐观锁校验：传入时校验与实际状态是否一致；
    /// 0 待开始，1 进行中）。
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置限时抢购任务开始时间（官方 <c>start_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置限时抢购任务结束时间（官方 <c>end_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>title</c>，必填，仅商家可见，最长 50 个字符）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置 SKU 抢购信息列表（官方 <c>limited_discount_skus</c>，必填；修改 SKU 时必须传入 product_id）。</summary>
    [JsonPropertyName("limited_discount_skus")]
    public List<ChannelsLimitedDiscountTaskSkuUpdate> LimitedDiscountSkus { get; set; } = new();
}

/// <summary>更新限时抢购任务（<c>product/limiteddiscounttask/update</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateLimitedDiscountTaskResponse : ChannelsResponse
{
    /// <summary>获取或设置限时抢购任务 ID（官方 <c>task_id</c>）。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

/// <summary>获取限时抢购任务列表（<c>product/limiteddiscounttask/list/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetLimitedDiscountTaskListRequest
{
    /// <summary>
    /// 获取或设置任务状态（官方 <c>status</c>，不填则获取所有状态：0 任务创建完成未开始（不能直接调用删除接口），
    /// 1 任务进行中（不能直接调用删除接口），2 任务已结束）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填：默认 10，不超过 50）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// 获取或设置翻页上下文（官方 <c>next_key</c>，由上次请求返回；传入时从上次返回的结果往后翻一页，
    /// 不传默认获取第一页数据）。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>限时抢购任务（官方 <c>limited_discount_tasks</c> 数组元素，<c>limiteddiscounttask/list/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsLimitedDiscountTask
{
    /// <summary>获取或设置限时抢购任务 ID（官方 <c>task_id</c>）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>获取或设置抢购商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置限时抢购任务状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置限时抢购任务创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置限时抢购任务开始时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置限时抢购任务结束时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置限时抢购 SKU（官方 <c>limited_discount_skus</c>）。</summary>
    [JsonPropertyName("limited_discount_skus")]
    public List<ChannelsLimitedDiscountSkuDetail>? LimitedDiscountSkus { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置限购数量（官方 <c>limit_purchase_num</c>，0 表示不限购）。</summary>
    [JsonPropertyName("limit_purchase_num")]
    public long? LimitPurchaseNum { get; set; }

    /// <summary>获取或设置活动支付 GMV（官方 <c>discount_pay_gmv</c>）。</summary>
    [JsonPropertyName("discount_pay_gmv")]
    public long? DiscountPayGmv { get; set; }

    /// <summary>获取或设置活动支付 UV（官方 <c>discount_pay_uv</c>）。</summary>
    [JsonPropertyName("discount_pay_uv")]
    public long? DiscountPayUv { get; set; }

    /// <summary>获取或设置活动支付订单数（官方 <c>discount_pay_order_cnt</c>）。</summary>
    [JsonPropertyName("discount_pay_order_cnt")]
    public long? DiscountPayOrderCnt { get; set; }
}

/// <summary>限时抢购 SKU 明细（官方 <c>limited_discount_skus</c> 数组元素，<c>limiteddiscounttask/list/get</c> 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsLimitedDiscountSkuDetail
{
    /// <summary>获取或设置限时抢购任务下不同的 sku_id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置该 sku_id 的抢购价格（官方 <c>sale_price</c>）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置该 sku_id 设置的抢购数量（官方 <c>sale_stock</c>）。</summary>
    [JsonPropertyName("sale_stock")]
    public long? SaleStock { get; set; }

    /// <summary>获取或设置 SKU 所属商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置 SKU 剩余抢购库存（官方 <c>remaining_stock</c>）。</summary>
    [JsonPropertyName("remaining_stock")]
    public long? RemainingStock { get; set; }
}

/// <summary>获取限时抢购任务列表（<c>product/limiteddiscounttask/list/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetLimitedDiscountTaskListResponse : ChannelsResponse
{
    /// <summary>获取或设置限时抢购信息（官方 <c>limited_discount_tasks</c>）。</summary>
    [JsonPropertyName("limited_discount_tasks")]
    public List<ChannelsLimitedDiscountTask>? LimitedDiscountTasks { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>next_key</c>，用于请求下一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}

// ---- 枚举常量 ---------------------------------------------------------------

/// <summary>限时抢购任务状态常量（官方 <c>status</c> 枚举值，<c>limiteddiscounttask</c>）。</summary>
public static class ChannelsLimitedDiscountTaskStatuses
{
    /// <summary>限时抢购任务创建完成，未开始（此时不能直接调用删除接口）。</summary>
    public const int Pending = 0;

    /// <summary>限时抢购任务进行中（此时不能直接调用删除接口）。</summary>
    public const int Running = 1;

    /// <summary>限时抢购任务已结束。</summary>
    public const int Finished = 2;
}

/// <summary>库存事件类型常量（官方 <c>op_type</c> 枚举值，<c>stock/getflow</c>）。</summary>
public static class ChannelsStockFlowOpTypes
{
    /// <summary>未知。</summary>
    public const int Unknown = 0;

    /// <summary>增加（API 操作）。</summary>
    public const int IncreaseOther = 1;

    /// <summary>减少。</summary>
    public const int DecreaseOther = 2;

    /// <summary>设置。</summary>
    public const int Set = 3;

    /// <summary>下单占用。</summary>
    public const int OrderLock = 4;

    /// <summary>退款/取消释放。</summary>
    public const int OrderUnlock = 5;

    /// <summary>分配（转出）。</summary>
    public const int MoveOut = 6;

    /// <summary>归还（转入）。</summary>
    public const int MoveIn = 7;
}

/// <summary>库存子类型常量（官方 <c>stock_sub_type</c> 枚举值，<c>stock/getflow</c>）。</summary>
public static class ChannelsStockSubTypes
{
    /// <summary>普通库存。</summary>
    public const int Normal = 0;

    /// <summary>限时抢购库存。</summary>
    public const int LimitedDiscount = 1;

    /// <summary>区域库存。</summary>
    public const int Warehouse = 2;

    /// <summary>直播预热/专享库存。</summary>
    public const int Exclusive = 3;

    /// <summary>达人专属计划营销库存。</summary>
    public const int FinderPlan = 4;
}