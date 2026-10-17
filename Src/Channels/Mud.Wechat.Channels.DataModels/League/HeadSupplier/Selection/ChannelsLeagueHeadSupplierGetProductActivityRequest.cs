// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>商品活动补贴信息查询请求（官方 <c>headsupplier/product/getactivity</c> 请求体）。</summary>
/// <remarks>官方必填口径：<c>product_id</c> 与 <c>shop_appid</c> 均必填，缺一报 <c>10024000</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetProductActivityRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置所属小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppid { get; set; } = string.Empty;
}
