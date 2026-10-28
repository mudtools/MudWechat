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

/// <summary>赠品详情（官方 <c>product</c> / <c>edit_product</c> 对象，<c>product/gift/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftProductDetail
{
    /// <summary>获取或设置小店内部赠品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置外部平台自定义赠品 ID（官方 <c>out_product_id</c>）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置主图（官方 <c>head_imgs</c>，最多 9 张，每张不超过 2MB）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置赠品详情（官方 <c>desc_info</c>：<c>imgs</c> + <c>desc</c>）。</summary>
    [JsonPropertyName("desc_info")]
    public ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置赠品线上状态（官方 <c>status</c>，<c>edit_product</c> 和 <c>product</c> 都会返回）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置赠品草稿状态（官方 <c>edit_status</c>，以 <c>edit_product</c> 字段返回的值为准，
    /// <c>product</c> 不返回；在售赠品没有草稿）。
    /// </summary>
    [JsonPropertyName("edit_status")]
    public int? EditStatus { get; set; }

    /// <summary>获取或设置非卖商品类目（官方 <c>cats_v2</c>，新类目树结构）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsProductCategory>? CatsV2 { get; set; }

    /// <summary>获取或设置非卖商品参数（官方 <c>attrs</c>）。</summary>
    [JsonPropertyName("attrs")]
    public List<ChannelsProductAttr>? Attrs { get; set; }

    /// <summary>获取或设置商家自定义的赠品编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>，无品牌为 <c>"2100000000"</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置赠品 SKU（官方 <c>skus</c>）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsGiftSkuDetail>? Skus { get; set; }

    /// <summary>
    /// 获取或设置赠品类型（官方 <c>product_type</c>：4 在售赠品，5 非卖赠品；
    /// 在售赠品为只读数据，不支持编辑、下架操作，不支持用 <c>data_type=2</c> 的参数获取）。
    /// </summary>
    [JsonPropertyName("product_type")]
    public int? ProductType { get; set; }

    /// <summary>获取或设置赠品草稿最近一次修改时间（官方 <c>edit_time</c>）。</summary>
    [JsonPropertyName("edit_time")]
    public long? EditTime { get; set; }

    /// <summary>获取或设置在售赠品的来源商品 id（官方 <c>src_product_id</c>，非卖赠品没有该字段）。</summary>
    [JsonPropertyName("src_product_id")]
    public long? SrcProductId { get; set; }
}
