// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPartnerProductCoupon;

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【批次组发放规则】 批次组内的批次会继承此规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleStockBundlestockSendRule
{
    /// <summary>【批次组发放次数总上限】 批次组在生命周期内总发放次数，最多 100,000,000 次。批次组内的所有批次会继承此上限（官方字段路径 <c>stock_bundle.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【批次组每日发放次数上限】 批次组在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限。 注：为保证用户领取/使用多次优惠的体验，批次组内的首个批次会继承此上限，但其余批次会保持为不限制（官方字段路径 <c>stock_bundle.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取批次组次数上限】 每个用户最多可领取本批次组的次数，最多 100 次。批次组内的所有批次会继承此上限（官方字段路径 <c>stock_bundle.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleNormalCouponListItem
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.normal_coupon_list.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.normal_coupon_list.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleDiscountCouponListItem
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.discount_coupon_list.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.discount_coupon_list.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleExchangeCouponListItem
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.exchange_coupon_list.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.exchange_coupon_list.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.normal_coupon_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("normal_coupon_list")]
    public List<ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleNormalCouponListItem>? NormalCouponList { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.discount_coupon_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("discount_coupon_list")]
    public List<ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleDiscountCouponListItem>? DiscountCouponList { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule.exchange_coupon_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("exchange_coupon_list")]
    public List<ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRuleExchangeCouponListItem>? ExchangeCouponList { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【券使用规则展示信息】 券使用规则展示信息，批次组内每一个批次都会设置为此值·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleUsageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_bundle.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_bundle.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_bundle.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【券使用规则展示信息】 券使用规则展示信息，批次组内每一个批次都会设置为此值。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleUsageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock_bundle.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_bundle.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_bundle.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock_bundle.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_bundle.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock_bundle.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponStockBundleCreateRequestStockBundleUsageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock_bundle.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock_bundle.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口，·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleStockBundlecouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock_bundle.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock_bundle.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock_bundle.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock_bundle.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口，·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleStockBundlecouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock_bundle.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleStockBundlecouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock_bundle.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock_bundle.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock_bundle.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponStockBundleCreateRequestStockBundleStockBundlecouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock_bundle.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponStockBundleCreateRequestStockBundleStockBundlecouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情·【事件通知配置】 发生券相关事件时，微信支付会向品牌方发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundleStockBundlenotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock_bundle.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>创建商品券批次组合请求·【批次组】 为商品券创建的批次组详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequestStockBundle
{
    /// <summary>【备注】 仅配置品牌方可见，用于自定义信息，最多20个UTF-8字符（官方字段路径 <c>stock_bundle.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生，批次组内每一个批次都会设置为此值。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充（官方字段路径 <c>stock_bundle.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【批次组发放规则】 批次组内的批次会继承此规则（官方字段路径 <c>stock_bundle.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponStockBundleCreateRequestStockBundleStockBundlestockSendRule? StockSendRule { get; set; }

    /// <summary>【多次优惠批次组使用规则】 包含组合内部每一个批次的优惠规则，会根据内部的优惠规则列表依次创建批次放入批次组，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时必填。 多次优惠的批次组下各轮次，需要保证每一轮次 “优惠力度”大于等于前一轮次。但是首轮除外，即首轮的“优惠力度”可以小于/大于/等于其他任意轮次。 具体定义： 当满减券时：各轮次中“减免金额”数值，每轮次需大于等于前一轮次，首轮除外 当折扣券时：各轮次中“减免百…（官方字段路径 <c>stock_bundle.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponStockBundleCreateRequestStockBundleProgressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息，批次组内每一个批次都会设置为此值（官方字段路径 <c>stock_bundle.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponStockBundleCreateRequestStockBundleUsageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口，批次组内每一个批次都会设置为此值（官方字段路径 <c>stock_bundle.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponStockBundleCreateRequestStockBundleStockBundlecouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向品牌方发送通知，需要提供通知相关配置，批次组内每一个批次都会设置为此值（官方字段路径 <c>stock_bundle.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponStockBundleCreateRequestStockBundleStockBundlenotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用，批次组内每一个批次都会设置为此值。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock_bundle.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

}

/// <summary>创建商品券批次组合请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateRequest
{
    /// <summary>【创建请求单号】 品牌创建批次请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【批次组】 为商品券创建的批次组详情（官方字段路径 <c>stock_bundle</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle")]
    public ProductCouponStockBundleCreateRequestStockBundle? StockBundle { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemCouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock_list.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock_list.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemStockListstockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>stock_list.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>stock_list.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>stock_list.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【批次组信息】 批次所在批次组信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemStockListstockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>stock_list.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>stock_list.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemUsageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemUsageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock_list.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_list.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponStockBundleCreateResponseStockListItemUsageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock_list.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock_list.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemStockListcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemStockListcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock_list.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemStockListcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock_list.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock_list.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponStockBundleCreateResponseStockListItemStockListcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock_list.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponStockBundleCreateResponseStockListItemStockListcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemStockListnotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock_list.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItemStockListsentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock_list.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock_list.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>创建商品券批次组合响应·【批次列表】 批次组内批次列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponseStockListItem
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>stock_list.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，由微信支付生成（官方字段路径 <c>stock_list.stock_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【备注】 仅配置品牌可见，用于自定义信息（官方字段路径 <c>stock_list.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充（官方字段路径 <c>stock_list.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock_list.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponStockBundleCreateResponseStockListItemCouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock_list.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponStockBundleCreateResponseStockListItemStockListstockSendRule? StockSendRule { get; set; }

    /// <summary>【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>stock_list.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponStockBundleCreateResponseStockListItemProgressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息（官方字段路径 <c>stock_list.stock_bundle_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponStockBundleCreateResponseStockListItemStockListstockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock_list.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponStockBundleCreateResponseStockListItemUsageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock_list.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponStockBundleCreateResponseStockListItemStockListcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock_list.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponStockBundleCreateResponseStockListItemStockListnotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock_list.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock_list.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponStockBundleCreateResponseStockListItemStockListsentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock_list.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的请求流水号（官方字段路径 <c>stock_list.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock_list.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock_list.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock_list.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>创建商品券批次组合响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280622"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleCreateResponse : WechatPayResponse
{
    /// <summary>【批次组ID】 商品券批次组的唯一标识，由微信支付生成（官方字段路径 <c>stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次列表】 批次组内批次列表（官方字段路径 <c>stock_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("stock_list")]
    public List<ProductCouponStockBundleCreateResponseStockListItem>? StockList { get; set; }

}

/// <summary>修改商品券批次组合请求·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateRequestUsageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>修改商品券批次组合请求·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateRequestUsageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponStockBundleUpdateRequestUsageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>修改商品券批次组合请求·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateRequestCouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>修改商品券批次组合请求·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateRequestCouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>修改商品券批次组合请求·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateRequestCouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponStockBundleUpdateRequestCouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponStockBundleUpdateRequestCouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>修改商品券批次组合请求·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateRequestNotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>修改商品券批次组合请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateRequest
{
    /// <summary>【修改请求单号】 品牌修改批次请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【备注】 仅配置品牌可见，用于自定义信息（官方字段路径 <c>remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>usage_rule_display_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponStockBundleUpdateRequestUsageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>coupon_display_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponStockBundleUpdateRequestCouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>notify_config</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponStockBundleUpdateRequestNotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用，不填默认设置为 `NONE`。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>store_scope</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemCouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock_list.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock_list.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemStockListstockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>stock_list.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>stock_list.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>stock_list.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【批次组信息】 批次所在批次组信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemStockListstockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>stock_list.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>stock_list.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemUsageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemUsageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock_list.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_list.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponStockBundleUpdateResponseStockListItemUsageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock_list.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock_list.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemStockListcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemStockListcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock_list.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemStockListcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock_list.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock_list.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponStockBundleUpdateResponseStockListItemStockListcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock_list.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponStockBundleUpdateResponseStockListItemStockListcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemStockListnotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock_list.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItemStockListsentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock_list.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock_list.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>修改商品券批次组合响应·【批次列表】 批次组内批次列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponseStockListItem
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>stock_list.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，由微信支付生成（官方字段路径 <c>stock_list.stock_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【备注】 仅配置品牌可见，用于自定义信息（官方字段路径 <c>stock_list.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充（官方字段路径 <c>stock_list.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock_list.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponStockBundleUpdateResponseStockListItemCouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock_list.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponStockBundleUpdateResponseStockListItemStockListstockSendRule? StockSendRule { get; set; }

    /// <summary>【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>stock_list.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponStockBundleUpdateResponseStockListItemProgressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息（官方字段路径 <c>stock_list.stock_bundle_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponStockBundleUpdateResponseStockListItemStockListstockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock_list.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponStockBundleUpdateResponseStockListItemUsageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock_list.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponStockBundleUpdateResponseStockListItemStockListcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock_list.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponStockBundleUpdateResponseStockListItemStockListnotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock_list.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock_list.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponStockBundleUpdateResponseStockListItemStockListsentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock_list.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的请求流水号（官方字段路径 <c>stock_list.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock_list.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock_list.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock_list.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>修改商品券批次组合响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280633"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleUpdateResponse : WechatPayResponse
{
    /// <summary>【批次组ID】 商品券批次组的唯一标识，由微信支付生成（官方字段路径 <c>stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次列表】 批次组内批次列表（官方字段路径 <c>stock_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("stock_list")]
    public List<ProductCouponStockBundleUpdateResponseStockListItem>? StockList { get; set; }

}

/// <summary>批次组合关联门店请求·【门店列表】 商户欲关联的门店列表， 单次最多可批量关联200个门店。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280620"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleAssociateStoresRequestStoreListItem
{
    /// <summary>【门店ID】 门店的唯一标识，调用【 创建品牌门店API】时由微信支付生成（官方字段路径 <c>store_list.store_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

}

/// <summary>批次组合关联门店请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280620"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleAssociateStoresRequest
{
    /// <summary>【门店列表】 商户欲关联的门店列表， 单次最多可批量关联200个门店（官方字段路径 <c>store_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("store_list")]
    public List<ProductCouponStockBundleAssociateStoresRequestStoreListItem>? StoreList { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>批次组合关联门店响应·【关联成功的门店列表】 关联成功的门店列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280620"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleAssociateStoresResponseSuccessStoreListItem
{
    /// <summary>【门店ID】 门店的唯一标识，调用【 创建品牌门店API】时由微信支付生成（官方字段路径 <c>success_store_list.store_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

}

/// <summary>批次组合关联门店响应·【关联失败的门店列表】 每个关联失败门店的详细错误信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280620"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleAssociateStoresResponseFailedStoreListItem
{
    /// <summary>【门店ID】 关联失败的门店ID（官方字段路径 <c>failed_store_list.store_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【失败错误码】 门店关联失败的错误码（官方字段路径 <c>failed_store_list.code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>【失败错误信息】 门店关联失败的错误描述（官方字段路径 <c>failed_store_list.message</c>，<b>必填</b>，类型 <c>string(200)</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

}

/// <summary>批次组合关联门店响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280620"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleAssociateStoresResponse : WechatPayResponse
{
    /// <summary>【去重后门店总数】 系统会自动对请求中的门店去重，这里是去重后的门店总数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【关联成功的门店列表】 关联成功的门店列表（官方字段路径 <c>success_store_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("success_store_list")]
    public List<ProductCouponStockBundleAssociateStoresResponseSuccessStoreListItem>? SuccessStoreList { get; set; }

    /// <summary>【关联失败的门店列表】 每个关联失败门店的详细错误信息（官方字段路径 <c>failed_store_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("failed_store_list")]
    public List<ProductCouponStockBundleAssociateStoresResponseFailedStoreListItem>? FailedStoreList { get; set; }

}

/// <summary>批次组合解除门店关联请求·【门店列表】 商户欲取消关联的门店列表， 单次最多可批量取消200个门店。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280629"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleDisassociateStoresRequestStoreListItem
{
    /// <summary>【门店ID】 门店的唯一标识，调用【 创建品牌门店API】时由微信支付生成（官方字段路径 <c>store_list.store_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

}

/// <summary>批次组合解除门店关联请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280629"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleDisassociateStoresRequest
{
    /// <summary>【门店列表】 商户欲取消关联的门店列表， 单次最多可批量取消200个门店（官方字段路径 <c>store_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("store_list")]
    public List<ProductCouponStockBundleDisassociateStoresRequestStoreListItem>? StoreList { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>批次组合解除门店关联响应·【取消关联成功的门店列表】 取消关联成功的门店列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280629"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleDisassociateStoresResponseSuccessStoreListItem
{
    /// <summary>【门店ID】 门店的唯一标识，调用【 创建品牌门店API】时由微信支付生成（官方字段路径 <c>success_store_list.store_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

}

/// <summary>批次组合解除门店关联响应·【取消关联失败的门店列表】 每个取消关联失败门店的详细错误信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280629"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleDisassociateStoresResponseFailedStoreListItem
{
    /// <summary>【门店ID】 取消关联失败的门店ID（官方字段路径 <c>failed_store_list.store_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【失败错误码】 门店取消关联失败的错误码（官方字段路径 <c>failed_store_list.code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>【失败错误信息】 门店取消关联失败的错误描述（官方字段路径 <c>failed_store_list.message</c>，<b>必填</b>，类型 <c>string(200)</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

}

/// <summary>批次组合解除门店关联响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280629"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleDisassociateStoresResponse : WechatPayResponse
{
    /// <summary>【去重后门店总数】 系统会自动对请求中的门店去重，这里是去重后的门店总数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【取消关联成功的门店列表】 取消关联成功的门店列表（官方字段路径 <c>success_store_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("success_store_list")]
    public List<ProductCouponStockBundleDisassociateStoresResponseSuccessStoreListItem>? SuccessStoreList { get; set; }

    /// <summary>【取消关联失败的门店列表】 每个取消关联失败门店的详细错误信息（官方字段路径 <c>failed_store_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("failed_store_list")]
    public List<ProductCouponStockBundleDisassociateStoresResponseFailedStoreListItem>? FailedStoreList { get; set; }

}

/// <summary>修改批次组合预算请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetRequest
{
    /// <summary>【修改请求单号】 品牌修改批次发放次数的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【更新模式】 本接口每次只支持更新一种限额，请选择对应的模式，并传入对应的字段。如果你需要调整多个维度的限制，请使用不同的模式多次调用本接口。可选取值：`MAX_COUNT` 更新批次发放次数总上限 / `MAX_COUNT_PER_DAY` 更新批次每日发放次数上限。注：如果批次当前未限制每日发放次数（创建时未传入），则无法修改为限制次数；反之如果已经限制了每日发放次数，则无法修改为不限制。（官方字段路径 <c>update_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_mode")]
    public string? UpdateMode { get; set; }

    /// <summary>【当前批次发放次数总上限】 当前批次发放次数总上限，调用本接口前，请先通过【 查询商品券批次详情API】获取。当且仅当 `update_mode` 为 `MAX_COUNT` 时必传，其他模式下不应填写（官方字段路径 <c>current_max_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("current_max_count")]
    public long? CurrentMaxCount { get; set; }

    /// <summary>【目标批次发放次数总上限】 将批次发放次数总上限更新为本值，允许小于当前批次发放次数总上限，但是不得小于当前已发放总次数。当且仅当 `update_mode` 为 `MAX_COUNT` 时必传，其他模式下不应填写（官方字段路径 <c>target_max_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("target_max_count")]
    public long? TargetMaxCount { get; set; }

    /// <summary>【当前批次每日发放次数上限】 当前批次每日发放次数上限，调用本接口前，请先通过【 查询商品券批次详情API】获取。当且仅当 `update_mode` 为 `MAX_COUNT_PER_DAY` 时必传，其他模式下不应填写（官方字段路径 <c>current_max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("current_max_count_per_day")]
    public long? CurrentMaxCountPerDay { get; set; }

    /// <summary>【目标批次每日发放次数上限】 将批次每日发放次数上限更新为本值，允许小于当前批次每日发放次数上限。当且仅当 `update_mode` 为 `MAX_COUNT_PER_DAY` 时必传，其他模式下不应填写（官方字段路径 <c>target_max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("target_max_count_per_day")]
    public long? TargetMaxCountPerDay { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemCouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock_list.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock_list.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemStockListstockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>stock_list.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>stock_list.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>stock_list.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock_list.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【批次组信息】 批次所在批次组信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemStockListstockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>stock_list.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>stock_list.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemUsageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemUsageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock_list.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock_list.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock_list.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock_list.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponStockBundleBudgetResponseStockListItemUsageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock_list.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock_list.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemStockListcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemStockListcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock_list.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemStockListcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock_list.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock_list.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock_list.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponStockBundleBudgetResponseStockListItemStockListcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock_list.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponStockBundleBudgetResponseStockListItemStockListcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemStockListnotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock_list.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItemStockListsentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock_list.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock_list.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>修改批次组合预算响应·【批次列表】 批次组内批次列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponseStockListItem
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>stock_list.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，由微信支付生成（官方字段路径 <c>stock_list.stock_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【备注】 仅配置品牌可见，用于自定义信息（官方字段路径 <c>stock_list.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充（官方字段路径 <c>stock_list.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock_list.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponStockBundleBudgetResponseStockListItemCouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock_list.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponStockBundleBudgetResponseStockListItemStockListstockSendRule? StockSendRule { get; set; }

    /// <summary>【多次优惠使用规则】 本批次属于某个多次优惠批次组，这里记录的是本批次的使用规则，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>stock_list.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponStockBundleBudgetResponseStockListItemProgressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息（官方字段路径 <c>stock_list.stock_bundle_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponStockBundleBudgetResponseStockListItemStockListstockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock_list.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponStockBundleBudgetResponseStockListItemUsageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock_list.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponStockBundleBudgetResponseStockListItemStockListcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock_list.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponStockBundleBudgetResponseStockListItemStockListnotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock_list.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock_list.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponStockBundleBudgetResponseStockListItemStockListsentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock_list.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的请求流水号（官方字段路径 <c>stock_list.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock_list.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock_list.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock_list.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>修改批次组合预算响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016280642"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponStockBundleBudgetResponse : WechatPayResponse
{
    /// <summary>【批次组ID】 商品券批次组的唯一标识，由微信支付生成（官方字段路径 <c>stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次列表】 批次组内批次列表（官方字段路径 <c>stock_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("stock_list")]
    public List<ProductCouponStockBundleBudgetResponseStockListItem>? StockList { get; set; }

}
