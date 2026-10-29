// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 查询与设置授权页与商户信息（<c>POST /card/invoice/setbizattr</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：报文的三个业务字段（<c>auth_field</c>/<c>paymch_info</c>/<c>contact</c>）**与请求体同构**且
/// <b>各自仅被对应 action 返回</b>（<c>set_*</c> 返回被设置者、<c>get_*</c> 返回被查询者）。
/// 本 DTO 与请求体共用 <see cref="MpInvoiceAuthField"/> / <see cref="MpInvoicePayMchInfo"/> /
/// <see cref="MpInvoiceContact"/> 三个类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBizAttrResponse : MpResponse
{
    /// <summary>获取或设置授权页字段（官方 <c>auth_field</c>；<c>set_auth_field</c> / <c>get_auth_field</c> 返回）。</summary>
    [JsonPropertyName("auth_field")]
    public MpInvoiceAuthField? AuthField { get; set; }

    /// <summary>获取或设置商户号与开票平台关系（官方 <c>paymch_info</c>；<c>get_pay_mch</c> 返回）。</summary>
    [JsonPropertyName("paymch_info")]
    public MpInvoicePayMchInfo? PayMchInfo { get; set; }

    /// <summary>获取或设置联系方式（官方 <c>contact</c>；<c>get_contact</c> 返回）。</summary>
    [JsonPropertyName("contact")]
    public MpInvoiceContact? Contact { get; set; }
}
