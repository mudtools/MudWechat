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
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.AdvancedFeature;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 企业微信「高级功能成员申请单」（<c>/cgi-bin/advanced_feature/*</c>）域契约守卫（AF 系列）：
/// 3 端点路由表（官方全 POST）、<b>仅自建开放</b>的接口层级、令牌归属域、DTO 上下文登记与官方字段名陷阱。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方开放面（逐页明文标注，勿按「高级」二字误判为服务商面）</b>：99880 / 99883 / 99885 三页的
/// 应用类型表均为「自建应用：配置到『高级功能 - 成员申请管理 - 通过 api 设置申请流程』中；
/// 代开发应用：暂不支持；第三方应用：暂不支持」⇒ 本域只有唯一自建子接口，
/// 第三方 / 代开发（服务商）形态为零端点，由 AF2 锁定不得回潮。
/// 与「高级接口订单」（AA 域，走 <c>provider_access_token</c>）分属官方两棵章节树、两套凭证面。
/// </para>
/// <para>
/// <b>官方反直觉点（勿「顺手修正」）</b>：① 三支端点<b>全部为 POST</b>，含两支查询接口；
/// ② <c>approval_status</c> <b>两套值域不同名同义</b>：设置面 1/2/3/101，查询面 0/1/2/3/4/5
/// （多出「管理员驳回 / 管理员已分配 / 历史无流程记录」），SDK 不归一、各 DTO 照抄原文；
/// ③ 本域 <c>has_more</c> 官方为<b>布尔</b>，而 AA 域为整型 0/1；
/// ④ 审批节点在 <c>process_list.node_list</c> 的「容器 + 列表」包装形态下（非直挂数组字段）；
/// ⑤ 节点字段官方用缩写 <c>node_apv_status</c> / <c>node_apv_rel</c> / <c>apv_update_time</c>。
/// </para>
/// <para>
/// <b>覆盖式写入</b>：官方明确「变更审批节点信息需要<b>全量</b>节点都传入」⇒ 只传部分等于删除其余节点，
/// 且审批 id 与申请 id 一一对应、不可改变（AF1 锁定写面路由与响应形态，防「补增量语义」类误改）。
/// </para>
/// <para>
/// <b>个人信息约束</b>：<c>userid</c> / <c>applicant</c> / <c>current_approvers</c> /
/// <c>completed_approvers</c> 为成员身份，<c>request_reason</c> 为成员填写的自由文本，
/// 均不得写入日志、遥测或异常消息。
/// </para>
/// </remarks>
public class WechatAdvancedFeatureContractGuards
{
    private const string AdvancedFeatureRegistryGroupName = "AdvancedFeature";

    private const string AdvancedFeatureNamespace = "Mud.Wechat.Work.DataModels.AdvancedFeature";

    /// <summary>高级功能申请单域官方路由表（3 端点，全部 POST）。</summary>
    private static readonly (string Method, Type HttpAttribute, string Route, Type Response)[] AdvancedFeatureRoutes =
    {
        (nameof(IWechatWorkAdvancedFeatureService.SetApprovalDetailAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_feature/set_approval_detail", typeof(WechatWorkResponse)),
        (nameof(IWechatWorkAdvancedFeatureService.GetApplyIdListAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_feature/get_apply_id_list", typeof(GetAdvancedFeatureApplyIdListResponse)),
        (nameof(IWechatWorkAdvancedFeatureService.GetApprovalInfoAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_feature/get_approval_info", typeof(GetAdvancedFeatureApprovalInfoResponse)),
    };

    // ------------------------------------------------------------------
    // AF1：路由、HTTP 动词与响应形态必须与官方契约一致。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AF1：本域恒为 3 支端点且<b>全部 POST</b>；设置面成功响应无业务字段
    /// （复用 <see cref="WechatWorkResponse"/>），查询面各自返回官方容器。
    /// </summary>
    [Fact]
    public void AdvancedFeatureEndpoints_ShouldMatchOfficialRoutesAndPostOnlyShape()
    {
        AdvancedFeatureRoutes.Should().HaveCount(3,
            "本域官方仅 set_approval_detail / get_apply_id_list / get_approval_info 三支");
        AdvancedFeatureRoutes.Should().OnlyContain(r => r.HttpAttribute == typeof(PostAttribute),
            "官方三支（含两支查询）恒为 POST —— 勿「顺手」把查询改 GET");

        var iface = typeof(IWechatWorkAdvancedFeatureService);
        foreach (var (method, httpAttribute, route, response) in AdvancedFeatureRoutes)
        {
            var target = iface.GetMethod(method,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须声明于本域父接口");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");

            target.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(response),
                $"{iface.Name}.{method} 的响应形态必须与官方契约一致");

            target.GetParameters().Should().Contain(p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{iface.Name}.{method} 必须以 [Body] 承载请求体");
        }

        // 覆盖式写入面：节点必须挂在 process_list.node_list 包装形态下（官方「容器 + 列表」）。
        JsonNameShouldBe(typeof(SetAdvancedFeatureApprovalDetailRequest),
            nameof(SetAdvancedFeatureApprovalDetailRequest.ProcessList), "process_list");
        JsonNameShouldBe(typeof(AdvancedFeatureApprovalProcessList),
            nameof(AdvancedFeatureApprovalProcessList.NodeList), "node_list");
    }

    // ------------------------------------------------------------------
    // AF2：接口层级（仅自建子接口承载，第三方/代开发零回潮）。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AF2：官方三页应用类型表均标「代开发 / 第三方暂不支持」⇒
    /// 继承链上恰好只有唯一自建子接口，父接口承载全部端点、子接口零端点。
    /// </summary>
    [Fact]
    public void AdvancedFeatureInterfaceHierarchy_ShouldConvergeOnInternalOnlyChild()
    {
        var assignable = typeof(IWechatWorkAdvancedFeatureService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t != typeof(IWechatWorkAdvancedFeatureService)
                        && typeof(IWechatWorkAdvancedFeatureService).IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkInternalAdvancedFeatureService) },
            "官方逐页标注「代开发应用：暂不支持；第三方应用：暂不支持」⇒ 本域继承链上"
            + "不得出现第三方 / 服务商子接口（与高级接口订单 AA 域分属两套凭证面）");

        var parent = typeof(IWechatWorkAdvancedFeatureService);
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.TokenManage.Should().Be(nameof(IWechatAppManager), "各域父接口 TokenManage 形态统一");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(3, $"{parent.Name} 承载本域全部 3 条官方端点");

        var child = typeof(IWechatWorkInternalAdvancedFeatureService);
        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(AdvancedFeatureRegistryGroupName,
            $"{child.Name} 必须挂 {AdvancedFeatureRegistryGroupName} 注册组");
        childApi.InheritedFrom.Should().Be("WechatWorkAdvancedFeatureService",
            $"{child.Name} 必须继承父接口生成实现类");
        childApi.TokenManage.Should().Be(nameof(IWechatAppManager),
            "父子两级 TokenManage 必须统一（否则生成器以 new 隐藏基类切换成员）");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty($"{child.Name} 为标记子接口，端点全部由父接口承载");
    }

    // ------------------------------------------------------------------
    // AF3：自建应用令牌面 + 归属域键。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AF3：父子两级均为 <c>access_token</c>（Query）；归属域键<b>只由自建子接口声明</b>
    /// （父接口不声明，与 TO 系列同规则），且不得出现服务商 / 套件令牌。
    /// </summary>
    [Fact]
    public void AdvancedFeatureInterfaces_ShouldBindInternalAccessTokenOnly()
    {
        var parent = typeof(IWechatWorkAdvancedFeatureService).GetCustomAttribute<TokenAttribute>();
        parent.Should().NotBeNull("父接口必须声明 [Token]（本域走企业 access_token 链路）");
        parent!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
            "本域官方只开放自建应用 ⇒ 令牌类型恒为 Wechat.AccessToken");
        parent.InjectionMode.Should().Be(TokenInjectionMode.Query, "企微官方契约强制 Query 注入（MUD005）");
        parent.Name.Should().Be("access_token", "官方参数名不得漂移");
        parent.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
            "公共父接口不声明归属域键（归属域只在子接口处收敛，TO 系列同规则）");
        parent.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
            "公共父接口不声明归属域键（本域官方只开放自建应用）");

        var child = typeof(IWechatWorkInternalAdvancedFeatureService).GetCustomAttribute<TokenAttribute>();
        child.Should().NotBeNull($"{nameof(IWechatWorkInternalAdvancedFeatureService)} 必须声明 [Token]");
        child!.TokenType.Should().Be(WechatTokenTypes.AccessToken, "TokenType 不得按应用类型拆分");
        child.InjectionMode.Should().Be(TokenInjectionMode.Query, "企微官方契约强制 Query 注入（MUD005）");
        child.Name.Should().Be("access_token", "官方参数名不得漂移");
        child.TokenManagerKey.Should().Be(WechatTokenManagerKeys.InternalAccessToken,
            "自建应用族必须声明归属域键，否则错配无法在 WechatAppContext.GetTokenManager 处 fail-fast");
    }

    // ------------------------------------------------------------------
    // AF4：DTO 上下文登记与官方字段名陷阱。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AF4：本域 8 个顶层 DTO 必须登记进 <c>AdvancedFeatureJsonContext</c>，
    /// <c>SerializerClassName</c> 统一为域段 <c>AdvancedFeature</c>；同步锁定两套状态值域与布尔翻页标识。
    /// </summary>
    [Fact]
    public void AdvancedFeatureDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = AdvancedFeatureJsonContext.Default;

        var endpointContractTypes = new Type[]
        {
            typeof(SetAdvancedFeatureApprovalDetailRequest), typeof(AdvancedFeatureApprovalProcessList),
            typeof(AdvancedFeatureApprovalNode), typeof(GetAdvancedFeatureApplyIdListRequest),
            typeof(GetAdvancedFeatureApplyIdListResponse), typeof(GetAdvancedFeatureApprovalInfoRequest),
            typeof(GetAdvancedFeatureApprovalInfoResponse), typeof(AdvancedFeatureApprovalInfo),
        };

        endpointContractTypes.Should().HaveCount(8, "本域契约面类型清单漂移须先核对官方页面再同批调整本守卫");

        var domainTypes = typeof(GetAdvancedFeatureApprovalInfoResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == AdvancedFeatureNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(8,
            "高级功能申请单域契约面类型数漂移须先核对官方页面再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于高级功能申请单域命名空间，必须登记进 AdvancedFeatureJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("AdvancedFeature",
                $"{type.Name} 的 SerializerClassName 必须为高级功能申请单域段 AdvancedFeature");
        }

        // 官方字段名陷阱锁定（缩写照抄原文，勿「顺手展开」为 approval_*）。
        JsonNameShouldBe(typeof(AdvancedFeatureApprovalNode),
            nameof(AdvancedFeatureApprovalNode.NodeApprovalStatus), "node_apv_status");
        JsonNameShouldBe(typeof(AdvancedFeatureApprovalNode),
            nameof(AdvancedFeatureApprovalNode.NodeApprovalRelation), "node_apv_rel");
        JsonNameShouldBe(typeof(AdvancedFeatureApprovalNode),
            nameof(AdvancedFeatureApprovalNode.ApprovalUpdateTime), "apv_update_time");
        JsonNameShouldBe(typeof(AdvancedFeatureApprovalNode),
            nameof(AdvancedFeatureApprovalNode.CurrentApprovers), "current_approvers");
        JsonNameShouldBe(typeof(AdvancedFeatureApprovalNode),
            nameof(AdvancedFeatureApprovalNode.CompletedApprovers), "completed_approvers");
        JsonNameShouldBe(typeof(GetAdvancedFeatureApplyIdListRequest),
            nameof(GetAdvancedFeatureApplyIdListRequest.RequestType), "req_type");
        JsonNameShouldBe(typeof(GetAdvancedFeatureApprovalInfoResponse),
            nameof(GetAdvancedFeatureApprovalInfoResponse.ApprovalInfo), "approval_info");
        JsonNameShouldBe(typeof(AdvancedFeatureApprovalInfo),
            nameof(AdvancedFeatureApprovalInfo.RequestReason), "request_reason");

        // 本域 has_more 为布尔（AA 域为整型 0/1，勿归一）。
        typeof(GetAdvancedFeatureApplyIdListResponse).GetProperty(nameof(GetAdvancedFeatureApplyIdListResponse.HasMore))!
            .PropertyType.Should().Be(typeof(bool?),
                "官方本域 has_more 为布尔（高级接口订单域为整型 0/1，两域形态各异、SDK 不归一）");

        // 两套 approval_status 值域不同名同义 ⇒ 必须是两个独立属性的同名字段，不得合并为一个枚举类型。
        typeof(SetAdvancedFeatureApprovalDetailRequest).GetProperty(nameof(SetAdvancedFeatureApprovalDetailRequest.ApprovalStatus))!
            .PropertyType.Should().Be(typeof(int?), "设置面 approval_status 为 int?（值域 1/2/3/101）");
        typeof(AdvancedFeatureApprovalInfo).GetProperty(nameof(AdvancedFeatureApprovalInfo.ApprovalStatus))!
            .PropertyType.Should().Be(typeof(int?), "查询面 approval_status 为 int?（值域 0/1/2/3/4/5）");
    }

    /// <summary>JSON 字段名断言（属性映射的官方字段名必须与官方原文一致）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
