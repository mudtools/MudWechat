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
/// 平铺型事件载荷（信封之外仅 <c>EventKey</c>/<c>AgentID</c>；官方 path 90240）。
/// 覆盖 <c>subscribe</c> / <c>unsubscribe</c> / <c>enter_agent</c> / <c>click</c> / <c>view</c> /
/// <c>view_miniprogram</c> / <c>share_agent_change</c> / <c>share_chain_change</c> /
/// <c>close_inactive_agent</c> / <c>reopen_inactive_agent</c> / <c>low_active</c> / <c>active_restored</c> 共 12 键。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：这 12 个事件的官方报文<b>字段集合一致</b>（均为
/// 「信封 + <c>EventKey</c> + <c>AgentID</c>」，部分报文不携带 <c>EventKey</c> 节点 ⇒ 解析为 <c>null</c>），
/// 具体类别由信封 <c>Event</c> 判别 ⇒ 合并为一个载荷。
/// <c>click</c>/<c>view</c>/<c>view_miniprogram</c> 的 <c>EventKey</c> 承载菜单 KEY / URL / 小程序路径；
/// 其余事件官方参数表标注「此事件该值为空」⇒ 恒为 <c>null</c>（原始报文值仍可经 <c>Values</c> 读取）。
/// </para>
/// <para>
/// <b>开放面在同一载荷内分组登记</b>：<c>share_agent_change</c>/<c>share_chain_change</c> 官方触发时机为
/// 「共享<b>自建应用</b>」⇒ 仅企业自建应用，其余 10 键三类应用均开放（由 <c>OfficialPayloadContracts</c>
/// 的两段登记承载，载荷类型本身不区分模式 —— ADR-14 三模式无关性）。
/// </para>
/// <para>
/// <b>权限分层</b>：无授权差异；处理器<b>不得</b>假设 <c>EventKey</c> 必有值。
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
        WechatCallbackEventTypes.Subscribe,
        WechatCallbackEventTypes.Unsubscribe,
        WechatCallbackEventTypes.EnterAgent,
        WechatCallbackEventTypes.Click,
        WechatCallbackEventTypes.View,
        WechatCallbackEventTypes.ViewMiniProgram,
        WechatCallbackEventTypes.CloseInactiveAgent,
        WechatCallbackEventTypes.ReopenInactiveAgent,
        WechatCallbackEventTypes.LowActive,
        WechatCallbackEventTypes.ActiveRestored })]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.ShareAgentChange,
        WechatCallbackEventTypes.ShareChainChange })]
public sealed partial class PlainEventPayload : WechatCallbackPayload
{
    /// <summary>
    /// 事件 KEY 值（官方 <c>EventKey</c>：菜单 KEY / 跳转 URL / 小程序路径；
    /// 关注、进入应用、共享与应用状态类事件官方标注为空 ⇒ <c>null</c>）。
    /// </summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页面查看）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }
}
