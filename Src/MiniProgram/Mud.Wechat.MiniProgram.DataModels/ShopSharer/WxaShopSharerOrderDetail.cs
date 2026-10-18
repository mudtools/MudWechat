// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 分享员直播间订单明细（<c>orders[].order_detail</c>，五段：推广信息 / 商品 / 支付 / 价格 / 发货）。
/// </summary>
/// <remarks>
/// <para><b>本结构与交易组件「订单查询」域（<c>/shop/order/*</c>）的订单结构同形但不同源</b>：本域各类型独立建在
/// <c>DataModels.ShopSharer</c> 命名空间并登记进 <c>ShopSharerJsonContext</c>，<b>不得</b>跨域复用类型
/// （AOT 序列化按域上下文登记，跨域引用会破坏登记面，守卫 SHOPSH6 锁定）。</para>
/// <para><b>敏感面</b>：本对象树含支付凭据与收货/发货明细，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerOrderDetail
{
    /// <summary>推广信息（<c>promotion_info</c>，object），见 <see cref="WxaShopSharerPromotionInfo"/>。</summary>
    [JsonPropertyName("promotion_info")]
    public WxaShopSharerPromotionInfo? PromotionInfo { get; set; }

    /// <summary>商品信息列表（<c>product_infos</c>，object array），见 <see cref="WxaShopSharerProductInfo"/>。</summary>
    [JsonPropertyName("product_infos")]
    public List<WxaShopSharerProductInfo>? ProductInfos { get; set; }

    /// <summary>支付信息（<c>pay_info</c>，object），见 <see cref="WxaShopSharerPayInfo"/>。</summary>
    [JsonPropertyName("pay_info")]
    public WxaShopSharerPayInfo? PayInfo { get; set; }

    /// <summary>价格信息（<c>price_info</c>，object），见 <see cref="WxaShopSharerPriceInfo"/>；金额一律单位分。</summary>
    [JsonPropertyName("price_info")]
    public WxaShopSharerPriceInfo? PriceInfo { get; set; }

    /// <summary>发货信息（<c>delivery_detail</c>，object），见 <see cref="WxaShopSharerDeliveryDetail"/>。</summary>
    [JsonPropertyName("delivery_detail")]
    public WxaShopSharerDeliveryDetail? DeliveryDetail { get; set; }
}
