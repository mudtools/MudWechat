// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 时长/假勤时间刻度配置（config.date_range；获取模板详情响应仅返回 type，创建/更新模板请求可额外设置 official_holiday 与 perday_duration）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateDateRangeConfig
{
    /// <summary>
    /// 获取或设置时间刻度：hour-精确到分钟；halfday-上午/下午。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置时长计算口径：0-自然日；1-工作日（仅创建/更新模板请求侧）。
    /// </summary>
    [JsonPropertyName("official_holiday")]
    public int? OfficialHoliday { get; set; }

    /// <summary>
    /// 获取或设置一天的时长（单位为秒），必须大于 0 小于等于 86400（仅创建/更新模板请求侧）。
    /// </summary>
    [JsonPropertyName("perday_duration")]
    public int? PerdayDuration { get; set; }
}
