// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>可推广商品详情（官方 <c>product</c> 对象，<c>promoter/get_promote_product_detail</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：响应示例另出现 <c>product_promotion_link</c>（参数表登记在 <c>product.product_info</c> 下）及 <c>product_info.express_info</c> / <c>product_info.extra_service</c> / <c>product_info.statistical_data</c> / <c>commission_info.normal_commission_info</c> 四个参数表未登记的示例对象——本域按参数表建模，未登记的示例字段不入 DTO。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterPromoteProduct
{
    /// <summary>获取或设置所属小店 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsPromoterPromoteProductInfo? ProductInfo { get; set; }

    /// <summary>获取或设置跟佣信息（官方 <c>commission_info</c>）。</summary>
    [JsonPropertyName("commission_info")]
    public ChannelsPromoterProductCommissionInfo? CommissionInfo { get; set; }

    /// <summary>获取或设置供货机构信息（官方 <c>head_supplier_info</c>）。</summary>
    [JsonPropertyName("head_supplier_info")]
    public ChannelsPromoterHeadSupplierInfo? HeadSupplierInfo { get; set; }
}
