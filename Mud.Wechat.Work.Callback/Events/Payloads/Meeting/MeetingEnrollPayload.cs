// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>会议报名事件载荷（<b>结构族</b>：覆盖 <c>enroll</c> / <c>cancel_enroll</c>；官方 path 98781/98782 自建）。</summary>
/// <remarks>
/// <para>
/// <b>结构族</b>：两事件报文结构一致（信封 + <see cref="EnrollId"/>），具体类别由信封 <c>ChangeType</c> 判别；
/// 适用于 API 创建的会议或网络研讨会的用户报名/取消报名。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建文档树提供该事件（第三方/代开发无对应回调）⇒ 仅自建开放。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/98781">path 98781 用户报名事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98782">path 98782 用户取消报名事件（企业自建）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.Enroll,
        WechatCallbackEventTypes.CancelEnroll })]
public sealed partial class MeetingEnrollPayload : WechatCallbackPayload
{
    /// <summary>操作者会中临时 ID（官方 <c>FromUserTmpOpenId</c>；仅成员操作事件有值，系统触发缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("FromUserTmpOpenId")]
    public string? FromUserTmpOpenId { get; set; }

    /// <summary>会议 ID（官方 <c>MeetingId</c>）。</summary>
    [PayloadField("MeetingId")]
    public string? MeetingId { get; set; }

    /// <summary>报名 ID（官方 <c>EnrollId</c>）。</summary>
    [PayloadField("EnrollId")]
    public string? EnrollId { get; set; }
}
