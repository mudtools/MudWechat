// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 管理会中设置请求体（<c>/cgi-bin/meeting/realcontrol/set</c>；管理进行中会议的设置项）。
/// </summary>
/// <remarks>
/// <para>官方限制：目前暂不支持 MRA 设备作为被操作者的情况。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class SetMeetingRealtimeSettingsRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置是否全体静音：true - 全体静音；false - 关闭全体静音。</summary>
    [JsonPropertyName("mute_all")]
    public bool? MuteAll { get; set; }

    /// <summary>
    /// 获取或设置是否允许成员自己取消静音。
    /// <para>官方限制：请求参数 <see cref="MuteAll"/> 必传，且 <see cref="MuteAll"/> = true 时设置才生效。</para>
    /// </summary>
    [JsonPropertyName("allow_unmute_self")]
    public bool? AllowUnmuteSelf { get; set; }

    /// <summary>获取或设置成员入会静音：0 - 关闭静音；1 - 开启静音；2 - 超过 6 人自动开启静音。</summary>
    [JsonPropertyName("enable_enter_mute")]
    public int? EnableEnterMute { get; set; }

    /// <summary>获取或设置是否锁定会议：true - 锁定；false - 关闭锁定。</summary>
    [JsonPropertyName("meeting_locked")]
    public bool? MeetingLocked { get; set; }

    /// <summary>获取或设置隐藏会议号和密码：true - 隐藏；false - 不隐藏。</summary>
    [JsonPropertyName("hide_meeting_code_password")]
    public bool? HideMeetingCodePassword { get; set; }

    /// <summary>获取或设置允许参会者聊天设置：0 - 允许参会者自由聊天；1 - 仅允许参会者公开聊天；2 - 仅允许私聊主持人。</summary>
    [JsonPropertyName("allow_chat")]
    public int? AllowChat { get; set; }

    /// <summary>获取或设置是否允许参会者发起屏幕共享：true - 允许；false - 不允许。</summary>
    [JsonPropertyName("allow_share_screen")]
    public bool? AllowShareScreen { get; set; }

    /// <summary>获取或设置是否仅企业成员可入会：true - 仅企业成员可入会；false - 不限制。</summary>
    [JsonPropertyName("allow_external_user")]
    public bool? AllowExternalUser { get; set; }

    /// <summary>获取或设置成员入会是否播放提示音：true - 播放；false - 不播放。</summary>
    [JsonPropertyName("play_ivr_on_join")]
    public bool? PlayIvrOnJoin { get; set; }

    /// <summary>获取或设置是否开启等候室：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("enable_waiting_room")]
    public bool? EnableWaitingRoom { get; set; }
}
