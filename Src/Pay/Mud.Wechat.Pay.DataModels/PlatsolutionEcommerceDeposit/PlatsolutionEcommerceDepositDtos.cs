// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit;

/// <summary>请求保证金售后赔付请求·【收款方信息】 收款方信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateRequestReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保证金售后赔付请求·【收款方信息】 收款方信息·【转账接收方用户】 当接收方类型为WXPAY_USER时，需要设置。当接收方类型。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateRequestReceiverUserInfo
{
    /// <summary>【转账接收方用户 openid】 在商户sp_appid下的唯一用户标识（官方字段路径 <c>receiver.user_info.sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

}

/// <summary>请求保证金售后赔付请求·【收款方信息】 收款方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateRequestReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户 / `TRANSACTION_USER` 微信支付订单对应的用户 / `WXPAY_USER` 微信支付用户（官方字段路径 <c>receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER或WXPAY_USER时，需要设置（官方字段路径 <c>receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositAfterSalesCreateRequestReceiverTransactionInfo? TransactionInfo { get; set; }

    /// <summary>【转账接收方用户】 当接收方类型为WXPAY_USER时，需要设置。当接收方类型为TRANSACTION_USER，请勿设置（官方字段路径 <c>receiver.user_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_info")]
    public DepositAfterSalesCreateRequestReceiverUserInfo? UserInfo { get; set; }

}

/// <summary>请求保证金售后赔付请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，若指定二级商户号，则该商户为出资商户，若不指定二级商户号，必须指定出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【收款方信息】 收款方信息（官方字段路径 <c>receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositAfterSalesCreateRequestReceiver? Receiver { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【赔付金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【转账场景ID】 该笔转账使用的转账场景，可前往“商户平台-产品中心-商家转账”中申请或联系微信支付运营人员申请（官方字段路径 <c>transfer_scene_id</c>，选填，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【用户收款感知】 用户收款时感知到的收款原因将根据转账场景自动展示默认内容。如有其他展示需求，可在本字段传入。各场景展示的默认内容和支持传入的内容，可查看产品文档或联系微信支付运营人员了解。注：若指定该字段，必须同时指定转账场景ID，建议所有请求都传入转账场景ID（官方字段路径 <c>user_recv_perception</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

}

/// <summary>请求保证金售后赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保证金售后赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方用户】 当接收方类型为WXPAY_USER时，需要设置。当接收方类型。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateResponseReceiverDetailReceiverUserInfo
{
    /// <summary>【转账接收方用户 openid】 在商户sp_appid下的唯一用户标识（官方字段路径 <c>receiver_detail.receiver.user_info.sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

}

/// <summary>请求保证金售后赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户 / `TRANSACTION_USER` 微信支付订单对应的用户 / `WXPAY_USER` 微信支付用户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER或WXPAY_USER时，需要设置（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositAfterSalesCreateResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

    /// <summary>【转账接收方用户】 当接收方类型为WXPAY_USER时，需要设置。当接收方类型为TRANSACTION_USER，请勿设置（官方字段路径 <c>receiver_detail.receiver.user_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_info")]
    public DepositAfterSalesCreateResponseReceiverDetailReceiverUserInfo? UserInfo { get; set; }

}

/// <summary>请求保证金售后赔付响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositAfterSalesCreateResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>请求保证金售后赔付响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesCreateResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public DepositAfterSalesCreateResponseReceiverDetail? ReceiverDetail { get; set; }

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

    /// <summary>【转账状态】 转账状态。可选取值：`ACCEPTED` 已受理 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
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

/// <summary>商户单号查询保证金售后赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesQueryResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>商户单号查询保证金售后赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方用户】 当接收方类型为WXPAY_USER时，需要设置。当接收方类型。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesQueryResponseReceiverDetailReceiverUserInfo
{
    /// <summary>【转账接收方用户 openid】 在商户sp_appid下的唯一用户标识（官方字段路径 <c>receiver_detail.receiver.user_info.sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

}

/// <summary>商户单号查询保证金售后赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesQueryResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户 / `TRANSACTION_USER` 微信支付订单对应的用户 / `WXPAY_USER` 微信支付用户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER或WXPAY_USER时，需要设置（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositAfterSalesQueryResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

    /// <summary>【转账接收方用户】 当接收方类型为WXPAY_USER时，需要设置。当接收方类型为TRANSACTION_USER，请勿设置（官方字段路径 <c>receiver_detail.receiver.user_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_info")]
    public DepositAfterSalesQueryResponseReceiverDetailReceiverUserInfo? UserInfo { get; set; }

}

/// <summary>商户单号查询保证金售后赔付结果响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesQueryResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositAfterSalesQueryResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>商户单号查询保证金售后赔付结果响应·【关单信息】 当状态为已关闭时返回，转账的关闭信息，如关闭原因等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesQueryResponseCloseInfo
{
    /// <summary>【关单时间】 单据关闭时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【关单原因】 当状态为已关闭时返回，关闭信息，如关闭原因等（官方字段路径 <c>close_info.close_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>商户单号查询保证金售后赔付结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositAfterSalesQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public DepositAfterSalesQueryResponseReceiverDetail? ReceiverDetail { get; set; }

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

    /// <summary>【转账状态】 转账状态。可选取值：`ACCEPTED` 已受理 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
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
    public DepositAfterSalesQueryResponseCloseInfo? CloseInfo { get; set; }

    /// <summary>【出资商户号】 微信支付分配的商户号（官方字段路径 <c>sponsor_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchid { get; set; }

}

/// <summary>请求保证金运费赔付请求·【赔付用户信息】 赔付用户信息描述·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateRequestReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保证金运费赔付请求·【赔付用户信息】 赔付用户信息描述·【转账接收方用户】 转账接收方用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateRequestReceiverUserInfo
{
    /// <summary>【转账接收方用户 openid】 在商户sp_appid下的唯一用户标识（官方字段路径 <c>receiver.user_info.sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

}

/// <summary>请求保证金运费赔付请求·【赔付用户信息】 赔付用户信息描述。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateRequestReceiver
{
    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER时，需要设置（官方字段路径 <c>receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositShippingCostCreateRequestReceiverTransactionInfo? TransactionInfo { get; set; }

    /// <summary>【转账接收方用户】 转账接收方用户（官方字段路径 <c>receiver.user_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_info")]
    public DepositShippingCostCreateRequestReceiverUserInfo? UserInfo { get; set; }

}

/// <summary>请求保证金运费赔付请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，若指定二级商户号，则该商户为出资商户，若不指定二级商户号，必须指定出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【赔付用户信息】 赔付用户信息描述（官方字段路径 <c>receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositShippingCostCreateRequestReceiver? Receiver { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【赔付金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【转账场景ID】 当前默认枚举值有且只有：7002（运费赔付）（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【用户收款感知】 用户收款时感知到的收款原因将根据转账场景自动展示默认内容。如有其他展示需求，可在本字段传入。各场景展示的默认内容和支持传入的内容，可查看产品文档或联系微信支付运营人员了解。注：若指定该字段，必须同时指定转账场景ID，建议所有请求都传入转账场景ID（官方字段路径 <c>user_recv_perception</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

}

/// <summary>请求保证金运费赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保证金运费赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方用户】 转账接收方用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateResponseReceiverDetailReceiverUserInfo
{
    /// <summary>【转账接收方用户 openid】 在商户sp_appid下的唯一用户标识（官方字段路径 <c>receiver_detail.receiver.user_info.sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

}

/// <summary>请求保证金运费赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER时，需要设置（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositShippingCostCreateResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

    /// <summary>【转账接收方用户】 转账接收方用户（官方字段路径 <c>receiver_detail.receiver.user_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_info")]
    public DepositShippingCostCreateResponseReceiverDetailReceiverUserInfo? UserInfo { get; set; }

}

/// <summary>请求保证金运费赔付响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositShippingCostCreateResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>请求保证金运费赔付响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostCreateResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public DepositShippingCostCreateResponseReceiverDetail? ReceiverDetail { get; set; }

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

    /// <summary>【赔付状态】 赔付状态。可选取值：`ACCEPTED` 已受理 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
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

/// <summary>商户单号查询保证金运费赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostQueryResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【转账接收方订单类型】 转账接收方订单类型，默认WXPAY。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>商户单号查询保证金运费赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方用户】 转账接收方用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostQueryResponseReceiverDetailReceiverUserInfo
{
    /// <summary>【转账接收方用户 openid】 在商户sp_appid下的唯一用户标识（官方字段路径 <c>receiver_detail.receiver.user_info.sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

}

/// <summary>商户单号查询保证金运费赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostQueryResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方订单信息】 转账接收方订单信息，当接收方类型为TRANSACTION_USER时，需要设置（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositShippingCostQueryResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

    /// <summary>【转账接收方用户】 转账接收方用户（官方字段路径 <c>receiver_detail.receiver.user_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_info")]
    public DepositShippingCostQueryResponseReceiverDetailReceiverUserInfo? UserInfo { get; set; }

}

/// <summary>商户单号查询保证金运费赔付结果响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostQueryResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositShippingCostQueryResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>商户单号查询保证金运费赔付结果响应·【关单信息】 当状态为已关闭时返回，转账的关闭信息，如关闭原因等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostQueryResponseCloseInfo
{
    /// <summary>【关单时间】 单据关闭时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【关单原因】 当状态为已关闭时返回，关闭信息，如关闭原因等（官方字段路径 <c>close_info.close_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>商户单号查询保证金运费赔付结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositShippingCostQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public DepositShippingCostQueryResponseReceiverDetail? ReceiverDetail { get; set; }

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

    /// <summary>【转账状态】 转账状态。可选取值：`ACCEPTED` 已受理 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
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
    public DepositShippingCostQueryResponseCloseInfo? CloseInfo { get; set; }

    /// <summary>【出资商户号】 微信支付分配的商户号（官方字段路径 <c>sponsor_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchid { get; set; }

}

/// <summary>请求保证金保费赔付请求·【赔付用户信息】 赔付用户信息描述·【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateRequestReceiverMchInfo
{
    /// <summary>【商户号，接收方类型为MERCHANT时，接收方账号为商户号】 微信支付分配的商户号（官方字段路径 <c>receiver.mch_info.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>请求保证金保费赔付请求·【赔付用户信息】 赔付用户信息描述·【赔付单信息】 赔付单信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateRequestReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【赔付单类型】 赔付单类型。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver.transaction_info.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保证金保费赔付请求·【赔付用户信息】 赔付用户信息描述。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateRequestReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需设置接收方商户信息。此商户与服务商商户和二级商户不同，且需开通运营户（官方字段路径 <c>receiver.mch_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mch_info")]
    public DepositInsurancePremiumsCreateRequestReceiverMchInfo? MchInfo { get; set; }

    /// <summary>【赔付单信息】 赔付单信息（官方字段路径 <c>receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositInsurancePremiumsCreateRequestReceiverTransactionInfo? TransactionInfo { get; set; }

}

/// <summary>请求保证金保费赔付请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，若指定二级商户号，则该商户为出资商户，若不指定二级商户号，必须指定出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【赔付用户信息】 赔付用户信息描述（官方字段路径 <c>receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositInsurancePremiumsCreateRequestReceiver? Receiver { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【赔付金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

}

/// <summary>请求保证金保费赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateResponseReceiverDetailReceiverMchInfo
{
    /// <summary>【商户号，接收方类型为MERCHANT时，接收方账号为商户号】 微信支付分配的商户号（官方字段路径 <c>receiver_detail.receiver.mch_info.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>请求保证金保费赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【赔付单信息】 赔付单信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【赔付单类型】 赔付单类型。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>请求保证金保费赔付响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需设置接收方商户信息。此商户与服务商商户和二级商户不同，且需开通运营户（官方字段路径 <c>receiver_detail.receiver.mch_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mch_info")]
    public DepositInsurancePremiumsCreateResponseReceiverDetailReceiverMchInfo? MchInfo { get; set; }

    /// <summary>【赔付单信息】 赔付单信息（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositInsurancePremiumsCreateResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

}

/// <summary>请求保证金保费赔付响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositInsurancePremiumsCreateResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>请求保证金保费赔付响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsCreateResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public DepositInsurancePremiumsCreateResponseReceiverDetail? ReceiverDetail { get; set; }

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

    /// <summary>【赔付状态】 赔付状态。可选取值：`ACCEPTED` 已受理 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
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

/// <summary>商户单号查询保证金保费赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsQueryResponseReceiverDetailReceiverMchInfo
{
    /// <summary>【商户号，接收方类型为MERCHANT时，接收方账号为商户号】 微信支付分配的商户号（官方字段路径 <c>receiver_detail.receiver.mch_info.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>商户单号查询保证金保费赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【赔付单信息】 赔付单信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsQueryResponseReceiverDetailReceiverTransactionInfo
{
    /// <summary>【交易订单号】 当转账接收方订单类型为微信支付订单号时，传入微信支付系统生成的订单号,合单交易为微信支付子单订单号。当转账接收方订单类型为支付分服务订单时，传入微信支付支付分服务订单号（官方字段路径 <c>receiver_detail.receiver.transaction_info.transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【赔付单类型】 赔付单类型。可选取值：`WXPAY` 微信支付订单 / `WXVALUE` 支付分服务订单（官方字段路径 <c>receiver_detail.receiver.transaction_info.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>商户单号查询保证金保费赔付结果响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsQueryResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需设置接收方商户信息。此商户与服务商商户和二级商户不同，且需开通运营户（官方字段路径 <c>receiver_detail.receiver.mch_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mch_info")]
    public DepositInsurancePremiumsQueryResponseReceiverDetailReceiverMchInfo? MchInfo { get; set; }

    /// <summary>【赔付单信息】 赔付单信息（官方字段路径 <c>receiver_detail.receiver.transaction_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transaction_info")]
    public DepositInsurancePremiumsQueryResponseReceiverDetailReceiverTransactionInfo? TransactionInfo { get; set; }

}

/// <summary>商户单号查询保证金保费赔付结果响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsQueryResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public DepositInsurancePremiumsQueryResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>商户单号查询保证金保费赔付结果响应·【关单信息】 当状态为已关闭时返回，转账的关闭信息，如关闭原因等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsQueryResponseCloseInfo
{
    /// <summary>【关单时间】 单据关闭时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【关单原因】 当状态为已关闭时返回，关闭信息，如关闭原因等（官方字段路径 <c>close_info.close_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>商户单号查询保证金保费赔付结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceDeposit")]
public class DepositInsurancePremiumsQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public DepositInsurancePremiumsQueryResponseReceiverDetail? ReceiverDetail { get; set; }

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

    /// <summary>【转账状态】 转账状态。可选取值：`ACCEPTED` 已受理 / `SUCCESS` 已成功 / `CLOSED` 系统关闭（余额不足、转账失败等原因）。 / `WAIT_USER_CONFIRM` 预下单成功，等待用户确认 / `CANCELING` 商户撤销请求受理成功，该笔付款正在撤销中 / `CANCELLED` 付款撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
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
    public DepositInsurancePremiumsQueryResponseCloseInfo? CloseInfo { get; set; }

    /// <summary>【出资商户号】 微信支付分配的商户号（官方字段路径 <c>sponsor_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchid { get; set; }

}
