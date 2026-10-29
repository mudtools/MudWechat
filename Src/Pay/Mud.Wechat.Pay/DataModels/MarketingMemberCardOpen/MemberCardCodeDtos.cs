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

/// <summary>会员卡卡code预存请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545728"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardCodeDepositRequest
{
    /// <summary>【会员卡code】 商家定义的会员卡code，如会员卡code分配类型为商家预存code，则用户领卡时将从预存code中随机分配给会员（官方字段路径 <c>code</c>，选填，类型 <c>array[string(32)]</c>）。</summary>
    [JsonPropertyName("code")]
    public List<string>? Code { get; set; }

}

/// <summary>会员卡卡code预存响应·【导入结果】 导入结果。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545728"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardCodeDepositResponseDataItem
{
    /// <summary>【会员卡code】 商户导入的会员卡code（官方字段路径 <c>data.code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>【导入结果】 当前code的导入结果 - `SUCCESS`: 成功 - `DUPLICATE`: 重复 - `FAIL`: 失败（官方字段路径 <c>data.result</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

}

/// <summary>会员卡卡code预存响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545728"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardCodeDepositResponse : WechatPayResponse
{
    /// <summary>【导入结果】 导入结果（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MemberCardCodeDepositResponseDataItem>? Data { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【平台提供的通用开卡信息字段】 包含性别、手机、头像、昵称、姓名等基本信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseUserInformationCommonFieldListItem
{
    /// <summary>【平台提供的通用开卡信息字段名称】 平台提供了一些通用的开卡字段供开发者选用 - `USER_FORM_FLAG_MOBILE`: 手机号 - `USER_FORM_FLAG_SEX`: 性别 - `USER_FORM_FLAG_NAME`: 姓名 - `USER_FORM_FLAG_BIRTHDAY`: 生日 - `USER_FORM_FLAG_ADDRESS`: 地址 - `USER_FORM_FLAG_EMAIL`: 邮箱 - `USER_FORM_…（官方字段路径 <c>user_information.common_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 平台提供的通用开卡信息字段，用户在开卡时填写的信息（官方字段路径 <c>user_information.common_field_list.value</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【商户自定义的开卡信息字段】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseUserInformationCustomFieldListItem
{
    /// <summary>【字段名称】 商户自定义的开卡信息字段名称（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 商户自定义的开卡信息字段值，用户在开卡时需填写。填写项目为单选时的返回（官方字段路径 <c>user_information.custom_field_list.value</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

    /// <summary>【字段值列表】 商户自定义的开卡信息字段值，用户在开卡时需填写。填写项目为多选时的返回 特殊规则：列表最多支持10个，单个列表限制8个字符（官方字段路径 <c>user_information.custom_field_list.value_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("value_list")]
    public List<string>? ValueList { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 包含性别、手机、头像、昵称、姓名等基本信息字段（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<MemberCardUserCardResponseUserInformationCommonFieldListItem>? CommonFieldList { get; set; }

    /// <summary>【商户自定义的开卡信息字段】（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MemberCardUserCardResponseUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容·【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置·【内容模版设置】 用户的服务内容模版设置，这些设置会展示在用户的会员卡详情页。可·【服务内容列表】 当前服务状态下，可以设置的内容字段列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateDataFieldListItem
{
    /// <summary>【内容字段id】 当前服务状态下，可以设置的内容字段id（官方字段路径 <c>service_modules.state_setting.content_template_data.field_list.field_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("field_id")]
    public string? FieldId { get; set; }

    /// <summary>【内容字段值】 服务状态字段的值（官方字段路径 <c>service_modules.state_setting.content_template_data.field_list.field_value</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("field_value")]
    public string? FieldValue { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容·【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置·【内容模版设置】 用户的服务内容模版设置，这些设置会展示在用户的会员卡详情页。可·【操作设置】 服务状态对应的操作设置，可配置跳转文案和跳转小程序路径 需配置跳转·【跳转小程序】 服务状态指定跳转小程序。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateDataActionSettingJumpMiniprogram
{
    /// <summary>【小程序AppID】 小程序的AppID（官方字段路径 <c>service_modules.state_setting.content_template_data.action_setting.jump_miniprogram.appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序path】 指定跳转小程序页面的path（官方字段路径 <c>service_modules.state_setting.content_template_data.action_setting.jump_miniprogram.path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容·【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置·【内容模版设置】 用户的服务内容模版设置，这些设置会展示在用户的会员卡详情页。可·【操作设置】 服务状态对应的操作设置，可配置跳转文案和跳转小程序路径 需配置跳转。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateDataActionSetting
{
    /// <summary>【操作名称】 操作名称（官方字段路径 <c>service_modules.state_setting.content_template_data.action_setting.action_name</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("action_name")]
    public string? ActionName { get; set; }

    /// <summary>【跳转小程序】 服务状态指定跳转小程序（官方字段路径 <c>service_modules.state_setting.content_template_data.action_setting.jump_miniprogram</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_miniprogram")]
    public MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateDataActionSettingJumpMiniprogram? JumpMiniprogram { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容·【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置·【内容模版设置】 用户的服务内容模版设置，这些设置会展示在用户的会员卡详情页。可。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateData
{
    /// <summary>【服务内容列表】 当前服务状态下，可以设置的内容字段列表（官方字段路径 <c>service_modules.state_setting.content_template_data.field_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("field_list")]
    public List<MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateDataFieldListItem>? FieldList { get; set; }

    /// <summary>【操作设置】 服务状态对应的操作设置，可配置跳转文案和跳转小程序路径 需配置跳转文案及跳转小程序路径时必填（官方字段路径 <c>service_modules.state_setting.content_template_data.action_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("action_setting")]
    public MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateDataActionSetting? ActionSetting { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容·【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置·【消息模版设置】 公众号模版消息设置。部分服务项支持给用户发送触达消息，商户可通·【模板消息字段列表】 模板消息字段列表。商户在此设置模板消息每个字段需展示的内容。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItemStateSettingMessageTemplateSettingFieldListItem
{
    /// <summary>【消息字段id】 公众号模版消息字段id，相当于字段的id（官方字段路径 <c>service_modules.state_setting.message_template_setting.field_list.field_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("field_id")]
    public string? FieldId { get; set; }

    /// <summary>【消息字段的值】 公众号模版消息字段的值，会作为消息内容展示给用户（官方字段路径 <c>service_modules.state_setting.message_template_setting.field_list.field_value</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("field_value")]
    public string? FieldValue { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容·【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置·【消息模版设置】 公众号模版消息设置。部分服务项支持给用户发送触达消息，商户可通。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItemStateSettingMessageTemplateSetting
{
    /// <summary>【模版消息id】 公众号模版消息的id（官方字段路径 <c>service_modules.state_setting.message_template_setting.message_template_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("message_template_id")]
    public string? MessageTemplateId { get; set; }

    /// <summary>【模板消息字段列表】 模板消息字段列表。商户在此设置模板消息每个字段需展示的内容 特殊规则：限制最多可提交9条（官方字段路径 <c>service_modules.state_setting.message_template_setting.field_list</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("field_list")]
    public List<MemberCardUserCardResponseServiceModulesItemStateSettingMessageTemplateSettingFieldListItem>? FieldList { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容·【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItemStateSetting
{
    /// <summary>【状态id】 状态的id。一个服务项通常包含多个状态，商户可根据服务流程选择其中的状态进行设置，模版内容展示在用户的会员卡详情页，并支持通过公众号给用户发送触达消息（官方字段路径 <c>service_modules.state_setting.state_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("state_id")]
    public string? StateId { get; set; }

    /// <summary>【内容模版设置】 用户的服务内容模版设置，这些设置会展示在用户的会员卡详情页。可设置状态的内容字段、按钮等（官方字段路径 <c>service_modules.state_setting.content_template_data</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("content_template_data")]
    public MemberCardUserCardResponseServiceModulesItemStateSettingContentTemplateData? ContentTemplateData { get; set; }

    /// <summary>【消息模版设置】 公众号模版消息设置。部分服务项支持给用户发送触达消息，商户可通过模版id选择模版消息样式，并设置字段值。API调用成功后会通过微信公众号自动给用户发送此模版消息。不填则默认不发消息（官方字段路径 <c>service_modules.state_setting.message_template_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("message_template_setting")]
    public MemberCardUserCardResponseServiceModulesItemStateSettingMessageTemplateSetting? MessageTemplateSetting { get; set; }

}

/// <summary>查询用户会员卡信息响应·【用户当前服务项】 用户当前的会员服务项内容。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseServiceModulesItem
{
    /// <summary>【会员服务项id】 会员服务项的唯一识别ID。微信内部运营人员创建服务项后，会生成一个服务项id，商户可通过服务项id查询服务项的配置信息及用途（官方字段路径 <c>service_modules.service_module_id</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("service_module_id")]
    public string? ServiceModuleId { get; set; }

    /// <summary>【服务状态设置】 用户的服务状态设置，这些设置会展示在用户的会员卡详情页。可设置状态的内容字段、按钮、消息内容等（官方字段路径 <c>service_modules.state_setting</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("state_setting")]
    public MemberCardUserCardResponseServiceModulesItemStateSetting? StateSetting { get; set; }

}

/// <summary>查询用户会员卡信息响应·【客服联系信息】 设置商家联系人员的名字、头像和联系方式。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponseGuide
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

/// <summary>查询用户会员卡信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardResponse : WechatPayResponse
{
    /// <summary>【会员卡模板ID】 商户创建微信会员卡模板成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【会员卡code】 会员在card_id下的唯一标识，用户领取会员卡后获得的code（官方字段路径 <c>code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code")]
    public new string? Code { get; set; }

    /// <summary>【用户标识】 用户在会员卡绑定品牌AppID下的唯一标识，如通过手机号同步会员身份且用户未领取到卡包，无法查询该字段（官方字段路径 <c>openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【会员卡编号】 在用户会员卡上展示的会员卡编号，默认使用会员卡code作为membership_number。只能录入数字／英文／半角标点。商家可通过修改用户单张会员卡信息更新（官方字段路径 <c>membership_number</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("membership_number")]
    public string? MembershipNumber { get; set; }

    /// <summary>【用户等级】 用户会员等级（官方字段路径 <c>level</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>【微信昵称】 用户的微信昵称，如通过手机号同步会员身份且用户未领取到卡包，无法查询该字段（官方字段路径 <c>nickname</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>【微信头像URL】 用户的微信头像URL，如通过手机号同步会员身份且用户未领取到卡包，无法查询该字段（官方字段路径 <c>head_image_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("head_image_url")]
    public string? HeadImageUrl { get; set; }

    /// <summary>【会员卡背景图】 商家给当前用户设置的会员卡背景图。仅支持通过《 图片上传API》接口获取的图片URL地址。支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>background_picture_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("background_picture_url")]
    public string? BackgroundPictureUrl { get; set; }

    /// <summary>【用户储值余额】 用户储值的最新余额，单位分（官方字段路径 <c>balance</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>【用户会员卡状态】 用户当前的卡状态 - `NOT_ACTIVATE`: 未激活 - `EFFECTIVE`: 生效中 - `EXPIRE`: 已过期 - `UNAVAILABLE`: 已失效 - `DELETE`: 已删除 - `IMPORTED`: 会员身份已通过手机号或OpenID导入，但用户未领取到微信卡包（官方字段路径 <c>user_card_status</c>，选填，类型 <c>string（16）</c>）。</summary>
    [JsonPropertyName("user_card_status")]
    public string? UserCardStatus { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 用户开卡时填写的个人信息（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public MemberCardUserCardResponseUserInformation? UserInformation { get; set; }

    /// <summary>【用户当前积分】 用户当前的积分值（官方字段路径 <c>bonus_value</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("bonus_value")]
    public long? BonusValue { get; set; }

    /// <summary>【用户当前服务项】 用户当前的会员服务项内容（官方字段路径 <c>service_modules</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("service_modules")]
    public List<MemberCardUserCardResponseServiceModulesItem>? ServiceModules { get; set; }

    /// <summary>【会员专享价文案】 用户会员卡详情页会员优惠栏目中的会员专享价文案（官方字段路径 <c>member_price_word</c>，选填，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("member_price_word")]
    public string? MemberPriceWord { get; set; }

    /// <summary>【发票跳转文案】 发票栏跳转小程序的引导文案（官方字段路径 <c>fapiao_jump_word</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_word")]
    public string? FapiaoJumpWord { get; set; }

    /// <summary>【客服联系信息】 设置商家联系人员的名字、头像和联系方式（官方字段路径 <c>guide</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("guide")]
    public MemberCardUserCardResponseGuide? Guide { get; set; }

}

/// <summary>修改用户会员卡信息请求·【用户储值变更信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549292"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardUpdateRequestBalanceInformation
{
    /// <summary>【更新前用户储值余额】 用户储值的更新前余额，单位分（官方字段路径 <c>balance_information.before_balance</c>，<b>必填</b>，类型 <c>uint64</c>）。</summary>
    [JsonPropertyName("before_balance")]
    public string? BeforeBalance { get; set; }

    /// <summary>【更新后用户储值余额】 用户储值的更新后余额，单位分（官方字段路径 <c>balance_information.balance</c>，<b>必填</b>，类型 <c>uint64</c>）。</summary>
    [JsonPropertyName("balance")]
    public string? Balance { get; set; }

    /// <summary>【本次余额变动值】 用户储值余额本次的变动值，单位分，等于balance减去before_balance，传入正数表示余额增加，传入负数表示余额减少（官方字段路径 <c>balance_information.add_balance</c>，<b>必填</b>，类型 <c>uint64</c>）。</summary>
    [JsonPropertyName("add_balance")]
    public string? AddBalance { get; set; }

    /// <summary>【储值消耗记录】 商家自定义储值金额消耗记录，不超过30个字（官方字段路径 <c>balance_information.balance_remark</c>，选填，类型 <c>string(30)</c>）。</summary>
    [JsonPropertyName("balance_remark")]
    public string? BalanceRemark { get; set; }

}

/// <summary>修改用户会员卡信息请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549292"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserCardUpdateRequest
{
    /// <summary>【会员卡编号】 在用户会员卡上展示的会员卡编号，如不填写，默认使用会员卡code作为membership_number。只能录入数字／英文／半角标点（官方字段路径 <c>membership_number</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("membership_number")]
    public string? MembershipNumber { get; set; }

    /// <summary>【会员卡背景】 商家自定义会员卡背景图。仅支持通过《 图片上传API》接口获取的图片URL地址。支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>background_picture_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("background_picture_url")]
    public string? BackgroundPictureUrl { get; set; }

    /// <summary>【会员等级】 用户会员等级（官方字段路径 <c>level</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>【商户请求单号】 商户据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【用户储值变更信息】（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public MemberCardUserCardUpdateRequestBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【是否发送储值变更通知】 储值变更时是否触发系统模板消息，默认为true（官方字段路径 <c>need_inform_balance</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_inform_balance")]
    public bool? NeedInformBalance { get; set; }

    /// <summary>【是否发送等级变更通知】 等级变更时是否触发系统模板消息，默认为true（官方字段路径 <c>need_inform_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_inform_level")]
    public bool? NeedInformLevel { get; set; }

}

/// <summary>作废用户会员卡请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549677"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardCodeUnavailableRequest
{
    /// <summary>【失效理由】 将用户的会员卡置为失效的理由（官方字段路径 <c>reason</c>，选填，类型 <c>string(30)</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

}

/// <summary>确认用户入会请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4024293873"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardCodeConfirmRequest
{
    /// <summary>【入会失败原因】 用户入会失败时的原因描述，当user_card_confirm_state为CREATE_CARD_FAIL时填写（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【会员卡开卡状态】 商户完成内部开卡动作后回传的会员卡开卡状态。可选取值：`DEFAULT` 默认值，无实际业务含义 / `CREATE_CARD_SUCCESS` 开卡成功 / `CREATE_CARD_FAIL` 开卡失败 / `CREATE_CARD_ALREADY_EXISTS` 开卡成功但已在其他渠道入会（官方字段路径 <c>user_card_confirm_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_card_confirm_state")]
    public string? UserCardConfirmState { get; set; }

}

/// <summary>确认用户入会响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4024293873"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardCodeConfirmResponse : WechatPayResponse
{
    /// <summary>【会员卡模板ID】 商户创建微信会员卡模板成功后系统返回的会员卡模板ID，可在创建会员卡接口的应答中获取，也可通过查询会员卡列表接口获取（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【会员卡code】 会员在一个card_id下的唯一标志，用户领取会员卡后获得的code（官方字段路径 <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code")]
    public new string? Code { get; set; }

}
