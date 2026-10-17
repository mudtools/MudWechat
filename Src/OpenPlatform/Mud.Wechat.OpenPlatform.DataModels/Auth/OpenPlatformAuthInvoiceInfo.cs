// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Auth;

/// <summary>
/// 发票信息（<c>auth_data.invoice_info</c>，选填）。按 <c>invoice_type</c> 择一填充 <see cref="Electronic"/> 或 <see cref="Vat"/>。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_wxaauth.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class OpenPlatformAuthInvoiceInfo
{
    /// <summary>发票类型（官方 <c>invoice_type</c>，number，<c>invoice_info</c> 存在时必填；2=电子、4=增值税）。</summary>
    [JsonPropertyName("invoice_type")]
    public long InvoiceType { get; set; }

    /// <summary>电子发票信息（官方 <c>electronic</c>，<c>invoice_type = 2</c> 时必填）。</summary>
    [JsonPropertyName("electronic")]
    public OpenPlatformAuthInvoiceElectronicInfo? Electronic { get; set; }

    /// <summary>增值税发票信息（官方 <c>vat</c>，<c>invoice_type = 4</c> 时必填）。</summary>
    [JsonPropertyName("vat")]
    public OpenPlatformAuthInvoiceVatInfo? Vat { get; set; }
}
