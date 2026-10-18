// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// V2 售后商品（官方 <c>AfterSaleProductInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>与 V1 的差异</b>：本结构同时接受商家侧 <c>out_product_id</c> / <c>out_sku_id</c> 与微信侧</para>
/// <para><c>product_id</c> / <c>sku_id</c>（各二选一）；<c>product_cnt</c> <b>必填</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleProductInfo
{
    /// <summary>商家自定义商品 ID（与 <c>product_id</c> 二选一）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>微信侧商品 ID（与 <c>out_product_id</c> 二选一）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义 SKU ID（无 SKU 时不填）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>微信侧 SKU ID。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>参与售后的商品数量（必填）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}
