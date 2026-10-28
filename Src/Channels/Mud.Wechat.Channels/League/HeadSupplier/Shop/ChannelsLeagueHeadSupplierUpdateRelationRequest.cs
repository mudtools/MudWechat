// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>更新机构合作关系请求（官方 <c>headsupplier/headsupplierrelation/update</c> 请求体）。
/// 返回体仅 <c>errcode</c> / <c>errmsg</c> ⇒ 复用 <c>ChannelsResponse</c>，不建响应 DTO。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierUpdateRelationRequest
{
    /// <summary>获取或设置操作对象类型（官方 <c>object_type</c>，必填，见 <see cref="ChannelsLeagueHeadSupplierRelationObjectTypes"/>）。</summary>
    [JsonPropertyName("object_type")]
    public int ObjectType { get; set; }

    /// <summary>获取或设置目标商家 appid（官方 <c>to_shop_appid</c>，<c>object_type=2</c> 时必填）。</summary>
    [JsonPropertyName("to_shop_appid")]
    public string? ToShopAppid { get; set; }

    /// <summary>获取或设置目标机构 appid（官方 <c>to_head_supplier_appid</c>，<c>object_type=1</c> 时必填）。</summary>
    [JsonPropertyName("to_head_supplier_appid")]
    public string? ToHeadSupplierAppid { get; set; }

    /// <summary>获取或设置机构侧操作（官方 <c>hs_update_type</c>，<c>object_type=1</c> 时必填，见 <see cref="ChannelsLeagueHeadSupplierHsUpdateTypes"/>）。</summary>
    [JsonPropertyName("hs_update_type")]
    public int? HsUpdateType { get; set; }

    /// <summary>获取或设置店铺侧操作（官方 <c>shop_update_type</c>，<c>object_type=2</c> 时必填，见 <see cref="ChannelsLeagueHeadSupplierShopUpdateTypes"/>）。</summary>
    [JsonPropertyName("shop_update_type")]
    public int? ShopUpdateType { get; set; }
}
