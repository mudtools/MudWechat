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
/// 微信客服新消息通知载荷（<c>kf_msg_or_event</c>；官方 94670/94699/96426）。
/// </summary>
/// <remarks>
/// <para>
/// <b>外层通知 + 拉取对齐</b>：官方推送的外层报文仅携带 <see cref="Token"/> 与 <see cref="OpenKfId"/>
/// 两个节点，具体消息/事件内容（msg_list：text/image/enter_session/session_status_change 等）
/// <b>不在回调报文里</b> —— 处理器须持 <see cref="Token"/> 调
/// <c>POST /cgi-bin/kf/sync_msg</c> 拉取（可指定 <c>open_kfid</c> 与 <c>cursor</c>；
/// 内容保留最近 3 天，官方建议以拉取接口为准做时序对齐）。
/// </para>
/// <para>
/// <b>官方开放面</b>：三类应用均可接收 —— 自建应用配置到「微信客服-可调用接口的应用」并授权客服账号；
/// 第三方/代开发需「微信客服→管理账号、分配会话和收发消息」权限。
/// 接收前置：客服账号须设置为 API 管理、接待人员需在应用可见范围内；
/// 2023-12-01 起不再支持系统应用 secret 调用。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/94670">path 94670 接收消息和事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/94699">path 94699（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96426">path 96426（服务商代开发）</see>
/// （三份报文结构逐字一致，ADR-14）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.KfEvent,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.KfMsgOrEvent })]
public sealed partial class KfMsgOrEventPayload : WechatCallbackPayload
{
    /// <summary>
    /// 拉取消息校验令牌（官方 <c>Token</c>）：调用 sync_msg 拉取接口时用于校验合法性，
    /// <b>10 分钟内有效</b> —— 处理器应及时消费，不得缓存或落盘。
    /// </summary>
    [PayloadField("Token")]
    public string? Token { get; set; }

    /// <summary>有新消息的客服账号（官方 <c>OpenKfId</c>，<c>wk</c> 前缀）；sync_msg 拉取时以 <c>open_kfid</c> 传入。</summary>
    [PayloadField("OpenKfId")]
    public string? OpenKfId { get; set; }
}
