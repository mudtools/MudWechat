// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表时长题答案（官方 <c>duration_reply</c>；读取收集表答案响应体嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormDurationReply
{
    /// <summary>获取或设置开始时间戳（官方 <c>begin_time</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public uint? BeginTime { get; set; }

    /// <summary>获取或设置结束时间戳（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public uint? EndTime { get; set; }

    /// <summary>
    /// 获取或设置时间刻度（官方 <c>time_scale</c>）。
    /// 官方取值：<c>1</c> 按天、<c>2</c> 按小时。
    /// </summary>
    [JsonPropertyName("time_scale")]
    public uint? TimeScale { get; set; }

    /// <summary>获取或设置单位换算、多少小时/天（官方 <c>day_range</c>），<c>time_scale</c> 为 2 时返回。</summary>
    [JsonPropertyName("day_range")]
    public uint? DayRange { get; set; }

    /// <summary>获取或设置天数（官方 <c>days</c>），<c>time_scale</c> 为 1 时返回。</summary>
    [JsonPropertyName("days")]
    public double? Days { get; set; }

    /// <summary>获取或设置小时数（官方 <c>hours</c>），<c>time_scale</c> 为 2 时返回。</summary>
    [JsonPropertyName("hours")]
    public double? Hours { get; set; }
}
