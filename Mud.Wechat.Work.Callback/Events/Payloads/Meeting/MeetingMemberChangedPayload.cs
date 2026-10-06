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
/// 会议成员级变更事件载荷（<b>结构族</b>：覆盖带 <c>OperatedUser</c> 对象的 5 个 <c>ChangeType</c>：
/// <c>quit_waiting_room</c> / <c>join_from_meeting_room</c> / <c>move_to_waiting_room</c> /
/// <c>role_change</c> / <c>webinar_role_change</c>；官方 98355/98393/98394/98397/98771 自建）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：5 个子事件的结构同构（信封 + <see cref="FromUserTmpOpenId"/> +
/// <see cref="OperatedUser"/> + <see cref="MeetingId"/>），具体类别由信封 <c>ChangeType</c> 判别。
/// </para>
/// <para>
/// <b>操作者与被操作者</b>：操作者经信封 <c>FromUserName</c>/<c>FromUserTmpOpenId</c> 承载
/// （系统触发为 <c>sys</c>）；<see cref="OperatedUser"/> 为被操作者（移出/允许入会/移入等候室的对象，
/// 或角色变更的成员）。
/// </para>
/// <para>
/// <b>可空超集（ADR-14）</b>：<see cref="WechatCallbackMeetingOperatedUser.UserRole"/> 仅
/// <c>role_change</c>/<c>webinar_role_change</c> 携带，等候室类事件缺失 ⇒ <c>null</c>。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建文档树提供该事件（第三方/代开发无对应回调）⇒ 仅自建开放。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/98355">path 98355 成员离开等候室事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98393">path 98393 成员从等候室进入会议事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98394">path 98394 成员从会议中被移入等候室事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98397">path 98397 会议成员角色变更事件</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98771">path 98771 网络研讨会成员角色变更事件</see>（均企业自建）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.QuitWaitingRoom,
        WechatCallbackEventTypes.JoinFromMeetingRoom,
        WechatCallbackEventTypes.MoveToWaitingRoom,
        WechatCallbackEventTypes.RoleChange,
        WechatCallbackEventTypes.WebinarRoleChange })]
public sealed partial class MeetingMemberChangedPayload : WechatCallbackPayload
{
    /// <summary>操作者会中临时 ID（官方 <c>FromUserTmpOpenId</c>；仅成员操作事件有值，系统触发缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("FromUserTmpOpenId")]
    public string? FromUserTmpOpenId { get; set; }

    /// <summary>被操作者（官方 <c>OperatedUser</c>：UserId + TmpOpenId，角色变更事件另含 UserRole；节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("OperatedUser")]
    public WechatCallbackMeetingOperatedUser? OperatedUser { get; set; }

    /// <summary>会议 ID（官方 <c>MeetingId</c>）。</summary>
    [PayloadField("MeetingId")]
    public string? MeetingId { get; set; }
}
