// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 创建卡券请求体的 <c>card</c> 对象（<c>card_type</c> + 11 个券型分支，<b>分支互斥</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>分支与 <c>card_type</c> 的对应</b>：groupon 团购券 / cash 代金券 / discount 折扣券 / gift <b>兑换券</b> /
/// general_coupon 优惠券 / general_card <b>礼品卡</b> / member_card <b>会员卡</b> / meeting_ticket 会议门票 /
/// scenic_ticket 景区门票 / movie_ticket 电影票 / boarding_pass 飞机票。
/// </para>
/// <para>
/// <b>易错点（照官方键名，勿凭常识改写）</b>：<c>gift</c> 是「兑换券」而非礼品卡，礼品卡的官方键是
/// <c>general_card</c>；会员卡键为 <c>member_card</c>（<b>不存在</b> <c>membership_card</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateBody
{
    /// <summary>获取或设置卡券类型（官方 <c>card_type</c>，必填，取值见本类型 remarks 的分支对应表）。</summary>
    [JsonPropertyName("card_type")]
    public string CardType { get; set; } = string.Empty;

    /// <summary>获取或设置团购券信息（官方 <c>groupon</c>）。</summary>
    [JsonPropertyName("groupon")]
    public MpCardCreateGroupon? Groupon { get; set; }

    /// <summary>获取或设置代金券信息（官方 <c>cash</c>）。</summary>
    [JsonPropertyName("cash")]
    public MpCardCreateCash? Cash { get; set; }

    /// <summary>获取或设置折扣券信息（官方 <c>discount</c>）。</summary>
    [JsonPropertyName("discount")]
    public MpCardCreateDiscount? Discount { get; set; }

    /// <summary>获取或设置<b>兑换券</b>信息（官方 <c>gift</c>；注意与礼品卡 <see cref="GeneralCard"/> 不是一回事）。</summary>
    [JsonPropertyName("gift")]
    public MpCardCreateGift? Gift { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>general_coupon</c>）。</summary>
    [JsonPropertyName("general_coupon")]
    public MpCardCreateGeneralCoupon? GeneralCoupon { get; set; }

    /// <summary>获取或设置<b>礼品卡</b>信息（官方 <c>general_card</c>）。</summary>
    [JsonPropertyName("general_card")]
    public MpCardCreateGeneralCard? GeneralCard { get; set; }

    /// <summary>获取或设置<b>会员卡</b>信息（官方 <c>member_card</c>）。</summary>
    [JsonPropertyName("member_card")]
    public MpCardCreateMemberCard? MemberCard { get; set; }

    /// <summary>获取或设置会议门票信息（官方 <c>meeting_ticket</c>）。</summary>
    [JsonPropertyName("meeting_ticket")]
    public MpCardCreateMeetingTicket? MeetingTicket { get; set; }

    /// <summary>获取或设置景区门票信息（官方 <c>scenic_ticket</c>）。</summary>
    [JsonPropertyName("scenic_ticket")]
    public MpCardCreateScenicTicket? ScenicTicket { get; set; }

    /// <summary>获取或设置电影票信息（官方 <c>movie_ticket</c>）。</summary>
    [JsonPropertyName("movie_ticket")]
    public MpCardCreateMovieTicket? MovieTicket { get; set; }

    /// <summary>获取或设置飞机票信息（官方 <c>boarding_pass</c>）。</summary>
    [JsonPropertyName("boarding_pass")]
    public MpCardCreateBoardingPass? BoardingPass { get; set; }
}
