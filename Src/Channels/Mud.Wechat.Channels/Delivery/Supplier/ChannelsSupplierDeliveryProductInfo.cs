// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 包裹中的商品 / 货品信息（官方 <c>product_infos</c> 数组元素，<c>dropship/supplier/delivery/send</c> / <c>update</c>）。
/// </summary>
/// <remarks>
/// 官方原文：代发单传商品（<c>product_id</c> 为商品 spuid），供货单传货品（<c>product_id</c> 为货品 spuid），字段名同一、语义随单据类型而变。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDeliveryProductInfo
{
    /// <summary>获取或设置商品 / 货品 spuid（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 / 货品 skuid（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 / 货品数量（官方 <c>product_cnt</c>，必填）。</summary>
    [JsonPropertyName("product_cnt")]
    public int ProductCnt { get; set; }
}
