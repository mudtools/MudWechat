// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡报表假勤统计信息（日报/月报 <c>sp_items</c> 共用元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinSpItem
{
    /// <summary>获取或设置类型：1 - 请假；2 - 补卡；3 - 出差；4 - 外出；15 - 审批打卡；100 - 外勤。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置具体请假类型 id（当 type 为 1 请假时有效，可通过审批相关接口获取假期详情）。</summary>
    [JsonPropertyName("vacation_id")]
    public int? VacationId { get; set; }

    /// <summary>获取或设置假勤次数（日报为当日假勤次数；月报为统计周期内之和）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>获取或设置假勤时长（秒；时长单位为天直接除以 86400 即为天数，单位为小时直接除以 3600 即为小时数）。</summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>获取或设置时长单位：0 - 按天；1 - 按小时。</summary>
    [JsonPropertyName("time_type")]
    public int? TimeType { get; set; }

    /// <summary>获取或设置统计项名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
