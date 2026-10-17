// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟带货机构资金流水列表查询请求（官方 <c>headsupplier/funds/flowlist/get</c> 请求体）。</summary>
/// <remarks>
/// <para>
/// 与小店商家侧 <c>merchant/funds/flowlist/get</c>（Funds 域 <c>ChannelsGetFundsFlowListRequest</c>）
/// <b>字段集不同</b>：机构侧无 <c>transaction_id</c>、多 <c>funds_type</c>（资金类型筛选）⇒ 分建 DTO。
/// </para>
/// <para>官方翻页约束：深翻页时（<c>page * page_size &gt;= 5000</c>）<c>next_key</c> 必填；SDK 不做本地拦截，越界由 <c>10024000</c> 表达。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetFundsFlowListRequest
{
    /// <summary>获取或设置页码，从 <c>1</c> 开始（官方 <c>page</c>）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>获取或设置单页流水数，不填默认为 <c>10</c>（官方 <c>page_size</c>）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>获取或设置翻页上下文，写入上一页返回的 <c>next_key</c>（官方 <c>next_key</c>；仅在 <c>page</c> 为上一页加一且 <c>page_size</c> 与上一页相同时生效）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置流水产生开始时间，unix 时间戳（官方 <c>start_time</c>）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置流水产生结束时间，unix 时间戳（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置资金类型（官方 <c>funds_type</c>，枚举见 <see cref="ChannelsLeagueHeadSupplierFundsTypes"/>：1 提现 / 2 分账 / 3 联盟技术服务费 / 4 分账回退）。</summary>
    [JsonPropertyName("funds_type")]
    public int? FundsType { get; set; }
}
