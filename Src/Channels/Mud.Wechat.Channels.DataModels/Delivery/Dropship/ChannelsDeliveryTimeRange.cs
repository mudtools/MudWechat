// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>秒级时间范围（官方 <c>create_time_range</c>/<c>update_time_range</c>/<c>order_create_time_range</c> 对象，
/// <c>dropship/list</c>、<c>dropship/search</c>、<c>supplyorder/list</c>、<c>supplyorder/search</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：四个拉单/搜单端点的三种时间范围对象字段集完全一致
/// （<c>start_time</c>/<c>end_time</c>）⇒ 共用本 DTO；官方限制起止距离不可超过 7 天。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryTimeRange
{
    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，必填，秒级时间戳，距离 <c>end_time</c> 不可超过 7 天）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，必填，秒级时间戳，距离 <c>start_time</c> 不可超过 7 天）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }
}
