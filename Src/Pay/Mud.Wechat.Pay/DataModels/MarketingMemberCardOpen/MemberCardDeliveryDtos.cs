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

/// <summary>创建会员卡二维码请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardQrcodeRequest
{
    /// <summary>【激活类型】 可指定该场景下的激活类型，支持一键激活及跳转激活两种类型 1、一键激活（AUTO_ACTIVATE）用户点击开卡时，平台完成激活，并将用户授权的信息通过事件通知回调商家服务器； 2、跳转激活（JUMP_ACTIVATE）用户点击开卡时可跳转至商家h5/小程序，并将用户授权的信息及code加密传至商家h5/小程序，商家解密后可根据自由业务规则调用激活接口完成会员激活。 - `AUTO_ACTIVATE`: 用户点击开卡后，由平台来完成会员卡激活…（官方字段路径 <c>activate_type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("activate_type")]
    public string? ActivateType { get; set; }

    /// <summary>【跳转激活的URL】 激活类型是跳转激活时，用户点击开卡后跳转到商家网页的URL。如果同时配置了activate_url和activate_AppID/path，则优先跳转到小程序（官方字段路径 <c>activate_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>【跳转激活的AppID】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的AppID。如果同时配置了activate_url和activate_appid/path，则优先跳转到小程序（官方字段路径 <c>activate_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activate_appid")]
    public string? ActivateAppid { get; set; }

    /// <summary>【跳转激活的path】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的path（官方字段路径 <c>activate_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("activate_path")]
    public string? ActivatePath { get; set; }

    /// <summary>【自定义场景值】 自定义场景值，商户可以用于标记投放场景，如门店／来源等。只能录入数字及中英文／半角标点 匹配正则表达式: ^[0-9a-zA-Z\\u0000-\\u00FF\\u4e00-\\u9fa5]+$（官方字段路径 <c>outer_str</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }

}

/// <summary>创建会员卡二维码响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550605"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardQrcodeResponse : WechatPayResponse
{
    /// <summary>【二维码对应的URL】 商户可用此URL生成二维码，用户扫码即可开通会员卡（官方字段路径 <c>url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

}

/// <summary>解密会员卡code响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550742"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardDecryptCodeResponse : WechatPayResponse
{
    /// <summary>【会员卡code】 解密后获取的真实code码（官方字段路径 <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code")]
    public new string? Code { get; set; }

}

/// <summary>查询会员卡激活信息响应·【用户开卡时填写的个人信息】·【平台提供的通用开卡信息字段】 包含性别、手机、头像、昵称、姓名等基本信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550779"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardActivateInformationResponseUserInformationCommonFieldListItem
{
    /// <summary>【平台提供的通用开卡信息字段名称】 平台提供了一些通用的开卡字段供开发者选用 - `USER_FORM_FLAG_MOBILE`: 用户开卡时填写手机号 - `USER_FORM_FLAG_SEX`: 用户开卡时填写性别 - `USER_FORM_FLAG_NAME`: 用户开卡时填写姓名 - `USER_FORM_FLAG_BIRTHDAY`: 用户开卡时填写生日 - `USER_FORM_FLAG_ADDRESS`: 用户开卡时填写地址 - `USER…（官方字段路径 <c>user_information.common_field_list.name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 平台提供的通用开卡信息字段，用户在开卡时填写的信息（官方字段路径 <c>user_information.common_field_list.value</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

}

/// <summary>查询会员卡激活信息响应·【用户开卡时填写的个人信息】·【商户自定义的开卡信息字段】 商户自定义的开卡信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550779"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardActivateInformationResponseUserInformationCustomFieldListItem
{
    /// <summary>【字段名称】 商户自定义的开卡信息字段名称（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 商户自定义的开卡信息字段值，用户在开卡时需填写。填写项目为单选时的返回（官方字段路径 <c>user_information.custom_field_list.value</c>，选填，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

    /// <summary>【字段值列表】 商户自定义的开卡信息字段值，用户在开卡时需填写。填写项目为多选时的返回 特殊规则：列表最多支持10个，单个列表限制8个字符（官方字段路径 <c>user_information.custom_field_list.value_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("value_list")]
    public List<string>? ValueList { get; set; }

}

/// <summary>查询会员卡激活信息响应·【用户开卡时填写的个人信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550779"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardActivateInformationResponseUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 包含性别、手机、头像、昵称、姓名等基本信息字段（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<MemberCardActivateInformationResponseUserInformationCommonFieldListItem>? CommonFieldList { get; set; }

    /// <summary>【商户自定义的开卡信息字段】 商户自定义的开卡信息字段（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MemberCardActivateInformationResponseUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>查询会员卡激活信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550779"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardActivateInformationResponse : WechatPayResponse
{
    /// <summary>【微信昵称】 用户的微信昵称（官方字段路径 <c>nickname</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>【微信头像URL】 用户的微信头像URL（官方字段路径 <c>head_image_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("head_image_url")]
    public string? HeadImageUrl { get; set; }

    /// <summary>【用户开卡时填写的个人信息】（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public MemberCardActivateInformationResponseUserInformation? UserInformation { get; set; }

}

/// <summary>激活会员卡（跳转型激活）请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550809"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardActivateRequest
{
    /// <summary>【会员卡编号】 在用户会员卡上展示的会员卡编号，如不填写，默认使用会员卡code作为membership_number。只能录入数字／英文／半角标点（官方字段路径 <c>membership_number</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("membership_number")]
    public string? MembershipNumber { get; set; }

}

/// <summary>获取用户授权令牌请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550867"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardPermissionTokenRequest
{
    /// <summary>【激活类型】 可指定该场景下的激活类型，支持一键自动激活及跳转激活两种类型。 - `AUTO_ACTIVATE`：一键自动激活。用户点击开卡时，平台完成激活，并将用户授权的信息通过事件通知回调商家服务器； - `JUMP_ACTIVATE`：跳转激活。用户点击开卡时可跳转至商家h5/小程序，并将用户授权的信息及code加密传至商家h5/小程序，商家解密后可根据自由业务规则调用激活接口完成会员激活（官方字段路径 <c>activate_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activate_type")]
    public string? ActivateType { get; set; }

    /// <summary>【设置用户在开卡完成后，是否返回拉起开卡组件的商家页面】 默认为false，若填写false，开卡完成后不返回商家页面，默认跳转到会员卡详情页面； 若填写true，则开卡完成后返回拉起开卡组件的商家页面。 注意：若该字段设置为true，且同时入参activate_url/activate_appid及activate_path，这种情况下，navigate_back_previous_page的设置不生效，系统将优先执行跳转（官方字段路径 <c>navigate_back_previous_page</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("navigate_back_previous_page")]
    public bool? NavigateBackPreviousPage { get; set; }

    /// <summary>【跳转激活的URL】 激活类型是跳转激活时，用户点击开卡后跳转到商家网页的URL。如果同时配置了activate_url和activate_appid/path，则优先跳转到小程序（官方字段路径 <c>activate_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("activate_url")]
    public string? ActivateUrl { get; set; }

    /// <summary>【跳转激活的AppID】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的AppID。如果同时配置了activate_url和activate_appid/path，则优先跳转到小程序（官方字段路径 <c>activate_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activate_appid")]
    public string? ActivateAppid { get; set; }

    /// <summary>【跳转激活的path】 激活类型是跳转激活时，用户点击开卡后跳转到商家小程序的path（官方字段路径 <c>activate_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("activate_path")]
    public string? ActivatePath { get; set; }

    /// <summary>【自定义场景值】 自定义场景值，商户可以用于标记投放场景，如门店／来源等。只能录入数字及中英文／半角标点（官方字段路径 <c>outer_str</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }

}

/// <summary>获取用户授权令牌响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550867"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardPermissionTokenResponse : WechatPayResponse
{
    /// <summary>【预授权token】 用于H5场景拉起会员开卡组件投放会员卡（官方字段路径 <c>permission_token</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("permission_token")]
    public string? PermissionToken { get; set; }

}
