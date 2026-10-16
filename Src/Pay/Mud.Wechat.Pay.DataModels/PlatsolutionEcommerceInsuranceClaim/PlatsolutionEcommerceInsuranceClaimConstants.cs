// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceClaim;

/// <summary>
/// 保险理赔收款方类型（<c>receiver.type</c>）取值 —— <b>比赔付支多一个 <c>MERCHANT</c></b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/>（请求）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504188"/>（应答），三页同集：
/// <c>MERCHANT</c> 微信支付商户 / <c>TRANSACTION_USER</c> 微信支付订单对应的用户（两值）。
/// </para>
/// <para>
/// <b>同名参数、多套取值集合，逐页核对不得合并</b>：赔付支只 <c>TRANSACTION_USER</c>、
/// 保证金售后面三值（多 <c>WXPAY_USER</c>）、保证金保费支与商户付款支只 <c>MERCHANT</c>、
/// 保证金运费支官方<b>整字段不印</b> <c>receiver.type</c> ⇒ 本支的「二值集」是<b>独有</b>口径，
/// 详见 <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceCompensate.PlatsolutionEcommerceCompensateReceiverTypes"/>
/// 与 <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit.PlatsolutionEcommerceDepositAfterSalesReceiverTypes"/>。
/// 跨支套用即 <c>PARAM_ERROR</c>。
/// </para>
/// <para>
/// 本域付款单状态与转账接收方订单类型与出款族其余四支逐值同集，统一登记在
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransferStates"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransactionTypes"/>。
/// </para>
/// </remarks>
public static class PlatsolutionEcommerceInsuranceClaimReceiverTypes
{
    /// <summary>微信支付商户（<c>MERCHANT</c>）：理赔款直接付给商户。</summary>
    public const string Merchant = "MERCHANT";

    /// <summary>微信支付订单对应的用户（<c>TRANSACTION_USER</c>）：理赔款付给原订单用户。</summary>
    public const string TransactionUser = "TRANSACTION_USER";
}
