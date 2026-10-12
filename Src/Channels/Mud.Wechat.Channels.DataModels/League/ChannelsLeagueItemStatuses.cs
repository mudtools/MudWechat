// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>商品推广状态常量（官方 <c>status</c> 枚举值，<c>item/get</c> 与 <c>item/list/get</c> 同值域）。</summary>
public static class ChannelsLeagueItemStatuses
{
    /// <summary>已上架推广。</summary>
    public const int Listed = 1;

    /// <summary>已下架推广。</summary>
    public const int Delisted = 2;

    /// <summary>已删除。</summary>
    public const int Deleted = 4;

    /// <summary>未达到准入标准。</summary>
    public const int BelowThreshold = 5;

    /// <summary>商品下架终止推广（官方 2026-07-15 新增）。</summary>
    public const int TerminatedByProductDelisted = 7;

    /// <summary>计划过期终止推广（官方 2026-07-15 新增）。</summary>
    public const int TerminatedByPlanExpired = 8;

    /// <summary>商品被封禁下架终止推广（官方 2026-07-15 新增）。</summary>
    public const int TerminatedByProductBanned = 9;

    /// <summary>待生效。</summary>
    public const int PendingEffective = 10;
}
