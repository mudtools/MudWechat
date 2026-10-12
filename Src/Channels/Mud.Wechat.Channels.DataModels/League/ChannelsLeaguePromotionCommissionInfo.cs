// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 推广佣金信息（官方 <c>commission_info</c> 对象，<c>promotion/talent/list</c>）。
/// </summary>
/// <remarks>
/// <b>分建裁决</b>：与 <see cref="ChannelsLeagueTalentCommissionInfo"/> 字段集不同
/// （计划 id + 计划类型 + 佣金率，无 <c>commission_type</c>），按端点分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeaguePromotionCommissionInfo
{
    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }

    /// <summary>获取或设置推广计划类型（官方 <c>promotion_plan_type</c>，见 <see cref="ChannelsLeaguePromotionPlanTypes"/>）。</summary>
    [JsonPropertyName("promotion_plan_type")]
    public int? PromotionPlanType { get; set; }

    /// <summary>获取或设置推广佣金率（官方 <c>ratio</c>，取值 <c>[0, 1000000]</c>%，1000000 = 100%）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }
}
