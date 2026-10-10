// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 排班单项（为打卡人员排班请求 <c>items</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinSetScheduleItem
{
    /// <summary>获取或设置打卡人员 userid（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置要设置的天日期（取值在 1-31 之间，联合 yearmonth 组成唯一日期，官方必填）。</summary>
    [JsonPropertyName("day")]
    public int? Day { get; set; }

    /// <summary>获取或设置对应 groupid 规则下的班次 id（官方必填；通过预先拉取规则信息获取，0 代表休息）。</summary>
    [JsonPropertyName("schedule_id")]
    public long? ScheduleId { get; set; }
}
