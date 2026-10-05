// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡日报打卡时间信息（<c>datas.rule_info.checkin_time</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinDayRuleCheckin
{
    /// <summary>获取或设置打卡类型：1 - 上下班；2 - 外出。</summary>
    [JsonPropertyName("checkin_type")]
    public int? CheckinType { get; set; }

    /// <summary>获取或设置异常类型：1 - 时间异常；2 - 地点异常；3 - wifi 异常；4 - 非常用设备异常；100 - 其他异常类型。</summary>
    [JsonPropertyName("exception_type")]
    public int? ExceptionType { get; set; }

    /// <summary>获取或设置异常时长（秒）。</summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>获取或设置实际异常时长（秒）。</summary>
    [JsonPropertyName("actual_duration")]
    public int? ActualDuration { get; set; }
}
