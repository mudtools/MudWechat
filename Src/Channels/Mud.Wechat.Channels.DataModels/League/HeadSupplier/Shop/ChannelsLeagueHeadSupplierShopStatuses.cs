// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构与商家合作状态常量（官方 <c>status</c>，<c>headsupplier/shop/{get,list/get}</c>）。</summary>
public static class ChannelsLeagueHeadSupplierShopStatuses
{
    /// <summary>邀请中。</summary>
    public const int Inviting = 1;

    /// <summary>已接受邀请。</summary>
    public const int Accepted = 2;

    /// <summary>已拒绝邀请（官方 <c>shop/get</c> 页写「已接拒绝邀请」、<c>shop/list/get</c> 页写「已拒绝邀请」，同一枚举值）。</summary>
    public const int Rejected = 3;

    /// <summary>取消邀请。</summary>
    public const int InviteCancelled = 4;

    /// <summary>取消合作。</summary>
    public const int CooperationCancelled = 5;
}
