// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>会议室应答事件载荷（<c>meeting_room_response</c>；官方 path 98783 自建）。</summary>
/// <remarks>
/// <para>
/// API 创建的会议对会议室发起的呼叫，当有应答结果时推送。系统触发 ⇒ 信封 <c>FromUserName</c> 固定 <c>sys</c>。
/// </para>
/// <para>
/// <b>二选一节点</b>：<see cref="MeetingRoomId"/>（会议室）与 <see cref="MraAddress"/>（MRA 设备）
/// 官方声明二选一携带 —— 处理器不得假设两者必有其一同时出现，也不得假设必有值。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建文档树提供该事件（第三方/代开发无对应回调）⇒ 仅自建开放。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/98783">path 98783 会议室应答事件（企业自建）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.MeetingRoomResponse })]
public sealed partial class MeetingRoomResponsePayload : WechatCallbackPayload
{
    /// <summary>操作者会中临时 ID（官方 <c>FromUserTmpOpenId</c>；本事件为系统触发 ⇒ 恒为 <c>null</c>，参数表照官方原文保留该字段）。</summary>
    [PayloadField("FromUserTmpOpenId")]
    public string? FromUserTmpOpenId { get; set; }

    /// <summary>会议 ID（官方 <c>MeetingId</c>）。</summary>
    [PayloadField("MeetingId")]
    public string? MeetingId { get; set; }

    /// <summary>会议室 ID（官方 <c>MeetingRoomId</c>；与 <see cref="MraAddress"/> 二选一）。</summary>
    [PayloadField("MeetingRoomId")]
    public string? MeetingRoomId { get; set; }

    /// <summary>MRA 设备（官方 <c>MraAddress</c>：Protocol + DialString；与 <see cref="MeetingRoomId"/> 二选一，节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("MraAddress")]
    public WechatCallbackMeetingRoomAddress? MraAddress { get; set; }

    /// <summary>
    /// 应答结果（官方 <c>RoomResponseStatus</c>，闭合值域）：
    /// 0 无应答 / 2 入会中 / 3 被拒绝 / 5 取消呼叫。
    /// </summary>
    [PayloadField("RoomResponseStatus")]
    public long? RoomResponseStatus { get; set; }
}
