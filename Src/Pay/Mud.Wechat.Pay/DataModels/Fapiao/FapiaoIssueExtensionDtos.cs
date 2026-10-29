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

/// <summary>开具通用行业电子发票请求·【购买方信息】 购买方信息，即发票抬头。若商户使用微信官方抬头，可从 【获取用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792574"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoGeneralIssueRequestBuyerInformation
{
    /// <summary>【购买方类型】 购买方类型。可选取值：`INDIVIDUAL` 个人、其他 / `ORGANIZATION` 单位（官方字段路径 <c>buyer_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【名称】 购买方名称（官方字段路径 <c>buyer_information.name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【纳税人识别号】 购买方纳税人识别号，购买方类型为ORGANIZATION时必填（官方字段路径 <c>buyer_information.taxpayer_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>【地址】 购买方地址（官方字段路径 <c>buyer_information.address</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【电话】 购买方电话（官方字段路径 <c>buyer_information.telephone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>【开户银行名称】 购买方开户银行名称（官方字段路径 <c>buyer_information.bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 购买方银行账号（官方字段路径 <c>buyer_information.bank_account</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>【手机号】 用户手机号。注意：该字段为密文字段，作为请求字段需加密，使用微信支付公钥加密（推荐），参考获取微信支付公钥ID说明以及微信支付公钥加密敏感信息指引；也可以使用微信支付平台证书公钥加密，参考获取平台证书序列号、平台证书加密敏感信息指引；作为响应字段需解密，参考如何使用API证书解密敏感字段（官方字段路径 <c>buyer_information.phone</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>【邮箱地址】 用户邮箱地址。注意：该字段为密文字段，作为请求字段需加密，使用微信支付公钥加密（推荐），参考获取微信支付公钥ID说明以及微信支付公钥加密敏感信息指引；也可以使用微信支付平台证书公钥加密，参考获取平台证书序列号、平台证书加密敏感信息指引；作为响应字段需解密，参考如何使用API证书解密敏感字段（官方字段路径 <c>buyer_information.email</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>【订单金额】 微信支付订单金额，仅在微信支付场景下有效，单位：分（官方字段路径 <c>buyer_information.amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【商户订单号】 商户订单号，仅在微信支付场景下有效（官方字段路径 <c>buyer_information.out_trade_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【开具的发票类型】 用户选择提交开具发票的类型。可选取值：`COMM_FAPIAO` 增值税普通发票 / `VAT_FAPIAO` 增值税专用发票（官方字段路径 <c>buyer_information.fapiao_bill_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fapiao_bill_type")]
    public string? FapiaoBillType { get; set; }

    /// <summary>【留言信息】 用户申请开票时提交的留言信息（官方字段路径 <c>buyer_information.user_apply_message</c>，选填，类型 <c>string(100)</c>）。</summary>
    [JsonPropertyName("user_apply_message")]
    public string? UserApplyMessage { get; set; }

}

/// <summary>开具通用行业电子发票请求·【需要开具的数电发票信息】 需要开具的通用行业数电发票信息·【发票行信息】 发票行信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792574"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoGeneralIssueRequestFapiaoInformationItemsItem
{
    /// <summary>【税局侧规定的货物或应税劳务、服务税收分类编码】 税局侧规定的货物或应税劳务、服务税收分类编码。可自行指定符合税务部门规定的货物或应税劳务、服务编码（官方字段路径 <c>fapiao_information.items.tax_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>【货物或应税劳务、服务名称】 由商户自定义的货物或应税劳务、服务名称（官方字段路径 <c>fapiao_information.items.goods_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【规格型号】 规格型号，展示在发票中间的规格型号列（官方字段路径 <c>fapiao_information.items.specification</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("specification")]
    public string? Specification { get; set; }

    /// <summary>【单位】 单位，展示在发票中间的单位列。若是折扣行，则不填（官方字段路径 <c>fapiao_information.items.unit</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>【数量】 数量，展示在发票中间的数量列，单位为10-8，100000000表示数量为1。非折扣行不填时默认为100000000，表示数量为1。若是折扣行，则不填（官方字段路径 <c>fapiao_information.items.quantity</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>【单行金额合计】 单行金额和税费的和，折扣行的金额为负数，非折扣行的金额为正数，单位:分（官方字段路径 <c>fapiao_information.items.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【税率】 税率，单位为万分之一，如1300代表13%。 现在支持的税率为0、1%、1.5%、3%、5%、6%、9%、10%、11%、13%、16%、17%（官方字段路径 <c>fapiao_information.items.tax_rate</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>【是否折扣行】 指定该发票行是否折扣行，折扣行必须是被折扣行的下一行（官方字段路径 <c>fapiao_information.items.discount</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("discount")]
    public bool? Discount { get; set; }

    /// <summary>【优惠政策标识】 优惠政策标识。1:简易征收; 2:稀土产品; 3:免税; 4:不征税; 5:先征后退; 6:100%先征后退; 7:50%先征后退; 8:按3%简易征收; 9:按5%简易征收; 10:按5%简易征收减按1.5%计征; 11:即征即退30%; 12:即征即退50%; 13:即征即退70%; 14:即征即退100%; 15:超税负3%即征即退; 16:超税负8%即征即退; 17:超税负12%即征即退; 18:超税负6%即征即退（官方字段路径 <c>fapiao_information.items.preferential_policy_code</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("preferential_policy_code")]
    public long? PreferentialPolicyCode { get; set; }

}

/// <summary>开具通用行业电子发票请求·【需要开具的数电发票信息】 需要开具的通用行业数电发票信息·【交易信息】 发票对应的交易信息，最大支持传入10条交易信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792574"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoGeneralIssueRequestFapiaoInformationTransactionInformationItem
{
    /// <summary>【支付渠道】 发票对应交易所属的交易渠道。可选取值：`WECHAT_PAY` 微信支付（官方字段路径 <c>fapiao_information.transaction_information.pay_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("pay_channel")]
    public string? PayChannel { get; set; }

    /// <summary>【支付订单号】 发票对应交易的支付订单号，支付系统生成的订单号。微信支付交易渠道时，支付订单号、支付商户订单号必须传递其中一个参数（官方字段路径 <c>fapiao_information.transaction_information.transaction_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【支付商户订单号】 发票对应交易的商户订单号，商户系统内部订单号。微信支付交易渠道时，支付订单号、支付商户订单号必须传递其中一个参数（官方字段路径 <c>fapiao_information.transaction_information.out_trade_no</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【交易金额】 发票对应交易信息的交易金额，单位：分（官方字段路径 <c>fapiao_information.transaction_information.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

}

/// <summary>开具通用行业电子发票请求·【需要开具的数电发票信息】 需要开具的通用行业数电发票信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792574"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoGeneralIssueRequestFapiaoInformation
{
    /// <summary>【商户发票单号】 商户发票单号，唯一标识一张要开具的发票。只能是字母、数字、中划线-、下划线_、竖线&amp;#124;、星号*这些英文半角字符，且该单号在每个商户下必须唯一 当申请单下发票状态status为‘ISSUE_FAILED’-发票开具已失败/‘REVERSED’-发票已冲红时，可以更换fapiao_id重新开票。1个fapiao_apply_id下最多支持5个fapiao_id（官方字段路径 <c>fapiao_information.fapiao_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>【总价税合计】 总价税合计，所有发票行单行金额合计的累加，展示在发票的价税合计处，单位：分 注意：所有发票的总价税合计之和不能超过交易的总金额（官方字段路径 <c>fapiao_information.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【发票行信息】 发票行信息（官方字段路径 <c>fapiao_information.items</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("items")]
    public List<FapiaoGeneralIssueRequestFapiaoInformationItemsItem>? Items { get; set; }

    /// <summary>【出口业务适用政策代码】 1:退税政策; 2:免税政策; 3:征税政策（官方字段路径 <c>fapiao_information.export_business_policy_code</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("export_business_policy_code")]
    public long? ExportBusinessPolicyCode { get; set; }

    /// <summary>【增值税即征即退代码】 1:软件产品; 2:资源综合利用产品; 3:管道运输服务; 4:有形动产融资租赁服务; 5:有形动产融资性售后回租服务; 6:新型墙体材料; 7:风力发电产品; 8:光伏发电产品; 9:动漫软件产品; 10:飞机维修劳务; 11:黄金; 12:铂金（官方字段路径 <c>fapiao_information.vat_refund_levy_code</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("vat_refund_levy_code")]
    public long? VatRefundLevyCode { get; set; }

    /// <summary>【开票人ID】 税局乐企系统登记的开票人ID（官方字段路径 <c>fapiao_information.billing_person_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("billing_person_id")]
    public string? BillingPersonId { get; set; }

    /// <summary>【开票人名称】 税局乐企系统登记的开票人名称的脱敏格式，格式为：除最后一个字外其它字用*填充的姓名+空格+身份证后四位（官方字段路径 <c>fapiao_information.billing_person</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("billing_person")]
    public string? BillingPerson { get; set; }

    /// <summary>【发票类型】 商户提交申请开票的发票类型。可选取值：`COMM_FAPIAO` 增值税普通发票 / `VAT_FAPIAO` 增值税专用发票（官方字段路径 <c>fapiao_information.fapiao_bill_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fapiao_bill_type")]
    public string? FapiaoBillType { get; set; }

    /// <summary>【交易信息】 发票对应的交易信息，最大支持传入10条交易信息（官方字段路径 <c>fapiao_information.transaction_information</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("transaction_information")]
    public List<FapiaoGeneralIssueRequestFapiaoInformationTransactionInformationItem>? TransactionInformation { get; set; }

    /// <summary>【发票备注】 发票备注（官方字段路径 <c>fapiao_information.remark</c>，选填，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

}

/// <summary>开具通用行业电子发票请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792574"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoGeneralIssueRequest
{
    /// <summary>【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【发票申请单号】 发票申请单号，唯一标识一次开票行为，关联唯一的购买方信息。当微信支付账单开票时，为微信支付交易单号（官方字段路径 <c>fapiao_apply_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("fapiao_apply_id")]
    public string? FapiaoApplyId { get; set; }

    /// <summary>【购买方信息】 购买方信息，即发票抬头。若商户使用微信官方抬头，可从 【获取用户填写抬头信息】 接口获取用户填写的抬头；也可自行收集发票抬头（官方字段路径 <c>buyer_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("buyer_information")]
    public FapiaoGeneralIssueRequestBuyerInformation? BuyerInformation { get; set; }

    /// <summary>【需要开具的数电发票信息】 需要开具的通用行业数电发票信息（官方字段路径 <c>fapiao_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fapiao_information")]
    public FapiaoGeneralIssueRequestFapiaoInformation? FapiaoInformation { get; set; }

}

/// <summary>开具不动产租赁行业电子发票请求·【购买方信息】 购买方信息，即发票抬头。若商户使用微信官方抬头，可从 【获取用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941740"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoRealEstateLeasingIssueRequestBuyerInformation
{
    /// <summary>【购买方类型】 购买方类型。可选取值：`INDIVIDUAL` 个人、其他 / `ORGANIZATION` 单位（官方字段路径 <c>buyer_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【名称】 购买方名称（官方字段路径 <c>buyer_information.name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【纳税人识别号】 购买方纳税人识别号，购买方类型为ORGANIZATION时必填（官方字段路径 <c>buyer_information.taxpayer_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>【地址】 购买方地址（官方字段路径 <c>buyer_information.address</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【电话】 购买方电话（官方字段路径 <c>buyer_information.telephone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>【开户银行名称】 购买方开户银行名称（官方字段路径 <c>buyer_information.bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 购买方银行账号（官方字段路径 <c>buyer_information.bank_account</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>【手机号】 用户手机号。注意：该字段为密文字段，作为请求字段需加密，使用微信支付公钥加密（推荐），参考获取微信支付公钥ID说明以及微信支付公钥加密敏感信息指引；也可以使用微信支付平台证书公钥加密，参考获取平台证书序列号、平台证书加密敏感信息指引；作为响应字段需解密，参考如何使用API证书解密敏感字段（官方字段路径 <c>buyer_information.phone</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>【邮箱地址】 用户邮箱地址。注意：该字段为密文字段，作为请求字段需加密，使用微信支付公钥加密（推荐），参考获取微信支付公钥ID说明以及微信支付公钥加密敏感信息指引；也可以使用微信支付平台证书公钥加密，参考获取平台证书序列号、平台证书加密敏感信息指引；作为响应字段需解密，参考如何使用API证书解密敏感字段（官方字段路径 <c>buyer_information.email</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>【订单金额】 微信支付订单金额，仅在微信支付场景下有效，单位：分（官方字段路径 <c>buyer_information.amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【商户订单号】 商户订单号，仅在微信支付场景下有效（官方字段路径 <c>buyer_information.out_trade_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【开具的发票类型】 用户选择提交开具发票的类型。可选取值：`COMM_FAPIAO` 增值税普通发票 / `VAT_FAPIAO` 增值税专用发票（官方字段路径 <c>buyer_information.fapiao_bill_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fapiao_bill_type")]
    public string? FapiaoBillType { get; set; }

    /// <summary>【留言信息】 用户申请开票时提交的留言信息（官方字段路径 <c>buyer_information.user_apply_message</c>，选填，类型 <c>string(100)</c>）。</summary>
    [JsonPropertyName("user_apply_message")]
    public string? UserApplyMessage { get; set; }

}

/// <summary>开具不动产租赁行业电子发票请求·【需要开具的数电发票信息】 需要开具的不动产租赁特定行业数电发票信息·【发票行信息】 发票行信息，单张发票的发票行不能超过8行·【不动产租赁特定行业额外信息】 不动产租赁特定行业额外信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941740"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoRealEstateLeasingIssueRequestFapiaoInformationItemsItemRealEstateLeasingInformation
{
    /// <summary>【不动产坐落省份】 不动产坐落省份必须以省、自治区、特别行政区、北京市、天津市、上海市、重庆市任意一个关键词结尾。涉及多笔不动产租赁订单时，该字段值必须相同（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.province</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>【不动产坐落城市】 不动产坐落城市。必须以市、盟、自治州、地区、区任意一个关键词结尾。不动产坐落地址（省）”为香港特别行政区、澳门特别行政区、台湾省时，“不动产坐落地址（市）”可为空，其他情况必填。当“不动产坐落地址（省）”填写重庆市、海南省时，“不动产坐落地址（市）”可以“县”结尾（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.city</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>【不动产坐落地址（详细地址）】 不动产坐落详细地址。必须包含街、路、村、乡、镇、道、巷、号任意一个关键词。发票版式文件上展示的不动产地址取值，将由“不动产坐落地址（省）”、“不动产坐落地址（市）”、“不动产坐落地址（详细地址）”三个字段拼接而成（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.address</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【租赁期起】 租赁期起，遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.lease_period_start</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("lease_period_start")]
    public string? LeasePeriodStart { get; set; }

    /// <summary>【租赁期止】 租赁期起，遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.lease_period_end</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("lease_period_end")]
    public string? LeasePeriodEnd { get; set; }

    /// <summary>【跨地（市）标志】 销售方营业执照注册地址与不动产地址中地市一致时，为false，不一致时，为true（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.cross_city_flag</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("cross_city_flag")]
    public bool? CrossCityFlag { get; set; }

    /// <summary>【产权证书/不动产权证号】 有"产权证书/不动产权证号"时，则填入详细证号。 无"产权证书/不动产权证号"时，则不填。 注意：该字段为密文字段，需要使用微信支付公钥加密（推荐），参考获取微信支付公钥ID说明以及微信支付公钥加密敏感信息指引；也可以使用微信支付平台证书公钥加密，参考获取平台证书序列号、平台证书加密敏感信息指引（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.certificate_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("certificate_no")]
    public string? CertificateNo { get; set; }

    /// <summary>【车牌号】 发票行信息tax_code为"3040502020200000000"时，此处必填，否则为空。最多填入3个车牌号（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information.plate_no</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("plate_no")]
    public List<string>? PlateNo { get; set; }

}

/// <summary>开具不动产租赁行业电子发票请求·【需要开具的数电发票信息】 需要开具的不动产租赁特定行业数电发票信息·【发票行信息】 发票行信息，单张发票的发票行不能超过8行。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941740"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoRealEstateLeasingIssueRequestFapiaoInformationItemsItem
{
    /// <summary>【税局侧规定的货物或应税劳务、服务税收分类编码】 税局侧规定的货物或应税劳务、服务税收分类编码。可自行指定符合税务部门规定的货物或应税劳务、服务编码；当税收分类编码为3040502020102000000 个人出租住房时，销售方（纳税人）必须为个体工商户或自然人。个体工商户指的是税局【登记注册类型】为个体工商户。因乐企当前预检不支持个人或者个人工商户接入，暂时不支持个人出租房业务（官方字段路径 <c>fapiao_information.items.tax_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>【货物或应税劳务、服务名称】 由商户自定义的货物或应税劳务、服务名称（官方字段路径 <c>fapiao_information.items.goods_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【规格型号】 规格型号，展示在发票中间的规格型号列（官方字段路径 <c>fapiao_information.items.specification</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("specification")]
    public string? Specification { get; set; }

    /// <summary>【面积单位】 面积单位，展示在发票中间的单位列。此场景下，只能选择米、平方千米、公顷、亩、h㎡、k㎡、㎡。非折扣行必填。若是折扣行，必须为空 当“商品和服务税收分类合并编码”为“3040502020200000000”时，此处只能填写“平方米”。可选取值：`METER` 米 / `SQUARE_METER` 平方米 / `SQUARE_KILOMETER` 平方千米 / `HECTARE` 公顷 / `SQUARE_KILOMETER_ABBREVIATION` k㎡ / `SQUARE_METER_ABBREVIATION` ㎡ / `HECTARE_ABBREVIATION` h㎡ / `MU` 亩（官方字段路径 <c>fapiao_information.items.unit</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>【数量】 数量，展示在发票中间的数量列，单位为10-8，100000000表示数量为1。非折扣行不填时默认为100000000，表示数量为1。若是折扣行，则不填（官方字段路径 <c>fapiao_information.items.quantity</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>【单行金额合计】 单行金额和税费的和，折扣行的金额为负数，非折扣行的金额为正数，单位:分（官方字段路径 <c>fapiao_information.items.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【税率】 税率，单位为万分之一，如1300代表13%。 现在支持的税率为0、1%、1.5%、3%、5%、6%、9%、10%、11%、13%、16%、17%（官方字段路径 <c>fapiao_information.items.tax_rate</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>【是否折扣行】 指定该发票行是否折扣行，折扣行必须是被折扣行的下一行（官方字段路径 <c>fapiao_information.items.discount</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("discount")]
    public bool? Discount { get; set; }

    /// <summary>【优惠政策标识】 优惠政策标识。1:简易征收; 2:稀土产品; 3:免税; 4:不征税; 5:先征后退; 6:100%先征后退; 7:50%先征后退; 8:按3%简易征收; 9:按5%简易征收; 10:按5%简易征收减按1.5%计征; 11:即征即退30%; 12:即征即退50%; 13:即征即退70%; 14:即征即退100%; 15:超税负3%即征即退; 16:超税负8%即征即退; 17:超税负12%即征即退; 18:超税负6%即征即退（官方字段路径 <c>fapiao_information.items.preferential_policy_code</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("preferential_policy_code")]
    public long? PreferentialPolicyCode { get; set; }

    /// <summary>【不动产租赁特定行业额外信息】 不动产租赁特定行业额外信息（官方字段路径 <c>fapiao_information.items.real_estate_leasing_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("real_estate_leasing_information")]
    public FapiaoRealEstateLeasingIssueRequestFapiaoInformationItemsItemRealEstateLeasingInformation? RealEstateLeasingInformation { get; set; }

}

/// <summary>开具不动产租赁行业电子发票请求·【需要开具的数电发票信息】 需要开具的不动产租赁特定行业数电发票信息·【交易信息】 发票对应的交易信息，最大支持传入10条交易信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941740"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoRealEstateLeasingIssueRequestFapiaoInformationTransactionInformationItem
{
    /// <summary>【支付渠道】 发票对应交易所属的交易渠道。可选取值：`WECHAT_PAY` 微信支付（官方字段路径 <c>fapiao_information.transaction_information.pay_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("pay_channel")]
    public string? PayChannel { get; set; }

    /// <summary>【支付订单号】 发票对应交易的支付订单号，支付系统生成的订单号。微信支付交易渠道时，支付订单号、支付商户订单号必须传递其中一个参数（官方字段路径 <c>fapiao_information.transaction_information.transaction_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【支付商户订单号】 发票对应交易的商户订单号，商户系统内部订单号。微信支付交易渠道时，支付订单号、支付商户订单号必须传递其中一个参数（官方字段路径 <c>fapiao_information.transaction_information.out_trade_no</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【交易金额】 发票对应交易信息的交易金额，单位：分（官方字段路径 <c>fapiao_information.transaction_information.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

}

/// <summary>开具不动产租赁行业电子发票请求·【需要开具的数电发票信息】 需要开具的不动产租赁特定行业数电发票信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941740"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoRealEstateLeasingIssueRequestFapiaoInformation
{
    /// <summary>【商户发票单号】 商户发票单号，唯一标识一张要开具的发票。只能是字母、数字、中划线-、下划线_、竖线&amp;#124;、星号*这些英文半角字符，且该单号在每个商户下必须唯一 当申请单下发票状态status为‘ISSUE_FAILED’-发票开具已失败/‘REVERSED’-发票已冲红时，可以更换fapiao_id重新开票。1个fapiao_apply_id下最多支持5个fapiao_id（官方字段路径 <c>fapiao_information.fapiao_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>【总价税合计】 总价税合计，所有发票行单行金额合计的累加，展示在发票的价税合计处，单位：分 注意：所有发票的总价税合计之和不能超过交易的总金额（官方字段路径 <c>fapiao_information.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【发票行信息】 发票行信息，单张发票的发票行不能超过8行（官方字段路径 <c>fapiao_information.items</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("items")]
    public List<FapiaoRealEstateLeasingIssueRequestFapiaoInformationItemsItem>? Items { get; set; }

    /// <summary>【出口业务适用政策代码】 1:退税政策; 2:免税政策; 3:征税政策（官方字段路径 <c>fapiao_information.export_business_policy_code</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("export_business_policy_code")]
    public long? ExportBusinessPolicyCode { get; set; }

    /// <summary>【增值税即征即退代码】 1:软件产品; 2:资源综合利用产品; 3:管道运输服务; 4:有形动产融资租赁服务; 5:有形动产融资性售后回租服务; 6:新型墙体材料; 7:风力发电产品; 8:光伏发电产品; 9:动漫软件产品; 10:飞机维修劳务; 11:黄金; 12:铂金（官方字段路径 <c>fapiao_information.vat_refund_levy_code</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("vat_refund_levy_code")]
    public long? VatRefundLevyCode { get; set; }

    /// <summary>【开票人ID】 税局乐企系统登记的开票人ID（官方字段路径 <c>fapiao_information.billing_person_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("billing_person_id")]
    public string? BillingPersonId { get; set; }

    /// <summary>【开票人名称】 税局乐企系统登记的脱敏后开票人名称，格式为：除最后一个字外其它字用*填充的姓名+空格+身份证后四位（官方字段路径 <c>fapiao_information.billing_person</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("billing_person")]
    public string? BillingPerson { get; set; }

    /// <summary>【发票类型】 商户提交申请开票的发票类型。可选取值：`COMM_FAPIAO` 增值税普通发票 / `VAT_FAPIAO` 增值税专用发票（官方字段路径 <c>fapiao_information.fapiao_bill_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fapiao_bill_type")]
    public string? FapiaoBillType { get; set; }

    /// <summary>【交易信息】 发票对应的交易信息，最大支持传入10条交易信息（官方字段路径 <c>fapiao_information.transaction_information</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("transaction_information")]
    public List<FapiaoRealEstateLeasingIssueRequestFapiaoInformationTransactionInformationItem>? TransactionInformation { get; set; }

    /// <summary>【发票备注】 发票备注（官方字段路径 <c>fapiao_information.remark</c>，选填，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

}

/// <summary>开具不动产租赁行业电子发票请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941740"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoRealEstateLeasingIssueRequest
{
    /// <summary>【子商户号】 微信支付分配的子商户号，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【发票申请单号】 发票申请单号，唯一标识一次开票行为，关联唯一的购买方信息。当微信支付账单开票时，为微信支付交易单号（官方字段路径 <c>fapiao_apply_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("fapiao_apply_id")]
    public string? FapiaoApplyId { get; set; }

    /// <summary>【购买方信息】 购买方信息，即发票抬头。若商户使用微信官方抬头，可从 【获取用户填写抬头信息】 接口获取用户填写的抬头；也可自行收集发票抬头（官方字段路径 <c>buyer_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("buyer_information")]
    public FapiaoRealEstateLeasingIssueRequestBuyerInformation? BuyerInformation { get; set; }

    /// <summary>【需要开具的数电发票信息】 需要开具的不动产租赁特定行业数电发票信息（官方字段路径 <c>fapiao_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fapiao_information")]
    public FapiaoRealEstateLeasingIssueRequestFapiaoInformation? FapiaoInformation { get; set; }

}
