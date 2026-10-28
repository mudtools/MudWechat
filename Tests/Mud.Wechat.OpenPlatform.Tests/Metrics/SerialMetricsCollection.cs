// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Xunit;

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 禁并行集合：凡<b>真实</b>触达 <c>OpenPlatformMetrics</c> 静态仪表的测试类都必须加入 ——
/// 接收器测试（<c>ComponentVerifyTicketReceiverTests</c>）的每次处置都会向静态仪表发射样本，
/// 而指标测试的 <c>MeterListener</c> 会收到<b>所有</b>启用仪表的广播样本；
/// 跨类并行即「外来样本串入 + 无锁 List 并发枚举」双竞态（满载偶发、隔离不现，批次 4 定位）。
/// 两个类同入本集合后串行执行，互不污染。
/// </summary>
[CollectionDefinition("Serial-OpenPlatform-Metrics", DisableParallelization = true)]
public sealed class SerialOpenPlatformMetricsCollection
{
}
