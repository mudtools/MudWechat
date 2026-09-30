// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调 DI 装配测试（P0-2）：抗重放守卫可解析、跨 scope 同一实例、宿主可前置覆盖为分布式实现。
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
        });

        using var provider = services.BuildServiceProvider();

        ReferenceEquals(provider.GetRequiredService<IWechatCallbackReplayGuard>(), distributed.Object)
            .Should().BeTrue("宿主注册的分布式实现优先（多实例部署必需）");
    }
}
