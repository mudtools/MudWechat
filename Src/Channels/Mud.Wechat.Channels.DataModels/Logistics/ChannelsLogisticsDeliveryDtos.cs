// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「订单发货」类 DTO（order/delivery/send、order/deliverycompanylist/new/get、
// order/delivery/compensation）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>课程地址（官方 <c>delivery_list[].course_info.course_path</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsCoursePath
{
    /// <summary>获取或设置课程地址类型（官方 <c>type</c>，0：小程序地址）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置小程序 appid（官方 <c>wxa_appid</c>）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string? WxaAppid { get; set; }

    /// <summary>获取或设置小程序地址（官方 <c>wxa_path</c>）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? WxaPath { get; set; }
}

/// <summary>课程相关信息（官方 <c>delivery_list[].course_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsCourseInfo
{
    /// <summary>获取或设置课程开始时间（官方 <c>start_time</c>，时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置课程结束时间（官方 <c>end_time</c>，时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置课程地址（官方 <c>course_path</c>）。</summary>
    [JsonPropertyName("course_path")]
    public ChannelsLogisticsCoursePath? CoursePath { get; set; }
}

/// <summary>商品 SN 码信息（官方 <c>delivery_list[].sn_info</c>，部分订单如无人机/国补订单需传入 SN 码）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsDeliverySnInfo
{
    /// <summary>获取或设置商品 SN 码（官方 <c>sn_code</c>）。</summary>
    [JsonPropertyName("sn_code")]
    public string? SnCode { get; set; }

    /// <summary>获取或设置 IMEI 码（官方 <c>imei1</c>）。</summary>
    [JsonPropertyName("imei1")]
    public string? Imei1 { get; set; }

    /// <summary>获取或设置 IMEI 码（官方 <c>imei2</c>）。</summary>
    [JsonPropertyName("imei2")]
    public string? Imei2 { get; set; }
}

/// <summary>包裹中的商品信息（官方 <c>delivery_list[].product_infos</c>，发货 / 补发货共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsDeliveryProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 sku（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置商品数量（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public int ProductCnt { get; set; }
}

/// <summary>订单发货物流信息（官方 <c>delivery_list</c>，发货 / 补发货共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsDeliveryItem
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，deliver_type=1 时必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，经获取快递公司列表获得，非主流可填 OTHER，deliver_type=1 时必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置包裹中的商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsLogisticsDeliveryProductInfo> ProductInfos { get; set; } = new();

    /// <summary>
    /// 获取或设置发货方式（官方 <c>deliver_type</c>）：1 自寄快递发货；3 虚拟商品无需物流发货（仅 deliver_method=1 的订单可用，senddelivery）；
    /// 补发货场景：1 第三方快递发货；6 自建物流发货。
    /// </summary>
    [JsonPropertyName("deliver_type")]
    public int DeliverType { get; set; }

    /// <summary>获取或设置课程相关信息（官方 <c>course_info</c>）。</summary>
    [JsonPropertyName("course_info")]
    public ChannelsLogisticsCourseInfo? CourseInfo { get; set; }

    /// <summary>获取或设置商品 SN 码信息（官方 <c>sn_info</c>）。</summary>
    [JsonPropertyName("sn_info")]
    public ChannelsLogisticsDeliverySnInfo? SnInfo { get; set; }
}

/// <summary>订单发货（<c>order/delivery/send</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsSendDeliveryRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置物流信息（官方 <c>delivery_list</c>，必填）。</summary>
    [JsonPropertyName("delivery_list")]
    public List<ChannelsLogisticsDeliveryItem> DeliveryList { get; set; } = new();
}

/// <summary>获取快递公司列表（<c>order/deliverycompanylist/new/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetDeliveryCompanyListRequest
{
    /// <summary>获取或设置是否仅返回支持电子面单功能的快递公司（官方 <c>ewaybill_only</c>，必填）。</summary>
    [JsonPropertyName("ewaybill_only")]
    public bool EwaybillOnly { get; set; }
}

/// <summary>快递公司列表项（官方 <c>company_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsCompany
{
    /// <summary>获取或设置快递公司 ID（官方 <c>delivery_id</c>，如 SF / ZTO 等）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置快递公司名称（官方 <c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}

/// <summary>获取快递公司列表（<c>order/deliverycompanylist/new/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetDeliveryCompanyListResponse : ChannelsResponse
{
    /// <summary>获取或设置快递公司列表（官方 <c>company_list</c>）。</summary>
    [JsonPropertyName("company_list")]
    public List<ChannelsLogisticsCompany>? CompanyList { get; set; }
}

/// <summary>订单补发货（<c>order/delivery/compensation</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsDeliveryCompensationRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置物流信息（官方 <c>delivery_list</c>，必填）。</summary>
    [JsonPropertyName("delivery_list")]
    public List<ChannelsLogisticsDeliveryItem> DeliveryList { get; set; } = new();

    /// <summary>获取或设置补发原因（官方 <c>reason</c>，必填）：1 商品漏发；2 商品拆分包裹；3 商品坏损；4 赠品。</summary>
    [JsonPropertyName("reason")]
    public int Reason { get; set; }
}
