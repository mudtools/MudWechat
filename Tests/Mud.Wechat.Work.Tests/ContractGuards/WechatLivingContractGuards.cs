// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Living;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 直播模块（Living 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 直播管理域形态：官方对三类应用开放完全一致的 9 个端点（创建预约直播 / 修改预约直播 / 取消预约直播 /
/// 删除直播回放 / 获取微信观看直播凭证 / 获取成员直播 ID 列表 / 获取直播详情 / 获取直播观看明细 /
/// 获取跳转小程序商城的直播观众信息），收敛声明于公共父接口；三个应用类型子接口均为零差异端点空标记。
/// </para>
/// <para>
/// 官方契约陷阱（勿「顺手修正」）：获取直播详情官方即 <b>GET</b>（livingid 走 Query），其余 8 个端点官方即 POST
/// （含仅查询语义的 get_user_all_livingid / get_watch_stat / get_living_share_info）；
/// 获取成员直播 ID 列表与删除直播回放两条路由官方在直播与「家校沟通·上课直播域」（School 模块）两处分文档
/// 承载同一端点——本域按直播文档页独立声明接口与 DTO（请求形态同构：userid/cursor/limit 与 livingid），
/// 两域同路由不互斥；获取成员直播 ID 列表以 <c>next_cursor</c> 分页（返回空字符串代表已是最后一页），
/// 获取直播观看明细以 <c>next_key</c> 分页且是否拉完只能依据 <c>ending</c>（0 还需拉取 / 1 已拉完）判断，
/// 两种翻页形态并存勿统一；直播 ID 官方字段名作 <c>livingid</c>（无下划线）；邀请人字段官方拼写作
/// <c>invitor_userid</c> / <c>invitor_external_userid</c>（非 inviter，照抄勿纠正）；
/// 获取微信观看直播凭证的 <c>living_code</c> 5 分钟内可重复使用且仅能在微信上使用；
/// 获取跳转小程序商城的直播观众信息以 <c>ww_share_code</c> 换取观众/邀请人身份（五分钟内有效，
/// 跳转的小程序需与企业有绑定关系）；创建预约直播的直播类型大班课/小班课仅 k12 学校和 IT 行业能发起，
/// 活动直播附图最多 5 张（超过取前五张）。
/// </para>
/// </summary>
public class WechatLivingContractGuards
{
    private const string LivingRegistryGroupName = "Living";

    // ------------------------------------------------------------------
    // 路由表：9 条官方路由全部挂 /cgi-bin/living/ 段；get_living_info 官方即 GET（勿改 POST），
    // 其余 8 条官方即 POST（勿「顺手统一」为 GET）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        LivingRoutes =
        {
            // 创建预约直播（自建 93637 / 第三方 93717 / 代开发 96837；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.CreateLivingAsync),
                typeof(PostAttribute), "/cgi-bin/living/create"),
            // 修改预约直播（自建 93640 / 第三方 93720 / 代开发 96839；仅预约状态可修改）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.ModifyLivingAsync),
                typeof(PostAttribute), "/cgi-bin/living/modify"),
            // 取消预约直播（自建 93638 / 第三方 93718 / 代开发 96838；仅预约状态可取消）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.CancelLivingAsync),
                typeof(PostAttribute), "/cgi-bin/living/cancel"),
            // 删除直播回放（自建 93874 / 第三方 93719 / 代开发 96841；仅允许删除当前应用自己创建的直播；
            // 与家校沟通·上课直播域共用路由）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.DeleteReplayDataAsync),
                typeof(PostAttribute), "/cgi-bin/living/delete_replay_data"),
            // 获取微信观看直播凭证（自建 93641 / 第三方 93721 / 代开发 96840；living_code 5 分钟内可重复使用）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.GetLivingCodeAsync),
                typeof(PostAttribute), "/cgi-bin/living/get_living_code"),
            // 获取成员直播 ID 列表（自建 93634 / 第三方 93714 / 代开发 96834；官方即 POST；
            // 与家校沟通·上课直播域共用路由；next_cursor 分页）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.GetUserAllLivingIdAsync),
                typeof(PostAttribute), "/cgi-bin/living/get_user_all_livingid"),
            // 获取直播详情（自建 93635 / 第三方 93715 / 代开发 96835；官方即 GET，livingid 走 Query——勿改 POST）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.GetLivingInfoAsync),
                typeof(GetAttribute), "/cgi-bin/living/get_living_info"),
            // 获取直播观看明细（自建 93636 / 第三方 93716 / 代开发 96836；官方即 POST；
            // next_key + ending 分页，拉完判定只能依据 ending）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.GetWatchStatAsync),
                typeof(PostAttribute), "/cgi-bin/living/get_watch_stat"),
            // 获取跳转小程序商城的直播观众信息（自建 94442 / 第三方 94578 / 代开发 96843；官方即 POST）。
            (typeof(IWechatWorkLivingService),
                nameof(IWechatWorkLivingService.GetLivingShareInfoAsync),
                typeof(PostAttribute), "/cgi-bin/living/get_living_share_info"),
        };

    // ------------------------------------------------------------------
    // LV1：全部端点路由与官方契约一致（9 条路由，全部挂 /cgi-bin/living/ 段，
    // get_living_info 官方即 GET、其余官方即 POST）。
    // ------------------------------------------------------------------

    [Fact]
    public void LivingEndpoints_ShouldMatchOfficialRoutes()
    {
        LivingRoutes.Should().HaveCount(9, "直播管理域 = 创建 + 修改 + 取消 + 删除回放 + 微信观看凭证 + 成员直播 ID 列表 + 直播详情 + 观看明细 + 小程序商城观众信息 9 端点");
        LivingRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/living/", StringComparison.Ordinal), "直播域全部路由位于 /cgi-bin/living/ 段");

        // 获取直播详情官方即 GET（livingid 走 Query）——与家校沟通·上课直播域同形态，勿统一为 POST。
        LivingRoutes.Single(r => r.Route == "/cgi-bin/living/get_living_info").HttpAttribute
            .Should().Be(typeof(GetAttribute), "获取直播详情官方即 GET（livingid 走 Query）");
        LivingRoutes.Count(r => r.HttpAttribute == typeof(PostAttribute))
            .Should().Be(8, "其余 8 个端点官方即 POST（含仅查询语义的 get_user_all_livingid / get_watch_stat / get_living_share_info）");

        AssertRoutes(LivingRoutes);

        // 全部官方路由去重清单锁定。
        LivingRoutes.Select(r => r.Route).Distinct().Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/living/create",
            "/cgi-bin/living/modify",
            "/cgi-bin/living/cancel",
            "/cgi-bin/living/delete_replay_data",
            "/cgi-bin/living/get_living_code",
            "/cgi-bin/living/get_user_all_livingid",
            "/cgi-bin/living/get_living_info",
            "/cgi-bin/living/get_watch_stat",
            "/cgi-bin/living/get_living_share_info",
        }, "直播域全部官方路由须与官方文档一一对应");

        // 无业务负载端点：响应直接用 WechatWorkResponse，不得新建空响应 DTO
        //（修改/取消预约直播与删除直播回放仅返回 errcode/errmsg）。
        typeof(IWechatWorkLivingService)
            .GetMethod(nameof(IWechatWorkLivingService.ModifyLivingAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "修改预约直播仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkLivingService)
            .GetMethod(nameof(IWechatWorkLivingService.CancelLivingAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "取消预约直播仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkLivingService)
            .GetMethod(nameof(IWechatWorkLivingService.DeleteReplayDataAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "删除直播回放仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");

        // 获取直播详情的 livingid 为官方 Query 参数，必须以 [Query("livingid")] 标注。
        typeof(IWechatWorkLivingService)
            .GetMethod(nameof(IWechatWorkLivingService.GetLivingInfoAsync), BindingFlags.Public | BindingFlags.Instance)!
            .GetParameters().Single(p => p.GetCustomAttribute<QueryAttribute>() != null)
            .GetCustomAttribute<QueryAttribute>()!.Name.Should().Be("livingid",
                "获取直播详情 livingid 官方走 Query 传参");
    }

    // ------------------------------------------------------------------
    // LV2：接口层级与生成器注册形态（公共父 9 端点 + 三个零端点空标记子接口）。
    // ------------------------------------------------------------------

    [Fact]
    public void LivingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 直播管理族：官方对三类应用开放完全一致的 9 端点，公共父收敛 + 三个空标记子接口。
        AssertFamily(
            parent: typeof(IWechatWorkLivingService),
            parentImplementation: "WechatWorkLivingService",
            parentDeclaredEndpointCount: 9,
            new[]
            {
                (typeof(IWechatWorkInternalLivingService), 0),
                (typeof(IWechatWorkProviderLivingService), 0),
                (typeof(IWechatWorkThirdPartyLivingService), 0),
            });
    }

    /// <summary>族断言：父接口 IsAbstract + 指定端点数，子接口集合不漂移 + 指定端点数 + 注册组/继承契约。</summary>
    private static void AssertFamily(
        Type parent,
        string parentImplementation,
        int parentDeclaredEndpointCount,
        (Type Interface, int DeclaredEndpointCount)[] children)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(parentDeclaredEndpointCount, $"{parent.Name} 承载官方开放面收敛的端点数");

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
        assignable.Should().BeEquivalentTo(children.Select(c => c.Interface),
            $"{parent.Name} 继承链子接口集合不得漂移（官方未开放的应用类型不得补子接口）");

        foreach (var (child, endpointCount) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(LivingRegistryGroupName,
                $"{child.Name} 必须挂 {LivingRegistryGroupName} 注册组（Living 模块共用 Add{LivingRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 承载官方开放面差异端点数");
        }
    }

    // ------------------------------------------------------------------
    // LV3：令牌绑定——直播域 4 个接口统一 AccessToken 路由键 + Query 注入
    //（归属域键由 WechatTokenOwnerContractGuards 全局锁定，此处不重复）。
    // ------------------------------------------------------------------

    [Fact]
    public void LivingTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkLivingService),
            typeof(IWechatWorkInternalLivingService),
            typeof(IWechatWorkProviderLivingService),
            typeof(IWechatWorkThirdPartyLivingService),
        };

        accessTokenInterfaces.Should().HaveCount(4, "直播域 = 公共父接口 + 三个应用类型空标记子接口");

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    // ------------------------------------------------------------------
    // LV4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Living；
    // LivingJsonContext 为多命名空间分组——家校沟通·上课直播域 25 型与本域 19 型共用同一上下文，
    // 上下文命名空间按字母序落在 DataModels.Living，见 GenerateJsonContext.ps1 头注）。
    // ------------------------------------------------------------------

    [Fact]
    public void LivingDataModels_ShouldBeRegisteredInJsonContext()
    {
        var livingContext = LivingJsonContext.Default;

        var domainTypes = typeof(CreateLivingRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Living"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 LivingJsonContext 且 SerializerClassName 统一为 Living
        //（生成物 LivingJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(19,
            "直播模块契约面类型数漂移须先核对官方文档再同批调整本守卫（创建 3：请求/响应/活动详情嵌套；修改 1；取消 1；删除回放 1；微信观看凭证 2；成员直播 ID 列表 2；直播详情 2：响应/信息嵌套；观看明细 5：请求/响应/统计容器/成员明细/外部成员明细；小程序商城观众信息 2）");

        foreach (var type in domainTypes)
        {
            livingContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于直播域命名空间，必须登记进 LivingJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Living",
                $"{type.Name} 的 SerializerClassName 必须为直播域段 Living");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            typeof(CreateLivingRequest), typeof(CreateLivingResponse), typeof(LivingActivityDetail),
            typeof(ModifyLivingRequest),
            typeof(CancelLivingRequest),
            typeof(DeleteLivingReplayDataRequest),
            typeof(GetLivingCodeRequest), typeof(GetLivingCodeResponse),
            typeof(GetUserAllLivingIdRequest), typeof(GetUserAllLivingIdResponse),
            typeof(GetLivingInfoResponse), typeof(LivingDetail),
            typeof(GetLivingWatchStatRequest), typeof(GetLivingWatchStatResponse),
            typeof(LivingWatchStatInfo), typeof(LivingWatchStatUser), typeof(LivingWatchStatExternalUser),
            typeof(GetLivingShareInfoRequest), typeof(GetLivingShareInfoResponse),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于直播域命名空间");
    }

    // ------------------------------------------------------------------
    // LV5：官方契约陷阱锁定——GET 详情、翻页形态并存与字段名照抄（livingid/invitor 拼写等）。
    // ------------------------------------------------------------------

    [Fact]
    public void LivingDataModels_ShouldLockOfficialContractTraps()
    {
        // 直播 ID 官方字段名作 livingid（无下划线），列表作 livingid_list——照抄勿「顺手修正」。
        JsonNameShouldBe(typeof(CreateLivingResponse), nameof(CreateLivingResponse.Livingid), "livingid");
        JsonNameShouldBe(typeof(ModifyLivingRequest), nameof(ModifyLivingRequest.Livingid), "livingid");
        JsonNameShouldBe(typeof(CancelLivingRequest), nameof(CancelLivingRequest.Livingid), "livingid");
        JsonNameShouldBe(typeof(DeleteLivingReplayDataRequest), nameof(DeleteLivingReplayDataRequest.Livingid), "livingid");
        JsonNameShouldBe(typeof(GetLivingCodeRequest), nameof(GetLivingCodeRequest.Livingid), "livingid");
        JsonNameShouldBe(typeof(GetUserAllLivingIdResponse), nameof(GetUserAllLivingIdResponse.LivingidList), "livingid_list");

        // 获取成员直播 ID 列表：next_cursor 分页（返回空字符串代表已是最后一页），limit 默认值和最大值都为 100。
        JsonNameShouldBe(typeof(GetUserAllLivingIdRequest), nameof(GetUserAllLivingIdRequest.Userid), "userid");
        JsonNameShouldBe(typeof(GetUserAllLivingIdRequest), nameof(GetUserAllLivingIdRequest.Cursor), "cursor");
        JsonNameShouldBe(typeof(GetUserAllLivingIdRequest), nameof(GetUserAllLivingIdRequest.Limit), "limit");
        JsonNameShouldBe(typeof(GetUserAllLivingIdResponse), nameof(GetUserAllLivingIdResponse.NextCursor), "next_cursor");

        // 微信观看直播凭证：living_code 5 分钟内可重复使用且仅能在微信上使用。
        JsonNameShouldBe(typeof(GetLivingCodeRequest), nameof(GetLivingCodeRequest.Openid), "openid");
        JsonNameShouldBe(typeof(GetLivingCodeResponse), nameof(GetLivingCodeResponse.LivingCode), "living_code");

        // 获取直播详情：living_info 包裹；回放状态仅 open_replay 为 1 时返回；推流地址仅活动直播待开播时返回。
        //（详情嵌套类型命名 LivingDetail：与家校沟通·上课直播域 School.Living.LivingInfo 同源不同构，
        // 且源生成上下文按类型简单名生成元数据、同上下文禁止同名根类型，命名消歧。）
        JsonNameShouldBe(typeof(GetLivingInfoResponse), nameof(GetLivingInfoResponse.LivingInfo), "living_info");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.AnchorUserid), "anchor_userid");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.MainDepartment), "main_department");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.ViewerNum), "viewer_num");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.CommentNum), "comment_num");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.MicNum), "mic_num");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.OpenReplay), "open_replay");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.ReplayStatus), "replay_status");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.ReserveLivingDuration), "reserve_living_duration");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.PushStreamUrl), "push_stream_url");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.OnlineCount), "online_count");
        JsonNameShouldBe(typeof(LivingDetail), nameof(LivingDetail.SubscribeCount), "subscribe_count");

        // 获取直播观看明细：next_key + ending 分页（与成员直播 ID 列表的 next_cursor 形态并存，勿统一）。
        JsonNameShouldBe(typeof(GetLivingWatchStatRequest), nameof(GetLivingWatchStatRequest.NextKey), "next_key");
        JsonNameShouldBe(typeof(GetLivingWatchStatResponse), nameof(GetLivingWatchStatResponse.Ending), "ending");
        JsonNameShouldBe(typeof(GetLivingWatchStatResponse), nameof(GetLivingWatchStatResponse.NextKey), "next_key");
        JsonNameShouldBe(typeof(GetLivingWatchStatResponse), nameof(GetLivingWatchStatResponse.StatInfo), "stat_info");
        JsonNameShouldBe(typeof(LivingWatchStatInfo), nameof(LivingWatchStatInfo.Users), "users");
        JsonNameShouldBe(typeof(LivingWatchStatInfo), nameof(LivingWatchStatInfo.ExternalUsers), "external_users");

        // 观看明细成员/外部成员字段名照抄官方原文（邀请人官方拼写 invitor 非 inviter；仅「推广产品」直播支持）。
        JsonNameShouldBe(typeof(LivingWatchStatUser), nameof(LivingWatchStatUser.Userid), "userid");
        JsonNameShouldBe(typeof(LivingWatchStatUser), nameof(LivingWatchStatUser.WatchTime), "watch_time");
        JsonNameShouldBe(typeof(LivingWatchStatUser), nameof(LivingWatchStatUser.IsComment), "is_comment");
        JsonNameShouldBe(typeof(LivingWatchStatUser), nameof(LivingWatchStatUser.IsMic), "is_mic");
        JsonNameShouldBe(typeof(LivingWatchStatUser), nameof(LivingWatchStatUser.InvitorUserid), "invitor_userid");
        JsonNameShouldBe(typeof(LivingWatchStatUser), nameof(LivingWatchStatUser.InvitorExternalUserid), "invitor_external_userid");
        JsonNameShouldBe(typeof(LivingWatchStatExternalUser), nameof(LivingWatchStatExternalUser.ExternalUserid), "external_userid");
        JsonNameShouldBe(typeof(LivingWatchStatExternalUser), nameof(LivingWatchStatExternalUser.Name), "name");
        JsonNameShouldBe(typeof(LivingWatchStatExternalUser), nameof(LivingWatchStatExternalUser.InvitorUserid), "invitor_userid");
        JsonNameShouldBe(typeof(LivingWatchStatExternalUser), nameof(LivingWatchStatExternalUser.InvitorExternalUserid), "invitor_external_userid");

        // 获取跳转小程序商城的直播观众信息：ww_share_code 换取观众/邀请人身份（官方 ww 前缀照抄）。
        JsonNameShouldBe(typeof(GetLivingShareInfoRequest), nameof(GetLivingShareInfoRequest.WwShareCode), "ww_share_code");
        JsonNameShouldBe(typeof(GetLivingShareInfoResponse), nameof(GetLivingShareInfoResponse.Livingid), "livingid");
        JsonNameShouldBe(typeof(GetLivingShareInfoResponse), nameof(GetLivingShareInfoResponse.ViewerUserid), "viewer_userid");
        JsonNameShouldBe(typeof(GetLivingShareInfoResponse), nameof(GetLivingShareInfoResponse.ViewerExternalUserid), "viewer_external_userid");
        JsonNameShouldBe(typeof(GetLivingShareInfoResponse), nameof(GetLivingShareInfoResponse.InvitorUserid), "invitor_userid");
        JsonNameShouldBe(typeof(GetLivingShareInfoResponse), nameof(GetLivingShareInfoResponse.InvitorExternalUserid), "invitor_external_userid");

        // 创建预约直播：活动直播特定参数字段名照抄（mediaid 官方无下划线分隔 cover/share）。
        JsonNameShouldBe(typeof(CreateLivingRequest), nameof(CreateLivingRequest.AnchorUserid), "anchor_userid");
        JsonNameShouldBe(typeof(CreateLivingRequest), nameof(CreateLivingRequest.LivingStart), "living_start");
        JsonNameShouldBe(typeof(CreateLivingRequest), nameof(CreateLivingRequest.LivingDuration), "living_duration");
        JsonNameShouldBe(typeof(CreateLivingRequest), nameof(CreateLivingRequest.RemindTime), "remind_time");
        JsonNameShouldBe(typeof(CreateLivingRequest), nameof(CreateLivingRequest.ActivityCoverMediaid), "activity_cover_mediaid");
        JsonNameShouldBe(typeof(CreateLivingRequest), nameof(CreateLivingRequest.ActivityShareMediaid), "activity_share_mediaid");
        JsonNameShouldBe(typeof(CreateLivingRequest), nameof(CreateLivingRequest.ActivityDetail), "activity_detail");
        JsonNameShouldBe(typeof(LivingActivityDetail), nameof(LivingActivityDetail.ImageList), "image_list");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, Type HttpAttribute, string Route)[] routes)
    {
        foreach (var (iface, method, httpAttribute, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致（拼写差异属官方契约）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
