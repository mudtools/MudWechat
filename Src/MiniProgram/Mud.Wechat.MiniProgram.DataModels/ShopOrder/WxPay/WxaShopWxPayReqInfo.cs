// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 进件信息回显（<c>/shop/wxpay/get</c> 的 <c>wxpay_req</c>、<c>get_info</c> 的 <c>pay_req</c>；两页为同一进件数据的读侧形态）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayReqInfo
{
    /// <summary>营业执照 / 登记证书信息（读侧）。</summary>
    [JsonPropertyName("business_license_info")]
    public WxaShopWxPayBusinessLicenseInfoEcho? BusinessLicenseInfo { get; set; }

    /// <summary>超级管理员信息（读侧，敏感）。</summary>
    [JsonPropertyName("contact_info")]
    public WxaShopWxPayContactInfoEcho? ContactInfo { get; set; }

    /// <summary>经营者 / 法人身份证信息（读侧，敏感）。</summary>
    [JsonPropertyName("id_card_info")]
    public WxaShopWxPayIdCardInfoEcho? IdCardInfo { get; set; }

    /// <summary>商户简称。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortname { get; set; }

    /// <summary>是否需要结算账户信息（仅进件状态回显出现）。</summary>
    [JsonPropertyName("need_account_info")]
    public bool? NeedAccountInfo { get; set; }

    /// <summary>主体类型（读侧以字符串回显）。</summary>
    [JsonPropertyName("organization_type")]
    public string? OrganizationType { get; set; }

    /// <summary>商户自定义进件单号。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>销售场景信息。</summary>
    [JsonPropertyName("sales_scene_info")]
    public WxaShopWxPaySalesSceneInfo? SalesSceneInfo { get; set; }
}
