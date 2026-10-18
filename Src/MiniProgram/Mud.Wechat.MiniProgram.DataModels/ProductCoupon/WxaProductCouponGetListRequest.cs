// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 获取优惠券列表请求体（<c>POST /product/coupon/get_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>时间窗口必填</b>：<c>start_create_time</c> / <c>end_create_time</c> 均必填、格式
/// <c>yyyy-MM-dd HH:mm:ss</c>；<c>page_size</c> 官方上限 200。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponGetListRequest
{
    /// <summary>创建时间搜索开始（必填）。</summary>
    [JsonPropertyName("start_create_time")]
    public string? StartCreateTime { get; set; }

    /// <summary>创建时间搜索结束（必填）。</summary>
    [JsonPropertyName("end_create_time")]
    public string? EndCreateTime { get; set; }

    /// <summary>优惠券状态（必填），见 <see cref="WxaProductCouponStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>第几页（最小 1）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>每页数量（不超过 200）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}
