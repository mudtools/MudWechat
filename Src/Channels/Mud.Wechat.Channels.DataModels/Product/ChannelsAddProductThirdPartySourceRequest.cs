// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>新增第三方货源商品（<c>addproductthirdpartysource</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddProductThirdPartySourceRequest
{
    /// <summary>获取或设置场景值（官方 <c>scene_value</c>，必填；见 <see cref="ChannelsThirdPartySourceScenes"/>）。</summary>
    [JsonPropertyName("scene_value")]
    public int SceneValue { get; set; }

    /// <summary>获取或设置商品发布方式（官方 <c>publish_method</c>，<c>scene_value==3</c> 其他场景时必填）。</summary>
    [JsonPropertyName("publish_method")]
    public int? PublishMethod { get; set; }

    /// <summary>获取或设置货主（官方 <c>supplier</c>，必填）。</summary>
    [JsonPropertyName("supplier")]
    public ChannelsThirdPartySourceSupplier Supplier { get; set; } = new();

    /// <summary>获取或设置货主店铺经营表现（官方 <c>supplier_shop_performance</c>）。</summary>
    [JsonPropertyName("supplier_shop_performance")]
    public ChannelsSupplierShopPerformance? SupplierShopPerformance { get; set; }

    /// <summary>获取或设置商品在货源平台的信息（官方 <c>product_source_info</c>，必填）。</summary>
    [JsonPropertyName("product_source_info")]
    public ChannelsThirdPartyProductSourceInfo ProductSourceInfo { get; set; } = new();
}
