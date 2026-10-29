// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 结算账户验证信息（进件状态回显）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayAccountValidation
{
    /// <summary>开户名称（个人金融信息，不得进日志）。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>银行账号（个人金融信息，不得进日志）。</summary>
    [JsonPropertyName("account_no")]
    public string? AccountNo { get; set; }

    /// <summary>需打款验证金额，<b>单位分</b>。</summary>
    [JsonPropertyName("pay_amount")]
    public long? PayAmount { get; set; }

    /// <summary>收款账户号（个人金融信息，不得进日志）。</summary>
    [JsonPropertyName("destination_account_number")]
    public string? DestinationAccountNumber { get; set; }

    /// <summary>收款账户名称。</summary>
    [JsonPropertyName("destination_account_name")]
    public string? DestinationAccountName { get; set; }

    /// <summary>收款账户开户行。</summary>
    [JsonPropertyName("destination_account_bank")]
    public string? DestinationAccountBank { get; set; }

    /// <summary>开户城市。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>汇款备注。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>验证截止时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("deadline")]
    public string? Deadline { get; set; }
}
