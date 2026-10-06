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
/// 会议变更事件载荷（<b>结构族</b>：覆盖 <c>Event = meeting_change</c> 的 18 个「平铺」<c>ChangeType</c>：
/// <c>modify_meeting</c> / <c>cancel_meeting</c> / <c>meeting_start</c> / <c>meeting_end</c> /
/// <c>meeting_mute_all</c> / <c>meeting_unmute_all</c> / <c>join_meeting</c> / <c>quit_meeting</c> /
/// <c>join_meeting_before_host</c> / <c>join_waiting_room</c> / <c>open_screen_share</c> /
/// <c>close_screen_share</c> / <c>start_recording</c> / <c>pause_recording</c> / <c>resume_recording</c> /
/// <c>stop_recording</c> / <c>recording_complete</c> / <c>delete_recording</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：18 个子事件的报文结构同构（信封 + <see cref="FromUserTmpOpenId"/> +
/// <see cref="MeetingId"/>），具体类别由信封 <c>ChangeType</c> 判别；携带独特节点的变更
/// （报名 <c>EnrollId</c>、等候室/角色类 <c>OperatedUser</c>、PSTN <c>PstnStatus</c>、
/// 会议室应答 <c>MeetingRoomId</c>/<c>MraAddress</c>、暖场/素材对象）分立其余载荷，
/// 避免超集稀释字段语义。
/// </para>
/// <para>
/// <b>两代信封并存（ADR-14 可空超集）</b>：修改/取消会议（99081/99082 与第三方 97451/代开发 97459）报文
/// 无 <see cref="FromUserTmpOpenId"/> 节点 ⇒ <c>null</c>；其余事件携带（会中临时 ID，仅成员操作事件有值）。
/// 信封 <c>FromUserName</c>：系统触发为 <c>sys</c>、成员操作仅企业成员有值（经 <c>evt.FromUserName</c> 读取）。
/// </para>
/// <para>
/// <b>开放面分组（同键不同面各标一条声明）</b>：官方第三方/代开发的会议回调通知仅覆盖
/// 「修改会议 / 取消会议」两事件（97451/97459 合页）⇒ 该两键三类应用开放；
/// 其余 16 键官方仅在企业自建文档树提供 ⇒ 仅自建。
/// </para>
/// <para>
/// <b>官方业务限制（不得弱化）</b>：仅 API 创建的会议触发；解除全体静音与全体静音并非成对出现、
/// 可能多次触发；成员等待主持人入会需预定会议时勾选「允许成员在主持人进会前加入会议选项」；
/// 成员入会为每个与会者各触发一次（高频事件，处理器须幂等）；云录制可由主持人/联席主持人手动开启，
/// 也可由企业管理员设置自动录制；停止云录制含「会议自动结束云录制」时机。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/99081">path 99081 修改会议事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/99082">path 99082 取消会议事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97451">path 97451（第三方，修改/取消合页）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97459">path 97459（代开发，同 97451）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98333">path 98333 会议开始事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98337">path 98337 会议结束事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98341">path 98341 会议全体静音事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98345">path 98345 会议解除全体静音事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98348">path 98348 成员入会事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98352">path 98352 成员离会事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98353">path 98353 成员等待主持人入会事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98354">path 98354 成员进入等候室事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98395">path 98395 共享屏幕开启事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98396">path 98396 共享屏幕结束事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98398">path 98398 开始云录制事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98399">path 98399 暂停云录制事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98400">path 98400 恢复云录制事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98401">path 98401 停止云录制事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98402">path 98402 云录制已完成事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98404">path 98404 删除云录制事件</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.ModifyMeeting,
        WechatCallbackEventTypes.CancelMeeting })]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.MeetingStart,
        WechatCallbackEventTypes.MeetingEnd,
        WechatCallbackEventTypes.MeetingMuteAll,
        WechatCallbackEventTypes.MeetingUnmuteAll,
        WechatCallbackEventTypes.JoinMeeting,
        WechatCallbackEventTypes.QuitMeeting,
        WechatCallbackEventTypes.JoinMeetingBeforeHost,
        WechatCallbackEventTypes.JoinWaitingRoom,
        WechatCallbackEventTypes.OpenScreenShare,
        WechatCallbackEventTypes.CloseScreenShare,
        WechatCallbackEventTypes.StartRecording,
        WechatCallbackEventTypes.PauseRecording,
        WechatCallbackEventTypes.ResumeRecording,
        WechatCallbackEventTypes.StopRecording,
        WechatCallbackEventTypes.RecordingComplete,
        WechatCallbackEventTypes.DeleteRecording })]
public sealed partial class MeetingChangedPayload : WechatCallbackPayload
{
    /// <summary>操作者会中临时 ID（官方 <c>FromUserTmpOpenId</c>；仅成员操作事件有值，
    /// 修改/取消会议等旧式报文与系统触发事件缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("FromUserTmpOpenId")]
    public string? FromUserTmpOpenId { get; set; }

    /// <summary>会议 ID（官方 <c>MeetingId</c>）。</summary>
    [PayloadField("MeetingId")]
    public string? MeetingId { get; set; }
}
