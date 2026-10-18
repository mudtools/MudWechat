// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 上传物流信息请求（<c>POST /user-order/orders/shippings</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShippingInfo.html"/></para>
/// <para>官方约束：仅支付完成后可调用；<c>upload_time</c> 为 RFC3339 时间串；
/// 顺丰速运必须填写 <c>contact</c>；<c>delivery_mode</c> 支持 <c>UNIFIED_DELIVERY</c> / <c>SPLIT_DELIVERY</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderShippingUploadRequest
{
    /// <summary>订单标识（官方必填 <c>order_key</c>）。</summary>
    [JsonPropertyName("order_key")]
    public OpenPlatformUserOrderKey? OrderKey { get; set; }

    /// <summary>发货模式（官方必填 <c>delivery_mode</c>；<c>UNIFIED_DELIVERY</c> 或 <c>SPLIT_DELIVERY</c>）。</summary>
    [JsonPropertyName("delivery_mode")]
    public string? DeliveryMode { get; set; }

    /// <summary>物流信息列表（官方必填 <c>shipping_list</c>）。</summary>
    [JsonPropertyName("shipping_list")]
    public List<OpenPlatformUserOrderShipping>? ShippingList { get; set; }

    /// <summary>上传时间（官方选填 <c>upload_time</c>，RFC3339 字符串）。</summary>
    [JsonPropertyName("upload_time")]
    public string? UploadTime { get; set; }

    /// <summary>支付者信息（官方选填 <c>payer</c>）。</summary>
    [JsonPropertyName("payer")]
    public OpenPlatformUserOrderPayer? Payer { get; set; }
}
