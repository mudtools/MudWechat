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

/// <summary>条件查询用户代金券列表响应·【结果集】 结果集·【单品优惠特定信息】 单品优惠特定信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534690"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponUserCouponListResponseDataItemDatacutToMessage
{
    /// <summary>【可用优惠的商品最高单价】 可用优惠的商品最高单价，单位：分（官方字段路径 <c>data.cut_to_message.single_price_max</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("single_price_max")]
    public long? SinglePriceMax { get; set; }

    /// <summary>【减至后的优惠单价】 减至后的优惠单价，单位：分（官方字段路径 <c>data.cut_to_message.cut_to_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("cut_to_price")]
    public long? CutToPrice { get; set; }

}

/// <summary>条件查询用户代金券列表响应·【结果集】 结果集·【满减券或消费金信息】 普通满减券或消费金面额、门槛信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534690"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponUserCouponListResponseDataItemDatanormalCouponInformation
{
    /// <summary>【面额】 面额，单位：分（官方字段路径 <c>data.normal_coupon_information.coupon_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("coupon_amount")]
    public long? CouponAmount { get; set; }

    /// <summary>【门槛】 使用券金额门槛，单位：分（官方字段路径 <c>data.normal_coupon_information.transaction_minimum</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>条件查询用户代金券列表响应·【结果集】 结果集·【折扣券信息】 折扣券金面额、门槛信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534690"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponUserCouponListResponseDataItemDatadiscountMsg
{
    /// <summary>【最高折扣金额】 最高折扣金额，单位分（官方字段路径 <c>data.discount_msg.discount_amount_max</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_amount_max")]
    public long? DiscountAmountMax { get; set; }

    /// <summary>【折扣百分比】 折扣百分比，例如88-八八折（官方字段路径 <c>data.discount_msg.discount_percent</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_percent")]
    public long? DiscountPercent { get; set; }

    /// <summary>【门槛】 使用券金额门槛，单位分（官方字段路径 <c>data.discount_msg.transaction_minimum</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_minimum")]
    public long? TransactionMinimum { get; set; }

}

/// <summary>条件查询用户代金券列表响应·【结果集】 结果集。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534690"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponUserCouponListResponseDataItem
{
    /// <summary>【创建批次的商户号】 微信为创建方商户分配的商户号（官方字段路径 <c>data.stock_creator_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

    /// <summary>【批次号】 微信为每个代金券批次分配的唯一id（官方字段路径 <c>data.stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【代金券或消费金id】 微信为代金券或消费金唯一分配的id（官方字段路径 <c>data.coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>【单品优惠特定信息】 单品优惠特定信息（官方字段路径 <c>data.cut_to_message</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("cut_to_message")]
    public CouponUserCouponListResponseDataItemDatacutToMessage? CutToMessage { get; set; }

    /// <summary>【券或消费金名称】 券或消费金名称（官方字段路径 <c>data.coupon_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_name")]
    public string? CouponName { get; set; }

    /// <summary>【券或消费金状态】 券或消费金状态： SENDED：可用 USED：已实扣 EXPIRED：已过期 RECOVER：已回收 REVOKED: 已失效（官方字段路径 <c>data.status</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【使用说明】 券或消费金描述说明字段（官方字段路径 <c>data.description</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【领券时间】 领券时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【券或消费金类型】 券或消费金类型： NORMAL：满减券 CUT_TO：减至券（官方字段路径 <c>data.coupon_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_type")]
    public string? CouponType { get; set; }

    /// <summary>【是否无资金流】 枚举值： true：是 false：否（官方字段路径 <c>data.no_cash</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("no_cash")]
    public bool? NoCash { get; set; }

    /// <summary>【可用开始时间】 可用开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【可用结束时间】 可用结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>data.available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【是否单品优惠】 枚举值： true：是 false：否（官方字段路径 <c>data.singleitem</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("singleitem")]
    public bool? Singleitem { get; set; }

    /// <summary>【满减券或消费金信息】 普通满减券或消费金面额、门槛信息（官方字段路径 <c>data.normal_coupon_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("normal_coupon_information")]
    public CouponUserCouponListResponseDataItemDatanormalCouponInformation? NormalCouponInformation { get; set; }

    /// <summary>【商户单据号】 商户此次发放凭据号（官方字段路径 <c>data.out_request_no</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【剩余金额】 可用余额，单位：分。 仅有当business_type=MULTIUSE时，才会返回（官方字段路径 <c>data.available_balance</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_balance")]
    public long? AvailableBalance { get; set; }

    /// <summary>【业务类型】 细分业务类型，仅当business_type=MULTIUSE时才返回，枚举值： MULTIUSE：消费金。可选取值：`MULTIUSE` 消费金类型（官方字段路径 <c>data.business_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>【折扣券信息】 折扣券金面额、门槛信息（官方字段路径 <c>data.discount_msg</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("discount_msg")]
    public CouponUserCouponListResponseDataItemDatadiscountMsg? DiscountMsg { get; set; }

}

/// <summary>条件查询用户代金券列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534690"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponUserCouponListResponse : WechatPayResponse
{
    /// <summary>【结果集】 结果集（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<CouponUserCouponListResponseDataItem>? Data { get; set; }

    /// <summary>【查询结果总数】 查询结果总数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页大小】 分页大小，该字段仅在按批次创建方商户号查询时返回（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【分页页码】 分页页码，该字段仅在按批次创建方商户号查询时返回（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

}
