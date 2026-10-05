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
using Mud.Wechat.Work.DataModels.Wedrive;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 微盘模块（Wedrive 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 两功能族形态（均为三类应用公共面收敛父接口 + 三个应用类型空标记子接口）：
/// 管理空间族（新建空间 + 重命名空间 + 解散空间 + 获取空间信息 4 端点）；
/// 管理空间权限族（添加成员/部门 + 移除成员/部门 + 安全设置 + 获取邀请链接 +
/// 获取空间信息（新版）5 端点）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：微盘域 9 条路由官方全部即 POST（含仅查询语义的
/// space_info / new_space_info / space_share）；「获取空间信息」存在新旧两条路由——旧版
/// <c>space_info</c> 挂官方「管理空间」分组，新版 <c>new_space_info</c> 额外返回安全设置
/// secure_setting 与已退出空间成员 quit_userid、官方归入「管理空间权限」分组，两版响应结构
/// 不同构，本 SDK 以两个类型分别承载；「禁止文件分享到企业外」写入口（space_setting）官方
/// 字段名作 <c>ban_share_external</c>、读取回显（new_space_info.secure_setting）作
/// <c>enable_share_external</c>，两侧不同名；移除成员/部门（space_acl_del）官方请求形态不携带
/// auth 字段；空间 ID 官方字段名作 <c>spaceid</c>（无下划线）；应用空间管理员（auth = 7）
/// 最多可指定 3 个且不支持设置部门；安全设置仅支持设置由本应用创建的空间。
/// </para>
/// </remarks>
public class WechatWedriveContractGuards
{
    private const string WedriveRegistryGroupName = "Wedrive";

    // ------------------------------------------------------------------
    // 路由表：9 条官方路由，全部 POST（勿「顺手统一」为 GET）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveSpaceRoutes =
        {
            // 新建空间（自建 93655 / 第三方 95857 / 代开发 96845；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.CreateSpaceAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_create"),
            // 重命名空间（自建 97856 / 第三方 97872 / 代开发 97862；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.RenameSpaceAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_rename"),
            // 解散空间（自建 97857 / 第三方 97873 / 代开发 97863；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.DismissSpaceAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_dismiss"),
            // 获取空间信息·旧版（自建 97858 / 第三方 97874 / 代开发 97864；三类公共收敛父接口；
            // 挂官方「管理空间」分组）。
            (typeof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkWedriveSpaceService.GetSpaceInfoAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_info"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        WedriveSpaceAclRoutes =
        {
            // 添加成员/部门（自建 93656 / 第三方 95858 / 代开发 96846；三类公共收敛父接口；
            // auth = 7 应用空间管理员最多 3 个且不支持部门）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.AddSpaceAclAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_acl_add"),
            // 移除成员/部门（自建 97875 / 第三方 97947 / 代开发 97910；三类公共收敛父接口；
            // 官方请求形态不携带 auth 字段）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.DelSpaceAclAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_acl_del"),
            // 安全设置（自建 97876 / 第三方 97948 / 代开发 97911；三类公共收敛父接口；
            // 仅支持设置由本应用创建的空间）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.SetSpaceSettingAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_setting"),
            // 获取邀请链接（自建 97877 / 第三方 97949 / 代开发 97912；三类公共收敛父接口）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.GetSpaceShareUrlAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/space_share"),
            // 获取空间信息·新版（自建 97878 / 第三方 97950 / 代开发 97913；三类公共收敛父接口；
            // 官方将其归入「管理空间权限」分组而非「管理空间」）。
            (typeof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkWedriveSpaceAclService.GetNewSpaceInfoAsync),
                typeof(PostAttribute), "/cgi-bin/wedrive/new_space_info"),
        };

    // ------------------------------------------------------------------
    // WR1：全部端点路由与官方契约一致（9 条路由，两族 4 + 5）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveEndpoints_ShouldMatchOfficialRoutes()
    {
        // 管理空间族公共面：4 端点收敛父接口。
        WedriveSpaceRoutes.Should().HaveCount(4, "管理空间族公共面 = 新建 + 重命名 + 解散 + 获取空间信息");
        WedriveSpaceRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/", StringComparison.Ordinal), "微盘域路由位于 /cgi-bin/wedrive/ 段");

        // 管理空间权限族公共面：5 端点收敛父接口（新版获取空间信息官方挂本组）。
        WedriveSpaceAclRoutes.Should().HaveCount(5, "管理空间权限族公共面 = 添加/移除成员部门 + 安全设置 + 获取邀请链接 + 新版获取空间信息");
        WedriveSpaceAclRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/wedrive/", StringComparison.Ordinal), "微盘域路由位于 /cgi-bin/wedrive/ 段");

        AssertRoutes(WedriveSpaceRoutes);
        AssertRoutes(WedriveSpaceAclRoutes);

        // 全部官方路由清单锁定（9 条，space_info 与 new_space_info 为两条独立官方路由）。
        var allRoutes = WedriveSpaceRoutes.Concat(WedriveSpaceAclRoutes)
            .Select(r => r.Route).Distinct().ToList();
        allRoutes.Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/wedrive/space_create",
            "/cgi-bin/wedrive/space_rename",
            "/cgi-bin/wedrive/space_dismiss",
            "/cgi-bin/wedrive/space_info",
            "/cgi-bin/wedrive/space_acl_add",
            "/cgi-bin/wedrive/space_acl_del",
            "/cgi-bin/wedrive/space_setting",
            "/cgi-bin/wedrive/space_share",
            "/cgi-bin/wedrive/new_space_info",
        }, "微盘域全部官方路由须与官方文档一一对应");
        allRoutes.Should().HaveCount(9, "微盘域共 9 条官方路由（新旧获取空间信息为两条独立路由）");

        // 无业务负载端点：响应直接用 WechatWorkResponse，不得新建空响应 DTO
        //（重命名 / 解散 / 添加成员部门 / 移除成员部门 / 安全设置共 5 个）。
        var noPayloadEndpoints = new (Type Interface, string Method)[]
        {
            (typeof(IWechatWorkWedriveSpaceService), nameof(IWechatWorkWedriveSpaceService.RenameSpaceAsync)),
            (typeof(IWechatWorkWedriveSpaceService), nameof(IWechatWorkWedriveSpaceService.DismissSpaceAsync)),
            (typeof(IWechatWorkWedriveSpaceAclService), nameof(IWechatWorkWedriveSpaceAclService.AddSpaceAclAsync)),
            (typeof(IWechatWorkWedriveSpaceAclService), nameof(IWechatWorkWedriveSpaceAclService.DelSpaceAclAsync)),
            (typeof(IWechatWorkWedriveSpaceAclService), nameof(IWechatWorkWedriveSpaceAclService.SetSpaceSettingAsync)),
        };
        foreach (var (iface, method) in noPayloadEndpoints)
        {
            iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
                .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                    $"{iface.Name}.{method} 仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        }
    }

    // ------------------------------------------------------------------
    // WR2：接口层级与生成器注册形态（两族继承链、父/子端点数与开放面收敛）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 管理空间族：公共父 4 端点，三个应用类型子接口均为零端点空标记（9 端点官方对三类应用开放一致）。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveSpaceService),
            parentImplementation: "WechatWorkWedriveSpaceService",
            parentDeclaredEndpointCount: 4,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveSpaceService), 0),
                (typeof(IWechatWorkProviderWedriveSpaceService), 0),
                (typeof(IWechatWorkThirdPartyWedriveSpaceService), 0),
            });

        // 管理空间权限族：公共父 5 端点，三个应用类型子接口均为零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkWedriveSpaceAclService),
            parentImplementation: "WechatWorkWedriveSpaceAclService",
            parentDeclaredEndpointCount: 5,
            new[]
            {
                (typeof(IWechatWorkInternalWedriveSpaceAclService), 0),
                (typeof(IWechatWorkProviderWedriveSpaceAclService), 0),
                (typeof(IWechatWorkThirdPartyWedriveSpaceAclService), 0),
            });
    }

    /// <summary>族断言：父接口 IsAbstract + 指定端点数，子接口集合不漂移 + 指定端点数 + 注册组/继承契约。</summary>
    private static void AssertFamily(
        Type parent,
        string parentImplementation,
        int parentDeclaredEndpointCount,
        (Type Interface, int DeclaredEndpointCount)[] children)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(parentDeclaredEndpointCount, $"{parent.Name} 承载官方开放面收敛的端点数");

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
        assignable.Should().BeEquivalentTo(children.Select(c => c.Interface),
            $"{parent.Name} 继承链子接口集合不得漂移（官方未开放的应用类型不得补子接口）");

        foreach (var (child, endpointCount) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(WedriveRegistryGroupName,
                $"{child.Name} 必须挂 {WedriveRegistryGroupName} 注册组（Wedrive 模块共用 Add{WedriveRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 为空标记子接口，不承载差异端点");
        }
    }

    // ------------------------------------------------------------------
    // WR3：令牌绑定——微盘域 8 个接口统一 AccessToken 路由键 + Query 注入
    //（归属域键由 WechatTokenOwnerContractGuards 全局锁定，此处不重复）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkWedriveSpaceService),
            typeof(IWechatWorkInternalWedriveSpaceService),
            typeof(IWechatWorkProviderWedriveSpaceService),
            typeof(IWechatWorkThirdPartyWedriveSpaceService),
            typeof(IWechatWorkWedriveSpaceAclService),
            typeof(IWechatWorkInternalWedriveSpaceAclService),
            typeof(IWechatWorkProviderWedriveSpaceAclService),
            typeof(IWechatWorkThirdPartyWedriveSpaceAclService),
        };

        accessTokenInterfaces.Should().HaveCount(8, "微盘域两族 = 管理空间族 4 接口 + 管理空间权限族 4 接口");

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    // ------------------------------------------------------------------
    // WR4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Wedrive）。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveDataModels_ShouldBeRegisteredInJsonContext()
    {
        var wedriveContext = WedriveJsonContext.Default;

        var domainTypes = typeof(CreateWedriveSpaceRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Wedrive"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 WedriveJsonContext 且 SerializerClassName 统一为 Wedrive
        //（生成物 WedriveJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(18,
            "微盘模块契约面类型数漂移须先核对官方文档再同批调整本守卫（管理空间族 9：新建 2 + 重命名 1 + 解散 1 + 获取空间信息（旧版）3；" +
            "管理空间权限族 6：添加 1 + 移除 1 + 安全设置 1 + 获取邀请链接 2 + 获取空间信息（新版）1；共用嵌套 3）");

        foreach (var type in domainTypes)
        {
            wedriveContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于微盘域命名空间，必须登记进 WedriveJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Wedrive",
                $"{type.Name} 的 SerializerClassName 必须为微盘域段 Wedrive");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            // 管理空间族。
            typeof(CreateWedriveSpaceRequest), typeof(CreateWedriveSpaceResponse),
            typeof(RenameWedriveSpaceRequest),
            typeof(DismissWedriveSpaceRequest),
            typeof(GetWedriveSpaceInfoRequest), typeof(GetWedriveSpaceInfoResponse),
            // 管理空间权限族。
            typeof(AddWedriveSpaceAclRequest),
            typeof(DelWedriveSpaceAclRequest),
            typeof(SetWedriveSpaceSettingRequest),
            typeof(GetWedriveSpaceShareUrlRequest), typeof(GetWedriveSpaceShareUrlResponse),
            typeof(GetWedriveNewSpaceInfoRequest), typeof(GetWedriveNewSpaceInfoResponse),
            // 共用嵌套对象。
            typeof(WedriveSpaceAclMember), typeof(WedriveSpaceAuthList),
            typeof(WedriveSpaceInfo), typeof(WedriveNewSpaceInfo), typeof(WedriveSpaceSecureSetting),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于微盘域命名空间");
    }

    // ------------------------------------------------------------------
    // WR5：官方契约陷阱锁定——新旧获取空间信息分形态承载、共用嵌套结构、
    //      ban_share_external/enable_share_external 不同名与 spaceid 拼写形态。
    // ------------------------------------------------------------------

    [Fact]
    public void WedriveDataModels_ShouldLockOfficialContractTraps()
    {
        // 新旧「获取空间信息」响应结构不同构，分别以两个类型承载（新版额外返回 secure_setting）。
        typeof(GetWedriveSpaceInfoResponse).GetProperty(nameof(GetWedriveSpaceInfoResponse.SpaceInfo))!
            .PropertyType.Should().Be(typeof(WedriveSpaceInfo),
                "旧版 space_info 响应结构不含安全设置，必须以 WedriveSpaceInfo 承载");
        typeof(GetWedriveNewSpaceInfoResponse).GetProperty(nameof(GetWedriveNewSpaceInfoResponse.SpaceInfo))!
            .PropertyType.Should().Be(typeof(WedriveNewSpaceInfo),
                "新版 new_space_info 响应结构含 secure_setting，必须以 WedriveNewSpaceInfo 承载");
        typeof(WedriveSpaceInfo).GetProperty("SecureSetting").Should().BeNull(
            "旧版空间信息不承载 secure_setting（新版同名字段仅落 WedriveNewSpaceInfo）");
        typeof(WedriveNewSpaceInfo).GetProperty(nameof(WedriveNewSpaceInfo.SecureSetting))!
            .PropertyType.Should().Be(typeof(WedriveSpaceSecureSetting), "新版空间信息安全设置结构");

        // 新建空间 / 添加成员部门 / 移除成员部门请求与两版响应 auth_list 共用同一成员权限结构
        //（官方参数表同构；移除成员部门不携带 auth，由可空 + WhenWritingNull 保证不序列化）。
        typeof(CreateWedriveSpaceRequest).GetProperty(nameof(CreateWedriveSpaceRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "新建空间 auth_info 与添加/移除成员共用同一结构");
        typeof(AddWedriveSpaceAclRequest).GetProperty(nameof(AddWedriveSpaceAclRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "添加成员 auth_info 与新建空间/移除成员共用同一结构");
        typeof(DelWedriveSpaceAclRequest).GetProperty(nameof(DelWedriveSpaceAclRequest.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "移除成员 auth_info 与新建空间/添加成员共用同一结构");
        typeof(WedriveSpaceAuthList).GetProperty(nameof(WedriveSpaceAuthList.AuthInfo))!
            .PropertyType.Should().Be(typeof(List<WedriveSpaceAclMember>), "空间成员权限列表与请求侧共用同一成员结构");
        typeof(WedriveSpaceAclMember).GetProperty(nameof(WedriveSpaceAclMember.Auth))!
            .PropertyType.Should().Be(typeof(ulong?), "移除成员/部门官方请求不携带 auth，auth 必须可空");

        // 空间 ID 官方字段名作 spaceid（无下划线）——照抄勿「顺手修正」。
        JsonNameShouldBe(typeof(CreateWedriveSpaceResponse), nameof(CreateWedriveSpaceResponse.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(RenameWedriveSpaceRequest), nameof(RenameWedriveSpaceRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(DismissWedriveSpaceRequest), nameof(DismissWedriveSpaceRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(GetWedriveSpaceInfoRequest), nameof(GetWedriveSpaceInfoRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(AddWedriveSpaceAclRequest), nameof(AddWedriveSpaceAclRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(DelWedriveSpaceAclRequest), nameof(DelWedriveSpaceAclRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(GetWedriveSpaceShareUrlRequest), nameof(GetWedriveSpaceShareUrlRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(GetWedriveNewSpaceInfoRequest), nameof(GetWedriveNewSpaceInfoRequest.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(WedriveSpaceInfo), nameof(WedriveSpaceInfo.Spaceid), "spaceid");
        JsonNameShouldBe(typeof(WedriveNewSpaceInfo), nameof(WedriveNewSpaceInfo.Spaceid), "spaceid");

        // 邀请链接字段名与已退出空间成员字段名照抄官方原文。
        JsonNameShouldBe(typeof(GetWedriveSpaceShareUrlResponse), nameof(GetWedriveSpaceShareUrlResponse.SpaceShareUrl), "space_share_url");
        JsonNameShouldBe(typeof(WedriveSpaceAuthList), nameof(WedriveSpaceAuthList.QuitUserid), "quit_userid");
        JsonNameShouldBe(typeof(WedriveSpaceAuthList), nameof(WedriveSpaceAuthList.AuthInfo), "auth_info");

        // 「禁止文件分享到企业外」写入口（space_setting）与读取回显（secure_setting）官方字段名不同构。
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.BanShareExternal), "ban_share_external");
        JsonNameShouldBe(typeof(WedriveSpaceSecureSetting), nameof(WedriveSpaceSecureSetting.EnableShareExternal), "enable_share_external");

        // 安全设置可写字段名照抄官方原文（未填充字段保持原有状态由 WhenWritingNull 保证）。
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.EnableWatermark), "enable_watermark");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.EnableConfidentialMode), "enable_confidential_mode");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.ShareUrlNoApprove), "share_url_no_approve");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.ShareUrlNoApproveDefaultAuth), "share_url_no_approve_default_auth");
        JsonNameShouldBe(typeof(SetWedriveSpaceSettingRequest), nameof(SetWedriveSpaceSettingRequest.DefaultFileScope), "default_file_scope");
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
