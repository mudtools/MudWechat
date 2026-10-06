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
/// 令牌失效判定器组合器语义（跨产品线共存的架构基石）。
/// </summary>
public class WechatCompositeTokenInvalidationDetectorTests
{
    private sealed class FakeDetector : ITokenInvalidationDetector
    {
        private readonly bool _shouldInspect;
        private readonly bool _invalid;
        private readonly string _hostFragment;
        private readonly int _errcode;

        public FakeDetector(string hostFragment, int errcode, bool shouldInspect = true)
        {
            _hostFragment = hostFragment;
            _errcode = errcode;
            _shouldInspect = shouldInspect;
            _invalid = true;
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

    private static HttpResponseMessage Response() => new(HttpStatusCode.OK)
    {
        Content = new ByteArrayContent(Array.Empty<byte>()),
    };

    private static HttpRequestMessage Request(string host) => new(HttpMethod.Get, "https://" + host + "/cgi-bin/x");

    private static ReadOnlyMemory<byte>? Body(string json) => Encoding.UTF8.GetBytes(json);

    /// <summary>
    /// CD1：`ShouldInspect` 任一放行即放行（短路）——产品线按各自域名过滤，互不干扰。
    /// </summary>
    [Fact]
    public void ShouldInspect_ShouldShortCircuitOnAnyDetector()
    {
        var composite = new WechatCompositeTokenInvalidationDetector(new ITokenInvalidationDetector[]
        {
            new FakeDetector("qyapi.weixin.qq.com", 42001),
            new FakeDetector("api.weixin.qq.com", 40001),
        });

        composite.Count.Should().Be(2);
        composite.ShouldInspect(Request("qyapi.weixin.qq.com")).Should().BeTrue();
        composite.ShouldInspect(Request("api.weixin.qq.com")).Should().BeTrue();
        composite.ShouldInspect(Request("example.com")).Should().BeFalse();
    }

    /// <summary>
    /// CD2：`IsTokenInvalidAsync` 任一判真即判真（各产品线失效码集合独立，不合并语义）。
    /// </summary>
    [Fact]
    public async Task IsTokenInvalidAsync_ShouldReturnTrueIfAnyDetectorMatches()
    {
        var composite = new WechatCompositeTokenInvalidationDetector(new ITokenInvalidationDetector[]
        {
            new FakeDetector("qyapi.weixin.qq.com", 42001),
            new FakeDetector("api.weixin.qq.com", 40001),
        });

        using var response = Response();

        (await composite.IsTokenInvalidAsync(response, Body("{\"errcode\":42001}"), CancellationToken.None))
            .Should().BeTrue("企微失效码命中");
        (await composite.IsTokenInvalidAsync(response, Body("{\"errcode\":40001}"), CancellationToken.None))
            .Should().BeTrue("公众号失效码命中（40001 不属于企微集合，只能由 MP 子判定器命中）");
        (await composite.IsTokenInvalidAsync(response, Body("{\"errcode\":0}"), CancellationToken.None))
            .Should().BeFalse("未命中任何产品线失效码集合");
    }

    /// <summary>CD3：空集合不构造组合器（由登记点保证选项保持 null ⇒ 组件默认「仅 401」语义不变）。</summary>
    [Fact]
    public void EmptyDetectorSet_ShouldNotInstallComposite()
    {
        var services = new ServiceCollection();
        services.AddWechatTokenRecovery();

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value.TokenInvalidationDetector
            .Should().BeNull("未登记任何子判定器时必须保持 null，避免改变组件默认行为");
    }

    /// <summary>CD4：登记子判定器后，组合器由公用层 PostConfigure 装配（产品线不直接写选项属性）。</summary>
    [Fact]
    public void RegisteredDetectors_ShouldBeComposedByPostConfigure()
    {
        var services = new ServiceCollection();
        services.AddWechatTokenRecovery();
        services.AddSingleton<ITokenInvalidationDetector>(new FakeDetector("api.weixin.qq.com", 40001));
        services.AddSingleton<ITokenInvalidationDetector>(new FakeDetector("qyapi.weixin.qq.com", 42001));

        using var provider = services.BuildServiceProvider();
        var detector = provider.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value.TokenInvalidationDetector;

        detector.Should().BeOfType<WechatCompositeTokenInvalidationDetector>();
        ((WechatCompositeTokenInvalidationDetector)detector!).Count.Should().Be(2);
    }

    /// <summary>CD5：重复调用登记入口幂等（多产品线各调一次不会重复装配 PostConfigure）。</summary>
    [Fact]
    public void AddWechatTokenRecovery_ShouldBeIdempotent()
    {
        var services = new ServiceCollection();
        services.AddWechatTokenRecovery();
        services.AddWechatTokenRecovery();

        using var provider = services.BuildServiceProvider();
        var detector = provider.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value.TokenInvalidationDetector;
        detector.Should().BeNull("无子判定器时仍为 null（幂等：PostConfigure 被 TryAddEnumerable 去重）");

        services.Count(d => d.ServiceType == typeof(IValidateOptions<TokenRecoveryOptions>))
            .Should().Be(1, "选项校验器只登记一次");
    }
}
