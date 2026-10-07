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
using Mud.Wechat.Work.DataModels.DataZone;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 数据与智能专区模块（DataZone 模块）契约守卫：路由表、接口层级、开放面差异与令牌绑定锁定
/// （基础接口域：三类应用公共面收敛父接口 + 差异端点子接口——「获取数据与智能专区授权信息」官方不支持自建、
/// 「获取数据与智能专区文档存档授权信息」官方仅第三方；应用调用专区程序域：三类应用公共面收敛父接口 + 空标记子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：基础接口域 9 个公共端点（设置公钥 / 获取会话存档授权成员列表 / 设置专区接收回调事件 /
/// 会话组件敏感信息隐藏设置×2 / 日志打印级别×2 / 上传临时文件到专区 / 获取文件内容存档授权成员列表）
/// 收敛声明于 <see cref="IWechatWorkDataZoneService"/> 父接口（IsAbstract）；
/// 第三方子接口额外声明 2 个差异端点、代开发子接口额外声明 1 个差异端点、自建子接口为空标记。
/// 应用调用专区程序域 3 个端点（同步调用 / 创建异步任务 / 查询任务结果）收敛声明于
/// <see cref="IWechatWorkDataZoneProgramService"/> 父接口，三个子接口均为空标记。
/// </para>
/// <para>
/// <b>「数据与智能功能族」（/cgi-bin/data/get_conversation_records、/cgi-bin/data/get_message_statistics）
/// 已按证伪回退删除（定夺三 3c，追踪号 DZ-DATA-INTEL-VERIFY-01）</b>：官方三棵文档树
/// （自建 / 第三方 / 代开发）的「数据与智能专区」章节均只有基础接口 / 应用调用专区程序 /
/// 专区程序调用 SDK / 专区程序接收事件通知四组，无任何 HTTP「数据与智能功能族」分组；
/// 官方「获取会话记录」实为专区程序调用 SDK 的 sync_msg 专区面（非 HTTP 端点），
/// 「获取消息统计」官方无任何文档页；两端点路由在官方网关 qyapi.weixin.qq.com 实测 404
/// （同前缀负对照同签名、正对照均 200）。证据链与原方案字段面（对照基准 Senparc，其官方链接即同源讹传）
/// 详见 .docs 定夺三 §6 核实记录。
/// </para>
/// </remarks>
public class WechatDataZoneContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string DataZoneParentImplementationClassName = "WechatWorkDataZoneService";

    private const string DataZoneProgramParentImplementationClassName = "WechatWorkDataZoneProgramService";

    private const string DataZoneRegistryGroupName = "DataZone";

    /// <summary>
    /// 基础接口域公共面官方路由表（父接口 9 条端点；除上传临时文件到专区以 <c>type</c> 走 Query 外均为 JSON 包体）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] DataZoneRoutes =
    {
        // 设置公钥（自建 99961、第三方 99845、代开发 100016）：设置公钥之后消息才开始存档。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetPublicKeyAsync), "/cgi-bin/chatdata/set_public_key"),
        // 获取会话存档授权成员列表（自建 99962、第三方 99846、代开发 100017）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetChatArchiveAuthUserListAsync), "/cgi-bin/chatdata/get_auth_user_list"),
        // 设置专区接收回调事件（自建 99963、第三方 99850、代开发 100018）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetReceiveCallbackAsync), "/cgi-bin/chatdata/set_receive_callback"),
        // 会话组件敏感信息隐藏设置（自建 100139、第三方 100055、代开发 100054）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetHideSensitiveInfoConfigAsync), "/cgi-bin/chatdata/set_hide_sensitiveinfo_config"),
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetHideSensitiveInfoConfigAsync), "/cgi-bin/chatdata/get_hide_sensitiveinfo_config"),
        // 设置/获取日志打印级别（自建 100108、第三方 100106、代开发 100109）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetLogLevelAsync), "/cgi-bin/chatdata/set_log_level"),
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetLogLevelAsync), "/cgi-bin/chatdata/get_log_level"),
        // 上传临时文件到专区（自建 100174、第三方 100140、代开发 100175）：type 走 Query、文件走 multipart。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.UploadMediaAsync), "/cgi-bin/chatdata/upload_media"),
        // 获取文件内容存档授权成员列表（自建 101873、第三方 101681、代开发 101882）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetDocArchiveAuthUserListAsync), "/cgi-bin/docdata/get_auth_user_list"),
    };

    /// <summary>
    /// 基础接口域差异端点官方路由表：第三方 2 条（获取授权信息 + 文档存档授权信息）、代开发 1 条（获取授权信息）；
    /// 自建官方不支持这两端点，不得出现在自建子接口。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] DataZoneDiffRoutes =
    {
        // 获取数据与智能专区授权信息（第三方 100244、代开发 100237；官方标注自建应用不支持）。
        (typeof(IWechatWorkThirdPartyDataZoneService),
            nameof(IWechatWorkThirdPartyDataZoneService.GetCorpAuthInfoAsync), "/cgi-bin/chatdata/get_corp_auth_info"),
        (typeof(IWechatWorkProviderDataZoneService),
            nameof(IWechatWorkProviderDataZoneService.GetCorpAuthInfoAsync), "/cgi-bin/chatdata/get_corp_auth_info"),
        // 获取数据与智能专区文档存档授权信息（第三方 101488；官方标注自建/代开发均不支持）。
        (typeof(IWechatWorkThirdPartyDataZoneService),
            nameof(IWechatWorkThirdPartyDataZoneService.GetDocArchiveAuthInfoAsync), "/cgi-bin/docdata/get_auth_info"),
    };

    /// <summary>
    /// 应用调用专区程序域官方路由表（父接口 3 条端点：同步调用 / 创建异步任务 / 查询任务结果）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] DataZoneProgramRoutes =
    {
        // 应用同步调用专区程序（自建 99965、第三方 99811、代开发 100020）。
        (typeof(IWechatWorkDataZoneProgramService),
            nameof(IWechatWorkDataZoneProgramService.SyncCallProgramAsync), "/cgi-bin/chatdata/sync_call_program"),
        // 创建专区程序调用任务（自建 99966、第三方 99812、代开发 100021）。
        (typeof(IWechatWorkDataZoneProgramService),
            nameof(IWechatWorkDataZoneProgramService.CreateAsyncProgramTaskAsync), "/cgi-bin/chatdata/async_program_task"),
        // 获取专区程序任务结果（自建 99966、第三方 99812、代开发 100021）。
        (typeof(IWechatWorkDataZoneProgramService),
            nameof(IWechatWorkDataZoneProgramService.GetAsyncProgramResultAsync), "/cgi-bin/chatdata/async_program_result"),
    };

    /// <summary>
    /// 契约守卫 DZ1：数据与智能专区基础接口域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 全部端点官方即 POST（含分页查询类，勿改 GET）。
    /// </summary>
    [Fact]
    public void DataZoneEndpoints_ShouldMatchOfficialRoutes()
    {
        DataZoneRoutes.Should().HaveCount(9,
            "基础接口域 9 个端点为三类应用公共面，全部收敛父接口");
        DataZoneRoutes.Select(r => r.Route).Distinct().Should().HaveCount(9, "基础接口域各端点路由互不重复");

        AssertRoutes(DataZoneRoutes);

        // 上传临时文件到专区：type 为官方必填 Query 参数（目前仅支持 file），文件走 multipart/form-data。
        var upload = typeof(IWechatWorkDataZoneService).GetMethod(
            nameof(IWechatWorkDataZoneService.UploadMediaAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        upload.Should().NotBeNull();
        var typeParam = upload!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "type");
        typeParam.Should().NotBeNull("upload_media 的 type 必须以 [Query(\"type\")] 显式承载");
        upload.GetParameters().Should().Contain(p =>
            p.GetCustomAttribute<MultipartFormAttribute>() != null,
            "upload_media 为 multipart/form-data 上传端点（文件标识名 media）");
    }

    /// <summary>
    /// 契约守卫 DZ1b：基础接口域差异端点路由必须与官方契约一致，且开放面与官方标注一致——
    /// 「获取数据与智能专区授权信息」官方不支持自建应用；「获取数据与智能专区文档存档授权信息」官方仅第三方应用开放。
    /// </summary>
    [Fact]
    public void DataZoneDiffEndpoints_ShouldMatchOfficialOpenSurface()
    {
        DataZoneDiffRoutes.Should().HaveCount(3, "差异端点：第三方 2 条 + 代开发 1 条");
        AssertRoutes(DataZoneDiffRoutes);

        // 差异端点不得漂移进官方不支持的应用类型子接口（nameof 取承载接口上的方法名，断言目标为不得声明的子接口）。
        typeof(IWechatWorkInternalDataZoneService)
            .GetMethod(nameof(IWechatWorkThirdPartyDataZoneService.GetCorpAuthInfoAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取数据与智能专区授权信息官方标注自建应用不支持");
        typeof(IWechatWorkInternalDataZoneService)
            .GetMethod(nameof(IWechatWorkThirdPartyDataZoneService.GetDocArchiveAuthInfoAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取数据与智能专区文档存档授权信息官方标注自建应用不支持");
        typeof(IWechatWorkProviderDataZoneService)
            .GetMethod(nameof(IWechatWorkThirdPartyDataZoneService.GetDocArchiveAuthInfoAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取数据与智能专区文档存档授权信息官方标注代开发应用不支持");
    }

    /// <summary>
    /// 契约守卫 DZ1c：应用调用专区程序域全部端点路由必须与官方契约一致。
    /// </summary>
    [Fact]
    public void DataZoneProgramEndpoints_ShouldMatchOfficialRoutes()
    {
        DataZoneProgramRoutes.Should().HaveCount(3, "应用调用专区程序域 3 个端点为三类应用公共面");
        DataZoneProgramRoutes.Select(r => r.Route).Distinct().Should().HaveCount(3, "各端点路由互不重复");

        AssertRoutes(DataZoneProgramRoutes);
    }

    /// <summary>
    /// 契约守卫 DZ2：数据与智能专区接口层级与生成器注册形态——公共端点收敛于 IsAbstract 父接口，
    /// 自建子接口为空标记；第三方/代开发子接口仅承载官方标注的差异端点（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void DataZoneInterfaceHierarchy_ShouldConvergeOnAbstractParentWithDataZoneRegistry()
    {
        // —— 基础接口域 ——
        var baseChildren = new[]
        {
            typeof(IWechatWorkInternalDataZoneService),
            typeof(IWechatWorkThirdPartyDataZoneService),
            typeof(IWechatWorkProviderDataZoneService),
        };

        foreach (var child in baseChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkDataZoneService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkDataZoneService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkDataZoneService),
            DataZoneParentImplementationClassName,
            expectedDeclaredMethods: null,
            new[] { (typeof(IWechatWorkInternalDataZoneService), 0),
                    (typeof(IWechatWorkThirdPartyDataZoneService), 2),
                    (typeof(IWechatWorkProviderDataZoneService), 1) });

        // —— 应用调用专区程序域 ——
        var programChildren = new[]
        {
            typeof(IWechatWorkInternalDataZoneProgramService),
            typeof(IWechatWorkThirdPartyDataZoneProgramService),
            typeof(IWechatWorkProviderDataZoneProgramService),
        };

        foreach (var child in programChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkDataZoneProgramService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkDataZoneProgramService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkDataZoneProgramService),
            DataZoneProgramParentImplementationClassName,
            expectedDeclaredMethods: 3,
            new[] { (typeof(IWechatWorkInternalDataZoneProgramService), 0),
                    (typeof(IWechatWorkThirdPartyDataZoneProgramService), 0),
                    (typeof(IWechatWorkProviderDataZoneProgramService), 0) });
    }

    /// <summary>
    /// 契约守卫 DZ3：令牌绑定——数据与智能专区全部 8 个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；第三方/代开发消费授权企业级令牌，scope = authCorpId）。
    /// </summary>
    [Fact]
    public void DataZoneTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkDataZoneService),
            typeof(IWechatWorkInternalDataZoneService),
            typeof(IWechatWorkThirdPartyDataZoneService),
            typeof(IWechatWorkProviderDataZoneService),
            typeof(IWechatWorkDataZoneProgramService),
            typeof(IWechatWorkInternalDataZoneProgramService),
            typeof(IWechatWorkThirdPartyDataZoneProgramService),
            typeof(IWechatWorkProviderDataZoneProgramService),
        };

        foreach (var iface in interfaces)
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
    /// 契约守卫 DZ4：数据与智能专区模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 24 个契约面类型）。
    /// </summary>
    [Fact]
    public void DataZoneDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = DataZoneJsonContext.Default;

        var requiredTypes = new Type[]
        {
            // 设置公钥。
            typeof(SetDataZonePublicKeyRequest),
            // 获取会话存档授权成员列表。
            typeof(GetChatArchiveAuthUserListRequest), typeof(GetChatArchiveAuthUserListResponse),
            typeof(ChatArchiveAuthUser),
            // 设置专区接收回调事件。
            typeof(SetDataZoneReceiveCallbackRequest),
            // 会话组件敏感信息隐藏设置。
            typeof(SetDataZoneHideSensitiveInfoConfigRequest), typeof(GetDataZoneHideSensitiveInfoConfigRequest),
            typeof(GetDataZoneHideSensitiveInfoConfigResponse), typeof(DataZoneSensitiveInfoHideConfig),
            // 设置/获取日志打印级别。
            typeof(SetDataZoneLogLevelRequest), typeof(GetDataZoneLogLevelRequest), typeof(GetDataZoneLogLevelResponse),
            // 上传临时文件到专区。
            typeof(UploadDataZoneMediaResponse),
            // 获取文件内容存档授权成员列表。
            typeof(GetDocArchiveAuthUserListRequest), typeof(GetDocArchiveAuthUserListResponse),
            typeof(DocArchiveAuthUser),
            // 应用同步调用专区程序。
            typeof(SyncCallDataZoneProgramRequest), typeof(SyncCallDataZoneProgramResponse),
            // 应用异步调用专区程序。
            typeof(CreateDataZoneAsyncProgramTaskRequest), typeof(CreateDataZoneAsyncProgramTaskResponse),
            typeof(GetDataZoneAsyncProgramResultRequest), typeof(GetDataZoneAsyncProgramResultResponse),
            // 获取数据与智能专区授权信息 / 文档存档授权信息。
            typeof(GetDataZoneCorpAuthInfoResponse), typeof(DataZoneCorpAuthEdition), typeof(DataZoneAuthScope),
            typeof(GetDocArchiveAuthInfoResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是数据与智能专区模块契约面类型，必须登记进 DataZoneJsonContext（AOT 源生成）");
        }
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, string Route)[] routes)
    {
        foreach (var (iface, method, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<HttpMethodAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 HTTP 方法特性");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 接口层级断言：父接口 IsAbstract 且不进注册组；子接口挂 DataZone 注册组、继承父实现类；
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
            childApi!.RegistryGroupName.Should().Be(DataZoneRegistryGroupName,
                $"{child.Name} 必须挂 {DataZoneRegistryGroupName} 注册组" +
                $"（DataZone 模块共用 Add{DataZoneRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(declaredMethods,
                    $"{child.Name} 自身声明端点数必须与官方开放面一致（差异端点漂移须先核对官方文档再同批调整守卫）");
        }
    }
}
