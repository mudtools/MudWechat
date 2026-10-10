// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 创建/更新日历请求的日历信息（<c>/cgi-bin/oa/calendar/add</c> 与 <c>/cgi-bin/oa/calendar/update</c> 共用的扁平 calendar 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 两端点参数表差异以可空性承载：<c>cal_id</c> 仅更新日历必填（创建日历不适用）；
/// <see cref="SetAsDefault"/>、<see cref="IsPublic"/>、<see cref="IsCorpCalendar"/> 仅创建日历可传
///（<c>is_public</c> 与 <c>is_corp_calendar</c> 属性不可更新，更新日历官方参数表不含三者）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleCalendar
{
    /// <summary>获取或设置日历 ID（仅更新日历必填；创建日历不适用）。</summary>
    [JsonPropertyName("cal_id")]
    public string? CalId { get; set; }

    /// <summary>获取或设置日历管理员 userid 列表（须在通知范围成员中，最多指定 3 人）。</summary>
    [JsonPropertyName("admins")]
    public List<string>? Admins { get; set; }

    /// <summary>获取或设置是否设为该 access_token 对应应用的默认日历：0-否；1-是（默认 0）；仅创建日历可传，第三方应用不支持使用该参数。</summary>
    [JsonPropertyName("set_as_default")]
    public int? SetAsDefault { get; set; }

    /// <summary>获取或设置日历标题（官方必填；1 ~ 128 字符）。</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>获取或设置日历在终端上显示的颜色（官方必填；RGB 颜色编码 16 进制表示，如 "#0000FF" 表示纯蓝色）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>获取或设置日历描述（0 ~ 512 字符）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置是否公共日历：0-否；1-是（仅创建日历可传；该属性不可更新；每人最多可创建或订阅 100 个公共日历）。</summary>
    [JsonPropertyName("is_public")]
    public int? IsPublic { get; set; }

    /// <summary>获取或设置日历的公开范围（仅公共日历时有效；公开成员最多 1000 个、部门最多 100 个）。</summary>
    [JsonPropertyName("public_range")]
    public ScheduleCalendarPublicRange? PublicRange { get; set; }

    /// <summary>
    /// 获取或设置是否全员日历：0-否；1-是（仅创建日历可传；该属性不可更新）。
    /// </summary>
    /// <remarks>
    /// <para>每个企业最多可创建 20 个全员日历；全员日历也是公共日历的一种，需要指定 public_range；
    /// 全员日历不支持指定颜色、默认日历、只读权限。</para>
    /// </remarks>
    [JsonPropertyName("is_corp_calendar")]
    public int? IsCorpCalendar { get; set; }

    /// <summary>获取或设置日历通知范围成员列表（最多 2000 人）。</summary>
    [JsonPropertyName("shares")]
    public List<ScheduleCalendarShare>? Shares { get; set; }
}
