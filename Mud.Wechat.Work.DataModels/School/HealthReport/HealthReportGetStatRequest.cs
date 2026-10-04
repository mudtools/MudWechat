// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.HealthReport;

/// <summary>
/// 获取健康上报使用统计请求体（<c>/cgi-bin/health/get_health_report_stat</c>，健康上报域）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HealthReport")]
public class HealthReportGetStatRequest
{
    /// <summary>
    /// 获取或设置具体某天的使用统计（如 <c>2020-03-27</c>）。
    /// <para>官方业务限制：最长支持获取 30 天前数据。</para>
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;
}
