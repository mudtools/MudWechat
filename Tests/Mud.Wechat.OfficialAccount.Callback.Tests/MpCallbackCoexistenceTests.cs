// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.Abstractions.Callback;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// P7 跨产品线共存验证：同一宿主同时接入企微与公众号回调链路。
/// </summary>
/// <remarks>
/// 锁定的三条不变式：① 协议与安全内核是**同一事实来源**（叶层单源）；② 两侧注册/Dispatch 互不干扰；
/// ③ 默认路由前缀不冲突（企微 <c>wechat</c> / 公众号 <c>mp</c>）。测试工程引用两条产品线运行时包
/// 属**测试专用边**，生产程序集仍严格单向（CB-L1a/L1b 守卫锁定引用行）。
/// </remarks>
public class MpCallbackCoexistenceTests
{
    /// <summary>① 共用内核单源：重放守卫口与实现、注册表基类、加解密内核全部只在叶层声明。</summary>
    [Fact]
    public void SharedKernel_ShouldBeSingleSourceInAbstractions()
    {
        var leaf = typeof(IWechatCallbackReplayGuard).Assembly.GetName().Name;
        var leafCrypto = typeof(WechatCallbackCrypto).Assembly.GetName().Name;
        var leafRegistry = typeof(WechatCallbackTypeRegistry<>).Assembly.GetName().Name;
        var leafEnvelope = typeof(IWechatCallbackEnvelope).Assembly.GetName().Name;

        leaf.Should().Be("Mud.Wechat.Abstractions");
        leafCrypto.Should().Be("Mud.Wechat.Abstractions");
        leafRegistry.Should().Be("Mud.Wechat.Abstractions");
        leafEnvelope.Should().Be("Mud.Wechat.Abstractions");

        // 两侧都**不再**各自持有重放守卫实现（下沉物唯一性）。
        typeof(Mud.Wechat.OfficialAccount.Callback.MpCallbackDispatcher).Assembly
            .GetType("Mud.Wechat.OfficialAccount.Callback.InMemoryWechatCallbackReplayGuard")
            .Should().BeNull("公众号侧不得留有叶层实现副本");
        typeof(Mud.Wechat.Work.Callback.WechatCallbackDispatcher).Assembly
            .GetType("Mud.Wechat.Work.Callback.InMemoryWechatCallbackReplayGuard")
            .Should().BeNull("企微侧不得留有叶层实现副本");
    }

    /// <summary>③ 默认路由前缀不冲突（同宿主可同时挂载两条链路）。</summary>
    [Fact]
    public void DefaultRoutePrefixes_ShouldNotCollide()
    {
        var work = new WechatCallbackOptions().GlobalRoutePrefix;
        var mp = new MpCallbackOptions().GlobalRoutePrefix;

        work.Should().Be("wechat");
        mp.Should().Be("mp");
        work.Should().NotBe(mp);
    }

    /// <summary>② 两侧 DI 注册可在同一 ServiceCollection 共存，且各自的分发器都能解析出实例。</summary>
    [Fact]
    public void BothServiceRegistrations_ShouldCoexistInOneContainer()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddWechatCallback(_ => { });
        services.AddMpCallback(options =>
        {
            options.Apps[MpCallbackOptions.WildcardAppKey] = new MpAppCallbackOptions
            {
                PushToken = "t",
                AppId = "wxCoexistAppId",
                SecurityMode = MpCallbackSecurityMode.Plain,
            };
        });

        using var provider = services.BuildServiceProvider();

        provider.GetService<MpCallbackDispatcher>().Should().NotBeNull("公众号链路可用");
        provider.GetService<WechatCallbackDispatcher>().Should().NotBeNull("企微链路可用");

        // 两侧共享**同一**叶层重放守卫类型（默认为进程内实现；多实例部署由宿主替换为分布式实现）。
        var guard = provider.GetRequiredService<IWechatCallbackReplayGuard>();
        guard.GetType().Assembly.GetName().Name.Should().Be("Mud.Wechat.Abstractions");
    }

    /// <summary>② 键空间隔离：两侧的处理器注册表类型互不相通（同名键不会串到对方链路）。</summary>
    [Fact]
    public void HandlerRegistries_ShouldBeIsolated()
    {
        var mpRegistry = new MpCallbackHandlerRegistry();
        var workRegistry = new WechatCallbackHandlerRegistry();

        mpRegistry.GetType().Should().NotBe(workRegistry.GetType());
        typeof(MpCallbackHandlerRegistry).Assembly.GetName().Name.Should().Be("Mud.Wechat.OfficialAccount.Callback");
        typeof(WechatCallbackHandlerRegistry).Assembly.GetName().Name.Should().Be("Mud.Wechat.Work.Callback");

        // 同名字符串键分落两表（互不覆盖）。
        mpRegistry.Register("shared-key", typeof(MpCallbackHandlerRegistry));
        workRegistry.Register("shared-key", typeof(WechatCallbackHandlerRegistry));

        mpRegistry.GetAll("shared-key").Should().ContainSingle().Which.Should().Be(typeof(MpCallbackHandlerRegistry));
        workRegistry.GetAll("shared-key").Should().ContainSingle().Which.Should().Be(typeof(WechatCallbackHandlerRegistry));
    }
}
