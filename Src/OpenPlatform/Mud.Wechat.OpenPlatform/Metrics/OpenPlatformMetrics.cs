// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.Metrics;
using Mud.Wechat.Abstractions.Observability;

namespace Mud.Wechat.OpenPlatform.Abstractions.Metrics;

/// <summary>
/// 开放平台产品线性能指标源（总案 D5，追踪号 MASTER-REMEDIATION-2026-10-24）。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="MeterName"/> = <c>Mud.Wechat.OpenPlatform</c>，经 OTel 适配包的通配注册
/// <c>AddMeter("Mud.Wechat*")</c> 自动采集。
/// </para>
/// <para>
/// 开放平台的推送接收（component_verify_ticket / 授权事件）由主包承载（本线无独立 Callback 包），
/// 观测面为「指令事件接收 + 分发」。HTTP/Token 指标由 <c>Mud.HttpUtils</c> 横切层自动采集。
/// 当前为<b>指标面先行</b>（对齐公众号线 <c>MpMetrics</c> 形态），运行期埋点接线按
/// 《可观测性解决方案》§10 分阶段推进。
/// </para>
/// </remarks>
public static class OpenPlatformMetrics
{
    /// <summary>Meter 名称（产品线段）。</summary>
    public const string MeterName = "Mud.Wechat.OpenPlatform";

    /// <summary>Meter 版本（与 <see cref="WechatActivitySource.Version"/> 一致）。</summary>
    public const string Version = WechatActivitySource.Version;

    /// <summary>静态 Meter 实例。</summary>
    public static readonly Meter Instance = new(MeterName, Version);

    // ── 推送接收指标 ──

    /// <summary>推送入站计数（维度：app_key, outcome）。</summary>
    public static readonly Counter<long> CallbackRequestCount = Instance.CreateCounter<long>(
        "callback.request",
        unit: "{request}",
        description: "开放平台推送入站计数（ticket / 授权事件）");

    /// <summary>推送全程耗时直方图（毫秒，维度：app_key）。</summary>
    public static readonly Histogram<double> CallbackRequestDuration = Instance.CreateHistogram<double>(
        "callback.request.duration",
        unit: "ms",
        description: "开放平台推送全程耗时分布");

    // ── 事件分发指标 ──

    /// <summary>事件分发次数（维度：app_key, event_type, outcome）。</summary>
    public static readonly Counter<long> EventHandlingCount = Instance.CreateCounter<long>(
        "event.handling",
        unit: "{event}",
        description: "开放平台事件分发次数（InfoType）");

    /// <summary>事件处理耗时直方图（毫秒，维度：app_key, event_type）。</summary>
    public static readonly Histogram<double> EventHandlingDuration = Instance.CreateHistogram<double>(
        "event.handling.duration",
        unit: "ms",
        description: "开放平台事件处理耗时分布");

    // ── 标签常量 ──

    /// <summary>开放平台特有标签常量集合。</summary>
    public static class Tags
    {
        /// <summary>事件类型（InfoType）</summary>
        public const string EventType = "wechat.op.event_type";
    }
}
