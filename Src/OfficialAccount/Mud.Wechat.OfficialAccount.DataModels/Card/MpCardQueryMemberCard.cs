// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 会员卡（查询方向，官方 <c>card.member_card</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与建卡方向的字段差集</b>（守卫 CD5 锁定，勿「顺手补齐」）：本向<b>多出</b> <c>bind_old_card_url</c>
/// （绑定旧卡入口），<b>不含</b> <c>background_pic_url</c> / <c>wx_activate*</c> /
/// <c>activate_app_brand_*</c> / <c>bonus_url</c> / <c>balance_url</c> / <c>custom_cell1..3</c>。
/// </para>
/// <para><b>积分规则两形态并存</b>：<c>bonus_rules</c>（字符串）与 <c>bonus_rule</c>（对象）照官方同时保留。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryMemberCard
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，会员卡查询形态）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryMemberCardBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置特权说明（官方 <c>prerogative</c>）。</summary>
    [JsonPropertyName("prerogative")]
    public string? Prerogative { get; set; }

    /// <summary>获取或设置绑定旧卡入口（官方 <c>bind_old_card_url</c>；<b>查询方向独有</b>）。</summary>
    [JsonPropertyName("bind_old_card_url")]
    public string? BindOldCardUrl { get; set; }

    /// <summary>获取或设置激活外链（官方 <c>activate_url</c>）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>获取或设置是否支持积分（官方 <c>supply_bonus</c>）。</summary>
    [JsonPropertyName("supply_bonus")]
    public bool? SupplyBonus { get; set; }

    /// <summary>获取或设置积分清零规则（官方 <c>bonus_cleared</c>）。</summary>
    [JsonPropertyName("bonus_cleared")]
    public string? BonusCleared { get; set; }

    /// <summary>获取或设置积分规则字符串（官方 <c>bonus_rules</c>）。</summary>
    [JsonPropertyName("bonus_rules")]
    public string? BonusRules { get; set; }

    /// <summary>获取或设置积分规则对象（官方 <c>bonus_rule</c>，与 <see cref="BonusRules"/> 并存）。</summary>
    [JsonPropertyName("bonus_rule")]
    public MpCardBonusRule? BonusRule { get; set; }

    /// <summary>获取或设置是否支持储值（官方 <c>supply_balance</c>）。</summary>
    [JsonPropertyName("supply_balance")]
    public bool? SupplyBalance { get; set; }

    /// <summary>获取或设置储值规则（官方 <c>balance_rules</c>）。</summary>
    [JsonPropertyName("balance_rules")]
    public string? BalanceRules { get; set; }

    /// <summary>获取或设置会员专享折扣（官方 <c>discount</c>）。</summary>
    [JsonPropertyName("discount")]
    public int? Discount { get; set; }
}
