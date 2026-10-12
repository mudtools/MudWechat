// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>商品推广带货者条目（官方 <c>list</c> 数组元素，<c>promotion/talent/list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeaguePromotionTalentItem
{
    /// <summary>获取或设置带货者基础信息（官方 <c>base_talent_info</c>，与 <c>promoter/get</c> 同形态 ⇒ 共用 <see cref="ChannelsLeagueTalentBaseInfo"/>）。</summary>
    [JsonPropertyName("base_talent_info")]
    public ChannelsLeagueTalentBaseInfo? BaseTalentInfo { get; set; }

    /// <summary>获取或设置带货者佣金信息（官方 <c>commission_info</c>）。</summary>
    [JsonPropertyName("commission_info")]
    public ChannelsLeaguePromotionCommissionInfo? CommissionInfo { get; set; }

    /// <summary>获取或设置带货者推广状态（官方 <c>talent_relation_status</c>，见 <see cref="ChannelsLeagueTalentRelationStatuses"/>）。</summary>
    [JsonPropertyName("talent_relation_status")]
    public int? TalentRelationStatus { get; set; }
}
