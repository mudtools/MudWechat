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
using Mud.Wechat.Work.DataModels.Service;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 企业微信「代开发应用授权企业数据接口」（<c>/cgi-bin/service/*</c>）域契约守卫（SV 系列）：
/// 5 端点路由表、<b>纯服务商凭证面</b>、接口层级与注册组收敛、DTO 上下文登记与官方字段名陷阱。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域令牌面（与通讯录 Contact 域的分界）</b>：官方「服务商代开发 → 应用接口调用」根树的
/// <c>service/contact/*</c> 与 <c>service/batch/getresult</c> 全部以 <c>provider_access_token</c> 鉴权，
/// 且被检索企业由请求体首字段 <c>auth_corpid</c> 显式指定 ⇒
/// 本域<b>只有服务商形态子接口</b>，自建 / 第三方子接口为<b>零端点</b>，由 SV2 锁定不得回潮。
/// 若把本域端点并入 <c>Contact</c> 注册组，会让同一注册组内出现
/// <c>access_token</c> 与 <c>provider_access_token</c> 两套令牌面（组内令牌面必须同构），
/// 故注册组名单列为 <c>Service</c>。
/// </para>
/// <para>
/// <b>官方反直觉点（勿「顺手修正」）</b>：① 检索四支为 POST、异步任务结果查询为 <b>GET</b>；
/// ② <c>sort</c> 的成员标识列表官方字段名为 <c>useridlist</c>（连写、无下划线）；
/// ③ <c>id_translate</c> 的官方输出格式<b>仅 pdf</b>、入参为 <c>media_id</c> 列表且为异步任务（返回 <c>jobid</c>）；
/// ④ <c>batchsearch</c> 的失败语义在<b>子项</b>：<c>query_result_list[]</c> 逐条携带
/// <c>errcode</c>/<c>errmsg</c>，顶层 errcode 为 0 不代表子请求全部成功；
/// ⑤ 本域 <c>is_last</c> 为布尔，与「汇报」「高级功能申请单」翻页标识形态各异、SDK 不归一。
/// </para>
/// <para>
/// <b>个人信息约束</b>：响应侧 <c>userid</c> / <c>open_userid</c> / <c>department_id</c> 为成员与部门身份，
/// <c>service/batch/getresult</c> 返回的 <c>url</c>（含 <c>contact_id_translate.url</c>）指向
/// 携带成员身份的导出文件且官方口径 24 小时有效 ⇒ 均不得写入日志、遥测或异常消息。
/// </para>
/// <para>
/// SKIT 清单内其余 10 条 <c>service/*</c> 行（旧硬件族等）已核定为<b>非契约</b>（脱树 / 正文清空），
/// 由 <see cref="WechatNonContractRouteZeroDeclarationContractGuards"/>（RD1）锁定零声明。
/// </para>
/// </remarks>
public class WechatServiceContractGuards
{
    private const string ServiceRegistryGroupName = "Service";

    private const string ServiceNamespace = "Mud.Wechat.Work.DataModels.Service";

    /// <summary>服务商代开发域官方路由表（5 端点：检索四支 POST + 任务结果查询 GET）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ServiceRoutes =
    {
        (typeof(IWechatWorkServiceContactService),
            nameof(IWechatWorkServiceContactService.SearchContactsAsync),
            typeof(PostAttribute), "/cgi-bin/service/contact/search"),
        (typeof(IWechatWorkServiceContactService),
            nameof(IWechatWorkServiceContactService.BatchSearchContactsAsync),
            typeof(PostAttribute), "/cgi-bin/service/contact/batchsearch"),
        (typeof(IWechatWorkServiceContactService),
            nameof(IWechatWorkServiceContactService.TranslateContactIdsAsync),
            typeof(PostAttribute), "/cgi-bin/service/contact/id_translate"),
        (typeof(IWechatWorkServiceContactService),
            nameof(IWechatWorkServiceContactService.SortContactsAsync),
            typeof(PostAttribute), "/cgi-bin/service/contact/sort"),
        (typeof(IWechatWorkServiceBatchService),
            nameof(IWechatWorkServiceBatchService.GetServiceBatchJobResultAsync),
            typeof(GetAttribute), "/cgi-bin/service/batch/getresult"),
    };

    // ------------------------------------------------------------------
    // SV1：路由与请求形态必须与官方契约一致。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 SV1：本域端点恒为 5 支（检索四支 + 任务结果查询一支），路由与方法名不得漂移；
    /// 检索四支的请求体首字段恒为被授权企业 <c>auth_corpid</c>，任务结果查询以 <c>[Query("jobid")]</c> 承载。
    /// </summary>
    [Fact]
    public void ServiceEndpoints_ShouldMatchOfficialRoutesAndCredentialShape()
    {
        ServiceRoutes.Should().HaveCount(5,
            "代开发授权企业数据面官方仅 service/contact 四支 + service/batch/getresult 一支");
        ServiceRoutes.Count(r => r.HttpAttribute == typeof(PostAttribute)).Should().Be(4,
            "检索四支官方为 POST、getresult 官方为 GET（勿「顺手统一」）");

        foreach (var (iface, method, httpAttribute, route) in ServiceRoutes)
        {
            var target = iface.GetMethod(method,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须声明于本域父接口");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 四支检索端点：请求体承载（[Body]），且请求 DTO 首字段为 auth_corpid。
        foreach (var (iface, method, _, _) in ServiceRoutes.Where(r => r.HttpAttribute == typeof(PostAttribute)))
        {
            var target = iface.GetMethod(method,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            var bodyParam = target.GetParameters().SingleOrDefault(p =>
                p.GetCustomAttribute<BodyAttribute>() != null);
            bodyParam.Should().NotBeNull($"{iface.Name}.{method} 必须以 [Body] 承载请求体");

            var requestType = bodyParam!.ParameterType;
            requestType.GetProperty(nameof(SearchServiceContactsRequest.AuthCorpId))
                .Should().NotBeNull($"{requestType.Name} 必须承载官方必填 auth_corpid（被授权企业）");
            JsonNameShouldBe(requestType, nameof(SearchServiceContactsRequest.AuthCorpId), "auth_corpid");
        }

        // getresult：jobid 走 Query（官方 GET）。
        var getResult = typeof(IWechatWorkServiceBatchService).GetMethod(
            nameof(IWechatWorkServiceBatchService.GetServiceBatchJobResultAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        getResult.GetParameters().Should().Contain(p =>
            p.GetCustomAttributes(typeof(QueryAttribute), false).Cast<QueryAttribute>().Any(q => q.Name == "jobid"),
            "getresult 的 jobid 必须以 [Query(\"jobid\")] 承载（官方 GET）");
    }

    // ------------------------------------------------------------------
    // SV2：接口层级与注册组收敛（仅服务商子接口、父接口零端点）。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 SV2：两个父接口各挂唯一服务商子接口，端点全部由父接口承载、子接口零端点；
    /// 两级统一挂 <c>Service</c> 注册组，<c>TokenManage</c> 恒为 <see cref="IWechatAppManager"/>。
    /// </summary>
    [Fact]
    public void ServiceInterfaceHierarchy_ShouldHaveProviderOnlyChildrenInServiceGroup()
    {
        var pairs = new (Type Parent, Type Child, string ParentImplementation)[]
        {
            (typeof(IWechatWorkServiceContactService),
                typeof(IWechatWorkProviderServiceContactService), "WechatWorkServiceContactService"),
            (typeof(IWechatWorkServiceBatchService),
                typeof(IWechatWorkProviderServiceBatchService), "WechatWorkServiceBatchService"),
        };

        foreach (var (parent, child, parentImplementation) in pairs)
        {
            // 继承链上只允许唯一服务商子接口：自建 / 第三方形态一旦回潮即为官方开放面误判。
            var assignable = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
                .ToList();
            assignable.Should().BeEquivalentTo(new[] { child },
                $"{parent.Name} 官方仅服务商代开发开放，继承链上不得出现自建 / 第三方子接口");

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.TokenManage.Should().Be(nameof(IWechatAppManager),
                "父接口不注入令牌也必须统一为 IWechatAppManager（全仓各域形态一致）");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeEmpty($"{parent.Name} 为本域端点承载面（与「零端点父接口」域形态不同）");

            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(ServiceRegistryGroupName,
                $"{child.Name} 必须挂 Service 注册组（不得并入 Contact：组内令牌面须同构）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类");
            childApi.TokenManage.Should().Be(nameof(IWechatAppManager),
                "父子两级 TokenManage 必须统一（否则生成器以 new 隐藏基类切换成员）");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为标记子接口，端点全部由父接口承载");
        }
    }

    // ------------------------------------------------------------------
    // SV3：纯服务商凭证面（provider_access_token，无归属域键）。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 SV3：本域父子两级均声明 <c>provider_access_token</c> 且 Query 注入，
    /// 不得出现 <c>access_token</c> / <c>suite_access_token</c>，也不得叠加归属域键。
    /// </summary>
    [Fact]
    public void ServiceInterfaces_ShouldDeclareProviderAccessTokenOnly()
    {
        var faces = new[]
        {
            typeof(IWechatWorkServiceContactService), typeof(IWechatWorkProviderServiceContactService),
            typeof(IWechatWorkServiceBatchService), typeof(IWechatWorkProviderServiceBatchService),
        };

        foreach (var iface in faces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]（官方以 provider_access_token 鉴权）");
            token!.TokenType.Should().Be(WechatTokenTypes.ProviderAccessToken,
                $"{iface.Name} 的令牌类型必须为服务商令牌（本域官方不给应用令牌链路）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 令牌注入须为 Query（企微官方契约，MUD005 已知接受风险）");
            token.Name.Should().Be("provider_access_token", $"{iface.Name} 的官方参数名不得漂移");
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
                $"{iface.Name} 令牌类型已无歧义，不得叠加归属域键（TO 系列同规则，永不增设 @Provider/@Suite）");
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
                $"{iface.Name} 令牌类型已无歧义，不得叠加归属域键（TO 系列同规则）");
        }
    }

    // ------------------------------------------------------------------
    // SV4：DTO 上下文登记与官方字段名陷阱。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 SV4：本域 17 个顶层 DTO 必须登记进 <c>ServiceJsonContext</c>，
    /// <c>SerializerClassName</c> 统一为域段 <c>Service</c>；同步锁定官方字段名陷阱。
    /// </summary>
    [Fact]
    public void ServiceDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = ServiceJsonContext.Default;

        var endpointContractTypes = new Type[]
        {
            typeof(SearchServiceContactsRequest), typeof(SearchServiceContactsResponse),
            typeof(ServiceContactQueryResult), typeof(ServiceContactUser), typeof(ServiceContactParty),
            typeof(BatchSearchServiceContactsRequest), typeof(ServiceContactQueryRequest),
            typeof(BatchSearchServiceContactsResponse), typeof(ServiceContactBatchQueryItem),
            typeof(TranslateServiceContactIdsRequest), typeof(TranslateServiceContactIdsResponse),
            typeof(SortServiceContactsRequest), typeof(ServiceContactSortOption), typeof(SortServiceContactsResponse),
            typeof(GetServiceBatchJobResultResponse), typeof(ServiceBatchJobResultDetail),
            typeof(ServiceContactIdTranslateResult),
        };

        endpointContractTypes.Should().HaveCount(17,
            "本域契约面类型清单漂移须先核对官方页面再同批调整本守卫");

        var domainTypes = typeof(SearchServiceContactsResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == ServiceNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(17,
            "代开发数据面域契约面类型数漂移须先核对官方页面再同批调整本守卫");

        foreach (var type in endpointContractTypes.Concat(domainTypes).Distinct())
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于代开发数据面域命名空间，必须登记进 ServiceJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Service",
                $"{type.Name} 的 SerializerClassName 必须为代开发数据面域段 Service");
        }

        // 官方字段名陷阱锁定（照抄原文，勿「顺手驼峰化」）。
        JsonNameShouldBe(typeof(SortServiceContactsRequest), nameof(SortServiceContactsRequest.UserIdList), "useridlist");
        JsonNameShouldBe(typeof(SortServiceContactsResponse), nameof(SortServiceContactsResponse.UserIdList), "useridlist");
        JsonNameShouldBe(typeof(TranslateServiceContactIdsResponse), nameof(TranslateServiceContactIdsResponse.JobId), "jobid");
        JsonNameShouldBe(typeof(TranslateServiceContactIdsRequest), nameof(TranslateServiceContactIdsRequest.MediaIdList), "media_id_list");
        JsonNameShouldBe(typeof(TranslateServiceContactIdsRequest), nameof(TranslateServiceContactIdsRequest.OutputFileFormat), "output_file_format");
        JsonNameShouldBe(typeof(ServiceContactParty), nameof(ServiceContactParty.DepartmentId), "department_id");
        JsonNameShouldBe(typeof(ServiceContactUser), nameof(ServiceContactUser.OpenUserId), "open_userid");
        JsonNameShouldBe(typeof(SearchServiceContactsResponse), nameof(SearchServiceContactsResponse.IsLast), "is_last");
        JsonNameShouldBe(typeof(BatchSearchServiceContactsRequest), nameof(BatchSearchServiceContactsRequest.QueryRequestList), "query_request_list");

        // batchsearch 失败语义在子项：逐条 errcode / errmsg（顶层 errcode 为 0 不代表子请求全成功）。
        JsonNameShouldBe(typeof(ServiceContactBatchQueryItem), nameof(ServiceContactBatchQueryItem.ErrCode), "errcode");
        JsonNameShouldBe(typeof(ServiceContactBatchQueryItem), nameof(ServiceContactBatchQueryItem.ErrMsg), "errmsg");

        // 异步任务结果与检索响应形态不同：getresult 用 status/type/result/url，不含 is_last / next_cursor。
        typeof(GetServiceBatchJobResultResponse).GetProperty(nameof(SearchServiceContactsResponse.IsLast))
            .Should().BeNull("getresult 官方无翻页标识，勿复用检索响应 DTO");
        typeof(GetServiceBatchJobResultResponse).GetProperty(nameof(SearchServiceContactsResponse.NextCursor))
            .Should().BeNull("getresult 官方无 next_cursor");
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
