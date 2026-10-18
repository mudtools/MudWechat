// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 优惠单品明细（官方 <c>GoodsDetail</c> 表）。
/// </summary>
/// <remarks>
/// <para><b>官方字段名拼写为 <c>goods_reamark</c>（少一个 k）</b>：JSON 名照官方原文钉住，属性名按 C# 口径写 <c>GoodsRemark</c>；
/// 若照属性名反推 JSON 名（goods_remark）会静默丢字段 ⇒ 由守卫 SO5 锁定该字面量。</para>
/// <para>金额单位分。官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopGoodsDetail
{
    /// <summary>goods_id（官方类型 <c>string</c>）：商品编码。</summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

    /// <summary>quantity（官方类型 <c>number</c>）：商品数量。</summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>unit_price（官方类型 <c>number</c>）：商品单价，单位分。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>discount_amount（官方类型 <c>number</c>）：商品优惠金额，单位分。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>goods_reamark（官方原文拼写，类型 <c>string</c>）：商品备注。</summary>
    [JsonPropertyName("goods_reamark")]
    public string? GoodsRemark { get; set; }
}
