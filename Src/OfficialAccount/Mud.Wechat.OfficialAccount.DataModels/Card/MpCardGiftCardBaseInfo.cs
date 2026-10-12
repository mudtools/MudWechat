// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 礼品卡的 <c>base_info</c>（建卡方向）—— 通用形态 + <c>giftcard_info</c> + <c>max_give_friend_times</c> +
/// <c>need_push_on_view</c>。
/// </summary>
/// <remarks>
/// <b>继承而非重复声明</b>：本型在 <see cref="MpCardCreateBaseInfo"/> 之上<b>只增三字段</b>，
/// 与 SKIT 的 <c>GeneralCard.Types.Base : GrouponCard.Types.Base</c> 同构；守卫 CD4 锁定「继承即等价于
/// 通用形态字段全集」这一复用选择（若日后官方礼品卡页删除某通用字段，须改为分建而非改基类）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftCardBaseInfo : MpCardCreateBaseInfo
{
    /// <summary>获取或设置礼品卡面额（官方 <c>giftcard_info</c>，建卡必填）。</summary>
    [JsonPropertyName("giftcard_info")]
    public MpCardGiftCardPriceInfo? GiftCardInfo { get; set; }

    /// <summary>获取或设置最大可赠送次数（官方 <c>max_give_friend_times</c>，必填）。</summary>
    [JsonPropertyName("max_give_friend_times")]
    public int MaxGiveFriendTimes { get; set; }

    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>，选填）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }
}
