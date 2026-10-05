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
/// 应用版本付费订单回调事件载荷（<b>结构族</b>：覆盖官方事件键 <c>open_order</c> / <c>change_order</c> /
/// <c>pay_for_app_success</c> / <c>refund</c> / <c>change_editon</c> / <c>cancel_order</c> 六键；
/// 官方 91929~91933 / 99353，经<b>指令回调 URL</b> 以套件信封推送，外层事件值在 <c>InfoType</c> 节点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何一个载荷覆盖六键</b>：六键的官方报文骨架同一（<c>SuiteId</c> / <c>PaidCorpId</c> / <c>InfoType</c> /
/// <c>TimeStamp</c> + 订单号类字段），差异仅在订单号类字段：
/// <c>open_order</c> 为 <c>OrderId</c> + <c>OperatorId</c>；<c>change_order</c> 为
/// <c>OldOrderId</c> + <c>NewOrderId</c>（无 <c>OrderId</c>）；<c>change_editon</c> <b>四者皆无</b>；
/// 其余三键仅 <c>OrderId</c>。故以一份可空超集承载，<b>缺失字段为 <see langword="null"/></b>，
/// 处理器不得假设必有值。
/// </para>
/// <para>
/// <b>不入载荷的字段</b>：<c>SuiteId</c> 与 <c>TimeStamp</c> 属信封字段（<c>WechatCallbackEvent.SuiteId</c> /
/// <c>WechatCallbackEvent.TimeStamp</c>），按 ADR-9 不在载荷中重复。
/// <c>PaidCorpId</c>（购买方 corpid）<b>须</b>由载荷承载：官方报文无 <c>AuthCorpId</c> 与
/// <c>FromUserName</c> 节点，故信封 <c>AuthCorpId</c> 对本族恒为 <see langword="null"/>，
/// 购买方 corpid 只能取自本字段。
/// </para>
/// <para>
/// <b>族与开放面</b>：本族走套件信封（<c>InfoType</c> 非空）⇒ 事件族为
/// <see cref="WechatCallbackEventFamily.Authorization"/>；官方仅在第三方应用开发文档树提供，
/// 故开放面声明为「第三方 × 套件指令通道」（不宽于族默认，组合根期 fail-fast 校验）。
/// </para>
/// <para>
/// <b>官方拼写陷阱</b>：应用版本变更通知的 <c>InfoType</c> 官方原文为 <c>change_editon</c>（少一个字母 i）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Authorization,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[]
    {
        WechatCallbackEventTypes.OpenOrder,
        WechatCallbackEventTypes.ChangeOrder,
        WechatCallbackEventTypes.PayForAppSuccess,
        WechatCallbackEventTypes.Refund,
        WechatCallbackEventTypes.ChangeEditon,
        WechatCallbackEventTypes.CancelOrder,
    })]
public sealed partial class PayToolVersionOrderPayload : WechatCallbackPayload
{
    /// <summary>
    /// 购买方 corpid（官方 <c>PaidCorpId</c>）：下单的企业 corpid。
    /// </summary>
    [PayloadField("PaidCorpId")]
    public string? PaidCorpId { get; set; }

    /// <summary>
    /// 订单号（官方 <c>OrderId</c>）：付费订单的唯一标志，服务商可据此拉取购买信息。
    /// <para>官方口径：由企业微信生成，不超过 32 个字符；<b>仅改单通知（change_order）不含该节点</b>，
    /// 该事件请改用 <see cref="NewOrderId"/>。</para>
    /// </summary>
    [PayloadField("OrderId")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 下单操作者 userid（官方 <c>OperatorId</c>）：<b>仅下单成功通知（open_order）携带</b>，
    /// 当服务商或代理商代下单时该字段为空。
    /// </summary>
    [PayloadField("OperatorId")]
    public string? OperatorId { get; set; }

    /// <summary>
    /// 原订单号（官方 <c>OldOrderId</c>）：<b>仅改单通知（change_order）携带</b>。
    /// </summary>
    [PayloadField("OldOrderId")]
    public string? OldOrderId { get; set; }

    /// <summary>
    /// 改单后新的订单号（官方 <c>NewOrderId</c>）：<b>仅改单通知（change_order）携带</b>。
    /// <para>官方口径：由企业微信生成，不超过 32 个字符，<b>每次修改价格都会产生新的订单号</b>；
    /// 服务商须用新的订单号查询订单详情以及关联授权应用。</para>
    /// </summary>
    [PayloadField("NewOrderId")]
    public string? NewOrderId { get; set; }
}