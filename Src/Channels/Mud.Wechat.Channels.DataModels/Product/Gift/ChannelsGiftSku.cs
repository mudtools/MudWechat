// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「赠品子域」（6 端点：gift/add、gift/get、gift/list/get、gift/onsale/set、
// gift/stock/update、gift/update）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。
// gift/list/get 请求/响应与商品列表字段集一致 ⇒ 分别复用 ChannelsProductListRequest / ChannelsProductIdListResponse；
// gift/stock/update 与商品库存更新字段集一致 ⇒ 复用 ChannelsUpdateStockRequest。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>新增赠品（<c>product/gift/add</c>）的非卖商品 SKU（官方 <c>skus</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftSku
{
    /// <summary>
    /// 获取或设置类目 ID（官方 <c>cat_id</c>；数组下标与一、二、…、N 级类目严格一致，
    /// 下标 length-1 为最后一级叶子类目）。
    /// </summary>
    [JsonPropertyName("cat_id")]
    public string? CatId { get; set; }

    /// <summary>获取或设置属性键 key（官方 <c>attr_key</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_key")]
    public string? AttrKey { get; set; }

    /// <summary>获取或设置属性值（官方 <c>attr_value</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_value")]
    public string? AttrValue { get; set; }

    /// <summary>
    /// 获取或设置外部平台自定义 sku_id（官方 <c>out_sku_id</c>，最多 128 字符；
    /// 一旦添加成功后该字段无法修改）。
    /// </summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分，不超过 1000000000（1000 万元））。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置商家自定义的 sku 编码（官方 <c>sku_code</c>，最多 100 字符）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置创建非卖商品初始化设置的库存（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }
}
