// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// P3-b 扫二维码打开小程序域契约守卫（<c>/cgi-bin/wxopen/qrcodejump*</c> 4 端点，服务号专属）。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07，4 页全服务号域命中）。关键核验：
/// ① 4 页均 5 次/秒（错误码 44990 原文）；② 发布配额每月 100 次（886000）；
/// ③ 官方 <c>qrcodejumpadd</c> 页<b>无「校验文件托管」正文</b>（仅错误码 85069，属普通二维码场景）
/// ⇒ v2 方案原稿的「宿主须上传校验文件」为误植，SDK 不写入该断言；
/// ④ 3 个只回 <c>errcode</c>/<c>errmsg</c> 的端点复用 <see cref="MpResponse"/>（不造空壳 DTO）。
/// </remarks>
public class MpQrcodeJumpContractGuards
{
    private const string RegistryGroupName = "QrcodeJump";

    /// <summary>官方路由表（4 端点，全 POST）。</summary>
    private static readonly (string Method, string Route)[] Routes =
    {
        (nameof(IMpQrcodeJumpService.GetQrcodeJumpRulesAsync), "/cgi-bin/wxopen/qrcodejumpget"),
        (nameof(IMpQrcodeJumpService.AddOrUpdateQrcodeJumpRuleAsync), "/cgi-bin/wxopen/qrcodejumpadd"),
        (nameof(IMpQrcodeJumpService.PublishQrcodeJumpRuleAsync), "/cgi-bin/wxopen/qrcodejumppublish"),
        (nameof(IMpQrcodeJumpService.DeleteQrcodeJumpRuleAsync), "/cgi-bin/wxopen/qrcodejumpdelete"),
    };

    /// <summary>契约守卫 QJ1：路由表与官方契约一致（4 端点全 POST、前缀 /cgi-bin/wxopen/）。</summary>
    [Fact]
    public void QrcodeJumpEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Select(r => r.Route).Distinct().Should().HaveCount(Routes.Length, "各端点路由互不重复");

        foreach (var (method, route) in Routes)
        {
            route.Should().StartWith("/cgi-bin/wxopen/", "本域四端点同前缀（官方路径安排）");

            var target = FindMethod(method);
            var attr = target.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");
        }

        typeof(IMpQrcodeJumpService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
            .Should().AllBeAssignableTo<PostAttribute>("4 端点官方全部为 POST");
    }

    /// <summary>契约守卫 QJ2：请求 / 响应 DTO 的官方字段名锁定。</summary>
    [Fact]
    public void QrcodeJumpShapes_ShouldLockOfficialFieldNames()
    {
        // 查询请求：服务号场景参数（官方字段表标必填、正文称仅服务号场景需要 ⇒ 可空超集）。
        AssertJsonProperty<MpQrcodeJumpGetRequest>("appid", "小程序的 appid（仅服务号场景需传）");
        AssertJsonProperty<MpQrcodeJumpGetRequest>("get_type", "查询类型（0 最近 10000 条 / 1 prefix / 2 分页）");
        AssertJsonProperty<MpQrcodeJumpGetRequest>("prefix_list", "prefix 查询（最多 200 个前缀）");
        AssertJsonProperty<MpQrcodeJumpGetRequest>("page_num", "页码（从 1 开始）");
        AssertJsonProperty<MpQrcodeJumpGetRequest>("page_size", "每页数量（最大 200）");

        // 查询响应：规则列表 + 本月剩余发布配额 + 总量。
        AssertJsonProperty<MpQrcodeJumpGetResponse>("rule_list", "二维码规则详情列表");
        AssertJsonProperty<MpQrcodeJumpGetResponse>("qrcodejump_open", "是否已打开二维码跳转链接设置");
        AssertJsonProperty<MpQrcodeJumpGetResponse>("list_size", "二维码规则数量");
        AssertJsonProperty<MpQrcodeJumpGetResponse>("qrcodejump_pub_quota", "本月还可发布的次数");
        AssertJsonProperty<MpQrcodeJumpGetResponse>("total_count", "二维码规则总数据量");

        // 规则条目：open_version / debug_url 仅普通二维码场景返回 ⇒ 可空。
        AssertJsonProperty<MpQrcodeJumpRule>("prefix", "二维码规则");
        AssertJsonProperty<MpQrcodeJumpRule>("path", "小程序功能页面");
        AssertJsonProperty<MpQrcodeJumpRule>("state", "发布标志位（1 未发布 / 2 已发布）");
        AssertJsonProperty<MpQrcodeJumpRule>("open_version", "测试范围（仅普通二维码场景返回）");
        AssertJsonProperty<MpQrcodeJumpRule>("debug_url", "测试链接，最多 5 个（仅普通二维码场景返回）");

        // 新增 / 修改请求：两场景参数合并（不许改名成驼峰或缩写）。
        AssertJsonProperty<MpQrcodeJumpAddRequest>("prefix", "二维码规则（必填）");
        AssertJsonProperty<MpQrcodeJumpAddRequest>("appid", "跳转的小程序 appid（仅服务号场景）");
        AssertJsonProperty<MpQrcodeJumpAddRequest>("path", "小程序功能页面（必填）");
        AssertJsonProperty<MpQrcodeJumpAddRequest>("is_edit", "编辑标志位（0 新增 / 1 修改；已发布规则不支持修改）");
        AssertJsonProperty<MpQrcodeJumpAddRequest>("open_version", "测试范围（仅普通二维码场景）");
        AssertJsonProperty<MpQrcodeJumpAddRequest>("debug_url", "测试链接（仅普通二维码场景）");
        AssertJsonProperty<MpQrcodeJumpAddRequest>("permit_sub_rule", "是否独占子规则（1 不占用 / 2 占用）");

        // 发布 / 删除请求。
        AssertJsonProperty<MpQrcodeJumpPublishRequest>("prefix", "二维码规则（服务号场景为带参二维码 url）");
        AssertJsonProperty<MpQrcodeJumpDeleteRequest>("prefix", "二维码规则（必填）");
        AssertJsonProperty<MpQrcodeJumpDeleteRequest>("appid", "小程序 appid（删除服务号规则时需传）");

        // 官方示例出现但字段表未收录的 permit_sub_rule 不得进「查询响应」（示例字段无字段表形态说明）。
        JsonNamesOf<MpQrcodeJumpGetResponse>().Should().NotContain("permit_sub_rule",
            "官方 qrcodejumpget 页 permit_sub_rule 仅见示例、字段表未收录 ⇒ 不建模");
    }

    /// <summary>契约守卫 QJ3：三端点「仅 errcode/errmsg」响应复用 <see cref="MpResponse"/>（防空壳 DTO 增生）。</summary>
    [Fact]
    public void QrcodeJumpVoidResponses_ShouldReuseSharedMpResponse()
    {
        foreach (var method in new[]
                 {
                     nameof(IMpQrcodeJumpService.AddOrUpdateQrcodeJumpRuleAsync),
                     nameof(IMpQrcodeJumpService.PublishQrcodeJumpRuleAsync),
                     nameof(IMpQrcodeJumpService.DeleteQrcodeJumpRuleAsync),
                 })
        {
            TaskResultType(method).Should().Be(typeof(MpResponse),
                $"{method} 官方响应仅 errcode/errmsg ⇒ 复用 MpResponse，不新建空壳 DTO");
        }

        TaskResultType(nameof(IMpQrcodeJumpService.GetQrcodeJumpRulesAsync))
            .Should().Be(typeof(MpQrcodeJumpGetResponse));
    }

    /// <summary>契约守卫 QJ4：DTO 全量登记进上下文 + 模块枚举 / 注册入口 / Query 令牌白名单。</summary>
    [Fact]
    public void QrcodeJumpModuleAndDataModels_ShouldBeRegistered()
    {
        const string ns = "Mud.Wechat.OfficialAccount.DataModels.QrcodeJump";
        var domainTypes = typeof(MpQrcodeJumpGetRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == ns
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(6,
            "请求 4（get/add/publish/delete）+ 响应 1（get）+ 条目 1（rule）");
        foreach (var type in domainTypes)
        {
            QrcodeJumpJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 QrcodeJumpJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        Enum.GetNames(typeof(MpModule)).Should().Contain("QrcodeJump");
        typeof(MpServiceBuilder).GetMethod("AddQrcodeJumpApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();
        queryInterfaces.Should().Contain(nameof(IMpQrcodeJumpService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");

        // 与带参二维码域是两族能力，不得合并（域边界按功能族）。
        typeof(IMpQrcodeJumpService).Should().NotBe(typeof(IMpQrcodeService));
    }

    /// <summary>契约守卫 QJ5：枚举常量取值与已核验错误码锁定。</summary>
    [Fact]
    public void QrcodeJumpConstants_ShouldMatchVerifiedOfficialValues()
    {
        MpQrcodeJumpGetTypes.Recent.Should().Be(0);
        MpQrcodeJumpGetTypes.Prefix.Should().Be(1);
        MpQrcodeJumpGetTypes.Paged.Should().Be(2);

        MpQrcodeJumpStates.Unpublished.Should().Be(1);
        MpQrcodeJumpStates.Published.Should().Be(2);

        MpQrcodeJumpEditFlags.Add.Should().Be(0);
        MpQrcodeJumpEditFlags.Edit.Should().Be(1);

        MpQrcodeJumpOpenVersions.Develop.Should().Be(1);
        MpQrcodeJumpOpenVersions.Trial.Should().Be(2);
        MpQrcodeJumpOpenVersions.Release.Should().Be(3);

        MpQrcodeJumpSubRuleModes.NotOccupied.Should().Be(1, "官方原文「1 为不占用，2 为占用」");
        MpQrcodeJumpSubRuleModes.Occupied.Should().Be(2);

        MpErrorCodes.QrcodeJumpRequestTooFast.Should().Be(44990, "官方 5 次/秒的唯一数值出口");
        MpErrorCodes.QrcodeJumpInvalidAppId.Should().Be(40166);
        MpErrorCodes.QrcodeJumpLinkError.Should().Be(85066);
        MpErrorCodes.QrcodeJumpTestUrlNotSubPrefix.Should().Be(85068);
        MpErrorCodes.QrcodeJumpConfirmFileFailed.Should().Be(85069);
        MpErrorCodes.QrcodeJumpUrlBlacklisted.Should().Be(85070);
        MpErrorCodes.QrcodeJumpLinkDuplicated.Should().Be(85071);
        MpErrorCodes.QrcodeJumpLinkOccupied.Should().Be(85072);
        MpErrorCodes.QrcodeJumpRuleQuotaFull.Should().Be(85073);
        MpErrorCodes.QrcodeJumpMiniProgramNotPublished.Should().Be(85074);
        MpErrorCodes.QrcodeJumpPersonalMiniProgramDenied.Should().Be(85075);
        MpErrorCodes.QrcodeJumpIcpCheckFailed.Should().Be(85076);
        MpErrorCodes.QrcodeJumpDataAbnormal.Should().Be(85095);
        MpErrorCodes.QrcodeJumpPublishQuotaExceeded.Should().Be(886000, "官方原文「本月发布次数达到上限（100 次）」");
        MpErrorCodes.QrcodeJumpSystemBusy.Should().Be(886001);
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IMpQrcodeJumpService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IMpQrcodeJumpService.{methodName} 必须存在");

    /// <summary>取 <c>Task&lt;T&gt;</c> 的 <c>T</c>（接口方法恒返回 Task，比较 ReturnType 会拿到 Task 本身）。</summary>
    private static Type TaskResultType(string methodName)
        => FindMethod(methodName).ReturnType.GetGenericArguments()[0];

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
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
