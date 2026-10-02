// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调 DI 装配测试（P0-2 / P1-3）：抗重放守卫可解析、跨 scope 同一实例、宿主可前置覆盖为分布式实现；
/// 统一注册表（接收方 ID 必填唯一、注册期 fail-fast）、接收器与 URL 验证器同实例。
/// </summary>
public class WechatCallbackServiceCollectionExtensionsTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";

    private static ServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatCallback(options =>
        {
            options.PushToken = "push-token";
            options.PushEncodingAESKey = AesKey;
            options.CorpId = "ww-corp";
        });

        return services;
    }

    [Fact]
    public void ReplayGuard_ShouldBeResolvableAndSingleton()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        var guard = provider.GetRequiredService<IWechatCallbackReplayGuard>();
        guard.Should().BeOfType<InMemoryWechatCallbackReplayGuard>();

        using var scope = provider.CreateScope();
        ReferenceEquals(guard, scope.ServiceProvider.GetRequiredService<IWechatCallbackReplayGuard>())
            .Should().BeTrue("重放守卫必须跨 scope 同一实例，否则去重窗口形同虚设");
    }

    [Fact]
    public void Receiver_ShouldBeGroup_AndShareInstanceWithUrlVerifier()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider();

        var receiver = provider.GetRequiredService<IWechatCallbackReceiver>();
        var verifier = provider.GetRequiredService<IWechatCallbackUrlVerifier>();

        receiver.Should().BeOfType<WechatCallbackReceiverGroup>(
            "P1-3：统一注册表形态下 IWechatCallbackReceiver 恒为组合接收器（按 ToUserName 路由）");
        ReferenceEquals(receiver, verifier)
            .Should().BeTrue("IWechatCallbackUrlVerifier 与接收器共享同一组合接收器实例");
    }

    [Fact]
    public void Receiver_ShouldReceiveReplayGuardFromContainer()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider();

        var receiver = provider.GetRequiredService<IWechatCallbackReceiver>();
        var guard = provider.GetRequiredService<IWechatCallbackReplayGuard>();

        // 经容器注入的守卫必须真实生效（同一指纹第二次拒绝）。
        var mark1 = guard.TryMarkAsync("fingerprint-1", TimeSpan.FromMinutes(5)).GetAwaiter().GetResult();
        var mark2 = guard.TryMarkAsync("fingerprint-1", TimeSpan.FromMinutes(5)).GetAwaiter().GetResult();

        mark1.Should().BeTrue();
        mark2.Should().BeFalse("同键在窗口内只能被首次消费");
        receiver.Should().NotBeNull();
    }

    [Fact]
    public void ReplayGuard_ShouldBeOverridableByHost_WhenRegisteredFirst()
    {
        // TryAdd 前置注册语义：宿主的分布式实现必须先于 AddWechatCallback 注册。
        var services = new ServiceCollection();
        services.AddLogging();
        var distributed = new Mock<IWechatCallbackReplayGuard>();
        services.AddSingleton(distributed.Object);

        services.AddWechatCallback(options =>
        {
            options.PushToken = "push-token";
            options.PushEncodingAESKey = AesKey;
            options.CorpId = "ww-corp";
        });

        using var provider = services.BuildServiceProvider();

        ReferenceEquals(provider.GetRequiredService<IWechatCallbackReplayGuard>(), distributed.Object)
            .Should().BeTrue("宿主注册的分布式实现优先（多实例部署必需）");
    }

    [Fact]
    public void AddWechatCallbackSuite_ShouldAppendEntry_ToSameRegistry()
    {
        // P1-3/D11：两个语义化入口登记同一注册表，无模式互斥。
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatCallback(o =>
        {
            o.PushToken = "token-a";
            o.PushEncodingAESKey = AesKey;
            o.CorpId = "ww-suite-a";
        });
        services.AddWechatCallbackSuite(o =>
        {
            o.PushToken = "token-b";
            o.PushEncodingAESKey = AesKey;
            o.CorpId = "ww-suite-b";
        });

        using var provider = services.BuildServiceProvider();
        var receiver = provider.GetRequiredService<IWechatCallbackReceiver>();
        receiver.Should().NotBeNull();
        receiver.Should().BeOfType<WechatCallbackReceiverGroup>();
    }

    [Fact]
    public void AddWechatCallback_ShouldThrow_WhenDuplicateReceiverId()
    {
        var services = new ServiceCollection();
        services.AddWechatCallback(o =>
        {
            o.PushToken = "token-a";
            o.PushEncodingAESKey = AesKey;
            o.CorpId = "ww-suite-a";
        });

        var act = () => services.AddWechatCallbackSuite(o =>
        {
            o.PushToken = "token-b";
            o.PushEncodingAESKey = AesKey;
            o.CorpId = "ww-suite-a";
        });

        act.Should().Throw<InvalidOperationException>().WithMessage("*接收方 ID 重复*");
    }

    [Fact]
    public void AddWechatCallback_ShouldThrow_WhenReceiverIdEmpty()
    {
        // P1-3 配套收紧：接收方 ID（CorpId）必填——原「为空则跳过校验」的静默弱化在注册期即拦截。
        var services = new ServiceCollection();
        var act = () => services.AddWechatCallback(o =>
        {
            o.PushToken = "push-token";
            o.PushEncodingAESKey = AesKey;
        });

        act.Should().Throw<InvalidOperationException>().WithMessage("*CorpId*");
    }

    [Fact]
    public void AddWechatCallback_ShouldFailFastAtRegistration_WhenConfigMissing()
    {
        // F12：配置校验前置——注册期即抛，不再等首次解析。
        var services = new ServiceCollection();
        var act = () => services.AddWechatCallback(o => o.CorpId = "ww-corp");
        act.Should().Throw<InvalidOperationException>().WithMessage("*PushToken*");
    }
}
