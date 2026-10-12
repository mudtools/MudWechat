// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
