// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.FreePublish;

/// <summary>发布草稿（<c>freepublish/submit</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishSubmitRequest
{
    /// <summary>获取或设置要发布的草稿的 media_id（官方 <c>media_id</c>，必填）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;
}

/// <summary>发布草稿（<c>freepublish/submit</c>）响应。</summary>
/// <remarks>
/// <para>
/// 官方原文：「正常情况下调用成功时，errcode 将为 0，此时<b>只意味着发布任务提交成功</b>，
/// 并不意味着此时发布已经完成」——发布结果经 <c>PUBLISHJOBFINISH</c> 事件推送（SDK 未建模该事件：
/// 官方 API 页仅文字提及，XML 报文无独立页面，与 kf 会话事件同为待实测项）。
/// </para>
/// <para><b>官方文档矛盾（照录）</b>：返回体表列 <c>msg_data_id</c> 但返回示例未含该字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishSubmitResponse : MpResponse
{
    /// <summary>获取或设置发布任务的 id（官方 <c>publish_id</c>）。</summary>
    [JsonPropertyName("publish_id")]
    public string? PublishId { get; set; }

    /// <summary>获取或设置消息的数据 ID（官方 <c>msg_data_id</c>；官方返回表示例缺失，矛盾照录）。</summary>
    [JsonPropertyName("msg_data_id")]
    public string? MsgDataId { get; set; }
}

/// <summary>发布状态查询（<c>freepublish/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishGetRequest
{
    /// <summary>获取或设置发布任务 id（官方 <c>publish_id</c>，必填）。</summary>
    [JsonPropertyName("publish_id")]
    public string PublishId { get; set; } = string.Empty;
}

/// <summary>发布状态查询（<c>freepublish/get</c>）响应。</summary>
/// <remarks>
/// <b>publish_status 为标量 number</b>（逐页核验修正：方案文档预判「publish_status 数组」不成立；
/// 数组是 article_detail.item 与 fail_idx）。取值：0 成功 / 1 发布中 / 2 原创失败 / 3 常规失败 /
/// 4 平台审核不通过 / 5 成功后用户删除所有文章 / 6 成功后系统封禁所有文章。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishGetResponse : MpResponse
{
    /// <summary>获取或设置发布任务 id（官方 <c>publish_id</c>）。</summary>
    [JsonPropertyName("publish_id")]
    public string? PublishId { get; set; }

    /// <summary>获取或设置发布状态（官方 <c>publish_status</c>：0~6，见类型 remarks）。</summary>
    [JsonPropertyName("publish_status")]
    public int? PublishStatus { get; set; }

    /// <summary>获取或设置成功时的图文 article_id（官方 <c>article_id</c>）。</summary>
    [JsonPropertyName("article_id")]
    public string? ArticleId { get; set; }

    /// <summary>获取或设置文章详情（官方 <c>article_detail</c>；发布状态为 0（成功）时返回）。</summary>
    [JsonPropertyName("article_detail")]
    public MpFreePublishArticleDetail? ArticleDetail { get; set; }

    /// <summary>获取或设置失败文章编号列表（官方 <c>fail_idx</c>）。</summary>
    [JsonPropertyName("fail_idx")]
    public List<long>? FailIdx { get; set; }
}

/// <summary>文章详情容器（官方 <c>article_detail</c>；发布成功时返回）。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishArticleDetail
{
    /// <summary>获取或设置文章数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>获取或设置文章详情列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<MpFreePublishArticleUrlItem>? Items { get; set; }
}

/// <summary>文章 URL 条目（官方 <c>article_detail.item</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishArticleUrlItem
{
    /// <summary>获取或设置文章对应编号（官方 <c>idx</c>）。</summary>
    [JsonPropertyName("idx")]
    public int? Idx { get; set; }

    /// <summary>获取或设置图文的永久链接（官方 <c>article_url</c>）。</summary>
    [JsonPropertyName("article_url")]
    public string? ArticleUrl { get; set; }
}

/// <summary>删除发布文章（<c>freepublish/delete</c>）请求体（官方「此操作不可逆，请谨慎操作」）。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishDeleteRequest
{
    /// <summary>获取或设置成功发布时返回的 article_id（官方 <c>article_id</c>，必填）。</summary>
    [JsonPropertyName("article_id")]
    public string ArticleId { get; set; } = string.Empty;

    /// <summary>获取或设置要删除的文章位置（官方 <c>index</c>，第一篇编号 1；不填或填 0 删除全部文章）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }
}

/// <summary>获取已发布图文信息（<c>freepublish/getarticle</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishGetArticleRequest
{
    /// <summary>获取或设置要获取的 article_id（官方 <c>article_id</c>，必填；官方说明原文误写「草稿」，照录）。</summary>
    [JsonPropertyName("article_id")]
    public string ArticleId { get; set; } = string.Empty;
}

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

/// <summary>获取已发布图文信息（<c>freepublish/getarticle</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishGetArticleResponse : MpResponse
{
    /// <summary>获取或设置图文信息集合（官方 <c>news_item</c>）。</summary>
    [JsonPropertyName("news_item")]
    public List<MpFreePublishNewsItem>? NewsItems { get; set; }
}

/// <summary>获取已发布消息列表（<c>freepublish/batchget</c>）请求体。</summary>
/// <remarks>
/// <b>逐页核验修正</b>：count 上限官方原文「取值在 1 到 20 之间」——<b>非</b>方案文档预判的 ≤100
/// （已双验，疑官方沿用草稿列表文案或收紧）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishBatchGetRequest
{
    /// <summary>获取或设置偏移位置（官方 <c>offset</c>，0 表示从第一个素材返回）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置返回数量（官方 <c>count</c>，取值 1~20——官方原文，非旧版口径的 100）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置是否不返回 content 字段（官方 <c>no_content</c>：1 不返回 / 0 正常返回，默认 0）。</summary>
    [JsonPropertyName("no_content")]
    public int? NoContent { get; set; }
}

/// <summary>获取已发布消息列表（<c>freepublish/batchget</c>）响应。</summary>
/// <remarks><b>逐页核验修正</b>：响应条目键为 <c>article_id</c>（非方案文档预判的 item_id）。</remarks>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishBatchGetResponse : MpResponse
{
    /// <summary>获取或设置成功发布素材总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    /// <summary>获取或设置本次获取数量（官方 <c>item_count</c>）。</summary>
    [JsonPropertyName("item_count")]
    public int ItemCount { get; set; }

    /// <summary>获取或设置图文消息条目列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<MpFreePublishListItem>? Items { get; set; }
}

/// <summary>已发布消息条目（官方 <c>item</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishListItem
{
    /// <summary>获取或设置成功发布的图文消息 id（官方 <c>article_id</c>）。</summary>
    [JsonPropertyName("article_id")]
    public string? ArticleId { get; set; }

    /// <summary>获取或设置图文消息内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public MpFreePublishNewsContent? Content { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }
}

/// <summary>图文内容容器（官方 <c>content.news_item</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishNewsContent
{
    /// <summary>获取或设置图文列表（官方 <c>news_item</c>，元素与 getarticle 同构 ⇒ 共用 <see cref="MpFreePublishNewsItem"/>）。</summary>
    [JsonPropertyName("news_item")]
    public List<MpFreePublishNewsItem>? NewsItems { get; set; }
}
