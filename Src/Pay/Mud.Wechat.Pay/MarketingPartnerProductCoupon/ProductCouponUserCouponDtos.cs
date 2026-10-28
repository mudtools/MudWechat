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

/// <summary>向用户发放商品券请求·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendRequestCouponTagInfoCouponTagInfomemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>向用户发放商品券请求·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendRequestCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponSendRequestCouponTagInfoCouponTagInfomemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>向用户发放商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendRequest
{
    /// <summary>【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 `product_coupon_id` 对应的商品券（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【用户商品券Code】 用户商品券的唯一标识，给用户发券时需要为该用户的券分配，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度不超过40个字符。 当且仅当 `coupon_code_mode` 为 `API_ASSIGN` 时必传，其他模式下用户商品券Code由微信支付分配，不允许传入（官方字段路径 <c>coupon_code</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后在服务商平台中绑定，详见 服务商商户号与AppID账号关联管理（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【发券请求单号】 品牌给用户发券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-128个字符之间（官方字段路径 <c>send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【自定义附加信息】 发券时品牌方可以使用该字段附带自定义附加信息，长度限制150个UTF-8字符。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过此接口发放以及调用「向用户预发放商品券」接口并在领券组件中发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>attach</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponSendRequestCouponTagInfo? CouponTagInfo { get; set; }

}

/// <summary>向用户发放商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseSingleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>向用户发放商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseSingleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>向用户发放商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseSingleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>single_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>single_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>single_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponSendResponseSingleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponSendResponseSingleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>single_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>single_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCouponProductCouponsingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCouponProductCouponsingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponSendResponseProductCouponProductCouponsingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponSendResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCouponProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCouponProductCoupondisplayInfoComboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>product_coupon.display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponUserCouponSendResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCouponProductCoupondisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>product_coupon.display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>product_coupon.display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>product_coupon.display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>product_coupon.display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>product_coupon.display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponUserCouponSendResponseProductCouponProductCoupondisplayInfoComboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>向用户发放商品券响应·【商品券信息】 该用户商品券对应的商品券详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseProductCoupon
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>product_coupon.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>product_coupon.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>product_coupon.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>product_coupon.single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponUserCouponSendResponseProductCouponProductCouponsingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>product_coupon.progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponUserCouponSendResponseProductCouponProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>product_coupon.display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponUserCouponSendResponseProductCouponProductCoupondisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>product_coupon.out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>product_coupon.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>product_coupon.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>product_coupon.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>product_coupon.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>product_coupon.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>product_coupon.show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>product_coupon.available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStockstockSendRule
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

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriod
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

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponSendResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponSendResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponSendResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponSendResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
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

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStockusageRuleDisplayInfo
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
    public ProductCouponUserCouponSendResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponUserCouponSendResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponUserCouponSendResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>向用户发放商品券响应·【批次信息】 该用户商品券发券时使用的批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseStock
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

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放；商品券的 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时不可使用本模式（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponUserCouponSendResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponUserCouponSendResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>stock.single_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponUserCouponSendResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponUserCouponSendResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponUserCouponSendResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponUserCouponSendResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponUserCouponSendResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>向用户发放商品券响应·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseCouponTagInfoCouponTagInfomemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>向用户发放商品券响应·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponseCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponSendResponseCouponTagInfoCouponTagInfomemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>向用户发放商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponSendResponse : WechatPayResponse
{
    /// <summary>【用户商品券Code】 用户商品券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【用户商品券状态】。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间 / `USED` 已核销，用户商品券已核销 / `EXPIRED` 已过期，用户商品券已超过有效期，不再可用 / `DELETED` 已删除，用户主动删除该券 / `DEACTIVATED` 已失效，品牌方主动调用【 [失效用户商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781578.md)】或【 [失效用户商品券组API](https://pay.weixin.qq.com/doc/v3/partner/4016280658.md)】使用户商品券失效（官方字段路径 <c>coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【有效期开始时间】 用户商品券可用开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【有效期结束时间】 用户商品券可用结束时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【领券时间】 用户领券时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】 发券时传入的请求流水号（官方字段路径 <c>send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【发券渠道】 描述用户商品券是经由什么渠道发送的。可选取值：`BRAND_MANAGE` 摇一摇有优惠，通过摇一摇有优惠渠道发放 / `API` 服务商自主发券，服务商通过发券接口自主发券到商家名片 / `RECEIVE_COMPONENT` 小程序领券组件，服务商通过小程序领券组件发券 / `MEMBER` 会员，品牌方通过会员相关活动或渠道进行发券 / `STICKER_GIFT` 贴图有礼，用户参与品牌贴图有礼活动后发放（官方字段路径 <c>send_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_channel")]
    public string? SendChannel { get; set; }

    /// <summary>【确认请求单号】 品牌方确认发券请求时传入的的请求流水号。当且仅当 品牌方调用【 确认发放用户商品券API】后提供（官方字段路径 <c>confirm_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_request_no")]
    public string? ConfirmRequestNo { get; set; }

    /// <summary>【确认发放时间】 品牌方确认发券时间，当且仅当 品牌方调用【 确认发放用户商品券API】后提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>confirm_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>【失效请求单号】 品牌方失效券请求时传入的的请求流水号。当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 失效时间，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 失效券的原因，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提供（官方字段路径 <c>single_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_detail")]
    public ProductCouponUserCouponSendResponseSingleUsageDetail? SingleUsageDetail { get; set; }

    /// <summary>【商品券信息】 该用户商品券对应的商品券详情（官方字段路径 <c>product_coupon</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("product_coupon")]
    public ProductCouponUserCouponSendResponseProductCoupon? ProductCoupon { get; set; }

    /// <summary>【批次信息】 该用户商品券发券时使用的批次详情（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponUserCouponSendResponseStock? Stock { get; set; }

    /// <summary>【自定义附加信息】 调用发券接口时品牌方使用 `attach` 字段主动设置的附加信息。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过发券接口发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>attach</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【渠道自定义信息】 使用微信支付提供的其他渠道（比如「摇一摇有优惠」）发放商品券时，渠道可能会设置该渠道特定的自定义信息，请根据 `send_channel` 字段判断如何解析本字段。不同渠道的自定义信息格式不同，请根据对应渠道的文档解析（官方字段路径 <c>channel_custom_info</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("channel_custom_info")]
    public string? ChannelCustomInfo { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponSendResponseCouponTagInfo? CouponTagInfo { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemSingleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemSingleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemSingleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>user_coupon_list.single_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.single_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>user_coupon_list.single_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.single_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemSingleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>user_coupon_list.single_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemSingleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>user_coupon_list.single_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>user_coupon_list.single_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProgressiveBundleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProgressiveBundleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemProgressiveBundleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserProductCouponBundleInfo
{
    /// <summary>【用户券组ID】 本券所属的用户券组ID，【 向用户发放商品券批次组】时由微信支付生成（官方字段路径 <c>user_coupon_list.user_product_coupon_bundle_info.user_coupon_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_id")]
    public string? UserCouponBundleId { get; set; }

    /// <summary>【用户券组内索引】 本券在所属用户券组内的序号，从 0 开始编号（官方字段路径 <c>user_coupon_list.user_product_coupon_bundle_info.user_coupon_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_index")]
    public long? UserCouponBundleIndex { get; set; }

    /// <summary>【总可使用次数】 所属用户券组总计可用次数（官方字段路径 <c>user_coupon_list.user_product_coupon_bundle_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【已使用次数】 所属用户券组已使用次数（官方字段路径 <c>user_coupon_list.user_product_coupon_bundle_info.used_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("used_count")]
    public long? UsedCount { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCouponSingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.product_coupon.single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>user_coupon_list.product_coupon.single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCouponSingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.product_coupon.single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>user_coupon_list.product_coupon.single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCouponSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>user_coupon_list.product_coupon.single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemProductCouponSingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>user_coupon_list.product_coupon.single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemProductCouponSingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCouponProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>user_coupon_list.product_coupon.progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>user_coupon_list.product_coupon.progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCouponDisplayInfoComboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCouponDisplayInfoComboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponUserCouponListResponseUserCouponListItemProductCouponDisplayInfoComboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCouponDisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>user_coupon_list.product_coupon.display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>user_coupon_list.product_coupon.display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>user_coupon_list.product_coupon.display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>user_coupon_list.product_coupon.display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>user_coupon_list.product_coupon.display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>user_coupon_list.product_coupon.display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponUserCouponListResponseUserCouponListItemProductCouponDisplayInfoComboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【商品券信息】 该用户商品券对应的商品券详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemProductCoupon
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>user_coupon_list.product_coupon.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>user_coupon_list.product_coupon.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>user_coupon_list.product_coupon.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>user_coupon_list.product_coupon.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>user_coupon_list.product_coupon.single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemProductCouponSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>user_coupon_list.product_coupon.progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemProductCouponProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>user_coupon_list.product_coupon.display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemProductCouponDisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>user_coupon_list.product_coupon.out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>user_coupon_list.product_coupon.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>user_coupon_list.product_coupon.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>user_coupon_list.product_coupon.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>user_coupon_list.product_coupon.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>user_coupon_list.product_coupon.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>user_coupon_list.product_coupon.show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>user_coupon_list.product_coupon.available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>user_coupon_list.stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>user_coupon_list.stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockStockSendRule
{
    /// <summary>【发放次数总上限】 批次在生命周期内总发放次数，最多 100,000,000 次（官方字段路径 <c>user_coupon_list.stock.stock_send_rule.max_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count")]
    public long? MaxCount { get; set; }

    /// <summary>【每日发放次数上限】 批次在每天内发放次数上限，每日刷新，最多 100,000,000 次。默认不限制每日发放上限（官方字段路径 <c>user_coupon_list.stock.stock_send_rule.max_count_per_day</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_day")]
    public long? MaxCountPerDay { get; set; }

    /// <summary>【每个用户领取次数上限】 每个用户最多可领取本批次的次数，最多 100 次（官方字段路径 <c>user_coupon_list.stock.stock_send_rule.max_count_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_count_per_user")]
    public long? MaxCountPerUser { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>user_coupon_list.stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `P。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockStockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>user_coupon_list.stock.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>user_coupon_list.stock.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockUsageRuleDisplayInfoCouponAvailableStoreInfo
{
    /// <summary>【券可用门店描述】 告知用户本券可在哪些门店使用，长度不超过1000个UTF-8字符（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.coupon_available_store_info.description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，用于展示可用门店列表的详细信息，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。需和 `mini_program_path` 一同填写（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.coupon_available_store_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部展示可用门店列表的详细信息的页面跳转路径，需和 `mini_program_appid` 一同填写（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.coupon_available_store_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockUsageRuleDisplayInfo
{
    /// <summary>【券使用方式列表】 可以配置多种使用方式。可选取值：`OFFLINE` 线下滴码核销，点击券“立即使用”跳转展示券二维码详情（投放计划暂不支持此核销方式） / `MINI_PROGRAM` 线上小程序核销，点击券“立即使用”跳转至配置的商家小程序（需要添加小程序AppID和Path） / `APP` APP核销，在商家APP内核销 / `PAYMENT_CODE` 微信支付付款码核销，用户点击券“立即使用”将跳转至微信支付钱包付款码，服务商扫描付款码后可通过付款码获取到用户openid，通过openid查询并核销用户券，核销后再发起下单，适合线下支付场景（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.coupon_usage_method_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_usage_method_list")]
    public List<string>? CouponUsageMethodList { get; set; }

    /// <summary>【小程序AppID】 品牌方或服务商小程序AppID，可在微信公众平台查看，该小程序与品牌或服务商存在绑定关系。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_path` 一同填写（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方或服务商小程序内部跳转路径。 在 `coupon_usage_method_list` 中包含 `MINI_PROGRAM` 时必填，需和 `mini_program_appid` 一同填写（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【APP的H5跳转路径】 品牌方或服务商APP的H5跳转路径。在 `app_jump_type` 为 `H5` 时必填（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.app_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_path")]
    public string? AppPath { get; set; }

    /// <summary>【券使用说明】 用于说明详细的券规则，长度不超过1000个UTF-8字符（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.usage_description</c>，<b>必填</b>，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("usage_description")]
    public string? UsageDescription { get; set; }

    /// <summary>【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.coupon_available_store_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_store_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockUsageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>user_coupon_list.stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockNotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>user_coupon_list.stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>user_coupon_list.stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>user_coupon_list.stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【批次信息】 该用户商品券发券时使用的批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstock
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>user_coupon_list.stock.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，由微信支付生成（官方字段路径 <c>user_coupon_list.stock.stock_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【备注】 仅配置品牌可见，用于自定义信息（官方字段路径 <c>user_coupon_list.stock.remark</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放；商品券的 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时不可使用本模式（官方字段路径 <c>user_coupon_list.stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>user_coupon_list.stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>user_coupon_list.stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockStockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>user_coupon_list.stock.single_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>user_coupon_list.stock.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockProgressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>user_coupon_list.stock.stock_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockStockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>user_coupon_list.stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockUsageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>user_coupon_list.stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockCouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>user_coupon_list.stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockNotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>user_coupon_list.stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>user_coupon_list.stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstockSentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>user_coupon_list.stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>user_coupon_list.stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>user_coupon_list.stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>user_coupon_list.stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>user_coupon_list.stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemCouponTagInfoMemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>user_coupon_list.coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItemCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>user_coupon_list.coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>user_coupon_list.coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemCouponTagInfoMemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>查询用户商品券列表响应·【用户商品券列表】 指定状态的用户商品券列表信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponseUserCouponListItem
{
    /// <summary>【用户商品券Code】 用户商品券的唯一标识（官方字段路径 <c>user_coupon_list.coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【用户商品券状态】。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间 / `USED` 已核销，用户商品券已核销 / `EXPIRED` 已过期，用户商品券已超过有效期，不再可用 / `DELETED` 已删除，用户主动删除该券 / `DEACTIVATED` 已失效，品牌方主动调用【 [失效用户商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781578.md)】或【 [失效用户商品券组API](https://pay.weixin.qq.com/doc/v3/partner/4016280658.md)】使用户商品券失效（官方字段路径 <c>user_coupon_list.coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【有效期开始时间】 用户商品券可用开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.valid_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【有效期结束时间】 用户商品券可用结束时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.valid_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【领券时间】 用户领券时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】 发券时传入的请求流水号（官方字段路径 <c>user_coupon_list.send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【发券渠道】 描述用户商品券是经由什么渠道发送的。可选取值：`BRAND_MANAGE` 摇一摇有优惠，通过摇一摇有优惠渠道发放 / `API` 服务商自主发券，服务商通过发券接口自主发券到商家名片 / `RECEIVE_COMPONENT` 小程序领券组件，服务商通过小程序领券组件发券 / `MEMBER` 会员，品牌方通过会员相关活动或渠道进行发券 / `STICKER_GIFT` 贴图有礼，用户参与品牌贴图有礼活动后发放（官方字段路径 <c>user_coupon_list.send_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_channel")]
    public string? SendChannel { get; set; }

    /// <summary>【确认请求单号】 品牌方确认发券请求时传入的的请求流水号。当且仅当 品牌方调用【 确认发放用户商品券API】后提供（官方字段路径 <c>user_coupon_list.confirm_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_request_no")]
    public string? ConfirmRequestNo { get; set; }

    /// <summary>【确认发放时间】 品牌方确认发券时间，当且仅当 品牌方调用【 确认发放用户商品券API】后提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.confirm_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>【失效请求单号】 品牌方失效券请求时传入的的请求流水号。当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的请求流水号（官方字段路径 <c>user_coupon_list.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 失效时间，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>user_coupon_list.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 失效券的原因，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的失效原因（官方字段路径 <c>user_coupon_list.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提供（官方字段路径 <c>user_coupon_list.single_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_detail")]
    public ProductCouponUserCouponListResponseUserCouponListItemSingleUsageDetail? SingleUsageDetail { get; set; }

    /// <summary>【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>user_coupon_list.progressive_bundle_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_detail")]
    public ProductCouponUserCouponListResponseUserCouponListItemProgressiveBundleUsageDetail? ProgressiveBundleUsageDetail { get; set; }

    /// <summary>【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>user_coupon_list.user_product_coupon_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_product_coupon_bundle_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserProductCouponBundleInfo? UserProductCouponBundleInfo { get; set; }

    /// <summary>【商品券信息】 该用户商品券对应的商品券详情（官方字段路径 <c>user_coupon_list.product_coupon</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("product_coupon")]
    public ProductCouponUserCouponListResponseUserCouponListItemProductCoupon? ProductCoupon { get; set; }

    /// <summary>【批次信息】 该用户商品券发券时使用的批次详情（官方字段路径 <c>user_coupon_list.stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponUserCouponListResponseUserCouponListItemUserCouponListstock? Stock { get; set; }

    /// <summary>【自定义附加信息】 调用发券接口时品牌方使用 `attach` 字段主动设置的附加信息。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过发券接口发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>user_coupon_list.attach</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【渠道自定义信息】 使用微信支付提供的其他渠道（比如「摇一摇有优惠」）发放商品券时，渠道可能会设置该渠道特定的自定义信息，请根据 `send_channel` 字段判断如何解析本字段。不同渠道的自定义信息格式不同，请根据对应渠道的文档解析（官方字段路径 <c>user_coupon_list.channel_custom_info</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("channel_custom_info")]
    public string? ChannelCustomInfo { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>user_coupon_list.coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponListResponseUserCouponListItemCouponTagInfo? CouponTagInfo { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>user_coupon_list.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>查询用户商品券列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781590"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponListResponse : WechatPayResponse
{
    /// <summary>【总个数】 满足条件的数据总条数，当且仅当 `page_token` 为空时提供（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【用户商品券列表】 指定状态的用户商品券列表信息（官方字段路径 <c>user_coupon_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("user_coupon_list")]
    public List<ProductCouponUserCouponListResponseUserCouponListItem>? UserCouponList { get; set; }

    /// <summary>【下一页Token】 分页查询时，如果还有更多数据，会返回下一页的Token，请在下一次查询时设置在 `page_token` 中；如果已无更多数据，则不返回此字段（官方字段路径 <c>next_page_token</c>，选填，类型 <c>string(100)</c>）。</summary>
    [JsonPropertyName("next_page_token")]
    public string? NextPageToken { get; set; }

}

/// <summary>查询用户商品券详情响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseSingleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券详情响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseSingleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券详情响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseSingleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>single_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>single_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>single_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponQueryResponseSingleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponQueryResponseSingleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>single_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>single_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>查询用户商品券详情响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProgressiveBundleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券详情响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>查询用户商品券详情响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProgressiveBundleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>progressive_bundle_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponQueryResponseProgressiveBundleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponQueryResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>progressive_bundle_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>查询用户商品券详情响应·【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseUserProductCouponBundleInfo
{
    /// <summary>【用户券组ID】 本券所属的用户券组ID，【 向用户发放商品券批次组】时由微信支付生成（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_id")]
    public string? UserCouponBundleId { get; set; }

    /// <summary>【用户券组内索引】 本券在所属用户券组内的序号，从 0 开始编号（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_index")]
    public long? UserCouponBundleIndex { get; set; }

    /// <summary>【总可使用次数】 所属用户券组总计可用次数（官方字段路径 <c>user_product_coupon_bundle_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【已使用次数】 所属用户券组已使用次数（官方字段路径 <c>user_product_coupon_bundle_info.used_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("used_count")]
    public long? UsedCount { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCouponProductCouponsingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCouponProductCouponsingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponQueryResponseProductCouponProductCouponsingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponQueryResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCouponProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCouponProductCoupondisplayInfoComboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>product_coupon.display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponUserCouponQueryResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCouponProductCoupondisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>product_coupon.display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>product_coupon.display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>product_coupon.display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>product_coupon.display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>product_coupon.display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponUserCouponQueryResponseProductCouponProductCoupondisplayInfoComboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>查询用户商品券详情响应·【商品券信息】 该用户商品券对应的商品券详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseProductCoupon
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>product_coupon.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>product_coupon.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>product_coupon.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>product_coupon.single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponUserCouponQueryResponseProductCouponProductCouponsingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>product_coupon.progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponUserCouponQueryResponseProductCouponProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>product_coupon.display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponUserCouponQueryResponseProductCouponProductCoupondisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>product_coupon.out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>product_coupon.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>product_coupon.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>product_coupon.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>product_coupon.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>product_coupon.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>product_coupon.show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>product_coupon.available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockstockSendRule
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

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriod
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

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponQueryResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `P。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockstockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
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

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockusageRuleDisplayInfo
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
    public ProductCouponUserCouponQueryResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponUserCouponQueryResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponUserCouponQueryResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>查询用户商品券详情响应·【批次信息】 该用户商品券发券时使用的批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseStock
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

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放；商品券的 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时不可使用本模式（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponUserCouponQueryResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponUserCouponQueryResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>stock.single_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponUserCouponQueryResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>stock.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponUserCouponQueryResponseStockStockprogressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>stock.stock_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponUserCouponQueryResponseStockStockstockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponUserCouponQueryResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponUserCouponQueryResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponUserCouponQueryResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponUserCouponQueryResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>查询用户商品券详情响应·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseCouponTagInfoCouponTagInfomemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>查询用户商品券详情响应·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponseCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponQueryResponseCouponTagInfoCouponTagInfomemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>查询用户商品券详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781582"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponQueryResponse : WechatPayResponse
{
    /// <summary>【用户商品券Code】 用户商品券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【用户商品券状态】。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间 / `USED` 已核销，用户商品券已核销 / `EXPIRED` 已过期，用户商品券已超过有效期，不再可用 / `DELETED` 已删除，用户主动删除该券 / `DEACTIVATED` 已失效，品牌方主动调用【 [失效用户商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781578.md)】或【 [失效用户商品券组API](https://pay.weixin.qq.com/doc/v3/partner/4016280658.md)】使用户商品券失效（官方字段路径 <c>coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【有效期开始时间】 用户商品券可用开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【有效期结束时间】 用户商品券可用结束时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【领券时间】 用户领券时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】 发券时传入的请求流水号（官方字段路径 <c>send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【发券渠道】 描述用户商品券是经由什么渠道发送的。可选取值：`BRAND_MANAGE` 摇一摇有优惠，通过摇一摇有优惠渠道发放 / `API` 服务商自主发券，服务商通过发券接口自主发券到商家名片 / `RECEIVE_COMPONENT` 小程序领券组件，服务商通过小程序领券组件发券 / `MEMBER` 会员，品牌方通过会员相关活动或渠道进行发券 / `STICKER_GIFT` 贴图有礼，用户参与品牌贴图有礼活动后发放（官方字段路径 <c>send_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_channel")]
    public string? SendChannel { get; set; }

    /// <summary>【确认请求单号】 品牌方确认发券请求时传入的的请求流水号。当且仅当 品牌方调用【 确认发放用户商品券API】后提供（官方字段路径 <c>confirm_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_request_no")]
    public string? ConfirmRequestNo { get; set; }

    /// <summary>【确认发放时间】 品牌方确认发券时间，当且仅当 品牌方调用【 确认发放用户商品券API】后提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>confirm_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>【失效请求单号】 品牌方失效券请求时传入的的请求流水号。当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 失效时间，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 失效券的原因，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提供（官方字段路径 <c>single_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_detail")]
    public ProductCouponUserCouponQueryResponseSingleUsageDetail? SingleUsageDetail { get; set; }

    /// <summary>【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>progressive_bundle_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_detail")]
    public ProductCouponUserCouponQueryResponseProgressiveBundleUsageDetail? ProgressiveBundleUsageDetail { get; set; }

    /// <summary>【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>user_product_coupon_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_product_coupon_bundle_info")]
    public ProductCouponUserCouponQueryResponseUserProductCouponBundleInfo? UserProductCouponBundleInfo { get; set; }

    /// <summary>【商品券信息】 该用户商品券对应的商品券详情（官方字段路径 <c>product_coupon</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("product_coupon")]
    public ProductCouponUserCouponQueryResponseProductCoupon? ProductCoupon { get; set; }

    /// <summary>【批次信息】 该用户商品券发券时使用的批次详情（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponUserCouponQueryResponseStock? Stock { get; set; }

    /// <summary>【自定义附加信息】 调用发券接口时品牌方使用 `attach` 字段主动设置的附加信息。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过发券接口发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>attach</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【渠道自定义信息】 使用微信支付提供的其他渠道（比如「摇一摇有优惠」）发放商品券时，渠道可能会设置该渠道特定的自定义信息，请根据 `send_channel` 字段判断如何解析本字段。不同渠道的自定义信息格式不同，请根据对应渠道的文档解析（官方字段路径 <c>channel_custom_info</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("channel_custom_info")]
    public string? ChannelCustomInfo { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponQueryResponseCouponTagInfo? CouponTagInfo { get; set; }

}

/// <summary>确认用户商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmRequest
{
    /// <summary>【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 `product_coupon_id` 对应的商品券（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后在服务商平台中绑定，详见 服务商商户号与AppID账号关联管理（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【确认发放请求单号】 品牌确认发放用户商品券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>确认用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseSingleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>确认用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseSingleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>确认用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseSingleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>single_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>single_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>single_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponConfirmResponseSingleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponConfirmResponseSingleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>single_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>single_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>确认用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProgressiveBundleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>确认用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>确认用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProgressiveBundleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>progressive_bundle_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponConfirmResponseProgressiveBundleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponConfirmResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>progressive_bundle_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>确认用户商品券响应·【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseUserProductCouponBundleInfo
{
    /// <summary>【用户券组ID】 本券所属的用户券组ID，【 向用户发放商品券批次组】时由微信支付生成（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_id")]
    public string? UserCouponBundleId { get; set; }

    /// <summary>【用户券组内索引】 本券在所属用户券组内的序号，从 0 开始编号（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_index")]
    public long? UserCouponBundleIndex { get; set; }

    /// <summary>【总可使用次数】 所属用户券组总计可用次数（官方字段路径 <c>user_product_coupon_bundle_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【已使用次数】 所属用户券组已使用次数（官方字段路径 <c>user_product_coupon_bundle_info.used_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("used_count")]
    public long? UsedCount { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCouponSingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCouponSingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCouponSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponConfirmResponseProductCouponSingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponConfirmResponseProductCouponSingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCouponProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCouponProductCoupondisplayInfoComboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>product_coupon.display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponUserCouponConfirmResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCouponProductCoupondisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>product_coupon.display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>product_coupon.display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>product_coupon.display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>product_coupon.display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>product_coupon.display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponUserCouponConfirmResponseProductCouponProductCoupondisplayInfoComboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>确认用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseProductCoupon
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>product_coupon.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>product_coupon.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>product_coupon.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>product_coupon.single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponUserCouponConfirmResponseProductCouponSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>product_coupon.progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponUserCouponConfirmResponseProductCouponProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>product_coupon.display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponUserCouponConfirmResponseProductCouponProductCoupondisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>product_coupon.out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>product_coupon.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>product_coupon.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>product_coupon.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>product_coupon.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>product_coupon.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>product_coupon.show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>product_coupon.available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockstockSendRule
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

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriod
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

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponConfirmResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `P。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockstockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
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

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockusageRuleDisplayInfo
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
    public ProductCouponUserCouponConfirmResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponUserCouponConfirmResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponUserCouponConfirmResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>确认用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseStock
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

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放；商品券的 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时不可使用本模式（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponUserCouponConfirmResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponUserCouponConfirmResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>stock.single_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponUserCouponConfirmResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>stock.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponUserCouponConfirmResponseStockStockprogressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>stock.stock_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponUserCouponConfirmResponseStockStockstockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponUserCouponConfirmResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponUserCouponConfirmResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponUserCouponConfirmResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponUserCouponConfirmResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>确认用户商品券响应·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseCouponTagInfoCouponTagInfomemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>确认用户商品券响应·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponseCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponConfirmResponseCouponTagInfoCouponTagInfomemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>确认用户商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781575"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponConfirmResponse : WechatPayResponse
{
    /// <summary>【用户商品券Code】 用户商品券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【用户商品券状态】。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间 / `USED` 已核销，用户商品券已核销 / `EXPIRED` 已过期，用户商品券已超过有效期，不再可用 / `DELETED` 已删除，用户主动删除该券 / `DEACTIVATED` 已失效，品牌方主动调用【 [失效用户商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781578.md)】或【 [失效用户商品券组API](https://pay.weixin.qq.com/doc/v3/partner/4016280658.md)】使用户商品券失效（官方字段路径 <c>coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【有效期开始时间】 用户商品券可用开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【有效期结束时间】 用户商品券可用结束时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【领券时间】 用户领券时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】 发券时传入的请求流水号（官方字段路径 <c>send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【发券渠道】 描述用户商品券是经由什么渠道发送的。可选取值：`BRAND_MANAGE` 摇一摇有优惠，通过摇一摇有优惠渠道发放 / `API` 服务商自主发券，服务商通过发券接口自主发券到商家名片 / `RECEIVE_COMPONENT` 小程序领券组件，服务商通过小程序领券组件发券 / `MEMBER` 会员，品牌方通过会员相关活动或渠道进行发券 / `STICKER_GIFT` 贴图有礼，用户参与品牌贴图有礼活动后发放（官方字段路径 <c>send_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_channel")]
    public string? SendChannel { get; set; }

    /// <summary>【确认请求单号】 品牌方确认发券请求时传入的的请求流水号。当且仅当 品牌方调用【 确认发放用户商品券API】后提供（官方字段路径 <c>confirm_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_request_no")]
    public string? ConfirmRequestNo { get; set; }

    /// <summary>【确认发放时间】 品牌方确认发券时间，当且仅当 品牌方调用【 确认发放用户商品券API】后提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>confirm_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>【失效请求单号】 品牌方失效券请求时传入的的请求流水号。当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 失效时间，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 失效券的原因，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提供（官方字段路径 <c>single_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_detail")]
    public ProductCouponUserCouponConfirmResponseSingleUsageDetail? SingleUsageDetail { get; set; }

    /// <summary>【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>progressive_bundle_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_detail")]
    public ProductCouponUserCouponConfirmResponseProgressiveBundleUsageDetail? ProgressiveBundleUsageDetail { get; set; }

    /// <summary>【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>user_product_coupon_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_product_coupon_bundle_info")]
    public ProductCouponUserCouponConfirmResponseUserProductCouponBundleInfo? UserProductCouponBundleInfo { get; set; }

    /// <summary>【商品券信息】 该用户商品券对应的商品券详情（官方字段路径 <c>product_coupon</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("product_coupon")]
    public ProductCouponUserCouponConfirmResponseProductCoupon? ProductCoupon { get; set; }

    /// <summary>【批次信息】 该用户商品券发券时使用的批次详情（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponUserCouponConfirmResponseStock? Stock { get; set; }

    /// <summary>【自定义附加信息】 调用发券接口时品牌方使用 `attach` 字段主动设置的附加信息。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过发券接口发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>attach</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【渠道自定义信息】 使用微信支付提供的其他渠道（比如「摇一摇有优惠」）发放商品券时，渠道可能会设置该渠道特定的自定义信息，请根据 `send_channel` 字段判断如何解析本字段。不同渠道的自定义信息格式不同，请根据对应渠道的文档解析（官方字段路径 <c>channel_custom_info</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("channel_custom_info")]
    public string? ChannelCustomInfo { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponConfirmResponseCouponTagInfo? CouponTagInfo { get; set; }

}

/// <summary>退用户商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnRequest
{
    /// <summary>【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 `product_coupon_id` 对应的商品券（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后与品牌方或服务商绑定（详见 服务商商户号与AppID账号关联管理）（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【退券请求单号】 品牌退用户商品券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>退用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseSingleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>退用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseSingleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>退用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseSingleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>single_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>single_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>single_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponReturnResponseSingleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponReturnResponseSingleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>single_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>single_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>退用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProgressiveBundleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>退用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>退用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProgressiveBundleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>progressive_bundle_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponReturnResponseProgressiveBundleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponReturnResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>progressive_bundle_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>退用户商品券响应·【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseUserProductCouponBundleInfo
{
    /// <summary>【用户券组ID】 本券所属的用户券组ID，【 向用户发放商品券批次组】时由微信支付生成（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_id")]
    public string? UserCouponBundleId { get; set; }

    /// <summary>【用户券组内索引】 本券在所属用户券组内的序号，从 0 开始编号（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_index")]
    public long? UserCouponBundleIndex { get; set; }

    /// <summary>【总可使用次数】 所属用户券组总计可用次数（官方字段路径 <c>user_product_coupon_bundle_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【已使用次数】 所属用户券组已使用次数（官方字段路径 <c>user_product_coupon_bundle_info.used_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("used_count")]
    public long? UsedCount { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCouponProductCouponsingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCouponProductCouponsingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponReturnResponseProductCouponProductCouponsingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponReturnResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCouponProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCouponProductCoupondisplayInfoComboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>product_coupon.display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponUserCouponReturnResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCouponProductCoupondisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>product_coupon.display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>product_coupon.display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>product_coupon.display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>product_coupon.display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>product_coupon.display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponUserCouponReturnResponseProductCouponProductCoupondisplayInfoComboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>退用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseProductCoupon
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>product_coupon.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>product_coupon.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>product_coupon.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>product_coupon.single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponUserCouponReturnResponseProductCouponProductCouponsingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>product_coupon.progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponUserCouponReturnResponseProductCouponProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>product_coupon.display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponUserCouponReturnResponseProductCouponProductCoupondisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>product_coupon.out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>product_coupon.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>product_coupon.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>product_coupon.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>product_coupon.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>product_coupon.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>product_coupon.show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>product_coupon.available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockstockSendRule
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

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriod
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

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponReturnResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `P。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockstockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
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

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockusageRuleDisplayInfo
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
    public ProductCouponUserCouponReturnResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponUserCouponReturnResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponUserCouponReturnResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>退用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseStock
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

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放；商品券的 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时不可使用本模式（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponUserCouponReturnResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponUserCouponReturnResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>stock.single_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponUserCouponReturnResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>stock.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponUserCouponReturnResponseStockStockprogressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>stock.stock_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponUserCouponReturnResponseStockStockstockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponUserCouponReturnResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponUserCouponReturnResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponUserCouponReturnResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponUserCouponReturnResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>退用户商品券响应·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseCouponTagInfoCouponTagInfomemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>退用户商品券响应·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponseCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponReturnResponseCouponTagInfoCouponTagInfomemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>退用户商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781599"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponReturnResponse : WechatPayResponse
{
    /// <summary>【用户商品券Code】 用户商品券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【用户商品券状态】。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间 / `USED` 已核销，用户商品券已核销 / `EXPIRED` 已过期，用户商品券已超过有效期，不再可用 / `DELETED` 已删除，用户主动删除该券 / `DEACTIVATED` 已失效，品牌方主动调用【 [失效用户商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781578.md)】或【 [失效用户商品券组API](https://pay.weixin.qq.com/doc/v3/partner/4016280658.md)】使用户商品券失效（官方字段路径 <c>coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【有效期开始时间】 用户商品券可用开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【有效期结束时间】 用户商品券可用结束时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【领券时间】 用户领券时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】 发券时传入的请求流水号（官方字段路径 <c>send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【发券渠道】 描述用户商品券是经由什么渠道发送的。可选取值：`BRAND_MANAGE` 摇一摇有优惠，通过摇一摇有优惠渠道发放 / `API` 服务商自主发券，服务商通过发券接口自主发券到商家名片 / `RECEIVE_COMPONENT` 小程序领券组件，服务商通过小程序领券组件发券 / `MEMBER` 会员，品牌方通过会员相关活动或渠道进行发券 / `STICKER_GIFT` 贴图有礼，用户参与品牌贴图有礼活动后发放（官方字段路径 <c>send_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_channel")]
    public string? SendChannel { get; set; }

    /// <summary>【确认请求单号】 品牌方确认发券请求时传入的的请求流水号。当且仅当 品牌方调用【 确认发放用户商品券API】后提供（官方字段路径 <c>confirm_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_request_no")]
    public string? ConfirmRequestNo { get; set; }

    /// <summary>【确认发放时间】 品牌方确认发券时间，当且仅当 品牌方调用【 确认发放用户商品券API】后提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>confirm_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>【失效请求单号】 品牌方失效券请求时传入的的请求流水号。当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 失效时间，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 失效券的原因，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提供（官方字段路径 <c>single_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_detail")]
    public ProductCouponUserCouponReturnResponseSingleUsageDetail? SingleUsageDetail { get; set; }

    /// <summary>【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>progressive_bundle_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_detail")]
    public ProductCouponUserCouponReturnResponseProgressiveBundleUsageDetail? ProgressiveBundleUsageDetail { get; set; }

    /// <summary>【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>user_product_coupon_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_product_coupon_bundle_info")]
    public ProductCouponUserCouponReturnResponseUserProductCouponBundleInfo? UserProductCouponBundleInfo { get; set; }

    /// <summary>【商品券信息】 该用户商品券对应的商品券详情（官方字段路径 <c>product_coupon</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("product_coupon")]
    public ProductCouponUserCouponReturnResponseProductCoupon? ProductCoupon { get; set; }

    /// <summary>【批次信息】 该用户商品券发券时使用的批次详情（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponUserCouponReturnResponseStock? Stock { get; set; }

    /// <summary>【自定义附加信息】 调用发券接口时品牌方使用 `attach` 字段主动设置的附加信息。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过发券接口发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>attach</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【渠道自定义信息】 使用微信支付提供的其他渠道（比如「摇一摇有优惠」）发放商品券时，渠道可能会设置该渠道特定的自定义信息，请根据 `send_channel` 字段判断如何解析本字段。不同渠道的自定义信息格式不同，请根据对应渠道的文档解析（官方字段路径 <c>channel_custom_info</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("channel_custom_info")]
    public string? ChannelCustomInfo { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponReturnResponseCouponTagInfo? CouponTagInfo { get; set; }

}

/// <summary>核销用户商品券请求·【关联微信支付订单信息】 券核销对应的微信支付订单信息 注: 1、订单信息必须且。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseRequestAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>核销用户商品券请求·【关联微信支付分订单信息】 券核销对应的关联微信支付分订单信息 注: 1、订单信。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseRequestAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>核销用户商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseRequest
{
    /// <summary>【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 `product_coupon_id` 对应的商品券（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后与品牌方或服务商绑定（详见 服务商商户号与AppID账号关联管理）（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【核销时间】 用户商品券的核销时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>use_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【核销请求单号】 品牌核销用户商品券请求的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【核销门店ID】 核销本用户券的门店ID，门店的唯一标识，调用【 创建品牌门店API】时由微信支付生成。门店归属品牌需与批次归属品牌相同，且当前门店状态为营业中（官方字段路径 <c>store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【关联微信支付订单信息】 券核销对应的微信支付订单信息 注: 1、订单信息必须且只能传入 `associated_order_info` 与 `associated_pay_score_order_info` 之一 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponUseRequestAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【关联微信支付分订单信息】 券核销对应的关联微信支付分订单信息 注: 1、订单信息必须且只能传入 `associated_order_info` 与 `associated_pay_score_order_info` 之一 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponUseRequestAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分，不小于0 注： 该券为 `满减券` 时，优惠金额应为减免金额； 该券为 `单品兑换券` 时，优惠金额应为商品原价减兑换价格； 该券为 `单品折扣券` 时，优惠金额应为商品原价乘以减免百分比； 该券为 `全场/品类折扣券`（scope= `ALL` 或 `CATEGORY`）时，优惠金额应为订单中通过本券优惠抵扣的金额（需要考虑券实际使用范围、折扣上限等限制条件，例如订单100元，其中参与折扣的部分是50元，8…（官方字段路径 <c>saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 本次核销对应的商品编码。商户可自行选择传入，传入的商品编码需在批次可用商品编码列表中，否则将核销失败（官方字段路径 <c>use_product_no_list</c>，选填，类型 <c>array[string(20)]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>核销用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseSingleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>核销用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseSingleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>核销用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseSingleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>single_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>single_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>single_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponUseResponseSingleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponUseResponseSingleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>single_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>single_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>核销用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProgressiveBundleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>核销用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>核销用户商品券响应·【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProgressiveBundleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>progressive_bundle_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>progressive_bundle_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>progressive_bundle_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponUseResponseProgressiveBundleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>progressive_bundle_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponUseResponseProgressiveBundleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>progressive_bundle_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>progressive_bundle_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>核销用户商品券响应·【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseUserProductCouponBundleInfo
{
    /// <summary>【用户券组ID】 本券所属的用户券组ID，【 向用户发放商品券批次组】时由微信支付生成（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_id")]
    public string? UserCouponBundleId { get; set; }

    /// <summary>【用户券组内索引】 本券在所属用户券组内的序号，从 0 开始编号（官方字段路径 <c>user_product_coupon_bundle_info.user_coupon_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("user_coupon_bundle_index")]
    public long? UserCouponBundleIndex { get; set; }

    /// <summary>【总可使用次数】 所属用户券组总计可用次数（官方字段路径 <c>user_product_coupon_bundle_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【已使用次数】 所属用户券组已使用次数（官方字段路径 <c>user_product_coupon_bundle_info.used_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("used_count")]
    public long? UsedCount { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCouponProductCouponsingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCouponProductCouponsingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponUseResponseProductCouponProductCouponsingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponUseResponseProductCouponProductCouponsingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCouponProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCouponProductCoupondisplayInfoComboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>product_coupon.display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponUserCouponUseResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCouponProductCoupondisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>product_coupon.display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>product_coupon.display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>product_coupon.display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>product_coupon.display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>product_coupon.display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponUserCouponUseResponseProductCouponProductCoupondisplayInfoComboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>核销用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseProductCoupon
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>product_coupon.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>product_coupon.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>product_coupon.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>product_coupon.single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponUserCouponUseResponseProductCouponProductCouponsingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>product_coupon.progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponUserCouponUseResponseProductCouponProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>product_coupon.display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponUserCouponUseResponseProductCouponProductCoupondisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>product_coupon.out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>product_coupon.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>product_coupon.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>product_coupon.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>product_coupon.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>product_coupon.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>product_coupon.show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>product_coupon.available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockstockSendRule
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

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriod
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

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponUseResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponUseResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponUseResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponUseResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod
{
    /// <summary>【批次开始时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 距离当前创建批次的时间不能超过90天，批次有…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【批次结束时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 批次有效期最长不超过366天（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【券生效后N天内可用】 日期区间内，券生效后 N 天内有效，最多365天。例如： 生效当天内有效填 1 生效后2天内有效填 2 依此类推…… 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数，无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 可配合 `wait_days_after_receive` 一同填写，也可单独填写。 注：不可与 `irregular_available_per…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_days")]
    public long? AvailableDays { get; set; }

    /// <summary>【领券后需等待N天才生效】 日期区间内，用户领券后需等待 N 天才开始生效，最多30天。例如： 领券后当天开始生效则无需填写 领券后第二天开始生效则填写 1 用户在有效期开始前领取商品券，则从有效期第1天开始计算天数，用户在有效期内领取商品券，则从领取当天开始计算天数。无论用户何时领取商品券，商品券在活动有效期结束后均不可用。 需配合 `available_days` 一同填写，不可单独填写。 注：不可与 `irregular_available_peri…（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESS。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.progressive_bundle_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 时必填，其他类型不应填写\`（官方字段路径 <c>stock.progressive_bundle_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 时必填，其他类型不应填写（官方字段路径 <c>stock.progressive_bundle_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `P。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockstockBundleInfo
{
    /// <summary>【批次所属批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("stock_bundle_id")]
    public string? StockBundleId { get; set; }

    /// <summary>【批次在批次组内的次序】 本批次在批次组内的次序，从0开始（官方字段路径 <c>stock.stock_bundle_info.stock_bundle_index</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("stock_bundle_index")]
    public long? StockBundleIndex { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
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

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockusageRuleDisplayInfo
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
    public ProductCouponUserCouponUseResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponUserCouponUseResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponUserCouponUseResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>核销用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseStock
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

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放；商品券的 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时不可使用本模式（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponUserCouponUseResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponUserCouponUseResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>stock.single_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponUserCouponUseResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【多次优惠使用规则】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他场景不提供（官方字段路径 <c>stock.progressive_bundle_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_rule")]
    public ProductCouponUserCouponUseResponseStockStockprogressiveBundleUsageRule? ProgressiveBundleUsageRule { get; set; }

    /// <summary>【批次组信息】 批次所在批次组信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>stock.stock_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_bundle_info")]
    public ProductCouponUserCouponUseResponseStockStockstockBundleInfo? StockBundleInfo { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponUserCouponUseResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponUserCouponUseResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponUserCouponUseResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponUserCouponUseResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>核销用户商品券响应·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseCouponTagInfoCouponTagInfomemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>核销用户商品券响应·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponseCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponUseResponseCouponTagInfoCouponTagInfomemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>核销用户商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781608"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponUseResponse : WechatPayResponse
{
    /// <summary>【用户商品券Code】 用户商品券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【用户商品券状态】。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间 / `USED` 已核销，用户商品券已核销 / `EXPIRED` 已过期，用户商品券已超过有效期，不再可用 / `DELETED` 已删除，用户主动删除该券 / `DEACTIVATED` 已失效，品牌方主动调用【 [失效用户商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781578.md)】或【 [失效用户商品券组API](https://pay.weixin.qq.com/doc/v3/partner/4016280658.md)】使用户商品券失效（官方字段路径 <c>coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【有效期开始时间】 用户商品券可用开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【有效期结束时间】 用户商品券可用结束时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【领券时间】 用户领券时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】 发券时传入的请求流水号（官方字段路径 <c>send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【发券渠道】 描述用户商品券是经由什么渠道发送的。可选取值：`BRAND_MANAGE` 摇一摇有优惠，通过摇一摇有优惠渠道发放 / `API` 服务商自主发券，服务商通过发券接口自主发券到商家名片 / `RECEIVE_COMPONENT` 小程序领券组件，服务商通过小程序领券组件发券 / `MEMBER` 会员，品牌方通过会员相关活动或渠道进行发券 / `STICKER_GIFT` 贴图有礼，用户参与品牌贴图有礼活动后发放（官方字段路径 <c>send_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_channel")]
    public string? SendChannel { get; set; }

    /// <summary>【确认请求单号】 品牌方确认发券请求时传入的的请求流水号。当且仅当 品牌方调用【 确认发放用户商品券API】后提供（官方字段路径 <c>confirm_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_request_no")]
    public string? ConfirmRequestNo { get; set; }

    /// <summary>【确认发放时间】 品牌方确认发券时间，当且仅当 品牌方调用【 确认发放用户商品券API】后提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>confirm_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>【失效请求单号】 品牌方失效券请求时传入的的请求流水号。当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 失效时间，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 失效券的原因，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提供（官方字段路径 <c>single_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_detail")]
    public ProductCouponUserCouponUseResponseSingleUsageDetail? SingleUsageDetail { get; set; }

    /// <summary>【多次优惠使用详情】 当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>progressive_bundle_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_detail")]
    public ProductCouponUserCouponUseResponseProgressiveBundleUsageDetail? ProgressiveBundleUsageDetail { get; set; }

    /// <summary>【用户券组信息】 当前用户券所属用户券组的信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供（官方字段路径 <c>user_product_coupon_bundle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_product_coupon_bundle_info")]
    public ProductCouponUserCouponUseResponseUserProductCouponBundleInfo? UserProductCouponBundleInfo { get; set; }

    /// <summary>【商品券信息】 该用户商品券对应的商品券详情（官方字段路径 <c>product_coupon</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("product_coupon")]
    public ProductCouponUserCouponUseResponseProductCoupon? ProductCoupon { get; set; }

    /// <summary>【批次信息】 该用户商品券发券时使用的批次详情（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponUserCouponUseResponseStock? Stock { get; set; }

    /// <summary>【自定义附加信息】 调用发券接口时品牌方使用 `attach` 字段主动设置的附加信息。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过发券接口发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>attach</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【渠道自定义信息】 使用微信支付提供的其他渠道（比如「摇一摇有优惠」）发放商品券时，渠道可能会设置该渠道特定的自定义信息，请根据 `send_channel` 字段判断如何解析本字段。不同渠道的自定义信息格式不同，请根据对应渠道的文档解析（官方字段路径 <c>channel_custom_info</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("channel_custom_info")]
    public string? ChannelCustomInfo { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponUseResponseCouponTagInfo? CouponTagInfo { get; set; }

}

/// <summary>作废用户商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateRequest
{
    /// <summary>【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 `product_coupon_id` 对应的商品券（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后与品牌方或服务商绑定（详见 服务商商户号与AppID账号关联管理）（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【失效请求单号】 品牌失效用户商品券的请求流水号，品牌侧需保持唯一性，可使用 数字、大小写字母、下划线 `_`、短横线 `-` 组成，长度在6-40个字符之间（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【失效原因】 记录用户商品券的失效原因，长度不超过150个UTF-8字符（官方字段路径 <c>deactivate_reason</c>，<b>必填</b>，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>作废用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseSingleUsageDetailAssociatedOrderInfo
{
    /// <summary>【微信支付单号】 由微信支付生成的支付单唯一标识，可以唯一定位到一笔微信支付单。 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.transaction_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户订单号】 在微信支付支付时传入的商户订单号，在商户侧具有唯一性，因此提供此字段时应同时提供下单商户号： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_order_info.out_trade_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商户号】 当提供 `out_trade_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_trade_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_trade_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>作废用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseSingleUsageDetailAssociatedPayScoreOrderInfo
{
    /// <summary>【微信支付服务订单号】 支付分订单在微信侧的唯一标识。 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.order_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【商户服务订单号】 商户系统内部服务订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124; 且在同一个商户号下唯一。： 如果此单为服务商模式下单，则需提供 父商户号 `mchid` 以及 子商户号 `sub_mchid` 如果此单为直连商户模式下单，则需提供 直连商户号 `mchid` 注：订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微信支付服务单（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.out_order_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>【商户号】 当提供 `out_order_no` 时此字段必传，否则可以省略。参考如下规则传入： 若为服务商模式，则此字段为服务商商户号 若为直连商户模式，则此字段为商户号（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 当提供 `out_order_no` 时，参考如下规则传入： 若为服务商模式，则此字段必填，为子商户号 若为直连商户模式，则此字段为空 不提供 `out_order_no` 时可忽略此字段（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>作废用户商品券响应·【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseSingleUsageDetail
{
    /// <summary>【券核销请求单号】 券核销的请求流水号，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供（官方字段路径 <c>single_usage_detail.use_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_request_no")]
    public string? UseRequestNo { get; set; }

    /// <summary>【券核销时间】 券被核销的时间，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.use_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>【退券请求单号】 品牌退券时传入的请求流水号，当且仅当券发生了退回后提供此字段（官方字段路径 <c>single_usage_detail.return_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_request_no")]
    public string? ReturnRequestNo { get; set; }

    /// <summary>【退券时间】 券被退回的时间，当且仅当券发生了退回后提供此字段。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>single_usage_detail.return_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>【券核销的微信支付订单信息】 券核销对应的微信支付订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `transaction_id` 与 `out_trade_no` 之一，亦可同时传入，同时传入时应保证二者指向同一…（官方字段路径 <c>single_usage_detail.associated_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_order_info")]
    public ProductCouponUserCouponDeactivateResponseSingleUsageDetailAssociatedOrderInfo? AssociatedOrderInfo { get; set; }

    /// <summary>【券核销的关联微信支付分订单信息】 券核销的关联微信支付分订单信息，当且仅当用户商品券状态 `coupon_state` 为 `USED` 时提供, 注: 1、 `associated_order_info` 与 `associated_pay_score_order_info` 会根据核销请求时的传入参数决定返回对应字段 2、订单信息必须至少传入 `order_id` 与 `out_order_no` 之一，亦可同时传入，同时传入时应保证二者指向同一笔微…（官方字段路径 <c>single_usage_detail.associated_pay_score_order_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("associated_pay_score_order_info")]
    public ProductCouponUserCouponDeactivateResponseSingleUsageDetailAssociatedPayScoreOrderInfo? AssociatedPayScoreOrderInfo { get; set; }

    /// <summary>【优惠金额】 使用本券的实际优惠金额，单位为分 注： 当本券为全场/品类折扣券（scope= `ALL` 或 `CATEGORY`）时，提供的是传入的值（官方字段路径 <c>single_usage_detail.saved_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("saved_amount")]
    public long? SavedAmount { get; set; }

    /// <summary>【核销商品编码列表】 核销商品编码列表 注： 当该券优惠范围为单品或品类且商户核销时传入的商品编码列表正确时提供（官方字段路径 <c>single_usage_detail.use_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("use_product_no_list")]
    public List<string>? UseProductNoList { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCouponSingleUsageInfoNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN·【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `typ。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCouponSingleUsageInfoDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SIN。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCouponSingleUsageInfo
{
    /// <summary>【满减券使用规则】 本商品券内所有批次均以此规则提供满减优惠，当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponDeactivateResponseProductCouponSingleUsageInfoNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 本商品券内所有批次均以此规则提供折扣优惠，当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `ALL` 或 `CATEGORY` 时必填，其他类型不应填写（官方字段路径 <c>product_coupon.single_usage_info.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponDeactivateResponseProductCouponSingleUsageInfoDiscountCoupon? DiscountCoupon { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCouponProgressiveBundleUsageInfo
{
    /// <summary>【可使用次数】 多次优惠领取后用户可使用次数，最少3次，最多15次（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【多次优惠使用间隔天数】 多次优惠多次使用之间需要间隔的天数，最高7天。例如： 间隔 `0` 天表示不限制，使用后当天仍然可以继续使用下一次优惠 间隔 `1` 天表示使用后当天不再可用，下一次优惠需要等到次日 00:00:00 才能使用 间隔 `2` 天表示使用后当天和次日不再可用，下一次优惠需要等到第三天 00:00:00 才能使用 依此类推…… 默认情况下为 `0` 天，即不限制，使用后当天仍然可以继续使用下一次优惠（官方字段路径 <c>product_coupon.progressive_bundle_usage_info.interval_days</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("interval_days")]
    public long? IntervalDays { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，·【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem
{
    /// <summary>【单品名称】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【单品价格】 单位为分，必须大于0（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【单品数量】 最多99个（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>【单品图片】 单品图片URL地址，仅支持通过【 图片上传API】获取的图片URL地址，接入领券组件和核销组件的品牌方建议填写。 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.image_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【单品跳转小程序AppID】 品牌方或服务商可展示单品的小程序AppID，小程序需与品牌方或服务商存在绑定关系，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_path` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【单品跳转小程序路径】 单品展示信息在小程序内的跳转路径，接入领券组件和核销组件的品牌方或服务商建议填写。需和 `mini_program_appid` 一同填写（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list.mini_program_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息·【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCouponProductCoupondisplayInfoComboPackageListItem
{
    /// <summary>【适用商品名】 长度不超过15个UTF-8字符（官方字段路径 <c>product_coupon.display_info.combo_package_list.name</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【可选单品数量】 用户可以从 `choice_list` 中选择的选项数量，例如 10 选 3，则这里填 3（官方字段路径 <c>product_coupon.display_info.combo_package_list.pick_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pick_count")]
    public long? PickCount { get; set; }

    /// <summary>【单品列表】 套餐内包含的选项列表，记录每个单品的信息，最多99个单品，且不能少于设置的 `pick_count` 数量（官方字段路径 <c>product_coupon.display_info.combo_package_list.choice_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("choice_list")]
    public List<ProductCouponUserCouponDeactivateResponseProductCouponProductCoupondisplayInfoComboPackageListItemChoiceListItem>? ChoiceList { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情·【展示信息】 商品券展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCouponProductCoupondisplayInfo
{
    /// <summary>【商品券名称】 商品券名称，长度为3-25个UTF-8字符（官方字段路径 <c>product_coupon.display_info.name</c>，<b>必填</b>，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商品券图片】 商品图片的链接地址，仅支持通过【 图片上传API】获取的图片URL地址。 请参考 商品券图片设计规范 进行设计 要求图片宽高比1:1，建议尺寸1080\*1080 大小不超过2M 建议图片格式：透明PNG格式 需要注意保留透明边距（上下左右各120px）（官方字段路径 <c>product_coupon.display_info.image_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>【商品券背景图】 商品背景图片的URL地址，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式 需要注意保留「商品名称」与「商品图」的安全区域（官方字段路径 <c>product_coupon.display_info.background_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_url")]
    public string? BackgroundUrl { get; set; }

    /// <summary>【商品券详情图列表】 商品详情图URL地址列表，用于最多可上传8张图片，仅支持通过【 图片上传API】获取的图片URL地址。 要求图片宽高比65:77，建议尺寸1170\*1326 大小不超过2M 建议图片格式：JPG/PNG格式（官方字段路径 <c>product_coupon.display_info.detail_image_url_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("detail_image_url_list")]
    public List<string>? DetailImageUrlList { get; set; }

    /// <summary>【商品原价】 单位为分，用于对外展示，最大为 10000000。不填写时默认为 0（官方字段路径 <c>product_coupon.display_info.original_price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>【适用商品组合】 当 `scope` 为 `SINGLE` 必填，用于对外展示，其他优惠范围选填。最多配置 50 个组合（官方字段路径 <c>product_coupon.display_info.combo_package_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("combo_package_list")]
    public List<ProductCouponUserCouponDeactivateResponseProductCouponProductCoupondisplayInfoComboPackageListItem>? ComboPackageList { get; set; }

}

/// <summary>作废用户商品券响应·【商品券信息】 该用户商品券对应的商品券详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseProductCoupon
{
    /// <summary>【商品券ID】 商品券的唯一标识，由微信支付生成（官方字段路径 <c>product_coupon.product_coupon_id</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【优惠范围】 商品券优惠范围。可选取值：`ALL` 全场商品可用券，此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT` / `SINGLE` 部分商品可用券，此时券类型 `type` 配置不受限制，即可配置为 `NORMAL`、 `DISCOUNT` 或 `EXCHANGE` / `CATEGORY` 品类券。适用于商户对某一品类商品提供优惠的场景；系统侧不维护商品/品类清单，具体可用商品范围由商户在 `display_info` 描述、图片等字段中向用户告知，并在核销时由商户自行判定。此时券类型 `type` 仅可配置为 `NORMAL` 或 `DISCOUNT`（官方字段路径 <c>product_coupon.scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【商品券类型】 商品券的优惠类型。可选取值：`NORMAL` 满减券 / `DISCOUNT` 折扣券 / `EXCHANGE` 兑换券，仅在 `scope` 为 `SINGLE` 时可配置（官方字段路径 <c>product_coupon.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【使用模式】 商品券使用模式。可选取值：`SINGLE` 单券，即用户只能使用一次，使用后券失效 / `PROGRESSIVE_BUNDLE` 多次优惠，由一组批次组成，每阶梯次序对应一个批次。用户按顺序使用，每次核销后发放下一张券，直到用完为止（官方字段路径 <c>product_coupon.usage_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("usage_mode")]
    public string? UsageMode { get; set; }

    /// <summary>【单券模式信息】 单券模式配置信息，仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>product_coupon.single_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_info")]
    public ProductCouponUserCouponDeactivateResponseProductCouponSingleUsageInfo? SingleUsageInfo { get; set; }

    /// <summary>【多次优惠模式信息】 多次优惠模式配置信息，当且仅当 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时提供，其他模式不提供（官方字段路径 <c>product_coupon.progressive_bundle_usage_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("progressive_bundle_usage_info")]
    public ProductCouponUserCouponDeactivateResponseProductCouponProgressiveBundleUsageInfo? ProgressiveBundleUsageInfo { get; set; }

    /// <summary>【展示信息】 商品券展示信息（官方字段路径 <c>product_coupon.display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ProductCouponUserCouponDeactivateResponseProductCouponProductCoupondisplayInfo? DisplayInfo { get; set; }

    /// <summary>【外部商品ID】 商户创建商品券时主动传入的外部商品ID，原样返回（官方字段路径 <c>product_coupon.out_product_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_product_no")]
    public string? OutProductNo { get; set; }

    /// <summary>【商品券状态】 商品券状态。可选取值：`AUDITING` 审批中，审批完成前商品券不可用 / `EFFECTIVE` 生效中，商品券已生效，可以正常使用 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使商品券失效（官方字段路径 <c>product_coupon.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的请求流水号（官方字段路径 <c>product_coupon.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>product_coupon.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券API】时传入的失效原因（官方字段路径 <c>product_coupon.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>product_coupon.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【是否展示兑换价】 是否按兑换价展示商品券。 `true` 表示 C 端以「商品原价 − 满减金额」展示兑换价。仅当开关为 `true` 时返回本字段；为 `false` 或未设置时不返回，商户应按未开启（按满减金额展示）处理（官方字段路径 <c>product_coupon.show_as_exchange</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_as_exchange")]
    public bool? ShowAsExchange { get; set; }

    /// <summary>【可用商品编码列表】 商户创建商品券时主动传入的可用商品编码，原样返回（官方字段路径 <c>product_coupon.available_product_no_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_product_no_list")]
    public List<string>? AvailableProductNoList { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mod。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStockcouponCodeCountInfo
{
    /// <summary>【已上传的Code总数】 品牌方为此批次已经上传过的券Code总数量（官方字段路径 <c>stock.coupon_code_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当前可用的Code数】 品牌方为此批次已经上传过的券Code中，当前剩余可用的券Code数量（官方字段路径 <c>stock.coupon_code_count_info.available_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_count")]
    public long? AvailableCount { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【发放规则】 发放规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStockstockSendRule
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

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl·【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem
{
    /// <summary>【当天可用开始时间】 当天可用时间距离当天 00:00:00 的秒数，如 `60` 表示当天 00:01:00 开始可用。取值范围 \[0,86399\] 且必须小于 `end_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.begin_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>【当天可用结束时间】 当天结束可用时间距离当天 00:00:00 的秒数，如 `86399` 表示当天 23:59:59 结束可用。取值范围 \[0,86399\] 且必须大于 `begin_time`（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list.end_time</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `availabl。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod
{
    /// <summary>【每周可用星期数】 每周特定星期X可用，在日期区间内每周循环有效。例如 配置 `["MONDAY", "FRIDAY"]` 表示每周一和周五可用。可选取值：`MONDAY` 周一可用 / `TUESDAY` 周二可用 / `WEDNESDAY` 周三可用 / `THURSDAY` 周四可用 / `FRIDAY` 周五可用 / `SATURDAY` 周六可用 / `SUNDAY` 周日可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("day_list")]
    public List<string>? DayList { get; set; }

    /// <summary>【当天可用时间段】 生效当天特定时间段内有效，可以配置多个时间段。最多可以配置 3 个时间段。不配置则全天可用（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period.day_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("day_period_list")]
    public List<ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriodDayPeriodListItem>? DayPeriodList { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销·【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem
{
    /// <summary>【开始时间】 区间开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【结束时间】 区间结束时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list.end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【券可核销时间】 确定用户领券后在什么时间段内可以核销。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriod
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

    /// <summary>【每周固定可用时间】 日期区间内，每周固定时间可以使用。可与 `available_days` 和 `wait_days_after_receive` 一同配置，一同配置时条件需同时满足券才可用（时间范围取交集）。 注：不可与 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.weekly_available_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("weekly_available_period")]
    public ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriodWeeklyAvailablePeriod? WeeklyAvailablePeriod { get; set; }

    /// <summary>【指定可用时间段】 可以配置多个无规律时间段，不同区间内请勿重叠，最多可配置2个区间。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.irregular_available_period_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("irregular_available_period_list")]
    public List<ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriodIrregularAvailablePeriodListItem>? IrregularAvailablePeriodList { get; set; }

    /// <summary>【领取后N秒内有效】 日期区间内，用户领券后立即生效，自领取时刻起 N 秒内可用，不与自然日对齐。最大为86400（24小时），且仅支持 3600 的整数倍。 注：不可与 `available_days`、 `wait_days_after_receive`、 `weekly_available_period`、 `irregular_available_period_list` 同时配置（官方字段路径 <c>stock.single_usage_rule.coupon_available_period.available_seconds</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_seconds")]
    public long? AvailableSeconds { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scop。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleNormalCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受满减优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.normal_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免金额】 固定减免金额，单位为分，必须大于0（官方字段路径 <c>stock.single_usage_rule.normal_coupon.discount_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleDiscountCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可享受折扣优惠。门槛可以为0，即无门槛。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.discount_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定减免百分比】 表示减免金额为总金额的百分比，必须大于0。例如：填入 `30` 表示减免 30%，即打 7 折（官方字段路径 <c>stock.single_usage_rule.discount_coupon.percent_off</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("percent_off")]
    public long? PercentOff { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提·【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `sc。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleExchangeCoupon
{
    /// <summary>【门槛金额】 使用门槛金额，单位为分，达到该门槛后可进行兑换。门槛可以为0，即无门槛兑换。例如： 满 100 元可用填 `10000` 满 0.01 元可用填 `1`（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.threshold</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("threshold")]
    public long? Threshold { get; set; }

    /// <summary>【固定兑换价格】 单位为分，必须大于或等于0（官方字段路径 <c>stock.single_usage_rule.exchange_coupon.exchange_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exchange_price")]
    public long? ExchangePrice { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRule
{
    /// <summary>【券可核销时间】 确定用户领券后在什么时间段内可以核销（官方字段路径 <c>stock.single_usage_rule.coupon_available_period</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_available_period")]
    public ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleCouponAvailablePeriod? CouponAvailablePeriod { get; set; }

    /// <summary>【满减券使用规则】 当且仅当 `type` 为 `NORMAL` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon")]
    public ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleNormalCoupon? NormalCoupon { get; set; }

    /// <summary>【折扣券使用规则】 当且仅当 `type` 为 `DISCOUNT` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_coupon")]
    public ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleDiscountCoupon? DiscountCoupon { get; set; }

    /// <summary>【兑换券使用规则】 当且仅当 `type` 为 `EXCHANGE` 且 `scope` 为 `SINGLE` 时必填，其他类型不应填写（官方字段路径 <c>stock.single_usage_rule.exchange_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("exchange_coupon")]
    public ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRuleExchangeCoupon? ExchangeCoupon { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息·【券可用门店信息】 用于描述全部可用门店信息，可配置小程序跳转地址进行展示。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo
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

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【券使用规则展示信息】 券使用规则展示信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStockusageRuleDisplayInfo
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
    public ProductCouponUserCouponDeactivateResponseStockStockusageRuleDisplayInfoCouponAvailableStoreInfo? CouponAvailableStoreInfo { get; set; }

    /// <summary>【APP跳转类型】 APP跳转类型，在 `coupon_usage_method_list` 中包含 `APP` 时必填。可选取值：`H5` 跳转H5，点击券立即使用跳转至配置的H5页面 / `PASSCODE_LINK` 口令链接，点击券立即使用展示口令链接供用户复制使用 / `USAGE_GUIDE` 展示使用指引，引导前往APP核销（官方字段路径 <c>stock.usage_rule_display_info.app_jump_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("app_jump_type")]
    public string? AppJumpType { get; set; }

    /// <summary>【口令链接】 口令链接。在 `app_jump_type` 为 `PASSCODE_LINK` 时必填（官方字段路径 <c>stock.usage_rule_display_info.passcode_link</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("passcode_link")]
    public string? PasscodeLink { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【小程序入口】 展示跳转小程序的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStockcouponDisplayInfoEntranceMiniProgram
{
    /// <summary>【小程序AppID】 品牌方或服务商小程序的AppID，可在 微信公众平台 查看，该小程序与品牌或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序跳转路径】 品牌方小程序内部跳转路径（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>【入口文案】 小程序入口文案，长度不超过5个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.entrance_wording</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("entrance_wording")]
    public string? EntranceWording { get; set; }

    /// <summary>【引导文案】 小程序入口引导文案，长度不超过6个UTF-8字符（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program.guidance_wording</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("guidance_wording")]
    public string? GuidanceWording { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口·【公众号入口】 展示跳转公众号的入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStockcouponDisplayInfoEntranceOfficialAccount
{
    /// <summary>【公众号AppID】 微信公众号唯一标识，可在 微信公众平台 查看，该公众号应与品牌方或服务商存在绑定关系（官方字段路径 <c>stock.coupon_display_info.entrance_official_account.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStockcouponDisplayInfo
{
    /// <summary>【用户商品券Code展示模式】 决定用户商品券Code在卡包中的展示形态。可选取值：`INVISIBLE` 不展示Code / `BARCODE` 条形码 / `QRCODE` 二维码（官方字段路径 <c>stock.coupon_display_info.code_display_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_display_mode")]
    public string? CodeDisplayMode { get; set; }

    /// <summary>【背景颜色】 券的背景颜色，可设置10种颜色，色值请参考下方说明。颜色取值为颜色图中的颜色名称，不填默认为 `Color020` 。 !（官方字段路径 <c>stock.coupon_display_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【小程序入口】 展示跳转小程序的入口（官方字段路径 <c>stock.coupon_display_info.entrance_mini_program</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_mini_program")]
    public ProductCouponUserCouponDeactivateResponseStockStockcouponDisplayInfoEntranceMiniProgram? EntranceMiniProgram { get; set; }

    /// <summary>【公众号入口】 展示跳转公众号的入口（官方字段路径 <c>stock.coupon_display_info.entrance_official_account</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("entrance_official_account")]
    public ProductCouponUserCouponDeactivateResponseStockStockcouponDisplayInfoEntranceOfficialAccount? EntranceOfficialAccount { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocknotifyConfig
{
    /// <summary>【事件通知AppID】 服务商或者品牌的AppID，支持小程序、服务号、公众号、APP类型的AppID，可在微信公众平台查看，用于券事件通知时计算用户的OpenID，需要与服务商或者品牌存在绑定关系（官方字段路径 <c>stock.notify_config.notify_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_appid")]
    public string? NotifyAppid { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情·【已发放次数】 本批次已发放次数。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStockStocksentCountInfo
{
    /// <summary>【已发放总次数】 批次在生命周期内已发放次数（官方字段路径 <c>stock.sent_count_info.total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【当天已发放次数】 批次在当天已发放次数（官方字段路径 <c>stock.sent_count_info.today_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("today_count")]
    public long? TodayCount { get; set; }

}

/// <summary>作废用户商品券响应·【批次信息】 该用户商品券发券时使用的批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseStock
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

    /// <summary>【券Code分配模式】 决定发券时用户商品券Code如何产生。可选取值：`WECHATPAY` 微信支付随机生成，发券时由微信支付系统自动随机生成券码，品牌方无需干预，微信支付随机生成的券Code长度限制为40个字符以内 / `UPLOAD` 品牌方预上传Code，品牌方需先使用【 [预上传券Code API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781572.md)】或【 [预上传券Code API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016434668.md)】上传自定义Code，微信支付系统发券时从中随机选取，当预存Code不足时可能影响发券，品牌应及时补充 / `API_ASSIGN` 品牌方自行指定，品牌方通过【 [向用户发放商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781605.md)】自行发券时指定，微信支付系统不再进行自动分配。特别注意：这种模式的商品批次只可由品牌方自行发券，无法在摇一摇有优惠等微信支付渠道投放；商品券的 `usage_mode` 为 `PROGRESSIVE_BUNDLE` 时不可使用本模式（官方字段路径 <c>stock.coupon_code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code_mode")]
    public string? CouponCodeMode { get; set; }

    /// <summary>【品牌方预上传的券Code数量信息】 当且仅当 `coupon_code_mode` 为 `UPLOAD` 时存在此字段（官方字段路径 <c>stock.coupon_code_count_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_code_count_info")]
    public ProductCouponUserCouponDeactivateResponseStockStockcouponCodeCountInfo? CouponCodeCountInfo { get; set; }

    /// <summary>【发放规则】 发放规则（官方字段路径 <c>stock.stock_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_send_rule")]
    public ProductCouponUserCouponDeactivateResponseStockStockstockSendRule? StockSendRule { get; set; }

    /// <summary>【单券使用规则】 当且仅当 `usage_mode` 为 `SINGLE` 时提供，其他场景不提供（官方字段路径 <c>stock.single_usage_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_rule")]
    public ProductCouponUserCouponDeactivateResponseStockStocksingleUsageRule? SingleUsageRule { get; set; }

    /// <summary>【券使用规则展示信息】 券使用规则展示信息（官方字段路径 <c>stock.usage_rule_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("usage_rule_display_info")]
    public ProductCouponUserCouponDeactivateResponseStockStockusageRuleDisplayInfo? UsageRuleDisplayInfo { get; set; }

    /// <summary>【用户商品券展示信息】 用户商品券在卡包中的展示详情，包括引导用户的自定义入口（官方字段路径 <c>stock.coupon_display_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_display_info")]
    public ProductCouponUserCouponDeactivateResponseStockStockcouponDisplayInfo? CouponDisplayInfo { get; set; }

    /// <summary>【事件通知配置】 发生券相关事件时，微信支付会向服务商发送通知，需要提供通知相关配置（官方字段路径 <c>stock.notify_config</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("notify_config")]
    public ProductCouponUserCouponDeactivateResponseStockStocknotifyConfig? NotifyConfig { get; set; }

    /// <summary>【可用门店范围】 控制该批次可以在品牌下哪些门店使用。可选取值：`NONE` 无关联门店，该批次对外不展示可用门店信息 / `ALL` 所有门店可用，该批次在品牌下的所有门店可用，品牌无需为该批次关联门店列表 / `SPECIFIC` 特定门店可用，品牌需调用关联门店接口关联门店，关联后该批次在关联的门店可用（官方字段路径 <c>stock.store_scope</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_scope")]
    public string? StoreScope { get; set; }

    /// <summary>【已发放次数】 本批次已发放次数（官方字段路径 <c>stock.sent_count_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("sent_count_info")]
    public ProductCouponUserCouponDeactivateResponseStockStocksentCountInfo? SentCountInfo { get; set; }

    /// <summary>【批次状态】 商品券批次状态。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方字段路径 <c>stock.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【失效请求单号】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用失效接口时传入的请求流水号（官方字段路径 <c>stock.deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 当且仅当 `state` 为 `DEACTIVATED` 时提供，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13…（官方字段路径 <c>stock.deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 当且仅当 `state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效商品券批次API】时传入的失效原因（官方字段路径 <c>stock.deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方字段路径 <c>stock.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>作废用户商品券响应·【用户商品券标签信息】 用户商品券标签信息·【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseCouponTagInfoMemberTagInfo
{
    /// <summary>【会员卡模板ID】 通过 商家名片会员API 创建会员卡模板成功后，获得的会员卡模板ID（官方字段路径 <c>coupon_tag_info.member_tag_info.member_card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>作废用户商品券响应·【用户商品券标签信息】 用户商品券标签信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponseCouponTagInfo
{
    /// <summary>【用户商品券标签列表】 用户商品券标签列表。可选取值：`MEMBER` 会员标签，标识该用户商品券为会员券 / `MEMBER_ACTIVITY` 会员活动标签，标识该用户券为会员活动券（官方字段路径 <c>coupon_tag_info.coupon_tag_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("coupon_tag_list")]
    public List<string>? CouponTagList { get; set; }

    /// <summary>【会员标签信息】 当用户商品券标签列表中有 `MEMBER` 时，该字段必填（官方字段路径 <c>coupon_tag_info.member_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_tag_info")]
    public ProductCouponUserCouponDeactivateResponseCouponTagInfoMemberTagInfo? MemberTagInfo { get; set; }

}

/// <summary>作废用户商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781578"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponUserCouponDeactivateResponse : WechatPayResponse
{
    /// <summary>【用户商品券Code】 用户商品券的唯一标识（官方字段路径 <c>coupon_code</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【用户商品券状态】。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间 / `USED` 已核销，用户商品券已核销 / `EXPIRED` 已过期，用户商品券已超过有效期，不再可用 / `DELETED` 已删除，用户主动删除该券 / `DEACTIVATED` 已失效，品牌方主动调用【 [失效用户商品券API](https://pay.weixin.qq.com/doc/v3/partner/4015781578.md)】或【 [失效用户商品券组API](https://pay.weixin.qq.com/doc/v3/partner/4016280658.md)】使用户商品券失效（官方字段路径 <c>coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【有效期开始时间】 用户商品券可用开始时间，遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【有效期结束时间】 用户商品券可用结束时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>valid_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【领券时间】 用户领券时间。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【发券请求单号】 发券时传入的请求流水号（官方字段路径 <c>send_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("send_request_no")]
    public string? SendRequestNo { get; set; }

    /// <summary>【发券渠道】 描述用户商品券是经由什么渠道发送的。可选取值：`BRAND_MANAGE` 摇一摇有优惠，通过摇一摇有优惠渠道发放 / `API` 服务商自主发券，服务商通过发券接口自主发券到商家名片 / `RECEIVE_COMPONENT` 小程序领券组件，服务商通过小程序领券组件发券 / `MEMBER` 会员，品牌方通过会员相关活动或渠道进行发券 / `STICKER_GIFT` 贴图有礼，用户参与品牌贴图有礼活动后发放（官方字段路径 <c>send_channel</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_channel")]
    public string? SendChannel { get; set; }

    /// <summary>【确认请求单号】 品牌方确认发券请求时传入的的请求流水号。当且仅当 品牌方调用【 确认发放用户商品券API】后提供（官方字段路径 <c>confirm_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_request_no")]
    public string? ConfirmRequestNo { get; set; }

    /// <summary>【确认发放时间】 品牌方确认发券时间，当且仅当 品牌方调用【 确认发放用户商品券API】后提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>confirm_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>【失效请求单号】 品牌方失效券请求时传入的的请求流水号。当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的请求流水号（官方字段路径 <c>deactivate_request_no</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deactivate_request_no")]
    public string? DeactivateRequestNo { get; set; }

    /// <summary>【失效时间】 失效时间，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供。遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>deactivate_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("deactivate_time")]
    public string? DeactivateTime { get; set; }

    /// <summary>【失效原因】 失效券的原因，当且仅当 `coupon_state` 为 `DEACTIVATED` 时提供，返回品牌方调用【 失效用户商品券API】或【 失效用户商品券组API】时传入的失效原因（官方字段路径 <c>deactivate_reason</c>，选填，类型 <c>string(150)</c>）。</summary>
    [JsonPropertyName("deactivate_reason")]
    public string? DeactivateReason { get; set; }

    /// <summary>【单券使用详情】 当且仅当 `usage_mode` 为 `SINGLE` 时提供（官方字段路径 <c>single_usage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("single_usage_detail")]
    public ProductCouponUserCouponDeactivateResponseSingleUsageDetail? SingleUsageDetail { get; set; }

    /// <summary>【商品券信息】 该用户商品券对应的商品券详情（官方字段路径 <c>product_coupon</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("product_coupon")]
    public ProductCouponUserCouponDeactivateResponseProductCoupon? ProductCoupon { get; set; }

    /// <summary>【批次信息】 该用户商品券发券时使用的批次详情（官方字段路径 <c>stock</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock")]
    public ProductCouponUserCouponDeactivateResponseStock? Stock { get; set; }

    /// <summary>【自定义附加信息】 调用发券接口时品牌方使用 `attach` 字段主动设置的附加信息。微信支付不会解析该信息，仅在查询用户商品券和回调中原样返回。 注： 发券渠道多样，只有品牌方通过发券接口发放的券才会在查询和回调中携带此字段，其他渠道发放的券 `attach` 为空（官方字段路径 <c>attach</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【渠道自定义信息】 使用微信支付提供的其他渠道（比如「摇一摇有优惠」）发放商品券时，渠道可能会设置该渠道特定的自定义信息，请根据 `send_channel` 字段判断如何解析本字段。不同渠道的自定义信息格式不同，请根据对应渠道的文档解析（官方字段路径 <c>channel_custom_info</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("channel_custom_info")]
    public string? ChannelCustomInfo { get; set; }

    /// <summary>【用户商品券标签信息】 用户商品券标签信息（官方字段路径 <c>coupon_tag_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("coupon_tag_info")]
    public ProductCouponUserCouponDeactivateResponseCouponTagInfo? CouponTagInfo { get; set; }

}
