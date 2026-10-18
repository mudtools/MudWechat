// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 限时抢购 SKU 配置（<c>limited_discount_sku_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>金额与库存约束</b>：<c>sale_price</c> 单位为分且必须小于原价；<c>sale_stock</c>
/// 必须小于等于现有库存（官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductLimitedDiscountSku
{
    /// <summary>SKU id（必填）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>抢购价，单位分，必须小于原价。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>参与抢购的库存，必须小于等于现有库存。</summary>
    [JsonPropertyName("sale_stock")]
    public long? SaleStock { get; set; }
}
