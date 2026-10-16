// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandCardMember;

/// <summary>用户会员卡信息响应·【会员卡有效期】 会员卡有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336980"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardResponseValidDateInformation
{
    /// <summary>【有效期类型】 传入会员卡有效期时必填。1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29…。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_time，会员卡在此时间\[available_begin_time, available_end_time)范围内有效 / `FIX_TERM` 必填available_day_after_receive，会员卡在用户领取之后available_day_after_receive天内有效 / `PERMANENT` 会员卡激活后永久有效（官方字段路径 <c>valid_date_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点2…（官方字段路径 <c>valid_date_information.available_begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29…（官方字段路径 <c>valid_date_information.available_end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【领取后N天内有效】 type为FIX_TERM时专用，表示领取后N个自然天内有效。最长不超过30年（10958 天）（官方字段路径 <c>valid_date_information.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

}

/// <summary>用户会员卡信息响应·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336980"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardResponseUserInformationCommonFieldListItem
{
    /// <summary>【平台提供的通用开卡信息字段名称】 平台提供了一些通用的开卡字段供开发者选用。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>user_information.common_field_list.name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户开卡时填写的个人信息】 平台提供的通用开卡信息字段，用户在开卡时填写的信息。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.common_field_list.value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

}

/// <summary>用户会员卡信息响应·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336980"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardResponseUserInformationCustomFieldListItem
{
    /// <summary>【字段名称】 商家自定义的开卡信息字段名称（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户选择的字段值列表】 商家自定义的开卡信息字段，用户在开卡时填写的值，明文不可超过 8 个字符。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.custom_field_list.user_chosen_values</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("user_chosen_values")]
    public List<string>? UserChosenValues { get; set; }

}

/// <summary>用户会员卡信息响应·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336980"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardResponseUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<BrandMemberUserCardResponseUserInformationCommonFieldListItem>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberUserCardResponseUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>用户会员卡信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336980"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardResponse : WechatPayResponse
{
    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>card_color</c>，选填，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引（官方字段路径 <c>card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡类型】 支持付费、普通、储值 3 种类型。目前仅支持普通会员卡，填写付费和储值类型时会返回错误。可选取值：`PURCHASE` 付费 / `NORMAL` 普通 / `BALANCE` 储值（官方字段路径 <c>card_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>【加密的手机号】 注册会员的手机号码，仅在用户授权手机号、商家通过 API 传入的情况下有值。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>phone_number</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>【等级】 用户会员等级，展示字段，商家可以自定义填写内容（官方字段路径 <c>level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期（官方字段路径 <c>valid_date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberUserCardResponseValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【领取时间】 用户领取会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>pickup_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("pickup_time")]
    public string? PickupTime { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 用户开卡时填写的个人信息（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberUserCardResponseUserInformation? UserInformation { get; set; }

    /// <summary>【商家数据包】 商家在创建用户会员卡时可传入自定义数据包，该数据对用户不可见，用于存储商家自定义信息，其总长度限制在256字符以内。查询用户会员卡详情时会将此字段返回给商家（官方字段路径 <c>attach</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【用户会员卡状态】 用户当前的卡状态。可选取值：`UNACTIVATED` 用户已领卡，但还未激活 / `EFFECTIVE` 用户的会员卡可正常使用 / `EXPIRED` 用户的会员卡已过期 / `INVALID` 用户的会员卡已失效（官方字段路径 <c>user_card_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_card_state")]
    public string? UserCardState { get; set; }

    /// <summary>【作废原因】 会员卡作废时传入的原因（官方字段路径 <c>invalid_reason</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("invalid_reason")]
    public string? InvalidReason { get; set; }

    /// <summary>【作废时间】 会员卡作废操作的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>invalid_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("invalid_time")]
    public string? InvalidTime { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>modify_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

}

/// <summary>用户会员卡列表响应·【会员卡列表】 符合条件的会员卡列表·【会员卡有效期】 会员卡有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336984"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardListResponseDataItemValidDateInformation
{
    /// <summary>【有效期类型】 传入会员卡有效期时必填。1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29…。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_time，会员卡在此时间\[available_begin_time, available_end_time)范围内有效 / `FIX_TERM` 必填available_day_after_receive，会员卡在用户领取之后available_day_after_receive天内有效 / `PERMANENT` 会员卡激活后永久有效（官方字段路径 <c>data.valid_date_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点2…（官方字段路径 <c>data.valid_date_information.available_begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29…（官方字段路径 <c>data.valid_date_information.available_end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【领取后N天内有效】 type为FIX_TERM时专用，表示领取后N个自然天内有效。最长不超过30年（10958 天）（官方字段路径 <c>data.valid_date_information.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

}

/// <summary>用户会员卡列表响应·【会员卡列表】 符合条件的会员卡列表·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336984"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardListResponseDataItemUserInformationCommonFieldListItem
{
    /// <summary>【平台提供的通用开卡信息字段名称】 平台提供了一些通用的开卡字段供开发者选用。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>data.user_information.common_field_list.name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户开卡时填写的个人信息】 平台提供的通用开卡信息字段，用户在开卡时填写的信息。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>data.user_information.common_field_list.value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

}

/// <summary>用户会员卡列表响应·【会员卡列表】 符合条件的会员卡列表·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336984"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardListResponseDataItemUserInformationCustomFieldListItem
{
    /// <summary>【字段名称】 商家自定义的开卡信息字段名称（官方字段路径 <c>data.user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户选择的字段值列表】 商家自定义的开卡信息字段，用户在开卡时填写的值，明文不可超过 8 个字符。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>data.user_information.custom_field_list.user_chosen_values</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("user_chosen_values")]
    public List<string>? UserChosenValues { get; set; }

}

/// <summary>用户会员卡列表响应·【会员卡列表】 符合条件的会员卡列表·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336984"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardListResponseDataItemUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段（官方字段路径 <c>data.user_information.common_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<BrandMemberUserCardListResponseDataItemUserInformationCommonFieldListItem>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项（官方字段路径 <c>data.user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberUserCardListResponseDataItemUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>用户会员卡列表响应·【会员卡列表】 符合条件的会员卡列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336984"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardListResponseDataItem
{
    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>data.user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>data.card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>data.openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>data.card_color</c>，选填，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引（官方字段路径 <c>data.card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>data.brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡类型】 支持付费、普通、储值 3 种类型。目前仅支持普通会员卡，填写付费和储值类型时会返回错误。可选取值：`PURCHASE` 付费 / `NORMAL` 普通 / `BALANCE` 储值（官方字段路径 <c>data.card_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>【加密的手机号】 注册会员的手机号码，仅在用户授权手机号、商家通过 API 传入的情况下有值。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>data.phone_number</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>【等级】 用户会员等级，展示字段，商家可以自定义填写内容（官方字段路径 <c>data.level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期（官方字段路径 <c>data.valid_date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberUserCardListResponseDataItemValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【领取时间】 用户领取会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>data.pickup_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("pickup_time")]
    public string? PickupTime { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 用户开卡时填写的个人信息（官方字段路径 <c>data.user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberUserCardListResponseDataItemUserInformation? UserInformation { get; set; }

    /// <summary>【商家数据包】 商家在创建用户会员卡时可传入自定义数据包，该数据对用户不可见，用于存储商家自定义信息，其总长度限制在256字符以内。查询用户会员卡详情时会将此字段返回给商家（官方字段路径 <c>data.attach</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【用户会员卡状态】 用户当前的卡状态。可选取值：`UNACTIVATED` 用户已领卡，但还未激活 / `EFFECTIVE` 用户的会员卡可正常使用 / `EXPIRED` 用户的会员卡已过期 / `INVALID` 用户的会员卡已失效（官方字段路径 <c>data.user_card_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_card_state")]
    public string? UserCardState { get; set; }

    /// <summary>【作废原因】 会员卡作废时传入的原因（官方字段路径 <c>data.invalid_reason</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("invalid_reason")]
    public string? InvalidReason { get; set; }

    /// <summary>【作废时间】 会员卡作废操作的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>data.invalid_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("invalid_time")]
    public string? InvalidTime { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>data.modify_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

}

/// <summary>用户会员卡列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336984"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardListResponse : WechatPayResponse
{
    /// <summary>【会员卡列表】 符合条件的会员卡列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<BrandMemberUserCardListResponseDataItem>? Data { get; set; }

    /// <summary>【总数量】 总数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小，范围为1~20（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}

/// <summary>用户会员卡信息修改请求·【会员卡有效期】 会员卡有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336985"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardUpdateRequestValidDateInformation
{
    /// <summary>【有效期类型】 传入会员卡有效期时必填。1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29…。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_time，会员卡在此时间\[available_begin_time, available_end_time)范围内有效 / `FIX_TERM` 必填available_day_after_receive，会员卡在用户领取之后available_day_after_receive天内有效 / `PERMANENT` 会员卡激活后永久有效（官方字段路径 <c>valid_date_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点2…（官方字段路径 <c>valid_date_information.available_begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29…（官方字段路径 <c>valid_date_information.available_end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【领取后N天内有效】 type为FIX_TERM时专用，表示领取后N个自然天内有效。最长不超过30年（10958 天）（官方字段路径 <c>valid_date_information.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

}

/// <summary>用户会员卡信息修改请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336985"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardUpdateRequestUserInformationCommonFieldListItem
{
    /// <summary>【平台提供的通用开卡信息字段名称】 平台提供了一些通用的开卡字段供开发者选用。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>user_information.common_field_list.name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户开卡时填写的个人信息】 平台提供的通用开卡信息字段，用户在开卡时填写的信息。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.common_field_list.value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

}

/// <summary>用户会员卡信息修改请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336985"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardUpdateRequestUserInformationCustomFieldListItem
{
    /// <summary>【字段名称】 商家自定义的开卡信息字段名称（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户选择的字段值列表】 商家自定义的开卡信息字段，用户在开卡时填写的值，明文不可超过 8 个字符。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.custom_field_list.user_chosen_values</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("user_chosen_values")]
    public List<string>? UserChosenValues { get; set; }

}

/// <summary>用户会员卡信息修改请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336985"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardUpdateRequestUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<BrandMemberUserCardUpdateRequestUserInformationCommonFieldListItem>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberUserCardUpdateRequestUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>用户会员卡信息修改请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336985"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardUpdateRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>card_color</c>，选填，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引（官方字段路径 <c>card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【加密的手机号】 注册会员的手机号码。该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>phone_number</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>【等级】 用户会员等级，展示字段，商家可以自定义填写内容（官方字段路径 <c>level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期（官方字段路径 <c>valid_date_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberUserCardUpdateRequestValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 用户开卡时填写的个人信息（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberUserCardUpdateRequestUserInformation? UserInformation { get; set; }

    /// <summary>【商家数据包】 商家在创建用户会员卡时可传入自定义数据包，该数据对用户不可见，用于存储商家自定义信息，其总长度限制在256字符以内。查询用户会员卡详情时会将此字段返回给商家（官方字段路径 <c>attach</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

}

/// <summary>会员开通结果同步请求·【会员卡有效期】 会员卡有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336979"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardConfirmRequestValidDateInformation
{
    /// <summary>【有效期类型】 传入会员卡有效期时必填。1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29…。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_time，会员卡在此时间\[available_begin_time, available_end_time)范围内有效 / `FIX_TERM` 必填available_day_after_receive，会员卡在用户领取之后available_day_after_receive天内有效 / `PERMANENT` 会员卡激活后永久有效（官方字段路径 <c>valid_date_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点2…（官方字段路径 <c>valid_date_information.available_begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29…（官方字段路径 <c>valid_date_information.available_end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【领取后N天内有效】 type为FIX_TERM时专用，表示领取后N个自然天内有效。最长不超过30年（10958 天）（官方字段路径 <c>valid_date_information.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

}

/// <summary>会员开通结果同步请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336979"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardConfirmRequestUserInformationCommonFieldListItem
{
    /// <summary>【平台提供的通用开卡信息字段名称】 平台提供了一些通用的开卡字段供开发者选用。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>user_information.common_field_list.name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户开卡时填写的个人信息】 平台提供的通用开卡信息字段，用户在开卡时填写的信息。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.common_field_list.value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

}

/// <summary>会员开通结果同步请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336979"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardConfirmRequestUserInformationCustomFieldListItem
{
    /// <summary>【字段名称】 商家自定义的开卡信息字段名称（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户选择的字段值列表】 商家自定义的开卡信息字段，用户在开卡时填写的值，明文不可超过 8 个字符。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.custom_field_list.user_chosen_values</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("user_chosen_values")]
    public List<string>? UserChosenValues { get; set; }

}

/// <summary>会员开通结果同步请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336979"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardConfirmRequestUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<BrandMemberUserCardConfirmRequestUserInformationCommonFieldListItem>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberUserCardConfirmRequestUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>会员开通结果同步请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336979"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardConfirmRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>card_color</c>，选填，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引（官方字段路径 <c>card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【加密的手机号】 注册会员的手机号码。该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>phone_number</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>【等级】 用户会员等级，展示字段，商家可以自定义填写内容（官方字段路径 <c>level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期（官方字段路径 <c>valid_date_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberUserCardConfirmRequestValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 用户开卡时填写的个人信息（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberUserCardConfirmRequestUserInformation? UserInformation { get; set; }

    /// <summary>【商家数据包】 商家在创建用户会员卡时可传入自定义数据包，该数据对用户不可见，用于存储商家自定义信息，其总长度限制在256字符以内。查询用户会员卡详情时会将此字段返回给商家（官方字段路径 <c>attach</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【会员开卡状态】 商家需要同步会员开卡状态。可选取值：`CREATE_CARD_SUCCESS` 开卡成功 / `CREATE_CARD_FAIL` 开卡失败 / `CREATE_CARD_ALREADY_EXISTS` 开卡成功但已在其他渠道入会（官方字段路径 <c>user_card_confirm_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_card_confirm_state")]
    public string? UserCardConfirmState { get; set; }

}

/// <summary>用户会员卡作废请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336983"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardInvalidateRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【作废原因】 会员卡作废原因（官方字段路径 <c>invalid_reason</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("invalid_reason")]
    public string? InvalidReason { get; set; }

}

/// <summary>按 OPENID 导入用户会员卡请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336981"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardImportRequestUserInformationCommonFieldListItem
{
    /// <summary>【平台提供的通用开卡信息字段名称】 平台提供了一些通用的开卡字段供开发者选用。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>user_information.common_field_list.name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户开卡时填写的个人信息】 平台提供的通用开卡信息字段，用户在开卡时填写的信息。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.common_field_list.value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("value")]
    public string? ValueValue { get; set; }

}

/// <summary>按 OPENID 导入用户会员卡请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336981"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardImportRequestUserInformationCustomFieldListItem
{
    /// <summary>【字段名称】 商家自定义的开卡信息字段名称（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【加密的用户选择的字段值列表】 商家自定义的开卡信息字段，用户在开卡时填写的值，明文不可超过 8 个字符。作为请求字段需加密，作为响应字段需解密。加密使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引。解密请参考 如何使用API证书解密敏感字段（官方字段路径 <c>user_information.custom_field_list.user_chosen_values</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("user_chosen_values")]
    public List<string>? UserChosenValues { get; set; }

}

/// <summary>按 OPENID 导入用户会员卡请求·【用户开卡时填写的个人信息】 用户开卡时填写的个人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336981"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardImportRequestUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 包含性别、姓名等基本信息字段（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<BrandMemberUserCardImportRequestUserInformationCommonFieldListItem>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段，当前最多只允许传入1项（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberUserCardImportRequestUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>按 OPENID 导入用户会员卡请求·【会员卡有效期】 会员卡有效期。传入的有效期类型，必须和会员卡模板保持一致。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336981"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardImportRequestValidDateInformation
{
    /// <summary>【有效期类型】 传入会员卡有效期时必填。1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29…。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_time，会员卡在此时间\[available_begin_time, available_end_time)范围内有效 / `FIX_TERM` 必填available_day_after_receive，会员卡在用户领取之后available_day_after_receive天内有效 / `PERMANENT` 会员卡激活后永久有效（官方字段路径 <c>valid_date_information.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点2…（官方字段路径 <c>valid_date_information.available_begin_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间。需需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29…（官方字段路径 <c>valid_date_information.available_end_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【领取后N天内有效】 type为FIX_TERM时专用，表示领取后N个自然天内有效。最长不超过30年（10958 天）（官方字段路径 <c>valid_date_information.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

}

/// <summary>按 OPENID 导入用户会员卡请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336981"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserCardImportRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【加密的手机号】 注册会员的手机号码。该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>phone_number</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>card_color</c>，选填，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引（官方字段路径 <c>card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【等级】 用户会员等级，展示字段，商家可以自定义填写内容（官方字段路径 <c>level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>【用户开卡时填写的个人信息】 用户开卡时填写的个人信息（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberUserCardImportRequestUserInformation? UserInformation { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期。传入的有效期类型，必须和会员卡模板保持一致（官方字段路径 <c>valid_date_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberUserCardImportRequestValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【领取时间】 用户领取会员卡的时间。仅在会员卡有效期类型是FIX_TERM时需要填写，领取时间必须早于当前时间，且不允许导入已过期的会员卡。需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+0…（官方字段路径 <c>pickup_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("pickup_time")]
    public string? PickupTime { get; set; }

}
