// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 修改卡券（POST /card/update）
//  请求形态与建卡**不同构**：本向为「card_id + 券型分支」**平级**（无 card 包装、无 card_type），
//  由「传了哪个分支键」判别改哪一支；且各分支**无 advanced_info**（高级信息建卡后不可改）。

/// <summary>
/// 修改卡券（<c>POST /card/update</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>顶层平级形态</b>（对齐基准：SKIT <c>CardUpdateRequest</c>，2026-10-10）：<c>card_id</c> 与 11 个券型分支
/// 同层，<b>没有</b>建卡那样的 <c>card</c> 包装、也<b>没有</b> <c>card_type</c> 判别字段。
/// 官方文档站正文本次不可达，该形态<b>待逐页核验</b>；守卫 CD4 锁定「本向无 card 包装」这一裁决，
/// 防止后人按建卡形态「顺手对齐」。
/// </para>
/// <para><b>分支互斥</b>：按 <c>card_id</c> 对应的实际券型只传该支，其余留空（不传即不改）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置团购券信息（官方 <c>groupon</c>）。</summary>
    [JsonPropertyName("groupon")]
    public MpCardUpdateGroupon? Groupon { get; set; }

    /// <summary>获取或设置代金券信息（官方 <c>cash</c>）。</summary>
    [JsonPropertyName("cash")]
    public MpCardUpdateCash? Cash { get; set; }

    /// <summary>获取或设置折扣券信息（官方 <c>discount</c>）。</summary>
    [JsonPropertyName("discount")]
    public MpCardUpdateDiscount? Discount { get; set; }

    /// <summary>获取或设置<b>兑换券</b>信息（官方 <c>gift</c>）。</summary>
    [JsonPropertyName("gift")]
    public MpCardUpdateGift? Gift { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>general_coupon</c>）。</summary>
    [JsonPropertyName("general_coupon")]
    public MpCardUpdateGeneralCoupon? GeneralCoupon { get; set; }

    /// <summary>获取或设置<b>礼品卡</b>信息（官方 <c>general_card</c>）。</summary>
    [JsonPropertyName("general_card")]
    public MpCardUpdateGeneralCard? GeneralCard { get; set; }

    /// <summary>获取或设置<b>会员卡</b>信息（官方 <c>member_card</c>）。</summary>
    [JsonPropertyName("member_card")]
    public MpCardUpdateMemberCard? MemberCard { get; set; }

    /// <summary>获取或设置会议门票信息（官方 <c>meeting_ticket</c>）。</summary>
    [JsonPropertyName("meeting_ticket")]
    public MpCardUpdateMeetingTicket? MeetingTicket { get; set; }

    /// <summary>获取或设置景区门票信息（官方 <c>scenic_ticket</c>）。</summary>
    [JsonPropertyName("scenic_ticket")]
    public MpCardUpdateScenicTicket? ScenicTicket { get; set; }

    /// <summary>获取或设置电影票信息（官方 <c>movie_ticket</c>）。</summary>
    [JsonPropertyName("movie_ticket")]
    public MpCardUpdateMovieTicket? MovieTicket { get; set; }

    /// <summary>获取或设置飞机票信息（官方 <c>boarding_pass</c>）。</summary>
    [JsonPropertyName("boarding_pass")]
    public MpCardUpdateBoardingPass? BoardingPass { get; set; }
}
