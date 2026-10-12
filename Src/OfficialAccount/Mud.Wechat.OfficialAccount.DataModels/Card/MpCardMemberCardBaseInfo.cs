// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 会员卡的 <c>base_info</c>（建卡方向）—— 通用形态 + <c>need_push_on_view</c> + <c>pay_info</c>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberCardBaseInfo : MpCardCreateBaseInfo
{
    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>，选填）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }

    /// <summary>获取或设置微信支付信息（官方 <c>pay_info</c>，选填）。</summary>
    [JsonPropertyName("pay_info")]
    public MpCardPayInfo? PayInfo { get; set; }
}
