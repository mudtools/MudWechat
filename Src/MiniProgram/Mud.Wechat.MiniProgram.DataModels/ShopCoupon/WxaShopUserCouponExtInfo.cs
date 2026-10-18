// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 用户券扩展信息（官方 <c>user_coupon.ext_info</c>）。
/// </summary>
/// <remarks>
/// <para>本页 <c>ext_info</c> 只有 <c>use_time</c> 一支键；整份覆盖式替换，未核销不填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopUserCouponExtInfo
{
    /// <summary>核销时间（秒级时间戳；未核销不填）。</summary>
    [JsonPropertyName("use_time")]
    public long? UseTime { get; set; }
}
