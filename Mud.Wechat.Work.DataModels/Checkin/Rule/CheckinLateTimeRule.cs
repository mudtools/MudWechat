// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 晚走晚到时间规则单项（<c>late_rule.timerules</c> 元素，请求/响应共用形态；timerules 不为空时优先使用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinLateTimeRule
{
    /// <summary>获取或设置晚走的时间（距离最晚一个下班的时间，单位：秒，不超过 600 分钟）。</summary>
    [JsonPropertyName("offwork_after_time")]
    public int? OffworkAfterTime { get; set; }

    /// <summary>获取或设置第二天第一个班次允许迟到的弹性时间（单位：秒，不超过 600 分钟）。</summary>
    [JsonPropertyName("onwork_flex_time")]
    public int? OnworkFlexTime { get; set; }
}
