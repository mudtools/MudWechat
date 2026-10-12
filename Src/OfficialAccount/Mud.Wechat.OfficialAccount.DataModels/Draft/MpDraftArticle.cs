// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Draft;

/// <summary>
/// 草稿文章条目（官方 <c>articles</c> 元素；draft_add 页字段表为权威，draft_update 页单对象同字段集）。
/// </summary>
/// <remarks>
/// <para>
/// <b>双形态（官方两页不一致，照录）</b>：draft_add 页 <c>articles</c> 为 objarray（数组），
/// draft_update 页 <c>articles</c> 为 object（单对象）——SDK 按各自页面建模（数组 / 单对象两个请求 DTO）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：content 字段同句自相矛盾——「大小不可超过 2kb」vs「必须少于 2 万字符，
/// 小于 1M」。约束：图片 url 必须来源「上传图文消息内的图片获取URL」接口（外部图片 url 将被过滤）。
/// </para>
/// <para>
/// <b>「8 条上限」不存在</b>：2026-10-07 核验官方页面（含指南页）无「图文上限 8 条」表述
/// （仅第三方教程转述）——SDK 不编造上限，也不做本地拦截。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftArticle
{
    /// <summary>获取或设置文章类型（官方 <c>article_type</c>：news 图文消息（默认）/ newspic 图片消息）。</summary>
    [JsonPropertyName("article_type")]
    public string? ArticleType { get; set; }

    /// <summary>获取或设置标题（官方 <c>title</c>，必填；总长度不超过 32 个字；不要使用 Unicode 转义格式）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置作者（官方 <c>author</c>，总长度不超过 16 个字）。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>获取或设置摘要（官方 <c>digest</c>，≤120 字；仅单图文有摘要；未填默认抓取正文前 54 个字）。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>获取或设置图文内容（官方 <c>content</c>，必填；支持 HTML 标签；图片 url 须经 uploadimg 接口获取，外部 url 被过滤）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>获取或设置图文原文地址（官方 <c>content_source_url</c>，「阅读原文」；大小不可超过 1kb）。</summary>
    [JsonPropertyName("content_source_url")]
    public string? ContentSourceUrl { get; set; }

    /// <summary>获取或设置封面图片素材 id（官方 <c>thumb_media_id</c>；article_type = news 时必填，必须是永久 MediaID）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>获取或设置是否打开评论（官方 <c>need_open_comment</c>：0 不打开（默认）/ 1 打开）。</summary>
    [JsonPropertyName("need_open_comment")]
    public int? NeedOpenComment { get; set; }

    /// <summary>获取或设置是否仅粉丝可评论（官方 <c>only_fans_can_comment</c>：0 所有人（默认）/ 1 仅粉丝）。</summary>
    [JsonPropertyName("only_fans_can_comment")]
    public int? OnlyFansCanComment { get; set; }

    /// <summary>获取或设置图片消息的图片信息（官方 <c>image_info</c>；图片消息（newspic）必填，最多 20 张，首张为封面）。</summary>
    [JsonPropertyName("image_info")]
    public MpDraftImageInfo? ImageInfo { get; set; }

    /// <summary>获取或设置封面信息（官方 <c>cover_info</c>；news 和 newspic 均适用）。</summary>
    [JsonPropertyName("cover_info")]
    public MpDraftCoverInfo? CoverInfo { get; set; }

    /// <summary>获取或设置商品信息（官方 <c>product_info</c>；带货场景）。</summary>
    [JsonPropertyName("product_info")]
    public MpDraftProductInfo? ProductInfo { get; set; }
}
