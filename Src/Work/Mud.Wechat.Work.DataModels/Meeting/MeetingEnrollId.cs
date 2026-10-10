// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议成员报名 ID 对象（获取会议成员报名 ID 响应 <c>enroll_id_list</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>仅返回已报名成员的报名 ID；若传入的成员无人报名，则响应不含该字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingEnrollId
{
    /// <summary>获取或设置当场会议的成员临时 ID（适用于所有成员）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>获取或设置报名 ID（成员的报名 ID 每场会议是唯一的）。</summary>
    [JsonPropertyName("enroll_id")]
    public string? EnrollId { get; set; }
}
