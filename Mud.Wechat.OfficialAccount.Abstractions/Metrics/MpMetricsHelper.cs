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

namespace Mud.Wechat.OfficialAccount.Abstractions.Metrics;

/// <summary>
/// 公众号 Metrics 辅助类，提供便捷的指标记录方法。
/// </summary>
/// <remarks>
/// 所有方法均以 <c>app_key</c> 为首要维度，并统一注入 <see cref="WechatActivitySource.Tags.Product"/>
/// = <see cref="WechatActivitySource.Products.OfficialAccount"/>，确保跨产品线聚合时可区分。
/// </remarks>
public static class MpMetricsHelper
{
    /// <summary>
    /// 记录回调入站请求。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <returns>可释放的耗时记录器。</returns>
    public static IDisposable RecordCallbackRequest(string appKey)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OfficialAccount },
            { WechatActivitySource.Tags.AppKey, appKey },
        };

        MpMetrics.CallbackRequestCount.Add(1, tags);
        return MpMetrics.CallbackRequestDuration.RecordDuration(tags);
    }

    /// <summary>
    /// 记录回调入站结果。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="outcome">操作结果（success / failure / timeout）。</param>
    /// <param name="errorType">错误类型（可选）。</param>
    public static void RecordCallbackOutcome(string appKey, string outcome, string? errorType = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OfficialAccount },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        if (errorType != null)
            tags.Add(new(WechatActivitySource.Tags.ErrorType, errorType));

        MpMetrics.CallbackRequestCount.Add(1, tags);
    }

    /// <summary>
    /// 记录事件分发指标，返回 IDisposable 用于标记耗时。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="eventType">事件类型（MsgType / Event）。</param>
    /// <returns>可释放的耗时记录器。</returns>
    public static IDisposable RecordEventHandling(string appKey, string eventType)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OfficialAccount },
            { WechatActivitySource.Tags.AppKey, appKey },
            { MpMetrics.Tags.EventType, eventType },
        };

        MpMetrics.EventHandlingCount.Add(1, tags);
        return MpMetrics.EventHandlingDuration.RecordDuration(tags);
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
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.OfficialAccount },
            { WechatActivitySource.Tags.AppKey, appKey },
            { MpMetrics.Tags.EventType, eventType },
            { WechatActivitySource.Tags.Outcome, success ? "success" : "failure" },
        };

        if (errorType != null)
            tags.Add(new(WechatActivitySource.Tags.ErrorType, errorType));

        MpMetrics.EventHandlingCount.Add(1, tags);
    }
}
