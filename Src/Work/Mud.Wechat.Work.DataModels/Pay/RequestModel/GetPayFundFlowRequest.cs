// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 获取资金流水请求体（<c>/cgi-bin/externalpay/get_fund_flow</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayFundFlowRequest
{
    /// <summary>
    /// 获取或设置资金流水记录开始时间（官方必填，单位秒）。
    /// <para>官方业务限制：可拉取不早于 2022 年 12 月 1 日的记录（最长保留 3 年）。</para>
    /// </summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>
    /// 获取或设置资金流水记录结束时间（官方必填，单位秒）。
    /// <para>官方业务限制：起止时间间隔不能超过 31 天；当日流水在次日上午 11 点后生成。</para>
    /// </summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>
    /// 获取或设置商户号 ID；不填则拉取所有商户号的资金流水。
    /// </summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置分页查询游标，由上次调用返回；首次调用可不填。
    /// <para>官方业务限制：无 next_cursor 返回时表示已拉取完全部数据。</para>
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回最大记录数（官方可选，默认 100，最大不超过 200）。
    /// <para>官方业务限制：会过滤操作人不在应用可见范围内的记录，返回条数可能小于 limit。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
