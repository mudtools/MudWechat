// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取网络研讨会报名信息响应体（<c>/cgi-bin/meeting/webinar/enroll/list</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档陷阱：官方示例将 <c>enroll_id</c> 展示为数字形态（1386442），参数表与普通会议报名文档均为字符串，本模型按字符串承载（结构同 <see cref="MeetingEnrollInfo"/>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ListWebinarEnrollsResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否还有待拉取的成员列表。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置分页游标（下一次拉取列表将该字段填入 <c>cursor</c> 字段）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置当前页的报名列表（结构同 <see cref="MeetingEnrollInfo"/>）。</summary>
    [JsonPropertyName("enroll_list")]
    public List<MeetingEnrollInfo>? EnrollList { get; set; }
}
