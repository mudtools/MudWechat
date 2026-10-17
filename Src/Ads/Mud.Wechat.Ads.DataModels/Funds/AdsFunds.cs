// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Funds;

/// <summary>
/// <c>GET /v3.0/funds/get</c> 的 <c>data.list[]</c> 元素（2026-10-17 官方页逐表核验，6 字段）。
/// </summary>
/// <remarks>
/// 金额字段单位均为<b>分</b>；<c>fund_type</c> / <c>fund_status</c> 为 <c>enum</c>，
/// 本页枚举详情未逐项展开 ⇒ 以 <see cref="string"/> 承载、不猜测可选值。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundInfo
{
    /// <summary>资金账户类型（官方 <c>fund_type</c>，<c>enum</c>；本页枚举详情未逐项核验）。</summary>
    [JsonPropertyName("fund_type")]
    public string? FundType { get; set; }

    /// <summary>余额（官方 <c>balance</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>锁定金额（官方 <c>bill_deposit_amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("bill_deposit_amount")]
    public long? BillDepositAmount { get; set; }

    /// <summary>资金账户状态（官方 <c>fund_status</c>，<c>enum</c>；本页枚举详情未逐项核验）。</summary>
    [JsonPropertyName("fund_status")]
    public string? FundStatus { get; set; }

    /// <summary>今日实时消耗（官方 <c>realtime_cost</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("realtime_cost")]
    public long? RealtimeCost { get; set; }

    /// <summary>有效资金列表（官方 <c>effect_funds</c>，<c>struct[]</c>，按有效期展开）。</summary>
    [JsonPropertyName("effect_funds")]
    public List<AdsEffectFund>? EffectFunds { get; set; }
}

/// <summary>有效资金条目（官方 <c>effect_funds[]</c> 元素，3 字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsEffectFund
{
    /// <summary>生效日期（官方 <c>effect_date</c>；格式 <c>YYYY-MM-DD</c>，字符串承载）。</summary>
    [JsonPropertyName("effect_date")]
    public string? EffectDate { get; set; }

    /// <summary>失效日期（官方 <c>expire_date</c>；格式 <c>YYYY-MM-DD</c>，字符串承载）。</summary>
    [JsonPropertyName("expire_date")]
    public string? ExpireDate { get; set; }

    /// <summary>金额（官方 <c>amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary><c>funds/get</c> 的 <c>data</c> 载荷（官方只有 <c>list</c>，无分页元信息）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundListData
{
    /// <summary>资金账户列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsFundInfo>? List { get; set; }
}

/// <summary><c>GET /v3.0/funds/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundsGetResponse : AdsResponse<AdsFundListData>
{
}
