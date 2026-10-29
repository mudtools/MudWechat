// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Auth;

/// <summary>
/// 小程序认证提审数据体（官方 <c>auth_data</c>；<c>wxaauth</c> 首次提审 与 <c>reauth</c> 重新提审 共用同一子树）。
/// 2026-10-17 逐页核验两页 <c>auth_data</c> 字段全集逐字相同 ⇒ 单一 DTO 承载，两支端点各标 <c>[Body]</c>。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_wxaauth.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class OpenPlatformAuthData
{
    /// <summary>客户类型（官方 <c>customer_type</c>，number，必填）。</summary>
    [JsonPropertyName("customer_type")]
    public long CustomerType { get; set; }

    /// <summary>任务号（官方 <c>taskid</c>，string，必填）。</summary>
    [JsonPropertyName("taskid")]
    public string? TaskId { get; set; }

    /// <summary>联系人信息（官方 <c>contact_info</c>，object，必填）。</summary>
    [JsonPropertyName("contact_info")]
    public OpenPlatformAuthContactInfo? ContactInfo { get; set; }

    /// <summary>发票信息（官方 <c>invoice_info</c>，object，选填）。</summary>
    [JsonPropertyName("invoice_info")]
    public OpenPlatformAuthInvoiceInfo? InvoiceInfo { get; set; }

    /// <summary>资质（官方 <c>qualification</c>，string，选填）。</summary>
    [JsonPropertyName("qualification")]
    public string? Qualification { get; set; }

    /// <summary>其他资质材料 <c>media_id</c> 列表（官方 <c>qualification_other</c>，array，元素为字符串，选填）。</summary>
    [JsonPropertyName("qualification_other")]
    public List<string>? QualificationOther { get; set; }

    /// <summary>账户名称（官方 <c>account_name</c>，string，必填）。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>账户名称类型（官方 <c>account_name_type</c>，number，必填）。</summary>
    [JsonPropertyName("account_name_type")]
    public long AccountNameType { get; set; }

    /// <summary>账户补充材料 <c>media_id</c> 列表（官方 <c>account_supplemental</c>，array，元素为字符串，选填）。</summary>
    [JsonPropertyName("account_supplemental")]
    public List<string>? AccountSupplemental { get; set; }

    /// <summary>支付方式（官方 <c>pay_type</c>，number，必填）。</summary>
    [JsonPropertyName("pay_type")]
    public long PayType { get; set; }

    /// <summary>认证身份（官方 <c>auth_identification</c>，string，选填）。</summary>
    [JsonPropertyName("auth_identification")]
    public string? AuthIdentification { get; set; }

    /// <summary>认证身份材料 <c>media_id</c> 列表（官方 <c>auth_ident_material</c>，array，元素为字符串，选填）。</summary>
    [JsonPropertyName("auth_ident_material")]
    public List<string>? AuthIdentMaterial { get; set; }

    /// <summary>第三方联系电话（官方 <c>third_party_phone</c>，string，必填）。</summary>
    [JsonPropertyName("third_party_phone")]
    public string? ThirdPartyPhone { get; set; }

    /// <summary>服务号 appid（官方 <c>service_appid</c>，string，选填）。</summary>
    [JsonPropertyName("service_appid")]
    public string? ServiceAppId { get; set; }
}
