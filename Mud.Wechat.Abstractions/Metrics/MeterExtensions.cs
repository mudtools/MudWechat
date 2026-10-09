// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Mud.Wechat.Abstractions.Metrics;

/// <summary>
/// <see cref="Histogram{T}"/> 扩展方法，提供可释放的持续时间记录器。
/// </summary>
/// <remarks>
/// 横切层承载、全产品线共用。使用 <see cref="RecordDuration(Histogram{double}, TagList)"/> 返回的
/// <see cref="IDisposable"/> 在 <c>using</c> 语句中自动记录耗时（毫秒）。
/// </remarks>
public static class MeterExtensions
{
    /// <summary>
    /// 创建一个可释放的持续时间记录器（无标签）。
    /// </summary>
    /// <param name="histogram">目标直方图。</param>
    /// <returns>可释放的耗时记录器，<see cref="IDisposable.Dispose"/> 时记录耗时。</returns>
    public static IDisposable RecordDuration(this Histogram<double> histogram)
    {
        return new DurationRecorder(histogram, default);
    }

    /// <summary>
    /// 创建一个带标签的可释放持续时间记录器。
    /// </summary>
    /// <param name="histogram">目标直方图。</param>
    /// <param name="tags">标签列表。</param>
    /// <returns>可释放的耗时记录器，<see cref="IDisposable.Dispose"/> 时记录耗时与标签。</returns>
    public static IDisposable RecordDuration(this Histogram<double> histogram, TagList tags)
    {
        return new DurationRecorder(histogram, tags);
    }

    private sealed class DurationRecorder : IDisposable
    {
        private readonly Histogram<double> _histogram;
        private readonly Stopwatch _stopwatch;
        private readonly TagList _tags;

        public DurationRecorder(Histogram<double> histogram, TagList tags)
        {
            _histogram = histogram;
            _tags = tags;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _histogram.Record(_stopwatch.ElapsedMilliseconds, _tags);
        }
    }
}
