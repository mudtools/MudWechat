// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>第三方货源商品 SKU（官方 <c>sku_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartyProductSku
{
    /// <summary>获取或设置 sku ID（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置规格名称（官方 <c>spec_name</c>，必填）。</summary>
    [JsonPropertyName("spec_name")]
    public string SpecName { get; set; } = string.Empty;

    /// <summary>获取或设置规格值（官方 <c>spec_value</c>，必填）。</summary>
    [JsonPropertyName("spec_value")]
    public string SpecValue { get; set; } = string.Empty;

    /// <summary>获取或设置 sku 图（官方 <c>sku_img</c>，必填）。</summary>
    [JsonPropertyName("sku_img")]
    public string SkuImg { get; set; } = string.Empty;

    /// <summary>获取或设置价格（官方 <c>price</c>，必填）。</summary>
    [JsonPropertyName("price")]
    public long Price { get; set; }

    /// <summary>获取或设置库存（官方 <c>stock</c>，必填）。</summary>
    [JsonPropertyName("stock")]
    public long Stock { get; set; }

    /// <summary>获取或设置库存类型（官方 <c>stock_type</c>，必填）。</summary>
    [JsonPropertyName("stock_type")]
    public long StockType { get; set; }
}
