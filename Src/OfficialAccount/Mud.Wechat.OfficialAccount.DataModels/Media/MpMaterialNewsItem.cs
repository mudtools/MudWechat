// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 永久图文素材的文章条目（官方 <c>news_item</c> 元素；<c>get_material</c> 与
/// <c>batchget_material</c> 两页字段表主体一致 ⇒ 共用超集 DTO，差异字段可空）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两页差异（共用裁决，守卫锁定）</b>：<c>thumb_url</c> 仅出现在列表页（batchget_material）
/// 字段表，且其官方描述原文为「图文消息的封面图片素材id」——是 id 的描述而非 URL 的描述，
/// 疑为官方文档复制粘贴错误（照录）。<c>get_material</c> 页无该字段 ⇒ 此处保留可空。
/// </para>
/// <para><c>content</c> 上限：官方原文「必须少于 2 万字符，小于 1M，且此处会去除 JS」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMaterialNewsItem
{
    /// <summary>获取或设置图文消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置作者（官方 <c>author</c>）。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>获取或设置图文消息的摘要（官方 <c>digest</c>，仅有单图文消息才有摘要，多图文此处为空）。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>获取或设置图文消息的具体内容（官方 <c>content</c>，支持 HTML 标签，&lt;2 万字符、&lt;1M、去除 JS）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置图文消息的原文地址（官方 <c>content_source_url</c>，点击「阅读原文」后的 URL）。</summary>
    [JsonPropertyName("content_source_url")]
    public string? ContentSourceUrl { get; set; }

    /// <summary>获取或设置封面图片素材 id（官方 <c>thumb_media_id</c>，必须是永久 media_id）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>获取或设置是否显示封面（官方 <c>show_cover_pic</c>，0 = 不显示 / 1 = 显示）。</summary>
    [JsonPropertyName("show_cover_pic")]
    public int? ShowCoverPic { get; set; }

    /// <summary>
    /// 获取或设置图文页 URL（官方 <c>url</c>；列表页为图片素材列表时该字段是图片的 URL——官方字段表原文）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置封面图片 URL（官方 <c>thumb_url</c>，<b>仅列表页出现</b>；官方描述原文疑有误，照录）。</summary>
    [JsonPropertyName("thumb_url")]
    public string? ThumbUrl { get; set; }
}
