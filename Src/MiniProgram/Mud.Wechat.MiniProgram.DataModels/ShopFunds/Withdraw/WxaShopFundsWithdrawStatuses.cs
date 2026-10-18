// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>
/// 提现状态官方枚举值（<c>status</c>，即「获取提现记录详情」页的 <c>WithDrawStatus</c> 表，恰 8 值）。
/// </summary>
/// <remarks>
/// 官方原文逐值：未扫码 / 已扫码 / 受理成功 / 提现成功 / 提现失败 / 提现退票 / 关单 / 业务单已创建
/// ⇒ 本类<b>不得扩充</b>；「提现成功」只有 <see cref="Success"/> 一档，<see cref="CreateSuccess"/>（受理成功）
/// 与 <see cref="Init"/>（业务单已创建）都不是到账证据。
/// </remarks>
public static class WxaShopFundsWithdrawStatuses
{
    /// <summary>未扫码（<c>BEFORE_SCAN_QRCODE</c>）。</summary>
    public const string BeforeScanQrcode = "BEFORE_SCAN_QRCODE";

    /// <summary>已扫码（<c>AFTER_SCAN_QRCODE</c>）。</summary>
    public const string AfterScanQrcode = "AFTER_SCAN_QRCODE";

    /// <summary>受理成功（<c>CREATE_SUCCESS</c>，非到账）。</summary>
    public const string CreateSuccess = "CREATE_SUCCESS";

    /// <summary>提现成功（<c>SUCCESS</c>，唯一到账态）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>提现失败（<c>FAIL</c>，失败原因见 <c>reason</c>）。</summary>
    public const string Fail = "FAIL";

    /// <summary>提现退票（<c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";

    /// <summary>关单（<c>CLOSE</c>）。</summary>
    public const string Close = "CLOSE";

    /// <summary>业务单已创建（<c>INIT</c>，非到账）。</summary>
    public const string Init = "INIT";
}
