// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work.Tests.Extensions;

/// <summary>
/// 令牌归属域链路测试：证明「应用类型子接口的 <c>[Token(TokenManagerKey=…)]</c> 声明」
/// 确实贯通到请求期并触发 fail-fast（方案 §3 声明层 / 校验层）。
/// </summary>
/// <remarks>
/// <para>
/// <b>测试路径刻意完全离线</b>：<see cref="RecordingTokenProvider"/> 记录生成的声明式客户端
/// 实际传入的查找键，并把键解析交回真实咽喉点 <c>WechatAppContext.GetTokenManager</c>，
/// 但<b>不</b>继续取令牌——因此任何断言路径都不会产生真实网络请求，
/// 同时错配场景仍由真实咽喉点抛错（而非测试桩伪造）。
/// </para>
/// </remarks>
public class WechatTokenOwnerEndToEndTests
{
    private const string DefaultAppKey = "default";

    private static WechatAppConfig InternalConfig() => new()
    {
        AppKey = DefaultAppKey,
        AppType = WechatAppType.Internal,
        CorpId = "ww-corp",
        AgentSecret = "agent-secret",
    };

    private static WechatAppConfig ThirdPartyConfig() => new()
    {
        AppKey = DefaultAppKey,
        AppType = WechatAppType.ThirdParty,
        CorpId = "ww-provider",
        ProviderSecret = "provider-secret",
        SuiteId = "ww-suite",
        SuiteSecret = "suite-secret",
    };

    private static ServiceProvider BuildProvider(WechatAppConfig config, RecordingTokenProvider? tokenProvider = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { config });
        services.AddWechatWorkServices(builder => builder.AddWedocApi());

        if (tokenProvider != null)
        {
            // 后注册胜出：替换框架 DefaultTokenProvider（两者入口签名一致，见 RecordingTokenProvider 注释）。
            services.AddSingleton<Mud.HttpUtils.ITokenProvider>(tokenProvider);
        }

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

    /// <summary>
    /// 令牌提供器探针：记录查找键并复用真实咽喉点的键解析，随后中断（不取令牌、不发请求）。
    /// </summary>
    private sealed class RecordingTokenProvider : Mud.HttpUtils.ITokenProvider
    {
        /// <summary>最近一次请求的令牌参数（反射生成的声明式客户端写入）。</summary>
        public Mud.HttpUtils.TokenRequest? LastRequest { get; private set; }

        /// <inheritdoc />
        public Task<string> GetTokenAsync(
            Mud.HttpUtils.IMudAppContext appContext,
            Mud.HttpUtils.TokenRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;

            // 与框架 DefaultTokenProvider 同一入口：键 → 令牌管理器（归属域闸在此 fail-fast）。
            appContext.GetTokenManager(request.TokenManagerKey);

            // 走到此处说明归属域闸放行——不再取令牌，直接中断以保持测试离线确定性。
            throw new InvalidOperationException(
                $"令牌提供器探针中断：key='{request.TokenManagerKey}'（归属域闸已放行，测试不进入真实令牌获取）。");
        }
    }

    /// <summary>
    /// 咽喉点（自建应用）：归属域键 <c>@Internal</c> 落到自建应用管理器，<c>@Corp</c> 被拒绝。
    /// </summary>
    [Fact]
    public void GetTokenManager_OnInternalApp_ShouldRouteInternalKeyAndRejectCorpKey()
    {
        using var provider = BuildProvider(InternalConfig());
        var context = (WechatAppContext)provider.GetRequiredService<Abstractions.Authentication.IWechatAppContext>();

        context.GetTokenManager(WechatTokenManagerKeys.InternalAccessToken)
            .Should().BeSameAs(context.InternalAppTokenManager,
                "归属域键 @Internal 必须路由到自建应用令牌管理器（corpid + corpsecret）");

        var act = () => context.GetTokenManager(WechatTokenManagerKeys.CorpAccessToken);
        act.Should().Throw<WechatTokenOwnerMismatchException>()
            .WithMessage("*ThirdParty/Provider*", "消息必须说明期望的应用类型")
            .WithMessage("*UseAppScope*", "消息必须给出可执行的修复动作")
            .Which.AppKey.Should().Be(DefaultAppKey);
    }

    /// <summary>
    /// 咽喉点（第三方应用）：归属域键 <c>@Corp</c> 落到授权企业管理器，<c>@Internal</c> 被拒绝。
    /// </summary>
    [Fact]
    public void GetTokenManager_OnThirdPartyApp_ShouldRouteCorpKeyAndRejectInternalKey()
    {
        using var provider = BuildProvider(ThirdPartyConfig());
        var context = (WechatAppContext)provider.GetRequiredService<Abstractions.Authentication.IWechatAppContext>();

        context.GetTokenManager(WechatTokenManagerKeys.CorpAccessToken)
            .Should().BeSameAs(context.CorpTokenManager,
                "归属域键 @Corp 必须路由到授权企业令牌管理器（get_corp_token / gettoken(permanent_code)）");

        var act = () => context.GetTokenManager(WechatTokenManagerKeys.InternalAccessToken);
        act.Should().Throw<WechatTokenOwnerMismatchException>()
            .Which.ActualAppType.Should().Be(WechatAppType.ThirdParty);
    }

    /// <summary>
    /// 兼容性：未声明归属域的旧键语义逐字节不变（公共父接口与宿主直调不受影响）。
    /// </summary>
    [Fact]
    public void GetTokenManager_OnLegacyKeys_ShouldKeepPreExistingRouting()
    {
        using var provider = BuildProvider(InternalConfig());
        var context = (WechatAppContext)provider.GetRequiredService<Abstractions.Authentication.IWechatAppContext>();

        context.GetTokenManager(WechatTokenTypes.AccessToken)
            .Should().BeSameAs(context.InternalAppTokenManager, "旧键按 AppType 路由的既有语义不变");

        var act = () => context.GetTokenManager(WechatTokenTypes.SuiteAccessToken);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*套件令牌管理器*", "自建上下文未装配套件管理器：既有消息形态不变");
    }

    /// <summary>
    /// 端到端（负例）：把第三方应用类型子接口注入到自建应用上——生成的声明式客户端必须
    /// 把接口声明的 <c>@Corp</c> 键贯通到请求期，并由咽喉点 fail-fast。
    /// </summary>
    /// <remarks>
    /// 本用例是本次改造的核心断言：它同时证明 ① 归属域声明经生成器的 F-Identity 接缝
    /// 覆盖到继承方法（键不是父接口的旧值）② 错配不再静默取错令牌。
    /// </remarks>
    [Fact]
    public async Task ThirdPartyFacade_OnInternalApp_ShouldFailFastWithCorpOwningKey()
    {
        var spy = new RecordingTokenProvider();
        using var provider = BuildProvider(InternalConfig(), spy);

        var facade = provider.GetRequiredService<IWechatWorkThirdPartyWedocSmartDocService>();

        Func<Task> act = async () => await facade.PublishSmartDocAsync(
            new PublishSmartDocRequest { Docid = "doc-1" }, CancellationToken.None);

        var thrown = await act.Should().ThrowAsync<WechatTokenOwnerMismatchException>(
            "第三方契约入口用在自建应用上必须 fail-fast，而不是用自建令牌把请求发出去");

        spy.LastRequest.Should().NotBeNull("生成的声明式客户端必须经 ITokenProvider 获取令牌");
        spy.LastRequest!.TokenManagerKey.Should().Be(WechatTokenManagerKeys.CorpAccessToken,
            "子接口声明的归属域键必须贯通到请求期");
        thrown.And.TokenManagerKey.Should().Be(WechatTokenManagerKeys.CorpAccessToken);
    }

    /// <summary>
    /// 端到端（正例）：自建应用类型子接口在自建应用上放行，且查找键为 <c>@Internal</c>。
    /// </summary>
    [Fact]
    public async Task InternalFacade_OnInternalApp_ShouldPassOwningGateWithInternalKey()
    {
        var spy = new RecordingTokenProvider();
        using var provider = BuildProvider(InternalConfig(), spy);

        var facade = provider.GetRequiredService<IWechatWorkInternalWedocSmartDocService>();

        Func<Task> act = async () => await facade.PublishSmartDocAsync(
            new PublishSmartDocRequest { Docid = "doc-1" }, CancellationToken.None);

        // 归属域闸放行后由探针中断（不进入真实令牌获取）。
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*探针中断*");
        spy.LastRequest!.TokenManagerKey.Should().Be(WechatTokenManagerKeys.InternalAccessToken);
    }
}
