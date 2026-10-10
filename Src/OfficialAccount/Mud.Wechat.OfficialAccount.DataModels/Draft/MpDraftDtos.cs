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

/// <summary>图片消息的图片信息容器（官方 <c>image_info</c>；最多 20 张，首张即为封面图）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftImageInfo
{
    /// <summary>获取或设置图片列表（官方 <c>image_list</c>）。</summary>
    [JsonPropertyName("image_list")]
    public List<MpDraftImageItem>? ImageList { get; set; }
}

/// <summary>图片条目（官方 <c>image_info.image_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftImageItem
{
    /// <summary>获取或设置图片素材 id（官方 <c>image_media_id</c>；必须是永久 MediaID）。</summary>
    [JsonPropertyName("image_media_id")]
    public string? ImageMediaId { get; set; }
}

/// <summary>封面信息容器（官方 <c>cover_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftCoverInfo
{
    /// <summary>获取或设置封面裁剪信息列表（官方 <c>crop_percent_list</c>；坐标以左上角 (0,0)、右下角 (1,1) 建立）。</summary>
    [JsonPropertyName("crop_percent_list")]
    public List<MpDraftCropPercent>? CropPercentList { get; set; }
}

/// <summary>封面裁剪条目（官方 <c>cover_info.crop_percent_list</c> 元素；官方示例坐标为字符串形态）。</summary>
/// <remarks>ratio 取值：news 仅支持 "2.35_1"/"1_1"；newspic 支持 "1_1"/"16_9"/"2.35_1"（官方页面原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftCropPercent
{
    /// <summary>获取或设置裁剪比例（官方 <c>ratio</c>，字符串形态如 "2.35_1"）。</summary>
    [JsonPropertyName("ratio")]
    public string? Ratio { get; set; }

    /// <summary>获取或设置裁剪区域左上角 x（官方 <c>x1</c>）。</summary>
    [JsonPropertyName("x1")]
    public string? X1 { get; set; }

    /// <summary>获取或设置裁剪区域左上角 y（官方 <c>y1</c>）。</summary>
    [JsonPropertyName("y1")]
    public string? Y1 { get; set; }

    /// <summary>获取或设置裁剪区域右下角 x（官方 <c>x2</c>）。</summary>
    [JsonPropertyName("x2")]
    public string? X2 { get; set; }

    /// <summary>获取或设置裁剪区域右下角 y（官方 <c>y2</c>）。</summary>
    [JsonPropertyName("y2")]
    public string? Y2 { get; set; }
}

/// <summary>商品信息容器（官方 <c>product_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftProductInfo
{
    /// <summary>获取或设置文末商品信息（官方 <c>footer_product_info</c>）。</summary>
    [JsonPropertyName("footer_product_info")]
    public MpDraftFooterProductInfo? FooterProductInfo { get; set; }
}

/// <summary>文末商品信息（官方 <c>product_info.footer_product_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftFooterProductInfo
{
    /// <summary>获取或设置商品 key（官方 <c>product_key</c>）。</summary>
    [JsonPropertyName("product_key")]
    public string? ProductKey { get; set; }
}

/// <summary>新建草稿（<c>draft/add</c>）请求体（官方 <c>articles</c> 为数组形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftAddRequest
{
    /// <summary>获取或设置图文素材集合（官方 <c>articles</c>，必填）。</summary>
    [JsonPropertyName("articles")]
    public List<MpDraftArticle> Articles { get; set; } = new List<MpDraftArticle>();
}

/// <summary>新建草稿（<c>draft/add</c>）响应。</summary>
/// <remarks>官方契约：media_id 为上传后的获取标志（不超过 128 字符）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftAddResponse : MpResponse
{
    /// <summary>获取或设置草稿 media_id（官方 <c>media_id</c>，≤128 字符）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>更新草稿（<c>draft/update</c>）请求体（官方 <c>articles</c> 为<b>单对象</b>形态——与 add 页数组形态不同，照各自页面）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftUpdateRequest
{
    /// <summary>获取或设置要修改的草稿 id（官方 <c>media_id</c>，必填）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;

    /// <summary>获取或设置要更新的文章位置（官方 <c>index</c>，必填；多图文时有意义，第一篇为 0）。</summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>获取或设置图文信息（官方 <c>articles</c>，必填；<b>单对象</b>——与 add 页数组形态不同，照官方页面）。</summary>
    [JsonPropertyName("articles")]
    public MpDraftArticle Articles { get; set; } = new MpDraftArticle();
}

/// <summary>获取草稿详情（<c>draft/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftGetRequest
{
    /// <summary>获取或设置要获取的草稿的 media_id（官方 <c>media_id</c>，必填）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;
}

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

/// <summary>获取草稿详情（<c>draft/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftGetResponse : MpResponse
{
    /// <summary>获取或设置图文素材列表（官方 <c>news_item</c>）。</summary>
    [JsonPropertyName("news_item")]
    public List<MpDraftNewsItem>? NewsItems { get; set; }
}

/// <summary>获取草稿总数（<c>draft/count</c>）响应（GET、无请求体；官方页面无草稿数量上限表述）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftCountResponse : MpResponse
{
    /// <summary>获取或设置草稿的总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}

/// <summary>获取草稿列表（<c>draft/batchget</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftBatchGetRequest
{
    /// <summary>获取或设置偏移位置（官方 <c>offset</c>，0 表示从第一个素材返回）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置返回数量（官方 <c>count</c>，取值 1~20）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置是否不返回 content 字段（官方 <c>no_content</c>：1 不返回 / 0 正常返回，默认 0）。</summary>
    [JsonPropertyName("no_content")]
    public int? NoContent { get; set; }
}

/// <summary>获取草稿列表（<c>draft/batchget</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftBatchGetResponse : MpResponse
{
    /// <summary>获取或设置草稿素材总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    /// <summary>获取或设置本次获取数量（官方 <c>item_count</c>）。</summary>
    [JsonPropertyName("item_count")]
    public int ItemCount { get; set; }

    /// <summary>获取或设置素材列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<MpDraftListItem>? Items { get; set; }
}

/// <summary>草稿列表条目（官方 <c>item</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftListItem
{
    /// <summary>获取或设置草稿 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置图文消息内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public MpDraftNewsContent? Content { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }
}

/// <summary>草稿图文内容容器（官方 <c>content.news_item</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftNewsContent
{
    /// <summary>获取或设置图文素材列表（官方 <c>news_item</c>，元素与 draft/get 同构 ⇒ 共用 <see cref="MpDraftNewsItem"/>）。</summary>
    [JsonPropertyName("news_item")]
    public List<MpDraftNewsItem>? NewsItems { get; set; }
}

/// <summary>删除草稿（<c>draft/delete</c>）请求体（官方「此操作不可逆，请谨慎操作」）。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftDeleteRequest
{
    /// <summary>获取或设置要删除的草稿的 media_id（官方 <c>media_id</c>，必填）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;
}
