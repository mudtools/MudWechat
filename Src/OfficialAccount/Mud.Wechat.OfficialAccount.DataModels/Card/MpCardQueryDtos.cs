// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 查询卡券（POST /card/get）
//  应答为「card 包装 + card_type 判别 + 券型分支」，与建卡方向**分支集不同**（本向无 general_card），
//  且 base_info 为查询形态（含 id/status/时间戳）。分支之间互斥 ⇒ 各支独立对象，不得合并成万能对象。

/// <summary>
/// 查询卡券详情（<c>POST /card/get</c>）请求体。
/// </summary>
/// <remarks>官方请求体仅一个字段 <c>card_id</c>；<c>access_token</c> 走 Query、不入请求体。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGetRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}

/// <summary>
/// 查询卡券详情（<c>POST /card/get</c>）应答。
/// </summary>
/// <remarks><b>官方应答只有一个业务字段</b>：<c>card</c> 对象（其内 <c>base_info.status</c> 即卡券状态）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGetResponse : MpResponse
{
    /// <summary>获取或设置卡券对象（官方 <c>card</c>）。</summary>
    [JsonPropertyName("card")]
    public MpCardQueryDetail? Card { get; set; }
}

/// <summary>
/// 查询方向的 <c>card</c> 对象（<c>card_type</c> + 10 个券型分支，<b>分支互斥</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与建卡方向的分支差异（对齐基准：SKIT <c>CardGetResponse.Types.Card</c>，2026-10-10）</b>：
/// 本向<b>无</b> <c>general_card</c>（礼品卡）分支 —— 礼品卡的详情由 <c>/card/giftcard/*</c> 族承载，
/// 不经本接口返回。故本型刻意<b>不</b>与 <see cref="MpCardCreateBody"/> 共用。
/// </para>
/// <para><b>易错点</b>：官方建卡用 <c>gift</c> 表示「兑换券」，本向同名键亦为兑换券，<b>不是</b>礼品卡。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryDetail
{
    /// <summary>获取或设置卡券类型（官方 <c>card_type</c>，判别取哪个分支有值）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>获取或设置团购券信息（官方 <c>groupon</c>）。</summary>
    [JsonPropertyName("groupon")]
    public MpCardQueryGroupon? Groupon { get; set; }

    /// <summary>获取或设置代金券信息（官方 <c>cash</c>）。</summary>
    [JsonPropertyName("cash")]
    public MpCardQueryCash? Cash { get; set; }

    /// <summary>获取或设置折扣券信息（官方 <c>discount</c>）。</summary>
    [JsonPropertyName("discount")]
    public MpCardQueryDiscount? Discount { get; set; }

    /// <summary>获取或设置<b>兑换券</b>信息（官方 <c>gift</c>）。</summary>
    [JsonPropertyName("gift")]
    public MpCardQueryGift? Gift { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>general_coupon</c>）。</summary>
    [JsonPropertyName("general_coupon")]
    public MpCardQueryGeneralCoupon? GeneralCoupon { get; set; }

    /// <summary>获取或设置会员卡信息（官方 <c>member_card</c>）。</summary>
    [JsonPropertyName("member_card")]
    public MpCardQueryMemberCard? MemberCard { get; set; }

    /// <summary>获取或设置会议门票信息（官方 <c>meeting_ticket</c>）。</summary>
    [JsonPropertyName("meeting_ticket")]
    public MpCardQueryMeetingTicket? MeetingTicket { get; set; }

    /// <summary>获取或设置景区门票信息（官方 <c>scenic_ticket</c>）。</summary>
    [JsonPropertyName("scenic_ticket")]
    public MpCardQueryScenicTicket? ScenicTicket { get; set; }

    /// <summary>获取或设置电影票信息（官方 <c>movie_ticket</c>）。</summary>
    [JsonPropertyName("movie_ticket")]
    public MpCardQueryMovieTicket? MovieTicket { get; set; }

    /// <summary>获取或设置飞机票信息（官方 <c>boarding_pass</c>）。</summary>
    [JsonPropertyName("boarding_pass")]
    public MpCardQueryBoardingPass? BoardingPass { get; set; }
}

/// <summary>团购券（查询方向，官方 <c>card.groupon</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryGroupon
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，查询形态）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>，与建卡同表）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置团购详情（官方 <c>deal_detail</c>）。</summary>
    [JsonPropertyName("deal_detail")]
    public string? DealDetail { get; set; }
}

/// <summary>代金券（查询方向，官方 <c>card.cash</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryCash
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置起用金额（官方 <c>least_cost</c>，单位：分）。</summary>
    [JsonPropertyName("least_cost")]
    public int? LeastCost { get; set; }

    /// <summary>获取或设置减免金额（官方 <c>reduce_cost</c>，单位：分）。</summary>
    [JsonPropertyName("reduce_cost")]
    public int? ReduceCost { get; set; }
}

/// <summary>折扣券（查询方向，官方 <c>card.discount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryDiscount
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置打折额度（官方 <c>discount</c>，如 80 表示八折）。</summary>
    [JsonPropertyName("discount")]
    public int? Discount { get; set; }
}

/// <summary>兑换券（查询方向，官方 <c>card.gift</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryGift
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置兑换内容（官方 <c>gift</c>）。</summary>
    [JsonPropertyName("gift")]
    public string? Gift { get; set; }
}

/// <summary>优惠券（查询方向，官方 <c>card.general_coupon</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryGeneralCoupon
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置优惠详情（官方 <c>default_detail</c>）。</summary>
    [JsonPropertyName("default_detail")]
    public string? DefaultDetail { get; set; }
}

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

/// <summary>
/// 会员卡的 <c>base_info</c>（查询方向）—— 通用查询形态 + <c>need_push_on_view</c>。
/// </summary>
/// <remarks><b>与建卡方向的会员卡 base 不同</b>：查询形态<b>无</b> <c>pay_info</c>（SKIT 对齐基准）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryMemberCardBaseInfo : MpCardQueryBaseInfo
{
    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }
}

/// <summary>会议门票（查询方向，官方 <c>card.meeting_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryMeetingTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置会议详情（官方 <c>meeting_detail</c>）。</summary>
    [JsonPropertyName("meeting_detail")]
    public string? MeetingDetail { get; set; }

    /// <summary>获取或设置会场导览图 URL（官方 <c>map_url</c>）。</summary>
    [JsonPropertyName("map_url")]
    public string? MapUrl { get; set; }
}

/// <summary>景区门票（查询方向，官方 <c>card.scenic_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryScenicTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置票种（官方 <c>ticket_class</c>）。</summary>
    [JsonPropertyName("ticket_class")]
    public string? TicketClass { get; set; }

    /// <summary>获取或设置景区导览图 URL（官方 <c>guide_url</c>）。</summary>
    [JsonPropertyName("guide_url")]
    public string? GuideUrl { get; set; }
}

/// <summary>电影票（查询方向，官方 <c>card.movie_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryMovieTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置影片详情（官方 <c>detail</c>，<b>官方键名为 <c>detail</c></b>）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

/// <summary>飞机票（查询方向，官方 <c>card.boarding_pass</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryBoardingPass
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardQueryBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置出发地（官方 <c>from</c>）。</summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>获取或设置目的地（官方 <c>to</c>）。</summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>获取或设置航班号（官方 <c>flight</c>）。</summary>
    [JsonPropertyName("flight")]
    public string? Flight { get; set; }

    /// <summary>获取或设置登机口（官方 <c>gate</c>）。</summary>
    [JsonPropertyName("gate")]
    public string? Gate { get; set; }

    /// <summary>获取或设置在线值机外链（官方 <c>check_in_url</c>）。</summary>
    [JsonPropertyName("check_in_url")]
    public string? CheckInUrl { get; set; }

    /// <summary>获取或设置机型（官方 <c>air_model</c>）。</summary>
    [JsonPropertyName("air_model")]
    public string? AirModel { get; set; }

    /// <summary>获取或设置起飞时间戳（官方 <c>departure_time</c>，秒）。</summary>
    [JsonPropertyName("departure_time")]
    public long DepartureTime { get; set; }

    /// <summary>获取或设置降落时间戳（官方 <c>landing_time</c>，秒）。</summary>
    [JsonPropertyName("landing_time")]
    public long LandingTime { get; set; }
}

// ---------------------------------------------------------------- 获取卡券列表（POST /card/batchget）

/// <summary>
/// 获取卡券列表（<c>POST /card/batchget</c>）请求体。
/// </summary>
/// <remarks>
/// <b>分页形态</b>：官方为 <c>offset</c> + <c>count</c>（<b>无游标、无 page_info</b>），
/// 与素材域 <c>batchget</c> 的游标形态不同；<c>status_list</c> 为状态筛选数组（官方状态枚举<b>待逐页核验</b>，
/// SDK 取 <c>string</c> 不建枚举常量）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardBatchGetRequest
{
    /// <summary>获取或设置分页偏移量（官方 <c>offset</c>，必填，从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置单页取数上限（官方 <c>count</c>，必填；上限数值<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置卡券状态筛选（官方 <c>status_list</c>，选填；取值表<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("status_list")]
    public List<string>? StatusList { get; set; }
}

/// <summary>
/// 获取卡券列表（<c>POST /card/batchget</c>）应答。
/// </summary>
/// <remarks>
/// <b>字段名为 <c>card_id_list</c> / <c>total_num</c></b>（对齐基准：SKIT <c>CardBatchGetResponse</c>）——
/// <b>不是</b> <c>card_list</c> / <c>total_count</c>；SDK 不凭常识改写，守卫 CD6 锁定。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardBatchGetResponse : MpResponse
{
    /// <summary>获取或设置本页卡券模板编号列表（官方 <c>card_id_list</c>）。</summary>
    [JsonPropertyName("card_id_list")]
    public List<string>? CardIdList { get; set; }

    /// <summary>获取或设置卡券总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int TotalNum { get; set; }
}
