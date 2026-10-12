// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>请求侧「获取商品推广类别」常量（官方请求体 <c>type</c> 枚举值，<c>item/batchadd</c> / <c>item/get</c> / <c>item/upd</c>）。</summary>
/// <remarks>与计划/佣金侧「普通/定向/专属」三分类（<see cref="ChannelsLeaguePromotionTypes"/>，值 1/2/3）是两套不同枚举，勿混用。</remarks>
public static class ChannelsLeaguePlanQueryTypes
{
    /// <summary>全部带货者计划。</summary>
    public const int All = 100;

    /// <summary>指定带货者计划。</summary>
    public const int Designated = 300;
}
