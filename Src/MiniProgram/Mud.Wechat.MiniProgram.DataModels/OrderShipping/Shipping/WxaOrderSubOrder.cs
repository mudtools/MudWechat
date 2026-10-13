// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「子单物流详情」（合单录入的 <c>sub_orders</c> 数组元素）。
/// </summary>
/// <remarks>
/// <para><b>子单的 <c>order_key</c> 类型必须与合单主单一致</b>（官方原文）：<c>order_number_type</c>
/// 两侧取值不同即报错，不得在同一请求里混用「商户侧单号」与「微信支付单号」两种形式。</para>
/// <para><b>与主单请求体的差别</b>：合单侧的 <c>shipping_list</c> 官方标「否」（主单标「是」），
/// 且物流单只挂在子单上 ⇒ 本域唯一的「同一层结构在两支端点必填性不同」的位置。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadcombinedshippinginfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderSubOrder
{
    /// <summary>order_key（官方类型 <c>object</c>，必填）：需要上传物流详情的子单订单，订单类型与合单订单保持一致</summary>
    [JsonPropertyName("order_key")]
    public WxaOrderKey? OrderKey { get; set; }

    /// <summary>logistics_type（官方类型 <c>number</c>，必填）：物流模式，1、实体物流配送 2、同城配送 3、虚拟商品 4、用户自提。取值见 <see cref="WxaOrderLogisticsTypeValues"/></summary>
    [JsonPropertyName("logistics_type")]
    public long? LogisticsType { get; set; }

    /// <summary>delivery_mode（官方类型 <c>number</c>，必填）：发货模式，1、统一发货 2、分拆发货。取值见 <see cref="WxaOrderDeliveryModeValues"/></summary>
    [JsonPropertyName("delivery_mode")]
    public long? DeliveryMode { get; set; }

    /// <summary>is_all_delivered（官方类型 <c>boolean</c>）：分拆发货模式时必填，用于标识分拆发货模式下是否已全部发货完成。</summary>
    [JsonPropertyName("is_all_delivered")]
    public bool? IsAllDelivered { get; set; }

    /// <summary>shipping_list（官方类型 <c>objarray</c>）：子单物流信息列表，多重性: [1, 15]</summary>
    [JsonPropertyName("shipping_list")]
    public List<WxaOrderShippingItem>? ShippingList { get; set; }
}
