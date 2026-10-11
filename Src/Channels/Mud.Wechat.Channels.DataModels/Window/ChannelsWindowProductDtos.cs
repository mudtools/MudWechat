// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 上架商品到橱窗（<c>addwindowproduct</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：官方注明该 API「已无实际作用」——橱窗中自营商品的上下架状态会自动同步为商品在店铺中的状态。
/// </para>
/// <para>
/// <b>官方文档矛盾照录</b>：参数表把 <c>product_id</c> 标注为 number、请求示例为带引号字符串
/// （"100234056"）——SDK 按参数表建模；同域 get/off/list 三端点的参数表均为 string(uint64)。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAddWindowProductRequest
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，必填。不支持带货中心来源的商品，其余商品的橱窗商品 ID 与商品来源处的平台内部商品 ID 相同）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string Appid { get; set; } = string.Empty;

    /// <summary>获取或设置是否需要在个人橱窗页隐藏（官方 <c>is_hide_for_window</c>，选填，默认为 false）。</summary>
    [JsonPropertyName("is_hide_for_window")]
    public bool? IsHideForWindow { get; set; }
}

/// <summary>获取橱窗商品详情（<c>getwindowproduct</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetWindowProductRequest
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，string(uint64)，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，必填；need_cps_product 为 true 时可留空）。</summary>
    [JsonPropertyName("appid")]
    public string Appid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置是否查询带货商品（官方 <c>need_cps_product</c>，选填）。填 true 后 product_id 支持两种 ID 查询：
    /// 输入小店商品 ID 可获取该商品的所有带货计划；输入带货商品 ID 只能获取该带货商品的计划。
    /// </summary>
    [JsonPropertyName("need_cps_product")]
    public bool? NeedCpsProduct { get; set; }
}

/// <summary>获取橱窗商品详情（<c>getwindowproduct</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetWindowProductResponse : ChannelsResponse
{
    /// <summary>获取或设置自营商品详情（官方 <c>product</c>）。</summary>
    [JsonPropertyName("product")]
    public ChannelsWindowProduct? Product { get; set; }

    /// <summary>获取或设置橱窗中的带货商品详情列表（官方 <c>cps_products</c>，元素结构与 product 一致，另携带 <c>cps_item_type</c>）。</summary>
    [JsonPropertyName("cps_products")]
    public List<ChannelsWindowProduct>? CpsProducts { get; set; }
}

/// <summary>
/// 橱窗商品详情（官方 <c>product</c> / <c>cps_products[]</c> 元素对象，<c>getwindowproduct</c>——两者结构一致 ⇒ 共用，
/// 仅自营商品携带 <c>page_path</c>/<c>banned_details</c>/<c>branch_info</c>/<c>limit_discount_info</c>）。
/// </summary>
/// <remarks>
/// 官方契约：自营商品橱窗 ID 即小店商品 ID；带货商品橱窗 ID 为货源商品在小店的商品 ID（另有一个平台分配的带货商品 ID）。
/// 官方注明 <c>leaf_category_id</c>（叶子类目 ID）将替代 <c>third_category_id</c>，后者最终会下掉，需尽快迁移。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowProduct
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>；自营为橱窗商品 ID，带货为带货商品 ID）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置外部商品 ID（官方 <c>out_product_id</c>；自营为商家侧外部商品 ID，带货为货源店铺的商品 ID）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品头图 url（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    /// <summary>获取或设置商品所属三级类目 ID（官方 <c>third_category_id</c>；官方将用 leaf_category_id 替代本字段）。</summary>
    [JsonPropertyName("third_category_id")]
    public string? ThirdCategoryId { get; set; }

    /// <summary>获取或设置商品所属叶子类目（品类）ID（官方 <c>leaf_category_id</c>）。</summary>
    [JsonPropertyName("leaf_category_id")]
    public string? LeafCategoryId { get; set; }

    /// <summary>获取或设置商品状态（官方 <c>status</c>，见 <see cref="ChannelsWindowProductStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置价格区间最大值/市场价/原价（官方 <c>market_price</c>，单位：分）。</summary>
    [JsonPropertyName("market_price")]
    public long? MarketPrice { get; set; }

    /// <summary>获取或设置价格区间最小值/销售价（官方 <c>selling_price</c>，单位：分）。</summary>
    [JsonPropertyName("selling_price")]
    public long? SellingPrice { get; set; }

    /// <summary>获取或设置剩余库存（官方 <c>stock</c>）。</summary>
    [JsonPropertyName("stock")]
    public int? Stock { get; set; }

    /// <summary>获取或设置销量（官方 <c>sales</c>）。</summary>
    [JsonPropertyName("sales")]
    public int? Sales { get; set; }

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，非带货商品才拥有）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置商品详情页路径（官方 <c>page_path</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("page_path")]
    public ChannelsWindowPagePath? PagePath { get; set; }

    /// <summary>获取或设置商品所属电商平台 ID（官方 <c>platform_id</c>）。</summary>
    [JsonPropertyName("platform_id")]
    public long? PlatformId { get; set; }

    /// <summary>获取或设置商品所属电商平台名（官方 <c>platform_name</c>）。</summary>
    [JsonPropertyName("platform_name")]
    public string? PlatformName { get; set; }

    /// <summary>获取或设置是否在个人橱窗页隐藏（官方 <c>is_hide_for_window</c>；官方参数表 boolean、返回示例为数字 0——矛盾照录，按参数表建模）。</summary>
    [JsonPropertyName("is_hide_for_window")]
    public bool? IsHideForWindow { get; set; }

    /// <summary>获取或设置商品是否处于禁止售卖的状态（官方 <c>banned</c>）。</summary>
    [JsonPropertyName("banned")]
    public bool? Banned { get; set; }

    /// <summary>获取或设置禁售明细（官方 <c>banned_details</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("banned_details")]
    public ChannelsWindowBannedDetails? BannedDetails { get; set; }

    /// <summary>获取或设置分店信息（官方 <c>branch_info</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("branch_info")]
    public ChannelsWindowBranchInfo? BranchInfo { get; set; }

    /// <summary>获取或设置抢购活动信息（官方 <c>limit_discount_info</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("limit_discount_info")]
    public ChannelsWindowLimitDiscountInfo? LimitDiscountInfo { get; set; }

    /// <summary>获取或设置带货商品所属计划类型（官方 <c>cps_item_type</c>，带货商品返回，见 <see cref="ChannelsWindowCpsItemTypes"/>）。</summary>
    [JsonPropertyName("cps_item_type")]
    public int? CpsItemType { get; set; }

    /// <summary>获取或设置跟佣跳转链接（官方 <c>product_promotion_link</c>，用于在小程序跳转小店场景添加商品时传递跟佣信息）。</summary>
    [JsonPropertyName("product_promotion_link")]
    public string? ProductPromotionLink { get; set; }
}

/// <summary>橱窗商品详情页路径（官方 <c>page_path</c> 对象，<c>getwindowproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowPagePath
{
    /// <summary>获取或设置商品详情半屏页、全屏页所属 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置商品详情半屏页 path（官方 <c>half_page_path</c>）。</summary>
    [JsonPropertyName("half_page_path")]
    public string? HalfPagePath { get; set; }

    /// <summary>获取或设置商品详情全屏页 path（官方 <c>full_page_path</c>）。</summary>
    [JsonPropertyName("full_page_path")]
    public string? FullPagePath { get; set; }
}

/// <summary>橱窗商品禁售明细（官方 <c>banned_details</c> 对象，<c>getwindowproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowBannedDetails
{
    /// <summary>获取或设置禁售原因（官方 <c>reason</c>，见 <see cref="ChannelsWindowBannedReasons"/>）。</summary>
    [JsonPropertyName("reason")]
    public int? Reason { get; set; }

    /// <summary>获取或设置需要申请的类目 ID（官方 <c>need_apply_category_id</c>；未申请售卖对应商品类目时带有）。</summary>
    [JsonPropertyName("need_apply_category_id")]
    public string? NeedApplyCategoryId { get; set; }

    /// <summary>获取或设置需要申请的类目名（官方 <c>need_apply_category_name</c>；未申请售卖对应商品类目时带有）。</summary>
    [JsonPropertyName("need_apply_category_name")]
    public string? NeedApplyCategoryName { get; set; }
}

/// <summary>橱窗商品分店信息（官方 <c>branch_info</c> 对象，<c>getwindowproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowBranchInfo
{
    /// <summary>获取或设置分店 ID（官方 <c>branch_id</c>）。</summary>
    [JsonPropertyName("branch_id")]
    public long? BranchId { get; set; }

    /// <summary>获取或设置分店名（官方 <c>branch_name</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    /// <summary>获取或设置分店状态（官方 <c>branch_status</c>，见 <see cref="ChannelsWindowBranchStatuses"/>）。</summary>
    [JsonPropertyName("branch_status")]
    public int? BranchStatus { get; set; }
}

/// <summary>橱窗商品抢购活动信息（官方 <c>limit_discount_info</c> 对象，<c>getwindowproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowLimitDiscountInfo
{
    /// <summary>获取或设置是否有生效中的抢购活动（官方 <c>is_effect</c>）。</summary>
    [JsonPropertyName("is_effect")]
    public bool? IsEffect { get; set; }

    /// <summary>获取或设置抢购价（官方 <c>discount_price</c>，单位：分）。</summary>
    [JsonPropertyName("discount_price")]
    public long? DiscountPrice { get; set; }

    /// <summary>
    /// 获取或设置抢购活动结束时间（官方 <c>end_time_ms</c>，毫秒时间戳；
    /// 官方参数表标注 string、返回示例为数字 1666086005500——矛盾照录，按参数表建模为字符串）。
    /// </summary>
    [JsonPropertyName("end_time_ms")]
    public string? EndTimeMs { get; set; }

    /// <summary>获取或设置抢购剩余库存（官方 <c>stock</c>）。</summary>
    [JsonPropertyName("stock")]
    public int? Stock { get; set; }
}

/// <summary>获取已添加到橱窗的商品列表（<c>getwindowproductlist</c>）请求体。</summary>
/// <remarks>官方契约：appid 与 query_cps_product 互斥，两者都传时以 appid 优先（仅返回来源指定店铺的自营商品）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetWindowProductListRequest
{
    /// <summary>获取或设置用于指定查询某个店铺来源的商品（官方 <c>appid</c>，选填；与 query_cps_product 互斥）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置用于指定查询属于某个分店 ID 下的商品（官方 <c>branch_id</c>，选填）。</summary>
    [JsonPropertyName("branch_id")]
    public long? BranchId { get; set; }

    /// <summary>获取或设置单页商品数（官方 <c>page_size</c>，必填，不超过 200；page_size × page_index ≤ 10000）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，选填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int? PageIndex { get; set; }

    /// <summary>获取或设置顺序翻页上下文（官方 <c>last_buffer</c>，选填；由上次请求返回，填了该值后 page_index 不生效）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置是否需要返回满足筛选条件的商品总数（官方 <c>need_total_num</c>，选填）。</summary>
    [JsonPropertyName("need_total_num")]
    public bool? NeedTotalNum { get; set; }

    /// <summary>获取或设置是否仅查询橱窗中的带货商品（官方 <c>query_cps_product</c>，选填；与 appid 互斥）。</summary>
    [JsonPropertyName("query_cps_product")]
    public bool? QueryCpsProduct { get; set; }
}

/// <summary>获取已添加到橱窗的商品列表（<c>getwindowproductlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetWindowProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置橱窗商品列表（官方 <c>products</c>）。</summary>
    [JsonPropertyName("products")]
    public List<ChannelsWindowListItem>? Products { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>last_buffer</c>，用于顺序翻页请求）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>橱窗商品列表项（官方 <c>products[]</c> 元素，<c>getwindowproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowListItem
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，string(uint64)；对于带货商品则为带货商品 ID）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>；对于自营商品会返回）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置货源店铺的商品 ID（官方 <c>out_product_id</c>；对于带货商品会返回）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品来源（官方 <c>product_source</c>，见 <see cref="ChannelsWindowProductSources"/>）。</summary>
    [JsonPropertyName("product_source")]
    public int? ProductSource { get; set; }
}

/// <summary>下架橱窗商品（<c>offwindowproduct</c>）请求体。</summary>
/// <remarks>
/// 官方契约：官方注明该 API「已无实际作用」——橱窗中自营商品的上下架状态会自动同步为商品在店铺中的状态。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsOffWindowProductRequest
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，string(uint64)，必填。不支持带货中心来源的商品）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string Appid { get; set; } = string.Empty;
}

/// <summary>橱窗商品状态常量（官方 <c>status</c> 枚举值，<c>getwindowproduct</c>）。</summary>
public static class ChannelsWindowProductStatuses
{
    /// <summary>已上架到橱窗。</summary>
    public const int OnWindow = 1;

    /// <summary>未上架到橱窗。</summary>
    public const int NotOnWindow = 2;

    /// <summary>已在商品来源处删除。</summary>
    public const int DeletedAtSource = 3;
}

/// <summary>橱窗商品禁售原因常量（官方 <c>reason</c> 枚举值，<c>getwindowproduct</c>）。</summary>
public static class ChannelsWindowBannedReasons
{
    /// <summary>三级类目在橱窗禁售，或商品在来源处被禁售。</summary>
    public const int CategoryOrSourceBanned = 0;

    /// <summary>商品属于可申请售卖的类目，但商家未完成申请。</summary>
    public const int CategoryApplyRequired = 1;

    /// <summary>商品所属分店未处于营业状态。</summary>
    public const int BranchClosed = 2;
}

/// <summary>橱窗商品分店状态常量（官方 <c>branch_status</c> 枚举值，<c>getwindowproduct</c>）。</summary>
public static class ChannelsWindowBranchStatuses
{
    /// <summary>营业中。</summary>
    public const int Open = 0;

    /// <summary>停业。</summary>
    public const int Closed = 1;
}

/// <summary>橱窗带货商品所属计划类型常量（官方 <c>cps_item_type</c> 枚举值，<c>getwindowproduct</c>）。</summary>
public static class ChannelsWindowCpsItemTypes
{
    /// <summary>普通计划。</summary>
    public const int Normal = 0;

    /// <summary>定向计划。</summary>
    public const int Directed = 1;

    /// <summary>专属计划。</summary>
    public const int Exclusive = 2;

    /// <summary>团长计划。</summary>
    public const int Leader = 3;
}

/// <summary>橱窗商品来源常量（官方 <c>product_source</c> 枚举值，<c>getwindowproductlist</c>）。</summary>
public static class ChannelsWindowProductSources
{
    /// <summary>来源店铺的自营商品。</summary>
    public const int SelfOperated = 0;

    /// <summary>来源选品中心的带货商品。</summary>
    public const int SelectionCenter = 1;
}
