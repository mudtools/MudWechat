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
