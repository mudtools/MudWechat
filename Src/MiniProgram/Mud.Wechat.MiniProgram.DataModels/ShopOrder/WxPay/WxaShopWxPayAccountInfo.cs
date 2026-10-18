// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 结算账户信息（<c>/shop/wxpay/apply</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayAccountInfo
{
    /// <summary>账户类型（<b>必填</b>）：企业可填 <c>74</c> 对公账户；个体工商户可填 <c>74</c> 对公或 <c>75</c> 对私账户。</summary>
    [JsonPropertyName("bank_account_type")]
    public string? BankAccountType { get; set; }

    /// <summary>开户银行（<b>必填</b>）：对公银行须取自「查询支持对公业务的银行列表」API。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户名称（<b>必填</b>）：经营者个人银行卡须与身份证姓名一致；对公账户须与营业执照「商户名称」一致（敏感，不得进日志）。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>开户银行省市编码（<b>必填</b>）：至少精确到市。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>开户银行联行号：按「是否需要填写支行」判断；与开户银行全称（含支行）<b>二选一</b>。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>开户银行全称（含支行）：与联行号 <b>二选一</b>。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<b>必填</b>）：数字，长度遵循对公 / 对私卡号长度要求（个人金融信息，不得进日志）。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }
}
