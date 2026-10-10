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
/// 接口调用许可订单回调事件载荷（<b>结构族</b>：覆盖官方事件键 <c>license_pay_success</c> /
/// <c>license_refund</c> 两键；官方 path 97196/97197，仅服务商代开发文档树提供——
/// 自建/第三方应用开发无对应事件回调，经<b>指令回调 URL</b>（官方「系统事件接收 URL」）以套件信封推送，
/// 外层事件值在 <c>InfoType</c> 节点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何一个载荷覆盖两键</b>：两键的官方报文骨架同一（<c>ServiceCorpId</c> / <c>AuthCorpId</c> /
/// <c>OrderId</c> / <c>TimeStamp</c>），差异仅一个专属字段 —— 支付成功通知携带 <c>BuyerUserId</c>、
/// 退款结果通知携带 <c>OrderStatus</c>。故以一份可空超集承载，<b>缺失字段为 <see langword="null"/></b>，
/// 处理器不得假设必有值。
/// </para>
/// <para>
/// <b>不入载荷的字段</b>：<c>TimeStamp</c> 属信封字段（<c>WechatCallbackEvent.TimeStamp</c>，与 URL 验签
/// 时间戳同源），按 ADR-9 不在载荷中重复；<c>AuthCorpId</c>（客户企业 CorpID）由信封
/// <c>WechatCallbackEvent.AuthCorpId</c> 承载（本族报文含该节点，无须兜底）；<c>InfoType</c> 即事件键。
/// <c>ServiceCorpId</c>（服务商 CorpID）<b>须</b>由载荷承载：信封无对应字段（本族报文亦无
/// <c>SuiteId</c> 节点），服务商身份只能取自本字段。
/// </para>
/// <para>
/// <b>官方业务限制（不得弱化）</b>：支付成功通知官方明示时序 —— 支付完成后<b>先生成账号码、后推送本通知</b>；
/// 一次购买较多数量的账号码时通知会出现一定延迟。退款结果通知的 <see cref="OrderStatus"/> 为
/// 1（退款成功）/ 2（退款被拒绝）。
/// </para>
/// <para>
/// <b>开放面</b>：本族走套件信封（<c>InfoType</c> 非空）⇒ 事件族为
/// <see cref="WechatCallbackEventFamily.Authorization"/>；官方仅在服务商代开发文档树提供，
/// 开放面声明为「代开发 × 套件指令通道」（不宽于族默认，组合根期 fail-fast 校验）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97196">path 97196 支付成功通知（服务商代开发）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97197">path 97197 退款结果通知（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Authorization,
    SupportedAppTypes = WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[]
    {
        WechatCallbackEventTypes.LicensePaySuccess,
        WechatCallbackEventTypes.LicenseRefund,
    })]
public sealed partial class LicenseOrderPayload : WechatCallbackPayload
{
    /// <summary>
    /// 服务商 CorpID（官方 <c>ServiceCorpId</c>）：信封无对应字段（本族报文亦无 <c>SuiteId</c> 节点），
    /// 服务商身份只能取自本字段。
    /// </summary>
    [PayloadField("ServiceCorpId")]
    public string? ServiceCorpId { get; set; }

    /// <summary>
    /// 订单号（官方 <c>OrderId</c>）：<b>仅支付成功通知携带</b>时为多企业新购订单的子订单号
    /// （官方口径）；退款结果通知携带的是被退款订单号。
    /// </summary>
    [PayloadField("OrderId")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 服务商内下单用户的 UserID（官方 <c>BuyerUserId</c>）；
    /// <b>仅支付成功通知（license_pay_success）携带</b>，退款结果通知缺失。
    /// </summary>
    [PayloadField("BuyerUserId")]
    public string? BuyerUserId { get; set; }

    /// <summary>
    /// 退款订单状态（官方 <c>OrderStatus</c>）：1 = 退款成功，2 = 退款被拒绝；
    /// <b>仅退款结果通知（license_refund）携带</b>，支付成功通知缺失。
    /// </summary>
    [PayloadField("OrderStatus")]
    public long? OrderStatus { get; set; }
}
