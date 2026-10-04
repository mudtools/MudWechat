// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 查询申请单状态响应体（<c>/cgi-bin/miniapppay/get_applyment_status</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayApplymentStatusResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置申请单具体状态。
    /// </summary>
    [JsonPropertyName("status")]
    public PayApplymentStatus? Status { get; set; }

    /// <summary>
    /// 获取或设置申请单当前阶段：0 - 初始状态、1 - 申请中、2 - 绑定成功、3 - 已撤销申请、
    /// 4 - 绑定失败、5 - 未申请、6 - 待法人验证。
    /// </summary>
    [JsonPropertyName("apply_state")]
    public int? ApplyState { get; set; }

    /// <summary>
    /// 获取或设置当前签约阶段：0 - 不可签约、1 - 未签约、2 - 已签约。
    /// </summary>
    [JsonPropertyName("real_sign_state")]
    public int? RealSignState { get; set; }

    /// <summary>
    /// 获取或设置驳回理由。
    /// </summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}

/// <summary>
/// 申请单具体状态（<see cref="GetPayApplymentStatusResponse.Status"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayApplymentStatus
{
    /// <summary>
    /// 获取或设置申请状态：CHECKING - 资料校验中、NEED_SIGN - 待签约、
    /// ACCOUNT_NEED_VERIFY - 待账户验证、FINISH - 完成、AUDITING - 审核中、
    /// FROZEN - 已冻结、REJECTED - 已驳回、CANCELED - 已作废。
    /// </summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

    /// <summary>
    /// 获取或设置申请状态描述。
    /// </summary>
    [JsonPropertyName("applyment_state_desc")]
    public string? ApplymentStateDesc { get; set; }

    /// <summary>
    /// 获取或设置签约状态：UNSIGNED - 未签约（可获取签约链接引导签约）、
    /// SIGNED - 已签约、NOT_SIGNABLE - 不可签约（一般处于已驳回、已冻结、机器校验中状态）。
    /// </summary>
    [JsonPropertyName("sign_state")]
    public string? SignState { get; set; }

    /// <summary>
    /// 获取或设置签约链接（申请状态为 NEED_SIGN 或签约状态为 UNSIGNED 时返回，
    /// 链接永久有效；需超级管理员用已实名微信扫码完成签约）。
    /// </summary>
    [JsonPropertyName("sign_url")]
    public string? SignUrl { get; set; }

    /// <summary>
    /// 获取或设置电商平台二级商户号（申请状态为 NEED_SIGN 或 FINISH 时返回）。
    /// </summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>
    /// 获取或设置驳回原因详情（申请状态为 REJECTED 或 FROZEN 时返回）。
    /// </summary>
    [JsonPropertyName("audit_detail")]
    public List<PayApplymentAuditDetail>? AuditDetail { get; set; }

    /// <summary>
    /// 获取或设置汇款账户验证信息（签约阶段为 2 且申请状态为 ACCOUNT_NEED_VERIFY 时返回）。
    /// </summary>
    [JsonPropertyName("account_validation")]
    public PayAccountValidation? AccountValidation { get; set; }

    /// <summary>
    /// 获取或设置法人验证链接（建议转成二维码，供商户法人用微信扫码完成账户验证；
    /// 法人证件与营业执照匹配才返回）。
    /// </summary>
    [JsonPropertyName("legal_validation_url")]
    public string? LegalValidationUrl { get; set; }
}

/// <summary>
/// 申请单驳回原因详情（<see cref="PayApplymentStatus.AuditDetail"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayApplymentAuditDetail
{
    /// <summary>
    /// 获取或设置资料项名称。
    /// </summary>
    [JsonPropertyName("param_name")]
    public string? ParamName { get; set; }

    /// <summary>
    /// 获取或设置驳回原因。
    /// </summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}

/// <summary>
/// 汇款账户验证信息（<see cref="PayApplymentStatus.AccountValidation"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayAccountValidation
{
    /// <summary>
    /// 获取或设置付款户名。
    /// </summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>
    /// 获取或设置付款卡号（结算账户为对私时返回）。
    /// </summary>
    [JsonPropertyName("account_no")]
    public string? AccountNo { get; set; }

    /// <summary>
    /// 获取或设置汇款金额（单位分）。
    /// </summary>
    [JsonPropertyName("pay_amount")]
    public long? PayAmount { get; set; }

    /// <summary>
    /// 获取或设置收款卡号。
    /// </summary>
    [JsonPropertyName("destination_account_number")]
    public string? DestinationAccountNumber { get; set; }

    /// <summary>
    /// 获取或设置收款户名。
    /// </summary>
    [JsonPropertyName("destination_account_name")]
    public string? DestinationAccountName { get; set; }

    /// <summary>
    /// 获取或设置开户银行名称。
    /// </summary>
    [JsonPropertyName("destination_account_bank")]
    public string? DestinationAccountBank { get; set; }

    /// <summary>
    /// 获取或设置省市信息。
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// 获取或设置汇款备注信息。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置汇款截止时间。
    /// </summary>
    [JsonPropertyName("deadline")]
    public string? Deadline { get; set; }
}
