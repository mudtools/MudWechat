// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议配置对象（创建/修改预约会议请求与获取会议详情响应 <c>settings</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingSettings
{
    /// <summary>获取或设置入会密码（仅支持 4-6 位纯数字）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>获取或设置是否开启等候室（默认不开启）。</summary>
    [JsonPropertyName("enable_waiting_room")]
    public bool? EnableWaitingRoom { get; set; }

    /// <summary>获取或设置是否允许成员在主持人进会前加入（默认允许）。</summary>
    [JsonPropertyName("allow_enter_before_host")]
    public bool? AllowEnterBeforeHost { get; set; }

    /// <summary>
    /// 获取或设置会议开始时来电提醒方式：1 - 不提醒；2 - 仅提醒主持人；3 - 提醒所有成员；4 - 指定部分成员响铃。
    /// <para>创建预约会议文档页默认值为 2（仅提醒主持人）；获取会议详情文档页默认值为 3（提醒所有成员）。</para>
    /// </summary>
    [JsonPropertyName("remind_scope")]
    public int? RemindScope { get; set; }

    /// <summary>获取或设置成员入会时是否静音：1 - 开启；0 - 关闭；2 - 超过 6 人自动开启（默认）。</summary>
    [JsonPropertyName("enable_enter_mute")]
    public int? EnableEnterMute { get; set; }

    /// <summary>获取或设置是否开启屏幕水印（默认不开启）。</summary>
    [JsonPropertyName("enable_screen_watermark")]
    public bool? EnableScreenWatermark { get; set; }

    /// <summary>获取或设置主持人列表（最多 10 个，详见 <see cref="MeetingHosts"/>）。</summary>
    [JsonPropertyName("hosts")]
    public MeetingHosts? Hosts { get; set; }

    /// <summary>获取或设置指定响铃成员列表（详见 <see cref="MeetingRingUsers"/>）。</summary>
    [JsonPropertyName("ring_users")]
    public MeetingRingUsers? RingUsers { get; set; }
}
