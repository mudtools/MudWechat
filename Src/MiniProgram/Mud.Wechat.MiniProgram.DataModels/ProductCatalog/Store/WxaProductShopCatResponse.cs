// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 「获取店铺的商品分类」应答（<c>POST /product/store/get_shopcat</c>，本端点请求参数表为空、不设请求 DTO）。
/// </summary>
/// <remarks>
/// <para><b>店铺自建商品分类</b>，与官方平台类目（<c>/product/category/get</c>）是两个对象、不得混用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductShopCatResponse : WxaResponse
{
    /// <summary>shopcat_list（官方类型 <c>object array</c>）：店铺商品分类列表。</summary>
    [JsonPropertyName("shopcat_list")]
    public List<WxaProductShopCatInfo>? ShopCatList { get; set; }
}
