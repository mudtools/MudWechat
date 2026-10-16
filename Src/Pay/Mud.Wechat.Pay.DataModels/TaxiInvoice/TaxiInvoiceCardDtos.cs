// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.TaxiInvoice;

/// <summary>将出租车电子发票插入用户卡包请求·【发票卡券信息】 发票卡券信息·【购买方信息】 购买方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCardInsertRequestFapiaoCardInformationBuyerInformation
{
    /// <summary>【购买方类型】 购买方类型。可选取值：`CORPORATION` 公司 / `INDIVIDUAL` 个体工商户（官方字段路径 <c>fapiao_card_information.buyer_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【名称】 购买方名称（官方字段路径 <c>fapiao_card_information.buyer_information.name</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【纳税人识别号】 购买方纳税人识别号，购买方类型为ORGANIZATION时必须存在（官方字段路径 <c>fapiao_card_information.buyer_information.taxpayer_id</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>【地址】 购买方地址（官方字段路径 <c>fapiao_card_information.buyer_information.address</c>，选填，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【电话】 购买方电话（官方字段路径 <c>fapiao_card_information.buyer_information.telephone</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>【开户银行】 购买方开户银行（官方字段路径 <c>fapiao_card_information.buyer_information.bank_name</c>，选填，类型 <c>string(70)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 购买方银行账号（官方字段路径 <c>fapiao_card_information.buyer_information.bank_account</c>，选填，类型 <c>string(30)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

}

/// <summary>将出租车电子发票插入用户卡包请求·【发票卡券信息】 发票卡券信息·【销售方信息】 销售方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCardInsertRequestFapiaoCardInformationSellerInformation
{
    /// <summary>【名称】 销售方名称（官方字段路径 <c>fapiao_card_information.seller_information.name</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【纳税人识别号】 销售方纳税人识别号（官方字段路径 <c>fapiao_card_information.seller_information.taxpayer_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>【地址】 销售方地址（官方字段路径 <c>fapiao_card_information.seller_information.address</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【电话】 销售方电话（官方字段路径 <c>fapiao_card_information.seller_information.telephone</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>【开户银行】 销售方开户银行（官方字段路径 <c>fapiao_card_information.seller_information.bank_name</c>，选填，类型 <c>string(70)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 销售方银行账号（官方字段路径 <c>fapiao_card_information.seller_information.bank_account</c>，选填，类型 <c>string(30)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

}

/// <summary>将出租车电子发票插入用户卡包请求·【发票卡券信息】 发票卡券信息·【附加信息】 附加信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCardInsertRequestFapiaoCardInformationExtraInformation
{
    /// <summary>【收款人】 收款人（官方字段路径 <c>fapiao_card_information.extra_information.payee</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }

    /// <summary>【复核人】 复核人（官方字段路径 <c>fapiao_card_information.extra_information.reviewer</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("reviewer")]
    public string? Reviewer { get; set; }

    /// <summary>【开票人】 开票人（官方字段路径 <c>fapiao_card_information.extra_information.drawer</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("drawer")]
    public string? Drawer { get; set; }

}

/// <summary>将出租车电子发票插入用户卡包请求·【发票卡券信息】 发票卡券信息·【发票行信息】 发票行信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCardInsertRequestFapiaoCardInformationItemsItem
{
    /// <summary>【货物或应税劳务、服务编码】 货物或应税劳务、服务编码（官方字段路径 <c>fapiao_card_information.items.tax_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>【货物或应税劳务、服务名称】 货物或应税劳务、服务名称（官方字段路径 <c>fapiao_card_information.items.goods_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【规格型号】 规格型号（官方字段路径 <c>fapiao_card_information.items.specification</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("specification")]
    public string? Specification { get; set; }

    /// <summary>【单位】 单位（官方字段路径 <c>fapiao_card_information.items.unit</c>，选填，类型 <c>string(14)</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>【数量】 数量，单位为10-8分，100000000表示数量为1（官方字段路径 <c>fapiao_card_information.items.quantity</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>【单价】 单价，单位为10-6分，100000000表示1元（官方字段路径 <c>fapiao_card_information.items.unit_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>【单行金额】 单行金额，单位：分（官方字段路径 <c>fapiao_card_information.items.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【单行税额】 单行税额，单位：分（官方字段路径 <c>fapiao_card_information.items.tax_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("tax_amount")]
    public long? TaxAmount { get; set; }

    /// <summary>【单行金额合计】 单行金额和税费的和，单位:分（官方字段路径 <c>fapiao_card_information.items.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【税率】 税率，单位为万分之一，如1300代表13%（官方字段路径 <c>fapiao_card_information.items.tax_rate</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>【税收优惠政策标识】 税收优惠政策标识。可选取值：`NO_FAVORABLE` 无优惠 / `OUTSIDE_VAT` 不征税 / `VAT_EXEMPT` 免税 / `NORMAL_ZERO_RATED` 普通零税率 / `EXPORT_ZERO_RATED` 出口零税率（官方字段路径 <c>fapiao_card_information.items.tax_prefer_mark</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("tax_prefer_mark")]
    public string? TaxPreferMark { get; set; }

    /// <summary>【是否折扣行】 指定该发票行是否折扣行，折扣行一定是被折扣行的下一行（官方字段路径 <c>fapiao_card_information.items.discount</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("discount")]
    public bool? Discount { get; set; }

}

/// <summary>将出租车电子发票插入用户卡包请求·【发票卡券信息】 发票卡券信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCardInsertRequestFapiaoCardInformation
{
    /// <summary>【电子发票文件ID】 上传的电子发票文件对应的ID，可通过调用【将出租车电子发票插入微信用户卡包】获取（官方字段路径 <c>fapiao_card_information.fapiao_media_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("fapiao_media_id")]
    public string? FapiaoMediaId { get; set; }

    /// <summary>【发票号码】 发票号码（官方字段路径 <c>fapiao_card_information.fapiao_number</c>，<b>必填</b>，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("fapiao_number")]
    public string? FapiaoNumber { get; set; }

    /// <summary>【发票代码】 发票代码（官方字段路径 <c>fapiao_card_information.fapiao_code</c>，<b>必填</b>，类型 <c>string(12)</c>）。</summary>
    [JsonPropertyName("fapiao_code")]
    public string? FapiaoCode { get; set; }

    /// <summary>【开票时间】 开票时间，遵循 RFC3339 标准格式（官方字段路径 <c>fapiao_card_information.fapiao_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("fapiao_time")]
    public string? FapiaoTime { get; set; }

    /// <summary>【校验码】 校验码（官方字段路径 <c>fapiao_card_information.check_code</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("check_code")]
    public string? CheckCode { get; set; }

    /// <summary>【密码】 发票票面密码区内容（官方字段路径 <c>fapiao_card_information.password</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>【总价税合计】 总价税合计，所有发票行单行金额合计的累加，单位：分（官方字段路径 <c>fapiao_card_information.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【总税额】 总税额，所有发票行单行税额的累加，单位：分（官方字段路径 <c>fapiao_card_information.tax_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("tax_amount")]
    public long? TaxAmount { get; set; }

    /// <summary>【总金额】 总金额，所有发票行单行金额的累加，单位：分（官方字段路径 <c>fapiao_card_information.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【购买方信息】 购买方信息（官方字段路径 <c>fapiao_card_information.buyer_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("buyer_information")]
    public TaxiInvoiceCardInsertRequestFapiaoCardInformationBuyerInformation? BuyerInformation { get; set; }

    /// <summary>【销售方信息】 销售方信息（官方字段路径 <c>fapiao_card_information.seller_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("seller_information")]
    public TaxiInvoiceCardInsertRequestFapiaoCardInformationSellerInformation? SellerInformation { get; set; }

    /// <summary>【附加信息】 附加信息（官方字段路径 <c>fapiao_card_information.extra_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("extra_information")]
    public TaxiInvoiceCardInsertRequestFapiaoCardInformationExtraInformation? ExtraInformation { get; set; }

    /// <summary>【发票行信息】 发票行信息（官方字段路径 <c>fapiao_card_information.items</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("items")]
    public List<TaxiInvoiceCardInsertRequestFapiaoCardInformationItemsItem>? Items { get; set; }

    /// <summary>【备注信息】 备注信息（官方字段路径 <c>fapiao_card_information.remark</c>，选填，类型 <c>string(200)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

}

/// <summary>将出租车电子发票插入用户卡包请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCardInsertRequest
{
    /// <summary>【出租车经营主体商户号】 该发票关联的出租车经营主体的商户号（官方字段路径 <c>company_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("company_mchid")]
    public string? CompanyMchid { get; set; }

    /// <summary>【获取乘客行程单详情的凭证】 获取乘客行程单详情的凭证（官方字段路径 <c>token</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>【微信用户申请开票使用的AppID】 微信用户申请开票使用的公众号或小程序的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【微信用户OpenID】 微信用户在此AppID下的用户标识，发票卡券会插入该用户的卡包（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

    /// <summary>【发票卡券信息】 发票卡券信息（官方字段路径 <c>fapiao_card_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fapiao_card_information")]
    public TaxiInvoiceCardInsertRequestFapiaoCardInformation? FapiaoCardInformation { get; set; }

}

/// <summary>
/// 上传出租车电子发票文件响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692904"/>
/// </summary>
/// <remarks>
/// <para>
/// 本支官方请求体是 <c>multipart/form-data</c>（表单 <c>file</c> + <c>meta</c>）⇒ <b>不由声明式端点承载</b>，
/// 手写通道 <c>WechatPayTaxiInvoiceFileService</c> 只把<b>应答</b>落成 DTO；
/// <c>meta</c> 的 4 个字段（<c>company_mchid</c> / <c>digest</c> / <c>digest_algorithm</c> / <c>region_id</c>）
/// 只出现在表单段，故<b>不建请求 DTO</b>（同电子发票域口径），字段名常量锁在通道源码里。
/// </para>
/// <para>
/// <b><c>digest_algorithm</c> 的拼写是本页与电子发票上传页的唯一契约差异</b>：本页拼写正常，
/// 而 <c>/v3/new-tax-control-fapiao/.../upload-fapiao-file</c> 官方印的是 <c>digest_alogrithm</c>（少一个 r）
/// ⇒ 两支各照各页，不得互相「纠正」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceUploadFileResponse : WechatPayResponse
{
    /// <summary>【发票文件ID】 上传的出租车电子发票文件对应的ID，用于「将出租车电子发票插入微信用户卡包」接口。有效期为3天（官方字段路径 <c>fapiao_media_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fapiao_media_id")]
    public string? FapiaoMediaId { get; set; }

}
