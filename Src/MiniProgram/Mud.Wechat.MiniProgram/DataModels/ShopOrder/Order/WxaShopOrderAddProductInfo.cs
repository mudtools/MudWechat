// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「生成订单」的商品条目（官方写侧 <c>order_detail.product_infos[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>写侧只有商家侧单轨 id</b>：官方参数表无 <c>product_id</c> / <c>sku_id</c>（微信侧 id 由订单应答与详情给出）
/// ⇒ 与读侧 <see cref="WxaShopOrderProductInfo"/> 不共用类型（守卫 SO5 锁定「请求侧无 product_id」）。</para>
/// <para><b>价格两支都是必填、单位分</b>：<c>sale_price</c> 是售卖价、<c>real_price</c> 是扣优惠后的单件分摊价，
/// 官方明写「没优惠则与 sale_price 一致」⇒ 只填 sale_price 属参数缺失。</para>
/// <para><b><c>out_sku_id</c> 允许空串</b>（官方原文「可填空字符串，如果这个 product_id 下没有 sku」）⇒ 空串合法，不得当成 null 丢弃。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/add_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderAddProductInfo
{
    /// <summary>out_product_id（官方类型 <c>string</c>，必填）：商家自定义商品 ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>out_sku_id（官方类型 <c>string</c>，必填）：商家自定义 sku ID，该商品无 sku 时官方允许空串。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>product_cnt（官方类型 <c>number</c>，必填）：购买数量。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }

    /// <summary>sale_price（官方类型 <c>number</c>，必填）：生成订单时商品售卖价，单位分，可与上传商品接口的价格不一致。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>real_price（官方类型 <c>number</c>，必填）：扣除优惠后单件 sku 的分摊价格，单位分，无优惠时与 sale_price 一致。</summary>
    [JsonPropertyName("real_price")]
    public long? RealPrice { get; set; }

    /// <summary>path（官方类型 <c>string</c>，必填）：绑定的小程序商品路径。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>title（官方类型 <c>string</c>，必填）：生成订单时商品标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>head_img（官方类型 <c>string</c>，必填）：生成订单时商品头图。</summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }
}
