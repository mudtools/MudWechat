// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.ExtendedSDK.Finance;

namespace Mud.Wechat.Work.Tests.ExtendedSDK.Finance;

/// <summary>
/// 会话存档配置面与 DI 装配的测试（校验 fail-fast、密钥名误填拒绝、工厂注册形态、装配前置顺序）。
/// </summary>
/// <remarks>
/// <b>不触原生库</b>：本文件所有用例都停在「配置校验 / 密钥解析 / 机器人定位」这几步 ——
/// 它们在 <c>NewSdk</c> 之前，正是无原生库环境下能覆盖的全部真实逻辑。装载/Init 相关行为
/// 由 <c>WechatFinanceContractGuards</c> 的源码与反射断言承担（原生库缺失时的报错形态另有门控用例）。
/// </remarks>
public class WechatFinanceSdkTests
{
    private const string SecretName = "finance:archive:secret";
    private const string PrivateKeySecretName = "finance:archive:pk:1";

    [Fact]
    public void Validate_ShouldThrow_WhenRobotsMissing()
    {
        var options = new WechatFinanceOptions();

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("Robots");
    }

    [Theory]
    [InlineData(0, "DefaultTimeoutSeconds")]
    [InlineData(-1, "DefaultTimeoutSeconds")]
    public void Validate_ShouldThrow_WhenDefaultTimeoutNotPositive(int timeout, string expectedInMessage)
    {
        var options = NewOptions();
        options.DefaultTimeoutSeconds = timeout;

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain(expectedInMessage);
    }

    [Fact]
    public void Validate_ShouldThrow_WhenMediaShardRetryIsNegative()
    {
        var options = NewOptions();
        options.MediaShardRetryCount = -1;

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("MediaShardRetryCount");
    }

    /// <summary>最常见的配置事故：把 PEM 或密钥体填进「密钥名」字段（一旦回显即泄密）。</summary>
    [Theory]
    [InlineData("-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBg\n-----END PRIVATE KEY-----")]
    [InlineData("MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKXwgrHB6c8wggIiMA0GCBKXwgrHB6c8wIEF")]
    public void Validate_ShouldRejectKeyMaterial_WhenSecretNameHoldsTheValueItself(string misfilled)
    {
        var options = NewOptions();
        options.Robots["archive-1"].SecretSecretName = misfilled;

        var act = () => options.Validate();

        var ex = act.Should().Throw<InvalidOperationException>();
        ex.Which.Message.Should().Contain("密钥原文");
        ex.Which.Message.Should().NotContain("BEGIN PRIVATE KEY", "拒绝消息本身不得把误填的内容回显出去");
        ex.Which.Message.Should().NotContain("MIIEvQIBADANBg");
    }

    /// <summary>私钥映射缺失 ⇒ 任何记录都无法解密（必须是硬失败而非警告）。</summary>
    [Fact]
    public void Validate_ShouldThrow_WhenPrivateKeyMappingIsEmpty()
    {
        var options = NewOptions();
        options.Robots["archive-1"].PrivateKeySecretNames.Clear();

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("PrivateKeySecretNames");
    }

    [Theory]
    [InlineData("v1")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("")]
    public void Validate_ShouldThrow_WhenPrivateKeyVersionKeyIsNotPositiveInteger(string versionKey)
    {
        var options = NewOptions();
        var robot = options.Robots["archive-1"];
        robot.PrivateKeySecretNames.Clear();
        robot.PrivateKeySecretNames[versionKey] = PrivateKeySecretName;

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("正整数版本号");
    }

    /// <summary>机器人键参与诊断文本与配置定位，形状沿用 AppKey 规则（含 <c>:</c> 会造成键别名）。</summary>
    [Fact]
    public void Validate_ShouldThrow_WhenRobotKeyContainsColon()
    {
        var options = NewOptions();
        options.Robots["archive:1"] = NewRobot();

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Validate_ShouldThrow_WhenProxyAddressHasWhitespace()
    {
        var options = NewOptions();
        options.Robots["archive-1"].ProxyAddress = "http://127.0.0.1:8080 x";

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("ProxyAddress");
    }

    [Fact]
    public void ResolveTimeoutSeconds_ShouldFallBackToDefault_WhenRobotTimeoutUnset()
    {
        var robot = NewRobot();

        robot.ResolveTimeoutSeconds(20).Should().Be(20, "0 = 沿用全局默认（未设置不等于设置为 0 秒）");

        robot.TimeoutSeconds = 5;
        robot.ResolveTimeoutSeconds(20).Should().Be(5, "机器人级覆盖全局");
    }

    /// <summary>字典键是文化无关字面量：非英语文化下若用 <c>ToString()</c> 会查不到自己配的键。</summary>
    /// <remarks>版本号取 1234：de-DE 的 <c>ToString()</c> 会产出 "1.234"（带分组符），与 InvariantCulture 的
    /// "1234" 可区分 —— 版本号 1 在两种文化下字符串相同，对该缺陷零检测力。</remarks>
    [Fact]
    public void TryGetPrivateKeySecretName_ShouldBeCultureInvariant()
    {
        const int version = 1234;
        var versionSecretName = "finance:archive:pk:1234";
        var original = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = new("de-DE");
            var robot = NewRobot();
            robot.PrivateKeySecretNames[version.ToString(System.Globalization.CultureInfo.InvariantCulture)] = versionSecretName;

            robot.TryGetPrivateKeySecretName(version, out var secretName).Should().BeTrue();
            secretName.Should().Be(versionSecretName);

            robot.TryGetPrivateKeySecretName(2, out var missing).Should().BeFalse();
            missing.Should().BeNull();
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void AddWechatFinanceSdk_ShouldRegisterSingletonFactory()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(new Dictionary<string, string?>
        {
            [SecretName] = "archive-secret-value",
        }));

        services.AddWechatFinanceSdk(Configure);

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using var scope = provider.CreateScope();

        var first = scope.ServiceProvider.GetRequiredService<IWechatWorkFinanceClientFactory>();
        var second = provider.GetRequiredService<IWechatWorkFinanceClientFactory>();

        first.Should().BeSameAs(second,
            "工厂必须是 Singleton：按机器人缓存的原生 SDK 实例若跨 scope 各建一份，会出现同一机器人两份拉取进度");
        first.Dispose();
    }

    [Fact]
    public void AddWechatFinanceSdk_ShouldThrowNamedError_WhenSecretProviderMissing()
    {
        var services = new ServiceCollection();
        services.AddWechatFinanceSdk(Configure);

        using var provider = services.BuildServiceProvider();

        // 点名错误：默认 DI 缺失异常只报接口名，看不出「是谁要求注册的」。
        var act = () => provider.GetRequiredService<IWechatWorkFinanceClientFactory>();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should()
            .Contain("ISecretProvider").And.Contain("AddWechatFinanceSdk");
    }

    [Fact]
    public void AddWechatFinanceSdk_ShouldFailFastAtResolve_WhenRobotsMissing()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(new Dictionary<string, string?>()));

        // 默认配置即「无 Robots」，非法形态不必额外构造。
        services.AddWechatFinanceSdk(static _ => { });

        using var provider = services.BuildServiceProvider();

        var act = () => provider.GetRequiredService<IWechatWorkFinanceClientFactory>();

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("Robots",
            "配置非法必须钉在解析工厂时，而不是拖到某个机器人首次被用到");
    }

    /// <summary>配置节重载：验证源生成绑定能覆盖「字典 of 复杂类型 + 嵌套字典」形态。</summary>
    [Fact]
    public void AddWechatFinanceSdk_ShouldBindConfigurationSection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WechatFinance:DefaultTimeoutSeconds"] = "15",
                ["WechatFinance:Robots:archive-1:CorpId"] = "ww1234567890abcdef",
                ["WechatFinance:Robots:archive-1:SecretSecretName"] = SecretName,
                ["WechatFinance:Robots:archive-1:ProxyAddress"] = "http://127.0.0.1:8080",
                ["WechatFinance:Robots:archive-1:PrivateKeySecretNames:1"] = PrivateKeySecretName,
                ["WechatFinance:Robots:archive-1:PrivateKeySecretNames:2"] = "finance:archive:pk:2",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(new Dictionary<string, string?>
        {
            [SecretName] = "archive-secret-value",
        }));

        services.AddWechatFinanceSdk(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<WechatFinanceOptions>>().Value;

        options.DefaultTimeoutSeconds.Should().Be(15);
        var robot = options.Robots["archive-1"];
        robot.CorpId.Should().Be("ww1234567890abcdef");
        robot.PrivateKeySecretNames.Should().HaveCount(2, "嵌套字典必须由绑定器展开（手工 new 才会漏）");
        robot.PrivateKeySecretNames["2"].Should().Be("finance:archive:pk:2");

        // 解析工厂即跑 Validate —— 能拿到实例就说明绑定后的形状合法。
        using var factory = provider.GetRequiredService<IWechatWorkFinanceClientFactory>();
        factory.Should().NotBeNull();
    }

    [Fact]
    public async Task GetClientAsync_ShouldThrowUnknownRobot_WhenKeyNotConfigured()
    {
        using var factory = NewFactory();

        var act = () => factory.GetClientAsync("archive-404");

        var ex = await act.Should().ThrowAsync<InvalidOperationException>();
        ex.Which.Message.Should().Contain("archive-404").And.Contain("archive-1",
            "报错要点名缺失的键并列出已配置的键（否则宿主只能靠猜）");
    }

    /// <summary>
    /// 密钥解析必须早于 <c>NewSdk</c>：查无 secret 时不应已经分配了一份原生 SDK 实例。
    /// </summary>
    [Fact]
    public async Task GetClientAsync_ShouldThrowBeforeNativeInit_WhenSecretMissing()
    {
        using var factory = NewFactory(new InMemorySecretProvider(new Dictionary<string, string?>()));

        var act = () => factory.GetClientAsync("archive-1");

        var ex = await act.Should().ThrowAsync<InvalidOperationException>();
        ex.Which.Message.Should().Contain("查无值").And.Contain("archive-1");
        ex.Which.Message.Should().NotContain("NewSdk", "失败点应在密钥解析，而不是撞进原生装载错误");
    }

    [Fact]
    public async Task GetClientAsync_ShouldThrowObjectDisposed_WhenFactoryDisposed()
    {
        var factory = NewFactory();
        factory.Dispose();

        var act = () => factory.GetClientAsync("archive-1");

        await act.Should().ThrowAsync<ObjectDisposedException>();
    }

    /// <summary>
    /// 原生库缺失时的报错形态（点名探测目标 + 修复方向）。
    /// </summary>
    /// <remarks>
    /// 门控用例：若宿主环境真的把库放到了探测路径上（本机开发常见），本用例改为断言「不再抛装载错误」，
    /// 避免把「环境缺库」当成契约。两种形态都<b>点名可见</b>，不写空断言。
    /// </remarks>
    [Fact]
    public async Task GetClientAsync_ShouldNameProbeCandidates_WhenNativeLibraryIsAbsent()
    {
        var libraryPresent = File.Exists(Path.Combine(AppContext.BaseDirectory, "WeWorkFinanceSdk.dll"))
                             || File.Exists(Path.Combine(AppContext.BaseDirectory, "libWeWorkFinanceSdk_C.so"));

        using var factory = NewFactory(new InMemorySecretProvider(new Dictionary<string, string?>
        {
            [SecretName] = "archive-secret-value",
        }));

        if (libraryPresent)
        {
            // 有库时不做断言（真机 Init 属集成场景），但必须显式说明，否则该用例是假绿。
            return;
        }

        var act = () => factory.GetClientAsync("archive-1");

        var ex = await act.Should().ThrowAsync<InvalidOperationException>();
        ex.Which.Message.Should().Contain("无法装载原生库")
            .And.Contain("libWeWorkFinanceSdk_C.so", "Linux 官方文件名带 _C 段，必须出现在探测候选里")
            .And.Contain("NativeLibraryPath");
        ex.Which.Message.Should().NotContain("archive-secret-value");
    }

    private static void Configure(WechatFinanceOptions options)
    {
        options.Robots["archive-1"] = NewRobot();
    }

    private static WechatFinanceOptions NewOptions()
    {
        var options = new WechatFinanceOptions();
        Configure(options);
        return options;
    }

    private static WechatFinanceRobotOptions NewRobot()
    {
        var robot = new WechatFinanceRobotOptions
        {
            CorpId = "ww1234567890abcdef",
            SecretSecretName = SecretName,
        };
        robot.PrivateKeySecretNames["1"] = PrivateKeySecretName;
        return robot;
    }

    private static WechatFinanceClientFactory NewFactory(ISecretProvider? secrets = null)
    {
        var provider = secrets ?? new InMemorySecretProvider(new Dictionary<string, string?>
        {
            [SecretName] = "archive-secret-value",
        });

        return new WechatFinanceClientFactory(
            Microsoft.Extensions.Options.Options.Create(NewOptions()), provider);
    }
}
