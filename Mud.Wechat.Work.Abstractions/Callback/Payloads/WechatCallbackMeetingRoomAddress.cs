// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 会议「MRA 设备地址」对象（<c>meeting_room_response</c> 事件的 <c>MraAddress</c> 节点；官方 path 98783）。
/// </summary>
/// <remarks>
/// 经上游 G-ADR-17 的 <c>Object</c> 嵌套通道声明化：单节点对象（非列表），元素缺失 ⇒ <c>null</c>。
/// 官方 <c>MraAddress</c> 与 <c>MeetingRoomId</c> 为二选一携带。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackMeetingRoomAddress
{
    /// <summary>信令协议（官方 <c>MraAddress/Protocol</c>：1 SIP / 2 H.323）。</summary>
    [PayloadField("Protocol")]
    public long? Protocol { get; set; }

    /// <summary>信令地址（官方 <c>MraAddress/DialString</c>）。</summary>
    [PayloadField("DialString")]
    public string? DialString { get; set; }
}
