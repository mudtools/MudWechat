// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
