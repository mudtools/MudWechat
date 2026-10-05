// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 日历的公开范围（创建/更新日历请求 calendar.public_range 与获取日历详情响应公共范围，仅公共日历时有效）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleCalendarPublicRange
{
    /// <summary>获取或设置公开的成员列表范围（最多指定 1000 个成员）。</summary>
    [JsonPropertyName("userids")]
    public List<string>? Userids { get; set; }

    /// <summary>获取或设置公开的部门列表范围（最多指定 100 个部门）。</summary>
    [JsonPropertyName("partyids")]
    public List<long>? Partyids { get; set; }
}
