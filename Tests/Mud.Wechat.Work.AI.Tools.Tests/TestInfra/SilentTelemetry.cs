// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Moq;
using Mud.AI.Abstractions.Observability;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests;

/// <summary>
/// 禁并行集合：凡<b>真实</b>触达 <c>WechatToolTelemetry.Instance</c> 静态仪表的测试类（治理指标 / 遥测断言）
/// 都必须加入本集合 —— MeterListener 的事件广播到所有启用者，跨类并行即互相污染样本。
/// 其余使用 <see cref="SilentTelemetry"/> 的类不触达真实仪表，保持默认并行。
/// </summary>
[CollectionDefinition("Serial-Metrics", DisableParallelization = true)]
public sealed class SerialMetricsCollection
{
}

/// <summary>
/// 测试专用的静默工具遥测（全 no-op）：供不经 DI 直接构造
/// <c>WechatToolDispatcher</c> 的测试类注入，避免把调用记进真实
/// <c>WechatToolTelemetry.Instance</c> 的静态仪表 —— 那会与
/// <c>WechatToolTelemetryTests</c> 的 <c>SingleSample()</c> 断言在
/// xUnit 跨类并行下竞态（满载偶发、隔离不现，2026-10-25 批次 3 定位）。
/// </summary>
internal static class SilentTelemetry
{
    /// <summary>共享的 no-op 遥测实例（Mock 松散实现，全部成员为空操作）。</summary>
    internal static readonly IAgentToolTelemetry Instance = Mock.Of<IAgentToolTelemetry>();
}
