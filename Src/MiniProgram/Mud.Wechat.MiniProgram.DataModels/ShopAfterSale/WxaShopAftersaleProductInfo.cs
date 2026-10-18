// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 售后商品项（V1 <c>aftersale/add</c> 请求与 <c>aftersale/get</c> 应答共用的 <c>product_infos[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b>V1 只认商家侧 id</b>：本结构<b>没有</b> <c>product_id</c> / <c>sku_id</c> 两支微信侧 id，</para>
/// <para>与 V2 <see cref="WxaEcAftersaleProductInfo"/> 不是同一结构，不得互换。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopAftersaleProductInfo
{
    /// <summary>商家自定义商品 ID（必填）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>商家自定义 SKU ID（无 SKU 时官方要求不填）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>参与售后的商品数量（<c>product_infos</c> 存在时必填）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}
