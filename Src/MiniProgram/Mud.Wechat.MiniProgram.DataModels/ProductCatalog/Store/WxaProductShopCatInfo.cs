// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 店铺商品分类项（<c>shopcat_list[]</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductShopCatInfo
{
    /// <summary>shopcat_id（官方类型 <c>number</c>）：分类 ID。</summary>
    [JsonPropertyName("shopcat_id")]
    public long? ShopCatId { get; set; }

    /// <summary>shopcat_name（官方类型 <c>string</c>）：分类名称。</summary>
    [JsonPropertyName("shopcat_name")]
    public string? ShopCatName { get; set; }

    /// <summary>f_shopcat_id（官方类型 <c>number</c>）：父分类 ID。</summary>
    [JsonPropertyName("f_shopcat_id")]
    public long? FShopCatId { get; set; }

    /// <summary>cat_level（官方类型 <c>number</c>）：分类层级。</summary>
    [JsonPropertyName("cat_level")]
    public long? CatLevel { get; set; }
}
