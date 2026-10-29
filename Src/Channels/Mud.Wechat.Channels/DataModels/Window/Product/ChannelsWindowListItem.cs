// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>橱窗商品列表项（官方 <c>products[]</c> 元素，<c>getwindowproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowListItem
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，string(uint64)；对于带货商品则为带货商品 ID）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>；对于自营商品会返回）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置货源店铺的商品 ID（官方 <c>out_product_id</c>；对于带货商品会返回）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品来源（官方 <c>product_source</c>，见 <see cref="ChannelsWindowProductSources"/>）。</summary>
    [JsonPropertyName("product_source")]
    public int? ProductSource { get; set; }
}
