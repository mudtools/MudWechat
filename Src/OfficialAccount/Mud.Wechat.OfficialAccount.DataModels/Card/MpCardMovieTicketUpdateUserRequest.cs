// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
