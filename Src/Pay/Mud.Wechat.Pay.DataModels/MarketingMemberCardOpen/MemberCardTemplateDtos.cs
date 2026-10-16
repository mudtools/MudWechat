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

/// <summary>创建会员卡模板请求·【品牌信息】 该会员卡归属的品牌信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateRequestBrand
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand.brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌展示名称】 1.展示在会员卡面上 2.支持最长10个中文字 3.支持中文字、英文字符（官方字段路径 <c>brand.display_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

}

/// <summary>创建会员卡模板请求·【会员卡有效期】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateRequestDateInformation
{
    /// <summary>【有效期类型】 1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，格式为yyyy-mm-dd hh-mm-ss 4.相对有效期：用户激活后x天后有效，x为天数。最多支持10,957天（30年） 5.永久有效 6.过期后卡状态变为“已过期”，出现在卡包历史卡券。 7.过期后不再出现服务项，且会员服务触达无法再次下发。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_tim（官方字段路径 <c>date_information.type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表…（官方字段路径 <c>date_information.available_begin_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示…（官方字段路径 <c>date_information.available_end_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】 type为FIX_TERM时专用，表示生效后N天内有效，领取后当天有效填写0（单位为天）。最长不超过30年（官方字段路径 <c>date_information.available_day_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【领取后N天开始生效】 type为FIX_TERM时专用，表示自领取后N天开始生效。（单位为天）（官方字段路径 <c>date_information.wait_days_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

}

/// <summary>创建会员卡模板请求·【储值信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateRequestBalanceInformation
{
    /// <summary>【是否支持储值】 储值额在会员卡面上展示，展示该字段的商户需要有单用途商业预付卡资质。如不填写，默认不展示储值余额（官方字段路径 <c>balance_information.need_balance</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_balance")]
    public bool? NeedBalance { get; set; }

    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("balance_appid")]
    public string? BalanceAppid { get; set; }

    /// <summary>【储值小程序path】 点击储值额跳转的小程序页面path，建议为储值充值页面。只有支持储值，此字段才有效 若填写了储值小程序AppID，储值小程序path为必填项（官方字段路径 <c>balance_information.balance_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_path")]
    public string? BalancePath { get; set; }

    /// <summary>【储值URL】 点击储值额跳转的h5页面。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_url")]
    public string? BalanceUrl { get; set; }

}

/// <summary>创建会员卡模板请求·【开卡信息】 用户在开通会员卡时需要填写的信息·【商户自定义的开卡信息字段】 商户自定义的开卡信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateRequestUserInformationFormCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型（官方字段路径 <c>user_information_form.custom_field_list.type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称（官方字段路径 <c>user_information_form.custom_field_list.name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表 特殊规则：列表最多支持10个，单个列表限制8个字符（官方字段路径 <c>user_information_form.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>创建会员卡模板请求·【开卡信息】 用户在开通会员卡时需要填写的信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateRequestUserInformationForm
{
    /// <summary>【平台提供的通用开卡信息字段】。可选取值：`USER_FORM_FLAG_MOBILE` 手机号 - USER_FORM_FLAG_SEX: 性别 - USER_FORM（官方字段路径 <c>user_information_form.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商户自定义的开卡信息字段】 商户自定义的开卡信息字段（官方字段路径 <c>user_information_form.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MemberCardTemplateCreateRequestUserInformationFormCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>创建会员卡模板请求·【商户补充声明】 商户补充声明。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateRequestAdditionalStatement
{
    /// <summary>【标题】 商户补充声明的标题，如果填写，将会在用户开卡时展示给用户，用户可点击跳转到商户指定的链接或者小程序，了解声明的详情（官方字段路径 <c>additional_statement.title</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【跳转URL】 用户在开卡页面点击查看“商户补充声明”，会跳转到URL指定的页面。如果同时配置了URL和AppID/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>【跳转小程序AppID】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【跳转小程序path】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>创建会员卡模板请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateRequest
{
    /// <summary>【商户AppID】 商户的公众号AppID。 1、只能为服务号AppID，不支持App、小程序、订阅号的AppID。 2、该AppID用于获取会员OpenID及unionid。 3、会员相关（会员状态、权益、服务）消息将通过该服务号触达用户 4、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡logo】 会员卡logo的URL地址。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>logo_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【品牌信息】 该会员卡归属的品牌信息（官方字段路径 <c>brand</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("brand")]
    public MemberCardTemplateCreateRequestBrand? Brand { get; set; }

    /// <summary>【卡名称】 1.展示在卡面上 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>title</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【会员卡背景图】 商家自定义会员卡背景图。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、大小需为1000像素*600像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>background_picture_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("background_picture_url")]
    public string? BackgroundPictureUrl { get; set; }

    /// <summary>【使用须知】 展示在会员卡详情内，最长500个中文字符，建议填写会员权益及服务相关描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(500)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【会员码型】 员卡的码型支持二维码/条形码/二维码+条形码/不展示码，这4种码型是根据membershipnumber字段生成，用户领卡后membershipnumber默认为code值，支持商户修改。可选取值：`BAR_CODE` 条形码 - QRCODE：二维码 - BAR_CODE_AND_QRCODE：条（官方字段路径 <c>code_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【会员卡总库存】 可投放的最大会员卡数量。仅在会员卡code分配类型为系统自动分配（SYSTEM_ALLOCATE）时需要填写，其他分配类型不需要填写库存。系统分配code类型下，若未填写总库存，则微信支付系统会默认将总库存设置为5000000 （默认值）（官方字段路径 <c>total_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("total_quantity")]
    public long? TotalQuantity { get; set; }

    /// <summary>【会员卡有效期】（官方字段路径 <c>date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("date_information")]
    public MemberCardTemplateCreateRequestDateInformation? DateInformation { get; set; }

    /// <summary>【会员卡code分配类型】 1、会员卡code是会员在card_id下唯一身份标识，平台支持3中分配类型： SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信支付系统分配12位数字作为会员code； MERCHANT_DEPOSIT 商户预存code，商户可提前通过导入预存code接口导入code，用户领取会员卡时平台从商户导入的code中随机分配作为会员code REAL_TIME 商家在激活时实时传入code，该模式仅支持小程序拉…（官方字段路径 <c>code_mode</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）（官方字段路径 <c>need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【会员初始等级】 如果展示会员等级，必填init_level，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息（官方字段路径 <c>init_level</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【商户请求单号】 商户创建会员卡凭据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【储值信息】（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public MemberCardTemplateCreateRequestBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【开卡信息】 用户在开通会员卡时需要填写的信息（官方字段路径 <c>user_information_form</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information_form")]
    public MemberCardTemplateCreateRequestUserInformationForm? UserInformationForm { get; set; }

    /// <summary>【商户补充声明】 商户补充声明（官方字段路径 <c>additional_statement</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("additional_statement")]
    public MemberCardTemplateCreateRequestAdditionalStatement? AdditionalStatement { get; set; }

    /// <summary>【是否启用动态码】 是否启用动态码功能。若启用，用户会员卡的身份识别码会被系统生成的18位数字取替，动态改变，可有效保障用户的储值资产安全，降低用户因被截图带来的储值盗用风险。默认为false（官方字段路径 <c>need_dynamic_code</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_dynamic_code")]
    public bool? NeedDynamicCode { get; set; }

}

/// <summary>创建会员卡模板响应·【品牌信息】 该会员卡归属的品牌信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseBrand
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand.brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌展示名称】 1.展示在会员卡面上 2.支持最长10个中文字 3.支持中文字、英文字符（官方字段路径 <c>brand.display_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

}

/// <summary>创建会员卡模板响应·【有效期】 有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseDateInformation
{
    /// <summary>【有效期类型】 1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，格式为yyyy-mm-dd hh-mm-ss 4.相对有效期：用户激活后x天后有效，x为天数。最多支持10,957天（30年） 5.永久有效 6.过期后卡状态变为“已过期”，出现在卡包历史卡券。 7.过期后不再出现服务项，且会员服务触达无法再次下发。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_tim（官方字段路径 <c>date_information.type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表…（官方字段路径 <c>date_information.available_begin_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示…（官方字段路径 <c>date_information.available_end_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】 type为FIX_TERM时专用，表示生效后N天内有效，领取后当天有效填写0（单位为天）。最长不超过30年（官方字段路径 <c>date_information.available_day_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【领取后N天开始生效】 type为FIX_TERM时专用，表示自领取后N天开始生效。（单位为天）（官方字段路径 <c>date_information.wait_days_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

}

/// <summary>创建会员卡模板响应·【储值信息】 储值信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseBalanceInformation
{
    /// <summary>【是否支持储值】 储值额在会员卡面上展示，展示该字段的商户需要有单用途商业预付卡资质。如不填写，默认不展示储值余额（官方字段路径 <c>balance_information.need_balance</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_balance")]
    public bool? NeedBalance { get; set; }

    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("balance_appid")]
    public string? BalanceAppid { get; set; }

    /// <summary>【储值小程序path】 点击储值额跳转的小程序页面path，建议为储值充值页面。只有支持储值，此字段才有效 若填写了储值小程序AppID，储值小程序path为必填项（官方字段路径 <c>balance_information.balance_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_path")]
    public string? BalancePath { get; set; }

    /// <summary>【储值URL】 点击储值额跳转的h5页面。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_url")]
    public string? BalanceUrl { get; set; }

}

/// <summary>创建会员卡模板响应·【开卡信息】 用户在开通会员卡时需要填写的信息·【商户自定义的开卡信息字段】 商户自定义的开卡信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseUserInformationFormCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型 - `TEXT`: 用户可自由输入文本 - `SELECT`: 用户可从商户提供的选项中进行选择 - `RADIO`: 用户可从商户提供的选项中选择一项 - `CHECK_BOX`: 用户可从商户提供的选项中选择一到多项（官方字段路径 <c>user_information_form.custom_field_list.type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称（官方字段路径 <c>user_information_form.custom_field_list.name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表 特殊规则：列表最多支持10个，单个列表限制8个字符（官方字段路径 <c>user_information_form.custom_field_list.values</c>，选填，类型 <c>array</c>）。</summary>
    [JsonPropertyName("values")]
    public string? Values { get; set; }

}

/// <summary>创建会员卡模板响应·【开卡信息】 用户在开通会员卡时需要填写的信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseUserInformationForm
{
    /// <summary>【平台提供的通用开卡信息字段】 平台提供了一些通用的开卡字段供开发者选用。可选取值：`USER_FORM_FLAG_MOBILE` 手机号 - USER_FORM_FLAG_SEX: 性别 - USER_FORM（官方字段路径 <c>user_information_form.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商户自定义的开卡信息字段】 商户自定义的开卡信息字段（官方字段路径 <c>user_information_form.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MemberCardTemplateCreateResponseUserInformationFormCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>创建会员卡模板响应·【商户补充声明】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseAdditionalStatement
{
    /// <summary>【标题】 商户补充声明的标题，如果填写，将会在用户开卡时展示给用户，用户可点击跳转到商户指定的链接或者小程序，了解声明的详情（官方字段路径 <c>additional_statement.title</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【跳转URL】 用户在开卡页面点击查看“商户补充声明”，会跳转到URL指定的页面。如果同时配置了URL和AppID/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>【跳转小程序AppID】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【跳转小程序path】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>创建会员卡模板响应·【会员优惠】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseFavor
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

/// <summary>创建会员卡模板响应·【会员服务项】 已添加的会员服务项·【跳转小程序】 服务项指定跳转小程序。用户可在会员卡详情页中点击当前服务项，跳转。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseServiceModulesItemJumpMiniprogram
{
    /// <summary>【小程序AppID】 小程序的AppID（官方字段路径 <c>service_modules.jump_miniprogram.appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序path】 小程序的页面path（官方字段路径 <c>service_modules.jump_miniprogram.path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>创建会员卡模板响应·【会员服务项】 已添加的会员服务项。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponseServiceModulesItem
{
    /// <summary>【会员服务项id】 会员服务项的唯一识别ID。微信内部运营人员创建服务项后，会生成一个服务项id，商户可通过服务项id查询服务项的配置信息及用途（官方字段路径 <c>service_modules.service_module_id</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("service_module_id")]
    public string? ServiceModuleId { get; set; }

    /// <summary>【跳转小程序】 服务项指定跳转小程序。用户可在会员卡详情页中点击当前服务项，跳转到商家配置的小程序（官方字段路径 <c>service_modules.jump_miniprogram</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_miniprogram")]
    public MemberCardTemplateCreateResponseServiceModulesItemJumpMiniprogram? JumpMiniprogram { get; set; }

}

/// <summary>创建会员卡模板响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateCreateResponse : WechatPayResponse
{
    /// <summary>【会员卡模板ID】 商户创建微信会员卡模板成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【剩余库存】 会员卡剩余的库存（官方字段路径 <c>remain_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("remain_quantity")]
    public long? RemainQuantity { get; set; }

    /// <summary>【会员卡状态】 会员卡当前的状态 - `NOT_APPROVE`: 待审核 - `APPROVE_FAIL`: 审核失败 - `APPROVED`: 审核通过 - `DELETED`: 卡被商户删除（官方字段路径 <c>status</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【品牌信息】 该会员卡归属的品牌信息（官方字段路径 <c>brand</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("brand")]
    public MemberCardTemplateCreateResponseBrand? Brand { get; set; }

    /// <summary>【品牌AppID】 商户的公众号AppID。 1、只能为服务号AppID，不支持APP、小程序、订阅号的AppID。 2、该AppID用于获取会员OpenID及UnionID。3、会员相关（会员状态、权益、服务）消息将通过该服务号触达用户 4、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡logo】 会员卡logo的URL地址。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>logo_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【卡名称】 1.展示在卡面上 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>title</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【会员卡背景图】 商家自定义会员卡背景图。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、大小需为1000像素*600像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>background_picture_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("background_picture_url")]
    public string? BackgroundPictureUrl { get; set; }

    /// <summary>【使用须知】 展示在会员卡详情内，最长500个中文字符，建议填写会员权益及服务相关描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(500)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【会员码型】 员卡的码型支持二维码/条形码/二维码+条形码/不展示码，这4种码型是根据membershipnumber字段生成，用户领卡后membershipnumber默认为code值，支持商户修改。可选取值：`BAR_CODE` 条形码 - QRCODE: 二维码 - BAR_CODE_AND_QRCODE:（官方字段路径 <c>code_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【会员卡总库存】 可投放的最大会员卡数量。仅在会员卡code分配类型为系统自动分配（SYSTEM_ALLOCATE）时需要填写，其他分配类型不需要填写库存。系统分配code类型下，若未填写总库存，则微信支付系统会默认将总库存设置为5000000 （默认值）（官方字段路径 <c>total_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("total_quantity")]
    public long? TotalQuantity { get; set; }

    /// <summary>【有效期】 有效期（官方字段路径 <c>date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("date_information")]
    public MemberCardTemplateCreateResponseDateInformation? DateInformation { get; set; }

    /// <summary>【会员卡code分配类型】 `MERCHANT_DEPOSIT`: 商户预先存入自定义code，用户开卡时系统随机选取存入的code SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信支付系统分配12位数字作为会员code； MERCHANT_DEPOSIT 商户预存code，商户可提前通过导入预存code接口导入code，用户领取会员卡时平台从商户导入的code中随机分配作为会员code REAL_TIME 商家在激活时实时传入co…（官方字段路径 <c>code_mode</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）（官方字段路径 <c>need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【会员初始等级】 如果展示会员等级，必填init_level，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息（官方字段路径 <c>init_level</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【商户请求单号】 商户创建会员卡凭据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【储值信息】 储值信息（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public MemberCardTemplateCreateResponseBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【开卡信息】 用户在开通会员卡时需要填写的信息（官方字段路径 <c>user_information_form</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information_form")]
    public MemberCardTemplateCreateResponseUserInformationForm? UserInformationForm { get; set; }

    /// <summary>【商户补充声明】（官方字段路径 <c>additional_statement</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("additional_statement")]
    public MemberCardTemplateCreateResponseAdditionalStatement? AdditionalStatement { get; set; }

    /// <summary>【会员优惠】（官方字段路径 <c>favor</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("favor")]
    public MemberCardTemplateCreateResponseFavor? Favor { get; set; }

    /// <summary>【会员服务项】 已添加的会员服务项（官方字段路径 <c>service_modules</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("service_modules")]
    public List<MemberCardTemplateCreateResponseServiceModulesItem>? ServiceModules { get; set; }

    /// <summary>【是否启用动态码】 是否启用动态码功能。若启用，用户会员卡的身份识别码会被系统生成的18位数字取替，动态改变，可有效保障用户的储值资产安全，降低用户因被截图带来的储值盗用风险。默认为false（官方字段路径 <c>need_dynamic_code</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_dynamic_code")]
    public bool? NeedDynamicCode { get; set; }

}

/// <summary>查询会员卡模板列表响应·【会员卡列表】 符合条件的会员卡模板ID列表·【品牌信息】 该会员卡归属的品牌信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponseDataItemBrand
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>data.brand.brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌展示名称】 1.展示在会员卡面上 2.支持最长10个中文字 3.支持中文字、英文字符（官方字段路径 <c>data.brand.display_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

}

/// <summary>查询会员卡模板列表响应·【会员卡列表】 符合条件的会员卡模板ID列表·【有效期】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponseDataItemDateInformation
{
    /// <summary>【有效期类型】 1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，格式为yyyy-mm-dd hh-mm-ss 4.相对有效期：用户激活后x天后有效，x为天数。最多支持10,957天（30年） 5.永久有效 6.过期后卡状态变为“已过期”，出现在卡包历史卡券。 7.过期后不再出现服务项，且会员服务触达无法再次下发。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_tim（官方字段路径 <c>data.date_information.type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表…（官方字段路径 <c>data.date_information.available_begin_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示…（官方字段路径 <c>data.date_information.available_end_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】 type为FIX_TERM时专用，表示生效后N天内有效，领取后当天有效填写0（单位为天）。最长不超过30年（官方字段路径 <c>data.date_information.available_day_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【领取后N天开始生效】 type为FIX_TERM时专用，表示自领取后N天开始生效。（单位为天）（官方字段路径 <c>data.date_information.wait_days_after_receive</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

}

/// <summary>查询会员卡模板列表响应·【会员卡列表】 符合条件的会员卡模板ID列表·【储值信息】 储值信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponseDataItemBalanceInformation
{
    /// <summary>【是否支持储值】 储值额在会员卡面上展示，展示该字段的商户需要有单用途商业预付卡资质。如不填写，默认不展示储值余额（官方字段路径 <c>data.balance_information.need_balance</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_balance")]
    public bool? NeedBalance { get; set; }

    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID。只有支持储值，此字段才有效（官方字段路径 <c>data.balance_information.balance_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("balance_appid")]
    public string? BalanceAppid { get; set; }

    /// <summary>【储值小程序path】 点击储值额跳转的小程序页面path，建议为储值充值页面。只有支持储值，此字段才有效 若填写了储值小程序AppID，储值小程序path为必填项（官方字段路径 <c>data.balance_information.balance_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_path")]
    public string? BalancePath { get; set; }

    /// <summary>【储值URL】 点击储值额跳转的h5页面。只有支持储值，此字段才有效（官方字段路径 <c>data.balance_information.balance_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_url")]
    public string? BalanceUrl { get; set; }

}

/// <summary>查询会员卡模板列表响应·【会员卡列表】 符合条件的会员卡模板ID列表·【开卡信息】 用户在开通会员卡时需要填写的信息·【商户自定义的开卡信息字段】 商户自定义的开卡信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponseDataItemUserInformationFormCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型 - `TEXT`: 用户可自由输入文本 - `SELECT`: 用户可从商户提供的选项中进行选择 - `RADIO`: 用户可从商户提供的选项中选择一项 - `CHECK_BOX`: 用户可从商户提供的选项中选择一到多项（官方字段路径 <c>data.user_information_form.custom_field_list.type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称（官方字段路径 <c>data.user_information_form.custom_field_list.name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表 特殊规则：列表最多支持10个，单个列表限制8个字符（官方字段路径 <c>data.user_information_form.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>查询会员卡模板列表响应·【会员卡列表】 符合条件的会员卡模板ID列表·【开卡信息】 用户在开通会员卡时需要填写的信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponseDataItemUserInformationForm
{
    /// <summary>【是否允许修改】 是否允许用户在激活成功后修改信息（官方字段路径 <c>data.user_information_form.can_modify_after_activate</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_modify_after_activate")]
    public bool? CanModifyAfterActivate { get; set; }

    /// <summary>【平台提供的通用开卡信息字段】 平台提供了一些通用的开卡字段供开发者选用。可选取值：`USER_FORM_FLAG_MOBILE` 手机号 - USER_FORM_FLAG_SEX: 性别 - USER_FORM（官方字段路径 <c>data.user_information_form.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商户自定义的开卡信息字段】 商户自定义的开卡信息字段（官方字段路径 <c>data.user_information_form.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MemberCardTemplateListResponseDataItemUserInformationFormCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>查询会员卡模板列表响应·【会员卡列表】 符合条件的会员卡模板ID列表·【商户补充声明】 商户补充声明。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponseDataItemAdditionalStatement
{
    /// <summary>【标题】 商户补充声明的标题，如果填写，将会在用户开卡时展示给用户，用户可点击跳转到商户指定的链接或者小程序，了解声明的详情（官方字段路径 <c>data.additional_statement.title</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【跳转URL】 用户在开卡页面点击查看“商户补充声明”，会跳转到URL指定的页面。如果同时配置了URL和AppID/path，则优先跳转到小程序（官方字段路径 <c>data.additional_statement.url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>【跳转小程序AppID】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>data.additional_statement.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【跳转小程序path】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>data.additional_statement.path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>查询会员卡模板列表响应·【会员卡列表】 符合条件的会员卡模板ID列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponseDataItem
{
    /// <summary>【会员卡模板ID】 商户创建微信会员卡模板成功后系统返回的会员卡模板ID（官方字段路径 <c>data.card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>data.create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>data.update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【剩余库存】 会员卡剩余的库存（官方字段路径 <c>data.remain_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("remain_quantity")]
    public long? RemainQuantity { get; set; }

    /// <summary>【会员卡状态】 会员卡当前的状态 - `NOT_APPROVE`: 待审核 - `APPROVE_FAIL`: 审核不通过 - `APPROVED`: 审核通过 - `DELETED`: 商户删除了此会员卡（官方字段路径 <c>data.status</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【品牌信息】 该会员卡归属的品牌信息（官方字段路径 <c>data.brand</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("brand")]
    public MemberCardTemplateListResponseDataItemBrand? Brand { get; set; }

    /// <summary>【品牌AppID】 商户的公众号AppID。 1、只能为服务号AppID，不支持APP、小程序、订阅号的AppID。 2、该AppID用于获取会员OpenID及UnionID。3、会员相关（会员状态、权益、服务）消息将通过该服务号触达用户 4、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>data.appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡logo】 会员卡logo的URL地址。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>data.logo_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【卡名称】 1.展示在卡面上 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>data.title</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【会员卡背景图】 商家自定义会员卡背景图。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、大小需为1000像素*600像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>data.background_picture_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("background_picture_url")]
    public string? BackgroundPictureUrl { get; set; }

    /// <summary>【使用须知】 展示在会员卡详情内，最长500个中文字符，建议填写会员权益及服务相关描述（官方字段路径 <c>data.description</c>，<b>必填</b>，类型 <c>string(500)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>data.service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【会员码型】 员卡的码型支持二维码/条形码/二维码+条形码/不展示码，这4种码型是根据membershipnumber字段生成，用户领卡后membershipnumber默认为code值，支持商户修改。可选取值：`BAR_CODE` 条形码 - QRCODE: 二维码 - BAR_CODE_AND_QRCODE:（官方字段路径 <c>data.code_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【会员卡总库存】 可投放的最大会员卡数量。仅在会员卡code分配类型为系统自动分配（SYSTEM_ALLOCATE）时需要填写，其他分配类型不需要填写库存。系统分配code类型下，若未填写总库存，则微信支付系统会默认将总库存设置为5000000 （默认值）（官方字段路径 <c>data.total_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("total_quantity")]
    public long? TotalQuantity { get; set; }

    /// <summary>【有效期】（官方字段路径 <c>data.date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("date_information")]
    public MemberCardTemplateListResponseDataItemDateInformation? DateInformation { get; set; }

    /// <summary>【会员卡code分配类型】 1、会员卡code是会员在card_id下唯一身份标识，平台支持3中分配类型： SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信支付系统分配12位数字作为会员code； MERCHANT_DEPOSIT 商户预存code，商户可提前通过导入预存code接口导入code，用户领取会员卡时平台从商户导入的code中随机分配作为会员code REAL_TIME 商家在激活时实时传入code，该模式仅支持小程序拉…（官方字段路径 <c>data.code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）（官方字段路径 <c>data.need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【会员初始等级】 如果展示会员等级，必填init_level，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息（官方字段路径 <c>data.init_level</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【商户请求单号】 商户创建会员卡凭据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>data.out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【储值信息】 储值信息（官方字段路径 <c>data.balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public MemberCardTemplateListResponseDataItemBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【开卡信息】 用户在开通会员卡时需要填写的信息（官方字段路径 <c>data.user_information_form</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information_form")]
    public MemberCardTemplateListResponseDataItemUserInformationForm? UserInformationForm { get; set; }

    /// <summary>【商户补充声明】 商户补充声明（官方字段路径 <c>data.additional_statement</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("additional_statement")]
    public MemberCardTemplateListResponseDataItemAdditionalStatement? AdditionalStatement { get; set; }

    /// <summary>【是否启用动态码】 是否启用动态码功能。若启用，用户会员卡的身份识别码会被系统生成的18位数字取替，动态改变，可有效保障用户的储值资产安全，降低用户因被截图带来的储值盗用风险。默认为false（官方字段路径 <c>data.need_dynamic_code</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_dynamic_code")]
    public bool? NeedDynamicCode { get; set; }

}

/// <summary>查询会员卡模板列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateListResponse : WechatPayResponse
{
    /// <summary>【会员卡列表】 符合条件的会员卡模板ID列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MemberCardTemplateListResponseDataItem>? Data { get; set; }

    /// <summary>【总数量】 总数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}

/// <summary>查询会员卡模板信息响应·【品牌信息】 该会员卡归属的品牌信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseBrand
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand.brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌展示名称】 1.展示在会员卡面上 2.支持最长10个中文字 3.支持中文字、英文字符（官方字段路径 <c>brand.display_name</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

}

/// <summary>查询会员卡模板信息响应·【有效期】 有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseDateInformation
{
    /// <summary>【有效期类型】 1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，格式为yyyy-mm-dd hh-mm-ss 4.相对有效期：用户激活后x天后有效，x为天数。最多支持10,957天（30年） 5.永久有效 6.过期后卡状态变为“已过期”，出现在卡包历史卡券。 7.过期后不再出现服务项，且会员服务触达无法再次下发。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_tim（官方字段路径 <c>date_information.type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表…（官方字段路径 <c>date_information.available_begin_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示…（官方字段路径 <c>date_information.available_end_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】 type为FIX_TERM时专用，表示生效后N天内有效，领取后当天有效填写0（单位为天）。最长不超过30年（官方字段路径 <c>date_information.available_day_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【领取后N天开始生效】 type为FIX_TERM时专用，表示自领取后N天开始生效。（单位为天）（官方字段路径 <c>date_information.wait_days_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

}

/// <summary>查询会员卡模板信息响应·【储值信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseBalanceInformation
{
    /// <summary>【是否支持储值】 储值额在会员卡面上展示，展示该字段的商户需要有单用途商业预付卡资质。如不填写，默认不展示储值余额（官方字段路径 <c>balance_information.need_balance</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_balance")]
    public bool? NeedBalance { get; set; }

    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("balance_appid")]
    public string? BalanceAppid { get; set; }

    /// <summary>【储值小程序path】 点击储值额跳转的小程序页面path，建议为储值充值页面。只有支持储值，此字段才有效 若填写了储值小程序AppID，储值小程序path为必填项（官方字段路径 <c>balance_information.balance_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_path")]
    public string? BalancePath { get; set; }

    /// <summary>【储值URL】 点击储值额跳转的h5页面。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_url")]
    public string? BalanceUrl { get; set; }

}

/// <summary>查询会员卡模板信息响应·【开卡信息】 用户在开通会员卡时需要填写的信息·【商户自定义的开卡信息字段】 商户自定义的开卡信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseUserInformationFormCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型 TEXT：文本型 SELECT：自定义选择项 RADIO：自定义单选 CHECK_BOX：自定义多选（官方字段路径 <c>user_information_form.custom_field_list.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称（官方字段路径 <c>user_information_form.custom_field_list.name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表 特殊规则：列表最多支持10个，单个列表限制8个字符（官方字段路径 <c>user_information_form.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>查询会员卡模板信息响应·【开卡信息】 用户在开通会员卡时需要填写的信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseUserInformationForm
{
    /// <summary>【是否允许修改】 是否允许用户在激活成功后修改信息（官方字段路径 <c>user_information_form.can_modify_after_activate</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_modify_after_activate")]
    public bool? CanModifyAfterActivate { get; set; }

    /// <summary>【平台提供的通用开卡信息字段】 平台提供了一些通用的开卡字段供开发者选用。可选取值：`USER_FORM_FLAG_MOBILE` 用户开卡时填写手机号 - USER_FORM_FLAG_SEX: 用户开卡时填写（官方字段路径 <c>user_information_form.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商户自定义的开卡信息字段】 商户自定义的开卡信息字段（官方字段路径 <c>user_information_form.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MemberCardTemplateResponseUserInformationFormCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>查询会员卡模板信息响应·【商户补充声明】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseAdditionalStatement
{
    /// <summary>【标题】 商户补充声明的标题，如果填写，将会在用户开卡时展示给用户，用户可点击跳转到商户指定的链接或者小程序，了解声明的详情（官方字段路径 <c>additional_statement.title</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【跳转URL】 用户在开卡页面点击查看“商户补充声明”，会跳转到URL指定的页面。如果同时配置了URL和AppID/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>【跳转小程序AppID】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【跳转小程序path】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>查询会员卡模板信息响应·【会员积分】 会员积分信息。商户为当前会员卡设置了积分权益后有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseBonus
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

/// <summary>查询会员卡模板信息响应·【会员优惠】 会员优惠信息。商户为当前会员卡设置了优惠权益后有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseFavor
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

}

/// <summary>查询会员卡模板信息响应·【会员服务项】 已添加的会员服务项。商户为当前会员卡设置了会员服务项时返回·【跳转小程序】 服务项指定跳转小程序。用户可在会员卡详情页中点击当前服务项，跳转。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseServiceModulesItemJumpMiniprogram
{
    /// <summary>【小程序AppID】 小程序的AppID（官方字段路径 <c>service_modules.jump_miniprogram.appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【小程序path】 小程序的页面path（官方字段路径 <c>service_modules.jump_miniprogram.path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>查询会员卡模板信息响应·【会员服务项】 已添加的会员服务项。商户为当前会员卡设置了会员服务项时返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseServiceModulesItem
{
    /// <summary>【会员服务项id】 会员服务项的唯一识别ID。微信内部运营人员创建服务项后，会生成一个服务项id，商户可通过服务项id查询服务项的配置信息及用途（官方字段路径 <c>service_modules.service_module_id</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("service_module_id")]
    public string? ServiceModuleId { get; set; }

    /// <summary>【跳转小程序】 服务项指定跳转小程序。用户可在会员卡详情页中点击当前服务项，跳转到商家配置的小程序，填写时，服务项的cell可点击跳转，否则不能跳转（官方字段路径 <c>service_modules.jump_miniprogram</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_miniprogram")]
    public MemberCardTemplateResponseServiceModulesItemJumpMiniprogram? JumpMiniprogram { get; set; }

}

/// <summary>查询会员卡模板信息响应·【会员发票】 设置发票栏目的文案、跳转小程序。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseFapiao
{
    /// <summary>【发票跳转文案】 发票栏跳转小程序的引导文案（官方字段路径 <c>fapiao.fapiao_jump_word</c>，选填，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_word")]
    public string? FapiaoJumpWord { get; set; }

    /// <summary>【发票跳转AppID】 发票跳转小程序的AppID（官方字段路径 <c>fapiao.fapiao_jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_appid")]
    public string? FapiaoJumpAppid { get; set; }

    /// <summary>【发票跳转path】 发票跳转小程序的path（官方字段路径 <c>fapiao.fapiao_jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("fapiao_jump_path")]
    public string? FapiaoJumpPath { get; set; }

}

/// <summary>查询会员卡模板信息响应·【客服信息】 设置发票栏目的文案、跳转小程序。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponseGuide
{
    /// <summary>【联系人名字】 设置商家咨询联系人的名称（官方字段路径 <c>guide.staff_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
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

/// <summary>查询会员卡模板信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateResponse : WechatPayResponse
{
    /// <summary>【会员卡模板ID】 商户创建微信会员卡模板成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间 遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示北京时间2020年05月20日13点29分35秒（官方字段路径 <c>update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【剩余库存】 会员卡剩余的库存（官方字段路径 <c>remain_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("remain_quantity")]
    public long? RemainQuantity { get; set; }

    /// <summary>【会员卡状态】 会员卡当前的状态 - `NOT_APPROVE`: 待审核 - `APPROVE_FAIL`: 审核不通过 - `APPROVED`: 审核通过，会员卡可以正常投放 - `DELETED`: 商户删除了此会员卡（官方字段路径 <c>status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【品牌信息】 该会员卡归属的品牌信息（官方字段路径 <c>brand</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("brand")]
    public MemberCardTemplateResponseBrand? Brand { get; set; }

    /// <summary>【商户AppID】 商户的公众号AppID。 1、只能为服务号AppID，不支持App、小程序、订阅号的AppID。 2、该AppID用于获取会员OpenID及UnionID。 3、会员相关（会员状态、权益、服务）消息将通过该服务号触达用户 4、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡logo】 会员卡logo的URL地址。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>logo_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【卡名称】 1.展示在卡面上 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>title</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【会员卡背景图】 商家自定义会员卡背景图。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、大小需为1000像素*600像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>background_picture_url</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("background_picture_url")]
    public string? BackgroundPictureUrl { get; set; }

    /// <summary>【使用须知】 展示在会员卡详情内，最长500个中文字符，建议填写会员权益及服务相关描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(500)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【会员码型】 员卡的码型支持二维码/条形码/二维码+条形码/不展示码，这4种码型是根据membershipnumber字段生成，用户领卡后membershipnumber默认为code值，支持商户修改。可选取值：`BAR_CODE` 条形码 - QRCODE: 二维码 - BAR_CODE_AND_QRCODE:（官方字段路径 <c>code_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【会员卡总库存】 可投放的最大会员卡数量。仅在会员卡code分配类型为系统自动分配（SYSTEM_ALLOCATE）时需要填写，其他分配类型不需要填写库存。系统分配code类型下，若未填写总库存，则微信支付系统会默认将总库存设置为5000000 （默认值）（官方字段路径 <c>total_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("total_quantity")]
    public long? TotalQuantity { get; set; }

    /// <summary>【有效期】 有效期（官方字段路径 <c>date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("date_information")]
    public MemberCardTemplateResponseDateInformation? DateInformation { get; set; }

    /// <summary>【会员卡code分配类型】 1、会员卡code是会员在card_id下唯一身份标识，平台支持3中分配类型： SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信支付系统分配12位数字作为会员code； MERCHANT_DEPOSIT 商户预存code，商户可提前通过导入预存code接口导入code，用户领取会员卡时平台从商户导入的code中随机分配作为会员code REAL_TIME 商家在激活时实时传入code，该模式仅支持小程序拉…（官方字段路径 <c>code_mode</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）（官方字段路径 <c>need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【会员初始等级】 如果展示会员等级，必填init_level，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息（官方字段路径 <c>init_level</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【商户请求单号】 商户创建会员卡凭据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【储值信息】（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public MemberCardTemplateResponseBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【开卡信息】 用户在开通会员卡时需要填写的信息（官方字段路径 <c>user_information_form</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information_form")]
    public MemberCardTemplateResponseUserInformationForm? UserInformationForm { get; set; }

    /// <summary>【商户补充声明】（官方字段路径 <c>additional_statement</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("additional_statement")]
    public MemberCardTemplateResponseAdditionalStatement? AdditionalStatement { get; set; }

    /// <summary>【会员积分】 会员积分信息。商户为当前会员卡设置了积分权益后有返回（官方字段路径 <c>bonus</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("bonus")]
    public MemberCardTemplateResponseBonus? Bonus { get; set; }

    /// <summary>【会员优惠】 会员优惠信息。商户为当前会员卡设置了优惠权益后有返回（官方字段路径 <c>favor</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("favor")]
    public MemberCardTemplateResponseFavor? Favor { get; set; }

    /// <summary>【会员服务项】 已添加的会员服务项。商户为当前会员卡设置了会员服务项时返回（官方字段路径 <c>service_modules</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("service_modules")]
    public List<MemberCardTemplateResponseServiceModulesItem>? ServiceModules { get; set; }

    /// <summary>【会员发票】 设置发票栏目的文案、跳转小程序（官方字段路径 <c>fapiao</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("fapiao")]
    public MemberCardTemplateResponseFapiao? Fapiao { get; set; }

    /// <summary>【客服信息】 设置发票栏目的文案、跳转小程序（官方字段路径 <c>guide</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("guide")]
    public MemberCardTemplateResponseGuide? Guide { get; set; }

    /// <summary>【是否启用动态码】 是否启用动态码功能。若启用，用户会员卡的身份识别码会被系统生成的18位数字取替，动态改变，可有效保障用户的储值资产安全，降低用户因被截图带来的储值盗用风险。默认为false（官方字段路径 <c>need_dynamic_code</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_dynamic_code")]
    public bool? NeedDynamicCode { get; set; }

}

/// <summary>修改会员卡模板信息请求·【有效期】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549567"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateUpdateRequestDateInformation
{
    /// <summary>【有效期类型】 1.该有效期为会员卡激活后的有效期 2.支持绝对有效期&amp;相对有效期设置 3.绝对有效期：固定过期时间，格式为yyyy-mm-dd hh-mm-ss 4.相对有效期：用户激活后x天后有效，x为天数。最多支持10,957天（30年） 5.永久有效 6.过期后卡状态变为“已过期”，出现在卡包历史卡券。 7.过期后不再出现服务项，且会员服务触达无法再次下发。可选取值：`FIX_TIME_RANGE` 必填available_begin_time和available_end_tim（官方字段路径 <c>date_information.type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【有效期开始时间】 type为FIX_TIME_RANGE时专用， 表示有效期开始时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表…（官方字段路径 <c>date_information.available_begin_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_begin_time")]
    public string? AvailableBeginTime { get; set; }

    /// <summary>【有效期结束时间】 type为FIX_TIME_RANGE时专用，表示有效期结束时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss.sss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss.sss表示时分秒毫秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2020-05-20T13:29:35.120+08:00表示…（官方字段路径 <c>date_information.available_end_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("available_end_time")]
    public string? AvailableEndTime { get; set; }

    /// <summary>【生效后N天内有效】 type为FIX_TERM时专用，表示生效后N天内有效，领取后当天有效填写0（单位为天）。最长不超过30年（官方字段路径 <c>date_information.available_day_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("available_day_after_receive")]
    public long? AvailableDayAfterReceive { get; set; }

    /// <summary>【领取后N天开始生效】 type为FIX_TERM时专用，表示自领取后N天开始生效。（单位为天）（官方字段路径 <c>date_information.wait_days_after_receive</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("wait_days_after_receive")]
    public long? WaitDaysAfterReceive { get; set; }

}

/// <summary>修改会员卡模板信息请求·【储值信息】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549567"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateUpdateRequestBalanceInformation
{
    /// <summary>【是否支持储值】 储值额在会员卡面上展示，展示该字段的商户需要有单用途商业预付卡资质。如不填写，默认不展示储值余额（官方字段路径 <c>balance_information.need_balance</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_balance")]
    public bool? NeedBalance { get; set; }

    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("balance_appid")]
    public string? BalanceAppid { get; set; }

    /// <summary>【储值小程序path】 点击储值额跳转的小程序页面path，建议为储值充值页面。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_path")]
    public string? BalancePath { get; set; }

    /// <summary>【储值URL】 点击储值额跳转的h5页面。只有支持储值，此字段才有效（官方字段路径 <c>balance_information.balance_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("balance_url")]
    public string? BalanceUrl { get; set; }

}

/// <summary>修改会员卡模板信息请求·【开卡信息】 用户在开通会员卡时需要填写的信息·【商户自定义的开卡信息字段】 商户自定义的开卡信息字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549567"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateUpdateRequestUserInformationFormCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型 - `TEXT`: 用户可自由输入文本 - `SELECT`: 用户可从商户提供的选项中进行选择 - `RADIO`: 用户可从商户提供的选项中选择一项 - `CHECK_BOX`: 用户可从商户提供的选项中选择一到多项（官方字段路径 <c>user_information_form.custom_field_list.type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称（官方字段路径 <c>user_information_form.custom_field_list.name</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表 特殊规则：列表最多支持10个，单个列表限制8个字符（官方字段路径 <c>user_information_form.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>修改会员卡模板信息请求·【开卡信息】 用户在开通会员卡时需要填写的信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549567"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateUpdateRequestUserInformationForm
{
    /// <summary>【是否允许修改】 是否允许用户在激活成功后修改信息（官方字段路径 <c>user_information_form.can_modify_after_activate</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_modify_after_activate")]
    public bool? CanModifyAfterActivate { get; set; }

    /// <summary>【平台提供的通用开卡信息字段】（官方字段路径 <c>user_information_form.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商户自定义的开卡信息字段】 商户自定义的开卡信息字段（官方字段路径 <c>user_information_form.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MemberCardTemplateUpdateRequestUserInformationFormCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>修改会员卡模板信息请求·【商户补充声明】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549567"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateUpdateRequestAdditionalStatement
{
    /// <summary>【标题】 商户补充声明的标题，如果填写，将会在用户开卡时展示给用户，用户可点击跳转到商户指定的链接或者小程序，了解声明的详情（官方字段路径 <c>additional_statement.title</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【跳转URL】 用户在开卡页面点击查看“商户补充声明”，会跳转到URL指定的页面。如果同时配置了URL和AppID/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>【跳转小程序AppID】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【跳转小程序path】 用户在开卡页面点击查看“商户补充声明”，会跳转到小程序指定页面。如果同时配置了URL和appid/path，则优先跳转到小程序（官方字段路径 <c>additional_statement.path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

}

/// <summary>修改会员卡模板信息请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549567"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateUpdateRequest
{
    /// <summary>【会员卡code分配类型】 1、会员卡code是会员在card_id下唯一身份标识，平台支持3中分配类型： SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信支付系统分配12位数字作为会员code； MERCHANT_DEPOSIT 商户预存code，商户可提前通过导入预存code接口导入code，用户领取会员卡时平台从商户导入的code中随机分配作为会员code REAL_TIME 商家在激活时实时传入code，该模式仅支持小程序拉…（官方字段路径 <c>code_mode</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【会员码型】 员卡的码型支持二维码/条形码/二维码+条形码/不展示码，这4种码型是根据membershipnumber字段生成，用户领卡后membershipnumber默认为code值，支持商户修改 - `BAR_CODE`: 条形码 - `QRCODE`: 二维码 - `BAR_CODE_AND_QRCODE`: 条形码和二维码 - `NONE_CODE`: 不显示任何码型（官方字段路径 <c>code_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【商户AppID】 商户的公众号AppID。 1、只能为服务号AppID，不支持App、小程序、订阅号的AppID。 2、该AppID用于获取会员OpenID及unionid。 3、会员相关（会员状态、权益、服务）消息将通过该服务号触达用户 4、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡logo】 会员卡logo的URL地址。仅支持通过《 图片上传API》接口获取的图片URL地址。 1、商户logo大小需为120像素*120像素。 2、支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>logo_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【卡名称】 1.展示在卡面上 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>title</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>【会员卡背景图】 商家自定义会员卡背景图。仅支持通过 图片上传API 接口获取的图片URL地址。支持JPG/JPEG/PNG格式，且图片小于1M（官方字段路径 <c>background_picture_url</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("background_picture_url")]
    public string? BackgroundPictureUrl { get; set; }

    /// <summary>【使用须知】 展示在会员卡详情内，最长500个中文字符，建议填写会员权益及服务相关描述（官方字段路径 <c>description</c>，选填，类型 <c>string(500)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【会员卡总库存】 可投放的最大会员卡数量。仅在会员卡code分配类型为系统自动分配（SYSTEM_ALLOCATE）时需要填写，其他分配类型不需要填写库存。系统分配code类型下，若未填写总库存，则微信支付系统会默认将总库存设置为5000000 （默认值）（官方字段路径 <c>total_quantity</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("total_quantity")]
    public long? TotalQuantity { get; set; }

    /// <summary>【有效期】（官方字段路径 <c>date_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("date_information")]
    public MemberCardTemplateUpdateRequestDateInformation? DateInformation { get; set; }

    /// <summary>【会员初始等级】 如果展示会员等级，必填init_level，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息（官方字段路径 <c>init_level</c>，选填，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【储值信息】（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public MemberCardTemplateUpdateRequestBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【开卡信息】 用户在开通会员卡时需要填写的信息（官方字段路径 <c>user_information_form</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information_form")]
    public MemberCardTemplateUpdateRequestUserInformationForm? UserInformationForm { get; set; }

    /// <summary>【商户补充声明】（官方字段路径 <c>additional_statement</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("additional_statement")]
    public MemberCardTemplateUpdateRequestAdditionalStatement? AdditionalStatement { get; set; }

    /// <summary>【是否启用动态码】 是否启用动态码功能。若启用，用户会员卡的身份识别码会被系统生成的18位数字取替，动态改变，可有效保障用户的储值资产安全，降低用户因被截图带来的储值盗用风险。默认为false（官方字段路径 <c>need_dynamic_code</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_dynamic_code")]
    public bool? NeedDynamicCode { get; set; }

}

/// <summary>老会员卡模板升级请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545673"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardTemplateUpgradeRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【商户AppID】 商户的公众号AppID。 1、只能为服务号AppID，不支持App、小程序、订阅号的AppID。 2、该AppID用于获取会员OpenID及unionid。 3、会员相关（会员状态、权益、服务）消息将通过该服务号触达用户 4、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}
