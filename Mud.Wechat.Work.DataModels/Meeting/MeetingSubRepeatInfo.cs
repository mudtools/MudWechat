// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 周期性会议分段信息对象（获取会议详情响应 <c>sub_repeat_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingSubRepeatInfo
{
    /// <summary>获取或设置分段 ID。</summary>
    [JsonPropertyName("repeat_id")]
    public string? RepeatId { get; set; }

    /// <summary>获取或设置重复类型：0 - 每日；1 - 每周；2 - 每月；7 - 工作日。</summary>
    [JsonPropertyName("repeat_type")]
    public int? RepeatType { get; set; }

    /// <summary>获取或设置是否自定义重复设置：0 - 否；1 - 是。</summary>
    [JsonPropertyName("is_custom_repeat")]
    public int? IsCustomRepeat { get; set; }

    /// <summary>
    /// 获取或设置周期间隔（该字段随 <see cref="RepeatType"/> 不同而含义不同，
    /// 如 3 + 每周 = 每 3 周重复一次，3 + 每月 = 每 3 个月重复一次）。
    /// </summary>
    [JsonPropertyName("repeat_interval")]
    public int? RepeatInterval { get; set; }

    /// <summary>获取或设置每周周几重复（取值 1~7，分别表示周一至周日）。</summary>
    [JsonPropertyName("repeat_day_of_week")]
    public List<int>? RepeatDayOfWeek { get; set; }

    /// <summary>获取或设置每月哪几天重复（取值 1~31，分别表示 1~31 号）。</summary>
    [JsonPropertyName("repeat_day_of_month")]
    public List<int>? RepeatDayOfMonth { get; set; }

    /// <summary>获取或设置周期性会议结束类型：0 - 按日期结束重复；1 - 按次数结束重复。</summary>
    [JsonPropertyName("repeat_until_type")]
    public int? RepeatUntilType { get; set; }

    /// <summary>获取或设置分段的重复截止次数（<see cref="RepeatUntilType"/> = 1 时返回）。</summary>
    [JsonPropertyName("repeat_until_count")]
    public int? RepeatUntilCount { get; set; }

    /// <summary>获取或设置分段的重复截止时间（<see cref="RepeatUntilType"/> = 0 时返回）。</summary>
    [JsonPropertyName("repeat_until")]
    public long? RepeatUntil { get; set; }
}
