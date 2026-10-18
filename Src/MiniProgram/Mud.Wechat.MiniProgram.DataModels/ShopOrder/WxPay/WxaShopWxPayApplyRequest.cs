// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 申请开通微信支付请求（<c>/shop/wxpay/apply</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayApplyRequest
{
    /// <summary>主体类型（<b>必填</b>）：<c>4</c> 个体工商户、<c>2</c> 企业；官方参数表类型为 <c>string</c>（示例以数字书写）。</summary>
    [JsonPropertyName("organization_type")]
    public string? OrganizationType { get; set; }

    /// <summary>营业执照 / 登记证书信息（<b>必填</b>）。</summary>
    [JsonPropertyName("business_license_info")]
    public WxaShopWxPayBusinessLicenseInfo? BusinessLicenseInfo { get; set; }

    /// <summary>经营者 / 法人证件类型（<b>必填</b>）：<c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c> 中国大陆居民-身份证、<c>IDENTIFICATION_TYPE_OVERSEA_PASSPORT</c> 其他国家或地区居民-护照、<c>IDENTIFICATION_TYPE_HONGKONG</c> 中国香港居民-来往内地通行证、<c>IDENTIFICATION_TYPE_MACAO</c> 中国澳门居民-来往内地通行证、<c>IDENTIFICATION_TYPE_TAIWAN</c> 中国台湾居民-来往大陆通行证、<c>IDENTIFICATION_TYPE_FOREIGN_RESIDENT</c> 外国人居留证、<c>IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT</c> 港澳居民证、<c>IDENTIFICATION_TYPE_TAIWAN_RESIDENT</c> 台湾居民证。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>经营者 / 法人身份证：证件类型为 <c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c> 时必填。</summary>
    [JsonPropertyName("id_card_info")]
    public WxaShopWxPayIdCardInfo? IdCardInfo { get; set; }

    /// <summary>经营者 / 法人其他类型证件信息：证件类型为来往内地通行证 / 来往大陆通行证 / 护照时填写。</summary>
    [JsonPropertyName("id_doc_info")]
    public WxaShopWxPayIdDocInfo? IdDocInfo { get; set; }

    /// <summary>超级管理员信息（<b>必填</b>）：管理员需在开户后签约，并可接收日常重要管理信息和进行资金操作。</summary>
    [JsonPropertyName("contact_info")]
    public WxaShopWxPayContactInfo? ContactInfo { get; set; }

    /// <summary>特殊资质：按经营业务要求提供《行业对应特殊资质》，最多 5 张，须填写经图片上传接口生成的 <c>media_id</c>（选填）。</summary>
    [JsonPropertyName("qualifications")]
    public WxaShopWxPayPicFileList? Qualifications { get; set; }

    /// <summary>补充材料（选填，最多 5 张）。</summary>
    [JsonPropertyName("business_addition_pics")]
    public WxaShopWxPayPicFileList? BusinessAdditionPics { get; set; }

    /// <summary>补充描述（选填）。</summary>
    [JsonPropertyName("business_addition_desc")]
    public string? BusinessAdditionDesc { get; set; }

    /// <summary>结算账户信息（<b>必填</b>）。</summary>
    [JsonPropertyName("account_info")]
    public WxaShopWxPayAccountInfo? AccountInfo { get; set; }

    /// <summary>商户简称（<b>必填</b>）：UTF-8 中文占 3 字节，最多 21 个汉字；将在支付完成页向买家展示，须与实际售卖商品相符。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortname { get; set; }
}
