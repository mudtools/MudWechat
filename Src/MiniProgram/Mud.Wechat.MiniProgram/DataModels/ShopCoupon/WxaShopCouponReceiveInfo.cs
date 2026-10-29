// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 领券信息（官方 <c>coupon_info.receive_info</c>）。
/// </summary>
/// <remarks>
/// <para><c>total_num</c> / <c>limit_num_one_person</c> 官方原文明写「只做展示」，微信侧不据此扣减或限领，真实记账走库存端点与领券回调。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponReceiveInfo
{
    /// <summary>领券开始时间（秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>领券结束时间（秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>个人限领数（官方：只做展示）。</summary>
    [JsonPropertyName("limit_num_one_person")]
    public int? LimitNumOnePerson { get; set; }

    /// <summary>总发放量（官方：只做展示）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}
