// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 检查扫码状态回包（<c>POST /product/qrcode/check</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方枚举表头重复（照录）</b>：两段枚举都标成「枚举值-status」，第二段实际是
/// <c>scan_user_type</c> ⇒ 本仓按字段语义拆分常量类。</para>
/// <para><b>业务错误独立于 errcode</b>：<c>self_check_err_code</c> / <c>self_check_err_msg</c>
/// 是扫码校验业务面的返回，与顶层 <c>errcode</c> 分属两层。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductQrCodeCheckResponse : WxaResponse
{
    /// <summary>扫码状态，见 <see cref="WxaProductQrCodeScanStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>业务返回错误码。</summary>
    [JsonPropertyName("self_check_err_code")]
    public int? SelfCheckErrCode { get; set; }

    /// <summary>业务返回错误信息。</summary>
    [JsonPropertyName("self_check_err_msg")]
    public string? SelfCheckErrMsg { get; set; }

    /// <summary>扫码者身份，见 <see cref="WxaProductQrCodeScanUserTypes"/>。</summary>
    [JsonPropertyName("scan_user_type")]
    public int? ScanUserType { get; set; }
}
