// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// 会员卡用户结构化信息复用既有 MpCardMemberUserInfo / MpCardUserField（MpCardCodeDtos.cs，同形共用——
// common_field_list / custom_field_list 两列在 userinfo/get 与 activatetempinfo/get 两端点形态一致）。

/// <summary>
/// 「查询会员卡用户信息」请求（官方 <c>membercard/userinfo/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberUserInfoGetRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// 「查询会员卡用户信息」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberUserInfoGetResponse : MpResponse
{
    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置性别（官方 <c>sex</c>）。</summary>
    [JsonPropertyName("sex")]
    public string? Sex { get; set; }

    /// <summary>获取或设置会员号（官方 <c>membership_number</c>）。</summary>
    [JsonPropertyName("membership_number")]
    public string? MembershipNumber { get; set; }

    /// <summary>获取或设置结构化用户信息（官方 <c>user_info</c>，可选）。</summary>
    [JsonPropertyName("user_info")]
    public MpCardMemberUserInfo? UserInfo { get; set; }

    /// <summary>获取或设置积分余额（官方 <c>bonus</c>）。</summary>
    [JsonPropertyName("bonus")]
    public int Bonus { get; set; }

    /// <summary>获取或设置余额（官方 <c>balance</c>）。</summary>
    [JsonPropertyName("balance")]
    public int Balance { get; set; }

    /// <summary>获取或设置用户卡状态（官方 <c>user_card_status</c>）。</summary>
    [JsonPropertyName("user_card_status")]
    public string? UserCardStatus { get; set; }
}

/// <summary>
/// 服务声明链接（官方 <c>service_statement</c> / <c>bind_old_card</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberStatement
{
    /// <summary>获取或设置声明名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置声明链接（官方 <c>url</c>，必填）。</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// 会员卡激活表单富文本字段（官方 <c>rich_field_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberRichField
{
    /// <summary>获取或设置字段类型（官方 <c>type</c>；FORM_FIELD_MOBILE 等）。</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>获取或设置字段名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置字段值列表（官方 <c>values</c>，必填）。</summary>
    [JsonPropertyName("values")]
    public List<string> ValueList { get; set; } = new();
}

/// <summary>
/// 会员卡激活表单（官方 <c>required_form</c> / <c>optional_form</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberForm
{
    /// <summary>获取或设置是否允许用户修改（官方 <c>can_modify</c>，可选）。</summary>
    [JsonPropertyName("can_modify")]
    public bool? CanModify { get; set; }

    /// <summary>获取或设置富文本字段列表（官方 <c>rich_field_list</c>，可选）。</summary>
    [JsonPropertyName("rich_field_list")]
    public List<MpCardMemberRichField>? RichFieldList { get; set; }

    /// <summary>获取或设置通用字段 ID 列表（官方 <c>common_field_id_list</c>，可选）。</summary>
    [JsonPropertyName("common_field_id_list")]
    public List<string>? CommonFieldIdList { get; set; }

    /// <summary>获取或设置自定义字段 ID 列表（官方 <c>custom_field_list</c>，可选）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<string>? CustomFieldList { get; set; }
}

/// <summary>
/// 「设置会员卡激活表单」请求（官方 <c>membercard/activateuserform/set</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberActivateUserFormSetRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置服务声明（官方 <c>service_statement</c>，可选）。</summary>
    [JsonPropertyName("service_statement")]
    public MpCardMemberStatement? ServiceStatement { get; set; }

    /// <summary>获取或设置绑定老卡声明（官方 <c>bind_old_card</c>，可选）。</summary>
    [JsonPropertyName("bind_old_card")]
    public MpCardMemberStatement? BindOldCard { get; set; }

    /// <summary>获取或设置必填表单（官方 <c>required_form</c>，可选）。</summary>
    [JsonPropertyName("required_form")]
    public MpCardMemberForm? RequiredForm { get; set; }

    /// <summary>获取或设置选填表单（官方 <c>optional_form</c>，可选）。</summary>
    [JsonPropertyName("optional_form")]
    public MpCardMemberForm? OptionalForm { get; set; }
}

/// <summary>
/// 「更新会员卡用户信息」请求。
/// </summary>
/// <remarks>
/// <para>
/// <b>custom_field 覆盖语义（官方原文）</b>：传入的自定义字段值<b>整字段覆盖</b>卡面展示；
/// <c>add_bonus</c>/<c>add_balance</c> 为增量、<c>bonus</c>/<c>balance</c> 为全量，二者<b>二选一</b>不可同时传入。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberUpdateUserRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置背景图片 URL（官方 <c>background_pic_url</c>，可选）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置积分全量值（官方 <c>bonus</c>，可选；与 add_bonus 二选一）。</summary>
    [JsonPropertyName("bonus")]
    public int? Bonus { get; set; }

    /// <summary>获取或设置积分增量（官方 <c>add_bonus</c>，可选；与 bonus 二选一）。</summary>
    [JsonPropertyName("add_bonus")]
    public int? AddBonus { get; set; }

    /// <summary>获取或设置积分变动说明（官方 <c>record_bonus</c>，可选）。</summary>
    [JsonPropertyName("record_bonus")]
    public string? RecordBonus { get; set; }

    /// <summary>获取或设置余额全量值（官方 <c>balance</c>，可选；与 add_balance 二选一）。</summary>
    [JsonPropertyName("balance")]
    public int? Balance { get; set; }

    /// <summary>获取或设置余额增量（官方 <c>add_balance</c>，可选；与 balance 二选一）。</summary>
    [JsonPropertyName("add_balance")]
    public int? AddBalance { get; set; }

    /// <summary>获取或设置余额变动说明（官方 <c>record_balance</c>，可选）。</summary>
    [JsonPropertyName("record_balance")]
    public string? RecordBalance { get; set; }

    /// <summary>获取或设置自定义字段 1 值（官方 <c>custom_field_value1</c>，可选）。</summary>
    [JsonPropertyName("custom_field_value1")]
    public string? CustomFieldValue1 { get; set; }

    /// <summary>获取或设置自定义字段 2 值（官方 <c>custom_field_value2</c>，可选）。</summary>
    [JsonPropertyName("custom_field_value2")]
    public string? CustomFieldValue2 { get; set; }

    /// <summary>获取或设置自定义字段 3 值（官方 <c>custom_field_value3</c>，可选）。</summary>
    [JsonPropertyName("custom_field_value3")]
    public string? CustomFieldValue3 { get; set; }

    /// <summary>获取或设置通知选项（官方 <c>notify_optional</c>，可选）。</summary>
    [JsonPropertyName("notify_optional")]
    public MpCardMemberNotifyOption? NotifyOptional { get; set; }
}

/// <summary>
/// 会员卡变更通知选项（官方 <c>notify_optional</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberNotifyOption
{
    /// <summary>是否推送积分变更通知（官方 <c>is_notify_bonus</c>，可选）。</summary>
    [JsonPropertyName("is_notify_bonus")]
    public bool? IsNotifyBonus { get; set; }

    /// <summary>是否推送余额变更通知（官方 <c>is_notify_balance</c>，可选）。</summary>
    [JsonPropertyName("is_notify_balance")]
    public bool? IsNotifyBalance { get; set; }

    /// <summary>是否推送自定义字段 1 变更通知（官方 <c>is_notify_custom_field1</c>，可选）。</summary>
    [JsonPropertyName("is_notify_custom_field1")]
    public bool? IsNotifyCustomField1 { get; set; }

    /// <summary>是否推送自定义字段 2 变更通知（官方 <c>is_notify_custom_field2</c>，可选）。</summary>
    [JsonPropertyName("is_notify_custom_field2")]
    public bool? IsNotifyCustomField2 { get; set; }

    /// <summary>是否推送自定义字段 3 变更通知（官方 <c>is_notify_custom_field3</c>，可选）。</summary>
    [JsonPropertyName("is_notify_custom_field3")]
    public bool? IsNotifyCustomField3 { get; set; }
}

/// <summary>
/// 「更新会员卡用户信息」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberUpdateUserResponse : MpResponse
{
    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置变动后积分余额（官方 <c>result_bonus</c>）。</summary>
    [JsonPropertyName("result_bonus")]
    public int ResultBonus { get; set; }

    /// <summary>获取或设置变动后余额（官方 <c>result_balance</c>）。</summary>
    [JsonPropertyName("result_balance")]
    public int ResultBalance { get; set; }
}

/// <summary>
/// 「激活会员卡（接口）」请求（官方 <c>membercard/activate</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberActivateRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，可选）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置会员号（官方 <c>membership_number</c>，必填）。</summary>
    [JsonPropertyName("membership_number")]
    public string MembershipNumber { get; set; } = string.Empty;

    /// <summary>获取或设置背景图片 URL（官方 <c>background_pic_url</c>，可选）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置激活有效起始时间（官方 <c>activate_begin_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("activate_begin_time")]
    public long? ActivateBeginTime { get; set; }

    /// <summary>获取或设置激活有效截止时间（官方 <c>activate_end_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("activate_end_time")]
    public long? ActivateEndTime { get; set; }

    /// <summary>获取或设置初始积分（官方 <c>init_bonus</c>，可选）。</summary>
    [JsonPropertyName("init_bonus")]
    public int? InitBonus { get; set; }

    /// <summary>获取或设置初始积分说明（官方 <c>init_bonus_record</c>，可选）。</summary>
    [JsonPropertyName("init_bonus_record")]
    public string? InitBonusRecord { get; set; }

    /// <summary>获取或设置初始余额（官方 <c>init_balance</c>，可选）。</summary>
    [JsonPropertyName("init_balance")]
    public int? InitBalance { get; set; }

    /// <summary>获取或设置初始自定义字段 1 值（官方 <c>init_custom_field_value1</c>，可选）。</summary>
    [JsonPropertyName("init_custom_field_value1")]
    public string? InitCustomFieldValue1 { get; set; }

    /// <summary>获取或设置初始自定义字段 2 值（官方 <c>init_custom_field_value2</c>，可选）。</summary>
    [JsonPropertyName("init_custom_field_value2")]
    public string? InitCustomFieldValue2 { get; set; }

    /// <summary>获取或设置初始自定义字段 3 值（官方 <c>init_custom_field_value3</c>，可选）。</summary>
    [JsonPropertyName("init_custom_field_value3")]
    public string? InitCustomFieldValue3 { get; set; }
}

/// <summary>
/// 「获取会员卡激活链接」请求（官方 <c>membercard/activate/geturl</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberActivateGetUrlRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置渠道参数（官方 <c>outer_str</c>，可选）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }
}

/// <summary>
/// 「获取会员卡激活链接」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberActivateGetUrlResponse : MpResponse
{
    /// <summary>获取或设置激活链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 「获取用户激活信息（临时票）」请求（官方 <c>membercard/activatetempinfo/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberActivateTempInfoGetRequest
{
    /// <summary>获取或设置激活Ticket（官方 <c>activate_ticket</c>，必填；用户点击激活时经事件推送下发）。</summary>
    [JsonPropertyName("activate_ticket")]
    public string ActivateTicket { get; set; } = string.Empty;
}

/// <summary>
/// 「获取用户激活信息（临时票）」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberActivateTempInfoGetResponse : MpResponse
{
    /// <summary>获取或设置用户填写的激活信息（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public MpCardMemberUserInfo? Info { get; set; }
}
