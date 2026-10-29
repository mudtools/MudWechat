// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 近 30 天销量范围档位常量（官方 <c>item.product_info.monthly_sales_range</c> 枚举值）。
/// </summary>
/// <remarks>
/// 官方档位为「值 = 区间下界」的不连续集合（<c>0</c> 表示销量在 0~10，<c>10</c> 表示 10~50，依此类推，<c>10000</c> 表示 10000+），
/// 常量名按档位值命名以免误读为「销量本身」。选品广场筛选条件 <c>spu_item_condition.monthly_sales_range</c> 的区间端点同用该档位集。
/// </remarks>
public static class ChannelsLeagueHeadSupplierMonthlySalesRanges
{
    /// <summary>销量在 0~10。</summary>
    public const int Level0 = 0;

    /// <summary>销量在 10~50。</summary>
    public const int Level10 = 10;

    /// <summary>销量在 50~100。</summary>
    public const int Level50 = 50;

    /// <summary>销量在 100~300。</summary>
    public const int Level100 = 100;

    /// <summary>销量在 300~500。</summary>
    public const int Level300 = 300;

    /// <summary>销量在 500~800。</summary>
    public const int Level500 = 500;

    /// <summary>销量在 800~1000。</summary>
    public const int Level800 = 800;

    /// <summary>销量在 1000~3000。</summary>
    public const int Level1000 = 1000;

    /// <summary>销量在 3000~5000。</summary>
    public const int Level3000 = 3000;

    /// <summary>销量在 5000~8000。</summary>
    public const int Level5000 = 5000;

    /// <summary>销量在 8000~10000。</summary>
    public const int Level8000 = 8000;

    /// <summary>销量在 10000+。</summary>
    public const int Level10000 = 10000;
}
