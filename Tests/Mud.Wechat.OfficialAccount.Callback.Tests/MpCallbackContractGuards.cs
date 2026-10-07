// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Text;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 公众号回调域契约守卫（CB-L1a/L1b/L1f/L1g + CB-MP-1/2/3/6）。
/// </summary>
/// <remarks>
/// 与企微侧 <c>CB1~CB24</c>、叶层 <c>CB-L1c/L1d/L1e/L1h/L1i</c> 分工：
/// 本组锁定「公众号侧纵向依赖、验签/回写口径、门禁同批」。
/// </remarks>
public class MpCallbackContractGuards
{
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }

    private static string ReadSource(string relativePath)
        => File.ReadAllText(Path.Combine(GetSolutionRoot(), relativePath), Encoding.UTF8);

    /// <summary>CB-L1a：公众号侧纵向依赖 —— 不得引用任何 Work 程序集（含 Roslyn 工具工程旧名形态），且必须经叶层。</summary>
    [Fact]
    public void OfficialAccountSide_ShouldNotReferenceWork()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.OfficialAccount.Callback/Mud.Wechat.OfficialAccount.Callback.csproj",
                     "Mud.Wechat.OfficialAccount.Abstractions/Mud.Wechat.OfficialAccount.Abstractions.csproj",
                 })
        {
            // 只审计**引用行**（Description 等文案中的类比说明不构成依赖边）。
            var referenceLines = ReadSource(project)
                .Split('\n')
                .Where(line => line.Contains("ProjectReference", StringComparison.Ordinal)
                               || line.Contains("PackageReference", StringComparison.Ordinal))
                .ToArray();

            referenceLines.Should().NotBeEmpty($"{project} 应至少有一条引用（守卫空转即失效）");
            referenceLines.Should().NotContain(line => line.Contains("Mud.Wechat.Work", StringComparison.Ordinal),
                $"{project} 不得横向引用企业微信产品线（含 Work.Callback.Generator / Work.Callback.Analyzers 旧名形态）");
        }

        // 叶层依赖：OA.Abstractions 直引叶层；运行时包经 OA.Abstractions 传递可见（不新增第三边）。
        ReadSource("Mud.Wechat.OfficialAccount.Abstractions/Mud.Wechat.OfficialAccount.Abstractions.csproj")
            .Should().Contain("Mud.Wechat.Abstractions.csproj",
                "公众号契约面必须经叶层（协议与安全内核）落地共用能力");
        ReadSource("Mud.Wechat.OfficialAccount.Callback/Mud.Wechat.OfficialAccount.Callback.csproj")
            .Should().Contain("Mud.Wechat.OfficialAccount.Abstractions.csproj",
                "回调运行时包只引公众号契约面（ASP.NET 依赖只存在于本包）");
    }

    /// <summary>CB-L1b：企微侧反向 —— 不得引用公众号侧任何工程。</summary>
    [Fact]
    public void WorkSide_ShouldNotReferenceOfficialAccount()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Work.Callback/Mud.Wechat.Work.Callback.csproj",
                     "Mud.Wechat.Work.Abstractions/Mud.Wechat.Work.Abstractions.csproj",
                 })
        {
            ReadSource(project).Should().NotContain("Mud.Wechat.OfficialAccount");
        }
    }

    /// <summary>CB-L1f：Roslyn 工具工程中立化 —— 旧名不存在、新名存在且被两条产品线以 Analyzer 方式引用。</summary>
    [Fact]
    public void CallbackTools_ShouldBeNeutralNamedAndShared()
    {
        var root = GetSolutionRoot();

        File.Exists(Path.Combine(root, "Mud.Wechat.Work.Callback.Generator", "Mud.Wechat.Work.Callback.Generator.csproj"))
            .Should().BeFalse("旧名工具工程必须已改名（否则公众号引用它即为横向引用）");
        File.Exists(Path.Combine(root, "Mud.Wechat.Work.Callback.Analyzers", "Mud.Wechat.Work.Callback.Analyzers.csproj"))
            .Should().BeFalse("旧名工具工程必须已改名");

        File.Exists(Path.Combine(root, "Mud.Wechat.Callback.Generator", "Mud.Wechat.Callback.Generator.csproj"))
            .Should().BeTrue();
        File.Exists(Path.Combine(root, "Mud.Wechat.Callback.Analyzers", "Mud.Wechat.Callback.Analyzers.csproj"))
            .Should().BeTrue();

        // 两条产品线各自引用中立工具（Analyzer 形态），互不引用对方。
        ReadSource("Mud.Wechat.Work.Callback/Mud.Wechat.Work.Callback.csproj")
            .Should().Contain("Mud.Wechat.Callback.Generator.csproj")
            .And.Contain("Mud.Wechat.Callback.Analyzers.csproj");
        ReadSource("Mud.Wechat.OfficialAccount.Abstractions/Mud.Wechat.OfficialAccount.Abstractions.csproj")
            .Should().Contain("Mud.Wechat.Callback.Generator.csproj");
    }

    /// <summary>
    /// CB-L1g：去中间层 —— 不新建 <c>Mud.Wechat.Callback.Abstractions</c>；回调相关工程恰为 4 个
    /// （两条产品线运行时 + 两个中立工具）。
    /// </summary>
    [Fact]
    public void CallbackProjects_ShouldBeExactlyFour()
    {
        var root = GetSolutionRoot();
        Directory.Exists(Path.Combine(root, "Mud.Wechat.Callback.Abstractions"))
            .Should().BeFalse("跨产品线回调中间层已被否决（叶层是唯一公共祖先）");

        var slnx = ReadSource("Mud.Wechat.slnx");
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Work.Callback/Mud.Wechat.Work.Callback.csproj",
                     "Mud.Wechat.OfficialAccount.Callback/Mud.Wechat.OfficialAccount.Callback.csproj",
                     "Mud.Wechat.Callback.Generator/Mud.Wechat.Callback.Generator.csproj",
                     "Mud.Wechat.Callback.Analyzers/Mud.Wechat.Callback.Analyzers.csproj",
                 })
        {
            slnx.Should().Contain(project);
        }
    }

    /// <summary>CB-MP-1：路由前缀默认 <c>mp</c>，方法白名单仅 GET/POST，其余 405。</summary>
    [Fact]
    public void RouteAndMethodWhitelist_ShouldMatchDesign()
    {
        ReadSource("Mud.Wechat.OfficialAccount.Callback/MpCallbackOptions.cs")
            .Should().Contain("GlobalRoutePrefix { get; set; } = \"mp\"", "默认前缀必须为 mp（与企微 wechat 不冲突）");

        var middleware = ReadSource("Mud.Wechat.OfficialAccount.Callback/MpCallbackMiddleware.cs");
        middleware.Should().Contain("HttpMethods.IsGet").And.Contain("HttpMethods.IsPost");
        middleware.Should().Contain("StatusCodes.Status405MethodNotAllowed");
        middleware.Should().Contain("StatusCodes.Status413RequestEntityTooLarge");
        middleware.Should().Contain("StatusCodes.Status415UnsupportedMediaType");
    }

    /// <summary>
    /// CB-MP-2：验签口径 —— 密文分支必须用 4 参 <c>msg_signature</c>，公众号侧使用常量时间比较。
    /// </summary>
    [Fact]
    public void SignaturePolicy_ShouldBeFourParamOnPostAndConstantTime()
    {
        var receiver = ReadSource("Mud.Wechat.OfficialAccount.Callback/MpCallbackReceiver.cs");

        receiver.Should().Contain("msg_signature", "密文分支必须读取 msg_signature");
        receiver.Should().Contain("nonce ?? string.Empty, encrypt!)",
            "密文分支必须计算 4 参签名（含 Encrypt 参与项）—— 若误用 3 参 signature 验 POST，本断言必红");
        receiver.Should().Contain("FixedTimeEquals", "公众号侧比较必须常量时间（防时序侧信道）");
    }

    /// <summary>CB-MP-3：回写口径 —— 无回复为明文 <c>success</c>；有回复时非明文模式必加密。</summary>
    [Fact]
    public void ReplyPolicy_ShouldEncryptEverythingButSuccess()
    {
        var writer = ReadSource("Mud.Wechat.OfficialAccount.Callback/MpCallbackReplyWriter.cs");

        writer.Should().Contain("\"success\"", "无回复出口必须是官方许可的明文 success");
        writer.Should().Contain("WechatCallbackCrypto.Encrypt(");
        writer.Should().Contain("mode == MpCallbackSecurityMode.Plain",
            "仅明文模式可回明文 XML；安全/兼容模式必须加密回写（否则平台判非法响应）");
    }

    /// <summary>CB-MP-6 + CB-INV2：软超时上限与必填校验 fail-fast。</summary>
    [Fact]
    public void Validation_ShouldFailFastOnIllegalValues()
    {
        var optionsSource = ReadSource("Mud.Wechat.OfficialAccount.Callback/MpCallbackOptions.cs");
        optionsSource.Should().Contain("EventHandlingTimeoutMs >= 5000", "软超时必须严格小于平台 5 秒断连时限");
        optionsSource.Should().Contain("缺少 AppId", "AppId 必填（解密后 appid 校验基准）");
        optionsSource.Should().Contain("必须为 43 位字符");

        var act = () => new MpCallbackOptions
        {
            EventHandlingTimeoutMs = 5000,
            Apps = { ["mp1"] = new MpAppCallbackOptions { PushToken = "t", AppId = "a" } },
        }.Validate();
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>CB-MP-4：键集合下限 —— 常量类声明的官方键数量与本轮可锁定集合一致（防静默删键）。</summary>
    [Fact]
    public void KeySets_ShouldMatchVerifiedScope()
    {
        var messageKeys = typeof(MpCallbackMessageTypes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();
        var eventKeys = typeof(MpCallbackEventTypes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();
        var cardKeys = typeof(MpCardEventTypes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();
        var authorizationKeys = typeof(MpAuthorizationEventTypes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();

        // 消息键 8 个常量（含 event）⇒ 7 个普通消息键；事件键 13 个（9 菜单 + 4 通用）；
        // 卡券 13 个（含审核通过/不通过两键）；用户授权变更 3 个。
        messageKeys.Should().HaveCount(8);
        eventKeys.Should().HaveCount(13);
        cardKeys.Should().HaveCount(13);
        authorizationKeys.Should().HaveCount(3);
        // 跨族不得撞键（官方键全局唯一，撞键会让注册表相互覆盖）。
        eventKeys.Concat(cardKeys).Concat(authorizationKeys).Should().OnlyHaveUniqueItems();
        messageKeys.Should().OnlyHaveUniqueItems();
        eventKeys.Should().OnlyHaveUniqueItems();
    }
}
