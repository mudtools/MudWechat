// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer;

/// <summary>
/// 平台/服务商解决方案「电商出款族」付款单状态（<c>state</c>）取值 —— <b>四个出款族共用同一取值集合，故只建一类</b>。
/// </summary>
/// <remarks>
/// <para>
/// 取证口径（2026-10-16 逐页取「可选取值」原文比对，六值<b>完全同名同集</b>）：
/// 赔付 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504178"/>、
/// 保险理赔 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/>、
/// 保证金售后 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/>、
/// 保证金运费 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/>、
/// 保证金保费 <see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/>、
/// 商户付款 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/>。
/// </para>
/// <para>
/// <b>释义文案随页不同、值集相同</b>：赔付与保险理赔页把 <c>ACCEPTED</c> 印为
/// 「请求已受理，处理结果尚未明确，如一直处于此状态，建议检查账户余额是否足够」，
/// 商户付款与保证金页只印「已受理」⇒ 本类取<b>信息量更大的那一版</b>注释，不构成契约差异。
/// </para>
/// <para>
/// <b>状态机红线</b>：<c>ACCEPTED</c> / <c>WAIT_USER_CONFIRM</c> / <c>CANCELING</c> 是<b>在途态</b>，
/// 只有 <c>SUCCESS</c> / <c>CLOSED</c> / <c>CANCELLED</c> 是<b>终态</b>；
/// <c>CLOSED</c> 官方释义为「系统关闭（余额不足、转账失败等原因）」⇒ <b>不等于商户撤销成功</b>，
/// 撤销成功只认 <c>CANCELLED</c>；预下单支（<c>pre-transfer</c>）受理后落 <c>WAIT_USER_CONFIRM</c>，
/// 用户未确认前不得按已出款记账。
/// </para>
/// </remarks>
public static class PlatsolutionEcommerceTransferStates
{
    /// <summary>已受理（<c>ACCEPTED</c>）：处理结果尚未明确，长期停留此态须检查账户余额是否足够。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>已成功（<c>SUCCESS</c>）：终态，出款成功。</summary>
    public const string Success = "SUCCESS";

    /// <summary>系统关闭（<c>CLOSED</c>）：终态，余额不足 / 转账失败等<b>系统侧</b>原因关闭，非商户撤销。</summary>
    public const string Closed = "CLOSED";

    /// <summary>等待用户确认（<c>WAIT_USER_CONFIRM</c>）：预下单成功后的在途态，用户确认前不出款。</summary>
    public const string WaitUserConfirm = "WAIT_USER_CONFIRM";

    /// <summary>撤销中（<c>CANCELING</c>）：撤销请求已受理，该笔付款正在撤销中（官方拼写为单 L）。</summary>
    public const string Canceling = "CANCELING";

    /// <summary>撤销完成（<c>CANCELLED</c>）：终态，付款撤销完成（官方拼写为双 L，与 <c>CANCELING</c> 不对称，照页）。</summary>
    public const string Cancelled = "CANCELLED";
}

/// <summary>
/// 转账接收方订单类型（<c>receiver.transaction_info.type</c>）取值 —— <b>商户付款支官方不印本字段</b>。
/// </summary>
/// <remarks>
/// 出处：赔付 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504175"/>、
/// 保险理赔 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/>、
/// 保证金售后 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/>、
/// 保证金运费 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/>、
/// 保证金保费 <see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/> 的请求与应答表；
/// <b>而商户付款支（<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/>）的
/// <c>transaction_info</c> 子树官方只印订单号一支字段，无本类型字段</b>
/// ⇒ 不得把本枚举「顺手」当作出款族通用字段跨支套用。
/// 官方注明「默认 <c>WXPAY</c>」⇒ 选填，缺省即按微信支付订单解析。
/// </remarks>
public static class PlatsolutionEcommerceTransactionTypes
{
    /// <summary>微信支付订单（<c>WXPAY</c>）：官方默认值，订单号填微信支付系统生成的订单号（合单交易填子单号）。</summary>
    public const string Wxpay = "WXPAY";

    /// <summary>支付分服务订单（<c>WXVALUE</c>）：订单号填微信支付分服务订单号。</summary>
    public const string Wxvalue = "WXVALUE";
}

/// <summary>
/// 商户付款的付款原因类型（<c>business_type</c>）取值 —— <b>只有商户付款支打印本字段</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/>（请求）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504200"/>（应答），四值三页同集。
/// <b>四值里没有「普通商户付款」</b>：本域的四支专用出款（赔付 / 保险理赔 / 保证金三支）各有独立路由，
/// 商户付款支用 <c>business_type</c> 标注的是「这笔通用付款单归属于哪类赔付场景」⇒
/// 取值与专用路由<b>不是一一映射</b>（例如 <c>DEPOSIT_COMPENSATION</c> 走保证金专用路由，不经本支）。
/// </remarks>
public static class PlatsolutionEcommerceTransferBusinessTypes
{
    /// <summary>保证金赔付（<c>DEPOSIT_COMPENSATION</c>）。</summary>
    public const string DepositCompensation = "DEPOSIT_COMPENSATION";

    /// <summary>平台售后赔付（<c>PLATFORM_AFTER_SALES_COMPENSATION</c>）。</summary>
    public const string PlatformAfterSalesCompensation = "PLATFORM_AFTER_SALES_COMPENSATION";

    /// <summary>保险理赔（<c>INSURANCE_CLAIM</c>）。</summary>
    public const string InsuranceClaim = "INSURANCE_CLAIM";

    /// <summary>保证金售后赔付（用户）（<c>DEPOSIT_AFTER_SALES_COMPENSATION</c>）：官方原文带「（用户）」限定。</summary>
    public const string DepositAfterSalesCompensation = "DEPOSIT_AFTER_SALES_COMPENSATION";
}

/// <summary>
/// 商户付款的收款方类型（<c>receiver.type</c>）取值 —— <b>本域唯一「收款方是商户」的支</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/>。官方<b>只印</b> <c>MERCHANT</c> 一个值
/// ⇒ 商户付款支的接收方恒为微信支付商户；<b>同名参数在其余三支是不同集合</b>（赔付只
/// <c>TRANSACTION_USER</c>、保险理赔 <c>MERCHANT</c> / <c>TRANSACTION_USER</c>、
/// 保证金售后 <c>MERCHANT</c> / <c>TRANSACTION_USER</c> / <c>WXPAY_USER</c>），
/// 逐页核对不得合并，详见各域 <c>*Constants</c>。
/// </remarks>
public static class PlatsolutionEcommerceMchTransferReceiverTypes
{
    /// <summary>微信支付商户（<c>MERCHANT</c>）：本支唯一合法取值。</summary>
    public const string Merchant = "MERCHANT";
}
