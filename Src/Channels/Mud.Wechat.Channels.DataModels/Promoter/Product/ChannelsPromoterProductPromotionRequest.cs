// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>推客商品推广请求（官方 <c>promoter/get_product_promotion_link_info</c> 与 <c>promoter/get_product_promotion_qrcode_info</c> 请求体字段集一致 ⇒ 共用）。</summary>
/// <remarks>本两支端点的参数表无 <c>custom_token</c>（仅 <c>promoter/get_promoter_single_product_promotion_info</c> 有）⇒ 与 <see cref="ChannelsPromoterGetSingleProductPromotionInfoRequest"/> 分建。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterProductPromotionRequest
{
    /// <summary>获取或设置推客 appid（官方 <c>sharer_appid</c>，和 <c>sharer_openid</c> 二选一）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置推客在小程序中的 openid（官方 <c>sharer_openid</c>，必须是机构绑定小程序的用户 openid，和 <c>sharer_appid</c> 二选一）。</summary>
    [JsonPropertyName("sharer_openid")]
    public string? SharerOpenId { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，使用该参数时需要传入 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品所属店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置商品短链（官方 <c>product_short_link</c>，和 <c>product_id</c> 二选一）。</summary>
    [JsonPropertyName("product_short_link")]
    public string? ProductShortLink { get; set; }

    /// <summary>获取或设置商品所属供货机构 appid（官方 <c>head_supplier_appid</c>）。</summary>
    [JsonPropertyName("head_supplier_appid")]
    public string? HeadSupplierAppId { get; set; }
}
