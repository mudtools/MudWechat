// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.OneCode;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// P4 第二域「微信『一物一码』」契约守卫（<c>/intp/marketcode/*</c> 6 端点）。
/// </summary>
/// <remarks>
/// <para>
/// 官方事实来源（逐页核验 2026-10-07，6 页全服务号域命中）。关键核验：
/// ① 账号门槛为「<b>服务号（需申请）</b>」（非「仅认证」）；② 无新票据体系、与支付不耦合；
/// ③ 6 页全部支持第三方代调用（权限集 <b>46</b>）；④ 无频率数值；
/// ⑤ <b>响应为平级字段</b>（无 <c>data</c> 包裹）；⑥ <b>错误码面极薄</b>（仅通用 <c>40001</c>）⇒ 本域零新增错误码。
/// </para>
/// <para>
/// <b>守卫锁定的三处形态裁决（勿「顺手修正」）</b>：
/// <c>code</c> 取 <c>string</c>（前导零）；<c>applycodequery</c> 的 <c>application_id</c> 请求侧取数值（请求示例不构成口径依据）；
/// <c>codeactivequery</c> 与 <c>tickettocode</c> 共用 <see cref="MpCodeInfoResponse"/> 超集。
/// </para>
/// </remarks>
public class MpOneCodeContractGuards
{
    private const string RegistryGroupName = "OneCode";

    /// <summary>官方路由表（6 端点，全 POST，同前缀）。</summary>
    private static readonly (string Route, string Method)[] Routes =
    {
        ("/intp/marketcode/applycode", nameof(IMpOneCodeService.ApplyCodeAsync)),
        ("/intp/marketcode/applycodequery", nameof(IMpOneCodeService.QueryCodeApplicationAsync)),
        ("/intp/marketcode/applycodedownload", nameof(IMpOneCodeService.DownloadCodePackageAsync)),
        ("/intp/marketcode/codeactive", nameof(IMpOneCodeService.ActivateCodeAsync)),
        ("/intp/marketcode/codeactivequery", nameof(IMpOneCodeService.QueryCodeActivationAsync)),
        ("/intp/marketcode/tickettocode", nameof(IMpOneCodeService.ExchangeCodeTicketAsync)),
    };

    /// <summary>契约守卫 OT1：6 端点路由与方法一致（全 POST、同前缀 /intp/marketcode/）。</summary>
    [Fact]
    public void OneCodeEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Select(r => r.Route).Distinct().Should().HaveCount(Routes.Length, "各端点路由互不重复");
        Routes.Should().OnlyContain(r => r.Route.StartsWith("/intp/marketcode/", StringComparison.Ordinal),
            "本域 6 端点同前缀（官方路径安排）");

        foreach (var (route, method) in Routes)
        {
            var target = FindMethod(method);
            var attr = target.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");
            HasBody(method).Should().BeTrue($"{method} 官方为 POST + 请求体");
        }

        typeof(IMpOneCodeService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
            .Should().AllBeAssignableTo<PostAttribute>("6 端点官方全部为 POST（本域无 GET）");
    }

    /// <summary>契约守卫 OT2：响应形态（平级 / 复用 / 共用）与请求参数位置锁定。</summary>
    [Fact]
    public void OneCodeResponseShapes_ShouldKeepAdjudicatedForms()
    {
        // 仅 codeactive 响应为「纯 errcode/errmsg」⇒ 复用 MpResponse（防空壳 DTO）。
        TaskResultType(nameof(IMpOneCodeService.ActivateCodeAsync)).Should().Be(typeof(MpResponse),
            "codeactive 官方响应仅 errcode/errmsg ⇒ 复用 MpResponse");

        // codeactivequery 与 tickettocode 语义相同、字段集仅差 wxa_type（疑官方漏列）⇒ 共用超集 DTO。
        TaskResultType(nameof(IMpOneCodeService.QueryCodeActivationAsync))
            .Should().Be(typeof(MpCodeInfoResponse));
        TaskResultType(nameof(IMpOneCodeService.ExchangeCodeTicketAsync))
            .Should().Be(typeof(MpCodeInfoResponse), "两页语义相同 ⇒ 共用（差异照录于 DTO remarks）");
        typeof(MpCodeInfoResponse).GetProperty(nameof(MpCodeInfoResponse.WxaType)).Should().NotBeNull(
            "tickettocode 页字段表未列 wxa_type（疑漏），共用超集保留该字段");

        // 响应为平级字段：不得出现 data 包裹层（与门店域形态不同）。
        foreach (var type in new[]
                 {
                     typeof(MpApplyCodeResponse), typeof(MpCodeApplyQueryResponse),
                     typeof(MpCodeDownloadResponse), typeof(MpCodeInfoResponse),
                 })
        {
            JsonNamesOf(type).Should().NotContain("data",
                $"{type.Name} 本域响应为平级字段，官方无 data 包裹层");
        }
    }

    /// <summary>契约守卫 OT3：DTO 官方字段名锁定（逐类型）。</summary>
    [Fact]
    public void OneCodeShapes_ShouldLockOfficialFieldNames()
    {
        AssertJsonProperty<MpApplyCodeRequest>("code_count", "10000 的整数倍且 ∈ [10000, 20000000]");
        AssertJsonProperty<MpApplyCodeRequest>("isv_application_id", "幂等键：相同值视为同一申请单");
        AssertJsonProperty<MpApplyCodeResponse>("application_id", "申请单号（平级，非 data 包裹）");

        AssertJsonProperty<MpApplyCodeQueryRequest>("application_id", "与 isv_application_id 二选一");
        AssertJsonProperty<MpApplyCodeQueryRequest>("isv_application_id", "外部单号");
        AssertJsonProperty<MpCodeApplyQueryResponse>("status", "申请单状态（官方未给枚举表）");
        AssertJsonProperty<MpCodeApplyQueryResponse>("code_generate_list", "二维码信息数组");
        AssertJsonProperty<MpCodeApplyQueryResponse>("create_time", "创建时间（官方未说明单位）");
        AssertJsonProperty<MpCodeApplyQueryResponse>("update_time", "更新时间（官方未说明单位）");
        AssertJsonProperty<MpCodeRange>("code_start", "开始位置");
        AssertJsonProperty<MpCodeRange>("code_end", "结束位置");

        AssertJsonProperty<MpCodeDownloadRequest>("application_id", "申请单号");
        AssertJsonProperty<MpCodeDownloadRequest>("code_start", "开始位置（来自 applycodequery）");
        AssertJsonProperty<MpCodeDownloadRequest>("code_end", "结束位置（来自 applycodequery）");
        AssertJsonProperty<MpCodeDownloadResponse>("buffer", "base64 二维码包（SDK 不代 decode / 解密）");

        AssertJsonProperty<MpCodeActiveRequest>("activity_name", "数据分析活动区分依据");
        AssertJsonProperty<MpCodeActiveRequest>("product_brand", "数据分析品牌区分依据");
        AssertJsonProperty<MpCodeActiveRequest>("product_title", "数据分析商品区分依据");
        AssertJsonProperty<MpCodeActiveRequest>("product_code", "EAN 商品条码");
        AssertJsonProperty<MpCodeActiveRequest>("wxa_appid", "扫码跳转小程序 appid");
        AssertJsonProperty<MpCodeActiveRequest>("wxa_path", "扫码跳转小程序 path");
        AssertJsonProperty<MpCodeActiveRequest>("wxa_type", "可选：0 正式版 / 1 开发版 / 2 体验版");
        AssertJsonProperty<MpCodeActiveRequest>("code_start", "激活码段起始位（闭区间）");
        AssertJsonProperty<MpCodeActiveRequest>("code_end", "激活码段结束位（闭区间）");

        AssertJsonProperty<MpCodeActiveQueryRequest>("application_id", "查询路径一（配合 code_index）");
        AssertJsonProperty<MpCodeActiveQueryRequest>("code_index", "该码在批次中的偏移量");
        AssertJsonProperty<MpCodeActiveQueryRequest>("code_url", "28 位普通码（与 code 二选一）");
        AssertJsonProperty<MpCodeActiveQueryRequest>("code", "九位字符串原始码（与 code_url 二选一）");

        AssertJsonProperty<MpTicketToCodeRequest>("openid", "用户 openid");
        AssertJsonProperty<MpTicketToCodeRequest>("code_ticket", "临时票据（换取正式营销码）");

        AssertJsonProperty<MpCodeInfoResponse>("code", "原始码");
        AssertJsonProperty<MpCodeInfoResponse>("product_code", "示例独有（两页字段表均未收录）");
        AssertJsonProperty<MpCodeInfoResponse>("wxa_type", "小程序版本");
        AssertJsonProperty<MpCodeInfoResponse>("code_start", "激活码段起始位");
        AssertJsonProperty<MpCodeInfoResponse>("code_end", "激活码段结束位");
    }

    /// <summary>契约守卫 OT4：官方文档冲突处置的**负向**锁定（防「顺手修正」）。</summary>
    [Fact]
    public void OneCodeDocumentConflicts_ShouldKeepAdjudicatedShapes()
    {
        // code 为「九位字符串原始码」：请求表标 number 为误标（描述 + 返回表 + 前导零三重证据 ⇒ string）。
        typeof(MpCodeActiveQueryRequest).GetProperty(nameof(MpCodeActiveQueryRequest.Code))!.PropertyType
            .Should().Be(typeof(string), "官方描述为「九位的字符串原始码」且返回表标 string ⇒ 取 string（前导零不可丢）");
        typeof(MpCodeInfoResponse).GetProperty(nameof(MpCodeInfoResponse.Code))!.PropertyType
            .Should().Be(typeof(string));

        // 请求示例的类型不构成口径依据：applycodequery 的 application_id 取字段表（数值）。
        typeof(MpApplyCodeQueryRequest).GetProperty(nameof(MpApplyCodeQueryRequest.ApplicationId))!.PropertyType
            .Should().Be(typeof(long?), "官方请求示例写作字符串，但「取示例」仅适用于返回示例 ⇒ 请求侧取字段表（数值）");

        // 下载响应按示例建模为 JSON 字符串（官方类型列写「formdata 文件 buffer」，非合法 JSON 类型，无法建模）。
        typeof(MpCodeDownloadResponse).GetProperty(nameof(MpCodeDownloadResponse.Buffer))!.PropertyType
            .Should().Be(typeof(string), "官方类型列非 JSON 类型 ⇒ 取示例（JSON 包裹的 buffer 字符串）");

        // 二选一参数全部可空（官方把两者都标必填，与注意事项的「或」矛盾 ⇒ 可空承载，不本地校验）。
        // 注：值类型可空（long?/int?）可运行期反射验证；引用类型（IsvApplicationId/CodeUrl/Code）的
        // 可空标注不可运行期反射，其「可空」由字段名断言（OT3）+ 序列化测试（未设置则不输出）承担。
        typeof(MpApplyCodeQueryRequest).GetProperty(nameof(MpApplyCodeQueryRequest.ApplicationId))!.PropertyType
            .Should().Be(typeof(long?));
        typeof(MpCodeActiveQueryRequest).GetProperty(nameof(MpCodeActiveQueryRequest.CodeIndex))!.PropertyType
            .Should().Be(typeof(int?));
        typeof(MpCodeActiveQueryRequest).GetProperty(nameof(MpCodeActiveQueryRequest.ApplicationId))!.PropertyType
            .Should().Be(typeof(long?));
    }

    /// <summary>契约守卫 OT5：DTO 全量登记进上下文 + 模块枚举 / 注册入口 / Query 令牌白名单。</summary>
    [Fact]
    public void OneCodeModuleAndDataModels_ShouldBeRegistered()
    {
        const string ns = "Mud.Wechat.OfficialAccount.DataModels.OneCode";
        var domainTypes = typeof(MpApplyCodeRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == ns
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(11,
            "申请 2 + 查询申请单 3 + 下载 2 + 激活 1 + 查询请求 1 + 共用响应 1 + 换码请求 1");
        foreach (var type in domainTypes)
        {
            OneCodeJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 OneCodeJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        Enum.GetNames(typeof(MpModule)).Should().Contain("OneCode");
        typeof(MpServiceBuilder).GetMethod("AddOneCodeApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();
        queryInterfaces.Should().Contain(nameof(IMpOneCodeService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");

        var token = typeof(IMpOneCodeService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull();
        token!.Name.Should().Be("access_token");
        token.TokenType.Should().Be(MpTokenTypes.AccessToken);
    }

    /// <summary>契约守卫 OT6：枚举常量与「本域零新增错误码」的锁定。</summary>
    [Fact]
    public void OneCodeConstants_ShouldMatchVerifiedOfficialValues()
    {
        MpCodeWxaTypes.Release.Should().Be(0, "官方原文「默认为 0 正式版」");
        MpCodeWxaTypes.Develop.Should().Be(1);
        MpCodeWxaTypes.Trial.Should().Be(2);

        MpCodeApplyStatuses.Finished.Should().Be("FINISH",
            "官方未给状态枚举表，SDK 只登记返回示例值（不得据此推断完整状态机）");

        // 本域错误码面极薄：6 页仅列通用 40001（applycode 页误挂的 40002 不建模）⇒ 零新增常量，复用既存。
        MpErrorCodes.InvalidCredential.Should().Be(40001);
        MpErrorCodes.InvalidGrantType.Should().Be(40002, "既存常量；本域不为其新增语义（误挂码不建模）");

        // 该域不得出现以 OneCode 前缀命名的专有错误码（防「为薄错误码面硬造常量」）。
        typeof(MpErrorCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.Name)
            .Should().NotContain(n => n.StartsWith("OneCode", StringComparison.Ordinal),
                "官方 6 页无专有错误码 ⇒ 本域零新增错误码常量（仅复用通用码）");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IMpOneCodeService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IMpOneCodeService.{methodName} 必须存在");

    private static Type TaskResultType(string methodName)
        => FindMethod(methodName).ReturnType.GetGenericArguments()[0];

    private static bool HasBody(string methodName)
        => FindMethod(methodName).GetParameters().Any(p => p.GetCustomAttribute<BodyAttribute>() != null);

    private static List<string> JsonNamesOf(Type type)
        => type.GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
