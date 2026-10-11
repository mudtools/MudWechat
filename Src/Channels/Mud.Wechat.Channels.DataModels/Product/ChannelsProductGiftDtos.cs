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

/// <summary>新增赠品（<c>product/gift/add</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + 请求体；仅支持单 sku（<c>skus</c> 长度固定为 1）。</para>
/// <para>
/// 主图 / 详情图等图片参数务必使用「上传图片」接口（resp_type=1）并回填返回的 img_url
/// （前缀 mmecimage.cn/p/），不接受其他任何格式的图片 url。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddGiftProductRequest
{
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

    /// <summary>获取或设置非卖商品类目（官方 <c>cats_v2</c>，必填，新类目树结构）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsProductCategory> CatsV2 { get; set; } = new();

    /// <summary>获取或设置非卖商品参数（官方 <c>attrs</c>；部分类目有必填参数）。</summary>
    [JsonPropertyName("attrs")]
    public List<ChannelsProductAttr>? Attrs { get; set; }

    /// <summary>获取或设置商家自定义的非卖商品编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>，无品牌为 <c>"2100000000"</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置非卖商品 SKU（官方 <c>skus</c>，必填：仅支持单 sku，长度固定为 1）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsGiftSku> Skus { get; set; } = new();

    /// <summary>获取或设置添加完成后是否立即上架（官方 <c>listing</c>：1 是，0 否；默认 0）。</summary>
    [JsonPropertyName("listing")]
    public int? Listing { get; set; }
}

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

/// <summary>新增赠品（<c>product/gift/add</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddGiftProductResponse : ChannelsResponse
{
    /// <summary>获取或设置非卖商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}

/// <summary>获取赠品（<c>product/gift/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetGiftProductRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置获取数据类型（官方 <c>data_type</c>，默认取 1：1 获取线上数据，
    /// 2 获取草稿数据，3 同时获取线上和草稿数据（注意：上架过的商品才有线上数据））。
    /// </summary>
    [JsonPropertyName("data_type")]
    public int? DataType { get; set; }
}

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

/// <summary>赠品 SKU（官方 <c>skus</c> 数组元素，<c>product/gift/get</c> 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftSkuDetail
{
    /// <summary>获取或设置 skuID（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置外部平台自定义 skuID（官方 <c>out_sku_id</c>）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置 sku 库存（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }

    /// <summary>获取或设置商家自定义的 sku 编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置 sku 状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>获取赠品（<c>product/gift/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetGiftProductResponse : ChannelsResponse
{
    /// <summary>
    /// 获取或设置赠品线上数据（官方 <c>product</c>；入参 <c>data_type==2</c> 时返回该字段；
    /// 入参 <c>data_type==3</c> 时赠品从未上线过，不返回该字段）。
    /// </summary>
    [JsonPropertyName("product")]
    public ChannelsGiftProductDetail? Product { get; set; }

    /// <summary>
    /// 获取或设置赠品草稿数据（官方 <c>edit_product</c>；入参 <c>data_type==1</c> 时不返回该字段）。
    /// </summary>
    [JsonPropertyName("edit_product")]
    public ChannelsGiftProductDetail? EditProduct { get; set; }
}

/// <summary>设置赠品上架（<c>product/gift/onsale/set</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSetGiftOnsaleRequest
{
    /// <summary>获取或设置设置为原始商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置赠品 SKU 列表（官方 <c>skus</c>，必填）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsGiftOnsaleSku> Skus { get; set; } = new();
}

/// <summary>赠品上架 SKU（官方 <c>skus</c> 数组元素，<c>product/gift/onsale/set</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftOnsaleSku
{
    /// <summary>
    /// 获取或设置设置为原始商品 skuID（官方 <c>sku_id</c>，必填；目前仅支持将单品商品设置为赠品）。
    /// </summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置划拨给赠品的库存数量（官方 <c>stock_num</c>，将直接从原始商品的库存中扣除，
    /// 且这部分库存不参与售卖）。
    /// </summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }
}

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

/// <summary>更新赠品（<c>product/gift/update</c>）的 SKU（官方 <c>skus</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftSkuUpdate
{
    /// <summary>获取或设置属性键 key（官方 <c>attr_key</c>，必填，属性自定义用）。</summary>
    [JsonPropertyName("attr_key")]
    public string AttrKey { get; set; } = string.Empty;

    /// <summary>获取或设置属性值（官方 <c>attr_value</c>，必填，属性自定义用）。</summary>
    [JsonPropertyName("attr_value")]
    public string AttrValue { get; set; } = string.Empty;

    /// <summary>获取或设置 sku_id（官方 <c>sku_id</c>；若填了已存在 sku_id，则进行更新 sku 操作，否则新增 sku）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>
    /// 获取或设置外部平台自定义 sku_id（官方 <c>out_sku_id</c>，最多 128 字符；
    /// 一旦添加成功后该字段无法修改）。
    /// </summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，必填，单位：分，不超过 1000000000（1000 万元））。</summary>
    [JsonPropertyName("sale_price")]
    public long SalePrice { get; set; }

    /// <summary>
    /// 获取或设置直接设置非卖商品库存（官方 <c>stock_num</c>，必填；高并发场景可能出现预期外表现，
    /// 更新时建议使用 <c>stock_diff</c>）。
    /// </summary>
    [JsonPropertyName("stock_num")]
    public long StockNum { get; set; }

    /// <summary>获取或设置库存信息（官方 <c>stock_diff</c>）。</summary>
    [JsonPropertyName("stock_diff")]
    public ChannelsGiftStockDiff? StockDiff { get; set; }

    /// <summary>获取或设置商家自定义的 sku 编码（官方 <c>sku_code</c>，最多 100 字符）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>
    /// 获取或设置类目 ID（官方 <c>cat_id</c>，必填；数组下标与一、二、…、N 级类目严格一致，
    /// 下标 length-1 为最后一级叶子类目）。
    /// </summary>
    [JsonPropertyName("cat_id")]
    public string CatId { get; set; } = string.Empty;
}

/// <summary>库存修改对象（官方 <c>stock_diff</c>，<c>product/gift/update</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGiftStockDiff
{
    /// <summary>获取或设置库存修改类型（官方 <c>diff_type</c>：1 增加，2 减少）。</summary>
    [JsonPropertyName("diff_type")]
    public int? DiffType { get; set; }

    /// <summary>获取或设置增加、减少或者设置的库存值（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }
}

/// <summary>更新赠品（<c>product/gift/update</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateGiftProductResponse : ChannelsResponse
{
    /// <summary>获取或设置小店内部非卖商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}