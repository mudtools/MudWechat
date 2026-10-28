// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 个人排班表信息（获取打卡人员排班信息响应 <c>schedule_list</c> 元素）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：<c>yearmonth</c> 是 uint32 数字（如 202011），不是字符串。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinScheduleListInfo
{
    /// <summary>获取或设置打卡人员 userid。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置排班表月份（格式为年月数字，如 202011）。</summary>
    [JsonPropertyName("yearmonth")]
    public int? Yearmonth { get; set; }

    /// <summary>获取或设置打卡规则 id。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置打卡规则名。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置个人排班信息。</summary>
    [JsonPropertyName("schedule")]
    public CheckinPersonalSchedule? Schedule { get; set; }
}
