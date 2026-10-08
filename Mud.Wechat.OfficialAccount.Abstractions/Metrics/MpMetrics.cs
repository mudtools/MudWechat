// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！
//  任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.Metrics;
using Mud.Wechat.Abstractions.Observability;

namespace Mud.Wechat.OfficialAccount.Abstractions.Metrics;

/// <summary>
/// 公众号产品线性能指标源。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="MeterName"/> = <c>Mud.Wechat.OfficialAccount</c>，通过 OTel 适配包的
/// 通配注册 <c>AddMeter("Mud.Wechat*")</c> 自动采集。
/// </para>
/// <para>
/// HTTP 请求指标由 <c>Mud.HttpUtils.MudHttpMeter</c> 自动采集。
/// Token 刷新指标由 <c>Mud.HttpUtils.TokenManagerBase</c> 自动采集。
/// 本类只定义公众号特有指标（回调接收、事件分发）。
/// </para>
/// </remarks>
public static class MpMetrics
{
    /// <summary>
    /// Meter 名称，遵循 OTel 命名约定（产品线段）。
    /// </summary>
    public const string MeterName = "Mud.Wechat.OfficialAccount";

    /// <summary>
    /// Meter 版本（与 <see cref="WechatActivitySource.Version"/> 一致）。
    /// </summary>
    public const string Version = WechatActivitySource.Version;

    /// <summary>
    /// 静态 Meter 实例。
    /// </summary>
    public static readonly Meter Instance = new(MeterName, Version);

    // ── 回调接收指标 ──

    /// <summary>
    /// 回调入站计数（维度：app_key, outcome）。
    /// </summary>
    public static readonly Counter<long> CallbackRequestCount = Instance.CreateCounter<long>(
        "mp.callback.request",
        unit: "{request}",
        description: "公众号回调入站计数（含 echo / receive）");

    /// <summary>
    /// 回调全程耗时直方图（毫秒，维度：app_key）。
    /// </summary>
    public static readonly Histogram<double> CallbackRequestDuration = Instance.CreateHistogram<double>(
        "mp.callback.request.duration",
        unit: "ms",
        description: "公众号回调全程耗时分布");

    // ── 事件分发指标 ──

    /// <summary>
    /// 事件分发次数（维度：app_key, event_type, outcome）。
    /// </summary>
    public static readonly Counter<long> EventHandlingCount = Instance.CreateCounter<long>(
        "mp.event.handling",
        unit: "{event}",
        description: "公众号事件分发次数");

    /// <summary>
    /// 事件处理耗时直方图（毫秒，维度：app_key, event_type）。
    /// </summary>
    public static readonly Histogram<double> EventHandlingDuration = Instance.CreateHistogram<double>(
        "mp.event.handling.duration",
        unit: "ms",
        description: "公众号事件处理耗时分布");

    // ── 标签常量 ──

    /// <summary>
    /// 公众号产品线特有标签常量集合。
    /// </summary>
    public static class Tags
    {
        /// <summary>事件类型（MsgType / Event）</summary>
        public const string EventType = "wechat.oa.event_type";

        /// <summary>事件处理器类型名</summary>
        public const string HandlerType = "wechat.oa.handler_type";
    }
}
