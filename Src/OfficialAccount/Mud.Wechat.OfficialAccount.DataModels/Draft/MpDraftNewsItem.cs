// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Draft;

/// <summary>草稿文章详情条目（官方 <c>news_item</c> 元素；draft/get 与 draft/batchget 两页同构 ⇒ 共用）。</summary>
/// <remarks>
/// <b>官方文档矛盾（照录）</b>：draft/get 响应示例含 <c>show_cover_pic</c> 字段但响应字段表无该行
/// ⇒ SDK 按字段表建模（不补 show_cover_pic；以官方字段表为权威）。
/// 相比永久素材 news_item（<see cref="Media.MpMaterialNewsItem"/>），本条目多 article_type/image_info/
/// cover_info/product_info/url（草稿临时链接），字段集不同 ⇒ 不与素材域共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftNewsItem
{
    /// <summary>获取或设置文章类型（官方 <c>article_type</c>：news / newspic）。</summary>
    [JsonPropertyName("article_type")]
    public string? ArticleType { get; set; }

    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置作者（官方 <c>author</c>）。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>获取或设置摘要（官方 <c>digest</c>）。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>获取或设置图文内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置原文地址（官方 <c>content_source_url</c>）。</summary>
    [JsonPropertyName("content_source_url")]
    public string? ContentSourceUrl { get; set; }

    /// <summary>获取或设置封面素材 id（官方 <c>thumb_media_id</c>）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>获取或设置是否打开评论（官方 <c>need_open_comment</c>）。</summary>
    [JsonPropertyName("need_open_comment")]
    public int? NeedOpenComment { get; set; }

    /// <summary>获取或设置是否仅粉丝可评论（官方 <c>only_fans_can_comment</c>）。</summary>
    [JsonPropertyName("only_fans_can_comment")]
    public int? OnlyFansCanComment { get; set; }

    /// <summary>获取或设置图片信息（官方 <c>image_info</c>；仅 newspic 携带）。</summary>
    [JsonPropertyName("image_info")]
    public MpDraftImageInfo? ImageInfo { get; set; }

    /// <summary>获取或设置封面信息（官方 <c>cover_info</c>）。</summary>
    [JsonPropertyName("cover_info")]
    public MpDraftCoverInfo? CoverInfo { get; set; }

    /// <summary>获取或设置商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public MpDraftProductInfo? ProductInfo { get; set; }

    /// <summary>获取或设置草稿的临时链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
