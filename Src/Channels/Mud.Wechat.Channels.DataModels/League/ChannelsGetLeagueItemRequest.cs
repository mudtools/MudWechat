// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取联盟推广计划详情（<c>item/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetLeagueItemRequest
{
    /// <summary>获取或设置获取商品推广类别（官方 <c>type</c>，必填，见 <see cref="ChannelsLeaguePlanQueryTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>，选填）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }
}
