// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Tag;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 用户管理·标签管理域契约守卫：路由表、注册形态、令牌绑定、DTO 字段与 JSON 上下文登记锁定。
/// </summary>
/// <remarks>
/// <para>
/// 官方事实来源：服务端 API 索引 → 用户管理 → 标签管理，共 <b>8</b> 个端点；
/// 各端点的请求方式与请求路径逐页核验（官方索引页原文）。
/// </para>
/// <para>
/// 域级约束（守卫不表达到频率上限——官方本域各页均<b>未声明</b>独立频次，不得编造数值）：
/// 全部端点适用范围均为「公众号 / 服务号 —— <b>仅认证</b>」；标签上限 100 个（<c>45056</c>）；
/// 单用户标签上限 20 个（<c>45059</c>）；标签名 ≤ 30 字符（<c>45158</c>）。
/// </para>
/// </remarks>
public class MpTagContractGuards
{
    private const string TagRegistryGroupName = "Tag";

    /// <summary>标签管理域官方路由表（8 端点：请求方式与路径均逐页核验）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] TagRoutes =
    {
        (typeof(IMpTagService), nameof(IMpTagService.CreateTagAsync), typeof(PostAttribute), "/cgi-bin/tags/create"),
        (typeof(IMpTagService), nameof(IMpTagService.GetTagsAsync), typeof(GetAttribute), "/cgi-bin/tags/get"),
        (typeof(IMpTagService), nameof(IMpTagService.UpdateTagAsync), typeof(PostAttribute), "/cgi-bin/tags/update"),
        (typeof(IMpTagService), nameof(IMpTagService.DeleteTagAsync), typeof(PostAttribute), "/cgi-bin/tags/delete"),
        (typeof(IMpTagService), nameof(IMpTagService.GetTagFansAsync), typeof(PostAttribute), "/cgi-bin/user/tag/get"),
        (typeof(IMpTagService), nameof(IMpTagService.BatchTaggingAsync), typeof(PostAttribute), "/cgi-bin/tags/members/batchtagging"),
        (typeof(IMpTagService), nameof(IMpTagService.BatchUnTaggingAsync), typeof(PostAttribute), "/cgi-bin/tags/members/batchuntagging"),
        (typeof(IMpTagService), nameof(IMpTagService.GetTagIdListAsync), typeof(PostAttribute), "/cgi-bin/tags/getidlist"),
    };

    /// <summary>契约守卫 TG1：标签管理域 8 端点路由必须与官方契约一致（漂移即红）。</summary>
    [Fact]
    public void TagEndpoints_ShouldMatchOfficialRoutes()
    {
        TagRoutes.Should().HaveCount(8, "官方「用户管理 → 标签管理」恰 8 个端点");
        TagRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8, "各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in TagRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方反直觉点锁定：getTags 为 GET，其余 7 个为 POST（勿为「统一风格」改动）。
        TagRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(1,
            "标签管理域仅 getTags 为 GET（官方原文），其余全部 POST");
    }

    /// <summary>契约守卫 TG2：注册形态——本接口自身即注册接口，不得标 IsAbstract、不得有子接口。</summary>
    [Fact]
    public void TagInterfaceHierarchy_ShouldRegisterDirectly()
    {
        var iface = typeof(IMpTagService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull("标签管理域接口必须声明 [HttpClientApi]");
        api!.IsAbstract.Should().BeFalse("公众号无应用类型分化 ⇒ 本接口直接作注册接口");
        api.RegistryGroupName.Should().Be(TagRegistryGroupName,
            "必须挂 Tag 注册组（AddTagWebApiHttpClient() 注册）");
        api.TokenManage.Should().Be(nameof(IMpAppManager), "声明式令牌注入必须经公众号应用管理器定位上下文");

        iface.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .Should().BeEmpty("标签管理域不得出现 _Internal / _ThirdParty / _Provider 子接口");
    }

    /// <summary>契约守卫 TG3：令牌绑定——统一 AccessToken 路由键 + Query 注入。</summary>
    [Fact]
    public void TagTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var token = typeof(IMpTagService).GetCustomAttribute<TokenAttribute>();

        token.Should().NotBeNull("标签管理域必须声明 [Token]");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query,
            "官方契约强制 Query 注入（access_token），无法改用 Header（MUD005）");
        token.Name.Should().Be("access_token");
    }

    /// <summary>
    /// 契约守卫 TG4：标签域 DTO 全量登记进 AOT JSON 上下文（<c>SerializerClassName = "Tag"</c>）。
    /// </summary>
    [Fact]
    public void TagDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = TagJsonContext.Default;

        var domainTypes = typeof(MpTag).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Tag"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 16;
        domainTypes.Should().HaveCount(expectedCount,
            "标签域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（4 标签形态 + 2 创建 + 1 获取标签 + 1 编辑 + 1 删除 + 3 粉丝分页 + 2 批量 + 2 用户标签）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于标签域命名空间，必须登记进 TagJsonContext（AOT 源生成）");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(TagRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为标签域段 Tag");
        }
    }

    /// <summary>
    /// 契约守卫 TG5：官方字段名与形态差异锁定（**禁止**按「统一美观」改名或合并）。
    /// </summary>
    [Fact]
    public void TagDataModels_ShouldLockOfficialFieldNames()
    {
        AssertJsonProperty<MpTag>("id", "标签 ID（官方 createTag/getTags 页）");
        AssertJsonProperty<MpTag>("name", "标签名（官方页）");
        AssertJsonProperty<MpTag>("count", "标签下粉丝数（仅 getTags 下发）");

        AssertJsonProperty<MpCreateTagRequest>("tag", "创建标签请求体外层对象名照抄官方");
        AssertJsonProperty<MpTagName>("name", "创建标签 tag 对象字段");
        AssertJsonProperty<MpTagIdName>("id", "编辑标签 tag 对象标识字段");
        AssertJsonProperty<MpTagId>("id", "删除标签 tag 对象标识字段");

        AssertJsonProperty<MpGetTagFansRequest>("tagid", "官方为 tagid（无下划线），勿写成 tag_id");
        AssertJsonProperty<MpGetTagFansRequest>("next_openid", "官方分页游标名");
        AssertJsonProperty<MpGetTagFansResponse>("count", "本次获取的粉丝数量");
        AssertJsonProperty<MpGetTagFansResponse>("data", "粉丝数据容器（官方名为 data）");
        AssertJsonProperty<MpGetTagFansData>("openid", "粉丝 openid 数组（官方名为 openid，非 openid_list）");

        AssertJsonProperty<MpTagMembersRequest>("openid_list", "批量为用户打 / 取消标签请求字段");
        AssertJsonProperty<MpTagMembersRequest>("tagid", "官方为 tagid（无下划线）");
        AssertJsonProperty<MpTagMembersResponse>("fail_openid_list", "45171 部分失败时官方下发的失败清单");

        AssertJsonProperty<MpGetTagIdListRequest>("openid", "获取用户标签列表请求字段");
        AssertJsonProperty<MpGetTagIdListResponse>("tagid_list", "官方名为 tagid_list（无下划线）");

        // 「同类型两形态」处置：createTag 不下发 count ⇒ Count 必须可空（否则会以 0 冒充「无粉丝」）。
        typeof(MpTag).GetProperty(nameof(MpTag.Count))!.PropertyType.Should().Be(typeof(int?),
            "createTag 响应不含 count ⇒ 必须可空以区分「未下发」与「0 个粉丝」");

        // 失败清单字段必须可空（仅 45171 等场景下发）。
        typeof(MpTagMembersResponse).GetProperty(nameof(MpTagMembersResponse.FailOpenIdList))!.PropertyType
            .Should().Be(typeof(List<string>), "官方下发 JSON 数组，反序列化为 List<string>");
    }

    /// <summary>契约守卫 TG6：官方无请求体的端点不得不设请求 DTO（<c>getTags</c> 签名仅剩取消令牌）。</summary>
    [Fact]
    public void GetTags_ShouldHaveNoRequestBody()
    {
        var target = typeof(IMpTagService).GetMethod(
            nameof(IMpTagService.GetTagsAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        target.Should().NotBeNull();
        target!.GetParameters().Should().ContainSingle(
            p => p.ParameterType == typeof(CancellationToken),
            "官方 getTags 为 GET 且无请求体，签名仅剩取消令牌");
        target.GetCustomAttributes<QueryAttribute>().Should().BeEmpty("官方无业务 Query 参数（access_token 经 [Token] 注入）");
    }

    /// <summary>契约守卫 TG7：批量打 / 取消标签共用同一 DTO（官方两页字段表逐字段一致，拆两同构类属冗余）。</summary>
    [Fact]
    public void BatchTaggingAndUntagging_ShouldShareRequestDto()
    {
        var tagging = typeof(IMpTagService).GetMethod(nameof(IMpTagService.BatchTaggingAsync))!;
        var untagging = typeof(IMpTagService).GetMethod(nameof(IMpTagService.BatchUnTaggingAsync))!;

        tagging.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpTagMembersRequest));
        untagging.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpTagMembersRequest),
            "官方两页请求体字段表一致 ⇒ 共用 DTO；拆分为两个同构类属冗余");

        // 响应体亦共用（仅 fail_openid_list 在部分失败时出现）：两端点返回类型必须一致，
        // 否则「部分失败清单」只在一侧可读，另一侧调用方无法定向重试。
        tagging.ReturnType.Should().Be(untagging.ReturnType)
            .And.Be(typeof(Task<MpTagMembersResponse>), "两端点响应体字段表一致，共用 MpTagMembersResponse");
    }

    /// <summary>契约守卫 TG8：模块枚举 / 注册入口 / Query 令牌白名单同批更新（防新增域绕过审计面）。</summary>
    [Fact]
    public void TagModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("Tag", "标签管理域必须有模块枚举成员");
        typeof(MpServiceBuilder).GetMethod("AddTagApi").Should().NotBeNull("必须有链式注册入口 AddTagApi()");

        // Query 令牌白名单（MUD005 审计面）：新增 Query 注入接口必须同批登记。
        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpTagService),
            "本域必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 TG9：已核验官方错误码常量锁定（漂移即红——错误码是调用方重试策略的判据）。</summary>
    [Fact]
    public void TagErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.InvalidOpenId.Should().Be(40003);
        MpErrorCodes.InvalidOpenIdListSize.Should().Be(40032);
        MpErrorCodes.EmptyPostData.Should().Be(44002);
        MpErrorCodes.DailyQuotaReached.Should().Be(45009);
        MpErrorCodes.TagCountExceeded.Should().Be(45056);
        MpErrorCodes.SystemTagImmutable.Should().Be(45058);
        MpErrorCodes.UserTagCountExceeded.Should().Be(45059);
        MpErrorCodes.InvalidTagName.Should().Be(45157);
        MpErrorCodes.TagNameTooLong.Should().Be(45158);
        MpErrorCodes.InvalidTagId.Should().Be(45159);
        MpErrorCodes.ConcurrentTaggingConflict.Should().Be(45169);
        MpErrorCodes.SomeOpenIdFailed.Should().Be(45171);
        MpErrorCodes.PostDataFormatError.Should().Be(47001);
        MpErrorCodes.ApiUnauthorized.Should().Be(48001);
        MpErrorCodes.OpenIdAppIdMismatch.Should().Be(49003);
        MpErrorCodes.UserLimited.Should().Be(50002);
        MpErrorCodes.UserUnsubscribed.Should().Be(50005);
        MpErrorCodes.ThirdPartyClientIpNotRegistered.Should().Be(61004);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        var property = typeof(T).GetProperties()
            .SingleOrDefault(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName);
        property.Should().NotBeNull($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
