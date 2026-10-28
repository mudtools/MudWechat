// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 礼品卡（修改方向，官方 <c>general_card</c>）。
/// </summary>
/// <remarks>
/// <b>字段面窄于建卡</b>：本向<b>无</b> <c>sub_card_type</c> / <c>giftcard_info</c> /
/// <c>max_give_friend_times</c>（卡种与面额建卡后不可改），也<b>无</b> <c>init_bonus</c> / <c>init_balance</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateGeneralCard
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，礼品卡修改形态）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateGiftCardBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置背景图 URL（官方 <c>background_pic_url</c>，选填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置特权说明（官方 <c>prerogative</c>，选填）。</summary>
    [JsonPropertyName("prerogative")]
    public string? Prerogative { get; set; }

    /// <summary>获取或设置是否领取后自动激活（官方 <c>auto_activate</c>，选填）。</summary>
    [JsonPropertyName("auto_activate")]
    public bool? AutoActivate { get; set; }

    /// <summary>获取或设置是否支持积分（官方 <c>supply_bonus</c>，选填）。</summary>
    [JsonPropertyName("supply_bonus")]
    public bool? SupplyBonus { get; set; }

    /// <summary>获取或设置是否支持余额（官方 <c>supply_balance</c>，选填）。</summary>
    [JsonPropertyName("supply_balance")]
    public bool? SupplyBalance { get; set; }

    /// <summary>获取或设置自定义会员信息类目 1（官方 <c>custom_field1</c>）。</summary>
    [JsonPropertyName("custom_field1")]
    public MpCardCustomField? CustomField1 { get; set; }

    /// <summary>获取或设置自定义会员信息类目 2（官方 <c>custom_field2</c>）。</summary>
    [JsonPropertyName("custom_field2")]
    public MpCardCustomField? CustomField2 { get; set; }

    /// <summary>获取或设置自定义会员信息类目 3（官方 <c>custom_field3</c>）。</summary>
    [JsonPropertyName("custom_field3")]
    public MpCardCustomField? CustomField3 { get; set; }
}
