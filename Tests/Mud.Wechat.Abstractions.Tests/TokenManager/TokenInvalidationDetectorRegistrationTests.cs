// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Text;

namespace Mud.Wechat.Abstractions.Tests.TokenManager;

/// <summary>
/// 判定器登记点语义锁定：公用层后置配置把各产品线子判定器<b>追加</b>进
/// <see cref="TokenRecoveryOptions.AdditionalTokenInvalidationDetectors"/>（与单槽属性为并集语义），
/// 而非包装成组合器写入单槽。并集短路、判定链异常降级（不外抛）、取消异常原样传播由组件判定链路承载。
/// </summary>
public class TokenInvalidationDetectorRegistrationTests
{
    private sealed class FakeDetector : ITokenInvalidationDetector
    {
        private readonly bool _shouldInspect;
        private readonly string _hostFragment;
        private readonly int _errcode;

        public FakeDetector(string hostFragment, int errcode, bool shouldInspect = true)
        {
            _hostFragment = hostFragment;
            _errcode = errcode;
            _shouldInspect = shouldInspect;
        }

        public int InspectCalls { get; private set; }

        public bool ShouldInspect(HttpRequestMessage request)
        {
            if (!_shouldInspect)
            {
                return false;
            }

            return request.RequestUri != null
                   && request.RequestUri.Host.Contains(_hostFragment, StringComparison.OrdinalIgnoreCase);
        }

        public ValueTask<bool> IsTokenInvalidAsync(
            HttpResponseMessage response, ReadOnlyMemory<byte>? body, CancellationToken cancellationToken)
        {
            InspectCalls++;
            if (body is not { Length: > 0 } captured)
            {
                return new ValueTask<bool>(false);
            }

            var text = Encoding.UTF8.GetString(captured.ToArray());
            return new ValueTask<bool>(text.Contains($"\"errcode\":{_errcode}", StringComparison.Ordinal));
        }
    }

    /// <summary>恒抛异常的判定器（故障隔离用）。</summary>
    private sealed class ThrowingDetector : ITokenInvalidationDetector
    {
        public bool ShouldInspect(HttpRequestMessage request) => true;

        public ValueTask<bool> IsTokenInvalidAsync(
            HttpResponseMessage response, ReadOnlyMemory<byte>? body, CancellationToken cancellationToken)
            => throw new InvalidOperationException("子判定器故障");
    }

    /// <summary>恒抛取消的判定器（取消传播用）。</summary>
    private sealed class CancellingDetector : ITokenInvalidationDetector
    {
        public bool ShouldInspect(HttpRequestMessage request) => true;

        public ValueTask<bool> IsTokenInvalidAsync(
            HttpResponseMessage response, ReadOnlyMemory<byte>? body, CancellationToken cancellationToken)
            => throw new OperationCanceledException(cancellationToken);
    }

    private sealed class FakeOptionsMonitor : IOptionsMonitor<TokenRecoveryOptions>
    {
        public TokenRecoveryOptions CurrentValue { get; init; } = new();

        public TokenRecoveryOptions Get(string? name) => CurrentValue;

        public IDisposable? OnChange(Action<TokenRecoveryOptions, string?> listener) => null;
    }

    private static TokenRecoveryOptions BuildOptions(params ITokenInvalidationDetector[] detectors)
    {
        var services = new ServiceCollection();
        services.AddWechatTokenRecovery();
        foreach (var detector in detectors)
        {
            services.AddSingleton(detector);
        }

        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value;
    }

    private static HttpResponseMessage Json(string body) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(body, Encoding.UTF8, "application/json"),
    };

    /// <summary>以组件执行器跑一次请求，返回实际发送次数（2 = 触发恢复并重试，1 = 未触发）。</summary>
    private static async Task<int> SendAndCountAsync(TokenRecoveryOptions options, string url, string body)
    {
        var managerMock = new Mock<ITokenManager>();
        managerMock.Setup(m => m.GetOrRefreshTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("fresh-token");

        var executor = new TokenRecoveryExecutor(managerMock.Object, new FakeOptionsMonitor { CurrentValue = options });
        var sendCount = 0;

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        // 生成代码在请求属性上携带恢复上下文；无上下文的请求不进入恢复链路（组件既有契约）。
        request.Options.Set(
            new HttpRequestOptionsKey<TokenRecoveryContext>(TokenRecoveryContext.PropertyKey),
            new TokenRecoveryContext
            {
                InjectionMode = TokenInjectionMode.Query,
                QueryParameterName = "access_token",
                TokenManagerKey = "AccessToken",
            });
        using var response = await executor.ExecuteAsync(
            request,
            (req, ct) =>
            {
                Interlocked.Increment(ref sendCount);
                return Task.FromResult(Json(body));
            },
            CancellationToken.None);

        return sendCount;
    }

    /// <summary>未登记任何子判定器 ⇒ 追加集为空、单槽保持 null（组件退化为「仅 401」默认语义）。</summary>
    [Fact]
    public void EmptyDetectorSet_ShouldLeaveBothChannelsEmpty()
    {
        var options = BuildOptions();

        options.AdditionalTokenInvalidationDetectors.Should().BeEmpty(
            "未登记子判定器时不得凭空追加，避免改变组件默认行为");
        options.TokenInvalidationDetector.Should().BeNull("公用层不再占用单槽属性");
    }

    /// <summary>多产品线各自登记子判定器 ⇒ 全部进入追加集（互不覆盖，取代单槽「后者胜」）。</summary>
    [Fact]
    public void RegisteredDetectors_ShouldAllBeAppended()
    {
        var mpDetector = new FakeDetector("api.weixin.qq.com", 40001);
        var workDetector = new FakeDetector("qyapi.weixin.qq.com", 42001);
        var channelsDetector = new FakeDetector("api.weixin.qq.com", 40001);

        var options = BuildOptions(mpDetector, workDetector, channelsDetector);

        options.AdditionalTokenInvalidationDetectors.Should().HaveCount(3,
            "三条产品线的子判定器必须共存，不得互相覆盖");
        options.AdditionalTokenInvalidationDetectors.Should().ContainInOrder(mpDetector, workDetector, channelsDetector);
        options.TokenInvalidationDetector.Should().BeNull("追加集是唯一装配形态，单槽不再被写入");
    }

    /// <summary>重复调用登记入口幂等：后置配置只装配一次，追加集不重复累积。</summary>
    [Fact]
    public void AddWechatTokenRecovery_ShouldBeIdempotent()
    {
        var detector = new FakeDetector("qyapi.weixin.qq.com", 42001);

        var services = new ServiceCollection();
        services.AddWechatTokenRecovery();
        services.AddWechatTokenRecovery();
        services.AddSingleton<ITokenInvalidationDetector>(detector);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value;

        options.AdditionalTokenInvalidationDetectors.Should().ContainSingle(
            "后置配置经 TryAddEnumerable 去重，多次登记只追加一次");
        services.Count(d => d.ServiceType == typeof(IValidateOptions<TokenRecoveryOptions>))
            .Should().Be(1, "选项校验器只登记一次");
    }

    /// <summary>
    /// 追加集按并集求值：任一产品线的失效码命中即触发恢复。
    /// （企微 42001 与公众号 40001 分属不同判定器集合，互不合并语义。）
    /// </summary>
    [Fact]
    public async Task AppendedDetectors_ShouldUnionAcrossProductLines()
    {
        var mpDetector = new FakeDetector("api.weixin.qq.com", 40001);
        var workDetector = new FakeDetector("qyapi.weixin.qq.com", 42001);
        var options = BuildOptions(mpDetector, workDetector);

        (await SendAndCountAsync(options, "https://qyapi.weixin.qq.com/cgi-bin/x", "{\"errcode\":42001}"))
            .Should().Be(2, "企微失效码命中 ⇒ 刷新令牌后重试");

        (await SendAndCountAsync(options, "https://api.weixin.qq.com/cgi-bin/x", "{\"errcode\":40001}"))
            .Should().Be(2, "公众号失效码命中（40001 不属企微集合，只能由 MP 子判定器命中）");

        (await SendAndCountAsync(options, "https://api.weixin.qq.com/cgi-bin/x", "{\"errcode\":0}"))
            .Should().Be(1, "未命中任何产品线失效码集合 ⇒ 不触发恢复");
    }

    /// <summary>域名不匹配的子判定器被跳过（产品线按各自域名过滤，互不干扰）。</summary>
    [Fact]
    public async Task DetectorWithUnmatchedHost_ShouldBeSkipped()
    {
        var workDetector = new FakeDetector("qyapi.weixin.qq.com", 42001);
        var options = BuildOptions(workDetector);

        (await SendAndCountAsync(options, "https://example.com/cgi-bin/x", "{\"errcode\":42001}"))
            .Should().Be(1, "域名不匹配时判定器不参与求值");
        workDetector.InspectCalls.Should().Be(0, "跳过发生在预过滤阶段，不应进入响应体判定");
    }

    /// <summary>单个判定器抛异常 ⇒ 仅该判定器降级为「未失效」，后续判定器继续求值，异常不外抛。</summary>
    /// <remarks>
    /// 粒度说明：组件判定链路为<b>逐判定器</b>隔离（与库内组合器契约一致）——前序判定器故障不阻断
    /// 后续判定器。此处断言发送次数为 2：第一个判定器抛异常降级，第二个健康判定器命中 errcode
    /// ⇒ 触发恢复。若上游回退为整链单 <c>try/catch</c>，本断言会变为 1，需同批更新本断言与方案留痕。
    /// </remarks>
    [Fact]
    public async Task FaultedDetector_ShouldDegradeToNotInvalid_AndNotSurfaceError()
    {
        var healthy = new FakeDetector("api.weixin.qq.com", 40001);
        var options = BuildOptions(new ThrowingDetector(), healthy);

        (await SendAndCountAsync(options, "https://api.weixin.qq.com/cgi-bin/x", "{\"errcode\":40001}"))
            .Should().Be(2, "前序判定器异常只降级该判定器；后续健康判定器命中 ⇒ 仍触发恢复（逐判定器隔离）");
    }

    /// <summary>子判定器抛取消 ⇒ 原样上抛（不被降级吞掉）。</summary>
    [Fact]
    public async Task CancelledDetector_ShouldPropagateOperationCanceledException()
    {
        var options = BuildOptions(new CancellingDetector());

        var act = async () => await SendAndCountAsync(
            options, "https://api.weixin.qq.com/cgi-bin/x", "{\"errcode\":0}");

        await act.Should().ThrowAsync<OperationCanceledException>("取消必须原样传播，不得降级为「未失效」");
    }
}
