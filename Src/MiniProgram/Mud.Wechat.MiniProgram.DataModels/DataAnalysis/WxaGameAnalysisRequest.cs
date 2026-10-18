// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>
/// 小游戏数据分析请求体（<c>POST /datacube/getgameanalysisdata</c>，2026-10-18 逐表核验）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/data-analysis/api_analysis.getgameanalysisdata.html"/>。</para>
/// <para><b>指标与维度由官方指标表定义</b>：<c>metric</c> 的取值、<c>filter_list[].dimension</c> /
/// <c>group_list[]</c> 的合法维度均来自官方「小游戏数据指标」表 ⇒ SDK <b>不做本地枚举与校验</b>，
/// 非法值由官方错误码表达（同本域「日期越界不本地校验」纪律）。</para>
/// <para><b>时间戳为秒级</b>：<c>start_time</c> / <c>end_time</c> 是 Unix 时间戳（官方示例为 10 位）；
/// <c>granularity</c> 为官方定义的时间粒度枚举值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaGameAnalysisRequest
{
    /// <summary>指标 ID（<c>metric</c>，必填，取值见官方小游戏数据指标表）。</summary>
    [JsonPropertyName("metric")]
    public string? Metric { get; set; }

    /// <summary>时间粒度（<c>granularity</c>，必填，官方定义的时间粒度枚举值）。</summary>
    [JsonPropertyName("granularity")]
    public int? Granularity { get; set; }

    /// <summary>开始时间（<c>start_time</c>，必填，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTimestamp { get; set; }

    /// <summary>结束时间（<c>end_time</c>，必填，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTimestamp { get; set; }

    /// <summary>维度筛选列表（<c>filter_list</c>，选填），见 <see cref="WxaGameAnalysisFilter"/>。</summary>
    [JsonPropertyName("filter_list")]
    public List<WxaGameAnalysisFilter>? FilterList { get; set; }

    /// <summary>维度分组列表（<c>group_list</c>，选填；元素为官方维度 ID）。</summary>
    [JsonPropertyName("group_list")]
    public List<string>? GroupList { get; set; }
}
