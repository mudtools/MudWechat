// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 会议设置（官方 meeting.option 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailMeetingOption
{
    /// <summary>获取或设置入会密码（官方 password；仅可输入 4-6 位纯数字）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>
    /// 获取或设置是否自动录制（官方 auto_record）：0 - 未开启（默认），1 - 自动本地录制，2 - 自动云录制。
    /// </summary>
    [JsonPropertyName("auto_record")]
    public int? AutoRecord { get; set; }

    /// <summary>获取或设置是否开启等候室（官方 enable_waiting_room，默认不开启）。</summary>
    [JsonPropertyName("enable_waiting_room")]
    public bool? EnableWaitingRoom { get; set; }

    /// <summary>获取或设置是否允许成员在主持人进会前加入（官方 allow_enter_before_host，默认允许）。</summary>
    [JsonPropertyName("allow_enter_before_host")]
    public bool? AllowEnterBeforeHost { get; set; }

    /// <summary>
    /// 获取或设置是否限制成员入会（官方 enter_restraint）：0 - 所有人可入会（默认），2 - 仅企业内部用户可入会。
    /// </summary>
    [JsonPropertyName("enter_restraint")]
    public int? EnterRestraint { get; set; }

    /// <summary>获取或设置是否开启屏幕水印（官方 enable_screen_watermark，默认不开启）。</summary>
    [JsonPropertyName("enable_screen_watermark")]
    public bool? EnableScreenWatermark { get; set; }

    /// <summary>
    /// 获取或设置入会是否静音（官方 enable_enter_mute）：1 - 开启，0 - 关闭，2 - 超过 6 人后自动开启（默认）。
    /// </summary>
    [JsonPropertyName("enable_enter_mute")]
    public int? EnableEnterMute { get; set; }

    /// <summary>
    /// 获取或设置会议开始时是否提醒（官方 remind_scope）：1 - 不提醒，2 - 仅提醒主持人（默认），3 - 提醒所有成员。
    /// </summary>
    [JsonPropertyName("remind_scope")]
    public int? RemindScope { get; set; }

    /// <summary>获取或设置水印类型（官方 water_mark_type）：0 - 单排水印（默认），1 - 多排水印。</summary>
    [JsonPropertyName("water_mark_type")]
    public int? WaterMarkType { get; set; }
}
