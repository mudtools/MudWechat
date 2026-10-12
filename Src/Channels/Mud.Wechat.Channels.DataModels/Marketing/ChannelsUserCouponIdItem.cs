// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>用户优惠券 ID 条目（官方 <c>user_coupon_list</c> 数组元素，getusercouponlist 响应）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：官方参数表将 <c>user_coupon_id</c>/<c>coupon_id</c> 类型标注为 number，
/// 官方示例为字符串（<c>"109694341"</c> / <c>"123456789"</c>）——按参数表（number）建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsUserCouponIdItem
{
    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>；参数表类型 number，示例为字符串——矛盾照录）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public long? UserCouponId { get; set; }

    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>；参数表类型 number，示例为字符串——矛盾照录）。</summary>
    [JsonPropertyName("coupon_id")]
    public long? CouponId { get; set; }
}
