// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Abstractions.Contracts;
using Mud.Wechat.OfficialAccount.DataModels.User;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 用户管理·用户信息域契约守卫：路由表、注册形态、令牌绑定、DTO 字段与形态差异、JSON 上下文登记。
/// </summary>
/// <remarks>
/// 官方事实来源：服务端 API 索引 → 用户管理 → 用户信息，共 <b>7</b> 个端点（含黑名单三端点）；
/// 请求方式与请求路径逐页核验。域级约束：均「仅认证」；<c>updateRemark</c> 正文更窄
/// （「暂时开放给微信认证的服务号」）；批量 100 / 关注者 10000 / 黑名单 1000 / 拉黑 20。
/// </remarks>
public class MpUserContractGuards
{
    private const string UserRegistryGroupName = "User";

    /// <summary>用户信息域官方路由表（7 端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] UserRoutes =
    {
        (typeof(IMpUserService), nameof(IMpUserService.GetUserInfoAsync), typeof(GetAttribute), "/cgi-bin/user/info"),
        (typeof(IMpUserService), nameof(IMpUserService.BatchGetUserInfoAsync), typeof(PostAttribute), "/cgi-bin/user/info/batchget"),
        (typeof(IMpUserService), nameof(IMpUserService.GetFansAsync), typeof(GetAttribute), "/cgi-bin/user/get"),
        (typeof(IMpUserService), nameof(IMpUserService.UpdateRemarkAsync), typeof(PostAttribute), "/cgi-bin/user/info/updateremark"),
        (typeof(IMpUserService), nameof(IMpUserService.GetBlacklistAsync), typeof(PostAttribute), "/cgi-bin/tags/members/getblacklist"),
        (typeof(IMpUserService), nameof(IMpUserService.BatchBlacklistAsync), typeof(PostAttribute), "/cgi-bin/tags/members/batchblacklist"),
        (typeof(IMpUserService), nameof(IMpUserService.BatchUnblacklistAsync), typeof(PostAttribute), "/cgi-bin/tags/members/batchunblacklist"),
        // 转换 openid：官方列为「用户管理」下与「用户信息」并列的子分组，本 SDK 以「用户管理」为域边界收在本接口。
        (typeof(IMpUserService), nameof(IMpUserService.ChangeOpenIdAsync), typeof(PostAttribute), "/cgi-bin/changeopenid"),
    };

    /// <summary>
    /// 契约守卫 US1：用户管理域 8 端点路由必须与官方契约一致
    /// （用户信息 7 + 转换 openid 1；域边界合并的理由见接口 remarks 与实现文档）。
    /// </summary>
    [Fact]
    public void UserEndpoints_ShouldMatchOfficialRoutes()
    {
        UserRoutes.Should().HaveCount(8, "本域 = 用户信息 7 端点 + 转换 openid 1 端点");
        UserRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8, "各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in UserRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方反直觉点锁定：user/info 与 user/get 为 GET（Query 传参、无请求体），其余 5 个为 POST。
        UserRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(2,
            "仅 user/info 与 user/get 为 GET（官方原文）");

        // 黑名单路径前缀为 /cgi-bin/tags/members/（与标签域同前缀但属用户信息分组）——勿误移入标签域。
        UserRoutes.Count(r => r.Route.StartsWith("/cgi-bin/tags/members/", StringComparison.Ordinal))
            .Should().Be(3, "黑名单三端点官方路径位于 tags/members 前缀下");
    }

    /// <summary>契约守卫 US2：注册形态——本接口自身即注册接口，无应用类型子接口。</summary>
    [Fact]
    public void UserInterfaceHierarchy_ShouldRegisterDirectly()
    {
        var iface = typeof(IMpUserService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull("用户信息域接口必须声明 [HttpClientApi]");
        api!.IsAbstract.Should().BeFalse("公众号无应用类型分化 ⇒ 本接口直接作注册接口");
        api.RegistryGroupName.Should().Be(UserRegistryGroupName, "必须挂 User 注册组（AddUserWebApiHttpClient()）");
        api.TokenManage.Should().Be(nameof(IMpAppManager));

        iface.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .Should().BeEmpty("用户信息域不得出现应用类型子接口");
    }

    /// <summary>契约守卫 US3：令牌绑定——统一 AccessToken 路由键 + Query 注入。</summary>
    [Fact]
    public void UserTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var token = typeof(IMpUserService).GetCustomAttribute<TokenAttribute>();

        token.Should().NotBeNull();
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");
    }

    /// <summary>契约守卫 US4：用户信息域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void UserDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = UserJsonContext.Default;

        var domainTypes = typeof(MpUserInfo).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.User"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 14;
        domainTypes.Should().HaveCount(expectedCount,
            "用户管理域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（2 用户信息 + 3 批量 + 4 分页/黑名单 + 1 备注 + 1 拉黑请求 + 3 转换 openid）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于用户信息域命名空间，必须登记进 UserJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(UserRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 User");
        }
    }

    /// <summary>
    /// 契约守卫 US5：官方字段名、已停供字段的<b>不建模</b>决定、以及两页类型不一致的处置。
    /// </summary>
    [Fact]
    public void UserDataModels_ShouldLockOfficialFieldsAndStopSuppliedFields()
    {
        AssertJsonProperty<MpUserInfo>("subscribe", "是否订阅（0 = 未关注，此时拉不到其余信息）");
        AssertJsonProperty<MpUserInfo>("openid", "用户标识");
        AssertJsonProperty<MpUserInfo>("language", "官方标注「不再提供」但仍在字段表 ⇒ 保留为可空");
        AssertJsonProperty<MpUserInfo>("subscribe_time", "关注时间戳（多次关注取最后一次）");
        AssertJsonProperty<MpUserInfo>("unionid", "需绑定开放平台才出现");
        AssertJsonProperty<MpUserInfo>("remark", "运营者备注");
        AssertJsonProperty<MpUserInfo>("groupid", "兼容旧用户分组接口");
        AssertJsonProperty<MpUserInfo>("tagid_list", "标签 ID 列表");
        AssertJsonProperty<MpUserInfo>("subscribe_scene", "关注渠道来源（ADD_SCENE_*）");
        AssertJsonProperty<MpUserInfo>("qr_scene", "二维码扫码场景");
        AssertJsonProperty<MpUserInfo>("qr_scene_str", "二维码扫码场景描述");

        // 2021-12-27 起官方不再输出头像 / 昵称：这些字段<b>不得</b>被建模（否则暗示「也许能读到」）。
        var stopSupplied = new[] { "nickname", "sex", "city", "province", "country", "headimgurl" };
        var modeledJsonNames = typeof(MpUserInfo).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .ToList();
        foreach (var field in stopSupplied)
        {
            modeledJsonNames.Should().NotContain(field,
                $"官方已明示「不再输出头像、昵称信息」（{field}）⇒ SDK 不建模，避免给出可读到的错误暗示");
        }

        // 官方两页对 subscribe_scene 的类型标注不一致（string vs number）⇒ 以带示例的一页为准（string）。
        typeof(MpUserInfo).GetProperty(nameof(MpUserInfo.SubscribeScene))!.PropertyType
            .Should().Be(typeof(string), "官方示例为 \"ADD_SCENE_QR_CODE\" ⇒ 按字符串建模");
        typeof(MpUserInfo).GetProperty(nameof(MpUserInfo.SubscribeTime))!.PropertyType
            .Should().Be(typeof(long), "官方类型为 timestamp ⇒ 秒级 Unix 时间戳");

        // user/info 响应形态：用户字段平铺 + 出错时附 errcode（判错契约显式实现）。
        typeof(MpUserInfoResponse).Should().BeAssignableTo<MpUserInfo>();
        typeof(MpUserInfoResponse).Should().BeAssignableTo<IWechatApiResponse>(
            "成功与失败共用同一层级 ⇒ 判错契约必须以显式实现补上");
        new MpUserInfoResponse().IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0 视为成功");
        new MpUserInfoResponse { ErrorCode = 40013 }.IsSuccess.Should().BeFalse();

        AssertJsonProperty<MpBatchGetUserInfoRequest>("user_list", "批量请求外层对象（单次最多 100 条）");
        AssertJsonProperty<MpBatchUserInfoItem>("openid", "批量请求元素字段");
        AssertJsonProperty<MpBatchUserInfoItem>("lang", "批量请求元素可选语言");
        AssertJsonProperty<MpBatchGetUserInfoResponse>("user_info_list", "批量响应外层对象");

        // 批量响应元素与单查响应字段集一致 ⇒ 必须共用同一类型（两份声明会漂移）。
        typeof(MpBatchGetUserInfoResponse).GetProperty(nameof(MpBatchGetUserInfoResponse.UserInfoList))!
            .PropertyType.Should().Be(typeof(List<MpUserInfo>));

        AssertJsonProperty<MpUpdateRemarkRequest>("openid", "设置备注请求字段");
        AssertJsonProperty<MpUpdateRemarkRequest>("remark", "新的备注名（字节数须 < 30）");

        // 关注者列表：单批 10000；分页游标请求/响应同名 next_openid。
        AssertJsonProperty<MpGetFansResponse>("total", "关注总用户数");
        AssertJsonProperty<MpGetFansResponse>("count", "本批个数（最大 10000）");
        AssertJsonProperty<MpGetFansResponse>("data", "openid 容器");
        AssertJsonProperty<MpGetFansResponse>("next_openid", "为空表示列表结束");
        AssertJsonProperty<MpOpenIdPageData>("openid", "关注者 / 黑名单共用容器字段");

        // 黑名单：请求游标 begin_openid 与响应游标 next_openid <b>名字不同</b>（官方原文，勿互相「对齐」）。
        AssertJsonProperty<MpGetBlacklistRequest>("begin_openid", "请求起始游标（每批最多 1000）");
        AssertJsonProperty<MpGetBlacklistResponse>("next_openid", "响应游标（回填为请求的 begin_openid）");

        AssertJsonProperty<MpBlacklistRequest>("openid_list", "拉黑 / 取消拉黑共用请求字段（单次最多 20）");

        // 转换 openid：from_appid 是「原账号原始 id（gh_ 开头，不是 appid）」；官方无 to_appid。
        AssertJsonProperty<MpChangeOpenIdRequest>("from_appid", "原账号原始 id（不是 appid）");
        AssertJsonProperty<MpChangeOpenIdRequest>("openid_list", "单次最多 100 个，且必须是旧账号仍关注的用户");
        AssertJsonProperty<MpChangeOpenIdResponse>("result_list", "逐项转换结果");
        AssertJsonProperty<MpChangeOpenIdResult>("ori_openid", "旧 openid");
        AssertJsonProperty<MpChangeOpenIdResult>("new_openid", "新 openid");
        AssertJsonProperty<MpChangeOpenIdResult>("err_msg", "逐项错误描述（ok / ori_openid error）");

        JsonNamesOf<MpChangeOpenIdRequest>().Should().NotContain("to_appid",
            "官方字段表无 to_appid（目标账号由调用方令牌身份隐含）⇒ 不得凭空建模");
    }

    /// <summary>契约守卫 US6：GET 端点无请求体（签名仅剩 Query 参数与取消令牌）。</summary>
    [Fact]
    public void GetEndpoints_ShouldHaveNoRequestBody()
    {
        var userInfo = typeof(IMpUserService).GetMethod(nameof(IMpUserService.GetUserInfoAsync))!;
        // Query 特性挂在<b>参数</b>上（方法级为空——Work 侧守卫断言方法级为空的原因即此）。
        userInfo.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "openid", "lang" }, "官方 Query 参数逐项一致");
        userInfo.GetParameters().Should().NotContain(p => p.ParameterType == typeof(MpUserInfo));

        var fans = typeof(IMpUserService).GetMethod(nameof(IMpUserService.GetFansAsync))!;
        fans.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "next_openid" });
        fans.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(CancellationToken));
    }

    /// <summary>契约守卫 US7：批量 / 备注限制常量与共用 DTO 关系锁定。</summary>
    [Fact]
    public void UserLimitsAndSharedDtos_ShouldBeLocked()
    {
        MpUpdateRemarkRequest.RemarkByteLimit.Should().Be(30,
            "官方原文「长度必须小于 30 字节」——是字节不是字符，SDK 以常量表达上限但不自动截断");

        var blacklist = typeof(IMpUserService).GetMethod(nameof(IMpUserService.BatchBlacklistAsync))!;
        var unblacklist = typeof(IMpUserService).GetMethod(nameof(IMpUserService.BatchUnblacklistAsync))!;
        blacklist.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpBlacklistRequest));
        unblacklist.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpBlacklistRequest),
            "官方两页请求体字段表一致 ⇒ 共用 DTO");

        // 黑名单响应与关注者响应共用 data 容器，但响应类型必须各自声明（单批上限不同：1000 vs 10000）。
        typeof(MpGetBlacklistResponse).Should().NotBe(typeof(MpGetFansResponse));
        typeof(MpGetBlacklistResponse).GetProperty(nameof(MpGetBlacklistResponse.Data))!.PropertyType
            .Should().Be(typeof(MpOpenIdPageData));
        typeof(MpGetFansResponse).GetProperty(nameof(MpGetFansResponse.Data))!.PropertyType
            .Should().Be(typeof(MpOpenIdPageData));
    }

    /// <summary>契约守卫 US8：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void UserModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("User");
        typeof(MpServiceBuilder).GetMethod("AddUserApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpUserService),
            "本域必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 US9：用户管理域已核验错误码常量锁定。</summary>
    [Fact]
    public void UserErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.InvalidRemarkName.Should().Be(40092);
        MpErrorCodes.RequireSubscribe.Should().Be(43004);
        MpErrorCodes.RequirePostMethod.Should().Be(43002);
        MpErrorCodes.BatchBlacklistSystemBusy.Should().Be(268487001);
        MpErrorCodes.GetFansSystemBusy.Should().Be(268487002);
        MpErrorCodes.ChangeOpenIdAppIdWrong.Should().Be(63178);
        MpErrorCodes.ChangeOpenIdListEmpty.Should().Be(63182);
        MpErrorCodes.ChangeOpenIdAppIdError.Should().Be(63183);
    }

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    /// <remarks>
    /// 以 LINQ <c>Any</c> + <c>BeTrue</c> 表达（<c>Should().Contain(表达式, because)</c> 的谓词是表达式树，
    /// 不接受空传播运算符 <c>?.</c>）。
    /// </remarks>
    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
