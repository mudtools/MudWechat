// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>获取小程序性能数据请求体（<c>POST /wxa/business/performance/boot</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>data-analysis/others/api_getperformancedata.html</c>。</para>
/// <para>
/// <b>时间粒度（官方原文）</b>：<c>time</c> 为开始时间，格式 <c>yyyymmddhh</c>，<b>精确到小时</b>
/// （与 datacube 各端点的 <c>yyyymmdd</c> 粒度不同，勿混用）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPerformanceBootRequest
{
    /// <summary>开始时间（<c>time</c>，必填；格式 <c>yyyymmddhh</c>，精确到小时）。</summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    /// <summary>模块类型（<c>module</c>，必填）：<c>1</c> 启动性能 / <c>2</c> 运行性能。</summary>
    [JsonPropertyName("module")]
    public long? Module { get; set; }

    /// <summary>模块扩展参数（<c>extinfo</c>，选填；标准 JSON 序列化字符串，具体字段以官方页面为准）。</summary>
    [JsonPropertyName("extinfo")]
    public string? ExtInfo { get; set; }
}
