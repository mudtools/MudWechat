// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.Tests.ContractGuards;
using Mud.Wechat.Pay.Tests.Credential;

namespace Mud.Wechat.Pay.Tests.Transport;

/// <summary>
/// 纯支付宿主的 SSRF 启动诊断回归锁：<c>AddPayApp</c> 已登记白名单 ⇒ 组件不会发
/// EventId 198（<c>SsrfStrictModeWithoutAllowlist</c>）。
/// </summary>
/// <remarks>
/// 198 的成立条件是「严格模式客户端 + 全局空白名单」。本用例把白名单清空后建纯支付宿主，
/// 再解析支付客户端（组件的启动检查在客户端创建路径上强制触发）——若 <c>AddPayApp</c>
/// 漏登记白名单，宿主会在**每一笔请求**被连接期 SSRF 拦下的同时收到该 Warning。
/// </remarks>
public class WechatPaySsrfStartupDiagnosticsTests
{
    private const string MerchantKeyId = "pay:merchant:1900000000:key";
    private const string ApiKeyId = "pay:merchant:1900000000:apikey";

    /// <summary>记录全部日志事件的捕获器（按 EventId 断言，不解析消息文本）。</summary>
    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        private readonly List<EventId> _events;

        public CapturingLoggerProvider(List<EventId> events) => _events = events;

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(_events);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger : ILogger
        {
            private readonly List<EventId> _events;

            public CapturingLogger(List<EventId> events) => _events = events;

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                lock (_events)
                {
                    _events.Add(eventId);
                }
            }
        }
    }

    [Fact]
    public async Task PurePayHost_ShouldNotEmitSsrfStrictModeWithoutAllowlist()
    {
        var backup = UrlValidator.GetAllowedDomains().ToArray();
        var events = new List<EventId>();

        try
        {
            // 模拟「未登记任何白名单」的宿主初态（全局静态，结束时还原）。
            UrlValidator.ConfigureAllowedDomains(Array.Empty<string>());

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddProvider(new CapturingLoggerProvider(events)));
            services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
                new Dictionary<string, string>
                {
                    [MerchantKeyId] = WechatPayGoldenVectors.MerchantPrivateKeyPem,
                    [ApiKeyId] = new string('k', 32),
                }));
            services.AddPayApp(new List<WechatPayMerchantConfig>
            {
                new()
                {
                    MchId = "1900000000",
                    SerialNumber = WechatPayGoldenVectors.MerchantSerialNumber,
                    PrivateKeySecretName = MerchantKeyId,
                    ApiKeySecretName = ApiKeyId,
                },
            });
            services.AddHttpClient(WechatPayHttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => new StubHandler());

            using var provider = services.BuildServiceProvider();
            var client = provider.GetRequiredService<IWechatPayHttpClient>();

            // 真正发一笔（客户端实例在请求路径上创建；198 的启动检查在创建时触发）。
            using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/certificates");
            using var response = await client.SendRawAsync(request);
            response.IsSuccessStatusCode.Should().BeTrue();

            events.Should().NotContain(
                e => e.Id == 198,
                "AddPayApp 已登记 SSRF 白名单，纯支付宿主不得出现「严格模式 + 空白名单」启动告警");
        }
        finally
        {
            UrlValidator.ConfigureAllowedDomains(backup);
        }
    }

    /// <summary>只回 200 的桩 handler（不碰外网）。</summary>
    private sealed class StubHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
    }
}
