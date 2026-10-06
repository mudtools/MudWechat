// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 会议统计事件载荷（<b>结构族</b>：覆盖 <c>Event = meeting_statistics</c> 的 <c>start_meeting</c>；官方 path 99648 自建）。
/// </summary>
/// <remarks>
/// <para>
/// <b>独立 Event 值</b>：本族 <c>Event = meeting_statistics</c>，与会议变更族
/// （<c>meeting_change</c>，见 <see cref="MeetingChangedPayload"/>）是两个独立的信封 <c>Event</c> 值，
/// 事件键（<c>start_meeting</c>）仍取 <c>ChangeType</c> 节点。
/// </para>
/// <para>
/// <b>触发边界</b>：应用可见范围内的成员发起快速会议，或作为首位参与者进入预约会议时触发。
/// 信封 <c>FromUserName</c> 为会议发起者的 UserID；企业外部成员作为首位参与者进入预约会议时，
/// 为该预约会议创建者的 UserID（信封字段）。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建文档树提供该事件（第三方/代开发无对应回调）⇒ 仅自建开放。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/99648">path 99648 会议发起事件（企业自建）</see>
/// （官方样例 <c>ChangeType</c> 的 CDATA 闭合缺 <c>&gt;</c>，官方原文如此，键值以参数表为准）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingStatistics,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.StartMeeting })]
public sealed partial class MeetingStatisticsPayload : WechatCallbackPayload
{
    /// <summary>会议状态（官方 <c>Status</c>：1 会议发起成功 / 2 会议发起失败）。</summary>
    [PayloadField("Status")]
    public long? Status { get; set; }
}
