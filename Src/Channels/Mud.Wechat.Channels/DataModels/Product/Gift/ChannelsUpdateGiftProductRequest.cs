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

/// <summary>更新赠品（<c>product/gift/update</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；仅支持单 sku（<c>skus</c> 长度固定为 1）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateGiftProductRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填，uint64）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>
    /// 获取或设置外部平台自定义非卖商品 ID（官方 <c>out_product_id</c>，最多 128 字符；
    /// 一旦添加成功后该字段无法修改）。
    /// </summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>
    /// 获取或设置标题（官方 <c>title</c>，必填：应至少含 5 个有效字符数，最多 60 字符；
    /// 不得仅为数字或英文，允许的特殊字符集见官方文档）。
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置主图（官方 <c>head_imgs</c>，必填：最少 3 张（食品饮料和生鲜类目非卖商品最少 4 张）、
    /// 最多 9 张，不得有重复图片）。
    /// </summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置赠品详情（官方 <c>desc_info</c>：<c>imgs</c> + <c>desc</c>）。</summary>
    [JsonPropertyName("desc_info")]
    public ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置非卖商品参数（官方 <c>attrs</c>；部分类目有必填参数）。</summary>
    [JsonPropertyName("attrs")]
    public List<ChannelsProductAttr>? Attrs { get; set; }

    /// <summary>获取或设置非卖商品 SKU（官方 <c>skus</c>，必填：仅支持单 sku，长度固定为 1）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsGiftSkuUpdate> Skus { get; set; } = new();

    /// <summary>获取或设置添加完成后是否立即上架（官方 <c>listing</c>：1 是，0 否；默认 0）。</summary>
    [JsonPropertyName("listing")]
    public int? Listing { get; set; }

    /// <summary>获取或设置非卖商品类目（官方 <c>cats_v2</c>，必填，新类目树结构）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsProductCategory> CatsV2 { get; set; } = new();

    /// <summary>获取或设置商家自定义的非卖商品编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>，无品牌为 <c>"2100000000"</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }
}
