// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>sku 信息（官方 <c>product_info.skus</c> 数组项）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：<c>sku_attrs</c> 参数表标注 object，官方响应示例为对象数组 ⇒ 按数组建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterPromoteProductSku
{
    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>）。</summary>
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

    /// <summary>获取或设置 sku 属性（官方 <c>sku_attrs</c>）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<ChannelsPromoterPromoteProductSkuAttr>? SkuAttrs { get; set; }

    /// <summary>获取或设置预售信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsPromoterSkuDeliverInfo? SkuDeliverInfo { get; set; }

    /// <summary>获取或设置用户视角普惠价（官方 <c>user_inclusive_price</c>，单位：分）。</summary>
    [JsonPropertyName("user_inclusive_price")]
    public long? UserInclusivePrice { get; set; }
}
