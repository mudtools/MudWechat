// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>
/// 提现记录详情字段（官方 <c>Object WithDrawInfo</c>，由 <c>POST /shop/funds/getwithdrawdetail</c> 返回）。
/// </summary>
/// <remarks>
/// <para><b>金额单位官方本页未标注</b>（只写「金额」+ number，示例 7）⇒ 照录 <c>long</c> 不换算，与发起提现请求侧「单位：分」不可互推。</para>
/// <para><b><c>reason</c> 只在失败态有值</b>（官方说明「失败原因」），终态判定一律以 <c>status</c>（见 <see cref="WxaShopFundsWithdrawStatuses"/>）为准。</para>
/// <para><b>敏感面（AGENTS §8）</b>：<c>bank_num</c>（银行账户）、<c>bank_name</c>、金额与备注类字段禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsWithdrawInfo
{
    /// <summary>提现金额（<c>amount</c>，官方类型 number；本页未标单位）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>创建时间（<c>create_time</c>，官方类型 number，示例为秒级时间戳 1590733930）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>更新时间（<c>update_time</c>，官方类型 number，示例为秒级时间戳 1590773555）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>失败原因（<c>reason</c>，官方类型 string，成功态可为空）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>提现备注（<c>remark</c>，官方类型 string）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>银行附言（<c>bank_memo</c>，官方类型 string）。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>银行名称（<c>bank_name</c>，官方类型 string）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账户（<c>bank_num</c>，官方类型 string）。<b>个人金融信息，禁入日志、遥测与异常消息。</b></summary>
    [JsonPropertyName("bank_num")]
    public string? BankNum { get; set; }

    /// <summary>提现状态（<c>status</c>，官方类型 string），取值见 <see cref="WxaShopFundsWithdrawStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
