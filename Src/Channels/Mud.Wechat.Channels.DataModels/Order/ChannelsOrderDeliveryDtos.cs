// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「发货协商 / 物流变更 / 生鲜质检 / 预约发货」DTO
// （order/deliveryinfo/update、order/deliverynegotiation/submit、order/deliverynegotiation/result/get、
//  order/freshinspect/submit、order/userbooking/list）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>生鲜质检审核项名称（官方 <c>freshinspect</c> 的 <c>audit_items[].item_name</c> 枚举）。</summary>
public static class ChannelsFreshInspectItemNames
{
    /// <summary>商品快递单图片 url。</summary>
    public const string ProductExpressPicUrl = "product_express_pic_url";

    /// <summary>商品包装箱全景视频 url。</summary>
    public const string ProductPackagingBoxPanoramicVideoUrl = "product_packaging_box_panoramic_video_url";

    /// <summary>商品开箱全景视频 url。</summary>
    public const string ProductUnboxingPanoramicVideoUrl = "product_unboxing_panoramic_video_url";

    /// <summary>商品单个细节全景视频 url。</summary>
    public const string SingleProductDetailPanoramicVideoUrl = "single_product_detail_panoramic_video_url";
}

/// <summary>生鲜质检审核项（官方 <c>order/freshinspect/submit</c> 的 <c>audit_items</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsFreshInspectAuditItem
{
    /// <summary>获取或设置审核项名称（官方 <c>item_name</c>，枚举值见 <see cref="ChannelsFreshInspectItemNames"/>）。</summary>
    [JsonPropertyName("item_name")]
    public string? ItemName { get; set; }

    /// <summary>获取或设置图片 / 视频 url（官方 <c>item_value</c>）。</summary>
    [JsonPropertyName("item_value")]
    public string? ItemValue { get; set; }
}

/// <summary>上传生鲜质检信息（<c>order/freshinspect/submit</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSubmitFreshInspectRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置审核项（官方 <c>audit_items</c>，必填）。</summary>
    [JsonPropertyName("audit_items")]
    public List<ChannelsFreshInspectAuditItem> AuditItems { get; set; } = new();
}

/// <summary>包裹新物流信息（官方 <c>order/deliveryinfo/update</c> 的 <c>new</c> / <c>old</c> 包对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDeliveryInfoPair
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，通过 获取快递公司列表 接口获得，非主流快递公司可填 OTHER）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置包裹中的商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsDeliveryProductInfo> ProductInfos { get; set; } = new();
}

/// <summary>更新包裹物流信息（官方 <c>order/deliveryinfo/update</c> 的 <c>change_infos</c> 数组元素，支持拆单）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDeliveryChangeInfo
{
    /// <summary>获取或设置包裹原物流信息（官方 <c>old</c>）。</summary>
    [JsonPropertyName("old")]
    public ChannelsOrderDeliveryInfoPair Old { get; set; } = new();

    /// <summary>获取或设置包裹新物流信息（官方 <c>new</c>）。</summary>
    [JsonPropertyName("new")]
    public ChannelsOrderDeliveryInfoPair New { get; set; } = new();
}

/// <summary>整单物流信息（官方 <c>order/deliveryinfo/update</c> 的 <c>delivery_list</c> 数组元素，不支持拆单）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDeliveryItem
{
    /// <summary>获取或设置发货方式（官方 <c>deliver_type</c>，1:自寄快递发货，目前仅支持 1）。</summary>
    [JsonPropertyName("deliver_type")]
    public int DeliverType { get; set; }

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，非主流快递公司可填 OTHER）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置包裹中的商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsDeliveryProductInfo> ProductInfos { get; set; } = new();
}

/// <summary>修改物流信息（<c>order/deliveryinfo/update</c>）请求体。</summary>
/// <remarks>官方业务限制：整单物流信息（<c>delivery_list</c>）不支持拆单，更新包裹物流信息（<c>change_infos</c>）支持拆单。</remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsUpdateDeliveryInfoRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填，可通过 获取订单列表 接口获取）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    /// <summary>获取或设置整单物流信息（官方 <c>delivery_list</c>，不支持拆单）。</summary>
    [JsonPropertyName("delivery_list")]
    public List<ChannelsOrderDeliveryItem>? DeliveryList { get; set; }

    /// <summary>获取或设置更新包裹物流信息（官方 <c>change_infos</c>，支持拆单）。</summary>
    [JsonPropertyName("change_infos")]
    public List<ChannelsOrderDeliveryChangeInfo>? ChangeInfos { get; set; }
}

/// <summary>提交发货协商申请（<c>order/deliverynegotiation/submit</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSubmitDeliveryNegotiationRequest
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置预计发货时间（官方 <c>predict_delivery_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("predict_delivery_time")]
    public long PredictDeliveryTime { get; set; }

    /// <summary>获取或设置协商原因（官方 <c>reason</c>，必填，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("reason")]
    public int Reason { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>，选填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>获取发货协商结果（<c>order/deliverynegotiation/result/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetDeliveryNegotiationResultRequest
{
    /// <summary>获取或设置订单 ID 数组（官方 <c>order_id_list</c>，必填）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<long> OrderIdList { get; set; } = new();
}

/// <summary>发货协商结果（官方 <c>order/deliverynegotiation/result/get</c> 的 <c>negotiation_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderNegotiation
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置原发货时间（官方 <c>original_delivery_time</c>，秒）。</summary>
    [JsonPropertyName("original_delivery_time")]
    public long? OriginalDeliveryTime { get; set; }

    /// <summary>获取或设置预计发货时间（官方 <c>predict_delivery_time</c>，秒）。</summary>
    [JsonPropertyName("predict_delivery_time")]
    public long? PredictDeliveryTime { get; set; }

    /// <summary>获取或设置状态（官方 <c>state</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置理由（官方 <c>reason</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("reason")]
    public int? Reason { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置截止处理时间（官方 <c>deadline</c>，秒）。</summary>
    [JsonPropertyName("deadline")]
    public long? Deadline { get; set; }

    /// <summary>获取或设置申请时间（官方 <c>create_time</c>，秒）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>获取发货协商结果（<c>order/deliverynegotiation/result/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetDeliveryNegotiationResultResponse : ChannelsResponse
{
    /// <summary>获取或设置协商结果列表（官方 <c>negotiation_list</c>）。</summary>
    [JsonPropertyName("negotiation_list")]
    public List<ChannelsOrderNegotiation>? NegotiationList { get; set; }
}

/// <summary>页码信息（官方 <c>order/userbooking/list</c> 的 <c>page_info</c> / <c>next_page</c> 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderPageInfo
{
    /// <summary>获取或设置偏移量（官方 <c>offset</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>获取或设置每页条数（官方 <c>limit</c>，默认为 20，不能超过 100）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，上一页请求返回）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>用户预约发货记录（官方 <c>order/userbooking/list</c> 的 <c>record_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsUserBookingRecord
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置预约类型（官方 <c>booking_type</c>，1=指定时间发货，2=暂不发货）。</summary>
    [JsonPropertyName("booking_type")]
    public long? BookingType { get; set; }

    /// <summary>获取或设置用户预约的发货时间（官方 <c>booking_predict_time</c>）。</summary>
    [JsonPropertyName("booking_predict_time")]
    public long? BookingPredictTime { get; set; }

    /// <summary>获取或设置原承诺发货时间（官方 <c>original_delivery_time</c>）。</summary>
    [JsonPropertyName("original_delivery_time")]
    public long? OriginalDeliveryTime { get; set; }

    /// <summary>获取或设置订单创建时间（官方 <c>order_create_time</c>）。</summary>
    [JsonPropertyName("order_create_time")]
    public long? OrderCreateTime { get; set; }

    /// <summary>获取或设置用户预约时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置记录更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>获取用户预约发货列表（<c>order/userbooking/list</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetUserBookingListRequest
{
    /// <summary>获取或设置订单 ID 数组（官方 <c>order_id_list</c>）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<long>? OrderIdList { get; set; }

    /// <summary>获取或设置用户预约类型（官方 <c>booking_type</c>，1=指定时间发货，2=暂不发货）。</summary>
    [JsonPropertyName("booking_type")]
    public long? BookingType { get; set; }

    /// <summary>获取或设置下单时间范围（官方 <c>time_range_order_create</c>，间隔不超过 7 天）。</summary>
    [JsonPropertyName("time_range_order_create")]
    public ChannelsOrderTimeRange TimeRangeOrderCreate { get; set; } = new();

    /// <summary>获取或设置预约发货时间范围（官方 <c>time_range_booking_predict</c>，间隔不超过 7 天）。</summary>
    [JsonPropertyName("time_range_booking_predict")]
    public ChannelsOrderTimeRange TimeRangeBookingPredict { get; set; } = new();

    /// <summary>获取或设置页码信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsOrderPageInfo? PageInfo { get; set; }
}

/// <summary>获取用户预约发货列表（<c>order/userbooking/list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetUserBookingListResponse : ChannelsResponse
{
    /// <summary>获取或设置预约记录列表（官方 <c>record_list</c>）。</summary>
    [JsonPropertyName("record_list")]
    public List<ChannelsUserBookingRecord>? RecordList { get; set; }

    /// <summary>获取或设置下一页信息（官方 <c>next_page</c>）。</summary>
    [JsonPropertyName("next_page")]
    public ChannelsOrderPageInfo? NextPage { get; set; }

    /// <summary>获取或设置记录总数（官方 <c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }
}