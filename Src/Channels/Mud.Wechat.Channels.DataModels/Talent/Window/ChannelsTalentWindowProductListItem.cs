// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>橱窗商品列表条目（官方 <c>products</c> 数组项：只回商品 id 与来源标识，详情须再调 <c>window/product/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Talent")]
public class ChannelsTalentWindowProductListItem
{
    /// <summary>获取或设置橱窗商品 id（官方 <c>product_id</c>，参数表标 number、示例为字符串形态 ⇒ 按参数表取整型并在守卫处照录矛盾）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品来源店铺 appid（官方 <c>appid</c>，自营商品时返回）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置商品来源类型（官方 <c>product_source</c>，见 <see cref="ChannelsTalentWindowProductSources"/>）。</summary>
    [JsonPropertyName("product_source")]
    public int? ProductSource { get; set; }

    /// <summary>获取或设置商品在货源小店的 id（官方 <c>out_product_id</c>，带货商品时返回）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }
}
