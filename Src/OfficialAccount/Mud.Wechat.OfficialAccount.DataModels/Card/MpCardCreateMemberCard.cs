// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 会员卡（官方 <c>card.member_card</c>）—— 建卡方向字段面最宽的一支。
/// </summary>
/// <remarks>
/// <b>三组「同一个东西的两种写法」在本支并存</b>，均照官方保留、不得择一删除：
/// 积分规则 <c>bonus_rules</c>（字符串）与 <c>bonus_rule</c>（对象）；
/// 一键激活 <c>wx_activate</c>（直接型）与 <c>wx_activate_after_submit</c> + <c>wx_activate_after_submit_url</c>（跳转型）；
/// 入口外链 <c>activate_url</c> 与 <c>activate_app_brand_*</c>（小程序）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateMemberCard
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，会员卡形态见 <see cref="MpCardMemberCardBaseInfo"/>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardMemberCardBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置背景图 URL（官方 <c>background_pic_url</c>，选填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置特权说明（官方 <c>prerogative</c>，必填）。</summary>
    [JsonPropertyName("prerogative")]
    public string? Prerogative { get; set; }

    /// <summary>获取或设置是否领取后自动激活（官方 <c>auto_activate</c>，选填）。</summary>
    [JsonPropertyName("auto_activate")]
    public bool? AutoActivate { get; set; }

    /// <summary>获取或设置是否支持一键开卡（官方 <c>wx_activate</c>，选填）。</summary>
    [JsonPropertyName("wx_activate")]
    public bool? WxActivate { get; set; }

    /// <summary>获取或设置是否支持跳转型一键激活（官方 <c>wx_activate_after_submit</c>，选填）。</summary>
    [JsonPropertyName("wx_activate_after_submit")]
    public bool? WxActivateAfterSubmit { get; set; }

    /// <summary>获取或设置激活外链（官方 <c>activate_url</c>，选填）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>获取或设置激活小程序原始 id（官方 <c>activate_app_brand_user_name</c>，选填）。</summary>
    [JsonPropertyName("activate_app_brand_user_name")]
    public string? ActivateAppBrandUserName { get; set; }

    /// <summary>获取或设置激活小程序页面路径（官方 <c>activate_app_brand_pass</c>，选填）。</summary>
    [JsonPropertyName("activate_app_brand_pass")]
    public string? ActivateAppBrandPass { get; set; }

    /// <summary>获取或设置跳转型一键激活的跳转外链（官方 <c>wx_activate_after_submit_url</c>，选填）。</summary>
    [JsonPropertyName("wx_activate_after_submit_url")]
    public string? WxActivateAfterSubmitUrl { get; set; }

    /// <summary>获取或设置是否支持积分（官方 <c>supply_bonus</c>，必填）。</summary>
    [JsonPropertyName("supply_bonus")]
    public bool SupplyBonus { get; set; }

    /// <summary>获取或设置查看积分详情外链（官方 <c>bonus_url</c>，选填）。</summary>
    [JsonPropertyName("bonus_url")]
    public string? BonusUrl { get; set; }

    /// <summary>获取或设置积分清零规则（官方 <c>bonus_cleared</c>，选填）。</summary>
    [JsonPropertyName("bonus_cleared")]
    public string? BonusCleared { get; set; }

    /// <summary>获取或设置积分规则字符串（官方 <c>bonus_rules</c>，选填；与 <see cref="BonusRule"/> 并存）。</summary>
    [JsonPropertyName("bonus_rules")]
    public string? BonusRules { get; set; }

    /// <summary>获取或设置积分规则对象（官方 <c>bonus_rule</c>，选填；与 <see cref="BonusRules"/> 并存）。</summary>
    [JsonPropertyName("bonus_rule")]
    public MpCardBonusRule? BonusRule { get; set; }

    /// <summary>获取或设置是否支持储值（官方 <c>supply_balance</c>，必填）。</summary>
    [JsonPropertyName("supply_balance")]
    public bool SupplyBalance { get; set; }

    /// <summary>获取或设置查看余额详情外链（官方 <c>balance_url</c>，选填）。</summary>
    [JsonPropertyName("balance_url")]
    public string? BalanceUrl { get; set; }

    /// <summary>获取或设置储值规则（官方 <c>balance_rules</c>，选填）。</summary>
    [JsonPropertyName("balance_rules")]
    public string? BalanceRules { get; set; }

    /// <summary>获取或设置自定义会员信息类目 1（官方 <c>custom_field1</c>）。</summary>
    [JsonPropertyName("custom_field1")]
    public MpCardCustomField? CustomField1 { get; set; }

    /// <summary>获取或设置自定义会员信息类目 2（官方 <c>custom_field2</c>）。</summary>
    [JsonPropertyName("custom_field2")]
    public MpCardCustomField? CustomField2 { get; set; }

    /// <summary>获取或设置自定义会员信息类目 3（官方 <c>custom_field3</c>）。</summary>
    [JsonPropertyName("custom_field3")]
    public MpCardCustomField? CustomField3 { get; set; }

    /// <summary>获取或设置自定义会员服务入口 1（官方 <c>custom_cell1</c>）。</summary>
    [JsonPropertyName("custom_cell1")]
    public MpCardCustomCell? CustomCell1 { get; set; }

    /// <summary>获取或设置自定义会员服务入口 2（官方 <c>custom_cell2</c>）。</summary>
    [JsonPropertyName("custom_cell2")]
    public MpCardCustomCell? CustomCell2 { get; set; }

    /// <summary>获取或设置自定义会员服务入口 3（官方 <c>custom_cell3</c>）。</summary>
    [JsonPropertyName("custom_cell3")]
    public MpCardCustomCell? CustomCell3 { get; set; }

    /// <summary>获取或设置会员专享折扣（官方 <c>discount</c>，选填，如 80 表示八折）。</summary>
    [JsonPropertyName("discount")]
    public int? Discount { get; set; }
}
