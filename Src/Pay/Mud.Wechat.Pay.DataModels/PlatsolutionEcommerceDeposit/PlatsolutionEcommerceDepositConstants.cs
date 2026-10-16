// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit;

/// <summary>
/// 保证金<b>售后</b>赔付收款方类型（<c>receiver.type</c>）取值 —— <b>出款族里取值最宽的一支（三值）</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/>（请求）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504196"/>（应答），三页同集。
/// 其中 <c>WXPAY_USER</c>（微信支付用户）是<b>本域独有的第三值</b>
/// —— 赔付、保险理赔、保费、商户付款四支出款面的 <c>receiver.type</c> 都不含它。
/// </para>
/// <para>
/// <b>同名参数、多套取值集合，逐页核对不得合并</b>（2026-10-16 逐页取「可选取值」原文比对）：
/// 赔付 <c>TRANSACTION_USER</c>（
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceCompensate.PlatsolutionEcommerceCompensateReceiverTypes"/>）、
/// 保险理赔 <c>MERCHANT</c> / <c>TRANSACTION_USER</c>（
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceClaim.PlatsolutionEcommerceInsuranceClaimReceiverTypes"/>）、
/// 保费 <c>MERCHANT</c>（下方
/// <see cref="PlatsolutionEcommerceDepositInsurancePremiumsReceiverTypes"/>）、
/// 商户付款 <c>MERCHANT</c>（
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceMchTransferReceiverTypes"/>），
/// 而<b>保证金运费三支官方整字段不印 <c>receiver.type</c></b>（只有 <c>receiver.transaction_info</c>）
/// ⇒ 同域三支的字段面都不同，DTO 不得互套。
/// </para>
/// <para>
/// 本域付款单状态与转账接收方订单类型与出款族其余四支逐值同集，统一登记在
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransferStates"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransactionTypes"/>。
/// </para>
/// </remarks>
public static class PlatsolutionEcommerceDepositAfterSalesReceiverTypes
{
    /// <summary>微信支付商户（<c>MERCHANT</c>）。</summary>
    public const string Merchant = "MERCHANT";

    /// <summary>微信支付订单对应的用户（<c>TRANSACTION_USER</c>）。</summary>
    public const string TransactionUser = "TRANSACTION_USER";

    /// <summary>微信支付用户（<c>WXPAY_USER</c>）：本域独有第三值，其它出款支传入即 <c>PARAM_ERROR</c>。</summary>
    public const string WxpayUser = "WXPAY_USER";
}

/// <summary>
/// 保证金<b>保费</b>赔付收款方类型（<c>receiver.type</c>）取值 —— <b>本支官方只印一个值</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/>（请求）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/>（应答），两页同集，只 <c>MERCHANT</c>
/// ⇒ 保费赔付的收款方恒为商户（保险公司／平台侧商户），不得按同域的售后支传用户。
/// 另注：本支查询定位键是<b>裸 path 段</b> <c>/{out_bill_no}</c>（<c>…-bills/{out_bill_no}</c>），
/// 与同域另两支的 <c>/out-bill-no/{out_bill_no}</c> 形态不同，照官方原文不得「顺手对齐」。
/// </remarks>
public static class PlatsolutionEcommerceDepositInsurancePremiumsReceiverTypes
{
    /// <summary>微信支付商户（<c>MERCHANT</c>）：本支唯一合法取值。</summary>
    public const string Merchant = "MERCHANT";
}
