// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>分发优惠券给其他机构请求（官方 <c>promoter/set_coupon_to_headsupplier</c> 请求体）。</summary>
/// <remarks>官方契约：只能分发店铺设置给当前机构的定向券；设置后无法取消。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterSetCouponToHeadSupplierRequest
{
    /// <summary>获取或设置优惠券的接收机构 appid（官方 <c>to_head_supplier_appid</c>，必填）。</summary>
    [JsonPropertyName("to_head_supplier_appid")]
    public string? ToHeadSupplierAppId { get; set; }

    /// <summary>获取或设置优惠券 id（官方 <c>coupon_id</c>，必填；只能是商家给予本机构的定向券，见 <c>promoter/get_cooperative_coupon_list</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }
}
