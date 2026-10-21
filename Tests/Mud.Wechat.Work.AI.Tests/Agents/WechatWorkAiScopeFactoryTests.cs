using FluentAssertions;
using Moq;
using Mud.AI.Agent.Events;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.AI.Agents;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Agents;

/// <summary>
/// 工具作用域工厂（<see cref="ITurnScopeFactory"/> 的企微实现）：
/// 自建应用走 <c>UseAppScope</c>、携带企业维度走 <c>UseCorpScope</c>、缺失应用维度走默认应用，
/// 且返回的作用域释放时不得残留状态（嵌套回合越权读取的防线）。
/// </summary>
public class WechatWorkAiScopeFactoryTests
{
    private readonly Mock<IWechatAppContextSwitcher> _switcher = new(MockBehavior.Strict);
    private readonly WechatAiCorpContextAccessor _corpContext = new();
    private readonly WechatWorkAiScopeFactory _factory;

    public WechatWorkAiScopeFactoryTests()
    {
        _factory = new WechatWorkAiScopeFactory(_switcher.Object, _corpContext);
    }

    [Fact]
    public void BeginToolScope_SelfBuiltApp_ShouldUseAppScope()
    {
        var scope = new FakeScope();
        _switcher.Setup(s => s.UseAppScope("corp-app")).Returns(scope);
        _corpContext.Current = new WechatToolPrincipal("corp-app");

        var result = _factory.BeginToolScope(Context(tenantKey: "corp-app"));

        result.Should().BeSameAs(scope);
        _switcher.Verify(s => s.UseAppScope("corp-app"), Times.Once);
        _switcher.VerifyNoOtherCalls();
    }

    [Fact]
    public void BeginToolScope_WithCorpIdentity_ShouldUseCorpScope()
    {
        var scope = new FakeScope();
        _switcher.Setup(s => s.UseCorpScope("suite-app", "corp-1", It.IsAny<string?>())).Returns(scope);
        _corpContext.Current = new WechatToolPrincipal("suite-app", authCorpId: "corp-1");

        var result = _factory.BeginToolScope(Context(tenantKey: "suite-app"));

        result.Should().BeSameAs(scope);
        _switcher.Verify(s => s.UseCorpScope("suite-app", "corp-1", It.IsAny<string?>()), Times.Once);
        _switcher.VerifyNoOtherCalls();
    }

    [Fact]
    public void BeginToolScope_WithoutTenantKey_ShouldFallBackToPrincipalAppKey()
    {
        _switcher.Setup(s => s.UseAppScope("principal-app")).Returns(new FakeScope());
        _corpContext.Current = new WechatToolPrincipal("principal-app");

        _factory.BeginToolScope(Context(tenantKey: "  "));

        _switcher.Verify(s => s.UseAppScope("principal-app"), Times.Once);
    }

    [Fact]
    public void BeginToolScope_WithoutAnyAppIdentity_ShouldUseDefaultAppScope()
    {
        _switcher.Setup(s => s.UseDefaultAppScope()).Returns(new FakeScope());

        _factory.BeginToolScope(Context(tenantKey: string.Empty));

        _switcher.Verify(s => s.UseDefaultAppScope(), Times.Once);
    }

    [Fact]
    public void BeginToolScope_Dispose_ShouldBeForwardedToSwitcherScope()
    {
        var scope = new FakeScope();
        _switcher.Setup(s => s.UseAppScope("corp-app")).Returns(scope);

        using (_factory.BeginToolScope(Context(tenantKey: "corp-app")))
        {
            scope.Disposed.Should().BeFalse();
        }

        scope.Disposed.Should().BeTrue("作用域释放必须归还进入前的应用/企业上下文（嵌套回合不得读到外层租户）");
    }

    [Fact]
    public void Constructor_ShouldRejectNullDependencies()
    {
        var act1 = () => new WechatWorkAiScopeFactory(null!, _corpContext);
        var act2 = () => new WechatWorkAiScopeFactory(_switcher.Object, null!);

        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void BeginToolScope_WithFactsOnly_ShouldUseCorpScopeWithoutAmbientIdentity()
    {
        // Facts 是回合管线的显式输入：即便事件层没建立环境量，作用域也必须按事实切到「应用 + 企业」。
        var scope = new FakeScope();
        _switcher.Setup(s => s.UseCorpScope("suite-app", "corp-9", It.IsAny<string?>())).Returns(scope);

        var result = _factory.BeginToolScope(ContextWithFacts(
            tenantKey: "suite-app",
            WechatTurnFacts.AppKey,
            "suite-app",
            WechatTurnFacts.AuthCorpId,
            "corp-9"));

        result.Should().BeSameAs(scope);
        _switcher.Verify(s => s.UseCorpScope("suite-app", "corp-9", It.IsAny<string?>()), Times.Once);
        _switcher.VerifyNoOtherCalls();
    }

    [Fact]
    public void BeginToolScope_FactsAppKey_ShouldWinOverAmbientIdentity()
    {
        var scope = new FakeScope();
        _switcher.Setup(s => s.UseAppScope("facts-app")).Returns(scope);
        _corpContext.Current = new WechatToolPrincipal("stale-app");

        _factory.BeginToolScope(ContextWithFacts(
            tenantKey: "tenant-app",
            WechatTurnFacts.AppKey,
            "facts-app"));

        _switcher.Verify(s => s.UseAppScope("facts-app"), Times.Once);
        _switcher.VerifyNoOtherCalls();
    }

    [Fact]
    public void BeginToolScope_WithFactsMissingCorp_ShouldFallBackToAmbientCorp()
    {
        // 逐字段软缺席：Facts 只给应用维度时，企业维度仍可由环境量补齐（不因「Facts 非空」就整份回退）。
        var scope = new FakeScope();
        _switcher.Setup(s => s.UseCorpScope("suite-app", "corp-1", It.IsAny<string?>())).Returns(scope);
        _corpContext.Current = new WechatToolPrincipal("suite-app", authCorpId: "corp-1");

        _factory.BeginToolScope(ContextWithFacts(
            tenantKey: "suite-app",
            WechatTurnFacts.AppKey,
            "suite-app"));

        _switcher.Verify(s => s.UseCorpScope("suite-app", "corp-1", It.IsAny<string?>()), Times.Once);
        _switcher.VerifyNoOtherCalls();
    }

    [Fact]
    public void BeginToolScope_WithBlankFactValues_ShouldTreatThemAsAbsent()
    {
        _switcher.Setup(s => s.UseAppScope("tenant-app")).Returns(new FakeScope());

        _factory.BeginToolScope(ContextWithFacts(
            tenantKey: "tenant-app",
            WechatTurnFacts.AppKey,
            "   ",
            WechatTurnFacts.AuthCorpId,
            ""));

        _switcher.Verify(s => s.UseAppScope("tenant-app"), Times.Once);
        _switcher.VerifyNoOtherCalls();
    }

    private static TurnScopeContext Context(string tenantKey)
        => new(tenantKey, "wechat:corp-app:g:chat-1", "chat-1", "zhangsan", null);

    private static TurnScopeContext ContextWithFacts(string tenantKey, params string[] pairs)
    {
        var facts = new Dictionary<string, string?>(StringComparer.Ordinal);
        for (var i = 0; i + 1 < pairs.Length; i += 2)
        {
            facts[pairs[i]] = pairs[i + 1];
        }

        return new TurnScopeContext(tenantKey, "wechat:corp-app:g:chat-1", "chat-1", "zhangsan", null, facts);
    }

    private sealed class FakeScope : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }
}
