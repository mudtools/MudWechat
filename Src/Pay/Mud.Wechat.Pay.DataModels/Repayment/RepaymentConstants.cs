// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Repayment;

/// <summary>
/// 用户证件类型（官方 <c>combine_payer_info.identity.type</c>，7 值）。
/// </summary>
/// <remarks>
/// <b>本域官方拼写自成一套</b>：与进件域证件体系的 <c>IDENTITY_TYPE_*</c> / 港澳台证件写法均不同
/// （此处无前缀、且 <c>HONGKONG_MACAO</c> 与 <c>HONGKONG_MACAO_RESIDENT</c> 是两个不同值）
/// ⇒ 不得跨域取别名（守卫 RP-B4 锁定 7 值集）。
/// </remarks>
public static class RepaymentIdentityTypes
{
    /// <summary>身份证（官方 <c>IDCARD</c>）。</summary>
    public const string Idcard = "IDCARD";

    /// <summary>港澳回乡证（官方 <c>HONGKONG_MACAO</c>）。</summary>
    public const string HongkongMacao = "HONGKONG_MACAO";

    /// <summary>港澳居住证（官方 <c>HONGKONG_MACAO_RESIDENT</c>）。</summary>
    public const string HongkongMacaoResident = "HONGKONG_MACAO_RESIDENT";

    /// <summary>台湾居住证（官方 <c>TAIWAN_RESIDENT</c>）。</summary>
    public const string TaiwanResident = "TAIWAN_RESIDENT";

    /// <summary>外国人永居证（官方 <c>FOREIGN_RESIDENT</c>）。</summary>
    public const string ForeignResident = "FOREIGN_RESIDENT";

    /// <summary>护照（官方 <c>OVERSEA_PASSPORT</c>）。</summary>
    public const string OverseaPassport = "OVERSEA_PASSPORT";

    /// <summary>台胞证（官方 <c>TAIWAN_COMPATRIOT_PERMIT</c>）。</summary>
    public const string TaiwanCompatriotPermit = "TAIWAN_COMPATRIOT_PERMIT";
}

/// <summary>
/// H5 场景类型（官方 <c>h5_info.type</c>，4 值）—— <b>混合大小写</b>，与其余枚举的全大写风格不同。
/// </summary>
/// <remarks>官方原文为「使用 H5 还款的场景：Wap、iOS、Android、Harmony」⇒ 值须照抄原文大小写。</remarks>
public static class RepaymentH5SceneTypes
{
    /// <summary>Wap 站点（官方 <c>Wap</c>）。</summary>
    public const string Wap = "Wap";

    /// <summary>iOS（官方 <c>iOS</c>）。</summary>
    public const string IOS = "iOS";

    /// <summary>Android（官方 <c>Android</c>）。</summary>
    public const string Android = "Android";

    /// <summary>Harmony（官方 <c>Harmony</c>）。</summary>
    public const string Harmony = "Harmony";
}

/// <summary>
/// 还款明细交易状态（官方 <c>sub_orders[].trade_state</c>，3 值）。
/// </summary>
/// <remarks>
/// <b>本域只有三值，不是直连交易域 <c>trade_state</c> 的子集别名</b>：直连面另有
/// <c>REFUND</c> / <c>USERPAYING</c> / <c>PAYERROR</c> 等值 ⇒ 分建类型，比较时勿用直连常量
/// （守卫 RP-B4 锁定三值集）。官方指向「开发指引-订单状态流转图」，未识别值按「状态未知」处置。
/// </remarks>
public static class RepaymentTradeStates
{
    /// <summary>支付成功（官方 <c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>未支付（官方 <c>NOTPAY</c>）。</summary>
    public const string NotPay = "NOTPAY";

    /// <summary>已关闭（官方 <c>CLOSED</c>）。</summary>
    public const string Closed = "CLOSED";
}
