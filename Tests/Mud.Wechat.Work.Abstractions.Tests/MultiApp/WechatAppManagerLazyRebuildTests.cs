// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Abstractions.Tests.MultiApp;

/// <summary>
/// P1-1：<c>Lazy&lt;IWechatAppContext&gt;</c> 的**瞬时失败重建**语义
/// （<see cref="Lazy{T}"/> 会缓存异常 ⇒ 一次网络抖动后应用永久不可用）。
/// </summary>
public class WechatAppManagerLazyRebuildTests
{
    /// <summary>按「第 N 次装配」注入故障的测试用管理器。</summary>
    private sealed class FlakyAppManager : WechatAppManager
    {
        private readonly Func<int, Exception?> _failureFactory;
        private int _created;

        public FlakyAppManager(Func<int, Exception?> failureFactory, params WechatAppConfig[] configs)
            : base(new ServiceCollection().AddLogging().BuildServiceProvider(), configs, NullLogger<WechatAppManager>.Instance)
        {
            _failureFactory = failureFactory;
        }

        /// <summary>装配次数（每次 <c>CreateAppContext</c> 调用 +1）。</summary>
        public int CreateCount => Volatile.Read(ref _created);

        protected override IWechatAppContext CreateAppContext(WechatAppConfig config)
        {
            var attempt = Interlocked.Increment(ref _created);
            var failure = _failureFactory(attempt);
            if (failure != null)
            {
                throw failure;
            }

            return new Mock<IWechatAppContext>().Object;
        }
    }

    private static WechatAppConfig Config(string appKey) => new()
    {
        AppKey = appKey,
        AppType = WechatAppType.Internal,
        CorpId = "ww-corp",
        AgentSecret = "agent-secret",
    };

    [Fact]
    public void GetApp_ShouldRebuildLazy_WhenTransientInitFailure()
    {
        using var manager = new FlakyAppManager(
            attempt => attempt == 1 ? new HttpRequestException("transient-io") : null,
            Config("a"));

        var context = manager.GetApp("a");

        context.Should().NotBeNull("瞬时装配故障后应重置 Lazy 并重建成功");
        manager.CreateCount.Should().Be(2, "首次失败 + 重建一次成功");
    }

    [Fact]
    public void GetApp_ShouldNotRebuild_WhenDeterministicFailure()
    {
        using var manager = new FlakyAppManager(_ => new ArgumentException("config-invalid"), Config("a"));

        var act = () => manager.GetApp("a");

        act.Should().Throw<ArgumentException>("确定性失败必须直抛原异常，不得伪装成可重试");
        manager.CreateCount.Should().Be(1, "确定性失败不重建（避免掩盖配置错误）");
    }

    [Fact]
    public void GetApp_ShouldNotRebuild_WhenDiResolutionFails()
    {
        // M4（F4）：IOE 不再属瞬时白名单——装配路径的 IOE 全是 DI 确定性失败（GetRequiredService /
        // 命名客户端缺失），留白名单会把配置错误伪装成可重试并驱动 5s 节流的反复重建（叠加 F1 即慢性泄漏）。
        using var manager = new FlakyAppManager(_ => new InvalidOperationException("di-resolution-failed"), Config("a"));

        var act = () => manager.GetApp("a");

        act.Should().Throw<InvalidOperationException>("DI 确定性失败直抛原异常（异常类型与消息不变）");
        manager.CreateCount.Should().Be(1, "确定性失败不重建、不重置 Lazy");
    }

    [Fact]
    public void GetApp_ShouldThrottleRebuild_WhenTransientFailurePersists()
    {
        using var manager = new FlakyAppManager(_ => new HttpRequestException("still-down"), Config("a"));

        for (var i = 0; i < 3; i++)
        {
            var act = () => manager.GetApp("a");
            act.Should().Throw<HttpRequestException>();
        }

        manager.CreateCount.Should().Be(2,
            "节流窗口内最多重建一次（避免瞬时故障持续时的热循环重建）");
    }

    [Fact]
    public void TryGetApp_ShouldReturnTrue_AfterRebuild_WhenTransientInitFailure()
    {
        using var manager = new FlakyAppManager(
            attempt => attempt == 1 ? new TimeoutException("transient-timeout") : null,
            Config("a"));

        manager.TryGetApp("a", out var context).Should().BeTrue();
        context.Should().NotBeNull();
        manager.CreateCount.Should().Be(2);
    }

    [Fact]
    public void TryGetApp_ShouldReturnFalse_WhenDeterministicFailure()
    {
        using var manager = new FlakyAppManager(_ => new ArgumentException("config-invalid"), Config("a"));

        manager.TryGetApp("a", out var context).Should().BeFalse("Try 语义：确定性失败返回 false 而非抛出");
        context.Should().BeNull();
        manager.CreateCount.Should().Be(1);
    }
}
