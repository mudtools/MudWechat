// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>更新商品请求（<c>POST /product/spu/update</c>）；官方参数表不含 <c>skus</c>，SKU 变更走 <c>sku/update</c>。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSpuUpdateRequest
{
    /// <summary>小商店内部商品 ID，与 <c>out_product_id</c> 二选一。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义商品 ID，与 <c>product_id</c> 二选一。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>标题（必填），3~60 字符；不得仅为数字、字母或特殊字符组合。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>副标题，最多 18 字符。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>主图（必填），多张，最多 9 张。</summary>
    [JsonPropertyName("head_img")]
    public List<string>? HeadImg { get; set; }

    /// <summary>商品详情图文信息。</summary>
    [JsonPropertyName("desc_info")]
    public WxaProductSpuDescInfo? DescInfo { get; set; }

    /// <summary>品牌 ID（必填）；无品牌填 <c>2100000000</c>。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>类目列表（必填，须三级）。</summary>
    [JsonPropertyName("cats")]
    public List<WxaProductSpuCat>? Cats { get; set; }

    /// <summary>商品属性列表。</summary>
    [JsonPropertyName("attrs")]
    public List<WxaProductSpuAttr>? Attrs { get; set; }

    /// <summary>商品型号。</summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>运费信息（必填）。</summary>
    [JsonPropertyName("express_info")]
    public WxaProductSpuExpressInfo? ExpressInfo { get; set; }

    /// <summary>商品类型：1 实物商品（默认）；2 券码商品；须与添加时一致，否则报参数错误。</summary>
    [JsonPropertyName("product_type")]
    public int? ProductType { get; set; }

    /// <summary>券码核销信息（核销商品必填）。</summary>
    [JsonPropertyName("couponcode_info")]
    public WxaProductSpuCouponCodeInfo? CouponCodeInfo { get; set; }
}
