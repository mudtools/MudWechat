// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceClaim;

/// <summary>请求保险理赔请求·【收款方信息】 收款方信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCreateRequestReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保险理赔请求·【收款方信息】 收款方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCreateRequestReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户 / `TRANSACTION_USER` 微信支付订单对应的用户（官方字段路径 <c>receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER时，需要设置（官方字段路径 <c>receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public InsuranceClaimCreateRequestReceiverTransactionInfo? TransactionInfo { get; set; }

}

/// <summary>请求保险理赔请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCreateRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，若指定二级商户号，则该商户为出资商户，若不指定二级商户号，必须指定出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【收款方信息】 收款方信息（官方字段路径 <c>receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public InsuranceClaimCreateRequestReceiver? Receiver { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

}

/// <summary>请求保险理赔响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCreateResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保险理赔响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCreateResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户 / `TRANSACTION_USER` 微信支付订单对应的用户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER时，需要设置（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public InsuranceClaimCreateResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

}

/// <summary>请求保险理赔响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCreateResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public InsuranceClaimCreateResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>请求保险理赔响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCreateResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public InsuranceClaimCreateResponseReceiverDetail? ReceiverDetail { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【赔付金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【微信支付转账单号】 微信支付转账单号（官方字段路径 <c>bill_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>【转账状态】 转账状态。可选取值：`ACCEPTED` 请求已受理，处理结果尚未明确，如一直处于此状态，建议检查账户余额是否足够 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【受理时间】 受理成功时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>accept_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>【成功时间】 转账成功时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【出资商户号】 微信支付分配的商户号（官方字段路径 <c>sponsor_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchid { get; set; }

}

/// <summary>请求保险理赔预下单请求·【收款方信息】 收款方信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504185"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimPreTransferRequestReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保险理赔预下单请求·【收款方信息】 收款方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504185"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimPreTransferRequestReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户 / `TRANSACTION_USER` 微信支付订单对应的用户（官方字段路径 <c>receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER时，需要设置（官方字段路径 <c>receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public InsuranceClaimPreTransferRequestReceiverTransactionInfo? TransactionInfo { get; set; }

}

/// <summary>请求保险理赔预下单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504185"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimPreTransferRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，若指定二级商户号，则该商户为出资商户，若不指定二级商户号，必须指定出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【收款方信息】 收款方信息（官方字段路径 <c>receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public InsuranceClaimPreTransferRequestReceiver? Receiver { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【商户回调地址】 异步接收微信支付转账结果通知的回调地址，通知URL必须为公网可访问的URL，必须为HTTPS，不能携带参数（官方字段路径 <c>notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>请求保险理赔预下单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504185"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimPreTransferResponse : WechatPayResponse
{
    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户单号】 商户系统内部的商家单号，只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【微信支付付款单号】 微信支付付款单号（官方字段路径 <c>bill_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>【受理时间】 受理成功时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>accept_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>【跳转领取页面的package信息】 商家付款单跳转微信支付收款页的package信息，该数据在24小时内有效；只有在待用户确认中才返回（官方字段路径 <c>package_info</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }

}

/// <summary>请求撤销保险理赔请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504186"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCancelRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>请求撤销保险理赔响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504186"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimCancelResponse : WechatPayResponse
{
    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【微信支付付款单号】 微信支付付款单号（官方字段路径 <c>bill_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>【付款状态】 付款状态。可选取值：`ACCEPTED` 请求已受理，处理结果尚未明确，如一直处于此状态，建议检查账户余额是否足够 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【撤销受理时间】 撤销受理成功时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>cancel_accept_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("cancel_accept_time")]
    public string? CancelAcceptTime { get; set; }

}

/// <summary>商户单号查询保险理赔结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimQueryResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>商户单号查询保险理赔结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimQueryResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户 / `TRANSACTION_USER` 微信支付订单对应的用户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER时，需要设置（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public InsuranceClaimQueryResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

}

/// <summary>商户单号查询保险理赔结果响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimQueryResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public InsuranceClaimQueryResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>商户单号查询保险理赔结果响应·【关单信息】 当状态为已关闭时返回，转账的关闭信息，如关闭原因等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimQueryResponseCloseInfo
{
    /// <summary>【关单时间】 单据关闭时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【关单原因】 当状态为已关闭时返回，关闭信息，如关闭原因等（官方字段路径 <c>close_info.close_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>商户单号查询保险理赔结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceInsuranceClaim")]
public class InsuranceClaimQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public InsuranceClaimQueryResponseReceiverDetail? ReceiverDetail { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【赔付金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【微信支付转账单号】 微信支付转账单号（官方字段路径 <c>bill_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>【转账状态】 转账状态。可选取值：`ACCEPTED` 请求已受理，处理结果尚未明确，如一直处于此状态，建议检查账户余额是否足够 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【受理时间】 受理成功时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>accept_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>【成功时间】 转账成功时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【关单信息】 当状态为已关闭时返回，转账的关闭信息，如关闭原因等（官方字段路径 <c>close_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("close_info")]
    public InsuranceClaimQueryResponseCloseInfo? CloseInfo { get; set; }

    /// <summary>【出资商户号】 微信支付分配的商户号（官方字段路径 <c>sponsor_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchid { get; set; }

}
