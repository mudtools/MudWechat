// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 「上传图文消息素材（高级群发前置）」单篇文章项（官方 <c>articles</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpUploadNewsArticle
{
    /// <summary>获取或设置图文消息缩略图的 <c>media_id</c>（官方 <c>thumb_media_id</c>，必填；先经临时/永久素材接口上传）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string ThumbnailMediaId { get; set; } = string.Empty;

    /// <summary>获取或设置图文标题（官方 <c>title</c>，必填；不超过 64 字）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置作者（官方 <c>author</c>，可选）。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>获取或设置图文摘要（官方 <c>digest</c>，可选；单图文时默认取正文前 54 字）。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>获取或设置「阅读原文」链接（官方 <c>content_source_url</c>，可选）。</summary>
    [JsonPropertyName("content_source_url")]
    public string? ContentSourceUrl { get; set; }

    /// <summary>获取或设置图文内容（官方 <c>content</c>，必填；支持 HTML 标签子集，不超过 1 万字符，图片须先上传取 URL）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>获取或设置是否显示封面（官方 <c>show_cover_pic</c>，可选；0/1）。</summary>
    [JsonPropertyName("show_cover_pic")]
    public bool? IsShowCover { get; set; }

    /// <summary>获取或设置是否打开评论（官方 <c>need_open_comment</c>，可选；0/1）。</summary>
    [JsonPropertyName("need_open_comment")]
    public bool? IsOpenComment { get; set; }

    /// <summary>获取或设置是否仅粉丝可评论（官方 <c>only_fans_can_comment</c>，可选；0/1，仅在评论打开时生效）。</summary>
    [JsonPropertyName("only_fans_can_comment")]
    public bool? IsOnlyFansCanComment { get; set; }
}

/// <summary>
/// 「上传图文消息素材」请求（官方 <c>media/uploadnews</c>；纯 JSON 请求体——<b>非</b> multipart）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpUploadNewsRequest
{
    /// <summary>获取或设置图文消息列表（官方 <c>articles</c>，必填；1-8 条）。</summary>
    [JsonPropertyName("articles")]
    public List<MpUploadNewsArticle> ArticleList { get; set; } = new();
}

/// <summary>
/// 「上传视频素材（高级群发前置）」请求（官方 <c>media/uploadvideo</c>；官方契约
/// <b>multipart/form-data</b>——三个表单字段由调用方经 <see cref="IFormContent"/> 构建）。
/// </summary>
/// <remarks>
/// <para>官方约束：<c>media_id</c> 须为已上传视频素材的 <c>media_id</c>（<c>type=video</c>），
/// 群发视频消息前必须先经本端点换取群发专用 <c>media_id</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpUploadVideoRequest
{
    /// <summary>获取或设置视频素材 <c>media_id</c>（官方表单字段 <c>media_id</c>，必填）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;

    /// <summary>获取或设置视频标题（官方表单字段 <c>title</c>，必填；不超过 128 字）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置视频描述（官方表单字段 <c>description</c>，可选；不超过 512 字）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
