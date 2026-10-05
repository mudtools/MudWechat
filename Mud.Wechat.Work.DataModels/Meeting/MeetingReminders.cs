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

    /// <summary>获取或设置重复间隔（仅 <c>repeat_type</c> 为 1 即每周时支持，且值不能大于 2）。</summary>
    [JsonPropertyName("repeat_interval")]
    public int? RepeatInterval { get; set; }

    /// <summary>
    /// 获取或设置会议开始前的提醒时间（相对于会议开始时间的秒数列表，仅支持 0、300、900、3600、86400）。
    /// <para>传入其他值时表现为会议开始时提醒；默认不提醒。</para>
    /// </summary>
    [JsonPropertyName("remind_before")]
    public List<int>? RemindBefore { get; set; }
}
