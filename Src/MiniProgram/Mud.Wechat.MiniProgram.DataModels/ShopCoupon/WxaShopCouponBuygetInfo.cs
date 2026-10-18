// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 买赠券的买赠信息（官方 <c>discount_condition.buyget_info</c>）。
/// </summary>
/// <remarks>
/// <para>仅 <c>type=6</c>（商品买赠券）必填；四支键共同描述「买 N 件 A 赠 M 件 B」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponBuygetInfo
{
    /// <summary>购买商品商家侧 id。</summary>
    [JsonPropertyName("buy_out_product_id")]
    public string? BuyOutProductId { get; set; }

    /// <summary>购买商品数。</summary>
    [JsonPropertyName("buy_product_cnt")]
    public int? BuyProductCnt { get; set; }

    /// <summary>赠送商品商家侧 id。</summary>
    [JsonPropertyName("get_out_product_id")]
    public string? GetOutProductId { get; set; }

    /// <summary>赠送商品数。</summary>
    [JsonPropertyName("get_product_cnt")]
    public int? GetProductCnt { get; set; }
}
