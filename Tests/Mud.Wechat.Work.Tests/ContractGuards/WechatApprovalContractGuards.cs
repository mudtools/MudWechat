// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 审批模块（Approval 模块）契约守卫：路由表、接口层级、开放面差异与令牌绑定锁定
/// （审批申请数据域：3 个端点为三类应用公共面 + 差异端点子接口——获取审批数据（旧）官方仅自建；
/// 审批模板域：获取模板详情三类公共，创建/更新模板自建与代开发开放（官方对第三方标注暂不支持）、
/// 复制/更新模板到企业官方仅第三方；假期管理域 + 审批流程引擎域：三类应用公共面收敛父接口 + 空标记子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：审批申请数据域与审批模板域对齐数据与智能专区基础接口域（公共面收敛 + 差异端点子接口）；
/// 假期管理域与审批流程引擎域对齐素材管理域（公共面收敛 + 空标记子接口）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：审批全部 12 条路由中除假期管理 getcorpconf 官方即 GET 外全部官方即 POST
/// （含仅查询语义的 getapprovalinfo / getapprovaldetail / gettemplatedetail / getopenapprovaldata）；
/// 批量获取审批单号官方请求示例的 starttime / endtime 按字符串传输（如 "1569546000"），照抄为字符串；
/// 获取审批模板详情响应的 selector 选项 value 为数组、创建/更新模板请求示例的 selector 选项 value 为单对象，
/// 两形态官方不一致，分别以各自示例为准（ApprovalTemplateSelectorOption / ApprovalTemplateSettingSelectorOption）；
/// 获取审批申请详情响应字段官方拼写为 notifyer（非 notifier）、备注人字段为驼峰 commentUserInfo；
/// 审批流程引擎查询端点响应为官方原文 PascalCase 字段（OpenSpstatus / ApplyUsername / ItemUserId），
/// 请求字段为官方原文驼峰 thirdNo；获取审批数据（旧）响应字段官方拼写为 spname / mediaids。
/// </para>
/// </remarks>
public class WechatApprovalContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ApprovalParentImplementationClassName = "WechatWorkApprovalService";

    private const string ApprovalTemplateParentImplementationClassName = "WechatWorkApprovalTemplateService";

    private const string VacationParentImplementationClassName = "WechatWorkVacationService";

    private const string ApprovalEngineParentImplementationClassName = "WechatWorkApprovalEngineService";

    private const string ApprovalRegistryGroupName = "Approval";

    /// <summary>
    /// 审批申请数据域官方路由表（三类应用公共面 3 条端点全部收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ApprovalRoutes =
    {
        // 提交审批申请（自建 91853、第三方 92632、代开发 96507；官方即 POST）。
        (typeof(IWechatWorkApprovalService),
            nameof(IWechatWorkApprovalService.ApplyApprovalAsync),
            typeof(PostAttribute), "/cgi-bin/oa/applyevent"),
        // 批量获取审批单号（自建 91816、第三方 94603、代开发 96509；官方即 POST）。
        (typeof(IWechatWorkApprovalService),
            nameof(IWechatWorkApprovalService.BatchGetApprovalNumbersAsync),
            typeof(PostAttribute), "/cgi-bin/oa/getapprovalinfo"),
        // 获取审批申请详情（自建 91983、第三方 92634、代开发 96510；官方即 POST）。
        (typeof(IWechatWorkApprovalService),
            nameof(IWechatWorkApprovalService.GetApprovalDetailAsync),
            typeof(PostAttribute), "/cgi-bin/oa/getapprovaldetail"),
    };

    /// <summary>
    /// 审批申请数据域差异端点官方路由表（获取审批数据（旧）官方仅自建应用开放，
    /// 第三方/代开发文档未提供该端点；由唯一自建子接口承载）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ApprovalLegacyRoutes =
    {
        // 获取审批数据（旧）（91530；官方即 POST；官方推荐使用 getapprovalinfo / getapprovaldetail，本端点后续将不再维护）。
        (typeof(IWechatWorkInternalApprovalService),
            nameof(IWechatWorkInternalApprovalService.GetApprovalDataAsync),
            typeof(PostAttribute), "/cgi-bin/corp/getapprovaldata"),
    };

    /// <summary>
    /// 审批模板域公共面官方路由表（获取审批模板详情，三类应用公共面，收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ApprovalTemplateRoutes =
    {
        // 获取审批模板详情（自建 91982、第三方 92631、代开发 96506；官方即 POST）。
        (typeof(IWechatWorkApprovalTemplateService),
            nameof(IWechatWorkApprovalTemplateService.GetTemplateDetailAsync),
            typeof(PostAttribute), "/cgi-bin/oa/gettemplatedetail"),
    };

    /// <summary>
    /// 审批模板域差异端点官方路由表（创建/更新审批模板：自建 97437/97438 + 代开发 97439/97440，
    /// 官方权限表对第三方应用标注暂不支持；复制/更新模板到企业：官方仅第三方 92630）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ApprovalTemplateDiffRoutes =
    {
        // 创建审批模板（自建 97437、代开发 97439；官方即 POST；第三方应用官方标注暂不支持）。
        (typeof(IWechatWorkInternalApprovalTemplateService),
            nameof(IWechatWorkInternalApprovalTemplateService.CreateTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/oa/approval/create_template"),
        // 更新审批模板（自建 97438、代开发 97440；官方即 POST；第三方应用官方标注暂不支持）。
        (typeof(IWechatWorkInternalApprovalTemplateService),
            nameof(IWechatWorkInternalApprovalTemplateService.UpdateTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/oa/approval/update_template"),
        // 创建审批模板（代开发 97439）。
        (typeof(IWechatWorkProviderApprovalTemplateService),
            nameof(IWechatWorkProviderApprovalTemplateService.CreateTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/oa/approval/create_template"),
        // 更新审批模板（代开发 97440）。
        (typeof(IWechatWorkProviderApprovalTemplateService),
            nameof(IWechatWorkProviderApprovalTemplateService.UpdateTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/oa/approval/update_template"),
        // 复制/更新模板到企业（92630；官方仅第三方应用开放）。
        (typeof(IWechatWorkThirdPartyApprovalTemplateService),
            nameof(IWechatWorkThirdPartyApprovalTemplateService.CopyTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/oa/approval/copytemplate"),
    };

    /// <summary>
    /// 假期管理域官方路由表（三类应用公共面，3 条端点全部收敛父接口；
    /// getcorpconf 官方即 GET，其余官方即 POST，勿「顺手统一」）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] VacationRoutes =
    {
        // 获取企业假期管理配置（自建 93375、第三方 94211、代开发 96512；官方即 GET，无请求参数）。
        (typeof(IWechatWorkVacationService),
            nameof(IWechatWorkVacationService.GetCorpVacationConfAsync),
            typeof(GetAttribute), "/cgi-bin/oa/vacation/getcorpconf"),
        // 获取成员假期余额（自建 93376、第三方 94212、代开发 96513；官方即 POST）。
        (typeof(IWechatWorkVacationService),
            nameof(IWechatWorkVacationService.GetUserVacationQuotaAsync),
            typeof(PostAttribute), "/cgi-bin/oa/vacation/getuservacationquota"),
        // 修改成员假期余额（自建 93377、第三方 94213、代开发 96514；官方即 POST）。
        (typeof(IWechatWorkVacationService),
            nameof(IWechatWorkVacationService.SetUserVacationQuotaAsync),
            typeof(PostAttribute), "/cgi-bin/oa/vacation/setoneuserquota"),
    };

    /// <summary>
    /// 审批流程引擎域官方路由表（三类应用公共面，单端点收敛父接口；
    /// 审批单据体系（thirdNo / OpenTemplateId）与「审批应用」单据体系（sp_no / template_id）相互独立）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ApprovalEngineRoutes =
    {
        // 查询审批单当前状态（自建 90269、第三方 93798、代开发 97114；官方即 POST）。
        (typeof(IWechatWorkApprovalEngineService),
            nameof(IWechatWorkApprovalEngineService.GetOpenApprovalDataAsync),
            typeof(PostAttribute), "/cgi-bin/corp/getopenapprovaldata"),
    };

    /// <summary>
    /// 契约守卫 AP1a：审批申请数据域公共面全部端点路由必须与官方契约一致
    /// （3 个端点为三类应用公共面，全部收敛父接口；官方即 POST，勿「顺手统一」为 GET）。
    /// </summary>
    [Fact]
    public void ApprovalEndpoints_ShouldMatchOfficialRoutes()
    {
        ApprovalRoutes.Should().HaveCount(3,
            "审批申请数据域 3 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = ApprovalRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "审批申请数据域各端点路由互不重复");

        AssertRoutes(ApprovalRoutes);
    }

    /// <summary>
    /// 契约守卫 AP1b：获取审批数据（旧）官方仅自建应用开放，
    /// 端点必须由唯一自建子接口承载；第三方/代开发子接口不得声明该端点（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void ApprovalLegacyEndpoint_ShouldBeInternalOnly()
    {
        ApprovalLegacyRoutes.Should().HaveCount(1,
            "获取审批数据（旧）官方仅自建应用开放");

        AssertRoutes(ApprovalLegacyRoutes);

        typeof(IWechatWorkProviderApprovalService)
            .GetMethod(nameof(IWechatWorkInternalApprovalService.GetApprovalDataAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取审批数据（旧）官方仅自建开放，代开发子接口不得声明该端点");
        typeof(IWechatWorkThirdPartyApprovalService)
            .GetMethod(nameof(IWechatWorkInternalApprovalService.GetApprovalDataAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取审批数据（旧）官方仅自建开放，第三方子接口不得声明该端点");
    }

    /// <summary>
    /// 契约守卫 AP1c：审批模板域公共面与差异端点全部端点路由必须与官方契约一致
    /// （获取模板详情三类公共；创建/更新模板自建与代开发开放、官方对第三方标注暂不支持；
    /// 复制/更新模板到企业官方仅第三方）。
    /// </summary>
    [Fact]
    public void ApprovalTemplateEndpoints_ShouldMatchOfficialRoutes()
    {
        ApprovalTemplateRoutes.Should().HaveCount(1,
            "获取审批模板详情为三类应用公共面，收敛父接口");
        ApprovalTemplateDiffRoutes.Should().HaveCount(5,
            "创建/更新审批模板由自建与代开发子接口各承载 2 条、复制/更新模板到企业由第三方子接口承载 1 条");

        AssertRoutes(ApprovalTemplateRoutes);
        AssertRoutes(ApprovalTemplateDiffRoutes);

        // 官方权限表对第三方应用标注暂不支持：第三方子接口不得声明创建/更新审批模板。
        typeof(IWechatWorkThirdPartyApprovalTemplateService)
            .GetMethod(nameof(IWechatWorkInternalApprovalTemplateService.CreateTemplateAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("创建审批模板官方对第三方应用标注暂不支持，第三方子接口不得声明该端点");
        typeof(IWechatWorkThirdPartyApprovalTemplateService)
            .GetMethod(nameof(IWechatWorkInternalApprovalTemplateService.UpdateTemplateAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("更新审批模板官方对第三方应用标注暂不支持，第三方子接口不得声明该端点");
        // 复制/更新模板到企业官方仅第三方开放：自建/代开发子接口不得声明。
        typeof(IWechatWorkInternalApprovalTemplateService)
            .GetMethod(nameof(IWechatWorkThirdPartyApprovalTemplateService.CopyTemplateAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("复制/更新模板到企业官方仅第三方开放，自建子接口不得声明该端点");
        typeof(IWechatWorkProviderApprovalTemplateService)
            .GetMethod(nameof(IWechatWorkThirdPartyApprovalTemplateService.CopyTemplateAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("复制/更新模板到企业官方仅第三方开放，代开发子接口不得声明该端点");
    }

    /// <summary>
    /// 契约守卫 AP1d：假期管理域全部端点路由必须与官方契约一致
    /// （3 个端点为三类应用公共面，全部收敛父接口；
    /// getcorpconf 官方即 GET 且无请求参数，其余官方即 POST，勿「顺手统一」）。
    /// </summary>
    [Fact]
    public void VacationEndpoints_ShouldMatchOfficialRoutes()
    {
        VacationRoutes.Should().HaveCount(3,
            "假期管理域 3 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = VacationRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "假期管理域各端点路由互不重复");
        distinctRoutes.Should().OnlyContain(r => r.StartsWith("/cgi-bin/oa/vacation/", StringComparison.Ordinal),
            "假期管理域全部端点路由位于 /cgi-bin/oa/vacation/ 段");

        AssertRoutes(VacationRoutes);

        // getcorpconf 官方即 GET 且无请求参数：不得声明 [Body] 参数。
        var getCorpConf = typeof(IWechatWorkVacationService).GetMethod(
            nameof(IWechatWorkVacationService.GetCorpVacationConfAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        getCorpConf.Should().NotBeNull();
        getCorpConf!.GetParameters()
            .Any(p => p.GetCustomAttribute<BodyAttribute>() is not null)
            .Should().BeFalse("getcorpconf 官方即 GET 且无请求参数，不得声明 [Body] 参数");
    }

    /// <summary>
    /// 契约守卫 AP1e：审批流程引擎域端点路由必须与官方契约一致
    /// （单端点为三类应用公共面，收敛父接口；官方即 POST）。
    /// </summary>
    [Fact]
    public void ApprovalEngineEndpoints_ShouldMatchOfficialRoutes()
    {
        ApprovalEngineRoutes.Should().HaveCount(1,
            "审批流程引擎域查询审批单当前状态为三类应用公共面，收敛父接口");

        AssertRoutes(ApprovalEngineRoutes);
    }

    /// <summary>
    /// 契约守卫 AP2：审批模块接口层级与生成器注册形态——公共端点收敛于 IsAbstract 父接口，
    /// 差异端点仅由官方开放面对应的子接口承载、空标记子接口不新增端点（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void ApprovalInterfaceHierarchy_ShouldConvergeOnAbstractParentWithApprovalRegistry()
    {
        AssertFamilyHierarchy(
            typeof(IWechatWorkApprovalService),
            ApprovalParentImplementationClassName,
            expectedDeclaredMethods: 3,
            new[] { (typeof(IWechatWorkInternalApprovalService), 1),
                    (typeof(IWechatWorkProviderApprovalService), 0),
                    (typeof(IWechatWorkThirdPartyApprovalService), 0) });

        AssertFamilyHierarchy(
            typeof(IWechatWorkApprovalTemplateService),
            ApprovalTemplateParentImplementationClassName,
            expectedDeclaredMethods: 1,
            new[] { (typeof(IWechatWorkInternalApprovalTemplateService), 2),
                    (typeof(IWechatWorkProviderApprovalTemplateService), 2),
                    (typeof(IWechatWorkThirdPartyApprovalTemplateService), 1) });

        AssertFamilyHierarchy(
            typeof(IWechatWorkVacationService),
            VacationParentImplementationClassName,
            expectedDeclaredMethods: 3,
            new[] { (typeof(IWechatWorkInternalVacationService), 0),
                    (typeof(IWechatWorkProviderVacationService), 0),
                    (typeof(IWechatWorkThirdPartyVacationService), 0) });

        AssertFamilyHierarchy(
            typeof(IWechatWorkApprovalEngineService),
            ApprovalEngineParentImplementationClassName,
            expectedDeclaredMethods: 1,
            new[] { (typeof(IWechatWorkInternalApprovalEngineService), 0),
                    (typeof(IWechatWorkProviderApprovalEngineService), 0),
                    (typeof(IWechatWorkThirdPartyApprovalEngineService), 0) });
    }

    /// <summary>
    /// 契约守卫 AP3：令牌绑定——审批模块四族 16 个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；自建为应用自身令牌，代开发/第三方为授权企业级令牌 scope = authCorpId）。
    /// </summary>
    [Fact]
    public void ApprovalTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkApprovalService),
            typeof(IWechatWorkInternalApprovalService),
            typeof(IWechatWorkProviderApprovalService),
            typeof(IWechatWorkThirdPartyApprovalService),
            typeof(IWechatWorkApprovalTemplateService),
            typeof(IWechatWorkInternalApprovalTemplateService),
            typeof(IWechatWorkProviderApprovalTemplateService),
            typeof(IWechatWorkThirdPartyApprovalTemplateService),
            typeof(IWechatWorkVacationService),
            typeof(IWechatWorkInternalVacationService),
            typeof(IWechatWorkProviderVacationService),
            typeof(IWechatWorkThirdPartyVacationService),
            typeof(IWechatWorkApprovalEngineService),
            typeof(IWechatWorkInternalApprovalEngineService),
            typeof(IWechatWorkProviderApprovalEngineService),
            typeof(IWechatWorkThirdPartyApprovalEngineService),
        };

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 AP4：审批模块的请求/响应 DTO 必须全部登记进 AOT JSON 上下文
    /// （109 个契约面类型落 ApprovalJsonContext，SerializerClassName 统一为 Approval）；
    /// 更新审批模板 / 修改成员假期余额官方响应仅 errcode/errmsg，直接复用
    /// <see cref="WechatWorkResponse"/>，获取企业假期管理配置官方即 GET 无请求包体、无请求 DTO。
    /// </summary>
    [Fact]
    public void ApprovalDataModels_ShouldBeRegisteredInJsonContext()
    {
        var approvalContext = ApprovalJsonContext.Default;

        // 端点级请求/响应 DTO 全量清单（守卫是权威描述，新增/删减端点须同批更新）。
        var endpointContractTypes = new Type[]
        {
            // 提交审批申请。
            typeof(ApplyApprovalRequest), typeof(ApplyApprovalResponse),
            // 批量获取审批单号。
            typeof(BatchGetApprovalNumbersRequest), typeof(BatchGetApprovalNumbersResponse), typeof(ApprovalInfoFilter),
            // 获取审批申请详情。
            typeof(GetApprovalDetailRequest), typeof(GetApprovalDetailResponse),
            // 获取审批数据（旧）。
            typeof(GetApprovalDataRequest), typeof(GetApprovalDataResponse),
            // 获取审批模板详情。
            typeof(GetApprovalTemplateDetailRequest), typeof(GetApprovalTemplateDetailResponse),
            // 创建/更新审批模板。
            typeof(CreateApprovalTemplateRequest), typeof(CreateApprovalTemplateResponse), typeof(UpdateApprovalTemplateRequest),
            // 复制/更新模板到企业。
            typeof(CopyApprovalTemplateRequest), typeof(CopyApprovalTemplateResponse),
            // 假期管理。
            typeof(GetVacationUserQuotaRequest), typeof(GetVacationUserQuotaResponse), typeof(SetVacationUserQuotaRequest),
            typeof(GetVacationCorpConfResponse),
            // 审批流程引擎。
            typeof(GetOpenApprovalDataRequest), typeof(GetOpenApprovalDataResponse),
        };

        foreach (var type in endpointContractTypes)
        {
            approvalContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是审批模块端点契约面类型，必须登记进 ApprovalJsonContext（AOT 源生成）");
        }

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 ApprovalJsonContext 且 SerializerClassName 统一为 Approval
        //（生成物 ApprovalJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        var domainTypes = typeof(ApprovalLangTextItem).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Approval"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(109,
            "审批模块契约面类型数漂移须先核对官方文档再同批调整本守卫");
        domainTypes.Should().Contain(endpointContractTypes,
            "端点级请求/响应 DTO 必须落位于审批域命名空间");

        foreach (var type in domainTypes)
        {
            approvalContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于审批域命名空间，必须登记进 ApprovalJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Approval",
                $"{type.Name} 的 SerializerClassName 必须为审批域段 Approval");
        }
    }

    /// <summary>
    /// 契约守卫 AP5：官方契约陷阱锁定——字段名照抄官方原文、请求/响应示例形态不一致处分别以各自示例为准
    /// （批量获取审批单号 starttime/endtime 官方示例按字符串传输；获取审批模板详情响应 selector 选项 value 为数组、
    /// 创建/更新模板请求 selector 选项 value 为单对象；获取审批申请详情 notifyer / commentUserInfo；
    /// 审批流程引擎响应 OpenSpstatus / ApplyUsername / ItemUserId、请求 thirdNo；获取审批数据（旧） spname / mediaids）。
    /// </summary>
    [Fact]
    public void ApprovalDataModels_ShouldLockOfficialContractTraps()
    {
        // 批量获取审批单号：官方请求示例按字符串传输时间戳（如 "1569546000"），照抄为字符串。
        typeof(BatchGetApprovalNumbersRequest).GetProperty(nameof(BatchGetApprovalNumbersRequest.Starttime))!
            .PropertyType.Should().Be(typeof(string),
                "批量获取审批单号 starttime 官方请求示例按字符串传输");
        typeof(BatchGetApprovalNumbersRequest).GetProperty(nameof(BatchGetApprovalNumbersRequest.Endtime))!
            .PropertyType.Should().Be(typeof(string),
                "批量获取审批单号 endtime 官方请求示例按字符串传输");

        // 获取审批模板详情响应 selector 选项 value 为数组（多语言列表）。
        typeof(ApprovalTemplateSelectorOption).GetProperty(nameof(ApprovalTemplateSelectorOption.Value))!
            .PropertyType.Should().Be(typeof(List<ApprovalLangTextItem>),
                "获取审批模板详情响应示例的 selector 选项 value 按数组传输");
        // 创建/更新模板请求 selector 选项 value 为单对象（官方示例与响应侧形态不一致，以各自示例为准）。
        typeof(ApprovalTemplateSettingSelectorOption).GetProperty(nameof(ApprovalTemplateSettingSelectorOption.Value))!
            .PropertyType.Should().Be(typeof(ApprovalLangTextItem),
                "创建/更新审批模板请求示例的 selector 选项 value 按单对象传输");

        // 官方字段名拼写照抄（拼写差异属官方契约，勿「顺手修正」）。
        JsonNameShouldBe(typeof(ApprovalDetailInfo), nameof(ApprovalDetailInfo.Notifyer), "notifyer");
        JsonNameShouldBe(typeof(ApprovalComment), nameof(ApprovalComment.CommentUserInfo), "commentUserInfo");
        JsonNameShouldBe(typeof(GetOpenApprovalDataRequest), nameof(GetOpenApprovalDataRequest.ThirdNo), "thirdNo");
        JsonNameShouldBe(typeof(OpenApprovalData), nameof(OpenApprovalData.OpenSpStatus), "OpenSpstatus");
        JsonNameShouldBe(typeof(OpenApprovalData), nameof(OpenApprovalData.ApplyUsername), "ApplyUsername");
        JsonNameShouldBe(typeof(OpenApprovalNodeItem), nameof(OpenApprovalNodeItem.ItemUserId), "ItemUserId");
        JsonNameShouldBe(typeof(GetApprovalDataItem), nameof(GetApprovalDataItem.Spname), "spname");
        JsonNameShouldBe(typeof(GetApprovalDataItem), nameof(GetApprovalDataItem.Mediaids), "mediaids");

        // 附件控件 file_size 官方文档「类型为number」与「没有可以填空字符串」矛盾，以字符串承载兼容两种形态。
        typeof(ApprovalFileItem).GetProperty(nameof(ApprovalFileItem.FileSize))!
            .PropertyType.Should().Be(typeof(string),
                "file_size 官方文档参数说明自相矛盾，以字符串承载兼容两种形态");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, Type HttpAttribute, string Route)[] routes)
    {
        foreach (var (iface, method, httpAttribute, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 接口层级断言：父接口 IsAbstract 且不进注册组；子接口挂 Approval 注册组、继承父实现类；
    /// 各子接口的自身声明端点数必须与官方开放面一致。
    /// </summary>
    private static void AssertFamilyHierarchy(
        Type parent,
        string parentImplementationClassName,
        int? expectedDeclaredMethods,
        (Type Child, int DeclaredMethods)[] children)
    {
        if (expectedDeclaredMethods.HasValue)
        {
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(expectedDeclaredMethods.Value, $"{parent.Name} 公共端点数漂移");
        }

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var (child, declaredMethods) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(ApprovalRegistryGroupName,
                $"{child.Name} 必须挂 {ApprovalRegistryGroupName} 注册组" +
                $"（Approval 模块共用 Add{ApprovalRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(declaredMethods,
                    $"{child.Name} 自身声明端点数必须与官方开放面一致（差异端点漂移须先核对官方文档再同批调整守卫）");
        }
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致（拼写差异属官方契约）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
