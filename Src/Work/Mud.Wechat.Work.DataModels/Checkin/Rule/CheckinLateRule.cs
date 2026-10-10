// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则响应侧晚走晚到时间规则（<c>group.schedulelist.late_rule</c>；与请求侧不同构，响应无直接 offwork_after_time/onwork_flex_time 子字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinLateRule
{
    /// <summary>获取或设置是否允许超时下班（下班晚走次日晚到；允许时 onwork_flex_time、offwork_after_time 才有意义）。</summary>
    [JsonPropertyName("allow_offwork_after_time")]
    public bool? AllowOffworkAfterTime { get; set; }

    /// <summary>获取或设置迟到规则时间列表。</summary>
    [JsonPropertyName("timerules")]
    public List<CheckinLateTimeRule>? Timerules { get; set; }
}
