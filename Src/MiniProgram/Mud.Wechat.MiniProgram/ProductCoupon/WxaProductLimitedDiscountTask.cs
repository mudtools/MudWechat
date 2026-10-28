// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 限时抢购任务（<c>limited_discount_list[]</c>）。
/// </summary>
/// <remarks>
/// <para>时间为秒级时间戳（number），与优惠券域的字符串时间不同源。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductLimitedDiscountTask
{
    /// <summary>抢购任务 ID。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }

    /// <summary>抢购商品 ID。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>抢购任务状态。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>任务创建时间，秒级时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>任务开始时间，秒级时间戳。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>任务结束时间，秒级时间戳。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>参与抢购的 SKU 列表。</summary>
    [JsonPropertyName("limited_discount_sku_list")]
    public List<WxaProductLimitedDiscountSku>? LimitedDiscountSkuList { get; set; }
}
