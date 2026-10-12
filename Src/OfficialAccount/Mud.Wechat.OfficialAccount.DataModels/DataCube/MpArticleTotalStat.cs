// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

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
