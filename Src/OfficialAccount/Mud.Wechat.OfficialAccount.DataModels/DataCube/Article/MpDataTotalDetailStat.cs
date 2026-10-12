// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

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
