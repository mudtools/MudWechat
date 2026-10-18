// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券有效期设置（请求侧 <c>valid_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方两页枚举值域冲突（照录）</b>：create 页 <c>valid_type</c> 为 1 指定时间 / 2 生效天数；
/// update 页为 0 指定时间 / 1 生效天数 ⇒ 本仓常量类同时登记两套并注明页面，调用方按所调端点取值。</para>
/// <para><c>start_time</c> 与 <c>valid_day_num</c> 二选一（按 <c>valid_type</c>）；填了 <c>start_time</c> 则
/// <c>end_time</c> 必填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponValidInfo
{
    /// <summary>有效期结束时间戳（valid_type=指定时间时必填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>有效期开始时间戳（与 valid_day_num 二选一）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>有效期天数（与 start_time 二选一）。</summary>
    [JsonPropertyName("valid_day_num")]
    public int? ValidDayNum { get; set; }

    /// <summary>有效期类型，见 <see cref="WxaProductCouponValidTypes"/>。</summary>
    [JsonPropertyName("valid_type")]
    public int? ValidType { get; set; }
}
