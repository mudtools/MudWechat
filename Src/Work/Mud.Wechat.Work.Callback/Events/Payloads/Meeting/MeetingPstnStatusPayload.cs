// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>PSTN 外呼状态更新事件载荷（<c>pstn_status_update</c>；官方 path 98774 自建）。</summary>
/// <remarks>
/// <para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建文档树提供该事件（第三方/代开发无对应回调）⇒ 仅自建开放。
/// </para>
/// 仅 API 创建的会议触发；当 PSTN 外呼的状态发生变化时推送。
/// </para>
/// <para>
/// <b>官方拼写陷阱（不得弱化）</b>：<see cref="PstnStatus"/> 的官方枚举值
/// <c>CANCLE_INVITE</c>（呼叫中邀请人主动取消）为官方原文拼写（非 CANCEL），本 SDK 照抄原文，
/// 处理器做值比较时须以官方拼写为准。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/98774">path 98774 PSTN 外呼状态更新事件（企业自建）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.PstnStatusUpdate })]
public sealed partial class MeetingPstnStatusPayload : WechatCallbackPayload
{
    /// <summary>操作者会中临时 ID（官方 <c>FromUserTmpOpenId</c>；仅成员操作事件有值，系统触发缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("FromUserTmpOpenId")]
    public string? FromUserTmpOpenId { get; set; }

    /// <summary>会议 ID（官方 <c>MeetingId</c>）。</summary>
    [PayloadField("MeetingId")]
    public string? MeetingId { get; set; }

    /// <summary>
    /// PSTN 外呼状态（官方 <c>PstnStatus</c>，闭合值域）：
    /// <c>START_INVITE</c> 开始邀请 / <c>START_ACCEPT</c> 用户接听 /
    /// <c>LEAVE_WITHOUT_ACCEPT</c> 用户无接听挂断或拒绝 / <c>LEAVE_WITH_ACCEPTED</c> 用户已接听挂断 /
    /// <c>CANCLE_INVITE</c> 呼叫中邀请人主动取消（官方原文拼写即缺一个 E）。
    /// </summary>
    [PayloadField("PstnStatus")]
    public string? PstnStatus { get; set; }
}
