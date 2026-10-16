// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer;

/// <summary>请求商户付款请求·【赔付用户信息】 赔付用户信息描述·【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferCreateRequestReceiverMchInfo
{
    /// <summary>【商户号，接收方类型为MERCHANT时，接收方账号为商户号】 微信支付分配的商户号（官方字段路径 <c>receiver.mch_info.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>请求商户付款请求·【赔付用户信息】 赔付用户信息描述。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferCreateRequestReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需设置接收方商户信息。此商户与服务商商户和二级商户不同，且需开通运营户（官方字段路径 <c>receiver.mch_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mch_info")]
    public MchTransferCreateRequestReceiverMchInfo? MchInfo { get; set; }

}

/// <summary>请求商户付款请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferCreateRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，若指定二级商户号，则该商户为出资商户，若不指定二级商户号，必须指定出资商户号（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【公众账号ID】 微信分配的公众账号ID（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【业务类型】 业务类型。可选取值：`DEPOSIT_COMPENSATION` 保证金赔付 / `PLATFORM_AFTER_SALES_COMPENSATION` 平台售后赔付 / `INSURANCE_CLAIM` 保险理赔 / `DEPOSIT_AFTER_SALES_COMPENSATION` 保证金售后赔付（用户）（官方字段路径 <c>business_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【赔付用户信息】 赔付用户信息描述（官方字段路径 <c>receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public MchTransferCreateRequestReceiver? Receiver { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【赔付金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【赔付原因】 赔付原因（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【收款方备注】 收款方备注（官方字段路径 <c>receiver_remark</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("receiver_remark")]
    public string? ReceiverRemark { get; set; }

}

/// <summary>请求商户付款响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方商户信息】接收方商户信息1. 接收方类型为MERCHANT时，需设置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferCreateResponseReceiverDetailReceiverMchInfo
{
    /// <summary>【商户号，接收方类型为MERCHANT时，接收方账号为商户号】 微信支付分配的商户号（官方字段路径 <c>receiver_detail.receiver.mch_info.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>请求商户付款响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferCreateResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方商户信息】接收方商户信息1. 接收方类型为MERCHANT时，需设置接收方商户信息。此商户与服务商商户和二级商户不同，且需开通运营户（官方字段路径 <c>receiver_detail.receiver.mch_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mch_info")]
    public MchTransferCreateResponseReceiverDetailReceiverMchInfo? MchInfo { get; set; }

}

/// <summary>请求商户付款响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferCreateResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public MchTransferCreateResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>请求商户付款响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferCreateResponse : WechatPayResponse
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

    /// <summary>【业务类型】 业务类型。可选取值：`DEPOSIT_COMPENSATION` 保证金赔付 / `PLATFORM_AFTER_SALES_COMPENSATION` 平台售后赔付 / `INSURANCE_CLAIM` 保险理赔 / `DEPOSIT_AFTER_SALES_COMPENSATION` 保证金售后赔付（用户）（官方字段路径 <c>business_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public MchTransferCreateResponseReceiverDetail? ReceiverDetail { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【付款备注】 付款备注（官方字段路径 <c>transfer_remark</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【接收方备注】 接收方备注（官方字段路径 <c>receiver_remark</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("receiver_remark")]
    public string? ReceiverRemark { get; set; }

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

/// <summary>商户单号查询商户付款记录响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息·【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferQueryResponseReceiverDetailReceiverMchInfo
{
    /// <summary>【商户号，接收方类型为MERCHANT时，接收方账号为商户号】 微信支付分配的商户号（官方字段路径 <c>receiver_detail.receiver.mch_info.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>商户单号查询商户付款记录响应·【转账接收方信息】 转账接收方信息·【转账接收者信息】 转账接收者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferQueryResponseReceiverDetailReceiver
{
    /// <summary>【转账接收方类型】 转账接收方类型。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>receiver_detail.receiver.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【转账接收方商户信息】 接收方商户信息1\. 接收方类型为MERCHANT时，需设置接收方商户信息。此商户与服务商商户和二级商户不同，且需开通运营户（官方字段路径 <c>receiver_detail.receiver.mch_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mch_info")]
    public MchTransferQueryResponseReceiverDetailReceiverMchInfo? MchInfo { get; set; }

}

/// <summary>商户单号查询商户付款记录响应·【转账接收方信息】 转账接收方信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferQueryResponseReceiverDetail
{
    /// <summary>【转账接收者信息】 转账接收者信息（官方字段路径 <c>receiver_detail.receiver</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public MchTransferQueryResponseReceiverDetailReceiver? Receiver { get; set; }

}

/// <summary>商户单号查询商户付款记录响应·【关单信息】 当状态为已关闭时返回，转账的关闭信息，如关闭原因等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferQueryResponseCloseInfo
{
    /// <summary>【关单时间】 单据关闭时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【关单原因】 当状态为已关闭时返回，关闭信息，如关闭原因等（官方字段路径 <c>close_info.close_reason</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>商户单号查询商户付款记录响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceMchTransfer")]
public class MchTransferQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【业务类型】 业务类型。可选取值：`DEPOSIT_COMPENSATION` 保证金赔付 / `PLATFORM_AFTER_SALES_COMPENSATION` 平台售后赔付 / `INSURANCE_CLAIM` 保险理赔 / `DEPOSIT_AFTER_SALES_COMPENSATION` 保证金售后赔付（用户）（官方字段路径 <c>business_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【转账接收方信息】 转账接收方信息（官方字段路径 <c>receiver_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver_detail")]
    public MchTransferQueryResponseReceiverDetail? ReceiverDetail { get; set; }

    /// <summary>【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【金额】 金额，单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【付款备注】 付款备注（官方字段路径 <c>transfer_remark</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【接收方备注】 接收方备注（官方字段路径 <c>receiver_remark</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("receiver_remark")]
    public string? ReceiverRemark { get; set; }

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
    public MchTransferQueryResponseCloseInfo? CloseInfo { get; set; }

    /// <summary>【出资商户号】 微信支付分配的商户号（官方字段路径 <c>sponsor_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchid { get; set; }

}
