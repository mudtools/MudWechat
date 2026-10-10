// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则请求模型特殊工作日（<c>group.spe_workdays</c> 元素，个数不超过 366；自定义排班/自由签到不可设置）。
/// </summary>
/// <remarks>
/// <para>特殊工作日不可设置 earliest_work_sec/latest_work_sec/earliest_off_work_sec/latest_off_work_sec 字段（301094）；仅允许设置未来日期。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRuleSpeWorkday
{
    /// <summary>获取或设置特殊日期时间戳（type 为 2 时表示开始时间；type 为 0 时需要设置为某天零点的时间戳，且大于 2000 年 1 月 1 日零点）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>获取或设置特殊日期备注（官方必填；字节长度不超过 40，不可为空）。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>获取或设置类型：0 - 时间点；1 - 时间段；2 - 两周重复。type=0 表示单个日期（填 timestamp）；type=1 表示时间段（填 begtime 和 endtime，闭区间）；type=2 表示按两周重复（填 timestamp 和 endtime）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置开始时间（type 为 1 时有效；需要是某天零点的时间戳，且大于 2000 年 1 月 1 日零点）。</summary>
    [JsonPropertyName("begtime")]
    public long? Begtime { get; set; }

    /// <summary>获取或设置结束时间（type 为 1、2 时有效；某天零点时间戳，两端为闭区间；type 为 1/2 时时间范围不可超过一年）。</summary>
    [JsonPropertyName("endtime")]
    public long? Endtime { get; set; }

    /// <summary>获取或设置特殊工作日的上下班打卡时间配置（官方必填；上下班时间需要为整分钟；上班时间不可早于当日 04 点、不可晚于 18:00，下班时间不可早于当日 04 点、不超过次日 03:00）。</summary>
    [JsonPropertyName("checkintime")]
    public List<CheckinRuleCheckintime>? Checkintime { get; set; }
}
