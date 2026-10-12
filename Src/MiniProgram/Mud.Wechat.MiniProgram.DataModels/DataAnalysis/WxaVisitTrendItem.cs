// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>趋势数据条目（<c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitTrendItem
{
    /// <summary>日期（<c>ref_date</c>，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>打开次数（<c>session_cnt</c>）。</summary>
    [JsonPropertyName("session_cnt")]
    public long? SessionCount { get; set; }

    /// <summary>访问次数（<c>visit_pv</c>）。</summary>
    [JsonPropertyName("visit_pv")]
    public long? VisitPv { get; set; }

    /// <summary>访问人数（<c>visit_uv</c>）。</summary>
    [JsonPropertyName("visit_uv")]
    public long? VisitUv { get; set; }

    /// <summary>新用户数（<c>visit_uv_new</c>）。</summary>
    [JsonPropertyName("visit_uv_new")]
    public long? VisitUvNew { get; set; }

    /// <summary>人均停留时长（<c>stay_time_uv</c>，<b>浮点</b>，单位秒）。</summary>
    [JsonPropertyName("stay_time_uv")]
    public double? StayTimeUv { get; set; }

    /// <summary>次均停留时长（<c>stay_time_session</c>，<b>浮点</b>，单位秒）。</summary>
    [JsonPropertyName("stay_time_session")]
    public double? StayTimeSession { get; set; }

    /// <summary>平均访问深度（<c>visit_depth</c>，<b>浮点</b>）。</summary>
    [JsonPropertyName("visit_depth")]
    public double? VisitDepth { get; set; }
}
