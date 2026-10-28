// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Auth;

/// <summary>
/// 增值税发票信息（<c>auth_data.invoice_info.vat</c>，<c>invoice_type = 4</c> 时使用）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_wxaauth.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class OpenPlatformAuthInvoiceVatInfo
{
    /// <summary>企业电话（官方 <c>enterprise_phone</c>，必填）。</summary>
    [JsonPropertyName("enterprise_phone")]
    public string? EnterprisePhone { get; set; }

    /// <summary>发票标识（官方 <c>id</c>，必填）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>企业地址（官方 <c>enterprise_address</c>，必填）。</summary>
    [JsonPropertyName("enterprise_address")]
    public string? EnterpriseAddress { get; set; }

    /// <summary>开户银行（官方 <c>bank_name</c>，选填）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（官方 <c>bank_account</c>，选填）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>备注说明（官方 <c>desc</c>，选填）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}
