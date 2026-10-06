// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions;

namespace Mud.Wechat.Abstractions.Tests;

/// <summary>
/// 跨产品线共存（同一宿主同时引用企业微信与公众号 SDK）：
/// 两处进程级全局态必须保持并集且互不覆盖 —— 这是公用层存在的最硬理由。
/// </summary>
public class CrossProductLineCoexistenceTests
{
    private static ServiceCollection BuildBothProductLines()
    {
        var services = new ServiceCollection();

        services.AddWechatApp(config =>
        {
            config.AppKey = "work-default";
            config.CorpId = "ww-test";
            config.AgentSecret = "work-secret";
            config.IsDefault = true;
        });

        services.AddMpApp(config =>
        {
            config.AppKey = "mp-default";
            config.AppId = "wx-test";
            config.AppSecret = "mp-secret";
            config.IsDefault = true;
        });

        return services;
    }

    /// <summary>
    /// X1：SSRF 白名单为并集且不被后注册的产品线清空。
    /// </summary>
    /// <remarks>
    /// 组件 <c>UrlValidator.ConfigureAllowedDomains</c> 是全局静态 + <b>整体替换</b>语义；
    /// 若两条产品线各自以自身白名单调用，后调用者会清空前者。本用例锁定「公用层单点登记」的收敛结果。
    /// </remarks>
    [Fact]
    public void AllowedDomains_ShouldBeUnionAfterBothProductLinesRegistered()
    {
        _ = BuildBothProductLines();

        var domains = UrlValidator.GetAllowedDomains();
        domains.Should().Contain("weixin.qq.com", "公众号默认域名 api.weixin.qq.com 依赖此项");
        domains.Should().Contain("work.weixin.qq.com", "企业微信默认域名依赖此项");
    }

    /// <summary>
    /// X2：令牌失效判定器为组合器，且同时覆盖两条产品线的失效码集合。
    /// </summary>
    /// <remarks>
    /// 组件选项属性为<b>单槽</b>：若两条产品线各自 PostConfigure，后注册者会覆盖前者 ⇒
    /// 一方令牌恢复静默失效。本用例锁定组合器的存在与「任一判真即真」语义。
    /// </remarks>
    [Fact]
    public void TokenInvalidationDetector_ShouldComposeBothProductLines()
    {
        var services = BuildBothProductLines();
        using var provider = services.BuildServiceProvider();

        var detector = provider.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value.TokenInvalidationDetector;
        detector.Should().BeOfType<WechatCompositeTokenInvalidationDetector>();

        var composite = (WechatCompositeTokenInvalidationDetector)detector!;
        composite.Count.Should().BeGreaterOrEqualTo(2, "企微与公众号各登记一个子判定器");

        // 非同步断言上下文内执行（组合器 API 为 ValueTask，这里同步阻塞以保证用例简洁）。
        using var qyapi = new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Get, "https://qyapi.weixin.qq.com/cgi-bin/gettoken");
        using var api = new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Get, "https://api.weixin.qq.com/cgi-bin/token");

        composite.ShouldInspect(qyapi).Should().BeTrue("企微域名必须被放行");
        composite.ShouldInspect(api).Should().BeTrue("公众号域名必须被放行");
    }

    /// <summary>X3：后台刷新框架服务只登记一次（组件注册非幂等，多产品线各自调用会产生多个刷新循环）。</summary>
    [Fact]
    public void TokenRefreshBackgroundService_ShouldBeRegisteredOnce()
    {
        var services = BuildBothProductLines();

        services.Count(d => d.ServiceType == typeof(ITokenRefreshBackgroundService))
            .Should().Be(1, "多产品线共存时只能有一个后台刷新循环（公用层登记点带「已注册即跳过」守卫）");
    }

    /// <summary>X4：两条产品线的令牌存储端口共用同一实现实例（键前缀含产品线令牌类型，故键空间不交叠）。</summary>
    [Fact]
    public void TokenStore_ShouldBeSharedAcrossProductLines()
    {
        var services = BuildBothProductLines();
        using var provider = services.BuildServiceProvider();

        var store = provider.GetRequiredService<IWechatTokenStore>();
        store.Should().BeOfType<InMemoryWechatTokenStore>();
    }

    /// <summary>X5：两条产品线的核心服务可同时解析（DI 图无冲突）。</summary>
    [Fact]
    public void BothProductLineManagers_ShouldBeResolvable()
    {
        var services = BuildBothProductLines();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<Mud.Wechat.Work.Abstractions.Authentication.IWechatAppManager>()
            .Should().NotBeNull();
        provider.GetRequiredService<MpAppManager>().Should().NotBeNull();
    }
}
