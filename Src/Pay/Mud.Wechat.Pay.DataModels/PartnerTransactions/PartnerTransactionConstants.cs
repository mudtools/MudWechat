// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商查单应答的交易类型（官方 <c>trade_type</c>，6 值）。
/// </summary>
/// <remarks>
/// <para>取值照「通过微信支付订单号查询订单」页原文
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012738964"/>，2026-10-14 核验）。</para>
/// <para><b>比下单族多两个值</b>：<c>MICROPAY</c>（付款码支付）与 <c>FACEPAY</c>（刷脸支付）——
/// 前者是 APIv2 产品（本域只落 <c>scannedpos</c> 的 APIv3 面，V2 归 P0-8），
/// 故本枚举<b>不是</b>「本域可下单的产品清单」，只描述查单应答可能出现的取值。</para>
/// </remarks>
public static class PartnerTradeTypes
{
    /// <summary>JSAPI / 小程序支付（<c>JSAPI</c>）。</summary>
    public const string Jsapi = "JSAPI";

    /// <summary>扫码支付（用户扫码，<c>NATIVE</c>）。</summary>
    public const string Native = "NATIVE";

    /// <summary>APP 支付（<c>APP</c>）。</summary>
    public const string App = "APP";

    /// <summary>付款码支付（商户扫用户，<c>MICROPAY</c>）：APIv2 产品，查单应答可出现。</summary>
    public const string Micropay = "MICROPAY";

    /// <summary>H5 支付（<c>MWEB</c>）。</summary>
    public const string Mweb = "MWEB";

    /// <summary>刷脸支付（<c>FACEPAY</c>）。</summary>
    public const string Facepay = "FACEPAY";
}

/// <summary>
/// 服务商查单应答的交易状态（官方 <c>trade_state</c>，7 值）。
/// </summary>
/// <remarks>
/// <para>取值照 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738964"/> 原文（2026-10-14 核验）。</para>
/// <para><b>判定口径</b>：只有 <see cref="Success"/> 才是支付成功；<b>查单不可代替支付结果通知</b>。
/// <see cref="Revoked"/>（已撤销）与 <see cref="UserPaying"/>（用户支付中）是付款码支付专用状态。</para>
/// </remarks>
public static class PartnerTradeStates
{
    /// <summary>支付成功（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>转入退款（<c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";

    /// <summary>未支付（<c>NOTPAY</c>）。</summary>
    public const string Notpay = "NOTPAY";

    /// <summary>已关闭（<c>CLOSED</c>）。</summary>
    public const string Closed = "CLOSED";

    /// <summary>已撤销（<c>REVOKED</c>，付款码支付专用）。</summary>
    public const string Revoked = "REVOKED";

    /// <summary>用户支付中（<c>USERPAYING</c>，付款码支付专用）。</summary>
    public const string UserPaying = "USERPAYING";

    /// <summary>支付失败（<c>PAYERROR</c>）。</summary>
    public const string PayError = "PAYERROR";
}

/// <summary>优惠范围（官方 <c>promotion_detail[].scope</c>，2 值）。</summary>
public static class PartnerPromotionScopes
{
    /// <summary>全场代金券（<c>GLOBAL</c>）。</summary>
    public const string Global = "GLOBAL";

    /// <summary>单品优惠（<c>SINGLE</c>）。</summary>
    public const string Single = "SINGLE";
}

/// <summary>优惠类型（官方 <c>promotion_detail[].type</c>，2 值）。</summary>
public static class PartnerPromotionTypes
{
    /// <summary>充值代金券（<c>CASH</c>）。</summary>
    public const string Cash = "CASH";

    /// <summary>非充值优惠券（<c>NOCASH</c>）。</summary>
    public const string Nocash = "NOCASH";
}

/// <summary>
/// 分期曝光咨询的用户标识类型（官方 <c>user_id.user_id_type</c>，4 值）。
/// </summary>
/// <remarks>
/// <para>取值照「分期支付曝光咨询」页原文
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4016196820"/>，2026-10-14 核验）。</para>
/// <para><b>注意没有 <c>APP_OPENID</c> 之类的直连形态</b>：本域标识一律带 <c>SP_</c>/<c>SUB_</c> 归属前缀
/// （或通用 <c>OPENID</c> / <c>UNKNOWN_USER_ID_TYPE</c>），与下单族 <c>payer</c> 的双 openid 命名同源。</para>
/// </remarks>
public static class PartnerUserIdTypes
{
    /// <summary>未知类型（<c>UNKNOWN_USER_ID_TYPE</c>）。</summary>
    public const string UnknownUserIdType = "UNKNOWN_USER_ID_TYPE";

    /// <summary>通用 openid（<c>OPENID</c>）。</summary>
    public const string OpenId = "OPENID";

    /// <summary>服务商 APPID 下的 openid（<c>SP_OPENID</c>）。</summary>
    public const string SpOpenId = "SP_OPENID";

    /// <summary>子商户 APPID 下的 openid（<c>SUB_OPENID</c>）。</summary>
    public const string SubOpenId = "SUB_OPENID";
}

/// <summary>
/// 分期支付的档期类型（官方 <c>repayment_info[].repayment_type</c>，5 值）。
/// </summary>
/// <remarks>
/// <para>取值照 <see href="https://pay.weixin.qq.com/doc/v3/partner/4016196820"/> 原文（2026-10-14 核验）。</para>
/// <para><b>命名陷阱（官方原文，不得「纠正」）</b>：档期数字直接拼在枚举名里
/// （<c>REPAYMENT_INSTALL_3</c> 而非 <c>REPAYMENT_INSTALL_THREE</c>），日付档是 <c>REPAYMENT_DAILY</c>
/// 而非 <c>REPAYMENT_INSTALL_1</c> —— 守卫 PT-B5 逐值锁定。</para>
/// </remarks>
public static class PartnerRepaymentTypes
{
    /// <summary>日付（<c>REPAYMENT_DAILY</c>）。</summary>
    public const string Daily = "REPAYMENT_DAILY";

    /// <summary>3 期分期（<c>REPAYMENT_INSTALL_3</c>）。</summary>
    public const string Install3 = "REPAYMENT_INSTALL_3";

    /// <summary>6 期分期（<c>REPAYMENT_INSTALL_6</c>）。</summary>
    public const string Install6 = "REPAYMENT_INSTALL_6";

    /// <summary>12 期分期（<c>REPAYMENT_INSTALL_12</c>）。</summary>
    public const string Install12 = "REPAYMENT_INSTALL_12";

    /// <summary>24 期分期（<c>REPAYMENT_INSTALL_24</c>）。</summary>
    public const string Install24 = "REPAYMENT_INSTALL_24";
}
