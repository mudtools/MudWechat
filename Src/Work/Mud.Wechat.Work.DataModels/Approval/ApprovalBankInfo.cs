// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 收款账户银行信息（bank_account.bank）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalBankInfo
{
    /// <summary>
    /// 获取或设置银行名称。
    /// </summary>
    [JsonPropertyName("bank_alias")]
    public string? BankAlias { get; set; }

    /// <summary>
    /// 获取或设置银行代码。
    /// </summary>
    [JsonPropertyName("bank_alias_code")]
    public string? BankAliasCode { get; set; }

    /// <summary>
    /// 获取或设置省份。
    /// </summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>
    /// 获取或设置省份代码。
    /// </summary>
    [JsonPropertyName("province_code")]
    public int? ProvinceCode { get; set; }

    /// <summary>
    /// 获取或设置城市。
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// 获取或设置城市代码。
    /// </summary>
    [JsonPropertyName("city_code")]
    public int? CityCode { get; set; }

    /// <summary>
    /// 获取或设置银行支行。
    /// </summary>
    [JsonPropertyName("bank_branch_name")]
    public string? BankBranchName { get; set; }

    /// <summary>
    /// 获取或设置银行支行联行号。
    /// </summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }
}
