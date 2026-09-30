// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

namespace Mud.Wechat.Work.Abstractions.Tests.MultiApp;

/// <summary>
/// 应用上下文切换器测试（R9 / R12）：
/// <see cref="IWechatAppContextSwitcher"/> 可达性、<c>UseApp</c> 上下文传播、
/// <c>SetCorp</c> 归属应用写入、授权拒绝路径。
/// </summary>
public class WechatAppContextSwitcherTests
{
    private static Mock<IWechatAppContext> CreateContext(string appKey)
    {
        var context = new Mock<IWechatAppContext>();
        context.SetupGet(c => c.AppKey).Returns(appKey);
        return context;
    }

    [Fact]
    public void UseApp_ShouldSwitchCurrentContext_AndSetCorpShouldCarryAppKey()
    {
        var contextA = CreateContext("app-a");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(contextA.Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.UseApp("app-a").Should().BeSameAs(contextA.Object, "UseApp 应返回并切换到目标应用上下文");
            switcher.Current.Should().BeSameAs(contextA.Object);

            switcher.SetCorp("corp-1", "pc-1");
            WechatCorpContext.AppKey.Should().Be("app-a", "SetCorp 应携带当前环境应用作为归属维度（R9）");
            WechatCorpContext.AuthCorpId.Should().Be("corp-1");
            WechatCorpContext.PermanentCode.Should().Be("pc-1");
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    [Fact]
    public void UseDefaultApp_ShouldResolveDefaultContext()
    {
        var defaultContext = CreateContext("app-default");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetDefaultApp()).Returns(defaultContext.Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.UseDefaultApp().Should().BeSameAs(defaultContext.Object);
            switcher.Current.Should().BeSameAs(defaultContext.Object);
        }
        finally
        {
            switcher.SwitchTo(null);
        }
    }

    [Fact]
    public void UseApp_ShouldThrow_WhenAuthorizerDenies()
    {
        var manager = new Mock<IWechatAppManager>();
        var authorizer = new Mock<IAppAccessAuthorizer>();
        authorizer.Setup(a => a.CanSwitchTo("app-a")).Returns(false);

        var switcher = new WechatAppContextSwitcher(manager.Object, authorizer.Object);

        var act = () => switcher.UseApp("app-a");
        act.Should().Throw<InvalidOperationException>().WithMessage("*无权切换*");
        manager.Verify(m => m.GetApp(It.IsAny<string>()), Times.Never, "授权拒绝时不应解析目标应用");
    }

    [Fact]
    public void SetCorp_ShouldNotDeclareOwnership_WhenNoAppSwitched()
    {
        var manager = new Mock<IWechatAppManager>();
        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SetCorp("corp-1");
            WechatCorpContext.AppKey.Should().BeNull("未切换应用时不声明归属，CorpTokenManager 归属校验随之跳过");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    // ---------------------------------------------------------------- P2-1 / P2-3

    [Fact]
    public void ClearCorp_ShouldClearAmbientState()
    {
        var manager = new Mock<IWechatAppManager>();
        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SetCorp("corp-1", "pc-1");
            WechatCorpContext.AuthCorpId.Should().Be("corp-1");

            switcher.ClearCorp();

            WechatCorpContext.AuthCorpId.Should().BeNull("ClearCorp 为 SetCorp 的对称重置入口（P2-1）");
            WechatCorpContext.PermanentCode.Should().BeNull();
            WechatCorpContext.AppKey.Should().BeNull();
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    [Fact]
    public void BeginCorpScope_ShouldRestoreThreeValues_OnDispose()
    {
        try
        {
            WechatCorpContext.SetCorp("outer-app", "outer-corp", "outer-pc");

            using (WechatCorpContext.BeginCorpScope("inner-app", "inner-corp", "inner-pc"))
            {
                WechatCorpContext.AppKey.Should().Be("inner-app");
                WechatCorpContext.AuthCorpId.Should().Be("inner-corp");
                WechatCorpContext.PermanentCode.Should().Be("inner-pc");
            }

            WechatCorpContext.AppKey.Should().Be("outer-app", "作用域释放后必须还原进入前的快照（而非清空）");
            WechatCorpContext.AuthCorpId.Should().Be("outer-corp");
            WechatCorpContext.PermanentCode.Should().Be("outer-pc");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    [Fact]
    public void GetTokenAsync_ShouldHonorTokenType()
    {
        var suiteManager = new Mock<ITokenManager>();
        suiteManager.Setup(m => m.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("suite-token");

        var context = CreateContext("app-a");
        context.Setup(c => c.GetTokenManager(WechatTokenTypes.SuiteAccessToken)).Returns(suiteManager.Object);

        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(context.Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.UseApp("app-a");

            var token = switcher.GetTokenAsync(WechatTokenTypes.SuiteAccessToken).GetAwaiter().GetResult();

            token.Should().Be("suite-token", "P2-3：重载必须按 tokenType 路由（原无参成员硬编码 AccessToken）");
            suiteManager.Verify(m => m.GetTokenAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            switcher.SwitchTo(null);
        }
    }

    [Fact]
    public async Task GetTokenAsync_ShouldThrow_WhenTokenTypeEmpty()
    {
        var switcher = new WechatAppContextSwitcher(new Mock<IWechatAppManager>().Object);

        var act = async () => await switcher.GetTokenAsync(string.Empty);
        await act.Should().ThrowAsync<ArgumentNullException>();
        await Task.CompletedTask;
    }
}