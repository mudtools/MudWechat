// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.Metrics;
using Mud.Wechat.Abstractions.Observability;

namespace Mud.Wechat.Pay.Abstractions.Metrics;

/// <summary>
/// 微信支付产品线性能指标源（总案 D5，追踪号 MASTER-REMEDIATION-2026-10-24）。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="MeterName"/> = <c>Mud.Wechat.Pay</c>，经 OTel 适配包的通配注册
/// <c>AddMeter("Mud.Wechat*")</c> 自动采集。
/// </para>
/// <para>
/// HTTP 请求指标由 <c>Mud.HttpUtils.MudHttpMeter</c> 自动采集；Token 刷新指标由
/// <c>Mud.HttpUtils.TokenManagerBase</c> 自动采集（支付线零 <c>[Token]</c>，实际为 APIv3 凭据面）。
/// 本类只定义支付线特有指标（回调接收、平台证书轮换）。当前为<b>指标面先行</b>（对齐公众号线
/// <c>MpMetrics</c> 形态），运行期埋点接线按《可观测性解决方案》§10 分阶段推进。
/// </para>
/// </remarks>
public static class PayMetrics
{
    /// <summary>Meter 名称（产品线段）。</summary>
    public const string MeterName = "Mud.Wechat.Pay";

    /// <summary>Meter 版本（与 <see cref="WechatActivitySource.Version"/> 一致）。</summary>
    public const string Version = WechatActivitySource.Version;

    /// <summary>静态 Meter 实例。</summary>
    public static readonly Meter Instance = new(MeterName, Version);

    // ── 回调接收指标 ──

    /// <summary>支付回调入站计数（维度：mch_id, outcome）。</summary>
    public static readonly Counter<long> CallbackRequestCount = Instance.CreateCounter<long>(
        "callback.request",
        unit: "{request}",
        description: "微信支付回调入站计数（含验签前后各阶段）");

    /// <summary>支付回调全程耗时直方图（毫秒，维度：mch_id）。</summary>
    public static readonly Histogram<double> CallbackRequestDuration = Instance.CreateHistogram<double>(
        "callback.request.duration",
        unit: "ms",
        description: "微信支付回调全程耗时分布");

    // ── 平台证书轮换指标 ──

    /// <summary>平台证书刷新计数（维度：mch_id, outcome）。</summary>
    public static readonly Counter<long> CertificateRefreshCount = Instance.CreateCounter<long>(
        "pay.cert.refresh",
        unit: "{refresh}",
        description: "微信支付平台证书拉取/轮换计数");

    // ── 标签常量 ──

    /// <summary>支付线特有标签常量集合。</summary>
    public static class Tags
    {
        /// <summary>平台证书序列号（用于区分轮换前后）</summary>
        public const string CertSerial = "wechat.pay.cert_serial";
    }
}
