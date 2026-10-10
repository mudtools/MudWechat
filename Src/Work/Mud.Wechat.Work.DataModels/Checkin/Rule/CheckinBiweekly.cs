// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 大小周配置（打卡规则 <c>checkindate.biweekly</c>，固定上下班规则时有效；官方参数表未提供返回示例，字段可空承载）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinBiweekly
{
    /// <summary>获取或设置是否开启大小周（固定上下班规则时有效）。</summary>
    [JsonPropertyName("enable_weekday_recurrence")]
    public bool? EnableWeekdayRecurrence { get; set; }

    /// <summary>获取或设置奇数周工作日（以 20240101 周次为第 1 周，后续奇数周次的工作日使用本字段；1 到 6 分别表示星期一到星期六，0 表示星期日）。</summary>
    [JsonPropertyName("odd_workdays")]
    public List<int>? OddWorkdays { get; set; }

    /// <summary>获取或设置偶数周工作日（以 20240101 周次为第 1 周，后续偶数周次的工作日使用本字段）。</summary>
    [JsonPropertyName("even_workdays")]
    public List<int>? EvenWorkdays { get; set; }
}
