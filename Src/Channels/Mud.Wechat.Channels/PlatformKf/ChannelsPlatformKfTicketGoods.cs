// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>工单关联订单商品（官方 <c>goods_list</c> 数组元素）。</summary>
/// <remarks><b>官方历史拼写照录</b>：字段名为 <c>good_name</c> / <c>good_count</c>（少一个 s），勿「顺手纠正」。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketGoods
{
    /// <summary>获取或设置商品名称（官方 <c>good_name</c>，官方历史拼写照录）。</summary>
    [JsonPropertyName("good_name")]
    public string? GoodName { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>good_count</c>，官方历史拼写照录）。</summary>
    [JsonPropertyName("good_count")]
    public int? GoodCount { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 SKU id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置商品缩略图 URL（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }
}
