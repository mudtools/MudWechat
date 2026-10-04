// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 设置日志打印级别请求体（<c>/cgi-bin/chatdata/set_log_level</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class SetDataZoneLogLevelRequest
{
    /// <summary>获取或设置应用关联的程序 id（官方必填；指定的程序需与应用有授权关系）。</summary>
    [JsonPropertyName("program_id")]
    public string? ProgramId { get; set; }

    /// <summary>
    /// 获取或设置日志级别（官方必填，uint32）：1 - ERR；2 - INFO；3 - DBG（级别顺序 ERR &lt; INFO &lt; DBG）。
    /// <para>指定后仅会存储不高于该级别的日志（如指定 2，则只存储级别为 1 或 2 的日志）；默认级别为 2。
    /// 程序稳定后可降低级别以节省日志存储空间。</para>
    /// </summary>
    [JsonPropertyName("log_level")]
    public long? LogLevel { get; set; }
}
