// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟推广计划信息（官方 <c>item</c> 对象，<c>item/get</c>）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：<c>exclusive_info</c> 参数表字段为 <c>promotion_plan_id</c>/<c>begin_time</c>/
/// <c>end_time</c>/<c>is_forerver</c>/<c>talent_num</c>，代码示例写作 <c>info_id</c>/<c>finder_ids</c>/
/// <c>finder_num</c>/<c>finder_info_list</c>；<c>ext_info</c> 参数表为 <c>is_banned</c>、示例为
/// <c>is_sale_forbidden</c>——SDK 均按参数表建模（官方变更日志 2026-06-17：新增 promotion_plan_id、删除 info_id）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品推广类别（官方 <c>type</c>，见 <see cref="ChannelsLeaguePromotionTypes"/>：1 普通推广商品 / 2 定向推广商品 / 3 专属推广商品——与请求侧 <see cref="ChannelsLeaguePlanQueryTypes"/> 是两套枚举）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置商品推广状态（官方 <c>status</c>，见 <see cref="ChannelsLeagueItemStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置推广佣金（官方 <c>ratio</c>，取值 <c>[0, 90]</c>%）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }

    /// <summary>获取或设置推广信息（官方 <c>exclusive_info</c>，特殊推广计划形态）。</summary>
    [JsonPropertyName("exclusive_info")]
    public ChannelsLeagueExclusiveInfo? ExclusiveInfo { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsLeagueItemExtInfo? ExtInfo { get; set; }
}
