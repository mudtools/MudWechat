// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Apply4SubMerchant;

/// <summary>
/// 修改特约商户结算账户（<c>POST /v3/apply4sub/sub_merchants/{sub_mchid}/modify-settlement</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761102"/>（普通服务商 / 平台商户文档树各一页
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012761138"/>；2026-10-14 逐字段核验）。
/// </para>
/// <para>
/// <b>异步审核</b>：应答只有 <see cref="Apply4SubModifySettlementResponse.ApplicationNo"/>，
/// 生效与否须经「查询结算账户修改申请状态」轮询（<see cref="Apply4SubSettlementAuditResults"/>）⇒
/// <b>不得把拿到申请单号当成结算账户已改</b>。
/// </para>
/// <para>
/// <b>频率限制（官方原文）</b>：接口 <b>20 次 / 分钟</b>，且<b>同一特约商户每日仅能提交 5 次</b>修改申请；
/// 超限返回 <c>429 FREQENCY_LIMIT</c>（官方拼写少一个 U，见 <see cref="Apply4SubMerchantErrorCodes.FrequencyLimit"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubModifySettlementRequest
{
    /// <summary>账户类型（<c>account_type</c>，必填）：取值见 <see cref="Apply4SubSettlementAccountTypes"/>（<b>非</b>进件侧的 <c>BANK_ACCOUNT_TYPE_*</c>）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>开户银行（<c>account_bank</c>，必填 string(128)）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户银行全称（含支行）（<c>bank_name</c>，选填 string(128)）：与 <see cref="BankBranchId"/> 二选一。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，选填 string(128)）：与 <see cref="BankName"/> 二选一。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>银行账号（<c>account_number</c>，必填 string(1024)，<b>需加密</b>）：数字本身不超过 32 字符，密文按加密后长度。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>开户名称（<c>account_name</c>，选填，<b>需加密</b>）：仅可修改为与主体同名或经营者本人名称，不一致时官方以 <c>INVALID_REQUEST</c> 驳回。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
}

/// <summary>修改结算账户的应答（<b>异步受理</b>，仅返回申请单号）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubModifySettlementResponse : WechatPayResponse
{
    /// <summary>修改结算账户申请单号（<c>application_no</c>，必填 string(64)）：查询审核状态的唯一标识。</summary>
    [JsonPropertyName("application_no")]
    public string? ApplicationNo { get; set; }
}

/// <summary>
/// 特约商户当前结算账户（<c>GET /v3/apply4sub/sub_merchants/{sub_mchid}/settlement</c> 的应答）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761113"/>（另一文档树同页
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012761142"/>；2026-10-14 逐字段核验；频率限制 <b>100 次 / 秒</b>）。
/// <b>本应答无 <c>account_name</c> 与 <c>verify_finish_time</c></b>（与「查询修改申请状态」应答字段表不同，故是两个类型而非一个）。
/// <c>account_number</c> 按请求的 <c>account_number_rule</c> 掩码返回，<b>永不</b>返回明文账号。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubSettlementResponse : WechatPayResponse
{
    /// <summary>账户类型（<c>account_type</c>，必填）：取值见 <see cref="Apply4SubSettlementAccountTypes"/>。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>开户银行全称（<c>account_bank</c>，必填 string(128)）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户银行全称（含支行）（<c>bank_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>银行账号（<c>account_number</c>，必填 string(128)）：按 <c>account_number_rule</c> 掩码展示。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>账户验证结果（<c>verify_result</c>，必填）：取值见 <see cref="Apply4SubSettlementVerifyResults"/>。</summary>
    [JsonPropertyName("verify_result")]
    public string? VerifyResult { get; set; }

    /// <summary>验证失败原因（<c>verify_fail_reason</c>，选填 string(1024)）：验证成功时为空。</summary>
    [JsonPropertyName("verify_fail_reason")]
    public string? VerifyFailReason { get; set; }
}

/// <summary>
/// 结算账户<b>修改申请</b>的状态应答（<c>GET /v3/apply4sub/sub_merchants/{sub_mchid}/application/{application_no}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761169"/>（另一文档树同页
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012761120"/>；2026-10-14 逐字段核验）。
/// </para>
/// <para>
/// <b>与 <see cref="Apply4SubSettlementResponse"/> 的差异是刻意的</b>：本应答多 <c>account_name</c> 与
/// <c>verify_finish_time</c>，且 <c>verify_result</c> 取 <c>AUDIT_*</c> 三值（申请审核态）而非 <c>VERIFY_*</c>
/// （账户验证态）—— 同名字段两套语义，<b>不得</b>合并成一个类型。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubSettlementModifyApplicationResponse : WechatPayResponse
{
    /// <summary>开户名称（<c>account_name</c>，必填 string(1024)）：掩码显示。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>账户类型（<c>account_type</c>，必填）：取值见 <see cref="Apply4SubSettlementAccountTypes"/>。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>开户银行全称（<c>account_bank</c>，必填 string(128)）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户银行全称（含支行）（<c>bank_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>银行账号（<c>account_number</c>，必填 string(128)）：按请求的 <c>account_number_rule</c> 掩码展示。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>修改申请审核状态（<c>verify_result</c>，必填）：取值见 <see cref="Apply4SubSettlementAuditResults"/>。</summary>
    [JsonPropertyName("verify_result")]
    public string? VerifyResult { get; set; }

    /// <summary>审核驳回原因（<c>verify_fail_reason</c>，选填）：审核成功时为空，驳回时为具体原因。</summary>
    [JsonPropertyName("verify_fail_reason")]
    public string? VerifyFailReason { get; set; }

    /// <summary>审核结果更新时间（<c>verify_finish_time</c>，选填）：RFC3339 <c>yyyy-MM-ddTHH:mm:ss+TIMEZONE</c>。</summary>
    [JsonPropertyName("verify_finish_time")]
    public string? VerifyFinishTime { get; set; }
}
