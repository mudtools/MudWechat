// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 多组休息时间（打卡规则请求/响应共用 <c>rest_times</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRestTime
{
    /// <summary>获取或设置休息开始时间（距离 0 点的秒）。</summary>
    [JsonPropertyName("rest_begin_time")]
    public int? RestBeginTime { get; set; }

    /// <summary>获取或设置休息结束时间（距离 0 点的秒）。</summary>
    [JsonPropertyName("rest_end_time")]
    public int? RestEndTime { get; set; }
}
