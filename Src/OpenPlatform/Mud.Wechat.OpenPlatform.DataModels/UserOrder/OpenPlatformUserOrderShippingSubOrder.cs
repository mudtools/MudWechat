// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 合单子订单物流信息（官方 <c>sub_orders</c> 元素；上传合单物流信息）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadCombinedShippingInfo.html"/></para>
/// <para><c>delivery_mode</c> 官方取值 <c>UNIFIED_DELIVERY</c>（统一发货）或 <c>SPLIT_DELIVERY</c>（分拆发货）；
/// 统一发货仅允许一条物流，分拆发货可多条。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderShippingSubOrder
{
    /// <summary>子订单标识（官方必填 <c>order_key</c>）。</summary>
    [JsonPropertyName("order_key")]
    public OpenPlatformUserOrderKey? OrderKey { get; set; }

    /// <summary>发货模式（官方必填 <c>delivery_mode</c>；<c>UNIFIED_DELIVERY</c> 或 <c>SPLIT_DELIVERY</c>）。</summary>
    [JsonPropertyName("delivery_mode")]
    public string? DeliveryMode { get; set; }

    /// <summary>物流信息列表（官方 <c>shipping_list</c>）。</summary>
    [JsonPropertyName("shipping_list")]
    public List<OpenPlatformUserOrderShipping>? ShippingList { get; set; }
}
