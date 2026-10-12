// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>结算账户修改明细（官方 <c>account_info</c> 对象，<c>setbankacct</c> 提交形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSettleAccountModify
{
    /// <summary>
    /// 获取或设置账户类型（官方 <c>bank_account_type</c>，必填）——企业类型只能绑定对公账户，
    /// 个体工商户可以绑定对公或者对私账户（见 <see cref="ChannelsBankAccountTypes"/>）。
    /// </summary>
    [JsonPropertyName("bank_account_type")]
    public string BankAccountType { get; set; } = string.Empty;

    /// <summary>获取或设置开户银行（官方 <c>account_bank</c>，必填；经「搜索银行列表」获取）。</summary>
    [JsonPropertyName("account_bank")]
    public string AccountBank { get; set; } = string.Empty;

    /// <summary>获取或设置开户银行省市编码（官方 <c>bank_address_code</c>，必填；经「获取城市列表」获取）。</summary>
    [JsonPropertyName("bank_address_code")]
    public string BankAddressCode { get; set; } = string.Empty;

    /// <summary>获取或设置开户银行联行号（官方 <c>bank_branch_id</c>，选填；经「获取支行联号」获取）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>
    /// 获取或设置开户银行全称（官方 <c>bank_name</c>，选填）——若开户银行为「其他银行」，
    /// 则需二选一填写「开户银行全称（含支行）」或「开户银行联行号」。
    /// </summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账号（官方 <c>account_number</c>，必填）。</summary>
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置开户银行名称前端展示值（官方 <c>account_bank4show</c>，选填；为空时默认用 <c>account_bank</c> 字段）。
    /// </summary>
    [JsonPropertyName("account_bank4show")]
    public string? AccountBank4Show { get; set; }

    /// <summary>
    /// 获取或设置账户名称（官方 <c>account_name</c>，选填；需要修改结算账户名称的时候传入，
    /// 对公账户需要与公司名称一致，对私账户需要与法人姓名一致）。
    /// </summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
}
