// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>
/// 结算账户信息（官方 <c>Object AccountInfo</c>，「修改结算账户」请求体与「查询结算账户」应答共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段集来源裁决</b>：官方「查询结算账户」页只写 <c>account_info</c> 为 Object AccountInfo、
/// <b>未展开子字段表</b>（示例出现 4 支）；本仓取「修改结算账户」页的 AccountInfo 参数表（官方唯一一份定义）
/// 作为两端点共用类型 ⇒ <c>bank_branch_id</c> / <c>bank_name</c> 在查询应答里可缺省，不得当成必有。
/// </para>
/// <para>
/// <b>敏感个人信息（AGENTS §8）</b>：<c>account_number</c> 是完整银行账号，<c>account_bank</c> / <c>bank_name</c>
/// 与开户主体名称同属个人金融信息 ⇒ 禁止写入日志、遥测或异常消息；守卫 SHOPF6 锁定脱敏红线。
/// </para>
/// <para><b>必填集只在请求侧成立</b>：官方参数表标注 <c>bank_account_type</c> / <c>account_bank</c> / <c>bank_address_code</c> / <c>account_number</c> 必填，<c>bank_branch_id</c> / <c>bank_name</c> 选填且「其他银行」时二者需二选一；本类可空标注按应答侧口径统一为可空（禁 <c>required</c>，见 AGENTS §3）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsBankAccountInfo
{
    /// <summary>账户类型（<c>bank_account_type</c>，必填），取值见 <see cref="WxaShopFundsBankAccountTypes"/>；官方要求与开店主体一致。</summary>
    [JsonPropertyName("bank_account_type")]
    public string? BankAccountType { get; set; }

    /// <summary>开户银行（<c>account_bank</c>，必填，如「招商银行」）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户银行省市编码（<c>bank_address_code</c>，必填，示例 <c>100000</c>）。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，选填；开户银行为「其他银行」时与 <see cref="BankName"/> 二选一）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>开户银行全称（<c>bank_name</c>，选填；开户银行为「其他银行」时与 <see cref="BankBranchId"/> 二选一）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>account_number</c>，必填）。<b>个人金融信息，禁入日志、遥测与异常消息。</b></summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }
}
