// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Moq;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 回调 <c>AppId</c> ⇄ 多应用基座配置的惰性交叉校验用例（方案 §4 要点 1 / R6）。
/// </summary>
public class MpAppIdCrossCheckTests
{
    /// <summary>捕获日志的测试用日志器。</summary>
    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }

    private static MpAppIdCrossChecker CreateChecker(string? managerAppId, CapturingLogger logger)
    {
        var manager = new Mock<IMpAppManager>();
        if (managerAppId == null)
        {
            manager
                .Setup(m => m.TryGetConfig(It.IsAny<string>(), out It.Ref<MpAppConfig?>.IsAny))
                .Returns(false);
        }
        else
        {
            MpAppConfig? config = new MpAppConfig { AppId = managerAppId };
            manager
                .Setup(m => m.TryGetConfig(It.IsAny<string>(), out config))
                .Returns(true);
        }

        var services = new Mock<IServiceProvider>();
        services.Setup(s => s.GetService(typeof(IMpAppManager))).Returns(manager.Object);

        return new MpAppIdCrossChecker(services.Object, logger);
    }

    /// <summary>不一致 ⇒ 记 Error 且**不抛**（回调链路按回调配置继续，回调-only 宿主自足性不被破坏）。</summary>
    [Fact]
    public void Check_WithMismatchedAppId_ShouldLogErrorAndNotThrow()
    {
        var logger = new CapturingLogger();
        var checker = CreateChecker("wxConfiguredAppId", logger);

        checker.Check("mp1", "wxCallbackAppId");

        logger.Entries.Should().ContainSingle(e => e.Level == LogLevel.Error);
        logger.Entries[0].Message.Should().Contain("wxCallbackAppId").And.Contain("wxConfiguredAppId");
    }

    /// <summary>一致 / 未注册多应用基座 / 未命中 appKey ⇒ 零 Error；且结论按 appKey 缓存（只判一次）。</summary>
    [Fact]
    public void Check_ShouldBeIdempotentAndSilentWhenConsistent()
    {
        var consistent = new CapturingLogger();
        var checker = CreateChecker("wxSame", consistent);
        checker.Check("mp1", "wxSame");
        checker.Check("mp1", "wxSame");
        consistent.Entries.Should().NotContain(e => e.Level == LogLevel.Error);

        var mismatch = new CapturingLogger();
        var mismatchChecker = CreateChecker("other", mismatch);
        mismatchChecker.Check("mp1", "wxCallbackAppId");
        mismatchChecker.Check("mp1", "wxCallbackAppId");
        mismatch.Entries.Count(e => e.Level == LogLevel.Error).Should().Be(1, "结论按 appKey 缓存一次，避免日志风暴");

        var notConfigured = new CapturingLogger();
        CreateChecker(null, notConfigured).Check("mp1", "wxAnything");
        notConfigured.Entries.Should().NotContain(e => e.Level == LogLevel.Error);
    }

    /// <summary>未注册多应用基座（回调-only 宿主）⇒ 空转，零异常零 Error。</summary>
    [Fact]
    public void Check_WithoutAppManagerRegistered_ShouldBeNoOp()
    {
        var logger = new CapturingLogger();
        var services = new Mock<IServiceProvider>();
        services.Setup(s => s.GetService(typeof(IMpAppManager))).Returns(null!);

        var checker = new MpAppIdCrossChecker(services.Object, logger);
        checker.Check("mp1", "wxCallbackAppId");

        logger.Entries.Should().NotContain(e => e.Level == LogLevel.Error);
    }
}
