// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 选品广场商品条目（官方 <c>product_list</c> 数组元素）。
/// </summary>
/// <remarks>
/// <para>与机构「获取合作商品列表」条目 <see cref="ChannelsLeagueHeadSupplierCooperativeItem"/> 字段集不同：
/// 本条目多 <c>shop_appid</c>、无 <c>cooperative_item_id</c> ⇒ 分建 DTO。</para>
/// <para><c>head_supplier_item_link</c> 官方 <c>2025-11-03</c> 由 string 改为 number，本仓统一建模为 string（兼容两种形态）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierSelectionProduct
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，number）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品所属店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppid { get; set; }

    /// <summary>获取或设置机构商品链接（官方 <c>head_supplier_item_link</c>，可直接用于「机构添加合作商品」「查询商品推广参数详情」等接口）。</summary>
    [JsonPropertyName("head_supplier_item_link")]
    public string? HeadSupplierItemLink { get; set; }
}
