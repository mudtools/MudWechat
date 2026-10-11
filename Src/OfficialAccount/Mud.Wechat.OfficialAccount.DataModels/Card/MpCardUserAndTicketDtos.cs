// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「获取用户卡包」请求（官方 <c>user/getcardlist</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUserGetCardListRequest
{
    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置指定卡券 ID（官方 <c>card_id</c>，可选）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}

/// <summary>
/// 用户卡包内的卡券项（官方 <c>card_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUserCardListItem
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

/// <summary>
/// 「获取用户卡包」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUserGetCardListResponse : MpResponse
{
    /// <summary>获取或设置卡券列表（官方 <c>card_list</c>）。</summary>
    [JsonPropertyName("card_list")]
    public MpCardUserCardListItem[]? CardList { get; set; }

    /// <summary>获取或设置是否有分享卡（官方 <c>has_share_card</c>）。</summary>
    [JsonPropertyName("has_share_card")]
    public bool HasShareCard { get; set; }
}

/// <summary>
/// 「机票值机」请求（官方 <c>boardingpass/checkin</c>；特殊票券族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardBoardingPassCheckinRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，自定义 code 的卡券必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置票号（官方 <c>etkt_bnr</c>，可选）。</summary>
    [JsonPropertyName("etkt_bnr")]
    public string? EtktBnr { get; set; }

    /// <summary>获取或设置舱等（官方 <c>class</c>，可选）。</summary>
    [JsonPropertyName("class")]
    public string? Class { get; set; }

    /// <summary>获取或设置乘客姓名（官方 <c>passenger_name</c>，可选）。</summary>
    [JsonPropertyName("passenger_name")]
    public string? PassengerName { get; set; }

    /// <summary>获取或设置座位号（官方 <c>seat</c>，可选）。</summary>
    [JsonPropertyName("seat")]
    public string? Seat { get; set; }

    /// <summary>获取或设置二维码数据（官方 <c>qrcode_data</c>，可选）。</summary>
    [JsonPropertyName("qrcode_data")]
    public string? QrcodeData { get; set; }

    /// <summary>获取或设置是否取消值机（官方 <c>is_cancel</c>，可选）。</summary>
    [JsonPropertyName("is_cancel")]
    public bool? IsCancel { get; set; }
}

/// <summary>
/// 「更新会议门票」请求（官方 <c>meetingticket/updateuser</c>；特殊票券族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMeetingTicketUpdateUserRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，自定义 code 的卡券必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置入场时间（官方 <c>begin_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置离场时间（官方 <c>end_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置区域（官方 <c>zone</c>，可选）。</summary>
    [JsonPropertyName("zone")]
    public string? Zone { get; set; }

    /// <summary>获取或设置入口（官方 <c>entrance</c>，可选）。</summary>
    [JsonPropertyName("entrance")]
    public string? Entrance { get; set; }

    /// <summary>获取或设置座位号（官方 <c>seat_number</c>，可选）。</summary>
    [JsonPropertyName("seat_number")]
    public string? SeatNumber { get; set; }
}

/// <summary>
/// 「更新电影票」请求（官方 <c>movieticket/updateuser</c>；特殊票券族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMovieTicketUpdateUserRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，自定义 code 的卡券必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置放映厅类别（官方 <c>ticket_class</c>，可选）。</summary>
    [JsonPropertyName("ticket_class")]
    public string? TicketClass { get; set; }

    /// <summary>获取或设置影厅名称（官方 <c>screening_room</c>，可选）。</summary>
    [JsonPropertyName("screening_room")]
    public string? ScreeningRoom { get; set; }

    /// <summary>获取或设置放映时间（官方 <c>show_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("show_time")]
    public long? ShowTime { get; set; }

    /// <summary>获取或设置影片时长（官方 <c>duration</c>，单位分钟，可选）。</summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>获取或设置座位号列表（官方 <c>seat_number</c>，可选）。</summary>
    [JsonPropertyName("seat_number")]
    public List<string>? SeatNumberList { get; set; }
}

/// <summary>
/// 「更新通用券用户信息」请求（官方 <c>generalcard/updateuser</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>custom_field 覆盖语义</b>：官方原文字段值为「覆盖更新」——传入即整字段覆盖卡面自定义字段值，
/// 不传的字段保持不变；积分 / 余额按 <c>record_*</c> 说明文案同步展示。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGeneralCardUpdateUserRequest
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

    /// <summary>获取或设置积分余额（官方 <c>bonus</c>，可选）。</summary>
    [JsonPropertyName("bonus")]
    public int? Bonus { get; set; }

    /// <summary>获取或设置积分变动说明（官方 <c>record_bonus</c>，可选）。</summary>
    [JsonPropertyName("record_bonus")]
    public string? RecordBonus { get; set; }

    /// <summary>获取或设置余额（官方 <c>balance</c>，可选）。</summary>
    [JsonPropertyName("balance")]
    public int? Balance { get; set; }

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

    /// <summary>获取或设置是否可转赠（官方 <c>can_give_friend</c>，可选）。</summary>
    [JsonPropertyName("can_give_friend")]
    public bool? CanGiveFriend { get; set; }
}

/// <summary>
/// 「更新通用券用户信息」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGeneralCardUpdateUserResponse : MpResponse
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
