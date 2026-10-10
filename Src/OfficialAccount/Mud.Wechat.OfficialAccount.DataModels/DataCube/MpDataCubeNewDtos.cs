// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>阅读场景来源计数（官方 <c>read_user_source</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataSceneCount
{
    /// <summary>获取或设置阅读人数（官方 <c>user_count</c>）。</summary>
    [JsonPropertyName("user_count")]
    public int? UserCount { get; set; }

    /// <summary>获取或设置阅读场景来源描述（官方 <c>scene_desc</c>：全部 / 公众号消息 / 聊天会话 / 朋友圈 / 公众号主页 / 其他 / 推荐 / 搜一搜）。</summary>
    [JsonPropertyName("scene_desc")]
    public string? SceneDesc { get; set; }
}

/// <summary>文章阅读详细数据（官方 <c>getarticleread</c> 的 detail 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataReadDetail
{
    /// <summary>获取或设置阅读人数（官方 <c>read_user</c>，即来源为全部的阅读人数）。</summary>
    [JsonPropertyName("read_user")]
    public int? ReadUser { get; set; }

    /// <summary>获取或设置阅读数据来源分布（官方 <c>read_user_source</c>）。</summary>
    [JsonPropertyName("read_user_source")]
    public List<MpDataSceneCount>? ReadUserSource { get; set; }
}

/// <summary>
/// 发表内容每日阅读数据条目（官方 <c>getarticleread</c> 的 list 元素；新图文族 4 端点顶层均为
/// list + is_delay 形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleReadItem
{
    /// <summary>获取或设置统计日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>，由 msg_data_id 与 index 组成，如 "12003_3"）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置每篇文章的详细数据（官方 <c>detail</c>）。</summary>
    [JsonPropertyName("detail")]
    public MpDataReadDetail? Detail { get; set; }
}

/// <summary>发表内容每日阅读数据（<c>getarticleread</c>）响应（跨度仅支持查询 1 天）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleReadResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>；当天无文章指标变化时为空）。</summary>
    [JsonPropertyName("list")]
    public List<MpDataArticleReadItem>? List { get; set; }

    /// <summary>获取或设置数据是否有延迟（官方 <c>is_delay</c>；false 表示数据是最新的）。</summary>
    [JsonPropertyName("is_delay")]
    public bool? IsDelay { get; set; }
}

/// <summary>文章分享详细数据（官方 <c>getarticleshare</c> 的 detail 对象；仅 1 个标量）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataShareDetail
{
    /// <summary>获取或设置分享人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }
}

/// <summary>发表内容每日分享数据条目（官方 <c>getarticleshare</c> 的 list 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleShareItem
{
    /// <summary>获取或设置统计日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置每篇文章的详细数据（官方 <c>detail</c>；仅 share_user 一项）。</summary>
    [JsonPropertyName("detail")]
    public MpDataShareDetail? Detail { get; set; }
}

/// <summary>发表内容每日分享数据（<c>getarticleshare</c>）响应（跨度仅支持查询 1 天）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleShareResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpDataArticleShareItem>? List { get; set; }

    /// <summary>获取或设置数据是否有延迟（官方 <c>is_delay</c>）。</summary>
    [JsonPropertyName("is_delay")]
    public bool? IsDelay { get; set; }
}

/// <summary>
/// 账号级概况详细数据（官方 <c>getbizsummary</c> 的 detail 对象；阅读/分享/赞/留言/收藏/跳转原文/发布篇数聚合）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataBizDetail
{
    /// <summary>获取或设置阅读人数（官方 <c>read_user</c>）。</summary>
    [JsonPropertyName("read_user")]
    public int? ReadUser { get; set; }

    /// <summary>获取或设置阅读数据来源分布（官方 <c>read_user_source</c>）。</summary>
    [JsonPropertyName("read_user_source")]
    public List<MpDataSceneCount>? ReadUserSource { get; set; }

    /// <summary>获取或设置分享人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置爱心赞人数（官方 <c>zaikan_user</c>——官方拼写 zaikan，勿「修正」）。</summary>
    [JsonPropertyName("zaikan_user")]
    public int? ZaiKanUser { get; set; }

    /// <summary>获取或设置拇指赞人数（官方 <c>like_user</c>）。</summary>
    [JsonPropertyName("like_user")]
    public int? LikeUser { get; set; }

    /// <summary>获取或设置留言条数（官方 <c>comment_count</c>）。</summary>
    [JsonPropertyName("comment_count")]
    public int? CommentCount { get; set; }

    /// <summary>获取或设置微信收藏人数（官方 <c>collection_user</c>）。</summary>
    [JsonPropertyName("collection_user")]
    public int? CollectionUser { get; set; }

    /// <summary>获取或设置跳转原文人数（官方 <c>redirect_ori_page_user</c>）。</summary>
    [JsonPropertyName("redirect_ori_page_user")]
    public int? RedirectOriPageUser { get; set; }

    /// <summary>获取或设置发布篇数（官方 <c>send_page_count</c>）。</summary>
    [JsonPropertyName("send_page_count")]
    public int? SendPageCount { get; set; }
}

/// <summary>发表内容概况总数据条目（官方 <c>getbizsummary</c> 的 list 元素；账号级聚合、非按 msgid 拆分）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataBizSummaryItem
{
    /// <summary>获取或设置统计日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置账号级详细数据（官方 <c>detail</c>）。</summary>
    [JsonPropertyName("detail")]
    public MpDataBizDetail? Detail { get; set; }
}

/// <summary>发表内容概况总数据（<c>getbizsummary</c>）响应（最长支持查询 30 天）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataBizSummaryResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpDataBizSummaryItem>? List { get; set; }

    /// <summary>获取或设置数据是否有延迟（官方 <c>is_delay</c>）。</summary>
    [JsonPropertyName("is_delay")]
    public bool? IsDelay { get; set; }
}

/// <summary>文章进度跳出区间条目（官方 <c>read_jump_position</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataJumpPosition
{
    /// <summary>获取或设置跳出区间（官方 <c>position</c>：1~5 对应 0~20% 至 80%~100% 的文章进度区间）。</summary>
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    /// <summary>获取或设置当前跳出位置所占用户比例（官方 <c>rate</c>）。</summary>
    [JsonPropertyName("rate")]
    public int? Rate { get; set; }
}

/// <summary>
/// 单日累计统计条目（官方 <c>getarticletotaldetail</c> 的 detail_list 元素；统计数据为
/// [发表日期-统计日期] 期间指标总和）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataTotalDetailStat
{
    /// <summary>获取或设置统计日期（官方 <c>stat_date</c>）。</summary>
    [JsonPropertyName("stat_date")]
    public string? StatDate { get; set; }

    /// <summary>获取或设置阅读人数（官方 <c>read_user</c>）。</summary>
    [JsonPropertyName("read_user")]
    public int? ReadUser { get; set; }

    /// <summary>获取或设置阅读数据来源分布（官方 <c>read_user_source</c>）。</summary>
    [JsonPropertyName("read_user_source")]
    public List<MpDataSceneCount>? ReadUserSource { get; set; }

    /// <summary>获取或设置分享人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置爱心赞人数（官方 <c>zaikan_user</c>）。</summary>
    [JsonPropertyName("zaikan_user")]
    public int? ZaiKanUser { get; set; }

    /// <summary>获取或设置拇指赞人数（官方 <c>like_user</c>）。</summary>
    [JsonPropertyName("like_user")]
    public int? LikeUser { get; set; }

    /// <summary>获取或设置留言条数（官方 <c>comment_count</c>）。</summary>
    [JsonPropertyName("comment_count")]
    public int? CommentCount { get; set; }

    /// <summary>获取或设置微信收藏人数（官方 <c>collection_user</c>）。</summary>
    [JsonPropertyName("collection_user")]
    public int? CollectionUser { get; set; }

    /// <summary>获取或设置赞赏金额（官方 <c>praise_money</c>，单位分）。</summary>
    [JsonPropertyName("praise_money")]
    public long? PraiseMoney { get; set; }

    /// <summary>获取或设置阅读后关注人数（官方 <c>read_subscribe_user</c>）。</summary>
    [JsonPropertyName("read_subscribe_user")]
    public int? ReadSubscribeUser { get; set; }

    /// <summary>获取或设置阅读送达率（官方 <c>read_delivery_rate</c>）。</summary>
    [JsonPropertyName("read_delivery_rate")]
    public int? ReadDeliveryRate { get; set; }

    /// <summary>获取或设置阅读完成率（官方 <c>read_finish_rate</c>）。</summary>
    [JsonPropertyName("read_finish_rate")]
    public int? ReadFinishRate { get; set; }

    /// <summary>获取或设置平均阅读时长（官方 <c>read_avg_activetime</c>，单位分钟）。</summary>
    [JsonPropertyName("read_avg_activetime")]
    public int? ReadAvgActiveTime { get; set; }

    /// <summary>获取或设置文章进度跳出分布（官方 <c>read_jump_position</c>）。</summary>
    [JsonPropertyName("read_jump_position")]
    public List<MpDataJumpPosition>? ReadJumpPosition { get; set; }
}

/// <summary>发表内容发表详细数据条目（官方 <c>getarticletotaldetail</c> 的 list 元素）。</summary>
/// <remarks>
/// 官方文档缺陷（照录）：返回参数表未列出 ref_date/msgid/title/content_url 字段行，
/// 仅出现在返回示例与变更日志（2026-03-03 补充文章标题和链接）——SDK 按示例形态建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleTotalDetailItem
{
    /// <summary>获取或设置文章发表日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置发表类型（官方 <c>publish_type</c>：0 已通知 / 1 未开启通知）。</summary>
    [JsonPropertyName("publish_type")]
    public int? PublishType { get; set; }

    /// <summary>获取或设置逐日详细数据（官方 <c>detail_list</c>；每篇文章仅统计发表日期起 30 天内）。</summary>
    [JsonPropertyName("detail_list")]
    public List<MpDataTotalDetailStat>? DetailList { get; set; }

    /// <summary>获取或设置文章标题（官方 <c>title</c>；2026-03-03 变更日志补充）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置文章链接（官方 <c>content_url</c>；2026-03-03 变更日志补充）。</summary>
    [JsonPropertyName("content_url")]
    public string? ContentUrl { get; set; }
}

/// <summary>发表内容发表详细数据（<c>getarticletotaldetail</c>）响应（跨度仅支持查询 1 天）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleTotalDetailResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpDataArticleTotalDetailItem>? List { get; set; }

    /// <summary>获取或设置数据是否有延迟（官方 <c>is_delay</c>）。</summary>
    [JsonPropertyName("is_delay")]
    public bool? IsDelay { get; set; }
}

/// <summary>
/// 消息发送概况数据条目（官方 <c>getupstreammsg</c> 的 list 元素；week/month/hour 三端点响应<b>完全同构</b>
/// ⇒ 共用本 DTO——分时端点的 ref_hour 仅出现在官方示例、字段表未列（官方文档缺口，照录不建模））。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUpstreamMsgItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置消息类型（官方 <c>msg_type</c>：1 文字 / 2 图片 / 3 语音 / 4 视频 / 6 第三方应用消息（链接消息））。</summary>
    [JsonPropertyName("msg_type")]
    public int? MsgType { get; set; }

    /// <summary>获取或设置上行发送消息的用户数（官方 <c>msg_user</c>）。</summary>
    [JsonPropertyName("msg_user")]
    public int? MsgUser { get; set; }

    /// <summary>获取或设置上行发送消息的消息总数（官方 <c>msg_count</c>）。</summary>
    [JsonPropertyName("msg_count")]
    public int? MsgCount { get; set; }
}

/// <summary>消息发送概况数据（<c>getupstreammsg</c> / week / month / hour 四端点共用响应——元素完全同构）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUpstreamMsgResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpUpstreamMsgItem>? List { get; set; }
}

/// <summary>
/// 消息发送分布数据条目（官方 <c>getupstreammsgdist</c> 的 list 元素；week/month 两端点响应完全同构 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUpstreamMsgDistItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置当日发送消息量分布区间（官方 <c>count_interval</c>：0 代表 "0"、1 代表 "1-5"、2 代表 "6-10"、3 代表 "10 次以上"）。</summary>
    [JsonPropertyName("count_interval")]
    public int? CountInterval { get; set; }

    /// <summary>获取或设置上行发送消息的用户数（官方 <c>msg_user</c>）。</summary>
    [JsonPropertyName("msg_user")]
    public int? MsgUser { get; set; }
}

/// <summary>消息发送分布数据（<c>getupstreammsgdist</c> / distweek / distmonth 三端点共用响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUpstreamMsgDistResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpUpstreamMsgDistItem>? List { get; set; }
}

/// <summary>
/// 被动回复概要/分布数据条目（官方 <c>getinterfacesummary</c> 的 list 元素；hour 端点多 ref_hour
/// ⇒ 超集共用本 DTO）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpInterfaceSummaryItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置数据的小时（官方 <c>ref_hour</c>；<b>仅分时端点返回</b>，日数据端点为 null）。</summary>
    [JsonPropertyName("ref_hour")]
    public int? RefHour { get; set; }

    /// <summary>获取或设置被动回复用户消息的次数（官方 <c>callback_count</c>）。</summary>
    [JsonPropertyName("callback_count")]
    public int? CallbackCount { get; set; }

    /// <summary>获取或设置被动回复的失败次数（官方 <c>fail_count</c>）。</summary>
    [JsonPropertyName("fail_count")]
    public int? FailCount { get; set; }

    /// <summary>获取或设置总耗时（官方 <c>total_time_cost</c>；除以 callback_count 即平均耗时）。</summary>
    [JsonPropertyName("total_time_cost")]
    public int? TotalTimeCost { get; set; }

    /// <summary>获取或设置最大耗时（官方 <c>max_time_cost</c>）。</summary>
    [JsonPropertyName("max_time_cost")]
    public int? MaxTimeCost { get; set; }
}

/// <summary>被动回复概要数据（<c>getinterfacesummary</c> / <c>getinterfacesummaryhour</c> 共用响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpInterfaceSummaryResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpInterfaceSummaryItem>? List { get; set; }
}
