// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 创建日历请求体（<c>/cgi-bin/oa/calendar/add</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：每个人最多可创建或订阅 100 个公共日历；每个企业最多可创建 20 个全员日历；
/// 全员日历也是公共日历的一种，需指定 public_range，且不支持指定颜色、默认日历、只读权限；
/// <c>is_public</c> 与 <c>is_corp_calendar</c> 属性不可更新。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class AddScheduleCalendarRequest
{
    /// <summary>获取或设置日历信息（官方必填）。</summary>
    [JsonPropertyName("calendar")]
    public ScheduleCalendar? Calendar { get; set; }

    /// <summary>获取或设置授权方安装的应用 agentid（仅旧的第三方多应用套件需填此参数）。</summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }
}
