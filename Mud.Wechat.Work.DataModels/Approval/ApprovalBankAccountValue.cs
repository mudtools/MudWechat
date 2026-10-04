// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 收款账户控件值（BankAccount）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalBankAccountValue
{
    /// <summary>
    /// 获取或设置账户类型：1-对公账户；2-个人账户。
    /// </summary>
    [JsonPropertyName("account_type")]
    public int? AccountType { get; set; }

    /// <summary>
    /// 获取或设置账户名。
    /// </summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>
    /// 获取或设置账号。
    /// </summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>
    /// 获取或设置备注。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置银行信息。
    /// </summary>
    [JsonPropertyName("bank")]
    public ApprovalBankInfo? Bank { get; set; }
}
