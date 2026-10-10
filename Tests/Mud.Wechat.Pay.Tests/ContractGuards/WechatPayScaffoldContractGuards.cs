// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 支付线**脚手架期**架构守卫（方案 P0-a / PAY-B1、B9 的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要本守卫</b>：支付线与其余产品线的**令牌范式根本不同**——APIv3 凭据是商户 RSA 私钥签名，
/// 不是 Bearer access_token。若照搬 <c>[Token]</c> 声明式注入，运行期只会静默退化为「注入了空令牌的
/// HttpClient」，且签名时抛 <c>WechatPayException</c>，排查成本极高。此约束是评审结论（方案 §5 PAY-B1），
/// 必须在实现期之前锁死。
/// </para>
/// </remarks>
public class WechatPayScaffoldContractGuards
{
    /// <summary>
    /// PAY-B1：支付线全部接口**禁止**声明 <c>[Token]</c>（凭据走商户私钥签名，非令牌注入）。
    /// </summary>
    [Fact]
    public void Interfaces_ShouldNotDeclareTokenAttribute_WhenScaffold()
    {
        var asm = Assembly.Load("Mud.Wechat.Pay.Abstractions");
        asm.GetName().Name.Should().Be("Mud.Wechat.Pay.Abstractions");

        var offenders = asm.GetTypes()
            .Where(t => t.IsInterface)
            .Where(t => t.GetCustomAttributes(true)
                .Any(a => a.GetType().Name == "TokenAttribute"))
            .ToArray();

        offenders.Should().BeEmpty(
            "支付线凭据为商户 RSA 私钥签名，不存在 access_token；[Token] 注入会静默退化（方案 §5 PAY-B1）：" +
            string.Join(", ", offenders.Select(t => t.FullName)));
    }

    /// <summary>
    /// PAY-B1 理由锁：<c>[Token]</c> 机制**承载不了**支付凭据 —— 这不是风格偏好，是契约不匹配。
    /// </summary>
    /// <remarks>
    /// <para>事实链（三条，任一成立即足以否决 <c>[Token]</c>）：</para>
    /// <list type="number">
    /// <item><b>算法不匹配</b>：注入机制里唯一的签名型模式 <c>HmacSignature</c> 背后是
    /// <c>IHmacSignatureProvider</c>，其契约为「<c>string secretKey</c>（对称共享密钥）→ <c>Task&lt;string&gt;</c>
    /// （单个 header 字符串）」，默认实现 <c>HMACSHA256</c>；而 APIv3 强制 <b>RSA-SHA256 + 商户私钥</b>（非对称）。
    /// 整个 <c>Mud.HttpUtils</c> 无任何 RSA 代码路径。</item>
    /// <item><b>输出形态不匹配</b>：APIv3 的 <c>Authorization</c> 是
    /// <c>WECHATPAY2-SHA256-RSA2048 mchid=…,nonce_str=…,signature=…,timestamp=…,serial_no=…</c> <b>五字段</b>；
    /// <c>[Token]</c> 只能写入<b>一个</b>标量到<b>一个</b>位置。</item>
    /// <item><b>输入时机不匹配</b>：签名串含 <c>TIMESTAMP\nNONCE\nBODY</c>，须在<b>请求体序列化之后</b>逐请求计算；
    /// <c>ITokenManager</c> 的语义是「解析<b>缓存中</b>的令牌」，拿不到 body。</item>
    /// </list>
    /// <para><b>本断言锁定第 1 条</b>：枚举取值集固定为这 7 个（未来若上游新增 RSA 形态模式，此处会失败，
    /// 强制重新评估 PAY-B1 而非默认沿用）；同时直接断言签名钩子的<b>参数类型</b>为 <c>string</c> secretKey。</para>
    /// </remarks>
    [Fact]
    public void TokenInjectionMode_ShouldHaveNoAsymmetricSignatureMode_WhenScaffold()
    {
        var names = Enum.GetNames(typeof(TokenInjectionMode));
        names.Should().Equal(
            new[] { "Header", "Query", "Path", "ApiKey", "HmacSignature", "BasicAuth", "Cookie" },
            "令牌注入模式取值集漂移须先重评方案 §5 PAY-B1（当前唯一签名模式 HmacSignature 是对称 HMAC，非 RSA）");

        // 签名钩子必须是「对称共享密钥 → 单标量」形态：这是 [Token] 无法承载 RSA 商户私钥的直接证据。
        var generator = typeof(IHmacSignatureProvider).GetMethod("GenerateSignatureAsync");
        generator.Should().NotBeNull("IHmacSignatureProvider.GenerateSignatureAsync 是唯一签名钩子，消失即契约变更");
        generator!.ReturnType.Should().Be(typeof(Task<string>),
            "签名钩子只返回单个 header 标量，装不下 APIv3 的五字段 Authorization");

        var secretKeyParam = generator.GetParameters()
            .FirstOrDefault(p => p.ParameterType == typeof(string) && p.Name == "secretKey");
        secretKeyParam.Should().NotBeNull("签名密钥须为 string secretKey（对称共享密钥）——RSA 私钥无法塞进该槽位");
    }

    /// <summary>
    /// PAY-B9（本地副本）：支付线**不得**目标 <c>netstandard2.0</c>。
    /// </summary>
    /// <remarks>
    /// 权威断言在 <c>WechatAbstractionsContractGuards.AB-G8</c>（逐 csproj）；此处为支付线本地守卫，
    /// 覆盖「AB-G8 漏网的新建支付工程」——<c>Directory.Build.props</c> 默认给 4 个 TFM，
    /// 忘写 <c>&lt;TargetFrameworks&gt;</c> 就会静默带上 ns2.0（该坑已踩过一次）。
    /// </remarks>
    [Fact]
    public void PayProjects_ShouldNotTargetNetStandard20_WhenScaffold()
    {
        // 注：csproj 落在各自的**子目录**内，必须递归找（TopDirectoryOnly 会漏掉全部四个 ⇒ 守卫假红）。
        // 但递归从仓库根出发会把 `Tests/Mud.Wechat.Pay*.Tests/*.csproj` 一并捞进来 ⇒ 须排除 Tests/（测试工程
        // 故意不继承支付线 TFM 例外，Tests/Directory.Build.props 单 TFM net8.0）。
        var payProjects = Directory.GetFiles(Root, "Mud.Wechat.Pay*.csproj", SearchOption.AllDirectories)
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                        && !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                        && !p.StartsWith(Path.Combine(Root, "Tests") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToArray();

        payProjects.Should().HaveCountGreaterThanOrEqualTo(4, "支付线至少四包（主包/Abstractions/DataModels/Callback）");

        foreach (var csproj in payProjects)
        {
            var xml = File.ReadAllText(csproj);
            xml.Should().Contain("<TargetFrameworks>net6.0;net8.0;net10.0</TargetFrameworks>",
                $"{Path.GetFileName(csproj)} 必须显式声明支付线 TFM 例外（AesGcm 在 netstandard2.0 不存在）");
            xml.Should().NotContain("<TargetFramework>netstandard2.0</TargetFramework>");
        }
    }

    /// <summary>
    /// PAY-B9 补充：支付线程序集必须能解析 <c>AesGcm</c>（APIv3 回调 <c>AEAD_AES_256_GCM</c> 官方强制）。
    /// </summary>
    [Fact]
    public void AesGcm_ShouldBeResolvable_WhenTargetingNet60()
    {
        var act = () => typeof(System.Security.Cryptography.AesGcm).Assembly.FullName;
        act.Should().NotThrow("net6.0+ 必须提供 AesGcm；若失败说明测试跑在 ns2.0 目标上");
        act().Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// PAY-B11：工程引用隔离 —— 支付线必须引用共享叶层 <c>Mud.Wechat.Abstractions</c>，
    /// 且**禁止**引用公众号线 / Work 线。
    /// </summary>
    [Fact]
    public void ProjectReferences_ShouldIsolateProductLines_WhenScaffold()
    {
        var csproj = SourcePath("Mud.Wechat.Pay", "Mud.Wechat.Pay.csproj");
        File.Exists(csproj).Should().BeTrue($"工程文件缺失：{csproj}");

        var xml = File.ReadAllText(csproj);
        xml.Should().Contain("Mud.Wechat.Abstractions.csproj",
            "支付线必须复用 Abstractions 叶层（存储端口/配置基座/异常），不得另起炉灶（方案 §4 PAY-B9）");

        foreach (var banned in new[]
                 {
                     "Mud.Wechat.OfficialAccount.csproj",
                     "Mud.Wechat.Work.csproj",
                     "Mud.Wechat.MiniProgram.csproj",
                 })
        {
            xml.Should().NotContain(banned, "产品线之间只允许共享 Abstractions/DataModels 叶层（方案 §4）");
        }
    }

    // ---- helpers -------------------------------------------------------------

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

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
