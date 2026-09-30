// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.TokenManagers;

namespace Mud.Wechat.Work.Tests.TokenManagers;

/// <summary>
/// WechatTokenInvalidationDetector 两阶段判定测试（详细设计 §11.2 / §18.2）。
/// </summary>
public class WechatTokenInvalidationDetectorTests
{
    private readonly WechatTokenInvalidationDetector _detector = new();

    private static HttpRequestMessage WechatRequest()
        => new(HttpMethod.Post, "https://qyapi.weixin.qq.com/cgi-bin/service/get_pre_auth_code");

    private static HttpResponseMessage WechatJsonResponse(string body)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };

    [Theory]
    [InlineData(40014, true)]
    [InlineData(42001, true)]
    [InlineData(42007, true)]
    [InlineData(42009, true)]
    [InlineData(42011, true)]
    [InlineData(0, false)]
    [InlineData(40013, false, "非失效码走业务异常，不触发恢复链路")]
    [InlineData(61028, false)]
    public async Task IsTokenInvalidAsync_ShouldMatchErrcodeCollection(int errcode, bool expected, string? because = null)
    {
        var body = Encoding.UTF8.GetBytes($"{{\"errcode\":{errcode},\"errmsg\":\"x\"}}");
        var result = await _detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);
        result.Should().Be(expected, because);
    }

    [Fact]
    public async Task IsTokenInvalidAsync_ShouldReturnFalse_WhenBodyUnreadable()
    {
        // body = null（超限 / chunked / 未声明 Content-Length）→ 回退状态码判定（返回 false）。
        var result = await _detector.IsTokenInvalidAsync(WechatJsonResponse(""), null, CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsTokenInvalidAsync_ShouldReturnFalse_WhenBodyNotJson()
    {
        var body = "<html>gateway error</html>"u8.ToArray();
        var result = await _detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);
        result.Should().BeFalse("非 JSON 响应体不参与判定（异常降级语义）");
    }

    [Fact]
    public void ShouldInspect_ShouldFilterNonWechatHosts()
    {
        _detector.ShouldInspect(WechatRequest()).Should().BeTrue();
        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://open.feishu.cn/open-apis")).Should().BeFalse();
        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://qyapi.weixin.qq.com")).Should().BeTrue();
        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://evil.example.com/u/weixin.qq.com")).Should().BeFalse("仅按 host 判定");
    }

    /// <summary>
    /// P2-9：显式登记的自定义 BaseUrl 主机（<c>AllowCustomBaseUrl = true</c> 的私有化/网关部署）
    /// 也必须参与判定，否则这些部署会静默失去 errcode 令牌恢复能力。
    /// </summary>
    [Fact]
    public void ShouldInspect_ShouldPass_WhenCustomBaseUrlRegistered()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig>
        {
            new()
            {
                AppKey = "private",
                AppType = WechatAppType.Internal,
                CorpId = "ww-corp",
                AgentSecret = "agent-secret",
                BaseUrl = "https://gateway.example.com",
                AllowCustomBaseUrl = true,
            },
        });

        // 自定义主机在 **DI 注册期**（AddWechatApp）登记，无需物化应用上下文
        // （物化会触发组件连接期 SSRF 校验，测试环境无法解析公网域名）。
        using var provider = services.BuildServiceProvider();

        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://gateway.example.com/cgi-bin/gettoken"))
            .Should().BeTrue("注册期登记的自定义主机应放行同步预过滤");

        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://other.example.com/cgi-bin/gettoken"))
            .Should().BeFalse("未登记的第三方主机仍被过滤");
    }
}
