// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 会员卡的 <c>base_info</c>（查询方向）—— 通用查询形态 + <c>need_push_on_view</c>。
/// </summary>
/// <remarks><b>与建卡方向的会员卡 base 不同</b>：查询形态<b>无</b> <c>pay_info</c>（SKIT 对齐基准）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryMemberCardBaseInfo : MpCardQueryBaseInfo
{
    /// <summary>获取或设置进入卡面时是否推送事件（官方 <c>need_push_on_view</c>）。</summary>
    [JsonPropertyName("need_push_on_view")]
    public bool? NeedPushOnView { get; set; }
}
