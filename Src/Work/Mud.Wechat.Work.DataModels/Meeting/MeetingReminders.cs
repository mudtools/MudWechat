// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 重复会议相关配置对象（创建/修改预约会议请求与获取会议详情响应 <c>reminders</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 字段为预约会议基础管理（创建/修改/获取详情）与预约会议高级管理（创建/修改/获取详情）两组文档页的并集超集；
/// 自定义重复字段（<c>is_custom_repeat</c> / <c>repeat_day_of_week</c> / <c>repeat_day_of_month</c> /
/// <c>repeat_until_type</c> / <c>repeat_until_count</c>）仅预约会议高级管理与获取会议详情文档页声明。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingReminders
{
    /// <summary>获取或设置是否周期性会议：1 - 周期性；0 - 非周期性（默认）。</summary>
    [JsonPropertyName("is_repeat")]
    public int? IsRepeat { get; set; }

    /// <summary>获取或设置重复类型：0 - 每天；1 - 每周；2 - 每月；7 - 每个工作日（默认 0，仅周期性会议生效）。</summary>
    [JsonPropertyName("repeat_type")]
    public int? RepeatType { get; set; }

    /// <summary>
    /// 获取或设置重复结束时刻（Unix 时间戳，仅周期性会议生效）。
    /// <para>超出最大结束时间或未设置时取最大结束时间：每天/每个工作日/每周最多 200 次，每两周/每月最多 50 次。</para>
    /// </summary>
    [JsonPropertyName("repeat_until")]
    public long? RepeatUntil { get; set; }

    /// <summary>
    /// 获取或设置重复间隔（仅周期性会议生效）。
    /// <para>基础文档页口径：仅 <c>repeat_type</c> 为 1 即每周时支持，且值不能大于 2；高级管理文档页口径：指定为自定义重复时必填，含义随 <c>repeat_type</c> 不同而不同（如 3 + 每周 = 每 3 周重复一次，3 + 每月 = 每 3 个月重复一次）。</para>
    /// </summary>
    [JsonPropertyName("repeat_interval")]
    public int? RepeatInterval { get; set; }

    /// <summary>
    /// 获取或设置是否自定义重复设置：0 - 否；1 - 是（仅 <c>is_repeat</c> 为 1 时生效）。
    /// <para>指定为自定义重复时仅能选择按每日、每周或者每月重复。</para>
    /// </summary>
    [JsonPropertyName("is_custom_repeat")]
    public int? IsCustomRepeat { get; set; }

    /// <summary>
    /// 获取或设置结束重复类型：0 - 按日期结束重复；1 - 按次数结束重复（默认 0）。
    /// <para>
    /// 官方文档陷阱：创建预约会议（预约会议高级管理）参数表将本字段标注为 <c>uint32[]</c>，
    /// 但官方示例与修改/获取会议详情文档页均为单值整数，故本模型按单值整数承载。
    /// </para>
    /// </summary>
    [JsonPropertyName("repeat_until_type")]
    public int? RepeatUntilType { get; set; }

    /// <summary>
    /// 获取或设置周期会议限定次数。
    /// <para>官方限制：每天/每个工作日/每周最大支持 200 场子会议，每月最大支持 50 场子会议；未填写时默认 7 次。</para>
    /// </summary>
    [JsonPropertyName("repeat_until_count")]
    public int? RepeatUntilCount { get; set; }

    /// <summary>
    /// 获取或设置每周周几重复（取值 1~7，分别表示周一至周日；仅自定义重复且重复类型为每周时有效）。
    /// <para>官方限制：自定义按周重复时，会议开始时间对应的那天需要包含在该数组中（如开始时间是周一则需包含 1），未指定时后台会自动补上。</para>
    /// </summary>
    [JsonPropertyName("repeat_day_of_week")]
    public List<int>? RepeatDayOfWeek { get; set; }

    /// <summary>
    /// 获取或设置每月哪几天重复（取值 1~31，分别表示 1~31 号；仅自定义重复且重复类型为每月时有效）。
    /// <para>官方限制：自定义按月重复时，会议开始时间对应的那天需要包含在该数组中，未指定时后台会自动补上。</para>
    /// </summary>
    [JsonPropertyName("repeat_day_of_month")]
    public List<int>? RepeatDayOfMonth { get; set; }

    /// <summary>
    /// 获取或设置会议开始前的提醒时间（相对于会议开始时间的秒数列表，仅支持 0、300、900、3600、86400）。
    /// <para>
    /// 传入其他值时表现为会议开始时提醒；默认不提醒。
    /// 官方文档陷阱：创建预约会议（预约会议高级管理）参数表将本字段标注为 <c>uint32</c> 单值，
    /// 但官方请求/响应示例与修改/获取会议详情文档页均为数组形态，故本模型按数组承载。
    /// </para>
    /// </summary>
    [JsonPropertyName("remind_before")]
    public List<int>? RemindBefore { get; set; }
}
