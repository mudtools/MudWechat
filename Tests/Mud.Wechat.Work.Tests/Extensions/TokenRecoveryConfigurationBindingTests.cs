// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mud.HttpUtils;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Tests.Extensions;

/// <summary>
/// W-14：令牌恢复选项的"纯 DTO（TokenRecoveryConfiguration）+ Apply"绑定语义。
/// </summary>
/// <remarks>
/// 旧写法直接 <c>Bind(TokenRecoveryOptions)</c>（含接口型判定器属性）会产 SYSLIB1100/1101 并靠项目级
/// NoWarn 整体静音；新写法绑定仅基元字段的 <see cref="TokenRecoveryConfiguration"/> 再 <c>Apply</c>。
/// 本组用例锁定三件事：绑定等价（配置值逐字段到达 Options）、Apply 不触碰判定器槽位（PostConfigure
/// 组装不受影响）、热更（配置重载 ⇒ CurrentValue 刷新）不因绑定方式变更而退化（R5）。
/// </remarks>
public class TokenRecoveryConfigurationBindingTests
{
    /// <summary>可外部驱动的内存配置源（InMemory 源不支持 Reload 后取新值，本替身补齐该能力）。</summary>
    private sealed class ReloadableMemorySource : IConfigurationSource
    {
        public Dictionary<string, string?> Data { get; } = new();

        public IConfigurationProvider Build(IConfigurationBuilder builder) => new ReloadableMemoryProvider(this);
    }

    private sealed class ReloadableMemoryProvider : ConfigurationProvider
    {
        private readonly ReloadableMemorySource _source;

        public ReloadableMemoryProvider(ReloadableMemorySource source)
        {
            _source = source;
            Load();
        }

        public override void Load()
        {
            Data = new Dictionary<string, string?>(_source.Data, StringComparer.OrdinalIgnoreCase);
        }

        public void SetExternal(string key, string? value)
        {
            _source.Data[key] = value;
            OnReload();
        }
    }

    private static (IConfigurationRoot Root, ReloadableMemoryProvider Provider) BuildConfiguration(
        Dictionary<string, string?> initial)
    {
        var source = new ReloadableMemorySource();
        foreach (var (key, value) in initial)
        {
            source.Data[key] = value;
        }

        var root = new ConfigurationBuilder().Add(source).Build();
        return (root, (ReloadableMemoryProvider)root.Providers.Single());
    }

    private static Dictionary<string, string?> WorkAppAndRecoverySection(
        Dictionary<string, string?> recovery)
    {
        var data = new Dictionary<string, string?>
        {
            ["WechatApps:0:AppKey"] = "work-default",
            ["WechatApps:0:CorpId"] = "ww-corp",
            ["WechatApps:0:AgentSecret"] = "agent-secret",
        };
        foreach (var (key, value) in recovery)
        {
            data[$"{TokenRecoveryOptions.SectionName}:{key}"] = value;
        }

        return data;
    }

    /// <summary>绑定等价：节内值经纯 DTO + Apply 逐字段到达 TokenRecoveryOptions。</summary>
    [Fact]
    public void SectionValues_ShouldFlowThroughConfigurationDto_ToRecoveryOptions()
    {
        var (configuration, _) = BuildConfiguration(WorkAppAndRecoverySection(new Dictionary<string, string?>
        {
            ["RecoveryMaxRetries"] = "5",
            ["RefreshDedupWindowSeconds"] = "7",
        }));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<TokenRecoveryOptions>>().CurrentValue;

        options.RecoveryMaxRetries.Should().Be(5, "配置节值必须经 TokenRecoveryConfiguration + Apply 到达 Options");
        options.RefreshDedupWindowSeconds.Should().Be(7, "Apply 必须覆盖全部可绑定字段");
    }

    /// <summary>Apply 不触碰判定器槽位：组合器仍由公用层 PostConfigure 组装（两通道写不同字段，无冲突）。</summary>
    [Fact]
    public void ApplyBinding_ShouldNotClobberCompositeDetector_FromPostConfigure()
    {
        var (configuration, _) = BuildConfiguration(WorkAppAndRecoverySection(new Dictionary<string, string?>
        {
            ["RecoveryMaxRetries"] = "3",
        }));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<TokenRecoveryOptions>>().CurrentValue;

        options.TokenInvalidationDetector.Should().BeOfType<Mud.HttpUtils.CompositeTokenInvalidationDetector>(
            "W-13 的 PostConfigure 组装不得被配置绑定覆盖（Apply 不写判定器槽位）");
        options.RecoveryMaxRetries.Should().Be(3);
    }

    /// <summary>热更语义（R5）：配置重载 ⇒ IOptionsMonitor.CurrentValue 读到新值（不因绑定方式变更而退化）。</summary>
    [Fact]
    public void ConfigurationReload_ShouldRefreshRecoveryOptions()
    {
        var (configuration, memoryProvider) = BuildConfiguration(WorkAppAndRecoverySection(new Dictionary<string, string?>
        {
            ["RecoveryMaxRetries"] = "1",
        }));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(configuration);

        using var provider = services.BuildServiceProvider();
        var monitor = provider.GetRequiredService<IOptionsMonitor<TokenRecoveryOptions>>();
        monitor.CurrentValue.RecoveryMaxRetries.Should().Be(1);

        memoryProvider.SetExternal($"{TokenRecoveryOptions.SectionName}:RecoveryMaxRetries", "9");
        configuration.Reload();

        monitor.CurrentValue.RecoveryMaxRetries.Should().Be(9,
            "变更令牌来源（IOptionsChangeTokenSource，指向同一节）必须把配置重载传导到 Options 缓存失效");
    }

    /// <summary>回归锚：配置节缺省时 Options 保持组件默认值（不得因两段式引入意外默认）。</summary>
    [Fact]
    public void MissingSection_ShouldKeepComponentDefaults()
    {
        var (configuration, _) = BuildConfiguration(WorkAppAndRecoverySection(new Dictionary<string, string?>()));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(configuration);

        using var provider = services.BuildServiceProvider();
        var defaults = new TokenRecoveryOptions();
        var options = provider.GetRequiredService<IOptionsMonitor<TokenRecoveryOptions>>().CurrentValue;

        options.RecoveryMaxRetries.Should().Be(defaults.RecoveryMaxRetries);
        options.Enabled.Should().Be(defaults.Enabled);
        options.TokenInvalidationDetector.Should().NotBeNull("PostConfigure 组装与配置绑定无关（应用已注册时必有判定器）");
    }
}
