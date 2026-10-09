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
