// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R7：DI 编排（顺序守卫 fail-fast / 四端口解析 / TryAdd 覆盖语义 / 幂等 / Scope 同实例）。
/// </summary>
public class WechatRedisServiceCollectionExtensionsTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";

    private static WechatAppConfig CreateAppConfig()
        => new() { AppKey = "default", CorpId = "ww-corp", AgentSecret = "agent-secret" };

    private static IServiceCollection CreateServices(Action<WechatRedisOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatRedis(configure ?? (o => o.Connection.ServerAddress = "localhost:6379"));
        // 单测不连真实 Redis：后置 mock 覆盖（DI 解析取最后注册者；AddWechatRedis 的连接 lambda 惰性不实例化）。
        services.AddSingleton(new FakeRedisBackend().Multiplexer.Object);
        return services;
    }

    [Fact]
    public void AddWechatRedis_ShouldThrow_WhenCalledAfterAddWechatApp()
    {
        var services = new ServiceCollection();
        services.AddWechatApp(new List<WechatAppConfig> { CreateAppConfig() });

        var act = () => services.AddWechatRedis(o => { });

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddWechatApp*");
    }

    [Fact]
    public void AddWechatRedis_ShouldThrow_WhenCalledAfterAddWechatCallback()
    {
        var services = new ServiceCollection();
        services.AddWechatCallback(o =>
        {
            o.Apps["default"] = new WechatAppCallbackOptions
            {
                PushToken = "token",
                PushEncodingAESKey = AesKey,
                ReceiveId = "ww-corp",
            };
        });

        var act = () => services.AddWechatRedis(o => { });

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddWechatCallback*");
    }

    [Fact]
    public void AddWechatRedis_ShouldNotThrow_WhenHostPreRegisteredCustomReplayGuard()
    {
        // R-3：宿主预注册自定义实现属 TryAdd 契约内的合法前置覆盖，不视为「回调已注册」。
        var services = new ServiceCollection();
        services.AddSingleton<IWechatCallbackReplayGuard>(new InMemoryWechatCallbackReplayGuard());

        var act = () => services.AddWechatRedis(o => { });

        act.Should().NotThrow();
    }

    [Fact]
    public async Task AddWechatRedis_ShouldResolveAllFourPorts_WhenCorrectOrder()
    {
        var services = CreateServices();
        services.AddWechatApp(new List<WechatAppConfig> { CreateAppConfig() });
        services.AddWechatCallback(o =>
        {
            o.Apps["default"] = new WechatAppCallbackOptions
            {
                PushToken = "token",
                PushEncodingAESKey = AesKey,
                ReceiveId = "ww-corp",
            };
        });

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        (await provider.GetRequiredService<IWechatTokenStore>().GetTokenTypesAsync()).Should().NotBeNull();
        provider.GetRequiredService<IWechatCorpAuthStore>().Should().BeOfType<RedisWechatCorpAuthStore>();
        provider.GetRequiredService<IWechatSuiteTicketStore>().Should().BeOfType<RedisWechatSuiteTicketStore>();
        provider.GetRequiredService<IWechatCallbackReplayGuard>().Should().BeOfType<RedisWechatCallbackReplayGuard>();
        provider.GetRequiredService<IWechatCallbackReceiver>().Should().NotBeNull();
    }

    [Fact]
    public async Task AddWechatRedis_ShouldRegisterSingletonAsSameInstanceAcrossScopes()
    {
        var services = CreateServices();
        services.AddWechatApp(new List<WechatAppConfig> { CreateAppConfig() });

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using var scope = provider.CreateScope();

        provider.GetRequiredService<IWechatTokenStore>().Should().BeSameAs(scope.ServiceProvider.GetRequiredService<IWechatTokenStore>());
        provider.GetRequiredService<IWechatCorpAuthStore>().Should().BeSameAs(scope.ServiceProvider.GetRequiredService<IWechatCorpAuthStore>());
        provider.GetRequiredService<IWechatSuiteTicketStore>().Should().BeSameAs(scope.ServiceProvider.GetRequiredService<IWechatSuiteTicketStore>());
        provider.GetRequiredService<IWechatCallbackReplayGuard>().Should().BeSameAs(scope.ServiceProvider.GetRequiredService<IWechatCallbackReplayGuard>());
        await Task.CompletedTask;
    }

    [Fact]
    public void AddWechatRedis_ShouldBeIdempotent_WhenCalledTwice()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatRedis(o => { });
        services.AddWechatRedis(o => { });

        services.Count(s => s.ServiceType == typeof(IConnectionMultiplexer)).Should().Be(1, "重复调用不得重复注册连接基座");
        services.Count(s => s.ServiceType == typeof(IWechatCallbackReplayGuard)).Should().Be(1);
    }

    [Fact]
    public void AddWechatRedis_ShouldLetPreRegisteredCustomImplementationWin()
    {
        // R-3：端口接口 TryAdd——宿主预注册的自定义实现胜出；Redis 具体类型仍恒注册可自省。
        var custom = new InMemoryWechatTokenStore();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IWechatTokenStore>(custom);
        services.AddWechatRedis(o => { });
        services.AddSingleton(new FakeRedisBackend().Multiplexer.Object);

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IWechatTokenStore>().Should().BeSameAs(custom);
        provider.GetRequiredService<RedisWechatTokenStore>().Should().NotBeNull();
    }

    [Fact]
    public void AddWechatRedis_ShouldExposeConcreteTypes()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<RedisWechatTokenStore>().Should().BeAssignableTo<IWechatTokenStoreBatchRemove>();
        provider.GetRequiredService<RedisHealthCheck>().Should().NotBeNull();
    }

    [Fact]
    public void AddWechatRedis_ShouldSkipHealthCheckPipelineRegistration_WhenDisabled()
    {
        // registerHealthCheck=false：不向宿主健康检查管线注册（§7.5——避免向未使用健康检查的宿主
        // 隐式注册整套基础设施），仅注册 RedisHealthCheck 类型由宿主自行 AddCheck。
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatRedis(o => { }, registerHealthCheck: false);
        services.AddSingleton(new FakeRedisBackend().Multiplexer.Object);

        using var provider = services.BuildServiceProvider();
        provider.GetService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>()
            .Should().BeNull("未向宿主健康检查管线注册");
        provider.GetRequiredService<RedisHealthCheck>().Should().NotBeNull("类型恒注册，供宿主自行 AddCheck");
    }

    [Fact]
    public void AddWechatRedis_ShouldRegisterHealthCheckService_WhenEnabled()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatRedis(o => { }, registerHealthCheck: true);
        services.AddSingleton(new FakeRedisBackend().Multiplexer.Object);

        using var provider = services.BuildServiceProvider();
        provider.GetService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>()
            .Should().NotBeNull();
    }
}
