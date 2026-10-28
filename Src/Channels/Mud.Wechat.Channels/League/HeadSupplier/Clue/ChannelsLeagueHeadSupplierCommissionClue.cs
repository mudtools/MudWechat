// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>线索订单结构（官方 <c>commssion_clue</c>，<c>headsupplier/clue/get</c>）。
/// 官方参数表根层<b>无 status</b>，代码示例却出现 <c>status</c>（订单状态）⇒ 以参数表为权威，状态只走
/// <c>clue_detail.order_info.order_status</c>；<c>sku_id</c> 在参数表为 number、请求侧为 string，两处类型不同。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierCommissionClue
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置商品 skuid（官方 <c>sku_id</c>，本处参数表为 number）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置线索订单详情（官方 <c>clue_detail</c>）。</summary>
    [JsonPropertyName("clue_detail")]
    public ChannelsLeagueHeadSupplierClueDetail? ClueDetail { get; set; }
}
