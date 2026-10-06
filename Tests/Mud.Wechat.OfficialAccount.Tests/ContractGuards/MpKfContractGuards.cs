// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.OfficialAccount.DataModels.KfAccount;
using Mud.Wechat.OfficialAccount.DataModels.KfSession;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 客服管理（7）+ 会话控制（5）契约守卫：路由表、参数位置、multipart 契约、DTO 形态与类型差异。
/// </summary>
public class MpKfContractGuards
{
    /// <summary>客服管理官方路由表（7 端点）。</summary>
    private static readonly (string Method, Type HttpAttribute, string Route)[] KfAccountRoutes =
    {
        (nameof(IMpKfAccountService.GetKfListAsync), typeof(GetAttribute), "/cgi-bin/customservice/getkflist"),
        (nameof(IMpKfAccountService.GetOnlineKfListAsync), typeof(GetAttribute), "/cgi-bin/customservice/getonlinekflist"),
        (nameof(IMpKfAccountService.AddKfAccountAsync), typeof(PostAttribute), "/customservice/kfaccount/add"),
        (nameof(IMpKfAccountService.UpdateKfAccountAsync), typeof(PostAttribute), "/customservice/kfaccount/update"),
        (nameof(IMpKfAccountService.DelKfAccountAsync), typeof(PostAttribute), "/customservice/kfaccount/del"),
        (nameof(IMpKfAccountService.UploadKfHeadImgAsync), typeof(PostAttribute), "/customservice/kfaccount/uploadheadimg"),
        (nameof(IMpKfAccountService.InviteKfWorkerAsync), typeof(PostAttribute), "/customservice/kfaccount/inviteworker"),
    };

    /// <summary>会话控制官方路由表（5 端点）。</summary>
    private static readonly (string Method, Type HttpAttribute, string Route)[] KfSessionRoutes =
    {
        (nameof(IMpKfSessionService.CreateSessionAsync), typeof(PostAttribute), "/customservice/kfsession/create"),
        (nameof(IMpKfSessionService.CloseSessionAsync), typeof(PostAttribute), "/customservice/kfsession/close"),
        (nameof(IMpKfSessionService.GetSessionAsync), typeof(GetAttribute), "/customservice/kfsession/getsession"),
        (nameof(IMpKfSessionService.GetSessionListAsync), typeof(GetAttribute), "/customservice/kfsession/getsessionlist"),
        (nameof(IMpKfSessionService.GetWaitCaseAsync), typeof(GetAttribute), "/customservice/kfsession/getwaitcase"),
    };

    /// <summary>契约守卫 KF1：客服管理 7 端点路由必须与官方契约一致。</summary>
    [Fact]
    public void KfAccountEndpoints_ShouldMatchOfficialRoutes()
    {
        KfAccountRoutes.Should().HaveCount(7, "官方「客服消息 → 客服管理」恰 7 个端点");
        KfAccountRoutes.Select(r => r.Route).Distinct().Should().HaveCount(7);

        foreach (var (method, httpAttribute, route) in KfAccountRoutes)
        {
            var target = typeof(IMpKfAccountService)
                .GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route);
        }

        // 官方路径前缀差异：仅列表两端点带 /cgi-bin，账号增删改与头像 / 邀请无该前缀 ⇒ 勿「顺手统一」。
        KfAccountRoutes.Count(r => r.Route.StartsWith("/cgi-bin/", StringComparison.Ordinal))
            .Should().Be(2, "仅 getkflist / getonlinekflist 官方带 /cgi-bin 前缀");
    }

    /// <summary>契约守卫 KF2：会话控制 5 端点路由与 Query 参数位置（官方契约）。</summary>
    [Fact]
    public void KfSessionEndpoints_ShouldMatchOfficialRoutesAndQueryBinding()
    {
        KfSessionRoutes.Should().HaveCount(5, "官方「客服消息 → 会话控制」恰 5 个端点");

        foreach (var (method, httpAttribute, route) in KfSessionRoutes)
        {
            var target = typeof(IMpKfSessionService)
                .GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull();
            attr!.RequestUri.Should().Be(route);
        }

        // 官方参数位置（易错点）：getsession 的 openid 与 getsessionlist 的 kf_account 都是 Query，不是请求体。
        QueryNamesOf<IMpKfSessionService>(nameof(IMpKfSessionService.GetSessionAsync))
            .Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf<IMpKfSessionService>(nameof(IMpKfSessionService.GetSessionListAsync))
            .Should().BeEquivalentTo(new[] { "kf_account" });

        // getwaitcase 官方无业务 Query 参数（仅 access_token 由 [Token] 注入）。
        QueryNamesOf<IMpKfSessionService>(nameof(IMpKfSessionService.GetWaitCaseAsync)).Should().BeEmpty();

        // 创建 / 关闭会话为 POST + 请求体（官方两页字段表一致 ⇒ 共用 MpKfSessionRequest）。
        typeof(IMpKfSessionService).GetMethod(nameof(IMpKfSessionService.CreateSessionAsync))!
            .GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpKfSessionRequest));
        typeof(IMpKfSessionService).GetMethod(nameof(IMpKfSessionService.CloseSessionAsync))!
            .GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpKfSessionRequest),
                "官方创建 / 关闭会话请求体字段表逐字段一致 ⇒ 共用 DTO");
    }

    /// <summary>
    /// 契约守卫 KF3：客服头像上传的 multipart 契约——表单内容由调用方以文件字段名 <c>media</c> 构建，
    /// 且 <c>kf_account</c> 必须走 <b>Query</b>（官方契约，非表单字段）。
    /// </summary>
    [Fact]
    public void UploadKfHeadImg_ShouldUseMultipartWithQueryAccount()
    {
        var target = typeof(IMpKfAccountService).GetMethod(nameof(IMpKfAccountService.UploadKfHeadImgAsync))!;
        var parameters = target.GetParameters();

        parameters.Should().ContainSingle(p => p.ParameterType == typeof(IFormContent),
            "multipart 表单内容必须由调用方构建 IFormContent 实现（文件字段名须为 media，≤5M）");
        parameters.Single(p => p.ParameterType == typeof(IFormContent))
            .GetCustomAttribute<MultipartFormAttribute>().Should().NotBeNull(
                "生成器对 [FormContent] 发射的调用在运行时接口上不存在（CS1061）⇒ 必须标 [MultipartForm]");

        var kfAccountParam = parameters.Single(p => p.Name == "kfAccount");
        kfAccountParam.GetCustomAttribute<QueryAttribute>()!.Name.Should().Be("kf_account",
            "官方把 kf_account 放在 Query（…/uploadheadimg?access_token=…&kf_account=…）");
    }

    /// <summary>契约守卫 KF4：两域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void KfDataModels_ShouldBeRegisteredInJsonContext()
    {
        AssertDomainRegistered<MpKfAccount>("KfAccount", KfAccountJsonContext.Default, 8,
            "（2 列表响应 + 2 元素 + 4 请求：添加 / 修改 / 删除 / 邀请）");
        AssertDomainRegistered<MpKfSession>("KfSession", KfSessionJsonContext.Default, 6,
            "（1 共用请求 + 1 会话元素 + 1 状态响应 + 1 列表响应 + 1 未接入元素 + 1 未接入响应）");
    }

    /// <summary>
    /// 契约守卫 KF5：官方字段名与「两页 <c>kf_id</c> 类型不同」的显式处置锁定。
    /// </summary>
    [Fact]
    public void KfDataModels_ShouldLockOfficialFieldNamesAndTypeDifference()
    {
        JsonNamesOf<MpKfAccount>().Should().BeEquivalentTo(
            new[] { "kf_account", "kf_nick", "kf_headimgurl", "kf_id" });
        JsonNamesOf<MpKfOnlineAccount>().Should().BeEquivalentTo(
            new[] { "kf_account", "status", "kf_id", "accepted_case", "kf_openid" });

        // 官方两页对 kf_id 的类型标注不同（列表页 string、在线页 number）⇒ SDK 分别建模，不合并类型。
        typeof(MpKfAccount).GetProperty(nameof(MpKfAccount.KfId))!.PropertyType.Should().Be(typeof(string),
            "官方 getkflist 页 kf_id 为字符串");
        typeof(MpKfOnlineAccount).GetProperty(nameof(MpKfOnlineAccount.KfId))!.PropertyType.Should().Be(typeof(long),
            "官方 getonlinekflist 页 kf_id 为数值 ⇒ 两页形态不同，故保留两套 DTO");

        JsonNamesOf<MpAddKfAccountRequest>().Should().BeEquivalentTo(new[] { "kf_account", "nickname", "business_id" });
        JsonNamesOf<MpUpdateKfAccountRequest>().Should().BeEquivalentTo(new[] { "kf_account", "nickname" },
            "官方修改页无 business_id ⇒ 不与添加共用 DTO");
        JsonNamesOf<MpDelKfAccountRequest>().Should().BeEquivalentTo(new[] { "kf_account" });
        JsonNamesOf<MpInviteKfWorkerRequest>().Should().BeEquivalentTo(new[] { "kf_account", "invite_wx" });

        JsonNamesOf<MpKfSessionRequest>().Should().BeEquivalentTo(new[] { "kf_account", "openid" });
        JsonNamesOf<MpKfSession>().Should().BeEquivalentTo(new[] { "createtime", "openid" });
        JsonNamesOf<MpGetKfSessionResponse>().Should().BeEquivalentTo(
            new[] { "createtime", "kf_account", "errcode", "errmsg" });
        JsonNamesOf<MpGetKfSessionListResponse>().Should().BeEquivalentTo(new[] { "sessionlist", "errcode", "errmsg" });

        // getwaitcase 元素官方仅 latest_time + openid（页面未出现 kf_account/createtime）⇒ 不得补全。
        JsonNamesOf<MpWaitCase>().Should().BeEquivalentTo(new[] { "latest_time", "openid" });
        JsonNamesOf<MpGetWaitCaseResponse>().Should().BeEquivalentTo(
            new[] { "count", "waitcaselist", "errcode", "errmsg" });

        // 官方「添加客服账号」无 password 字段 ⇒ 不得凭空建模（避免给出可设密码的错误暗示）。
        JsonNamesOf<MpAddKfAccountRequest>().Should().NotContain("password");
    }

    /// <summary>契约守卫 KF6：模块枚举 / 注册入口 / Query 令牌白名单（本域只断言「在内」）。</summary>
    [Fact]
    public void KfModules_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain(new[] { "KfAccount", "KfSession" });
        typeof(MpServiceBuilder).GetMethod("AddKfAccountApi").Should().NotBeNull();
        typeof(MpServiceBuilder).GetMethod("AddKfSessionApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();

        queryInterfaces.Should().Contain(new[] { nameof(IMpKfAccountService), nameof(IMpKfSessionService) },
            "本域必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
    }

    /// <summary>契约守卫 KF7：客服域错误码常量锁定。</summary>
    [Fact]
    public void KfErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.InvalidFileType.Should().Be(40005);
        MpErrorCodes.InvalidKfAccount.Should().Be(65401);
        MpErrorCodes.KfAccountNotBound.Should().Be(65402);
        MpErrorCodes.IllegalKfNickname.Should().Be(65403);
        MpErrorCodes.IllegalKfAccount.Should().Be(65404);
        MpErrorCodes.KfAccountCountExceeded.Should().Be(65405);
        MpErrorCodes.KfAccountExists.Should().Be(65406);
        MpErrorCodes.InviteeAlreadyWorker.Should().Be(65407);
        MpErrorCodes.InviteeAlreadyInvited.Should().Be(65408);
        MpErrorCodes.InvalidWeChatId.Should().Be(65409);
        MpErrorCodes.InviteeBindingLimitReached.Should().Be(65410);
        MpErrorCodes.PendingInvitationExists.Should().Be(65411);
        MpErrorCodes.KfAccountAlreadyBound.Should().Be(65412);
        MpErrorCodes.NoEffectiveSession.Should().Be(65413);
        MpErrorCodes.CustomerServedByAnother.Should().Be(65414);
        MpErrorCodes.WorkerNotOnline.Should().Be(65415);
    }

    /// <summary>契约守卫 KF8：官方数量 / 形状常量锁定（它们是调用方前置校验依据）。</summary>
    [Fact]
    public void KfLimitConstants_ShouldMatchOfficialValues()
    {
        MpAddKfAccountRequest.AccountPrefixMaxLength.Should().Be(10);
        MpAddKfAccountRequest.AccountSuffixMaxLength.Should().Be(30);
        MpAddKfAccountRequest.NicknameMaxLength.Should().Be(16);
        MpAddKfAccountRequest.MaxKfAccountCount.Should().Be(100);
        MpKfOnlineStatus.Offline.Should().Be(0);
        MpKfOnlineStatus.WebOnline.Should().Be(1);
    }

    /// <summary>
    /// 契约守卫 KF9：常量类不得落在<b>域级</b>数据模型命名空间（否则按命名空间整体扫描的 DTO 登记守卫会误报 / 漏放）。
    /// </summary>
    /// <remarks>
    /// 扫描面刻意收窄为「<c>DataModels.&lt;域段&gt;</c>」的<b>顶层、非编译器生成</b>类型：
    /// 根命名空间 <c>DataModels</c> 还容纳 <c>MpResponse</c> 与生成器产出的 <c>*JsonContext</c>
    /// （含其闭包显示类），若不放宽过滤会对生成产物误报。
    /// </remarks>
    [Fact]
    public void ConstantClasses_ShouldNotLiveInDataModelNamespaces()
    {
        const string rootNamespace = "Mud.Wechat.OfficialAccount.DataModels.";

        var offenders = typeof(MpKfAccount).Assembly.GetTypes()
            .Where(t => t.IsClass && t.IsSealed && !t.IsNested)
            .Where(t => !t.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false))
            .Where(t => t.Namespace != null
                        && t.Namespace.StartsWith(rootNamespace, StringComparison.Ordinal)
                        && t.Namespace.IndexOf('.', rootNamespace.Length) < 0)
            .Where(t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Where(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Any(f => f.IsLiteral) == true)
            .Where(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .All(f => f.IsLiteral))
            .Select(t => t.FullName)
            .ToList();

        offenders.Should().BeEmpty(
            "纯常量类须落在 Abstractions（如 MpKfOnlineStatus），域级数据模型命名空间仅容纳带 [HttpJsonSerializable] 的 DTO");
    }

    private static void AssertDomainRegistered<TAnchor>(
        string expectedGroupName,
        JsonSerializerContext context,
        int expectedCount,
        string reason)
    {
        var domainTypes = typeof(TAnchor).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == typeof(TAnchor).Namespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(expectedCount,
            $"{expectedGroupName} 域契约面类型数漂移须先核对官方文档再同批调整本守卫{reason}");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull($"{type.Name} 必须登记进 {expectedGroupName}JsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(expectedGroupName);
        }
    }

    private static List<string> QueryNamesOf<TInterface>(string methodName)
        => typeof(TInterface).GetMethod(methodName)!
            .GetParameters()
            .SelectMany(p => p.GetCustomAttributes<QueryAttribute>())
            .Select(a => a.Name!)
            .ToList();

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();
}
