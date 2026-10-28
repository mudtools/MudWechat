// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPayGiftActivity;

/// <summary>创建支付有礼活动请求·【活动基本信息】 用于创建活动的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点·【可用时间】 可用时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableTimeItem
{
    /// <summary>【可用开始时间】 可用开始时间（官方字段路径 <c>activity_base_info.available_periods.available_time.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【可用结束时间】 可用结束时间（官方字段路径 <c>activity_base_info.available_periods.available_time.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动基本信息】 用于创建活动的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点·【每日可用时间】 每日可用时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableDayTimeItem
{
    /// <summary>【每日可用开始时间】 每日可用开始时间(格式:110000,表示11:00:00)（官方字段路径 <c>activity_base_info.available_periods.available_day_time.begin_day_time</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("begin_day_time")]
    public string? BeginDayTime { get; set; }

    /// <summary>【每日可用结束时间】 每日可用结束时间 (格式:135959,表示13:59:59)（官方字段路径 <c>activity_base_info.available_periods.available_day_time.end_day_time</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("end_day_time")]
    public string? EndDayTime { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动基本信息】 用于创建活动的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestActivityBaseInfoActivityBaseInfoavailablePeriods
{
    /// <summary>【可用时间】 可用时间（官方字段路径 <c>activity_base_info.available_periods.available_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_time")]
    public List<PayGiftActivityCreateRequestActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableTimeItem>? AvailableTime { get; set; }

    /// <summary>【每日可用时间】 每日可用时间（官方字段路径 <c>activity_base_info.available_periods.available_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_day_time")]
    public List<PayGiftActivityCreateRequestActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableDayTimeItem>? AvailableDayTime { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动基本信息】 用于创建活动的基本信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestActivityBaseInfo
{
    /// <summary>【活动名称】 活动名称（官方字段路径 <c>activity_base_info.activity_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>【活动副标题】 活动副标题（官方字段路径 <c>activity_base_info.activity_second_title</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("activity_second_title")]
    public string? ActivitySecondTitle { get; set; }

    /// <summary>【商户logo】 商户logo，送出优惠券时展示（官方字段路径 <c>activity_base_info.merchant_logo_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }

    /// <summary>【背景颜色】 代金券的背景颜色，可设置10种颜色，颜色取值可参考 开发指引 3.2.1 background_color取值，默认为微信支付绿色， 颜色取值为颜色图中的颜色名称（官方字段路径 <c>activity_base_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【活动开始时间】 活动开始时间，最长可以配置1年内的活动，活动有效期最长90天（官方字段路径 <c>activity_base_info.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【活动结束时间】 活动结束时间，最长可以配置1年内的活动，活动有效期最长90天（官方字段路径 <c>activity_base_info.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点（官方字段路径 <c>activity_base_info.available_periods</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("available_periods")]
    public PayGiftActivityCreateRequestActivityBaseInfoActivityBaseInfoavailablePeriods? AvailablePeriods { get; set; }

    /// <summary>【商户请求单号】 商户创建批次凭据号（格式：商户id+日期+流水号），商户侧需保持唯一性，可包含英文字母，数字，｜，_，\*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>activity_base_info.out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【投放目的】 OFF_LINE_PAY：拉用户回店消费；JUMP_MINI_APP：引导用户前往小程序消费。可选取值：`OFF_LINE_PAY` 拉用户回店消费 / `JUMP_MINI_APP` 引导用户前往小程序消费（官方字段路径 <c>activity_base_info.delivery_purpose</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_purpose")]
    public string? DeliveryPurpose { get; set; }

    /// <summary>【商家小程序appid】 投放目的为跳转小程序时必填（官方字段路径 <c>activity_base_info.mini_programs_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 投放目的为跳转小程序必填（官方字段路径 <c>activity_base_info.mini_programs_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动奖品发放规则】 满送活动规则·【奖品基本信息列表】 奖品基本信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestAwardSendRuleAwardSendRuleawardListItem
{
    /// <summary>【代金券批次Id】 代金券批次Id（官方字段路径 <c>award_send_rule.award_list.stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【奖品原始图（大图）】 奖品大图，必填（官方字段路径 <c>award_send_rule.award_list.original_image_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("original_image_url")]
    public string? OriginalImageUrl { get; set; }

    /// <summary>【奖品缩略图（小图）】 奖品小图，当选多张券时必填（官方字段路径 <c>award_send_rule.award_list.thumbnail_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动奖品发放规则】 满送活动规则。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestAwardSendRule
{
    /// <summary>【消费金额门槛】 消费金额门槛 单位分（官方字段路径 <c>award_send_rule.transaction_amount_minimum</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_amount_minimum")]
    public long? TransactionAmountMinimum { get; set; }

    /// <summary>【发放内容】 发放内容，可选单张券或礼包，选礼包时奖品限定3-5个。可选取值：`SINGLE_COUPON` 单张券 / `GIFT_PACKAGE` 礼包（官方字段路径 <c>award_send_rule.send_content</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_content")]
    public string? SendContent { get; set; }

    /// <summary>【奖品类型】 奖品类型，暂时只支持商家券。可选取值：`BUSIFAVOR` 商家券（官方字段路径 <c>award_send_rule.award_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("award_type")]
    public string? AwardType { get; set; }

    /// <summary>【奖品基本信息列表】 奖品基本信息列表（官方字段路径 <c>award_send_rule.award_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_list")]
    public List<PayGiftActivityCreateRequestAwardSendRuleAwardSendRuleawardListItem>? AwardList { get; set; }

    /// <summary>【发券商户号选项】 发券商户号选取规则，支持选择在用券商户或手动输入商户号两种，选择手动时，发券商户号必填（商家券只支持手动输入）。可选取值：`IN_SEVICE_COUPON_MERCHANT` 在用券商户号 / `MANUAL_INPUT_MERCHANT` 手动输入发券商户号（官方字段路径 <c>award_send_rule.merchant_option</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_option")]
    public string? MerchantOption { get; set; }

    /// <summary>【发券商户号】 发券商户号，列表（官方字段路径 <c>award_send_rule.merchant_id_list</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("merchant_id_list")]
    public List<string>? MerchantIdList { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动高级设置】 其他高级配置项·【支付模式】 主要用于指定可用的支付场景，不限制支付模式不填，限制选填（商家券暂。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestAdvancedSettingAdvancedSettingpaymentMode
{
    /// <summary>【支付场景列表】 APP_SCENE：App场景；SWING_CARD:刷卡;NO_SECRET_SCENE:免密支付;MINIAPP_SCENE:小程序;FACE_PAY_SCENE:人脸支付; OTHER_SCENE:其他支付场景。可选取值：`APP_SCENE` app场景 / `SWING_CARD_SCENE` 刷卡 / `NO_SECRET_SCENE` 免密支付 / `MINIAPP_SCENE` 小程序 / `FACE_PAY_SCENE` 人脸支付 / `OTHER_SCENE` 其他支付场景（官方字段路径 <c>advanced_setting.payment_mode.payment_scene_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("payment_scene_list")]
    public List<string>? PaymentSceneList { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动高级设置】 其他高级配置项·【支付方式信息】 支付方式信息，不填为不限制（商家券暂不开放）。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestAdvancedSettingPaymentMethodInformation
{
    /// <summary>【支付方式】 支付方式，可以指定银行卡或零钱。可选取值：`CFT` 零钱 / `SPECIFIC_BANK_CARD` 指定银行卡（官方字段路径 <c>advanced_setting.payment_method_information.payment_method</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }

    /// <summary>【银行简称】 银行简称，指定支付方式为银行卡必填，详询附录（官方字段路径 <c>advanced_setting.payment_method_information.bank_abbreviation</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("bank_abbreviation")]
    public string? BankAbbreviation { get; set; }

}

/// <summary>创建支付有礼活动请求·【活动高级设置】 其他高级配置项。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequestAdvancedSetting
{
    /// <summary>【投放用户类别】 DELIVERY_ALL_PERSON:所有用户；DELIVERY_MEMBER_PERSON：会员用户。可选取值：`DELIVERY_ALL_PERSON` 所有用户 / `DELIVERY_MEMBER_PERSON` 会员用户（官方字段路径 <c>advanced_setting.delivery_user_category</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_user_category")]
    public string? DeliveryUserCategory { get; set; }

    /// <summary>【商家会员appid】 当投放用户类别为会员用户时必填（官方字段路径 <c>advanced_setting.merchant_member_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("merchant_member_appid")]
    public string? MerchantMemberAppid { get; set; }

    /// <summary>【支付模式】 主要用于指定可用的支付场景，不限制支付模式不填，限制选填（商家券暂不开放）（官方字段路径 <c>advanced_setting.payment_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_mode")]
    public PayGiftActivityCreateRequestAdvancedSettingAdvancedSettingpaymentMode? PaymentMode { get; set; }

    /// <summary>【支付方式信息】 支付方式信息，不填为不限制（商家券暂不开放）（官方字段路径 <c>advanced_setting.payment_method_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_method_information")]
    public PayGiftActivityCreateRequestAdvancedSettingPaymentMethodInformation? PaymentMethodInformation { get; set; }

    /// <summary>【订单优惠标记】 商户下单时需要传入相同的标记(goods_tag)，用户同时符合其他规则才能享受优惠（官方字段路径 <c>advanced_setting.goods_tags</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("goods_tags")]
    public List<string>? GoodsTags { get; set; }

}

/// <summary>创建支付有礼活动请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateRequest
{
    /// <summary>【活动基本信息】 用于创建活动的基本信息（官方字段路径 <c>activity_base_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activity_base_info")]
    public PayGiftActivityCreateRequestActivityBaseInfo? ActivityBaseInfo { get; set; }

    /// <summary>【活动奖品发放规则】 满送活动规则（官方字段路径 <c>award_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_send_rule")]
    public PayGiftActivityCreateRequestAwardSendRule? AwardSendRule { get; set; }

    /// <summary>【活动高级设置】 其他高级配置项（官方字段路径 <c>advanced_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("advanced_setting")]
    public PayGiftActivityCreateRequestAdvancedSetting? AdvancedSetting { get; set; }

}

/// <summary>创建支付有礼活动响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityCreateResponse : WechatPayResponse
{
    /// <summary>【活动id】 活动id（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【创建时间】 创建时间（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动基本信息】 创建活动时录入的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点·【可用时间】 可用时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataactivityBaseInfoAvailablePeriodsAvailableTimeItem
{
    /// <summary>【可用开始时间】 可用开始时间（官方字段路径 <c>data.activity_base_info.available_periods.available_time.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【可用结束时间】 可用结束时间（官方字段路径 <c>data.activity_base_info.available_periods.available_time.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动基本信息】 创建活动时录入的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点·【每日可用时间】 每日可用时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataactivityBaseInfoAvailablePeriodsAvailableDayTimeItem
{
    /// <summary>【每日可用开始时间】 每日可用开始时间(格式:110000,表示11:00:00)（官方字段路径 <c>data.activity_base_info.available_periods.available_day_time.begin_day_time</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("begin_day_time")]
    public string? BeginDayTime { get; set; }

    /// <summary>【每日可用结束时间】 每日可用结束时间 (格式:135959,表示13:59:59)（官方字段路径 <c>data.activity_base_info.available_periods.available_day_time.end_day_time</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("end_day_time")]
    public string? EndDayTime { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动基本信息】 创建活动时录入的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataactivityBaseInfoAvailablePeriods
{
    /// <summary>【可用时间】 可用时间（官方字段路径 <c>data.activity_base_info.available_periods.available_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_time")]
    public List<PayGiftActivityListResponseDataItemDataactivityBaseInfoAvailablePeriodsAvailableTimeItem>? AvailableTime { get; set; }

    /// <summary>【每日可用时间】 每日可用时间（官方字段路径 <c>data.activity_base_info.available_periods.available_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_day_time")]
    public List<PayGiftActivityListResponseDataItemDataactivityBaseInfoAvailablePeriodsAvailableDayTimeItem>? AvailableDayTime { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动基本信息】 创建活动时录入的基本信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataactivityBaseInfo
{
    /// <summary>【活动名称】 活动名称（官方字段路径 <c>data.activity_base_info.activity_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>【活动副标题】 活动副标题（官方字段路径 <c>data.activity_base_info.activity_second_title</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("activity_second_title")]
    public string? ActivitySecondTitle { get; set; }

    /// <summary>【商户logo】 商户logo，送出优惠券时展示（官方字段路径 <c>data.activity_base_info.merchant_logo_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }

    /// <summary>【背景颜色】 代金券的背景颜色，可设置10种颜色，颜色取值可参考 开发指引 3.2.1 background_color取值，默认为微信支付绿色， 颜色取值为颜色图中的颜色名称（官方字段路径 <c>data.activity_base_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【活动开始时间】 活动开始时间，最长可以配置1年内的活动，活动有效期最长90天（官方字段路径 <c>data.activity_base_info.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【活动结束时间】 活动结束时间，最长可以配置1年内的活动，活动有效期最长90天（官方字段路径 <c>data.activity_base_info.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点（官方字段路径 <c>data.activity_base_info.available_periods</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("available_periods")]
    public PayGiftActivityListResponseDataItemDataactivityBaseInfoAvailablePeriods? AvailablePeriods { get; set; }

    /// <summary>【商户请求单号】 商户创建批次凭据号（格式：商户id+日期+流水号），商户侧需保持唯一性，可包含英文字母，数字，｜，_，\*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>data.activity_base_info.out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【投放目的】 OFF_LINE_PAY：拉用户回店消费；JUMP_MINI_APP：引导用户前往小程序消费。可选取值：`OFF_LINE_PAY` 拉用户回店消费 / `JUMP_MINI_APP` 引导用户前往小程序消费（官方字段路径 <c>data.activity_base_info.delivery_purpose</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_purpose")]
    public string? DeliveryPurpose { get; set; }

    /// <summary>【商家小程序appid】 投放目的为跳转小程序时必填（官方字段路径 <c>data.activity_base_info.mini_programs_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 投放目的为跳转小程序必填（官方字段路径 <c>data.activity_base_info.mini_programs_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种·【满送活动奖品发放规则】·【奖品基本信息列表】 奖品基本信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataawardSendRuleAwardSendRulefullSendRuleAwardListItem
{
    /// <summary>【代金券批次Id】 代金券批次Id（官方字段路径 <c>data.award_send_rule.full_send_rule.award_list.stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【奖品原始图（大图）】 奖品大图，必填（官方字段路径 <c>data.award_send_rule.full_send_rule.award_list.original_image_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("original_image_url")]
    public string? OriginalImageUrl { get; set; }

    /// <summary>【奖品缩略图（小图）】 奖品小图，当选多张券时必填（官方字段路径 <c>data.award_send_rule.full_send_rule.award_list.thumbnail_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种·【满送活动奖品发放规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataawardSendRuleAwardSendRulefullSendRule
{
    /// <summary>【消费金额门槛】 消费金额门槛 单位分（官方字段路径 <c>data.award_send_rule.full_send_rule.transaction_amount_minimum</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_amount_minimum")]
    public long? TransactionAmountMinimum { get; set; }

    /// <summary>【发放内容】 发放内容，可选单张券或礼包，选礼包时奖品限定3-5个。可选取值：`SINGLE_COUPON` 单张券 / `GIFT_PACKAGE` 礼包（官方字段路径 <c>data.award_send_rule.full_send_rule.send_content</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_content")]
    public string? SendContent { get; set; }

    /// <summary>【奖品类型】 奖品类型，暂时只支持商家券。可选取值：`BUSIFAVOR` 商家券（官方字段路径 <c>data.award_send_rule.full_send_rule.award_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("award_type")]
    public string? AwardType { get; set; }

    /// <summary>【奖品基本信息列表】 奖品基本信息列表（官方字段路径 <c>data.award_send_rule.full_send_rule.award_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_list")]
    public List<PayGiftActivityListResponseDataItemDataawardSendRuleAwardSendRulefullSendRuleAwardListItem>? AwardList { get; set; }

    /// <summary>【发券商户号选项】 发券商户号选取规则，支持选择在用券商户或手动输入商户号两种，选择手动时，发券商户号必填（商家券只支持手动输入）。可选取值：`IN_SEVICE_COUPON_MERCHANT` 在用券商户号 / `MANUAL_INPUT_MERCHANT` 手动输入发券商户号（官方字段路径 <c>data.award_send_rule.full_send_rule.merchant_option</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_option")]
    public string? MerchantOption { get; set; }

    /// <summary>【发券商户号】 发券商户号，列表（官方字段路径 <c>data.award_send_rule.full_send_rule.merchant_id_list</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("merchant_id_list")]
    public List<string>? MerchantIdList { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataawardSendRule
{
    /// <summary>【满送活动奖品发放规则】（官方字段路径 <c>data.award_send_rule.full_send_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("full_send_rule")]
    public PayGiftActivityListResponseDataItemDataawardSendRuleAwardSendRulefullSendRule? FullSendRule { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动高级设置】 创建时传入的高级设置信息·【支付模式】 主要用于指定可用的支付场景，不限制支付模式不填，限制选填（商家券暂。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataadvancedSettingPaymentMode
{
    /// <summary>【支付场景列表】 APP_SCENE：App场景；SWING_CARD:刷卡;NO_SECRET_SCENE:免密支付;MINIAPP_SCENE:小程序;FACE_PAY_SCENE:人脸支付; OTHER_SCENE:其他支付场景。可选取值：`APP_SCENE` app场景 / `SWING_CARD_SCENE` 刷卡 / `NO_SECRET_SCENE` 免密支付 / `MINIAPP_SCENE` 小程序 / `FACE_PAY_SCENE` 人脸支付 / `OTHER_SCENE` 其他支付场景（官方字段路径 <c>data.advanced_setting.payment_mode.payment_scene_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("payment_scene_list")]
    public List<string>? PaymentSceneList { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动高级设置】 创建时传入的高级设置信息·【支付方式信息】 支付方式信息，不填为不限制（商家券暂不开放）。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataadvancedSettingPaymentMethodInformation
{
    /// <summary>【支付方式】 支付方式，可以指定银行卡或零钱。可选取值：`CFT` 零钱 / `SPECIFIC_BANK_CARD` 指定银行卡（官方字段路径 <c>data.advanced_setting.payment_method_information.payment_method</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }

    /// <summary>【银行简称】 银行简称，指定支付方式为银行卡必填，详询附录（官方字段路径 <c>data.advanced_setting.payment_method_information.bank_abbreviation</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("bank_abbreviation")]
    public string? BankAbbreviation { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表·【活动高级设置】 创建时传入的高级设置信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItemDataadvancedSetting
{
    /// <summary>【投放用户类别】 DELIVERY_ALL_PERSON:所有用户；DELIVERY_MEMBER_PERSON：会员用户。可选取值：`DELIVERY_ALL_PERSON` 所有用户 / `DELIVERY_MEMBER_PERSON` 会员用户（官方字段路径 <c>data.advanced_setting.delivery_user_category</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_user_category")]
    public string? DeliveryUserCategory { get; set; }

    /// <summary>【商家会员appid】 当投放用户类别为会员用户时必填（官方字段路径 <c>data.advanced_setting.merchant_member_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("merchant_member_appid")]
    public string? MerchantMemberAppid { get; set; }

    /// <summary>【支付模式】 主要用于指定可用的支付场景，不限制支付模式不填，限制选填（商家券暂不开放）（官方字段路径 <c>data.advanced_setting.payment_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_mode")]
    public PayGiftActivityListResponseDataItemDataadvancedSettingPaymentMode? PaymentMode { get; set; }

    /// <summary>【支付方式信息】 支付方式信息，不填为不限制（商家券暂不开放）（官方字段路径 <c>data.advanced_setting.payment_method_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_method_information")]
    public PayGiftActivityListResponseDataItemDataadvancedSettingPaymentMethodInformation? PaymentMethodInformation { get; set; }

    /// <summary>【订单优惠标记】 商户下单时需要传入相同的标记(goods_tag)，用户同时符合其他规则才能享受优惠（官方字段路径 <c>data.advanced_setting.goods_tags</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("goods_tags")]
    public List<string>? GoodsTags { get; set; }

}

/// <summary>查询支付有礼活动列表响应·【结果集】 支付有礼活动列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponseDataItem
{
    /// <summary>【活动Id】 活动Id（官方字段路径 <c>data.activity_id</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【活动类型】 活动类型。可选取值：`FULL_SEND_ACT_TYPE` 满额送 / `STEP_SEND_ACT_TYPE` 阶梯送 / `SPECIFIC_SEND_ACT_TYPE` 满A送B（官方字段路径 <c>data.activity_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_type")]
    public string? ActivityType { get; set; }

    /// <summary>【活动基本信息】 创建活动时录入的基本信息（官方字段路径 <c>data.activity_base_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activity_base_info")]
    public PayGiftActivityListResponseDataItemDataactivityBaseInfo? ActivityBaseInfo { get; set; }

    /// <summary>【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种（官方字段路径 <c>data.award_send_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_send_rule")]
    public PayGiftActivityListResponseDataItemDataawardSendRule? AwardSendRule { get; set; }

    /// <summary>【活动高级设置】 创建时传入的高级设置信息（官方字段路径 <c>data.advanced_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("advanced_setting")]
    public PayGiftActivityListResponseDataItemDataadvancedSetting? AdvancedSetting { get; set; }

    /// <summary>【活动状态】 活动当前状态枚举值。可选取值：`ACT_STATUS_UNKNOWN` 状态未知 / `CREATE_ACT_STATUS` 已创建 / `ONGOING_ACT_STATUS` 运行中 / `TERMINATE_ACT_STATUS` 已终止 / `STOP_ACT_STATUS` 已暂停 / `OVER_TIME_ACT_STATUS` 已过期 / `CREATE_ACT_FAILED` 创建活动失败（官方字段路径 <c>data.activity_status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_status")]
    public string? ActivityStatus { get; set; }

    /// <summary>【创建商户号】 创建商户号（官方字段路径 <c>data.creator_merchant_id</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("creator_merchant_id")]
    public string? CreatorMerchantId { get; set; }

    /// <summary>【所属商户号】 所属商户号（官方字段路径 <c>data.belong_merchant_id</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("belong_merchant_id")]
    public string? BelongMerchantId { get; set; }

    /// <summary>【活动创建时间】 活动创建时间（官方字段路径 <c>data.create_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【活动更新时间】 活动更新时间（官方字段路径 <c>data.update_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询支付有礼活动列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityListResponse : WechatPayResponse
{
    /// <summary>【结果集】 支付有礼活动列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<PayGiftActivityListResponseDataItem>? Data { get; set; }

    /// <summary>【总数】 总数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动基本信息】 创建活动时录入的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点·【可用时间】 可用时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableTimeItem
{
    /// <summary>【可用开始时间】 可用开始时间（官方字段路径 <c>activity_base_info.available_periods.available_time.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【可用结束时间】 可用结束时间（官方字段路径 <c>activity_base_info.available_periods.available_time.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动基本信息】 创建活动时录入的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点·【每日可用时间】 每日可用时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableDayTimeItem
{
    /// <summary>【每日可用开始时间】 每日可用开始时间(格式:110000,表示11:00:00)（官方字段路径 <c>activity_base_info.available_periods.available_day_time.begin_day_time</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("begin_day_time")]
    public string? BeginDayTime { get; set; }

    /// <summary>【每日可用结束时间】 每日可用结束时间 (格式:135959,表示13:59:59)（官方字段路径 <c>activity_base_info.available_periods.available_day_time.end_day_time</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("end_day_time")]
    public string? EndDayTime { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动基本信息】 创建活动时录入的基本信息·【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseActivityBaseInfoActivityBaseInfoavailablePeriods
{
    /// <summary>【可用时间】 可用时间（官方字段路径 <c>activity_base_info.available_periods.available_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_time")]
    public List<PayGiftActivityQueryResponseActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableTimeItem>? AvailableTime { get; set; }

    /// <summary>【每日可用时间】 每日可用时间（官方字段路径 <c>activity_base_info.available_periods.available_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("available_day_time")]
    public List<PayGiftActivityQueryResponseActivityBaseInfoActivityBaseInfoavailablePeriodsAvailableDayTimeItem>? AvailableDayTime { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动基本信息】 创建活动时录入的基本信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseActivityBaseInfo
{
    /// <summary>【活动名称】 活动名称（官方字段路径 <c>activity_base_info.activity_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>【活动副标题】 活动副标题（官方字段路径 <c>activity_base_info.activity_second_title</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("activity_second_title")]
    public string? ActivitySecondTitle { get; set; }

    /// <summary>【商户logo】 商户logo，送出优惠券时展示（官方字段路径 <c>activity_base_info.merchant_logo_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }

    /// <summary>【背景颜色】 代金券的背景颜色，可设置10种颜色，颜色取值可参考 开发指引 3.2.1 background_color取值，默认为微信支付绿色， 颜色取值为颜色图中的颜色名称（官方字段路径 <c>activity_base_info.background_color</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>【活动开始时间】 活动开始时间，最长可以配置1年内的活动，活动有效期最长90天（官方字段路径 <c>activity_base_info.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【活动结束时间】 活动结束时间，最长可以配置1年内的活动，活动有效期最长90天（官方字段路径 <c>activity_base_info.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【可用时间段】 可自定义活动有效时间内可用的多个时间段以及天内时间点（官方字段路径 <c>activity_base_info.available_periods</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("available_periods")]
    public PayGiftActivityQueryResponseActivityBaseInfoActivityBaseInfoavailablePeriods? AvailablePeriods { get; set; }

    /// <summary>【商户请求单号】 商户创建批次凭据号（格式：商户id+日期+流水号），商户侧需保持唯一性，可包含英文字母，数字，｜，_，\*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>activity_base_info.out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【投放目的】 OFF_LINE_PAY：拉用户回店消费；JUMP_MINI_APP：引导用户前往小程序消费。可选取值：`OFF_LINE_PAY` 拉用户回店消费 / `JUMP_MINI_APP` 引导用户前往小程序消费（官方字段路径 <c>activity_base_info.delivery_purpose</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_purpose")]
    public string? DeliveryPurpose { get; set; }

    /// <summary>【商家小程序appid】 投放目的为跳转小程序时必填（官方字段路径 <c>activity_base_info.mini_programs_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_programs_appid")]
    public string? MiniProgramsAppid { get; set; }

    /// <summary>【商家小程序path】 投放目的为跳转小程序必填（官方字段路径 <c>activity_base_info.mini_programs_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("mini_programs_path")]
    public string? MiniProgramsPath { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种·【满送活动奖品发放规则】·【奖品基本信息列表】 奖品基本信息列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseAwardSendRuleAwardSendRulefullSendRuleAwardListItem
{
    /// <summary>【代金券批次Id】 代金券批次Id（官方字段路径 <c>award_send_rule.full_send_rule.award_list.stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【奖品原始图（大图）】 奖品大图，必填（官方字段路径 <c>award_send_rule.full_send_rule.award_list.original_image_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("original_image_url")]
    public string? OriginalImageUrl { get; set; }

    /// <summary>【奖品缩略图（小图）】 奖品小图，当选多张券时必填（官方字段路径 <c>award_send_rule.full_send_rule.award_list.thumbnail_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种·【满送活动奖品发放规则】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseAwardSendRuleAwardSendRulefullSendRule
{
    /// <summary>【消费金额门槛】 消费金额门槛 单位分（官方字段路径 <c>award_send_rule.full_send_rule.transaction_amount_minimum</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transaction_amount_minimum")]
    public long? TransactionAmountMinimum { get; set; }

    /// <summary>【发放内容】 发放内容，可选单张券或礼包，选礼包时奖品限定3-5个。可选取值：`SINGLE_COUPON` 单张券 / `GIFT_PACKAGE` 礼包（官方字段路径 <c>award_send_rule.full_send_rule.send_content</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("send_content")]
    public string? SendContent { get; set; }

    /// <summary>【奖品类型】 奖品类型，暂时只支持商家券。可选取值：`BUSIFAVOR` 商家券（官方字段路径 <c>award_send_rule.full_send_rule.award_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("award_type")]
    public string? AwardType { get; set; }

    /// <summary>【奖品基本信息列表】 奖品基本信息列表（官方字段路径 <c>award_send_rule.full_send_rule.award_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_list")]
    public List<PayGiftActivityQueryResponseAwardSendRuleAwardSendRulefullSendRuleAwardListItem>? AwardList { get; set; }

    /// <summary>【发券商户号选项】 发券商户号选取规则，支持选择在用券商户或手动输入商户号两种，选择手动时，发券商户号必填（商家券只支持手动输入）。可选取值：`IN_SEVICE_COUPON_MERCHANT` 在用券商户号 / `MANUAL_INPUT_MERCHANT` 手动输入发券商户号（官方字段路径 <c>award_send_rule.full_send_rule.merchant_option</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("merchant_option")]
    public string? MerchantOption { get; set; }

    /// <summary>【发券商户号】 发券商户号，列表（官方字段路径 <c>award_send_rule.full_send_rule.merchant_id_list</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("merchant_id_list")]
    public List<string>? MerchantIdList { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseAwardSendRule
{
    /// <summary>【满送活动奖品发放规则】（官方字段路径 <c>award_send_rule.full_send_rule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("full_send_rule")]
    public PayGiftActivityQueryResponseAwardSendRuleAwardSendRulefullSendRule? FullSendRule { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动高级设置】 创建时传入的高级设置信息·【支付模式】 主要用于指定可用的支付场景，不限制支付模式不填，限制选填（商家券暂。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseAdvancedSettingAdvancedSettingpaymentMode
{
    /// <summary>【支付场景列表】 APP_SCENE：App场景；SWING_CARD:刷卡;NO_SECRET_SCENE:免密支付;MINIAPP_SCENE:小程序;FACE_PAY_SCENE:人脸支付; OTHER_SCENE:其他支付场景。可选取值：`APP_SCENE` app场景 / `SWING_CARD_SCENE` 刷卡 / `NO_SECRET_SCENE` 免密支付 / `MINIAPP_SCENE` 小程序 / `FACE_PAY_SCENE` 人脸支付 / `OTHER_SCENE` 其他支付场景（官方字段路径 <c>advanced_setting.payment_mode.payment_scene_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("payment_scene_list")]
    public List<string>? PaymentSceneList { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动高级设置】 创建时传入的高级设置信息·【支付方式信息】 支付方式信息，不填为不限制（商家券暂不开放）。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseAdvancedSettingPaymentMethodInformation
{
    /// <summary>【支付方式】 支付方式，可以指定银行卡或零钱。可选取值：`CFT` 零钱 / `SPECIFIC_BANK_CARD` 指定银行卡（官方字段路径 <c>advanced_setting.payment_method_information.payment_method</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }

    /// <summary>【银行简称】 银行简称，指定支付方式为银行卡必填，详询附录（官方字段路径 <c>advanced_setting.payment_method_information.bank_abbreviation</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("bank_abbreviation")]
    public string? BankAbbreviation { get; set; }

}

/// <summary>查询支付有礼活动详情响应·【活动高级设置】 创建时传入的高级设置信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponseAdvancedSetting
{
    /// <summary>【投放用户类别】 DELIVERY_ALL_PERSON:所有用户；DELIVERY_MEMBER_PERSON：会员用户。可选取值：`DELIVERY_ALL_PERSON` 所有用户 / `DELIVERY_MEMBER_PERSON` 会员用户（官方字段路径 <c>advanced_setting.delivery_user_category</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("delivery_user_category")]
    public string? DeliveryUserCategory { get; set; }

    /// <summary>【商家会员appid】 当投放用户类别为会员用户时必填（官方字段路径 <c>advanced_setting.merchant_member_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("merchant_member_appid")]
    public string? MerchantMemberAppid { get; set; }

    /// <summary>【支付模式】 主要用于指定可用的支付场景，不限制支付模式不填，限制选填（商家券暂不开放）（官方字段路径 <c>advanced_setting.payment_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_mode")]
    public PayGiftActivityQueryResponseAdvancedSettingAdvancedSettingpaymentMode? PaymentMode { get; set; }

    /// <summary>【支付方式信息】 支付方式信息，不填为不限制（商家券暂不开放）（官方字段路径 <c>advanced_setting.payment_method_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_method_information")]
    public PayGiftActivityQueryResponseAdvancedSettingPaymentMethodInformation? PaymentMethodInformation { get; set; }

    /// <summary>【订单优惠标记】 商户下单时需要传入相同的标记(goods_tag)，用户同时符合其他规则才能享受优惠（官方字段路径 <c>advanced_setting.goods_tags</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("goods_tags")]
    public List<string>? GoodsTags { get; set; }

}

/// <summary>查询支付有礼活动详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityQueryResponse : WechatPayResponse
{
    /// <summary>【活动Id】 活动Id（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【活动类型】 活动类型。可选取值：`FULL_SEND_ACT_TYPE` 满额送 / `STEP_SEND_ACT_TYPE` 阶梯送 / `SPECIFIC_SEND_ACT_TYPE` 满A送B（官方字段路径 <c>activity_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_type")]
    public string? ActivityType { get; set; }

    /// <summary>【活动基本信息】 创建活动时录入的基本信息（官方字段路径 <c>activity_base_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activity_base_info")]
    public PayGiftActivityQueryResponseActivityBaseInfo? ActivityBaseInfo { get; set; }

    /// <summary>【活动奖品发放规则】 奖品派送规则，分别对应满送、阶梯送、满A送B中的一种（官方字段路径 <c>award_send_rule</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_send_rule")]
    public PayGiftActivityQueryResponseAwardSendRule? AwardSendRule { get; set; }

    /// <summary>【活动高级设置】 创建时传入的高级设置信息（官方字段路径 <c>advanced_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("advanced_setting")]
    public PayGiftActivityQueryResponseAdvancedSetting? AdvancedSetting { get; set; }

    /// <summary>【活动状态】 活动当前状态枚举值。可选取值：`ACT_STATUS_UNKNOWN` 状态未知 / `CREATE_ACT_STATUS` 已创建 / `ONGOING_ACT_STATUS` 运行中 / `TERMINATE_ACT_STATUS` 已终止 / `STOP_ACT_STATUS` 已暂停 / `OVER_TIME_ACT_STATUS` 已过期 / `CREATE_ACT_FAILED` 创建活动失败（官方字段路径 <c>activity_status</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_status")]
    public string? ActivityStatus { get; set; }

    /// <summary>【创建商户号】 创建商户号（官方字段路径 <c>creator_merchant_id</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("creator_merchant_id")]
    public string? CreatorMerchantId { get; set; }

    /// <summary>【所属商户号】 所属商户号（官方字段路径 <c>belong_merchant_id</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("belong_merchant_id")]
    public string? BelongMerchantId { get; set; }

    /// <summary>【活动暂停时间】 活动暂停时间（官方字段路径 <c>pause_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("pause_time")]
    public string? PauseTime { get; set; }

    /// <summary>【活动恢复时间】 活动恢复时间（官方字段路径 <c>recovery_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recovery_time")]
    public string? RecoveryTime { get; set; }

    /// <summary>【活动创建时间】 活动创建时间（官方字段路径 <c>create_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【活动更新时间】 活动更新时间（官方字段路径 <c>update_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>终止支付有礼活动响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466523"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityTerminateResponse : WechatPayResponse
{
    /// <summary>【生效时间】 生效时间（官方字段路径 <c>terminate_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("terminate_time")]
    public string? TerminateTime { get; set; }

    /// <summary>【活动Id】 活动Id（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

}
