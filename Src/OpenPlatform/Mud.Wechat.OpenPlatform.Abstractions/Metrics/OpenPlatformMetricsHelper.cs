// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using Mud.Wechat.Abstractions.Metrics;
using Mud.Wechat.Abstractions.Observability;

namespace Mud.Wechat.OpenPlatform.Abstractions.Metrics;

/// <summary>
/// 开放平台指标埋点入口（与公众号线 <c>MpMetricsHelper</c> 同形态）：耗时直方图与计数分开记录，
/// 计数由 Outcome 方法统一完成（避免重复计数）。
/// </summary>
public static class OpenPlatformMetricsHelper
{
    /// <summary>记录推送入站耗时（计时起点）。</summary>
    /// <param name="appKey">应用键。</param>
    /// <returns>可释放的耗时记录器（释放即落点）。</returns>
    public static IDisposable RecordCallbackRequest(string appKey)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OpenPlatform },
            { WechatActivitySource.Tags.AppKey, appKey },
        };

        return OpenPlatformMetrics.CallbackRequestDuration.RecordDuration(tags);
    }

    /// <summary>记录推送入站结果。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="outcome">操作结果（success / failure / timeout）。</param>
    /// <param name="errorType">错误类型（可选，如 decrypt / verify）。</param>
    public static void RecordCallbackOutcome(string appKey, string outcome, string? errorType = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OpenPlatform },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        if (errorType != null)
        {
            tags.Add(new(WechatActivitySource.Tags.ErrorType, errorType));
        }

        OpenPlatformMetrics.CallbackRequestCount.Add(1, tags);
    }

    /// <summary>记录事件分发耗时（计时起点）。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="eventType">事件类型（InfoType）。</param>
    /// <returns>可释放的耗时记录器（释放即落点）。</returns>
    public static IDisposable RecordEventHandling(string appKey, string eventType)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OpenPlatform },
            { WechatActivitySource.Tags.AppKey, appKey },
            { OpenPlatformMetrics.Tags.EventType, eventType },
        };

        return OpenPlatformMetrics.EventHandlingDuration.RecordDuration(tags);
    }

    /// <summary>记录事件分发结果。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="eventType">事件类型。</param>
    /// <param name="success">是否成功。</param>
    /// <param name="errorType">错误类型（可选）。</param>
    public static void RecordEventOutcome(string appKey, string eventType, bool success, string? errorType = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OpenPlatform },
            { WechatActivitySource.Tags.AppKey, appKey },
            { OpenPlatformMetrics.Tags.EventType, eventType },
            { WechatActivitySource.Tags.Outcome, success ? "success" : "failure" },
        };

        if (errorType != null)
        {
            tags.Add(new(WechatActivitySource.Tags.ErrorType, errorType));
        }

        OpenPlatformMetrics.EventHandlingCount.Add(1, tags);
    }
}
