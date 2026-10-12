// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 支付回调分发器用例（v1 方案 F7/F8，PAY-D1~D3）：并发闸（容量观测）、未注册处理器的归因、
/// 软超时夹取；以及配置面 <c>MaxConcurrentEvents</c> 的启动期校验。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不用 Mock 处理器</b>：并发闸的性质必须由「真实并行 + 真实观测」表达 —— 两个并发
/// <c>DispatchAsync</c> + 处理器内计数，直接度量观测到的最大并行度；Mock 只能证明「被调用」。
/// </para>
/// <para>
/// 通知上下文经<b>真实接收器</b>（三闸全真）产出；两条通知的 <c>resource</c> 明文不同 ⇒ 密文不同
/// ⇒ 指纹不同，互不触发重放闸。
/// </para>
/// </remarks>
public class WechatPayCallbackDispatcherTests
{
    /// <summary>并发探针处理器：度量观测到的最大并行度。</summary>
    private sealed class ConcurrencyProbeHandler : IWechatPayNotificationHandler
    {
        private readonly object _gate = new();
        private readonly int _holdMs;

        /// <summary>创建探针。</summary>
        /// <param name="holdMs">处理器内保持时长（模拟业务耗时）。</param>
        public ConcurrencyProbeHandler(int holdMs) => _holdMs = holdMs;

        /// <summary>当前在处理器内的并发数。</summary>
        public int Current { get; private set; }

        /// <summary>观测到的最大并发数。</summary>
        public int Max { get; private set; }

        /// <inheritdoc />
        public async Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
        {
            lock (_gate)
            {
                Current++;
                if (Current > Max)
                {
                    Max = Current;
                }
            }

            try
            {
                await Task.Delay(_holdMs, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                lock (_gate) { Current--; }
            }
        }
    }

    /// <summary>慢处理器（软超时用例；在 DI 注册）。</summary>
    private sealed class SlowHandler : IWechatPayNotificationHandler
    {
        /// <inheritdoc />
        public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
            => Task.Delay(500, cancellationToken);
    }

    /// <summary>「注册表登记了、DI 未注册」的处理器类型（F8 归因用例）。</summary>
    private sealed class UnregisteredHandler : IWechatPayNotificationHandler
    {
        /// <inheritdoc />
        public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    // ---------------------------------------------------------------- PAY-D1：并发闸

    [Fact]
    public async Task Dispatch_ShouldSerializeHandlers_WhenMaxConcurrentEventsIsOne()
    {
        // F7 回归（先红后绿）：容量 1 ⇒ 两条并发通知必须串行；旧实现无闸，观测并行度 = 2。
        using var fixture = new WechatPayCallbackTestFixture();
        fixture.Options.MaxConcurrentEvents = 1;
        var probe = new ConcurrencyProbeHandler(holdMs: 150);
        var dispatcher = BuildDispatcher(fixture, probe);

        var contexts = await ReceiveTwoNotificationsAsync(fixture);

        await Task.WhenAll(
            dispatcher.DispatchAsync(contexts[0]),
            dispatcher.DispatchAsync(contexts[1]));

        probe.Max.Should().Be(1, "MaxConcurrentEvents=1 时并发闸必须把并行度压到 1（防 scoped DbContext 并发击穿）");
    }

    [Fact]
    public async Task Dispatch_ShouldAllowParallelism_WhenCapacityAboveOne()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        fixture.Options.MaxConcurrentEvents = 2;
        var probe = new ConcurrencyProbeHandler(holdMs: 150);
        var dispatcher = BuildDispatcher(fixture, probe);

        var contexts = await ReceiveTwoNotificationsAsync(fixture);

        await Task.WhenAll(
            dispatcher.DispatchAsync(contexts[0]),
            dispatcher.DispatchAsync(contexts[1]));

        probe.Max.Should().Be(2, "容量 2 时允许两条通知并行（闸不得退化为全串行）");
    }

    // ---------------------------------------------------------------- PAY-D2：未注册处理器归因

    [Fact]
    public async Task Dispatch_ShouldSkipUnregisteredHandler_WithAccurateAttribution()
    {
        // F8：DI 漏注册是配置问题，不得误入「处理器执行失败」（Error）归因。
        using var fixture = new WechatPayCallbackTestFixture();
        var logger = new CapturingLogger<WechatPayCallbackDispatcher>();

        var services = new ServiceCollection();
        services.AddLogging();
        var provider = services.BuildServiceProvider();

        var registry = new WechatPayCallbackHandlerRegistry();
        registry.Register("TRANSACTION.SUCCESS", typeof(UnregisteredHandler));

        var dispatcher = new WechatPayCallbackDispatcher(registry, provider, fixture.OptionsMonitor, logger);
        var context = await ReceiveOneNotificationAsync(fixture, outTradeNo: "ORDER-UNREG-1");

        var outcome = await dispatcher.DispatchAsync(context);

        outcome.Should().Be(WechatPayCallbackDispatchOutcome.Handled, "跳过未注册处理器后按已处理收敛");
        logger.Entries.Should().Contain(e => e.Level == LogLevel.Warning && e.Message.Contains("未在 DI 注册"),
            "必须给出可定位的配置告警（AddWechatPayCallback().AddHandler<> 未覆盖该事件类型）");
        logger.Entries.Should().NotContain(e => e.Level == LogLevel.Error,
            "DI 漏注册不是业务故障，不得记 Error（旧实现经 GetRequiredService 抛错误入该归因）");
    }

    // ---------------------------------------------------------------- PAY-D3：软超时夹取

    [Fact]
    public async Task Dispatch_ShouldTimeOut_WhenHandlerExceedsWindow()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        fixture.Options.EventHandlingTimeoutMs = 1;   // 极端值：不得 ArgumentOutOfRangeException
        var dispatcher = BuildDispatcher(fixture, new SlowHandler());

        var context = await ReceiveOneNotificationAsync(fixture, outTradeNo: "ORDER-SLOW-1");

        var outcome = await dispatcher.DispatchAsync(context);

        outcome.Should().Be(WechatPayCallbackDispatchOutcome.TimedOut, "软超时收敛为 TimedOut（旧实现 CTS(0) 会直接抛）");
    }

    // ---------------------------------------------------------------- 配置面校验

    [Fact]
    public void Validate_ShouldRejectNonPositiveMaxConcurrentEvents()
    {
        var options = new WechatPayCallbackOptions { MaxConcurrentEvents = 0 };

        Action act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*MaxConcurrentEvents*");
    }

    [Fact]
    public void Validate_ShouldRejectNegativeTimeout()
    {
        var options = new WechatPayCallbackOptions { EventHandlingTimeoutMs = -1 };

        Action act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*EventHandlingTimeoutMs*");
    }

    // ---------------------------------------------------------------- 测试基建

    private static WechatPayCallbackDispatcher BuildDispatcher(
        WechatPayCallbackTestFixture fixture, IWechatPayNotificationHandler handler)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        // 按具体类型注册（与生产 AddWechatPayCallback().AddHandler&lt;THandler&gt; 同形态）——
        // 按接口静态类型注册会让 GetService(handlerType) 解析不到实例。
        services.AddSingleton(handler.GetType(), handler);
        var provider = services.BuildServiceProvider();

        var registry = new WechatPayCallbackHandlerRegistry();
        registry.Register("TRANSACTION.SUCCESS", handler.GetType());

        return new WechatPayCallbackDispatcher(
            registry, provider, fixture.OptionsMonitor, NullLogger<WechatPayCallbackDispatcher>.Instance);
    }

    private static async Task<WechatPayCallbackContext> ReceiveOneNotificationAsync(
        WechatPayCallbackTestFixture fixture, string outTradeNo)
    {
        var (headers, body) = fixture.CreateNotification(
            TransactionResource(outTradeNo),
            eventType: "TRANSACTION.SUCCESS",
            originalType: "transaction");

        return await fixture.CreateReceiver().ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);
    }

    private static async Task<WechatPayCallbackContext[]> ReceiveTwoNotificationsAsync(WechatPayCallbackTestFixture fixture)
    {
        var receiver = fixture.CreateReceiver();

        var (h1, b1) = fixture.CreateNotification(
            TransactionResource("ORDER-PAR-1"), eventType: "TRANSACTION.SUCCESS", originalType: "transaction");
        var (h2, b2) = fixture.CreateNotification(
            TransactionResource("ORDER-PAR-2"), eventType: "TRANSACTION.SUCCESS", originalType: "transaction");

        // 明文不同 ⇒ 密文不同 ⇒ 指纹不同（指纹 = 密文 SHA1），互不触发重放闸。
        return new[]
        {
            await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, h1, b1),
            await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, h2, b2),
        };
    }

    private static string TransactionResource(string outTradeNo)
        => "{\"mchid\":\"1900000000\",\"appid\":\"wx-test\",\"out_trade_no\":\"" + outTradeNo + "\"," +
           "\"transaction_id\":\"4200001234\",\"trade_type\":\"JSAPI\",\"trade_state\":\"SUCCESS\"," +
           "\"trade_state_desc\":\"支付成功\",\"bank_type\":\"OTHERS\",\"attach\":\"\"," +
           "\"success_time\":\"2026-10-09T12:00:00+08:00\",\"payer\":{\"openid\":\"o-test\"}," +
           "\"amount\":{\"total\":100,\"payer_total\":100,\"currency\":\"CNY\",\"payer_currency\":\"CNY\"}}";

    /// <summary>记录日志条目的最小 <see cref="ILogger{T}"/> 实现（归因断言用）。</summary>
    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }
}
