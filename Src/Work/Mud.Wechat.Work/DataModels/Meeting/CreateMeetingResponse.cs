// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 创建预约会议响应体（<c>/cgi-bin/meeting/create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CreateMeetingResponse : WechatWorkResponse
{
    /// <summary>获取或设置会议 ID（可用于调用「进入会议」接口，通过小程序和 JS-SDK 提供入会入口）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置参会人中包含无效会议账号的 userid 列表。
    /// <para>仅在购买会议专业版的企业且参会人中存在无有效会议账号的用户时返回。</para>
    /// </summary>
    [JsonPropertyName("excess_users")]
    public List<string>? ExcessUsers { get; set; }

    /// <summary>获取或设置会议的会议号（仅预约会议高级管理文档页声明该字段）。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置入会链接（仅预约会议高级管理文档页声明该字段）。</summary>
    [JsonPropertyName("meeting_link")]
    public string? MeetingLink { get; set; }
}
