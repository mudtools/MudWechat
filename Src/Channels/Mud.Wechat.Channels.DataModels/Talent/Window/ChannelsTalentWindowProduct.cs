// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>橱窗商品详情（官方 <c>product</c>，带货助手视角的橱窗商品全量字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Talent")]
public class ChannelsTalentWindowProduct
{
    /// <summary>获取或设置橱窗商品 id（官方 <c>product_id</c>，参数表标 string）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置商品在货源店铺的商品 id（官方 <c>out_product_id</c>，带货商品返回）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品头图 url（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    /// <summary>获取或设置商品所属叶子类目（品类）id（官方 <c>leaf_category_id</c>）。</summary>
    [JsonPropertyName("leaf_category_id")]
    public long? LeafCategoryId { get; set; }

    /// <summary>获取或设置商品状态（官方 <c>status</c>，见 <see cref="ChannelsTalentWindowProductStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置价格区间最小值（官方 <c>selling_price</c>，单位分，即销售价）。</summary>
    [JsonPropertyName("selling_price")]
    public long? SellingPrice { get; set; }

    /// <summary>获取或设置剩余库存（官方 <c>stock</c>）。</summary>
    [JsonPropertyName("stock")]
    public long? Stock { get; set; }

    /// <summary>获取或设置销量（官方 <c>sales</c>）。</summary>
    [JsonPropertyName("sales")]
    public long? Sales { get; set; }

    /// <summary>获取或设置是否在橱窗设置对外隐藏（官方 <c>is_hide</c>）。</summary>
    [JsonPropertyName("is_hide")]
    public bool? IsHide { get; set; }

    /// <summary>获取或设置跟佣信息串（官方 <c>product_promotion_link</c>，用于小程序跳转小店场景添加商品时传递）。</summary>
    [JsonPropertyName("product_promotion_link")]
    public string? ProductPromotionLink { get; set; }
}
