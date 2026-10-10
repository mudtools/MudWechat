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
/// 会话内容存档「产生会话回调事件」载荷（<c>msgaudit_notify</c>，官方 path 95039，仅企业自建应用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>事件语义</b>：为减少无效轮询，企业收到或发送新消息时推送本事件。
/// <b>回调间隔为 15 秒</b>——15 秒内有消息则触发回调，无消息则不触发。
/// </para>
/// <para>
/// <b>事件不携带消息内容</b>：信封之外仅 <see cref="AgentId"/> 一个业务节点；
/// 企业收到通知后须通过「获取会话内容」接口拉取数据。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建应用开发文档树提供该事件（第三方/代开发无对应事件回调）⇒ 仅自建开放；
/// 应用须在「会话内容存档」配置回调 URL 并开启接收。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/95039">path 95039 产生会话回调事件（企业自建）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.MsgAuditNotify })]
public sealed partial class MsgAuditNotifyPayload : WechatCallbackPayload
{
    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页查看；<c>Event</c> 信封即事件键，无 <c>ChangeType</c> 分组段）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }
}
