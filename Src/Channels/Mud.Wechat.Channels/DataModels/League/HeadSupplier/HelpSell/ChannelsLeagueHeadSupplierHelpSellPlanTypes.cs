// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>帮卖供货计划类型常量（官方 <c>secondheadsupplieritem/{add,update}</c> 的 <c>plan_type</c>）。
/// 与选品广场 <see cref="ChannelsLeagueHeadSupplierSelectionPlanTypes"/> 取值相同（1 定向 / 2 公开）但属不同接口的枚举表。</summary>
public static class ChannelsLeagueHeadSupplierHelpSellPlanTypes
{
    /// <summary>定向计划。</summary>
    public const int DirectedPlan = 1;

    /// <summary>公开计划。</summary>
    public const int PublicPlan = 2;
}
