// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 营业执照识别（<c>POST /cv/ocr/bizlicense</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>reg_num</c> / <c>serial</c> / <c>legal_representative</c> /
/// <c>enterprise_name</c> / <c>type_of_organization</c> / <c>address</c> / <c>type_of_enterprise</c> /
/// <c>business_scope</c> / <c>registered_capital</c> / <c>paid_in_capital</c> / <c>valid_period</c> /
/// <c>registered_date</c> / <c>cert_position</c>（营业执照位置）/ <c>img_size</c>（图片大小）。
/// </para>
/// <para>
/// 官方「注意事项」原文：「<b>返回字段仅包含当前营业执照图片中存在的字段，若对应字段不存在则不返回</b>」
/// ⇒ 各字段均为可空，不得以非空判定识别成功。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101001</c> / <c>101002</c> / <c>101003</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrBizLicenseResponse : MpResponse
{
    /// <summary>获取或设置注册号（官方 <c>reg_num</c>）。</summary>
    [JsonPropertyName("reg_num")]
    public string? RegNumber { get; set; }

    /// <summary>获取或设置编号（官方 <c>serial</c>）。</summary>
    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    /// <summary>获取或设置法定代表人姓名（官方 <c>legal_representative</c>）。</summary>
    [JsonPropertyName("legal_representative")]
    public string? LegalRepresentative { get; set; }

    /// <summary>获取或设置企业名称（官方 <c>enterprise_name</c>）。</summary>
    [JsonPropertyName("enterprise_name")]
    public string? EnterpriseName { get; set; }

    /// <summary>获取或设置组成形式（官方 <c>type_of_organization</c>）。</summary>
    [JsonPropertyName("type_of_organization")]
    public string? TypeOfOrganization { get; set; }

    /// <summary>获取或设置经营场所 / 企业住所（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置公司类型（官方 <c>type_of_enterprise</c>）。</summary>
    [JsonPropertyName("type_of_enterprise")]
    public string? TypeOfEnterprise { get; set; }

    /// <summary>获取或设置经营范围（官方 <c>business_scope</c>）。</summary>
    [JsonPropertyName("business_scope")]
    public string? BusinessScope { get; set; }

    /// <summary>获取或设置注册资本（官方 <c>registered_capital</c>）。</summary>
    [JsonPropertyName("registered_capital")]
    public string? RegisteredCapital { get; set; }

    /// <summary>获取或设置实收资本（官方 <c>paid_in_capital</c>）。</summary>
    [JsonPropertyName("paid_in_capital")]
    public string? PaidInCapital { get; set; }

    /// <summary>获取或设置营业期限（官方 <c>valid_period</c>）。</summary>
    [JsonPropertyName("valid_period")]
    public string? ValidPeriod { get; set; }

    /// <summary>获取或设置注册日期 / 成立日期（官方 <c>registered_date</c>）。</summary>
    [JsonPropertyName("registered_date")]
    public string? RegisteredDate { get; set; }

    /// <summary>获取或设置营业执照位置（官方 <c>cert_position</c>）。</summary>
    [JsonPropertyName("cert_position")]
    public MpOcrRect? CertPosition { get; set; }

    /// <summary>获取或设置图片尺寸（官方 <c>img_size</c>）。</summary>
    [JsonPropertyName("img_size")]
    public MpImageSize? ImageSize { get; set; }
}
