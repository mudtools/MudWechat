// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>商品档案信息（官方 <c>product_info</c> 对象，<c>getshopproductdata</c>，官方类型 string）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品图（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品价格（官方 <c>price</c>，单位：分）。</summary>
    [JsonPropertyName("price")]
    public string? Price { get; set; }

    /// <summary>获取或设置商品一级类目（官方 <c>first_category_id</c>）。</summary>
    [JsonPropertyName("first_category_id")]
    public string? FirstCategoryId { get; set; }

    /// <summary>获取或设置商品二级类目（官方 <c>second_category_id</c>）。</summary>
    [JsonPropertyName("second_category_id")]
    public string? SecondCategoryId { get; set; }

    /// <summary>获取或设置商品三级类目（官方 <c>third_category_id</c>）。</summary>
    [JsonPropertyName("third_category_id")]
    public string? ThirdCategoryId { get; set; }

    /// <summary>获取或设置详细数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassShopProductDetailData? Data { get; set; }
}
