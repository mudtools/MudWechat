// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceBatch;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-C 平台商户保险理赔商家转账「商家转账批次与明细（保险场景）」域契约守卫 —— <b>5 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>五支建四型</b>：批次两支查询（按 <c>out_batch_no</c> / 按 <c>batch_id</c>）应答<b>逐字段同形</b>（24 行， 含 <c>transfer_batch</c> 对象与 <c>data</c> 明细数组 + <c>offset</c> / <c>limit</c>）⇒ 共用 <see cref="InsuranceBatchQueryResponse"/>； 明细两支查询同形（17 行）⇒ 共用 <see cref="InsuranceBatchDetailQueryResponse"/>；受理支单建一型。</para>
/// <para><b>受理成功 ≠ 转账成功（官方原文）</b>：受理应答只回批次号与 <c>batch_state</c>，须走查询支取付款状态； 幂等标识 <c>out_batch_no</c>、有效性 5 年 ⇒ 不得把受理应答的 <c>batch_state</c> 当终态判定。</para>
/// <para><b>分页 query 只印在批次查询两支</b>：<c>need_query_detail</c>（必填）/ <c>offset</c> / <c>limit</c> / <c>detail_state</c>； 明细两支<b>零分页</b>且仅 <c>sub_mchid</c> 必填 query ⇒ 不得把批次查询的参数表套到明细查询上。</para>
/// <para><b>与 <see cref="IWechatPayTransferService"/> / <see cref="IWechatPayTransferToQqWalletService"/> 不同契约</b>： 本域是<b>【平台商户】保险场景</b>的 <c>/v3/platsolution/insurance/mch-transfer/*</c>，应答多 <c>sp_mchid</c> / <c>sp_appid</c> / <c>sub_appid</c> 三层标识、字段名用 <c>batch_id</c>（不是 <c>transfer_bill_no</c>）⇒ DTO 不得与商家转账各域互相套用。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayPlatsolutionInsuranceBatchContractGuards
{
    private const string RegistryGroupName = "PlatsolutionInsuranceBatch";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceBatch";

    private static readonly Type Service = typeof(IWechatPayPlatsolutionInsuranceBatchService);

    /// <summary>PSI-BT-B1：拓扑 —— 一个注册组一个接口、5 方法、5 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/platsolution/insurance/mch-transfer/batches。</summary>
    [Fact]
    public void PlatsolutionInsuranceBatchTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(5, "商家转账批次与明细（保险场景）声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(5, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/platsolution/insurance/mch-transfer/batches", StringComparison.Ordinal),
            "本域路由全在 /v3/platsolution/insurance/mch-transfer/batches 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>PSI-BT-B2：5 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void PlatsolutionInsuranceBatchEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 受理商家转账｜官方页 partner/4013504220｜身份 【平台商户】｜官方页更新 2025.01.21
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.ApplyInsuranceBatchAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/batches/apply");
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.ApplyInsuranceBatchAsync)).Should().BeEmpty();

        // 商户转账批次单号查询批次单｜官方页 partner/4013504222｜身份 【平台商户】｜官方页更新 2025.03.11
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByOutBatchNoAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}");
        PathKeysOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByOutBatchNoAsync)).Should().BeEquivalentTo(new[] { "outBatchNo" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByOutBatchNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "need_query_detail", "offset", "limit", "detail_state" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByOutBatchNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "need_query_detail" }, "商户转账批次单号查询批次单的 sub_mchid / need_query_detail 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByOutBatchNoAsync)).Should().BeNull("商户转账批次单号查询批次单官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 微信支付转账批次单号查询批次单｜官方页 partner/4013504221｜身份 【平台商户】｜官方页更新 2025.03.11
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByBatchIdAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}");
        PathKeysOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByBatchIdAsync)).Should().BeEquivalentTo(new[] { "batchId" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByBatchIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "need_query_detail", "offset", "limit", "detail_state" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByBatchIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "need_query_detail" }, "微信支付转账批次单号查询批次单的 sub_mchid / need_query_detail 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchByBatchIdAsync)).Should().BeNull("微信支付转账批次单号查询批次单官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 商户明细单号查询明细单｜官方页 partner/4013504224｜身份 【平台商户】｜官方页更新 2025.03.11
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByOutDetailNoAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}");
        PathKeysOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByOutDetailNoAsync)).Should().BeEquivalentTo(new[] { "outBatchNo", "outDetailNo" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByOutDetailNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByOutDetailNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "商户明细单号查询明细单的 sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByOutDetailNoAsync)).Should().BeNull("商户明细单号查询明细单官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 微信支付转账明细单号查询明细单｜官方页 partner/4013504223｜身份 【平台商户】｜官方页更新 2025.03.11
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByDetailIdAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}");
        PathKeysOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByDetailIdAsync)).Should().BeEquivalentTo(new[] { "batchId", "detailId" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByDetailIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByDetailIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "微信支付转账明细单号查询明细单的 sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPlatsolutionInsuranceBatchService.QueryInsuranceBatchDetailByDetailIdAsync)).Should().BeNull("微信支付转账明细单号查询明细单官方无请求体 ⇒ 不得凭空造请求 DTO");
    }

    /// <summary>PSI-BT-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void PlatsolutionInsuranceBatchDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<InsuranceBatchApplyRequest>().Should().BeEquivalentTo(new[] { "sub_mchid", "sp_appid", "sub_appid", "out_batch_no", "batch_name", "batch_remark", "total_amount", "total_num", "transfer_scene_id", "transfer_detail_list", "notify_url" },
            "受理商家转账请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<InsuranceBatchApplyResponse>().Should().BeEquivalentTo(new[] { "out_batch_no", "batch_id", "create_time", "batch_state" },
            "受理商家转账应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<InsuranceBatchQueryResponse>().Should().BeEquivalentTo(new[] { "transfer_batch", "data", "offset", "limit" },
            "商户转账批次单号查询批次单应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<InsuranceBatchQueryResponse>().Should().BeEquivalentTo(new[] { "transfer_batch", "data", "offset", "limit" },
            "微信支付转账批次单号查询批次单应答顶层字段名册（官方应答表逐字，与 <c>batches/out-batch-no/{out_batch_no}</c> 同表 ⇒ 共用一个类型）");

        DeclaredJsonNamesOf<InsuranceBatchDetailQueryResponse>().Should().BeEquivalentTo(new[] { "out_batch_no", "batch_id", "out_detail_no", "detail_id", "sp_appid", "sub_mchid", "sub_appid", "create_time", "detail_state", "transfer_amount", "transfer_remark", "fail_reason", "openid", "bank_type", "user_name", "transfer_finish_time", "bank_refund_time" },
            "商户明细单号查询明细单应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<InsuranceBatchDetailQueryResponse>().Should().BeEquivalentTo(new[] { "out_batch_no", "batch_id", "out_detail_no", "detail_id", "sp_appid", "sub_mchid", "sub_appid", "create_time", "detail_state", "transfer_amount", "transfer_remark", "fail_reason", "openid", "bank_type", "user_name", "transfer_finish_time", "bank_refund_time" },
            "微信支付转账明细单号查询明细单应答顶层字段名册（官方应答表逐字，与 <c>batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}</c> 同表 ⇒ 共用一个类型）");
    }

    /// <summary>PSI-BT-B4：判错面 + AOT 上下文登记 + DI 与模块接线（1 请求根型 / 3 应答根型 / 7 个 DTO）。</summary>
    [Fact]
    public void PlatsolutionInsuranceBatchDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(7, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            PlatsolutionInsuranceBatchJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PlatsolutionInsuranceBatchJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 PlatsolutionInsuranceBatch");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(InsuranceBatchApplyResponse)).Should().BeTrue("InsuranceBatchApplyResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(InsuranceBatchQueryResponse)).Should().BeTrue("InsuranceBatchQueryResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(InsuranceBatchDetailQueryResponse)).Should().BeTrue("InsuranceBatchDetailQueryResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(InsuranceBatchApplyRequest)).Should().BeFalse("InsuranceBatchApplyRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PlatsolutionInsuranceBatchJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 PlatsolutionInsuranceBatch 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPlatsolutionInsuranceBatchWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 PlatsolutionInsuranceBatch 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayPlatsolutionInsuranceBatchService.{methodName} 必须存在");

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

    private static Type? BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Select(p => (p: p, attr: p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "BodyAttribute")))
            .Where(x => x.attr != null)
            .Select(x => x.p.ParameterType)
            .FirstOrDefault();

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
