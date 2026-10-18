// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>
/// 资金流水详情字段（官方 <c>Object FundsFlow</c>，由 <c>POST /shop/funds/getorderflow</c> 返回，八支字段官方类型<b>全为 string</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>金额口径与本域其它端点不同</b>：<c>amount</c> / <c>balance</c> 官方为 <c>string</c> 且示例是两位小数
/// （「0.10」「115.12」，元）⇒ 本仓照录 <c>string</c>、<b>不转 <c>decimal</c> 也不换算成分</b>；
/// 发起提现请求侧的 <c>amount</c> 是 number「单位：分」，两处不可互推（守卫 SHOPF5 锁定类型口径）。
/// </para>
/// <para><b><c>balance</c> 语义官方写死为「账户余额(可提现+待结算)」</b> ⇒ 与 getbalance 的两个 number 字段是不同口径的合计，不得用来校验余额接口。</para>
/// <para><b><c>request_no</c> 含义随 <c>busi_type</c> 变化</b>（订单 / 售后 / 提现 ⇒ 订单号 / 售后单号 / 提现单号），官方不提供按业务单号反查的端点。</para>
/// <para><b>敏感面（AGENTS §8）</b>：金额、账户余额与业务单号不得进日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsFlowInfo
{
    /// <summary>记账时间（<c>bookkeeping_time</c>，官方类型 string，示例「2020-08-20 00:08:13」）。</summary>
    [JsonPropertyName("bookkeeping_time")]
    public string? BookkeepingTime { get; set; }

    /// <summary>资金类型（<c>type</c>，官方类型 string，示例「交易」，官方说明「手续费，退款等」）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>收支类型（<c>flow_type</c>，官方类型 string，官方说明「收入，支出」）。</summary>
    [JsonPropertyName("flow_type")]
    public string? FlowType { get; set; }

    /// <summary>收支金额（<c>amount</c>，官方类型 <b>string</b>，示例「0.10」）。</summary>
    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    /// <summary>账户余额（<c>balance</c>，官方类型 <b>string</b>，官方说明「账户余额(可提现+待结算)」）。</summary>
    [JsonPropertyName("balance")]
    public string? Balance { get; set; }

    /// <summary>业务类型（<c>busi_type</c>，官方类型 string，官方说明「订单，售后，提现」）。</summary>
    [JsonPropertyName("busi_type")]
    public string? BusiType { get; set; }

    /// <summary>业务单号（<c>request_no</c>，官方类型 string，含义随 <see cref="BusiType"/> 变化）。</summary>
    [JsonPropertyName("request_no")]
    public string? RequestNo { get; set; }

    /// <summary>流水 id（<c>funds_flow_id</c>，官方类型 string）。注意与详情<b>请求</b>体的 <c>funds_flow_no</c> 同名不同键。</summary>
    [JsonPropertyName("funds_flow_id")]
    public string? FundsFlowId { get; set; }
}
