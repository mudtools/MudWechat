// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 修改卡券（POST /card/update）
//  请求形态与建卡**不同构**：本向为「card_id + 券型分支」**平级**（无 card 包装、无 card_type），
//  由「传了哪个分支键」判别改哪一支；且各分支**无 advanced_info**（高级信息建卡后不可改）。

/// <summary>
/// 修改卡券（<c>POST /card/update</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>顶层平级形态</b>（对齐基准：SKIT <c>CardUpdateRequest</c>，2026-10-10）：<c>card_id</c> 与 11 个券型分支
/// 同层，<b>没有</b>建卡那样的 <c>card</c> 包装、也<b>没有</b> <c>card_type</c> 判别字段。
/// 官方文档站正文本次不可达，该形态<b>待逐页核验</b>；守卫 CD4 锁定「本向无 card 包装」这一裁决，
/// 防止后人按建卡形态「顺手对齐」。
/// </para>
/// <para><b>分支互斥</b>：按 <c>card_id</c> 对应的实际券型只传该支，其余留空（不传即不改）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置团购券信息（官方 <c>groupon</c>）。</summary>
    [JsonPropertyName("groupon")]
    public MpCardUpdateGroupon? Groupon { get; set; }

    /// <summary>获取或设置代金券信息（官方 <c>cash</c>）。</summary>
    [JsonPropertyName("cash")]
    public MpCardUpdateCash? Cash { get; set; }

    /// <summary>获取或设置折扣券信息（官方 <c>discount</c>）。</summary>
    [JsonPropertyName("discount")]
    public MpCardUpdateDiscount? Discount { get; set; }

    /// <summary>获取或设置<b>兑换券</b>信息（官方 <c>gift</c>）。</summary>
    [JsonPropertyName("gift")]
    public MpCardUpdateGift? Gift { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>general_coupon</c>）。</summary>
    [JsonPropertyName("general_coupon")]
    public MpCardUpdateGeneralCoupon? GeneralCoupon { get; set; }

    /// <summary>获取或设置<b>礼品卡</b>信息（官方 <c>general_card</c>）。</summary>
    [JsonPropertyName("general_card")]
    public MpCardUpdateGeneralCard? GeneralCard { get; set; }

    /// <summary>获取或设置<b>会员卡</b>信息（官方 <c>member_card</c>）。</summary>
    [JsonPropertyName("member_card")]
    public MpCardUpdateMemberCard? MemberCard { get; set; }

    /// <summary>获取或设置会议门票信息（官方 <c>meeting_ticket</c>）。</summary>
    [JsonPropertyName("meeting_ticket")]
    public MpCardUpdateMeetingTicket? MeetingTicket { get; set; }

    /// <summary>获取或设置景区门票信息（官方 <c>scenic_ticket</c>）。</summary>
    [JsonPropertyName("scenic_ticket")]
    public MpCardUpdateScenicTicket? ScenicTicket { get; set; }

    /// <summary>获取或设置电影票信息（官方 <c>movie_ticket</c>）。</summary>
    [JsonPropertyName("movie_ticket")]
    public MpCardUpdateMovieTicket? MovieTicket { get; set; }

    /// <summary>获取或设置飞机票信息（官方 <c>boarding_pass</c>）。</summary>
    [JsonPropertyName("boarding_pass")]
    public MpCardUpdateBoardingPass? BoardingPass { get; set; }
}

/// <summary>团购券（修改方向，官方 <c>groupon</c>；<b>无 advanced_info</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateGroupon
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，修改形态：全字段选填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置团购详情（官方 <c>deal_detail</c>，选填）。</summary>
    [JsonPropertyName("deal_detail")]
    public string? DealDetail { get; set; }
}

/// <summary>代金券（修改方向，官方 <c>cash</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateCash
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置起用金额（官方 <c>least_cost</c>，单位：分，选填）。</summary>
    [JsonPropertyName("least_cost")]
    public int? LeastCost { get; set; }

    /// <summary>获取或设置减免金额（官方 <c>reduce_cost</c>，单位：分，选填）。</summary>
    [JsonPropertyName("reduce_cost")]
    public int? ReduceCost { get; set; }
}

/// <summary>折扣券（修改方向，官方 <c>discount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateDiscount
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置打折额度（官方 <c>discount</c>，选填）。</summary>
    [JsonPropertyName("discount")]
    public int? Discount { get; set; }
}

/// <summary>兑换券（修改方向，官方 <c>gift</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateGift
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置兑换内容（官方 <c>gift</c>，选填）。</summary>
    [JsonPropertyName("gift")]
    public string? Gift { get; set; }
}

/// <summary>优惠券（修改方向，官方 <c>general_coupon</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateGeneralCoupon
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置优惠详情（官方 <c>default_detail</c>，选填）。</summary>
    [JsonPropertyName("default_detail")]
    public string? DefaultDetail { get; set; }
}

/// <summary>
/// 礼品卡（修改方向，官方 <c>general_card</c>）。
/// </summary>
/// <remarks>
/// <b>字段面窄于建卡</b>：本向<b>无</b> <c>sub_card_type</c> / <c>giftcard_info</c> /
/// <c>max_give_friend_times</c>（卡种与面额建卡后不可改），也<b>无</b> <c>init_bonus</c> / <c>init_balance</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateGeneralCard
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，礼品卡修改形态）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateGiftCardBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置背景图 URL（官方 <c>background_pic_url</c>，选填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置特权说明（官方 <c>prerogative</c>，选填）。</summary>
    [JsonPropertyName("prerogative")]
    public string? Prerogative { get; set; }

    /// <summary>获取或设置是否领取后自动激活（官方 <c>auto_activate</c>，选填）。</summary>
    [JsonPropertyName("auto_activate")]
    public bool? AutoActivate { get; set; }

    /// <summary>获取或设置是否支持积分（官方 <c>supply_bonus</c>，选填）。</summary>
    [JsonPropertyName("supply_bonus")]
    public bool? SupplyBonus { get; set; }

    /// <summary>获取或设置是否支持余额（官方 <c>supply_balance</c>，选填）。</summary>
    [JsonPropertyName("supply_balance")]
    public bool? SupplyBalance { get; set; }

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

/// <summary>礼品卡的 <c>base_info</c>（修改方向）—— 通用修改形态 + <c>need_push_on_view</c>。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateGiftCardBaseInfo : MpCardUpdateBaseInfo
{
    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>，选填）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }
}

/// <summary>
/// 会员卡（修改方向，官方 <c>member_card</c>）—— 本域字段面最宽的分支。
/// </summary>
/// <remarks>
/// <para>
/// <b>与建卡方向的差异</b>：本向<b>多出</b>两个消息入口对象 <c>modify_msg_operation</c> /
/// <c>activate_msg_operation</c>（运营期设置，建卡页无该组），其余字段同名但全部选填（不传即不改）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateMemberCard
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，会员卡修改形态）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateMemberCardBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置背景图 URL（官方 <c>background_pic_url</c>，选填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置特权说明（官方 <c>prerogative</c>，选填）。</summary>
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

    /// <summary>获取或设置是否支持积分（官方 <c>supply_bonus</c>，选填）。</summary>
    [JsonPropertyName("supply_bonus")]
    public bool? SupplyBonus { get; set; }

    /// <summary>获取或设置查看积分详情外链（官方 <c>bonus_url</c>，选填）。</summary>
    [JsonPropertyName("bonus_url")]
    public string? BonusUrl { get; set; }

    /// <summary>获取或设置积分清零规则（官方 <c>bonus_cleared</c>，选填）。</summary>
    [JsonPropertyName("bonus_cleared")]
    public string? BonusCleared { get; set; }

    /// <summary>获取或设置积分规则字符串（官方 <c>bonus_rules</c>，选填）。</summary>
    [JsonPropertyName("bonus_rules")]
    public string? BonusRules { get; set; }

    /// <summary>获取或设置积分规则对象（官方 <c>bonus_rule</c>，选填）。</summary>
    [JsonPropertyName("bonus_rule")]
    public MpCardBonusRule? BonusRule { get; set; }

    /// <summary>获取或设置是否支持储值（官方 <c>supply_balance</c>，选填）。</summary>
    [JsonPropertyName("supply_balance")]
    public bool? SupplyBalance { get; set; }

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

    /// <summary>获取或设置会员专享折扣（官方 <c>discount</c>，选填）。</summary>
    [JsonPropertyName("discount")]
    public int? Discount { get; set; }

    /// <summary>获取或设置「修改信息」消息入口（官方 <c>modify_msg_operation</c>，<b>修改方向独有</b>）。</summary>
    [JsonPropertyName("modify_msg_operation")]
    public MpCardMessageOperation? ModifyMsgOperation { get; set; }

    /// <summary>获取或设置「领卡后」消息入口（官方 <c>activate_msg_operation</c>，<b>修改方向独有</b>）。</summary>
    [JsonPropertyName("activate_msg_operation")]
    public MpCardMessageOperation? ActivateMsgOperation { get; set; }
}

/// <summary>会员卡的 <c>base_info</c>（修改方向）—— 通用修改形态 + <c>need_push_on_view</c> + <c>pay_info</c>。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateMemberCardBaseInfo : MpCardUpdateBaseInfo
{
    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>，选填）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }

    /// <summary>获取或设置微信支付信息（官方 <c>pay_info</c>，选填）。</summary>
    [JsonPropertyName("pay_info")]
    public MpCardPayInfo? PayInfo { get; set; }
}

/// <summary>会议门票（修改方向，官方 <c>meeting_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateMeetingTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置会议详情（官方 <c>meeting_detail</c>，选填）。</summary>
    [JsonPropertyName("meeting_detail")]
    public string? MeetingDetail { get; set; }

    /// <summary>获取或设置会场导览图 URL（官方 <c>map_url</c>，选填）。</summary>
    [JsonPropertyName("map_url")]
    public string? MapUrl { get; set; }
}

/// <summary>景区门票（修改方向，官方 <c>scenic_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateScenicTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置票种（官方 <c>ticket_class</c>，选填）。</summary>
    [JsonPropertyName("ticket_class")]
    public string? TicketClass { get; set; }

    /// <summary>获取或设置景区导览图 URL（官方 <c>guide_url</c>，选填）。</summary>
    [JsonPropertyName("guide_url")]
    public string? GuideUrl { get; set; }
}

/// <summary>电影票（修改方向，官方 <c>movie_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateMovieTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置影片详情（官方 <c>detail</c>，选填）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

/// <summary>飞机票（修改方向，官方 <c>boarding_pass</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateBoardingPass
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置出发地（官方 <c>from</c>，选填）。</summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>获取或设置目的地（官方 <c>to</c>，选填）。</summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>获取或设置航班号（官方 <c>flight</c>，选填）。</summary>
    [JsonPropertyName("flight")]
    public string? Flight { get; set; }

    /// <summary>获取或设置登机口（官方 <c>gate</c>，选填）。</summary>
    [JsonPropertyName("gate")]
    public string? Gate { get; set; }

    /// <summary>获取或设置在线值机外链（官方 <c>check_in_url</c>，选填）。</summary>
    [JsonPropertyName("check_in_url")]
    public string? CheckInUrl { get; set; }

    /// <summary>获取或设置机型（官方 <c>air_model</c>，选填）。</summary>
    [JsonPropertyName("air_model")]
    public string? AirModel { get; set; }

    /// <summary>获取或设置起飞时间戳（官方 <c>departure_time</c>，秒，选填）。</summary>
    [JsonPropertyName("departure_time")]
    public long? DepartureTime { get; set; }

    /// <summary>获取或设置降落时间戳（官方 <c>landing_time</c>，秒，选填）。</summary>
    [JsonPropertyName("landing_time")]
    public long? LandingTime { get; set; }
}

/// <summary>
/// 修改卡券（<c>POST /card/update</c>）应答。
/// </summary>
/// <remarks>
/// <b>本向应答不回传 <c>card_id</c></b>（与建卡不同），只有是否需要送审的 <c>send_check</c>。
/// 名称取 <see cref="RequireSendCheck"/> 以贴合官方语义「是否需要提交审核」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateResponse : MpResponse
{
    /// <summary>获取或设置是否需要提交审核（官方 <c>send_check</c>）。</summary>
    [JsonPropertyName("send_check")]
    public bool RequireSendCheck { get; set; }
}

// ---------------------------------------------------------------- 修改库存（POST /card/modifystock）

/// <summary>
/// 修改卡券库存（<c>POST /card/modifystock</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>键名为 <c>increase_stock_value</c> / <c>reduce_stock_value</c></b>（对齐基准：SKIT
/// <c>CardModifyStockRequest</c>）—— <b>不是</b>建卡 <c>sku.quantity</c> 的「quantity」词族，也<b>不是</b>
/// <c>increase_quantity</c>；SDK 不凭常识改写，守卫 CD6 锁定。
/// </para>
/// <para>
/// <b>增删同传</b>：两字段均为「增量」语义（非目标值），官方允许同时携带 ⇒ SDK <b>不做互斥校验</b>，
/// 越界由官方错误码表达。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardModifyStockRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置增加库存数（官方 <c>increase_stock_value</c>，选填，正整数增量）。</summary>
    [JsonPropertyName("increase_stock_value")]
    public int? IncreaseStockValue { get; set; }

    /// <summary>获取或设置减少库存数（官方 <c>reduce_stock_value</c>，选填，正整数减量）。</summary>
    [JsonPropertyName("reduce_stock_value")]
    public int? ReduceStockValue { get; set; }
}

// ---------------------------------------------------------------- 删除卡券（POST /card/delete）

/// <summary>删除卡券（<c>POST /card/delete</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardDeleteRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}
