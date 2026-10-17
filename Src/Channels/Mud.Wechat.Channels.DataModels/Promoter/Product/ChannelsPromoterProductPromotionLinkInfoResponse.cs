// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取推客对某个商品的推广短链（<c>promoter/get_product_promotion_link_info</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterProductPromotionLinkInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置推广短链（官方 <c>short_link</c>）。</summary>
    [JsonPropertyName("short_link")]
    public string? ShortLink { get; set; }

    /// <summary>获取或设置 http 短链（官方 <c>http_short_link</c>，外部点击可跳转微信）。</summary>
    [JsonPropertyName("http_short_link")]
    public string? HttpShortLink { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }
}
