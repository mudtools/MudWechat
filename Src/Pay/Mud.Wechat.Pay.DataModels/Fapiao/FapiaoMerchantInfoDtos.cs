// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Fapiao;

/// <summary>查询商户开票基础信息响应·【销售方信息】 商户配置的销售方信息，展示在电子发票的销售方中。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538180"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474053"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantBaseInformationResponseSellerInformation
{
    /// <summary>【名称】 销售方名称（官方字段路径 <c>seller_information.name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【纳税人识别号】 销售方纳税人识别号（官方字段路径 <c>seller_information.taxpayer_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>【地址】 销售方地址。数电模式下无地址信息（官方字段路径 <c>seller_information.address</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【电话】 销售方电话（官方字段路径 <c>seller_information.telephone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>【开户银行】 销售方开户银行（官方字段路径 <c>seller_information.bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 销售方银行账号（官方字段路径 <c>seller_information.bank_account</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

}

/// <summary>查询商户开票基础信息响应·【附加信息】 商户配置的开票附加信息，展示在电子发票下方。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538180"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474053"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantBaseInformationResponseExtraInformation
{
    /// <summary>【收款人】 收款人（官方字段路径 <c>extra_information.payee</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }

    /// <summary>【复核人】 复核人（官方字段路径 <c>extra_information.reviewer</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("reviewer")]
    public string? Reviewer { get; set; }

    /// <summary>【开票人】 开票人（官方字段路径 <c>extra_information.drawer</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("drawer")]
    public string? Drawer { get; set; }

}

/// <summary>查询商户开票基础信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538180"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474053"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantBaseInformationResponse : WechatPayResponse
{
    /// <summary>【销售方信息】 商户配置的销售方信息，展示在电子发票的销售方中（官方字段路径 <c>seller_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("seller_information")]
    public FapiaoMerchantBaseInformationResponseSellerInformation? SellerInformation { get; set; }

    /// <summary>【附加信息】 商户配置的开票附加信息，展示在电子发票下方（官方字段路径 <c>extra_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("extra_information")]
    public FapiaoMerchantBaseInformationResponseExtraInformation? ExtraInformation { get; set; }

}

/// <summary>查询商户开发选项响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529526"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474033"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantDevelopmentConfigResponse : WechatPayResponse
{
    /// <summary>【商户回调地址】 商户/服务商需要设置回调地址，收取微信的授权通知、开票通知、插卡通知等相关通知（官方字段路径 <c>callback_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }

    /// <summary>【全部账单展示开发票入口开关】 指定全部账单是否展示开发票入口（官方字段路径 <c>show_fapiao_cell</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_fapiao_cell")]
    public bool? ShowFapiaoCell { get; set; }

}

/// <summary>配置商户开发选项请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529457"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474031"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792563"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantDevelopmentConfigUpdateRequest
{
    /// <summary>【商户回调地址】 商户/服务商需要设置回调地址，收取微信的授权通知、开票通知、插卡通知等相关通知（官方字段路径 <c>callback_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }

    /// <summary>【全部账单展示开发票入口开关】 指定全部账单是否展示开发票入口（官方字段路径 <c>show_fapiao_cell</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_fapiao_cell")]
    public bool? ShowFapiaoCell { get; set; }

    /// <summary>【子商户号】 微信支付分配的子商户号，服务商为子商户设置全部账单展示发票入口开关时必填（官方字段路径 <c>sub_mch_code</c>，选填，仅<b>服务商面</b>官方页印发，服务商面页标必填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mch_code")]
    public string? SubMchCode { get; set; }

    /// <summary>【商户是否支持开电子专票】 填写抬头页面是否展示选择电子专票开关（官方字段路径 <c>support_vat_fapiao</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("support_vat_fapiao")]
    public bool? SupportVatFapiao { get; set; }

}

/// <summary>配置商户开发选项响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529457"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474031"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792563"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantDevelopmentConfigUpdateResponse : WechatPayResponse
{
    /// <summary>【商户回调地址】 商户/服务商需要设置回调地址，收取微信的授权通知、开票通知、插卡通知等相关通知（官方字段路径 <c>callback_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }

    /// <summary>【全部账单展示开发票入口开关】 指定全部账单是否展示开发票入口（官方字段路径 <c>show_fapiao_cell</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_fapiao_cell")]
    public bool? ShowFapiaoCell { get; set; }

    /// <summary>【商户是否支持开电子专票】 填写抬头页面是否展示选择电子专票开关，完成开发选项配置后返回（官方字段路径 <c>support_vat_fapiao</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("support_vat_fapiao")]
    public bool? SupportVatFapiao { get; set; }

}

/// <summary>查询商户税收分类编码对照表响应·【相关链接】 相关链接。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538236"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474057"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoTaxCodeListResponseLinks
{
    /// <summary>【下一页链接】 使用同样的limit进行下一页查询时的请求链接。如果已经到最后时，为空（官方字段路径 <c>links.next</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>【上一页链接】 使用同样的limit进行上一页查询时的请求链接。如果是第一页，为空（官方字段路径 <c>links.prev</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    /// <summary>【当前链接】 当前的请求链接（官方字段路径 <c>links.self</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }

}

/// <summary>查询商户税收分类编码对照表响应·【本次查询到的数据】 本次查询到的商品及税收分类编码对照表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538236"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474057"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoTaxCodeListResponseDataItem
{
    /// <summary>【由商户自定义的货物或应税劳务、服务名称】 由商户自定义的货物或应税劳务、服务名称（官方字段路径 <c>data.goods_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【企业侧维护的货物或应税劳务、服务编码】 企业侧维护的货物或应税劳务、服务编码。若使用在电子发票商户平台配置的商品类型进行开票时，需要传该编号（官方字段路径 <c>data.goods_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("goods_id")]
    public long? GoodsId { get; set; }

    /// <summary>【税局侧规定的货物或应税劳务、服务分类名称】 税局侧规定的货物或应税劳务、服务分类名称（官方字段路径 <c>data.goods_category</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("goods_category")]
    public string? GoodsCategory { get; set; }

    /// <summary>【税局侧规定的货物或应税劳务、服务税收分类编码】 税局侧规定的货物或应税劳务、服务税收分类编码（官方字段路径 <c>data.tax_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>【税率】 税率，单位为万分之一，如1300代表13%（官方字段路径 <c>data.tax_rate</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>【税收优惠政策标识】 税收优惠政策标识。可选取值：`NO_FAVORABLE` 无优惠 / `OUTSIDE_VAT` 不征税 / `VAT_EXEMPT` 免税 / `NORMAL_ZERO_RATED` 普通零税率 / `EXPORT_ZERO_RATED` 出口零税率（官方字段路径 <c>data.tax_prefer_mark</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("tax_prefer_mark")]
    public string? TaxPreferMark { get; set; }

}

/// <summary>查询商户税收分类编码对照表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538236"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474057"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoTaxCodeListResponse : WechatPayResponse
{
    /// <summary>【本次查询的起始位置】 本次查询的起始位置（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【本次查询的最大数量】 本次查询的最大数量（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【相关链接】 相关链接（官方字段路径 <c>links</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("links")]
    public FapiaoTaxCodeListResponseLinks? Links { get; set; }

    /// <summary>【本次查询到的数据】 本次查询到的商品及税收分类编码对照表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<FapiaoTaxCodeListResponseDataItem>? Data { get; set; }

    /// <summary>【总记录数】 总记录数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

}
