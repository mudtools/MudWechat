// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 礼品卡（官方 <c>card.general_card</c>，<b>2026-10-10 对齐基准为 SKIT</b>；建卡方向独有序列）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateGeneralCard
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>，礼品卡形态见 <see cref="MpCardGiftCardBaseInfo"/>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardGiftCardBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置高级信息（官方 <c>advanced_info</c>）。</summary>
    [JsonPropertyName("advanced_info")]
    public MpCardAdvancedInfo? AdvancedInfo { get; set; }

    /// <summary>获取或设置礼品卡子类型（官方 <c>sub_card_type</c>，建卡必填）。</summary>
    [JsonPropertyName("sub_card_type")]
    public string? SubCardType { get; set; }

    /// <summary>获取或设置背景图 URL（官方 <c>background_pic_url</c>，选填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置特权说明（官方 <c>prerogative</c>，必填）。</summary>
    [JsonPropertyName("prerogative")]
    public string? Prerogative { get; set; }

    /// <summary>获取或设置是否领取后自动激活（官方 <c>auto_activate</c>，选填）。</summary>
    [JsonPropertyName("auto_activate")]
    public bool? AutoActivate { get; set; }

    /// <summary>获取或设置是否支持积分（官方 <c>supply_bonus</c>，必填）。</summary>
    [JsonPropertyName("supply_bonus")]
    public bool SupplyBonus { get; set; }

    /// <summary>获取或设置初始积分（官方 <c>init_bonus</c>，选填）。</summary>
    [JsonPropertyName("init_bonus")]
    public int? InitBonus { get; set; }

    /// <summary>获取或设置是否支持余额（官方 <c>supply_balance</c>，必填）。</summary>
    [JsonPropertyName("supply_balance")]
    public bool SupplyBalance { get; set; }

    /// <summary>获取或设置初始余额（官方 <c>init_balance</c>，单位：分，选填）。</summary>
    [JsonPropertyName("init_balance")]
    public int? InitBalance { get; set; }

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
