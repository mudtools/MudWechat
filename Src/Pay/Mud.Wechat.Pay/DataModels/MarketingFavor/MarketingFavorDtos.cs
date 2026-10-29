// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MarketingFavor;

/// <summary>
/// 创建代金券批次（<c>POST /v3/marketing/favor/coupon-stocks</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534633"/>
/// （2026-10-09 逐字段核验；更新时间 2024.10.31；产品归属「营销产品 &gt; 代金券」；
/// <b>支持商户：【普通商户】</b>，官方未声明下线）。
/// </para>
/// <para>
/// <b>官方流程地位</b>：本接口只<b>创建</b>批次，<b>创建后须再调「激活代金券批次」</b>
/// （<c>POST /v3/marketing/favor/stocks/{stock_id}/start</c>）该批次才可发放 —— 勿把创建成功当成可发放。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockCreateRequest
{
    /// <summary>批次名称（<c>stock_name</c>，必填）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>备注（<c>comment</c>，选填）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>归属商户号（<c>belong_merchant</c>，必填）。</summary>
    [JsonPropertyName("belong_merchant")]
    public string? BelongMerchant { get; set; }

    /// <summary>可用开始时间（<c>available_begin_time</c>，必填）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>可用结束时间（<c>available_end_time</c>，必填）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>发放规则（<c>stock_use_rule</c>，必填），见 <see cref="CouponStockUseRule"/>。</summary>
    [JsonPropertyName("stock_use_rule")]
    public CouponStockUseRule? StockUseRule { get; set; }

    /// <summary>代金券详情页（<c>pattern_info</c>，选填），见 <see cref="CouponPatternInfo"/>。</summary>
    [JsonPropertyName("pattern_info")]
    public CouponPatternInfo? PatternInfo { get; set; }

    /// <summary>核销规则（<c>coupon_use_rule</c>，必填），见 <see cref="CouponUseRule"/>。</summary>
    [JsonPropertyName("coupon_use_rule")]
    public CouponUseRule? CouponUseRule { get; set; }

    /// <summary>是否无资金流（<c>no_cash</c>，必填）。</summary>
    [JsonPropertyName("no_cash")]
    public bool? NoCash { get; set; }

    /// <summary>批次类型（<c>stock_type</c>，必填）：<c>NORMAL</c> 等（取值表未在本轮核验）。</summary>
    [JsonPropertyName("stock_type")]
    public string? StockType { get; set; }

    /// <summary>商户单据号（<c>out_request_no</c>，必填）：<b>业务幂等键</b>。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>扩展信息（<c>ext_info</c>，选填）。</summary>
    [JsonPropertyName("ext_info")]
    public string? ExtInfo { get; set; }
}

/// <summary>发放规则（<c>stock_use_rule</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockUseRule
{
    /// <summary>总发放量（<c>max_coupons</c>，必填）。</summary>
    [JsonPropertyName("max_coupons")]
    public long? MaxCoupons { get; set; }

    /// <summary>最大发放金额（<c>max_amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("max_amount")]
    public long? MaxAmount { get; set; }

    /// <summary>单日最大发放金额（<c>max_amount_by_day</c>，选填，单位分）。</summary>
    [JsonPropertyName("max_amount_by_day")]
    public long? MaxAmountByDay { get; set; }

    /// <summary>单个用户可领数（<c>max_coupons_per_user</c>，必填）。</summary>
    [JsonPropertyName("max_coupons_per_user")]
    public long? MaxCouponsPerUser { get; set; }

    /// <summary>是否限制自然人（<c>natural_person_limit</c>，必填）。</summary>
    [JsonPropertyName("natural_person_limit")]
    public bool? NaturalPersonLimit { get; set; }

    /// <summary>是否防刷（<c>prevent_api_abuse</c>，必填）。</summary>
    [JsonPropertyName("prevent_api_abuse")]
    public bool? PreventApiAbuse { get; set; }
}

/// <summary>代金券详情页（<c>pattern_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponPatternInfo
{
    /// <summary>使用说明（<c>description</c>，必填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>商户 logo（<c>merchant_logo</c>，选填）。</summary>
    [JsonPropertyName("merchant_logo")]
    public string? MerchantLogo { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，选填）。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>背景色（<c>background_color</c>，选填）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>券图（<c>coupon_image</c>，选填）。</summary>
    [JsonPropertyName("coupon_image")]
    public string? CouponImage { get; set; }

    /// <summary>跳转目标（<c>jump_target</c>，选填）。</summary>
    [JsonPropertyName("jump_target")]
    public string? JumpTarget { get; set; }

    /// <summary>小程序 AppID（<c>mini_program_appid</c>，选填）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppId { get; set; }

    /// <summary>小程序路径（<c>mini_program_path</c>，选填）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }
}

/// <summary>
/// 核销规则（<c>coupon_use_rule</c>）。
/// </summary>
/// <remarks>
/// 官方本对象内多数子字段为选填，但 <c>available_merchants</c> 标为<b>必填</b>；
/// 且 <c>fixed_normal_coupon</c> 在 <c>stock_type = NORMAL</c> 时<b>必填</b>（跨字段条件，已留档）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponUseRule
{
    /// <summary>券生效时间（<c>coupon_available_time</c>，选填），见 <see cref="CouponAvailableTime"/>。</summary>
    [JsonPropertyName("coupon_available_time")]
    public CouponAvailableTime? CouponAvailableTime { get; set; }

    /// <summary>固定面额满减券规则（<c>fixed_normal_coupon</c>，选填；<c>stock_type=NORMAL</c> 时必填），见 <see cref="CouponFixedNormalCoupon"/>。</summary>
    [JsonPropertyName("fixed_normal_coupon")]
    public CouponFixedNormalCoupon? FixedNormalCoupon { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>可用支付方式（<c>trade_type</c>，选填）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>是否可叠加（<c>combine_use</c>，选填）。</summary>
    [JsonPropertyName("combine_use")]
    public bool? CombineUse { get; set; }

    /// <summary>可用单品（<c>available_items</c>，选填）。</summary>
    [JsonPropertyName("available_items")]
    public string? AvailableItems { get; set; }

    /// <summary>不可用单品（<c>unavailable_items</c>，选填）。</summary>
    [JsonPropertyName("unavailable_items")]
    public string? UnavailableItems { get; set; }

    /// <summary>可用商户（<c>available_merchants</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("available_merchants")]
    public string? AvailableMerchants { get; set; }

    /// <summary>指定银行卡 BIN（<c>limit_card</c>，选填），见 <see cref="CouponLimitCard"/>。</summary>
    [JsonPropertyName("limit_card")]
    public CouponLimitCard? LimitCard { get; set; }
}

/// <summary>券生效时间（<c>coupon_available_time</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponAvailableTime
{
    /// <summary>固定时间段可用（<c>fix_available_time</c>，选填），见 <see cref="CouponFixAvailableTime"/>。</summary>
    [JsonPropertyName("fix_available_time")]
    public CouponFixAvailableTime? FixAvailableTime { get; set; }

    /// <summary>次日生效（<c>second_day_available</c>，选填）。</summary>
    [JsonPropertyName("second_day_available")]
    public bool? SecondDayAvailable { get; set; }

    /// <summary>领取后多久可用（<c>available_time_after_receive</c>，选填）。</summary>
    [JsonPropertyName("available_time_after_receive")]
    public long? AvailableTimeAfterReceive { get; set; }
}

/// <summary>固定时间段可用（<c>fix_available_time</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponFixAvailableTime
{
    /// <summary>可用星期（<c>available_week_day</c>，必填）。</summary>
    [JsonPropertyName("available_week_day")]
    public long? AvailableWeekDay { get; set; }

    /// <summary>开始时间（<c>begin_time</c>，必填）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>结束时间（<c>end_time</c>，选填）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }
}

/// <summary>固定面额满减券规则（<c>fixed_normal_coupon</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponFixedNormalCoupon
{
    /// <summary>券面额（<c>coupon_amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("coupon_amount")]
    public long? CouponAmount { get; set; }

    /// <summary>门槛金额（<c>transaction_minimum</c>，必填，单位分）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }
}

/// <summary>指定银行卡 BIN（<c>limit_card</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponLimitCard
{
    /// <summary>银行名称（<c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>银行卡 BIN（<c>bin</c>，必填）。</summary>
    [JsonPropertyName("bin")]
    public string? Bin { get; set; }
}

/// <summary>创建代金券批次应答（<b>仅 2 字段</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockCreateResponse : WechatPayResponse
{
    /// <summary>批次号（<c>stock_id</c>，必填）：微信为每个批次分配的唯一 ID。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>创建时间（<c>create_time</c>，必填，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}

/// <summary>
/// 查询代金券详情应答（<c>GET /v3/marketing/favor/users/{openid}/coupons/{coupon_id}?appid=…</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012486942"/>
/// （2026-10-09 逐字段核验；更新时间 2024.09.19；支持商户：普通商户；官方注明<b>支持幂等重入</b>，
/// 且「支持批次创建商户号与批次发放商户调用」）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponQueryResponse : WechatPayResponse
{
    /// <summary>批次创建商户号（<c>stock_creator_mchid</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchId { get; set; }

    /// <summary>批次号（<c>stock_id</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>代金券 ID（<c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>立减券信息（<c>cut_to_message</c>），见 <see cref="CouponCutToMessage"/>。</summary>
    [JsonPropertyName("cut_to_message")]
    public CouponCutToMessage? CutToMessage { get; set; }

    /// <summary>券名称（<c>coupon_name</c>）。</summary>
    [JsonPropertyName("coupon_name")]
    public string? CouponName { get; set; }

    /// <summary>券状态（<c>status</c>）：<b>值域未在本轮核验</b>，勿臆造取值判定。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>使用说明（<c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>创建时间（<c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>券类型（<c>coupon_type</c>）：值域未在本轮核验。</summary>
    [JsonPropertyName("coupon_type")]
    public string? CouponType { get; set; }

    /// <summary>是否无资金流（<c>no_cash</c>）。</summary>
    [JsonPropertyName("no_cash")]
    public bool? NoCash { get; set; }

    /// <summary>可用开始时间（<c>available_begin_time</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>可用结束时间（<c>available_end_time</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>是否单品券（<c>singleitem</c>）。</summary>
    [JsonPropertyName("singleitem")]
    public bool? SingleItem { get; set; }

    /// <summary>普通券信息（<c>normal_coupon_information</c>），见 <see cref="CouponNormalCouponInformation"/>。</summary>
    [JsonPropertyName("normal_coupon_information")]
    public CouponNormalCouponInformation? NormalCouponInformation { get; set; }

    /// <summary>商户单据号（<c>out_request_no</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>可用余额（<c>available_balance</c>，单位分）。</summary>
    [JsonPropertyName("available_balance")]
    public long? AvailableBalance { get; set; }

    /// <summary>业务类型（<c>business_type</c>）：值域未在本轮核验。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>折扣券信息（<c>discount_msg</c>），见 <see cref="CouponDiscountMessage"/>。</summary>
    [JsonPropertyName("discount_msg")]
    public CouponDiscountMessage? DiscountMessage { get; set; }
}

/// <summary>立减券信息（<c>cut_to_message</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponCutToMessage
{
    /// <summary>单品最高价（<c>single_price_max</c>，单位分）。</summary>
    [JsonPropertyName("single_price_max")]
    public long? SinglePriceMax { get; set; }

    /// <summary>立减金额（<c>cut_to_price</c>，单位分）。</summary>
    [JsonPropertyName("cut_to_price")]
    public long? CutToPrice { get; set; }
}

/// <summary>普通券信息（<c>normal_coupon_information</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponNormalCouponInformation
{
    /// <summary>券面额（<c>coupon_amount</c>，单位分）。</summary>
    [JsonPropertyName("coupon_amount")]
    public long? CouponAmount { get; set; }

    /// <summary>门槛金额（<c>transaction_minimum</c>，单位分）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }
}

/// <summary>折扣券信息（<c>discount_msg</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponDiscountMessage
{
    /// <summary>折扣上限（<c>discount_amount_max</c>，单位分）。</summary>
    [JsonPropertyName("discount_amount_max")]
    public long? DiscountAmountMax { get; set; }

    /// <summary>折扣比例（<c>discount_percent</c>）。</summary>
    [JsonPropertyName("discount_percent")]
    public long? DiscountPercent { get; set; }

    /// <summary>门槛金额（<c>transaction_minimum</c>，单位分）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }
}

// ======================= 批次管理（激活 / 暂停 / 重启 / 查询）与发放 =======================

/// <summary>
/// 批次生命周期操作请求（<b>激活 / 暂停 / 重启三接口共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>（2026-10-09 逐字段核验，更新时间均为 2024.09.19）：
/// 激活 <c>…/cash-coupons/stock/start-stock.html</c>、
/// 暂停 <c>…/cash-coupons/stock/pause-stock.html</c>、
/// 重启 <c>…/cash-coupons/stock/restart-stock.html</c>。
/// </para>
/// <para>
/// <b>为何三接口共用一个请求类型</b>：三页的请求体字段表<b>完全相同</b> ——
/// 只有 <c>stock_creator_mchid</c> 一个必填字段（批次号走 path）⇒ 表相同则共用；
/// 而三者的<b>应答</b>字段名各不相同（<c>start_time</c> / <c>pause_time</c> / <c>restart_time</c>）
/// ⇒ 应答<b>必须</b>分建三个类型。
/// </para>
/// <para><b>幂等</b>：三接口官方均注明「接口支持幂等重入」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockOperationRequest
{
    /// <summary>创建批次的商户号（<c>stock_creator_mchid</c>，必填 string(20)）：须为批次的创建方。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchId { get; set; }
}

/// <summary>激活代金券批次应答（<c>POST /v3/marketing/favor/stocks/{stock_id}/start</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockStartResponse : WechatPayResponse
{
    /// <summary>激活时间（<c>start_time</c>，rfc3339）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>批次号（<c>stock_id</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }
}

/// <summary>暂停代金券批次应答（<c>POST /v3/marketing/favor/stocks/{stock_id}/pause</c>）。</summary>
/// <remarks>
/// <b>为何不与激活应答共用</b>：字段名不同（本类为 <c>pause_time</c>）——
/// 合并会让「激活」的调用方读到一个<b>永不返回</b>的 <c>pause_time</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockPauseResponse : WechatPayResponse
{
    /// <summary>暂停时间（<c>pause_time</c>，rfc3339）。</summary>
    [JsonPropertyName("pause_time")]
    public string? PauseTime { get; set; }

    /// <summary>批次号（<c>stock_id</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }
}

/// <summary>重启代金券批次应答（<c>POST /v3/marketing/favor/stocks/{stock_id}/restart</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockRestartResponse : WechatPayResponse
{
    /// <summary>重启时间（<c>restart_time</c>，rfc3339）。</summary>
    [JsonPropertyName("restart_time")]
    public string? RestartTime { get; set; }

    /// <summary>批次号（<c>stock_id</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }
}

/// <summary>
/// 查询批次详情应答（<c>GET /v3/marketing/favor/stocks/{stock_id}?stock_creator_mchid=…</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/query-stock.html"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.25）。<b>支持商户：【普通商户】</b>；官方注明支持幂等重入。
/// </para>
/// <para>
/// <b>部分字段有前置条件</b>：<c>business_type</c> / <c>available_region_list</c> /
/// <c>available_industry_list</c> 三者<b>仅当</b> <c>business_type = MULTIUSE</c>（消费金）时返回
/// ⇒ 普通代金券批次的这三项<b>恒为 null</b>，消费侧不得据此判错。
/// </para>
/// <para>
/// <b>可发放判定</b>：只有 <c>status = running</c>（见 <see cref="CouponStockStatuses"/>）才可调发放接口。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockQueryResponse : WechatPayResponse
{
    /// <summary>批次号（<c>stock_id</c>，必填）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>批次创建方商户号（<c>stock_creator_mchid</c>，必填）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchId { get; set; }

    /// <summary>批次名称（<c>stock_name</c>，必填）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>批次状态（<c>status</c>，必填，<b>全小写</b>）：取值见 <see cref="CouponStockStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>创建时间（<c>create_time</c>，必填，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>使用说明 / 批次描述（<c>description</c>，必填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>满减券 / 消费金批次使用规则（<c>stock_use_rule</c>，选填），见 <see cref="CouponStockQueryUseRule"/>。</summary>
    [JsonPropertyName("stock_use_rule")]
    public CouponStockQueryUseRule? StockUseRule { get; set; }

    /// <summary>可用开始时间（<c>available_begin_time</c>，必填，rfc3339）：<b>顶层字段</b>，不在 <c>stock_use_rule</c> 内。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>可用结束时间（<c>available_end_time</c>，必填，rfc3339）：同样是顶层字段。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>已发券数量（<c>distributed_coupons</c>，必填）。</summary>
    [JsonPropertyName("distributed_coupons")]
    public long? DistributedCoupons { get; set; }

    /// <summary>是否无资金流（<c>no_cash</c>，必填）：官方说明里「是」的英文拼写为 <c>ture</c>（<b>官方原文如此</b>，照录）。</summary>
    [JsonPropertyName("no_cash")]
    public bool? NoCash { get; set; }

    /// <summary>激活批次的时间（<c>start_time</c>，选填）：未激活时为 <c>null</c>。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>终止批次的时间（<c>stop_time</c>，选填）。</summary>
    [JsonPropertyName("stop_time")]
    public string? StopTime { get; set; }

    /// <summary>减至批次特定信息（<c>cut_to_message</c>，选填）：字段表与券详情的同名字段一致 ⇒ 复用 <see cref="CouponCutToMessage"/>。</summary>
    [JsonPropertyName("cut_to_message")]
    public CouponCutToMessage? CutToMessage { get; set; }

    /// <summary>是否单品优惠（<c>singleitem</c>，必填）。</summary>
    [JsonPropertyName("singleitem")]
    public bool? SingleItem { get; set; }

    /// <summary>批次类型（<c>stock_type</c>，必填）：取值见 <see cref="CouponStockTypes"/>。</summary>
    [JsonPropertyName("stock_type")]
    public string? StockType { get; set; }

    /// <summary>微信卡包 ID（<c>card_id</c>，选填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>业务类型（<c>business_type</c>，选填）：<b>仅</b>消费金（<c>MULTIUSE</c>）时返回，取值见 <see cref="CouponBusinessTypes"/>。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>消费金可用地域（<c>available_region_list</c>，选填）：<b>仅</b>消费金时返回，见 <see cref="CouponAvailableRegion"/>。</summary>
    [JsonPropertyName("available_region_list")]
    public List<CouponAvailableRegion>? AvailableRegionList { get; set; }

    /// <summary>消费金可用行业（<c>available_industry_list</c>，选填）：<b>仅</b>消费金时返回。</summary>
    [JsonPropertyName("available_industry_list")]
    public List<string>? AvailableIndustryList { get; set; }
}

/// <summary>
/// 查询批次详情的 <c>stock_use_rule</c>（<b>10 字段</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>⚠️ 这是本域<b>第三</b>张「使用规则」字段表，三者互不相同</b>：
/// ① 创建批次的 <c>stock_use_rule</c> → <see cref="CouponStockUseRule"/>（6 字段，含
/// <c>natural_person_limit</c> / <c>prevent_api_abuse</c>）；
/// ② 创建批次的 <c>coupon_use_rule</c> → <see cref="CouponUseRule"/>（9 字段，含
/// <c>coupon_available_time</c> / 可用单品 / 可用商户）；
/// ③ <b>本类</b>（查询应答）：10 字段，含 <c>coupon_type</c> / <c>fixed_discount_coupon</c>
/// 而<b>无</b> <c>available_time</c>（可用时间在应答<b>顶层</b>的两个独立字段里）。
/// 三者任一方向合并都会给出「某接口永不返回的字段」，调用方据此写出的分支永不命中。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockQueryUseRule
{
    /// <summary>发放总上限（<c>max_coupons</c>，必填）。</summary>
    [JsonPropertyName("max_coupons")]
    public long? MaxCoupons { get; set; }

    /// <summary>总预算（<c>max_amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("max_amount")]
    public long? MaxAmount { get; set; }

    /// <summary>单天发放上限金额（<c>max_amount_by_day</c>，必填，单位分）。</summary>
    [JsonPropertyName("max_amount_by_day")]
    public long? MaxAmountByDay { get; set; }

    /// <summary>固定面额批次特定信息（<c>fixed_normal_coupon</c>，选填）：字段表与创建侧一致 ⇒ 复用 <see cref="CouponFixedNormalCoupon"/>。</summary>
    [JsonPropertyName("fixed_normal_coupon")]
    public CouponFixedNormalCoupon? FixedNormalCoupon { get; set; }

    /// <summary>单个用户可领个数（<c>max_coupons_per_user</c>，必填）。</summary>
    [JsonPropertyName("max_coupons_per_user")]
    public long? MaxCouponsPerUser { get; set; }

    /// <summary>券或消费金类型（<c>coupon_type</c>，选填）：取值见 <see cref="CouponTypes"/>。</summary>
    [JsonPropertyName("coupon_type")]
    public string? CouponType { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填，array）：官方注明「该字段<b>暂未开放返回</b>」。</summary>
    [JsonPropertyName("goods_tag")]
    public List<string>? GoodsTag { get; set; }

    /// <summary>指定支付模式（<c>trade_type</c>，选填，array）：取值见 <see cref="CouponTradeTypes"/>；默认不限制。</summary>
    [JsonPropertyName("trade_type")]
    public List<string>? TradeType { get; set; }

    /// <summary>是否可叠加其他优惠（<c>combine_use</c>，选填）。</summary>
    [JsonPropertyName("combine_use")]
    public bool? CombineUse { get; set; }

    /// <summary>
    /// 固定折扣特定信息（<c>fixed_discount_coupon</c>，选填）：三字段与券详情的
    /// <c>discount_msg</c> 逐项一致 ⇒ 复用 <see cref="CouponDiscountMessage"/>
    /// （同一份事实两处漂移是本仓明令避免的形态）。
    /// </summary>
    [JsonPropertyName("fixed_discount_coupon")]
    public CouponDiscountMessage? FixedDiscountCoupon { get; set; }
}

/// <summary>
/// 消费金可用地域（<c>available_region_list</c> 项）—— <b>仅</b>消费金批次返回。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponAvailableRegion
{
    /// <summary>类型（<c>type</c>，选填）：取值见 <see cref="CouponRegionTypes"/>（国家 / 省 / 市 / 区级）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>省（<c>province</c>，选填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>市（<c>city</c>，选填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区（<c>district</c>，选填）。</summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>国家（<c>country</c>，选填）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

/// <summary>
/// 发放指定批次的代金券请求（<c>POST /v3/marketing/favor/users/{openid}/coupons</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463767"/>
/// （2026-10-09 逐字段核验；更新时间 2024.09.19）。
/// </para>
/// <para>
/// <b>前置校验（官方错误码口径）</b>：批次状态必须为「运营中」（<c>running</c>），
/// 否则报 <c>param_error</c>（「非法的批次状态」）；批次预算不足报 <c>not_enough</c>。
/// </para>
/// <para>
/// <b><c>out_request_no</c> 是商户侧的幂等键</b>：官方标注必填（string(128)）——
/// 重试发放时须沿用同一值，避免重复发券。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponIssueRequest
{
    /// <summary>批次号（<c>stock_id</c>，必填 string(20)）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>商户发放凭据号（<c>out_request_no</c>，必填 string(128)）：<b>商户侧幂等键</b>。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(128)）：须与 path 中 openid 对应。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>创建批次的商户号（<c>stock_creator_mchid</c>，必填 string(20)）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchId { get; set; }

    /// <summary>
    /// 券面额（<c>coupon_value</c>，选填 integer）：<b>官方标注「暂未开放」</b> ⇒
    /// 本仓保留字段以求契约完整，但<b>不应</b>由业务侧依赖（填了也不保证生效）。
    /// </summary>
    [JsonPropertyName("coupon_value")]
    public long? CouponValue { get; set; }

    /// <summary>
    /// 券门槛（<c>coupon_minimum</c>，选填 integer）：<b>官方标注「暂未开放」</b>，同 <see cref="CouponValue"/>。
    /// </summary>
    [JsonPropertyName("coupon_minimum")]
    public long? CouponMinimum { get; set; }
}

/// <summary>发放指定批次代金券应答（仅 <c>coupon_id</c> 一个字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponIssueResponse : WechatPayResponse
{
    /// <summary>代金券 id（<c>coupon_id</c>，必填）：后续可用它调「查询代金券详情」。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }
}
