// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构合作商品条目（官方 <c>cooperativeitem/list/get</c> 的 <c>list</c> 数组元素）。</summary>
/// <remarks>官方变更日志 2025-11-03：<c>product_id</c> 类型由 <c>string</c> 变更为 <c>number</c> ⇒ 建模为 <see cref="long"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierCooperativeItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置机构合作计划 id（官方 <c>cooperative_item_id</c>）。</summary>
    [JsonPropertyName("cooperative_item_id")]
    public long? CooperativeItemId { get; set; }

    /// <summary>获取或设置机构推广商品链接（官方 <c>head_supplier_item_link</c>，用于「获取商品推广参数详情」）。</summary>
    [JsonPropertyName("head_supplier_item_link")]
    public string? HeadSupplierItemLink { get; set; }
}
