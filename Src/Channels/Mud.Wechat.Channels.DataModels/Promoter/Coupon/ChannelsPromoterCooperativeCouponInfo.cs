// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>定向券信息（官方 <c>coupons</c> 数组项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCooperativeCouponInfo
{
    /// <summary>获取或设置券的 id（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置券的来源机构（官方 <c>from_head_supplier_appid</c>，仅在 <c>is_from_head_supplier</c> 为 true 时返回）。</summary>
    [JsonPropertyName("from_head_supplier_appid")]
    public string? FromHeadSupplierAppId { get; set; }
}
