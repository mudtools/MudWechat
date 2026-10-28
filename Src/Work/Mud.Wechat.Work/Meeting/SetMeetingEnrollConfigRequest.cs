// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 修改会议报名配置请求体（<c>/cgi-bin/meeting/enroll/set_config</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：需要会议已开启报名（<c>settings.enable_enroll</c> 为 true）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class SetMeetingEnrollConfigRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置审批类型：1 - 自动审批（默认）；2 - 手动审批。</summary>
    [JsonPropertyName("approve_type")]
    public int? ApproveType { get; set; }

    /// <summary>获取或设置是否收集问题：1 - 不收集（默认）；2 - 收集。</summary>
    [JsonPropertyName("is_collect_question")]
    public int? IsCollectQuestion { get; set; }

    /// <summary>
    /// 获取或设置本企业成员是否无需报名：true - 本企业成员无需报名；false - 默认配置（本企业成员及企业外成员需要报名）。
    /// </summary>
    [JsonPropertyName("no_registration_needed_for_staff")]
    public bool? NoRegistrationNeededForStaff { get; set; }

    /// <summary>
    /// 获取或设置报名问题列表（详见 <see cref="MeetingEnrollQuestion"/>）。
    /// <para>非特殊问题按传入的顺序排序，特殊问题会优先放在最前面；仅开启收集问题时有效。</para>
    /// </summary>
    [JsonPropertyName("question_list")]
    public List<MeetingEnrollQuestion>? QuestionList { get; set; }
}
