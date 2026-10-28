// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using Mud.Wechat.Abstractions.Metrics;
using Mud.Wechat.Abstractions.Observability;

namespace Mud.Wechat.Pay.Abstractions.Metrics;

/// <summary>
/// 支付线指标埋点入口（与公众号线 <c>MpMetricsHelper</c> 同形态）：耗时直方图与计数分开记录，
/// 计数由 Outcome 方法统一完成（避免重复计数）。维度取回调配置键（路由段；直连与服务商键形态不同由标签值区分）。
/// </summary>
public static class PayMetricsHelper
{
    /// <summary>记录回调入站耗时（计时起点）。</summary>
    /// <param name="appKey">回调配置键（路由路径段；直连为商户键，服务商含 <c>:</c> 复合键）。</param>
    /// <returns>可释放的耗时记录器（释放即落点）。</returns>
    public static IDisposable RecordCallbackRequest(string appKey)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Pay },
            { WechatActivitySource.Tags.AppKey, appKey },
        };

        return PayMetrics.CallbackRequestDuration.RecordDuration(tags);
    }

    /// <summary>记录回调入站结果。</summary>
    /// <param name="appKey">回调配置键（路由路径段）。</param>
    /// <param name="outcome">操作结果（success / failure / timeout）。</param>
    /// <param name="errorType">错误类型（可选，如 signature / replay / timestamp）。</param>
    public static void RecordCallbackOutcome(string appKey, string outcome, string? errorType = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Pay },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        if (errorType != null)
        {
            tags.Add(new(WechatActivitySource.Tags.ErrorType, errorType));
        }

        PayMetrics.CallbackRequestCount.Add(1, tags);
    }

    /// <summary>记录平台证书刷新结果。</summary>
    /// <param name="appKey">回调/商户配置键。</param>
    /// <param name="outcome">刷新结果（success / failure）。</param>
    /// <param name="certSerial">新证书序列号（可选；轮换期会先后出现两个序列号）。</param>
    public static void RecordCertificateRefresh(string appKey, string outcome, string? certSerial = null)
    {
        var tags = new TagList
        {
            { WechatActivitySource.Tags.Product, WechatActivitySource.Products.Pay },
            { WechatActivitySource.Tags.AppKey, appKey },
            { WechatActivitySource.Tags.Outcome, outcome },
        };

        if (certSerial != null)
        {
            tags.Add(new(PayMetrics.Tags.CertSerial, certSerial));
        }

        PayMetrics.CertificateRefreshCount.Add(1, tags);
    }
}
