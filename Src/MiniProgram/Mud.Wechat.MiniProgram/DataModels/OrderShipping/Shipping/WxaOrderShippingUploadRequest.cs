// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「发货信息录入」请求体（<c>POST /wxa/sec/order/upload_shipping_info</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>upload_time</c> 是 RFC 3339 字符串而非时间戳</b>（官方类型 <c>string</c>，示例
/// <c>2022-12-15T13:29:35.120+08:00</c>）：与查询侧物流单的 <c>upload_time</c>（<c>number</c> 时间戳）
/// <b>同名不同类型</b>，是本域最易踩的形态差异；且更新物流信息时该值必须比上一次更晚。</para>
/// <para><b>重新发货机会只有一次</b>（官方原文）：对已完成发货的支付单再次调用、或用本接口修改发货模式／物流模式，
/// 均计为「重新发货」，每笔支付单仅一次机会 ⇒ 宿主不得把本接口当幂等重试通道。</para>
/// <para><b>分拆发货约束</b>：<c>is_all_delivered</c> 官方标选填，原文为「分拆发货模式时必填」；
/// 分拆发货仅支持物流快递，一笔支付单最多分拆 15 个包裹（<c>shipping_list</c> 多重性 [1, 15]）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderShippingUploadRequest
{
    /// <summary>order_key（官方类型 <c>object</c>，必填）：订单，需要上传物流信息的订单</summary>
    [JsonPropertyName("order_key")]
    public WxaOrderKey? OrderKey { get; set; }

    /// <summary>logistics_type（官方类型 <c>number</c>，必填）：物流模式，1、实体物流配送 2、同城配送 3、虚拟商品 4、用户自提。取值见 <see cref="WxaOrderLogisticsTypeValues"/></summary>
    [JsonPropertyName("logistics_type")]
    public long? LogisticsType { get; set; }

    /// <summary>delivery_mode（官方类型 <c>number</c>，必填）：发货模式，1、UNIFIED_DELIVERY（统一发货）2、SPLIT_DELIVERY（分拆发货）。取值见 <see cref="WxaOrderDeliveryModeValues"/></summary>
    [JsonPropertyName("delivery_mode")]
    public long? DeliveryMode { get; set; }

    /// <summary>is_all_delivered（官方类型 <c>boolean</c>）：分拆发货模式时必填，用于标识分拆发货模式下是否已全部发货完成，只有全部发货完成的情况下才会向用户推送发货完成通知。</summary>
    [JsonPropertyName("is_all_delivered")]
    public bool? IsAllDelivered { get; set; }

    /// <summary>shipping_list（官方类型 <c>objarray</c>，必填）：物流信息列表，支持统一发货（单个物流单）和分拆发货（多个物流单）两种模式，多重性: [1, 15]</summary>
    [JsonPropertyName("shipping_list")]
    public List<WxaOrderShippingItem>? ShippingList { get; set; }

    /// <summary>upload_time（官方类型 <c>string</c>，必填）：上传时间，用于标识请求的先后顺序，RFC 3339 格式（示例 2022-12-15T13:29:35.120+08:00）</summary>
    [JsonPropertyName("upload_time")]
    public string? UploadTime { get; set; }

    /// <summary>payer（官方类型 <c>object</c>，必填）：支付者，支付者信息</summary>
    [JsonPropertyName("payer")]
    public WxaOrderPayer? Payer { get; set; }
}
