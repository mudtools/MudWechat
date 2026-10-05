// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

namespace Mud.Wechat.Work.Abstractions.Tests.MultiApp;

/// <summary>
/// 应用上下文切换器测试（R9 / R12）：
/// <see cref="IWechatAppContextSwitcher"/> 可达性、<c>UseApp</c> 上下文传播、
/// <c>SetCorp</c> 归属应用写入、授权拒绝路径，
/// 以及 P2-1 强化的 <c>UseCorpScope</c> 复合作用域（两级还原 / 嵌套 / 失败安全 / 异常 / 幂等）。
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

    // ---------------------------------------------------------------- M7（F7）：SetCorp 参数前置校验

    [Fact]
    public void SetCorp_ShouldThrowArgumentNullException_WhenAuthCorpIdNull()
    {
        var act = () => WechatCorpContext.SetCorp(null, null!, null);
        act.Should().Throw<ArgumentNullException>(
            "M7：authCorpId 是企业令牌 scope 的唯一来源，null 不得延迟到首次令牌刷新才报错");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void SetCorp_ShouldThrowArgumentException_WhenAuthCorpIdBlank(string authCorpId)
    {
        var act = () => WechatCorpContext.SetCorp("app-a", authCorpId, null);
        act.Should().Throw<ArgumentException>("M7/MR9：空白 CorpId 无法成为合法 scope，写入点即拒");
    }

    [Fact]
    public void SetCorp_ShouldKeepUnDeclaredOwnershipSemantics_WhenAppKeyNull()
    {
        try
        {
            WechatCorpContext.SetCorp(null, "corp-1", "pc-1");

            WechatCorpContext.AppKey.Should().BeNull("appKey=null「未声明归属」语义保持（M7 校验不得波及）");
            WechatCorpContext.AuthCorpId.Should().Be("corp-1");
            WechatCorpContext.PermanentCode.Should().Be("pc-1");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    [Fact]
    public void BeginCorpScope_ShouldValidateAuthCorpId_SameAsSetCorp()
    {
        var act = () => WechatCorpContext.BeginCorpScope("app-a", "  ");
        act.Should().Throw<ArgumentException>("BeginCorpScope 经 SetCorp 收敛，自动获得同一校验（M7）");

        var actNull = () => WechatCorpContext.BeginCorpScope("app-a", null!);
        actNull.Should().Throw<ArgumentNullException>();
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

    // ---------------------------------------------------------------- Mud.HttpUtils 3.0.0 适配（BC-27）

    [Fact]
    public void UseAppScope_ShouldRestorePreviousContext_OnDispose()
    {
        var outer = CreateContext("app-outer");
        var target = CreateContext("app-target");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-target")).Returns(target.Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SwitchTo(outer.Object);

            using (var scope = switcher.UseAppScope("app-target"))
            {
                scope.Should().NotBeNull();
                switcher.Current.Should().BeSameAs(target.Object, "作用域内应切到目标应用");
            }

            switcher.Current.Should().BeSameAs(outer.Object,
                "作用域释放后必须还原进入前的上下文 —— 若实现「先 SwitchTo 再 BeginScope」，" +
                "快照值会等于目标应用本身，此处将残留为 app-target（跨应用串号风险）");
        }
        finally
        {
            switcher.SwitchTo(null);
        }
    }

    [Fact]
    public void UseDefaultAppScope_ShouldRestorePreviousContext_OnDispose()
    {
        var outer = CreateContext("app-outer");
        var defaultContext = CreateContext("app-default");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetDefaultApp()).Returns(defaultContext.Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SwitchTo(outer.Object);

            using (switcher.UseDefaultAppScope())
            {
                switcher.Current.Should().BeSameAs(defaultContext.Object);
            }

            switcher.Current.Should().BeSameAs(outer.Object, "默认应用作用域同样必须自动归还上下文");
        }
        finally
        {
            switcher.SwitchTo(null);
        }
    }

    [Theory]
    [InlineData("app-a")]              // 合法：字母/数字/.'_- 组成
    [InlineData("app.a_b-c")]          // 合法：含全部允许的分隔符
    [InlineData("!!!非法")]             // 非法：含不允许字符
    [InlineData(".leading-dot")]       // 非法：首字符必须是字母或数字
    public void UseAppScope_ShouldValidateAppKeyFormat(string appKey)
    {
        var context = CreateContext("used");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp(It.IsAny<string>())).Returns(context.Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);

        if (AppKey.IsValid(appKey))
        {
            using (switcher.UseAppScope(appKey))
            {
                switcher.Current.Should().BeSameAs(context.Object);
            }

            manager.Verify(m => m.GetApp(appKey), Times.Once);
        }
        else
        {
            var act = () => switcher.UseAppScope(appKey);

            act.Should().Throw<ArgumentException>(
                "Mud.HttpUtils 3.0.0 适配补齐：appKey 格式校验必须先于应用解析（原实现缺失该校验）");
            manager.Verify(m => m.GetApp(It.IsAny<string>()), Times.Never,
                "格式非法时不得触碰 IWechatAppManager");
        }

        switcher.SwitchTo(null);
    }

    [Fact]
    public void InterfaceContract_ShouldExposeScopeFaceAndKeepLegacyMembersMigratable()
    {
        // Phase 1 + Phase 2 的编译期契约守卫：
        //   ① IWechatAppContextSwitcher 继承 IAppScopeSwitcher ⇒ 推荐面可经接口类型使用；
        //   ② 三个旧成员由本接口「接续声明」⇒ 下游以接口类型编写的既有代码在 3.0.0 上仍可编译，
        //      且被标记 [Obsolete] 以引导迁移（本 SDK 下个大版本随上游移除）。
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(CreateContext("app-a").Object);

        IWechatAppContextSwitcher switcher = new WechatAppContextSwitcher(manager.Object);

        using (switcher.UseAppScope("app-a"))
        {
            switcher.Current.Should().NotBeNull();
        }

#pragma warning disable CS0618 // 类型或成员已过时：本用例正是在钉死「旧成员仍可经接口类型调用」
        var legacy = switcher.UseApp("app-a");
        legacy.Should().NotBeNull();
        using (switcher.BeginScope("app-a"))
        {
            switcher.Current.Should().NotBeNull();
        }
#pragma warning restore CS0618

        switcher.SwitchTo(null);
    }

    // ---------------------------------------------------------------- P2-1 强化：应用 + 企业复合作用域

    /// <summary>
    /// 复合作用域：进入时同时切换应用与企业上下文，释放时两级都还原——否则企业作用域残留到后续不相关调用
    /// （同应用 + 旧 authCorpId）会取到错误企业的令牌。
    /// </summary>
    [Fact]
    public void UseCorpScope_ShouldSwitchAppAndCorp_AndRestoreBothOnDispose()
    {
        var outerApp = CreateContext("app-outer");
        var target = CreateContext("app-target");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-target")).Returns(target.Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SwitchTo(outerApp.Object);

            using (var scope = switcher.UseCorpScope("app-target", "corp-1", "pc-1"))
            {
                scope.Should().NotBeNull();
                switcher.Current.Should().BeSameAs(target.Object, "复合作用域内应切到目标应用");
                WechatCorpContext.AppKey.Should().Be("app-target",
                    "归属应用取解析后的上下文 AppKey（R9：CorpTokenManager 归属校验据此命中）");
                WechatCorpContext.AuthCorpId.Should().Be("corp-1");
                WechatCorpContext.PermanentCode.Should().Be("pc-1");
            }

            switcher.Current.Should().BeSameAs(outerApp.Object,
                "释放后必须还原进入前的应用上下文（与 UseAppScope 同一接缝：不得预先 SwitchTo）");
            WechatCorpContext.AuthCorpId.Should().BeNull("释放后必须还原进入前的企业上下文，而非残留");
            WechatCorpContext.AppKey.Should().BeNull();
            WechatCorpContext.PermanentCode.Should().BeNull();
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    /// <summary>
    /// 嵌套安全：内层复合作用域释放后必须还原<b>外层</b>企业上下文（逆序还原 + 快照语义，
    /// 而非 ClearCorp 的无条件清空）。
    /// </summary>
    [Fact]
    public void UseCorpScope_ShouldRestoreOuterCorpContext_WhenNested()
    {
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(CreateContext("app-a").Object);
        manager.Setup(m => m.GetApp("app-b")).Returns(CreateContext("app-b").Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            WechatCorpContext.SetCorp("outer-app", "outer-corp", "outer-pc");

            using (switcher.UseCorpScope("app-a", "inner-corp", "inner-pc"))
            {
                WechatCorpContext.AppKey.Should().Be("app-a");
                WechatCorpContext.AuthCorpId.Should().Be("inner-corp");
            }

            WechatCorpContext.AppKey.Should().Be("outer-app", "内层代操作不得「清掉」外层企业上下文");
            WechatCorpContext.AuthCorpId.Should().Be("outer-corp");
            WechatCorpContext.PermanentCode.Should().Be("outer-pc");
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    /// <summary>
    /// 失败安全性：企业参数校验失败必须回滚已进入的应用作用域，不留半开作用域。
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void UseCorpScope_ShouldRollbackAppScope_WhenAuthCorpIdBlank(string authCorpId)
    {
        var outerApp = CreateContext("app-outer");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(CreateContext("app-a").Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SwitchTo(outerApp.Object);

            var act = () => switcher.UseCorpScope("app-a", authCorpId);

            act.Should().Throw<ArgumentException>("空白 CorpId 无法成为合法 scope（校验由 WechatCorpContext 收敛）");
            switcher.Current.Should().BeSameAs(outerApp.Object,
                "参数校验失败必须回滚已进入的应用作用域，否则应用上下文被静默切换走");
            WechatCorpContext.AuthCorpId.Should().BeNull();
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    /// <summary>失败安全性：authCorpId 为 null 时同样回滚应用作用域，且异常类型与既有校验一致。</summary>
    [Fact]
    public void UseCorpScope_ShouldRollbackAppScope_WhenAuthCorpIdNull()
    {
        var outerApp = CreateContext("app-outer");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(CreateContext("app-a").Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SwitchTo(outerApp.Object);

            var act = () => switcher.UseCorpScope("app-a", null!);

            act.Should().Throw<ArgumentNullException>("M7：authCorpId 不得缺省");
            switcher.Current.Should().BeSameAs(outerApp.Object);
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    /// <summary>异常路径：作用域块内抛异常时，<c>using</c> 仍须完整还原两级上下文。</summary>
    [Fact]
    public void UseCorpScope_ShouldRestoreBothContexts_WhenExceptionThrownInsideScope()
    {
        var outerApp = CreateContext("app-outer");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(CreateContext("app-a").Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SwitchTo(outerApp.Object);

            var act = () =>
            {
                using (switcher.UseCorpScope("app-a", "corp-1", "pc-1"))
                {
                    throw new InvalidOperationException("业务异常");
                }
            };

            act.Should().Throw<InvalidOperationException>().WithMessage("业务异常");
            switcher.Current.Should().BeSameAs(outerApp.Object, "异常路径也必须归还应用上下文");
            WechatCorpContext.AuthCorpId.Should().BeNull("异常路径也必须归还企业上下文");
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    /// <summary>
    /// 幂等释放：重复 <c>Dispose</c> 不得把「释放后新建立」的上下文改回快照
    /// （<c>using</c> + 显式 Dispose 混用是常见写法）。
    /// </summary>
    [Fact]
    public void UseCorpScope_ShouldBeIdempotent_OnRepeatedDispose()
    {
        var outerApp = CreateContext("app-outer");
        var manager = new Mock<IWechatAppManager>();
        manager.Setup(m => m.GetApp("app-a")).Returns(CreateContext("app-a").Object);

        var switcher = new WechatAppContextSwitcher(manager.Object);
        try
        {
            switcher.SwitchTo(outerApp.Object);

            var scope = switcher.UseCorpScope("app-a", "corp-1", "pc-1");
            scope.Dispose();

            WechatCorpContext.SetCorp("later-app", "later-corp", "later-pc");
            switcher.SwitchTo(CreateContext("app-later").Object);

            scope.Dispose();

            WechatCorpContext.AuthCorpId.Should().Be("later-corp", "重复释放不得把后续上下文改回旧快照");
            switcher.Current!.AppKey.Should().Be("app-later");
        }
        finally
        {
            WechatCorpContext.Clear();
            switcher.SwitchTo(null);
        }
    }

    /// <summary>
    /// 契约引导守卫：复合作用域入口必须声明在契约上（返回 <see cref="IDisposable"/> 以供 <c>using</c>），
    /// 且裸写入原语 <c>SetCorp</c> 必须标记 <see cref="ObsoleteAttribute"/> 并指向该入口——防止引导退化。
    /// </summary>
    [Fact]
    public void InterfaceContract_ShouldExposeCorpScopeEntry_AndSteerPrimitiveToIt()
    {
        var contract = typeof(IWechatAppContextSwitcher);

        var useCorpScope = contract.GetMethod(nameof(IWechatAppContextSwitcher.UseCorpScope));
        useCorpScope.Should().NotBeNull("「应用 + 企业」复合作用域入口必须声明在契约上（调用方经 DI 拿到的即接口类型）");
        useCorpScope!.ReturnType.Should().Be(typeof(IDisposable), "必须可直接用于 using（释放时自动归还两级上下文）");
        useCorpScope.GetParameters().Select(p => p.Name)
            .Should().Equal("appKey", "authCorpId", "permanentCode");
        useCorpScope.GetParameters()[2].HasDefaultValue.Should().BeTrue(
            "permanentCode 可选：缺省时由 IWechatCorpAuthStore 持久化仓储提供");

        var setCorp = contract.GetMethod(nameof(IWechatAppContextSwitcher.SetCorp))!;
        var obsolete = setCorp.GetCustomAttribute<ObsoleteAttribute>();
        obsolete.Should().NotBeNull(
            "裸写入原语必须标记过时以引导迁移：忘记配对 ClearCorp 会让企业作用域残留（跨企业令牌串号）");
        obsolete!.Message.Should().Contain(nameof(IWechatAppContextSwitcher.UseCorpScope),
            "过时提示必须给出可执行的替代入口");
    }
}