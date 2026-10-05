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
using Mud.Wechat.Work.DataModels.PromotionQrCode;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 推广二维码域（PromotionQrCode 模块）契约守卫：路由表、两族两令牌形态、接口层级与
/// JSON 上下文全量登记锁定（获取注册码 + 查询注册状态 + 设置授权应用可见范围 + 设置通讯录同步完成）。
/// </summary>
/// <remarks>
/// <para>
/// <b>开放面</b>：官方仅在第三方应用开发文档树提供本域端点（90581~90584），
/// 企业自建应用开发与服务商代开发文档树均无对应 API，故全域不设自建 / 代开发子接口。
/// </para>
/// <para>
/// <b>两族两令牌（本域最关键的结构约束）</b>：4 个端点消费两种互不兼容的凭据，故按令牌路由键拆为两族：
/// ① 企业注册族（<see cref="IWechatWorkThirdPartyServicePromotionQrCodeService"/>）走服务商
/// <c>provider_access_token</c>，零端点父接口 + 唯一第三方子接口承载；
/// ② 通讯录迁移族（<see cref="IWechatWorkPromotionQrCodeContactSyncService"/>）走「查询注册状态」返回的
/// 通讯录迁移 <c>access_token</c>，官方明文「请注意与 provider_access_token 的区别」且不可用后者代替；
/// 该凭证 30 分钟有效且被「设置通讯录同步完成」立即作废，SDK 令牌基座刻意不缓存，
/// 故以显式 <c>[Query("access_token")]</c> 传入、不带 <c>[Token]</c>（形态对齐
/// <see cref="IWechatWorkProviderAuthenticationUrl"/>；因无 SDK 管理令牌，亦无父接口与子接口）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：「设置通讯录同步完成」官方即 GET 却执行状态变更
/// （设置同步完成 + 解除通讯录锁定 + 使迁移 access_token 失效），勿改成 POST；
/// 「设置授权应用可见范围」的 allow_user / allow_party / allow_tag 未填即清空对应列表（非「保持不变」），
/// 故三者须为可空、缺省即官方「清空」语义。
/// </para>
/// </remarks>
public class WechatPromotionQrCodeContractGuards
{
    private const string PromotionQrCodeRegistryGroupName = "PromotionQrCode";

    private const string RegisterParentImplementationClassName = "WechatWorkServicePromotionQrCodeService";

    private const string ContactSyncImplementationClassName = "WechatWorkPromotionQrCodeContactSyncService";

    /// <summary>推广二维码域官方路由表（4 端点分属两族：2 个 provider 令牌 + 2 个通讯录迁移令牌）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // 获取注册码（90581），官方即 POST。
        (typeof(IWechatWorkThirdPartyServicePromotionQrCodeService),
            nameof(IWechatWorkThirdPartyServicePromotionQrCodeService.GetRegisterCodeAsync),
            typeof(PostAttribute), "/cgi-bin/service/get_register_code"),
        // 查询注册状态（90582），官方即 POST。
        (typeof(IWechatWorkThirdPartyServicePromotionQrCodeService),
            nameof(IWechatWorkThirdPartyServicePromotionQrCodeService.GetRegisterInfoAsync),
            typeof(PostAttribute), "/cgi-bin/service/get_register_info"),
        // 设置授权应用可见范围（90583），官方即 POST。
        (typeof(IWechatWorkPromotionQrCodeContactSyncService),
            nameof(IWechatWorkPromotionQrCodeContactSyncService.SetAuthorizedAppScopeAsync),
            typeof(PostAttribute), "/cgi-bin/agent/set_scope"),
        // 设置通讯录同步完成（90584），官方即 GET 却执行状态变更，勿「顺手统一」为 POST。
        (typeof(IWechatWorkPromotionQrCodeContactSyncService),
            nameof(IWechatWorkPromotionQrCodeContactSyncService.SetContactSyncSuccessAsync),
            typeof(GetAttribute), "/cgi-bin/sync/contact_sync_success"),
    };

    /// <summary>
    /// 契约守卫 PQ1：推广二维码域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void PromotionQrCodeEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(4,
            "推广二维码域官方共 4 个端点 = 企业注册族 2 条（provider_access_token）+ 通讯录迁移族 2 条（access_token）");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "本域各端点路由互不重复");

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
    /// 契约守卫 PQ2：企业注册族接口层级——零端点父接口（IsAbstract）+ 唯一第三方子接口承载 2 条端点；
    /// 官方自建 / 代开发文档树无对应 API，继承链上不得出现自建 / 代开发子接口（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void PromotionQrCodeRegisterHierarchy_ShouldConvergeOnThirdPartyChildOnly()
    {
        var parent = typeof(IWechatWorkServicePromotionQrCodeService);
        var child = typeof(IWechatWorkThirdPartyServicePromotionQrCodeService);

        child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承零端点父接口 {parent.Name}");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("官方仅第三方应用开放本族端点：父接口零端点，端点全部由唯一第三方子接口承载");

        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
        derived.Should().BeEquivalentTo(new[] { child.Name },
            "推广二维码·企业注册族官方仅第三方应用开放（自建 / 代开发无对应 API），继承链上不得出现其它子接口");

        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(PromotionQrCodeRegistryGroupName,
            $"{child.Name} 必须挂 {PromotionQrCodeRegistryGroupName} 注册组" +
            $"（经 Add{PromotionQrCodeRegistryGroupName}WebApiHttpClient() 注册）");
        childApi.InheritedFrom.Should().Be(RegisterParentImplementationClassName,
            $"{child.Name} 必须继承父接口生成实现类 {RegisterParentImplementationClassName}");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, $"{child.Name} 承载本族全部 2 条官方端点");
    }

    /// <summary>
    /// 契约守卫 PQ3：通讯录迁移族接口层级——独立接口（无父接口、无应用类型子接口）、挂同一注册组，
    /// 且<b>不得声明 <c>[Token]</c></b>（该凭证非 SDK 管理，必须以显式 Query 参数传入）。
    /// </summary>
    [Fact]
    public void PromotionQrCodeContactSync_ShouldBeTokenlessStandaloneInterface()
    {
        var iface = typeof(IWechatWorkPromotionQrCodeContactSyncService);

        // 显式传令牌形态：绝不可声明 [Token]（否则会把 provider_access_token / 企业 access_token 错注入）。
        iface.GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "通讯录迁移族的 access_token 非 SDK 管理凭证（官方：请注意与 provider_access_token 的区别），"
            + "必须以显式 [Query(\"access_token\")] 传入；若声明 [Token] 会错注入其它令牌");

        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull($"{iface.Name} 必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(PromotionQrCodeRegistryGroupName,
            $"{iface.Name} 必须挂 {PromotionQrCodeRegistryGroupName} 注册组（与本域企业注册族共用同一 HttpClient 注册项）");

        // 无父接口：接口自身即为根（生成实现类名 = 接口名去 I 前缀）。
        iface.GetInterfaces().Should().BeEmpty($"{iface.Name} 无父接口（端点直接声明于本接口）");
        ContactSyncImplementationClassName.Should().Be("WechatWorkPromotionQrCodeContactSyncService",
            "通讯录迁移族的生成实现类名锁定（接口名去 I 前缀）");

        iface.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, $"{iface.Name} 承载本族全部 2 条官方端点");

        // 两个端点都必须以显式 Query 参数承载 access_token，且参数名与官方契约一致。
        foreach (var method in new[]
                 {
                     nameof(IWechatWorkPromotionQrCodeContactSyncService.SetAuthorizedAppScopeAsync),
                     nameof(IWechatWorkPromotionQrCodeContactSyncService.SetContactSyncSuccessAsync),
                 })
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var tokenParam = target!.GetParameters().FirstOrDefault(p =>
                p.GetCustomAttribute<QueryAttribute>()?.Name == "access_token");
            tokenParam.Should().NotBeNull(
                $"{method} 必须以显式 [Query(\"access_token\")] 参数承载通讯录迁移凭证（官方参数名 access_token）");
        }
    }

    /// <summary>
    /// 契约守卫 PQ4：令牌绑定——企业注册族两接口统一消费 ProviderAccessToken 路由键并以 Query 注入，
    /// 且<b>不声明凭据归属域键</b>（该令牌本身已无歧义，见归属域守卫 TO1）。
    /// </summary>
    [Fact]
    public void PromotionQrCodeTokenBinding_ShouldBeProviderAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkServicePromotionQrCodeService),
            typeof(IWechatWorkThirdPartyServicePromotionQrCodeService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.ProviderAccessToken,
                $"{iface.Name} 令牌路由键必须为 ProviderAccessToken（服务商级凭证）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("provider_access_token",
                $"{iface.Name} Query 注入参数名必须为官方契约的 provider_access_token");

            // 反射陷阱：未显式书写时读到的是构造函数默认值哨兵，此处断言「不等于两个归属域键」。
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
                $"{iface.Name} 已消费 provider_access_token，不得叠加自建归属域键");
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
                $"{iface.Name} 已消费 provider_access_token，不得叠加授权企业归属域键");
        }
    }

    /// <summary>
    /// 契约守卫 PQ5：推广二维码模块的请求/响应 DTO 必须全量登记进 AOT JSON 上下文
    /// （SerializerClassName 统一为 PromotionQrCode）。
    /// </summary>
    [Fact]
    public void PromotionQrCodeDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = PromotionQrCodeJsonContext.Default;

        var domainTypes = typeof(GetRegisterCodeResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.PromotionQrCode"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记（请求 3 + 响应 3 + 嵌套对象 2 = 8；
        // 「设置通讯录同步完成」无业务负载 ⇒ 直接返回 WechatWorkResponse，不新建空响应 DTO）。
        domainTypes.Should().HaveCount(8,
            "推广二维码模块契约面类型数漂移须先核对官方文档再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于推广二维码域命名空间，必须登记进 PromotionQrCodeJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be(PromotionQrCodeRegistryGroupName,
                $"{type.Name} 的 SerializerClassName 必须为推广二维码域段 PromotionQrCode");
        }
    }

    /// <summary>
    /// 契约守卫 PQ6：官方契约陷阱锁定——可见范围三参数<b>未填即清空</b>（故须为可空、缺省即清空）、
    /// <c>follow_user</c> 为 userid 字符串、<c>state</c> 官方仅允许英文字母与数字；
    /// 字段名一律照抄官方原文。
    /// </summary>
    [Fact]
    public void PromotionQrCodeDataModels_ShouldLockOfficialContractTraps()
    {
        // 设置授权应用可见范围：三者可空 —— 官方明文「若未填该字段，则清空可见范围中成员/部门/标签列表」。
        typeof(SetAuthorizedAppScopeRequest).GetProperty(nameof(SetAuthorizedAppScopeRequest.AllowUser))!
            .PropertyType.Should().Be(typeof(List<string>), "allow_user 为 userid 字符串数组");
        JsonNameShouldBe(typeof(SetAuthorizedAppScopeRequest), nameof(SetAuthorizedAppScopeRequest.AllowUser), "allow_user");
        JsonNameShouldBe(typeof(SetAuthorizedAppScopeRequest), nameof(SetAuthorizedAppScopeRequest.AllowParty), "allow_party");
        JsonNameShouldBe(typeof(SetAuthorizedAppScopeRequest), nameof(SetAuthorizedAppScopeRequest.AllowTag), "allow_tag");

        // 非法项列表：官方返回 invaliduser / invalidparty / invalidtag（复数与请求侧单数字段名不同，勿混用）。
        JsonNameShouldBe(typeof(SetAuthorizedAppScopeResponse), nameof(SetAuthorizedAppScopeResponse.InvalidUser), "invaliduser");
        JsonNameShouldBe(typeof(SetAuthorizedAppScopeResponse), nameof(SetAuthorizedAppScopeResponse.InvalidParty), "invalidparty");
        JsonNameShouldBe(typeof(SetAuthorizedAppScopeResponse), nameof(SetAuthorizedAppScopeResponse.InvalidTag), "invalidtag");

        // contact_sync 仅当注册推广包开启通讯录迁移接口时返回 ⇒ 可空；其 access_token 为通讯录凭证。
        typeof(GetRegisterInfoResponse).GetProperty(nameof(GetRegisterInfoResponse.ContactSync))!
            .PropertyType.Should().Be(typeof(ContactSyncCredential), "contact_sync 为嵌套对象");
        JsonNameShouldBe(typeof(GetRegisterInfoResponse), nameof(GetRegisterInfoResponse.ContactSync), "contact_sync");
        JsonNameShouldBe(typeof(ContactSyncCredential), nameof(ContactSyncCredential.AccessToken), "access_token");
        JsonNameShouldBe(typeof(ContactSyncCredential), nameof(ContactSyncCredential.ExpiresIn), "expires_in");

        // state 未指定则响应中无该字段 ⇒ 可空。
        typeof(GetRegisterInfoResponse).GetProperty(nameof(GetRegisterInfoResponse.State))!
            .PropertyType.Should().Be(typeof(string), "state 为字符串，未指定时无该字段");

        // 企业注册族请求体字段名照抄官方原文；follow_user 为 userid 字符串。
        JsonNameShouldBe(typeof(GetRegisterCodeRequest), nameof(GetRegisterCodeRequest.TemplateId), "template_id");
        JsonNameShouldBe(typeof(GetRegisterCodeRequest), nameof(GetRegisterCodeRequest.CorpName), "corp_name");
        JsonNameShouldBe(typeof(GetRegisterCodeRequest), nameof(GetRegisterCodeRequest.AdminName), "admin_name");
        JsonNameShouldBe(typeof(GetRegisterCodeRequest), nameof(GetRegisterCodeRequest.AdminMobile), "admin_mobile");
        JsonNameShouldBe(typeof(GetRegisterCodeRequest), nameof(GetRegisterCodeRequest.State), "state");
        JsonNameShouldBe(typeof(GetRegisterCodeRequest), nameof(GetRegisterCodeRequest.FollowUser), "follow_user");
        JsonNameShouldBe(typeof(GetRegisterCodeResponse), nameof(GetRegisterCodeResponse.RegisterCode), "register_code");
        JsonNameShouldBe(typeof(GetRegisterCodeResponse), nameof(GetRegisterCodeResponse.ExpiresIn), "expires_in");
        JsonNameShouldBe(typeof(GetRegisterInfoRequest), nameof(GetRegisterInfoRequest.RegisterCode), "register_code");
        JsonNameShouldBe(typeof(GetRegisterInfoResponse), nameof(GetRegisterInfoResponse.Corpid), "corpid");
        JsonNameShouldBe(typeof(GetRegisterInfoResponse), nameof(GetRegisterInfoResponse.TemplateId), "template_id");
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
