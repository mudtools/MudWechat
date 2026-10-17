// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构侧合作关系操作类型常量（官方 <c>hs_update_type</c>）。</summary>
public static class ChannelsLeagueHeadSupplierHsUpdateTypes
{
    /// <summary>邀请。</summary>
    public const int Invite = 1;

    /// <summary>通过。</summary>
    public const int Pass = 2;

    /// <summary>不通过。</summary>
    public const int Reject = 3;

    /// <summary>取消邀请。</summary>
    public const int CancelInvite = 4;

    /// <summary>取消合作。</summary>
    public const int CancelCooperation = 5;
}
