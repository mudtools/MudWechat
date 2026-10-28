// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议报名信息对象（获取会议报名信息响应 <c>enroll_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingEnrollInfo
{
    /// <summary>获取或设置报名 ID。</summary>
    [JsonPropertyName("enroll_id")]
    public string? EnrollId { get; set; }

    /// <summary>获取或设置报名时间（utc +8，非时间戳，如 "2023/03/23 14:00"）。</summary>
    [JsonPropertyName("enroll_time")]
    public string? EnrollTime { get; set; }

    /// <summary>获取或设置报名来源：1 - 成员手动报名；2 - 批量导入报名。</summary>
    [JsonPropertyName("enroll_source_type")]
    public int? EnrollSourceType { get; set; }

    /// <summary>获取或设置昵称（若通过手机号导入报名且未设置昵称，则该字段显示手机号）。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }

    /// <summary>获取或设置报名状态：1 - 待审批；2 - 已拒绝；3 - 已批准。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置企业内成员 userid（为企业内成员时返回）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置当场会议的成员临时 ID。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>获取或设置报名码（用于电话入会的凭证）。</summary>
    [JsonPropertyName("enroll_code")]
    public string? EnrollCode { get; set; }

    /// <summary>获取或设置答题对象列表（详见 <see cref="MeetingEnrollAnswer"/>）。</summary>
    [JsonPropertyName("answer_list")]
    public List<MeetingEnrollAnswer>? AnswerList { get; set; }
}
