// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 直播间订单商品条目（<c>order_detail.product_infos[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方参数表类型笔误（本域最典型的一处）</b>：<c>path</c> / <c>head_image</c> / <c>title</c> 三支
/// 官方「类型」列写 number，但「说明」列分别是商品路径 / 头图 / 标题、示例值均为字符串 ⇒ 本仓按语义建
/// <c>string?</c> 并留档该笔误（守卫 SHOPSH5 锁定，不得按表回填成数值类型）。</para>
/// <para><b>金额单位分</b>：<c>sale_price</c> 为下单时售卖价（官方明写可与上传商品接口的价格不一致），
/// <c>real_price</c> 为扣除优惠后单件 sku 均摊价（无优惠时与 <c>sale_price</c> 一致）。</para>
/// <para><b><c>out_sku_id</c> 可为空字符串</b>（官方原文：该 product_id 下没有 sku 时填空串）⇒ 判缺须区分
/// <c>null</c> 与 <c>""</c>，不得把空串当字段缺失。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerProductInfo
{
    /// <summary>商家自定义商品 ID（<c>out_product_id</c>，string）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>商家自定义商品 skuID（<c>out_sku_id</c>，string，该商品无 sku 时为空字符串）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>购买数量（<c>product_cnt</c>，number）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }

    /// <summary>下单时商品售卖价（<c>sale_price</c>，number，<b>单位分</b>，可与上传价不一致）。禁落日志。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>扣除优惠后单件 sku 均摊价（<c>real_price</c>，number，<b>单位分</b>；无优惠则与售卖价一致）。禁落日志。</summary>
    [JsonPropertyName("real_price")]
    public long? RealPrice { get; set; }

    /// <summary>绑定的小程序商品路径（<c>path</c>，string；官方表类型列笔误写 number，按语义建字符串）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>生成订单时商品的头图（<c>head_image</c>，string；官方表类型列笔误写 number，按语义建字符串）。</summary>
    [JsonPropertyName("head_image")]
    public string? HeadImage { get; set; }

    /// <summary>生成订单时商品的标题（<c>title</c>，string；官方表类型列笔误写 number，按语义建字符串）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
