// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 营业执照信息（官方 <c>busi_license</c>）。
/// </summary>
/// <remarks>
/// <para><c>registration_num</c> 为 15 位数字或 18 位大写字母；<c>end_date</c> 为长期时填「长期」，且有效期须大于 60 天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductBusiLicense
{
    /// <summary>营业执照类型（1 三证合一 / 2 普通营业执照）。</summary>
    [JsonPropertyName("license_type")]
    public int? LicenseType { get; set; }

    /// <summary>营业执照图片。</summary>
    [JsonPropertyName("pic_file")]
    public WxaProductPicFile? PicFile { get; set; }

    /// <summary>注册号 / 统一社会信用代码。</summary>
    [JsonPropertyName("registration_num")]
    public string? RegistrationNum { get; set; }

    /// <summary>商家名称（2~110 字符）。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>经营者 / 法定代表人姓名（敏感）。</summary>
    [JsonPropertyName("legal_representative")]
    public string? LegalRepresentative { get; set; }

    /// <summary>注册地址。</summary>
    [JsonPropertyName("registered_addrs")]
    public string? RegisteredAddrs { get; set; }

    /// <summary>注册日期（yyyy-MM-dd）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>结束有效期（yyyy-MM-dd 或「长期」）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }
}
