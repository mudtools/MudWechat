// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>订单商品信息（官方 <c>ec_order_list[].goods_list</c>，取号 / 预取号 / 查单共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGoods
{
    /// <summary>获取或设置商品名（官方 <c>good_name</c>）。</summary>
    [JsonPropertyName("good_name")]
    public string? GoodName { get; set; }

    /// <summary>获取或设置商品个数（官方 <c>good_count</c>）。</summary>
    [JsonPropertyName("good_count")]
    public int? GoodCount { get; set; }

    /// <summary>获取或设置商品 product_id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 sku_id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置商家自定义 spu_id（官方 <c>out_product_id</c>）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商家自定义 sku_id（官方 <c>out_sku_id</c>）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置商家自定义商品详情（官方 <c>out_goods_info</c>，若不传平台商品 id 则展示该字段在面单商品区域）。</summary>
    [JsonPropertyName("out_goods_info")]
    public string? OutGoodsInfo { get; set; }

    /// <summary>获取或设置商家自定义额外商品信息（官方 <c>goods_ext</c>，向快递公司透传，限制长度 512）。</summary>
    [JsonPropertyName("goods_ext")]
    public string? GoodsExt { get; set; }

    /// <summary>获取或设置 SN 码信息（官方 <c>sn_info</c>）。</summary>
    [JsonPropertyName("sn_info")]
    public ChannelsLogisticsEwaybillSnInfo? SnInfo { get; set; }
}
