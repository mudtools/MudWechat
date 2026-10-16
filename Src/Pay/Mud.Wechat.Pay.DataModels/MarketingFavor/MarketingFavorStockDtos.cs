// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingFavor;

/// <summary>条件查询代金券批次列表响应·【批次详情】 批次详情·【满减券/消费金批次使用规则】 满减券或消费金批次特定信息·【固定面额批次特定信息】 固定面额发券或消费金批次特定信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockListResponseDataItemDatastockUseRuleStockUseRulefixedNormalCoupon
{
    /// <summary>【面额】 面额，单位：分（官方字段路径 <c>data.stock_use_rule.fixed_normal_coupon.coupon_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("coupon_amount")]
    public long? CouponAmount { get; set; }

    /// <summary>【门槛】 使用券金额门槛，单位：分（官方字段路径 <c>data.stock_use_rule.fixed_normal_coupon.transaction_minimum</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>条件查询代金券批次列表响应·【批次详情】 批次详情·【满减券/消费金批次使用规则】 满减券或消费金批次特定信息·【固定折扣特定信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockListResponseDataItemDatastockUseRuleStockUseRulefixedDiscountCoupon
{
    /// <summary>【最高折扣金额】 最高折扣金额，单位分（官方字段路径 <c>data.stock_use_rule.fixed_discount_coupon.discount_amount_max</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount_max")]
    public long? DiscountAmountMax { get; set; }

    /// <summary>【折扣百分比】 折扣百分比，例如88-八八折（官方字段路径 <c>data.stock_use_rule.fixed_discount_coupon.discount_percent</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_percent")]
    public long? DiscountPercent { get; set; }

    /// <summary>【门槛】 使用券金额门槛，单位分（官方字段路径 <c>data.stock_use_rule.fixed_discount_coupon.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>条件查询代金券批次列表响应·【批次详情】 批次详情·【满减券/消费金批次使用规则】 满减券或消费金批次特定信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockListResponseDataItemDatastockUseRule
{
    /// <summary>【发放总上限】 最大发券数（官方字段路径 <c>data.stock_use_rule.max_coupons</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons")]
    public long? MaxCoupons { get; set; }

    /// <summary>【总预算】 总消耗金额，单位：分（官方字段路径 <c>data.stock_use_rule.max_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_amount")]
    public long? MaxAmount { get; set; }

    /// <summary>【单天发放上限金额】 单天最高消耗金额，单位：分（官方字段路径 <c>data.stock_use_rule.max_amount_by_day</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_amount_by_day")]
    public long? MaxAmountByDay { get; set; }

    /// <summary>【固定面额批次特定信息】 固定面额发券或消费金批次特定信息（官方字段路径 <c>data.stock_use_rule.fixed_normal_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fixed_normal_coupon")]
    public CouponStockListResponseDataItemDatastockUseRuleStockUseRulefixedNormalCoupon? FixedNormalCoupon { get; set; }

    /// <summary>【单个用户可领个数】 单个用户可领个数（官方字段路径 <c>data.stock_use_rule.max_coupons_per_user</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_coupons_per_user")]
    public long? MaxCouponsPerUser { get; set; }

    /// <summary>【券或消费金类型】 券或消费金类型 枚举值： NORMAL：满减券 CUT_TO：减至券（官方字段路径 <c>data.stock_use_rule.coupon_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_type")]
    public string? CouponType { get; set; }

    /// <summary>【订单优惠标记】 订单优惠标记 (该字段暂未开放返回) 特殊规则：单个优惠标记的字符长度为【1，128】,条目个数限制为【1，50】（官方字段路径 <c>data.stock_use_rule.goods_tag</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public List<string>? GoodsTag { get; set; }

    /// <summary>【指定支付模式】 默认不限制，枚举值： MICROAPP：小程序支付 APPPAY：APP支付 PPAY：免密支付 CARD：付款码支付 FACE：人脸支付 OTHER：（公众号、扫码等）。可选取值：`MICROAPP` 小程序支付 / `APPPAY` APP支付 / `PPAY` 免密支付 / `CARD` 刷卡支付 / `FACE` 人脸支付 / `OTHER` 其他支付，公众号、扫码等（官方字段路径 <c>data.stock_use_rule.trade_type</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("trade_type")]
    public List<string>? TradeType { get; set; }

    /// <summary>【是否可叠加其他优惠】 枚举值： true：是 false：否（官方字段路径 <c>data.stock_use_rule.combine_use</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("combine_use")]
    public bool? CombineUse { get; set; }

    /// <summary>【固定折扣特定信息】（官方字段路径 <c>data.stock_use_rule.fixed_discount_coupon</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fixed_discount_coupon")]
    public CouponStockListResponseDataItemDatastockUseRuleStockUseRulefixedDiscountCoupon? FixedDiscountCoupon { get; set; }

}

/// <summary>条件查询代金券批次列表响应·【批次详情】 批次详情·【减至批次特定信息】 单品优惠特定信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockListResponseDataItemDatacutToMessage
{
    /// <summary>【可用优惠的商品最高单价】 可用优惠的商品最高单价，单位：分（官方字段路径 <c>data.cut_to_message.single_price_max</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("single_price_max")]
    public long? SinglePriceMax { get; set; }

    /// <summary>【减至后的优惠单价】 减至后的优惠单价，单位：分（官方字段路径 <c>data.cut_to_message.cut_to_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("cut_to_price")]
    public long? CutToPrice { get; set; }

}

/// <summary>条件查询代金券批次列表响应·【批次详情】 批次详情·【消费金可用地域】 消费金可用地域列表，仅有当business_type=MUL。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockListResponseDataItemDataavailableRegionListItem
{
    /// <summary>【类型】 消费金可用地域的类型，COUNTRY表示国家级别可用，PROVINCE表示省级可用，CITY表示市级可用，DISTRICT表示区级可用。可选取值：`PROVINCE` 地域信息精确到省级 / `CITY` 地域信息精确到市级 / `DISTRICT` 地域信息精确到区级 / `COUNTRY` 地域信息精确到国家级（官方字段路径 <c>data.available_region_list.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【省】 消费金可用省（官方字段路径 <c>data.available_region_list.province</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>【市】 消费金可用（官方字段路径 <c>data.available_region_list.city</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>【区】 消费金可用区（官方字段路径 <c>data.available_region_list.district</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>【国家】 消费金可用国家（官方字段路径 <c>data.available_region_list.country</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

}

/// <summary>条件查询代金券批次列表响应·【批次详情】 批次详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockListResponseDataItem
{
    /// <summary>【批次号】 微信为每个代金券批次分配的唯一id（官方字段路径 <c>data.stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【批次创建方商户号】 微信为创建方商户分配的商户号（官方字段路径 <c>data.stock_creator_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

    /// <summary>【批次名称】 批次名称（官方字段路径 <c>data.stock_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_name")]
    public string? StockName { get; set; }

    /// <summary>【批次状态】 批次状态，枚举值： unactivated：未激活 audit：审核中 running：运行中 stoped：已停止 paused：暂停发放（官方字段路径 <c>data.status</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【创建时间】 批次创建时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【使用说明】 批次描述信息（官方字段路径 <c>data.description</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【满减券/消费金批次使用规则】 满减券或消费金批次特定信息（官方字段路径 <c>data.stock_use_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("stock_use_rule")]
    public CouponStockListResponseDataItemDatastockUseRule? StockUseRule { get; set; }

    /// <summary>【可用开始时间】 可用开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【可用结束时间】 可用结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【已发券或消费金数量】 已发券或消费金数量（官方字段路径 <c>data.distributed_coupons</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("distributed_coupons")]
    public long? DistributedCoupons { get; set; }

    /// <summary>【是否无资金流】 是否无资金流。枚举值： ture：是 false：否（官方字段路径 <c>data.no_cash</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("no_cash")]
    public bool? NoCash { get; set; }

    /// <summary>【激活批次的时间】 批次激活开启时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35…（官方字段路径 <c>data.start_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【终止批次的时间】 批次永久停止时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示，北京时间2015年5月20日 13点29分35…（官方字段路径 <c>data.stop_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stop_time")]
    public string? StopTime { get; set; }

    /// <summary>【减至批次特定信息】 单品优惠特定信息（官方字段路径 <c>data.cut_to_message</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("cut_to_message")]
    public CouponStockListResponseDataItemDatacutToMessage? CutToMessage { get; set; }

    /// <summary>【是否单品优惠】 枚举值： true：是 false：否（官方字段路径 <c>data.singleitem</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("singleitem")]
    public bool? Singleitem { get; set; }

    /// <summary>【批次类型】 批次类型 枚举值： NORMAL：代金券批次 DISCOUNT_CUT：立减与折扣 OTHER：其他（官方字段路径 <c>data.stock_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public string? StockType { get; set; }

    /// <summary>【卡包ID】 微信卡包ID（官方字段路径 <c>data.card_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【业务类型】 细分业务类型，仅有当business_type=MULTIUSE时，才会返回，枚举值： MULTIUSE：消费金。可选取值：`MULTIUSE` 消费金类型（官方字段路径 <c>data.business_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【消费金可用地域】 消费金可用地域列表，仅有当business_type=MULTIUSE时，才会返回（官方字段路径 <c>data.available_region_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_region_list")]
    public List<CouponStockListResponseDataItemDataavailableRegionListItem>? AvailableRegionList { get; set; }

    /// <summary>【消费金可用行业】 消费金可用行业列表，仅有当business_type=MULTIUSE时，才会返回（官方字段路径 <c>data.available_industry_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("available_industry_list")]
    public List<string>? AvailableIndustryList { get; set; }

}

/// <summary>条件查询代金券批次列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockListResponse : WechatPayResponse
{
    /// <summary>【批次总数】 经过条件筛选，查询到的批次总数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【批次详情】 批次详情（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<CouponStockListResponseDataItem>? Data { get; set; }

    /// <summary>【分页大小】 分页大小，最大10（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【分页页码】 页码从0开始，默认第0页（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

}

/// <summary>查询代金券可用单品响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463442"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockItemsResponse : WechatPayResponse
{
    /// <summary>【可用单品编码总数】 可用单品编码总数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【可用单品编码】 可用单品编码 特殊规则：单个商品编码的字符长度为【1，128】,条目个数限制为【1，100】（官方字段路径 <c>data</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<string>? Data { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【批次号】 批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>查询代金券可用商户响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463358"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockMerchantsResponse : WechatPayResponse
{
    /// <summary>【可用商户总数量】 可用商户总数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【可用商户列表】 可用商户列表 特殊规则：单个商户号的字符长度为【1，20】,条目个数限制为【1，50】（官方字段路径 <c>data</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<string>? Data { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【批次号】 批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>下载批次退款明细响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463523"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockRefundFlowResponse : WechatPayResponse
{
    /// <summary>【下载链接】 流水文件下载链接，30s内有效（官方字段路径 <c>url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>【安全校验码】 文件内容的哈希值，防止篡改（官方字段路径 <c>hash_value</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>【哈希算法类型】 哈希算法类型，目前只支持SHA1（官方字段路径 <c>hash_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

}

/// <summary>下载批次核销明细响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463585"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponStockUseFlowResponse : WechatPayResponse
{
    /// <summary>【下载链接】 流水文件下载链接，30s内有效（官方字段路径 <c>url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>【安全校验码】 文件内容的哈希值，防止篡改（官方字段路径 <c>hash_value</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>【哈希算法类型】 哈希算法类型，目前只支持sha1（官方字段路径 <c>hash_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

}
