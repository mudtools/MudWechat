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
