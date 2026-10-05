// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取用户专属参会链接请求体（<c>/cgi-bin/meeting/get_customer_short_url</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：该接口不支持个人会议号会议、网络研讨会（Webinar）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingCustomerShortUrlRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }
}
