// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>线索订单的订单信息（官方 <c>clue_detail.order_info</c>，只有 <c>order_status</c> 一个字段）。
/// 官方本页的状态字段名即为 <c>order_status</c>（佣金单侧是 <c>status</c>）⇒ 各自照抄，不得互改。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierClueOrderInfo
{
    /// <summary>获取或设置订单状态（官方 <c>order_status</c>，见 <see cref="ChannelsLeagueHeadSupplierTradeOrderStatuses"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }
}
