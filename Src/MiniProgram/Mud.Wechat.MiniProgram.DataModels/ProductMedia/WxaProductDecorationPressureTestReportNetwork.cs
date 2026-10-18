// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 压测报告中的单条网络请求统计（<c>network_list[]</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductDecorationPressureTestReportNetwork
{
    /// <summary>网络请求 path。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>该 path 平均耗时，单位 ms。</summary>
    [JsonPropertyName("aver_time_cost")]
    public long? AverTimeCost { get; set; }

    /// <summary>该 path 最大耗时，单位 ms。</summary>
    [JsonPropertyName("max_time_cost")]
    public long? MaxTimeCost { get; set; }

    /// <summary>该 path 请求次数。</summary>
    [JsonPropertyName("total_request_cnt")]
    public long? TotalRequestCnt { get; set; }

    /// <summary>该 path 请求成功率。</summary>
    [JsonPropertyName("succ_percent")]
    public int? SuccPercent { get; set; }
}
