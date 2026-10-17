// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>选品广场佣金类型常量（官方 <c>selectionproducts/list/get</c> 的 <c>commission_type</c> 枚举值，2026-03-09 新增）。</summary>
/// <remarks>
/// 与 <c>item/headsupplier/batchadd</c> 的 <c>commission_type</c>（见 <see cref="ChannelsLeagueHeadSupplierPlanTypes"/>，取值 0/1）
/// <b>取值集合不同</b>：本域为 1/2 ⇒ 常量类分建，不得合并。
/// </remarks>
public static class ChannelsLeagueHeadSupplierSelectionCommissionTypes
{
    /// <summary>商家指定达人佣金。</summary>
    public const int MerchantDesignated = 1;

    /// <summary>机构指定达人佣金。</summary>
    public const int HeadSupplierDesignated = 2;
}
