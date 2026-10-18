// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 进件回显的营业执照信息（<c>/shop/wxpay/get</c> 与 <c>get_info</c> 读侧；键名与写侧不同）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayBusinessLicenseInfoEcho
{
    /// <summary>营业执照扫描件 <c>media_id</c>（读侧键名，写侧为 <c>wxa_business_license_copy</c>）。</summary>
    [JsonPropertyName("business_license_copy")]
    public string? BusinessLicenseCopy { get; set; }

    /// <summary>营业执照号 / 统一社会信用代码（敏感，不得进日志）。</summary>
    [JsonPropertyName("business_license_number")]
    public string? BusinessLicenseNumber { get; set; }

    /// <summary>经营者 / 法定代表人姓名（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>商户名称。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }
}
