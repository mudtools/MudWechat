// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 获取单位居民上报数据统计响应体（<c>/cgi-bin/report/resident/get_corp_status</c>，政民沟通居民上报族）。
/// <para>与巡查上报单位统计的差异：本端点为「待受理 pending」与「累计受理 total_accepted」，
/// 无巡查上报的「待分配 to_be_assigned」。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovResidentGetCorpStatusResponse : WechatWorkResponse
{
    /// <summary>获取或设置办理中数量。</summary>
    [JsonPropertyName("processing")]
    public int? Processing { get; set; }

    /// <summary>获取或设置今日上报数量。</summary>
    [JsonPropertyName("added_today")]
    public int? AddedToday { get; set; }

    /// <summary>获取或设置今日办结数量。</summary>
    [JsonPropertyName("solved_today")]
    public int? SolvedToday { get; set; }

    /// <summary>获取或设置待受理数量。</summary>
    [JsonPropertyName("pending")]
    public int? Pending { get; set; }

    /// <summary>获取或设置累计上报数量。</summary>
    [JsonPropertyName("total_case")]
    public int? TotalCase { get; set; }

    /// <summary>获取或设置累计受理数量。</summary>
    [JsonPropertyName("total_accepted")]
    public int? TotalAccepted { get; set; }

    /// <summary>获取或设置累计办结数量。</summary>
    [JsonPropertyName("total_solved")]
    public int? TotalSolved { get; set; }
}
