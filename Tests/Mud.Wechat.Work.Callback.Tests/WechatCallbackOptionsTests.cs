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
        CorpId = "ww-corp",
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
}
