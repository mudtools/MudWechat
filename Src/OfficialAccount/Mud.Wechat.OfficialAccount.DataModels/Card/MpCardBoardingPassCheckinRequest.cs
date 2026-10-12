// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
