// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>商品详情（<c>spu/get</c> 的 <c>data.spu</c>；亦为 <c>spu/get_list</c> / <c>spu/search</c> 的 <c>spus[]</c> 元素，列表侧额外含 <c>path</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSpuDetail
{
    /// <summary>小商店内部商品 ID。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义商品 ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>副标题。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>主图列表，最多 9 张。</summary>
    [JsonPropertyName("head_img")]
    public List<string>? HeadImg { get; set; }

    /// <summary>商品详情图文信息。</summary>
    [JsonPropertyName("desc_info")]
    public WxaProductSpuDescInfo? DescInfo { get; set; }

    /// <summary>品牌 ID。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>商品线上状态，取值见 <see cref="WxaProductSpuStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>商品草稿状态，取值见 <see cref="WxaProductSpuEditStatuses"/>。</summary>
    [JsonPropertyName("edit_status")]
    public int? EditStatus { get; set; }

    /// <summary>商品 SKU 最小价格，单位分。</summary>
    [JsonPropertyName("min_price")]
    public long? MinPrice { get; set; }

    /// <summary>小程序页面路径（仅列表接口返回）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>类目列表。</summary>
    [JsonPropertyName("cats")]
    public List<WxaProductSpuCat>? Cats { get; set; }

    /// <summary>商品属性列表。</summary>
    [JsonPropertyName("attrs")]
    public List<WxaProductSpuAttr>? Attrs { get; set; }

    /// <summary>商品型号。</summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>店内分类列表。</summary>
    [JsonPropertyName("shopcat")]
    public List<WxaProductSpuShopCat>? Shopcat { get; set; }

    /// <summary>SKU 引用列表。</summary>
    [JsonPropertyName("skus")]
    public List<WxaProductSpuSkuRef>? Skus { get; set; }
}
