// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 查询方向的 <c>card</c> 对象（<c>card_type</c> + 10 个券型分支，<b>分支互斥</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与建卡方向的分支差异（对齐基准：SKIT <c>CardGetResponse.Types.Card</c>，2026-10-10）</b>：
/// 本向<b>无</b> <c>general_card</c>（礼品卡）分支 —— 礼品卡的详情由 <c>/card/giftcard/*</c> 族承载，
/// 不经本接口返回。故本型刻意<b>不</b>与 <see cref="MpCardCreateBody"/> 共用。
/// </para>
/// <para><b>易错点</b>：官方建卡用 <c>gift</c> 表示「兑换券」，本向同名键亦为兑换券，<b>不是</b>礼品卡。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryDetail
{
    /// <summary>获取或设置卡券类型（官方 <c>card_type</c>，判别取哪个分支有值）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>获取或设置团购券信息（官方 <c>groupon</c>）。</summary>
    [JsonPropertyName("groupon")]
    public MpCardQueryGroupon? Groupon { get; set; }

    /// <summary>获取或设置代金券信息（官方 <c>cash</c>）。</summary>
    [JsonPropertyName("cash")]
    public MpCardQueryCash? Cash { get; set; }

    /// <summary>获取或设置折扣券信息（官方 <c>discount</c>）。</summary>
    [JsonPropertyName("discount")]
    public MpCardQueryDiscount? Discount { get; set; }

    /// <summary>获取或设置<b>兑换券</b>信息（官方 <c>gift</c>）。</summary>
    [JsonPropertyName("gift")]
    public MpCardQueryGift? Gift { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>general_coupon</c>）。</summary>
    [JsonPropertyName("general_coupon")]
    public MpCardQueryGeneralCoupon? GeneralCoupon { get; set; }

    /// <summary>获取或设置会员卡信息（官方 <c>member_card</c>）。</summary>
    [JsonPropertyName("member_card")]
    public MpCardQueryMemberCard? MemberCard { get; set; }

    /// <summary>获取或设置会议门票信息（官方 <c>meeting_ticket</c>）。</summary>
    [JsonPropertyName("meeting_ticket")]
    public MpCardQueryMeetingTicket? MeetingTicket { get; set; }

    /// <summary>获取或设置景区门票信息（官方 <c>scenic_ticket</c>）。</summary>
    [JsonPropertyName("scenic_ticket")]
    public MpCardQueryScenicTicket? ScenicTicket { get; set; }

    /// <summary>获取或设置电影票信息（官方 <c>movie_ticket</c>）。</summary>
    [JsonPropertyName("movie_ticket")]
    public MpCardQueryMovieTicket? MovieTicket { get; set; }

    /// <summary>获取或设置飞机票信息（官方 <c>boarding_pass</c>）。</summary>
    [JsonPropertyName("boarding_pass")]
    public MpCardQueryBoardingPass? BoardingPass { get; set; }
}
