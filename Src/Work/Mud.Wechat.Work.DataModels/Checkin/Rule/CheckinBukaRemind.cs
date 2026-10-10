// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则补卡提醒（创建/修改打卡规则 <c>group.buka_remind</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：设置 buka_remind_day/buka_remind_month 需要先设置 open_remind 为 true；buka_remind_month 为 0（当月）时 buka_remind_day 不可小于 15、不可超过 30；为 1（次月）时不可小于 1、不可超过 26。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinBukaRemind
{
    /// <summary>获取或设置是否打开补卡提醒。</summary>
    [JsonPropertyName("open_remind")]
    public bool? OpenRemind { get; set; }

    /// <summary>获取或设置补卡提醒日期（不超过 30；当月不可小于 15，次月不可小于 1）。</summary>
    [JsonPropertyName("buka_remind_day")]
    public int? BukaRemindDay { get; set; }

    /// <summary>获取或设置补卡提醒月份：0 - 当月；1 - 次月。</summary>
    [JsonPropertyName("buka_remind_month")]
    public int? BukaRemindMonth { get; set; }
}
