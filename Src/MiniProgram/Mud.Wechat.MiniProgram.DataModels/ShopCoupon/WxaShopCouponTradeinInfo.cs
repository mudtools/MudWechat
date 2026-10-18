// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 换购券的换购信息（官方 <c>discount_condition.tradein_info</c>）。
/// </summary>
/// <remarks>
/// <para>仅 <c>type=5</c>（商品换购券）必填；<c>price</c> 单位为分。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponTradeinInfo
{
    /// <summary>换购商品商家侧 id（<c>out_product_id</c>）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>换购金额（<c>price</c>，单位<b>分</b>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }
}
