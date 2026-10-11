// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 创建卡券（POST /card/create）
//  请求体为「card 包装 + card_type 判别 + 11 个券型分支」。分支之间**互斥**（官方按 card_type 取其一），
//  故每个分支都是独立对象、各带自己的 base_info/advanced_info 引用 —— 不得为「省类型」而合并成
//  一个万能对象（那会让 cash 分支能传 discount 字段，官方该页未定义）。

/// <summary>
/// 创建卡券（<c>POST /card/create</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>顶层只有一个 <c>card</c> 对象</b>（官方形态，非扁平）；<c>access_token</c> 由 Query 注入、不入请求体。
/// </para>
/// <para>
/// <b>对齐基准</b>：SKIT <c>CardCreateRequest</c>（2026-10-10）。官方页面（现文档树
/// <c>doc/subscription/guide/product/card/Create_a_Coupon_Voucher_or_Card</c>）正文为 SPA、本次不可达，
/// 字段表与长度上限<b>待逐页核验</b>；守卫 CD1~CD4 只锁路由、分支归属与官方字段名。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateRequest
{
    /// <summary>获取或设置卡券对象（官方 <c>card</c>，必填）。</summary>
    [JsonPropertyName("card")]
    public MpCardCreateBody? Card { get; set; }
}

/// <summary>
/// 创建卡券请求体的 <c>card</c> 对象（<c>card_type</c> + 11 个券型分支，<b>分支互斥</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>分支与 <c>card_type</c> 的对应</b>：groupon 团购券 / cash 代金券 / discount 折扣券 / gift <b>兑换券</b> /
/// general_coupon 优惠券 / general_card <b>礼品卡</b> / member_card <b>会员卡</b> / meeting_ticket 会议门票 /
/// scenic_ticket 景区门票 / movie_ticket 电影票 / boarding_pass 飞机票。
/// </para>
/// <para>
/// <b>易错点（照官方键名，勿凭常识改写）</b>：<c>gift</c> 是「兑换券」而非礼品卡，礼品卡的官方键是
/// <c>general_card</c>；会员卡键为 <c>member_card</c>（<b>不存在</b> <c>membership_card</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateBody
{
    /// <summary>获取或设置卡券类型（官方 <c>card_type</c>，必填，取值见本类型 remarks 的分支对应表）。</summary>
    [JsonPropertyName("card_type")]
    public string CardType { get; set; } = string.Empty;

    /// <summary>获取或设置团购券信息（官方 <c>groupon</c>）。</summary>
    [JsonPropertyName("groupon")]
    public MpCardCreateGroupon? Groupon { get; set; }

    /// <summary>获取或设置代金券信息（官方 <c>cash</c>）。</summary>
    [JsonPropertyName("cash")]
    public MpCardCreateCash? Cash { get; set; }

    /// <summary>获取或设置折扣券信息（官方 <c>discount</c>）。</summary>
    [JsonPropertyName("discount")]
    public MpCardCreateDiscount? Discount { get; set; }

    /// <summary>获取或设置<b>兑换券</b>信息（官方 <c>gift</c>；注意与礼品卡 <see cref="GeneralCard"/> 不是一回事）。</summary>
    [JsonPropertyName("gift")]
    public MpCardCreateGift? Gift { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>general_coupon</c>）。</summary>
    [JsonPropertyName("general_coupon")]
    public MpCardCreateGeneralCoupon? GeneralCoupon { get; set; }

    /// <summary>获取或设置<b>礼品卡</b>信息（官方 <c>general_card</c>）。</summary>
    [JsonPropertyName("general_card")]
    public MpCardCreateGeneralCard? GeneralCard { get; set; }

    /// <summary>获取或设置<b>会员卡</b>信息（官方 <c>member_card</c>）。</summary>
    [JsonPropertyName("member_card")]
    public MpCardCreateMemberCard? MemberCard { get; set; }

    /// <summary>获取或设置会议门票信息（官方 <c>meeting_ticket</c>）。</summary>
    [JsonPropertyName("meeting_ticket")]
    public MpCardCreateMeetingTicket? MeetingTicket { get; set; }

    /// <summary>获取或设置景区门票信息（官方 <c>scenic_ticket</c>）。</summary>
    [JsonPropertyName("scenic_ticket")]
    public MpCardCreateScenicTicket? ScenicTicket { get; set; }

    /// <summary>获取或设置电影票信息（官方 <c>movie_ticket</c>）。</summary>
    [JsonPropertyName("movie_ticket")]
    public MpCardCreateMovieTicket? MovieTicket { get; set; }

    /// <summary>获取或设置飞机票信息（官方 <c>boarding_pass</c>）。</summary>
    [JsonPropertyName("boarding_pass")]
    public MpCardCreateBoardingPass? BoardingPass { get; set; }
}

/// <summary>团购券（官方 <c>card.groupon</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateGroupon
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置团购详情（官方 <c>deal_detail</c>，必填）。</summary>
    [JsonPropertyName("deal_detail")]
    public string? DealDetail { get; set; }
}

/// <summary>代金券（官方 <c>card.cash</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateCash
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置起用金额（官方 <c>least_cost</c>，单位：分）。</summary>
    [JsonPropertyName("least_cost")]
    public int LeastCost { get; set; }

    /// <summary>获取或设置减免金额（官方 <c>reduce_cost</c>，单位：分）。</summary>
    [JsonPropertyName("reduce_cost")]
    public int ReduceCost { get; set; }
}

/// <summary>折扣券（官方 <c>card.discount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateDiscount
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置打折额度（官方 <c>discount</c>，如 80 表示八折）。</summary>
    [JsonPropertyName("discount")]
    public int Discount { get; set; }
}

/// <summary>兑换券（官方 <c>card.gift</c> —— 键名 <c>gift</c> 指「兑换券」，非礼品卡）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateGift
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置兑换内容（官方 <c>gift</c>，必填）。</summary>
    [JsonPropertyName("gift")]
    public string? Gift { get; set; }
}

/// <summary>优惠券（官方 <c>card.general_coupon</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateGeneralCoupon
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置优惠详情（官方 <c>default_detail</c>，必填；<b>官方键名为 <c>default_detail</c></b>）。</summary>
    [JsonPropertyName("default_detail")]
    public string? DefaultDetail { get; set; }
}

/// <summary>
/// 礼品卡（官方 <c>card.general_card</c>，<b>2026-10-10 对齐基准为 SKIT</b>；建卡方向独有序列）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateGeneralCard
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，礼品卡形态见 <see cref="MpCardGiftCardBaseInfo"/>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardGiftCardBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置礼品卡子类型（官方 <c>sub_card_type</c>，建卡必填）。</summary>
    [JsonPropertyName("sub_card_type")]
    public string? SubCardType { get; set; }

    /// <summary>获取或设置背景图 URL（官方 <c>background_pic_url</c>，选填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置特权说明（官方 <c>prerogative</c>，必填）。</summary>
    [JsonPropertyName("prerogative")]
    public string? Prerogative { get; set; }

    /// <summary>获取或设置是否领取后自动激活（官方 <c>auto_activate</c>，选填）。</summary>
    [JsonPropertyName("auto_activate")]
    public bool? AutoActivate { get; set; }

    /// <summary>获取或设置是否支持积分（官方 <c>supply_bonus</c>，必填）。</summary>
    [JsonPropertyName("supply_bonus")]
    public bool SupplyBonus { get; set; }

    /// <summary>获取或设置初始积分（官方 <c>init_bonus</c>，选填）。</summary>
    [JsonPropertyName("init_bonus")]
    public int? InitBonus { get; set; }

    /// <summary>获取或设置是否支持余额（官方 <c>supply_balance</c>，必填）。</summary>
    [JsonPropertyName("supply_balance")]
    public bool SupplyBalance { get; set; }

    /// <summary>获取或设置初始余额（官方 <c>init_balance</c>，单位：分，选填）。</summary>
    [JsonPropertyName("init_balance")]
    public int? InitBalance { get; set; }

    /// <summary>获取或设置自定义会员信息类目 1（官方 <c>custom_field1</c>）。</summary>
    [JsonPropertyName("custom_field1")]
    public MpCardCustomField? CustomField1 { get; set; }

    /// <summary>获取或设置自定义会员信息类目 2（官方 <c>custom_field2</c>）。</summary>
    [JsonPropertyName("custom_field2")]
    public MpCardCustomField? CustomField2 { get; set; }

    /// <summary>获取或设置自定义会员信息类目 3（官方 <c>custom_field3</c>）。</summary>
    [JsonPropertyName("custom_field3")]
    public MpCardCustomField? CustomField3 { get; set; }
}

/// <summary>
/// 礼品卡的 <c>base_info</c>（建卡方向）—— 通用形态 + <c>giftcard_info</c> + <c>max_give_friend_times</c> +
/// <c>need_push_on_view</c>。
/// </summary>
/// <remarks>
/// <b>继承而非重复声明</b>：本型在 <see cref="MpCardCreateBaseInfo"/> 之上<b>只增三字段</b>，
/// 与 SKIT 的 <c>GeneralCard.Types.Base : GrouponCard.Types.Base</c> 同构；守卫 CD4 锁定「继承即等价于
/// 通用形态字段全集」这一复用选择（若日后官方礼品卡页删除某通用字段，须改为分建而非改基类）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftCardBaseInfo : MpCardCreateBaseInfo
{
    /// <summary>获取或设置礼品卡面额（官方 <c>giftcard_info</c>，建卡必填）。</summary>
    [JsonPropertyName("giftcard_info")]
    public MpCardGiftCardPriceInfo? GiftCardInfo { get; set; }

    /// <summary>获取或设置最大可赠送次数（官方 <c>max_give_friend_times</c>，必填）。</summary>
    [JsonPropertyName("max_give_friend_times")]
    public int MaxGiveFriendTimes { get; set; }

    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>，选填）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }
}

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

/// <summary>
/// 会员卡的 <c>base_info</c>（建卡方向）—— 通用形态 + <c>need_push_on_view</c> + <c>pay_info</c>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberCardBaseInfo : MpCardCreateBaseInfo
{
    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>，选填）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }

    /// <summary>获取或设置微信支付信息（官方 <c>pay_info</c>，选填）。</summary>
    [JsonPropertyName("pay_info")]
    public MpCardPayInfo? PayInfo { get; set; }
}

/// <summary>会议门票（官方 <c>card.meeting_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateMeetingTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置会议详情（官方 <c>meeting_detail</c>，必填）。</summary>
    [JsonPropertyName("meeting_detail")]
    public string? MeetingDetail { get; set; }

    /// <summary>获取或设置会场导览图 URL（官方 <c>map_url</c>，选填）。</summary>
    [JsonPropertyName("map_url")]
    public string? MapUrl { get; set; }
}

/// <summary>景区门票（官方 <c>card.scenic_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateScenicTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置票种（官方 <c>ticket_class</c>，必填）。</summary>
    [JsonPropertyName("ticket_class")]
    public string? TicketClass { get; set; }

    /// <summary>获取或设置景区导览图 URL（官方 <c>guide_url</c>，选填）。</summary>
    [JsonPropertyName("guide_url")]
    public string? GuideUrl { get; set; }
}

/// <summary>电影票（官方 <c>card.movie_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateMovieTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置影片详情（官方 <c>detail</c>，必填；<b>官方键名为 <c>detail</c>，非 <c>movie_detail</c></b>）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

/// <summary>飞机票（官方 <c>card.boarding_pass</c>，字段面最长的一支门票）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateBoardingPass
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardCreateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置出发地（官方 <c>from</c>，必填）。</summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>获取或设置目的地（官方 <c>to</c>，必填）。</summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>获取或设置航班号（官方 <c>flight</c>，必填）。</summary>
    [JsonPropertyName("flight")]
    public string? Flight { get; set; }

    /// <summary>获取或设置登机口（官方 <c>gate</c>，选填）。</summary>
    [JsonPropertyName("gate")]
    public string? Gate { get; set; }

    /// <summary>获取或设置在线值机外链（官方 <c>check_in_url</c>，选填）。</summary>
    [JsonPropertyName("check_in_url")]
    public string? CheckInUrl { get; set; }

    /// <summary>获取或设置机型（官方 <c>air_model</c>，必填）。</summary>
    [JsonPropertyName("air_model")]
    public string? AirModel { get; set; }

    /// <summary>获取或设置起飞时间戳（官方 <c>departure_time</c>，秒，必填）。</summary>
    [JsonPropertyName("departure_time")]
    public long DepartureTime { get; set; }

    /// <summary>获取或设置降落时间戳（官方 <c>landing_time</c>，秒，必填）。</summary>
    [JsonPropertyName("landing_time")]
    public long LandingTime { get; set; }
}

/// <summary>
/// 创建卡券（<c>POST /card/create</c>）应答。
/// </summary>
/// <remarks><b>官方应答只有一个业务字段</b>：新建卡券的模板编号 <c>card_id</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateResponse : MpResponse
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}
