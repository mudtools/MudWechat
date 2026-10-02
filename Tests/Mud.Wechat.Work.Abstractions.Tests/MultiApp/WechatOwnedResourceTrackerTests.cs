// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

namespace Mud.Wechat.Work.Abstractions.Tests.MultiApp;

/// <summary>
/// 装配期所有权收集表（M1/F1，方案 §九 T1）：登记资源在装配中途失败时的逆序确定性回收、
/// 异常隔离与成功路径零回收。
/// </summary>
public class WechatOwnedResourceTrackerTests
{
    /// <summary>记录 Dispose 调用次序的测试资源。</summary>
    private sealed class TrackedResource : IDisposable
    {
        private readonly List<string> _log;
        private readonly string _name;
        private readonly bool _throwOnDispose;

        public TrackedResource(List<string> log, string name, bool throwOnDispose = false)
        {
            _log = log;
            _name = name;
            _throwOnDispose = throwOnDispose;
        }

        public void Dispose()
        {
            _log.Add(_name);
            if (_throwOnDispose)
            {
                throw new InvalidOperationException($"dispose boom: {_name}");
            }
        }
    }

    [Fact]
    public void DisposeAll_ShouldDisposeInReverseOrder()
    {
        var log = new List<string>();
        var tracker = new WechatOwnedResourceTracker(NullLogger.Instance);
        tracker.Track(new TrackedResource(log, "first"));
        tracker.Track(new TrackedResource(log, "second"));
        tracker.Track(new TrackedResource(log, "third"));

        tracker.DisposeAll();

        log.Should().Equal(new[] { "third", "second", "first" },
            "逆序释放：后建管理器引用先建客户端，必须按依赖序回收（M1）");
    }

    [Fact]
    public void DisposeAll_ShouldIsolateDisposeExceptions_AndContinue()
    {
        var log = new List<string>();
        var tracker = new WechatOwnedResourceTracker(NullLogger.Instance);
        tracker.Track(new TrackedResource(log, "first"));
        tracker.Track(new TrackedResource(log, "boom", throwOnDispose: true));
        tracker.Track(new TrackedResource(log, "second"));

        var act = () => tracker.DisposeAll();

        act.Should().NotThrow("单个资源的释放异常必须逐个隔离，不得中断其余资源的回收");
        log.Should().Equal(new[] { "second", "boom", "first" }, "释放异常不得阻断后续逆序回收");
    }

    [Fact]
    public void Clear_ShouldPreventDisposal_OnSuccessPath()
    {
        var log = new List<string>();
        var tracker = new WechatOwnedResourceTracker(NullLogger.Instance);
        tracker.Track(new TrackedResource(log, "managed"));
        tracker.Track(new TrackedResource(log, "unmanaged"));

        // 成功路径：所有权移交 WechatAppContext，登记表清空 ⇒ catch 兜底不再回收（T1：成功路径零 Dispose）。
        tracker.Clear();
        tracker.DisposeAll();

        log.Should().BeEmpty("所有权移交成功后不得重复回收（否则与 WechatAppContext.Dispose 双重释放）");
    }

    [Fact]
    public void Track_ShouldIgnoreNonDisposableResources()
    {
        var tracker = new WechatOwnedResourceTracker(NullLogger.Instance);
        var nonDisposable = new object();

        tracker.Track(nonDisposable).Should().BeSameAs(nonDisposable, "非 IDisposable 资源直接透传");
        tracker.DisposeAll();

        // 无登记资源时DisposeAll 应为安全的空操作（无异常即通过）。
    }

    [Fact]
    public void Constructor_ShouldRejectNullLogger()
    {
        var act = () => new WechatOwnedResourceTracker(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
