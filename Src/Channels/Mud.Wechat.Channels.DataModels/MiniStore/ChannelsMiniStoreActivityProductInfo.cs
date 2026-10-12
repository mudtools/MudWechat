// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>活动礼物商品信息（官方 <c>product_info_list</c> 数组元素）。</summary>
/// <remarks><c>can_use_stock_num</c>（剩余库存）仅小店授权类型活动才有。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreActivityProductInfo
{
    /// <summary>获取或设置商品 spu id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置商品图片 url（官方 <c>product_img_url</c>）。</summary>
    [JsonPropertyName("product_img_url")]
    public string? ProductImgUrl { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>product_name</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>获取或设置售卖单价（官方 <c>price</c>，单位为分）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>获取或设置商品 sku 名称（官方 <c>sku_name</c>）。</summary>
    [JsonPropertyName("sku_name")]
    public string? SkuName { get; set; }

    /// <summary>获取或设置活动 sku 总库存（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public int? StockNum { get; set; }

    /// <summary>获取或设置活动 sku 剩余库存（官方 <c>can_use_stock_num</c>，小店授权类型才有）。</summary>
    [JsonPropertyName("can_use_stock_num")]
    public int? CanUseStockNum { get; set; }
}
