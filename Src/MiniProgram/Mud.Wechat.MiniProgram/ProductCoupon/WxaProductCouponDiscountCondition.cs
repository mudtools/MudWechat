// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券使用门槛（<c>discount_info.discount_condition</c>）。
/// </summary>
/// <remarks>
/// <para><b>按券型取字段</b>：商品条件折扣券填 <c>product_ids</c> + <c>product_cnt</c>；满减券填
/// <c>product_ids</c> + <c>product_price</c>；店铺券不填 <c>product_ids</c> ⇒ 三支并存、由券型决定必填面。</para>
/// <para><c>product_price</c> 单位为<b>分</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponDiscountCondition
{
    /// <summary>满 x 件商品可用（商品折扣券 / 店铺条件折扣券）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }

    /// <summary>指定商品 id 列表（商品券填）。</summary>
    [JsonPropertyName("product_ids")]
    public List<long>? ProductIds { get; set; }

    /// <summary>价格满 x 可用，单位分（满减券填）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }
}
