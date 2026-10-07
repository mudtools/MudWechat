// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 获取消息统计请求体（<c>/cgi-bin/data/get_message_statistics</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class GetMessageStatisticsRequest
{
    /// <summary>
    /// 获取或设置统计起始时间（官方必填；Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("starttime")]
    public long StartTime { get; set; }

    /// <summary>
    /// 获取或设置统计结束时间（官方必填；Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("endtime")]
    public long EndTime { get; set; }

    /// <summary>
    /// 获取或设置统计粒度：day - 按天（默认），week - 按周，month - 按月。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置企业应用的 id（可选；为空时统计全部应用）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public string? AgentId { get; set; }

    /// <summary>
    /// 获取或设置成员 ID 列表（可选；为空时统计全部成员）。
    /// </summary>
    [JsonPropertyName("userids")]
    public List<string>? UserIds { get; set; }
}
