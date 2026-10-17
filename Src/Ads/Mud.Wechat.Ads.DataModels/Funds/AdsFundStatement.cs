// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Funds;

/// <summary>
/// 资金流水明细条目（官方 <c>fund_statements_detailed/get</c> 的 <c>data.list[]</c> 元素，
/// 2026-10-17 逐表核验，9 字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundStatementInfo
{
    /// <summary>推广帐号 id（官方 <c>account_id</c>，<c>integer</c>；含代理商和账户 id）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>资金账户类型（官方 <c>fund_type</c>，<c>enum</c>；请求侧 36 支可选值见
    /// <c>IWechatAdsFundsService.GetFundStatementsDetailedAsync</c> 的 XML）。</summary>
    [JsonPropertyName("fund_type")]
    public string? FundType { get; set; }

    /// <summary>余额（官方 <c>balance</c>，<c>integer</c>，单位为分 —— 本条流水后的余额快照）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>交易时间（官方 <c>time</c>，<c>integer</c>，时间戳）。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>调用方订单号（官方 <c>external_bill_no</c>；无关联订单的流水缺省）。</summary>
    [JsonPropertyName("external_bill_no")]
    public string? ExternalBillNo { get; set; }

    /// <summary>交易类型（官方 <c>trade_type_ext</c>，<c>enum</c>；枚举详情未逐项核验）。</summary>
    [JsonPropertyName("trade_type_ext")]
    public string? TradeTypeExt { get; set; }

    /// <summary>金额（官方 <c>amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>描述信息（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>记录主键（官方 <c>primary_key</c>；下一页请求以「上一页最后一条的该值」续拉）。</summary>
    [JsonPropertyName("primary_key")]
    public string? PrimaryKey { get; set; }
}

/// <summary><c>fund_statements_detailed/get</c> 的 <c>data</c> 载荷（列表 + 分页）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundStatementListData
{
    /// <summary>流水列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsFundStatementInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/fund_statements_detailed/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundStatementsDetailedGetResponse : AdsResponse<AdsFundStatementListData>
{
}
