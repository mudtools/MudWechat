// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>可推广商品条目（官方 <c>product_list</c> 数组项，<c>promoter/get_promote_product_list</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：本端点参数表标注 <c>product_id</c> 为 string（官方响应示例写为数字），而详情端点 <c>product.product_id</c> 参数表标注 number ⇒ 两支分别按各自参数表建模，跨端点比对时须自行换算。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterPromoteProductListItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品所属店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置商品所属供货机构 appid（官方 <c>head_supplier_appid</c>）。</summary>
    [JsonPropertyName("head_supplier_appid")]
    public string? HeadSupplierAppId { get; set; }
}
