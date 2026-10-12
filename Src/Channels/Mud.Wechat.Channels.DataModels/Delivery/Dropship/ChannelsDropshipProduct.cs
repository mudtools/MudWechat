// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>代发商品信息（官方 <c>dropship_product_list</c> 数组元素，<c>dropship/assign</c> 与 <c>dropship/cancel</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：分配订单代发与取消分配代发单两端点的 <c>dropship_product_list</c>
/// 元素字段集完全一致（<c>spu_id</c>/<c>sku_id</c>/<c>product_cnt</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipProduct
{
    /// <summary>获取或设置商品 spuid（官方 <c>spu_id</c>，必填）。</summary>
    [JsonPropertyName("spu_id")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 skuid（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置数量（官方 <c>product_cnt</c>，必填）。</summary>
    [JsonPropertyName("product_cnt")]
    public int ProductCnt { get; set; }
}
