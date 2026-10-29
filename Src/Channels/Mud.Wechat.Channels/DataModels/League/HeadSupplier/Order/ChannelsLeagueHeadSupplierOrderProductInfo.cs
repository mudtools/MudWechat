// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>佣金单的商品信息（官方 <c>order_detail.product_info</c>）。</summary>
/// <remarks>参数表定 <c>product_id</c> 为 number、代码示例写字符串 ⇒ 以参数表为权威。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierOrderProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置订单实付金额（官方 <c>actual_payment</c>，单位：分）。</summary>
    [JsonPropertyName("actual_payment")]
    public long? ActualPayment { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>product_cnt</c>，2026-03-31 新增）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}
