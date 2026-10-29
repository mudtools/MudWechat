// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 秒级时间戳区间（官方 <c>start_time</c> / <c>end_time</c> 对象，机构侧多族共用形态）。
/// </summary>
/// <remarks>
/// 官方在「佣金单列表的 <c>create_time_range</c> / <c>update_time_range</c>」与「商品详情的春节可发货时段
/// <c>delivery_time_list[]</c>」处给出的字段集完全同形（两个 number 时间戳）⇒ 一支类型复用，
/// 由守卫 CLG5 断言各处 <c>PropertyType</c> 同一，防止逐处 fork。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierTimeRange
{
    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
