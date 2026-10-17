// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>商品信息（官方 <c>product.product_info</c> 对象，<c>promoter/get_promote_product_detail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterPromoteProductInfo
{
    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置副标题（官方 <c>sub_title</c>，添加时未录入则回包可能不含该字段）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>获取或设置主图（官方 <c>head_imgs</c>，多张，最多 9 张，每张不超过 2MB）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置商详信息（官方 <c>desc_info</c>）。</summary>
    [JsonPropertyName("desc_info")]
    public ChannelsPromoterPromoteProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置类目信息（官方 <c>cats</c>，旧三级类目树）。</summary>
    [JsonPropertyName("cats")]
    public List<ChannelsPromoterPromoteProductCat>? Cats { get; set; }

    /// <summary>获取或设置新类目树——类目信息（官方 <c>cats_v2</c>，多级类目树，过渡期与 <c>cats</c> 兼容并存）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsPromoterPromoteProductCat>? CatsV2 { get; set; }

    /// <summary>获取或设置 sku 信息（官方 <c>skus</c>）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsPromoterPromoteProductSku>? Skus { get; set; }

    /// <summary>获取或设置用于在小程序跳转小店场景添加商品时传递跟佣信息（官方 <c>product_promotion_link</c>）。</summary>
    [JsonPropertyName("product_promotion_link")]
    public string? ProductPromotionLink { get; set; }

    /// <summary>获取或设置商品的状态（官方 <c>status</c>，见 <see cref="ChannelsPromoterProductStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
