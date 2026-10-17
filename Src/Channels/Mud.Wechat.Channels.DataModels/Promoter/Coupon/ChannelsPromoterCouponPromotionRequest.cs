// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>券推广请求（官方 <c>promoter/get_coupon_promoter_share_link</c>、<c>promoter/get_coupon_qr_code</c>、<c>promoter/get_coupon_short_link</c> 三端点请求体字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCouponPromotionRequest
{
    /// <summary>获取或设置券 id（官方 <c>coupon_id</c>，必填）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置推客的 appid（官方 <c>sharer_appid</c>，不传时生成只带机构参数的推广参数/短链/二维码）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置来源机构（官方 <c>from_head_supplier_appid</c>，该来源机构通过 <c>promoter/set_coupon_to_headsupplier</c> 给予优惠券）。</summary>
    [JsonPropertyName("from_head_supplier_appid")]
    public string? FromHeadSupplierAppId { get; set; }
}
