// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingBusifavor;

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18·【当天可用时间段】可以填写多个时间段，最多不超过2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeekAvailableDayTimeItem
{
    /// <summary>【当天可用开始时间】当天可用开始时间 单位秒 1代表当天0点0分1秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time.begin_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】当天可用结束时间 单位秒 86399代表当天23点59分59秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time.end_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeek
{
    /// <summary>【可用星期数】0代表周日，1代表周一，以此类推 当填写available_day_time时，week_day必填（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.week_day</c>，选填，类型 <c>array[integer]</c>）。</summary>
    [JsonPropertyName("week_day")]
    public List<long>? WeekDay { get; set; }

    /// <summary>【当天可用时间段】可以填写多个时间段，最多不超过2个（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_day_time")]
    public List<BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeekAvailableDayTimeItem>? AvailableDayTime { get; set; }

}

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则·【券可核销时间】日期区间内可以使用优惠·【无规律的有效时间段】无规律的有效时间，多个无规律时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTimeIrregularyAvaliableTimeItem
{
    /// <summary>【开始时间】开始时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time.begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time.end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则·【券可核销时间】日期区间内可以使用优惠。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTime
{
    /// <summary>【开始时间】批次开始时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 注意：商家券有效期最长为1年（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【结束时间】批次结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 注意：商家券有效期最长为1年（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】日期区间内，券生效后x天内有效。例如生效当天内有效填1，生效后2天内有效填2，以此类推……注意，用户在有效期开始前领取商家券，则从有效期第1天开始计算天数，用户在有效期内领取商家券，则从领取当天开始计算天数，无论用户何时领取商家券，商家券在活动有效期结束后均不可用。可配合wait_days_after_receive一同填写，也可单独填写。单独填写时，有效期内领券后立即生效，生效后x天内有效（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18点（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("available_week")]
    public BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeek? AvailableWeek { get; set; }

    /// <summary>【无规律的有效时间段】无规律的有效时间，多个无规律时间段（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregulary_avaliable_time")]
    public List<BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTimeIrregularyAvaliableTimeItem>? IrregularyAvaliableTime { get; set; }

    /// <summary>【领取后N天开始生效】日期区间内，用户领券后需等待x天开始生效。例如领券后当天开始生效则无需填写，领券后第2天开始生效填1，以此类推……用户在有效期开始前领取商家券，则从有效期第1天开始计算天数，用户在有效期内领取商家券，则从领取当天开始计算天数。无论用户何时领取商家券，商家券在活动有效期结束后均不可用。需配合available_day_after_receive一同填写，不可单独填写。注：最大不能超过30天（官方字段路径 <c>coupon_use_rule.coupon_available_time.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【领取后N秒内有效】 领取后N秒内有效。用户领券后，券立即生效，自领取时刻起N秒内可用，不以自然日对齐。取值范围1~86400秒（最长24小时），当前仅支持3600的整数倍（即整小时，如3600、7200...86400）。与available_day_after_receive、wait_days_after_receive、available_week、irregulary_avaliable_time互斥，不可同时填写（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_second_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_second_after_receive")]
    public long? AvailableSecondAfterReceive { get; set; }

}

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则·【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRuleCouponUseRulefixedNormalCoupon
{
    /// <summary>【优惠金额】优惠金额 单位分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon.discount_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则·【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRuleCouponUseRulediscountCoupon
{
    /// <summary>【折扣比例】折扣百分比，88为88折（官方字段路径 <c>coupon_use_rule.discount_coupon.discount_percent</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_percent")]
    public long? DiscountPercent { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.discount_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则·【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRuleCouponUseRuleexchangeCoupon
{
    /// <summary>【单品换购价】单品换购价，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.exchange_coupon.exchange_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.exchange_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>创建商家券批次请求·【核销规则】券核销相关规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCouponUseRule
{
    /// <summary>【券可核销时间】日期区间内可以使用优惠（官方字段路径 <c>coupon_use_rule.coupon_available_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_time")]
    public BusifavorStockCreateRequestCouponUseRuleCouponUseRulecouponAvailableTime? CouponAvailableTime { get; set; }

    /// <summary>【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为NORMAL时必填（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fixed_normal_coupon")]
    public BusifavorStockCreateRequestCouponUseRuleCouponUseRulefixedNormalCoupon? FixedNormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为DISCOUNT时必填（官方字段路径 <c>coupon_use_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public BusifavorStockCreateRequestCouponUseRuleCouponUseRulediscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为EXCHANGE时必填（官方字段路径 <c>coupon_use_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public BusifavorStockCreateRequestCouponUseRuleCouponUseRuleexchangeCoupon? ExchangeCoupon { get; set; }

    /// <summary>【核销方式】核销方式。可选取值：`OFF_LINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情。 / `MINI_PROGRAMS` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和path）。 / `SELF_CONSUME` 用户自助核销，点击券“立即使用”跳转至用户自助操作核销界面（当前暂不支持用户自助核销）。 / `PAYMENT_CODE` 微信支付付款码核销，点击券“立即使用”跳转至微信支付钱包付款码。（官方字段路径 <c>coupon_use_rule.use_method</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_method")]
    public string? UseMethod { get; set; }

    /// <summary>【小程序AppID】核销方式为线上小程序核销才有效（官方字段路径 <c>coupon_use_rule.mini_programs_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【小程序path】核销方式为线上小程序核销才有效（官方字段路径 <c>coupon_use_rule.mini_programs_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>创建商家券批次请求·【发放规则】券发放相关规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestStockSendRule
{
    /// <summary>【批次总预算】总预算金额，单位分 注：该字段暂未开放（官方字段路径 <c>stock_send_rule.max_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_amount")]
    public long? MaxAmount { get; set; }

    /// <summary>【批次最大发放个数】批次最大可发放个数限制 特殊规则：取值范围 1 ≤ value ≤ 1000000000（官方字段路径 <c>stock_send_rule.max_coupons</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons")]
    public long? MaxCoupons { get; set; }

    /// <summary>【用户最大可领个数】用户可领个数，每个用户最多100张券（官方字段路径 <c>stock_send_rule.max_coupons_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons_per_user")]
    public long? MaxCouponsPerUser { get; set; }

    /// <summary>【单天发放上限金额】单天发放上限金额 注：该字段暂未开放（官方字段路径 <c>stock_send_rule.max_amount_by_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_amount_by_day")]
    public long? MaxAmountByDay { get; set; }

    /// <summary>【单天发放上限个数】单天发放上限个数（stock_type为DISCOUNT或EXCHANGE时可传入此字段控制单天发放上限）。 特殊规则：取值范围 1 ≤ value ≤ 1000000000（官方字段路径 <c>stock_send_rule.max_coupons_by_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons_by_day")]
    public long? MaxCouponsByDay { get; set; }

    /// <summary>【是否开启自然人限领】不填默认否，枚举值： true：是 false：否 注：自然人防刷即同证件号下的所有账户合并计算的限领次数（限领次数指的是参数字段“用户最大领取个数”填写的值）（官方字段路径 <c>stock_send_rule.natural_person_limit</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("natural_person_limit")]
    public bool? NaturalPersonLimit { get; set; }

    /// <summary>【可疑账号拦截】true-是；false-否，不填默认否（官方字段路径 <c>stock_send_rule.prevent_api_abuse</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("prevent_api_abuse")]
    public bool? PreventApiAbuse { get; set; }

    /// <summary>【是否允许转赠，暂不开放】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>stock_send_rule.transferable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("transferable")]
    public bool? Transferable { get; set; }

    /// <summary>【是否允许分享链接，暂不开放】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>stock_send_rule.shareable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("shareable")]
    public bool? Shareable { get; set; }

}

/// <summary>创建商家券批次请求·【自定义入口】卡详情页面，可选择多种入口引导用户·【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCustomEntranceCustomEntranceminiProgramsInfo
{
    /// <summary>【商家小程序AppID】商家小程序AppID要与归属商户号有M-A or M-m-suba关系。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方字段路径 <c>custom_entrance.mini_programs_info.mini_programs_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】商家小程序path（官方字段路径 <c>custom_entrance.mini_programs_info.mini_programs_path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

    /// <summary>【入口文案】入口文案，字数上限为5个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>custom_entrance.mini_programs_info.entrance_words</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_words")]
    public string? EntranceWords { get; set; }

    /// <summary>【引导文案】小程序入口引导文案，用户自定义字段。字数上限为6个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>custom_entrance.mini_programs_info.guiding_words</c>，选填，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guiding_words")]
    public string? GuidingWords { get; set; }

}

/// <summary>创建商家券批次请求·【自定义入口】卡详情页面，可选择多种入口引导用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestCustomEntrance
{
    /// <summary>【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小程序，APPID、path、入口文案为必填，引导文案非必填。 AppID要与归属商户号有M-A or M-m-suba关系。 注：请查看绑定关系说明文档（官方字段路径 <c>custom_entrance.mini_programs_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mini_programs_info")]
    public BusifavorStockCreateRequestCustomEntranceCustomEntranceminiProgramsInfo? MiniProgramsInfo { get; set; }

    /// <summary>【商户公众号AppID】可配置商户公众号，从券详情可跳转至公众号，用户自定义字段。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方字段路径 <c>custom_entrance.appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【更多优惠入口；营销馆创建地址：https://pay.weixin.qq.com/index.php/xphp/cfav_market/hall#/pages/list/list填写微信支付营销馆的馆ID，用户自定义字段。营销馆需在商户平台创建（官方字段路径 <c>custom_entrance.hall_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hall_id")]
    public string? HallId { get; set; }

    /// <summary>【可用门店ID】填写代金券可用门店ID（官方字段路径 <c>custom_entrance.store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【code展示模式】code展示模式。可选取值：`NOT_SHOW` 不展示code / `BARCODE` 一维码 / `QRCODE` 二维码（官方字段路径 <c>custom_entrance.code_display_mode</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

}

/// <summary>创建商家券批次请求·【样式信息】创建批次时的样式信息·【视频号相关信息】视频号相关信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestDisplayPatternInfoDisplayPatternInfofinderInfo
{
    /// <summary>【视频号ID】关联视频号将展示在优惠券详情的顶部右侧，作为跳转去视频号的入口，入参参数请配置视频号id，请前往视频号助手管理查看视频号ID（官方字段路径 <c>display_pattern_info.finder_info.finder_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>【视频号视频ID】券详情视频内容，支持配置关联视频号下的具体视频内容，入参参数请配置视频id，请前往视频号助手管理后台复制具体视频ID（官方字段路径 <c>display_pattern_info.finder_info.finder_video_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_id")]
    public string? FinderVideoId { get; set; }

    /// <summary>【视频号封面图】截取的视频号图片将在券到期提醒消息、券详情中展示。 1.图片尺寸：716像素（宽）\*402像素（高）；图片大小不超过2M，支持JPG/PNG格式。 2.仅支持通过《图片上传API》接口获取的图片URL地址（官方字段路径 <c>display_pattern_info.finder_info.finder_video_cover_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_cover_image_url")]
    public string? FinderVideoCoverImageUrl { get; set; }

}

/// <summary>创建商家券批次请求·【样式信息】创建批次时的样式信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestDisplayPatternInfo
{
    /// <summary>【使用须知】用于说明详细的活动规则，会展示在代金券详情页（官方字段路径 <c>display_pattern_info.description</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【商户logo】若券归属商户号有认证品牌，则系统将自动拉取对应品牌logo；若券归属商户号不在认证品牌下，需自定义上传logo，未上传时将展示兜底灰色logo样式，影响券详情页用户体验，请及时上传。 商户logo的URL地址，仅支持通过《图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素\*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M。 注：该字段暂不支持修改（官方字段路径 <c>display_pattern_info.merchant_logo_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }

    /// <summary>【商户名称】不支持商户自定义。若券归属商户号有认证品牌，系统将自动拉取认证品牌号下的品牌名称；若券归属商户号不在认证品牌下，则拉取本商户号的商户简称。展示上限12个字符。 注：该字段暂不支持修改（官方字段路径 <c>display_pattern_info.merchant_name</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>【背景颜色】券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称。 !（官方字段路径 <c>display_pattern_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【券详情图片】券详情图片，1074像素（宽）\*603像素（高），图片大小不超过2M，支持JPG/PNG格式。仅支持通过《图片上传API》接口获取的图片URL地址。\*（官方字段路径 <c>display_pattern_info.coupon_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_image_url")]
    public string? CouponImageUrl { get; set; }

    /// <summary>【视频号相关信息】视频号相关信息（官方字段路径 <c>display_pattern_info.finder_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("finder_info")]
    public BusifavorStockCreateRequestDisplayPatternInfoDisplayPatternInfofinderInfo? FinderInfo { get; set; }

}

/// <summary>创建商家券批次请求·【事件通知配置】事件回调通知商户的配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequestNotifyConfig
{
    /// <summary>【事件通知AppID】用于回调通知时，计算返回操作用户的OpenID（诸如领券用户），支持小程序or公众号的AppID；如该字段不填写，则回调通知中涉及到用户身份信息的OpenID与UnionID都将为空（官方字段路径 <c>notify_config.notify_appid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>创建商家券批次请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateRequest
{
    /// <summary>【商家券批次名称】批次名称，字数上限为21个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>stock_name</c>，<b>必填</b>，类型 <c>string(21)</c>）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>【批次归属商户号】批次归属于哪个商户。 注： 普通直连模式，该参数为直连商户号； 服务商模式，该参数为子商户号； 间连模式，该参数为子商户号（官方字段路径 <c>belong_merchant</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("belong_merchant")]
    public string? BelongMerchant { get; set; }

    /// <summary>【批次备注】仅配置商户可见，用于自定义信息。字数上限为20个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>comment</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>【适用商品范围】用来描述批次在哪些商品可用，会显示在微信卡包中。字数上限为15个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>goods_name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【批次类型】批次类型。可选取值：`NORMAL` 固定面额满减券批次 / `DISCOUNT` 折扣券批次 / `EXCHANGE` 换购券批次（官方字段路径 <c>stock_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public string? StockType { get; set; }

    /// <summary>【核销规则】券核销相关规则（官方字段路径 <c>coupon_use_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_use_rule")]
    public BusifavorStockCreateRequestCouponUseRule? CouponUseRule { get; set; }

    /// <summary>【发放规则】券发放相关规则（官方字段路径 <c>stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public BusifavorStockCreateRequestStockSendRule? StockSendRule { get; set; }

    /// <summary>【商户请求单号】商户创建批次凭据号（格式：商户ID+日期+流水号），商户侧需保持唯一性（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【自定义入口】卡详情页面，可选择多种入口引导用户（官方字段路径 <c>custom_entrance</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("custom_entrance")]
    public BusifavorStockCreateRequestCustomEntrance? CustomEntrance { get; set; }

    /// <summary>【样式信息】创建批次时的样式信息（官方字段路径 <c>display_pattern_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_pattern_info")]
    public BusifavorStockCreateRequestDisplayPatternInfo? DisplayPatternInfo { get; set; }

    /// <summary>【券code模式】特殊规则： 1、券code模式为WECHATPAY_MODE时，是微信自动分配券code（微信侧自动生成coupon_code作为批次下券的唯一标识），商户不需要预存code；适用于多种场景 2、券code模式为MERCHANT_API时，无需调用上传预存code接口，调用发券接口时需指定券code；更多用在商家自有流量场景（例如：商家自有小程序、H5网页等） 3、券code模式为MERCHANT_UPLOAD，需要调用上传预存code接…。可选取值：`WECHATPAY_MODE` 系统分配code，商户无需额外操作 / `MERCHANT_API` 商户发放时接口指定券code / `MERCHANT_UPLOAD` 商户上传自定义code，发券时系统随机选取上传的券code（官方字段路径 <c>coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【事件通知配置】事件回调通知商户的配置（官方字段路径 <c>notify_config</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public BusifavorStockCreateRequestNotifyConfig? NotifyConfig { get; set; }

    /// <summary>【是否允许营销补差】该批次发放的券是否允许进行补差，默认为false 注：该字段暂未开放（官方字段路径 <c>subsidy</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("subsidy")]
    public bool? Subsidy { get; set; }

}

/// <summary>创建商家券批次响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534855"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockCreateResponse : WechatPayResponse
{
    /// <summary>【批次号】批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【创建时间】创建时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18·【当天可用时间段】可以填写多个时间段，最多不超过2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeekAvailableDayTimeItem
{
    /// <summary>【当天可用开始时间】当天可用开始时间 单位秒 1代表当天0点0分1秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time.begin_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】当天可用结束时间 单位秒 86399代表当天23点59分59秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time.end_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeek
{
    /// <summary>【可用星期数】0代表周日，1代表周一，以此类推 当填写available_day_time时，week_day必填（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.week_day</c>，选填，类型 <c>array[integer]</c>）。</summary>
    [JsonPropertyName("week_day")]
    public List<long>? WeekDay { get; set; }

    /// <summary>【当天可用时间段】可以填写多个时间段，最多不超过2个（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_day_time")]
    public List<BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeekAvailableDayTimeItem>? AvailableDayTime { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则·【券可核销时间】日期区间内可以使用优惠·【无规律的有效时间段】无规律的有效时间，多个无规律时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeIrregularyAvaliableTimeItem
{
    /// <summary>【开始时间】开始时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time.begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time.end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则·【券可核销时间】日期区间内可以使用优惠。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTime
{
    /// <summary>【开始时间】批次开始时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 注意：商家券有效期最长为1年（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【结束时间】批次结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 注意：商家券有效期最长为1年（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】日期区间内，券生效后x天内有效。例如生效当天内有效填1，生效后2天内有效填2，以此类推……注意，用户在有效期开始前领取商家券，则从有效期第1天开始计算天数，用户在有效期内领取商家券，则从领取当天开始计算天数，无论用户何时领取商家券，商家券在活动有效期结束后均不可用。可配合wait_days_after_receive一同填写，也可单独填写。单独填写时，有效期内领券后立即生效，生效后x天内有效（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18点（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("available_week")]
    public BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeek? AvailableWeek { get; set; }

    /// <summary>【无规律的有效时间段】无规律的有效时间，多个无规律时间段（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregulary_avaliable_time")]
    public List<BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeIrregularyAvaliableTimeItem>? IrregularyAvaliableTime { get; set; }

    /// <summary>【领取后N天开始生效】日期区间内，用户领券后需等待x天开始生效。例如领券后当天开始生效则无需填写，领券后第2天开始生效填1，以此类推……用户在有效期开始前领取商家券，则从有效期第1天开始计算天数，用户在有效期内领取商家券，则从领取当天开始计算天数。无论用户何时领取商家券，商家券在活动有效期结束后均不可用。需配合available_day_after_receive一同填写，不可单独填写。注：最大不能超过30天（官方字段路径 <c>coupon_use_rule.coupon_available_time.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【领取后N秒内有效】 领取后N秒内有效。用户领券后，券立即生效，自领取时刻起N秒内可用，不以自然日对齐。取值范围1~86400秒（最长24小时），当前仅支持3600的整数倍（即整小时，如3600、7200...86400）。与available_day_after_receive、wait_days_after_receive、available_week、irregulary_avaliable_time互斥，不可同时填写（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_second_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_second_after_receive")]
    public long? AvailableSecondAfterReceive { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则·【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRuleCouponUseRulefixedNormalCoupon
{
    /// <summary>【优惠金额】优惠金额 单位分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon.discount_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则·【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRuleCouponUseRulediscountCoupon
{
    /// <summary>【折扣比例】折扣百分比，88为88折（官方字段路径 <c>coupon_use_rule.discount_coupon.discount_percent</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_percent")]
    public long? DiscountPercent { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.discount_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则·【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRuleCouponUseRuleexchangeCoupon
{
    /// <summary>【单品换购价】单品换购价，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.exchange_coupon.exchange_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.exchange_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>查询商家券批次详情响应·【核销规则】核销规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponUseRule
{
    /// <summary>【券可核销时间】日期区间内可以使用优惠（官方字段路径 <c>coupon_use_rule.coupon_available_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_time")]
    public BusifavorStockQueryResponseCouponUseRuleCouponUseRulecouponAvailableTime? CouponAvailableTime { get; set; }

    /// <summary>【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为NORMAL时必填（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fixed_normal_coupon")]
    public BusifavorStockQueryResponseCouponUseRuleCouponUseRulefixedNormalCoupon? FixedNormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为DISCOUNT时必填（官方字段路径 <c>coupon_use_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public BusifavorStockQueryResponseCouponUseRuleCouponUseRulediscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为EXCHANGE时必填（官方字段路径 <c>coupon_use_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public BusifavorStockQueryResponseCouponUseRuleCouponUseRuleexchangeCoupon? ExchangeCoupon { get; set; }

    /// <summary>【核销方式】核销方式。可选取值：`OFF_LINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情。 / `MINI_PROGRAMS` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和path）。 / `SELF_CONSUME` 用户自助核销，点击券“立即使用”跳转至用户自助操作核销界面（当前暂不支持用户自助核销）。 / `PAYMENT_CODE` 微信支付付款码核销，点击券“立即使用”跳转至微信支付钱包付款码。（官方字段路径 <c>coupon_use_rule.use_method</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_method")]
    public string? UseMethod { get; set; }

    /// <summary>【小程序AppID】核销方式为线上小程序核销才有效（官方字段路径 <c>coupon_use_rule.mini_programs_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【小程序path】核销方式为线上小程序核销才有效（官方字段路径 <c>coupon_use_rule.mini_programs_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>查询商家券批次详情响应·【发放规则】发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseStockSendRule
{
    /// <summary>【批次总预算】总预算金额，单位分 注：该字段暂未开放（官方字段路径 <c>stock_send_rule.max_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_amount")]
    public long? MaxAmount { get; set; }

    /// <summary>【批次最大发放个数】批次最大可发放个数限制 特殊规则：取值范围 1 ≤ value ≤ 1000000000（官方字段路径 <c>stock_send_rule.max_coupons</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons")]
    public long? MaxCoupons { get; set; }

    /// <summary>【用户最大可领个数】用户可领个数，每个用户最多100张券（官方字段路径 <c>stock_send_rule.max_coupons_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons_per_user")]
    public long? MaxCouponsPerUser { get; set; }

    /// <summary>【单天发放上限金额】单天发放上限金额 注：该字段暂未开放（官方字段路径 <c>stock_send_rule.max_amount_by_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_amount_by_day")]
    public long? MaxAmountByDay { get; set; }

    /// <summary>【单天发放上限个数】单天发放上限个数（stock_type为DISCOUNT或EXCHANGE时可传入此字段控制单天发放上限）。 特殊规则：取值范围 1 ≤ value ≤ 1000000000（官方字段路径 <c>stock_send_rule.max_coupons_by_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons_by_day")]
    public long? MaxCouponsByDay { get; set; }

    /// <summary>【是否开启自然人限领】不填默认否，枚举值： true：是 false：否 注：自然人防刷即同证件号下的所有账户合并计算的限领次数（限领次数指的是参数字段“用户最大领取个数”填写的值）（官方字段路径 <c>stock_send_rule.natural_person_limit</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("natural_person_limit")]
    public bool? NaturalPersonLimit { get; set; }

    /// <summary>【可疑账号拦截】true-是；false-否，不填默认否（官方字段路径 <c>stock_send_rule.prevent_api_abuse</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("prevent_api_abuse")]
    public bool? PreventApiAbuse { get; set; }

    /// <summary>【是否允许转赠，暂不开放】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>stock_send_rule.transferable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("transferable")]
    public bool? Transferable { get; set; }

    /// <summary>【是否允许分享链接，暂不开放】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>stock_send_rule.shareable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("shareable")]
    public bool? Shareable { get; set; }

}

/// <summary>查询商家券批次详情响应·【自定义入口】卡详情页面，可选择多种入口引导用户·【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCustomEntranceCustomEntranceminiProgramsInfo
{
    /// <summary>【商家小程序AppID】商家小程序AppID要与归属商户号有M-A or M-m-suba关系。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方字段路径 <c>custom_entrance.mini_programs_info.mini_programs_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】商家小程序path（官方字段路径 <c>custom_entrance.mini_programs_info.mini_programs_path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

    /// <summary>【入口文案】入口文案，字数上限为5个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>custom_entrance.mini_programs_info.entrance_words</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_words")]
    public string? EntranceWords { get; set; }

    /// <summary>【引导文案】小程序入口引导文案，用户自定义字段。字数上限为6个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>custom_entrance.mini_programs_info.guiding_words</c>，选填，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guiding_words")]
    public string? GuidingWords { get; set; }

}

/// <summary>查询商家券批次详情响应·【自定义入口】卡详情页面，可选择多种入口引导用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCustomEntrance
{
    /// <summary>【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小程序，APPID、path、入口文案为必填，引导文案非必填。 AppID要与归属商户号有M-A or M-m-suba关系。 注：请查看绑定关系说明文档（官方字段路径 <c>custom_entrance.mini_programs_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mini_programs_info")]
    public BusifavorStockQueryResponseCustomEntranceCustomEntranceminiProgramsInfo? MiniProgramsInfo { get; set; }

    /// <summary>【商户公众号AppID】可配置商户公众号，从券详情可跳转至公众号，用户自定义字段。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方字段路径 <c>custom_entrance.appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【更多优惠入口；营销馆创建地址：https://pay.weixin.qq.com/index.php/xphp/cfav_market/hall#/pages/list/lis填写微信支付营销馆的馆ID，用户自定义字段。营销馆需在商户平台创建（官方字段路径 <c>custom_entrance.hall_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hall_id")]
    public string? HallId { get; set; }

    /// <summary>【可用门店ID】填写代金券可用门店ID（官方字段路径 <c>custom_entrance.store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【code展示模式】code展示模式。可选取值：`NOT_SHOW` 不展示code / `BARCODE` 一维码 / `QRCODE` 二维码（官方字段路径 <c>custom_entrance.code_display_mode</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

}

/// <summary>查询商家券批次详情响应·【样式信息】创建批次时的样式信息·【视频号相关信息】视频号相关信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseDisplayPatternInfoDisplayPatternInfofinderInfo
{
    /// <summary>【视频号ID】关联视频号将展示在优惠券详情的顶部右侧，作为跳转去视频号的入口，入参参数请配置视频号id，请前往视频号助手管理查看视频号ID（官方字段路径 <c>display_pattern_info.finder_info.finder_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>【视频号视频ID】券详情视频内容，支持配置关联视频号下的具体视频内容，入参参数请配置视频id，请前往视频号助手管理后台复制具体视频ID（官方字段路径 <c>display_pattern_info.finder_info.finder_video_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_id")]
    public string? FinderVideoId { get; set; }

    /// <summary>【视频号封面图】截取的视频号图片将在券到期提醒消息、券详情中展示。 1.图片尺寸：716像素（宽）\*402像素（高）；图片大小不超过2M，支持JPG/PNG格式。 2.仅支持通过《图片上传API》接口获取的图片URL地址（官方字段路径 <c>display_pattern_info.finder_info.finder_video_cover_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_cover_image_url")]
    public string? FinderVideoCoverImageUrl { get; set; }

}

/// <summary>查询商家券批次详情响应·【样式信息】创建批次时的样式信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseDisplayPatternInfo
{
    /// <summary>【使用须知】用于说明详细的活动规则，会展示在代金券详情页（官方字段路径 <c>display_pattern_info.description</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【商户logo】若券归属商户号有认证品牌，则系统将自动拉取对应品牌logo；若券归属商户号不在认证品牌下，需自定义上传logo，未上传时将展示兜底灰色logo样式，影响券详情页用户体验，请及时上传。 商户logo的URL地址，仅支持通过《图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素\*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M。 注：该字段暂不支持修改（官方字段路径 <c>display_pattern_info.merchant_logo_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }

    /// <summary>【商户名称】不支持商户自定义。若券归属商户号有认证品牌，系统将自动拉取认证品牌号下的品牌名称；若券归属商户号不在认证品牌下，则拉取本商户号的商户简称。展示上限12个字符。 注：该字段暂不支持修改（官方字段路径 <c>display_pattern_info.merchant_name</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>【背景颜色】券的背景颜色，可设置10种颜色，色值请下方说明。颜色取值为颜色图中的颜色名称。 !（官方字段路径 <c>display_pattern_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【券详情图片】券详情图片，1074像素（宽）\*603像素（高），图片大小不超过2M，支持JPG/PNG格式。仅支持通过《图片上传API》接口获取的图片URL地址。\*（官方字段路径 <c>display_pattern_info.coupon_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_image_url")]
    public string? CouponImageUrl { get; set; }

    /// <summary>【视频号相关信息】视频号相关信息（官方字段路径 <c>display_pattern_info.finder_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("finder_info")]
    public BusifavorStockQueryResponseDisplayPatternInfoDisplayPatternInfofinderInfo? FinderInfo { get; set; }

}

/// <summary>查询商家券批次详情响应·【券code数量】当且仅当coupon_code_mode(券code模式)为M。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseCouponCodeCount
{
    /// <summary>【该批次总共已上传的code总数】该批次总共已上传的code总数（官方字段路径 <c>coupon_code_count.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【该批次当前可用的code数】该批次当前可用的code数（官方字段路径 <c>coupon_code_count.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>查询商家券批次详情响应·【事件通知配置】事件回调通知商户的配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseNotifyConfig
{
    /// <summary>【事件通知AppID】用于回调通知时，计算返回操作用户的OpenID（诸如领券用户），支持小程序or公众号的AppID；如该字段不填写，则回调通知中涉及到用户身份信息的OpenID与UnionID都将为空（官方字段路径 <c>notify_config.notify_appid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>查询商家券批次详情响应·【批次发放情况】批次发放情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponseSendCountInformation
{
    /// <summary>【已发放券张数】批次已发放的券数量，满减、折扣、换购类型会返回该字段（官方字段路径 <c>send_count_information.total_send_num</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_send_num")]
    public long? TotalSendNum { get; set; }

    /// <summary>【已发放券金额】批次已发放的预算金额，满减券类型会返回该字段（官方字段路径 <c>send_count_information.total_send_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_send_amount")]
    public long? TotalSendAmount { get; set; }

    /// <summary>【单天已发放券张数】批次当天已发放的券数量，设置了单天发放上限的满减、折扣、换购类型返回该字段（官方字段路径 <c>send_count_information.today_send_num</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_send_num")]
    public long? TodaySendNum { get; set; }

    /// <summary>【单天已发放券金额】批次当天已发放的预算金额，设置了当天发放上限的满减券类型返回该字段（官方字段路径 <c>send_count_information.today_send_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_send_amount")]
    public long? TodaySendAmount { get; set; }

}

/// <summary>查询商家券批次详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534928"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockQueryResponse : WechatPayResponse
{
    /// <summary>【商家券批次名称】批次名称，字数上限为21个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>stock_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>【批次归属商户号】批次是归属于哪个商户 注： 普通直连模式，该参数为直连商户号； 服务商模式，该参数为子商户号； 间连模式，该参数为子商户号（官方字段路径 <c>belong_merchant</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("belong_merchant")]
    public string? BelongMerchant { get; set; }

    /// <summary>【批次备注】仅配置商户可见，用于自定义信息。字数上限为20个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>comment</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>【适应商品范围】用来描述批次在哪些商品可用，会显示在微信卡包中。字数上限为15个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>goods_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【批次类型】批次类型。可选取值：`NORMAL` 固定面额满减券批次 / `DISCOUNT` 折扣券批次 / `EXCHANGE` 换购券批次（官方字段路径 <c>stock_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public string? StockType { get; set; }

    /// <summary>【核销规则】核销规则（官方字段路径 <c>coupon_use_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_use_rule")]
    public BusifavorStockQueryResponseCouponUseRule? CouponUseRule { get; set; }

    /// <summary>【发放规则】发放规则（官方字段路径 <c>stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public BusifavorStockQueryResponseStockSendRule? StockSendRule { get; set; }

    /// <summary>【自定义入口】卡详情页面，可选择多种入口引导用户（官方字段路径 <c>custom_entrance</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("custom_entrance")]
    public BusifavorStockQueryResponseCustomEntrance? CustomEntrance { get; set; }

    /// <summary>【样式信息】创建批次时的样式信息（官方字段路径 <c>display_pattern_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_pattern_info")]
    public BusifavorStockQueryResponseDisplayPatternInfo? DisplayPatternInfo { get; set; }

    /// <summary>【批次状态】批次状态。可选取值：`UNAUDIT` 审核中 / `RUNNING` 运行中 / `STOPED` 已停止 / `PAUSED` 暂停发放（官方字段路径 <c>stock_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_state")]
    public string? StockState { get; set; }

    /// <summary>【券code模式】券code模式。可选取值：`WECHATPAY_MODE` 系统分配code，商户无需额外操作 / `MERCHANT_API` 商户发放时接口指定券code / `MERCHANT_UPLOAD` 商户上传自定义code，发券时系统随机选取上传的券code（官方字段路径 <c>coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【批次号】批次唯一标识（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【券code数量】当且仅当coupon_code_mode(券code模式)为MERCHANT_UPLOAD(商户上传自定义code)模式时，返回该字段；返回内容为商户上传code的数量信息（官方字段路径 <c>coupon_code_count</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count")]
    public BusifavorStockQueryResponseCouponCodeCount? CouponCodeCount { get; set; }

    /// <summary>【事件通知配置】事件回调通知商户的配置（官方字段路径 <c>notify_config</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public BusifavorStockQueryResponseNotifyConfig? NotifyConfig { get; set; }

    /// <summary>【批次发放情况】批次发放情况（官方字段路径 <c>send_count_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("send_count_information")]
    public BusifavorStockQueryResponseSendCountInformation? SendCountInformation { get; set; }

    /// <summary>【是否允许营销补差】该批次发放的券是否允许进行补差 注：该字段暂未开放（官方字段路径 <c>subsidy</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("subsidy")]
    public bool? Subsidy { get; set; }

}

/// <summary>修改商家券批次请求·【自定义入口】卡详情页面，可选择多种入口引导用户·【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequestCustomEntranceCustomEntranceminiProgramsInfo
{
    /// <summary>【商家小程序AppID】需要小程序AppID与归属商户号有绑定关系（官方字段路径 <c>custom_entrance.mini_programs_info.mini_programs_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】商家小程序path（官方字段路径 <c>custom_entrance.mini_programs_info.mini_programs_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

    /// <summary>【入口文案】入口文案，字数上限为5个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>custom_entrance.mini_programs_info.entrance_words</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("entrance_words")]
    public string? EntranceWords { get; set; }

    /// <summary>【引导文案】小程序入口引导文案，字数上限为6个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>custom_entrance.mini_programs_info.guiding_words</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("guiding_words")]
    public string? GuidingWords { get; set; }

}

/// <summary>修改商家券批次请求·【自定义入口】卡详情页面，可选择多种入口引导用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequestCustomEntrance
{
    /// <summary>【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小程序，APPID、path、入口文案为必填，引导文案非必填。AppID要与归属商户号有绑定关系（官方字段路径 <c>custom_entrance.mini_programs_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mini_programs_info")]
    public BusifavorStockUpdateRequestCustomEntranceCustomEntranceminiProgramsInfo? MiniProgramsInfo { get; set; }

    /// <summary>【商户公众号AppID】可配置商户公众号，从券详情可跳转至公众号（官方字段路径 <c>custom_entrance.appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【更多优惠入口；营销馆创建地址：https://pay.weixin.qq.com/index.php/xphp/cfav_market/hall#/pages/list/lis填写微信支付营销馆的馆id，用户自定义字段。营销馆需在商户平台创建（官方字段路径 <c>custom_entrance.hall_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hall_id")]
    public string? HallId { get; set; }

    /// <summary>【code展示模式】code展示模式。可选取值：`NOT_SHOW` 不展示code / `BARCODE` 一维码 / `QRCODE` 二维码（官方字段路径 <c>custom_entrance.code_display_mode</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

}

/// <summary>修改商家券批次请求·【样式信息】·【视频号相关信息】视频号相关信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequestDisplayPatternInfoDisplayPatternInfofinderInfo
{
    /// <summary>【视频号ID】关联视频号将展示在优惠券详情的顶部右侧，作为跳转去视频号的入口，入参参数请配置视频号id，请前往视频号助手管理查看视频号ID（官方字段路径 <c>display_pattern_info.finder_info.finder_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>【视频号视频ID】券详情视频内容，支持配置关联视频号下的具体视频内容，入参参数请配置视频id，请前往视频号助手管理后台复制具体视频ID（官方字段路径 <c>display_pattern_info.finder_info.finder_video_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_id")]
    public string? FinderVideoId { get; set; }

    /// <summary>【视频号封面图】截取的视频号图片将在券到期提醒消息、券详情中展示。 1.图片尺寸：716像素（宽）\*402像素（高）；图片大小不超过2M，支持JPG/PNG格式。 2.仅支持通过《图片上传API》接口获取的图片URL地址（官方字段路径 <c>display_pattern_info.finder_info.finder_video_cover_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_cover_image_url")]
    public string? FinderVideoCoverImageUrl { get; set; }

}

/// <summary>修改商家券批次请求·【样式信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequestDisplayPatternInfo
{
    /// <summary>【使用须知】用于说明详细的活动规则，会展示在代金券详情页（官方字段路径 <c>display_pattern_info.description</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【商户logo】若券归属商户号有认证品牌，则系统将自动拉取对应品牌logo；若券归属商户号不在认证品牌下，需自定义上传logo，未上传时将展示兜底灰色logo样式，影响券详情页用户体验，请及时上传。 商户logo的URL地址，仅支持通过《图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素\*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M。 注：该字段暂不支持修改（官方字段路径 <c>display_pattern_info.merchant_logo_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }

    /// <summary>【商户名称】不支持商户自定义。若券归属商户号有认证品牌，系统将自动拉取认证品牌号下的品牌名称；若券归属商户号不在认证品牌下，则拉取本商户号的商户简称。展示上限12个字符。 注：该字段暂不支持修改（官方字段路径 <c>display_pattern_info.merchant_name</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>【背景颜色】券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称。 !（官方字段路径 <c>display_pattern_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【券详情图片】券详情图片，1074像素（宽）\*603像素（高），图片大小不超过2M，支持JPG/PNG格式。仅支持通过《图片上传API》接口获取的图片URL地址。\*（官方字段路径 <c>display_pattern_info.coupon_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_image_url")]
    public string? CouponImageUrl { get; set; }

    /// <summary>【视频号相关信息】视频号相关信息（官方字段路径 <c>display_pattern_info.finder_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("finder_info")]
    public BusifavorStockUpdateRequestDisplayPatternInfoDisplayPatternInfofinderInfo? FinderInfo { get; set; }

}

/// <summary>修改商家券批次请求·【核销规则】券核销相关规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequestCouponUseRule
{
    /// <summary>【核销方式】核销方式。可选取值：`OFF_LINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情。 / `MINI_PROGRAMS` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和path）。 / `SELF_CONSUME` 用户自助核销，点击券“立即使用”跳转至用户自助操作核销界面（当前暂不支持用户自助核销）。 / `PAYMENT_CODE` 微信支付付款码核销，点击券“立即使用”跳转至微信支付钱包付款码。（官方字段路径 <c>coupon_use_rule.use_method</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_method")]
    public string? UseMethod { get; set; }

    /// <summary>【小程序AppID】核销方式为线上小程序核销才有效（官方字段路径 <c>coupon_use_rule.mini_programs_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【小程序path】核销方式为线上小程序核销才有效（官方字段路径 <c>coupon_use_rule.mini_programs_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>修改商家券批次请求·【发放规则】券发放相关规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequestStockSendRule
{
    /// <summary>【是否开启自然人限领】true-是；false-否，不填默认否 注：该字段暂不支持修改（官方字段路径 <c>stock_send_rule.natural_person_limit</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("natural_person_limit")]
    public bool? NaturalPersonLimit { get; set; }

    /// <summary>【可疑账号拦截】true-是；false-否，不填默认否（官方字段路径 <c>stock_send_rule.prevent_api_abuse</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("prevent_api_abuse")]
    public bool? PreventApiAbuse { get; set; }

}

/// <summary>修改商家券批次请求·【事件通知配置】事件回调通知商户的配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequestNotifyConfig
{
    /// <summary>【事件通知AppID】用于回调通知时，计算返回操作用户的OpenID（诸如领券用户），支持小程序or公众号的AppID；如该字段不填写，则回调通知中涉及到用户身份信息的OpenID与UnionID都将为空（官方字段路径 <c>notify_config.notify_appid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>修改商家券批次请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535020"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockUpdateRequest
{
    /// <summary>【自定义入口】卡详情页面，可选择多种入口引导用户（官方字段路径 <c>custom_entrance</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("custom_entrance")]
    public BusifavorStockUpdateRequestCustomEntrance? CustomEntrance { get; set; }

    /// <summary>【批次名称】批次名称，字数上限为21个，一个中文汉字/英文字母/数字均占用一个字数。 注：该字段暂不支持修改（官方字段路径 <c>stock_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>【批次备注】仅配置商户可见，用于自定义信息。字数上限为20个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>comment</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>【适用商品范围】用来描述批次在哪些商品可用，会显示在微信卡包中。字数上限为15个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>goods_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【商户请求单号】商户修改批次凭据号（格式：商户ID+日期+流水号），商户侧需保持唯一性（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【样式信息】（官方字段路径 <c>display_pattern_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_pattern_info")]
    public BusifavorStockUpdateRequestDisplayPatternInfo? DisplayPatternInfo { get; set; }

    /// <summary>【核销规则】券核销相关规则（官方字段路径 <c>coupon_use_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_use_rule")]
    public BusifavorStockUpdateRequestCouponUseRule? CouponUseRule { get; set; }

    /// <summary>【发放规则】券发放相关规则（官方字段路径 <c>stock_send_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public BusifavorStockUpdateRequestStockSendRule? StockSendRule { get; set; }

    /// <summary>【事件通知配置】事件回调通知商户的配置（官方字段路径 <c>notify_config</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public BusifavorStockUpdateRequestNotifyConfig? NotifyConfig { get; set; }

    /// <summary>【是否允许营销补差】该批次发放的券是否允许进行补差。只允许从false改为true，不支持从true改为false 注：该字段暂未开放（官方字段路径 <c>subsidy</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("subsidy")]
    public bool? Subsidy { get; set; }

}

/// <summary>修改商家券批次预算请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465739"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockBudgetRequest
{
    /// <summary>【目标批次最大发放个数】批次最大发放个数，target_max_coupons与target_max_coupons_by_day只能修改一个 注：目标批次即修改后的批次（官方字段路径 <c>target_max_coupons</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("target_max_coupons")]
    public long? TargetMaxCoupons { get; set; }

    /// <summary>【当前批次最大发放个数】当前批次最大发放个数，当传入target_max_coupons大于0时，current_max_coupons必传 注：当前批次即未修改的批次（官方字段路径 <c>current_max_coupons</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("current_max_coupons")]
    public long? CurrentMaxCoupons { get; set; }

    /// <summary>【目标单天发放上限个数】单天发放上限个数，target_max_coupons与target_max_coupons_by_day只能修改一个 注：目标批次即修改后的批次（官方字段路径 <c>target_max_coupons_by_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("target_max_coupons_by_day")]
    public long? TargetMaxCouponsByDay { get; set; }

    /// <summary>【当前单天发放上限个数】当前单天发放上限个数 ，当传入target_max_coupons_by_day大于0时，current_max_coupons_by_day必填 注：当前批次即未修改的批次（官方字段路径 <c>current_max_coupons_by_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("current_max_coupons_by_day")]
    public long? CurrentMaxCouponsByDay { get; set; }

    /// <summary>【修改预算请求单据号】修改预算请求单据号（官方字段路径 <c>modify_budget_request_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("modify_budget_request_no")]
    public string? ModifyBudgetRequestNo { get; set; }

}

/// <summary>修改商家券批次预算响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465739"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorStockBudgetResponse : WechatPayResponse
{
    /// <summary>【批次当前最大发放个数】批次最大发放个数（官方字段路径 <c>max_coupons</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons")]
    public long? MaxCoupons { get; set; }

    /// <summary>【当前单天发放上限个数】当前单天发放上限个数（官方字段路径 <c>max_coupons_by_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons_by_day")]
    public long? MaxCouponsByDay { get; set; }

}

/// <summary>预上传商家券自定义券码请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487038"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponCodesUploadRequest
{
    /// <summary>【券code列表】商户上传的券code列表；code允许包含的字符有 0-9 a-z A-Z 空格（仅含空格，不含制表符、换行符、换页符等） 中划线- 下划线_ 反斜线\ 斜线/ 等号= 竖线&amp;#124（官方字段路径 <c>coupon_code_list</c>，<b>必填</b>，类型 <c>array[string(32)]</c>）。</summary>
    [JsonPropertyName("coupon_code_list")]
    public List<string>? CouponCodeList { get; set; }

    /// <summary>【请求业务单据号】商户上传code的凭据号，商户侧需保持唯一性（官方字段路径 <c>upload_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("upload_request_no")]
    public string? UploadRequestNo { get; set; }

}

/// <summary>预上传商家券自定义券码响应·【上传失败的code及原因】本次导入失败的code信息，请参照错误信息，修改后重。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487038"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponCodesUploadResponseFailCodesItem
{
    /// <summary>【上传失败的券code】商户通过API上传的券code（官方字段路径 <c>fail_codes.coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【上传失败错误码】对应券code上传失败的错误码（官方字段路径 <c>fail_codes.code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>【上传失败错误信息】上传失败的错误信息描述（官方字段路径 <c>fail_codes.message</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

}

/// <summary>预上传商家券自定义券码响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487038"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCouponCodesUploadResponse : WechatPayResponse
{
    /// <summary>【批次号】商家券批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【去重后上传code总数】本次上传操作，去重后实际上传的code数目（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【上传成功code个数】本次上传操作上传成功个数（官方字段路径 <c>success_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_count")]
    public long? SuccessCount { get; set; }

    /// <summary>【上传成功的code列表】本次新增上传成功的code信息。 特殊规则：单个券code长度为【1，32】，条目个数限制为【1，200】（官方字段路径 <c>success_codes</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("success_codes")]
    public List<string>? SuccessCodes { get; set; }

    /// <summary>【上传成功时间】上传操作完成时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>success_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【上传失败code个数】本次上传操作上传失败的code数（官方字段路径 <c>fail_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("fail_count")]
    public long? FailCount { get; set; }

    /// <summary>【上传失败的code及原因】本次导入失败的code信息，请参照错误信息，修改后重试（官方字段路径 <c>fail_codes</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("fail_codes")]
    public List<BusifavorCouponCodesUploadResponseFailCodesItem>? FailCodes { get; set; }

    /// <summary>【已存在的code列表】历史已存在的code列表，本次不会重复导入。 特殊规则：单个券code长度为【1，32】，条目个数限制为【1，200】（官方字段路径 <c>exist_codes</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("exist_codes")]
    public List<string>? ExistCodes { get; set; }

    /// <summary>【本次请求中重复的code列表】本次重复导入的code会被自动过滤，仅保留一个做导入，如满足要求则成功；如不满足要求，则失败；请参照报错提示修改重试。 特殊规则：单个券code长度为【1，32】，条目个数限制为【1，200】（官方字段路径 <c>duplicate_codes</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("duplicate_codes")]
    public List<string>? DuplicateCodes { get; set; }

}
