// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 批量外呼请求体（<c>/cgi-bin/meeting/phone/callout</c>；创建批量电话入会呼叫）。
/// </summary>
/// <remarks>
/// <para>官方限制：支持在会议未开始、会中外呼；每次调用支持批量外呼 50 路；支持境外电话号及分机号的外呼；Webinar 暂不支持外呼。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class PstnBatchCalloutRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置外呼的电话号码对象数组（官方必填，详见 <see cref="PstnPhoneNumber"/>）。</summary>
    [JsonPropertyName("phone_numbers")]
    public List<PstnPhoneNumber>? PhoneNumbers { get; set; }
}
