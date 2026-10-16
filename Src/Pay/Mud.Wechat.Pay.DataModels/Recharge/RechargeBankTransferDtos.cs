// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Recharge;

/// <summary>申请银行转账充值请求·【充值金额】单位为分，仅支持CNY。单次充值上限为2千万。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585425"/></summary>
[HttpJsonSerializable(SerializerClassName = "Recharge")]
public class RechargeBankTransferApplyRequestRechargeAmount
{
    /// <summary>【总金额】单位为分（官方字段路径 <c>recharge_amount.amount</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【货币类型】人民币为CNY（官方字段路径 <c>recharge_amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>申请银行转账充值请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585425"/></summary>
[HttpJsonSerializable(SerializerClassName = "Recharge")]
public class RechargeBankTransferApplyRequest
{
    /// <summary>【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方字段路径 <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_recharge_no")]
    public string? OutRechargeNo { get; set; }

    /// <summary>【充值场景】仅支持普通商户银行转账充值。可选取值：`NORMAL_BANK_TRANSFER` 普通商户银行转账充值（官方字段路径 <c>recharge_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recharge_scene")]
    public string? RechargeScene { get; set; }

    /// <summary>【充值入账账户】 充值入账账户资金。可选取值：`BASIC` 基本户 / `OPERATION` 运营户 / `FEES` 手续费账户（官方字段路径 <c>account_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>【充值金额】单位为分，仅支持CNY。单次充值上限为2千万（官方字段路径 <c>recharge_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("recharge_amount")]
    public RechargeBankTransferApplyRequestRechargeAmount? RechargeAmount { get; set; }

}

/// <summary>申请银行转账充值响应·【银行账户】充值目标账户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585425"/></summary>
[HttpJsonSerializable(SerializerClassName = "Recharge")]
public class RechargeBankTransferApplyResponseTransferInAccount
{
    /// <summary>【开户银行名称】开户银行名称（官方字段路径 <c>transfer_in_account.bank_name</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【开户银行省市编码】 开户银行省市编码，详细参见《 省市区编码对照表》（官方字段路径 <c>transfer_in_account.bank_address_code</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>【开户银行行号】开户银行行号（官方字段路径 <c>transfer_in_account.bank_code</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_code")]
    public string? BankCode { get; set; }

    /// <summary>【银行账户名称】 银行账户名称（官方字段路径 <c>transfer_in_account.bank_account_name</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("bank_account_name")]
    public string? BankAccountName { get; set; }

    /// <summary>【银行账号】 银行账号（官方字段路径 <c>transfer_in_account.bank_account_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_account_no")]
    public string? BankAccountNo { get; set; }

}

/// <summary>申请银行转账充值响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585425"/></summary>
[HttpJsonSerializable(SerializerClassName = "Recharge")]
public class RechargeBankTransferApplyResponse : WechatPayResponse
{
    /// <summary>【微信支付充值单号】微信支付充值单号（官方字段路径 <c>recharge_id</c>，<b>必填</b>，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recharge_id")]
    public string? RechargeId { get; set; }

    /// <summary>【商户充值单号】商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方字段路径 <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_recharge_no")]
    public string? OutRechargeNo { get; set; }

    /// <summary>【银行账户】充值目标账户（官方字段路径 <c>transfer_in_account</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transfer_in_account")]
    public RechargeBankTransferApplyResponseTransferInAccount? TransferInAccount { get; set; }

}

/// <summary>查询银行转账充值结果响应·【充值金额】单位为分，仅支持CNY。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585430"/></summary>
[HttpJsonSerializable(SerializerClassName = "Recharge")]
public class RechargeBankTransferQueryResponseRechargeAmount
{
    /// <summary>【总金额】单位为分（官方字段路径 <c>recharge_amount.amount</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【货币类型】人民币为CNY（官方字段路径 <c>recharge_amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>查询银行转账充值结果响应·【转账充值的付款信息】包括充值附言等内容。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585430"/></summary>
[HttpJsonSerializable(SerializerClassName = "Recharge")]
public class RechargeBankTransferQueryResponseBankTransferInfo
{
    /// <summary>【转账充值附言】转账充值附言（官方字段路径 <c>bank_transfer_info.memo</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("memo")]
    public string? Memo { get; set; }

    /// <summary>【银行转账退回时间】使用rfc3339标准格式（官方字段路径 <c>bank_transfer_info.return_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【银行转账退回原因】银行转账退回原因（官方字段路径 <c>bank_transfer_info.return_reason</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("return_reason")]
    public string? ReturnReason { get; set; }

    /// <summary>【开户银行名称】充值出资账户开户银行名称（官方字段路径 <c>bank_transfer_info.bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行卡号后四位】充值出资账户卡号后四位（官方字段路径 <c>bank_transfer_info.bank_card_tail</c>，选填，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("bank_card_tail")]
    public string? BankCardTail { get; set; }

}

/// <summary>查询银行转账充值结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012585430"/></summary>
[HttpJsonSerializable(SerializerClassName = "Recharge")]
public class RechargeBankTransferQueryResponse : WechatPayResponse
{
    /// <summary>【充值商户号】微信支付分配的商户号，充值商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【微信支付充值单号】 微信支付充值单号（官方字段路径 <c>recharge_id</c>，<b>必填</b>，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recharge_id")]
    public string? RechargeId { get; set; }

    /// <summary>【商户充值单号】商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方字段路径 <c>out_recharge_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_recharge_no")]
    public string? OutRechargeNo { get; set; }

    /// <summary>【充值入账账户】充值入账账户。可选取值：`BASIC` 基本户 / `OPERATION` 运营户 / `FEES` 手续费账户（官方字段路径 <c>account_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>【充值状态】充值状态。可选取值：`SUCCESS` 充值成功 / `RECHARGING` 充值中 / `CLOSED` 已关闭（充值单七天有效，如若商户不充值，则过期系统会自动关闭）（官方字段路径 <c>recharge_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recharge_state")]
    public string? RechargeState { get; set; }

    /// <summary>【充值场景】充值场景。可选取值：`NORMAL_BANK_TRANSFER` 银行转账充值（官方字段路径 <c>recharge_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recharge_scene")]
    public string? RechargeScene { get; set; }

    /// <summary>【充值状态描述】展示关闭充值的原因，如“平台商户主动关闭充值单”、“超过时间限制，系统自动关闭充值单”等（官方字段路径 <c>recharge_state_desc</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("recharge_state_desc")]
    public string? RechargeStateDesc { get; set; }

    /// <summary>【充值金额】单位为分，仅支持CNY（官方字段路径 <c>recharge_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("recharge_amount")]
    public RechargeBankTransferQueryResponseRechargeAmount? RechargeAmount { get; set; }

    /// <summary>【转账充值的付款信息】包括充值附言等内容（官方字段路径 <c>bank_transfer_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("bank_transfer_info")]
    public RechargeBankTransferQueryResponseBankTransferInfo? BankTransferInfo { get; set; }

    /// <summary>【受理充值时间】微信支付成功受理充值时间，不代表充值已经完成，使用rfc3339标准格式（官方字段路径 <c>accept_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>【充值成功时间】当状态为“充值成功”时返回，使用rfc3339标准格式（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【关闭充值时间】当状态为“已关闭”时返回，使用rfc3339标准格式（官方字段路径 <c>close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

}
