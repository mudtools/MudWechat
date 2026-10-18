// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 物流信息项（官方 <c>shipping_list</c> 元素）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShippingInfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderShipping
{
    /// <summary>物流单号（官方必填 <c>tracking_no</c>）。</summary>
    [JsonPropertyName("tracking_no")]
    public string? TrackingNumber { get; set; }

    /// <summary>物流公司编码（官方必填 <c>express_company</c>；取微信支付物流公司编码表）。</summary>
    [JsonPropertyName("express_company")]
    public string? ExpressCompany { get; set; }

    /// <summary>订单购买的商品列表（官方选填 <c>item_list</c>；元素仅需 <c>merchant_item_id</c>）。</summary>
    [JsonPropertyName("item_list")]
    public List<OpenPlatformUserOrderItem>? ItemList { get; set; }

    /// <summary>联系方式（官方选填 <c>contact</c>；顺丰速运必填）。</summary>
    [JsonPropertyName("contact")]
    public OpenPlatformUserOrderContact? Contact { get; set; }
}
