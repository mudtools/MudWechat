// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 日历通知范围成员（创建/更新日历请求 calendar.shares 与获取日历详情响应 shares 元素，通知范围成员最多 2000 人）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleCalendarShare
{
    /// <summary>获取或设置日历通知范围成员的 id（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置成员对日历的权限（不填默认「可查看」）：1-可查看；3-仅查看闲忙状态。</summary>
    [JsonPropertyName("permission")]
    public int? Permission { get; set; }
}
