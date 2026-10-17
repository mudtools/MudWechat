// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>优惠券有效信息（官方 <c>valid_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCouponValidInfo
{
    /// <summary>获取或设置优惠券有效期类型（官方 <c>valid_type</c>，1：指定时间范围生效 2：生效天数，见 <see cref="ChannelsPromoterCouponValidTypes"/>）。</summary>
    [JsonPropertyName("valid_type")]
    public int? ValidType { get; set; }

    /// <summary>获取或设置优惠券有效天数（官方 <c>valid_day_num</c>，<c>valid_type=2</c> 时才有意义）。</summary>
    [JsonPropertyName("valid_day_num")]
    public int? ValidDayNum { get; set; }

    /// <summary>获取或设置优惠券有效期开始时间（官方 <c>start_time</c>，<c>valid_type=1</c> 时才有意义）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置优惠券有效期结束时间（官方 <c>end_time</c>，<c>valid_type=1</c> 时才有意义）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
