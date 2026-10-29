// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingMemberCardOpen;

/// <summary>设置用户会员卡权益请求·【会员专属咨询信息】 设置商家联系人员的名字、头像和联系方式。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551213"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCodeRightsRequestGuide
{
    /// <summary>【联系人名字】 设置商家咨询联系人的名称（官方字段路径 <c>guide.staff_name</c>，<b>必填</b>，类型 <c>string(12)</c>）。</summary>
    [JsonPropertyName("staff_name")]
    public string? StaffName { get; set; }

    /// <summary>【联系人头像】 设置商家咨询联系人的头像。仅支持通过 图片上传API 接口获取的图片URL地址。 1、logo大小需为120像素*120像素 。2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>guide.head_image_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("head_image_url")]
    public string? HeadImageUrl { get; set; }

    /// <summary>【联系方式】 展示说明商家咨询联系人的联系方式，例如“微信号”“企业微信号”（官方字段路径 <c>guide.contact_information_name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("contact_information_name")]
    public string? ContactInformationName { get; set; }

    /// <summary>【联系信息值】 展示说明商家咨询联系人的联系方式信息，例如“weixin123”“135266664”（官方字段路径 <c>guide.contact_information_value</c>，选填，类型 <c>string(14)</c>）。</summary>
    [JsonPropertyName("contact_information_value")]
    public string? ContactInformationValue { get; set; }

    /// <summary>【联系电话】 商家咨询联系人的联系电话（官方字段路径 <c>guide.phone_number</c>，选填，类型 <c>string(13)</c>）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

}

/// <summary>设置用户会员卡权益请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551213"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCodeRightsRequest
{
    /// <summary>【变更前的会员积分值】 变更前的用户的积分值（官方字段路径 <c>before_bonus_value</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("before_bonus_value")]
    public long? BeforeBonusValue { get; set; }

    /// <summary>【变更后的会员积分值】 变更后的用户最新积分值，该值会展示在会员卡详页上（官方字段路径 <c>bonus_value</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("bonus_value")]
    public long? BonusValue { get; set; }

    /// <summary>【会员积分变动值】 本次变更的积分值，等于bonus_value-before_bonus_value，正数表示增加，负数表示减小（官方字段路径 <c>add_bonus_value</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("add_bonus_value")]
    public long? AddBonusValue { get; set; }

    /// <summary>【商户请求单号】 商户凭据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【是否发送积分变更通知】 积分变更时是否触发系统模板消息，默认为true（官方字段路径 <c>need_inform_bonus</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_inform_bonus")]
    public bool? NeedInformBonus { get; set; }

    /// <summary>【会员专享价文案】 用户会员卡详情页会员优惠栏目中的会员专享价文案（官方字段路径 <c>member_price_word</c>，选填，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("member_price_word")]
    public string? MemberPriceWord { get; set; }

    /// <summary>【会员发票跳转文案】 发票栏跳转小程序的引导文案（官方字段路径 <c>fapiao_jump_word</c>，选填，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_word")]
    public string? FapiaoJumpWord { get; set; }

    /// <summary>【会员专属咨询信息】 设置商家联系人员的名字、头像和联系方式（官方字段路径 <c>guide</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("guide")]
    public MemberCardUserCodeRightsRequestGuide? Guide { get; set; }

}

/// <summary>设置会员卡模板权益项请求·【会员积分】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551219"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateRightsRequestBonus
{
    /// <summary>【会员初始积分值】 用户开卡后的初始积分（官方字段路径 <c>bonus.init_bonus</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("init_bonus")]
    public long? InitBonus { get; set; }

    /// <summary>【积分值文案】 展示在积分上方的文案（官方字段路径 <c>bonus.bonus_value_word</c>，<b>必填</b>，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("bonus_value_word")]
    public string? BonusValueWord { get; set; }

    /// <summary>【积分价值项标题】 积分价值项文案上方的标题（官方字段路径 <c>bonus.bonus_cost_title</c>，选填，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("bonus_cost_title")]
    public string? BonusCostTitle { get; set; }

    /// <summary>【积分价值项文案】 描述积分推荐优惠（官方字段路径 <c>bonus.bonus_cost_word</c>，<b>必填</b>，类型 <c>string(14)</c>）。</summary>
    [JsonPropertyName("bonus_cost_word")]
    public string? BonusCostWord { get; set; }

    /// <summary>【积分跳转文案】 会员积分栏右上角的文案（官方字段路径 <c>bonus.bonus_jump_word</c>，选填，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("bonus_jump_word")]
    public string? BonusJumpWord { get; set; }

    /// <summary>【积分跳转AppID】 会员积分跳转小程序的AppID（官方字段路径 <c>bonus.bonus_jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bonus_jump_appid")]
    public string? BonusJumpAppid { get; set; }

    /// <summary>【积分跳转path】 积分跳转小程序的path（官方字段路径 <c>bonus.bonus_jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bonus_jump_path")]
    public string? BonusJumpPath { get; set; }

    /// <summary>【自助积分跳转AppID】 自助积分跳转的小程序AppID。只有填写了此AppID，才会展示“自助积分”按钮（官方字段路径 <c>bonus.bonus_support_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bonus_support_appid")]
    public string? BonusSupportAppid { get; set; }

    /// <summary>【自助积分跳转path】 自助积分跳转小程序的path（官方字段路径 <c>bonus.bonus_support_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bonus_support_path")]
    public string? BonusSupportPath { get; set; }

}

/// <summary>设置会员卡模板权益项请求·【会员优惠】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551219"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateRightsRequestFavor
{
    /// <summary>【是否展示优惠券】 会员优惠栏分为左右两边，这里选择是否展示左边的优惠券（官方字段路径 <c>favor.show_coupon</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_coupon")]
    public bool? ShowCoupon { get; set; }

    /// <summary>【会员专享价文案】 用户会员卡详情页会员优惠栏目中的会员专享价文案（官方字段路径 <c>favor.member_price_word</c>，选填，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("member_price_word")]
    public string? MemberPriceWord { get; set; }

    /// <summary>【会员专享价跳转AppID】 会员专享价跳转小程序的AppID（官方字段路径 <c>favor.member_price_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("member_price_appid")]
    public string? MemberPriceAppid { get; set; }

    /// <summary>【会员专享价跳转path】 会员专享价跳转小程序的path（官方字段路径 <c>favor.member_price_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("member_price_path")]
    public string? MemberPricePath { get; set; }

    /// <summary>【会员专享价标题】 用户会员卡详情页会员优惠栏目中的会员专享价标题，不填默认为MEMBER_PRICE（官方字段路径 <c>favor.member_price_title</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("member_price_title")]
    public string? MemberPriceTitle { get; set; }

    /// <summary>【会员优惠栏目名称】 用户会员卡详情页会员优惠栏目的名称，不填默认为FAVOR（官方字段路径 <c>favor.favor_module_title</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("favor_module_title")]
    public string? FavorModuleTitle { get; set; }

}

/// <summary>设置会员卡模板权益项请求·【会员发票】 设置发票栏目的文案、跳转小程序。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551219"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateRightsRequestFapiao
{
    /// <summary>【发票跳转文案】 发票栏跳转小程序的引导文案（官方字段路径 <c>fapiao.fapiao_jump_word</c>，选填，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_word")]
    public string? FapiaoJumpWord { get; set; }

    /// <summary>【发票跳转AppID】 发票跳转小程序的AppID（官方字段路径 <c>fapiao.fapiao_jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_appid")]
    public string? FapiaoJumpAppid { get; set; }

    /// <summary>【发票跳转path】 发票跳转小程序的path（官方字段路径 <c>fapiao.fapiao_jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_path")]
    public string? FapiaoJumpPath { get; set; }

}

/// <summary>设置会员卡模板权益项请求·【会员专属咨询信息】 设置商家联系人员的名字、头像和联系方式。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551219"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateRightsRequestGuide
{
    /// <summary>【联系人名字】 设置商家咨询联系人的名称（官方字段路径 <c>guide.staff_name</c>，<b>必填</b>，类型 <c>string(12)</c>）。</summary>
    [JsonPropertyName("staff_name")]
    public string? StaffName { get; set; }

    /// <summary>【联系人头像】 设置商家咨询联系人的头像。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>guide.head_image_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("head_image_url")]
    public string? HeadImageUrl { get; set; }

    /// <summary>【联系方式】 展示说明商家咨询联系人的联系方式，例如“微信号”“企业微信号”（官方字段路径 <c>guide.contact_information_name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("contact_information_name")]
    public string? ContactInformationName { get; set; }

    /// <summary>【联系信息值】 展示说明商家咨询联系人的联系方式信息，例如“weixin123”“135266664”（官方字段路径 <c>guide.contact_information_value</c>，选填，类型 <c>string(14)</c>）。</summary>
    [JsonPropertyName("contact_information_value")]
    public string? ContactInformationValue { get; set; }

    /// <summary>【联系电话】 商家咨询联系人的联系电话（官方字段路径 <c>guide.phone_number</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

}

/// <summary>设置会员卡模板权益项请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551219"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateRightsRequest
{
    /// <summary>【是否展示会员积分】 会员卡详情页中是否要展示会员积分栏，不填默认false（官方字段路径 <c>show_bonus</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_bonus")]
    public bool? ShowBonus { get; set; }

    /// <summary>【是否展示会员优惠】 会员卡详情页中是否要展示会员优惠栏，不填默认false（官方字段路径 <c>show_favor</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_favor")]
    public bool? ShowFavor { get; set; }

    /// <summary>【会员积分】（官方字段路径 <c>bonus</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("bonus")]
    public MemberCardTemplateRightsRequestBonus? Bonus { get; set; }

    /// <summary>【会员优惠】（官方字段路径 <c>favor</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("favor")]
    public MemberCardTemplateRightsRequestFavor? Favor { get; set; }

    /// <summary>【是否展示会员发票】 会员卡详情页中是否要展示发票栏，不填默认false（官方字段路径 <c>show_fapiao</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_fapiao")]
    public bool? ShowFapiao { get; set; }

    /// <summary>【会员发票】 设置发票栏目的文案、跳转小程序（官方字段路径 <c>fapiao</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fapiao")]
    public MemberCardTemplateRightsRequestFapiao? Fapiao { get; set; }

    /// <summary>【是否展示会员专属咨询】 是否在用户的会员卡详情页展示联系客服信息，不填默认false（官方字段路径 <c>show_guide</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("show_guide")]
    public bool? ShowGuide { get; set; }

    /// <summary>【会员专属咨询信息】 设置商家联系人员的名字、头像和联系方式（官方字段路径 <c>guide</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("guide")]
    public MemberCardTemplateRightsRequestGuide? Guide { get; set; }

}

/// <summary>提交会员卡积分请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551350"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardBonusCommitRequest
{
    /// <summary>【商户订单号】 商户订单号是商户系统进行下单的内部订单号（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【直连商户号】 直连商户的商户号，由微信支付生成并下发。直连模式填入 （官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【会员积分值】 本次积分变更值，本次支付订单完成后在支付结果页和弹窗展示的积分变更值，变更值需要为正数（官方字段路径 <c>bonus_value</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("bonus_value")]
    public long? BonusValue { get; set; }

}

/// <summary>预下单会员卡积分请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012595808"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardBonusPrepareRequest
{
    /// <summary>【商户订单号】 商户订单号是商户系统进行下单的内部订单号（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【直连商户号】 直连商户的商户号，由微信支付生成并下发（官方字段路径 <c>mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【会员积分值】 本次积分变更值，本次支付订单完成后在支付结果页和弹窗展示的积分变更值，变更值需要为正数（官方字段路径 <c>bonus_value</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("bonus_value")]
    public long? BonusValue { get; set; }

}
