// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Mud.Wechat.Pay.Callback.Tests.ContractGuards;

/// <summary>
/// 支付回调包**脚手架期**架构守卫（方案 P0-a / PAY-CB1 与红线 7 的机械化落地）。
/// </summary>
/// <remarks>
/// <b>为何需要本守卫</b>：支付回调的解密是 <c>AEAD_AES_256_GCM</c>（12 字节 nonce + 16 字节 tag），
/// 与 XML 回调的「AES-CBC + 32 字节块 PKCS7」**完全不同**。复用 <c>WechatCallbackCrypto</c> 会：
/// ① 算法错配直接解密失败；② 若有人为「跑通」而改造共用类，会破坏既有回调线的 32 字节块补位纪律
/// （AGENTS §5.5「禁用 .NET 内置 16 块 PaddingMode.PKCS7」）。密码学必须分家。
/// </remarks>
public class PayCallbackScaffoldContractGuards
{
    /// <summary>
    /// 红线 7：支付回调包源码**不得**出现 XML 回调的 PKCS7/CBC 解密设施。
    /// </summary>
    /// <remarks>
    /// 关键事实：<c>WechatCallbackCrypto</c> 声明在 <b><c>Mud.Wechat.Abstractions</c></b> 里，而支付回调包
    /// **必须**引用 Abstractions（PAY-CB1）⇒ 该类型对支付回调是**编译期可见的**。诱惑是真实的，
    /// 只能靠本守卫拦截。当前支付回调包尚无源文件（P0-c 才落），故断言分两段：
    /// ① 危害源必须仍存在（防守卫因上游改名而静默空跑，AGENTS §6 同款纪律）；
    /// ② 一旦出现源文件，即禁止引用该类型与 PKCS7 填充。
    /// </remarks>
    [Fact]
    public void Source_ShouldNotReuseXmlCallbackCrypto_WhenScaffold()
    {
        // ① 危害源存在性：WechatCallbackCrypto 必须仍公开于 Abstractions（支付回调可见）。
        var abstractionsDir = Path.Combine(Root, "Mud.Wechat.Abstractions");
        var hazard = Directory.GetFiles(abstractionsDir, "WechatCallbackCrypto.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                        && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();
        hazard.Should().HaveCount(1,
            "WechatCallbackCrypto 消失/改名 ⇒ 红线 7 守卫对象漂移，须同批核对本守卫（AGENTS §6：防静默空跑）");

        // ② 引用禁令：支付线全部源文件不得触碰 XML 回调密码学。
        var srcRoot = Path.Combine(Root, "Mud.Wechat.Pay.Callback");
        Directory.Exists(srcRoot).Should().BeTrue($"目录缺失：{srcRoot}");

        var files = Directory.GetFiles(srcRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                        && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        foreach (var file in files)
        {
            var code = StripComments(File.ReadAllText(file));

            code.Should().NotContain("WechatCallbackCrypto",
                $"{Path.GetFileName(file)} 不得引用 XML 回调密码学类（算法族不同：GCM vs CBC+PKCS7）");
            code.Should().NotContain("PaddingMode.PKCS7",
                $"{Path.GetFileName(file)} 不得使用 PKCS7 填充（支付回调走 AEAD_AES_256_GCM，无填充）");
        }
    }

    /// <summary>
    /// PAY-CB1：支付回调包**必须**引用共享叶层 <c>Mud.Wechat.Abstractions</c>
    /// （复用 <c>IWechatCallbackReplayGuard</c> 抗重放端口与回调选项基座），且**禁止**引用回调宿主包。
    /// </summary>
    [Fact]
    public void ProjectReferences_ShouldReuseSharedLeafOnly_WhenScaffold()
    {
        var csproj = Path.Combine(Root, "Mud.Wechat.Pay.Callback", "Mud.Wechat.Pay.Callback.csproj");
        File.Exists(csproj).Should().BeTrue($"工程文件缺失：{csproj}");

        var xml = File.ReadAllText(csproj);

        xml.Should().Contain("<TargetFrameworks>net6.0;net8.0;net10.0</TargetFrameworks>",
            "支付回调包必须显式声明支付线 TFM 例外（AesGcm 在 netstandard2.0 不存在）");
        xml.Should().Contain("Mud.Wechat.Abstractions.csproj",
            "支付回调必须复用 Abstractions 的抗重放端口/选项基座，不得自建第二套（方案 §5.7 PAY-CB1）");

        foreach (var banned in new[]
                 {
                     "Mud.Wechat.Work.Callback.csproj",
                     "Mud.Wechat.OfficialAccount.Callback.csproj",
                     "Mud.Wechat.MiniProgram.Callback.csproj",
                 })
        {
            xml.Should().NotContain(banned, "支付回调密码学独立，禁止与 XML 回调宿主包耦合（红线 7）");
        }
    }

    /// <summary>
    /// PAY-B9 补充：支付回调包不得目标 <c>netstandard2.0</c>（GCM + <c>FrameworkReference</c> 均需 net6+）。
    /// </summary>
    [Fact]
    public void CallbackProject_ShouldNotTargetNetStandard20_WhenScaffold()
    {
        var csproj = Path.Combine(Root, "Mud.Wechat.Pay.Callback", "Mud.Wechat.Pay.Callback.csproj");
        var raw = File.ReadAllText(csproj);

        raw.Should().Contain("<TargetFrameworks>net6.0;net8.0;net10.0</TargetFrameworks>");

        // 先剔除 XML 注释：csproj 的**理由注释**本身会提到 netstandard2.0（AB-G9 已踩过同款坑）。
        var code = System.Text.RegularExpressions.Regex.Replace(raw, @"<!--.*?-->", string.Empty,
            System.Text.RegularExpressions.RegexOptions.Singleline);

        // 断言口径 = 「TFM 属性不含 ns2.0」，而非全文不含：分析器 DLL 的打包路径
        // `bin\$(Configuration)\netstandard2.0\...` 是**合法资产路径**（分析器工程本就 TFM ns2.0），会误报。
        System.Text.RegularExpressions.Regex.IsMatch(code, @"<TargetFrameworks?[^>]*>[^<]*netstandard2\.0")
            .Should().BeFalse("支付回调禁止 ns2.0（AesGcm 不存在 + ASP.NET Core FrameworkReference 不可用）");
    }

    // ---- helpers -------------------------------------------------------------

    /// <summary>剔除 <c>//</c>、<c>/* */</c> 与 XML 文档注释，避免守卫对「文档提及」误报（AB-G9 已踩过此坑）。</summary>
    private static string StripComments(string text)
        => Regex.Replace(text, @"//.*?$|/\*.*?\*/", string.Empty,
            RegexOptions.Multiline | RegexOptions.Singleline);

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
