// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>帮卖供货计划操作类型常量（官方 <c>secondheadsupplieritem/update</c> 的 <c>update_type</c>）。</summary>
public static class ChannelsLeagueHeadSupplierHelpSellUpdateTypes
{
    /// <summary>更新佣金率。</summary>
    public const int UpdateRatio = 1;

    /// <summary>结束推广。</summary>
    public const int EndPromotion = 2;
}
