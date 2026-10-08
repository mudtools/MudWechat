// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.AccountId;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 「账号ID」域（AccountId 模块）契约守卫：路由表、接口层级、令牌绑定与 JSON 上下文登记锁定。
/// </summary>
/// <remarks>
/// <para>
/// 七接口族形态（官方「账号ID」模块，自建 98729/95884/101521、第三方 96516/97061/97063/95900/96169/97064/98412/99375/96168/96518、
/// 代开发 97104/97105/97106/97107/97108/97109/97110/98741/99378/99601/96250）：
/// <b>ID 转换族</b>（<see cref="IWechatWorkAccountIdService"/>）：第三方/代开发公共面 9 端点收敛父接口，
/// 第三方子接口空标记，代开发子接口另持「群 ID 升级」2 差异端点（99601）；
/// <b>tmp_external_userid 转换族</b>（<see cref="IWechatWorkAccountIdTmpExternalUserIdService"/>）：
/// 三类应用公共面 1 端点收敛父接口，三子接口空标记；
/// <b>自建应用对接族</b>（<see cref="IWechatWorkAccountIdInteropService"/>）：官方仅自建开放
/// （父接口零端点 + 仅自建子接口承载端点；openuserid_to_userid 双场景同路由多方法）；
/// <b>corpid 转换族</b>（<see cref="IWechatWorkAccountIdCorpidService"/>）与
/// <b>智能机器人 userid 转换族</b>（<see cref="IWechatWorkAccountIdBotService"/>）：
/// provider_access_token 鉴权、第三方/代开发公共面收敛父接口；
/// <b>ID 迁移完成状态族</b>（<see cref="IWechatWorkAccountIdMigrationService"/>）：
/// provider_access_token 鉴权，第三方/代开发共用端点收敛父接口，第三方子接口另持
/// external_userid 迁移完成差异端点（99375，与父接口同属官方「ID迁移完成状态的设置」页）；
/// <b>群 ID 升级（新授权企业）族</b>（<see cref="IWechatWorkAccountIdChatIdUpgradeService"/>）：
/// suite_access_token 鉴权、仅代开发模板开放（父接口零端点 + 仅代开发子接口承载端点）。
/// </para>
/// <para>
/// 官方契约陷阱（见 AGENTS.md §7.3 同类口径）：external_userid 查询 pending_id 的请求体数组字段名为
/// <c>external_userid</c>（无 <c>_list</c> 后缀）；「未明确企业身份场景」路由沿用
/// <c>userid_to_openuserid</c> 命名但实际转换为 open_userid → userid；openuserid_to_userid
/// 官方双场景（对接 95884 传 source_agentid / 机器人 101521 不传）共用路由；
/// 群 ID 升级第三接口改用代开发模板凭证（suite_access_token）且为 GET。
/// </para>
/// </remarks>
public class WechatAccountIdContractGuards
{
    private const string AccountIdRegistryGroupName = "AccountId";

    /// <summary>
    /// 官方路由表（20 个端点方法；唯一路由 19 条——openuserid_to_userid 被
    /// 「自建应用与第三方/代开发应用的对接」与「自建应用与智能机器人的对接」双场景共用）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // ── ID 转换族·父接口（第三方/代开发公共面 9 端点） ──
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.UserIdToOpenUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/batch/userid_to_openuserid"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.GetNewExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_new_external_userid"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.GetGroupChatNewExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/groupchat/get_new_external_userid"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.ConvertUnionidToExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/idconvert/unionid_to_external_userid"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.ConvertExternalUserIdToPendingIdAsync),
            typeof(PostAttribute), "/cgi-bin/idconvert/batch/external_userid_to_pending_id"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.ConvertExternalTagIdAsync),
            typeof(PostAttribute), "/cgi-bin/idconvert/external_tagid"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.ConvertOpenKfIdAsync),
            typeof(PostAttribute), "/cgi-bin/idconvert/open_kfid"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.GetOpenIdMigrationAsync),
            typeof(PostAttribute), "/cgi-bin/corp/get_openid_migration"),
        (typeof(IWechatWorkAccountIdService), nameof(IWechatWorkAccountIdService.ApplyMassCallTicketAsync),
            typeof(GetAttribute), "/cgi-bin/corp/apply_mass_call_ticket"),
        // ── ID 转换族·代开发子接口差异端点（群 ID 升级，99601） ──
        (typeof(IWechatWorkProviderAccountIdService), nameof(IWechatWorkProviderAccountIdService.ApplyToUpgradeChatIdAsync),
            typeof(PostAttribute), "/cgi-bin/idconvert/apply_to_upgrade_chatid"),
        (typeof(IWechatWorkProviderAccountIdService), nameof(IWechatWorkProviderAccountIdService.ConvertChatIdAsync),
            typeof(PostAttribute), "/cgi-bin/idconvert/chatid"),
        // ── tmp_external_userid 转换族·父接口（三类应用公共面 1 端点，98729/98412/98741） ──
        (typeof(IWechatWorkAccountIdTmpExternalUserIdService), nameof(IWechatWorkAccountIdTmpExternalUserIdService.ConvertTmpExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/idconvert/convert_tmp_external_userid"),
        // ── 自建应用对接族·自建子接口（3 端点，95884/101521；openuserid_to_userid 双场景同路由多方法） ──
        (typeof(IWechatWorkInternalAccountIdInteropService), nameof(IWechatWorkInternalAccountIdInteropService.OpenUserIdToUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/batch/openuserid_to_userid"),
        (typeof(IWechatWorkInternalAccountIdInteropService), nameof(IWechatWorkInternalAccountIdInteropService.OpenUserIdToUserIdForBotAsync),
            typeof(PostAttribute), "/cgi-bin/batch/openuserid_to_userid"),
        (typeof(IWechatWorkInternalAccountIdInteropService), nameof(IWechatWorkInternalAccountIdInteropService.ConvertFromServiceExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/from_service_external_userid"),
        // ── corpid 转换族·父接口（provider_access_token，97061/97105） ──
        (typeof(IWechatWorkAccountIdCorpidService), nameof(IWechatWorkAccountIdCorpidService.CorpidToOpenCorpidAsync),
            typeof(PostAttribute), "/cgi-bin/service/corpid_to_opencorpid"),
        // ── ID 迁移完成状态族·父接口（provider_access_token，99375/99378） ──
        (typeof(IWechatWorkAccountIdMigrationService), nameof(IWechatWorkAccountIdMigrationService.FinishOpenIdMigrationAsync),
            typeof(PostAttribute), "/cgi-bin/service/finish_openid_migration"),
        // ── ID 迁移完成状态族·第三方子接口差异端点（99375，与父接口同页） ──
        (typeof(IWechatWorkThirdPartyAccountIdMigrationService), nameof(IWechatWorkThirdPartyAccountIdMigrationService.FinishExternalUserIdMigrationAsync),
            typeof(PostAttribute), "/cgi-bin/service/externalcontact/finish_external_userid_migration"),
        // ── 智能机器人 userid 转换族·父接口（provider_access_token，96516/97106 未明确企业身份场景） ──
        (typeof(IWechatWorkAccountIdBotService), nameof(IWechatWorkAccountIdBotService.ServiceUserIdToOpenUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/service/batch/userid_to_openuserid"),
        // ── 群 ID 升级（新授权企业）族·代开发子接口（suite_access_token，99601 接口三） ──
        (typeof(IWechatWorkProviderAccountIdChatIdUpgradeService), nameof(IWechatWorkProviderAccountIdChatIdUpgradeService.UpgradeChatIdForNewCorpAsync),
            typeof(GetAttribute), "/cgi-bin/idconvert/upgrade_chatid_for_new_corp"),
    };

    /// <summary>
    /// 契约守卫 ACCT1：账号ID域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void AccountIdEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(20,
            "官方账号ID目录共 20 个端点方法（ID 转换族 11 + tmp 转换族 1 + 自建对接族 3 + " +
            "corpid 转换族 1 + 迁移完成状态族 2 + 智能机器人族 1 + 群 ID 升级族 1）");

        Routes.Select(r => $"{r.Interface.Name}.{r.Method}").Should().OnlyHaveUniqueItems(
            "各端点方法（接口 + 方法名）不得重复");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(19,
            "唯一路由 19 条：openuserid_to_userid 被 95884（对接）与 101521（智能机器人）双场景共用");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：端点必须落在其归属接口自身声明，不得上浮/下沉重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 ACCT2：七接口族层级与生成器注册形态（父接口 IsAbstract、子接口注册组与 InheritedFrom、
    /// 各接口端点数、继承链能力漂移守卫）。
    /// </summary>
    [Fact]
    public void AccountIdInterfaceHierarchy_ShouldConvergeOnAccountIdRegistry()
    {
        // ── ID 转换族：第三方/代开发公共面收敛父接口；第三方空标记；代开发持群 ID 升级 2 差异端点。 ──
        var conversionParent = typeof(IWechatWorkAccountIdService);
        var conversionChildren = new[]
        {
            typeof(IWechatWorkThirdPartyAccountIdService),
            typeof(IWechatWorkProviderAccountIdService),
        };
        AssertAbstractParentWithEndpoints(conversionParent, 9, "官方自建应用无 ID 转换族端点，故不设自建子接口，公共面仅第三方/代开发");
        AssertRegistryChild(conversionChildren[0], 0, "WechatWorkAccountIdService", "第三方子接口为空标记");
        AssertRegistryChild(conversionChildren[1], 2, "WechatWorkAccountIdService",
            "代开发子接口恰持群 ID 升级 2 差异端点（apply_to_upgrade_chatid + chatid，99601）");
        AssertDerivedInterfaces(conversionParent, conversionChildren, "ID 转换族继承链上不得出现其它应用类型子接口");

        // ── tmp_external_userid 转换族：三类应用公共面收敛父接口，三子接口空标记。 ──
        var tmpParent = typeof(IWechatWorkAccountIdTmpExternalUserIdService);
        var tmpChildren = new[]
        {
            typeof(IWechatWorkInternalAccountIdTmpExternalUserIdService),
            typeof(IWechatWorkThirdPartyAccountIdTmpExternalUserIdService),
            typeof(IWechatWorkProviderAccountIdTmpExternalUserIdService),
        };
        AssertAbstractParentWithEndpoints(tmpParent, 1, "tmp_external_userid 转换为三类应用公共面，端点收敛父接口");
        foreach (var child in tmpChildren)
        {
            AssertRegistryChild(child, 0, "WechatWorkAccountIdTmpExternalUserIdService", $"{child.Name} 为空标记子接口");
        }

        AssertDerivedInterfaces(tmpParent, tmpChildren, "tmp 转换族继承链上不得出现其它应用类型子接口");

        // ── 自建应用对接族：官方仅自建开放，父接口零端点 + 仅自建子接口承载端点。 ──
        var interopParent = typeof(IWechatWorkAccountIdInteropService);
        var interopChild = typeof(IWechatWorkInternalAccountIdInteropService);
        AssertAbstractParentWithEndpoints(interopParent, 0, "官方仅自建开放，父接口零端点");
        AssertRegistryChild(interopChild, 3, "WechatWorkAccountIdInteropService",
            "自建子接口恰持 3 端点（openuserid_to_userid 双场景多方法 + from_service_external_userid）");
        AssertDerivedInterfaces(interopParent, new[] { interopChild }, "自建对接族继承链上不得出现第三方/代开发子接口");

        // ── corpid 转换族：provider_access_token，第三方/代开发公共面收敛父接口，子接口空标记。 ──
        var corpidParent = typeof(IWechatWorkAccountIdCorpidService);
        var corpidChildren = new[]
        {
            typeof(IWechatWorkThirdPartyAccountIdCorpidService),
            typeof(IWechatWorkProviderAccountIdCorpidService),
        };
        AssertAbstractParentWithEndpoints(corpidParent, 1, "corpid 转换为第三方/代开发公共面，端点收敛父接口");
        foreach (var child in corpidChildren)
        {
            AssertRegistryChild(child, 0, "WechatWorkAccountIdCorpidService", $"{child.Name} 为空标记子接口");
        }

        AssertDerivedInterfaces(corpidParent, corpidChildren, "corpid 转换族继承链上不得出现其它应用类型子接口");

        // ── ID 迁移完成状态族：第三方/代开发共用端点收敛父接口；第三方持 external_userid 迁移完成差异端点。 ──
        var migrationParent = typeof(IWechatWorkAccountIdMigrationService);
        var migrationChildren = new[]
        {
            typeof(IWechatWorkThirdPartyAccountIdMigrationService),
            typeof(IWechatWorkProviderAccountIdMigrationService),
        };
        AssertAbstractParentWithEndpoints(migrationParent, 1, "设置迁移完成（finish_openid_migration）为第三方/代开发公共面");
        AssertRegistryChild(migrationChildren[0], 1, "WechatWorkAccountIdMigrationService",
            "第三方子接口恰持 external_userid 迁移完成 1 差异端点（99375）");
        AssertRegistryChild(migrationChildren[1], 0, "WechatWorkAccountIdMigrationService", "代开发子接口为空标记");
        AssertDerivedInterfaces(migrationParent, migrationChildren, "迁移完成状态族继承链上不得出现其它应用类型子接口");

        // ── 智能机器人 userid 转换族：provider_access_token，第三方/代开发公共面收敛父接口，子接口空标记。 ──
        var botParent = typeof(IWechatWorkAccountIdBotService);
        var botChildren = new[]
        {
            typeof(IWechatWorkThirdPartyAccountIdBotService),
            typeof(IWechatWorkProviderAccountIdBotService),
        };
        AssertAbstractParentWithEndpoints(botParent, 1, "未明确企业身份场景为第三方/代开发公共面，端点收敛父接口");
        foreach (var child in botChildren)
        {
            AssertRegistryChild(child, 0, "WechatWorkAccountIdBotService", $"{child.Name} 为空标记子接口");
        }

        AssertDerivedInterfaces(botParent, botChildren, "智能机器人族继承链上不得出现其它应用类型子接口");

        // ── 群 ID 升级（新授权企业）族：suite_access_token，仅代开发模板开放，父接口零端点 + 仅代开发子接口承载端点。 ──
        var chatIdUpgradeParent = typeof(IWechatWorkAccountIdChatIdUpgradeService);
        var chatIdUpgradeChild = typeof(IWechatWorkProviderAccountIdChatIdUpgradeService);
        AssertAbstractParentWithEndpoints(chatIdUpgradeParent, 0, "官方仅代开发模板开放，父接口零端点");
        AssertRegistryChild(chatIdUpgradeChild, 1, "WechatWorkAccountIdChatIdUpgradeService",
            "代开发子接口恰持 upgrade_chatid_for_new_corp 1 端点（99601 接口三）");
        AssertDerivedInterfaces(chatIdUpgradeParent, new[] { chatIdUpgradeChild },
            "群 ID 升级族继承链上不得出现自建/第三方子接口");
    }

    /// <summary>
    /// 契约守卫 ACCT3：令牌绑定——九接口消费 AccessToken、九接口消费 ProviderAccessToken、
    /// 两接口消费 SuiteAccessToken，全部以 Query 注入（官方契约 access_token / provider_access_token /
    /// suite_access_token，MUD005 已知接受风险）。
    /// </summary>
    [Fact]
    public void AccountIdTokenBinding_ShouldMatchOfficialTokenTypes()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkAccountIdService),
            typeof(IWechatWorkThirdPartyAccountIdService),
            typeof(IWechatWorkProviderAccountIdService),
            typeof(IWechatWorkAccountIdTmpExternalUserIdService),
            typeof(IWechatWorkInternalAccountIdTmpExternalUserIdService),
            typeof(IWechatWorkThirdPartyAccountIdTmpExternalUserIdService),
            typeof(IWechatWorkProviderAccountIdTmpExternalUserIdService),
            typeof(IWechatWorkAccountIdInteropService),
            typeof(IWechatWorkInternalAccountIdInteropService),
        };
        var providerTokenInterfaces = new[]
        {
            typeof(IWechatWorkAccountIdCorpidService),
            typeof(IWechatWorkThirdPartyAccountIdCorpidService),
            typeof(IWechatWorkProviderAccountIdCorpidService),
            typeof(IWechatWorkAccountIdMigrationService),
            typeof(IWechatWorkThirdPartyAccountIdMigrationService),
            typeof(IWechatWorkProviderAccountIdMigrationService),
            typeof(IWechatWorkAccountIdBotService),
            typeof(IWechatWorkThirdPartyAccountIdBotService),
            typeof(IWechatWorkProviderAccountIdBotService),
        };
        var suiteTokenInterfaces = new[]
        {
            typeof(IWechatWorkAccountIdChatIdUpgradeService),
            typeof(IWechatWorkProviderAccountIdChatIdUpgradeService),
        };

        foreach (var (interfaces, tokenType, queryName) in new[]
                 {
                     (accessTokenInterfaces, WechatTokenTypes.AccessToken, "access_token"),
                     (providerTokenInterfaces, WechatTokenTypes.ProviderAccessToken, "provider_access_token"),
                     (suiteTokenInterfaces, WechatTokenTypes.SuiteAccessToken, "suite_access_token"),
                 })
        {
            foreach (var iface in interfaces)
            {
                var token = iface.GetCustomAttribute<TokenAttribute>();
                token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
                token!.TokenType.Should().Be(tokenType,
                    $"{iface.Name} 令牌路由键必须为 {tokenType}（按应用上下文/scope/服务商/模板路由）");
                token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                    $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
                token.Name.Should().Be(queryName, $"{iface.Name} Query 注入参数名必须为官方契约的 {queryName}");
            }
        }
    }

    /// <summary>
    /// 契约守卫 ACCT4：账号ID域的请求/响应/嵌套 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void AccountIdDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = AccountIdJsonContext.Default;

        var requiredTypes = new[]
        {
            // ID 转换族。
            typeof(UserIdToOpenUserIdRequest), typeof(UserIdToOpenUserIdResponse),
            typeof(GetNewExternalUserIdRequest), typeof(GetNewExternalUserIdResponse),
            typeof(GetGroupChatNewExternalUserIdRequest), typeof(GetGroupChatNewExternalUserIdResponse),
            typeof(ConvertUnionidToExternalUserIdRequest), typeof(ConvertUnionidToExternalUserIdResponse),
            typeof(ConvertExternalUserIdToPendingIdRequest), typeof(ConvertExternalUserIdToPendingIdResponse),
            typeof(ExternalTagIdConvertRequest), typeof(ExternalTagIdConvertResponse),
            typeof(OpenKfIdConvertRequest), typeof(OpenKfIdConvertResponse),
            typeof(GetOpenIdMigrationResponse), typeof(ApplyMassCallTicketResponse),
            typeof(ApplyToUpgradeChatIdRequest), typeof(ConvertChatIdRequest), typeof(ConvertChatIdResponse),
            // tmp_external_userid 转换族。
            typeof(ConvertTmpExternalUserIdRequest), typeof(ConvertTmpExternalUserIdResponse),
            typeof(TmpExternalUserIdResultItem),
            // 自建应用对接族。
            typeof(OpenUserIdToUserIdRequest), typeof(OpenUserIdToUserIdForBotRequest),
            typeof(OpenUserIdToUserIdResponse), typeof(FromServiceExternalUserIdRequest),
            typeof(FromServiceExternalUserIdResponse),
            // corpid 转换族。
            typeof(CorpidToOpenCorpidRequest), typeof(CorpidToOpenCorpidResponse),
            // ID 迁移完成状态族。
            typeof(FinishOpenIdMigrationRequest), typeof(FinishExternalUserIdMigrationRequest),
            typeof(OpenIdMigrationInfoItem),
            // 智能机器人 userid 转换族。
            typeof(ServiceUserIdToOpenUserIdRequest), typeof(ServiceUserIdToOpenUserIdResponse),
            // 群 ID 升级（新授权企业）族。
            typeof(ChatIdConvertItem),
            // 共享嵌套项。
            typeof(OpenUserIdMapItem), typeof(NewExternalUserIdItem),
            typeof(ExternalUserIdPendingIdItem), typeof(ExternalTagIdConvertItem), typeof(OpenKfIdConvertItem),
        };

        requiredTypes.Should().HaveCount(40, "账号ID域契约面共 40 个 DTO 类型");
        requiredTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是账号ID域契约面类型，必须登记进 AccountIdJsonContext（AOT 源生成）");
        }
    }

    /// <summary>断言父接口：IsAbstract、不进注册组、DeclaredOnly 端点数精确匹配。</summary>
    private static void AssertAbstractParentWithEndpoints(Type parent, int endpointCount, string because)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(endpointCount, $"{parent.Name} 端点数漂移：{because}");
    }

    /// <summary>断言子接口：注册组、InheritedFrom 与 DeclaredOnly 端点数。</summary>
    private static void AssertRegistryChild(Type child, int endpointCount, string implementationClassName, string because)
    {
        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(AccountIdRegistryGroupName,
            $"{child.Name} 必须挂 {AccountIdRegistryGroupName} 注册组（经 Add{AccountIdRegistryGroupName}Api() 注册）");
        childApi.InheritedFrom.Should().Be(implementationClassName,
            $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(endpointCount, $"{child.Name} 端点数漂移：{because}");
    }

    /// <summary>能力漂移守卫：父接口继承链上的子接口集合必须精确匹配。</summary>
    private static void AssertDerivedInterfaces(Type parent, Type[] expectedChildren, string because)
    {
        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .ToList();
        derived.Should().BeEquivalentTo(expectedChildren, because);
    }
}
