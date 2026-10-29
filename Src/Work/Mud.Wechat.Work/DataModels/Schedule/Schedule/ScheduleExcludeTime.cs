// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 重复日程排除的日期（获取日程详情响应 reminders.exclude_time_list 元素；对重复日程修改/删除特定一天或多天时，原日程将排除对应日期）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleExcludeTime
{
    /// <summary>获取或设置不包含的日期时间戳（Unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }
}
