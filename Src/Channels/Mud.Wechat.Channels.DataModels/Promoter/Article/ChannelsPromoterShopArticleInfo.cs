// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>小店关联账号的文章（官方 <c>article_list</c> 数组项，<c>promoter/get_shop_mp_article_list</c>）。</summary>
/// <remarks>商品数组名官方为 <c>product_list</c>；带货者侧为 <c>product_info_list</c> ⇒ 两支数组项对象分建。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterShopArticleInfo
{
    /// <summary>获取或设置文章信息（官方 <c>article_info</c>）。</summary>
    [JsonPropertyName("article_info")]
    public ChannelsPromoterArticleInfo? ArticleInfo { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>product_list</c>）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsPromoterShopArticleProductInfo>? ProductList { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}
