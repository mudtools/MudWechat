// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.PasswordExemptContract;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-4 品牌线「免密支付契约」域契约守卫 —— <b>6 端点</b>：清算机构面两支预签约 + 服务商面两支预签约 + 协议查询 + 协议解约。
/// </summary>
/// <remarks>
/// <para><b>两套标识体系不得互换（本域红线）</b>：<c>/contracts/qs/*</c> 是【从业机构（支付机构）】面，用 <c>appid</c> /<c>openid</c>（<c>string(128)</c>）；<c>/contracts/service-provider/*</c> 是【普通服务商】面，用 <c>sp_appid</c> /<c>sp_openid</c>（<c>string(64)</c>）⇒ 官方字段名与长度上限都不同，四支预签约<b>各建一型</b>（PEC-B1）。</para>
/// <para><b>应答差异逐支取证</b>：服务商面比清算机构面多 <c>sub_mchid</c>（小程序 7 行 vs 6 行、支付后 4 行 vs 3 行）；小程序场景比支付后场景多 <c>sign_mp_appid</c> / <c>sign_mp_path</c>（拉起签约小程序用）⇒ 两场景、两面共四型，不得归并（PEC-B3）。</para>
/// <para><b>查询与解约以两个 path 段共同定位</b>：<c>service_id</c>（<c>integer</c>）+ <c>out_contract_code</c>（<c>string(32)</c>）⇒ 官方<b>无</b>按 <c>contract_id</c> 的支；两支应答 15 行与 3 行不同形 ⇒ 分建（PEC-B2）。</para>
/// <para><b>本域是本批次唯一「身份按支分裂」的品牌面</b>：qs 两支的官方页支持身份是【从业机构（支付机构）】，而查询与解约两支的页面同时印【普通服务商】【从业机构（支付机构）】【平台商户】⇒ 签名主体由宿主作用域决定，SDK 不做身份断言。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayPasswordExemptContractContractGuards
{
    private const string RegistryGroupName = "PasswordExemptContract";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PasswordExemptContract";

    private static readonly Type Service = typeof(IWechatPayPasswordExemptContractService);

    /// <summary>PEC-B1：拓扑 —— 一个注册组一个接口、6 个方法、6 条 (方法, 路由)、动词集 POST / GET；路由前缀 /v3/password-exempt-contract/。</summary>
    [Fact]
    public void PasswordExemptContractTopology_ShouldBeOneInterfaceOnOneGroup()
    {
        var api = Service.GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
            .Should().Be(RegistryGroupName, "组名即源生成注册入口名（改组名即改 Add{组}WebApiHttpClient）");
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");

        Service.GetCustomAttributes(false)
            .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        Service.GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线无 access_token（红线 PAY-B1）");
        Service.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(6, "免密支付契约声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(6, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/password-exempt-contract/", StringComparison.Ordinal),
            "本域路由全在 /v3/password-exempt-contract/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(4);
    }

    /// <summary>PEC-B2：6 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void PasswordExemptContractEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayPasswordExemptContractService.CreateQsMiniProgramPreEntrustSignAsync)).Should().Be("/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/mini-program");
        // 清算机构模式小程序场景预签约｜官方页 partner/4012471282｜身份 从业机构（支付机构）

        RouteOf(nameof(IWechatPayPasswordExemptContractService.CreateQsPayRedirectPreEntrustSignAsync)).Should().Be("/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/pay-redirect-sign");
        // 清算机构模式支付后场景预签约｜官方页 partner/4012471286｜身份 从业机构（支付机构）

        RouteOf(nameof(IWechatPayPasswordExemptContractService.CreateSpMiniProgramPreEntrustSignAsync)).Should().Be("/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/mini-program");
        // 服务商模式小程序场景预签约｜官方页 partner/4012471269｜身份 普通服务商

        RouteOf(nameof(IWechatPayPasswordExemptContractService.CreateSpPayRedirectPreEntrustSignAsync)).Should().Be("/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/pay-redirect-sign");
        // 服务商模式支付后场景预签约｜官方页 partner/4012471272｜身份 普通服务商

        RouteOf(nameof(IWechatPayPasswordExemptContractService.QueryPasswordExemptContractAsync)).Should().Be("/v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}");
        PathKeysOf(nameof(IWechatPayPasswordExemptContractService.QueryPasswordExemptContractAsync)).Should().BeEquivalentTo(new[] { "serviceId", "outContractCode" });
        // 免密支付协议查询｜官方页 partner/4012715321｜身份 普通服务商

        RouteOf(nameof(IWechatPayPasswordExemptContractService.TerminatePasswordExemptContractAsync)).Should().Be("/v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}/terminate");
        PathKeysOf(nameof(IWechatPayPasswordExemptContractService.TerminatePasswordExemptContractAsync)).Should().BeEquivalentTo(new[] { "serviceId", "outContractCode" });
        // 免密支付协议解约｜官方页 partner/4012715383｜身份 普通服务商
    }

    /// <summary>PEC-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void PasswordExemptContractDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<PasswordExemptQsMiniProgramPreSignRequest>().Should().BeEquivalentTo(new[] { "appid", "sub_appid", "sub_mchid", "service_id", "out_contract_code", "notify_url", "contract_display_account", "openid", "sub_openid" },
            "清算机构模式小程序场景预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PasswordExemptQsMiniProgramPreSignResponse>().Should().BeEquivalentTo(new[] { "pre_entrustweb_id", "mchid", "service_id", "out_contract_code", "sign_mp_appid", "sign_mp_path" },
            "清算机构模式小程序场景预签约应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<PasswordExemptQsPayRedirectPreSignRequest>().Should().BeEquivalentTo(new[] { "appid", "sub_appid", "sub_mchid", "service_id", "out_contract_code", "notify_url", "contract_display_account", "prepay_id", "openid", "sub_openid" },
            "清算机构模式支付后场景预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PasswordExemptQsPayRedirectPreSignResponse>().Should().BeEquivalentTo(new[] { "mchid", "service_id", "out_contract_code" },
            "清算机构模式支付后场景预签约应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<PasswordExemptSpMiniProgramPreSignRequest>().Should().BeEquivalentTo(new[] { "sp_appid", "sp_openid", "sub_mchid", "sub_appid", "sub_openid", "service_id", "out_contract_code", "notify_url", "contract_display_account" },
            "服务商模式小程序场景预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PasswordExemptSpMiniProgramPreSignResponse>().Should().BeEquivalentTo(new[] { "pre_entrustweb_id", "mchid", "sub_mchid", "service_id", "out_contract_code", "sign_mp_appid", "sign_mp_path" },
            "服务商模式小程序场景预签约应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<PasswordExemptSpPayRedirectPreSignRequest>().Should().BeEquivalentTo(new[] { "sp_appid", "sp_openid", "sub_mchid", "sub_appid", "sub_openid", "service_id", "out_contract_code", "notify_url", "contract_display_account", "prepay_id" },
            "服务商模式支付后场景预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PasswordExemptSpPayRedirectPreSignResponse>().Should().BeEquivalentTo(new[] { "mchid", "sub_mchid", "service_id", "out_contract_code" },
            "服务商模式支付后场景预签约应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<PasswordExemptContractQueryResponse>().Should().BeEquivalentTo(new[] { "mchid", "contract_id", "appid", "openid", "sub_appid", "sub_openid", "service_id", "out_contract_code", "contract_display_account", "contract_state", "contract_signed_time", "contract_terminate_info" },
            "免密支付协议查询应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<PasswordExemptContractTerminateRequest>().Should().BeEquivalentTo(new[] { "contract_termination_remark" },
            "免密支付协议解约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PasswordExemptContractTerminateResponse>().Should().BeEquivalentTo(new[] { "mchid", "service_id", "out_contract_code" },
            "免密支付协议解约应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>PEC-B4：判错面 + AOT 上下文登记 + DI 与模块接线（5 请求型 / 6 应答型）。</summary>
    [Fact]
    public void PasswordExemptContractDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(12, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            PasswordExemptContractJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PasswordExemptContractJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 PasswordExemptContract");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(PasswordExemptQsMiniProgramPreSignResponse), typeof(PasswordExemptQsPayRedirectPreSignResponse), typeof(PasswordExemptSpMiniProgramPreSignResponse), typeof(PasswordExemptSpPayRedirectPreSignResponse), typeof(PasswordExemptContractQueryResponse), typeof(PasswordExemptContractTerminateResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(PasswordExemptQsMiniProgramPreSignRequest), typeof(PasswordExemptQsPayRedirectPreSignRequest), typeof(PasswordExemptSpMiniProgramPreSignRequest), typeof(PasswordExemptSpPayRedirectPreSignRequest), typeof(PasswordExemptContractTerminateRequest) })
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PasswordExemptContractJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 PasswordExemptContract 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPasswordExemptContractWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 PasswordExemptContract 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayPasswordExemptContractService.{methodName} 必须存在");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => $"{a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant()} {a.RequestUri}"))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(Root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
