// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback.Tests.ContractGuards;

/// <summary>
/// 支付回调包契约守卫（方案 §2.8 / §2.6 的机械化落地：配置面、应答面、依赖边界、装配面）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何这些是「契约」而非「实现细节」</b>：失败文案是**安全属性**（区分原因即成为验签 oracle）、
/// 指纹窗口与时效窗的关系是**抗重放属性**、依赖边界是**产品线隔离属性** —— 三者都不是风格偏好。
/// </para>
/// </remarks>
public class WechatPayCallbackContractGuards
{
    /// <summary>配置面：默认值即安全默认（指纹闸开启、指纹窗 ≥ 时效窗）。</summary>
    [Fact]
    public void Options_ShouldHaveSafeDefaults()
    {
        var options = new WechatPayCallbackOptions();

        options.GlobalRoutePrefix.Should().Be("pay", "与企微 wechat / 公众号 mp 前缀不冲突");
        options.RequireReplayGuard.Should().BeTrue("指纹闸默认必须开启（生产不得关闭）");
        options.AllowClockSkewSeconds.Should().Be(300);
        options.ReplayWindowSeconds.Should().BeGreaterThanOrEqualTo(options.AllowClockSkewSeconds,
            "指纹窗短于时效窗时，重放可穿过指纹闸");
        options.EventHandlingTimeoutMs.Should().BeLessThan(5000, "平台 5 秒断连契约不可协商");

        options.Validate();
    }

    /// <summary>配置面 fail-fast：非法取值在启动期即抛，不潜伏到首条通知。</summary>
    [Theory]
    [InlineData("pay/extra", 600, 4000)]   // 前缀含 '/'
    [InlineData("", 600, 4000)]            // 空前缀
    [InlineData("pay", 100, 4000)]         // 指纹窗 < 时效窗
    [InlineData("pay", 600, 5000)]         // 软超时 ≥ 5000ms
    [InlineData("pay", 600, 0)]            // 软超时非正
    public void Options_ShouldFailFast_WhenInvalid(string prefix, int replayWindow, int timeoutMs)
    {
        var options = new WechatPayCallbackOptions
        {
            GlobalRoutePrefix = prefix,
            ReplayWindowSeconds = replayWindow,
            EventHandlingTimeoutMs = timeoutMs,
        };

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>应答面：成功体阻止重推、失败体固定且不含任何请求细节（防探测 oracle）。</summary>
    [Fact]
    public void Responses_ShouldBeFixedAndNonLeaky()
    {
        WechatPayCallbackMiddleware.SuccessResponseBody.Should().Be("{\"code\":\"SUCCESS\"}");
        WechatPayCallbackMiddleware.ResponseContentType.Should().Be("application/json");

        var failure = WechatPayCallbackMiddleware.FailureResponseBody;
        failure.Should().Contain("FAIL");
        failure.Should().NotContain("SUCCESS");
        failure.Should().NotContain("签名", "失败文案不得区分失败原因");
        failure.Should().NotContain("ciphertext");
        failure.Should().NotContain("serial");
    }

    /// <summary>待决项裁决落地：公钥模式前缀常量与官方一致（识别而非落入「未知序列号」）。</summary>
    [Fact]
    public void PublicKeyModePrefix_ShouldMatchOfficial()
    {
        WechatPayCallbackReceiver.PublicKeyIdPrefix.Should().Be("PUB_KEY_ID_");
    }

    /// <summary>
    /// 依赖边界：回调包<b>不得</b>引用主包 <c>Mud.Wechat.Pay</c>（否则违背 <c>Callback → {Abstractions, DataModels}</c> 单向约束）。
    /// </summary>
    [Fact]
    public void ProjectReferences_ShouldNotReferenceMainPayPackage()
    {
        var asm = typeof(WechatPayCallbackMiddleware).Assembly;
        asm.GetReferencedAssemblies()
            .Select(static a => a.Name)
            .Should().NotContain("Mud.Wechat.Pay",
                "Pay.Callback 必须只依赖 Pay.Abstractions + Pay.DataModels —— 不得把业务接口面拖进回调包");
    }

    /// <summary>装配面：未先装凭据底座时必须在<b>注册期</b>点名 fail-fast。</summary>
    [Fact]
    public void AddWechatPayCallback_ShouldFailFast_WhenAddPayAppMissing()
    {
        var services = new ServiceCollection();

        var act = () => services.AddWechatPayCallback(_ => { });

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddPayApp*");
    }

    /// <summary>装配面：仅装回调的宿主（不调 AddWechatPayApi）可解析接收器 / 分发器 / 抗重放默认实现。</summary>
    [Fact]
    public void AddWechatPayCallback_ShouldRegisterReceiverAndDispatcher_WithoutBusinessApis()
    {
        using var fixture = new WechatPayCallbackTestFixture();

        var services = new ServiceCollection();
        services.AddSingleton<IWechatPayMerchantManager>(fixture.MerchantManager);
        services.AddSingleton<IWechatPaySignatureProviderFactory>(fixture.SignatureProviders);
        services.AddSingleton<IWechatPayMerchantCredentialProvider>(fixture.Credentials);

        var builder = services.AddWechatPayCallback(_ => { });
        builder.AddHandler<RecordingHandler>("TRANSACTION.SUCCESS");

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        });

        provider.GetRequiredService<WechatPayCallbackReceiver>().Should().NotBeNull();
        provider.GetRequiredService<WechatPayCallbackDispatcher>().Should().NotBeNull();
        provider.GetRequiredService<IWechatCallbackReplayGuard>()
            .Should().BeOfType<InMemoryWechatCallbackReplayGuard>("默认进程内实现；多实例由宿主前置 TryAdd 覆盖");
    }

    /// <summary>处理器注册表：精确桶在前、通配桶在后（两者可叠加）。</summary>
    [Fact]
    public void HandlerRegistry_ShouldResolveExactBeforeWildcard()
    {
        WechatPayCallbackHandlerRegistry.WildcardEventType.Should().Be("*");

        var registry = new WechatPayCallbackHandlerRegistry();
        registry.Register("TRANSACTION.SUCCESS", typeof(RecordingHandler));
        registry.Register(WechatPayCallbackHandlerRegistry.WildcardEventType, typeof(OtherHandler));

        registry.Resolve("TRANSACTION.SUCCESS").Should().Equal(typeof(RecordingHandler), typeof(OtherHandler));
        registry.Resolve("REFUND.SUCCESS").Should().Equal(typeof(OtherHandler));
        registry.Resolve(null).Should().Equal(typeof(OtherHandler));
    }

    /// <summary>处理器注册：空白事件类型即编程错误。</summary>
    [Fact]
    public void HandlerRegistry_ShouldRejectBlankEventType()
    {
        var registry = new WechatPayCallbackHandlerRegistry();

        var act = () => registry.Register("  ", typeof(RecordingHandler));

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>空实现处理器（仅用于注册表断言）。</summary>
    private sealed class RecordingHandler : IWechatPayNotificationHandler
    {
        /// <inheritdoc />
        public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    /// <summary>另一个空实现处理器（仅用于注册表断言）。</summary>
    private sealed class OtherHandler : IWechatPayNotificationHandler
    {
        /// <inheritdoc />
        public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
