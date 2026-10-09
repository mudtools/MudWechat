// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！
//  任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Mud.Wechat.Abstractions.Metrics;
using Mud.Wechat.Abstractions.Observability;
using Mud.Wechat.Work.Abstractions.Metrics;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// Redis 存储操作可观测性辅助类，封装指标记录与异常分类映射。
/// </summary>
/// <remarks>
/// <para>
/// 在每个 Redis 存储操作（Get/Set/Remove 等）的 <c>try-catch</c> 中使用：
/// </para>
/// <list type="number">
/// <item>操作开始时调用 <see cref="BeginOperation"/> 获取耗时记录器；</item>
/// <item>操作成功时调用 <see cref="RecordSuccess"/>；</item>
/// <item>操作失败时调用 <see cref="RecordFailure"/>（自动分类异常）。</item>
/// </list>
/// <para>
/// 指标发布到 <c>Mud.Wechat.Work</c> Meter，通过通配注册 <c>AddMeter("Mud.Wechat*")</c> 自动采集。
/// </para>
/// </remarks>
internal static class RedisMetricsHelper
{
    /// <summary>
    /// 开始记录 Redis 操作耗时。
    /// </summary>
    /// <param name="appKey">应用键（可为 null，运维场景如 ClearAsync 无应用上下文）。</param>
    /// <param name="command">Redis 命令名（取 <see cref="WorkMetrics.RedisCommands"/> 常量）。</param>
    /// <returns>可释放的耗时记录器。</returns>
    public static IDisposable BeginOperation(string? appKey, string command)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey ?? "system" },
            { WorkMetrics.Tags.RedisCommand, command },
        };

        return WorkMetrics.RedisOperationDuration.RecordDuration(tags);
    }

    /// <summary>
    /// 记录 Redis 操作成功。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="command">Redis 命令名。</param>
    public static void RecordSuccess(string? appKey, string command)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey ?? "system" },
            { WorkMetrics.Tags.RedisCommand, command },
            { WechatActivitySource.Tags.Outcome, WorkMetrics.RedisOutcomes.Success },
        };

        WorkMetrics.RedisOperationCount.Add(1, tags);
    }

    /// <summary>
    /// 记录 Redis 操作失败（自动按异常类型分类 outcome）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="command">Redis 命令名。</param>
    /// <param name="ex">捕获的异常。</param>
    public static void RecordFailure(string? appKey, string command, Exception ex)
    {
        var outcome = ClassifyException(ex);

        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey ?? "system" },
            { WorkMetrics.Tags.RedisCommand, command },
            { WechatActivitySource.Tags.Outcome, outcome },
            { WechatActivitySource.Tags.ErrorType, ex.GetType().Name },
        };

        WorkMetrics.RedisOperationCount.Add(1, tags);
    }

    /// <summary>
    /// 将异常映射为 Redis 操作结果取值（对齐 <see cref="WechatRedisErrors"/> 的分类）。
    /// </summary>
    private static string ClassifyException(Exception ex)
    {
        if (ex is WechatRedisException wre)
        {
            return wre.FailureKind switch
            {
                WechatRedisFailureKind.Timeout => WorkMetrics.RedisOutcomes.Timeout,
                WechatRedisFailureKind.Connection => WorkMetrics.RedisOutcomes.Connection,
                WechatRedisFailureKind.Server => WorkMetrics.RedisOutcomes.Server,
                WechatRedisFailureKind.InvalidArgument => WorkMetrics.RedisOutcomes.InvalidArgument,
                _ => WorkMetrics.RedisOutcomes.Server,
            };
        }

        return ex is TimeoutException
            ? WorkMetrics.RedisOutcomes.Timeout
            : WorkMetrics.RedisOutcomes.Server;
    }
}
