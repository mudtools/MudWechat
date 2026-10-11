// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>获取文件下载链接（<c>get_download_url</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetDownloadUrlRequest
{
    /// <summary>获取或设置下单用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置用户下单的订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>获取文件下载链接（<c>get_download_url</c>）响应。</summary>
/// <remarks>官方契约：链接有效期 10 分钟；order_id 维度限频 10 次/天；订单已结算或超售后期 7 天不可获取。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetDownloadUrlResponse : ChannelsResponse
{
    /// <summary>获取或设置文件下载链接（官方 <c>url</c>；官方变更记录：原 download_url 字段已修正为 url）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置链接到期时间（官方 <c>expire_time</c>，秒级时间戳，有效期 10 分钟）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}

/// <summary>上传资料（<c>upload</c>）请求体（官方 JSON 形态：upload_type=1 文件 url 上传）。</summary>
/// <remarks><c>upload_type=0</c> 二进制上传官方为 multipart/form-data（openid 走 Query、文件走 form-data），
/// 不在 JSON 请求体语义内；URL 上传支持 &lt;50M 文件。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreUploadMaterialRequest
{
    /// <summary>获取或设置上传资料的用户 openid（官方 <c>openid</c>，必填——upload_type=1 时走请求体）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置上传文件的 url（官方 <c>file_url</c>，upload_type=1 时必填，支持 &lt;50M 的文件）。</summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }
}

/// <summary>上传资料（<c>upload</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreUploadMaterialResponse : ChannelsResponse
{
    /// <summary>获取或设置文件 media_id（官方 <c>media_id</c>，用于后续定制商品发货下发）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>查询小店礼物活动列表（<c>activity/list/promoter/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetActivityListRequest
{
    /// <summary>获取或设置偏移量（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置单次请求个数（官方 <c>limit</c>，必填，最大为 100）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>，选填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置活动状态（官方 <c>status</c>，选填，见 <see cref="ChannelsMiniStoreActivityStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置类型（官方 <c>type</c>，选填，见 <see cref="ChannelsMiniStoreActivityTypes"/>；默认为 0）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}

/// <summary>小店礼物活动条目（官方 <c>list</c> 数组元素，<c>activity/list/promoter/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreActivityItem
{
    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置小店昵称（官方 <c>shop_nickname</c>）。</summary>
    [JsonPropertyName("shop_nickname")]
    public string? ShopNickname { get; set; }

    /// <summary>获取或设置小店活动 id（官方 <c>activity_id</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public long? ActivityId { get; set; }

    /// <summary>获取或设置活动状态（官方 <c>status</c>，见 <see cref="ChannelsMiniStoreActivityStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置类型（官方 <c>type</c>，见 <see cref="ChannelsMiniStoreActivityTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}

/// <summary>查询小店礼物活动列表（<c>activity/list/promoter/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetActivityListResponse : ChannelsResponse
{
    /// <summary>获取或设置信息列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsMiniStoreActivityItem>? List { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>查询礼物活动详情（<c>activity/info/promoter/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetActivityRequest
{
    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppId { get; set; } = string.Empty;

    /// <summary>获取或设置小店活动 id（官方 <c>activity_id</c>，必填）。</summary>
    [JsonPropertyName("activity_id")]
    public long ActivityId { get; set; }
}

/// <summary>查询礼物活动详情（<c>activity/info/promoter/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetActivityResponse : ChannelsResponse
{
    /// <summary>获取或设置活动信息（官方 <c>activity</c>）。</summary>
    [JsonPropertyName("activity")]
    public ChannelsMiniStoreActivity? Activity { get; set; }
}

/// <summary>礼物活动信息（官方 <c>activity</c> 对象，<c>activity/info/promoter/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreActivity
{
    /// <summary>获取或设置活动 id（官方 <c>activity_id</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public long? ActivityId { get; set; }

    /// <summary>获取或设置活动状态（官方 <c>status</c>，见 <see cref="ChannelsMiniStoreActivityStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置活动信息（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsMiniStoreActivityInfo? Info { get; set; }
}

/// <summary>礼物活动信息明细（官方 <c>activity.info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreActivityInfo
{
    /// <summary>获取或设置活动基础信息（官方 <c>basic_info</c>）。</summary>
    [JsonPropertyName("basic_info")]
    public ChannelsMiniStoreActivityBasicInfo? BasicInfo { get; set; }

    /// <summary>获取或设置活动礼物商品信息（官方 <c>prize_info</c>）。</summary>
    [JsonPropertyName("prize_info")]
    public ChannelsMiniStoreActivityPrizeInfo? PrizeInfo { get; set; }
}

/// <summary>礼物活动基础信息（官方 <c>basic_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreActivityBasicInfo
{
    /// <summary>获取或设置活动开始时间（官方 <c>activity_start_time</c>，单位为秒）。</summary>
    [JsonPropertyName("activity_start_time")]
    public long? ActivityStartTime { get; set; }

    /// <summary>获取或设置活动结束时间（官方 <c>activity_end_time</c>，单位为秒）。</summary>
    [JsonPropertyName("activity_end_time")]
    public long? ActivityEndTime { get; set; }

    /// <summary>获取或设置小店信息（官方 <c>shop_info</c>）。</summary>
    [JsonPropertyName("shop_info")]
    public ChannelsMiniStoreShopInfo? ShopInfo { get; set; }
}

/// <summary>礼物活动小店信息（官方 <c>shop_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreShopInfo
{
    /// <summary>获取或设置小店昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置小店 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>活动礼物商品信息（官方 <c>prize_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreActivityPrizeInfo
{
    /// <summary>获取或设置商品信息列表（官方 <c>product_info_list</c>）。</summary>
    [JsonPropertyName("product_info_list")]
    public List<ChannelsMiniStoreActivityProductInfo>? ProductInfoList { get; set; }

    /// <summary>获取或设置礼物订单号（官方 <c>present_order_id</c>，自购类型才有）。</summary>
    [JsonPropertyName("present_order_id")]
    public long? PresentOrderId { get; set; }
}

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

/// <summary>指定礼物收礼者（<c>presentorder/receiver/set</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreSetPresentReceiverRequest
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>，必填）。</summary>
    [JsonPropertyName("present_order_id")]
    public string PresentOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置礼物单子单列表（官方 <c>sub_order_list</c>，必填，不可为空）。</summary>
    [JsonPropertyName("sub_order_list")]
    public List<ChannelsMiniStorePresentSubOrder> SubOrderList { get; set; } = new();
}

/// <summary>礼物单子单（指定收礼者形态：官方 <c>sub_order_list</c> 数组元素，<c>presentorder/receiver/set</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresentSubOrder
{
    /// <summary>获取或设置礼物单子单订单 ID（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置收礼者 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;
}

/// <summary>创建并发送礼物（<c>presentorder/create</c>）请求体。</summary>
/// <remarks>祝福语（wishmessage）最大 100 字符；idempotent_id 填写时相同幂等 id 的并发请求返回 6011104。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreCreatePresentOrderRequest
{
    /// <summary>获取或设置活动 id（官方 <c>activity_id</c>，必填）。</summary>
    [JsonPropertyName("activity_id")]
    public long ActivityId { get; set; }

    /// <summary>获取或设置收礼者用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置合作小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppId { get; set; } = string.Empty;

    /// <summary>获取或设置礼物祝福语信息（官方 <c>wishmessage</c>，必填，最大 100 字符）。</summary>
    [JsonPropertyName("wishmessage")]
    public string WishMessage { get; set; } = string.Empty;

    /// <summary>获取或设置指定活动商品（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置指定活动 sku（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public long SkuId { get; set; }

    /// <summary>获取或设置幂等 id（官方 <c>idempotent_id</c>，选填）。</summary>
    [JsonPropertyName("idempotent_id")]
    public string? IdempotentId { get; set; }
}

/// <summary>创建并发送礼物（<c>presentorder/create</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreCreatePresentOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置礼物单 id（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }

    /// <summary>获取或设置外部 app 跳转业务类型（官方 <c>business_type</c>，仅外部应用身份调用返回）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>获取或设置外部 app 跳转参数（官方 <c>query</c>，仅外部应用身份调用返回）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }
}

/// <summary>
/// 时间范围（官方 <c>create_time_range</c> / <c>update_time_range</c> 对象）——
/// <c>presentlist/get</c> 与 <c>receiverorderlist/get</c> 两端点的两种时间范围字段集完全一致
/// （start_time / end_time，均必填且跨度不可超过 7 天）⇒ 共用同一 DTO。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreTimeRange
{
    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，必填，秒级时间戳，距离 end_time 不可超过 7 天）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，必填，秒级时间戳，距离 start_time 不可超过 7 天）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }
}

/// <summary>查询礼物订单列表（<c>presentlist/get</c>）请求体。</summary>
/// <remarks>创建 / 更新时间范围至少填一个，跨度均不可超过 7 天；page_size 不超过 100；游标翻页（next_key，首次置空）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetPresentListRequest
{
    /// <summary>获取或设置礼物单创建时间范围（官方 <c>create_time_range</c>，选填）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsMiniStoreTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置礼物单更新时间范围（官方 <c>update_time_range</c>，选填）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsMiniStoreTimeRange? UpdateTimeRange { get; set; }

    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppId { get; set; } = string.Empty;

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，选填，上一页请求返回，首次置空即可）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>礼物单 id 引用（官方 <c>present_order_list</c> 数组元素，<c>presentlist/get</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：官方将 <c>present_order_list</c> 声明为 objarray 且返回示例以对象形式
/// （含 present_order_id）展示——SDK 按元素对象形态建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresentOrderRef
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }
}

/// <summary>查询礼物订单列表（<c>presentlist/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetPresentListResponse : ChannelsResponse
{
    /// <summary>获取或设置礼物单列表（官方 <c>present_order_list</c>）。</summary>
    [JsonPropertyName("present_order_list")]
    public List<ChannelsMiniStorePresentOrderRef>? PresentOrderList { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，下一页请求时传入）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>查询收礼者订单列表（<c>receiverorderlist/get</c>）请求体。</summary>
/// <remarks>时间范围跨度不可超过 7 天；page_size 不超过 100；游标翻页（next_key）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetReceiverOrderListRequest
{
    /// <summary>获取或设置订单创建时间范围（官方 <c>create_time_range</c>，选填）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsMiniStoreTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置订单更新时间范围（官方 <c>update_time_range</c>，选填）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsMiniStoreTimeRange? UpdateTimeRange { get; set; }

    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppId { get; set; } = string.Empty;

    /// <summary>获取或设置收礼者 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，选填，上一页请求返回，首次置空即可）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>收礼者订单条目（官方 <c>order_list</c> 数组元素，<c>receiverorderlist/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreReceiverOrderItem
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }

    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>查询收礼者订单列表（<c>receiverorderlist/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetReceiverOrderListResponse : ChannelsResponse
{
    /// <summary>获取或设置订单列表（官方 <c>order_list</c>）。</summary>
    [JsonPropertyName("order_list")]
    public List<ChannelsMiniStoreReceiverOrderItem>? OrderList { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>查询礼物订单详情（<c>present/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetPresentRequest
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>，必填）。</summary>
    [JsonPropertyName("present_order_id")]
    public string PresentOrderId { get; set; } = string.Empty;
}

/// <summary>查询礼物订单详情（<c>present/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetPresentResponse : ChannelsResponse
{
    /// <summary>获取或设置礼物单结构（官方 <c>present</c>）。</summary>
    [JsonPropertyName("present")]
    public ChannelsMiniStorePresent? Present { get; set; }
}

/// <summary>礼物单结构（官方 <c>present</c> 对象，<c>present/get</c>）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：<c>is_b2c_free_present</c> 官方声明 boolean 类型但描述为
/// 「0：已付款 1：无需付款」——SDK 按声明类型 boolean 建模，0/1 描述照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresent
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置礼物单详情（官方 <c>present_detail</c>）。</summary>
    [JsonPropertyName("present_detail")]
    public ChannelsMiniStorePresentDetail? PresentDetail { get; set; }

    /// <summary>获取或设置礼物单中的子单列表（官方 <c>sub_order_list</c>）。</summary>
    [JsonPropertyName("sub_order_list")]
    public List<ChannelsMiniStorePresentSubOrderDetail>? SubOrderList { get; set; }

    /// <summary>获取或设置是否无需付款（官方 <c>is_b2c_free_present</c>；官方描述照录「0：已付款 1：无需付款」）。</summary>
    [JsonPropertyName("is_b2c_free_present")]
    public bool? IsB2cFreePresent { get; set; }

    /// <summary>获取或设置礼物单关联的活动 ID（官方 <c>activity_id</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }
}

/// <summary>礼物单详情（官方 <c>present_detail</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresentDetail
{
    /// <summary>获取或设置价格信息（官方 <c>price_info</c>）。</summary>
    [JsonPropertyName("price_info")]
    public ChannelsMiniStorePresentPriceInfo? PriceInfo { get; set; }

    /// <summary>获取或设置支付信息（官方 <c>pay_info</c>）。</summary>
    [JsonPropertyName("pay_info")]
    public ChannelsMiniStorePresentPayInfo? PayInfo { get; set; }

    /// <summary>获取或设置祝福语（官方 <c>wishmessage</c>）。</summary>
    [JsonPropertyName("wishmessage")]
    public string? WishMessage { get; set; }
}

/// <summary>礼物单价格信息（官方 <c>price_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresentPriceInfo
{
    /// <summary>获取或设置礼物单价格（官方 <c>order_price</c>，单位为分）。</summary>
    [JsonPropertyName("order_price")]
    public long? OrderPrice { get; set; }
}

/// <summary>礼物单支付信息（官方 <c>pay_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresentPayInfo
{
    /// <summary>获取或设置礼物单支付时间（官方 <c>pay_time</c>）。</summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }
}

/// <summary>
/// 礼物单子单（详情形态：官方 <c>sub_order_list</c> 数组元素，<c>present/get</c>）——
/// 与指定收礼者形态（<see cref="ChannelsMiniStorePresentSubOrder"/>）相比多出 <c>status</c>，
/// 字段集不一致 ⇒ 按端点分建。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresentSubOrderDetail
{
    /// <summary>获取或设置子单订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置收礼者 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置子单订单状态（官方 <c>status</c>，见 <see cref="ChannelsMiniStorePresentSubOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>指定礼物单祝福语（<c>presentorder/wishmessage/set</c>）请求体。</summary>
/// <remarks>仅可在「指定礼物收礼者」之前修改；中文 ≤ 20 字 / 英文 ≤ 40 字符（6080091）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreSetPresentWishMessageRequest
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>，必填）。</summary>
    [JsonPropertyName("present_order_id")]
    public string PresentOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置祝福语信息（官方 <c>wish_message</c>，必填；中文 ≤ 20 字 / 英文 ≤ 40 字符）。</summary>
    [JsonPropertyName("wish_message")]
    public string WishMessage { get; set; } = string.Empty;
}

/// <summary>上传方式常量（官方 <c>upload_type</c> 枚举值，<c>upload</c>）。</summary>
public static class ChannelsMiniStoreUploadTypes
{
    /// <summary>上传文件二进制（multipart/form-data，文件 &lt;10M）。</summary>
    public const int Binary = 0;

    /// <summary>文件 url 上传（JSON 请求体，文件 &lt;50M）。</summary>
    public const int Url = 1;
}

/// <summary>礼物活动状态常量（官方 <c>status</c> 枚举值）。</summary>
public static class ChannelsMiniStoreActivityStatuses
{
    /// <summary>未开始。</summary>
    public const int NotStarted = 1;

    /// <summary>进行中。</summary>
    public const int InProgress = 2;

    /// <summary>已开奖。</summary>
    public const int Drawn = 4;

    /// <summary>失效。</summary>
    public const int Invalid = 5;
}

/// <summary>礼物活动类型常量（官方 <c>type</c> 枚举值）。</summary>
public static class ChannelsMiniStoreActivityTypes
{
    /// <summary>小店授权。</summary>
    public const int ShopAuthorized = 0;

    /// <summary>自购。</summary>
    public const int SelfPurchase = 1;

    /// <summary>全部。</summary>
    public const int All = 2;
}

/// <summary>礼物单子单订单状态常量（官方 <c>status</c> 枚举值，<c>present/get</c>）。</summary>
public static class ChannelsMiniStorePresentSubOrderStatuses
{
    /// <summary>礼物待收下。</summary>
    public const int PendingAccept = 12;

    /// <summary>待发货。</summary>
    public const int PendingDelivery = 20;

    /// <summary>部分发货。</summary>
    public const int PartialDelivery = 21;

    /// <summary>待收货。</summary>
    public const int PendingReceipt = 30;

    /// <summary>完成。</summary>
    public const int Completed = 100;

    /// <summary>礼物单过期。</summary>
    public const int Expired = 180;

    /// <summary>全部商品售后之后，订单取消。</summary>
    public const int CancelledAfterAftersale = 200;
}
