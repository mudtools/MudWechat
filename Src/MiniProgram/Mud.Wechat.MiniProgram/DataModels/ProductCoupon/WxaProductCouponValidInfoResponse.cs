// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券有效期（响应侧，时间为字符串、<c>valid_type</c> 为符号字符串）。
/// </summary>
/// <remarks>
/// <para><b>同名不同型</b>：响应侧 <c>valid_type</c> 为
/// <c>COUPON_VALID_TYPE_TIME</c> / <c>COUPON_VALID_TYPE_DAY</c> 符号串，时间字段为字符串。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponValidInfoResponse
{
    /// <summary>有效期类型符号串，见 <see cref="WxaProductCouponValidTypeNames"/>。</summary>
    [JsonPropertyName("valid_type")]
    public string? ValidType { get; set; }

    /// <summary>有效天数（valid_type=DAY 时有意义）。</summary>
    [JsonPropertyName("valid_day_num")]
    public int? ValidDayNum { get; set; }

    /// <summary>有效期开始时间（valid_type=TIME 时有意义）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>有效期结束时间（valid_type=TIME 时有意义）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }
}
