// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandProfitSharing;

/// <summary>品牌最大分账比例响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467022"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingMaxRatioResponse : WechatPayResponse
{
    /// <summary>【品牌商户号】 微信支付分配的商户号，分账出资的品牌商户号（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【最大分账比例】 品牌商户允许父商户分账的最大比例，单位万分比，比如2000表示20%（官方字段路径 <c>max_ratio</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_ratio")]
    public long? MaxRatio { get; set; }

}

/// <summary>品牌分账解冻剩余资金请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467016"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingFinishRequest
{
    /// <summary>【特约商户号】 订单收款方商户号，填写微信支付分配的商户号，可以是品牌主商户号，也可以是门店商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户分账单号】 商户系统内部的分账单号，在商户系统内部唯一（单次分账、多次分账、完结分账应使用不同的商户分账单号），同一分账单号多次请求等同一次。只能是数字、大小写字母_-&amp;#124;\*@（官方字段路径 <c>out_order_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【解冻原因描述】 解冻剩余资金的原因描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}

/// <summary>品牌分账解冻剩余资金响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467016"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingFinishResponse : WechatPayResponse
{
    /// <summary>【特约商户号】 订单收款方商户号，填写微信支付分配的商户号，可以是品牌主商户号，也可以是门店商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户分账单号】 商户系统内部的分账单号，在商户系统内部唯一（单次分账、多次分账、完结分账应使用不同的商户分账单号），同一分账单号多次请求等同一次，只能是数字、大小写字母_-&amp;#124;\*@（官方字段路径 <c>out_order_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【微信分账单号】 微信分账单号，微信系统返回的唯一标识（官方字段路径 <c>order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

}

/// <summary>品牌分账请求请求·【分账接收方列表】 分账接收方列表，可以设置出资商户作为分账接受方，最多可有50。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692975"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingOrderRequestReceiversItem
{
    /// <summary>【接收方类型】 MERCHANT_ID：商户号 PERSONAL_OPENID：个人OpenID（用户在商户appid下的唯一标识，详见 OpenID获取） PERSONAL_SUB_OPENID：个人OpenID（用户在品牌主appid下的唯一标识，详见 OpenID获取）（官方字段路径 <c>receivers.type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【接收方账号】 分账接收方类型为MERCHANT_ID时，分账接收方账号为商户号（mch_id或者sub_mch_id） 分账接收方类型为PERSONAL_OPENID时，分账接收方账号为个人OpenID（由服务商的AppID转换得到） 分账接收方类型为PERSONAL_SUB_OPENID时，分账接收方账号为个人OpenID（由品牌主的AppID转换得到）（官方字段路径 <c>receivers.account</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>【分账金额】 分账金额，单位为分，只能为整数，不能超过原订单支付金额及最大分账比例金额。调整最大分账比例：品牌主登录商户平台，进入以下页面：品牌专区-品牌交易-品牌供应链分账-供应链分账管理（官方字段路径 <c>receivers.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【分账描述】 分账的原因描述，会在查询分账结果接口和分账账单中原样返回（官方字段路径 <c>receivers.description</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【分账个人接收方姓名】 \\* 分账接收方类型是MERCHANT_ID时，是商户全称（必传），当商户是小微商户或个体户时，是开户人姓名 分账接收方类型是PERSONAL_OPENID时，是个人姓名（选传，传则会检查与 name 是否实名匹配，不匹配会拒绝分账请求） 分账接收方类型是PERSONAL_SUB_OPENID时，是个人姓名（选传，传则会检查与 name 是否实名匹配，不匹配会拒绝分账请求） 此字段需要使用微信支付公钥加密（推荐），请参考 获取微信…（官方字段路径 <c>receivers.name</c>，选填，类型 <c>string(10240)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

}

/// <summary>品牌分账请求请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692975"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingOrderRequest
{
    /// <summary>【品牌主商户号】 品牌主商户号，填写微信支付分配的商户号。详情见： 品牌-门店关系管理（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【特约商户号】 订单收款方商户号，填写微信支付分配的商户号，可以是品牌主商户号，也可以是门店商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 APPID是微信开放平台(移动应用)或微信公众平台(小程序、公众号)为开发者的应用程序提供的唯一标识。此处，可以填写这三种类型中的任意一种APPID，但请确保该appid与mchid有绑定关系。详见： 开发必要参数说明。分账接收方类型包含PERSONAL_OPENID时必填（官方字段路径 <c>appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【品牌主公众账号ID】 APPID是微信开放平台(移动应用)或微信公众平台(小程序、公众号)为开发者的应用程序提供的唯一标识。此处，可以填写这三种类型中的任意一种APPID，但请确保该sub_appid与sub_mchid有绑定关系。详见： 开发必要参数说明。分账接收方类型包含PERSONAL_SUB_OPENID时必填（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【微信订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户分账单号】 商户系统内部的分账单号，在商户系统内部唯一（单次分账、多次分账、完结分账应使用不同的商户分账单号），同一分账单号多次请求等同一次。只能是数字、大小写字母_-&amp;#124;\*@（官方字段路径 <c>out_order_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【分账接收方列表】 分账接收方列表，可以设置出资商户作为分账接受方，最多可有50个分账接收方（官方字段路径 <c>receivers</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("receivers")]
    public List<BrandProfitSharingOrderRequestReceiversItem>? Receivers { get; set; }

    /// <summary>【是否分账完成】 如果为true，则该笔订单剩余未分账的金额会解冻回分账方商户 如果为false，则该笔订单剩余未分账的金额不会解冻回分账方商户，可以对该笔订单再次进行分账（官方字段路径 <c>finish</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("finish")]
    public bool? Finish { get; set; }

}

/// <summary>品牌分账请求响应·【分账接收方列表】 分账接收方列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692975"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingOrderResponseReceiversItem
{
    /// <summary>【接收方类型】 MERCHANT_ID：商户号 PERSONAL_OPENID：个人OpenID（用户在商户appid下的唯一标识，详见 OpenID获取） PERSONAL_SUB_OPENID：个人OpenID（用户在品牌主appid下的唯一标识，详见 OpenID获取）（官方字段路径 <c>receivers.type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【接收方账号】 分账接收方类型为MERCHANT_ID时，分账接收方账号为商户号（mch_id或者sub_mch_id） 分账接收方类型为PERSONAL_OPENID时，分账接收方账号为个人OpenID（由服务商的AppID转换得到） 分账接收方类型为PERSONAL_SUB_OPENID时，分账接收方账号为个人OpenID（由品牌主的AppID转换得到）（官方字段路径 <c>receivers.account</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>【分账金额】 分账金额，单位为分，只能为整数（官方字段路径 <c>receivers.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【分账描述】 分账的原因描述，会在查询分账结果接口和分账账单中原样返回（官方字段路径 <c>receivers.description</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【分账结果】 PENDING: 待分账，非终态 SUCCESS: 分账成功，终态 CLOSED: 已关闭，终态（官方字段路径 <c>receivers.result</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>【分账完成时间】 分账完成时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>receivers.finish_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>【分账失败原因】 分账失败原因，当分账结果result为CLOSED（已关闭）时，返回该字段，具体处理方案请参考 分账失败处理指引。可选取值：`ACCOUNT_ABNORMAL` 分账接收账户异常 / `NO_RELATION` 分账关系已解除 / `RECEIVER_HIGH_RISK` 高风险接收方 / `RECEIVER_REAL_NAME_NOT_VERIFIED` 接收方未实名 / `NO_AUTH` 分账权限已解除 / `RECEIVER_RECEIPT_LIMIT` 超出用户月收款限额 / `PAYER_ACCOUNT_ABNORMAL` 分出方账户异常 / `INVALID_REQUEST` 描述参数设置失败（官方字段路径 <c>receivers.fail_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【分账明细单号】 微信分账明细单号，每笔分账业务执行的明细单号，可与资金账单对账使用，对应资金账单中的“业务凭证号”（官方字段路径 <c>receivers.detail_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

}

/// <summary>品牌分账请求响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692975"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingOrderResponse : WechatPayResponse
{
    /// <summary>【品牌主商户号】 品牌主商户号，微信支付分配的商户号（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【特约商户号】 订单收款方商户号，填写微信支付分配的商户号，可以是品牌主商户号，也可以是门店商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户分账单号】 商户系统内部的分账单号，在商户系统内部唯一（单次分账、多次分账、完结分账应使用不同的商户分账单号），同一分账单号多次请求等同一次，只能是数字、大小写字母_-&amp;#124;\*@（官方字段路径 <c>out_order_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【微信分账单号】 微信分账单号，微信系统返回的唯一标识（官方字段路径 <c>order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【分账接收方列表】 分账接收方列表（官方字段路径 <c>receivers</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("receivers")]
    public List<BrandProfitSharingOrderResponseReceiversItem>? Receivers { get; set; }

    /// <summary>【分账单状态】 分账单状态（每个接收方的分账结果请查看receivers中的result字段）。可选取值：`PROCESSING` 非终态，分账结果存在非终态，可稍后再次查询，直到状态为FINISHED。 / `FINISHED` 终态，仅代表分账动账执行完毕，每个接收方的分账结果请查看receivers中的result字段（官方字段路径 <c>status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

}

/// <summary>品牌分账结果查询响应·【分账接收方列表】 分账接收方列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467002"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingOrderListResponseReceiversItem
{
    /// <summary>【接收方类型】 MERCHANT_ID：商户号 PERSONAL_OPENID：个人OpenID（用户在商户appid下的唯一标识，详见 OpenID获取） PERSONAL_SUB_OPENID：个人OpenID（用户在品牌主appid下的唯一标识，详见 OpenID获取）（官方字段路径 <c>receivers.type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【接收方账号】 分账接收方类型为MERCHANT_ID时，分账接收方账号为商户号（mch_id或者sub_mch_id） 分账接收方类型为PERSONAL_OPENID时，分账接收方账号为个人OpenID（由服务商的AppID转换得到） 分账接收方类型为PERSONAL_SUB_OPENID时，分账接收方账号为个人OpenID（由品牌主的AppID转换得到）（官方字段路径 <c>receivers.account</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>【分账金额】 分账金额，单位为分，只能为整数（官方字段路径 <c>receivers.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【分账描述】 分账的原因描述，会在查询分账结果接口和分账账单中原样返回（官方字段路径 <c>receivers.description</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【分账结果】 PENDING: 待分账，非终态 SUCCESS: 分账成功，终态 CLOSED: 已关闭，终态（官方字段路径 <c>receivers.result</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>【分账完成时间】 分账完成时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>receivers.finish_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>【分账失败原因】 分账失败原因，当分账结果result为CLOSED（已关闭）时，返回该字段，具体处理方案请参考 分账失败处理指引。可选取值：`ACCOUNT_ABNORMAL` 分账接收账户异常 / `NO_RELATION` 分账关系已解除 / `RECEIVER_HIGH_RISK` 高风险接收方 / `RECEIVER_REAL_NAME_NOT_VERIFIED` 接收方未实名 / `NO_AUTH` 分账权限已解除 / `RECEIVER_RECEIPT_LIMIT` 超出用户月收款限额 / `PAYER_ACCOUNT_ABNORMAL` 分出方账户异常 / `INVALID_REQUEST` 描述参数设置失败（官方字段路径 <c>receivers.fail_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【分账明细单号】 微信分账明细单号，每笔分账业务执行的明细单号，可与资金账单对账使用，对应资金账单中的“业务凭证号”（官方字段路径 <c>receivers.detail_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

}

/// <summary>品牌分账结果查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467002"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingOrderListResponse : WechatPayResponse
{
    /// <summary>【特约商户号】 订单收款方商户号，填写微信支付分配的商户号，可以是品牌主商户号，也可以是门店商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户分账单号】 请求分账时的商户分账单号（官方字段路径 <c>out_order_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【微信分账单号】 微信分账单号，微信系统返回的唯一标识（官方字段路径 <c>order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【分账单状态】。可选取值：`PROCESSING` 处理中，非终态，分账结果存在非终态，可稍后再次查询，直到状态为FINISHED。 / `FINISHED` 分账完成，终态，仅代表分账动账执行完毕，每个接收方的分账结果请查看receivers中的result字段（官方字段路径 <c>status</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【分账接收方列表】 分账接收方列表（官方字段路径 <c>receivers</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("receivers")]
    public List<BrandProfitSharingOrderListResponseReceiversItem>? Receivers { get; set; }

    /// <summary>【分账完结金额】 分账完结的分账金额，单位为分， 仅当查询分账完结的执行结果时，存在本字段（官方字段路径 <c>finish_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("finish_amount")]
    public long? FinishAmount { get; set; }

    /// <summary>【分账完结描述】 分账完结的原因描述，仅当查询分账完结的执行结果时，存在本字段（官方字段路径 <c>finish_description</c>，选填，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("finish_description")]
    public string? FinishDescription { get; set; }

}

/// <summary>订单剩余待分金额响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467021"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingAmountsResponse : WechatPayResponse
{
    /// <summary>【微信订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【订单剩余待分金额】 订单剩余待分金额，整数，单元为分（官方字段路径 <c>unsplit_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("unsplit_amount")]
    public long? UnsplitAmount { get; set; }

}

/// <summary>品牌分账接收方添加请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467100"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingReceiverAddRequest
{
    /// <summary>【品牌主商户号】 品牌主商户号即绑定具体品牌的商户号，填写微信支付分配的商户号（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【公众账号ID】 APPID是微信开放平台(移动应用)或微信公众平台(小程序、公众号)为开发者的应用程序提供的唯一标识。此处，可以填写这三种类型中的任意一种APPID，但请确保该appid与mchid有绑定关系。详见： 开发必要参数说明（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【品牌主公众账号ID】 APPID是微信开放平台(移动应用)或微信公众平台(小程序、公众号)为开发者的应用程序提供的唯一标识。此处，可以填写这三种类型中的任意一种APPID，但请确保该sub_appid与brand_mchid有绑定关系。详见： 开发必要参数说明。分账接收方类型包含PERSONAL_SUB_OPENID时必填（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【接收方类型】。可选取值：`PERSONAL_OPENID：个人OpenID（用户在商户appid下的唯一标识，详见 [OpenID获取](https` //pay.weixin.qq.com/doc/v3/partner/4012081935.md)） / `PERSONAL_SUB_OPENID：个人OpenID（用户在品牌主appid下的唯一标识，详见 [OpenID获取](https` //pay.weixin.qq.com/doc/v3/partner/4012081935.md)）（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【接收方账号】 分账接收方类型为MERCHANT_ID时，分账接收方账号为商户号（mch_id或者sub_mch_id） 分账接收方类型为PERSONAL_OPENID时，分账接收方账号为个人OpenID（由服务商的AppID转换得到） 分账接收方类型为PERSONAL_SUB_OPENID时，分账接收方账号为个人OpenID（由品牌主的AppID转换得到）（官方字段路径 <c>account</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>【接收方名称】 分账接收方的名称。 当type为MERCHANT_ID时，接收方名称必传，且为商户全称。 当type为PERSONAL_OPENID时，接收方名称选传，且为个人姓名。（传入该字段时需要进行校验。） 当type为PERSONAL_SUB_OPENID时，接收方名称选传，且为个人姓名。（传入该字段时需要进行校验。）（官方字段路径 <c>name</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【与品牌主的关系类型】（官方字段路径 <c>relation_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("relation_type")]
    public string? RelationType { get; set; }

}

/// <summary>品牌分账接收方添加响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467100"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingReceiverResponse : WechatPayResponse
{
    /// <summary>【品牌主商户号】 品牌主商户号，即绑定具体品牌的商户号，填写微信支付分配的商户号（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【接收方类型】。可选取值：`PERSONAL_OPENID：个人OpenID（用户在商户appid下的唯一标识，详见 [OpenID获取](https` //pay.weixin.qq.com/doc/v3/partner/4012081935.md)） / `PERSONAL_SUB_OPENID：个人OpenID（用户在品牌主appid下的唯一标识，详见 [OpenID获取](https` //pay.weixin.qq.com/doc/v3/partner/4012081935.md)）（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【接收方账号】 分账接收方类型为MERCHANT_ID时，分账接收方账号为商户号（mch_id或者sub_mch_id） 分账接收方类型为PERSONAL_OPENID时，分账接收方账号为个人OpenID（由服务商的AppID转换得到） 分账接收方类型为PERSONAL_SUB_OPENID时，分账接收方账号为个人OpenID（由品牌主的AppID转换得到）（官方字段路径 <c>account</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

}

/// <summary>品牌分账接收方删除请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467103"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingReceiverDeleteRequest
{
    /// <summary>【品牌主商户号】 品牌主商户号，即绑定具体品牌的商户号，填写微信支付分配的商户号（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【公众账号ID】 APPID是微信开放平台(移动应用)或微信公众平台(小程序、公众号)为开发者的应用程序提供的唯一标识。此处，可以填写这三种类型中的任意一种APPID，但请确保该appid与mchid有绑定关系。详见： 开发必要参数说明（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【品牌主公众账号ID】 APPID是微信开放平台(移动应用)或微信公众平台(小程序、公众号)为开发者的应用程序提供的唯一标识。此处，可以填写这三种类型中的任意一种APPID，但请确保该sub_appid与brand_mchid有绑定关系。详见： 开发必要参数说明。分账接收方类型包含PERSONAL_SUB_OPENID时必填（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【接收方类型】。可选取值：`PERSONAL_OPENID：个人OpenID（用户在商户appid下的唯一标识，详见 [OpenID获取](https` //pay.weixin.qq.com/doc/v3/partner/4012081935.md)） / `PERSONAL_SUB_OPENID：个人OpenID（用户在品牌主appid下的唯一标识，详见 [OpenID获取](https` //pay.weixin.qq.com/doc/v3/partner/4012081935.md)）（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【接收方账号】 分账接收方类型为MERCHANT_ID时，分账接收方账号为商户号（mch_id或者sub_mch_id） 分账接收方类型为PERSONAL_OPENID时，分账接收方账号为个人OpenID（由服务商的AppID转换得到） 分账接收方类型为PERSONAL_SUB_OPENID时，分账接收方账号为个人OpenID（由品牌主的AppID转换得到）（官方字段路径 <c>account</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

}

/// <summary>品牌分账回退请求请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467097"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingReturnOrderRequest
{
    /// <summary>【特约商户号】 微信支付分配的特约商户号，分账回退的接收商户，对应原分账出资的分账方商户（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信分账单号】 微信分账单号，微信系统返回的唯一标识。微信分账单号和商户分账单号二选一填写（官方字段路径 <c>order_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户分账单号】 商户系统内部的分账单号，在商户系统内部唯一（单次分账、多次分账、完结分账应使用不同的商户分账单号），同一分账单号多次请求等同一次。微信分账单号和商户分账单号二选一填写，只能是数字、大小写字母_-&amp;#124;\*@（官方字段路径 <c>out_order_no</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户回退单号】 此回退单号是商户在自己后台生成的一个新的回退单号，在商户后台唯一。只能是数字、大小写字母_-&amp;#124;\*@：\[0-9a-zA-Z_\*@-\]（官方字段路径 <c>out_return_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>【回退商户号】 对应请求分账接口时传入的receiver_account：分账接收方账号（官方字段路径 <c>return_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("return_mchid")]
    public string? ReturnMchid { get; set; }

    /// <summary>【回退金额】 需要从分账接收方回退的金额，单位为分，只能为整数，不能超过原始分账单分出给该接收方的金额（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【回退描述】 分账回退原因描述，会在查询分账回退结果接口和分账账单中原样返回（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(80)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}

/// <summary>品牌分账回退请求响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467097"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandProfitSharing")]
public class BrandProfitSharingReturnOrderResponse : WechatPayResponse
{
    /// <summary>【特约商户号】 微信支付分配的特约商户号，分账回退的接收商户，对应原分账出资的分账方商户（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信分账单号】 微信分账单号，微信系统返回的唯一标识（官方字段路径 <c>order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户分账单号】 商户系统内部的分账单号，在商户系统内部唯一（单次分账、多次分账、完结分账应使用不同的商户分账单号），同一分账单号多次请求等同一次。只能是数字、大小写字母_-&amp;#124;\*@（官方字段路径 <c>out_order_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户回退单号】 此回退单号是商户在自己后台生成的一个新的回退单号，在商户后台唯一。只能是数字、大小写字母_-&amp;#124;\*@：\[0-9a-zA-Z_\*@-\]（官方字段路径 <c>out_return_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>【回退商户号】 对应请求分账接口时传入的receiver_account：分账接收方账号（官方字段路径 <c>return_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("return_mchid")]
    public string? ReturnMchid { get; set; }

    /// <summary>【回退金额】 需要从分账接收方回退的金额，单位为分，只能为整数，不能超过原始分账单分出给该接收方的金额（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【微信回退单号】 微信分账回退单号，微信系统返回的唯一标识（官方字段路径 <c>return_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("return_no")]
    public string? ReturnNo { get; set; }

    /// <summary>【回退结果】 如果请求返回为处理中，则商户可以通过调用回退结果查询接口获取请求的最终处理结果。如果返回为处理中，请勿变更商户回退单号，使用相同的参数再次发起分账回退，否则会出现资金风险。在处理中状态的回退单如果5天没有成功，会因为超时被设置为已失败（官方字段路径 <c>result</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>【失败原因】 在回退结果为FAILED（已失败）时返回。可选取值：`PAYER_ACCOUNT_ABNORMAL` 原分账分出方账户异常（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【完成时间】 分账回退完成时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>finish_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

}
