// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Mud.Wechat.Abstractions.Metrics;
using Mud.Wechat.Abstractions.Observability;

namespace Mud.Wechat.Work.Abstractions.Metrics;

/// <summary>
/// 企业微信 Metrics 辅助类，提供便捷的指标记录方法。
/// </summary>
/// <remarks>
/// 所有方法均以 <c>app_key</c> 为首要维度，并统一注入 <see cref="WechatActivitySource.Tags.Product"/>
/// = <see cref="WechatActivitySource.Products.Work"/>，确保跨产品线聚合时可区分。
/// </remarks>
public static class WorkMetricsHelper
{
    /// <summary>
    /// 记录回调入站请求。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="channel">回调通道（app / suite / bot）。</param>
    /// <returns>可释放的耗时记录器，<see cref="IDisposable.Dispose"/> 时记录 <c>callback.request.duration</c>。</returns>
    public static IDisposable RecordCallbackRequest(string appKey, string channel)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.Channel, channel },
        };

        // 只记录耗时直方图；计数由 RecordCallbackOutcome 统一完成（避免重复计数）。
        return WorkMetrics.CallbackRequestDuration.RecordDuration(tags);
    }

    /// <summary>
    /// 记录回调入站结果。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="channel">回调通道。</param>
    /// <param name="outcome">操作结果（success / failure / timeout）。</param>
    /// <param name="errorType">错误类型（可选，仅 <c>outcome=failure</c> 时有意义）。</param>
    public static void RecordCallbackOutcome(string appKey, string channel, string outcome, string? errorType = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.Channel, channel },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        if (errorType != null)
            tags.Add(new(WechatActivitySource.Tags.ErrorType, errorType));

        WorkMetrics.CallbackRequestCount.Add(1, tags);
    }

    /// <summary>
    /// 记录抗重放指纹命中/未命中。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="dedupType">去重类型。</param>
    /// <param name="hit">是否命中去重（<c>true</c> = 重复报文被拦截）。</param>
    public static void RecordCallbackDeduplication(string appKey, string dedupType, bool hit)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.DedupType, dedupType },
            { WechatActivitySource.Tags.Outcome, hit ? "hit" : "passed" },
        };

        WorkMetrics.CallbackDeduplicationCount.Add(1, tags);
    }

    /// <summary>
    /// 记录事件分发指标，返回 IDisposable 用于标记耗时。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="eventType">事件类型（EventTypeKey）。</param>
    /// <param name="handlerType">处理器类型名（可选）。</param>
    /// <returns>可释放的耗时记录器。</returns>
    public static IDisposable RecordEventHandling(string appKey, string eventType, string? handlerType = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.EventType, eventType },
        };

        if (handlerType != null)
            tags.Add(new(WorkMetrics.Tags.HandlerType, handlerType));

        // 只记录耗时直方图；计数由 RecordEventOutcome 统一完成（避免重复计数）。
        return WorkMetrics.EventHandlingDuration.RecordDuration(tags);
    }

    /// <summary>
    /// 记录事件分发结果。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="eventType">事件类型。</param>
    /// <param name="success">是否成功。</param>
    /// <param name="errorType">错误类型（可选）。</param>
    public static void RecordEventOutcome(string appKey, string eventType, bool success, string? errorType = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.EventType, eventType },
            { WechatActivitySource.Tags.Outcome, success ? "success" : "failure" },
        };

        if (errorType != null)
            tags.Add(new(WechatActivitySource.Tags.ErrorType, errorType));

        WorkMetrics.EventHandlingCount.Add(1, tags);
    }

    /// <summary>
    /// 记录令牌失效恢复。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="tokenOwner">令牌归属域（internal / corp / suite / provider）。</param>
    /// <param name="outcome">操作结果（success / failure）。</param>
    public static void RecordTokenInvalidation(string appKey, string tokenOwner, string outcome)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.TokenOwner, tokenOwner },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        WorkMetrics.TokenInvalidationCount.Add(1, tags);
    }

    /// <summary>
    /// 记录 Redis 操作，返回 IDisposable 用于标记耗时。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="command">Redis 命令名（取 <see cref="WorkMetrics.RedisCommands"/> 常量）。</param>
    /// <returns>可释放的耗时记录器。</returns>
    public static IDisposable RecordRedisOperation(string appKey, string command)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.RedisCommand, command },
        };

        return WorkMetrics.RedisOperationDuration.RecordDuration(tags);
    }

    /// <summary>
    /// 记录 Redis 操作结果。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="command">Redis 命令名。</param>
    /// <param name="outcome">操作结果（取 <see cref="WorkMetrics.RedisOutcomes"/> 常量）。</param>
    public static void RecordRedisOutcome(string appKey, string command, string outcome)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WorkMetrics.Tags.RedisCommand, command },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        WorkMetrics.RedisOperationCount.Add(1, tags);
    }

    /// <summary>
    /// 记录授权编排（换码/刷新/撤销）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="outcome">操作结果（success / failure）。</param>
    public static void RecordAuthorizationExchange(string appKey, string outcome)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        WorkMetrics.AuthorizationExchangeCount.Add(1, tags);
    }
}
