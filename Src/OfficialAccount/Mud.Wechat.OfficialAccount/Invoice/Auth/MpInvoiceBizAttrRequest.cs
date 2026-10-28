// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 查询与设置授权页与商户信息（POST /card/invoice/setbizattr?action=…）

/// <summary>
/// 查询与设置授权页与商户信息（<c>POST /card/invoice/setbizattr</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>按 Query <c>action</c> 六值分流</b>（见 <c>MpInvoiceBizAttrActions</c>）：
/// <c>set_auth_field</c>/<c>set_pay_mch</c>/<c>set_contact</c> 三者各自携带对应的<b>唯一一个</b>字段；
/// 三个 <c>get_*</c> 的请求体为 <c>{}</c>。
/// </para>
/// <para>
/// <b>官方必填性矛盾（照录，SDK 全部可空、不本地校验）</b>：官方字段表把 <c>auth_field</c>/<c>paymch_info</c>/
/// <c>contact</c> 三者<b>均标必填</b>，但实际按 <c>action</c> 三选一、且 <c>get_*</c> 时请求体为空。
/// </para>
/// <para>
/// 官方一次性设置约束（正文原文，SDK 不编排）：<c>set_auth_field</c>/<c>set_pay_mch</c> 为<b>一次性设置</b>，
/// 除非调整字段 / 识别号变更或更换开票平台；<c>set_contact</c> 需在<b>获取授权链接之前</b>先设置联系方式。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBizAttrRequest
{
    /// <summary>获取或设置授权页字段（官方 <c>auth_field</c>；<c>action = set_auth_field</c> 时使用）。</summary>
    [JsonPropertyName("auth_field")]
    public MpInvoiceAuthField? AuthField { get; set; }

    /// <summary>获取或设置微信商户号与开票平台关系信息（官方 <c>paymch_info</c>；<c>action = set_pay_mch</c> 时使用）。</summary>
    [JsonPropertyName("paymch_info")]
    public MpInvoicePayMchInfo? PayMchInfo { get; set; }

    /// <summary>获取或设置联系方式信息（官方 <c>contact</c>；<c>action = set_contact</c> 时使用）。</summary>
    [JsonPropertyName("contact")]
    public MpInvoiceContact? Contact { get; set; }
}
