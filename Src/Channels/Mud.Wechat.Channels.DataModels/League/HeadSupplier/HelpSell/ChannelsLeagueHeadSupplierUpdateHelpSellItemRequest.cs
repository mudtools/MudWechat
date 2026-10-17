// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构更新或结束已添加的推广计划请求（官方 <c>headsupplier/secondheadsupplieritem/update</c> 请求体）。
/// 官方标注 <c>service_ratio</c> 与 <c>to_head_supplier_appid</c> 为「更新 / 定向计划的情况下必填」⇒ 建模为可空。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest
{
    /// <summary>获取或设置计划类型（官方 <c>plan_type</c>，必填，见 <see cref="ChannelsLeagueHeadSupplierHelpSellPlanTypes"/>）。</summary>
    [JsonPropertyName("plan_type")]
    public int PlanType { get; set; }

    /// <summary>获取或设置操作类型（官方 <c>update_type</c>，必填，见 <see cref="ChannelsLeagueHeadSupplierHelpSellUpdateTypes"/>）。</summary>
    [JsonPropertyName("update_type")]
    public int UpdateType { get; set; }

    /// <summary>获取或设置合作计划 id（官方 <c>cooperative_item_id</c>，与 <c>head_supplier_item_link</c> 必填其一）。</summary>
    [JsonPropertyName("cooperative_item_id")]
    public long? CooperativeItemId { get; set; }

    /// <summary>获取或设置机构商品链接（官方 <c>head_supplier_item_link</c>，与 <c>cooperative_item_id</c> 必填其一）。</summary>
    [JsonPropertyName("head_supplier_item_link")]
    public string? HeadSupplierItemLink { get; set; }

    /// <summary>获取或设置帮卖服务费率（官方 <c>service_ratio</c>，更新佣金率时必填，百万分之 N）。</summary>
    [JsonPropertyName("service_ratio")]
    public long? ServiceRatio { get; set; }

    /// <summary>获取或设置目标帮卖机构 appid（官方 <c>to_head_supplier_appid</c>，定向计划必填）。</summary>
    [JsonPropertyName("to_head_supplier_appid")]
    public string? ToHeadSupplierAppid { get; set; }
}
