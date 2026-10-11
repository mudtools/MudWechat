// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests;

/// <summary>
/// 小店模块注册器行为用例：增量交付期 fail-fast、幂等注册、Build 先决条件与已落地域可解析。
/// </summary>
/// <remarks>
/// <para>
/// 小店线按设计方案 v1 P1/P2 分阶段交付 27 个业务域，<see cref="ChannelsModule"/> 枚举**先于**实现全量声明，
/// 故「枚举成员存在但域未落地」是现实路径 —— <c>AddModules</c> 必须对该形态 fail-fast，
/// 不得静默跳过（静默吞会让误装配在首次业务调用时才暴露，且单装未落地域时
/// <c>Build()</c> 的「至少需要添加一个模块」错误信息误导排障方向）。
/// </para>
/// </remarks>
public class ChannelsServiceBuilderTests
{
    /// <summary>T1：注册未落地域（枚举成员存在但无注册器）必须抛出带域名的明确错误。</summary>
    [Fact]
    public void AddModules_ShouldThrow_WhenModuleNotYetLanded()
    {
        var act = () => new ServiceCollection().CreateChannelsServicesBuilder().AddModules(ChannelsModule.Vip);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*ChannelsModule.Vip*尚未落地*");
    }

    /// <summary>T2：枚举版入口同样 fail-fast（同根因不得有两种表象）。</summary>
    [Fact]
    public void AddWechatChannelsApi_ShouldThrow_WhenOnlyUnlandedModulePassed()
    {
        var act = () => new ServiceCollection().AddWechatChannelsApi(ChannelsModule.PlatformKf);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*ChannelsModule.PlatformKf*尚未落地*");
    }

    /// <summary>T3：已落地域重复注册幂等（第二次 AddModule 不重复登记服务）。</summary>
    [Fact]
    public void AddModules_ShouldBeIdempotent_WhenSameLandedModuleAddedTwice()
    {
        var services = new ServiceCollection()
            .AddChannelsApp(config =>
            {
                config.AppKey = "channels-builder-test";
                config.AppId = "wx-builder-test";
                config.AppSecret = "secret";
            })
            .CreateChannelsServicesBuilder()
            .AddModules(ChannelsModule.Basic)
            .AddModules(ChannelsModule.Basic)
            .Build();

        var namedClients = services.Where(static d => d.ServiceType == typeof(IHttpClientFactory)).ToList();
        var basicDescriptors = services
            .Where(static d => d.ServiceType == typeof(IChannelsBasicService))
            .ToList();

        basicDescriptors.Should().HaveCount(1, "重复 AddModules 同一域不得重复注册客户端");
        namedClients.Should().NotBeEmpty("已落地域必须登记命名 HttpClient");
    }

    /// <summary>T4：未装配令牌底座时 Build() 必须给出「先 AddChannelsApp」的明确指引。</summary>
    [Fact]
    public void Build_ShouldThrow_WhenTokenBaseMissing()
    {
        var act = () => new ServiceCollection()
            .CreateChannelsServicesBuilder()
            .AddModules(ChannelsModule.Basic)
            .Build();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*IChannelsAppManager*AddChannelsApp*");
    }

    /// <summary>T5：AddAllApis 覆盖全部已落地域，六域客户端均可从容器解析（ValidateScopes 防 Captive Dependency）。</summary>
    [Fact]
    public void AddAllApis_ShouldResolveAllLandedDomainClients()
    {
        var services = new ServiceCollection()
            .AddChannelsApp(config =>
            {
                config.AppKey = "channels-builder-test";
                config.AppId = "wx-builder-test";
                config.AppSecret = "secret";
            })
            .AddWechatChannelsApi(b => b.AddAllApis());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        });

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IChannelsBasicService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IChannelsFundsService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IChannelsProductService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IChannelsOrderService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IChannelsAftersaleService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IChannelsLogisticsService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IChannelsAppManager>().Should().NotBeNull();
    }
}
