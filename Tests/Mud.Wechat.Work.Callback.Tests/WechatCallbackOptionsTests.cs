// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调配置校验测试（v1 方案 §5.2 / §8）：Apps 唯一来源、逐应用凭据校验、
/// 通配键豁免 AppKey 形状校验（且不可能与真实键碰撞）、ResolveApp 精确优先回退通配。
/// </summary>
public class WechatCallbackOptionsTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";

    private static WechatAppCallbackOptions ValidApp() => new()
    {
        PushToken = "token",
        PushEncodingAESKey = AesKey,
        ReceiveId = "ww-corp",
    };

    [Fact]
    public void Validate_ShouldThrow_WhenAppsEmpty()
    {
        var options = new WechatCallbackOptions();

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*Apps*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenPushTokenMissing()
    {
        var options = new WechatCallbackOptions();
        options.Apps["app1"] = new WechatAppCallbackOptions { PushEncodingAESKey = AesKey };

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*PushToken*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenAesKeyNot43Chars()
    {
        var options = new WechatCallbackOptions();
        options.Apps["app1"] = new WechatAppCallbackOptions { PushToken = "token", PushEncodingAESKey = "short-key" };

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*43 位*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenAppKeyShapeIllegal()
    {
        var options = new WechatCallbackOptions();
        // AppKey 含 ':'（键别名风险，WechatAppKeyValidator 禁止）。
        options.Apps["a:b"] = ValidApp();

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*AppKey*");
    }

    [Fact]
    public void Validate_ShouldAllowWildcardKey_SkippingShapeValidation()
    {
        var options = new WechatCallbackOptions();
        options.Apps[WechatCallbackOptions.WildcardAppKey] = ValidApp();

        var act = () => options.Validate();

        act.Should().NotThrow("\"*\" 不满足应用键形状故与真实键不可能碰撞，显式豁免");
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllAppsValid()
    {
        var options = new WechatCallbackOptions();
        options.Apps["app1"] = ValidApp();
        options.Apps["app2.corp-x"] = ValidApp();

        var act = () => options.Validate();

        act.Should().NotThrow();
    }

    [Fact]
    public void ResolveApp_ShouldPreferExactKey_OverWildcard()
    {
        var options = new WechatCallbackOptions();
        var exact = ValidApp();
        exact.PushToken = "exact-token";
        var wildcard = ValidApp();
        wildcard.PushToken = "wild-token";
        options.Apps["app1"] = exact;
        options.Apps[WechatCallbackOptions.WildcardAppKey] = wildcard;

        options.ResolveApp("app1")!.PushToken.Should().Be("exact-token", "精确键优先");
        options.ResolveApp("contact-sync")!.PushToken.Should().Be("wild-token", "未命中精确键回退通配（通讯录同步助手）");
        options.ResolveApp(null)!.PushToken.Should().Be("wild-token", "空键（仅前缀路由）按通配解析");
    }

    [Fact]
    public void ResolveApp_ShouldReturnNull_WhenNoMatchAndNoWildcard()
    {
        var options = new WechatCallbackOptions();
        options.Apps["app1"] = ValidApp();

        options.ResolveApp("unknown").Should().BeNull("未知应用且无通配时由中间件跳过（404）");
    }

    [Fact]
    public void Validate_ShouldTreatWildcardAsSpecialKey()
    {
        // 通配键豁免形状校验；而同样含 '*' 的真实键名仍被形状校验拒绝（二者互斥、不可能碰撞）。
        var options = new WechatCallbackOptions();
        options.Apps[WechatCallbackOptions.WildcardAppKey] = ValidApp();
        options.Apps["app*1"] = ValidApp();

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>("含 '*' 的普通键名属非法应用键形状，仅通配键本身豁免");
    }

    // ---------------------------------------------------------------- 应用类型 × 回调通道（v1.2）

    /// <summary>构造指定「应用类型 × 回调通道」的应用回调凭据。</summary>
    private static WechatAppCallbackOptions AppOf(
        WechatAppType appType, WechatCallbackChannel channel, string receiveId = "")
        => new()
        {
            PushToken = "token",
            PushEncodingAESKey = AesKey,
            AppType = appType,
            Channel = channel,
            ReceiveId = receiveId,
        };

    [Fact]
    public void Validate_ShouldThrow_WhenInternalAppUsesSuiteChannel()
    {
        var options = new WechatCallbackOptions();
        options.Apps["app1"] = AppOf(WechatAppType.Internal, WechatCallbackChannel.Suite, "ww-suite");

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*自建应用*", "企业自建应用只存在应用数据回调，不得占用套件指令/票据通道");
    }

    [Fact]
    public void Validate_ShouldAllow_WhenThirdPartyUsesSuiteChannel()
    {
        var options = new WechatCallbackOptions();
        options.Apps["saas-suite"] = AppOf(WechatAppType.ThirdParty, WechatCallbackChannel.Suite, "ww-suite");

        var act = () => options.Validate();

        act.Should().NotThrow("第三方应用套件通道（suite_ticket / 授权族）合法");
    }

    [Fact]
    public void Validate_ShouldAllow_WhenProviderUsesSuiteChannel()
    {
        var options = new WechatCallbackOptions();
        options.Apps["dev-suite"] = AppOf(WechatAppType.Provider, WechatCallbackChannel.Suite, "ww-suite");

        var act = () => options.Validate();

        act.Should().NotThrow("服务商代开发套件通道合法");
    }

    [Fact]
    public void Validate_ShouldAllow_WhenThirdPartyUsesAppChannelWithoutReceiveId()
    {
        // 第三方/代开发的「应用数据通道」receiveid = 动态授权企业 CorpId，无法静态预置，留空合法。
        var options = new WechatCallbackOptions();
        options.Apps["saas-app"] = AppOf(WechatAppType.ThirdParty, WechatCallbackChannel.App);

        var act = () => options.Validate();

        act.Should().NotThrow("第三方应用数据通道 ReceiveId 留空合法（接收器动态比对 ToUserName）");
    }

    // ---------------------------------------------------------------- 开放面合法性矩阵

    [Fact]
    public void IsEventFamilyAllowed_ShouldEnforceAppTypeByChannelMatrix()
    {
        // 授权族：仅第三方/代开发的套件通道。
        AppOf(WechatAppType.ThirdParty, WechatCallbackChannel.Suite)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.Authorization).Should().BeTrue("第三方套件通道承载授权族");
        AppOf(WechatAppType.Provider, WechatCallbackChannel.Suite)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.Authorization).Should().BeTrue("代开发套件通道承载授权族");
        AppOf(WechatAppType.Internal, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.Authorization).Should().BeFalse("自建应用无套件指令回调");
        AppOf(WechatAppType.ThirdParty, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.Authorization).Should().BeFalse("应用数据通道不承载授权族");

        // 上下游变更族：仅自建应用 + 应用通道（官方 95796）。
        AppOf(WechatAppType.Internal, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.ChainChange).Should().BeTrue("自建应用可配置上下游变更回调");
        AppOf(WechatAppType.ThirdParty, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.ChainChange).Should().BeFalse("第三方/代开发暂不支持上下游变更");

        // 通讯录/异步族：应用数据通道承载，三类应用均开放。
        AppOf(WechatAppType.Internal, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.ContactChange).Should().BeTrue();
        AppOf(WechatAppType.ThirdParty, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.ContactChange).Should().BeTrue();
        AppOf(WechatAppType.Provider, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.BatchJob).Should().BeTrue();
        AppOf(WechatAppType.ThirdParty, WechatCallbackChannel.Suite)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.ContactChange).Should().BeFalse("套件通道不承载业务事件");

        // 无法判别族：不拦截（协议外报文交兜底处理器）。
        AppOf(WechatAppType.Internal, WechatCallbackChannel.App)
            .IsEventFamilyAllowed(WechatCallbackEventFamily.Unknown).Should().BeTrue();
    }
}
