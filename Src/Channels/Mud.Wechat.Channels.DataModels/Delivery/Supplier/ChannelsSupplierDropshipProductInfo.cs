// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 代发单商品明细（官方 <c>dropship_order_detail.product_infos</c> 数组元素，供货商视角，<c>dropship/supplier/get</c>）。
/// </summary>
/// <remarks>
/// 与商家侧 <see cref="ChannelsDropshipProductInfo"/> 的差异均为官方两页参数表原文差异：
/// ①本页 <c>product_id</c>/<c>sku_id</c> 为 <b>string</b>（商家侧为 number）；②本页含 <c>sku_attrs</c>（商家侧参数表无该字段）。
/// 发货信息 / 代发信息 / 贺卡 / 定制等子树官方逐字段一致 ⇒ 复用同一类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDropshipProductInfo
{
    /// <summary>获取或设置商品 spuid（官方 <c>product_id</c>，本视角为 string）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 skuid（官方 <c>sku_id</c>，本视角为 string）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置 sku 数量（官方 <c>sku_cnt</c>）。</summary>
    [JsonPropertyName("sku_cnt")]
    public int? SkuCnt { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置 sku 属性/款式信息列表（官方 <c>sku_attrs</c>，供货商侧参数表返回该字段）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<ChannelsSkuAttr>? SkuAttrs { get; set; }

    /// <summary>获取或设置商品外部 spuid（官方 <c>out_product_id</c>）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品外部 skuid（官方 <c>out_sku_id</c>）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置区域库存 id（官方 <c>out_warehouse_id</c>）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string? OutWarehouseId { get; set; }

    /// <summary>获取或设置商品发货信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }

    /// <summary>获取或设置商品发货时效（官方 <c>delivery_deadline</c>，秒级时间戳，超时未发货即发货超时）。</summary>
    [JsonPropertyName("delivery_deadline")]
    public long? DeliveryDeadline { get; set; }

    /// <summary>获取或设置是否赠品（官方 <c>is_free_gift</c>：1 是赠品；赠品商品返回）。</summary>
    [JsonPropertyName("is_free_gift")]
    public int? IsFreeGift { get; set; }

    /// <summary>获取或设置商品常量编号（官方 <c>product_unique_id</c>，订单内商品唯一标识，下单后不变，可用于判断商品是否更换款式）。</summary>
    [JsonPropertyName("product_unique_id")]
    public string? ProductUniqueId { get; set; }

    /// <summary>获取或设置代发相关信息（官方 <c>dropship_info</c>）。</summary>
    [JsonPropertyName("dropship_info")]
    public ChannelsSupplierDropshipInfo? DropshipInfo { get; set; }
}
