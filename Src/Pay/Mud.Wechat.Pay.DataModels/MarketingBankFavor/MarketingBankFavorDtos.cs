// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingBankFavor;

/// <summary>银行商家券营销活动发券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014569793"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBankFavor")]
public class BankFavorMultiActivitySendRequest
{
    /// <summary>【批次ID】 微信为每个批次分配的唯一ID。校验规则：必须为绑定多笔立减活动的代金券（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【多笔立减活动ID】 多笔立减活动ID（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【商户单据号】 商户此次发放凭据号（格式：商户ID+日期+流水号），商户侧需保持唯一性（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【公众账号ID】 微信为发券方商户分配的公众账号ID，接口传入的所有appid应该为公众号的appid或者小程序的appid（在mp.weixin.qq.com申请的）或APP的appid（在open.weixin.qq.com申请的）。校验规则：1、该appid需要与接口传入中的openid有对应关系；2、该appid需要与调用接口的商户号（即请求头中的商户号）有绑定关系，若未绑定，可参考该指引完成绑定（ 商家商户号与AppID账号关联管理）（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【创建批次的商户号】 批次创建方商户号（官方字段路径 <c>stock_creator_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

}

/// <summary>银行商家券营销活动发券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014569793"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBankFavor")]
public class BankFavorMultiActivitySendResponse : WechatPayResponse
{
    /// <summary>【代金券ID】 优惠券的主键，唯一定义此资源的标识（官方字段路径 <c>coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

}

/// <summary>银行商家券详情响应·【已用金额列表】 券的已使用金额列表，列表中的元素代表该次使用的金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014569864"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBankFavor")]
public class BankFavorCouponQueryResponseUseAmountList
{
    /// <summary>【已用金额】 券的本次使用金额，单位：分（官方字段路径 <c>use_amount_list.used_amounts</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("used_amounts")]
    public List<string>? UsedAmounts { get; set; }

}

/// <summary>银行商家券详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014569864"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBankFavor")]
public class BankFavorCouponQueryResponse : WechatPayResponse
{
    /// <summary>【创建批次的商户号】 微信为创建方商户分配的商户号（官方字段路径 <c>stock_creator_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

    /// <summary>【批次ID】 微信为每个代金券批次分配的唯一ID（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【代金券ID】 微信为代金券唯一分配的ID（官方字段路径 <c>coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>【券名称】 券名称（官方字段路径 <c>coupon_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_name")]
    public string? CouponName { get; set; }

    /// <summary>【券状态】 券状态。可选取值：`COUPON_STATE_UNKNOW` 未知状态 / `COUPON_STATE_SEND` 可用 / `COUPON_STATE_USED` 已实扣 / `COUPON_STATE_EXPIRED` 已过期（官方字段路径 <c>coupon_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_state")]
    public string? CouponState { get; set; }

    /// <summary>【领券时间】 领券时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>receive_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }

    /// <summary>【可用开始时间】 可用开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>available_begin_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【可用结束时间】 可用结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>available_end_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【活动ID】 多笔立减活动ID（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【最大使用次数】 券的最大使用次数（官方字段路径 <c>max_use_number</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("max_use_number")]
    public long? MaxUseNumber { get; set; }

    /// <summary>【可用次数】 券的剩余可用次数（官方字段路径 <c>available_number</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_number")]
    public long? AvailableNumber { get; set; }

    /// <summary>【已使用次数】 券的已使用次数（官方字段路径 <c>used_number</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("used_number")]
    public long? UsedNumber { get; set; }

    /// <summary>【已用金额列表】 券的已使用金额列表，列表中的元素代表该次使用的金额（官方字段路径 <c>use_amount_list</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("use_amount_list")]
    public BankFavorCouponQueryResponseUseAmountList? UseAmountList { get; set; }

    /// <summary>【用户标识ID】 用户标识ID是微信用户在公众账号ID下的唯一用户标识（公众账号ID不同，则获取到的用户标识ID就不同），可用于永久标记一个用户。 获取用户标识ID的方式（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}
