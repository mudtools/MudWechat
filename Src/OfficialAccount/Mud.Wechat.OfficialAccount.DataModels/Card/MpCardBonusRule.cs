// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 积分规则（官方 <c>member_card.bonus_rule</c>）—— 建卡与查询<b>同表</b>，共用一型；单位均为「分」或「积分」。
/// </summary>
/// <remarks>与 <c>bonus_rules</c>（字符串形态的旧口径）<b>并存且不可互换</b>，故两字段都保留。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardBonusRule
{
    /// <summary>获取或设置消费获取积分的金额单位（官方 <c>cost_money_unit</c>，分）。</summary>
    [JsonPropertyName("cost_money_unit")]
    public int? CostMoneyUnit { get; set; }

    /// <summary>获取或设置对应增加的积分（官方 <c>increase_bonus</c>）。</summary>
    [JsonPropertyName("increase_bonus")]
    public int? IncreaseBonus { get; set; }

    /// <summary>获取或设置单次可获取积分上限（官方 <c>max_increase_bonus</c>）。</summary>
    [JsonPropertyName("max_increase_bonus")]
    public int? MaxIncreaseBonus { get; set; }

    /// <summary>获取或设置初始积分（官方 <c>init_increase_bonus</c>）。</summary>
    [JsonPropertyName("init_increase_bonus")]
    public int? InitIncreaseBonus { get; set; }

    /// <summary>获取或设置抵扣所需积分单位（官方 <c>cost_bonus_unit</c>）。</summary>
    [JsonPropertyName("cost_bonus_unit")]
    public int? CostBonusUnit { get; set; }

    /// <summary>获取或设置积分抵扣金额（官方 <c>reduce_money</c>，分）。</summary>
    [JsonPropertyName("reduce_money")]
    public int? ReduceMoney { get; set; }

    /// <summary>获取或设置积分抵扣门槛（官方 <c>least_money_to_use_bonus</c>，分）。</summary>
    [JsonPropertyName("least_money_to_use_bonus")]
    public int? LeastMoneyToUseBonus { get; set; }

    /// <summary>获取或设置单次可用积分上限（官方 <c>max_reduce_bonus</c>）。</summary>
    [JsonPropertyName("max_reduce_bonus")]
    public int? MaxReduceBonus { get; set; }
}
