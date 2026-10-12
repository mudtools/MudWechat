// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>获取资金流水列表（<c>getfundsflowlist</c>）请求体。</summary>
/// <remarks>
/// 官方契约：当日资金流水出账时间为<b>次日 16 点后</b>，先用后付订单需用户确认收货自动付款后才会产生资金流水；
/// <c>end_time</c> 受出账时间约束（16 点前最大为前 1 天 0 点，16 点后最大为当天 0 点，超上限按上限查询）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetFundsFlowListRequest
{
    /// <summary>获取或设置页码（官方 <c>page</c>，选填，从 1 开始）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>获取或设置页大小（官方 <c>page_size</c>，选填，不填默认为 10）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>
    /// 获取或设置分页参数（官方 <c>next_key</c>，选填）——翻页时写入上一页返回的 <c>next_key</c>
    /// （page 为上一页加一，并且 page_size 与上一页相同的时候才生效）；<c>page * page_size &gt;= 10000</c> 时必填。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置流水产生的开始时间（官方 <c>start_time</c>，选填，unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置流水产生的结束时间（官方 <c>end_time</c>，选填，unix 时间戳）——受当日 16 点出账时间约束
    /// （16 点前最大为前 1 天 0 点 0 分 0 秒，16 点后最大为当天 0 点 0 分 0 秒；超过最大值将按最大值进行查询）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置支付单号（官方 <c>transaction_id</c>，选填）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}
