// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>分发给其他机构的优惠券信息（官方 <c>list</c> 数组项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCouponToHeadSupplierInfo
{
    /// <summary>获取或设置券 id（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置券的接收机构（官方 <c>to_head_supplier_appid</c>）。</summary>
    [JsonPropertyName("to_head_supplier_appid")]
    public string? ToHeadSupplierAppId { get; set; }

    /// <summary>获取或设置发放时间（官方 <c>give_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("give_time")]
    public long? GiveTime { get; set; }
}
