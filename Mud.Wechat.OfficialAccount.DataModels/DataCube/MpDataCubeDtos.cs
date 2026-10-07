// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 数据统计域统一请求体（官方 21 端点请求体均为 <c>{begin_date, end_date}</c> 两字段 ⇒ 共用 DTO，N4 裁决）。
/// </summary>
/// <remarks>
/// <para>
/// 日期格式 yyyy-MM-dd。各端点<b>跨度上限措辞不一（照官方各页原文，勿互相套用）</b>：
/// 用户数据 2 端点「最大跨度7天」；getupstreammsg「与end_date差值小于7天」；消息周/月/分时
/// 「结束日期(必须为同一天)」；消息分布 3 端点「跨度不超过15天」；getinterfacesummary
/// 「最大时间跨度30天」；getinterfacesummaryhour「最大时间跨度为1天」（注意事项）；
/// 新图文 3 端点「日期范围仅支持查询1天。」；getbizsummary「最长支持查询30天。」；
/// 旧图文 6 端点参数表仅「结束日期(最大值为昨日)」（无天数表述）。
/// </para>
/// <para>SDK 不做本地跨度校验（越界由官方 61501 表达）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDateRangeRequest
{
    /// <summary>获取或设置起始日期（官方 <c>begin_date</c>，格式 yyyy-MM-dd）。</summary>
    [JsonPropertyName("begin_date")]
    public string BeginDate { get; set; } = string.Empty;

    /// <summary>获取或设置结束日期（官方 <c>end_date</c>，格式 yyyy-MM-dd；各端点跨度上限见类型 remarks）。</summary>
    [JsonPropertyName("end_date")]
    public string EndDate { get; set; } = string.Empty;
}

/// <summary>用户增减数据条目（官方 <c>getusersummary</c> 的 list 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserSummaryItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置用户的渠道（官方 <c>user_source</c>：0 其他合计 / 1 公众号搜索 / 17 名片分享 / 30 扫描二维码 / 57 文章内账号名称 / 100 微信广告 / 161 他人转载 / 149 小程序关注 / 200 视频号 / 201 直播）。</summary>
    [JsonPropertyName("user_source")]
    public int? UserSource { get; set; }

    /// <summary>获取或设置新增的用户数量（官方 <c>new_user</c>）。</summary>
    [JsonPropertyName("new_user")]
    public int? NewUser { get; set; }

    /// <summary>获取或设置取消关注的用户数量（官方 <c>cancel_user</c>；new_user − cancel_user 即净增）。</summary>
    [JsonPropertyName("cancel_user")]
    public int? CancelUser { get; set; }
}

/// <summary>用户增减数据（<c>getusersummary</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserSummaryResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpUserSummaryItem>? List { get; set; }
}

/// <summary>累计用户数据条目（官方 <c>getusercumulate</c> 的 list 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserCumulateItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置总用户量（官方 <c>cumulate_user</c>）。</summary>
    [JsonPropertyName("cumulate_user")]
    public int? CumulateUser { get; set; }
}

/// <summary>累计用户数据（<c>getusercumulate</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserCumulateResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpUserCumulateItem>? List { get; set; }
}

/// <summary>
/// 图文群发每日数据条目（官方 <c>getarticlesummary</c> 的 list 元素；官方声明<b>本接口已停止维护</b>，
/// 推荐替换 getarticleread / getarticleshare / getbizsummary / getarticletotaldetail）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpArticleSummaryItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>：由 msg_data_id 与 index 组成）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置图文消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置图文页阅读人数（官方 <c>int_page_read_user</c>）。</summary>
    [JsonPropertyName("int_page_read_user")]
    public int? IntPageReadUser { get; set; }

    /// <summary>获取或设置图文页阅读次数（官方 <c>int_page_read_count</c>）。</summary>
    [JsonPropertyName("int_page_read_count")]
    public int? IntPageReadCount { get; set; }

    /// <summary>获取或设置原文页阅读人数（官方 <c>ori_page_read_user</c>；无原文页为 0）。</summary>
    [JsonPropertyName("ori_page_read_user")]
    public int? OriPageReadUser { get; set; }

    /// <summary>获取或设置原文页阅读次数（官方 <c>ori_page_read_count</c>）。</summary>
    [JsonPropertyName("ori_page_read_count")]
    public int? OriPageReadCount { get; set; }

    /// <summary>获取或设置分享的人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置分享的次数（官方 <c>share_count</c>）。</summary>
    [JsonPropertyName("share_count")]
    public int? ShareCount { get; set; }

    /// <summary>获取或设置收藏的人数（官方 <c>add_to_fav_user</c>）。</summary>
    [JsonPropertyName("add_to_fav_user")]
    public int? AddToFavUser { get; set; }

    /// <summary>获取或设置收藏的次数（官方 <c>add_to_fav_count</c>）。</summary>
    [JsonPropertyName("add_to_fav_count")]
    public int? AddToFavCount { get; set; }
}

/// <summary>图文群发每日数据（<c>getarticlesummary</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpArticleSummaryResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpArticleSummaryItem>? List { get; set; }
}

/// <summary>
/// 图文群发总数据的单日统计条目（官方 <c>getarticletotal</c> 的 list[].details 元素；
/// 反直觉拼写锁定：feed_share 三组后缀为 <c>_cnt</c>，其余为 <c>_count</c>——照抄官方）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpArticleTotalStat
{
    /// <summary>获取或设置数据统计日期（官方 <c>stat_date</c>，区别于 ref_date 群发日期）。</summary>
    [JsonPropertyName("stat_date")]
    public string? StatDate { get; set; }

    /// <summary>获取或设置送达人数（官方 <c>target_user</c>，约等于总粉丝数排除黑名单等异常）。</summary>
    [JsonPropertyName("target_user")]
    public int? TargetUser { get; set; }

    /// <summary>获取或设置图文页阅读人数（官方 <c>int_page_read_user</c>）。</summary>
    [JsonPropertyName("int_page_read_user")]
    public int? IntPageReadUser { get; set; }

    /// <summary>获取或设置图文页阅读次数（官方 <c>int_page_read_count</c>）。</summary>
    [JsonPropertyName("int_page_read_count")]
    public int? IntPageReadCount { get; set; }

    /// <summary>获取或设置原文页阅读人数（官方 <c>ori_page_read_user</c>）。</summary>
    [JsonPropertyName("ori_page_read_user")]
    public int? OriPageReadUser { get; set; }

    /// <summary>获取或设置原文页阅读次数（官方 <c>ori_page_read_count</c>）。</summary>
    [JsonPropertyName("ori_page_read_count")]
    public int? OriPageReadCount { get; set; }

    /// <summary>获取或设置分享的人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置分享的次数（官方 <c>share_count</c>）。</summary>
    [JsonPropertyName("share_count")]
    public int? ShareCount { get; set; }

    /// <summary>获取或设置收藏的人数（官方 <c>add_to_fav_user</c>）。</summary>
    [JsonPropertyName("add_to_fav_user")]
    public int? AddToFavUser { get; set; }

    /// <summary>获取或设置收藏的次数（官方 <c>add_to_fav_count</c>）。</summary>
    [JsonPropertyName("add_to_fav_count")]
    public int? AddToFavCount { get; set; }

    /// <summary>获取或设置公众号会话阅读人数（官方 <c>int_page_from_session_read_user</c>）。</summary>
    [JsonPropertyName("int_page_from_session_read_user")]
    public int? IntPageFromSessionReadUser { get; set; }

    /// <summary>获取或设置公众号会话阅读次数（官方 <c>int_page_from_session_read_count</c>）。</summary>
    [JsonPropertyName("int_page_from_session_read_count")]
    public int? IntPageFromSessionReadCount { get; set; }

    /// <summary>获取或设置历史消息页阅读人数（官方 <c>int_page_from_hist_msg_read_user</c>）。</summary>
    [JsonPropertyName("int_page_from_hist_msg_read_user")]
    public int? IntPageFromHistMsgReadUser { get; set; }

    /// <summary>获取或设置历史消息页阅读次数（官方 <c>int_page_from_hist_msg_read_count</c>）。</summary>
    [JsonPropertyName("int_page_from_hist_msg_read_count")]
    public int? IntPageFromHistMsgReadCount { get; set; }

    /// <summary>获取或设置朋友圈阅读人数（官方 <c>int_page_from_feed_read_user</c>）。</summary>
    [JsonPropertyName("int_page_from_feed_read_user")]
    public int? IntPageFromFeedReadUser { get; set; }

    /// <summary>获取或设置朋友圈阅读次数（官方 <c>int_page_from_feed_read_count</c>）。</summary>
    [JsonPropertyName("int_page_from_feed_read_count")]
    public int? IntPageFromFeedReadCount { get; set; }

    /// <summary>获取或设置好友转发阅读人数（官方 <c>int_page_from_friends_read_user</c>）。</summary>
    [JsonPropertyName("int_page_from_friends_read_user")]
    public int? IntPageFromFriendsReadUser { get; set; }

    /// <summary>获取或设置好友转发阅读次数（官方 <c>int_page_from_friends_read_count</c>）。</summary>
    [JsonPropertyName("int_page_from_friends_read_count")]
    public int? IntPageFromFriendsReadCount { get; set; }

    /// <summary>获取或设置其他场景阅读人数（官方 <c>int_page_from_other_read_user</c>）。</summary>
    [JsonPropertyName("int_page_from_other_read_user")]
    public int? IntPageFromOtherReadUser { get; set; }

    /// <summary>获取或设置其他场景阅读次数（官方 <c>int_page_from_other_read_count</c>）。</summary>
    [JsonPropertyName("int_page_from_other_read_count")]
    public int? IntPageFromOtherReadCount { get; set; }

    /// <summary>获取或设置公众号会话转发朋友圈人数（官方 <c>feed_share_from_session_user</c>）。</summary>
    [JsonPropertyName("feed_share_from_session_user")]
    public int? FeedShareFromSessionUser { get; set; }

    /// <summary>获取或设置公众号会话转发朋友圈次数（官方 <c>feed_share_from_session_cnt</c>——<b>反直觉 _cnt 后缀</b>）。</summary>
    [JsonPropertyName("feed_share_from_session_cnt")]
    public int? FeedShareFromSessionCnt { get; set; }

    /// <summary>获取或设置朋友圈转发朋友圈人数（官方 <c>feed_share_from_feed_user</c>）。</summary>
    [JsonPropertyName("feed_share_from_feed_user")]
    public int? FeedShareFromFeedUser { get; set; }

    /// <summary>获取或设置朋友圈转发朋友圈次数（官方 <c>feed_share_from_feed_cnt</c>——<b>反直觉 _cnt 后缀</b>）。</summary>
    [JsonPropertyName("feed_share_from_feed_cnt")]
    public int? FeedShareFromFeedCnt { get; set; }

    /// <summary>获取或设置其他场景转发朋友圈人数（官方 <c>feed_share_from_other_user</c>）。</summary>
    [JsonPropertyName("feed_share_from_other_user")]
    public int? FeedShareFromOtherUser { get; set; }

    /// <summary>获取或设置其他场景转发朋友圈次数（官方 <c>feed_share_from_other_cnt</c>——<b>反直觉 _cnt 后缀</b>）。</summary>
    [JsonPropertyName("feed_share_from_other_cnt")]
    public int? FeedShareFromOtherCnt { get; set; }

    /// <summary>获取或设置看一看来源阅读人数（官方 <c>int_page_from_kanyikan_read_user</c>）。</summary>
    [JsonPropertyName("int_page_from_kanyikan_read_user")]
    public int? IntPageFromKanyikanReadUser { get; set; }

    /// <summary>获取或设置看一看来源阅读次数（官方 <c>int_page_from_kanyikan_read_count</c>）。</summary>
    [JsonPropertyName("int_page_from_kanyikan_read_count")]
    public int? IntPageFromKanyikanReadCount { get; set; }

    /// <summary>获取或设置搜一搜来源阅读人数（官方 <c>int_page_from_souyisou_read_user</c>）。</summary>
    [JsonPropertyName("int_page_from_souyisou_read_user")]
    public int? IntPageFromSouyisouReadUser { get; set; }

    /// <summary>获取或设置搜一搜来源阅读次数（官方 <c>int_page_from_souyisou_read_count</c>）。</summary>
    [JsonPropertyName("int_page_from_souyisou_read_count")]
    public int? IntPageFromSouyisouReadCount { get; set; }
}

/// <summary>
/// 图文群发总数据条目（官方 <c>getarticletotal</c> 的 list 元素；官方声明已停止维护）。
/// </summary>
/// <remarks>
/// 官方原文：「每天对应的数值为该文章到该日为止的<b>总量</b>（而不是当日的量）」；
/// 「最多统计发表日后 7 天数据」。官方文档缺陷（照录）：返回参数表未列出 details/msgid/title/ref_date
/// 字段行，仅出现在返回示例——SDK 按示例形态建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpArticleTotalItem
{
    /// <summary>获取或设置群发日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置图文消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置逐日累计统计（官方 <c>details</c>；每项为该文章到该日为止的总量）。</summary>
    [JsonPropertyName("details")]
    public List<MpArticleTotalStat>? Details { get; set; }
}

/// <summary>图文群发总数据（<c>getarticletotal</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpArticleTotalResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpArticleTotalItem>? List { get; set; }
}

/// <summary>
/// 图文阅读概况数据条目（官方 <c>getuserread</c> 的 list 元素；分时端点 <c>getuserreadhour</c> 多一个
/// ref_hour ⇒ 超集共用本 DTO；官方声明两接口已停止维护）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserReadItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置数据小时（官方 <c>ref_hour</c>；<b>仅分时端点返回</b>，日数据端点为 null）。</summary>
    [JsonPropertyName("ref_hour")]
    public int? RefHour { get; set; }

    /// <summary>获取或设置阅读来源渠道（官方 <c>user_source</c>：99999999 全部 / 0 会话 / 1 好友 / 2 朋友圈 / 4 历史消息页 / 5 其他 / 6 看一看 / 7 搜一搜）。</summary>
    [JsonPropertyName("user_source")]
    public int? UserSource { get; set; }

    /// <summary>获取或设置图文页阅读人数（官方 <c>int_page_read_user</c>）。</summary>
    [JsonPropertyName("int_page_read_user")]
    public int? IntPageReadUser { get; set; }

    /// <summary>获取或设置图文页阅读次数（官方 <c>int_page_read_count</c>）。</summary>
    [JsonPropertyName("int_page_read_count")]
    public int? IntPageReadCount { get; set; }

    /// <summary>获取或设置原文页阅读人数（官方 <c>ori_page_read_user</c>）。</summary>
    [JsonPropertyName("ori_page_read_user")]
    public int? OriPageReadUser { get; set; }

    /// <summary>获取或设置原文页阅读次数（官方 <c>ori_page_read_count</c>）。</summary>
    [JsonPropertyName("ori_page_read_count")]
    public int? OriPageReadCount { get; set; }

    /// <summary>获取或设置分享的人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置分享的次数（官方 <c>share_count</c>）。</summary>
    [JsonPropertyName("share_count")]
    public int? ShareCount { get; set; }

    /// <summary>获取或设置收藏的人数（官方 <c>add_to_fav_user</c>）。</summary>
    [JsonPropertyName("add_to_fav_user")]
    public int? AddToFavUser { get; set; }

    /// <summary>获取或设置收藏的次数（官方 <c>add_to_fav_count</c>）。</summary>
    [JsonPropertyName("add_to_fav_count")]
    public int? AddToFavCount { get; set; }
}

/// <summary>图文阅读概况数据（<c>getuserread</c> / <c>getuserreadhour</c> 共用响应——两页元素同构，hour 多 ref_hour）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserReadResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpUserReadItem>? List { get; set; }
}

/// <summary>
/// 图文转发概况数据条目（官方 <c>getusershare</c> 的 list 元素；分时端点 <c>getusersharehour</c> 多 ref_hour
/// ⇒ 超集共用；官方声明两接口已停止维护）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserShareItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置数据小时（官方 <c>ref_hour</c>；<b>仅分时端点返回</b>）。</summary>
    [JsonPropertyName("ref_hour")]
    public int? RefHour { get; set; }

    /// <summary>获取或设置分享的人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置分享的次数（官方 <c>share_count</c>）。</summary>
    [JsonPropertyName("share_count")]
    public int? ShareCount { get; set; }

    /// <summary>获取或设置分享的场景（官方 <c>share_scene</c>：1 好友转发 / 2 朋友圈 / 255 其他）。</summary>
    [JsonPropertyName("share_scene")]
    public int? ShareScene { get; set; }
}

/// <summary>图文转发概况数据（<c>getusershare</c> / <c>getusersharehour</c> 共用响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserShareResponse : MpResponse
{
    /// <summary>获取或设置数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpUserShareItem>? List { get; set; }
}
