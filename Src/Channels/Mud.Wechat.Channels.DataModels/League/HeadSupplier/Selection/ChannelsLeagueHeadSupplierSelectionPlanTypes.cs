// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>选品广场计划类型常量（官方 <c>selectionproducts/list/get</c> 的 <c>plan_type</c> 枚举值）。</summary>
/// <remarks>官方语义：定向计划只有被定向的机构能拉取到；公开计划为商家/供货机构对所有机构可见的商品。</remarks>
public static class ChannelsLeagueHeadSupplierSelectionPlanTypes
{
    /// <summary>定向计划。</summary>
    public const int DirectedPlan = 1;

    /// <summary>公开计划。</summary>
    public const int PublicPlan = 2;
}
