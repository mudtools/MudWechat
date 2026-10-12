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
