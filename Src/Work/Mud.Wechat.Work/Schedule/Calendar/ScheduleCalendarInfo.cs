// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 获取日历详情响应的日历信息（响应 calendar_list 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档对日历管理员的参数表字段名标注为 <c>admins</c>，但三类应用文档页的返回示例均作 <c>adminis</c>，
/// 本模型以示例为准承载 <c>adminis</c>（照抄勿「顺手修正」）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleCalendarInfo
{
    /// <summary>获取或设置日历 ID。</summary>
    [JsonPropertyName("cal_id")]
    public string? CalId { get; set; }

    /// <summary>获取或设置日历的管理员 userid 列表（官方返回示例字段名为 adminis，参数表作 admins，以示例为准）。</summary>
    [JsonPropertyName("adminis")]
    public List<string>? Adminis { get; set; }

    /// <summary>获取或设置日历标题（1 ~ 128 字符）。</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>获取或设置日历颜色（RGB 颜色编码 16 进制表示，如 "#0000FF" 表示纯蓝色）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>获取或设置日历描述（0 ~ 512 字符）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置日历通知范围成员列表（最多 2000 人）。</summary>
    [JsonPropertyName("shares")]
    public List<ScheduleCalendarShare>? Shares { get; set; }

    /// <summary>获取或设置是否公共日历：0-否；1-是。</summary>
    [JsonPropertyName("is_public")]
    public int? IsPublic { get; set; }

    /// <summary>获取或设置公开范围（仅当是公共日历时有效）。</summary>
    [JsonPropertyName("public_range")]
    public ScheduleCalendarPublicRange? PublicRange { get; set; }

    /// <summary>获取或设置是否全员日历：0-否；1-是。</summary>
    [JsonPropertyName("is_corp_calendar")]
    public int? IsCorpCalendar { get; set; }
}
