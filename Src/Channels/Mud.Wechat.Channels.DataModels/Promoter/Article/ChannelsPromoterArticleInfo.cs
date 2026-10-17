// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>文章信息（官方 <c>article_info</c> 对象，<c>promoter/get_shop_mp_article_list</c> 与 <c>promoter/get_talent_biz_article_list</c> 字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterArticleInfo
{
    /// <summary>获取或设置文章标题（官方 <c>article_title</c>）。</summary>
    [JsonPropertyName("article_title")]
    public string? ArticleTitle { get; set; }

    /// <summary>获取或设置文章封面图（官方 <c>cover_image</c>）。</summary>
    [JsonPropertyName("cover_image")]
    public string? CoverImage { get; set; }

    /// <summary>获取或设置文章 url 内容（官方 <c>article_url</c>）。</summary>
    [JsonPropertyName("article_url")]
    public string? ArticleUrl { get; set; }

    /// <summary>获取或设置文章推广参数（官方 <c>promoter_share_link</c>）。</summary>
    [JsonPropertyName("promoter_share_link")]
    public string? PromoterShareLink { get; set; }
}
