// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 小程序会员族（静默关联取会员信息 / 静默关联通知 / 下单页初始权益 /
// 使用会员权益 / 取消使用会员权益；回调的是小程序的回调 URL）。
// 事件键：channels_ec_vip_user_info_get、channels_ec_vip_user_auto_add、channels_ec_gen_order_info_init、
// channels_ec_order_use_vip_discount、channels_ec_order_unuse_vip_discount。
// 注意：info_get / gen_order_info_init / use_vip_discount 三事件官方要求返回结构化 JSON（非 success 文本）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>下单页商品信息（官方 <c>gen_order_info.product_infos</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsGenOrderProductInfo
{
    /// <summary>获取或设置商品 spu id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置 sku 数量（官方 <c>sku_cnt</c>）。</summary>
    [JsonPropertyName("sku_cnt")]
    public long? SkuCnt { get; set; }

    /// <summary>获取或设置售卖单价，单位分（官方 <c>sale_price</c>）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置商家优惠金额，单位分（官方 <c>shop_discounted_price</c>）。</summary>
    [JsonPropertyName("shop_discounted_price")]
    public long? ShopDiscountedPrice { get; set; }

    /// <summary>获取或设置商品优惠券信息列表（官方 <c>order_product_coupon_info_list</c>）。</summary>
    [JsonPropertyName("order_product_coupon_info_list")]
    public List<ChannelsGenOrderCouponInfo>? OrderProductCouponInfoList { get; set; }
}
