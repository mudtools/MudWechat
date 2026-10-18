// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 新增限时抢购任务请求体（<c>POST /product/limiteddiscount/add</c>）。
/// </summary>
/// <remarks>
/// <para><b>时间窗口</b>：<c>start_time</c> / <c>end_time</c> 为 Unix 秒级时间戳，
/// <c>end_time</c> 必须大于当前时间且大于 <c>start_time</c>（官方原文）。</para>
/// <para><b>一商品一任务</b>：同一 <c>product_id</c> 已有抢购任务时返回 110000。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductLimitedDiscountAddRequest
{
    /// <summary>参与抢购的商品 ID（必填）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>抢购任务开始时间，秒级时间戳（必填）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>抢购任务结束时间，秒级时间戳（必填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>参与抢购的 SKU 列表。</summary>
    [JsonPropertyName("limited_discount_sku_list")]
    public List<WxaProductLimitedDiscountSku>? LimitedDiscountSkuList { get; set; }
}
