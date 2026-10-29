// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 机构侧商品 SKU（官方 <c>item.product_info.skus</c> 数组元素）。
/// </summary>
/// <remarks>
/// 与商家侧 <c>product/get</c> 的 <see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsProductSkuDetail"/> 字段集不同：
/// 机构侧无 <c>out_sku_id</c> / <c>sku_code</c> / <c>status</c>，多 <c>thumb_img</c> / <c>sku_deliver_info</c> /
/// <c>user_inclusive_price</c>（2026-09-16 新增「用户视角普惠价」）⇒ 分建 DTO；<c>sku_attrs</c> 元素与商家侧同形 ⇒ 复用。
/// <c>sku_id</c> 官方类型为 number，但同页代码示例为字符串（<c>"670813472"</c>）⇒ 与商家侧一致建模为 string。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProductDetailSku
{
    /// <summary>获取或设置 sku_id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>，添加时未录入则回包可能不含该字段）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置 sku 库存（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }

    /// <summary>获取或设置规格（官方 <c>sku_attrs</c>，与商家侧同形 ⇒ 复用 <see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsSkuAttr"/>）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<Mud.Wechat.Channels.DataModels.Product.ChannelsSkuAttr>? SkuAttrs { get; set; }

    /// <summary>获取或设置预售信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsLeagueHeadSupplierProductSkuDeliverInfo? SkuDeliverInfo { get; set; }

    /// <summary>获取或设置用户视角普惠价（官方 <c>user_inclusive_price</c>，<c>2026-09-16</c> 新增）。</summary>
    [JsonPropertyName("user_inclusive_price")]
    public long? UserInclusivePrice { get; set; }
}
