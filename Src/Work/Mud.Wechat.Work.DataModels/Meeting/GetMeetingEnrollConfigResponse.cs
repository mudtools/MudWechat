// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议报名配置响应体（<c>/cgi-bin/meeting/enroll/get_config</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：会议未开启报名时返回未开启报名错误。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingEnrollConfigResponse : WechatWorkResponse
{
    /// <summary>获取或设置审批类型：1 - 自动审批；2 - 手动审批（默认自动审批）。</summary>
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

    /// <summary>获取或设置报名问题列表（详见 <see cref="MeetingEnrollQuestion"/>）。</summary>
    [JsonPropertyName("question_list")]
    public List<MeetingEnrollQuestion>? QuestionList { get; set; }
}
