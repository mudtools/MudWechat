// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>橱窗上的机构商品详情（官方 <c>product_detail</c>，<c>headsupplier/window/getdetail</c>）。
/// 官方参数表把 <c>product_id</c> 定为 string（其余橱窗族为 number）⇒ 照抄本表类型。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierWindowProductDetail
{
    /// <summary>获取或设置所属小店 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，本接口参数表定为 string）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品内容（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsLeagueHeadSupplierWindowProductInfo? ProductInfo { get; set; }
}
