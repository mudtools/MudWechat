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
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.License;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 接口调用许可域（License 模块）契约守卫：路由表、四族一令牌形态、接口层级、官方契约陷阱与
/// JSON 上下文全量登记锁定（订单管理 15 端点 + 账号管理 9 端点 + 应用管理 1 端点 + 自动激活设置 2 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>开放面</b>：官方在第三方应用开发与服务商代开发两棵文档树的「接口调用许可」分组下提供本域端点
/// （97182~97194、97199/97200、97208、98887/98888/99420、100138，两棵文档树共享同一端点页），
/// 企业自建应用开发文档树<b>无对应 API</b>，故全域不设自建 / 代开发子接口；
/// 事件回调仅服务商代开发文档树提供（97195 接口许可失效通知 / 97196 支付成功 / 97197 退款结果 /
/// 97198 自动激活），由回调包的许可族载荷登记（<c>WechatCallbackContractGuards</c> CB 系列锁定）。
/// </para>
/// <para>
/// <b>四族一令牌（本域最关键的结构约束）</b>：27 个端点全部消费服务商 <c>provider_access_token</c>，
/// 按官方分组拆为四族：① 订单管理族（<see cref="IWechatWorkThirdPartyLicenseOrderService"/>）15 端点、
/// ② 账号管理族（<see cref="IWechatWorkThirdPartyLicenseAccountService"/>）9 端点、
/// ③ 应用管理族（<see cref="IWechatWorkThirdPartyLicenseAppService"/>）1 端点、
/// ④ 自动激活设置族（<see cref="IWechatWorkThirdPartyLicenseAutoActiveService"/>）2 端点，
/// 均为「零端点父接口 + 唯一第三方子接口承载」，且<b>不声明凭据归属域键</b>（provider 令牌本身已无歧义）。
/// </para>
/// <para>
/// <b>官方契约陷阱（勿「顺手修正」）</b>：
/// ① 获取企业的账号列表路由官方拼写为 <c>list_actived_account</c>（非 activated）；
/// ② 应用接口许可试用期字段官方拼写为 <c>trail_info</c>（非 trial_info）；
/// ③ 余额支付结果（99420）顶层 <c>errcode</c> 表示接口调用是否成功（支付失败时也返回 0），
/// 支付是否成功须以 <c>status</c> / <c>pay_job_result</c> 判定；
/// ④ 下单续期任务的 <c>account_duration</c> 中 months 与 new_expire_time 二者填其一，
/// 多企业新购 BuyInfo 与订单详情、续期任务的时长字段共用 <see cref="LicenseAccountDuration"/> 超集承载；
/// ⑤ 多处响应返回<b>加密的</b> corpid / userid（get_order、list_order_account、get_union_order、
/// get_active_info_by_code、list_actived_account、get_active_info_by_user、batch_active_account、batch_transfer_license）。
/// </para>
/// </remarks>
public class WechatLicenseContractGuards
{
    private const string LicenseRegistryGroupName = "License";

    private const string OrderParentImplementationClassName = "WechatWorkLicenseOrderService";

    private const string AccountParentImplementationClassName = "WechatWorkLicenseAccountService";

    private const string AppParentImplementationClassName = "WechatWorkLicenseAppService";

    private const string AutoActiveParentImplementationClassName = "WechatWorkLicenseAutoActiveService";

    /// <summary>接口调用许可域官方路由表（27 端点分属四族：订单管理 15 条 + 账号管理 9 条 + 应用管理 1 条 + 自动激活设置 2 条；余额查询官方即 GET，其余全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // 订单管理族（官方 97182/97183/97184/97185/97186/97187/98887/98888/99420，路由挂 /cgi-bin/license/ 段，官方全部即 POST）。
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.CreateNewOrderAsync),
            typeof(PostAttribute), "/cgi-bin/license/create_new_order"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.CreateRenewOrderJobAsync),
            typeof(PostAttribute), "/cgi-bin/license/create_renew_order_job"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.SubmitOrderJobAsync),
            typeof(PostAttribute), "/cgi-bin/license/submit_order_job"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.ListOrderAsync),
            typeof(PostAttribute), "/cgi-bin/license/list_order"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.GetOrderAsync),
            typeof(PostAttribute), "/cgi-bin/license/get_order"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.ListOrderAccountAsync),
            typeof(PostAttribute), "/cgi-bin/license/list_order_account"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.CancelOrderAsync),
            typeof(PostAttribute), "/cgi-bin/license/cancel_order"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.CreateNewOrderJobAsync),
            typeof(PostAttribute), "/cgi-bin/license/create_new_order_job"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.SubmitNewOrderJobAsync),
            typeof(PostAttribute), "/cgi-bin/license/submit_new_order_job"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.GetNewOrderJobResultAsync),
            typeof(PostAttribute), "/cgi-bin/license/new_order_job_result"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.GetUnionOrderAsync),
            typeof(PostAttribute), "/cgi-bin/license/get_union_order"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.SubmitPayJobAsync),
            typeof(PostAttribute), "/cgi-bin/license/submit_pay_job"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.GetPayJobResultAsync),
            typeof(PostAttribute), "/cgi-bin/license/pay_job_result"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.QuerySupportPolicyAsync),
            typeof(PostAttribute), "/cgi-bin/license/support_policy_query"),
        (typeof(IWechatWorkThirdPartyLicenseOrderService),
            nameof(IWechatWorkThirdPartyLicenseOrderService.GetAccountBalanceAsync),
            typeof(GetAttribute), "/cgi-bin/service/get_account_balance"),

        // 账号管理族（官方 97188/97189/97190/97191/97192/97193，路由挂 /cgi-bin/license/ 段，官方全部即 POST）。
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.ActiveAccountAsync),
            typeof(PostAttribute), "/cgi-bin/license/active_account"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.BatchActiveAccountAsync),
            typeof(PostAttribute), "/cgi-bin/license/batch_active_account"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.ActiveAccountByTypeAsync),
            typeof(PostAttribute), "/cgi-bin/license/active_account_by_type"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.GetActiveInfoByCodeAsync),
            typeof(PostAttribute), "/cgi-bin/license/get_active_info_by_code"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.BatchGetActiveInfoByCodeAsync),
            typeof(PostAttribute), "/cgi-bin/license/batch_get_active_info_by_code"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.ListActivedAccountAsync),
            typeof(PostAttribute), "/cgi-bin/license/list_actived_account"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.GetActiveInfoByUserAsync),
            typeof(PostAttribute), "/cgi-bin/license/get_active_info_by_user"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.BatchTransferLicenseAsync),
            typeof(PostAttribute), "/cgi-bin/license/batch_transfer_license"),
        (typeof(IWechatWorkThirdPartyLicenseAccountService),
            nameof(IWechatWorkThirdPartyLicenseAccountService.BatchShareActiveCodeAsync),
            typeof(PostAttribute), "/cgi-bin/license/batch_share_active_code"),

        // 应用管理族（官方 97194，路由挂 /cgi-bin/license/ 段，官方即 POST）。
        (typeof(IWechatWorkThirdPartyLicenseAppService),
            nameof(IWechatWorkThirdPartyLicenseAppService.GetAppLicenseInfoAsync),
            typeof(PostAttribute), "/cgi-bin/license/get_app_license_info"),

        // 自动激活设置族（官方 97199/97200，路由挂 /cgi-bin/license/ 段，官方全部即 POST）。
        (typeof(IWechatWorkThirdPartyLicenseAutoActiveService),
            nameof(IWechatWorkThirdPartyLicenseAutoActiveService.SetAutoActiveStatusAsync),
            typeof(PostAttribute), "/cgi-bin/license/set_auto_active_status"),
        (typeof(IWechatWorkThirdPartyLicenseAutoActiveService),
            nameof(IWechatWorkThirdPartyLicenseAutoActiveService.GetAutoActiveStatusAsync),
            typeof(PostAttribute), "/cgi-bin/license/get_auto_active_status"),
    };

    /// <summary>
    /// 契约守卫 LC1：接口调用许可域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void LicenseEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(27,
            "接口调用许可域官方共 27 个端点 = 订单管理族 15 条 + 账号管理族 9 条 + 应用管理族 1 条 + 自动激活设置族 2 条");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(27, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 LC2：官方「获取订单列表 / 获取企业的账号列表」等分页端点 limit 上限官方为 1000（默认 500）、
    /// list_order 的 start_time / end_time 必须同时指定——该约束以请求 DTO 注释承载，
    /// 本守卫锁定对应字段存在且可空（官方可选参数）。
    /// </summary>
    [Fact]
    public void LicensePagedRequests_ShouldCarryOptionalPagingFields()
    {
        foreach (var requestType in new[]
                 {
                     typeof(ListLicenseOrderRequest),
                     typeof(ListLicenseOrderAccountRequest),
                     typeof(GetUnionLicenseOrderRequest),
                     typeof(ListLicenseActivedAccountRequest),
                 })
        {
            var limit = requestType.GetProperty(nameof(ListLicenseOrderRequest.Limit));
            limit.Should().NotBeNull($"{requestType.Name} 必须声明 Limit（官方可选，最大值 1000，默认值 500）");
            limit!.PropertyType.Should().Be(typeof(int?), $"{requestType.Name}.Limit 官方为可选整型");

            var cursor = requestType.GetProperty(nameof(ListLicenseOrderRequest.Cursor));
            cursor.Should().NotBeNull($"{requestType.Name} 必须声明 Cursor（官方可选分页游标）");
            cursor!.PropertyType.Should().Be(typeof(string), $"{requestType.Name}.Cursor 官方为可选字符串（以 string? 承载）");
        }

        var startTime = typeof(ListLicenseOrderRequest).GetProperty(nameof(ListLicenseOrderRequest.StartTime));
        startTime.Should().NotBeNull("获取订单列表必须声明 StartTime（官方可选，与 EndTime 必须同时指定）");
        startTime!.PropertyType.Should().Be(typeof(long?), "StartTime 官方为 unix 时间戳（可选）");
        var endTime = typeof(ListLicenseOrderRequest).GetProperty(nameof(ListLicenseOrderRequest.EndTime));
        endTime.Should().NotBeNull("获取订单列表必须声明 EndTime（官方可选，与 StartTime 必须同时指定）");
        endTime!.PropertyType.Should().Be(typeof(long?), "EndTime 官方为 unix 时间戳（可选）");
    }

    /// <summary>
    /// 契约守卫 LC3：四族接口层级——均为零端点父接口（IsAbstract）+ 唯一第三方子接口承载；
    /// 官方自建 / 代开发（企业侧）文档树无对应 API，继承链上不得出现自建 / 代开发子接口（能力漂移守卫）。
    /// </summary>
    [Theory]
    [InlineData(typeof(IWechatWorkLicenseOrderService), typeof(IWechatWorkThirdPartyLicenseOrderService), 15, "WechatWorkLicenseOrderService")]
    [InlineData(typeof(IWechatWorkLicenseAccountService), typeof(IWechatWorkThirdPartyLicenseAccountService), 9, "WechatWorkLicenseAccountService")]
    [InlineData(typeof(IWechatWorkLicenseAppService), typeof(IWechatWorkThirdPartyLicenseAppService), 1, "WechatWorkLicenseAppService")]
    [InlineData(typeof(IWechatWorkLicenseAutoActiveService), typeof(IWechatWorkThirdPartyLicenseAutoActiveService), 2, "WechatWorkLicenseAutoActiveService")]
    public void LicenseHierarchy_ShouldConvergeOnThirdPartyChildOnly(
        Type parent, Type child, int endpointCount, string parentImplementationClassName)
    {
        child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承零端点父接口 {parent.Name}");
        parentImplementationClassName.Should().Be(parent.Name.TrimStart('I'));

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("官方仅第三方 / 代开发（服务商侧）开放本域端点：父接口零端点，端点全部由唯一第三方子接口承载");

        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
        derived.Should().BeEquivalentTo(new[] { child.Name },
            "接口调用许可域官方仅第三方应用 / 服务商代开发开放（自建无对应 API），继承链上不得出现其它子接口");

        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(LicenseRegistryGroupName,
            $"{child.Name} 必须挂 {LicenseRegistryGroupName} 注册组（经 Add{LicenseRegistryGroupName}WebApiHttpClient() 注册）");
        childApi.InheritedFrom.Should().Be(parentImplementationClassName,
            $"{child.Name} 必须继承父接口生成实现类 {parentImplementationClassName}");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(endpointCount, $"{child.Name} 承载本族全部 {endpointCount} 条官方端点");
    }

    /// <summary>
    /// 契约守卫 LC4：令牌绑定——四族统一消费 ProviderAccessToken、以 Query 注入，
    /// 且<b>不声明凭据归属域键</b>（provider 令牌本身已无歧义，见归属域守卫 TO1）。
    /// </summary>
    [Fact]
    public void LicenseTokenBinding_ShouldUseProviderAccessTokenOnAllFamilies()
    {
        var providerTokenInterfaces = new[]
        {
            typeof(IWechatWorkLicenseOrderService),
            typeof(IWechatWorkThirdPartyLicenseOrderService),
            typeof(IWechatWorkLicenseAccountService),
            typeof(IWechatWorkThirdPartyLicenseAccountService),
            typeof(IWechatWorkLicenseAppService),
            typeof(IWechatWorkThirdPartyLicenseAppService),
            typeof(IWechatWorkLicenseAutoActiveService),
            typeof(IWechatWorkThirdPartyLicenseAutoActiveService),
        };

        foreach (var iface in providerTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.ProviderAccessToken,
                $"{iface.Name} 令牌路由键必须为 ProviderAccessToken（服务商级凭证）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("provider_access_token",
                $"{iface.Name} Query 注入参数名必须为官方契约的 provider_access_token");

            // 反射陷阱：未显式书写时读到的是构造函数默认值哨兵，故断言「不等于两个归属域键」。
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
                $"{iface.Name} 已消费 provider 令牌，不得叠加自建归属域键");
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
                $"{iface.Name} 已消费 provider 令牌，不得叠加授权企业归属域键");
        }
    }

    /// <summary>
    /// 契约守卫 LC5：接口调用许可模块的请求/响应 DTO 必须全量登记进 AOT JSON 上下文
    /// （SerializerClassName 统一为 License）。
    /// </summary>
    [Fact]
    public void LicenseDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = LicenseJsonContext.Default;

        var domainTypes = typeof(LicenseOrderDetail).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.License"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记（请求 26 + 响应 22 + 嵌套对象 26 = 74）。
        domainTypes.Should().HaveCount(74,
            "接口调用许可模块契约面类型数漂移须先核对官方文档再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于接口调用许可域命名空间，必须登记进 LicenseJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be(LicenseRegistryGroupName,
                $"{type.Name} 的 SerializerClassName 必须为接口调用许可域段 License");
        }
    }

    /// <summary>
    /// 契约守卫 LC6：官方契约陷阱锁定——
    /// ① 路由 <c>list_actived_account</c> 官方拼写少一个 i（方法名照抄 ListActivedAccount）；
    /// ② 应用接口许可试用期字段官方拼写为 <c>trail_info</c>；
    /// ③ 余额支付结果顶层 errcode 表示接口调用成功而非支付成功（须另承载 status 与 pay_job_result）；
    /// ④ 续期任务与多企业新购共用时长字段（months / days / new_expire_time）超集承载；
    /// ⑤ jobid 官方为全小写连写（非 job_id）。
    /// </summary>
    [Fact]
    public void LicenseDataModels_ShouldLockOfficialContractTraps()
    {
        // ① 路由官方拼写陷阱：list_actived_account（少一个 i），不得「顺手修正」为 activated。
        typeof(IWechatWorkThirdPartyLicenseAccountService)
            .GetMethod(nameof(IWechatWorkThirdPartyLicenseAccountService.ListActivedAccountAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
            .GetCustomAttribute<PostAttribute>()!.RequestUri
            .Should().Be("/cgi-bin/license/list_actived_account",
                "官方路由原文即 list_actived_account（非 activated），照抄原文勿修正");

        // ② 试用期字段官方拼写 trail_info。
        JsonNameShouldBe(typeof(GetAppLicenseInfoResponse), nameof(GetAppLicenseInfoResponse.TrailInfo), "trail_info");
        JsonNameShouldBe(typeof(LicenseTrailInfo), nameof(LicenseTrailInfo.StartTime), "start_time");
        JsonNameShouldBe(typeof(LicenseTrailInfo), nameof(LicenseTrailInfo.EndTime), "end_time");

        // ③ 余额支付结果：errcode 仅表示接口调用成功与否，支付结果另有 status 与 pay_job_result。
        JsonNameShouldBe(typeof(GetLicensePayJobResultResponse), nameof(GetLicensePayJobResultResponse.Status), "status");
        JsonNameShouldBe(typeof(GetLicensePayJobResultResponse), nameof(GetLicensePayJobResultResponse.PayJobResult), "pay_job_result");
        typeof(GetLicensePayJobResultResponse).GetProperty(nameof(GetLicensePayJobResultResponse.Status))!.PropertyType
            .Should().Be(typeof(int?), "status 官方为支付任务结果（1 成功 / 2 执行中 / 3 失败），与顶层 errcode 语义不同");

        // ④ 时长字段超集承载：months / days（购买）与 new_expire_time（续期指定到期时间）同置一处。
        JsonNameShouldBe(typeof(LicenseAccountDuration), nameof(LicenseAccountDuration.Months), "months");
        JsonNameShouldBe(typeof(LicenseAccountDuration), nameof(LicenseAccountDuration.Days), "days");
        JsonNameShouldBe(typeof(LicenseAccountDuration), nameof(LicenseAccountDuration.NewExpireTime), "new_expire_time");
        typeof(LicenseAccountDuration).GetProperty(nameof(LicenseAccountDuration.NewExpireTime))!.PropertyType
            .Should().Be(typeof(long?), "new_expire_time 官方为 unix 时间戳");

        // ⑤ jobid 官方为全小写连写（非 job_id）。
        JsonNameShouldBe(typeof(CreateRenewLicenseOrderJobRequest), nameof(CreateRenewLicenseOrderJobRequest.Jobid), "jobid");
        JsonNameShouldBe(typeof(SubmitLicenseOrderJobRequest), nameof(SubmitLicenseOrderJobRequest.Jobid), "jobid");
        JsonNameShouldBe(typeof(CreateNewLicenseOrderJobRequest), nameof(CreateNewLicenseOrderJobRequest.Jobid), "jobid");
        JsonNameShouldBe(typeof(SubmitNewLicenseOrderJobRequest), nameof(SubmitNewLicenseOrderJobRequest.Jobid), "jobid");
        JsonNameShouldBe(typeof(GetNewLicenseOrderJobResultRequest), nameof(GetNewLicenseOrderJobResultRequest.Jobid), "jobid");
        JsonNameShouldBe(typeof(SubmitLicensePayJobResponse), nameof(SubmitLicensePayJobResponse.Jobid), "jobid");
        JsonNameShouldBe(typeof(GetLicensePayJobResultRequest), nameof(GetLicensePayJobResultRequest.Jobid), "jobid");
        JsonNameShouldBe(typeof(CreateRenewLicenseOrderJobResponse), nameof(CreateRenewLicenseOrderJobResponse.Jobid), "jobid");
        JsonNameShouldBe(typeof(CreateNewLicenseOrderJobResponse), nameof(CreateNewLicenseOrderJobResponse.Jobid), "jobid");

        // 账号个数与账号类型字段照抄官方原文。
        JsonNameShouldBe(typeof(LicenseAccountCount), nameof(LicenseAccountCount.BaseCount), "base_count");
        JsonNameShouldBe(typeof(LicenseAccountCount), nameof(LicenseAccountCount.ExternalContactCount), "external_contact_count");
        JsonNameShouldBe(typeof(LicenseBuyInfo), nameof(LicenseBuyInfo.AutoActiveStatus), "auto_active_status");
        JsonNameShouldBe(typeof(BatchShareLicenseActiveCodeRequest), nameof(BatchShareLicenseActiveCodeRequest.FromCorpid), "from_corpid");
        JsonNameShouldBe(typeof(BatchShareLicenseActiveCodeRequest), nameof(BatchShareLicenseActiveCodeRequest.ToCorpid), "to_corpid");
        JsonNameShouldBe(typeof(BatchShareLicenseActiveCodeRequest), nameof(BatchShareLicenseActiveCodeRequest.CorpLinkType), "corp_link_type");
        JsonNameShouldBe(typeof(GetAppLicenseInfoRequest), nameof(GetAppLicenseInfoRequest.SuiteId), "suite_id");
        JsonNameShouldBe(typeof(GetAppLicenseInfoRequest), nameof(GetAppLicenseInfoRequest.Appid), "appid");

        // 民生优惠条件查询（97208）与充值账户余额查询（100138）字段照抄官方原文。
        JsonNameShouldBe(typeof(QueryLicenseSupportPolicyRequest), nameof(QueryLicenseSupportPolicyRequest.Corpid), "corpid");
        JsonNameShouldBe(typeof(QueryLicenseSupportPolicyResponse), nameof(QueryLicenseSupportPolicyResponse.QueryResult), "query_result");
        JsonNameShouldBe(typeof(QueryLicenseSupportPolicyResponse), nameof(QueryLicenseSupportPolicyResponse.UnsatisfiedReason), "unsatisfied_reason");
        JsonNameShouldBe(typeof(GetAccountBalanceResponse), nameof(GetAccountBalanceResponse.Balance), "balance");
        typeof(GetAccountBalanceResponse).GetProperty(nameof(GetAccountBalanceResponse.Balance))!.PropertyType
            .Should().Be(typeof(long?), "balance 官方为充值账户余额（单位分），以 long? 承载");
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName, $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
