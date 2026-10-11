// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>
/// 罗盘日期查询请求体（官方 <c>ds</c>，格式 YYYYMMDD）。
/// </summary>
/// <remarks>
/// 共用裁决：getshopfinderoverall / getshopoverall / getfinderoverall / getfinderproductdata /
/// getfinderproductlist 五端点请求体字段集完全一致（仅 <c>ds</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassDsRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;
}

/// <summary>获取带货达人列表（<c>getshopfinderlist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderListRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置版本号（官方 <c>version</c>，选填；填 <c>v2</c> 会返回带货者身份的 id）。</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

/// <summary>获取带货达人列表（<c>getshopfinderlist</c>）响应。</summary>
/// <remarks>官方业务限制：仅返回当日有带货数据的达人（官方原文）；权限集 id：175。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderListResponse : ChannelsResponse
{
    /// <summary>获取或设置视频号列表（官方 <c>finder_list</c>）。</summary>
    [JsonPropertyName("finder_list")]
    public List<ChannelsCompassShopFinderItem>? FinderList { get; set; }
}

/// <summary>带货达人条目（官方 <c>finder_list</c> 数组元素，<c>getshopfinderlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderItem
{
    /// <summary>获取或设置视频号 id（官方 <c>finder_id</c>）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置视频号昵称（官方 <c>finder_nickname</c>）。</summary>
    [JsonPropertyName("finder_nickname")]
    public string? FinderNickname { get; set; }

    /// <summary>获取或设置成交数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassDealData? Data { get; set; }

    /// <summary>获取或设置带货者 id（官方 <c>talent_id</c>）。</summary>
    [JsonPropertyName("talent_id")]
    public string? TalentId { get; set; }

    /// <summary>获取或设置带货者昵称（官方 <c>talent_nickname</c>）。</summary>
    [JsonPropertyName("talent_nickname")]
    public string? TalentNickname { get; set; }

    /// <summary>获取或设置公众号 id（官方 <c>mp_id</c>）。</summary>
    [JsonPropertyName("mp_id")]
    public string? MpId { get; set; }

    /// <summary>获取或设置公众号昵称（官方 <c>mp_nickname</c>）。</summary>
    [JsonPropertyName("mp_nickname")]
    public string? MpNickname { get; set; }
}

/// <summary>罗盘成交数据（官方 <c>data</c> 对象，<c>getshopfinderlist</c>）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：参数表类型标注为 number，官方示例为字符串（如 <c>"pay_gmv": "1"</c>）——
/// 按参数表（number）建模；金额字段单位为<b>分</b>。与 <c>getshopfinderproductoverall</c> 的 string
/// 口径不共用 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassDealData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }

    /// <summary>获取或设置动销商品数（官方 <c>pay_product_id_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_id_cnt")]
    public long? PayProductIdCnt { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>）。</summary>
    [JsonPropertyName("pay_uv")]
    public long? PayUv { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("refund_gmv")]
    public long? RefundGmv { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public long? PayRefundGmv { get; set; }
}

/// <summary>
/// 罗盘带货达人商品维度请求体（<c>getshopfinderproductlist</c> / <c>getshopfinderproductoverall</c>）。
/// </summary>
/// <remarks>共用裁决：两端点请求体字段集完全一致（<c>ds</c> 必填 + 三选一身份 id）⇒ 共用本 DTO。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置视频号 id（官方 <c>finder_id</c>，与 talent_id、mp_id 三者必填其一）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置达人号 id（官方 <c>talent_id</c>，与 finder_id、mp_id 三者必填其一）。</summary>
    [JsonPropertyName("talent_id")]
    public string? TalentId { get; set; }

    /// <summary>获取或设置公众号 id（官方 <c>mp_id</c>，与 finder_id、talent_id 三者必填其一）。</summary>
    [JsonPropertyName("mp_id")]
    public string? MpId { get; set; }
}

/// <summary>获取授权视频号列表（<c>getshopfinderauthorizationlist</c>）响应。</summary>
/// <remarks>官方契约：<b>POST</b> + 空请求体（官方示例请求体为 <c>{}</c>）；权限集 id：175。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderAuthorizationListResponse : ChannelsResponse
{
    /// <summary>获取或设置授权视频号 id 列表（官方 <c>authorized_finder_id_list</c>）。</summary>
    [JsonPropertyName("authorized_finder_id_list")]
    public List<string>? AuthorizedFinderIdList { get; set; }
}

/// <summary>获取带货数据概览（<c>getshopfinderoverall</c>）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：参数表类型标注为 number，官方示例为字符串（<c>"pay_gmv": "0"</c>）——按参数表（number）建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderOverallResponse : ChannelsResponse
{
    /// <summary>获取或设置带货数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopFinderOverallData? Data { get; set; }
}

/// <summary>带货数据概览（官方 <c>data</c> 对象，<c>getshopfinderoverall</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderOverallData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }

    /// <summary>获取或设置动销达人数（官方 <c>pay_sales_finder_cnt</c>）。</summary>
    [JsonPropertyName("pay_sales_finder_cnt")]
    public long? PaySalesFinderCnt { get; set; }

    /// <summary>获取或设置动销商品数（官方 <c>pay_product_id_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_id_cnt")]
    public long? PayProductIdCnt { get; set; }

    /// <summary>获取或设置点击-成交转化率（官方 <c>click_to_pay_uv_ratio</c>）。</summary>
    [JsonPropertyName("click_to_pay_uv_ratio")]
    public double? ClickToPayUvRatio { get; set; }
}

/// <summary>获取带货达人商品列表（<c>getshopfinderproductlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置商品信息列表（官方 <c>product_list</c>）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsCompassShopFinderProductItem>? ProductList { get; set; }
}

/// <summary>带货达人商品条目（官方 <c>product_list</c> 数组元素，<c>getshopfinderproductlist</c>）。</summary>
/// <remarks>官方示例中档案与金额字段以字符串返回而参数表标注 number——按参数表建模，矛盾照录。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，参数表类型 number）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品头图（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品价格（官方 <c>price</c>，参数表类型 number）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>获取或设置商品 1 级类目（官方 <c>first_category_id</c>，参数表类型 number）。</summary>
    [JsonPropertyName("first_category_id")]
    public long? FirstCategoryId { get; set; }

    /// <summary>获取或设置商品 2 级类目（官方 <c>second_category_id</c>，参数表类型 number）。</summary>
    [JsonPropertyName("second_category_id")]
    public long? SecondCategoryId { get; set; }

    /// <summary>获取或设置商品 3 级类目（官方 <c>third_category_id</c>，参数表类型 number）。</summary>
    [JsonPropertyName("third_category_id")]
    public long? ThirdCategoryId { get; set; }

    /// <summary>获取或设置佣金数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopFinderProductCommission? Data { get; set; }
}

/// <summary>带货达人商品佣金数据（官方 <c>data</c> 对象，<c>getshopfinderproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductCommission
{
    /// <summary>获取或设置佣金率（官方 <c>commission_ratio</c>）。</summary>
    [JsonPropertyName("commission_ratio")]
    public double? CommissionRatio { get; set; }

    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分，参数表类型 number）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }
}

/// <summary>获取带货达人详情（<c>getshopfinderproductoverall</c>）响应。</summary>
/// <remarks>返回字段官方类型为 string（示例为空串）——与 <c>getshopfinderlist</c> 的 number 口径不同 ⇒ 不共用成交数据 DTO。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductOverallResponse : ChannelsResponse
{
    /// <summary>获取或设置带货数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopFinderProductOverallData? Data { get; set; }
}

/// <summary>带货达人详情数据（官方 <c>data</c> 对象，<c>getshopfinderproductoverall</c>，官方类型 string）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductOverallData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("pay_gmv")]
    public string? PayGmv { get; set; }

    /// <summary>获取或设置动销商品数（官方 <c>pay_product_id_cnt</c>，官方类型 string）。</summary>
    [JsonPropertyName("pay_product_id_cnt")]
    public string? PayProductIdCnt { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>，官方类型 string）。</summary>
    [JsonPropertyName("pay_uv")]
    public string? PayUv { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("refund_gmv")]
    public string? RefundGmv { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public string? PayRefundGmv { get; set; }
}

/// <summary>获取店铺开播列表（<c>getshoplivelist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopLiveListRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置视频号 id（官方 <c>finder_id</c>，必填）。</summary>
    [JsonPropertyName("finder_id")]
    public string FinderId { get; set; } = string.Empty;
}

/// <summary>获取店铺开播列表（<c>getshoplivelist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopLiveListResponse : ChannelsResponse
{
    /// <summary>获取或设置开播列表（官方 <c>live_list</c>）。</summary>
    [JsonPropertyName("live_list")]
    public List<ChannelsCompassShopLiveItem>? LiveList { get; set; }
}

/// <summary>店铺开播条目（官方 <c>live_list</c> 数组元素，<c>getshoplivelist</c>）。</summary>
/// <remarks><c>live_time</c>/<c>live_duration</c> 官方类型为 string（Unix 时间戳/时长以字符串返回），照抄建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopLiveItem
{
    /// <summary>获取或设置直播 id（官方 <c>live_id</c>）。</summary>
    [JsonPropertyName("live_id")]
    public string? LiveId { get; set; }

    /// <summary>获取或设置直播标题（官方 <c>live_title</c>）。</summary>
    [JsonPropertyName("live_title")]
    public string? LiveTitle { get; set; }

    /// <summary>获取或设置开播时间（官方 <c>live_time</c>，官方类型 string 的 Unix 时间戳）。</summary>
    [JsonPropertyName("live_time")]
    public string? LiveTime { get; set; }

    /// <summary>获取或设置直播时长（官方 <c>live_duration</c>，官方类型 string）。</summary>
    [JsonPropertyName("live_duration")]
    public string? LiveDuration { get; set; }

    /// <summary>获取或设置直播封面（官方 <c>live_cover_img_url</c>）。</summary>
    [JsonPropertyName("live_cover_img_url")]
    public string? LiveCoverImgUrl { get; set; }
}

/// <summary>获取电商数据概览（<c>getshopoverall</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopOverallResponse : ChannelsResponse
{
    /// <summary>获取或设置电商数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopOverallData? Data { get; set; }
}

/// <summary>电商数据概览（官方 <c>data</c> 对象，<c>getshopoverall</c>，官方类型 string）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopOverallData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("pay_gmv")]
    public string? PayGmv { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>，官方类型 string）。</summary>
    [JsonPropertyName("pay_uv")]
    public string? PayUv { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>pay_order_cnt</c>，官方类型 string）。</summary>
    [JsonPropertyName("pay_order_cnt")]
    public string? PayOrderCnt { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public string? PayRefundGmv { get; set; }

    /// <summary>获取或设置直播成交金额（官方 <c>live_pay_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("live_pay_gmv")]
    public string? LivePayGmv { get; set; }

    /// <summary>获取或设置短视频成交金额（官方 <c>feed_pay_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("feed_pay_gmv")]
    public string? FeedPayGmv { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>product_click_uv</c>，官方类型 string）。</summary>
    [JsonPropertyName("product_click_uv")]
    public string? ProductClickUv { get; set; }
}

/// <summary>获取商品详细信息（<c>getshopproductdata</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductDataRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填，参数表类型 number）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }
}

/// <summary>获取商品详细信息（<c>getshopproductdata</c>）响应。</summary>
/// <remarks>官方业务限制：仅历史数据，暂无当日数据（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductDataResponse : ChannelsResponse
{
    /// <summary>获取或设置商品详细信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsCompassShopProductInfo? ProductInfo { get; set; }
}

/// <summary>商品档案信息（官方 <c>product_info</c> 对象，<c>getshopproductdata</c>，官方类型 string）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品图（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品价格（官方 <c>price</c>，单位：分）。</summary>
    [JsonPropertyName("price")]
    public string? Price { get; set; }

    /// <summary>获取或设置商品一级类目（官方 <c>first_category_id</c>）。</summary>
    [JsonPropertyName("first_category_id")]
    public string? FirstCategoryId { get; set; }

    /// <summary>获取或设置商品二级类目（官方 <c>second_category_id</c>）。</summary>
    [JsonPropertyName("second_category_id")]
    public string? SecondCategoryId { get; set; }

    /// <summary>获取或设置商品三级类目（官方 <c>third_category_id</c>）。</summary>
    [JsonPropertyName("third_category_id")]
    public string? ThirdCategoryId { get; set; }

    /// <summary>获取或设置详细数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopProductDetailData? Data { get; set; }
}

/// <summary>
/// 商品详细数据（官方 <c>data</c> 对象，<c>getshopproductdata</c>，官方类型 string）。
/// </summary>
/// <remarks>
/// 金额字段单位均为<b>分</b>。<b>官方文档矛盾照录</b>：<c>pay_refund_ratio</c>/
/// <c>pay_refund_after_send_ratio</c>/<c>pay_refund_before_send_ratio</c> 与 <c>refund_gmv</c>
/// 官方类型列标注为 null（无类型）——比率按示例数值建模为 double、<c>refund_gmv</c> 按示例字符串建模为 string。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductDetailData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public string? PayGmv { get; set; }

    /// <summary>获取或设置下单金额（官方 <c>create_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("create_gmv")]
    public string? CreateGmv { get; set; }

    /// <summary>获取或设置下单订单数（官方 <c>create_cnt</c>）。</summary>
    [JsonPropertyName("create_cnt")]
    public string? CreateCnt { get; set; }

    /// <summary>获取或设置下单人数（官方 <c>create_uv</c>）。</summary>
    [JsonPropertyName("create_uv")]
    public string? CreateUv { get; set; }

    /// <summary>获取或设置下单件数（官方 <c>create_product_cnt</c>）。</summary>
    [JsonPropertyName("create_product_cnt")]
    public string? CreateProductCnt { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>pay_cnt</c>）。</summary>
    [JsonPropertyName("pay_cnt")]
    public string? PayCnt { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>）。</summary>
    [JsonPropertyName("pay_uv")]
    public string? PayUv { get; set; }

    /// <summary>获取或设置成交件数（官方 <c>pay_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_cnt")]
    public string? PayProductCnt { get; set; }

    /// <summary>获取或设置成交金额（剔除退款）（官方 <c>pure_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pure_pay_gmv")]
    public string? PurePayGmv { get; set; }

    /// <summary>获取或设置成交客单价（剔除退款）（官方 <c>pay_gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv_per_uv")]
    public string? PayGmvPerUv { get; set; }

    /// <summary>获取或设置实际结算金额（官方 <c>seller_actual_settle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("seller_actual_settle_amount")]
    public string? SellerActualSettleAmount { get; set; }

    /// <summary>获取或设置实际服务费金额（官方 <c>platform_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("platform_actual_commission")]
    public string? PlatformActualCommission { get; set; }

    /// <summary>获取或设置实际达人佣金支出（官方 <c>finderuin_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("finderuin_actual_commission")]
    public string? FinderuinActualCommission { get; set; }

    /// <summary>获取或设置实际团长佣金支出（官方 <c>captain_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("captain_actual_commission")]
    public string? CaptainActualCommission { get; set; }

    /// <summary>获取或设置预估结算金额（官方 <c>seller_predict_settle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("seller_predict_settle_amount")]
    public string? SellerPredictSettleAmount { get; set; }

    /// <summary>获取或设置预估服务费金额（官方 <c>platform_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("platform_predict_commission")]
    public string? PlatformPredictCommission { get; set; }

    /// <summary>获取或设置预估达人佣金支出（官方 <c>finderuin_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("finderuin_predict_commission")]
    public string? FinderuinPredictCommission { get; set; }

    /// <summary>获取或设置预估团长佣金支出（官方 <c>captain_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("captain_predict_commission")]
    public string? CaptainPredictCommission { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>product_click_uv</c>）。</summary>
    [JsonPropertyName("product_click_uv")]
    public string? ProductClickUv { get; set; }

    /// <summary>获取或设置商品点击次数（官方 <c>product_click_cnt</c>）。</summary>
    [JsonPropertyName("product_click_cnt")]
    public string? ProductClickCnt { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public string? PayRefundGmv { get; set; }

    /// <summary>获取或设置成交退款人数（官方 <c>pay_refund_uv</c>）。</summary>
    [JsonPropertyName("pay_refund_uv")]
    public string? PayRefundUv { get; set; }

    /// <summary>获取或设置成交退款率（官方 <c>pay_refund_ratio</c>，官方类型列标注 null、示例为数值——按 double 建模）。</summary>
    [JsonPropertyName("pay_refund_ratio")]
    public double? PayRefundRatio { get; set; }

    /// <summary>获取或设置发货后成交退款率（官方 <c>pay_refund_after_send_ratio</c>，官方类型列标注 null、示例为数值——按 double 建模）。</summary>
    [JsonPropertyName("pay_refund_after_send_ratio")]
    public double? PayRefundAfterSendRatio { get; set; }

    /// <summary>获取或设置成交退款订单数（官方 <c>pay_refund_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_cnt")]
    public string? PayRefundCnt { get; set; }

    /// <summary>获取或设置成交退款件数（官方 <c>pay_refund_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_product_cnt")]
    public string? PayRefundProductCnt { get; set; }

    /// <summary>获取或设置发货前成交退款率（官方 <c>pay_refund_before_send_ratio</c>，官方类型列标注 null、示例为数值——按 double 建模）。</summary>
    [JsonPropertyName("pay_refund_before_send_ratio")]
    public double? PayRefundBeforeSendRatio { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_gmv</c>，单位：分，官方类型列标注 null、示例为字符串——按 string 建模）。</summary>
    [JsonPropertyName("refund_gmv")]
    public string? RefundGmv { get; set; }

    /// <summary>获取或设置退款件数（官方 <c>refund_product_cnt</c>）。</summary>
    [JsonPropertyName("refund_product_cnt")]
    public string? RefundProductCnt { get; set; }

    /// <summary>获取或设置退款订单数（官方 <c>refund_cnt</c>）。</summary>
    [JsonPropertyName("refund_cnt")]
    public string? RefundCnt { get; set; }

    /// <summary>获取或设置退款人数（官方 <c>refund_uv</c>）。</summary>
    [JsonPropertyName("refund_uv")]
    public string? RefundUv { get; set; }
}

/// <summary>获取商品列表（<c>getshopproductlist</c>）请求体。</summary>
/// <remarks><c>limit</c>/<c>offset</c> 不填默认只返回 10 条商品数据（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductListRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置分页参数（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}

/// <summary>获取商品列表（<c>getshopproductlist</c>）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：返回示例含商品档案字段（head_img_url/title/price/三级类目）而参数表未列——按参数表建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置商品列表（官方 <c>product_list</c>）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsCompassShopProductListItem>? ProductList { get; set; }

    /// <summary>获取或设置一共有多少条商品信息（官方 <c>total_count</c>，参数表类型 number）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>商品列表条目（官方 <c>product_list</c> 数组元素，<c>getshopproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductListItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，官方类型 string）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置详细数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopProductListData? Data { get; set; }
}

/// <summary>
/// 商品列表详细数据（官方 <c>data</c> 对象，<c>getshopproductlist</c>，官方类型 string）。
/// </summary>
/// <remarks>
/// 字段集与 <c>getshopproductdata</c> 的明细数据不同（无 seller_actual/predict 结算金额、多 pay_refund_cnt）⇒ 分建不共用。
/// <b>官方文档矛盾照录</b>：<c>refund_gmv</c> 官方说明写「退款件数」（疑为笔误，退款件数为 refund_product_cnt），照录；
/// <c>pay_refund_ratio</c> 类型列标注 null、示例为数值——按 double 建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductListData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public string? PayGmv { get; set; }

    /// <summary>获取或设置下单金额（官方 <c>create_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("create_gmv")]
    public string? CreateGmv { get; set; }

    /// <summary>获取或设置下单订单数（官方 <c>create_cnt</c>）。</summary>
    [JsonPropertyName("create_cnt")]
    public string? CreateCnt { get; set; }

    /// <summary>获取或设置下单人数（官方 <c>create_uv</c>）。</summary>
    [JsonPropertyName("create_uv")]
    public string? CreateUv { get; set; }

    /// <summary>获取或设置下单件数（官方 <c>create_product_cnt</c>）。</summary>
    [JsonPropertyName("create_product_cnt")]
    public string? CreateProductCnt { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>pay_cnt</c>）。</summary>
    [JsonPropertyName("pay_cnt")]
    public string? PayCnt { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>）。</summary>
    [JsonPropertyName("pay_uv")]
    public string? PayUv { get; set; }

    /// <summary>获取或设置成交件数（官方 <c>pay_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_cnt")]
    public string? PayProductCnt { get; set; }

    /// <summary>获取或设置成交金额（剔除退款）（官方 <c>pure_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pure_pay_gmv")]
    public string? PurePayGmv { get; set; }

    /// <summary>获取或设置成交客单价（剔除退款）（官方 <c>pay_gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv_per_uv")]
    public string? PayGmvPerUv { get; set; }

    /// <summary>获取或设置实际服务费金额（官方 <c>platform_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("platform_actual_commission")]
    public string? PlatformActualCommission { get; set; }

    /// <summary>获取或设置实际达人佣金支出（官方 <c>finderuin_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("finderuin_actual_commission")]
    public string? FinderuinActualCommission { get; set; }

    /// <summary>获取或设置实际团长佣金支出（官方 <c>captain_actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("captain_actual_commission")]
    public string? CaptainActualCommission { get; set; }

    /// <summary>获取或设置预估服务费金额（官方 <c>platform_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("platform_predict_commission")]
    public string? PlatformPredictCommission { get; set; }

    /// <summary>获取或设置预估达人佣金支出（官方 <c>finderuin_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("finderuin_predict_commission")]
    public string? FinderuinPredictCommission { get; set; }

    /// <summary>获取或设置预估团长佣金支出（官方 <c>captain_predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("captain_predict_commission")]
    public string? CaptainPredictCommission { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>product_click_uv</c>）。</summary>
    [JsonPropertyName("product_click_uv")]
    public string? ProductClickUv { get; set; }

    /// <summary>获取或设置商品点击次数（官方 <c>product_click_cnt</c>）。</summary>
    [JsonPropertyName("product_click_cnt")]
    public string? ProductClickCnt { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public string? PayRefundGmv { get; set; }

    /// <summary>获取或设置成交退款人数（官方 <c>pay_refund_uv</c>）。</summary>
    [JsonPropertyName("pay_refund_uv")]
    public string? PayRefundUv { get; set; }

    /// <summary>获取或设置成交退款率（官方 <c>pay_refund_ratio</c>，官方类型列标注 null、示例为数值——按 double 建模）。</summary>
    [JsonPropertyName("pay_refund_ratio")]
    public double? PayRefundRatio { get; set; }

    /// <summary>获取或设置退款件数（官方 <c>refund_gmv</c>，官方说明原文「退款件数」照录，疑为官方笔误）。</summary>
    [JsonPropertyName("refund_gmv")]
    public string? RefundGmv { get; set; }

    /// <summary>获取或设置退款件数（官方 <c>refund_product_cnt</c>）。</summary>
    [JsonPropertyName("refund_product_cnt")]
    public string? RefundProductCnt { get; set; }

    /// <summary>获取或设置退款订单数（官方 <c>refund_cnt</c>）。</summary>
    [JsonPropertyName("refund_cnt")]
    public string? RefundCnt { get; set; }

    /// <summary>获取或设置退款人数（官方 <c>refund_uv</c>）。</summary>
    [JsonPropertyName("refund_uv")]
    public string? RefundUv { get; set; }

    /// <summary>获取或设置成交退款订单数（官方 <c>pay_refund_cnt</c>；官方变更日志 2025-11-24 新增）。</summary>
    [JsonPropertyName("pay_refund_cnt")]
    public string? PayRefundCnt { get; set; }
}

/// <summary>
/// 罗盘人群数据查询请求体（<c>getshopsaleprofiledata</c> / <c>getfindersaleprofiledata</c>）。
/// </summary>
/// <remarks>共用裁决：两端点请求体字段集完全一致（<c>ds</c> + <c>type</c>）⇒ 共用本 DTO；用户类型枚举两侧不同，分建常量类。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassSaleProfileRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置用户类型（官方 <c>type</c>，必填；商家版见 <see cref="ChannelsCompassShopSaleProfileUserTypes"/>、达人版见 <see cref="ChannelsCompassFinderSaleProfileUserTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}

/// <summary>获取店铺人群数据（<c>getshopsaleprofiledata</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopSaleProfileResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺人群数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopSaleProfile? Data { get; set; }
}

/// <summary>店铺人群数据（官方 <c>data</c> 对象，<c>getshopsaleprofiledata</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopSaleProfile
{
    /// <summary>获取或设置维度列表（官方 <c>field_list</c>）。</summary>
    [JsonPropertyName("field_list")]
    public List<ChannelsCompassShopSaleProfileField>? FieldList { get; set; }
}

/// <summary>人群维度类别（官方 <c>field_list</c> 数组元素，<c>getshopsaleprofiledata</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopSaleProfileField
{
    /// <summary>获取或设置维度类别名（官方 <c>field_name</c>，如 年龄/性别/省级/策略人群）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>获取或设置指标列表（官方 <c>data_list</c>）。</summary>
    [JsonPropertyName("data_list")]
    public List<ChannelsCompassShopSaleProfileDim>? DataList { get; set; }
}

/// <summary>人群维度指标（官方 <c>data_list</c> 数组元素，<c>getshopsaleprofiledata</c>，官方类型 string）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopSaleProfileDim
{
    /// <summary>获取或设置维度指标名（官方 <c>dim_key</c>）。</summary>
    [JsonPropertyName("dim_key")]
    public string? DimKey { get; set; }

    /// <summary>获取或设置维度指标值（官方 <c>dim_value</c>，官方类型 string）。</summary>
    [JsonPropertyName("dim_value")]
    public string? DimValue { get; set; }
}

/// <summary>罗盘商家版人群用户类型常量（官方 <c>type</c> 枚举值，<c>getshopsaleprofiledata</c>）。</summary>
public static class ChannelsCompassShopSaleProfileUserTypes
{
    /// <summary>商品曝光用户。</summary>
    public const int ProductExposureUser = 1;

    /// <summary>商品点击用户。</summary>
    public const int ProductClickUser = 2;

    /// <summary>购买用户。</summary>
    public const int PurchaseUser = 3;

    /// <summary>首购用户。</summary>
    public const int FirstPurchaseUser = 4;

    /// <summary>复购用户。</summary>
    public const int RepeatPurchaseUser = 5;
}
