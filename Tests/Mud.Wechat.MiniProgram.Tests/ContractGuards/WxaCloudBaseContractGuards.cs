// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.CloudBase;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「云开发（TCB）」域契约守卫（TCB 系列：44 端点路由表 + 请求体面 + 令牌绑定与注册组 +
/// DTO 登记计数与 JSON 上下文 + 官方字段名与官方拼写 + 敏感面不入日志；CBZ：本域零声明路由与跨线不交叉）。
/// </summary>
/// <remarks>
/// <para>
/// <b>取证口径（2026-10-15 逐页核验，留档 <c>.docs/_tmp_pages/p17-*.json</c>）</b>：官方云开发语料共
/// <b>96 条路由</b>，逐行裁定后落地 <b>94 条</b> ⇒ 小程序线 <b>44</b>（<c>/tcb/*</c> 42 +
/// <c>/wxa/getopendata</c> + <c>/wxa/getvoipsign</c>）+ 开放平台线 <b>50</b>（<c>/componenttcb/*</c>）。
/// 本文件只锁小程序侧的 44 条；跨线不交叉由 CBZ2 与开放平台线守卫双向锁定。
/// </para>
/// <para>
/// <b>两处文档树、一套路由</b>：同一 <c>/tcb/*</c> 路由在
/// <c>miniprogram/dev/server/API/cloudbase/*</c>（自研面）与
/// <c>doc/oplatform/openApi/cloudbase-common/*</c> + <c>cloudbase-batch/*</c>（通用面）各有一份正文，
/// 但令牌面同为小程序 <c>access_token</c> ⇒ 本域<b>不重复登记</b>（路由表恰 44 条即该裁决的机器形态）。
/// </para>
/// <para>
/// <b>路由错配两支</b>：清单把 <c>getopendata</c> / <c>getvoipsign</c> 记在 <c>/tcb/</c> 前缀下，
/// 官方正文的请求地址实为 <c>/wxa/*</c> ⇒ 本守卫锁真实路由，<c>/tcb/</c> 错配形态由 CBZ1 零声明。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：端点聚合计数（MP-X5 / ContractBaseline）、Query 令牌注入白名单（QT1）、
/// 跨线路由零重叠（MP-X1）由 <c>MiniProgramContractGuards</c> 与 <c>MpQueryTokenWhitelistGuard</c> 承担；
/// 本文件只锁 CloudBase 域内的精确契约。
/// </para>
/// </remarks>
public class WxaCloudBaseContractGuards
{
    private const string CloudBaseRegistryGroupName = "CloudBase";

    private const string CloudBaseDtoNamespace = "Mud.Wechat.MiniProgram.DataModels.CloudBase";

    /// <summary>环境与账号族（8 端点）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] EnvRoutes =
    {
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.CheckMobileConfigAsync), "/tcb/checkmobile"),
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.CreateCloudUserAsync), "/tcb/createclouduser"),
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.CreateEnvAsync), "/tcb/createenvandresource"),
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.GetEnvInfoAsync), "/tcb/getenvinfo"),
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.GetCloudTokenAsync), "/tcb/getqcloudtoken"),
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.ChangeTcbEnvAsync), "/tcb/modifyenv"),
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.ShareEnvAsync), "/tcb/shareenv"),
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.SetCloudAccessTokenAsync), "/tcb/usecloudaccesstoken"),
    };

    /// <summary>数据库与云存储族（16 端点）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] DataRoutes =
    {
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.DeleteTcbCloudFileAsync), "/tcb/batchdeletefile"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.GetDownloadTcbFileLinkAsync), "/tcb/batchdownloadfile"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.GetUploadTcbFileLinkAsync), "/tcb/uploadfile"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.AddDatabaseItemAsync), "/tcb/databaseadd"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.AggregateDatabaseAsync), "/tcb/databaseaggregate"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.AddDatabaseCollectionAsync), "/tcb/databasecollectionadd"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.DeleteDatabaseCollectionAsync), "/tcb/databasecollectiondelete"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.GetDatabaseCollectionAsync), "/tcb/databasecollectionget"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.GetDatabaseCountAsync), "/tcb/databasecount"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.DeleteDatabaseItemAsync), "/tcb/databasedelete"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.ExportDatabaseItemAsync), "/tcb/databasemigrateexport"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.ImportDatabaseItemAsync), "/tcb/databasemigrateimport"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.GetDatabaseMigrateStatusAsync), "/tcb/databasemigratequeryinfo"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.GetDatabaseRecordAsync), "/tcb/databasequery"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.UpdateDatabaseRecordAsync), "/tcb/databaseupdate"),
        (typeof(IWxaCloudBaseDataService), nameof(IWxaCloudBaseDataService.UpdateDatabaseIndexAsync), "/tcb/updateindex"),
    };

    /// <summary>云函数族（9 端点）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] ScfRoutes =
    {
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.AddDelayedFunctionTaskAsync), "/tcb/adddelayedfunctiontask"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.CreateFunctionAsync), "/tcb/createfunction"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.GetFuntionLinkAsync), "/tcb/downloadfunction"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.GetCodeSecretAsync), "/tcb/getcodesecret"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.GetFuntionConfigAsync), "/tcb/getfuncconfig"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.GetYploadSignatureAsync), "/tcb/getuploadsignature"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.InvokeCloudFunctionAsync), "/tcb/invokecloudfunction"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.GetFuntionListAsync), "/tcb/listfunctions"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.GetUploadFuntionConfigAsync), "/tcb/uploadfuncconfig"),
    };

    /// <summary>其它能力族（11 端点：消息推送配置 2 + 短信 3 + 统计与上报 2 + 商户号授权 2 + cloudID 与语音签名 2）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] MiscRoutes =
    {
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.CloudbaseReportAPIAsync), "/tcb/cloudbasereport"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.CreateSendSmsTaskAsync), "/tcb/createsendsmstask"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.DescribeExtensionUploadInfoAsync), "/tcb/describeextensionuploadinfo"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.DescribeSmsRecordsAsync), "/tcb/describesmsrecords"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetCallBackConfigAsync), "/tcb/getcallbackconfig"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetCloudBaseStatisticsAsync), "/tcb/getstatistics"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.SetCallBackConfigAsync), "/tcb/setcallbackconfig"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetWechatPayListAsync), "/tcb/wxpaylist"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetWechatPayAuthAsync), "/tcb/wxpayopenauth"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetOpenDataAsync), "/wxa/getopendata"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetCloudBaseVoIPSignAsync), "/wxa/getvoipsign"),
    };

    /// <summary>官方请求体为空的四支端点（除令牌外无任何入参）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] EmptyBodyEndpoints =
    {
        (typeof(IWxaCloudBaseEnvService), nameof(IWxaCloudBaseEnvService.CreateCloudUserAsync), "/tcb/createclouduser"),
        (typeof(IWxaCloudBaseScfService), nameof(IWxaCloudBaseScfService.GetCodeSecretAsync), "/tcb/getcodesecret"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetCallBackConfigAsync), "/tcb/getcallbackconfig"),
        (typeof(IWxaCloudBaseMiscService), nameof(IWxaCloudBaseMiscService.GetWechatPayListAsync), "/tcb/wxpaylist"),
    };

    /// <summary>
    /// CBZ1：本域<b>零声明</b>路由清单（官方页已下架 / 404 / 无现行详情页 / 清单路由错配 / 归属开放平台线）。
    /// </summary>
    private static readonly string[] ZeroDeclarationRoutes =
    {
        // 裸发短信两支：官方现行目录无对应详情页，能力由 /tcb/createsendsmstask 承载。
        "/tcb/sendsms",
        "/tcb/sendsmsv2",
        // 压测三支：出处页实测 404（压力测试能力官方已从云开发 API 面回收）。
        "/tcb/createpressuretesttask",
        "/tcb/getpressuretestreport",
        "/tcb/getpressureteststatus",
        // 小程序配置两支：官方页面标题即标「即将下架」⇒ 不建模。
        "/tcb/getappconfig",
        "/tcb/uploadappconfig",
        // 清单错配两支：真实请求地址是 /wxa/*（本域已按真实路由落位），/tcb/ 形态不得回潮。
        "/tcb/getopendata",
        "/tcb/getvoipsign",
    };

    /// <summary>契约守卫 TCB1：44 端点路由表必须与官方契约逐字一致（全部 POST）。</summary>
    [Fact]
    public void CloudBaseEndpoints_ShouldMatchOfficialRoutes()
    {
        var all = EnvRoutes.Concat(DataRoutes).Concat(ScfRoutes).Concat(MiscRoutes).ToArray();

        all.Should().HaveCount(44,
            "云开发域开放面为 44 端点（环境与账号 8 + 数据库与云存储 16 + 云函数 9 + 其它能力 11，2026-10-15 逐页核验）");
        all.Select(static r => r.Route).Distinct().Should().HaveCount(44, "路由表不得出现重复条目");

        foreach (var (iface, method, route) in all)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 官方调用方式恒为 POST（本域无 GET 端点）");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文，禁止改写）");
            (route.StartsWith("/tcb/", StringComparison.Ordinal) || route.StartsWith("/wxa/", StringComparison.Ordinal))
                .Should().BeTrue($"{route} 必须落在 /tcb/ 或 /wxa/ 两个已核实的族内");
        }

        // 两支 /wxa/* 是本域唯一非 /tcb/ 前缀成员（清单曾误记为 /tcb/*，官方正文实为 /wxa/*）。
        all.Count(static r => r.Route.StartsWith("/wxa/", StringComparison.Ordinal)).Should().Be(2,
            "/wxa/getopendata 与 /wxa/getvoipsign 是本域的两处路由错配纠正，不得多于两支");
    }

    /// <summary>
    /// 契约守卫 TCB2：请求体面 —— 四支官方请求体为空 ⇒ 不得 <c>[Body]</c>；其余 40 支恰一个 <c>[Body]</c>；
    /// 全域唯一一支方法级 <c>[Query]</c> 是 <c>/wxa/getopendata</c> 的可选 <c>openid</c>。
    /// </summary>
    [Fact]
    public void CloudBaseEndpoints_ShouldRespectRequestBodyAndQuerySurface()
    {
        foreach (var (iface, method, route) in EmptyBodyEndpoints)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetParameters().Should().OnlyContain(static p => p.ParameterType == typeof(CancellationToken),
                $"{route} 官方请求体为空 ⇒ 除 CancellationToken 外不得有参数（不得人为造请求体 DTO）");
        }

        var all = EnvRoutes.Concat(DataRoutes).Concat(ScfRoutes).Concat(MiscRoutes);
        foreach (var (iface, method, route) in all)
        {
            if (EmptyBodyEndpoints.Any(e => e.Method == method))
            {
                continue;
            }

            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetParameters()
                .Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Should().Be(1, $"{iface.Name}.{method} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
        }

        // 方法级 Query 参数全集：本域只有 getopendata 的可选 openid（令牌由接口级 [Token] 注入）。
        var queryNames = all
            .Select(static r => r.Interface.GetMethod(r.Method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!)
            .SelectMany(static t => t.GetParameters())
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Where(static n => n != null)
            .ToList();

        queryNames.Should().BeEquivalentTo(new[] { "openid" },
            "云开发域官方参数面全部走 JSON 请求体，唯一例外是 /wxa/getopendata 的可选 Query openid（照既有各线同名 Query 参数形态，非新增凭据参数）");

        var openData = typeof(IWxaCloudBaseMiscService)
            .GetMethod(nameof(IWxaCloudBaseMiscService.GetOpenDataAsync), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        var openidParameter = openData.GetParameters().Single(static p => p.GetCustomAttribute<QueryAttribute>() != null);
        openidParameter.ParameterType.Should().Be(typeof(string),
            "官方把 openid 列为选填（cloudID 本身已含用户标识）⇒ 必须是可空 string 且默认 null，不得改成必填");
        openidParameter.HasDefaultValue.Should().BeTrue("选填 Query 参数必须有默认值（宿主不传即不发送）");
        openidParameter.DefaultValue.Should().BeNull("默认值恒为 null，不得代填");
    }

    /// <summary>契约守卫 TCB3：令牌绑定与注册形态 —— 四接口共用 CloudBase 注册组、复用小程序令牌。</summary>
    /// <remarks>
    /// 官方各页令牌说明给「<c>access_token</c> / <c>authorizer_access_token</c>」或仅后者，两者都是
    /// 「同一令牌链的代调用形态」⇒ <b>不新增令牌类型</b>（新增会让 <c>MpTokenManagerRegistry</c> 单槽
    /// Resolve 返回 null，errcode 自愈静默失效）。仅 <c>component_access_token</c> 的 <c>/componenttcb/*</c>
    /// 面归开放平台线（CBZ1/CBZ2）。
    /// </remarks>
    [Theory]
    [InlineData(typeof(IWxaCloudBaseEnvService))]
    [InlineData(typeof(IWxaCloudBaseDataService))]
    [InlineData(typeof(IWxaCloudBaseScfService))]
    [InlineData(typeof(IWxaCloudBaseMiscService))]
    public void CloudBaseInterfaces_ShouldBindMpAccessToken_AndShareCloudBaseGroup(Type interfaceType)
    {
        var token = interfaceType.GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull($"{interfaceType.Name} 必须声明接口级 [Token]（本域全部端点消费小程序 access_token）");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
            "云开发域必须复用公众号/小程序同域令牌 Wechat.Mp.AccessToken（不得新增令牌类型）");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3 / QT1）");
        token.Name.Should().Be("access_token", "Query 参数名须照官方原文");

        var api = interfaceType.GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull($"{interfaceType.Name} 必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(CloudBaseRegistryGroupName, "四接口共用 CloudBase 注册组（一支注册器装载 44 端点）");
        api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");
    }

    /// <summary>契约守卫 TCB3b：三段式注册面 —— 枚举成员 ↔ <c>AddCloudBaseApi()</c> 同批成立。</summary>
    [Fact]
    public void CloudBaseModule_ShouldBeRegisteredInBuilder()
    {
        Enum.IsDefined(typeof(MiniProgramModule), MiniProgramModule.CloudBase).Should().BeTrue(
            "MiniProgramModule.CloudBase 必须存在（模块枚举 ↔ 注册入口 ↔ 源生成注册组三段式）");

        typeof(MiniProgramServiceBuilder).GetMethod(nameof(MiniProgramServiceBuilder.AddCloudBaseApi))
            .Should().NotBeNull("AddCloudBaseApi() 必须存在，否则本域 44 端点无装配入口");
    }

    /// <summary>契约守卫 TCB4：DTO 全量登记进 AOT JSON 上下文；命名空间清单锁死（98 DTO，无常量类）。</summary>
    [Fact]
    public void CloudBaseDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaCloudBaseCreateEnvRequest).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == CloudBaseDtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("CloudBase 域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 98 = 请求 45 + 响应 33 + 嵌套 20（逐端点参数表/响应表展开；目录 Env 14 + Database 28 + Storage 10 + Scf 21 + Misc 25）。
        dtoTypes.Should().HaveCount(98,
            "CloudBase 域 44 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方参数表与响应表再同批调整本守卫");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(CloudBaseRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 CloudBase");
        }

        // 生成上下文按反射定位（而非直接类型引用）：上下文尚未生成时守卫给出可执行的修复指引，
        // 而不是让测试工程编译失败——登记本身是「同批完成」契约的一部分，缺失必须红。
        var contextType = assembly.GetType(CloudBaseDtoNamespace + ".CloudBaseJsonContext");
        contextType.Should().NotBeNull(
            "CloudBaseJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"CloudBase\")]，必须登记进 CloudBaseJsonContext" +
                "（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：除已登记 DTO 与生成上下文外不得出现任何新类型
        // （本域官方参数表无数值枚举表 ⇒ 常量类为 0；后续新增须同批更新本守卫）。
        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType)
            .ToList();

        unregistered.Should().BeEmpty(
            "CloudBase 域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文的新类型：" +
            string.Join(", ", unregistered.Select(static t => t.Name)) +
            " —— 常量类或新 DTO 须同批更新本守卫");
    }

    /// <summary>
    /// 契约守卫 TCB5：官方字段名抽样 —— 含本域<b>唯一 PascalCase 字段族</b>（腾讯云风格）与
    /// 官方拼写照抄两支。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>两套线格式在同一域内并存</b>：微信侧端点字段一律 <c>snake_case</c>（<c>env</c> / <c>file_list</c> /
    /// <c>info_list</c>），而 <c>/tcb/describesmsrecords</c>、<c>/tcb/describeextensionuploadinfo</c>、
    /// <c>/tcb/getcallbackconfig</c> 等的请求与响应直接透传<b>腾讯云 API 形状</b> ⇒ 字段名是
    /// <c>EnvId</c> / <c>PageNumber</c> / <c>SmsRecords</c> / <c>FilesData</c>。<b>不得</b>「统一成小写下划线」
    /// ——那会让请求体在网关侧直接参数错误。
    /// </para>
    /// <para>
    /// <b>官方英文接口名拼写</b>：<c>getFuntionLink</c> / <c>getFuntionList</c> / <c>getFuntionConfig</c> /
    /// <c>getUploadFuntionConfig</c>（缺 c）与 <c>getYploadSignature</c>（Y 代 o）照抄进方法名与类型名，
    /// 而<b>路由</b>本身是官方正确拼写（<c>/tcb/getuploadsignature</c>）⇒ 二者不对称是本守卫要显式留档的点。
    /// </para>
    /// </remarks>
    [Fact]
    public void CloudBaseDataModels_ShouldLockOfficialFieldNames()
    {
        // 微信侧 snake_case 面。
        JsonNameShouldBe<WxaCloudBaseGetDatabaseRecordRequest>(nameof(WxaCloudBaseGetDatabaseRecordRequest.Env), "env", "官方参数表原文");
        JsonNameShouldBe<WxaCloudBaseGetDatabaseRecordRequest>(nameof(WxaCloudBaseGetDatabaseRecordRequest.Query), "query", "官方参数表原文");
        JsonNameShouldBe<WxaCloudBaseGetDatabaseRecordResponse>(nameof(WxaCloudBaseGetDatabaseRecordResponse.Pager), "pager", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetDatabaseRecordResponse>(nameof(WxaCloudBaseGetDatabaseRecordResponse.Data), "data", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetDownloadTcbFileLinkRequest>(nameof(WxaCloudBaseGetDownloadTcbFileLinkRequest.FileList), "file_list", "官方参数表原文");
        JsonNameShouldBe<WxaCloudBaseGetDownloadTcbFileLinkResponseFileListItem>(nameof(WxaCloudBaseGetDownloadTcbFileLinkResponseFileListItem.Fileid), "fileid", "官方响应表原文（单字连写，非 file_id）");
        JsonNameShouldBe<WxaCloudBaseGetDownloadTcbFileLinkResponseFileListItem>(nameof(WxaCloudBaseGetDownloadTcbFileLinkResponseFileListItem.DownloadUrl), "download_url", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetUploadTcbFileLinkResponse>(nameof(WxaCloudBaseGetUploadTcbFileLinkResponse.CosFileId), "cos_file_id", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetEnvInfoResponse>(nameof(WxaCloudBaseGetEnvInfoResponse.InfoList), "info_list", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseDeleteTcbCloudFileResponse>(nameof(WxaCloudBaseDeleteTcbCloudFileResponse.DeleteList), "delete_list", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseInvokeCloudFunctionRequest>(nameof(WxaCloudBaseInvokeCloudFunctionRequest.ReqData), "req_data", "官方参数表原文（现行页把 env/name/req_data 全放请求体）");
        JsonNameShouldBe<WxaCloudBaseInvokeCloudFunctionResponse>(nameof(WxaCloudBaseInvokeCloudFunctionResponse.RespData), "resp_data", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetFuntionLinkResponse>(nameof(WxaCloudBaseGetFuntionLinkResponse.Checksum), "checksum", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseUpdateDatabaseIndexRequest>(nameof(WxaCloudBaseUpdateDatabaseIndexRequest.CreateIndexes), "create_indexes", "官方参数表原文");
        JsonNameShouldBe<WxaCloudBaseUpdateDatabaseIndexRequest>(nameof(WxaCloudBaseUpdateDatabaseIndexRequest.DropIndexes), "drop_indexes", "官方参数表原文");
        JsonNameShouldBe<WxaCloudBaseGetCloudBaseStatisticsResponse>(nameof(WxaCloudBaseGetCloudBaseStatisticsResponse.DataColumn), "data_column", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetOpenDataRequest>(nameof(WxaCloudBaseGetOpenDataRequest.CloudidList), "cloudid_list", "官方参数表原文");
        JsonNameShouldBe<WxaCloudBaseGetCloudBaseVoIPSignRequest>(nameof(WxaCloudBaseGetCloudBaseVoIPSignRequest.GroupId), "group_id", "官方参数表原文");

        // 腾讯云透传面：PascalCase 是官方线格式，不得驼峰化/下划线化。
        JsonNameShouldBe<WxaCloudBaseDescribeSmsRecordsRequest>(nameof(WxaCloudBaseDescribeSmsRecordsRequest.EnvId), "EnvId", "腾讯云风格原文（本域两套线格式并存）");
        JsonNameShouldBe<WxaCloudBaseDescribeSmsRecordsRequest>(nameof(WxaCloudBaseDescribeSmsRecordsRequest.PageNumber), "PageNumber", "腾讯云风格原文");
        JsonNameShouldBe<WxaCloudBaseDescribeSmsRecordsResponse>(nameof(WxaCloudBaseDescribeSmsRecordsResponse.SmsRecords), "SmsRecords", "腾讯云风格原文");
        JsonNameShouldBe<WxaCloudBaseDescribeSmsRecordsResponse>(nameof(WxaCloudBaseDescribeSmsRecordsResponse.RequestId), "RequestId", "腾讯云风格原文");
        JsonNameShouldBe<WxaCloudBaseDescribeExtensionUploadInfoRequest>(nameof(WxaCloudBaseDescribeExtensionUploadInfoRequest.ExtensionFiles), "ExtensionFiles", "腾讯云风格原文");
        JsonNameShouldBe<WxaCloudBaseDescribeExtensionUploadInfoResponse>(nameof(WxaCloudBaseDescribeExtensionUploadInfoResponse.FilesData), "FilesData", "腾讯云风格原文");

        // 敏感字段名照官方原文（同时也是 TCB6 的扫描靶名，改名即失焦）。
        JsonNameShouldBe<WxaCloudBaseGetCodeSecretResponse>(nameof(WxaCloudBaseGetCodeSecretResponse.Codesecret), "codesecret", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetCloudTokenResponse>(nameof(WxaCloudBaseGetCloudTokenResponse.Secretid), "secretid", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetCloudTokenResponse>(nameof(WxaCloudBaseGetCloudTokenResponse.Secretkey), "secretkey", "官方响应表原文");
        JsonNameShouldBe<WxaCloudBaseGetUploadTcbFileLinkResponse>(nameof(WxaCloudBaseGetUploadTcbFileLinkResponse.Authorization), "authorization", "官方响应表原文（COS 直传凭证）");
        JsonNameShouldBe<WxaCloudBaseGetYploadSignatureResponse>(nameof(WxaCloudBaseGetYploadSignatureResponse.Headers), "headers", "官方响应表原文（直传签名头串）");

        // 类型口径：官方分页与用量参数为 number ⇒ 落 long?，不得收窄为 int 或改成 string。
        PropertyTypeShouldBe<WxaCloudBaseGetFuntionListRequest>(nameof(WxaCloudBaseGetFuntionListRequest.Limit), typeof(long?));
        PropertyTypeShouldBe<WxaCloudBaseGetFuntionListRequest>(nameof(WxaCloudBaseGetFuntionListRequest.Offset), typeof(long?));
        PropertyTypeShouldBe<WxaCloudBaseGetCloudTokenResponse>(nameof(WxaCloudBaseGetCloudTokenResponse.ExpiredTime), typeof(long?));
        PropertyTypeShouldBe<WxaCloudBaseGetOpenDataRequest>(nameof(WxaCloudBaseGetOpenDataRequest.CloudidList), typeof(List<string>));
        // databasequery 的 data 官方类型列只写 array（元素结构由查询语句决定）⇒ 落 List<JsonElement>，不得凭空造实体。
        PropertyTypeShouldBe<WxaCloudBaseGetDatabaseRecordResponse>(nameof(WxaCloudBaseGetDatabaseRecordResponse.Data), typeof(List<JsonElement>));

        // 凭证有效期缺省 7200 秒（官方「最长 7200 秒」）⇒ lifespan 可空，不得做成必填。
        typeof(WxaCloudBaseGetCloudTokenRequest).GetProperty(nameof(WxaCloudBaseGetCloudTokenRequest.Lifespan))!
            .PropertyType.Should().Be(typeof(long?), "官方 lifespan 选填、缺省 7200 秒且不得大于 7200 ⇒ 由宿主传值，SDK 不代填默认");

        // 官方拼写照抄：五支方法名保留官方 typo，而其路由是正确拼写 ⇒ 不对称须留档而非「顺手修正」。
        foreach (var (method, route) in new[]
                 {
                     (nameof(IWxaCloudBaseScfService.GetFuntionLinkAsync), "/tcb/downloadfunction"),
                     (nameof(IWxaCloudBaseScfService.GetFuntionListAsync), "/tcb/listfunctions"),
                     (nameof(IWxaCloudBaseScfService.GetFuntionConfigAsync), "/tcb/getfuncconfig"),
                     (nameof(IWxaCloudBaseScfService.GetUploadFuntionConfigAsync), "/tcb/uploadfuncconfig"),
                     (nameof(IWxaCloudBaseScfService.GetYploadSignatureAsync), "/tcb/getuploadsignature"),
                 })
        {
            typeof(IWxaCloudBaseScfService).GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeNull($"{method} 是官方英文接口名照抄的方法名，不得改写成正确拼写而破坏可追溯性（路由 {route} 仍是官方正确拼写）");
        }

        // 响应基类复用：仅 errcode/errmsg 的端点返回 WxaResponse，不得为其造空壳 DTO。
        typeof(WxaCloudBaseCreateEnvResponse).IsAssignableTo(typeof(WxaResponse)).Should().BeTrue("本域响应 DTO 一律继承 WxaResponse");
        typeof(IWxaCloudBaseEnvService).GetMethod(nameof(IWxaCloudBaseEnvService.CreateCloudUserAsync))!
            .ReturnType.Should().Be(typeof(Task<WxaResponse>),
                "官方 Request Payload 为空（请求示例 {}）——不得人为造请求体 DTO");
    }

    /// <summary>
    /// 契约守卫 CBZ1：九条「清单在册但非契约」的 <c>/tcb/*</c> 路由在<b>小程序线零声明</b>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 判据是<b>官方文档面</b>而非网关可达性：裸发短信两支与压测三支已无现行详情页（出处页 404）、
    /// 小程序配置两支官方标题即标「即将下架」、<c>/tcb/getopendata</c> 与 <c>/tcb/getvoipsign</c> 是
    /// <b>清单路由错配</b>（官方正文请求地址为 <c>/wxa/*</c>，本域已按真实路由落位）。
    /// </para>
    /// <para>
    /// <b>正对照（防空跑恒真）</b>：扫描必须同时命中本域<b>已落位</b>的相邻路由，否则扫描根或正则漂移
    /// 会让「零声明」静默恒真（同 RD1 / MP-D-BUCKET-RULING-01 口径）。
    /// </para>
    /// </remarks>
    [Fact]
    public void CloudBaseNonContractRoutes_ShouldStayUndeclared_InMiniProgramLine()
    {
        ZeroDeclarationRoutes.Should().HaveCount(9,
            "裸发短信 2 + 压测 3 + 即将下架配置 2 + 清单错配 2：" +
            "清单本身不是契约，增删须先复核官方页存活状态与请求地址正文");
        ZeroDeclarationRoutes.Distinct(StringComparer.Ordinal).Should().HaveCount(9, "清单内路由不得重复");

        var declared = DeclaredRoutesInLine();
        var offenders = declared.Intersect(ZeroDeclarationRoutes, StringComparer.Ordinal).ToList();

        foreach (var control in new[]
                 {
                     "/tcb/getqcloudtoken",   // /tcb/ 前缀的活体证明（下架与错配条目同前缀）
                     "/wxa/getopendata",      // 错配条目的官方真名，本域已按真名落位
                     "/wxa/getvoipsign",
                     "/tcb/createsendsmstask", // sendsms/sendsmsv2 的现行承载端点
                 })
        {
            declared.Should().Contain(control,
                "CBZ1 的扫描必须能捕获小程序线已声明的相邻路由（否则零声明断言静默恒真）");
        }

        offenders.Should().BeEmpty(
            "16 条路由不属小程序线云开发契约：官方下架 / 出处页 404 / 清单路由错配 / 仅 component_access_token 面（归开放平台线）");
    }

    /// <summary>
    /// 契约守卫 CBZ2：<c>/tcb/*</c> 与 <c>/componenttcb/*</c> 两线<b>不交叉</b>（小程序侧持镜像断言）。
    /// </summary>
    /// <remarks>
    /// 小程序线声明 <c>/componenttcb/*</c> 即把「仅 <c>component_access_token</c>」的批量代云开发面
    /// 错误挂到小程序令牌链上 ⇒ 令牌串号。开放平台线侧的镜像断言由 CBT 系列守卫持有，两侧合起来
    /// 才是该路由族的<b>归属唯一性</b>闸门。
    /// </remarks>
    [Fact]
    public void ComponentTcbPrefix_ShouldNotBeDeclared_InMiniProgramLine()
    {
        var declared = DeclaredRoutesInLine();

        declared.Should().NotContain(r => r.StartsWith("/componenttcb/", StringComparison.Ordinal),
            "/componenttcb/* 的令牌面是 component_access_token，属开放平台线；小程序线声明之即令牌串号");

        // 反空跑：本线必须确实声明 /tcb/ 面（若本域被误删，前一条会静默恒真）。
        declared.Count(r => r.StartsWith("/tcb/", StringComparison.Ordinal)).Should().Be(42,
            "小程序线 /tcb/ 面恰 42 条 —— 数量变化说明扫描口径或本域端点漂移");
    }

    /// <summary>
    /// 契约守卫 TCB6：敏感响应面<b>不入日志</b>（形态同 MP-X7，靶名为本域官方字段对应的属性名）。
    /// </summary>
    /// <remarks>
    /// <b>覆盖面</b>：腾讯云永久密钥（<c>Secretid</c> / <c>Secretkey</c>）、临时凭证（<c>Token</c>）、
    /// 代码保护密钥（<c>Codesecret</c>）、COS 直传凭证（<c>Authorization</c>）、直传签名头（<c>Headers</c>）、
    /// 云函数与容器配置（<c>Config</c>，内含环境变量）、用户开放数据（<c>DataList</c>）。
    /// 机械判定：主包源文件内<b>真实日志调用语句</b>（含换行的消息模板窗口）不得出现这些靶名。
    /// 注释必须排除 —— 本域接口的裁决注释会原样写出这些字段名，邻近域的文档注释里还有
    /// <c>WxaLogDataListResponse</c> 这类「Log 后紧跟靶名」的类型名，按裸 <c>Contains("Log")</c> 判定即假红。
    /// </remarks>
    [Fact]
    public void CloudBaseSensitiveFields_ShouldNeverBeLogged()
    {
        var sensitiveProperties = new[]
        {
            "Secretid", "Secretkey", "Codesecret", "Authorization",
            "Headers", "Config", "DataList", "ReqData", "RespData",
        };

        var files = Directory.GetFiles(SourceProjectDir("Mud.Wechat.MiniProgram"), "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToList();

        // 反空跑：扫描根必须确实含 CloudBase 接口源文件，否则 offenders 恒空。
        files.Should().Contain(static f => Path.GetFileName(f).StartsWith("IWxaCloudBase", StringComparison.Ordinal),
            "TCB6 的扫描根必须覆盖 CloudBase 接口源文件（定位失效会让断言静默恒真）");

        var offenders = new List<string>();
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                // 剥行内注释：裁决注释会原样写出靶名（如「Codesecret 禁入日志」），不排除即假红。
                var code = StripLineComment(lines[i]);
                if (!LogInvocation.IsMatch(code))
                {
                    continue;
                }

                // 消息模板常换到下一行 ⇒ 判定窗口取调用起点起的三条语句行。
                var statement = string.Concat(
                    code,
                    i + 1 < lines.Length ? StripLineComment(lines[i + 1]) : string.Empty,
                    i + 2 < lines.Length ? StripLineComment(lines[i + 2]) : string.Empty);
                var hit = sensitiveProperties.FirstOrDefault(p => statement.Contains(p, StringComparison.Ordinal));
                if (hit is not null)
                {
                    offenders.Add($"{Path.GetFileName(file)}:{i + 1}({hit})");
                }
            }
        }

        offenders.Should().BeEmpty(
            "云开发域的密钥、临时凭证、直传凭证、配置（含环境变量）与用户开放数据不得出现在任何日志调用中（AGENTS §8）");
    }

    /// <summary>契约守卫 TCB7：承载敏感面的 DTO <b>不得重写 <c>ToString</c></b>（防结构化日志整体打印）。</summary>
    [Theory]
    [InlineData(typeof(WxaCloudBaseGetCloudTokenResponse))]
    [InlineData(typeof(WxaCloudBaseGetCodeSecretResponse))]
    [InlineData(typeof(WxaCloudBaseGetUploadTcbFileLinkResponse))]
    [InlineData(typeof(WxaCloudBaseGetYploadSignatureResponse))]
    [InlineData(typeof(WxaCloudBaseGetOpenDataResponse))]
    [InlineData(typeof(WxaCloudBaseGetCallBackConfigResponse))]
    public void SensitiveDataModels_ShouldNotOverrideToString(Type dataType)
    {
        dataType.GetMethod("ToString", Type.EmptyTypes)!.DeclaringType
            .Should().Be(typeof(object),
                $"{dataType.Name} 承载密钥/凭证/用户开放数据，不得重写 ToString（一旦被 Log(\"{{Dto}}\") 即整体泄露）");
    }

    // ---- helpers -------------------------------------------------------------

    /// <summary>
    /// 日志调用起点：<c>Log</c> + 级别 + <c>(</c>（级别可省，故 <c>Log("…")</c> 也算）。
    /// 只匹配「调用形态」，不用裸 <c>Contains("Log")</c> —— 后者会把文档注释里的类型名
    /// <c>WxaLogDataListResponse</c>（<c>Log</c> 紧接靶名 <c>DataList</c>）判成日志调用，属假红。
    /// </summary>
    private static readonly Regex LogInvocation = new(
        @"\bLog(?:Information|Warning|Error|Debug|Trace|Critical)?\s*\(",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>剥行内与整行注释（<c>///</c> 亦经此路归空）：裁决注释会原样写出靶名，不排除即假红。</summary>
    private static string StripLineComment(string line)
    {
        var quote = 0;
        for (var i = 0; i < line.Length - 1; i++)
        {
            if (line[i] == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                quote ^= 1;
            }
            else if (quote == 0 && line[i] == '/' && line[i + 1] == '/')
            {
                return line[..i];
            }
        }

        return line;
    }

    /// <summary>
    /// 小程序线「已声明路由」：剥离注释后取 HTTP 特性字面量与引号路径常量
    /// （裁决注释里刻意不实现的 <c>/tcb/sendsms</c> 等会被原样命中，故必须先剥注释）。
    /// </summary>
    private static HashSet<string> DeclaredRoutesInLine()
    {
        var root = SourceProjectDir("Mud.Wechat.MiniProgram");
        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./{}]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal))
            {
                continue;
            }

            var code = string.Join("\n", Regex.Replace(File.ReadAllText(file), @"/\*[\s\S]*?\*/", " ")
                .Split('\n')
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            foreach (System.Text.RegularExpressions.Match m in literalRegex.Matches(code))
            {
                set.Add(m.Groups[1].Value);
            }
        }

        set.Should().NotBeEmpty("防「扫描口径失效导致真空断言」：扫描根必须能扫出已声明路由");
        return set;
    }

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void PropertyTypeShouldBe<T>(string propertyName, Type expectedType)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType, $"{typeof(T).Name}.{propertyName} 的类型须照官方参数表（{expectedType.Name}）");
    }

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
