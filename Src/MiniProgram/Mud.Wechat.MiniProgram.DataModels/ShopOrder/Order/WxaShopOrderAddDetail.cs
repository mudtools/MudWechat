// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「生成订单」的订单详细数据（官方写侧 <c>order_detail</c>：商品列表 + 支付信息 + 价格信息）。
/// </summary>
/// <remarks>
/// <para><b>写侧三支全必填</b>：官方参数表把 <c>product_infos</c> / <c>pay_info</c> / <c>price_info</c> 的子字段逐个标注必填，
/// 与读侧「有则回、无则缺」的形态不同。</para>
/// <para><b>价格自洽校验由官方执行</b>：<c>order_price = sum(sale_price) + freight - discounted_price</c>（官方原文）
/// ⇒ 金额不自洽时官方报错，宿主不得只改 order_price。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/add_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderAddDetail
{
    /// <summary>product_infos（官方类型 <c>object[]</c>，必填）：商品列表（写侧条目见 <see cref="WxaShopOrderAddProductInfo"/>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<WxaShopOrderAddProductInfo>? ProductInfos { get; set; }

    /// <summary>pay_info（官方类型 <c>object</c>，必填）：支付信息（写侧见 <see cref="WxaShopOrderAddPayInfo"/>）。</summary>
    [JsonPropertyName("pay_info")]
    public WxaShopOrderAddPayInfo? PayInfo { get; set; }

    /// <summary>price_info（官方类型 <c>object</c>，必填）：价格信息，金额单位分（与读侧同形，复用 <see cref="WxaShopPriceInfo"/>）。</summary>
    [JsonPropertyName("price_info")]
    public WxaShopPriceInfo? PriceInfo { get; set; }
}
