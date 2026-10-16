// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingMemberCardActivity;

/// <summary>创建会员有礼活动请求·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【有效期内指定部分活动时间段】 在整个活动有效期内，指定更小范围的活动时间。 举。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestAwardSendPeriodAwardSendTimeItem
{
    /// <summary>发放开始时间】 发放开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>award_send_period.award_send_time.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【发放结束时间】 发放结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>award_send_period.award_send_time.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建会员有礼活动请求·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设·【每日发放开始时间】 每日发放开始时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestAwardSendPeriodAwardSendDayTimeItemBeginDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>创建会员有礼活动请求·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设·【每日发放结束时间】 每日发放结束时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestAwardSendPeriodAwardSendDayTimeItemEndDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>创建会员有礼活动请求·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestAwardSendPeriodAwardSendDayTimeItem
{
    /// <summary>【每日发放开始时间】 每日发放开始时间（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("begin_day_time")]
    public MemberCardActivityCreateRequestAwardSendPeriodAwardSendDayTimeItemBeginDayTime? BeginDayTime { get; set; }

    /// <summary>【每日发放结束时间】 每日发放结束时间（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("end_day_time")]
    public MemberCardActivityCreateRequestAwardSendPeriodAwardSendDayTimeItemEndDayTime? EndDayTime { get; set; }

}

/// <summary>创建会员有礼活动请求·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestAwardSendPeriod
{
    /// <summary>【有效期内指定部分活动时间段】 在整个活动有效期内，指定更小范围的活动时间。 举例：活动整体时间在7月1日-31日，指定具体时间：7月7日、8日；7月17日、18日（官方字段路径 <c>award_send_period.award_send_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_time")]
    public List<MemberCardActivityCreateRequestAwardSendPeriodAwardSendTimeItem>? AwardSendTime { get; set; }

    /// <summary>【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设置每日可用时间，取两者的交集时间段发券。 注意：若想设置指定时间段，则必须同时设置指定日期award_send_time，否则指定时间段的设置不会生效。 举例：活动整体时间在7月1日-31日，指定具体时间：7月7日、8日；7月17日、18日，指定具体时间段：上午9点-下午6点（官方字段路径 <c>award_send_period.award_send_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_day_time")]
    public List<MemberCardActivityCreateRequestAwardSendPeriodAwardSendDayTimeItem>? AwardSendDayTime { get; set; }

}

/// <summary>创建会员有礼活动请求·【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestStockListItem
{
    /// <summary>【创建批次的商户号】 代金券或商家券的创建商户号。 注意：若制造的是代金券，则需要满足以下其一： （1）制券商户号=活动创建方商户号； （2）制券商户号已获得授权。若制造的是商家券，则需要满足以下关系：制券商户号=活动创建方商户号（官方字段路径 <c>stock_list.stock_creator_mchid</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

    /// <summary>【券批次ID】 活动发放的代金券或商家券的券批次ID。 注意： （1）针对活动类型为：支付后开卡有礼/支付后老会员有礼，若要发商家券，则商家券code分配方式不支持「实时分配」； （2）活动创建方的商户号，需要与会员卡归属品牌号（brand_id）绑定b-m关系（官方字段路径 <c>stock_list.stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>创建会员有礼活动请求·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE·【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestPayActivitySettingActivateSettingActivateMiniprogram
{
    /// <summary>【跳转激活的AppID】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的AppID。如果同时配置了activate_url和activate_appid/path，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram.activate_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activate_appid")]
    public string? ActivateAppid { get; set; }

    /// <summary>【跳转激活的path】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的path（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram.activate_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("activate_path")]
    public string? ActivatePath { get; set; }

}

/// <summary>创建会员有礼活动请求·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestPayActivitySettingActivateSetting
{
    /// <summary>【激活类型】 激活类型。可选取值：`AUTO_ACTIVATE` 用户领卡后微信后台自动将会员卡激活 - JUMP_ACTIVATE: 用户领卡后（官方字段路径 <c>pay_activity_setting.activate_setting.activate_type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activate_type")]
    public string? ActivateType { get; set; }

    /// <summary>【跳转激活的URL】 激活类型是跳转激活时，用户点击开卡后跳转到商家网页的URL。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_miniprogram")]
    public MemberCardActivityCreateRequestPayActivitySettingActivateSettingActivateMiniprogram? ActivateMiniprogram { get; set; }

}

/// <summary>创建会员有礼活动请求·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符·【支付模式】 不限制支付模式不填，限制选填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestPayActivitySettingPaymentSettingPaymentMode
{
    /// <summary>【支付场景列表】 支付场景列表。可选取值：`APP` APP支付场景 - MICROPAY: 付款码支付 - PAP: 委托代扣 - （官方字段路径 <c>pay_activity_setting.payment_setting.payment_mode.payment_scene_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("payment_scene_list")]
    public List<string>? PaymentSceneList { get; set; }

}

/// <summary>创建会员有礼活动请求·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestPayActivitySettingPaymentSetting
{
    /// <summary>【支付模式】 不限制支付模式不填，限制选填（官方字段路径 <c>pay_activity_setting.payment_setting.payment_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_mode")]
    public MemberCardActivityCreateRequestPayActivitySettingPaymentSettingPaymentMode? PaymentMode { get; set; }

    /// <summary>【指定银行的简称】 指定银行的简称，不填则不限制支付银行。银行简称列表详见： 银行类型（官方字段路径 <c>pay_activity_setting.payment_setting.limit_bank</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("limit_bank")]
    public string? LimitBank { get; set; }

    /// <summary>【订单优惠标记】 商户下单时需要传入相同的标记(goods_tag)，用户同时符合其他规则才能享受优惠（官方字段路径 <c>pay_activity_setting.payment_setting.goods_tags</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("goods_tags")]
    public List<string>? GoodsTags { get; set; }

}

/// <summary>创建会员有礼活动请求·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestPayActivitySetting
{
    /// <summary>【商户logo】 商户活动logo，会展示在支付结果页。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>pay_activity_setting.logo_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【支付结果页活动副标题】 支付结果页活动入口的副标题，商家填写的文案会直接展示给用户（官方字段路径 <c>pay_activity_setting.activity_second_title</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("activity_second_title")]
    public string? ActivitySecondTitle { get; set; }

    /// <summary>【活动曝光商户号】 活动曝光商户号（官方字段路径 <c>pay_activity_setting.mchid_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("mchid_list")]
    public List<string>? MchidList { get; set; }

    /// <summary>【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTERPAY时必填（官方字段路径 <c>pay_activity_setting.activate_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_setting")]
    public MemberCardActivityCreateRequestPayActivitySettingActivateSetting? ActivateSetting { get; set; }

    /// <summary>【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符合支付设置，才可能曝光（官方字段路径 <c>pay_activity_setting.payment_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_setting")]
    public MemberCardActivityCreateRequestPayActivitySettingPaymentSetting? PaymentSetting { get; set; }

}

/// <summary>创建会员有礼活动请求·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP·【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestMiniprogramActivitySettingAwardJumpDeploy
{
    /// <summary>【小程序AppID】 跳转的小程序AppID（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.mini_program_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序path】 跳转的小程序path（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.mini_program_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【跳转按钮文案】 领券页面跳转到小程序的按钮文案（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.button_text</c>，<b>必填</b>，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("button_text")]
    public string? ButtonText { get; set; }

}

/// <summary>创建会员有礼活动请求·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequestMiniprogramActivitySetting
{
    /// <summary>【指定领取渠道】 支付前会员有礼活动可通过配置outer_str来实现定向曝光： 1、outer_str 是商户自定义的字符串。只能录入数字及中英文／半角标点 匹配正则表达式: ^[0-9a-zA-Z\\u0000-\\u00FF\\u4e00-\\u9fa5]+$ 2、如果创建活动时不填写outer_st，默认活动在所有支付前场景生效； 3、如果活动配置了指定outer_str，则只有在拉起开卡组件时入参对应的outer_str，用户访问开卡组件时，才可…（官方字段路径 <c>miniprogram_activity_setting.outer_str</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("outer_str")]
    public List<string>? OuterStr { get; set; }

    /// <summary>【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_jump_deploy")]
    public MemberCardActivityCreateRequestMiniprogramActivitySettingAwardJumpDeploy? AwardJumpDeploy { get; set; }

}

/// <summary>创建会员有礼活动请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateRequest
{
    /// <summary>【会员卡ID】 商户创建微信会员卡模板成功后系统返回的会员卡ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【活动名称】 活动名称，用于内部管理，不对外展示。不超过10个字符（官方字段路径 <c>activity_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>活动类型 活动类型： - `NON_MEMBER_AFTERPAY`：支付后开卡有礼活动（说明：非会员用户在商户侧消费完成后，可曝光在微信支付成功页面的开卡有礼活动） - `MEMBER_AFTERPAY`：支付后老会员有礼活动 （会员用户在商户侧消费完成后，可曝光在微信支付成功页面的老会员有礼活动） - `NON_MEMBER_MINIPROGRAM`：支付前场景开卡有礼活动（说明：覆盖扫码/小程序场景，对于非会员用户，访问开卡组件时可看到开卡有礼活动信…（官方字段路径 <c>activity_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activity_type")]
    public string? ActivityType { get; set; }

    /// <summary>【活动开始时间】 活动开始时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【活动结束时间】 活动结束时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段（官方字段路径 <c>award_send_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_send_period")]
    public MemberCardActivityCreateRequestAwardSendPeriod? AwardSendPeriod { get; set; }

    /// <summary>【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次（官方字段路径 <c>stock_list</c>，<b>必填</b>，类型 <c>array[CouponInfo]</c>）。</summary>
    [JsonPropertyName("stock_list")]
    public List<MemberCardActivityCreateRequestStockListItem>? StockList { get; set; }

    /// <summary>【商户请求单号】 商户创建活动凭据号（建议格式：商户ID+日期+流水号），商户侧需保持唯一性，主要用于幂等判断，可包含英文字母，数字，｜，_，*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AFTERPAY或者MEMBER_AFTERPAY时，必填（官方字段路径 <c>pay_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("pay_activity_setting")]
    public MemberCardActivityCreateRequestPayActivitySetting? PayActivitySetting { get; set; }

    /// <summary>【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIPROGRAM或MEMBER_MINIPROGRAM时，可以选填（官方字段路径 <c>miniprogram_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("miniprogram_activity_setting")]
    public MemberCardActivityCreateRequestMiniprogramActivitySetting? MiniprogramActivitySetting { get; set; }

}

/// <summary>创建会员有礼活动响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间】 可选择活动时间指定部分日期发放。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseAwardSendPeriodAwardSendTimeItem
{
    /// <summary>【发放开始时间】 发放开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>award_send_period.award_send_time.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【发放结束时间】 发放结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>award_send_period.award_send_time.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>创建会员有礼活动响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 可选择指定时间的指定时段发放·【每日发放开始时间】 每日发放开始时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseAwardSendPeriodAwardSendDayTimeItemBeginDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>创建会员有礼活动响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 可选择指定时间的指定时段发放·【每日发放结束时间】 每日发放结束时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseAwardSendPeriodAwardSendDayTimeItemEndDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>创建会员有礼活动响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 可选择指定时间的指定时段发放。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseAwardSendPeriodAwardSendDayTimeItem
{
    /// <summary>【每日发放开始时间】 每日发放开始时间（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("begin_day_time")]
    public MemberCardActivityCreateResponseAwardSendPeriodAwardSendDayTimeItemBeginDayTime? BeginDayTime { get; set; }

    /// <summary>【每日发放结束时间】 每日发放结束时间（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("end_day_time")]
    public MemberCardActivityCreateResponseAwardSendPeriodAwardSendDayTimeItemEndDayTime? EndDayTime { get; set; }

}

/// <summary>创建会员有礼活动响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseAwardSendPeriod
{
    /// <summary>【指定时间】 可选择活动时间指定部分日期发放（官方字段路径 <c>award_send_period.award_send_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_time")]
    public List<MemberCardActivityCreateResponseAwardSendPeriodAwardSendTimeItem>? AwardSendTime { get; set; }

    /// <summary>【指定时间段】 可选择指定时间的指定时段发放（官方字段路径 <c>award_send_period.award_send_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_day_time")]
    public List<MemberCardActivityCreateResponseAwardSendPeriodAwardSendDayTimeItem>? AwardSendDayTime { get; set; }

}

/// <summary>创建会员有礼活动响应·【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseStockListItem
{
    /// <summary>【创建批次的商户号】 代金券或商家券的创建商户号（官方字段路径 <c>stock_list.stock_creator_mchid</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

    /// <summary>【券批次ID】 需要发放的代金券或商家券的批次ID（官方字段路径 <c>stock_list.stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>创建会员有礼活动响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE·【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponsePayActivitySettingActivateSettingActivateMiniprogram
{
    /// <summary>【跳转激活的AppID】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的AppID。如果同时配置了activate_url和activate_appid/path，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram.activate_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activate_appid")]
    public string? ActivateAppid { get; set; }

    /// <summary>【跳转激活的path】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的path（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram.activate_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("activate_path")]
    public string? ActivatePath { get; set; }

}

/// <summary>创建会员有礼活动响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponsePayActivitySettingActivateSetting
{
    /// <summary>【激活类型】 激活类型。可选取值：`AUTO_ACTIVATE` 用户领卡后微信后台自动将会员卡激活 - JUMP_ACTIVATE: 用户领卡后（官方字段路径 <c>pay_activity_setting.activate_setting.activate_type</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("activate_type")]
    public string? ActivateType { get; set; }

    /// <summary>【跳转激活的URL】 激活类型是跳转激活时，用户点击开卡后跳转到商家网页的URL。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_url</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_miniprogram")]
    public MemberCardActivityCreateResponsePayActivitySettingActivateSettingActivateMiniprogram? ActivateMiniprogram { get; set; }

}

/// <summary>创建会员有礼活动响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符·【支付模式】 不限制支付模式不填，限制选填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponsePayActivitySettingPaymentSettingPaymentMode
{
    /// <summary>【支付场景列表】 支付场景列表。可选取值：`APP` APP支付场景 - MICROPAY: 付款码支付 - PAP: 委托代扣 - （官方字段路径 <c>pay_activity_setting.payment_setting.payment_mode.payment_scene_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("payment_scene_list")]
    public List<string>? PaymentSceneList { get; set; }

}

/// <summary>创建会员有礼活动响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponsePayActivitySettingPaymentSetting
{
    /// <summary>【支付模式】 不限制支付模式不填，限制选填（官方字段路径 <c>pay_activity_setting.payment_setting.payment_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_mode")]
    public MemberCardActivityCreateResponsePayActivitySettingPaymentSettingPaymentMode? PaymentMode { get; set; }

    /// <summary>【指定银行的简称】 指定银行的简称，不填则不限制支付银行。银行简称列表详见： 银行类型（官方字段路径 <c>pay_activity_setting.payment_setting.limit_bank</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("limit_bank")]
    public string? LimitBank { get; set; }

    /// <summary>【订单优惠标记】 商户下单时需要传入相同的标记(goods_tag)，用户同时符合其他规则才能享受优惠（官方字段路径 <c>pay_activity_setting.payment_setting.goods_tags</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("goods_tags")]
    public List<string>? GoodsTags { get; set; }

}

/// <summary>创建会员有礼活动响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponsePayActivitySetting
{
    /// <summary>【商户logo】 商户活动logo，会展示在支付结果页。仅支持通过 图片上传API 接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>pay_activity_setting.logo_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【支付结果页活动副标题】 支付结果页活动入口的副标题，商家填写的文案会直接展示给用户（官方字段路径 <c>pay_activity_setting.activity_second_title</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("activity_second_title")]
    public string? ActivitySecondTitle { get; set; }

    /// <summary>【活动曝光商户号】 活动曝光商户号（官方字段路径 <c>pay_activity_setting.mchid_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("mchid_list")]
    public List<string>? MchidList { get; set; }

    /// <summary>【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTERPAY时必填（官方字段路径 <c>pay_activity_setting.activate_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_setting")]
    public MemberCardActivityCreateResponsePayActivitySettingActivateSetting? ActivateSetting { get; set; }

    /// <summary>【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符合支付设置，才可能曝光（官方字段路径 <c>pay_activity_setting.payment_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_setting")]
    public MemberCardActivityCreateResponsePayActivitySettingPaymentSetting? PaymentSetting { get; set; }

}

/// <summary>创建会员有礼活动响应·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP·【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseMiniprogramActivitySettingAwardJumpDeploy
{
    /// <summary>【小程序AppID】 跳转的小程序AppID（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.mini_program_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序path】 跳转的小程序path（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.mini_program_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【跳转按钮文案】 领券页面跳转到小程序的按钮文案（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.button_text</c>，<b>必填</b>，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("button_text")]
    public string? ButtonText { get; set; }

}

/// <summary>创建会员有礼活动响应·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponseMiniprogramActivitySetting
{
    /// <summary>【指定领取渠道】 支付前会员有礼活动可通过配置outer_str来实现定向曝光： 1、outer_str 是商户自定义的字符串。只能录入数字及中英文／半角标点 匹配正则表达式: ^[0-9a-zA-Z\\u0000-\\u00FF\\u4e00-\\u9fa5]+$ 2、如果创建活动时不填写outer_st，默认活动在所有支付前场景生效； 3、如果活动配置了指定outer_str，则只有在拉起开卡组件时入参对应的outer_str，用户访问开卡组件时，才可…（官方字段路径 <c>miniprogram_activity_setting.outer_str</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("outer_str")]
    public List<string>? OuterStr { get; set; }

    /// <summary>【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_jump_deploy")]
    public MemberCardActivityCreateResponseMiniprogramActivitySettingAwardJumpDeploy? AwardJumpDeploy { get; set; }

}

/// <summary>创建会员有礼活动响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityCreateResponse : WechatPayResponse
{
    /// <summary>【活动ID】 活动的当前状态（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【活动的当前状态】 活动的当前状态： - `CREATED`: 已创建，调用创建活动API成功后该活动状态 - `ONGOING`: 运行中，当活动正式开始的时候活动状态 - `TERMINATED`: 已终止，调用终止活动API接口活动状态变化 - `OVER_TIME`: 已过期，当前活动时间超过结束时间（官方字段路径 <c>activity_status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_status")]
    public string? ActivityStatus { get; set; }

    /// <summary>【创建时间】 创建时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【会员卡ID】 商户创建微信会员卡模板成功后系统返回的会员卡ID（官方字段路径 <c>card_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【活动名称】 活动名称，用于内部管理，不对外展示（官方字段路径 <c>activity_name</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>【活动类型】 活动类型： - `NON_MEMBER_AFTERPAY`：支付后开卡有礼活动（说明：非会员用户在商户侧消费完成后，可曝光在微信支付成功页面的开卡有礼活动） - `MEMBER_AFTERPAY`：支付后老会员有礼活动 （会员用户在商户侧消费完成后，可曝光在微信支付成功页面的老会员有礼活动） - `NON_MEMBER_MINIPROGRAM`：支付前场景开卡有礼活动（说明：覆盖扫码/小程序场景，对于非会员用户，访问开卡组件时可看到开卡有礼活…（官方字段路径 <c>activity_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_type")]
    public string? ActivityType { get; set; }

    /// <summary>【活动开始时间】 活动开始时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>begin_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【活动结束时间】 活动结束时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>end_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段（官方字段路径 <c>award_send_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_send_period")]
    public MemberCardActivityCreateResponseAwardSendPeriod? AwardSendPeriod { get; set; }

    /// <summary>【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次（官方字段路径 <c>stock_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("stock_list")]
    public List<MemberCardActivityCreateResponseStockListItem>? StockList { get; set; }

    /// <summary>【商户请求单号】 商户创建活动凭据号（建议格式：商户ID+日期+流水号），商户侧需保持唯一性，主要用于幂等判断，可包含英文字母，数字，｜，_，*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>out_request_no</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AFTERPAY或者MEMBER_AFTERPAY时，必填（官方字段路径 <c>pay_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("pay_activity_setting")]
    public MemberCardActivityCreateResponsePayActivitySetting? PayActivitySetting { get; set; }

    /// <summary>【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIPROGRAM或MEMBER_MINIPROGRAM时，可以选填（官方字段路径 <c>miniprogram_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("miniprogram_activity_setting")]
    public MemberCardActivityCreateResponseMiniprogramActivitySetting? MiniprogramActivitySetting { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【有效期内指定部分活动时间段】 在整个活动有效期内，指定更小范围的活动时间。 举。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendTimeItem
{
    /// <summary>【发放开始时间】 发放开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>data.award_send_period.award_send_time.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【发放结束时间】 发放结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>data.award_send_period.award_send_time.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设·【每日发放开始时间】 每日发放开始时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendDayTimeItemBeginDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>data.award_send_period.award_send_day_time.begin_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>data.award_send_period.award_send_day_time.begin_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>data.award_send_period.award_send_day_time.begin_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设·【每日发放结束时间】 每日发放结束时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendDayTimeItemEndDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>data.award_send_period.award_send_day_time.end_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>data.award_send_period.award_send_day_time.end_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>data.award_send_period.award_send_day_time.end_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendDayTimeItem
{
    /// <summary>【每日发放开始时间】 每日发放开始时间（官方字段路径 <c>data.award_send_period.award_send_day_time.begin_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("begin_day_time")]
    public MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendDayTimeItemBeginDayTime? BeginDayTime { get; set; }

    /// <summary>【每日发放结束时间】 每日发放结束时间（官方字段路径 <c>data.award_send_period.award_send_day_time.end_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("end_day_time")]
    public MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendDayTimeItemEndDayTime? EndDayTime { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemAwardSendPeriod
{
    /// <summary>【有效期内指定部分活动时间段】 在整个活动有效期内，指定更小范围的活动时间。 举例：活动整体时间在7月1日-31日，指定具体时间：7月7日、8日；7月17日、18日（官方字段路径 <c>data.award_send_period.award_send_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_time")]
    public List<MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendTimeItem>? AwardSendTime { get; set; }

    /// <summary>【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设置每日可用时间，取两者的交集时间段发券。 注意：若想设置指定时间段，则必须同时设置指定日期award_send_time，否则指定时间段的设置不会生效。 举例：活动整体时间在7月1日-31日，指定具体时间：7月7日、8日；7月17日、18日，指定具体时间段：上午9点-下午6点（官方字段路径 <c>data.award_send_period.award_send_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_day_time")]
    public List<MemberCardActivityListResponseDataItemAwardSendPeriodAwardSendDayTimeItem>? AwardSendDayTime { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemStockListItem
{
    /// <summary>【创建批次的商户号】 代金券或商家券的创建商户号。 注意：若制造的是代金券，则需要满足以下其一： （1）制券商户号=活动创建方商户号； （2）制券商户号已获得授权。若制造的是商家券，则需要满足以下关系：制券商户号=活动创建方商户号（官方字段路径 <c>data.stock_list.stock_creator_mchid</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

    /// <summary>【券批次ID】 活动发放的代金券或商家券的券批次ID。 注意： （1）针对活动类型为：支付后开卡有礼/支付后老会员有礼，若要发商家券，则商家券code分配方式不支持「实时分配」； （2）活动创建方的商户号，需要与会员卡归属品牌号（brand_id）绑定b-m关系（官方字段路径 <c>data.stock_list.stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE·【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemPayActivitySettingActivateSettingActivateMiniprogram
{
    /// <summary>【跳转激活的AppID】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的AppID。如果同时配置了activate_url和activate_appid/path，则优先跳转到小程序（官方字段路径 <c>data.pay_activity_setting.activate_setting.activate_miniprogram.activate_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activate_appid")]
    public string? ActivateAppid { get; set; }

    /// <summary>【跳转激活的path】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的path（官方字段路径 <c>data.pay_activity_setting.activate_setting.activate_miniprogram.activate_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("activate_path")]
    public string? ActivatePath { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemPayActivitySettingActivateSetting
{
    /// <summary>【激活类型】。可选取值：`AUTO_ACTIVATE` 一键激活；自动激活类型，在用户领卡后由微信自动激活 - JUMP_ACTIVAT（官方字段路径 <c>data.pay_activity_setting.activate_setting.activate_type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activate_type")]
    public string? ActivateType { get; set; }

    /// <summary>【跳转激活的URL】 激活类型是跳转激活时，用户点击开卡后跳转到商家网页的URL。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>data.pay_activity_setting.activate_setting.activate_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>data.pay_activity_setting.activate_setting.activate_miniprogram</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_miniprogram")]
    public MemberCardActivityListResponseDataItemPayActivitySettingActivateSettingActivateMiniprogram? ActivateMiniprogram { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符·【支付模式】 不限制支付模式不填，限制选填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemPayActivitySettingPaymentSettingPaymentMode
{
    /// <summary>【支付场景列表】。可选取值：`APP` APP支付场景 - MICROPAY: 付款码支付 - PAP: 委托代扣 - （官方字段路径 <c>data.pay_activity_setting.payment_setting.payment_mode.payment_scene_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("payment_scene_list")]
    public List<string>? PaymentSceneList { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemPayActivitySettingPaymentSetting
{
    /// <summary>【支付模式】 不限制支付模式不填，限制选填（官方字段路径 <c>data.pay_activity_setting.payment_setting.payment_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_mode")]
    public MemberCardActivityListResponseDataItemPayActivitySettingPaymentSettingPaymentMode? PaymentMode { get; set; }

    /// <summary>【指定银行的简称】 指定银行的简称，不填则不限制支付银行。银行简称列表详见： 银行类型（官方字段路径 <c>data.pay_activity_setting.payment_setting.limit_bank</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("limit_bank")]
    public string? LimitBank { get; set; }

    /// <summary>【订单优惠标记】 商户下单时需要传入相同的标记(goods_tag)，用户同时符合其他规则才能享受优惠（官方字段路径 <c>data.pay_activity_setting.payment_setting.goods_tags</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("goods_tags")]
    public List<string>? GoodsTags { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemPayActivitySetting
{
    /// <summary>【商户logo】 商户活动logo，会展示在支付结果页。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>data.pay_activity_setting.logo_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【支付结果页活动副标题】 支付结果页活动入口的副标题，商家填写的文案会直接展示给用户（官方字段路径 <c>data.pay_activity_setting.activity_second_title</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("activity_second_title")]
    public string? ActivitySecondTitle { get; set; }

    /// <summary>【活动曝光商户号】 活动曝光商户号。 注意：需要和会员卡归属的品牌号（brand_id）绑定b-m关系（官方字段路径 <c>data.pay_activity_setting.mchid_list</c>，<b>必填</b>，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("mchid_list")]
    public List<string>? MchidList { get; set; }

    /// <summary>【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTERPAY时必填（官方字段路径 <c>data.pay_activity_setting.activate_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_setting")]
    public MemberCardActivityListResponseDataItemPayActivitySettingActivateSetting? ActivateSetting { get; set; }

    /// <summary>【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符合支付设置，才可能曝光（官方字段路径 <c>data.pay_activity_setting.payment_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_setting")]
    public MemberCardActivityListResponseDataItemPayActivitySettingPaymentSetting? PaymentSetting { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP·【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemMiniprogramActivitySettingAwardJumpDeploy
{
    /// <summary>【小程序AppID】 跳转的小程序AppID（官方字段路径 <c>data.miniprogram_activity_setting.award_jump_deploy.mini_program_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序path】 跳转的小程序path（官方字段路径 <c>data.miniprogram_activity_setting.award_jump_deploy.mini_program_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【跳转按钮文案】 领券页面跳转到小程序的按钮文案（官方字段路径 <c>data.miniprogram_activity_setting.award_jump_deploy.button_text</c>，<b>必填</b>，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("button_text")]
    public string? ButtonText { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItemMiniprogramActivitySetting
{
    /// <summary>【指定领取渠道】 支付前会员有礼活动可通过配置outer_str来实现定向曝光： 1、outer_str 是商户自定义的字符串。只能录入数字及中英文／半角标点 匹配正则表达式: ^[0-9a-zA-Z\\u0000-\\u00FF\\u4e00-\\u9fa5]+$ 2、如果创建活动时不填写outer_st，默认活动在所有支付前场景生效； 3、如果活动配置了指定outer_str，则只有在拉起开卡组件时入参对应的outer_str，用户访问开卡组件时，才可…（官方字段路径 <c>data.miniprogram_activity_setting.outer_str</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("outer_str")]
    public List<string>? OuterStr { get; set; }

    /// <summary>【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置（官方字段路径 <c>data.miniprogram_activity_setting.award_jump_deploy</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_jump_deploy")]
    public MemberCardActivityListResponseDataItemMiniprogramActivitySettingAwardJumpDeploy? AwardJumpDeploy { get; set; }

}

/// <summary>查询会员有礼活动列表响应·【活动详情】 活动的详情，包括此资源的所有定义。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponseDataItem
{
    /// <summary>【活动ID】 活动的主键，唯一定义此资源的标识（官方字段路径 <c>data.activity_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【活动的当前状态】 动的当前状态： - `CREATED`: 已创建，调用创建活动API成功后该活动状态 - `ONGOING`: 运行中，当活动正式开始的时候活动状态 - `TERMINATED`: 已终止，调用终止活动API接口活动状态变 - `OVER_TIME`: 已过期，当前活动时间超过结束时间（官方字段路径 <c>data.activity_status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_status")]
    public string? ActivityStatus { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>data.create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>data.update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【会员卡ID】 商户创建微信会员卡模板成功后系统返回的会员卡ID（官方字段路径 <c>data.card_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【活动名称】 活动名称，用于内部管理，不对外展示。不超过10个字符（官方字段路径 <c>data.activity_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>【活动类型】 活动类型 - `NON_MEMBER_AFTERPAY`: 支付后开卡有礼活动（说明：非会员用户在商户侧消费完成后，可曝光在微信支付成功页面的开卡有礼活动） - `MEMBER_AFTERPAY`: 支付后老会员有礼活动 （会员用户在商户侧消费完成后，可曝光在微信支付成功页面的老会员有礼活动） - `NON_MEMBER_MINIPROGRAM`: 支付前场景开卡有礼活动（说明：覆盖扫码/小程序场景，对于非会员用户，访问开卡组件时可看到开卡有…（官方字段路径 <c>data.activity_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activity_type")]
    public string? ActivityType { get; set; }

    /// <summary>【活动开始时间】 活动开始时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>data.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【活动结束时间】 活动结束时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>data.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段（官方字段路径 <c>data.award_send_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_send_period")]
    public MemberCardActivityListResponseDataItemAwardSendPeriod? AwardSendPeriod { get; set; }

    /// <summary>【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次（官方字段路径 <c>data.stock_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("stock_list")]
    public List<MemberCardActivityListResponseDataItemStockListItem>? StockList { get; set; }

    /// <summary>【商户请求单号】 商户创建活动凭据号（建议格式：商户ID+日期+流水号），商户侧需保持唯一性，主要用于幂等判断，可包含英文字母，数字，｜，_，*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>data.out_request_no</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AFTERPAY或者MEMBER_AFTERPAY时，必填（官方字段路径 <c>data.pay_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("pay_activity_setting")]
    public MemberCardActivityListResponseDataItemPayActivitySetting? PayActivitySetting { get; set; }

    /// <summary>【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIPROGRAM或MEMBER_MINIPROGRAM时，可以选填（官方字段路径 <c>data.miniprogram_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("miniprogram_activity_setting")]
    public MemberCardActivityListResponseDataItemMiniprogramActivitySetting? MiniprogramActivitySetting { get; set; }

}

/// <summary>查询会员有礼活动列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityListResponse : WechatPayResponse
{
    /// <summary>【活动详情】 活动的详情，包括此资源的所有定义（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MemberCardActivityListResponseDataItem>? Data { get; set; }

    /// <summary>【请求资源起始位置】 该次请求资源的起始位置，同请求的（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【最大资源条数】 该次请求可返回的最大资源条数（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【资源总条数】 可选返回，资源总条数。当offset=0或者当前查询为空时应该返回总条数。如果系统性能允许时，建议返回（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【有效期内指定部分活动时间段】 在整个活动有效期内，指定更小范围的活动时间。 举。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseAwardSendPeriodAwardSendTimeItem
{
    /// <summary>【发放开始时间】 发放开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>award_send_period.award_send_time.begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【发放结束时间】 发放结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>award_send_period.award_send_time.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设·【每日发放开始时间】 每日发放开始时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseAwardSendPeriodAwardSendDayTimeItemBeginDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设·【每日发放结束时间】 每日发放结束时间。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseAwardSendPeriodAwardSendDayTimeItemEndDayTime
{
    /// <summary>【时】 小时，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.hours</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("hours")]
    public long? Hours { get; set; }

    /// <summary>【分】 分钟，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.minutes</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("minutes")]
    public long? Minutes { get; set; }

    /// <summary>【秒】 秒，比如时间09:34:06，则填hours填9，minutes填34，seconds填6（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time.seconds</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("seconds")]
    public long? Seconds { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段·【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseAwardSendPeriodAwardSendDayTimeItem
{
    /// <summary>【每日发放开始时间】 每日发放开始时间（官方字段路径 <c>award_send_period.award_send_day_time.begin_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("begin_day_time")]
    public MemberCardActivityDetailResponseAwardSendPeriodAwardSendDayTimeItemBeginDayTime? BeginDayTime { get; set; }

    /// <summary>【每日发放结束时间】 每日发放结束时间（官方字段路径 <c>award_send_period.award_send_day_time.end_day_time</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("end_day_time")]
    public MemberCardActivityDetailResponseAwardSendPeriodAwardSendDayTimeItemEndDayTime? EndDayTime { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseAwardSendPeriod
{
    /// <summary>【有效期内指定部分活动时间段】 在整个活动有效期内，指定更小范围的活动时间。 举例：活动整体时间在7月1日-31日，指定具体时间：7月7日、8日；7月17日、18日（官方字段路径 <c>award_send_period.award_send_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_time")]
    public List<MemberCardActivityDetailResponseAwardSendPeriodAwardSendTimeItem>? AwardSendTime { get; set; }

    /// <summary>【指定时间段】 基于“指定日期（award_send_time）”范围内，可再设置每日可用时间，取两者的交集时间段发券。 注意：若想设置指定时间段，则必须同时设置指定日期award_send_time，否则指定时间段的设置不会生效。 举例：活动整体时间在7月1日-31日，指定具体时间：7月7日、8日；7月17日、18日，指定具体时间段：上午9点-下午6点（官方字段路径 <c>award_send_period.award_send_day_time</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("award_send_day_time")]
    public List<MemberCardActivityDetailResponseAwardSendPeriodAwardSendDayTimeItem>? AwardSendDayTime { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseStockListItem
{
    /// <summary>【创建批次的商户号】 代金券或商家券的创建商户号。 注意：若制造的是代金券，则需要满足以下其一： （1）制券商户号=活动创建方商户号； （2）制券商户号已获得授权。若制造的是商家券，则需要满足以下关系：制券商户号=活动创建方商户号（官方字段路径 <c>stock_list.stock_creator_mchid</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("stock_creator_mchid")]
    public string? StockCreatorMchid { get; set; }

    /// <summary>【券批次ID】 活动发放的代金券或商家券的券批次ID。 注意： （1）针对活动类型为：支付后开卡有礼/支付后老会员有礼，若要发商家券，则商家券code分配方式不支持「实时分配」； （2）活动创建方的商户号，需要与会员卡归属品牌号（brand_id）绑定b-m关系（官方字段路径 <c>stock_list.stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE·【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponsePayActivitySettingActivateSettingActivateMiniprogram
{
    /// <summary>【跳转激活的AppID】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的AppID。如果同时配置了activate_url和activate_appid/path，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram.activate_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activate_appid")]
    public string? ActivateAppid { get; set; }

    /// <summary>【跳转激活的path】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的path（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram.activate_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("activate_path")]
    public string? ActivatePath { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTE。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponsePayActivitySettingActivateSetting
{
    /// <summary>【激活类型】。可选取值：`AUTO_ACTIVATE` 一键激活；自动激活类型，在用户领卡后由微信自动激活 - JUMP_ACTIVAT（官方字段路径 <c>pay_activity_setting.activate_setting.activate_type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activate_type")]
    public string? ActivateType { get; set; }

    /// <summary>【跳转激活的URL】 激活类型是跳转激活时，用户点击开卡后跳转到商家网页的URL。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>【跳转激活的小程序】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序。如果同时配置了activate_url和activate_miniprogram，则优先跳转到小程序（官方字段路径 <c>pay_activity_setting.activate_setting.activate_miniprogram</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_miniprogram")]
    public MemberCardActivityDetailResponsePayActivitySettingActivateSettingActivateMiniprogram? ActivateMiniprogram { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符·【支付模式】 不限制支付模式不填，限制选填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponsePayActivitySettingPaymentSettingPaymentMode
{
    /// <summary>【支付场景列表】。可选取值：`APP` APP支付场景 - MICROPAY: 付款码支付 - PAP: 委托代扣 - （官方字段路径 <c>pay_activity_setting.payment_setting.payment_mode.payment_scene_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("payment_scene_list")]
    public List<string>? PaymentSceneList { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF·【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponsePayActivitySettingPaymentSetting
{
    /// <summary>【支付模式】 不限制支付模式不填，限制选填（官方字段路径 <c>pay_activity_setting.payment_setting.payment_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_mode")]
    public MemberCardActivityDetailResponsePayActivitySettingPaymentSettingPaymentMode? PaymentMode { get; set; }

    /// <summary>【指定银行的简称】 指定银行的简称，不填则不限制支付银行。银行简称列表详见： 银行类型（官方字段路径 <c>pay_activity_setting.payment_setting.limit_bank</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("limit_bank")]
    public string? LimitBank { get; set; }

    /// <summary>【订单优惠标记】 商户下单时需要传入相同的标记(goods_tag)，用户同时符合其他规则才能享受优惠（官方字段路径 <c>pay_activity_setting.payment_setting.goods_tags</c>，选填，类型 <c>array[string(15)]</c>）。</summary>
    [JsonPropertyName("goods_tags")]
    public List<string>? GoodsTags { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AF。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponsePayActivitySetting
{
    /// <summary>【商户logo】 商户活动logo，会展示在支付结果页。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>pay_activity_setting.logo_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【支付结果页活动副标题】 支付结果页活动入口的副标题，商家填写的文案会直接展示给用户（官方字段路径 <c>pay_activity_setting.activity_second_title</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("activity_second_title")]
    public string? ActivitySecondTitle { get; set; }

    /// <summary>【活动曝光商户号】 活动曝光商户号。 注意：需要和会员卡归属的品牌号（brand_id）绑定b-m关系（官方字段路径 <c>pay_activity_setting.mchid_list</c>，<b>必填</b>，类型 <c>array</c>）。</summary>
    [JsonPropertyName("mchid_list")]
    public string? MchidList { get; set; }

    /// <summary>【激活方式设置】 设置会员卡激活方式，在活动类型是NON_MEMBER_AFTERPAY时必填（官方字段路径 <c>pay_activity_setting.activate_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activate_setting")]
    public MemberCardActivityDetailResponsePayActivitySettingActivateSetting? ActivateSetting { get; set; }

    /// <summary>【支付设置】 可以指定支付模式、支付方式、订单标识等，如果设置了，用户订单需要符合支付设置，才可能曝光（官方字段路径 <c>pay_activity_setting.payment_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payment_setting")]
    public MemberCardActivityDetailResponsePayActivitySettingPaymentSetting? PaymentSetting { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP·【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseMiniprogramActivitySettingAwardJumpDeploy
{
    /// <summary>【小程序AppID】 跳转的小程序AppID（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.mini_program_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppid { get; set; }

    /// <summary>【小程序path】 跳转的小程序path（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.mini_program_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("mini_program_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>【跳转按钮文案】 领券页面跳转到小程序的按钮文案（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy.button_text</c>，<b>必填</b>，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("button_text")]
    public string? ButtonText { get; set; }

}

/// <summary>查询会员有礼活动详情响应·【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIP。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponseMiniprogramActivitySetting
{
    /// <summary>【指定领取渠道】 支付前会员有礼活动可通过配置outer_str来实现定向曝光： 1、outer_str 是商户自定义的字符串。只能录入数字及中英文／半角标点 匹配正则表达式: ^[0-9a-zA-Z\\u0000-\\u00FF\\u4e00-\\u9fa5]+$ 2、如果创建活动时不填写outer_st，默认活动在所有支付前场景生效； 3、如果活动配置了指定outer_str，则只有在拉起开卡组件时入参对应的outer_str，用户访问开卡组件时，才可…（官方字段路径 <c>miniprogram_activity_setting.outer_str</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("outer_str")]
    public List<string>? OuterStr { get; set; }

    /// <summary>【领奖页面跳转配置】 用户获得优惠后，跳转到商家小程序页面配置（官方字段路径 <c>miniprogram_activity_setting.award_jump_deploy</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_jump_deploy")]
    public MemberCardActivityDetailResponseMiniprogramActivitySettingAwardJumpDeploy? AwardJumpDeploy { get; set; }

}

/// <summary>查询会员有礼活动详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityDetailResponse : WechatPayResponse
{
    /// <summary>【活动ID】 活动的主键，唯一定义此资源的标识（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【活动的当前状态】 活动的当前状态 - `CREATED`: 已创建，调用创建活动API成功后该活动状态 - `ONGOING`: 运行中，当活动正式开始的时候活动状态 - `TERMINATED`: 已终止，调用终止活动API接口活动状态变 - `OVER_TIME`: 活动已过期（官方字段路径 <c>activity_status</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activity_status")]
    public string? ActivityStatus { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【会员卡ID】 商户创建微信会员卡模板成功后系统返回的会员卡ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【活动名称】 活动名称，用于内部管理，不对外展示。不超过10个字符（官方字段路径 <c>activity_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>【活动类型】 活动类型 - `NON_MEMBER_AFTERPAY`: 支付后开卡有礼活动（说明：非会员用户在商户侧消费完成后，可曝光在微信支付成功页面的开卡有礼活动） - `MEMBER_AFTERPAY`: 支付后老会员有礼活动 （会员用户在商户侧消费完成后，可曝光在微信支付成功页面的老会员有礼活动） - `NON_MEMBER_MINIPROGRAM`: 支付前场景开卡有礼活动（说明：覆盖扫码/小程序场景，对于非会员用户，访问开卡组件时可看到开卡有…（官方字段路径 <c>activity_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activity_type")]
    public string? ActivityType { get; set; }

    /// <summary>【活动开始时间】 活动开始时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>begin_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public string? BeginTime { get; set; }

    /// <summary>【会员卡背景图】 活动结束时间，最长可以配置1年内的活动，活动有效期最长90天 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时…（官方字段路径 <c>end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【发放时间段】 活动时段，可选择活动有效期内部分日期或部分时间段（官方字段路径 <c>award_send_period</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("award_send_period")]
    public MemberCardActivityDetailResponseAwardSendPeriod? AwardSendPeriod { get; set; }

    /// <summary>【优惠券列表】 发放的优惠券，一个活动最多支持发放3个批次（官方字段路径 <c>stock_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("stock_list")]
    public List<MemberCardActivityDetailResponseStockListItem>? StockList { get; set; }

    /// <summary>【商户请求单号】 商户创建活动凭据号（建议格式：商户ID+日期+流水号），商户侧需保持唯一性，主要用于幂等判断，可包含英文字母，数字，｜，_，*，-等内容，不允许出现其他不合法符号（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【支付后活动设置】 支付后活动的相关配置。当活动类型是NON_MEMBER_AFTERPAY或者MEMBER_AFTERPAY时，必填（官方字段路径 <c>pay_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("pay_activity_setting")]
    public MemberCardActivityDetailResponsePayActivitySetting? PayActivitySetting { get; set; }

    /// <summary>【小程序活动设置】 小程序活动设置，当活动类型是NON_MEMBER_MINIPROGRAM或MEMBER_MINIPROGRAM时，可以选填（官方字段路径 <c>miniprogram_activity_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("miniprogram_activity_setting")]
    public MemberCardActivityDetailResponseMiniprogramActivitySetting? MiniprogramActivitySetting { get; set; }

}

/// <summary>终止会员有礼活动响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553381"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardActivity")]
public class MemberCardActivityTerminateResponse : WechatPayResponse
{
    /// <summary>【活动ID】 活动的主键，唯一定义此资源的标识（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【终止时间】 终止时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35.120+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>terminate_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("terminate_time")]
    public string? TerminateTime { get; set; }

}
