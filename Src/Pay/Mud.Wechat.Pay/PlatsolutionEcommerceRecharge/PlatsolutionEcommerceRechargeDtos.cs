// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRecharge;

/// <summary>申请二级商户充值请求·【充值金额】 单位为分，仅支持CNY。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeApplyRequestRechargeAmount
{
    /// <summary>【总金额】 单位为分（官方字段路径 <c>recharge_amount.amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【货币类型】 人民币为CNY（官方字段路径 <c>recharge_amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>申请二级商户充值请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeApplyRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方字段路径 <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_recharge_no")]
    public string? OutRechargeNo { get; set; }

    /// <summary>【充值场景】 按照实际情况填写。可选取值：`ECOMMERCE_DEPOSIT` 二级商户充值保证金 / `ECOMMERCE_PAYMENT` 二级商户充值运营资金（官方字段路径 <c>recharge_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recharge_scene")]
    public string? RechargeScene { get; set; }

    /// <summary>【充值入账账户】 使用充值场景所支持的入账账户。可选取值：`DEPOSIT` 保证金账户 / `OPERATION` 运营账户（官方字段路径 <c>account_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>【充值金额】 单位为分，仅支持CNY（官方字段路径 <c>recharge_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("recharge_amount")]
    public RechargeApplyRequestRechargeAmount? RechargeAmount { get; set; }

    /// <summary>【商户回调地址】 异步接收充值结果的回调地址（官方字段路径 <c>notify_url</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【可用充值渠道列表】 缺省展示全部充值渠道。若传入可用渠道列表，以传入内容为准。可选取值：`BANK_TRANSFER` 银行转账 / `QR_RECHARGE` 扫码充值 / `ONLINE_BANK` 网银充值（官方字段路径 <c>available_recharge_channels</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_recharge_channels")]
    public List<string>? AvailableRechargeChannels { get; set; }

    /// <summary>【充值备注】 将在充值页面展示。UTF8编码，最多允许20个字符（官方字段路径 <c>remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

}

/// <summary>申请二级商户充值响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762860"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeApplyResponse : WechatPayResponse
{
    /// <summary>【微信支付充值单号】 微信支付充值单号（官方字段路径 <c>recharge_id</c>，<b>必填</b>，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recharge_id")]
    public string? RechargeId { get; set; }

    /// <summary>【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方字段路径 <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_recharge_no")]
    public string? OutRechargeNo { get; set; }

    /// <summary>【充值链接】 由链接进入充值页面（官方字段路径 <c>recharge_url</c>，<b>必填</b>，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("recharge_url")]
    public string? RechargeUrl { get; set; }

    /// <summary>【跳转充值到商户账户页面的package信息】 APP调起充值到商户账户 时需要使用的参数（官方字段路径 <c>package_info</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }

}

/// <summary>查询二级商户充值结果响应·【充值金额】 单位为分，仅支持CNY。单笔限额规则：1、银行转账最多8亿元 2、。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeQueryResponseRechargeAmount
{
    /// <summary>【总金额】 单位为分（官方字段路径 <c>recharge_amount.amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【货币类型】 人民币为CNY（官方字段路径 <c>recharge_amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>查询二级商户充值结果响应·【转账充值的付款信息】 包括付款账户、银行附言等内容。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeQueryResponseBankTransferInfo
{
    /// <summary>【转入的银行流水单号】 转入的银行流水单号（官方字段路径 <c>bank_transfer_info.bill_no</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>【转账充值附言】 转账充值附言（官方字段路径 <c>bank_transfer_info.memo</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("memo")]
    public string? Memo { get; set; }

    /// <summary>【银行转账退回时间】 使用rfc3339标准格式（官方字段路径 <c>bank_transfer_info.return_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【银行转账退回原因】 银行转账退回原因（官方字段路径 <c>bank_transfer_info.return_reason</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("return_reason")]
    public string? ReturnReason { get; set; }

    /// <summary>【开户银行名称】 开户银行名称（官方字段路径 <c>bank_transfer_info.bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行卡号后四位】 银行卡号后四位（官方字段路径 <c>bank_transfer_info.bank_card_tail</c>，选填，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("bank_card_tail")]
    public string? BankCardTail { get; set; }

    /// <summary>【银行账户名称】 银行账户名称（官方字段路径 <c>bank_transfer_info.bank_account_name</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("bank_account_name")]
    public string? BankAccountName { get; set; }

}

/// <summary>查询二级商户充值结果响应·【扫码充值的付款信息】 包括充值用户 OpenID。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeQueryResponseQrRechargeInfo
{
    /// <summary>【用户 OpenID】 用户 OpenID（官方字段路径 <c>qr_recharge_info.openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【员工类型】 员工类型。可选取值：`ADMIN` 超级管理员 / `STAFF` 员工 / `LEGAL_PERSON` 法人（官方字段路径 <c>qr_recharge_info.employee_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("employee_type")]
    public string? EmployeeType { get; set; }

    /// <summary>【微信支付订单号】 微信付款的支付订单号（官方字段路径 <c>qr_recharge_info.transaction_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

}

/// <summary>查询二级商户充值结果响应·【网银充值的付款信息】 包括付款账户、银行交易订单号等内容。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeQueryResponseOnlineBankRechargeInfo
{
    /// <summary>【银行交易订单号】 银行交易订单号（官方字段路径 <c>online_bank_recharge_info.bill_no</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>【网银充值退回时间】 使用rfc3339标准格式（官方字段路径 <c>online_bank_recharge_info.return_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【网银充值退回原因】 网银充值退回原因（官方字段路径 <c>online_bank_recharge_info.return_reason</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("return_reason")]
    public string? ReturnReason { get; set; }

    /// <summary>【开户银行名称】 开户银行名称（官方字段路径 <c>online_bank_recharge_info.bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【网银类型】 网银类型。可选取值：`ONLINE_BANK_TYPE_CORPORATE` 企业网银 / `ONLINE_BANK_TYPE_PERSONAL` 个人网银（官方字段路径 <c>online_bank_recharge_info.online_bank_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("online_bank_type")]
    public string? OnlineBankType { get; set; }

    /// <summary>【银行卡号后四位】 银行卡号后四位（官方字段路径 <c>online_bank_recharge_info.bank_card_tail</c>，选填，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("bank_card_tail")]
    public string? BankCardTail { get; set; }

    /// <summary>【银行账户名称】 银行账户名称（官方字段路径 <c>online_bank_recharge_info.bank_account_name</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("bank_account_name")]
    public string? BankAccountName { get; set; }

}

/// <summary>查询二级商户充值结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012762874"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRecharge")]
public class RechargeQueryResponse : WechatPayResponse
{
    /// <summary>【平台商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号，充值商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信支付充值单号】 微信支付充值单号（官方字段路径 <c>recharge_id</c>，<b>必填</b>，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recharge_id")]
    public string? RechargeId { get; set; }

    /// <summary>【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方字段路径 <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_recharge_no")]
    public string? OutRechargeNo { get; set; }

    /// <summary>【充值渠道】 仅在“充值成功”时返回，其它状态不返回。可选取值：`BANK_TRANSFER` 银行转账 / `QR_RECHARGE` 扫码充值 / `ONLINE_BANK` 网银充值（官方字段路径 <c>recharge_channel</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recharge_channel")]
    public string? RechargeChannel { get; set; }

    /// <summary>【充值入账账户】 充值入账账户。可选取值：`DEPOSIT` 保证金账户 / `OPERATION` 运营账户（官方字段路径 <c>account_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>【充值状态】 充值状态。可选取值：`SUCCESS` 充值成功 / `RECHARGING` 充值中 / `CLOSED` 已关闭（官方字段路径 <c>recharge_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recharge_state")]
    public string? RechargeState { get; set; }

    /// <summary>【充值场景】 充值场景。可选取值：`ECOMMERCE_DEPOSIT` 二级商户充值保证金 / `ECOMMERCE_PAYMENT` 二级商户充值运营资金（官方字段路径 <c>recharge_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recharge_scene")]
    public string? RechargeScene { get; set; }

    /// <summary>【充值状态描述】 展示关闭充值的原因，如“平台商户主动关闭充值单”、“超过时间限制，系统自动关闭充值单”等（官方字段路径 <c>recharge_state_desc</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("recharge_state_desc")]
    public string? RechargeStateDesc { get; set; }

    /// <summary>【充值金额】 单位为分，仅支持CNY。单笔限额规则：1、银行转账最多8亿元 2、扫码充值5万元（官方字段路径 <c>recharge_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("recharge_amount")]
    public RechargeQueryResponseRechargeAmount? RechargeAmount { get; set; }

    /// <summary>【转账充值的付款信息】 包括付款账户、银行附言等内容（官方字段路径 <c>bank_transfer_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("bank_transfer_info")]
    public RechargeQueryResponseBankTransferInfo? BankTransferInfo { get; set; }

    /// <summary>【扫码充值的付款信息】 包括充值用户 OpenID（官方字段路径 <c>qr_recharge_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("qr_recharge_info")]
    public RechargeQueryResponseQrRechargeInfo? QrRechargeInfo { get; set; }

    /// <summary>【网银充值的付款信息】 包括付款账户、银行交易订单号等内容（官方字段路径 <c>online_bank_recharge_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("online_bank_recharge_info")]
    public RechargeQueryResponseOnlineBankRechargeInfo? OnlineBankRechargeInfo { get; set; }

    /// <summary>【受理充值时间】 微信支付成功受理充值时间，不代表充值已经完成，使用rfc3339标准格式（官方字段路径 <c>accept_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>【充值成功时间】 当状态为“充值成功”时返回，使用rfc3339标准格式（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【关闭充值时间】 当状态为“已关闭”时返回，使用rfc3339标准格式（官方字段路径 <c>close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【可用充值渠道列表】 申请充值时传入的可用渠道列表。可选取值：`BANK_TRANSFER` 银行转账 / `QR_RECHARGE` 扫码充值 / `ONLINE_BANK` 网银充值（官方字段路径 <c>available_recharge_channels</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_recharge_channels")]
    public List<string>? AvailableRechargeChannels { get; set; }

}
