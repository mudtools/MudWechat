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
/// 应用预警类事件载荷（<c>inactive_alert</c> / <c>low_active_alert</c> 两键；官方 path 90240）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族</b>：两事件的官方报文字段集合一致（信封 + <c>EffectTime</c> + <c>AgentID</c>），
/// 具体类别由信封 <c>Event</c> 判别 —— <c>inactive_alert</c> 为长期未使用应用停用预警，
/// <c>low_active_alert</c> 为应用低活跃预警（即将限制客户数据访问）。
/// </para>
/// <para>
/// 与 <see cref="PlainEventPayload"/> 分立的原因是<b>字段集合不同</b>（本载荷无 <c>EventKey</c>、
/// 多 <c>EffectTime</c>），按指南 §0 的「同构 = 字段集合一致」判据不合并。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 接收消息与事件（企业内部开发）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/90376">path 90376 接收消息与事件（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96468">path 96468 接收消息与事件（服务商代开发）</see>
/// （三份正文逐字一致，ADR-14）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.InactiveAlert,
        WechatCallbackEventTypes.LowActiveAlert })]
public sealed partial class AgentAlertPayload : WechatCallbackPayload
{
    /// <summary>生效时间戳（官方 <c>EffectTime</c>，秒级 Unix 时间戳）。</summary>
    [PayloadField("EffectTime")]
    public long? EffectTime { get; set; }

    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页面查看）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }
}
