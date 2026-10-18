// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductFunds;

/// <summary>结算账户信息（setbankaccount 请求体的 <c>account_info</c> 嵌套对象）。</summary>
/// <remarks>
/// <para>官方文档：<c>set_bank_account.html</c>。必填集：<c>bank_account_type</c> / <c>account_bank</c> /
/// <c>bank_address_code</c> / <c>account_number</c>；「其他银行」场景须 <c>bank_branch_id</c>（联行号）或
/// <c>bank_name</c>（含支行全称）二选一。</para>
/// <para><b>敏感面</b>：<c>account_number</c>（银行账号）为个人金融信息、<c>account_bank</c>（开户名/开户行）与
/// <c>bank_account_type</c>（账户类型：ACCOUNT_TYPE_BUSINESS 对公 / ACCOUNT_TYPE_PRIVATE 经营者个人银行卡）
/// 连同本对象整体<b>禁入日志、遥测或异常消息</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductFunds")]
public class WxaProductFundsBankAccountInfo
{
    /// <summary>账户类型（<c>bank_account_type</c>，string，<b>必填</b>）：ACCOUNT_TYPE_BUSINESS 对公 / ACCOUNT_TYPE_PRIVATE 经营者个人银行卡。敏感。</summary>
    [JsonPropertyName("bank_account_type")]
    public string? BankAccountType { get; set; }

    /// <summary>开户银行（<c>account_bank</c>，string，<b>必填</b>，取值为官方「开户银行对照表」）。敏感。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户银行省市编码（<c>bank_address_code</c>，string，<b>必填</b>，至少精确到市）。敏感。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，string，选填；「其他银行」时与 <see cref="BankName"/> 二选一）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>开户银行全称（含支行）（<c>bank_name</c>，string，选填；「其他银行」时与 <see cref="BankBranchId"/> 二选一）。敏感。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>account_number</c>，string，<b>必填</b>）。<b>敏感个人金融信息</b>，禁入日志、遥测或异常消息。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>开户银行名称前端展示值（<c>account_bank4show</c>，string，选填；为空时默认取 <see cref="AccountBank"/>）。字段名照官方原文（含数字 <c>4</c>）。</summary>
    [JsonPropertyName("account_bank4show")]
    public string? AccountBank4Show { get; set; }
}
