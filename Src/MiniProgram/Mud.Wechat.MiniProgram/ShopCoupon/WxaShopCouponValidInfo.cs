// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 券有效期规则（官方 <c>coupon_info.valid_info</c>）。
/// </summary>
/// <remarks>
/// <para><c>valid_type</c> 决定必填集：1 固定区间必填 <c>start_time</c>+<c>end_time</c>；2 领取后 N 天必填 <c>valid_day_num</c>；3 领取后 N 秒必填 <c>valid_second</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponValidInfo
{
    /// <summary>有效期类型（1 固定区间 / 2 领取后 N 天 / 3 领取后 N 秒）。</summary>
    [JsonPropertyName("valid_type")]
    public int? ValidType { get; set; }

    /// <summary>领取后有效天数（<c>valid_type=2</c>）。</summary>
    [JsonPropertyName("valid_day_num")]
    public int? ValidDayNum { get; set; }

    /// <summary>领取后有效秒数（<c>valid_type=3</c>）。</summary>
    [JsonPropertyName("valid_second")]
    public long? ValidSecond { get; set; }

    /// <summary>固定区间开始时间（秒级时间戳，<c>valid_type=1</c>）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>固定区间结束时间（秒级时间戳，<c>valid_type=1</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
