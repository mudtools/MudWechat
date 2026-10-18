// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单详细数据（官方 <c>OrderDetail</c> 表：商品列表 + 价格信息 + 支付信息三支）。
/// </summary>
/// <remarks>
/// <para><b>官方读侧比写侧多一支 <c>promotion_info</c></b>：两支按推广员/分享员列表的示例里 <c>order_detail.promotion_info</c> 出现，
/// 而 Order 表把 <c>promotion_info</c> 列在订单顶层 ⇒ 两处都可能存在，本模型按顶层建模、读侧示例差异留档。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderDetail
{
    /// <summary>product_infos（官方类型 <c>OrderProductInfo[]</c>）：商品列表。</summary>
    [JsonPropertyName("product_infos")]
    public List<WxaShopOrderProductInfo>? ProductInfos { get; set; }

    /// <summary>price_info（官方类型 <c>PriceInfo</c>）：价格信息（金额单位一律为分）。</summary>
    [JsonPropertyName("price_info")]
    public WxaShopPriceInfo? PriceInfo { get; set; }

    /// <summary>pay_info（官方类型 <c>PayInfo</c>）：支付信息。</summary>
    [JsonPropertyName("pay_info")]
    public WxaShopPayInfo? PayInfo { get; set; }
}
