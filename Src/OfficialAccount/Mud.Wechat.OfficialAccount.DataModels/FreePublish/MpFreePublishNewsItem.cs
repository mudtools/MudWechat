// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.FreePublish;

/// <summary>
/// 已发布图文条目（官方 <c>news_item</c> 元素；getarticle 与 freepublish/batchget 两页同构 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <b>官方文档矛盾（照录）</b>：字段表无 show_cover_pic 行、返回示例却含该字段——SDK 按字段表建模；
/// <c>is_deleted</c> 为 boolean 形态（是否被删除）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishNewsItem
{
    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置作者（官方 <c>author</c>）。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>获取或设置摘要（官方 <c>digest</c>；仅单图文有，未填默认抓取正文前 54 个字）。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>获取或设置图文内容（官方 <c>content</c>；支持 HTML、&lt;2 万字符、&lt;1M、去除 JS；图片 url 须经 uploadimg 获取）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置原文地址（官方 <c>content_source_url</c>）。</summary>
    [JsonPropertyName("content_source_url")]
    public string? ContentSourceUrl { get; set; }

    /// <summary>获取或设置封面素材 id（官方 <c>thumb_media_id</c>，必须是永久 MediaID）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>获取或设置封面图片 URL（官方 <c>thumb_url</c>）。</summary>
    [JsonPropertyName("thumb_url")]
    public string? ThumbUrl { get; set; }

    /// <summary>获取或设置是否打开评论（官方 <c>need_open_comment</c>）。</summary>
    [JsonPropertyName("need_open_comment")]
    public int? NeedOpenComment { get; set; }

    /// <summary>获取或设置是否仅粉丝可评论（官方 <c>only_fans_can_comment</c>）。</summary>
    [JsonPropertyName("only_fans_can_comment")]
    public int? OnlyFansCanComment { get; set; }

    /// <summary>获取或设置草稿的临时链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置该图文是否被删除（官方 <c>is_deleted</c>，boolean）。</summary>
    [JsonPropertyName("is_deleted")]
    public bool? IsDeleted { get; set; }
}
