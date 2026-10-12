// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>
/// 罗盘达人版带货商品（官方 <c>product_info</c> 对象，<c>getfinderproductdata</c>；
/// 与 <c>getfinderproductlist</c> 的 <c>product_list</c> 元素字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>官方类型均为 number（与商家版商品档案 string 口径不同 ⇒ 不共用 DTO）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品头图（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品价格（官方 <c>price</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>获取或设置 1 级类目（官方 <c>first_category_id</c>）。</summary>
    [JsonPropertyName("first_category_id")]
    public long? FirstCategoryId { get; set; }

    /// <summary>获取或设置 2 级类目（官方 <c>second_category_id</c>）。</summary>
    [JsonPropertyName("second_category_id")]
    public long? SecondCategoryId { get; set; }

    /// <summary>获取或设置 3 级类目（官方 <c>third_category_id</c>）。</summary>
    [JsonPropertyName("third_category_id")]
    public long? ThirdCategoryId { get; set; }

    /// <summary>获取或设置商品明细数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassFinderProductData? Data { get; set; }
}
