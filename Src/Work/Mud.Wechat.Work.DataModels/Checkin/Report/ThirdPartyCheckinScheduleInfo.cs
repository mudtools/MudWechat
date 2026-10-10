// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡日报班次信息（<c>datas.scheduleinfo</c>，仅当规则类型为按班次上下班时有值）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：<c>timesec</c> 在参数表中描述为「班次上下班时间，单位秒」（标量），但返回示例中是对象（{"checkintime":32400,"checkouttime":61200}），以示例的对象形态承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinScheduleInfo
{
    /// <summary>获取或设置当日班次 id。</summary>
    [JsonPropertyName("scheduleid")]
    public long? Scheduleid { get; set; }

    /// <summary>获取或设置当日班次名称。</summary>
    [JsonPropertyName("schedulename")]
    public string? Schedulename { get; set; }

    /// <summary>获取或设置班次上下班时间（参数表描述为标量，返回示例为对象，以示例的对象形态承载）。</summary>
    [JsonPropertyName("timesec")]
    public ThirdPartyCheckinTimesec? Timesec { get; set; }

    /// <summary>获取或设置班次打卡时间（Unix 时间戳）。</summary>
    [JsonPropertyName("checkintime")]
    public long? Checkintime { get; set; }

    /// <summary>获取或设置班次下班打卡时间（Unix 时间戳）。</summary>
    [JsonPropertyName("checkouttime")]
    public long? Checkouttime { get; set; }

    /// <summary>获取或设置班次类型：0 - 休息日；1 - 工作日。</summary>
    [JsonPropertyName("daytype")]
    public int? Daytype { get; set; }
}
