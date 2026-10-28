// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingBusifavor;

/// <summary>商家券关联订单补贴请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465596"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponAssociateRequest
{
    /// <summary>【批次号】微信为每个商家券批次分配的唯一ID，对于商户自定义code的批次，关联请求必须填写批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【券code】券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【关联的商户订单号】微信支付下单时的商户订单号，欲与该商家券关联的微信支付（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【请求业务单据号】商户创建批次凭据号（格式：商户ID+日期+流水号），商户侧需保持唯一性，可包含英文字母，数字，｜，_，\*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

}

/// <summary>商家券关联订单补贴响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465596"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponAssociateResponse : WechatPayResponse
{
    /// <summary>【关联成功时间】系统关联券成功的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>wechatpay_associate_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("wechatpay_associate_time")]
    public string? WechatpayAssociateTime { get; set; }

}

/// <summary>商家券取消关联订单补贴请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465673"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponDisassociateRequest
{
    /// <summary>【批次号】微信为每个商家券批次分配的唯一ID， 对于商户自定义code的批次，关联请求必须填写批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【券code】券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【取消关联的商户订单号】欲取消与商家券关联的微信支付下单的商户订单号，该单号需要已操作过与券关联，否则无法取消关联（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【请求业务单据号】商户创建批次凭据号（格式：商户ID+日期+流水号），商户侧需保持唯一性，可包含英文字母，数字，｜，_，\*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

}

/// <summary>商家券取消关联订单补贴响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465673"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponDisassociateResponse : WechatPayResponse
{
    /// <summary>【取消关联券成功时间】系统成功取消商家券与订单信息关联关系的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>wechatpay_disassociate_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("wechatpay_disassociate_time")]
    public string? WechatpayDisassociateTime { get; set; }

}

/// <summary>作废商家券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465890"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponDeactivateRequest
{
    /// <summary>【券code】券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【批次号】券的所属批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【失效请求单据号】每次失效请求的唯一标识，商户需保证唯一（官方字段路径 <c>deactivate_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效原因】商户失效券的原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

}

/// <summary>作废商家券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465890"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponDeactivateResponse : WechatPayResponse
{
    /// <summary>【系统失效成功的时间】系统券成功失效的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>wechatpay_deactivate_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("wechatpay_deactivate_time")]
    public string? WechatpayDeactivateTime { get; set; }

}

/// <summary>商家券退款退券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465845"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponReturnRequest
{
    /// <summary>【券code】券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【批次号】券的所属批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【退券请求单据号】每次退券请求的唯一标识，商户需保证唯一（官方字段路径 <c>return_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

}

/// <summary>商家券退款退券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465845"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponReturnResponse : WechatPayResponse
{
    /// <summary>【系统退券成功的时间】微信退券成功的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>wechatpay_return_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("wechatpay_return_time")]
    public string? WechatpayReturnTime { get; set; }

}

/// <summary>核销商家券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465292"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponUseRequest
{
    /// <summary>【券code】券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【批次号】微信为每个商家券批次分配的唯一ID，当你在创建商家券接口中的coupon_code_mode参数传值为MERCHANT_API或者MERCHANT_UPLOAD时，则核销接口中该字段必传，否则该字段可不传（官方字段路径 <c>stock_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【公众账号ID】支持传入与当前调用接口商户号有绑定关系的AppID。支持小程序AppID与公众号AppID。核销接口返回的OpenID会在该传入AppID下进行计算获得。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID或传入的AppID得是归属商户号有绑定关系的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【请求核销时间】商户请求核销用户券的时间。 遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>use_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【核销请求单据号】每次核销请求的唯一标识，商户需保证唯一（官方字段路径 <c>use_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【用户标识】用户的唯一标识，做安全校验使用，非必填。 校验规则：传入的OpenID得是调用方商户号（即请求头里面的商户号）有绑定关系的AppID获取的OpenID或传入的OpenID得是归属商户号有绑定关系的AppID获取的OpenID。获取OpenID文档（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>核销商家券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465292"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponUseResponse : WechatPayResponse
{
    /// <summary>【批次号】批次ID（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【用户标识】用户在该公众号内的唯一身份标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【系统核销券成功的时间】系统成功核销券的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>wechatpay_use_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("wechatpay_use_time")]
    public string? WechatpayUseTime { get; set; }

}
