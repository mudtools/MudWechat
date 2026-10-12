// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// WechatContractGuards 的 partial 分文件（F2 守卫拆分，审查报告 v1）：断言方法逐字搬移、
/// 类名与测试 FQN 不变 —— 本文件承载 AuthorizationEndpoints_ShouldMatchOfficialRoutes / CancelAuthCleanup_ShouldBeScopedToMatchedAppKeys。
/// </summary>
public partial class WechatContractGuards
{
    /// <summary>
    /// 契约守卫 G6（R14）：授权流端点路由表（新增 3 条，与 §4.5 接口声明一致），
    /// 并显式断言新增接口以 <c>[Query]</c> 显式传令牌、不携带 <c>[Token]</c>，
    /// 即 G5（Query 令牌注入白名单）<b>未放宽</b>。
    /// </summary>
    [Fact]
    public void AuthorizationEndpoints_ShouldMatchOfficialRoutes()
    {
        var expected = new (Type Interface, string Method, string Route)[]
        {
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetPermanentCodeV2Async),
                "/cgi-bin/service/v2/get_permanent_code"),
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetAuthInfoV2Async),
                "/cgi-bin/service/v2/get_auth_info"),
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetAppQrcodeAsync),
                "/cgi-bin/service/get_app_qrcode"),
            (typeof(IWechatWorkProviderAuthenticationUrl),
                nameof(IWechatWorkProviderAuthenticationUrl.GetCustomizedAuthUrlAsync),
                "/cgi-bin/service/get_customized_auth_url"),
        };

        foreach (var (iface, method, route) in expected)
        {
            var target = iface.GetMethod(method);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var post = target!.GetCustomAttribute<Mud.HttpUtils.Attributes.PostAttribute>();
            post.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [Post] 路由");
            post!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // R14：get_customized_auth_url 显式传 provider_access_token，不得进入 [Token] Query 注入白名单。
        typeof(IWechatWorkProviderAuthenticationUrl)
            .GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>()
            .Should().BeNull("G5 白名单仅 IWechatWorkProviderAuthenticationService，新增接口不得放宽");

        var providerTokenParam = typeof(IWechatWorkProviderAuthenticationUrl)
            .GetMethod(nameof(IWechatWorkProviderAuthenticationUrl.GetCustomizedAuthUrlAsync))!
            .GetParameters()[0];
        providerTokenParam.GetCustomAttribute<Mud.HttpUtils.Attributes.QueryAttribute>()!
            .Name.Should().Be("provider_access_token", "服务商令牌必须以显式 Query 参数传入");
    }

    /// <summary>
    /// 契约守卫 G9（P0-3）：<c>cancel_auth</c> 的清理范围必须收敛到 SuiteId 命中集，
    /// 不得再次引入「未命中即回退全部应用」的越权删除（行为用例见
    /// <c>WechatCallbackAuthorizationDispatchTests</c>）。
    /// </summary>
    [Fact]
    public void CancelAuthCleanup_ShouldBeScopedToMatchedAppKeys()
    {
        var handlerPath = SourcePath(
            "Mud.Wechat.Work.Callback", "WechatCallbackHandler.cs");
        File.Exists(handlerPath).Should().BeTrue($"未找到回调处理器源码：{handlerPath}");

        var source = File.ReadAllText(handlerPath);

        source.Should().NotContain("ResolveAppKeys(",
            "G9：cancel_auth/change_auth 不得再经「未命中即回退全部应用」的 ResolveAppKeys 兜底");
        source.Should().Contain("已跳过授权清理以避免误删其它套件授权",
            "G9：未命中归属应用时必须走「只告警不删库」分支");
        source.Should().Contain("MatchAppKeysBySuiteId",
            "G9：清理范围必须恒为 SuiteId 命中集");
    }

    /// <summary>
    /// 契约守卫 G10（P1-2）：<c>AwaitSharedAsync</c> 的取消信号 <c>TaskCompletionSource</c> 必须携带
    /// <c>RunContinuationsAsynchronously</c>——否则共享刷新完成时等待者续体同步内联到刷新线程。
    /// 该行为**不可由运行时确定性观测**（调度策略非契约），故以源码文本断言为权威锁定。
    /// </summary>
    [Fact]
    public void AwaitSharedAsync_CancellationSignal_ShouldRunContinuationsAsynchronously()
    {
        var source = ReadAuthorizationServiceSource();

        source.Should().Contain(
            "new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously)",
            "G10：取消信号必须异步调度续体（防等待者续体内联刷新线程；此断言为该修复的权威锁定）");
    }

    /// <summary>
    /// 契约守卫 G11（P1-3）：<c>ResolveAppKey</c> 必须「显式即有意、隐式即失败」——
    /// 多应用且未配置 <c>DefaultAppKey</c> 时以 fail-fast 取代静默回退默认应用（防误路由）。
    /// </summary>
    [Fact]
    public void ResolveAppKey_ShouldFailFast_WhenMultiAppWithoutConfiguredDefault()
    {
        var source = ReadAuthorizationServiceSource();

        var start = source.IndexOf("private string ResolveAppKey(string? appKey)", StringComparison.Ordinal);
        start.Should().BePositive("G11：ResolveAppKey 签名漂移须同步更新守卫");

        var end = source.IndexOf("private static void ValidateCustomizedState", StringComparison.Ordinal);
        end.Should().BeGreaterThan(start, "G11：ResolveAppKey 之后应紧跟 ValidateCustomizedState（定位锚点）");

        var body = source.Substring(start, end - start);

        body.Should().Contain("DefaultAppKey",
            "G11：显式配置的 DefaultAppKey 仍为无歧义回退路径（配置属性消费点保留）");
        var countGuardIndex = body.IndexOf("ConfiguredAppKeys.Count", StringComparison.Ordinal);
        var throwIndex = body.IndexOf("throw new InvalidOperationException", StringComparison.Ordinal);

        countGuardIndex.Should().BePositive("G11：必须按已注册应用数判定是否存在歧义");
        throwIndex.Should().BeGreaterThan(countGuardIndex,
            "G11：多应用且无 DefaultAppKey 时必须 fail-fast（不得静默落到默认应用）");
    }

    private static string ReadAuthorizationServiceSource()
    {
        var path = SourcePath("Mud.Wechat.Work", "Services", "Authorization", "WechatWorkAuthorizationService.cs");
        File.Exists(path).Should().BeTrue($"未找到授权服务源码：{path}");
        return File.ReadAllText(path);
    }
}
