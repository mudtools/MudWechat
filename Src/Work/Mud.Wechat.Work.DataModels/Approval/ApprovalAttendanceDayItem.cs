// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 假勤单日分片（slice_info.day_items 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalAttendanceDayItem
{
    /// <summary>
    /// 获取或设置当天零点时间戳（当天的东八区零点时间戳）。
    /// </summary>
    [JsonPropertyName("daytime")]
    public long? Daytime { get; set; }

    /// <summary>
    /// 获取或设置分隔当前日期的时长秒数，可以为 0。
    /// </summary>
    /// <remarks>
    /// <para>type 为 halfday 时：加班需为 8640 的整倍数、其他需为 43200 的整倍数；type 为 hour 时需为 360 的整倍数。</para>
    /// </remarks>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
}
