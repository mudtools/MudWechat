// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>线索订单的商品信息（官方 <c>clue_detail.product_info</c>）。
/// 官方把 <c>actual_payment</c> 的说明写为「可分佣金额」（佣金单侧同名说明是「订单实付金额」），
/// 且 <c>product_id</c> 定为 string、无 <c>product_cnt</c> ⇒ 与佣金单侧分建。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierClueProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，参数表为 string）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置可分佣金额（官方 <c>actual_payment</c>，单位：分）。</summary>
    [JsonPropertyName("actual_payment")]
    public long? ActualPayment { get; set; }
}
