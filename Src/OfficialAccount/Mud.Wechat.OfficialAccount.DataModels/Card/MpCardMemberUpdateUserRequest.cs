// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
