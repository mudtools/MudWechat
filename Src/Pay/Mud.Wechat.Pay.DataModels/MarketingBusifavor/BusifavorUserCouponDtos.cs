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

/// <summary>用户商家券列表响应·【结果集】·【样式信息】·【视频号相关信息】视频号相关信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatadisplayPatternInfoFinderInfo
{
    /// <summary>【视频号ID】关联视频号将展示在优惠券详情的顶部右侧，作为跳转去视频号的入口，入参参数请配置视频号id，请前往视频号助手管理查看视频号ID（官方字段路径 <c>data.display_pattern_info.finder_info.finder_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>【视频号视频ID】券详情视频内容，支持配置关联视频号下的具体视频内容，入参参数请配置视频id，请前往视频号助手管理后台复制具体视频ID（官方字段路径 <c>data.display_pattern_info.finder_info.finder_video_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_id")]
    public string? FinderVideoId { get; set; }

    /// <summary>【视频号封面图】截取的视频号图片将在券到期提醒消息、券详情中展示。 1.图片尺寸：716像素（宽）\*402像素（高）；图片大小不超过2M，支持JPG/PNG格式。 2.仅支持通过《图片上传API》接口获取的图片URL地址（官方字段路径 <c>data.display_pattern_info.finder_info.finder_video_cover_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("finder_video_cover_image_url")]
    public string? FinderVideoCoverImageUrl { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【样式信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatadisplayPatternInfo
{
    /// <summary>【使用须知】用于说明详细的活动规则，会展示在代金券详情页（官方字段路径 <c>data.display_pattern_info.description</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【商户logo】若券归属商户号有认证品牌，则系统将自动拉取对应品牌logo；若券归属商户号不在认证品牌下，需自定义上传logo，未上传时将展示兜底灰色logo样式，影响券详情页用户体验，请及时上传。 商户logo的URL地址，仅支持通过《图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素\*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M。 注：该字段暂不支持修改（官方字段路径 <c>data.display_pattern_info.merchant_logo_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }

    /// <summary>【商户名称】不支持商户自定义。若券归属商户号有认证品牌，系统将自动拉取认证品牌号下的品牌名称；若券归属商户号不在认证品牌下，则拉取本商户号的商户简称。展示上限12个字符。 注：该字段暂不支持修改（官方字段路径 <c>data.display_pattern_info.merchant_name</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>【背景颜色】券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称。 !（官方字段路径 <c>data.display_pattern_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【券详情图片】券详情图片，1074像素（宽）\*603像素（高），图片大小不超过2M，支持JPG/PNG格式。仅支持通过《图片上传API》接口获取的图片URL地址。\*（官方字段路径 <c>data.display_pattern_info.coupon_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_image_url")]
    public string? CouponImageUrl { get; set; }

    /// <summary>【视频号相关信息】视频号相关信息（官方字段路径 <c>data.display_pattern_info.finder_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("finder_info")]
    public BusifavorUserCouponListResponseDataItemDatadisplayPatternInfoFinderInfo? FinderInfo { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18·【当天可用时间段】可以填写多个时间段，最多不超过2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTimeAvailableWeekAvailableDayTimeItem
{
    /// <summary>【当天可用开始时间】当天可用开始时间 单位秒 1代表当天0点0分1秒（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_week.available_day_time.begin_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】当天可用结束时间 单位秒 86399代表当天23点59分59秒（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_week.available_day_time.end_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTimeAvailableWeek
{
    /// <summary>【可用星期数】0代表周日，1代表周一，以此类推 当填写available_day_time时，week_day必填（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_week.week_day</c>，选填，类型 <c>array[integer]</c>）。</summary>
    [JsonPropertyName("week_day")]
    public List<long>? WeekDay { get; set; }

    /// <summary>【当天可用时间段】可以填写多个时间段，最多不超过2个（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_week.available_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_day_time")]
    public List<BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTimeAvailableWeekAvailableDayTimeItem>? AvailableDayTime { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】·【券可核销时间】日期区间内可以使用优惠·【无规律的有效时间段】无规律的有效时间，多个无规律时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTimeIrregularyAvaliableTimeItem
{
    /// <summary>【开始时间】开始时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.irregulary_avaliable_time.begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.irregulary_avaliable_time.end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】·【券可核销时间】日期区间内可以使用优惠。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTime
{
    /// <summary>【开始时间】批次开始时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 注意：商家券有效期最长为1年（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【结束时间】批次结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 注意：商家券有效期最长为1年（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】日期区间内，券生效后x天内有效。例如生效当天内有效填1，生效后2天内有效填2，以此类推……注意，用户在有效期开始前领取商家券，则从有效期第1天开始计算天数，用户在有效期内领取商家券，则从领取当天开始计算天数，无论用户何时领取商家券，商家券在活动有效期结束后均不可用。可配合wait_days_after_receive一同填写，也可单独填写。单独填写时，有效期内领券后立即生效，生效后x天内有效（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18点（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_week</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("available_week")]
    public BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTimeAvailableWeek? AvailableWeek { get; set; }

    /// <summary>【无规律的有效时间段】无规律的有效时间，多个无规律时间段（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.irregulary_avaliable_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregulary_avaliable_time")]
    public List<BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTimeIrregularyAvaliableTimeItem>? IrregularyAvaliableTime { get; set; }

    /// <summary>【领取后N天开始生效】日期区间内，用户领券后需等待x天开始生效。例如领券后当天开始生效则无需填写，领券后第2天开始生效填1，以此类推……用户在有效期开始前领取商家券，则从有效期第1天开始计算天数，用户在有效期内领取商家券，则从领取当天开始计算天数。无论用户何时领取商家券，商家券在活动有效期结束后均不可用。需配合available_day_after_receive一同填写，不可单独填写。注：最大不能超过30天（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【领取后N秒内有效】 领取后N秒内有效。用户领券后，券立即生效，自领取时刻起N秒内可用，不以自然日对齐。取值范围1~86400秒（最长24小时），当前仅支持3600的整数倍（即整小时，如3600、7200...86400）。与available_day_after_receive、wait_days_after_receive、available_week、irregulary_avaliable_time互斥，不可同时填写（官方字段路径 <c>data.coupon_use_rule.coupon_available_time.available_second_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_second_after_receive")]
    public long? AvailableSecondAfterReceive { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】·【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRuleFixedNormalCoupon
{
    /// <summary>【优惠金额】优惠金额 单位分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>data.coupon_use_rule.fixed_normal_coupon.discount_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>data.coupon_use_rule.fixed_normal_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】·【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRuleDiscountCoupon
{
    /// <summary>【折扣比例】折扣百分比，88为88折（官方字段路径 <c>data.coupon_use_rule.discount_coupon.discount_percent</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_percent")]
    public long? DiscountPercent { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>data.coupon_use_rule.discount_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】·【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRuleExchangeCoupon
{
    /// <summary>【单品换购价】单品换购价，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>data.coupon_use_rule.exchange_coupon.exchange_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>data.coupon_use_rule.exchange_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【券核销规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacouponUseRule
{
    /// <summary>【券可核销时间】日期区间内可以使用优惠（官方字段路径 <c>data.coupon_use_rule.coupon_available_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_time")]
    public BusifavorUserCouponListResponseDataItemDatacouponUseRuleCouponAvailableTime? CouponAvailableTime { get; set; }

    /// <summary>【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为NORMAL时必填（官方字段路径 <c>data.coupon_use_rule.fixed_normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fixed_normal_coupon")]
    public BusifavorUserCouponListResponseDataItemDatacouponUseRuleFixedNormalCoupon? FixedNormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为DISCOUNT时必填（官方字段路径 <c>data.coupon_use_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public BusifavorUserCouponListResponseDataItemDatacouponUseRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为EXCHANGE时必填（官方字段路径 <c>data.coupon_use_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public BusifavorUserCouponListResponseDataItemDatacouponUseRuleExchangeCoupon? ExchangeCoupon { get; set; }

    /// <summary>【核销方式】核销方式。可选取值：`OFF_LINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情。 / `MINI_PROGRAMS` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和path）。 / `SELF_CONSUME` 用户自助核销，点击券“立即使用”跳转至用户自助操作核销界面（当前暂不支持用户自助核销）。 / `PAYMENT_CODE` 微信支付付款码核销，点击券“立即使用”跳转至微信支付钱包付款码。（官方字段路径 <c>data.coupon_use_rule.use_method</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_method")]
    public string? UseMethod { get; set; }

    /// <summary>【小程序AppID】核销方式为线上小程序核销才有效（官方字段路径 <c>data.coupon_use_rule.mini_programs_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【小程序path】核销方式为线上小程序核销才有效（官方字段路径 <c>data.coupon_use_rule.mini_programs_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【自定义入口】·【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacustomEntranceMiniProgramsInfo
{
    /// <summary>【商家小程序AppID】商家小程序AppID要与归属商户号有M-A or M-m-suba关系。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方字段路径 <c>data.custom_entrance.mini_programs_info.mini_programs_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】商家小程序path（官方字段路径 <c>data.custom_entrance.mini_programs_info.mini_programs_path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

    /// <summary>【入口文案】入口文案，字数上限为5个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>data.custom_entrance.mini_programs_info.entrance_words</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_words")]
    public string? EntranceWords { get; set; }

    /// <summary>【引导文案】小程序入口引导文案，用户自定义字段。字数上限为6个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>data.custom_entrance.mini_programs_info.guiding_words</c>，选填，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guiding_words")]
    public string? GuidingWords { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】·【自定义入口】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItemDatacustomEntrance
{
    /// <summary>【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小程序，APPID、path、入口文案为必填，引导文案非必填。 AppID要与归属商户号有M-A or M-m-suba关系。 注：请查看绑定关系说明文档（官方字段路径 <c>data.custom_entrance.mini_programs_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mini_programs_info")]
    public BusifavorUserCouponListResponseDataItemDatacustomEntranceMiniProgramsInfo? MiniProgramsInfo { get; set; }

    /// <summary>【商户公众号AppID】可配置商户公众号，从券详情可跳转至公众号，用户自定义字段。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方字段路径 <c>data.custom_entrance.appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【更多优惠入口；营销馆创建地址：https://pay.weixin.qq.com/index.php/xphp/cfav_market/hall#/pages/list/list填写微信支付营销馆的馆ID，用户自定义字段。营销馆需在商户平台创建（官方字段路径 <c>data.custom_entrance.hall_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hall_id")]
    public string? HallId { get; set; }

    /// <summary>【可用门店ID】填写代金券可用门店ID（官方字段路径 <c>data.custom_entrance.store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【code展示模式】code展示模式。可选取值：`NOT_SHOW` 不展示code / `BARCODE` 一维码 / `QRCODE` 二维码（官方字段路径 <c>data.custom_entrance.code_display_mode</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

}

/// <summary>用户商家券列表响应·【结果集】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponseDataItem
{
    /// <summary>【批次归属商户号】代金券的所属商户号（官方字段路径 <c>data.belong_merchant</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("belong_merchant")]
    public string? BelongMerchant { get; set; }

    /// <summary>【商家券批次名称】批次名称，字数上限为21个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>data.stock_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>【批次备注】仅配置商户可见，用于自定义信息。字数上限为20个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>data.comment</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>【适用商品范围】适用商品范围，字数上限为15个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>data.goods_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【批次类型】批次类型。可选取值：`NORMAL` 固定面额满减券批次 / `DISCOUNT` 折扣券批次 / `EXCHANGE` 换购券批次（官方字段路径 <c>data.stock_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public string? StockType { get; set; }

    /// <summary>【是否允许转赠】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>data.transferable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("transferable")]
    public bool? Transferable { get; set; }

    /// <summary>【是否允许分享领券链接】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>data.shareable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("shareable")]
    public bool? Shareable { get; set; }

    /// <summary>【券状态】商家券状态。可选取值：`SENDED` 可用 / `USED` 已核销 / `EXPIRED` 已过期 / `DELETED` 已删除 / `DEACTIVATED` 已失效（官方字段路径 <c>data.coupon_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【样式信息】（官方字段路径 <c>data.display_pattern_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_pattern_info")]
    public BusifavorUserCouponListResponseDataItemDatadisplayPatternInfo? DisplayPatternInfo { get; set; }

    /// <summary>【券核销规则】（官方字段路径 <c>data.coupon_use_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_use_rule")]
    public BusifavorUserCouponListResponseDataItemDatacouponUseRule? CouponUseRule { get; set; }

    /// <summary>【自定义入口】（官方字段路径 <c>data.custom_entrance</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("custom_entrance")]
    public BusifavorUserCouponListResponseDataItemDatacustomEntrance? CustomEntrance { get; set; }

    /// <summary>【券code】券的唯一标识（官方字段路径 <c>data.coupon_code</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【批次号】批次号（官方字段路径 <c>data.stock_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【券可使用开始时间】用户领取到的这张券实际可使用的开始时间：如批次设置的领取后可用，则开始时间即为券的领取时间； 如批次设置的领取后第X天可用，则为领取时间后第X天00:00:00（官方字段路径 <c>data.available_start_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_start_time")]
    public string? AvailableStartTime { get; set; }

    /// <summary>【券过期时间】用户领取到这张券的过期时间（官方字段路径 <c>data.expire_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    /// <summary>【券领取时间】用户领取到这张券的时间（官方字段路径 <c>data.receive_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】发券时传入的唯一凭证（官方字段路径 <c>data.send_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【核销请求单号】核销时传入的唯一凭证（如券已被核销，将返回此字段）（官方字段路径 <c>data.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】券被核销的时间（如券已被核销，将返回此字段）（官方字段路径 <c>data.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【关联的商户订单号】若商家券操作过关联商户订单信息，则该字段返回商家券已关联的商户订单号（官方字段路径 <c>data.associate_out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("associate_out_trade_no")]
    public string? AssociateOutTradeNo { get; set; }

    /// <summary>【回退请求单号】回退时传入的唯一凭证（如券发生了退回，将返回此字段）（官方字段路径 <c>data.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【券回退时间】券被回退的时间（如券发生了退回，将返回此字段）（官方字段路径 <c>data.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【失效请求单号】失效时传入的唯一凭证（如果一张券已失效，将返回此字段）（官方字段路径 <c>data.deactivate_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【券失效时间】券被失效的时间（如果一张券已失效，将返回此字段）（官方字段路径 <c>data.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】失效一张券的原因（如果一张券已失效，可能返回此字段）（官方字段路径 <c>data.deactivate_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

}

/// <summary>用户商家券列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534963"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponListResponse : WechatPayResponse
{
    /// <summary>【结果集】（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<BusifavorUserCouponListResponseDataItem>? Data { get; set; }

    /// <summary>【总数量】总数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页大小】分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【分页页码】分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

}

/// <summary>用户商家券详情响应·【样式信息】·【视频号相关信息】视频号相关信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseDisplayPatternInfoDisplayPatternInfofinderInfo
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

/// <summary>用户商家券详情响应·【样式信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseDisplayPatternInfo
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

    /// <summary>【券详情图片】券详情图片，1074像素（宽）\*603像素（高），图片大小不超过2M，支持JPG/PNG格式。仅支持通过《图片上传API》接口获取的图片URL地址（官方字段路径 <c>display_pattern_info.coupon_image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_image_url")]
    public string? CouponImageUrl { get; set; }

    /// <summary>【视频号相关信息】视频号相关信息（官方字段路径 <c>display_pattern_info.finder_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("finder_info")]
    public BusifavorUserCouponQueryResponseDisplayPatternInfoDisplayPatternInfofinderInfo? FinderInfo { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18·【当天可用时间段】可以填写多个时间段，最多不超过2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeekAvailableDayTimeItem
{
    /// <summary>【当天可用开始时间】当天可用开始时间 单位秒 1代表当天0点0分1秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time.begin_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】当天可用结束时间 单位秒 86399代表当天23点59分59秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time.end_time</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】·【券可核销时间】日期区间内可以使用优惠·【固定周期有效时间段】可以设置多个星期下的多个可用时间段，比如每周二10点到18。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeek
{
    /// <summary>【可用星期数】0代表周日，1代表周一，以此类推 当填写available_day_time时，week_day必填（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.week_day</c>，选填，类型 <c>array[integer]</c>）。</summary>
    [JsonPropertyName("week_day")]
    public List<long>? WeekDay { get; set; }

    /// <summary>【当天可用时间段】可以填写多个时间段，最多不超过2个（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_week.available_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_day_time")]
    public List<BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeekAvailableDayTimeItem>? AvailableDayTime { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】·【券可核销时间】日期区间内可以使用优惠·【无规律的有效时间段】无规律的有效时间，多个无规律时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeIrregularyAvaliableTimeItem
{
    /// <summary>【开始时间】开始时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time.begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time.end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】·【券可核销时间】日期区间内可以使用优惠。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTime
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
    public BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeAvailableWeek? AvailableWeek { get; set; }

    /// <summary>【无规律的有效时间段】无规律的有效时间，多个无规律时间段（官方字段路径 <c>coupon_use_rule.coupon_available_time.irregulary_avaliable_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregulary_avaliable_time")]
    public List<BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTimeIrregularyAvaliableTimeItem>? IrregularyAvaliableTime { get; set; }

    /// <summary>【领取后N天开始生效】日期区间内，用户领券后需等待x天开始生效。例如领券后当天开始生效则无需填写，领券后第2天开始生效填1，以此类推……用户在有效期开始前领取商家券，则从有效期第1天开始计算天数，用户在有效期内领取商家券，则从领取当天开始计算天数。无论用户何时领取商家券，商家券在活动有效期结束后均不可用。需配合available_day_after_receive一同填写，不可单独填写。注：最大不能超过30天（官方字段路径 <c>coupon_use_rule.coupon_available_time.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【领取后N秒内有效】 领取后N秒内有效。用户领券后，券立即生效，自领取时刻起N秒内可用，不以自然日对齐。取值范围1~86400秒（最长24小时），当前仅支持3600的整数倍（即整小时，如3600、7200...86400）。与available_day_after_receive、wait_days_after_receive、available_week、irregulary_avaliable_time互斥，不可同时填写（官方字段路径 <c>coupon_use_rule.coupon_available_time.available_second_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_second_after_receive")]
    public long? AvailableSecondAfterReceive { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】·【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulefixedNormalCoupon
{
    /// <summary>【优惠金额】优惠金额 单位分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon.discount_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】·【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulediscountCoupon
{
    /// <summary>【折扣比例】折扣百分比，88为88折（官方字段路径 <c>coupon_use_rule.discount_coupon.discount_percent</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_percent")]
    public long? DiscountPercent { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.discount_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】·【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRuleexchangeCoupon
{
    /// <summary>【单品换购价】单品换购价，单位：分。 特殊规则：取值范围 0 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.exchange_coupon.exchange_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

    /// <summary>【消费门槛】消费门槛，单位：分。 特殊规则：取值范围 1 ≤ value ≤ 10000000（官方字段路径 <c>coupon_use_rule.exchange_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>用户商家券详情响应·【券核销规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCouponUseRule
{
    /// <summary>【券可核销时间】日期区间内可以使用优惠（官方字段路径 <c>coupon_use_rule.coupon_available_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_time")]
    public BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulecouponAvailableTime? CouponAvailableTime { get; set; }

    /// <summary>【固定面额满减券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为NORMAL时必填（官方字段路径 <c>coupon_use_rule.fixed_normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fixed_normal_coupon")]
    public BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulefixedNormalCoupon? FixedNormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为DISCOUNT时必填（官方字段路径 <c>coupon_use_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRulediscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【换购券使用规则】固定面额满减，折扣券，换购券使用规则三选一，stock_type为EXCHANGE时必填（官方字段路径 <c>coupon_use_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public BusifavorUserCouponQueryResponseCouponUseRuleCouponUseRuleexchangeCoupon? ExchangeCoupon { get; set; }

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

/// <summary>用户商家券详情响应·【自定义入口】·【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCustomEntranceCustomEntranceminiProgramsInfo
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

/// <summary>用户商家券详情响应·【自定义入口】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponseCustomEntrance
{
    /// <summary>【小程序入口】需要小程序APPID、path、入口文案、引导文案。如果需要跳转小程序，APPID、path、入口文案为必填，引导文案非必填。 AppID要与归属商户号有M-A or M-m-suba关系。 注：请查看绑定关系说明文档（官方字段路径 <c>custom_entrance.mini_programs_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("mini_programs_info")]
    public BusifavorUserCouponQueryResponseCustomEntranceCustomEntranceminiProgramsInfo? MiniProgramsInfo { get; set; }

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

/// <summary>用户商家券详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535011"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorUserCouponQueryResponse : WechatPayResponse
{
    /// <summary>【批次归属商户号】代金券的所属商户号（官方字段路径 <c>belong_merchant</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("belong_merchant")]
    public string? BelongMerchant { get; set; }

    /// <summary>【商家券批次名称】批次名称，字数上限为21个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>stock_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>【批次备注】仅配置商户可见，用于自定义信息。字数上限为20个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>comment</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>【适用商品范围】适用商品范围，字数上限为15个，一个中文汉字/英文字母/数字均占用一个字数（官方字段路径 <c>goods_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【批次类型】批次类型。可选取值：`NORMAL` 固定面额满减券批次 / `DISCOUNT` 折扣券批次 / `EXCHANGE` 换购券批次（官方字段路径 <c>stock_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public string? StockType { get; set; }

    /// <summary>【是否允许转赠】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>transferable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("transferable")]
    public bool? Transferable { get; set; }

    /// <summary>【是否允许分享领券链接】不填默认否，枚举值： true：是 false：否 该字段暂未开放（官方字段路径 <c>shareable</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("shareable")]
    public bool? Shareable { get; set; }

    /// <summary>【券状态】商家券状态。可选取值：`SENDED` 可用 / `USED` 已核销 / `EXPIRED` 已过期 / `DELETED` 已删除 / `DEACTIVATED` 已失效（官方字段路径 <c>coupon_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【样式信息】（官方字段路径 <c>display_pattern_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_pattern_info")]
    public BusifavorUserCouponQueryResponseDisplayPatternInfo? DisplayPatternInfo { get; set; }

    /// <summary>【券核销规则】（官方字段路径 <c>coupon_use_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_use_rule")]
    public BusifavorUserCouponQueryResponseCouponUseRule? CouponUseRule { get; set; }

    /// <summary>【自定义入口】（官方字段路径 <c>custom_entrance</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("custom_entrance")]
    public BusifavorUserCouponQueryResponseCustomEntrance? CustomEntrance { get; set; }

    /// <summary>【券code】券的唯一标识（官方字段路径 <c>coupon_code</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【批次号】批次号（官方字段路径 <c>stock_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【券可使用开始时间】用户领取到的这张券实际可使用的开始时间：如批次设置的领取后可用，则开始时间即为券的领取时间； 如批次设置的领取后第X天可用，则为领取时间后第X天00:00:00（官方字段路径 <c>available_start_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_start_time")]
    public string? AvailableStartTime { get; set; }

    /// <summary>【券过期时间】用户领取到这张券的过期时间（官方字段路径 <c>expire_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    /// <summary>【券领取时间】用户领取到这张券的时间（官方字段路径 <c>receive_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】发券时传入的唯一凭证（官方字段路径 <c>send_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【核销请求单号】核销时传入的唯一凭证（如券已被核销，将返回此字段）（官方字段路径 <c>use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】券被核销的时间（如券已被核销，将返回此字段）（官方字段路径 <c>use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【关联的商户订单号】若商家券操作过关联商户订单信息，则该字段返回商家券已关联的商户订单号（官方字段路径 <c>associate_out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("associate_out_trade_no")]
    public string? AssociateOutTradeNo { get; set; }

    /// <summary>【回退请求单号】回退时传入的唯一凭证（如券发生了退回，将返回此字段）（官方字段路径 <c>return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【券回退时间】券被回退的时间（如券发生了退回，将返回此字段）（官方字段路径 <c>return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【失效请求单号】失效时传入的唯一凭证（如果一张券已失效，将返回此字段）（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【券失效时间】券被失效的时间（如果一张券已失效，将返回此字段）（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】失效一张券的原因（如果一张券已失效，可能返回此字段）（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

}
