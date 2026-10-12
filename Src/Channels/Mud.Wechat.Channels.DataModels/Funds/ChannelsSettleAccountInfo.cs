// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>结算账户信息（官方 <c>account_info</c> 对象，<c>getbankacct</c> 返回形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSettleAccountInfo
{
    /// <summary>获取或设置账户类型（官方 <c>bank_account_type</c>，见 <see cref="ChannelsBankAccountTypes"/>）。</summary>
    [JsonPropertyName("bank_account_type")]
    public string? BankAccountType { get; set; }

    /// <summary>获取或设置开户银行（官方 <c>account_bank</c>）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>获取或设置开户银行省市编码（官方 <c>bank_address_code</c>）。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>获取或设置开户银行联行号（官方 <c>bank_branch_id</c>）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>获取或设置开户银行全称（官方 <c>bank_name</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账号（官方 <c>account_number</c>）。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>获取或设置账户名称（官方 <c>account_name</c>）。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
}
