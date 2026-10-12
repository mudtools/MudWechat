// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>商品推广类别 / 佣金类型常量（官方响应 <c>item.type</c> 与 <c>talent_commission_info.commission_type</c> 枚举值，值域同为 1/2/3）。</summary>
public static class ChannelsLeaguePromotionTypes
{
    /// <summary>普通推广商品（佣金类型侧为「普通计划」）。</summary>
    public const int Normal = 1;

    /// <summary>定向推广商品（佣金类型侧为「定向计划」）。</summary>
    public const int Targeted = 2;

    /// <summary>专属推广商品（佣金类型侧为「专属计划」）。</summary>
    public const int Exclusive = 3;
}
