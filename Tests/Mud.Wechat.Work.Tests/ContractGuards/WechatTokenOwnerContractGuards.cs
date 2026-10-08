// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 令牌归属域契约守卫：把「应用类型子接口 ↔ 凭据归属域」从命名约定升级为可校验契约
/// （方案见 <c>.docs\MudWechatWork-接口应用类型契约与令牌归属域方案-v1.md</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要全仓守卫而不是逐域补断言</b>：应用类型子接口在 20+ 个域中以同构形态存在
/// （178 个后缀命名文件 + 31 个前缀命名文件），任何「补一个域忘另一个域」的漂移都会让
/// <c>WechatAppContext.GetTokenManager</c> 的归属域闸退化为部分生效。本守卫按<b>程序集反射</b>
/// 枚举全部子接口，与批量落地脚本同源判定，漏改即红。
/// </para>
/// <para>
/// <b>不校验 TokenType 之外的官方契约</b>：注入模式与参数名的既有锁定见各域守卫与 G5 白名单；
/// 本守卫只补「归属域」这一新维度、以及 TokenType → 官方参数名的一致性（防顺手改坏）。
/// </para>
/// </remarks>
public class WechatTokenOwnerContractGuards
{
    /// <summary>族别：企业自建应用。</summary>
    private const string FamilyInternal = "Internal";

    /// <summary>族别：第三方应用。</summary>
    private const string FamilyThirdParty = "ThirdParty";

    /// <summary>族别：服务商代开发。</summary>
    private const string FamilyProvider = "Provider";

    /// <summary>
    /// 子接口枚举下限（能力漂移守卫）：少于该数量说明族别判定规则被改坏（枚举静默空跑会让本守卫假绿）。
    /// </summary>
    private const int MinimumFacadeCount = 200;

    /// <summary>
    /// 官方契约：业务令牌类型 → 注入参数名（企业微信恒为 Query 注入）。
    /// </summary>
    private static readonly Dictionary<string, string> TokenTypeToQueryName = new(StringComparer.Ordinal)
    {
        [WechatTokenTypes.AccessToken] = "access_token",
        [WechatTokenTypes.SuiteAccessToken] = "suite_access_token",
        [WechatTokenTypes.ProviderAccessToken] = "provider_access_token",
    };

    /// <summary>
    /// 全仓应用类型子接口（按程序集反射枚举，兼容后缀 / 前缀两种命名风格）。
    /// </summary>
    private static readonly Lazy<Type[]> Facades = new(() =>
        typeof(IWechatWorkWedocSmartDocService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic && FacadeFamilyOf(t.Name) != null)
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .ToArray());

    /// <summary>
    /// 按接口名判定族别（与 <c>scripts\ApplyTokenOwnerKeys.ps1</c> 的规则逐字一致）。
    /// </summary>
    /// <param name="interfaceName">接口名。</param>
    /// <returns>族别；非应用类型子接口返回 <c>null</c>。</returns>
    private static string? FacadeFamilyOf(string interfaceName)
    {
        if (interfaceName.EndsWith("_Internal", StringComparison.Ordinal)
            || interfaceName.StartsWith("IWechatWorkInternal", StringComparison.Ordinal))
        {
            return FamilyInternal;
        }

        if (interfaceName.EndsWith("_ThirdParty", StringComparison.Ordinal)
            || interfaceName.StartsWith("IWechatWorkThirdParty", StringComparison.Ordinal))
        {
            return FamilyThirdParty;
        }

        if (interfaceName.EndsWith("_Provider", StringComparison.Ordinal)
            || interfaceName.StartsWith("IWechatWorkProvider", StringComparison.Ordinal))
        {
            return FamilyProvider;
        }

        return null;
    }

    /// <summary>
    /// 族别 → 该族必须声明的归属域键。
    /// </summary>
    /// <param name="family">族别。</param>
    /// <returns>归属域查找键常量值。</returns>
    private static string ExpectedOwningKeyFor(string family)
        => family == FamilyInternal
            ? WechatTokenManagerKeys.InternalAccessToken
            : WechatTokenManagerKeys.CorpAccessToken;

    /// <summary>
    /// 守卫 TO1：应用类型子接口的归属域声明必须与族别一致，且业务令牌类型 → 官方参数名不得漂移。
    /// </summary>
    [Fact]
    public void FacadeInterfaces_ShouldDeclareOwningTokenManagerKey()
    {
        var facades = Facades.Value;
        facades.Should().HaveCountGreaterThanOrEqualTo(MinimumFacadeCount,
            "全仓应用类型子接口数量下限：枚举规则被改坏会静默空跑，故显式断言");

        // 各族别都必须有实际命中（防前缀 / 后缀判定规则单侧失效）。
        facades.Select(f => FacadeFamilyOf(f.Name)).Distinct()
            .Should().BeEquivalentTo(new[] { FamilyInternal, FamilyThirdParty, FamilyProvider },
                "后缀命名（*_Internal / *_ThirdParty / *_Provider）与前缀命名（IWechatWorkInternal* 等）两种风格都必须被识别");

        var withoutToken = new List<string>();
        var violations = new List<string>();

        foreach (var facade in facades)
        {
            var family = FacadeFamilyOf(facade.Name)!;
            var token = facade.GetCustomAttribute<TokenAttribute>();
            if (token == null)
            {
                withoutToken.Add(facade.Name);
                continue;
            }

            // TokenType → 官方 Query 参数名一致（三态映射，与官方契约逐字一致）。
            if (TokenTypeToQueryName.TryGetValue(token.TokenType, out var expectedQueryName))
            {
                if (token.InjectionMode != TokenInjectionMode.Query || token.Name != expectedQueryName)
                {
                    violations.Add($"{facade.Name}：TokenType={token.TokenType} 必须为 Query 注入且参数名为 '{expectedQueryName}'"
                        + $"（实际 InjectionMode={token.InjectionMode}, Name='{token.Name}'）");
                }
            }

            if (token.TokenType == WechatTokenTypes.AccessToken)
            {
                var expectedKey = ExpectedOwningKeyFor(family);
                if (token.TokenManagerKey != expectedKey)
                {
                    violations.Add($"{facade.Name}：族别 {family} 必须声明 TokenManagerKey = '{expectedKey}'"
                        + $"（实际 '{token.TokenManagerKey}'）");
                }

                continue;
            }

            // 非 AccessToken 族（ProviderAccessToken / SuiteAccessToken）本身已无歧义：不得叠加归属域键。
            // 设计定夺（.docs/MudWechatWork-定夺二-Provider令牌归属域-v1.md 方案 A）：该族凭据来源唯一、
            // 族内不存在错配空间，错配防线由 WechatAppContext.GetTokenManager 的「未装配即抛」承担，
            // 归属域键对该族无新增拦截能力 ⇒ 永不增设 @Provider / @Suite 归属域键。
            if (token.TokenManagerKey == WechatTokenManagerKeys.InternalAccessToken
                || token.TokenManagerKey == WechatTokenManagerKeys.CorpAccessToken)
            {
                violations.Add($"{facade.Name}：TokenType={token.TokenType} 已无歧义，不得叠加归属域键" +
                    "（归属域键仅服务于 AccessToken 的双凭据来源消歧，永不增设 @Provider/@Suite——设计定夺见 .docs 定夺二）");
            }

            if (family == FamilyInternal)
            {
                violations.Add($"{facade.Name}：自建应用族不得消费 {token.TokenType}"
                    + "（该令牌管理器仅第三方/代开发应用可用）");
            }
        }

        violations.Should().BeEmpty("应用类型子接口的归属域声明必须与族别一一对应（漏改 / 越界均在此拦截）");

        withoutToken.Should().Equal(
            new[]
            {
                // 既存例外：get_customized_auth_url 以显式 Query 参数传令牌、不带 [Token]（G5 白名单例外）。
                "IWechatWorkInternalAibotService",
                "IWechatWorkInternalWebhookService",
                "IWechatWorkProviderAuthenticationUrl",
            },
            "无 [Token] 的应用类型子接口精确清单（按名称升序）：" +
            "① 智能机器人主动回复端点以 URL 一次性凭据 response_code 鉴权（官方 101138，不走任何令牌链路）；" +
            "② get_customized_auth_url 以显式 Query 参数传令牌；" +
            "③ 群机器人 Webhook 端点以 URL Query 上的 key 为机器人凭据（官方 91770，不走任何令牌链路，" +
            "无令牌锁定见守卫 WEB3）。新增条目须附理由并同步 G5/G7 评估");
    }

    /// <summary>
    /// 守卫 TO2：子接口声明的归属域键必须被恢复链路覆盖，且自建 / 非自建的键集互不交叉。
    /// </summary>
    /// <remarks>
    /// 归属域键来自生成代码的 <c>TokenRecoveryContext.TokenManagerKey</c>，
    /// 由 <c>ITokenManagerRegistry</c>（<c>WechatAppManager.WechatTokenManagerRegistry</c>）
    /// 按键定位管理器；该注册表<b>折叠</b>自 <c>WechatTokenRouting.OwnedKeys</c>——故本守卫断言
    /// 「声明的键 ⊆ OwnedKeys 并集」即等价于「恢复链路不会因新键漏入表而静默降级」。
    /// </remarks>
    [Fact]
    public void DeclaredOwningKeys_ShouldBeCoveredByRecoveryRegistryAndDisjoint()
    {
        var internalKeys = WechatTokenRouting.OwnedKeys(WechatAppType.Internal);
        var thirdPartyKeys = WechatTokenRouting.OwnedKeys(WechatAppType.ThirdParty);
        var providerKeys = WechatTokenRouting.OwnedKeys(WechatAppType.Provider);

        internalKeys.Should().NotBeEmpty();
        thirdPartyKeys.Should().Equal(providerKeys, "第三方与代开发的凭据归属域一致（同为授权企业级 access_token）");
        internalKeys.Should().NotIntersectWith(thirdPartyKeys,
            "自建应用绝不可命中企业级键（否则会取到错误的凭据）");

        var allOwningKeys = internalKeys.Concat(thirdPartyKeys).Distinct().ToArray();
        var legacyKeys = new[]
        {
            WechatTokenTypes.AccessToken, WechatTokenTypes.SuiteAccessToken, WechatTokenTypes.ProviderAccessToken,
        };

        var declaredKeys = Facades.Value
            .Select(f => f.GetCustomAttribute<TokenAttribute>()?.TokenManagerKey)
            .Where(k => !string.IsNullOrEmpty(k))
            // 反射陷阱：未显式书写 TokenManagerKey 时读到的是构造函数参数的默认值
            // （TokenTypes.AccessToken = "AccessToken"），而生成器在该情形下回退到 TokenType。
            // 该哨兵值不代表任何真实查找键，须排除（否则断言会被噪声击穿）。
            .Where(k => !string.Equals(k, TokenTypes.AccessToken, StringComparison.Ordinal))
            .Select(k => k!)
            .Distinct()
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToArray();

        declaredKeys.Should().BeSubsetOf(allOwningKeys.Concat(legacyKeys),
            "子接口声明的查找键必须是归属域键或既有业务令牌类型键；新增键必须同批加入 OwnedKeys（否则恢复链路降级）");

        declaredKeys.Should().Contain(allOwningKeys,
            "每个归属域键都必须至少被一个子接口使用（守卫 TO1 同时保证其值为契约字面量）");
    }

    /// <summary>
    /// 守卫 TO3：归属域键的字面契约、解析与允许矩阵。
    /// </summary>
    [Fact]
    public void OwningKeyRouting_ShouldEnforceAppTypeMatrix()
    {
        // 字面量即契约：改键值等于改契约，必须同批修订本守卫与方案文档。
        WechatTokenManagerKeys.InternalAccessToken.Should().Be("Wechat.AccessToken@Internal");
        WechatTokenManagerKeys.CorpAccessToken.Should().Be("Wechat.AccessToken@Corp");

        // 解析：归属域后缀剥离后必须回到官方业务令牌类型。
        WechatTokenRouting.ResolveBaseTokenType(WechatTokenManagerKeys.InternalAccessToken, "app-a", WechatAppType.Internal)
            .Should().Be(WechatTokenTypes.AccessToken);
        WechatTokenRouting.ResolveBaseTokenType(WechatTokenManagerKeys.CorpAccessToken, "app-a", WechatAppType.ThirdParty)
            .Should().Be(WechatTokenTypes.AccessToken);

        // 兼容：未声明归属域的旧键一律原样返回、不校验（公共父接口与宿主直调语义不变）。
        foreach (var legacyKey in new[]
                 {
                     WechatTokenTypes.AccessToken, WechatTokenTypes.SuiteAccessToken, WechatTokenTypes.ProviderAccessToken,
                 })
        {
            foreach (var appType in new[] { WechatAppType.Internal, WechatAppType.ThirdParty, WechatAppType.Provider })
            {
                WechatTokenRouting.ResolveBaseTokenType(legacyKey, "app-a", appType).Should().Be(legacyKey,
                    $"旧键 '{legacyKey}' 的既有路由语义不得因归属域闸改变");
            }
        }

        // 矩阵：Internal 键仅自建可用；Corp 键仅第三方 / 代开发可用。
        WechatTokenRouting.ResolveBaseTokenType(WechatTokenManagerKeys.InternalAccessToken, "app-a", WechatAppType.Internal)
            .Should().NotBeNull();
        foreach (var foreignAppType in new[] { WechatAppType.ThirdParty, WechatAppType.Provider })
        {
            var internalAct = () => WechatTokenRouting.ResolveBaseTokenType(
                WechatTokenManagerKeys.InternalAccessToken, "app-a", foreignAppType);
            internalAct.Should().Throw<WechatTokenOwnerMismatchException>()
                .Which.TokenManagerKey.Should().Be(WechatTokenManagerKeys.InternalAccessToken);

            var corpAct = () => WechatTokenRouting.ResolveBaseTokenType(
                WechatTokenManagerKeys.CorpAccessToken, "app-a", WechatAppType.Internal);
            corpAct.Should().Throw<WechatTokenOwnerMismatchException>()
                .Which.ActualAppType.Should().Be(WechatAppType.Internal);
        }

        // 未知后缀（键拼写错误）必须 fail-fast，而非落到「未知的令牌类型」这一含糊分支。
        var typoAct = () => WechatTokenRouting.ResolveBaseTokenType("Wechat.AccessToken@Corps", "app-a", WechatAppType.Internal);
        typoAct.Should().Throw<InvalidOperationException>().WithMessage("*未知的令牌归属域后缀*");
    }
}
