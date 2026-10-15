// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Mud.HttpUtils;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线「批量代云开发」域（<c>/componenttcb/*</c>，50 端点 / 六支接口）契约守卫。
/// </summary>
/// <remarks>
/// <para><b>本守卫锁定的裁决</b>（逐页取证见各接口 XML 注释与 <c>.docs</c> 落地记录）：</para>
/// <list type="number">
/// <item><description>契约面 = 六支接口 / 50 端点，(方法, 路由) 表逐条锁定（CBT1）。</description></item>
/// <item><description>全域恒 <b>POST</b>、恒 <c>/componenttcb/</c> 前缀、恒以 Query <c>access_token</c> 承载
/// <c>component_access_token</c>、<b>零额外 Query 参数</b>、六接口共用注册组 <c>ComponentTcb</c>（CBT2）。</description></item>
/// <item><description>目录 / 命名空间 / 注册入口三处一致：模块枚举 + builder 委托 + resolver 合并登记（CBT3）。</description></item>
/// <item><description>166 个 DTO 全量登记进 <c>ComponentTcbJsonContext</c>（AOT 源生成完整性，CBT4）。</description></item>
/// <item><description>官方<b>类型与拼写分叉照抄不修正</b>：同一语义在创建页是 <c>number</c>、在灰度更新页是
/// <c>string</c>；<c>image_pull_secret</c> 与 <c>image_pulls_secret</c> 两拼法各自属于不同官方页（CBT5）。</description></item>
/// <item><description>敏感面（镜像仓库凭证、环境变量、带签名临时链接、直传凭证、库记录明文）<b>不入日志</b>、
/// 承载 DTO <b>不重写 <c>ToString</c></b>（CBT6 / CBT7）。</description></item>
/// <item><description>与小程序线 <c>/tcb/*</c> <b>两线不交叉</b>（CBZ2 的本线镜像断言，CBT8）。</description></item>
/// <item><description>官方请求 Payload 为 <c>{}</c> 的 2 条端点<b>无请求体 DTO</b>、官方返回值仅
/// <c>errcode/errmsg</c> 的 14 条端点<b>返回基底响应</b>（CBT9，防人为造 DTO）。</description></item>
/// </list>
/// </remarks>
public class OpenPlatformComponentTcbContractGuards
{
    private static readonly Assembly MainAssembly = typeof(IWechatOpenPlatformComponentTcbEnvService).Assembly;

    private static readonly Assembly DataModelsAssembly = typeof(OpenPlatformComponentTcbCreateTcbEnvRequest).Assembly;

    /// <summary>接口名 →（HTTP 方法, 路由）期望表：50 条路由的归属唯一性由本表锁定（2026-10-15 逐页核验）。</summary>
    private static readonly Dictionary<string, (string Method, string Route)[]> ExpectedEndpoints = new()
    {
        [nameof(IWechatOpenPlatformComponentTcbEnvService)] = new[]
        {
            ("POST", "/componenttcb/createenv"),
            ("POST", "/componenttcb/describeenvs"),
            ("POST", "/componenttcb/batchgetenvid"),
            ("POST", "/componenttcb/batchshareenv"),
            ("POST", "/componenttcb/batchunshareenv"),
            ("POST", "/componenttcb/createcloudbaserunenv"),
            ("POST", "/componenttcb/describecloudbaserunenvs"),
        },
        [nameof(IWechatOpenPlatformComponentTcbDatabaseService)] = new[]
        {
            ("POST", "/componenttcb/dbaggregate"),
            ("POST", "/componenttcb/dbcollection"),
            ("POST", "/componenttcb/dbexport"),
            ("POST", "/componenttcb/dbgetacl"),
            ("POST", "/componenttcb/dbimport"),
            ("POST", "/componenttcb/dbindex"),
            ("POST", "/componenttcb/dbmigrationstate"),
            ("POST", "/componenttcb/dbmodifyacl"),
            ("POST", "/componenttcb/dbrecord"),
        },
        [nameof(IWechatOpenPlatformComponentTcbFileService)] = new[]
        {
            ("POST", "/componenttcb/batchdeletefile"),
            ("POST", "/componenttcb/batchdownloadfile"),
            ("POST", "/componenttcb/getbucket"),
            ("POST", "/componenttcb/uploadfile"),
        },
        [nameof(IWechatOpenPlatformComponentTcbScfService)] = new[]
        {
            ("POST", "/componenttcb/batchuploadscf"),
            ("POST", "/componenttcb/deletescf"),
            ("POST", "/componenttcb/getscflist"),
            ("POST", "/componenttcb/gettriggers"),
            ("POST", "/componenttcb/batchupdatetriggers"),
            ("POST", "/componenttcb/batchuploadscfcode"),
            ("POST", "/componenttcb/updatescfconfig"),
        },
        [nameof(IWechatOpenPlatformComponentTcbCloudRunService)] = new[]
        {
            ("POST", "/componenttcb/batchcreatecsversion"),
            ("POST", "/componenttcb/batchdelcsversion"),
            ("POST", "/componenttcb/batchsetcspropotion"),
            ("POST", "/componenttcb/createcloudbaserunresauth"),
            ("POST", "/componenttcb/createcloudbaserunserverversion"),
            ("POST", "/componenttcb/createcontainerservice"),
            ("POST", "/componenttcb/createcsversion"),
            ("POST", "/componenttcb/delcontainerservice"),
            ("POST", "/componenttcb/delcsversion"),
            ("POST", "/componenttcb/deletecloudbaserunserverversion"),
            ("POST", "/componenttcb/establishcloudbaserunserver"),
            ("POST", "/componenttcb/getcslist"),
            ("POST", "/componenttcb/getcsversiondetail"),
            ("POST", "/componenttcb/getcsversionlist"),
            ("POST", "/componenttcb/opencontainerservice"),
            ("POST", "/componenttcb/releasecloudbaserunversion"),
            ("POST", "/componenttcb/rollupdatecloudbaserunserverversion"),
            ("POST", "/componenttcb/setcspropotion"),
            ("POST", "/componenttcb/setcsversionconfig"),
        },
        [nameof(IWechatOpenPlatformComponentTcbStaticStoreService)] = new[]
        {
            ("POST", "/componenttcb/createstaticstore"),
            ("POST", "/componenttcb/describestaticstore"),
            ("POST", "/componenttcb/staticfilelist"),
            ("POST", "/componenttcb/staticuploadfile"),
        },
    };

    /// <summary>官方请求 Payload 为 <c>{}</c>（无任何请求参数）的端点 ⇒ SDK 不得造请求体 DTO。</summary>
    private static readonly (string InterfaceName, string MethodName)[] NoRequestBodyEndpoints =
    {
        (nameof(IWechatOpenPlatformComponentTcbEnvService), "GetEnvListAsync"),
        (nameof(IWechatOpenPlatformComponentTcbEnvService), "GetCloudRunEnvListAsync"),
    };

    /// <summary>官方返回值仅 <c>errcode / errmsg</c> 的 14 条端点 ⇒ 返回基底 <see cref="OpenPlatformResponse"/>，不得造空壳应答 DTO。</summary>
    private static readonly string[] BareResponseMethods =
    {
        "BatchCreateVersionsAsync", "OpenMicroserviceAsync", "CreateServiceAsync",
        "DeleteServiceAsync", "DeleteVersionAsync", "CreateCloudRunServerAsync",
        "OpenContainerServiceAsync", "SetPropotionAsync", "SetVersionConfigAsync",
        "ManageIndexAsync", "SetDatabasePermissionAsync",
        "DeleteFunctionAsync", "UpdateFunctionConfigAsync",
        "CreateStaticStoreAsync",
    };

    /// <summary>
    /// CBT1：六接口 / 50 端点的（方法, 路由）表逐条锁定，且本域接口数与端点数与官方清单一致。
    /// </summary>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface()
    {
        ExpectedEndpoints.Should().HaveCount(6, "环境/数据库/存储/云函数/云托管/静态网站六支接口——少一支即缺口");
        ExpectedEndpoints.Values.Sum(x => x.Length).Should().Be(50,
            "官方 cloudbase-batch + cloudrun-batch + cloudbase-common 三棵树合计恰 50 条 /componenttcb/* 路由");

        foreach (var (interfaceName, expected) in ExpectedEndpoints)
        {
            var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{interfaceName}")
                ?? throw new InvalidOperationException($"接口 {interfaceName} 不存在（契约面漂移）");

            var actual = MethodsOf(type)
                .Select(static m =>
                {
                    var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
                    return (attr.HttpMethod.Method.ToUpperInvariant(), attr.RequestUri!);
                })
                .ToArray();

            actual.Should().BeEquivalentTo(expected,
                because: $"{interfaceName} 的官方路由表——差集即缺口或漂移，须先逐页复核官方正文请求地址再同批调整");
        }
    }

    /// <summary>
    /// CBT2：令牌语义 + 恒 POST + 零额外 Query + 同注册组。
    /// </summary>
    /// <remarks>
    /// 令牌<b>值</b>是 <c>component_access_token</c>，但官方 <b>query 参数名</b>恒为 <c>access_token</c>
    /// （2026-10-15 逐页核验：50 页正文的调用凭证说明全部写「可使用 component_access_token」且参数名为
    /// <c>access_token</c>）⇒ 把 <c>Name</c> 改成 <c>component_access_token</c> 会让全部 50 端点 401。
    /// </remarks>
    [Fact]
    public void TokenSemantics_ShouldBeUniform_WhenAllSixInterfacesDeclared()
    {
        foreach (var interfaceName in ExpectedEndpoints.Keys)
        {
            var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{interfaceName}")!;

            var token = type.GetCustomAttributes<TokenAttribute>(false).Single();
            token.TokenType.Should().Be(OpenPlatformTokenTypes.ComponentAccessToken,
                $"{interfaceName} 消费平台自身令牌；取授权方令牌即令牌串号");
            token.Name.Should().Be("access_token",
                $"官方 {interfaceName} 各页正文的 query 参数名为 access_token（值才是 component_access_token）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                "本域无 Header 令牌形态，官方一律 Query 传递");

            var api = type.GetCustomAttributes<HttpClientApiAttribute>(false).Single();
            api.RegistryGroupName.Should().Be("ComponentTcb",
                "六支接口共用一个注册组 ⇒ 一次 AddModules(ComponentTcb) 覆盖全域；组名漂移即注册漏挂");
        }

        var allMethods = ExpectedEndpoints.Keys
            .Select(n => MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{n}")!)
            .SelectMany(MethodsOf)
            .ToArray();

        allMethods.Should().HaveCount(50, "枚举面不得空跑");
        allMethods
            .SelectMany(m => m.GetCustomAttributes<HttpMethodAttribute>(false))
            .Should().OnlyContain(a => a.HttpMethod.Method == "POST",
                "全域 50 条路由官方请求方式恒为 POST（含查询类路由如 getcslist / getbucket）");

        foreach (var method in allMethods)
        {
            method.GetParameters()
                .Where(static p => p.GetCustomAttributes<QueryAttribute>(false).Any())
                .Should().BeEmpty(
                    $"{method.Name} 不得带 [Query]：50 页正文零额外 Query 参数，令牌之外的参数一律走 JSON 请求体");
        }
    }

    /// <summary>
    /// CBT3：目录 / 命名空间 / 注册入口三处一致（模块枚举 + builder 委托 + JSON resolver 合并登记）。
    /// </summary>
    [Fact]
    public void RegistrationSurface_ShouldBeWired_WhenModuleAdded()
    {
        Enum.GetValues<OpenPlatformModule>().Should().Contain(OpenPlatformModule.ComponentTcb,
            "模块枚举缺值 ⇒ AddModules 无法寻址本域");

        var repoRoot = FindRepoRoot();
        var builderSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformServiceBuilder.cs"));
        builderSource.Should().Contain("[OpenPlatformModule.ComponentTcb] = static s => s.AddComponentTcbWebApiHttpClient()",
            "注册入口必须委托到生成器按注册组产出的 AddComponentTcbWebApiHttpClient()（六接口同组、一次注册）");

        // 每个模块枚举值都要有委托，否则 AddAllApis() 静默漏域。
        foreach (var module in Enum.GetValues<OpenPlatformModule>())
        {
            builderSource.Should().Contain($"[OpenPlatformModule.{module}] =",
                $"模块 {module} 无注册委托 ⇒ AddAllApis() 漏注册");
        }

        var resolverSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("ComponentTcbJsonContext.Default",
            "域上下文未并进合并 resolver ⇒ 运行期退回反射序列化，AOT 下直接 IL 诊断或抛异常");
    }

    /// <summary>
    /// CBT4：本域 166 个 DTO 全量登记进 <see cref="ComponentTcbJsonContext"/>，且 <c>SerializerClassName</c> 恒为域段。
    /// </summary>
    [Fact]
    public void DataModels_ShouldBeRegisteredInComponentTcbJsonContext_WhenDomainSurfaceEnumerated()
    {
        var domainTypes = DomainDtoTypes();

        domainTypes.Should().HaveCount(166,
            "本域 DTO 数漂移须先核对官方参数表（嵌套对象按 Body/Res 路径逐层建类），再同批重跑生成脚本");

        var context = (JsonSerializerContext)typeof(ComponentTcbJsonContext)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 ComponentTcb 命名空间，必须登记进 ComponentTcbJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("ComponentTcb",
                $"{type.Name} 的 SerializerClassName 必须为域段 ComponentTcb（错段即落到别的上下文，本上下文反而漏登记）");
        }
    }

    /// <summary>
    /// CBT5：官方<b>类型分叉</b>与<b>拼写分叉</b>照抄，不得「顺手对齐」。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>类型分叉</b>：<c>cpu</c> / <c>mem</c> / <c>policy_threshold</c> 在创建类页面为 <c>number</c>
    /// （示例 <c>"cpu": 0.25</c> ⇒ 必须 <c>double?</c>，写成 <c>long?</c> 读到小数即抛），
    /// 而灰度更新页（<c>rollupdatecloudbaserunserverversion</c>）官方参数表把同名字段标成 <c>string</c>
    /// ⇒ 该请求 DTO 必须 <c>string?</c>。两侧都锁：任一侧被「统一」即偏离官方契约。
    /// </para>
    /// <para>
    /// <b>拼写分叉</b>：<c>createcloudbaserunserverversion</c> 页写 <c>image_pull_secret</c>，
    /// <c>createcsversion</c> / <c>batchcreatecsversion</c> 页写 <c>image_pulls_secret</c>（多一个 s）
    /// ⇒ 属性名照各自页面，蛇形策略回填后必须与官方键逐字一致。
    /// </para>
    /// </remarks>
    [Theory]
    [InlineData(typeof(OpenPlatformComponentTcbCreateContainerServiceVersionRequest), "Cpu", typeof(double?))]
    [InlineData(typeof(OpenPlatformComponentTcbCreateContainerServiceVersionRequest), "Mem", typeof(double?))]
    [InlineData(typeof(OpenPlatformComponentTcbCreateContainerServiceVersionRequest), "PolicyThreshold", typeof(double?))]
    [InlineData(typeof(OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest), "Cpu", typeof(double?))]
    [InlineData(typeof(OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest), "Cpu", typeof(double?))]
    [InlineData(typeof(OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestSidecarSpecsItem), "Cpu", typeof(double?))]
    [InlineData(typeof(OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse), "Cpu", typeof(double?))]
    [InlineData(typeof(OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest), "Cpu", typeof(string))]
    [InlineData(typeof(OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest), "Mem", typeof(string))]
    [InlineData(typeof(OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest), "PolicyThreshold", typeof(string))]
    public void OfficialFieldTypeFork_ShouldBePreserved(Type dtoType, string property, Type expectedType)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.PropertyType.Should().Be(expectedType,
            $"{dtoType.Name}.{property} 的类型是官方页面原文，跨页分叉属官方不一致而非本仓缺陷");
    }

    /// <summary>CBT5b：镜像拉取密钥的两套官方拼写各自落在各自页面的 DTO 上，且不得互相「统一」。</summary>
    [Fact]
    public void ImagePullSecretSpelling_ShouldFollowEachOfficialPage()
    {
        typeof(OpenPlatformComponentTcbCreateContainerServiceVersionRequest)
            .GetProperty("ImagePullsSecret").Should().NotBeNull("createcsversion 页官方键为 image_pulls_secret（带 s）");
        typeof(OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest)
            .GetProperty("ImagePullsSecret").Should().NotBeNull("batchcreatecsversion 页官方键为 image_pulls_secret（带 s）");

        typeof(OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest)
            .GetProperty("ImagePullSecret").Should().NotBeNull("createcloudbaserunserverversion 页官方键为 image_pull_secret（无 s）");
        typeof(OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest)
            .GetProperty("ImagePullsSecret").Should().BeNull(
                "把无 s 页面改成带 s ⇒ 蛇形回填键与官方不符，字段静默丢失");
    }

    /// <summary>
    /// CBT5c：库记录明文数组由 SDK <b>原样承载不解析</b>（<c>List&lt;JsonElement&gt;</c>）。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformComponentTcbAggregateDatabaseResponse))]
    [InlineData(typeof(OpenPlatformComponentTcbManageDatabaseRecordResponse))]
    public void DatabaseRecordPayload_ShouldStayUnparsed(Type responseType)
    {
        var data = responseType.GetProperty("Data")
            ?? throw new InvalidOperationException($"{responseType.Name} 缺 Data 属性");
        data.PropertyType.Name.Should().Be("List`1",
            $"{responseType.Name}.Data 必须是 List<T>——结构由客户集合 schema 决定，SDK 解析即替客户猜语义");
        Nullable.GetUnderlyingType(data.PropertyType.GetGenericArguments()[0])
            .Should().BeNull("元素实参为 JsonElement（非可空值类型），原样透传");
    }

    /// <summary>
    /// CBT6：敏感面<b>不入日志</b>（形态同小程序线 TCB6，靶名为本域官方字段对应的属性名）。
    /// </summary>
    /// <remarks>
    /// <b>覆盖面与出处</b>：镜像仓库凭证（<c>ImageSecretInfo</c> / <c>ImagePullSecret</c> / <c>ImagePullsSecret</c> /
    /// <c>Username</c> / <c>Password</c> / <c>Email</c> / <c>SecretName</c>）、云函数与容器环境变量
    /// （<c>EnvParams</c> / <c>EnvironmentVariables</c>）、小程序调用凭证透传（<c>MountWxToken</c>）、
    /// 镜像与导出/下载链接（<c>ImageUrl</c> / <c>FileUrl</c> / <c>DownloadUrl</c> / <c>SignedUrl</c>）、
    /// COS 直传凭证（<c>Authorization</c> / <c>Token</c>）、库记录明文（<c>Data</c>）。
    /// 判据是<b>真实日志调用语句</b>的三行语句窗口（消息模板常换行），注释必须排除——本域接口注释会
    /// 原样写出这些字段名的裁决理由，不排除即假红。
    /// </remarks>
    [Fact]
    public void ComponentTcbSensitiveFields_ShouldNeverBeLogged()
    {
        var sensitiveProperties = new[]
        {
            "ImageSecretInfo", "ImagePullSecret", "ImagePullsSecret", "SecretName",
            "EnvParams", "EnvironmentVariables", "MountWxToken",
            "ImageUrl", "FileUrl", "DownloadUrl", "SignedUrl",
            "Authorization", "Token", "Username", "Password", "Email",
        };

        var files = Directory.GetFiles(SourceProjectDir("Mud.Wechat.OpenPlatform"), "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToList();

        // 反空跑：扫描根必须确实含本域接口源文件，否则 offenders 恒空。
        files.Should().Contain(static f => Path.GetFileName(f).StartsWith("IWechatOpenPlatformComponentTcb", StringComparison.Ordinal),
            "CBT6 的扫描根必须覆盖 ComponentTcb 接口源文件（定位失效会让断言静默恒真）");

        var offenders = new List<string>();
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var code = StripLineComment(lines[i]);
                if (!LogInvocation.IsMatch(code))
                {
                    continue;
                }

                // 判定窗口 = 该条日志调用语句本身（消息模板常换行 ⇒ 按括号配平续行，遇闭合即止）。
                // 不得改成「固定取后 N 行」：语句结束后的 return cached.AccessToken 之类会被误读进窗口，
                // 把「沿用旧令牌」的正常出口报成敏感面泄漏（假红），从而诱导后人放宽词表。
                var statement = code;
                var depth = ParenDelta(code);
                for (var k = i; depth > 0 && k + 1 < lines.Length && k - i < 10; k++)
                {
                    var next = StripLineComment(lines[k + 1]);
                    statement += next;
                    depth += ParenDelta(next);
                }

                var hit = sensitiveProperties.FirstOrDefault(p => statement.Contains(p, StringComparison.Ordinal));
                if (hit is not null)
                {
                    offenders.Add($"{Path.GetFileName(file)}:{i + 1}({hit})");
                }
            }
        }

        offenders.Should().BeEmpty(
            "代云开发域的镜像凭证、环境变量、带签名链接、直传凭据与库记录明文不得出现在任何日志调用中（AGENTS §8）");
    }

    /// <summary>CBT7：本域 DTO <b>一律不得重写 <c>ToString</c></b>（防结构化日志整体打印敏感字段）。</summary>
    [Fact]
    public void ComponentTcbDataModels_ShouldNotOverrideToString()
    {
        var types = DomainDtoTypes();
        types.Should().HaveCount(166, "枚举不得空跑：本域类型集缺失即扫描根漂移（否则前一条断言静默恒真）");

        var offenders = types
            .Select(static t => new
            {
                t.Name,
                Override = t.GetMethod("ToString",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                    binder: null, types: Type.EmptyTypes, modifiers: null),
            })
            .Where(static x => x.Override is not null)
            .Select(static x => x.Name)
            .ToArray();

        offenders.Should().BeEmpty(
            "敏感 DTO 一旦重写 ToString，宿主把对象插值进日志即整份密钥/明文落盘；序列化展示需求由宿主脱敏后自行承担");
    }

    /// <summary>
    /// CBT8：与小程序线 <c>/tcb/*</c> <b>两线不交叉</b>（CBZ2 的本线镜像断言），且 <c>/componenttcb/</c> 前缀全仓唯一归属本线。
    /// </summary>
    /// <remarks>
    /// 小程序线把 <c>/componenttcb/*</c> 挂到小程序令牌链、或本线把 <c>/tcb/*</c> 挂到平台令牌链，
    /// 都是「令牌面串号」的静默故障：官方对同一条路由只承认一种令牌，另一侧调用恒 40001/42001。
    /// </remarks>
    [Fact]
    public void TcbAndComponentTcbPrefixes_ShouldNotCrossLines_WhenScannedAcrossAllLines()
    {
        var declared = DeclaredRoutesInSrc();

        declared.Where(x => x.Line.Contains("/OpenPlatform/", StringComparison.Ordinal))
            .Select(static x => x.Route)
            .Should().NotContain(r => r.StartsWith("/tcb/", StringComparison.Ordinal),
                "/tcb/* 的消费方令牌是小程序 access_token，属小程序线；本线声明之即把小程序令牌当平台令牌注入");

        var foreign = declared
            .Where(x => x.Route.StartsWith("/componenttcb/", StringComparison.Ordinal)
                        && !x.Line.Contains("/OpenPlatform/", StringComparison.Ordinal))
            .Select(static x => $"{x.Line}: {x.Route}")
            .ToList();

        foreign.Should().BeEmpty(
            "/componenttcb/* 仅 component_access_token 可调用，前缀必须全仓唯一归属开放平台线");

        // 反空跑：扫描必须同时命中两线的活体路由，否则扫描根或正则漂移会让上面两条静默恒真。
        declared.Count(x => x.Line.Contains("/OpenPlatform/", StringComparison.Ordinal)
                            && x.Route.StartsWith("/componenttcb/", StringComparison.Ordinal))
            .Should().Be(50, "本线恰声明 50 条 /componenttcb/*（数量漂移即本域端点或扫描口径变了）");
        declared.Should().Contain(x => x.Route.StartsWith("/tcb/", StringComparison.Ordinal),
            "CBT8 的扫描必须能捕获小程序线已声明的 /tcb/ 路由（否则不交叉断言静默恒真）");
    }

    /// <summary>
    /// CBT9：官方空请求体的 2 条端点<b>无请求 DTO</b>、官方仅回 errcode/errmsg 的 14 条端点<b>返回基底响应</b>。
    /// </summary>
    /// <remarks>
    /// 造空壳 DTO 的后果不是啰嗦而是<b>契约失真</b>：请求侧凭空多一个可传 <c>{}</c> 的重载，
    /// 应答侧多一个除基类字段外永不为空的类型，宿主据此写出永远不会执行的判空分支。
    /// </remarks>
    [Fact]
    public void OfficiallyEmptyContractSurface_ShouldStayMinimal()
    {
        foreach (var (interfaceName, methodName) in NoRequestBodyEndpoints)
        {
            var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{interfaceName}")!;
            var method = type.GetMethod(methodName)
                ?? throw new InvalidOperationException($"{interfaceName}.{methodName} 不存在");

            method.GetParameters().Should().ContainSingle(p => p.Name == "cancellationToken",
                $"{methodName} 的官方 Request Payload 为空（请求示例 {{}}）——不得人为造请求体 DTO");
            method.GetParameters().Single().GetCustomAttributes(false)
                .Should().NotContain(static a => a.GetType().Name == "BodyAttribute");
        }

        DataModelsAssembly.GetTypes()
            .Where(static t => t.Namespace == "Mud.Wechat.OpenPlatform.DataModels.ComponentTcb")
            .Select(static t => t.Name)
            .Should().NotContain("OpenPlatformComponentTcbGetTcbEnvListRequest")
            .And.NotContain("OpenPlatformComponentTcbGetWxCloudBaseRunEnvsRequest",
                "两条空请求体端点若被补上 Request 类型，即与本守卫的官方取证结论矛盾");

        var bare = ExpectedEndpoints.Keys
            .Select(n => MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{n}")!)
            .SelectMany(MethodsOf)
            .Where(m => BareResponseMethods.Contains(m.Name))
            .ToArray();

        bare.Should().HaveCount(14, "14 条「官方返回值只有 errcode/errmsg」的端点，少一条即接口面漂移");
        foreach (var method in bare)
        {
            var returnType = method.ReturnType;
            returnType.IsGenericType.Should().BeTrue($"{method.Name} 须返回 Task<>");
            returnType.GetGenericArguments()[0]
                .Should().Be(typeof(OpenPlatformResponse),
                    $"{method.Name} 的官方应答无业务字段，须直接返回基底响应（空壳 DTO 即契约失真）");
        }
    }

    /// <summary>真实日志调用语句（含扩展方法形态）的起点判据。</summary>
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
    /// 本域 DTO 类型集（<c>CloudRun/Database/Env/File/Scf/StaticStore</c> 六个子目录仅作组织，
    /// 命名空间恒为 <c>…DataModels.ComponentTcb</c>；上下文自身按 <see cref="JsonSerializerContext"/> 剔除）。
    /// </summary>
    private static Type[] DomainDtoTypes()
        => DataModelsAssembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsNested
                               && t.Namespace == "Mud.Wechat.OpenPlatform.DataModels.ComponentTcb")
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToArray();

    /// <summary>本行括号净增（字符串字面量内的括号不计，日志模板里的全/半角括号因此不误判）。</summary>
    private static int ParenDelta(string line)
    {
        var inString = false;
        var delta = 0;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                inString = !inString;
                continue;
            }

            if (inString)
            {
                continue;
            }

            delta += c == '(' ? 1 : c == ')' ? -1 : 0;
        }

        return delta;
    }

    private static IEnumerable<MethodInfo> MethodsOf(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == interfaceType
                && m.GetCustomAttributes<HttpMethodAttribute>(false).Any());

    /// <summary>全 <c>Src/</c> 源码里的路由字面量 + 其所属产品线目录（CBT8 的扫描面）。</summary>
    private static List<(string Line, string Route)> DeclaredRoutesInSrc()
    {
        var srcRoot = Path.Combine(FindRepoRoot(), "Src");
        var routeRegex = new Regex(@"""(/[A-Za-z0-9_\-./{}]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var list = new List<(string, string)>();

        foreach (var file in Directory.EnumerateFiles(srcRoot, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal))
            {
                continue;
            }

            var srcIndex = normalized.IndexOf("/Src/", StringComparison.Ordinal);
            if (srcIndex < 0)
            {
                continue;
            }

            // Line 形如 /OpenPlatform/Mud.Wechat.OpenPlatform/Interfaces/…（产品线目录即首段，用于归属判定）。
            var line = "/" + normalized[(srcIndex + "/Src/".Length)..];

            foreach (System.Text.RegularExpressions.Match match in routeRegex.Matches(File.ReadAllText(file)))
            {
                var route = match.Groups[1].Value;
                if (route.StartsWith("/tcb/", StringComparison.Ordinal)
                    || route.StartsWith("/componenttcb/", StringComparison.Ordinal))
                {
                    list.Add((line, route));
                }
            }
        }

        return list;
    }

    private static string SourceProjectDir(string projectName)
    {
        var root = FindRepoRoot();
        return Directory.EnumerateFiles(root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f))!
            .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {projectName}.csproj（源码归类目录漂移，守卫定位失效）");
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName
            ?? throw new InvalidOperationException("未找到仓库根（缺 Mud.Wechat.slnx）——请在检出树内运行测试。");
    }
}
