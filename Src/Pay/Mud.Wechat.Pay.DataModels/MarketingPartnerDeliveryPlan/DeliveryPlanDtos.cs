// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPartnerDeliveryPlan;

/// <summary>设置投放计划回调地址请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184598"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanNotifyUrlSetRequest
{
    /// <summary>【通知回调地址】 通知回调地址，需按照 notify_url填写注意事项 规范填写（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>设置投放计划回调地址响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184598"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanNotifyUrlSetResponse : WechatPayResponse
{
    /// <summary>【通知回调地址】 通知回调地址，投放计划发生状态变更时通知品牌方的通知地址（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>创建投放计划请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184554"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanCreateRequest
{
    /// <summary>【创建请求单号】 创建投放计划的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【创建投放计划的品牌ID】 创建投放计划的品牌ID，可登录 合作伙伴平台 ，在“品牌经营中心-合作品牌管理”页面查看已授权的brand_id（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【商品券ID】 投放的商品券ID，通过请求创建商品券接口获得，具体可查看 创建商品券（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 投放的商品券批次ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 SINGLE 时必传，其他模式不应填写（官方字段路径 <c>stock_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【是否复用商品券和批次信息】 是：表示从商品券和批次获取信息自动填充plan_name、total_count、user_limit、daily_limit、delivery_start_time、delivery_end_time。当投放计划在投放中状态时，若商品券批次的库存发生变化，投放计划会自动更新最新库存。 注： plan_name默认取投放商品券的name字段； total_count默认取投放批次的max_count字段； user_limit…（官方字段路径 <c>reuse_coupon_config</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("reuse_coupon_config")]
    public bool? ReuseCouponConfig { get; set; }

    /// <summary>【投放计划名称】 投放计划名称，该名称会用于C端展示，场景说明详见产品介绍，建议与商品券名称保持一致，12个中文字符以内（官方字段路径 <c>plan_name</c>，选填，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>【投放总库存数量】 用于约定投放计划的总投放库存额度，创建投放计划的券可用库存需在 10000 以上。当 reuse_coupon_config 为 false 时必填（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【单用户限领】 约定了投放计划单用户维度的限领数量； 非必填，如创建时未填写，表示投放计划层不限制，且后续修改不支持修改（官方字段路径 <c>user_limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_limit")]
    public long? UserLimit { get; set; }

    /// <summary>【单日限领】 约定了投放计划单日维度的限领；非必填，如创建时未填写，则修改时不支持填写（官方字段路径 <c>daily_limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("daily_limit")]
    public long? DailyLimit { get; set; }

    /// <summary>【投放开始时间】 投放生效的开始时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。如未填写表示生效时间将以…（官方字段路径 <c>delivery_start_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_start_time")]
    public string? DeliveryStartTime { get; set; }

    /// <summary>【投放结束时间】 投放生效的结束时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。如未填写表示生效时间将以…（官方字段路径 <c>delivery_end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_end_time")]
    public string? DeliveryEndTime { get; set; }

    /// <summary>【营销标签】 用于在优惠左上角展示的运营推荐语信息。自定义文案，不超过9个中文字符或18个英文字符（官方字段路径 <c>recommend_word</c>，选填，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recommend_word")]
    public string? RecommendWord { get; set; }

    /// <summary>【使用模式】 商品券使用模式。多次优惠使用模式时，该字段必填。若未填写，默认为SINGLE。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>usage_mode</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【投放批次组ID】 投放的批次组ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 PROGRESSIVE_BUNDLE 时必传，其他模式不应填写（官方字段路径 <c>stock_bundle_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【过期券不占用领取次数】 是否在用户领券后未使用而过期时，回退已计入的单用户领取次数。 `true`：过期（未核销失效）后可再领； `false`：过期仍占次数。已核销无论开关如何仍占次数。不传默认为 `false`。创建后不可修改（官方字段路径 <c>exclude_expired_coupon_from_limit</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("exclude_expired_coupon_from_limit")]
    public bool? ExcludeExpiredCouponFromLimit { get; set; }

}

/// <summary>创建投放计划响应·【投放计划详情】 创建成功的投放计划详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184554"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanCreateResponsePlan
{
    /// <summary>【投放计划ID】 投放计划ID（官方字段路径 <c>plan.plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【投放计划名称】 投放计划名称（官方字段路径 <c>plan.plan_name</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>【投放计划状态】 投放计划状态，已终止、已过期是终态。可选取值：`CREATED` 创建成功 / `TERMINATED` 已终止 / `EXPIRED` 已过期 / `DELIVERING` 投放中 / `PAUSED` 已暂停（官方字段路径 <c>plan.plan_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("plan_state")]
    public string? PlanState { get; set; }

    /// <summary>【投放开始时间】 投放开始时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>plan.delivery_start_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_start_time")]
    public string? DeliveryStartTime { get; set; }

    /// <summary>【投放结束时间】 投放结束时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>plan.delivery_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_end_time")]
    public string? DeliveryEndTime { get; set; }

    /// <summary>【商品券ID】 商品券ID，通过请求创建商品券接口获得，具体可查看 创建商品券（官方字段路径 <c>plan.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>plan.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【投放批次ID】 投放的商品券批次ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 SINGLE 时必传（官方字段路径 <c>plan.stock_id</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【投放批次组ID】 投放的批次组ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 PROGRESSIVE_BUNDLE 时必填（官方字段路径 <c>plan.stock_bundle_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【营销标签】 用于在优惠左上角展示的运营推荐语信息。自定义文案，不超过9个中文字符或18个英文字符。若创建时未设置则不返回（官方字段路径 <c>plan.recommend_word</c>，选填，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recommend_word")]
    public string? RecommendWord { get; set; }

    /// <summary>【品牌ID】 创建投放计划的品牌ID，可登录 合作伙伴平台 ，在“品牌经营中心-合作品牌管理”页面查看已授权的brand_id信息（官方字段路径 <c>plan.brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【投放总库存数量】 约定了投放计划的总投放库存（官方字段路径 <c>plan.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【单用户限领】 约定了投放计划单用户维度的限领数量； 非必填，如创建时未填写，则修改时不支持填写（官方字段路径 <c>plan.user_limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_limit")]
    public long? UserLimit { get; set; }

    /// <summary>【单日限领】 用于约定投放计划单日可领取的最大数量，如创建时未填写，则修改时不支持填写（官方字段路径 <c>plan.daily_limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("daily_limit")]
    public long? DailyLimit { get; set; }

    /// <summary>【是否复用商品券和批次信息】 是：表示从商品券和批次获取信息自动填充plan_name、total_count、user_limit、daily_limit、delivery_start_time、delivery_end_time。当投放计划在投放中状态时，若商品券批次的库存发生变化，投放计划会自动更新最新库存。 注： plan_name默认取投放商品券的name字段； total_count默认取投放批次的max_count字段； user_limit…（官方字段路径 <c>plan.reuse_coupon_config</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("reuse_coupon_config")]
    public bool? ReuseCouponConfig { get; set; }

    /// <summary>【过期券不占用领取次数】 是否在用户领券后未使用而过期时，回退已计入的单用户领取次数。 `true`：过期（未核销失效）后可再领。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（过期仍占次数）处理。已核销无论开关如何仍占次数（官方字段路径 <c>plan.exclude_expired_coupon_from_limit</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("exclude_expired_coupon_from_limit")]
    public bool? ExcludeExpiredCouponFromLimit { get; set; }

}

/// <summary>创建投放计划响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184554"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanCreateResponse : WechatPayResponse
{
    /// <summary>【投放计划详情】 创建成功的投放计划详情（官方字段路径 <c>plan</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("plan")]
    public DeliveryPlanPlanCreateResponsePlan? Plan { get; set; }

}

/// <summary>查询投放计划列表响应·【投放计划列表】 投放计划列表，当有满足查询条件的投放计划时返回该字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184563"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanListResponsePlanListItem
{
    /// <summary>【投放计划ID】 投放计划ID（官方字段路径 <c>plan_list.plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【投放计划名称】 投放计划名称（官方字段路径 <c>plan_list.plan_name</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>【投放计划状态】 投放计划状态，已终止、已过期是终态。可选取值：`CREATED` 创建成功 / `TERMINATED` 已终止 / `EXPIRED` 已过期 / `DELIVERING` 投放中 / `PAUSED` 已暂停（官方字段路径 <c>plan_list.plan_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("plan_state")]
    public string? PlanState { get; set; }

    /// <summary>【投放开始时间】 投放开始时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>plan_list.delivery_start_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_start_time")]
    public string? DeliveryStartTime { get; set; }

    /// <summary>【投放结束时间】 投放结束时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>plan_list.delivery_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_end_time")]
    public string? DeliveryEndTime { get; set; }

    /// <summary>【商品券ID】 商品券ID，通过请求创建商品券接口获得，具体可查看 创建商品券（官方字段路径 <c>plan_list.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>plan_list.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【投放批次ID】 投放的商品券批次ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 SINGLE 时必传（官方字段路径 <c>plan_list.stock_id</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【投放批次组ID】 投放的批次组ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 PROGRESSIVE_BUNDLE 时必填（官方字段路径 <c>plan_list.stock_bundle_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【营销标签】 用于在优惠左上角展示的运营推荐语信息。自定义文案，不超过9个中文字符或18个英文字符。若创建时未设置则不返回（官方字段路径 <c>plan_list.recommend_word</c>，选填，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recommend_word")]
    public string? RecommendWord { get; set; }

    /// <summary>【品牌ID】 创建投放计划的品牌ID，可登录 合作伙伴平台 ，在“品牌经营中心-合作品牌管理”页面查看已授权的brand_id信息（官方字段路径 <c>plan_list.brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【投放总库存数量】 约定了投放计划的总投放库存（官方字段路径 <c>plan_list.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【单用户限领】 约定了投放计划单用户维度的限领数量； 非必填，如创建时未填写，则修改时不支持填写（官方字段路径 <c>plan_list.user_limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_limit")]
    public long? UserLimit { get; set; }

    /// <summary>【单日限领】 用于约定投放计划单日可领取的最大数量，如创建时未填写，则修改时不支持填写（官方字段路径 <c>plan_list.daily_limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("daily_limit")]
    public long? DailyLimit { get; set; }

    /// <summary>【是否复用商品券和批次信息】 是：表示从商品券和批次获取信息自动填充plan_name、total_count、user_limit、daily_limit、delivery_start_time、delivery_end_time。当投放计划在投放中状态时，若商品券批次的库存发生变化，投放计划会自动更新最新库存。 注： plan_name默认取投放商品券的name字段； total_count默认取投放批次的max_count字段； user_limit…（官方字段路径 <c>plan_list.reuse_coupon_config</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("reuse_coupon_config")]
    public bool? ReuseCouponConfig { get; set; }

    /// <summary>【过期券不占用领取次数】 是否在用户领券后未使用而过期时，回退已计入的单用户领取次数。 `true`：过期（未核销失效）后可再领。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（过期仍占次数）处理。已核销无论开关如何仍占次数（官方字段路径 <c>plan_list.exclude_expired_coupon_from_limit</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("exclude_expired_coupon_from_limit")]
    public bool? ExcludeExpiredCouponFromLimit { get; set; }

}

/// <summary>查询投放计划列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184563"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanListResponse : WechatPayResponse
{
    /// <summary>【资源总条数】 返回资源总条数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【投放计划列表】 投放计划列表，当有满足查询条件的投放计划时返回该字段（官方字段路径 <c>plan_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("plan_list")]
    public List<DeliveryPlanPlanListResponsePlanListItem>? PlanList { get; set; }

}

/// <summary>修改投放计划请求·【修改内容】 修改内容，修改哪个字段就填入哪个字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184594"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanUpdateRequestModifyContent
{
    /// <summary>【投放计划名称】 投放计划，用于内部管理，不做C端展示（官方字段路径 <c>modify_content.plan_name</c>，选填，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>【投放结束时间】 投放结束时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。注：活动结束时间仅支持延长，不…（官方字段路径 <c>modify_content.delivery_end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_end_time")]
    public string? DeliveryEndTime { get; set; }

    /// <summary>【投放总库存数量】 约定了投放计划的总投放库存，创建投放计划的券可用库存需在 10000 以上。注：修改后活动库存需大于原库存，不支持减少库存（官方字段路径 <c>modify_content.total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【单用户限领】 约定了投放计划用户维度的限领；非必填，如创建时未填写，则修改时不支持填写。注：修改后单用户限额需大于原单用户限额，不支持减少限额（官方字段路径 <c>modify_content.user_limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_limit")]
    public long? UserLimit { get; set; }

    /// <summary>【单日限领】 约定了投放计划单用户单日维度的限领；非必填，如创建时未填写，则修改时不支持填写。注：修改后活动日限额需大于原限额，不支持减少日限额（官方字段路径 <c>modify_content.daily_limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("daily_limit")]
    public long? DailyLimit { get; set; }

    /// <summary>【营销标签】 用于在优惠左上角展示的运营推荐语信息。自定义文案，不超过9个中文字符或18个英文字符（官方字段路径 <c>modify_content.recommend_word</c>，选填，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recommend_word")]
    public string? RecommendWord { get; set; }

}

/// <summary>修改投放计划请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184594"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanUpdateRequest
{
    /// <summary>【修改内容】 修改内容，修改哪个字段就填入哪个字段（官方字段路径 <c>modify_content</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("modify_content")]
    public DeliveryPlanPlanUpdateRequestModifyContent? ModifyContent { get; set; }

    /// <summary>【修改请求单号】 修改投放计划的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

}

/// <summary>修改投放计划响应·【投放计划详情】 修改后的投放计划详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184594"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanUpdateResponsePlan
{
    /// <summary>【投放计划ID】 投放计划ID（官方字段路径 <c>plan.plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【投放计划名称】 投放计划名称（官方字段路径 <c>plan.plan_name</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>【投放计划状态】 投放计划状态，已终止、已过期是终态。可选取值：`CREATED` 创建成功 / `TERMINATED` 已终止 / `EXPIRED` 已过期 / `DELIVERING` 投放中 / `PAUSED` 已暂停（官方字段路径 <c>plan.plan_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("plan_state")]
    public string? PlanState { get; set; }

    /// <summary>【投放开始时间】 投放开始时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>plan.delivery_start_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_start_time")]
    public string? DeliveryStartTime { get; set; }

    /// <summary>【投放结束时间】 投放结束时间，遵循 rfc3339 标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>plan.delivery_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_end_time")]
    public string? DeliveryEndTime { get; set; }

    /// <summary>【商品券ID】 商品券ID，通过请求创建商品券接口获得，具体可查看 创建商品券（官方字段路径 <c>plan.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>plan.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【投放批次ID】 投放的商品券批次ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 SINGLE 时必传（官方字段路径 <c>plan.stock_id</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【投放批次组ID】 投放的批次组ID，通过请求创建商品券接口获得，具体可查看 创建商品券 ，当且仅当 usage_mode 为 PROGRESSIVE_BUNDLE 时必填（官方字段路径 <c>plan.stock_bundle_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【营销标签】 用于在优惠左上角展示的运营推荐语信息。自定义文案，不超过9个中文字符或18个英文字符。若创建时未设置则不返回（官方字段路径 <c>plan.recommend_word</c>，选填，类型 <c>string(27)</c>）。</summary>
    [JsonPropertyName("recommend_word")]
    public string? RecommendWord { get; set; }

    /// <summary>【品牌ID】 创建投放计划的品牌ID，可登录 合作伙伴平台 ，在“品牌经营中心-合作品牌管理”页面查看已授权的brand_id信息（官方字段路径 <c>plan.brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【投放总库存数量】 约定了投放计划的总投放库存（官方字段路径 <c>plan.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【单用户限领】 约定了投放计划单用户维度的限领数量； 非必填，如创建时未填写，则修改时不支持填写（官方字段路径 <c>plan.user_limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_limit")]
    public long? UserLimit { get; set; }

    /// <summary>【单日限领】 用于约定投放计划单日可领取的最大数量，如创建时未填写，则修改时不支持填写（官方字段路径 <c>plan.daily_limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("daily_limit")]
    public long? DailyLimit { get; set; }

    /// <summary>【是否复用商品券和批次信息】 是：表示从商品券和批次获取信息自动填充plan_name、total_count、user_limit、daily_limit、delivery_start_time、delivery_end_time。当投放计划在投放中状态时，若商品券批次的库存发生变化，投放计划会自动更新最新库存。 注： plan_name默认取投放商品券的name字段； total_count默认取投放批次的max_count字段； user_limit…（官方字段路径 <c>plan.reuse_coupon_config</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("reuse_coupon_config")]
    public bool? ReuseCouponConfig { get; set; }

    /// <summary>【过期券不占用领取次数】 是否在用户领券后未使用而过期时，回退已计入的单用户领取次数。 `true`：过期（未核销失效）后可再领。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（过期仍占次数）处理。已核销无论开关如何仍占次数（官方字段路径 <c>plan.exclude_expired_coupon_from_limit</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("exclude_expired_coupon_from_limit")]
    public bool? ExcludeExpiredCouponFromLimit { get; set; }

}

/// <summary>修改投放计划响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184594"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerDeliveryPlan")]
public class DeliveryPlanPlanUpdateResponse : WechatPayResponse
{
    /// <summary>【投放计划详情】 修改后的投放计划详情（官方字段路径 <c>plan</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("plan")]
    public DeliveryPlanPlanUpdateResponsePlan? Plan { get; set; }

}
