// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>数值区间（官方 <c>min</c> / <c>max</c> 形态对象的共用建模）。</summary>
/// <remarks>用于机构侧多处的区间筛选：<c>cooperativeitem/list/get</c> 的 <c>listing_time_range</c>、选品广场 <c>spu_item_condition</c> 的售卖价 / 月销量 / 服务费率 / 佣金率 / 推广时间 / 创建时间六个区间。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierNumberRange
{
    /// <summary>获取或设置区间最小值（官方 <c>min</c>，秒级时间戳）。</summary>
    [JsonPropertyName("min")]
    public long? Min { get; set; }

    /// <summary>获取或设置区间最大值（官方 <c>max</c>，秒级时间戳）。</summary>
    [JsonPropertyName("max")]
    public long? Max { get; set; }
}
