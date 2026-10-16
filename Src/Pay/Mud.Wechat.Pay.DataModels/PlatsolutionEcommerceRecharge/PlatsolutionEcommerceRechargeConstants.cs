// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRecharge;

/// <summary>
/// 二级商户充值场景（<c>recharge_scene</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/>（申请充值）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762895"/>（查询 / 关闭应答），四页同集。
/// 本域<b>只覆盖二级商户充值</b>两场景 ⇒ <c>ECOMMERCE_DEPOSIT</c> 落保证金账户、
/// <c>ECOMMERCE_PAYMENT</c> 落运营账户，与 <see cref="PlatsolutionEcommerceRechargeAccountTypes"/> 一一对应，
/// 不存在「基本账户充值」或「手续费账户充值」的场景值。
/// </remarks>
public static class PlatsolutionEcommerceRechargeScenes
{
    /// <summary>二级商户充值保证金（<c>ECOMMERCE_DEPOSIT</c>）。</summary>
    public const string EcommerceDeposit = "ECOMMERCE_DEPOSIT";

    /// <summary>二级商户充值运营资金（<c>ECOMMERCE_PAYMENT</c>）。</summary>
    public const string EcommercePayment = "ECOMMERCE_PAYMENT";
}

/// <summary>
/// 二级商户充值的目标账户类型（<c>account_type</c>）取值 —— <b>比电商收款账户面的五值集窄</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/>（两页同集，均两值）。
/// <b>本域没有 <c>BASIC</c> / <c>FEES</c> / <c>ALL</c></b>：
/// <see cref="Mud.Wechat.Pay.DataModels.Ecommerce.EcommerceAccountTypes"/> 是「余额 / 提现 / 账单」面的取值集
/// （四值乃至 <c>ALL</c>），充值面只允许保证金与运营两账户
/// ⇒ 两个类的值集<b>不可互换</b>，按账户查询页的取值传充值请求即 <c>PARAM_ERROR</c>。
/// </remarks>
public static class PlatsolutionEcommerceRechargeAccountTypes
{
    /// <summary>保证金账户（<c>DEPOSIT</c>）。</summary>
    public const string Deposit = "DEPOSIT";

    /// <summary>运营账户（<c>OPERATION</c>）。</summary>
    public const string Operation = "OPERATION";
}

/// <summary>
/// 充值渠道取值 —— <b>同一值集在本域以两个字段名出现</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/>：
/// 请求与应答的 <c>available_recharge_channels</c> 是<b>数组</b>（官方「可用充值渠道」），
/// 应答的 <c>recharge_channel</c> 是<b>单值</b>（本次实际使用的充值渠道）
/// ⇒ 两字段名不同、类型不同，但取值集完全相同，故共用本类。
/// </para>
/// <para>
/// <b>三条渠道各走不同支</b>：<c>QR_RECHARGE</c> 依赖可扫码充值员工
/// （<see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRechargeEmployee.PlatsolutionEcommerceRechargeEmployeeStates"/>），
/// <c>ONLINE_BANK</c> 的应答另有 <c>online_bank_recharge_info</c> 子树
/// （<see cref="PlatsolutionEcommerceRechargeOnlineBankTypes"/>），
/// <c>BANK_TRANSFER</c> 走银行卡转账面 —— <b>而转账面另两支 <c>bank-transfer-recharges</c> 路由官方页不可达</b>
/// （取证结论 NO-PAGE）⇒ 本 SDK 零声明，由非契约路由零声明守卫第八条锁定。
/// </para>
/// </remarks>
public static class PlatsolutionEcommerceRechargeChannels
{
    /// <summary>银行转账（<c>BANK_TRANSFER</c>）。</summary>
    public const string BankTransfer = "BANK_TRANSFER";

    /// <summary>扫码充值（<c>QR_RECHARGE</c>）：由已登记的员工扫码发起。</summary>
    public const string QrRecharge = "QR_RECHARGE";

    /// <summary>网银充值（<c>ONLINE_BANK</c>）。</summary>
    public const string OnlineBank = "ONLINE_BANK";
}

/// <summary>网银充值类型（<c>online_bank_recharge_info.online_bank_type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/>（应答侧子树，两值）。
/// 值名带 <c>ONLINE_BANK_TYPE_</c> 前缀（官方原文如此，<b>不是</b>裸 <c>CORPORATE</c> / <c>PERSONAL</c>），照页不得简写。
/// </remarks>
public static class PlatsolutionEcommerceRechargeOnlineBankTypes
{
    /// <summary>企业网银（<c>ONLINE_BANK_TYPE_CORPORATE</c>）。</summary>
    public const string Corporate = "ONLINE_BANK_TYPE_CORPORATE";

    /// <summary>个人网银（<c>ONLINE_BANK_TYPE_PERSONAL</c>）。</summary>
    public const string Personal = "ONLINE_BANK_TYPE_PERSONAL";
}

/// <summary>扫码充值员工类型（<c>qr_recharge_info.employee_type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/>（应答侧子树，三值）。
/// 与「可扫码充值员工」域的 <c>state</c> 是<b>两个维度</b>（本类说「是谁」，
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRechargeEmployee.PlatsolutionEcommerceRechargeEmployeeStates"/>
/// 说「登记关系是否生效」），不得互相套用。
/// </remarks>
public static class PlatsolutionEcommerceRechargeEmployeeTypes
{
    /// <summary>超级管理员（<c>ADMIN</c>）。</summary>
    public const string Admin = "ADMIN";

    /// <summary>员工（<c>STAFF</c>）。</summary>
    public const string Staff = "STAFF";

    /// <summary>法人（<c>LEGAL_PERSON</c>）。</summary>
    public const string LegalPerson = "LEGAL_PERSON";
}

/// <summary>充值单状态（<c>recharge_state</c>）取值 —— <b>三值，且没有「受理中」态</b>。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012762895"/>（三页同集）。
/// <b>本域状态名是 <c>recharge_state</c>，不是出款族的 <c>state</c></b> ⇒ 与
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransferStates"/>
/// 的六值集（<c>ACCEPTED</c> / <c>WAIT_USER_CONFIRM</c> / …）<b>毫无交集</b>，不得套用；
/// <c>CLOSED</c> 是「已关闭」（含商户主动关闭支的结果），<b>不等于失败入账</b>。
/// </remarks>
public static class PlatsolutionEcommerceRechargeStates
{
    /// <summary>充值成功（<c>SUCCESS</c>）：终态。</summary>
    public const string Success = "SUCCESS";

    /// <summary>充值中（<c>RECHARGING</c>）：在途态，须继续查询。</summary>
    public const string Recharging = "RECHARGING";

    /// <summary>已关闭（<c>CLOSED</c>）：终态，充值单关闭（含关闭支受理成功）。</summary>
    public const string Closed = "CLOSED";
}
