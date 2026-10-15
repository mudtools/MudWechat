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

/// <summary>会员卡模板创建请求·【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestCodeJumpInformation
{
    /// <summary>【会员码跳转AppID】 会员码跳转的小程序AppID（官方字段路径 <c>code_jump_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员码跳转路径】 会员码跳转的小程序路径（官方字段路径 <c>code_jump_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建请求·【会员卡有效期】 会员卡有效期。不传时默认为永久有效。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestValidDateInformation
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

/// <summary>会员卡模板创建请求·【会员中心信息】 用户点击会员卡卡面的跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestMemberInformation
{
    /// <summary>【会员中心跳转AppID】 用户点击会员卡卡面后跳转的小程序AppID（官方字段路径 <c>member_information.jump_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员中心跳转路径】 用户点击会员卡卡面后跳转的小程序路径（官方字段路径 <c>member_information.jump_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建请求·【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestPointsInformation
{
    /// <summary>【积分跳转AppID】 会员积分跳转的小程序AppID（官方字段路径 <c>points_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【积分跳转路径】 会员积分跳转的小程序路径（官方字段路径 <c>points_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建请求·【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestBalanceInformation
{
    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID（官方字段路径 <c>balance_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【储值小程序路径】 点击储值额跳转的小程序页面路径，建议为储值充值页面（官方字段路径 <c>balance_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建请求·【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestPurchaseInformation
{
    /// <summary>【付费会员价格】 指用户为获取特定商家提供的会员权益而需支付的费用金额。（单位：分）（官方字段路径 <c>purchase_information.price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【付费会员跳转AppID】 用户购买付费会员时跳转的付费会员的AppID（官方字段路径 <c>purchase_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【付费会员跳转路径】 用户购买付费会员时购买跳转的路径（官方字段路径 <c>purchase_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建请求·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestUserInformationCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型。可选取值：`CHECK_BOX` 用户可从商家提供的选项中选择多项 / `RADIO` 用户可从商家提供的选项中选择一项（官方字段路径 <c>user_information.custom_field_list.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称。若商户在会员卡模板中设置该值，则查询结果中会返回给商户（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表。 特殊规则：列表最多支持10个，单个列表限制8个字符。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户（官方字段路径 <c>user_information.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>会员卡模板创建请求·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequestUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 平台提供的通用开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。当前最多只允许传入1项（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberCardCreateRequestUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>会员卡模板创建请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardCreateRequest
{
    /// <summary>【商家请求单号】 商家创建会员卡模板凭据号。商家自定义，注意保持唯一性，仅供参考的格式：品牌ID+时间戳+流水号。字符仅允许包含英文半角的数字、字母、连接线 `-` 和下划线 `_`（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【商家AppID】 商家的AppID，可以是服务号、订阅号、公众号、小程序的AppID。1、该AppID用于获取会员OpenID。2、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡类型】 支持付费、普通、储值 3 种类型。目前仅支持普通会员卡，填写付费和储值类型时会返回错误。可选取值：`PURCHASE` 付费 / `NORMAL` 普通 / `BALANCE` 储值（官方字段路径 <c>card_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>【卡名称】 1.可用于展示在卡面的名称 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>card_title</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("card_title")]
    public string? CardTitle { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制。不传时默认为 `#FFFFFF`（官方字段路径 <c>card_color</c>，选填，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引。不传时没有默认卡图（官方字段路径 <c>card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【会员卡code分配类型】 1、会员卡code是会员在一个会员卡模板下唯一身份标识，平台支持2种分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。 2、会员卡code分配模式若为“系统分配”，不支持修改为“商家分配”。 3、不传时默认为系统分配（…。可选取值：`SYSTEM_ALLOCATE` 系统分配 / `MERCHANT_ALLOCATE` 商家分配（官方字段路径 <c>code_mode</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【会员码展示类型】 会员码支持不展示码/二维码／条形码／二维码＋条形码/跳转商家小程序5种设置。当会员码展示类型为跳转商家小程序时，必填 `code_jump_information`。不传时默认为不显示任何码型（ `NONE_CODE`）。可选取值：`NONE_CODE` 不显示任何码型 / `BAR_CODE` 条形码 / `QR_CODE` 二维码 / `BAR_CODE_AND_QR_CODE` 条形码和二维码 / `JUMP_MINI_PROGRAM` 跳转商家小程序（官方字段路径 <c>code_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必填（官方字段路径 <c>code_jump_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("code_jump_information")]
    public BrandMemberCardCreateRequestCodeJumpInformation? CodeJumpInformation { get; set; }

    /// <summary>【会员权益】 会员权益是指平台、品牌或服务机构为付费会员（或等级会员）提供的专属优惠、服务或特权。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>benefits</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("benefits")]
    public string? Benefits { get; set; }

    /// <summary>【回调地址】 商家接收开卡成功回调通知的地址，需按照 notify_url填写注意事项 规范填写。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【是否置顶】 置顶卡是界面中的一种特殊展示模块，通过人工设置，使其固定在内容列表的顶部位置，确保用户优先看到。默认为false。同一个品牌下允许存在多张置顶卡，按照更新时间倒序排序（官方字段路径 <c>need_pinned</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_pinned")]
    public bool? NeedPinned { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）。在查询结果中会返回该字段（官方字段路径 <c>need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【会员初始等级】 展示字段，商家可以自定义填写内容。如果选择了展示会员等级，必填 `init_level`，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息。若该值不为空，则会返回该字段（官方字段路径 <c>init_level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话。若该值不为空，则会返回该字段（官方字段路径 <c>service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【商家法务协议】 指用户与商家之间签订的具有法律效力的合同文件，旨在明确双方在交易、服务提供、权利义务等方面的规则，以保障交易安全、规范商业行为，并在纠纷发生时提供法律依据。不支持链接跳转，支持使用 `\n` 代表换行，只支持纯文本展示。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>legal_agreement</c>，选填，类型 <c>string(20480)</c>）。</summary>
    [JsonPropertyName("legal_agreement")]
    public string? LegalAgreement { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期。不传时默认为永久有效（官方字段路径 <c>valid_date_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberCardCreateRequestValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【会员中心信息】 用户点击会员卡卡面的跳转信息（官方字段路径 <c>member_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_information")]
    public BrandMemberCardCreateRequestMemberInformation? MemberInformation { get; set; }

    /// <summary>【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能。若该值不为空，则会返回该字段（官方字段路径 <c>points_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("points_information")]
    public BrandMemberCardCreateRequestPointsInformation? PointsInformation { get; set; }

    /// <summary>【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能。若该值不为空，则会返回该字段（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public BrandMemberCardCreateRequestBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价格。否则不启动该功能。若该值不为空，则会返回该字段（官方字段路径 <c>purchase_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("purchase_information")]
    public BrandMemberCardCreateRequestPurchaseInformation? PurchaseInformation { get; set; }

    /// <summary>【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会员卡；用户填写的内容会在开卡回调中返回给商家（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberCardCreateRequestUserInformation? UserInformation { get; set; }

    /// <summary>【是否支持用户主动入会】 是否允许用户在微信支付内主动开通这张会员卡。不传时默认为 true。为 true 时，用户可以通过商家名片、支付相关入会入口，以及商家小程序或 H5 的入会流程填写开卡信息并开通；会员权益、回调地址、商家法务协议必填。为 false 时，用户不能在微信支付内主动开通，只能由商家通过导入等方式开通（官方字段路径 <c>support_user_active_join</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("support_user_active_join")]
    public bool? SupportUserActiveJoin { get; set; }

    /// <summary>【是否为付费会员】 标记这张会员卡是否为付费会员。不传时为 false。本字段仅作标记，为 true 不要求填写付费会员价格和跳转信息（官方字段路径 <c>purchase_membership</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("purchase_membership")]
    public bool? PurchaseMembership { get; set; }

}

/// <summary>会员卡模板创建响应·【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponseCodeJumpInformation
{
    /// <summary>【会员码跳转AppID】 会员码跳转的小程序AppID（官方字段路径 <c>code_jump_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员码跳转路径】 会员码跳转的小程序路径（官方字段路径 <c>code_jump_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建响应·【会员卡有效期】 会员卡有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponseValidDateInformation
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

/// <summary>会员卡模板创建响应·【会员中心信息】 用户点击会员卡卡面的跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponseMemberInformation
{
    /// <summary>【会员中心跳转AppID】 用户点击会员卡卡面后跳转的小程序AppID（官方字段路径 <c>member_information.jump_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员中心跳转路径】 用户点击会员卡卡面后跳转的小程序路径（官方字段路径 <c>member_information.jump_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建响应·【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponsePointsInformation
{
    /// <summary>【积分跳转AppID】 会员积分跳转的小程序AppID（官方字段路径 <c>points_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【积分跳转路径】 会员积分跳转的小程序路径（官方字段路径 <c>points_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建响应·【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponseBalanceInformation
{
    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID（官方字段路径 <c>balance_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【储值小程序路径】 点击储值额跳转的小程序页面路径，建议为储值充值页面（官方字段路径 <c>balance_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建响应·【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponsePurchaseInformation
{
    /// <summary>【付费会员价格】 指用户为获取特定商家提供的会员权益而需支付的费用金额。（单位：分）（官方字段路径 <c>purchase_information.price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【付费会员跳转AppID】 用户购买付费会员时跳转的付费会员的AppID（官方字段路径 <c>purchase_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【付费会员跳转路径】 用户购买付费会员时购买跳转的路径（官方字段路径 <c>purchase_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板创建响应·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponseUserInformationCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型。可选取值：`CHECK_BOX` 用户可从商家提供的选项中选择多项 / `RADIO` 用户可从商家提供的选项中选择一项（官方字段路径 <c>user_information.custom_field_list.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称。若商户在会员卡模板中设置该值，则查询结果中会返回给商户（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表。 特殊规则：列表最多支持10个，单个列表限制8个字符。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户（官方字段路径 <c>user_information.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>会员卡模板创建响应·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponseUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 平台提供的通用开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。当前最多只允许传入1项（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberCardResponseUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>会员卡模板创建响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardResponse : WechatPayResponse
{
    /// <summary>【商家请求单号】 商家创建会员卡模板凭据号。商家自定义，注意保持唯一性，仅供参考的格式：品牌ID+时间戳+流水号。字符仅允许包含英文半角的数字、字母、连接线 `-` 和下划线 `_`（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【商家AppID】 商家的AppID，可以是服务号、订阅号、公众号、小程序的AppID。1、该AppID用于获取会员OpenID。2、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡类型】 支持付费、普通、储值 3 种类型。目前仅支持普通会员卡，填写付费和储值类型时会返回错误。可选取值：`PURCHASE` 付费 / `NORMAL` 普通 / `BALANCE` 储值（官方字段路径 <c>card_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>【卡名称】 1.可用于展示在卡面的名称 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>card_title</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("card_title")]
    public string? CardTitle { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>card_color</c>，<b>必填</b>，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引。不传时没有默认卡图（官方字段路径 <c>card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【会员卡code分配类型】 1、会员卡code是会员在一个会员卡模板下唯一身份标识，平台支持2种分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code 2、会员卡code分配模式若为“系统分配”，不支持修改为“商家分配”。可选取值：`SYSTEM_ALLOCATE` 系统分配 / `MERCHANT_ALLOCATE` 商家分配（官方字段路径 <c>code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【会员码展示类型】 会员码支持不展示码/二维码／条形码／二维码＋条形码/跳转商家小程序5种设置。可选取值：`NONE_CODE` 不显示任何码型 / `BAR_CODE` 条形码 / `QR_CODE` 二维码 / `BAR_CODE_AND_QR_CODE` 条形码和二维码 / `JUMP_MINI_PROGRAM` 跳转商家小程序（官方字段路径 <c>code_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必填（官方字段路径 <c>code_jump_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("code_jump_information")]
    public BrandMemberCardResponseCodeJumpInformation? CodeJumpInformation { get; set; }

    /// <summary>【会员权益】 会员权益是指平台、品牌或服务机构为付费会员（或等级会员）提供的专属优惠、服务或特权。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>benefits</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("benefits")]
    public string? Benefits { get; set; }

    /// <summary>【回调地址】 商家接收开卡成功回调通知的地址，需按照 notify_url填写注意事项 规范填写。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【是否置顶】 置顶卡是界面中的一种特殊展示模块，通过人工设置，使其固定在内容列表的顶部位置，确保用户优先看到。默认为false。同一个品牌下允许存在多张置顶卡，按照更新时间倒序排序（官方字段路径 <c>need_pinned</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_pinned")]
    public bool? NeedPinned { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）（官方字段路径 <c>need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【会员初始等级】 展示字段，商家可以自定义填写内容。如果选择了展示会员等级，必填 `init_level`，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息（官方字段路径 <c>init_level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【商家法务协议】 指用户与商家之间签订的具有法律效力的合同文件，旨在明确双方在交易、服务提供、权利义务等方面的规则，以保障交易安全、规范商业行为，并在纠纷发生时提供法律依据。不支持链接跳转，支持使用 `\n` 代表换行，只支持纯文本展示。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>legal_agreement</c>，选填，类型 <c>string(20480)</c>）。</summary>
    [JsonPropertyName("legal_agreement")]
    public string? LegalAgreement { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期（官方字段路径 <c>valid_date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberCardResponseValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【会员中心信息】 用户点击会员卡卡面的跳转信息（官方字段路径 <c>member_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_information")]
    public BrandMemberCardResponseMemberInformation? MemberInformation { get; set; }

    /// <summary>【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能（官方字段路径 <c>points_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("points_information")]
    public BrandMemberCardResponsePointsInformation? PointsInformation { get; set; }

    /// <summary>【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public BrandMemberCardResponseBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价格。否则不启动该功能（官方字段路径 <c>purchase_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("purchase_information")]
    public BrandMemberCardResponsePurchaseInformation? PurchaseInformation { get; set; }

    /// <summary>【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会员卡；用户填写的内容会在开卡回调中返回给商家（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberCardResponseUserInformation? UserInformation { get; set; }

    /// <summary>【状态】 会员卡状态信息。可选取值：`CARD_EFFECTIVE` 生效中 / `CARD_INVALID` 已失效（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>modify_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>【是否支持用户主动入会】 true 表示允许用户在微信支付内主动开通这张会员卡，用户可以通过商家名片、支付相关入会入口，以及商家小程序或 H5 的入会流程填写开卡信息并开通；false 表示用户不能在微信支付内主动开通，只能由商家通过导入等方式开通（官方字段路径 <c>support_user_active_join</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("support_user_active_join")]
    public bool? SupportUserActiveJoin { get; set; }

    /// <summary>【是否为付费会员】 true 表示该会员卡是付费会员，false 表示不是（官方字段路径 <c>purchase_membership</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("purchase_membership")]
    public bool? PurchaseMembership { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemCodeJumpInformation
{
    /// <summary>【会员码跳转AppID】 会员码跳转的小程序AppID（官方字段路径 <c>data.code_jump_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员码跳转路径】 会员码跳转的小程序路径（官方字段路径 <c>data.code_jump_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【会员卡有效期】 会员卡有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemValidDateInformation
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

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【会员中心信息】 用户点击会员卡卡面的跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemMemberInformation
{
    /// <summary>【会员中心跳转AppID】 用户点击会员卡卡面后跳转的小程序AppID（官方字段路径 <c>data.member_information.jump_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员中心跳转路径】 用户点击会员卡卡面后跳转的小程序路径（官方字段路径 <c>data.member_information.jump_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemPointsInformation
{
    /// <summary>【积分跳转AppID】 会员积分跳转的小程序AppID（官方字段路径 <c>data.points_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【积分跳转路径】 会员积分跳转的小程序路径（官方字段路径 <c>data.points_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemBalanceInformation
{
    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID（官方字段路径 <c>data.balance_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【储值小程序路径】 点击储值额跳转的小程序页面路径，建议为储值充值页面（官方字段路径 <c>data.balance_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemPurchaseInformation
{
    /// <summary>【付费会员价格】 指用户为获取特定商家提供的会员权益而需支付的费用金额。（单位：分）（官方字段路径 <c>data.purchase_information.price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【付费会员跳转AppID】 用户购买付费会员时跳转的付费会员的AppID（官方字段路径 <c>data.purchase_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【付费会员跳转路径】 用户购买付费会员时购买跳转的路径（官方字段路径 <c>data.purchase_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemUserInformationCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型。可选取值：`CHECK_BOX` 用户可从商家提供的选项中选择多项 / `RADIO` 用户可从商家提供的选项中选择一项（官方字段路径 <c>data.user_information.custom_field_list.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称。若商户在会员卡模板中设置该值，则查询结果中会返回给商户（官方字段路径 <c>data.user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表。 特殊规则：列表最多支持10个，单个列表限制8个字符。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户（官方字段路径 <c>data.user_information.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItemUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 平台提供的通用开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>data.user_information.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。当前最多只允许传入1项（官方字段路径 <c>data.user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberCardListResponseDataItemUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>会员卡模板列表响应·【会员卡模板列表】 符合条件的会员卡模板列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponseDataItem
{
    /// <summary>【商家请求单号】 商家创建会员卡模板凭据号。商家自定义，注意保持唯一性，仅供参考的格式：品牌ID+时间戳+流水号。字符仅允许包含英文半角的数字、字母、连接线 `-` 和下划线 `_`（官方字段路径 <c>data.out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>data.card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>data.brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【商家AppID】 商家的AppID，可以是服务号、订阅号、公众号、小程序的AppID。1、该AppID用于获取会员OpenID。2、该AppID需要与会员卡归属品牌有B-A关系（官方字段路径 <c>data.appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【会员卡类型】 支持付费、普通、储值 3 种类型。目前仅支持普通会员卡，填写付费和储值类型时会返回错误。可选取值：`PURCHASE` 付费 / `NORMAL` 普通 / `BALANCE` 储值（官方字段路径 <c>data.card_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>【卡名称】 1.可用于展示在卡面的名称 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>data.card_title</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("card_title")]
    public string? CardTitle { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>data.card_color</c>，<b>必填</b>，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引。不传时没有默认卡图（官方字段路径 <c>data.card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【会员卡code分配类型】 1、会员卡code是会员在一个会员卡模板下唯一身份标识，平台支持2种分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code 2、会员卡code分配模式若为“系统分配”，不支持修改为“商家分配”。可选取值：`SYSTEM_ALLOCATE` 系统分配 / `MERCHANT_ALLOCATE` 商家分配（官方字段路径 <c>data.code_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_mode")]
    public string? CodeMode { get; set; }

    /// <summary>【会员码展示类型】 会员码支持不展示码/二维码／条形码／二维码＋条形码/跳转商家小程序5种设置。可选取值：`NONE_CODE` 不显示任何码型 / `BAR_CODE` 条形码 / `QR_CODE` 二维码 / `BAR_CODE_AND_QR_CODE` 条形码和二维码 / `JUMP_MINI_PROGRAM` 跳转商家小程序（官方字段路径 <c>data.code_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必填（官方字段路径 <c>data.code_jump_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("code_jump_information")]
    public BrandMemberCardListResponseDataItemCodeJumpInformation? CodeJumpInformation { get; set; }

    /// <summary>【会员权益】 会员权益是指平台、品牌或服务机构为付费会员（或等级会员）提供的专属优惠、服务或特权。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>data.benefits</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("benefits")]
    public string? Benefits { get; set; }

    /// <summary>【回调地址】 商家接收开卡成功回调通知的地址，需按照 notify_url填写注意事项 规范填写。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>data.notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【是否置顶】 置顶卡是界面中的一种特殊展示模块，通过人工设置，使其固定在内容列表的顶部位置，确保用户优先看到。默认为false。同一个品牌下允许存在多张置顶卡，按照更新时间倒序排序（官方字段路径 <c>data.need_pinned</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_pinned")]
    public bool? NeedPinned { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）（官方字段路径 <c>data.need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【会员初始等级】 展示字段，商家可以自定义填写内容。如果选择了展示会员等级，必填 `init_level`，作为新用户开卡后的初始等级。如因商家业务规则需要变更某会员等级，可通过更新用户会员卡接口更新等级信息（官方字段路径 <c>data.init_level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>data.service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【商家法务协议】 指用户与商家之间签订的具有法律效力的合同文件，旨在明确双方在交易、服务提供、权利义务等方面的规则，以保障交易安全、规范商业行为，并在纠纷发生时提供法律依据。不支持链接跳转，支持使用 `\n` 代表换行，只支持纯文本展示。支持用户主动入会时必填。若该值不为空，则会返回该字段（官方字段路径 <c>data.legal_agreement</c>，选填，类型 <c>string(20480)</c>）。</summary>
    [JsonPropertyName("legal_agreement")]
    public string? LegalAgreement { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期（官方字段路径 <c>data.valid_date_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberCardListResponseDataItemValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【会员中心信息】 用户点击会员卡卡面的跳转信息（官方字段路径 <c>data.member_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_information")]
    public BrandMemberCardListResponseDataItemMemberInformation? MemberInformation { get; set; }

    /// <summary>【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能（官方字段路径 <c>data.points_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("points_information")]
    public BrandMemberCardListResponseDataItemPointsInformation? PointsInformation { get; set; }

    /// <summary>【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能（官方字段路径 <c>data.balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public BrandMemberCardListResponseDataItemBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价格。否则不启动该功能（官方字段路径 <c>data.purchase_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("purchase_information")]
    public BrandMemberCardListResponseDataItemPurchaseInformation? PurchaseInformation { get; set; }

    /// <summary>【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会员卡；用户填写的内容会在开卡回调中返回给商家（官方字段路径 <c>data.user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberCardListResponseDataItemUserInformation? UserInformation { get; set; }

    /// <summary>【状态】 会员卡状态信息。可选取值：`CARD_EFFECTIVE` 生效中 / `CARD_INVALID` 已失效（官方字段路径 <c>data.state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【创建时间】 创建会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新会员卡的时间，需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>data.modify_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>【是否支持用户主动入会】 true 表示允许用户在微信支付内主动开通这张会员卡，用户可以通过商家名片、支付相关入会入口，以及商家小程序或 H5 的入会流程填写开卡信息并开通；false 表示用户不能在微信支付内主动开通，只能由商家通过导入等方式开通（官方字段路径 <c>data.support_user_active_join</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("support_user_active_join")]
    public bool? SupportUserActiveJoin { get; set; }

    /// <summary>【是否为付费会员】 true 表示该会员卡是付费会员，false 表示不是（官方字段路径 <c>data.purchase_membership</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("purchase_membership")]
    public bool? PurchaseMembership { get; set; }

}

/// <summary>会员卡模板列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardListResponse : WechatPayResponse
{
    /// <summary>【会员卡模板列表】 符合条件的会员卡模板列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<BrandMemberCardListResponseDataItem>? Data { get; set; }

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

/// <summary>会员卡模板修改请求·【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestCodeJumpInformation
{
    /// <summary>【会员码跳转AppID】 会员码跳转的小程序AppID（官方字段路径 <c>code_jump_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员码跳转路径】 会员码跳转的小程序路径（官方字段路径 <c>code_jump_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板修改请求·【会员卡有效期】 会员卡有效期。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestValidDateInformation
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

/// <summary>会员卡模板修改请求·【会员中心信息】 用户点击会员卡卡面的跳转信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestMemberInformation
{
    /// <summary>【会员中心跳转AppID】 用户点击会员卡卡面后跳转的小程序AppID（官方字段路径 <c>member_information.jump_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【会员中心跳转路径】 用户点击会员卡卡面后跳转的小程序路径（官方字段路径 <c>member_information.jump_path</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板修改请求·【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestPointsInformation
{
    /// <summary>【积分跳转AppID】 会员积分跳转的小程序AppID（官方字段路径 <c>points_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【积分跳转路径】 会员积分跳转的小程序路径（官方字段路径 <c>points_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板修改请求·【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestBalanceInformation
{
    /// <summary>【储值小程序AppID】 点击储值额跳转的小程序AppID（官方字段路径 <c>balance_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【储值小程序路径】 点击储值额跳转的小程序页面路径，建议为储值充值页面（官方字段路径 <c>balance_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板修改请求·【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestPurchaseInformation
{
    /// <summary>【付费会员价格】 指用户为获取特定商家提供的会员权益而需支付的费用金额。（单位：分）（官方字段路径 <c>purchase_information.price</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>【付费会员跳转AppID】 用户购买付费会员时跳转的付费会员的AppID（官方字段路径 <c>purchase_information.jump_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

    /// <summary>【付费会员跳转路径】 用户购买付费会员时购买跳转的路径（官方字段路径 <c>purchase_information.jump_path</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("jump_path")]
    public string? JumpPath { get; set; }

}

/// <summary>会员卡模板修改请求·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会·【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestUserInformationCustomFieldListItem
{
    /// <summary>【字段类型】 选择所需的字段类型。可选取值：`CHECK_BOX` 用户可从商家提供的选项中选择多项 / `RADIO` 用户可从商家提供的选项中选择一项（官方字段路径 <c>user_information.custom_field_list.type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【字段名称】 信息项的名称。若商户在会员卡模板中设置该值，则查询结果中会返回给商户（官方字段路径 <c>user_information.custom_field_list.name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【字段值】 信息项的值列表。 特殊规则：列表最多支持10个，单个列表限制8个字符。若商户创建会员卡模板时设置该值，则查询结果中会返回给商户（官方字段路径 <c>user_information.custom_field_list.values</c>，选填，类型 <c>array[string(8)]</c>）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

}

/// <summary>会员卡模板修改请求·【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequestUserInformation
{
    /// <summary>【平台提供的通用开卡信息字段】 平台提供的通用开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。可选取值：`USER_FORM_FLAG_SEX` 用户开卡时需填写性别，字段内容可选男或女。 / `USER_FORM_FLAG_NAME` 用户开卡时需填写姓名 / `USER_FORM_FLAG_BIRTHDAY` 用户开卡时需填写生日，字段内容需遵循 [RFC3339](https://datatracker.ietf.org/doc/html/rfc3339) 标准的 `full-date` 格式：yyyy-MM-DD，表示年月日。年份必须为 4 位数，月份和日期不足两位时需补零。 示例：2015-05-20。 / `USER_FORM_FLAG_ADDRESS` 用户开卡时需填写地址 / `USER_FORM_FLAG_EMAIL` 用户开卡时需填写邮箱，字段内容需为合法邮箱。 / `USER_FORM_FLAG_CITY` 用户开卡时需填写城市，字段格式需为："省份,城市"，第一项省份，第二项城市，用英文逗号分隔。（官方字段路径 <c>user_information.common_field_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<string>? CommonFieldList { get; set; }

    /// <summary>【商家自定义的开卡信息字段】 商家自定义的开卡信息字段。若商户在会员卡模板中设置该值，则查询结果中会返回给商户。当前最多只允许传入1项（官方字段路径 <c>user_information.custom_field_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<BrandMemberCardUpdateRequestUserInformationCustomFieldListItem>? CustomFieldList { get; set; }

}

/// <summary>会员卡模板修改请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardUpdateRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【卡名称】 1.可用于展示在卡面的名称 2.支持最长10个中文字 3.支持中文字、英文字符、标点（官方字段路径 <c>card_title</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("card_title")]
    public string? CardTitle { get; set; }

    /// <summary>【卡背景颜色】 用于卡片正面设计的RGB颜色编码，仅支持十六进制（官方字段路径 <c>card_color</c>，选填，类型 <c>string(7)</c>）。</summary>
    [JsonPropertyName("card_color")]
    public string? CardColor { get; set; }

    /// <summary>【卡图片】 商家自定义会员卡背景图。仅支持通过图片上传API接口获取的图片URL地址。支持JPG/JPEG/PNG格式，建议尺寸716px\*320px，且图片小于1M。查看以下链接后传入： 图片要求示例， 图片上传API指引（官方字段路径 <c>card_picture_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("card_picture_url")]
    public string? CardPictureUrl { get; set; }

    /// <summary>【会员码跳转信息】 会员码跳转的小程序信息，当会员码展示类型为跳转商家小程序时必填（官方字段路径 <c>code_jump_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("code_jump_information")]
    public BrandMemberCardUpdateRequestCodeJumpInformation? CodeJumpInformation { get; set; }

    /// <summary>【会员权益】 会员权益是指平台、品牌或服务机构为付费会员（或等级会员）提供的专属优惠、服务或特权。支持用户主动入会时必须已填写或在本次补齐，不能改成空。不传表示不修改（官方字段路径 <c>benefits</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("benefits")]
    public string? Benefits { get; set; }

    /// <summary>【回调地址】 商家接收开卡成功回调通知的地址，需按照 notify_url填写注意事项 规范填写。支持用户主动入会时必须已填写或在本次补齐，不能改成空。不传表示不修改（官方字段路径 <c>notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【是否置顶】 置顶卡是界面中的一种特殊展示模块，通过人工设置，使其固定在内容列表的顶部位置，确保用户优先看到。默认为false。同一个品牌下允许存在多张置顶卡，按照更新时间倒序排序（官方字段路径 <c>need_pinned</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_pinned")]
    public bool? NeedPinned { get; set; }

    /// <summary>【是否展示会员等级】 是否在会员卡面向用户展示等级信息，默认不展示（false）（官方字段路径 <c>need_display_level</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("need_display_level")]
    public bool? NeedDisplayLevel { get; set; }

    /// <summary>【服务电话】 展示在会员卡详情内，建议填写商家固定电话（官方字段路径 <c>service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【会员卡有效期】 会员卡有效期（官方字段路径 <c>valid_date_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("valid_date_information")]
    public BrandMemberCardUpdateRequestValidDateInformation? ValidDateInformation { get; set; }

    /// <summary>【会员中心信息】 用户点击会员卡卡面的跳转信息（官方字段路径 <c>member_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("member_information")]
    public BrandMemberCardUpdateRequestMemberInformation? MemberInformation { get; set; }

    /// <summary>【积分信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能（官方字段路径 <c>points_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("points_information")]
    public BrandMemberCardUpdateRequestPointsInformation? PointsInformation { get; set; }

    /// <summary>【储值信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息。否则不启动该功能（官方字段路径 <c>balance_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("balance_information")]
    public BrandMemberCardUpdateRequestBalanceInformation? BalanceInformation { get; set; }

    /// <summary>【付费会员信息】 若商家名片会员卡使用该功能，需传入跳转商家小程序的信息及会员价格。否则不启动该功能（官方字段路径 <c>purchase_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("purchase_information")]
    public BrandMemberCardUpdateRequestPurchaseInformation? PurchaseInformation { get; set; }

    /// <summary>【用户开卡信息】 用户开通会员卡时可填写的信息。用户可以选择不填写，不影响开通会员卡；用户填写的内容会在开卡回调中返回给商家（官方字段路径 <c>user_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("user_information")]
    public BrandMemberCardUpdateRequestUserInformation? UserInformation { get; set; }

    /// <summary>【商家法务协议】 指用户与商家之间签订的具有法律效力的合同文件，旨在明确双方在交易、服务提供、权利义务等方面的规则，以保障交易安全、规范商业行为，并在纠纷发生时提供法律依据。不支持链接跳转，支持使用 `\n` 代表换行，只支持纯文本展示。支持用户主动入会时必须已填写或在本次补齐，不能改成空。不传表示不修改（官方字段路径 <c>legal_agreement</c>，选填，类型 <c>string(20480)</c>）。</summary>
    [JsonPropertyName("legal_agreement")]
    public string? LegalAgreement { get; set; }

    /// <summary>【会员初始等级】 展示字段，商家可以自定义填写内容。修改后「是否展示会员等级」为 true 时，必须已有会员初始等级或在本次填写，作为新用户开卡后的初始等级，与是否支持用户主动入会无关。不传表示不修改（官方字段路径 <c>init_level</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("init_level")]
    public string? InitLevel { get; set; }

    /// <summary>【是否支持用户主动入会】 是否允许用户在微信支付内主动开通这张会员卡。不传表示不修改当前设置，不会因为创建时的默认值 true 重新打开已关闭的用户主动入会。改为 true 时，用户可以通过商家名片、支付相关入会入口，以及商家小程序或 H5 的入会流程填写开卡信息并开通，会员权益、回调地址、商家法务协议必须已填写或在本次补齐；改为 false 时，用户不能在微信支付内主动开通，只能由商家通过导入等方式开通，已填写的会员权益、回调地址、商家法务协议会保留，不…（官方字段路径 <c>support_user_active_join</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("support_user_active_join")]
    public bool? SupportUserActiveJoin { get; set; }

    /// <summary>【是否为付费会员】 标记这张会员卡是否为付费会员。不传表示不修改当前设置。本字段仅作标记，改为 true 不要求填写付费会员价格和跳转信息（官方字段路径 <c>purchase_membership</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("purchase_membership")]
    public bool? PurchaseMembership { get; set; }

}

/// <summary>会员卡模板作废请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336972"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberCardInvalidateRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}
