// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceCompensate;

/// <summary>
/// 赔付收款方类型（<c>receiver.type</c>）取值 —— <b>本支官方只印一个值</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504175"/>（请求）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504178"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504179"/>（应答），三页同集、均只
/// <c>TRANSACTION_USER</c> ⇒ <b>赔付的对象恒是「微信支付订单对应的用户」</b>。
/// </para>
/// <para>
/// <b>红线（同名参数、多套取值集合，逐页核对不得合并）</b>：
/// 保险理赔面是 <c>MERCHANT</c> / <c>TRANSACTION_USER</c> 两值
/// （<see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceClaim.PlatsolutionEcommerceInsuranceClaimReceiverTypes"/>），
/// 保证金售后面是 <c>MERCHANT</c> / <c>TRANSACTION_USER</c> / <c>WXPAY_USER</c> 三值
/// （<see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit.PlatsolutionEcommerceDepositAfterSalesReceiverTypes"/>），
/// 保证金保费支<b>只</b> <c>MERCHANT</c>
/// （<see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit.PlatsolutionEcommerceDepositInsurancePremiumsReceiverTypes"/>），
/// 保证金运费支<b>官方整字段不印</b> <c>receiver.type</c>（只印 <c>receiver.transaction_info</c>），
/// 商户付款支亦只 <c>MERCHANT</c>
/// （<see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceMchTransferReceiverTypes"/>）
/// ⇒ 各支路由的 <c>receiver.type</c> 合法集<b>互不相同</b>，跨支套用即 <c>PARAM_ERROR</c>。
/// </para>
/// <para>
/// 本域其余两个取值集合（付款单状态、转账接收方订单类型）与出款族其余四支<b>逐值同集</b>，
/// 统一登记在
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransferStates"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransactionTypes"/>
/// —— 逐页取值比对见 <c>.docs/_tmp_pages/p12b-enum-detail.mjs</c> 输出，勿在本域复制一份造成两套常量漂移。
/// </para>
/// </remarks>
public static class PlatsolutionEcommerceCompensateReceiverTypes
{
    /// <summary>微信支付订单对应的用户（<c>TRANSACTION_USER</c>）：本支唯一合法取值。</summary>
    public const string TransactionUser = "TRANSACTION_USER";
}
