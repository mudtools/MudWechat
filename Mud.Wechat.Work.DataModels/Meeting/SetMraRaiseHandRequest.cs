// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 设置 MRA 举手或手放下请求体（<c>/cgi-bin/meeting/mra/set_raise_hand</c>；API 创建的会议中对 MRA 进行举手和手放下操作）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class SetMraRaiseHandRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置 MRA 设备举手操作（官方必填）：true - 举手；false - 手放下。</summary>
    [JsonPropertyName("raise_hand")]
    public bool? RaiseHand { get; set; }

    /// <summary>获取或设置被操作 MRA 设备（官方必填，详见 <see cref="MraDeviceRef"/>）。</summary>
    [JsonPropertyName("mra")]
    public MraDeviceRef? Mra { get; set; }
}
