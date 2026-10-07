// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 上传永久素材（<c>material/add_material</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>media_id</c> 恒返回；<c>url</c> <b>仅图片素材返回</b>——
/// 官方「注意事项」原文「永久图片素材新增后，将带有 URL 返回给开发者，开发者可以在腾讯系域名内使用
/// （腾讯系域名外使用，图片将被屏蔽）」。视频示例中 <c>url</c> 为空串 ⇒ 非图片类型不得以
/// <c>url</c> 有值作任何判断。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpAddMaterialResponse : MpResponse
{
    /// <summary>获取或设置新增的永久素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置图片素材 URL（官方 <c>url</c>，<b>仅图片素材返回</b>；腾讯系域名内可用）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 永久素材的视频描述（<c>add_material</c> 的 multipart <c>description</c> 表单字段形态）。
/// </summary>
/// <remarks>
/// <para>
/// 官方表单字段 <c>description</c> 为 JSON 字符串（<c>-F description='{"title":…, "introduction":…}'</c>）；
/// 本 DTO 供调用方经 <c>MediaJsonContext</c> 序列化后作为表单 <c>description</c> 字段值传入。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：字段表将 <c>description</c>/<c>title</c>/<c>introduction</c>
/// 全部标为可选（否），但正文写「上传视频素材时需要」⇒ SDK 不做本地必填拦截，按官方错误码表达。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMaterialDescription
{
    /// <summary>获取或设置视频素材描述标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置视频素材描述简介（官方 <c>introduction</c>）。</summary>
    [JsonPropertyName("introduction")]
    public string? Introduction { get; set; }
}

/// <summary>
/// 获取永久素材（<c>material/get_material</c>）的 JSON 形态响应（图文 / 视频两种形态的字段超集）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>POST</b> + 请求体 <c>{"media_id":…}</c>；
/// 响应按素材类型分流——<b>图文</b>返回 <c>news_item</c> 数组；<b>视频</b>返回
/// <c>title</c>/<c>description</c>/<c>down_url</c>；<b>图片 / 语音</b>官方原文
/// 「响应的直接为素材的内容，开发者可以自行保存为文件」（二进制，不走本 DTO——
/// 由 <c>IMpMediaDownloadService.GetPermanentMaterialAsync</c> 的下载信封承载）。
/// </para>
/// <para>
/// <b>形态判定</b>：<c>NewsItems != null</c> ⇒ 图文；<c>VideoDownUrl != null</c> ⇒ 视频。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：视频返回示例 JSON 末尾带尾逗号且值为裸占位符无引号（页面原文如此）；
/// 本页 <c>news_item</c> 无 <c>need_open_comment</c>/<c>only_fans_can_comment</c> 字段（旧版文档有，
/// 新版页面不存在——SDK 按新版页面建模）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpPermanentMaterialResponse
{
    /// <summary>获取或设置图文素材内容（官方 <c>news_item</c>；<b>仅图文素材返回</b>）。</summary>
    [JsonPropertyName("news_item")]
    public List<MpMaterialNewsItem>? NewsItems { get; set; }

    /// <summary>获取或设置视频素材标题（官方 <c>title</c>；<b>仅视频素材返回</b>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置视频素材描述（官方 <c>description</c>；<b>仅视频素材返回</b>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置视频下载地址（官方 <c>down_url</c>；<b>仅视频素材返回</b>）。</summary>
    [JsonPropertyName("down_url")]
    public string? DownUrl { get; set; }
}

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
