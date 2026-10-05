// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 加班休息按加班时长扣除条件（<c>cal_ottime_rule.items</c> 元素，请求/响应共用形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtCalItem
{
    /// <summary>获取或设置加班满时长（秒；休息时间和加班时间必填；ot_time 应递增且为 15 分钟的倍数）。</summary>
    [JsonPropertyName("ot_time")]
    public int? OtTime { get; set; }

    /// <summary>获取或设置对应扣除时长（秒；ot_time 不小于 rest_time；不可大于加班时间，需要为 15 分钟的倍数且不超过 360 分钟）。</summary>
    [JsonPropertyName("rest_time")]
    public int? RestTime { get; set; }
}
