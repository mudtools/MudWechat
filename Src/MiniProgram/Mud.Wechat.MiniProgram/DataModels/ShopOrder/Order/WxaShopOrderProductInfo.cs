// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单商品条目（官方 <c>OrderProductInfo</c> 表，读侧）。
/// </summary>
/// <remarks>
/// <para><b>双单轨 id 并存</b>：官方同时给 <c>product_id</c> / <c>sku_id</c>（微信侧，类型 number）与
/// <c>out_product_id</c> / <c>out_sku_id</c>（商家侧，类型 string）⇒ 四支语义不同、不得互相顶替，
/// 且商家侧 id 是字符串（不得数值化）。</para>
/// <para><b>实付价两支不同名</b>：Order 表列的是 <c>sku_real_price</c>（sku 总实付价），而订单列表两支的示例回的是
/// <c>real_price</c>（单件分摊价）⇒ 两键都在册、均为可空，宿主判「实付」须先判键是否存在。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderProductInfo
{
    /// <summary>product_id（官方类型 <c>number</c>）：微信侧商品 spu id。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>out_product_id（官方类型 <c>string</c>）：商家自定义商品 ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>sku_id（官方类型 <c>number</c>）：微信侧商品 sku id。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>out_sku_id（官方类型 <c>string</c>）：商家自定义 sku ID（该 spu 无 sku 时官方允许空串）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>product_cnt（官方类型 <c>number</c>）：购买数量。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }

    /// <summary>sale_price（官方类型 <c>number</c>）：生成订单时商品售卖价，<b>单位分</b>，可与上传商品接口的价格不一致。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>sku_real_price（官方类型 <c>number</c>）：sku 总实付价，单位分。</summary>
    [JsonPropertyName("sku_real_price")]
    public long? SkuRealPrice { get; set; }

    /// <summary>real_price（官方示例键）：扣除优惠后单件 sku 的分摊价，单位分；仅订单列表两支的示例出现。</summary>
    [JsonPropertyName("real_price")]
    public long? RealPrice { get; set; }

    /// <summary>title（官方类型 <c>string</c>）：生成订单时商品标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>head_img（官方类型 <c>string</c>）：生成订单时商品头图。</summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }

    /// <summary>path（官方类型 <c>string</c>）：绑定的小程序商品路径。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }
}
