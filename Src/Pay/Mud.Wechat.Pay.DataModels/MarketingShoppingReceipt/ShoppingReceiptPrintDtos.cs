// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingShoppingReceipt;

/// <summary>发送购物小票响应·【电子小票上传信息】·【联系商家】 用户与商家的联系渠道。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698614"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptReceiptPrintResponseReceiptReceiptmerchantContactInformation
{
    /// <summary>【商户售后咨询电话】 商户售后咨询电话。该字段已做加密处理，具体解密方法详见 如何使用API证书解密敏感字段（官方字段路径 <c>receipt.merchant_contact_information.consultation_phone_number</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("consultation_phone_number")]
    public string? ConsultationPhoneNumber { get; set; }

}

/// <summary>发送购物小票响应·【电子小票上传信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698614"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptReceiptPrintResponseReceipt
{
    /// <summary>【电子小票ID】 电子小票ID（官方字段路径 <c>receipt.receipt_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("receipt_id")]
    public string? ReceiptId { get; set; }

    /// <summary>【电子小票图片审核状态】 电子小票图片审核状态。微信支付会对商户上传的电子小票图片内容进行安全审核。审核结束后会根据结果修改电子小票图片审核状态。处于审核不通过状态的电子小票，用户查看电子小票时将使用默认图代替，其他状态正常展示（官方字段路径 <c>receipt.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【微信支付订单号】 微信支付订单的交易单号，上传的电子小票会关联到该订单。用户可以在该笔微信支付订单的账单详情页，看到上传的电子小票（官方字段路径 <c>receipt.transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户号】 微信支付订单的商户号（官方字段路径 <c>receipt.transaction_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_mchid")]
    public string? TransactionMchid { get; set; }

    /// <summary>【子商户号】 微信支付订单的子商户号（官方字段路径 <c>receipt.transaction_sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_sub_mchid")]
    public string? TransactionSubMchid { get; set; }

    /// <summary>【电子小票归属的OpenID】 微信支付订单中OpenID（官方字段路径 <c>receipt.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【电子小票归属品牌ID】 电子小票归属品牌的ID（官方字段路径 <c>receipt.brand_id</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>【电子小票图片文件摘要】 图片文件的文件摘要，即对图片文件的二进制内容进行sha256计算得到的值（官方字段路径 <c>receipt.sha256</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }

    /// <summary>【电子小票图片类型】 标识电子小票图片类型（官方字段路径 <c>receipt.image_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_type")]
    public string? ImageType { get; set; }

    /// <summary>【创建时间】 电子小票创建的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>receipt.create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【修改时间】 电子小票最后一次修改时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>receipt.modify_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>【联系商家】 用户与商家的联系渠道（官方字段路径 <c>receipt.merchant_contact_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("merchant_contact_information")]
    public ShoppingReceiptReceiptPrintResponseReceiptReceiptmerchantContactInformation? MerchantContactInformation { get; set; }

    /// <summary>【上传时间】 上传时间，用于标识请求的先后顺序，该笔小票上传时填写则返回，没有不返回。遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss.SSS+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，SSS表示毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.100+08:00表示…（官方字段路径 <c>receipt.upload_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("upload_time")]
    public string? UploadTime { get; set; }

}

/// <summary>发送购物小票响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698614"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptReceiptPrintResponse : WechatPayResponse
{
    /// <summary>【电子小票上传信息】（官方字段路径 <c>receipt</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receipt")]
    public ShoppingReceiptReceiptPrintResponseReceipt? Receipt { get; set; }

}
