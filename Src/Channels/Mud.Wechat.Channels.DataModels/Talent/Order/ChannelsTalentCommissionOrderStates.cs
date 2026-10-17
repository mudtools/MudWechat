// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>佣金单结算状态（官方 <c>commission_info.state</c> 枚举表）。</summary>
/// <remarks>
/// <para>本表（1~4）与 League 域「佣金单状态」（20 未结算 / 100 已结算 / 200 取消结算）是两张表 ⇒ 常量类分建。</para>
/// </remarks>
public static class ChannelsTalentCommissionOrderStates
{
    /// <summary>获取待结算（官方原文「1 待结算」）的值。</summary>
    public const int WaitSettlement = 1;

    /// <summary>获取已结算（官方原文「2 已结算」）的值。</summary>
    public const int Settled = 2;

    /// <summary>获取取消结算（官方原文「3 取消结算」）的值。</summary>
    public const int SettlementCancelled = 3;

    /// <summary>获取结算异常（官方原文「4 结算异常」）的值。</summary>
    public const int SettlementError = 4;

}
