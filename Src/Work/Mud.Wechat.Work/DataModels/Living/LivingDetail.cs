// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 直播详情信息（获取直播详情响应 <c>living_info</c> 字段）。
/// </summary>
/// <remarks>
/// <para>本类型与「家校沟通·上课直播域」（School 模块）的 <c>School.Living.LivingInfo</c> 同源于官方
/// <c>living_info</c> 结构但字段集不同构（本域多承载预约信息、回放状态、直播类型与在线人数等），
/// 且源生成上下文按类型简单名生成元数据（同上下文禁止同名根类型），故命名 <see cref="LivingDetail"/> 以消歧。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class LivingDetail
{
    /// <summary>获取或设置直播主题。</summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; set; }

    /// <summary>获取或设置直播开始时间戳（单位秒）。</summary>
    [JsonPropertyName("living_start")]
    public long? LivingStart { get; set; }

    /// <summary>获取或设置直播时长（单位秒）。</summary>
    [JsonPropertyName("living_duration")]
    public int? LivingDuration { get; set; }

    /// <summary>
    /// 获取或设置直播的状态：0 - 预约中；1 - 直播中；2 - 已结束；3 - 已过期；4 - 已取消。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置直播预约的开始时间戳（单位秒）。</summary>
    [JsonPropertyName("reserve_start")]
    public long? ReserveStart { get; set; }

    /// <summary>获取或设置直播预约时长（单位秒）。</summary>
    [JsonPropertyName("reserve_living_duration")]
    public int? ReserveLivingDuration { get; set; }

    /// <summary>获取或设置直播的描述（最多支持 100 个汉字）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置主播的 userid。</summary>
    [JsonPropertyName("anchor_userid")]
    public string? AnchorUserid { get; set; }

    /// <summary>获取或设置主播所在主部门 id。</summary>
    [JsonPropertyName("main_department")]
    public int? MainDepartment { get; set; }

    /// <summary>获取或设置观看直播总人数。</summary>
    [JsonPropertyName("viewer_num")]
    public int? ViewerNum { get; set; }

    /// <summary>获取或设置评论数。</summary>
    [JsonPropertyName("comment_num")]
    public int? CommentNum { get; set; }

    /// <summary>获取或设置连麦发言人数。</summary>
    [JsonPropertyName("mic_num")]
    public int? MicNum { get; set; }

    /// <summary>获取或设置是否开启回放：1 表示开启，0 表示关闭。</summary>
    [JsonPropertyName("open_replay")]
    public int? OpenReplay { get; set; }

    /// <summary>获取或设置回放状态（open_replay 为 1 时才返回）：0 - 生成成功；1 - 生成中；2 - 回放已删除；3 - 生成失败。</summary>
    [JsonPropertyName("replay_status")]
    public int? ReplayStatus { get; set; }

    /// <summary>获取或设置直播类型：0 - 通用直播；1 - 小班课；2 - 大班课；3 - 企业培训；4 - 活动直播。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置推流地址（仅活动直播且状态待开播时返回）。</summary>
    [JsonPropertyName("push_stream_url")]
    public string? PushStreamUrl { get; set; }

    /// <summary>获取或设置当前在线观看人数。</summary>
    [JsonPropertyName("online_count")]
    public int? OnlineCount { get; set; }

    /// <summary>获取或设置直播预约人数。</summary>
    [JsonPropertyName("subscribe_count")]
    public int? SubscribeCount { get; set; }
}
