// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 机构侧商品基础详情查询请求（官方 <c>headsupplier/productdetail/get</c> 请求体）。
/// </summary>
/// <remarks>
/// 与小店商家侧 <c>product/get</c>（<see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsGetProductRequest"/>，按 <c>product_id</c> 或
/// <c>out_product_id</c> 查本小店商品）字段集与返回结构均不同 ⇒ 分建 DTO；本接口是「机构查别人的商品」，须同时给所属小店 appid。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetProductDetailRequest
{
    /// <summary>获取或设置机构商品所属小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppid { get; set; } = string.Empty;

    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填，number）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }
}
