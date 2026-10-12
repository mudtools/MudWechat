// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>更新联盟推广计划信息（<c>item/upd</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsUpdateLeagueItemRequest
{
    /// <summary>获取或设置获取商品推广类别（官方 <c>type</c>，必填，见 <see cref="ChannelsLeaguePlanQueryTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填，可从获取联盟商品推广列表接口中获取）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>，必填）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long PromotionPlanId { get; set; }

    /// <summary>获取或设置更新操作类别（官方 <c>operate_type</c>，必填，见 <see cref="ChannelsLeagueOperateTypes"/>）。</summary>
    [JsonPropertyName("operate_type")]
    public int OperateType { get; set; }

    /// <summary>获取或设置推广佣金（官方 <c>ratio</c>，选填，取值 <c>[0, 90]</c>%，操作为编辑时必填）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }

    /// <summary>获取或设置特殊推广信息（官方 <c>exclusive_info</c>，选填）。</summary>
    [JsonPropertyName("exclusive_info")]
    public ChannelsLeagueUpdExclusiveInfo? ExclusiveInfo { get; set; }
}
