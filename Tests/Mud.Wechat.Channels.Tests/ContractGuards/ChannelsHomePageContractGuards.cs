// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.HomePage;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「主页管理」域契约守卫（CHP 系列：HomePage 域 14 端点路由 + 三前缀分布 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 审核流语义 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「视频号橱窗」严格区分（守卫措辞锁定）</b>：本域 <c>/channels/ec/store/window/*</c> 是
/// <b>微信小店店铺主页</b>对用户展示的商品；视频号个人橱窗走 <c>/channels/ec/window/product/*</c>
/// （<c>ChannelsWindowContractGuards</c> 锁定）。两组「同族不同前缀」，各自照官方原文落位，
/// 禁止合并、禁止互相改写前缀。
/// </para>
/// <para>
/// <b>审核流语义</b>：背景图与精选展示位为「提交申请 → 审核 → 生效」模型——「撤销申请」不影响已生效内容，
/// 「清空」接口才同时移除生效内容；查询接口同时返回生效内容与流程中的申请。
/// </para>
/// <para>
/// <b>商品分类族只有读侧（CHP7 锁定）</b>：官方「主页管理」目录内商品分类仅 <c>tree/get</c> 与
/// <c>tree/product/get</c> 两支只读端点；分类树设置与分类商品增删（<c>tree/set</c>、
/// <c>tree/product/add|del</c>）在官方文档面<b>不存在</b>（页面 404、目录无页），故本域 14 端点即官方 14 端点，
/// 不得按第三方 SDK 清单回填写侧端点。分类的维护由商家在小店后台完成。
/// </para>
/// </remarks>
public class ChannelsHomePageContractGuards
{
    private const string HomePageRegistryGroupName = "HomePage";

    /// <summary>HomePage 域官方路由表（14 端点：/channels/ec/store/window/* 4 + /channels/ec/basics/homepage/* 8 + /channels/ec/store/classification/* 2，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] HomePageRoutes =
    {
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.GetStoreWindowProductListAsync), typeof(PostAttribute), "/channels/ec/store/window/product/list/get"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.ReorderStoreWindowProductAsync), typeof(PostAttribute), "/channels/ec/store/window/product/reorder"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.HideStoreWindowProductAsync), typeof(PostAttribute), "/channels/ec/store/window/product/hide"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.SetTopStoreWindowProductAsync), typeof(PostAttribute), "/channels/ec/store/window/product/settop"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.SubmitBackgroundApplyAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/background/apply/submit"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.GetBackgroundAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/background/get"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.RemoveBackgroundAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/background/remove"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.CancelBackgroundApplyAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/background/apply/cancel"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.SubmitBannerApplyAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/banner/apply/submit"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.GetBannerAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/banner/get"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.RemoveBannerAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/banner/remove"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.CancelBannerApplyAsync), typeof(PostAttribute), "/channels/ec/basics/homepage/banner/apply/cancel"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.GetClassificationTreeAsync), typeof(PostAttribute), "/channels/ec/store/classification/tree/get"),
        (typeof(IChannelsHomePageService), nameof(IChannelsHomePageService.GetClassificationProductListAsync), typeof(PostAttribute), "/channels/ec/store/classification/tree/product/get"),
    };

    /// <summary>契约守卫 CHP1：HomePage 域 14 端点路由必须与官方契约一致（全部 POST，三前缀并存）。</summary>
    [Fact]
    public void HomePageEndpoints_ShouldMatchOfficialRoutes()
    {
        // 14 端点 = 主页商品排序 4 + 主页背景图 4 + 精选展示位 4 + 商品分类 2。
        HomePageRoutes.Should().HaveCount(14, "主页商品排序 4 + 主页背景图 4 + 精选展示位 4 + 商品分类 2 = 14 端点");
        HomePageRoutes.Select(r => r.Route).Distinct().Should().HaveCount(14);

        HomePageRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/store/window/", StringComparison.Ordinal))
            .Should().Be(4, "主页商品排序 4 端点走 /channels/ec/store/window/*（小店店铺主页，非视频号橱窗）");
        HomePageRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/basics/homepage/", StringComparison.Ordinal))
            .Should().Be(8, "主页背景图 4 + 精选展示位 4 共 8 端点走 /channels/ec/basics/homepage/*");
        HomePageRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/store/classification/", StringComparison.Ordinal))
            .Should().Be(2, "店铺主页商品分类 2 端点走 /channels/ec/store/classification/*");

        foreach (var (iface, method, httpAttribute, route) in HomePageRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CHP2：HomePage 域 14 端点全部为 POST；官方空请求体端点不得携带 [Body] 参数。</summary>
    [Fact]
    public void HomePageEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in HomePageRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（主页管理无 GET 端点）");
        }

        // 官方参数表无 Body 参数的 5 端点不得人为造请求体 DTO（查询背景图 / 清空背景图 / 查询展示位 / 清空展示位 / 分类树）。
        foreach (var emptyEndpoint in new[]
                 {
                     nameof(IChannelsHomePageService.GetBackgroundAsync),
                     nameof(IChannelsHomePageService.RemoveBackgroundAsync),
                     nameof(IChannelsHomePageService.GetBannerAsync),
                     nameof(IChannelsHomePageService.RemoveBannerAsync),
                     nameof(IChannelsHomePageService.GetClassificationTreeAsync),
                 })
        {
            var target = typeof(IChannelsHomePageService).GetMethod(emptyEndpoint)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{emptyEndpoint} 官方参数表无 Body 参数——不得声明 [Body]");
        }
    }

    /// <summary>契约守卫 CHP3：令牌绑定与注册形态。</summary>
    [Fact]
    public void HomePageInterface_ShouldBindChannelsToken_AndRegisterUnderHomePageGroup()
    {
        var token = typeof(IChannelsHomePageService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("HomePage 域 14 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsHomePageService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(HomePageRegistryGroupName, "必须挂 HomePage 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("HomePage");
        typeof(ChannelsServiceBuilder).GetMethod("AddHomePageApi").Should().NotBeNull("必须提供 AddHomePageApi 链式注册方法");
    }

    /// <summary>契约守卫 CHP4：HomePage 域 DTO 全量登记进 AOT JSON 上下文；常量类不得标序列化特性。</summary>
    [Fact]
    public void HomePageDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = HomePageJsonContext.Default;

        var domainTypes = typeof(ChannelsGetStoreWindowProductListRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.HomePage"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(27,
            "HomePage 域 14 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（主页商品排序 6 + 主页背景图 3 + 背景图/展示位跨组共用申请 2 + 精选展示位 10 + 商品分类 6 = 27）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 HomePage 域命名空间，必须登记进 HomePageJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(HomePageRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 HomePage");
        }

        // 常量类（非序列化 DTO）：申请状态 1 + banner 样式 1 + banner 类型 1，不得标 [HttpJsonSerializable]。
        domainTypes.Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .Should().HaveCount(3, "本域常量类应为 ChannelsHomepageApplyStates / ChannelsBannerScales / ChannelsBannerTypes 三个");
    }

    /// <summary>契约守卫 CHP5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void HomePageDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 主页商品排序。
        AssertJsonProperty<ChannelsGetStoreWindowProductListRequest>("page_size", "每页数量");
        AssertJsonProperty<ChannelsGetStoreWindowProductListRequest>("next_key", "翻页上下文");
        AssertJsonProperty<ChannelsGetStoreWindowProductListResponse>("products", "主页展示商品列表");
        AssertJsonProperty<ChannelsGetStoreWindowProductListResponse>("total_num", "商品总数");
        AssertJsonProperty<ChannelsStoreWindowProduct>("product_id", "商品 ID");
        AssertJsonProperty<ChannelsStoreWindowProduct>("is_set_hide", "是否隐藏");
        AssertJsonProperty<ChannelsStoreWindowProduct>("is_set_top", "是否置顶");
        AssertJsonProperty<ChannelsReorderStoreWindowProductRequest>("index_num", "新序号（最大移动步长 500）");
        AssertJsonProperty<ChannelsHideStoreWindowProductRequest>("is_set_hide", "是否隐藏");
        AssertJsonProperty<ChannelsSetTopStoreWindowProductRequest>("is_set_top", "是否置顶");

        // 主页背景图。
        AssertJsonProperty<ChannelsSubmitHomepageBackgroundApplyRequest>("img_url", "图片链接");
        AssertJsonProperty<ChannelsGetHomepageBackgroundResponse>("img_url", "当前生效的背景图");
        AssertJsonProperty<ChannelsGetHomepageBackgroundResponse>("apply", "流程中的申请");
        AssertJsonProperty<ChannelsHomepageBackgroundApply>("audit_desc", "审核结果描述");

        // 精选展示位（嵌套形态 {"banner":{…}} 与官方请求示例一致）。
        AssertJsonProperty<ChannelsSubmitBannerApplyRequest>("banner", "精选展示位模块");
        AssertJsonProperty<ChannelsBannerModule>("scale", "展示样式");
        AssertJsonProperty<ChannelsBannerModule>("title", "模块标题");
        AssertJsonProperty<ChannelsBannerItem>("type", "展示位类型");
        AssertJsonProperty<ChannelsBannerItem>("finder", "视频号参数");
        AssertJsonProperty<ChannelsBannerItem>("official_account", "公众号参数");
        AssertJsonProperty<ChannelsBannerContent>("img_url", "展示位图片");
        AssertJsonProperty<ChannelsBannerContent>("description", "描述（仅大图模式）");
        AssertJsonProperty<ChannelsBannerProduct>("product_id", "商品 ID");
        AssertJsonProperty<ChannelsBannerFinder>("finder_user_name", "视频号 ID");
        AssertJsonProperty<ChannelsBannerFinder>("feed_id", "视频 ID");
        AssertJsonProperty<ChannelsBannerOfficialAccount>("url", "公众号文章链接");
        AssertJsonProperty<ChannelsBannerApply>("state", "申请状态");
        AssertJsonProperty<ChannelsBannerApplyItem>("audit_state", "审核状态（申请明细）");

        // 商品分类。
        AssertJsonProperty<ChannelsGetClassificationTreeResponse>("tree", "分类树");
        AssertJsonProperty<ChannelsGetClassificationTreeResponse>("version", "分类树版本号");
        AssertJsonProperty<ChannelsClassificationTree>("level_1", "一级分类列表");
        AssertJsonProperty<ChannelsClassificationTree>("tree_id", "分类树 ID");
        AssertJsonProperty<ChannelsClassificationNode>("level_2", "二级分类列表（自引用共用节点）");
        AssertJsonProperty<ChannelsGetClassificationProductListRequest>("req", "官方嵌套请求体对象名");
        AssertJsonProperty<ChannelsClassificationProductQuery>("level_1_id", "一级分类 ID");
        AssertJsonProperty<ChannelsClassificationProductQuery>("page_context", "翻页上下文");
        AssertJsonProperty<ChannelsGetClassificationProductListResponse>("product_ids", "分类关联的商品 ID 列表");

        // 共用 DTO 裁决：background/apply/cancel 与 banner/apply/cancel 请求体字段集一致（{"apply_id": number}）⇒ 共用 ChannelsHomepageApplyIdRequest。
        typeof(IChannelsHomePageService).GetMethod(nameof(IChannelsHomePageService.CancelBackgroundApplyAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsHomepageApplyIdRequest));
        typeof(IChannelsHomePageService).GetMethod(nameof(IChannelsHomePageService.CancelBannerApplyAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsHomepageApplyIdRequest));

        // 共用 DTO 裁决：background/apply/submit 与 banner/apply/submit 响应字段集一致（errcode/errmsg/apply_id）⇒ 共用 ChannelsHomepageApplyResponse。
        typeof(IChannelsHomePageService).GetMethod(nameof(IChannelsHomePageService.SubmitBackgroundApplyAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsHomepageApplyResponse>));
        typeof(IChannelsHomePageService).GetMethod(nameof(IChannelsHomePageService.SubmitBannerApplyAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsHomepageApplyResponse>));

        // 共用 DTO 裁决：getbanner 响应中生效展示位模块与提交请求体模块字段集一致（title 仅提交时设置）⇒ 共用 ChannelsBannerModule。
        typeof(ChannelsGetBannerResponse).GetProperty(nameof(ChannelsGetBannerResponse.Banner))!
            .PropertyType.Should().Be(typeof(ChannelsBannerModule));
        typeof(ChannelsSubmitBannerApplyRequest).GetProperty(nameof(ChannelsSubmitBannerApplyRequest.Banner))!
            .PropertyType.Should().Be(typeof(ChannelsBannerModule));

        // 共用 DTO 裁决：getbanner 的 apply.banner[].banner 与生效 banner.banner[] 元素结构一致 ⇒ 共用 ChannelsBannerItem。
        typeof(ChannelsBannerApplyItem).GetProperty(nameof(ChannelsBannerApplyItem.Banner))!
            .PropertyType.Should().Be(typeof(ChannelsBannerItem));
        typeof(ChannelsBannerModule).GetProperty(nameof(ChannelsBannerModule.Banner))!
            .PropertyType.Should().Be(typeof(List<ChannelsBannerItem>));

        // 共用 DTO 裁决：分类树一级/二级节点元素结构一致（仅 2 级树）⇒ ChannelsClassificationNode 自引用共用。
        typeof(ChannelsClassificationNode).GetProperty(nameof(ChannelsClassificationNode.Level2))!
            .PropertyType.Should().Be(typeof(List<ChannelsClassificationNode>));
    }

    /// <summary>契约守卫 CHP6：枚举常量字面量锁定（审核状态 / banner 样式 / banner 类型）。</summary>
    [Fact]
    public void HomePageEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsHomepageApplyStates.Waiting.Should().Be(1);
        ChannelsHomepageApplyStates.Rejected.Should().Be(2);

        ChannelsBannerScales.Small.Should().Be(1);
        ChannelsBannerScales.Large.Should().Be(2);

        ChannelsBannerTypes.Product.Should().Be(1);
        ChannelsBannerTypes.Finder.Should().Be(3);
        ChannelsBannerTypes.OfficialAccount.Should().Be(4);
    }

    /// <summary>
    /// 契约守卫 CHP7：官方目录已下架的「商品分类写侧」三支路由（<c>store/classification/tree/set</c>、
    /// <c>store/classification/tree/product/add</c>、<c>store/classification/tree/product/del</c>）在全线零声明。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 2026-10-17 核验：缺失端点清单（DotNetCore.SKIT.FlurlHttpClient.Wechat 转述）把三支写侧端点列为缺口，
    /// 但其文档 URL <c>doc/store/shop/API/homepage/classification/{add,del}classificationproduct</c>、
    /// <c>setclassificationtree</c> 全部返回 <b>HTTP 404</b>（非软 200），且官方「主页管理」接口目录页
    /// （<c>API/homepage/shoptype/</c>）与全站小店 API 导航（494 条链接）内<b>均无</b>商品分类写侧页面 ——
    /// 商品分类族现行面只有 <c>tree/get</c> 与 <c>tree/product/get</c> 两支只读端点。
    /// 按「判据是官方文档面而非网关可达性」⇒ 三支<b>非契约</b>，本域 <b>14 端点与官方目录 14 端点逐一对齐</b>，
    /// 不得据 SKIT 清单回填。
    /// </para>
    /// </remarks>
    [Fact]
    public void RemovedClassificationWriteRoutes_ShouldHaveZeroDeclaration()
    {
        var allRoutes = typeof(IChannelsHomePageService).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.Namespace != null && t.Namespace.StartsWith("Mud.Wechat.Channels", StringComparison.Ordinal))
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(static m => m.GetCustomAttributes().OfType<HttpMethodAttribute>())
            .Select(static a => a.RequestUri ?? string.Empty)
            .ToList();

        allRoutes.Should().NotBeEmpty("反射枚举不得空跑（正对照）");

        foreach (var removed in new[]
        {
            "/channels/ec/store/classification/tree/set",
            "/channels/ec/store/classification/tree/product/add",
            "/channels/ec/store/classification/tree/product/del",
        })
        {
            allRoutes.Count(r => r.Equals(removed, StringComparison.Ordinal))
                .Should().Be(0,
                    $"{removed} 的官方文档页返回 404 且不在「主页管理」接口目录内（2026-10-17 核验）⇒ 非契约路由零声明；"
                    + "商品分类族官方只读面仅 tree/get 与 tree/product/get 两支");
        }

        // 正对照：本域现行分类只读两支与官方目录 14 端点计数在场（扫描根确实覆盖 HomePage 域接口）。
        allRoutes.Should().Contain("/channels/ec/store/classification/tree/get",
            "正对照：现行分类树查询路由必须被扫描面覆盖，否则本断言恒真");
        allRoutes.Should().Contain("/channels/ec/store/classification/tree/product/get",
            "正对照：现行分类商品查询路由必须被扫描面覆盖");
        HomePageRoutes.Should().HaveCount(14,
            "本域端点计数须与官方「主页管理」目录页 14 支逐一对齐（4 排序 + 4 背景图 + 4 展示位 + 2 分类只读）");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
