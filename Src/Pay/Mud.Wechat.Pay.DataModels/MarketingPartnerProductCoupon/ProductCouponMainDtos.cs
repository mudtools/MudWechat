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

/// <summary>创建商品券（单品批次形态）请求·【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `S·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestSingleUsageInfoSingleUsageInfonormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `S·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestSingleUsageInfoSingleUsageInfodiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `S。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponCouponCreateRequestSingleUsageInfoSingleUsageInfonormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponCouponCreateRequestSingleUsageInfoSingleUsageInfodiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【发放规则】 批次的发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStockstockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>stock.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>stock.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>stock.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。取值范围为 3600~86400，且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【单券使用规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponCouponCreateRequestStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponCouponCreateRequestStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponCouponCreateRequestStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStockusageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponCouponCreateRequestStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponCouponCreateRequestStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponCouponCreateRequestStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求·【批次】 商品券批次信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequestStock
{
    /// <summary>【备注】 仅配置品牌方可见，用于自定义信息，最多20个UTF-8字符（官方字段路径 <c>stock.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【发放规则】 批次的发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponCouponCreateRequestStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】（官方字段路径 <c>stock.single_usage_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponCouponCreateRequestStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponCouponCreateRequestStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponCouponCreateRequestStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponCouponCreateRequestStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

}

/// <summary>创建商品券（单品批次形态）请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateRequest
{
    /// <summary>【创建请求单号】 品牌创建商品券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【优惠范围】 商品券的优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式 `SINGLE`: 单券，即用户只能使用一次，使用后券失效（官方字段路径 <c>usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `SINGLE` 时选填（是否填写取决于是否需要填写内部字段），其他模式不应填写（官方字段路径 <c>single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponCouponCreateRequestSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)（官方字段路径 <c>display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponCouponCreateRequestDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 品牌侧的商品标识，品牌可自行选择传入，长度不超过40个字符。 注：微信支付不校验此参数的唯一性，即允许多个商品券共享外部商品ID（官方字段路径 <c>out_product_no</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【批次】 商品券批次信息（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponCouponCreateRequestStock? Stock { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价，而非满减金额。仅单品满减券且品牌在换价白名单内可开启；不传默认为 `false`。创建后不可修改（官方字段路径 <c>show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 本券允许核销的商品编码，商户可自行选择传入，最多100个，可使用数字、大小写字母组成，长度不超过20个字符。仅优惠范围为单品或品类时可填写，全场不可填写（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string(20)]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE ·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseSingleUsageInfoSingleUsageInfonormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE ·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseSingleUsageInfoSingleUsageInfodiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponCouponCreateResponseSingleUsageInfoSingleUsageInfonormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponCouponCreateResponseSingleUsageInfoSingleUsageInfodiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStockstockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>stock.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>stock.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>stock.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。取值范围为 3600~86400，且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【单券使用规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponCouponCreateResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponCouponCreateResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponCouponCreateResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStockusageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponCouponCreateResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponCouponCreateResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponCouponCreateResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应·【批次】 商品券批次信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponseStock
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>stock.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，由微信支付生成（官方字段路径 <c>stock.stock_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【备注】 仅配置品牌可见，用于自定义信息（官方字段路径 <c>stock.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponCouponCreateResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponCouponCreateResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】（官方字段路径 <c>stock.single_usage_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponCouponCreateResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponCouponCreateResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponCouponCreateResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponCouponCreateResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponCouponCreateResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>创建商品券（单品批次形态）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponCreateResponse : WechatPayResponse
{
    /// <summary>【商品券ID】 商品券的唯一标识，本接口成功时由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围 `。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式 `SINGLE`: 单券，即用户只能使用一次，使用后券失效（官方字段路径 <c>usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE 时提供，其他模式不提供（官方字段路径 <c>single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponCouponCreateResponseSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponCouponCreateResponseDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【批次】 商品券批次信息（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponCouponCreateResponseStock? Stock { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `S·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestSingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `S·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestSingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `S。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponBundleCouponCreateRequestSingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponBundleCouponCreateRequestSingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponBundleCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponBundleCouponCreateRequestDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【发放规则】 批次的发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStockstockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>stock.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>stock.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>stock.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。取值范围为 3600~86400，且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【单券使用规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponBundleCouponCreateRequestStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStockusageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponBundleCouponCreateRequestStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponBundleCouponCreateRequestStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponBundleCouponCreateRequestStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求·【批次】 商品券批次信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequestStock
{
    /// <summary>【备注】 仅配置品牌方可见，用于自定义信息，最多20个UTF-8字符（官方字段路径 <c>stock.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【发放规则】 批次的发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponBundleCouponCreateRequestStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】（官方字段路径 <c>stock.single_usage_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponBundleCouponCreateRequestStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponBundleCouponCreateRequestStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponBundleCouponCreateRequestStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponBundleCouponCreateRequestStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

}

/// <summary>创建商品券（批次组合形态）请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateRequest
{
    /// <summary>【创建请求单号】 品牌创建商品券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【优惠范围】 商品券的优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式 `SINGLE`: 单券，即用户只能使用一次，使用后券失效（官方字段路径 <c>usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，当且仅当 `usage_mode` 为 `SINGLE` 时选填（是否填写取决于是否需要填写内部字段），其他模式不应填写（官方字段路径 <c>single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponBundleCouponCreateRequestSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息，展示结果可参考 商品券示意图(单券模式)（官方字段路径 <c>display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponBundleCouponCreateRequestDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 品牌侧的商品标识，品牌可自行选择传入，长度不超过40个字符。 注：微信支付不校验此参数的唯一性，即允许多个商品券共享外部商品ID（官方字段路径 <c>out_product_no</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【批次】 商品券批次信息（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponBundleCouponCreateRequestStock? Stock { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价，而非满减金额。仅单品满减券且品牌在换价白名单内可开启；不传默认为 `false`。创建后不可修改（官方字段路径 <c>show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 本券允许核销的商品编码，商户可自行选择传入，最多100个，可使用数字、大小写字母组成，长度不超过20个字符。仅优惠范围为单品或品类时可填写，全场不可填写（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string(20)]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE ·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseSingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE ·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseSingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponBundleCouponCreateResponseSingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponBundleCouponCreateResponseSingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponBundleCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponBundleCouponCreateResponseDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStockstockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>stock.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>stock.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>stock.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。取值范围为 3600~86400，且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【单券使用规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponBundleCouponCreateResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStockusageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>stock.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>stock.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>stock.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>stock.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>stock.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponBundleCouponCreateResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponBundleCouponCreateResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponBundleCouponCreateResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应·【批次】 商品券批次信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponseStock
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>stock.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，由微信支付生成（官方字段路径 <c>stock.stock_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【备注】 仅配置品牌可见，用于自定义信息（官方字段路径 <c>stock.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponBundleCouponCreateResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponBundleCouponCreateResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】（官方字段路径 <c>stock.single_usage_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponBundleCouponCreateResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponBundleCouponCreateResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponBundleCouponCreateResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponBundleCouponCreateResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponBundleCouponCreateResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>创建商品券（批次组合形态）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781289"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponBundleCouponCreateResponse : WechatPayResponse
{
    /// <summary>【商品券ID】 商品券的唯一标识，本接口成功时由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围 `。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式 `SINGLE`: 单券，即用户只能使用一次，使用后券失效（官方字段路径 <c>usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】单券模式配置信息，仅当 usage_mode 为 SINGLE 时提供，其他模式不提供（官方字段路径 <c>single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponBundleCouponCreateResponseSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponBundleCouponCreateResponseDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【批次】 商品券批次信息（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponBundleCouponCreateResponseStock? Stock { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>查询商品券详情响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponseSingleUsageInfoSingleUsageInfonormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>查询商品券详情响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponseSingleUsageInfoSingleUsageInfodiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>查询商品券详情响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponseSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponCouponQueryResponseSingleUsageInfoSingleUsageInfonormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponCouponQueryResponseSingleUsageInfoSingleUsageInfodiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>查询商品券详情响应·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponseProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>查询商品券详情响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>查询商品券详情响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponseDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponCouponQueryResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>查询商品券详情响应·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponseDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponCouponQueryResponseDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>查询商品券详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016434632"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponQueryResponse : WechatPayResponse
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponCouponQueryResponseSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponCouponQueryResponseProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponCouponQueryResponseDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>修改商品券请求·【展示信息】 商品券展示信息；与 available_product_no_li·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateRequestDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>修改商品券请求·【展示信息】 商品券展示信息；与 available_product_no_li·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateRequestDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponCouponUpdateRequestDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>修改商品券请求·【展示信息】 商品券展示信息；与 available_product_no_li。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateRequestDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 传入图片列表，将直接覆盖全部已有图片；不传参数或者传空列表，不会改动现有的图片列表；当前不支持清空已设置的图片列表。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponCouponUpdateRequestDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>修改商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateRequest
{
    /// <summary>【修改请求单号】 品牌修改商品券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【展示信息】 商品券展示信息；与 available_product_no_list 至少传其一（官方字段路径 <c>display_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponCouponUpdateRequestDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【可用商品编码列表】 可用商品编码列表；与 display_info 至少传其一；不传或空列表表示不修改本字段；仅单品或品类可填。单个编码可使用数字、大小写字母组成，长度不超过20个字符（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string(20)]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>修改商品券响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponseSingleUsageInfoSingleUsageInfonormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>修改商品券响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponseSingleUsageInfoSingleUsageInfodiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>修改商品券响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponseSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponCouponUpdateResponseSingleUsageInfoSingleUsageInfonormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponCouponUpdateResponseSingleUsageInfoSingleUsageInfodiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>修改商品券响应·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponseProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>修改商品券响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>修改商品券响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponseDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponCouponUpdateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>修改商品券响应·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponseDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 传入图片列表，将直接覆盖全部已有图片；不传参数或者传空列表，不会改动现有的图片列表；当前不支持清空已设置的图片列表。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponCouponUpdateResponseDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>修改商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781296"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponUpdateResponse : WechatPayResponse
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponCouponUpdateResponseSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponCouponUpdateResponseProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponCouponUpdateResponseDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>作废商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateRequest
{
    /// <summary>【失效请求单号】 品牌失效商品券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【失效原因】 记录商品券失效原因，长度不超过150个UTF-8字符（官方字段路径 <c>deactivate_reason</c>，<b>必填</b>，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>作废商品券响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponseSingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>作废商品券响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponseSingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>作废商品券响应·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponseSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponCouponDeactivateResponseSingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponCouponDeactivateResponseSingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>作废商品券响应·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponseProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>作废商品券响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>作废商品券响应·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponseDisplayInfoDisplayInfocomboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponCouponDeactivateResponseDisplayInfoDisplayInfocomboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>作废商品券响应·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponseDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponCouponDeactivateResponseDisplayInfoDisplayInfocomboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>作废商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781290"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponCouponDeactivateResponse : WechatPayResponse
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponCouponDeactivateResponseSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponCouponDeactivateResponseProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponCouponDeactivateResponseDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}
