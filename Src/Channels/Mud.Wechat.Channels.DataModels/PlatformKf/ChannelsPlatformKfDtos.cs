// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>
/// 查询工单分类配置（<c>getticketclassconfig</c>）响应。
/// </summary>
/// <remarks>官方契约：<b>POST</b> + 空请求体；当前仅开放催发货一种工单分类类型（type 9）。所属权限集 id：131。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfGetTicketClassConfigResponse : ChannelsResponse
{
    /// <summary>获取或设置商家回复选项配置（官方 <c>mch_reply_options</c>）。</summary>
    [JsonPropertyName("mch_reply_options")]
    public ChannelsPlatformKfMchReplyOptions? MchReplyOptions { get; set; }
}

/// <summary>商家回复选项配置（官方 <c>mch_reply_options</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfMchReplyOptions
{
    /// <summary>获取或设置分类配置列表（官方 <c>conf_list</c>）。</summary>
    [JsonPropertyName("conf_list")]
    public List<ChannelsPlatformKfTicketClassConfig>? ConfList { get; set; }
}

/// <summary>工单分类配置（官方 <c>conf_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketClassConfig
{
    /// <summary>获取或设置工单分类类型（官方 <c>ticket_class_type</c>，见 <see cref="ChannelsPlatformKfTicketClassTypes"/>；当前仅开放催发货）。</summary>
    [JsonPropertyName("ticket_class_type")]
    public int? TicketClassType { get; set; }

    /// <summary>获取或设置工单分类名称（官方 <c>ticket_class_name</c>）。</summary>
    [JsonPropertyName("ticket_class_name")]
    public string? TicketClassName { get; set; }

    /// <summary>获取或设置可选回复配置（官方 <c>option_list</c>）。</summary>
    [JsonPropertyName("option_list")]
    public List<ChannelsPlatformKfTicketReplyOption>? OptionList { get; set; }
}

/// <summary>工单回复选项配置（官方 <c>option_list</c> 数组元素）。</summary>
/// <remarks><c>operatemchticket</c> 的 <c>reply_option</c> / <c>description</c> / <c>confer_time</c>
/// 必填性由本配置的 <c>option_id</c> / <c>is_must_description</c> / <c>is_must_confer_time</c> 决定。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketReplyOption
{
    /// <summary>获取或设置工单回复选项（官方 <c>option_id</c>，处理工单时作为 <c>reply_option</c> 回传）。</summary>
    [JsonPropertyName("option_id")]
    public int? OptionId { get; set; }

    /// <summary>获取或设置工单回复名称（官方 <c>option_name</c>）。</summary>
    [JsonPropertyName("option_name")]
    public string? OptionName { get; set; }

    /// <summary>获取或设置补充说明是否必填（官方 <c>is_must_description</c>）。</summary>
    [JsonPropertyName("is_must_description")]
    public bool? IsMustDescription { get; set; }

    /// <summary>获取或设置图片 / 视频凭证是否必填（官方 <c>is_must_urls</c>）。</summary>
    [JsonPropertyName("is_must_urls")]
    public bool? IsMustUrls { get; set; }

    /// <summary>获取或设置协商时间是否必填（官方 <c>is_must_confer_time</c>）。</summary>
    [JsonPropertyName("is_must_confer_time")]
    public bool? IsMustConferTime { get; set; }
}

/// <summary>获取商家工单列表（<c>getmchticketlist</c>）请求体。</summary>
/// <remarks>全字段选填；不传 <c>ticket_state</c> 返回所有状态；默认按创建时间降序；limit 默认 10，最大建议不超过 50。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfGetTicketListRequest
{
    /// <summary>获取或设置工单状态（官方 <c>ticket_state</c>，选填，见 <see cref="ChannelsPlatformKfTicketStates"/>；不传则返回所有状态）。</summary>
    [JsonPropertyName("ticket_state")]
    public int? TicketState { get; set; }

    /// <summary>获取或设置工单类型（官方 <c>ticket_class_type</c>，选填；当前仅支持「9:催发货」）。</summary>
    [JsonPropertyName("ticket_class_type")]
    public int? TicketClassType { get; set; }

    /// <summary>获取或设置搜索关键字（官方 <c>search_ticket_id</c>，选填；工单 id / 订单 id 任意其一）。</summary>
    [JsonPropertyName("search_ticket_id")]
    public string? SearchTicketId { get; set; }

    /// <summary>获取或设置查询起始时间（官方 <c>search_start_time</c>，选填，Unix 时间戳，秒——按工单创建时间过滤）。</summary>
    [JsonPropertyName("search_start_time")]
    public long? SearchStartTime { get; set; }

    /// <summary>获取或设置查询结束时间（官方 <c>search_end_time</c>，选填，Unix 时间戳，秒——按工单创建时间过滤）。</summary>
    [JsonPropertyName("search_end_time")]
    public long? SearchEndTime { get; set; }

    /// <summary>获取或设置排序方式（官方 <c>sort_order_type</c>，选填，见 <see cref="ChannelsPlatformKfSortOrderTypes"/>；默认按创建时间降序）。</summary>
    [JsonPropertyName("sort_order_type")]
    public int? SortOrderType { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>limit</c>，选填，默认 10，最大建议不超过 50）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置分页起始偏移量（官方 <c>start</c>，选填，从 0 开始）。</summary>
    [JsonPropertyName("start")]
    public int? Start { get; set; }
}

/// <summary>获取商家工单列表（<c>getmchticketlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfGetTicketListResponse : ChannelsResponse
{
    /// <summary>获取或设置工单列表（官方 <c>ticket_item_list</c>）。</summary>
    [JsonPropertyName("ticket_item_list")]
    public List<ChannelsPlatformKfTicketItem>? TicketItemList { get; set; }

    /// <summary>获取或设置工单总数（官方 <c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }
}

/// <summary>
/// 商家工单条目（官方 <c>ticket_item_list</c> 数组元素，<c>getmchticketlist</c>）——
/// 官方契约「getmchticketdetail 的 merchant_ticket 结构同工单列表元素」⇒ 共用同一 DTO。
/// </summary>
/// <remarks>时间字段均为 Unix 秒级时间戳。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketItem
{
    /// <summary>获取或设置商家工单 id（官方 <c>ticket_id</c>）。</summary>
    [JsonPropertyName("ticket_id")]
    public string? TicketId { get; set; }

    /// <summary>获取或设置工单分类类型（官方 <c>ticket_class_type</c>，见 <see cref="ChannelsPlatformKfTicketClassTypes"/>）。</summary>
    [JsonPropertyName("ticket_class_type")]
    public int? TicketClassType { get; set; }

    /// <summary>获取或设置工单状态（官方 <c>ticket_state</c>，见 <see cref="ChannelsPlatformKfTicketStates"/>）。</summary>
    [JsonPropertyName("ticket_state")]
    public int? TicketState { get; set; }

    /// <summary>获取或设置工单开始时间（官方 <c>ticket_begin_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("ticket_begin_time")]
    public long? TicketBeginTime { get; set; }

    /// <summary>获取或设置工单处理截止时间（官方 <c>ticket_deal_end_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("ticket_deal_end_time")]
    public long? TicketDealEndTime { get; set; }

    /// <summary>获取或设置商户处理时间（官方 <c>merchant_operate_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("merchant_operate_time")]
    public long? MerchantOperateTime { get; set; }

    /// <summary>获取或设置平台处理时间（官方 <c>platform_operate_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("platform_operate_time")]
    public long? PlatformOperateTime { get; set; }

    /// <summary>获取或设置工单问题描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置关联订单 id（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置关联售后 id（官方 <c>aftersale_id</c>）。</summary>
    [JsonPropertyName("aftersale_id")]
    public string? AftersaleId { get; set; }

    /// <summary>获取或设置平台审核结果（官方 <c>platform_review_result</c>，见 <see cref="ChannelsPlatformKfReviewResults"/>）。</summary>
    [JsonPropertyName("platform_review_result")]
    public int? PlatformReviewResult { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsPlatformKfTicketOrderInfo? OrderInfo { get; set; }

    /// <summary>获取或设置用户信息（官方 <c>user_info</c>）。</summary>
    [JsonPropertyName("user_info")]
    public ChannelsPlatformKfTicketUserInfo? UserInfo { get; set; }

    /// <summary>获取或设置商户处理记录（官方 <c>merchant_operate</c>）。</summary>
    [JsonPropertyName("merchant_operate")]
    public ChannelsPlatformKfTicketMerchantOperate? MerchantOperate { get; set; }

    /// <summary>获取或设置平台处理记录（官方 <c>platform_operate</c>）。</summary>
    [JsonPropertyName("platform_operate")]
    public ChannelsPlatformKfTicketPlatformOperate? PlatformOperate { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>工单关联订单信息（官方 <c>order_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketOrderInfo
{
    /// <summary>获取或设置小店订单 id（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置实付款（官方 <c>actual_price</c>，单位：分）。</summary>
    [JsonPropertyName("actual_price")]
    public long? ActualPrice { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>goods_list</c>）。</summary>
    [JsonPropertyName("goods_list")]
    public List<ChannelsPlatformKfTicketGoods>? GoodsList { get; set; }
}

/// <summary>工单关联订单商品（官方 <c>goods_list</c> 数组元素）。</summary>
/// <remarks><b>官方历史拼写照录</b>：字段名为 <c>good_name</c> / <c>good_count</c>（少一个 s），勿「顺手纠正」。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketGoods
{
    /// <summary>获取或设置商品名称（官方 <c>good_name</c>，官方历史拼写照录）。</summary>
    [JsonPropertyName("good_name")]
    public string? GoodName { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>good_count</c>，官方历史拼写照录）。</summary>
    [JsonPropertyName("good_count")]
    public int? GoodCount { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 SKU id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置商品缩略图 URL（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }
}

/// <summary>工单用户信息（官方 <c>user_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketUserInfo
{
    /// <summary>获取或设置用户问题描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>商户处理记录（官方 <c>merchant_operate</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketMerchantOperate
{
    /// <summary>获取或设置商家回复选项 id（官方 <c>merchant_reply</c>，对应查询工单分类配置返回的 option_id）。</summary>
    [JsonPropertyName("merchant_reply")]
    public int? MerchantReply { get; set; }

    /// <summary>获取或设置商家补充说明（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置商家与用户协商的时间（官方 <c>confer_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("confer_time")]
    public long? ConferTime { get; set; }
}

/// <summary>平台处理记录（官方 <c>platform_operate</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketPlatformOperate
{
    /// <summary>获取或设置平台审核结果（官方 <c>audit_result</c>，1-5，含义同 <see cref="ChannelsPlatformKfReviewResults"/>）。</summary>
    [JsonPropertyName("audit_result")]
    public int? AuditResult { get; set; }

    /// <summary>获取或设置对商家的说明（官方 <c>reason</c>，审核不通过原因）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置用户是否认可（官方 <c>is_agree</c>）。</summary>
    [JsonPropertyName("is_agree")]
    public int? IsAgree { get; set; }

    /// <summary>获取或设置回复用户话术（官方 <c>reply_description</c>）。</summary>
    [JsonPropertyName("reply_description")]
    public string? ReplyDescription { get; set; }

    /// <summary>获取或设置平台答复时间（官方 <c>platform_reply_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("platform_reply_time")]
    public long? PlatformReplyTime { get; set; }
}

/// <summary>查询工单详情（<c>getmchticketdetail</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfGetTicketDetailRequest
{
    /// <summary>获取或设置工单 id（官方 <c>ticket_id</c>，必填）。</summary>
    [JsonPropertyName("ticket_id")]
    public string TicketId { get; set; } = string.Empty;
}

/// <summary>查询工单详情（<c>getmchticketdetail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfGetTicketDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置工单详细信息（官方 <c>merchant_ticket</c>，官方契约：结构同工单列表元素 ⇒ 共用 <see cref="ChannelsPlatformKfTicketItem"/>）。</summary>
    [JsonPropertyName("merchant_ticket")]
    public ChannelsPlatformKfTicketItem? MerchantTicket { get; set; }
}

/// <summary>商户处理商家工单（<c>operatemchticket</c>）请求体。</summary>
/// <remarks><c>description</c> / <c>confer_time</c> 在对应选项（经「查询工单分类配置」取得）的
/// <c>is_must_description</c> / <c>is_must_confer_time</c> 为 true 时必填。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfOperateTicketRequest
{
    /// <summary>获取或设置工单 id（官方 <c>ticket_id</c>，必填）。</summary>
    [JsonPropertyName("ticket_id")]
    public string TicketId { get; set; } = string.Empty;

    /// <summary>获取或设置回复选项 id（官方 <c>reply_option</c>，必填；对应查询工单分类配置接口返回的 option_id）。</summary>
    [JsonPropertyName("reply_option")]
    public int ReplyOption { get; set; }

    /// <summary>获取或设置补充说明文字（官方 <c>description</c>；对应选项 is_must_description=true 时必填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置商家与用户协商的时间（官方 <c>confer_time</c>，Unix 时间戳，秒；对应选项 is_must_confer_time=true 时必填）。</summary>
    [JsonPropertyName("confer_time")]
    public long? ConferTime { get; set; }
}

/// <summary>工单状态常量（官方 <c>ticket_state</c> 枚举值）。</summary>
public static class ChannelsPlatformKfTicketStates
{
    /// <summary>待商家处理。</summary>
    public const int PendingMerchant = 1;

    /// <summary>待平台处理。</summary>
    public const int PendingPlatform = 2;

    /// <summary>已完结。</summary>
    public const int Closed = 3;
}

/// <summary>工单分类类型常量（官方 <c>ticket_class_type</c> 枚举值；当前仅开放催发货）。</summary>
public static class ChannelsPlatformKfTicketClassTypes
{
    /// <summary>催发货。</summary>
    public const int UrgeDelivery = 9;
}

/// <summary>工单列表排序方式常量（官方 <c>sort_order_type</c> 枚举值）。</summary>
public static class ChannelsPlatformKfSortOrderTypes
{
    /// <summary>按创建时间排序（默认降序）。</summary>
    public const int ByCreateTime = 1;

    /// <summary>按工单截止时间升序。</summary>
    public const int ByDealEndTimeAsc = 2;
}

/// <summary>平台审核结果常量（官方 <c>platform_review_result</c> 枚举值）。</summary>
public static class ChannelsPlatformKfReviewResults
{
    /// <summary>审核通过，协商一致支持用户。</summary>
    public const int ApprovedSupportUser = 1;

    /// <summary>审核不通过，默认判支持用户。</summary>
    public const int RejectedDefaultSupportUser = 2;

    /// <summary>平台介入处理，判支持商家。</summary>
    public const int IntervenedSupportMerchant = 3;

    /// <summary>平台接入处理，判支持用户。</summary>
    public const int IntervenedSupportUser = 4;

    /// <summary>平台关闭工单。</summary>
    public const int PlatformClosed = 5;
}
