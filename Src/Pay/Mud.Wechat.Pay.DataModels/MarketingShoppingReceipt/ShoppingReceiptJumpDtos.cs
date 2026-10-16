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

/// <summary>上传商家电子小票跳转信息请求·【跳转信息】 账单详情页跳转商家电子小票的跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012443825"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptJumpInfoUploadRequestJumpInfo
{
    /// <summary>【商户电子小票跳转信息小程序AppID】 商户提供电子小票服务的小程序AppID（官方字段路径 <c>jump_info.merchant_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_appid")]
    public string? MerchantAppid { get; set; }

    /// <summary>【商户电子小票跳转信息小程序路径】 微信支付交易对应小票在商家小程序中的路径（官方字段路径 <c>jump_info.merchant_path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_path")]
    public string? MerchantPath { get; set; }

}

/// <summary>上传商家电子小票跳转信息请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012443825"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptJumpInfoUploadRequest
{
    /// <summary>【微信支付订单号】 微信支付订单的交易单号，上传的商家电子小票跳转信息会关联到该订单。用户可以在该笔微信支付订单的账单详情页，看到跳转入口（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户号】 微信支付订单的下单商户号。若该笔微信支付订单存在下单子商户号，则该字段可不填。订单中无下单子商户该字段必填（官方字段路径 <c>transaction_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_mchid")]
    public string? TransactionMchid { get; set; }

    /// <summary>【子商户号】 微信支付订单的下单子商户号。若该笔微信支付订单不存在下单子商户号，则该字段不填。订单中有下单商户号该字段必填（官方字段路径 <c>transaction_sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_sub_mchid")]
    public string? TransactionSubMchid { get; set; }

    /// <summary>【下单用户OpenID】 微信支付订单中OpenID。如果微信支付订单中有sub_OpenID，填写sub_OpenID内容（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户上传时间】 商户上传时间，用于标识请求的先后顺序。遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss.SSS+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，SSS表示毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.100+08:00表示，北京时间2015年5月20日…（官方字段路径 <c>merchant_upload_time</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("merchant_upload_time")]
    public string? MerchantUploadTime { get; set; }

    /// <summary>【跳转信息】 账单详情页跳转商家电子小票的跳转信息（官方字段路径 <c>jump_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_info")]
    public ShoppingReceiptJumpInfoUploadRequestJumpInfo? JumpInfo { get; set; }

}

/// <summary>上传商家电子小票跳转信息响应·【商家电子小票跳转信息】 商家电子小票跳转信息·【跳转信息】 账单详情页跳转商家电子小票的跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012443825"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptJumpInfoUploadResponseMerchantJumpInfoMerchantJumpInfojumpInfo
{
    /// <summary>【商户电子小票跳转信息小程序AppID】 商户提供电子小票服务的小程序AppID（官方字段路径 <c>merchant_jump_info.jump_info.merchant_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_appid")]
    public string? MerchantAppid { get; set; }

    /// <summary>【商户电子小票跳转信息小程序路径】 微信支付交易对应小票在商家小程序中的路径（官方字段路径 <c>merchant_jump_info.jump_info.merchant_path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_path")]
    public string? MerchantPath { get; set; }

}

/// <summary>上传商家电子小票跳转信息响应·【商家电子小票跳转信息】 商家电子小票跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012443825"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptJumpInfoUploadResponseMerchantJumpInfo
{
    /// <summary>【微信支付订单号】 微信支付订单的交易单号，上传的商家电子小票跳转信息会关联到该订单。用户可以在该笔微信支付订单的账单详情页，看到跳转入口（官方字段路径 <c>merchant_jump_info.transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户号】 微信支付订单的下单商户号。若该笔微信支付订单存在下单子商户号，则该字段可不填。订单中无下单子商户该字段必填（官方字段路径 <c>merchant_jump_info.transaction_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_mchid")]
    public string? TransactionMchid { get; set; }

    /// <summary>【子商户号】 微信支付订单的下单子商户号。若该笔微信支付订单不存在下单子商户号，则该字段不填。订单中有下单商户号该字段必填（官方字段路径 <c>merchant_jump_info.transaction_sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_sub_mchid")]
    public string? TransactionSubMchid { get; set; }

    /// <summary>【下单用户OpenID】 微信支付订单中OpenID。如果微信支付订单中有sub_OpenID，填写sub_OpenID内容（官方字段路径 <c>merchant_jump_info.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户上传时间】 商户上传时间，用于标识请求的先后顺序。遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss.SSS+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，SSS表示毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.100+08:00表示，北京时间2015年5月20日…（官方字段路径 <c>merchant_jump_info.merchant_upload_time</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("merchant_upload_time")]
    public string? MerchantUploadTime { get; set; }

    /// <summary>【跳转信息】 账单详情页跳转商家电子小票的跳转信息（官方字段路径 <c>merchant_jump_info.jump_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_info")]
    public ShoppingReceiptJumpInfoUploadResponseMerchantJumpInfoMerchantJumpInfojumpInfo? JumpInfo { get; set; }

    /// <summary>【创建时间】 商家电子小票跳转信息创建的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>merchant_jump_info.create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【修改时间】 商家电子小票跳转信息最后一次修改时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>merchant_jump_info.modify_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>【归属品牌ID】 归属品牌ID（官方字段路径 <c>merchant_jump_info.brand_id</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

}

/// <summary>上传商家电子小票跳转信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012443825"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingShoppingReceipt")]
public class ShoppingReceiptJumpInfoUploadResponse : WechatPayResponse
{
    /// <summary>【商家电子小票跳转信息】 商家电子小票跳转信息（官方字段路径 <c>merchant_jump_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("merchant_jump_info")]
    public ShoppingReceiptJumpInfoUploadResponseMerchantJumpInfo? MerchantJumpInfo { get; set; }

}
