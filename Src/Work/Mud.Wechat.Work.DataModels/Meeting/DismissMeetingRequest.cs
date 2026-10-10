// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 结束会议请求体（<c>/cgi-bin/meeting/realcontrol/dismiss</c>；结束一个进行中的会议）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class DismissMeetingRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置是否强制结束会议（默认值为 1）：0 - 不强制结束会议（会议中有参会者则无法强制结束）；
    /// 1 - 强制结束会议（会议中有参会者也会强制结束）。
    /// </summary>
    [JsonPropertyName("force_dismiss")]
    public int? ForceDismiss { get; set; }

    /// <summary>
    /// 获取或设置是否回收会议号（默认值为 0）：0 - 不回收会议号（可以重新入会）；1 - 回收会议号（不可重新入会）。
    /// <para>官方限制：周期性会议如果还有子会议，需设置为不回收会议号，否则会导致后续子会议无法正常进行；此字段对快速会议不生效，快速会议会强制收回会议号。</para>
    /// </summary>
    [JsonPropertyName("retrieve_code")]
    public int? RetrieveCode { get; set; }
}
