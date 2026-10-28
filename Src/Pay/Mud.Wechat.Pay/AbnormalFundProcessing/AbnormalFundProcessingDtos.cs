// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.AbnormalFundProcessing;

/// <summary>在途异常资金付款单查询响应·【金额】 付款金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408031"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingReceiptResponseTransferAmount
{
    /// <summary>【总金额】 在途异常资金付款总金额，单位为分，整型（官方字段路径 <c>transfer_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 固定返回：CNY，代表人民币。可选取值：`CNY` 人民币（官方字段路径 <c>transfer_amount.currency</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>在途异常资金付款单查询响应·【在途异常资金付款指令信息】 在途异常资金付款指令信息，仅当 receipt_s·【在途异常资金付款指令的发起方】 在途异常资金付款指令的发起方。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408031"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingReceiptResponseInstructionCommander
{
    /// <summary>【发起在途异常资金付款请求的角色】 发起在途异常资金付款请求的角色。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>instruction.commander.operator</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("operator")]
    public string? OperatorValue { get; set; }

    /// <summary>【微信支付商户号】 发起在途异常资金付款请求的微信支付商户号，仅当操作角色是"微信支付商户"时返回（官方字段路径 <c>instruction.commander.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>在途异常资金付款单查询响应·【在途异常资金付款指令信息】 在途异常资金付款指令信息，仅当 receipt_s·【收款方】 收款方帐号信息仅在以下情况下返回： 1. 当 transfer_mo。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408031"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingReceiptResponseInstructionReceiver
{
    /// <summary>【入账账户类型】 入账账户的类型银行类型对照表: https://pay.weixin.qq.com/doc/v3/merchant/4012076355.md XXX_CREDIT：转入XXX银行的一张信用卡。XXX_DEBIT：转入XXX银行的一张储蓄卡。OTHERS：转入零钱/零钱通等其他方式。仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 时返回（官方字段路径 <c>instruction.receiver.account_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>【入账用户OpenID】 入账用户在商户AppID下的唯一标识，仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 时返回（官方字段路径 <c>instruction.receiver.openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户AppID】 微信支付商户号绑定的AppID，如商户拉起用户收款页时使用的AppID。仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 时返回（官方字段路径 <c>instruction.receiver.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【入账商户号】 微信支付分配的商户号，仅当 transfer_mode 为 TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT 或 TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT 时返回（官方字段路径 <c>instruction.receiver.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>在途异常资金付款单查询响应·【在途异常资金付款指令信息】 在途异常资金付款指令信息，仅当 receipt_s。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408031"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingReceiptResponseInstruction
{
    /// <summary>【商户侧指令编号】 商户侧指令编号（官方字段路径 <c>instruction.out_instruction_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_instruction_no")]
    public string? OutInstructionNo { get; set; }

    /// <summary>【在途异常资金付款指令的发起方】 在途异常资金付款指令的发起方（官方字段路径 <c>instruction.commander</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("commander")]
    public AbnormalFundProcessingReceiptResponseInstructionCommander? Commander { get; set; }

    /// <summary>【付款方式】 资金处理规则。可选取值：`TRANSFER_TO_ORIGINAL_RECEIVE_USER` 付款至原收款用户 / `TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT` 付款至原收款商户 / `TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT` 付款到指定的收款商户（官方字段路径 <c>instruction.transfer_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_mode")]
    public string? TransferMode { get; set; }

    /// <summary>【收款方】 收款方帐号信息仅在以下情况下返回： 1. 当 transfer_mode 为 TRANSFER_TO_ORIGINAL_RECEIVE_USER 且 instruction_state 为 INSTRUCTION_STATE_SUCCESS 时； 2. 当 transfer_mode 为 TRANSFER_MODE_TO_ORIGINAL_RECEIVE_MERCHANT 或 TRANSFER_MODE_TO_SPECIFIED_RECEIVE…（官方字段路径 <c>instruction.receiver</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public AbnormalFundProcessingReceiptResponseInstructionReceiver? Receiver { get; set; }

    /// <summary>【在途异常资金付款指令状态】 在途异常资金付款指令状态。可选取值：`INSTRUCTION_STATE_PENDING` 待执行 / `INSTRUCTION_STATE_IN_PROGRESS` 执行中 / `INSTRUCTION_STATE_CLOSED` 已关闭，此时可更换商户侧指令编号，并重新发起付款指令 / `INSTRUCTION_STATE_SUCCESS` 执行成功，可通过查询在途异常资金付款结果接口，进一步确认付款结果（官方字段路径 <c>instruction.instruction_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("instruction_state")]
    public string? InstructionState { get; set; }

    /// <summary>【在途异常资金付款指令创建时间】 在途异常资金付款指令的创建时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>instruction.create_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【入账成功时间】 在途异常资金付款指令的入账时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。仅当 instruction_state = INSTRUCTION_STATE_SUCCESS 时返回（官方字段路径 <c>instruction.success_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【回调通知地址】 回调通知地址，只支持HTTPS协议，需为外网可访问的URL，不能携带查询参数。设置后，在途异常资金的付款结果会通过该URL通知商户（官方字段路径 <c>instruction.notify_url</c>，<b>必填</b>，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【微信支付在途异常资金付款指令执行流水号】 仅 instruction_state = INSTRUCTION_STATE_SUCCESS 时返回（官方字段路径 <c>instruction.instruction_instance_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("instruction_instance_id")]
    public string? InstructionInstanceId { get; set; }

}

/// <summary>在途异常资金付款单查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408031"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingReceiptResponse : WechatPayResponse
{
    /// <summary>【产品名称】 发起在途异常资金付款的业务产品名称（官方字段路径 <c>product_name</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>【微信支付在途异常资金付款单号】 微信支付在途异常资金付款单号（官方字段路径 <c>receipt_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("receipt_id")]
    public string? ReceiptId { get; set; }

    /// <summary>【金额】 付款金额信息（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public AbnormalFundProcessingReceiptResponseTransferAmount? TransferAmount { get; set; }

    /// <summary>【在途异常资金付款状态】 当前单据处理进度。可选取值：`RECEIPT_STATE_PENDING` 待处理 / `RECEIPT_STATE_PROGRESS` 处理中 / `RECEIPT_STATE_COMPLETED` 处理成功（官方字段路径 <c>receipt_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

    /// <summary>【在途异常资金付款单据创建时间】 微信支付在途异常资金付款单据的创建时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【在途异常资金付款单据更新时间】 微信支付在途异常资金付款单据的最新更新时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>last_update_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("last_update_time")]
    public string? LastUpdateTime { get; set; }

    /// <summary>【在途异常资金付款指令信息】 在途异常资金付款指令信息，仅当 receipt_state = RECEIPT_STATE_PROGRESS 或 receipt_state = RECEIPT_STATE_COMPLETED 且当前付款操作由商户发起时才返回（官方字段路径 <c>instruction</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("instruction")]
    public AbnormalFundProcessingReceiptResponseInstruction? Instruction { get; set; }

}

/// <summary>在途异常资金付款指令发起请求·【收款方】 在途异常资金的收款方。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionCreateRequestReceiver
{
    /// <summary>【入账用户OpenID】 入账用户在商户AppID下的唯一标识，仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 且用户直接入账时必填，且须与原收款用户保持同一实名身份（官方字段路径 <c>receiver.openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户AppID】 微信支付商户号绑定的AppID，仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 且用户直接入账时必填，传入的收款用户OpenID必须是该AppID下的OpenID，请确保OpenID与AppID的从属关系正确（官方字段路径 <c>receiver.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【入账商户号】 微信支付分配的商户号，仅当 transfer_mode 为 TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT 或 TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT 时必填（官方字段路径 <c>receiver.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>在途异常资金付款指令发起请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionCreateRequest
{
    /// <summary>【商户侧指令编号】 商户侧指令编号，商户需保证同一笔在途异常资金付款单下唯一（官方字段路径 <c>out_instruction_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_instruction_no")]
    public string? OutInstructionNo { get; set; }

    /// <summary>【付款方式】 资金处理规则。可选取值：`TRANSFER_TO_ORIGINAL_RECEIVE_USER` 付款至原收款用户 / `TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT` 付款至原收款商户 / `TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT` 付款到指定的收款商户（官方字段路径 <c>transfer_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_mode")]
    public string? TransferMode { get; set; }

    /// <summary>【回调通知地址】 回调通知地址，只支持HTTPS协议，需为外网可访问的URL，不能携带查询参数。设置后，在途异常资金的付款结果会通过该URL通知商户（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【收款方】 在途异常资金的收款方（官方字段路径 <c>receiver</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public AbnormalFundProcessingInstructionCreateRequestReceiver? Receiver { get; set; }

}

/// <summary>在途异常资金付款指令发起响应·【金额】 付款金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionCreateResponseTransferAmount
{
    /// <summary>【总金额】 在途异常资金付款总金额，单位为分，整型（官方字段路径 <c>transfer_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 固定返回：CNY，代表人民币。可选取值：`CNY` 人民币（官方字段路径 <c>transfer_amount.currency</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>在途异常资金付款指令发起响应·【在途异常资金付款指令信息】 在途异常资金付款指令信息·【在途异常资金付款指令的发起方】 在途异常资金付款指令的发起方。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionCreateResponseInstructionCommander
{
    /// <summary>【发起在途异常资金付款请求的角色】 发起在途异常资金付款请求的角色。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>instruction.commander.operator</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("operator")]
    public string? OperatorValue { get; set; }

    /// <summary>【微信支付商户号】 发起在途异常资金付款请求的微信支付商户号，仅当操作角色是"微信支付商户"时返回（官方字段路径 <c>instruction.commander.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>在途异常资金付款指令发起响应·【在途异常资金付款指令信息】 在途异常资金付款指令信息·【收款方】 收款方帐号信息，仅当 transfer_mode 为 TRANSFE。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionCreateResponseInstructionReceiver
{
    /// <summary>【入账用户OpenID】 入账用户在商户AppID下的唯一标识，仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 且用户直接入账时必填，且须与原收款用户保持同一实名身份（官方字段路径 <c>instruction.receiver.openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户AppID】 微信支付商户号绑定的AppID，仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 且用户直接入账时必填，传入的收款用户OpenID必须是该AppID下的OpenID，请确保OpenID与AppID的从属关系正确（官方字段路径 <c>instruction.receiver.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【入账商户号】 微信支付分配的商户号，仅当 transfer_mode 为 TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT 或 TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT 时必填（官方字段路径 <c>instruction.receiver.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>在途异常资金付款指令发起响应·【在途异常资金付款指令信息】 在途异常资金付款指令信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionCreateResponseInstruction
{
    /// <summary>【商户侧指令编号】 商户侧指令编号，商户需保证同一笔在途异常资金付款单下唯一（官方字段路径 <c>instruction.out_instruction_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_instruction_no")]
    public string? OutInstructionNo { get; set; }

    /// <summary>【在途异常资金付款指令的发起方】 在途异常资金付款指令的发起方（官方字段路径 <c>instruction.commander</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("commander")]
    public AbnormalFundProcessingInstructionCreateResponseInstructionCommander? Commander { get; set; }

    /// <summary>【付款方式】 资金处理规则。可选取值：`TRANSFER_TO_ORIGINAL_RECEIVE_USER` 付款至原收款用户 / `TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT` 付款至原收款商户 / `TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT` 付款到指定的收款商户（官方字段路径 <c>instruction.transfer_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_mode")]
    public string? TransferMode { get; set; }

    /// <summary>【收款方】 收款方帐号信息，仅当 transfer_mode 为 TRANSFER_MODE_TO_ORIGINAL_RECEIVE_MERCHANT 或 TRANSFER_MODE_TO_SPECIFIED_RECEIVE_MERCHANT 返回（官方字段路径 <c>instruction.receiver</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public AbnormalFundProcessingInstructionCreateResponseInstructionReceiver? Receiver { get; set; }

    /// <summary>【在途异常资金付款指令状态】 在途异常资金付款指令状态。可选取值：`INSTRUCTION_STATE_PENDING` 待执行 / `INSTRUCTION_STATE_IN_PROGRESS` 执行中 / `INSTRUCTION_STATE_CLOSED` 已关闭，此时可更换商户侧指令编号，并重新发起付款指令 / `INSTRUCTION_STATE_SUCCESS` 执行成功，可通过查询在途异常资金付款结果接口，进一步确认付款结果（官方字段路径 <c>instruction.instruction_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("instruction_state")]
    public string? InstructionState { get; set; }

    /// <summary>【在途异常资金付款指令创建时间】 在途异常资金付款指令的创建时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>instruction.create_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【入账成功时间】 在途异常资金付款指令的入账时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。仅当 instruction_state = INSTRUCTION_STATE_SUCCESS 时返回（官方字段路径 <c>instruction.success_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【回调通知地址】 回调通知地址，只支持HTTPS协议，需为外网可访问的URL，不能携带查询参数。设置后，在途异常资金的付款结果会通过该URL通知商户（官方字段路径 <c>instruction.notify_url</c>，<b>必填</b>，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>在途异常资金付款指令发起响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015408022"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionCreateResponse : WechatPayResponse
{
    /// <summary>【产品名称】 发起在途异常资金付款的业务产品名称（官方字段路径 <c>product_name</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>【微信支付在途异常资金付款单号】 微信支付在途异常资金付款单号（官方字段路径 <c>receipt_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("receipt_id")]
    public string? ReceiptId { get; set; }

    /// <summary>【金额】 付款金额信息（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public AbnormalFundProcessingInstructionCreateResponseTransferAmount? TransferAmount { get; set; }

    /// <summary>【在途异常资金付款状态】 当前单据处理进度。可选取值：`RECEIPT_STATE_PENDING` 待处理 / `RECEIPT_STATE_PROGRESS` 处理中 / `RECEIPT_STATE_COMPLETED` 处理成功（官方字段路径 <c>receipt_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

    /// <summary>【在途异常资金付款单据创建时间】 微信支付在途异常资金付款单据的创建时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【在途异常资金付款单据更新时间】 微信支付在途异常资金付款单据的最新更新时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>last_update_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("last_update_time")]
    public string? LastUpdateTime { get; set; }

    /// <summary>【在途异常资金付款指令信息】 在途异常资金付款指令信息（官方字段路径 <c>instruction</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("instruction")]
    public AbnormalFundProcessingInstructionCreateResponseInstruction? Instruction { get; set; }

}

/// <summary>在途异常资金付款指令执行结果查询响应·【在途异常资金付款指令的发起方】 在途异常资金付款指令的发起方。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016636882"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionResponseCommander
{
    /// <summary>【发起在途异常资金付款请求的角色】 发起在途异常资金付款请求的角色。可选取值：`MERCHANT` 微信支付商户（官方字段路径 <c>commander.operator</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("operator")]
    public string? OperatorValue { get; set; }

    /// <summary>【微信支付商户号】 发起在途异常资金付款请求的微信支付商户号，仅当操作角色是"微信支付商户"时返回（官方字段路径 <c>commander.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>在途异常资金付款指令执行结果查询响应·【收款方】 收款方帐号信息仅在以下情况下返回： 1. 当 transfer_mo。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016636882"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionResponseReceiver
{
    /// <summary>【入账账户类型】 入账账户的类型银行类型对照表: https://pay.weixin.qq.com/doc/v3/merchant/4012076355.md XXX_CREDIT：转入XXX银行的一张信用卡。XXX_DEBIT：转入XXX银行的一张储蓄卡。OTHERS：转入零钱/零钱通等其他方式。仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 时返回（官方字段路径 <c>receiver.account_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>【入账用户OpenID】 入账用户在商户AppID下的唯一标识，仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 时返回（官方字段路径 <c>receiver.openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户AppID】 微信支付商户号绑定的AppID，如商户拉起用户收款页时使用的AppID。仅当 transfer_mode = TRANSFER_TO_ORIGINAL_RECEIVE_USER 时返回（官方字段路径 <c>receiver.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【入账商户号】 微信支付分配的商户号，当 transfer_mode 为 TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT 或 TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT 时必填（官方字段路径 <c>receiver.mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>在途异常资金付款指令执行结果查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016636882"/></summary>
[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]
public class AbnormalFundProcessingInstructionResponse : WechatPayResponse
{
    /// <summary>【商户侧指令编号】 商户侧指令编号（官方字段路径 <c>out_instruction_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_instruction_no")]
    public string? OutInstructionNo { get; set; }

    /// <summary>【在途异常资金付款指令的发起方】 在途异常资金付款指令的发起方（官方字段路径 <c>commander</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("commander")]
    public AbnormalFundProcessingInstructionResponseCommander? Commander { get; set; }

    /// <summary>【付款方式】 资金处理规则。可选取值：`TRANSFER_TO_ORIGINAL_RECEIVE_USER` 付款至原收款用户 / `TRANSFER_TO_ORIGINAL_RECEIVE_MERCHANT` 付款至原收款商户 / `TRANSFER_TO_SPECIFIED_RECEIVE_MERCHANT` 付款到指定的收款商户（官方字段路径 <c>transfer_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_mode")]
    public string? TransferMode { get; set; }

    /// <summary>【收款方】 收款方帐号信息仅在以下情况下返回： 1. 当 transfer_mode 为 TRANSFER_TO_ORIGINAL_RECEIVE_USER 且 instruction_state 为 INSTRUCTION_STATE_SUCCESS 时； 2. 当 transfer_mode 为 TRANSFER_MODE_TO_ORIGINAL_RECEIVE_MERCHANT 或 TRANSFER_MODE_TO_SPECIFIED_RECEIVE…（官方字段路径 <c>receiver</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("receiver")]
    public AbnormalFundProcessingInstructionResponseReceiver? Receiver { get; set; }

    /// <summary>【在途异常资金付款指令状态】 在途异常资金付款指令状态。可选取值：`INSTRUCTION_STATE_PENDING` 待执行 / `INSTRUCTION_STATE_IN_PROGRESS` 执行中 / `INSTRUCTION_STATE_CLOSED` 已关闭，此时可更换商户侧指令编号，并重新发起付款指令 / `INSTRUCTION_STATE_SUCCESS` 执行成功，可通过查询在途异常资金付款结果接口，进一步确认付款结果（官方字段路径 <c>instruction_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("instruction_state")]
    public string? InstructionState { get; set; }

    /// <summary>【在途异常资金付款指令创建时间】 在途异常资金付款指令的创建时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【入账成功时间】 在途异常资金付款指令的入账时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。仅当 instruction_state = INSTRUCTION_STATE_SUCCESS 时返回（官方字段路径 <c>success_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【在途异常资金付款指令关闭时间】 在途异常资金付款指令的关闭时间，遵循rfc3339标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。 仅当 instruction_state = INSTRUCTION_STATE_CLOSED 时返回（官方字段路径 <c>close_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【在途异常资金付款指令关闭原因】 在途异常资金付款指令的关闭原因，仅当 instruction_state = INSTRUCTION_STATE_CLOSED 时返回（官方字段路径 <c>close_reason</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

    /// <summary>【回调通知地址】 回调通知地址，只支持HTTPS协议，需为外网可访问的URL，不能携带查询参数。设置后，在途异常资金的付款结果会通过该URL通知商户（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【微信支付在途异常资金付款指令执行流水号】 仅 instruction_state = INSTRUCTION_STATE_SUCCESS 时返回（官方字段路径 <c>instruction_instance_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("instruction_instance_id")]
    public string? InstructionInstanceId { get; set; }

}
