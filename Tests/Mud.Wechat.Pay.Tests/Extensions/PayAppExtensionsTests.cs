// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Tests.Credential;

namespace Mud.Wechat.Pay.Tests.Extensions;

/// <summary>
/// <see cref="PayAppExtensions"/>（<c>AddPayApp</c>）DI 装配测试（P0-c 出口门禁：多商户可注入）。
/// </summary>
public class PayAppExtensionsTests
{
    private const string PrivateKeyName = "pay:merchant:1900000000:key";
    private const string ApiKeyName = "pay:merchant:1900000000:apikey";

    /// <summary>
    /// 多商户可注入且为<b>跨 scope 单例</b>（AGENTS §7：新增 Singleton 必补「DI 可解析 + 跨 scope 同一实例」）。
    /// </summary>
    [Fact]
    public void MerchantManager_ShouldBeResolvableSingletonAcrossScopes_WhenRegistered()
    {
        using var provider = BuildProvider(
            services => services.AddPayApp(CreateMerchants()), withSecrets: true);

        IWechatPayMerchantManager Resolve(IServiceProvider sp) => sp.GetRequiredService<IWechatPayMerchantManager>();

        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        var fromA = Resolve(scopeA.ServiceProvider);
        var fromB = Resolve(scopeB.ServiceProvider);

        fromA.Should().BeSameAs(fromB, "多商户基座是启动期构建的只读快照，必须跨 scope 同一实例");
        fromA.Merchants.Should().HaveCount(2);
    }

    /// <summary>凭据取用器同样为跨 scope 单例。</summary>
    [Fact]
    public void CredentialProvider_ShouldBeResolvableSingletonAcrossScopes_WhenSecretProviderRegistered()
    {
        using var provider = BuildProvider(
            services => services.AddPayApp(CreateMerchants()), withSecrets: true);

        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        var fromA = scopeA.ServiceProvider.GetRequiredService<IWechatPayMerchantCredentialProvider>();
        var fromB = scopeB.ServiceProvider.GetRequiredService<IWechatPayMerchantCredentialProvider>();

        fromA.Should().BeSameAs(fromB);
    }

    /// <summary>
    /// 宿主未注册 <c>ISecretProvider</c> 时，必须给出<b>点名</b>错误 ——
    /// 默认 DI 缺失异常只报接口名，宿主很难一眼看出「是谁要求注册的、该怎么修」。
    /// </summary>
    [Fact]
    public void CredentialProvider_ShouldExplainMissingSecretProvider_WhenHostDidNotRegister()
    {
        using var provider = BuildProvider(
            services => services.AddPayApp(CreateMerchants()), withSecrets: false);

        var act = () => provider.GetRequiredService<IWechatPayMerchantCredentialProvider>();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*未注册 ISecretProvider*")
            .And.Message.Should().Contain("AddPayApp",
                "异常必须同时说明原因与修复动作");
    }

    /// <summary>单商户链式委托重载：与集合重载装配出等价的基座。</summary>
    [Fact]
    public void AddPayApp_ShouldSupportConfigureOverload_WhenSingleMerchant()
    {
        using var provider = BuildProvider(
            services => services.AddPayApp(config =>
            {
                config.MchId = "1900000000";
                config.SerialNumber = "SERIAL";
                config.PrivateKeySecretName = PrivateKeyName;
                config.ApiKeySecretName = ApiKeyName;
            }), withSecrets: true);

        provider.GetRequiredService<IWechatPayMerchantManager>()
            .Merchants.Single().MerchantKey.Should().Be("1900000000");
    }

    /// <summary>
    /// 坏配置必须<b>钉在注册期</b>，不得拖到首次解析（更不得拖到首次真实交易）。
    /// </summary>
    [Fact]
    public void AddPayApp_ShouldFailAtRegistrationTime_WhenConfigurationInvalid()
    {
        var services = new ServiceCollection();

        var act = () => services.AddPayApp(new List<WechatPayMerchantConfig>
        {
            new() { MchId = "1900000000", SerialNumber = "SERIAL" }, // 缺两个密钥名
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*PrivateKeySecretName*");
    }

    /// <summary>
    /// 宿主预注册的管理器按契约胜出（TryAdd 语义）——
    /// 宿主可能需要从配置中心动态增删商户，SDK 不得覆盖。
    /// </summary>
    [Fact]
    public void AddPayApp_ShouldNotOverrideHostRegisteredManager_WhenPreRegistered()
    {
        var hostManager = new Mock<IWechatPayMerchantManager>();
        var services = new ServiceCollection();
        services.AddSingleton(hostManager.Object);

        services.AddPayApp(CreateMerchants());

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IWechatPayMerchantManager>()
            .Should().BeSameAs(hostManager.Object, "宿主预注册者按契约胜出");
    }

    /// <summary>环境商户上下文为 Singleton：跨 scope 同一实例（否则作用域内外读到两份「当前商户」）。</summary>
    [Fact]
    public void MerchantContext_ShouldBeResolvableSingletonAcrossScopes_WhenRegistered()
    {
        // 两个商户 ⇒ 没有「默认商户」，未开作用域时上下文必须 fail-fast（顺带证明上下文已接管判定）。
        using var provider = BuildProvider(
            s => s.AddPayApp(CreateMerchants()),
            withSecrets: true);

        IWechatPayMerchantContext Resolve(IServiceProvider sp) =>
            sp.GetRequiredService<IWechatPayMerchantContext>();

        var act = () => Resolve(provider).ResolveCurrent();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*UseMerchant*", "因为注册了多个商户，静默挑第一个会让请求签到错误商户");

        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        Resolve(scopeA.ServiceProvider)
            .Should().BeSameAs(Resolve(scopeB.ServiceProvider), "because 环境商户上下文必须跨 scope 同一实例");
    }

    /// <summary>宿主预注册自己的上下文时按契约胜出（与管理器同一 TryAdd 语义）。</summary>
    [Fact]
    public void AddPayApp_ShouldNotOverrideHostRegisteredMerchantContext_WhenPreRegistered()
    {
        var services = new ServiceCollection();
        var hostContext = new Mock<IWechatPayMerchantContext>();

        services.AddSingleton(hostContext.Object);
        services.AddPayApp(CreateMerchants());

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IWechatPayMerchantContext>()
            .Should().BeSameAs(hostContext.Object, "宿主预注册者按契约胜出");
    }

    /// <summary>辅助：构造两个合法商户。</summary>
    private static List<WechatPayMerchantConfig> CreateMerchants() => new()
    {
        new()
        {
            MchId = "1900000000",
            SerialNumber = "SERIAL-0",
            PrivateKeySecretName = PrivateKeyName,
            ApiKeySecretName = ApiKeyName,
        },
        new()
        {
            MchId = "1900000001",
            SerialNumber = "SERIAL-1",
            PrivateKeySecretName = "pay:merchant:1900000001:key",
            ApiKeySecretName = "pay:merchant:1900000001:apikey",
        },
    };

    /// <summary>辅助：可选地注册密钥端口后构建根容器（开 scope 校验）。</summary>
    private static ServiceProvider BuildProvider(
        Action<IServiceCollection> configure, bool withSecrets)
    {
        var services = new ServiceCollection();

        if (withSecrets)
        {
            services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
                new Dictionary<string, string> { [PrivateKeyName] = "unused", [ApiKeyName] = "unused" }));
        }

        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            // 只开 scope 校验。**不**开 ValidateOnBuild：它会在 Build 期急切解析全部服务，
            // 而「宿主未注册 ISecretProvider」的用例正要断言错误发生在**解析期**而非构建期。
            ValidateScopes = true,
            ValidateOnBuild = false,
        });
    }
}
