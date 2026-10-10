// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 微信小店 / 视频号线<b>脚手架期</b>架构守卫（设计方案 v1 P0-c / CH-X1~X4 的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// 本守卫把「小店是独立产品线（4 包、自建回调），<b>不并入</b>公众号 / 小程序 / 企微 / 支付 /
/// 开放平台任何既有线」的产品决策<b>在实现期之前</b>锁死：若实现期「顺手」复用公众号回调通道、
/// 引用公众号令牌基座或把接口建成父接口形态，守卫立即变红。
/// </para>
/// <para>
/// <b>本文件只保留「形态类」断言</b>；端点计数、路由表、令牌归属等<b>契约类</b>断言集中在
/// <c>ChannelsRouteContractGuards</c>（CH-R1/R2）与 <c>ChannelsTokenOwnerContractGuards</c>（CH-T1~T3）。
/// </para>
/// </remarks>
public class ChannelsScaffoldContractGuards
{
    /// <summary>
    /// CH-X1：四包骨架必须齐备且落位于 <c>Src/Channels/</c>（4 包形态：主包 / Abstractions / DataModels / Callback）。
    /// </summary>
    /// <remarks>
    /// 与小程序线「无回调包」相反：小店回调为官方标准微信回调（msg_signature + EncodingAESKey + receiveid），
    /// 必须自建 <c>Mud.Wechat.Channels.Callback</c> 包承载（设计方案 v1 §3.1 / §3.5）。
    /// </remarks>
    [Fact]
    public void FourPackages_ShouldExistUnderSrcChannels()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Channels",
                     "Mud.Wechat.Channels.Abstractions",
                     "Mud.Wechat.Channels.DataModels",
                     "Mud.Wechat.Channels.Callback",
                 })
        {
            File.Exists(SourcePath(project, $"{project}.csproj"))
                .Should().BeTrue($"小店线四包缺一（{project}）：设计方案 §3.1 包家族必须齐备");
        }
    }

    /// <summary>
    /// CH-X2：接口形态 —— 小店线<b>平铺命名空间、无 <c>IsAbstract</c> 公共父接口</b>
    /// （对齐公众号 / 小程序线形态，不采用企微线的父子二分 —— 小店无「自建 / 第三方 / 代开发」
    /// 应用类型维度，设计方案 §3.1 明确不引入）。
    /// </summary>
    /// <remarks>
    /// <b>判据是「特性属性」而非 <c>Type.IsAbstract</c></b>：反射里 <c>Type.IsAbstract</c> 对<b>接口恒为
    /// <c>true</c></b>，照它判定会让守卫永远变红；反向的脚手架期空程序集又让它恒绿，属双向失效的写法
    /// （小程序线 P1-c 落地时实测踩到，判据已修正为 ≤c>HttpClientApiAttribute.IsAbstract</c> 特性属性）。
    /// </remarks>
    [Fact]
    public void Interfaces_ShouldNotUseAbstractParents_WhenContractLanded()
    {
        var asm = LoadProductLine("Mud.Wechat.Channels");

        var abstractParents = asm.GetTypes()
            .Where(static t => t.IsInterface)
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name is "HttpClientApiAttribute" or "TokenAttribute"
                                 && a.GetType().GetProperty("IsAbstract")?.GetValue(a) is true))
            .ToArray();

        abstractParents.Should().BeEmpty(
            "小店线不采用 IsAbstract 公共父接口形态（设计方案 §3.1）；如需改造须先改方案并同步更新本守卫");
    }

    /// <summary>
    /// CH-X3：工程引用隔离 —— 小店线四包<b>不得</b>引用公众号 / 小程序 / 企微 / 支付 / 开放平台
    /// 任何工程的 csproj；只允许引用 Core <c>Mud.Wechat.Abstractions</c>（唯一共享叶子）与同线兄弟包。
    /// </summary>
    [Fact]
    public void ProjectReferences_ShouldNotDependOnExistingProductLines()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Channels",
                     "Mud.Wechat.Channels.Abstractions",
                     "Mud.Wechat.Channels.DataModels",
                     "Mud.Wechat.Channels.Callback",
                 })
        {
            var csproj = SourcePath(project, $"{project}.csproj");
            File.Exists(csproj).Should().BeTrue($"工程文件缺失：{csproj}");

            var xml = File.ReadAllText(csproj);
            foreach (var banned in new[]
                     {
                         "Mud.Wechat.OfficialAccount",
                         "Mud.Wechat.MiniProgram",
                         "Mud.Wechat.Work",
                         "Mud.Wechat.Pay",
                         "Mud.Wechat.OpenPlatform",
                         "Mud.Wechat.Redis",
                     })
            {
                xml.Should().NotContain(banned,
                    $"{project} 不得引用既有产品线工程（设计方案 §3.1 依赖硬边界：{banned}）；" +
                    "小店线与五线平行，只允许共享 Core Abstractions 叶层");
            }
        }
    }

    /// <summary>
    /// CH-X3 补充：依赖方向必须严格单向 —— 主包 → {Abstractions, DataModels}、Callback → {Abstractions,
    /// DataModels}、Abstractions → {DataModels, Core}、DataModels → Core；<b>Callback 不得引用主包</b>。
    /// </summary>
    [Fact]
    public void ProjectDependencyDirection_ShouldFollowDesignMatrix()
    {
        var main = File.ReadAllText(SourcePath("Mud.Wechat.Channels", "Mud.Wechat.Channels.csproj"));
        main.Should().Contain("Mud.Wechat.Channels.Abstractions.csproj", "主包必须引用 Abstractions");
        main.Should().Contain("Mud.Wechat.Channels.DataModels.csproj", "主包必须引用 DataModels");

        var abstractions = File.ReadAllText(SourcePath("Mud.Wechat.Channels.Abstractions", "Mud.Wechat.Channels.Abstractions.csproj"));
        abstractions.Should().Contain("Mud.Wechat.Channels.DataModels.csproj", "Abstractions 必须引用 DataModels");
        abstractions.Should().Contain("Mud.Wechat.Abstractions.csproj", "Abstractions 必须引用 Core Abstractions");
        abstractions.Should().NotContain(".Channels\\Mud.Wechat.Channels.csproj", "Abstractions 不得向下引用主包");
        abstractions.Should().NotContain(".Channels.Callback", "Abstractions 不得引用回调包");

        var dataModels = File.ReadAllText(SourcePath("Mud.Wechat.Channels.DataModels", "Mud.Wechat.Channels.DataModels.csproj"));
        dataModels.Should().Contain("Mud.Wechat.Abstractions.csproj", "DataModels 只允许引用 Core Abstractions");
        dataModels.Should().NotContain("Mud.Wechat.Channels.Abstractions.csproj",
            "DataModels 不得向上引用 Abstractions（DTO 纯净面）");

        var callback = File.ReadAllText(SourcePath("Mud.Wechat.Channels.Callback", "Mud.Wechat.Channels.Callback.csproj"));
        callback.Should().Contain("Mud.Wechat.Channels.Abstractions.csproj", "Callback 必须引用 Abstractions");
        callback.Should().Contain("Mud.Wechat.Channels.DataModels.csproj", "Callback 必须引用 DataModels");
        callback.Should().NotContain(".Channels\\Mud.Wechat.Channels.csproj",
            "Callback 硬边界：不得引用主包（设计方案 §3.1）");
    }

    /// <summary>
    /// CH-X4：回调必须走<b>本线专用回调包</b> —— 不得借公众号 / 企微既有回调通道（「接收方共用信封
    /// / 统一注册表」形态被否决）。
    /// </summary>
    [Fact]
    public void CallbackPackage_ShouldBeDedicatedAndNotBridgeExistingChannels()
    {
        var callbackCsproj = SourcePath("Mud.Wechat.Channels.Callback", "Mud.Wechat.Channels.Callback.csproj");
        File.Exists(callbackCsproj).Should().BeTrue("小店回调必须自建 Callback 包（设计方案 §3.5）");

        var xml = File.ReadAllText(callbackCsproj);
        xml.Should().NotContain("OfficialAccount.Callback", "小店回调不得复用公众号回调包");

        // 程序集级复核：Callback 包运行时程序集不得引用公众号回调程序集（防仅改 csproj 文本绕过）。
        var callbackAsm = LoadProductLine("Mud.Wechat.Channels.Callback");
        callbackAsm.GetReferencedAssemblies()
            .Select(static a => a.Name)
            .Should().NotContain(
                new[] { "Mud.Wechat.OfficialAccount.Callback", "Mud.Wechat.Work.Callback", "Mud.Wechat.Pay.Callback" },
                "小店回调运行时程序集不得引用任何既有线的回调程序集");
    }

    // ---- helpers（对齐 MiniProgramScaffoldContractGuards）---------------------------

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

    /// <summary>解析源工程内路径（按 csproj 名称定位工程目录，带缓存 —— 目录再迁移时守卫不随之漂移）。</summary>
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

    private static Assembly LoadProductLine(string name)
    {
        var asm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == name)
            ?? Assembly.Load(name);
        asm.GetName().Name.Should().Be(name);
        return asm;
    }
}