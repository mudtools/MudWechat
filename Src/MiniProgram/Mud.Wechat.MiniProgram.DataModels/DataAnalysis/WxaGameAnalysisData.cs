// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>
/// 小游戏数据分析单条记录（<c>data_list[]</c> 元素，2026-10-18 官方页逐表核验）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaGameAnalysisData
{
    /// <summary>数据时间戳（<c>timestamp</c>，Unix 秒级）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>数据时间标签（<c>time_label</c>，官方按粒度给出的可读时间）。</summary>
    [JsonPropertyName("time_label")]
    public string? TimeLabel { get; set; }

    /// <summary>维度取值分布（<c>group_dimension_list</c>），见 <see cref="WxaGameAnalysisGroupDimension"/>。</summary>
    [JsonPropertyName("group_dimension_list")]
    public List<WxaGameAnalysisGroupDimension>? GroupDimensionList { get; set; }

    /// <summary>指标数值（<c>metric_value</c>；官方可能返回小数 ⇒ 用 <see cref="decimal"/> 承载）。</summary>
    [JsonPropertyName("metric_value")]
    public decimal? MetricValue { get; set; }
}
