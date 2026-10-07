// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 订阅通知项（官方 <c>List</c> 元素，V3 已核验）：三键共用（字段为并集，未携带者为 <c>null</c>）。
/// </summary>
/// <remarks>
/// 项元素名官方固定为 <c>List</c>，且同名兄弟在节点树上可能被合并为「同名容器」⇒ 经
/// <see cref="MpPayloadConverter.RepeatSubscribeMsgItems"/> 统一读取（单/多项两形态皆可）。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpSubscribeMsgItem
{
    /// <summary>模板 id（官方 <c>TemplateId</c>；一次订阅可能有多条通知）。</summary>
    [PayloadField("TemplateId")]
    public string? TemplateId { get; set; }

    /// <summary>用户点击行为（官方 <c>SubscribeStatusString</c>：<c>accept</c> 同意 / <c>reject</c> 取消）。</summary>
    [PayloadField("SubscribeStatusString")]
    public string? SubscribeStatusString { get; set; }

    /// <summary>弹窗场景（官方 <c>PopupScene</c>：1 = H5 页面 / 2 = 图文消息；**仅 popup 事件携带**）。</summary>
    [PayloadField("PopupScene")]
    public string? PopupScene { get; set; }

    /// <summary>消息 id（官方 <c>MsgID</c>；**仅 sent 事件携带**，注意官方拼写为 MsgID）。</summary>
    [PayloadField("MsgID")]
    public string? MsgId { get; set; }

    /// <summary>推送结果状态码（<c>ErrorCode</c>，0 表示成功；**仅 sent 事件携带**）。</summary>
    [PayloadField("ErrorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>推送结果状态码文字含义（官方 <c>ErrorStatus</c>，如 <c>success</c>；**仅 sent 事件携带**）。</summary>
    [PayloadField("ErrorStatus")]
    public string? ErrorStatus { get; set; }
}

/// <summary>
/// 用户操作订阅通知弹窗事件载荷（<c>subscribe_msg_popup_event</c>；V3 已核验）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpSubscriptionEventTypes.Popup })]
public sealed partial class MpSubscribeMsgPopupPayload : MpCallbackPayload
{
    /// <summary>订阅通知项列表（官方外层节点 <c>SubscribeMsgPopupEvent</c> 内的多个 <c>List</c>）。</summary>
    [PayloadField("SubscribeMsgPopupEvent", Method = nameof(MpPayloadConverter.RepeatSubscribeMsgItems))]
    public List<MpSubscribeMsgItem> Items { get; set; } = new List<MpSubscribeMsgItem>();
}

/// <summary>
/// 用户管理订阅通知事件载荷（<c>subscribe_msg_change_event</c>；V3 已核验；项内无 <c>PopupScene</c>）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpSubscriptionEventTypes.Change })]
public sealed partial class MpSubscribeMsgChangePayload : MpCallbackPayload
{
    /// <summary>订阅通知项列表（官方外层节点 <c>SubscribeMsgChangeEvent</c>）。</summary>
    [PayloadField("SubscribeMsgChangeEvent", Method = nameof(MpPayloadConverter.RepeatSubscribeMsgItems))]
    public List<MpSubscribeMsgItem> Items { get; set; } = new List<MpSubscribeMsgItem>();
}

/// <summary>
/// 发送订阅通知结果事件载荷（<c>subscribe_msg_sent_event</c>；V3 已核验；项内带 <c>MsgID</c>/<c>ErrorCode</c>/<c>ErrorStatus</c>）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpSubscriptionEventTypes.Sent })]
public sealed partial class MpSubscribeMsgSentPayload : MpCallbackPayload
{
    /// <summary>订阅通知项列表（官方外层节点 <c>SubscribeMsgSentEvent</c>）。</summary>
    [PayloadField("SubscribeMsgSentEvent", Method = nameof(MpPayloadConverter.RepeatSubscribeMsgItems))]
    public List<MpSubscribeMsgItem> Items { get; set; } = new List<MpSubscribeMsgItem>();
}

/// <summary>
/// 微信认证成功类事件载荷（<c>qualification_verify_success</c> / <c>naming_verify_success</c> /
/// <c>annual_renew</c> / <c>verify_expired</c>；V4 已核验，四键字段集一致 = <c>ExpiredTime</c>）。
/// </summary>
/// <remarks>
/// 语义差异由事件键判别：成功类 ⇒ 已获权限/打勾；<c>annual_renew</c> ⇒ 需尽快年审；
/// <c>verify_expired</c> ⇒ 已过期、需重新认证。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpVerificationEventTypes.QualificationVerifySuccess,
    MpVerificationEventTypes.NamingVerifySuccess,
    MpVerificationEventTypes.AnnualRenew,
    MpVerificationEventTypes.VerifyExpired,
})]
public sealed partial class MpVerificationEventPayload : MpCallbackPayload
{
    /// <summary>
    /// 认证有效期（官方 <c>ExpiredTime</c>，整型时间戳）：成功类为「将于该时间戳过期」，
    /// <c>annual_renew</c> 为「需尽快年审」，<c>verify_expired</c> 为「已于该时间戳过期」。
    /// </summary>
    [PayloadField("ExpiredTime")]
    public long? ExpiredTime { get; set; }
}

/// <summary>
/// 微信认证失败类事件载荷（<c>qualification_verify_fail</c> / <c>naming_verify_fail</c>；V4 已核验）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpVerificationEventTypes.QualificationVerifyFail,
    MpVerificationEventTypes.NamingVerifyFail,
})]
public sealed partial class MpVerificationFailEventPayload : MpCallbackPayload
{
    /// <summary>失败发生时间（官方 <c>FailTime</c>，整型时间戳）。</summary>
    [PayloadField("FailTime")]
    public long? FailTime { get; set; }

    /// <summary>认证失败原因（官方 <c>FailReason</c>，示例值 <c>by time</c>）。</summary>
    [PayloadField("FailReason")]
    public string? FailReason { get; set; }
}
