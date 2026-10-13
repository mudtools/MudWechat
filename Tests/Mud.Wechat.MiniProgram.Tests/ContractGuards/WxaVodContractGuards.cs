// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.Vod;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「短剧媒资」域契约守卫（DV 系列：32 端点路由与动词 + 令牌绑定 + 参数形态 +
/// DTO 登记计数与 JSON 上下文登记 + 字段抽样与共用 DTO 裁决 + 官方反直觉点锁定）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-13 逐页取证，<c>OpenApiDoc/minidrama</c> 分区全集 32 端点）</b>：
/// 上传 6（<see cref="IWxaVodUploadService"/>）+ 媒资 4（<see cref="IWxaVodMediaService"/>）+
/// 剧目 8（<see cref="IWxaVodDramaService"/>）+ CDN 与资源包 3（<see cref="IWxaVodCdnService"/>）+
/// 授权与版权 11（<see cref="IWxaVodAuthorizationService"/>）。
/// </para>
/// <para>
/// <b>与「短剧 25 条」旧口径的关系</b>：缺口清单曾按第三方转述把本域记为 25 端点。逐页取证结果为 32
/// （分片上传三支与版权授权五支在转述里缺失或合并；setplayerdramarecmdswitch 为 2026-10-13 新增落地）⇒ 以本守卫的路由表为准，清单侧数字已同批更正。
/// </para>
/// <para>
/// <b>本域最需要防误读的一处</b>：<c>authorizedrama</c> 官方「调用方式」写作 <b>GET</b> 且请求示例是裸
/// JSON 对象（全 31 页里唯一的一支 GET），而同页错误码表又列有 <c>43002</c>「HTTP 请求必须使用 POST 方法」
/// ——页面自相矛盾。本 SDK 照「调用方式 + 示例」落地为 GET + <c>[Body]</c>，动词由 DV2 锁定；
/// 若将来官方修正为 POST，须连同该裁决注释与守卫一起改，不得单边改代码。
/// </para>
/// <para>
/// <b>敏感值口径</b>：CDN 日志（<c>getcdnlogs</c>）含客户端 IP，媒资播放地址（<c>getmedialink</c> /
/// <c>getmedia</c> 的 <c>mp4_url</c> / <c>hls_url</c>）为带签名的临时地址 ⇒ 两者均禁止写入日志、
/// 遥测或异常消息（与 <c>permanent_code</c> 同级别处置）。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）、端点聚合计数
/// （MP-X5 / ContractBaseline）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaVodContractGuards
{
    private const string RegistryGroupName = "Vod";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.Vod";

    private const string RoutePrefix = "/wxa/sec/vod/";

    /// <summary>本域唯一非 /wxa/sec/vod/ 前缀的官方路由（播放器推荐开关独立族）。</summary>
    private const string ExceptionRoute = "/wxadrama/setplayerdramarecmdswitch";

    /// <summary>
    /// 短剧媒资域官方路由表（32 端点：上传 6 + 媒资 4 + 剧目 8 + CDN 3 + 授权 11；路由与动词照官方原文）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.SingleFileUploadAsync), typeof(PostAttribute), "/wxa/sec/vod/singlefileupload"),
        (typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.PullUploadAsync), typeof(PostAttribute), "/wxa/sec/vod/pullupload"),
        (typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.GetTaskAsync), typeof(PostAttribute), "/wxa/sec/vod/gettask"),
        (typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.ApplyUploadAsync), typeof(PostAttribute), "/wxa/sec/vod/applyupload"),
        (typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.UploadPartAsync), typeof(PostAttribute), "/wxa/sec/vod/uploadpart"),
        (typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.CommitUploadAsync), typeof(PostAttribute), "/wxa/sec/vod/commitupload"),

        (typeof(IWxaVodMediaService), nameof(IWxaVodMediaService.ListMediaAsync), typeof(PostAttribute), "/wxa/sec/vod/listmedia"),
        (typeof(IWxaVodMediaService), nameof(IWxaVodMediaService.GetMediaAsync), typeof(PostAttribute), "/wxa/sec/vod/getmedia"),
        (typeof(IWxaVodMediaService), nameof(IWxaVodMediaService.GetMediaLinkAsync), typeof(PostAttribute), "/wxa/sec/vod/getmedialink"),
        (typeof(IWxaVodMediaService), nameof(IWxaVodMediaService.DeleteMediaAsync), typeof(PostAttribute), "/wxa/sec/vod/deletemedia"),

        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.AuditDramaAsync), typeof(PostAttribute), "/wxa/sec/vod/auditdrama"),
        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.ListDramasAsync), typeof(PostAttribute), "/wxa/sec/vod/listdramas"),
        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.GetDramaAsync), typeof(PostAttribute), "/wxa/sec/vod/getdrama"),
        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.SubmitReplaceDramaMediasAsync), typeof(PostAttribute), "/wxa/sec/vod/submitreplacedramamedias"),
        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.ReplaceDramaMediaAsync), typeof(PostAttribute), "/wxa/sec/vod/replacedramamedia"),
        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.ModifyDramaBasicInfoAsync), typeof(PostAttribute), "/wxa/sec/vod/modifydramabasicinfo"),
        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.GetDramaLatestAuditInfoAsync), typeof(PostAttribute), "/wxa/sec/vod/getdramalatestauditinfo"),
        (typeof(IWxaVodDramaService), nameof(IWxaVodDramaService.SetPlayerDramaRecmdSwitchAsync), typeof(PostAttribute), "/wxadrama/setplayerdramarecmdswitch"),

        (typeof(IWxaVodCdnService), nameof(IWxaVodCdnService.GetCdnUsageDataAsync), typeof(PostAttribute), "/wxa/sec/vod/getcdnusagedata"),
        (typeof(IWxaVodCdnService), nameof(IWxaVodCdnService.GetCdnLogsAsync), typeof(PostAttribute), "/wxa/sec/vod/getcdnlogs"),
        (typeof(IWxaVodCdnService), nameof(IWxaVodCdnService.ListPackagesAsync), typeof(PostAttribute), "/wxa/sec/vod/listpackages"),

        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.GetAuthorizedObjectsAsync), typeof(PostAttribute), "/wxa/sec/vod/getauthorizedobjects"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.AuthorizeDramaAsync), typeof(GetAttribute), "/wxa/sec/vod/authorizedrama"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.DeauthorizeDramaAsync), typeof(PostAttribute), "/wxa/sec/vod/deauthorizedrama"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.GetAuthorizeObjectsAsync), typeof(PostAttribute), "/wxa/sec/vod/getauthorizeobjects"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.AuthorizeAppAsync), typeof(PostAttribute), "/wxa/sec/vod/authorizeapp"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.DeauthorizeAppAsync), typeof(PostAttribute), "/wxa/sec/vod/deauthorizeapp"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.GetAuthorizeAppsAsync), typeof(PostAttribute), "/wxa/sec/vod/getauthorizeapps"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.AuthorizeCopyrightAsync), typeof(PostAttribute), "/wxa/sec/vod/authorizecopyright"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.DeauthorizeCopyrightAsync), typeof(PostAttribute), "/wxa/sec/vod/deauthorizecopyright"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.GetCopyrightAuthorizationListAsync), typeof(PostAttribute), "/wxa/sec/vod/getcopyrightauthorizationlist"),
        (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.GetCopyrightAuthorizedListAsync), typeof(PostAttribute), "/wxa/sec/vod/getcopyrightauthorizedlist"),
    };

    private static readonly Type[] DomainInterfaces =
    {
        typeof(IWxaVodUploadService),
        typeof(IWxaVodMediaService),
        typeof(IWxaVodDramaService),
        typeof(IWxaVodCdnService),
        typeof(IWxaVodAuthorizationService),
    };

    /// <summary>官方为 multipart/form-data 的两支上传端点（无 JSON 请求体 DTO）。</summary>
    private static readonly string[] MultipartMethods =
    {
        nameof(IWxaVodUploadService.SingleFileUploadAsync),
        nameof(IWxaVodUploadService.UploadPartAsync),
    };

    /// <summary>官方请求体写明「无」的端点（零业务参数）。</summary>
    private static readonly string[] ZeroParamMethods =
    {
        nameof(IWxaVodAuthorizationService.GetAuthorizeAppsAsync),
    };

    /// <summary>契约守卫 DV1：32 端点路由与动词必须与官方契约一致（本域只有一个官方路由族）。</summary>
    [Fact]
    public void VodEndpoints_ShouldMatchOfficialRoutes()
    {
        // 32 端点 = 上传 6 + 媒资 4 + 剧目 8 + CDN 3 + 授权 11（官方 OpenApiDoc/minidrama 分区 2026-10-13 逐页取证全集）。
        Routes.Should().HaveCount(32,
            "短剧媒资域官方开放面为 32 端点（官方文档逐页核验：upload 6 + media 4 + drama 8 + cdn 3 + authorization 11）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(32, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文拼写，禁止改写）");

            // 32 端点里 31 支落在 /wxa/sec/vod/ 族内，仅 setplayerdramarecmdswitch 为例外（独立播放器族）。
            if (route == ExceptionRoute)
            {
                route.Should().StartWith("/wxadrama/", "本域唯一非 /wxa/sec/vod/ 前缀的官方路由（播放器推荐开关）");
            }
            else
            {
                route.Should().StartWith(RoutePrefix, "本域官方路由全部落在 /wxa/sec/vod/ 单一族内（无 business/api 前缀混入）");
            }
        }

        // 反直觉点锁定 1：全 31 条路由一律全小写连写（本域没有驼峰端点），且「单复数」逐页不同：
        // submitreplacedramamedias（复数）与 replacedramamedia（单数）是两支不同端点。
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "本域官方路由一律全小写（不得顺手改成驼峰）");
        Routes.Select(static r => r.Route).Should()
            .Contain("/wxa/sec/vod/submitreplacedramamedias")
            .And.Contain("/wxa/sec/vod/replacedramamedia",
                "复数的「提交替换」与单数的「替换」官方是两支端点，不得合并");

        // 反直觉点锁定 2：两支「查询授权」端点名称只差一处 ing 前的形态——
        // getauthorizedobjects（被授权方视角，过去式）与 getauthorizeobjects（授权方视角，原形），
        // 官方两页都存在且字段名集不同（authorized_appid vs authorizer_appid）⇒ 两条都在册、不得归一。
        Routes.Select(static r => r.Route).Should()
            .Contain("/wxa/sec/vod/getauthorizedobjects")
            .And.Contain("/wxa/sec/vod/getauthorizeobjects",
                "官方两视角查询端点同域并存（过去式 / 原形），拼写差异是官方原文而非笔误");

        // 反直觉点锁定 3：查询「已授权小程序列表」的官方拼写是 getauthorizeapps（原形 + apps），
        // 而查询授权对象两支才有 authorized/authorize 之分 ⇒ 「getauthorizedapps」这一顺理成章的拼法官方并不存在，
        // 零声明面由 MiniProgramNonContractRouteContractGuards 兜住，此处正向钉住正确落位。
        Routes.Should().NotContain(static r => r.Route == "/wxa/sec/vod/getauthorizedapps");

        // 动词分布：32 支里恰好 1 支 GET（authorizedrama），其余 31 支 POST。
        Routes.Count(static r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(1,
            "本域官方动词为 GET 的端点只有 authorizedrama 一支（官方调用方式行照录，DV2 锁其 GET+Body 形态）");
    }

    /// <summary>
    /// 契约守卫 DV2：参数形态 —— 本域业务参数一律走 JSON 请求体（零 <c>[Query]</c>），
    /// 两支上传走 multipart，<c>getauthorizeapps</c> 官方请求体为「无」⇒ 零业务参数。
    /// </summary>
    [Fact]
    public void VodEndpoints_ShouldRespectBodyMultipartAndZeroParamShapes()
    {
        foreach (var (iface, method, _, route) in Routes)
        {
            var target = iface.GetMethod(method)!;
            var parameters = target.GetParameters();

            // 令牌之外本域零 Query 参数：官方 31 页的请求示例一律是 JSON 对象（或 multipart 表单），
            // access_token 由接口级 [Token] 注入，不得落成方法参数（会与官方示例形态不符）。
            parameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null,
                $"{route} 本域无官方 Query 参数先例，业务参数须走 [Body] 或 [MultipartForm]");

            var bodyCount = parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null);
            var formCount = parameters.Count(static p => p.GetCustomAttribute<MultipartFormAttribute>() != null);

            if (MultipartMethods.Contains(method))
            {
                formCount.Should().Be(1, $"{route} 官方为 multipart/form-data 上传 ⇒ 必须恰有一个 [MultipartForm] 参数");
                bodyCount.Should().Be(0, $"{route} multipart 通路不得再挂 JSON 请求体");
                parameters.Single(static p => p.GetCustomAttribute<MultipartFormAttribute>() != null)
                    .ParameterType.Should().Be(typeof(IFormContent), $"{route} 表单载荷类型须为 IFormContent");
                continue;
            }

            if (ZeroParamMethods.Contains(method))
            {
                // 官方「查询已授权小程序列表」页请求体写明「无」⇒ 不得凭空造 request DTO。
                bodyCount.Should().Be(0, $"{route} 官方请求体为「无」⇒ 零业务参数（全量返回、无分页）");
                formCount.Should().Be(0, $"{route} 非 multipart");
                continue;
            }

            bodyCount.Should().Be(1, $"{route} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
            formCount.Should().Be(0, $"{route} 官方为 JSON 请求体，非 multipart");

            if (target.GetCustomAttribute<GetAttribute>() != null)
            {
                // authorizedrama：官方动词 GET + 裸 JSON 请求体（本仓唯一一支 GET+[Body]）。
                route.Should().Be("/wxa/sec/vod/authorizedrama",
                    "本域 GET+[Body] 只允许 authorizedrama 一支（官方调用方式与示例如此；改 POST 须连同裁决注释同批改）");
            }
        }

        Routes.Count(static r => !MultipartMethods.Contains(r.Method)
                                 && !ZeroParamMethods.Contains(r.Method)).Should().Be(29,
            "32 端点 = 29 支单 [Body] + 2 支 multipart + 1 支零业务参数");
    }

    /// <summary>契约守卫 DV3：五接口令牌绑定与注册组（复用公众号 <c>MpTokenTypes.AccessToken</c>，MP-X2/X3 同口径）。</summary>
    [Fact]
    public void VodInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup()
    {
        DomainInterfaces.Should().HaveCount(5, "短剧媒资域按官方子目录族分为五接口（合计 32 端点）");

        foreach (var iface in DomainInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方「服务器端调用」页令牌为 access_token，权限集 153）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（改绑小店令牌或新增类型会让单槽 Resolve 返回 null，errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3）");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 Vod 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");
        }

        // 五接口同组 ⇒ DI 侧由同一条生成入口装载；注册面漂移由 MiniProgramContractGuards 的装配用例兜住。
        DomainInterfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域五接口共用一个注册组（AddVodWebApiHttpClient）");

        // 端点归属自证：五接口的方法数之和必须等于路由表规模（防「加了接口忘了进路由表」）。
        DomainInterfaces
            .Select(static i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length)
            .Sum().Should().Be(Routes.Length, "五接口声明方法总数须与 DV1 路由表一致");

        // 逐接口规模锁定（官方子目录族划分，防「把某一族的端点错挂到另一族」）。
        MethodCount(typeof(IWxaVodUploadService)).Should().Be(6, "上传族 6（单文件 / 拉取 / 查任务 / 申请分片 / 传分片 / 确认上传）");
        MethodCount(typeof(IWxaVodMediaService)).Should().Be(4, "媒资族 4（列表 / 详情 / 播放地址 / 删除）");
        MethodCount(typeof(IWxaVodDramaService)).Should().Be(8, "剧目族 8（提审 / 列表 / 详情 / 提交替换 / 替换 / 改基础信息 / 查最新审核信息 / 播放器推荐开关）");
        MethodCount(typeof(IWxaVodCdnService)).Should().Be(3, "CDN 族 3（用量 / 日志 / 资源包列表）");
        MethodCount(typeof(IWxaVodAuthorizationService)).Should().Be(11, "授权族 11（剧目与版权双向授权 6 + 查询 3 + 应用授权 2）");

        static int MethodCount(Type type)
            => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
    }

    /// <summary>
    /// 契约守卫 DV4：官方字段名抽样 + 类型口径 + 共用 DTO 裁决 + 官方自相矛盾的改名裁决。
    /// </summary>
    [Fact]
    public void VodDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 上传族（官方 multipart 通路无请求 DTO，请求体字段照参数表 snake_case 原文）。
        AssertJsonProperty<WxaVodApplyUploadRequest>(nameof(WxaVodApplyUploadRequest.MediaType), "media_type", "官方原文拼写（值为 WxaVodMediaTypes 字符串常量）");
        AssertJsonProperty<WxaVodApplyUploadRequest>(nameof(WxaVodApplyUploadRequest.CoverType), "cover_type", "官方原文拼写（填了即表示本次还要传封面图）");
        AssertJsonProperty<WxaVodApplyUploadResponse>(nameof(WxaVodApplyUploadResponse.UploadId), "upload_id", "官方原文拼写（分片上传会话键）");
        AssertJsonProperty<WxaVodPartInfo>(nameof(WxaVodPartInfo.Etag), "etag", "官方原文全小写拼写（非 eTag）");
        AssertJsonProperty<WxaVodPartInfo>(nameof(WxaVodPartInfo.PartNumber), "part_number", "官方原文拼写（1~100）");
        AssertJsonProperty<WxaVodCommitUploadRequest>(nameof(WxaVodCommitUploadRequest.MediaPartInfos), "media_part_infos", "官方原文拼写（媒体分片清单）");
        AssertJsonProperty<WxaVodCommitUploadRequest>(nameof(WxaVodCommitUploadRequest.CoverPartInfos), "cover_part_infos", "官方原文拼写（封面分片清单，与媒体分片同元素类型）");
        AssertJsonProperty<WxaVodPullUploadRequest>(nameof(WxaVodPullUploadRequest.MediaUrl), "media_url", "官方原文拼写");
        AssertJsonProperty<WxaVodPullUploadResponse>(nameof(WxaVodPullUploadResponse.TaskId), "task_id", "官方原文拼写（异步任务 id，须走 gettask 轮询）");
        AssertJsonProperty<WxaVodTaskInfo>(nameof(WxaVodTaskInfo.Id), "id", "官方原文拼写（任务表内主键名为 id 而非 task_id）");

        // 任务级结果码不是接口级结果码：外层 WxaResponse 兜不住，必须在 task_info 上落字段。
        typeof(WxaVodTaskInfo).GetProperty(nameof(WxaVodTaskInfo.Errcode), BindingFlags.Public | BindingFlags.Instance)
            .Should().NotBeNull("官方 Res.task_info 子表含 errcode（任务失败时该值非零），基类 errcode 是接口级 ⇒ 必须单独承载");

        // 媒资族。
        AssertJsonProperty<WxaVodListMediaRequest>(nameof(WxaVodListMediaRequest.MediaName), "media_name", "官方原文拼写（% 通配模糊）");
        AssertJsonProperty<WxaVodListMediaRequest>(nameof(WxaVodListMediaRequest.MediaNameFuzzy), "media_name_fuzzy", "官方原文拼写（与 media_name 语义相反，两字段并存不得合并）");
        AssertJsonProperty<WxaVodGetMediaLinkRequest>(nameof(WxaVodGetMediaLinkRequest.T), "t", "官方原文单字母拼写（播放地址有效期，上限 2 小时）");
        AssertJsonProperty<WxaVodGetMediaLinkRequest>(nameof(WxaVodGetMediaLinkRequest.Us), "us", "官方原文单字母拼写（渠道标识，与 getcdnlogs 的文件路径列同源）");
        AssertJsonProperty<WxaVodGetMediaLinkRequest>(nameof(WxaVodGetMediaLinkRequest.Rlimit), "rlimit", "官方原文拼写（访问次数限制，上限 9）");
        AssertJsonProperty<WxaVodGetMediaLinkRequest>(nameof(WxaVodGetMediaLinkRequest.Whref), "whref", "官方原文拼写（Referer 白名单，逗号分隔且不带前缀协议）");
        AssertJsonProperty<WxaVodMediaInfo>(nameof(WxaVodMediaInfo.FileSize), "file_size", "官方原文拼写");
        AssertJsonProperty<WxaVodMediaInfo>(nameof(WxaVodMediaInfo.OriginalUrl), "original_url", "官方原文拼写");
        AssertJsonProperty<WxaVodMediaLinkInfo>(nameof(WxaVodMediaLinkInfo.HlsUrl), "hls_url", "官方原文拼写（带签名的临时地址 ⇒ 禁止进日志与遥测）");

        // 类型口径：官方 file_size 类型列为 string（带单位的可读串），照录不数值化。
        typeof(WxaVodMediaInfo).GetProperty(nameof(WxaVodMediaInfo.FileSize))!.PropertyType
            .Should().Be(typeof(string), "官方 file_size 类型列为 string ⇒ 不得建为 number");

        // 剧目族。
        AssertJsonProperty<WxaVodAuditDramaRequest>(nameof(WxaVodAuditDramaRequest.MediaIdList), "media_id_list", "官方原文拼写（媒资 id 数组）");
        AssertJsonProperty<WxaVodAuditDramaRequest>(nameof(WxaVodAuditDramaRequest.CostOfProduction), "cost_of_production", "官方原文拼写（制作成本，备案新规后上限口径见接口注释）");
        AssertJsonProperty<WxaVodAuditDramaRequest>(nameof(WxaVodAuditDramaRequest.AiSoftwareOwnershipProof), "ai_software_ownership_proof", "官方原文拼写（drama_type=3 数字真人侧字段）");
        AssertJsonProperty<WxaVodAuditDramaRequest>(nameof(WxaVodAuditDramaRequest.ActorList), "actor_list", "官方原文拼写（对象包装形态，非裸数组）");
        AssertJsonProperty<WxaVodActorList>(nameof(WxaVodActorList.Actor), "actor", "官方原文拼写（actor_list 的内层元素名是 actor）");
        AssertJsonProperty<WxaVodActor>(nameof(WxaVodActor.PhotoMaterialId), "photo_material_id", "官方原文拼写");
        AssertJsonProperty<WxaVodCopyright>(nameof(WxaVodCopyright.CopyrightVerification), "copyright_verification", "官方原文拼写");
        AssertJsonProperty<WxaVodReplaceMediaItem>(nameof(WxaVodReplaceMediaItem.New), "new", "官方原文元素名 new 是 C# 关键字 ⇒ 线格式靠 JsonPropertyName 钉回");
        AssertJsonProperty<WxaVodDramaInfo>(nameof(WxaVodDramaInfo.PromotionPoster), "promotion_poster", "官方原文拼写（应答侧是 url，请求侧是 *_material_id，两名不同源）");
        AssertJsonProperty<WxaVodListDramasResponse>(nameof(WxaVodListDramasResponse.DramaInfoList), "drama_info_list", "官方原文拼写");
        AssertJsonProperty<WxaVodMediaAuditDetail>(nameof(WxaVodMediaAuditDetail.EvidenceMaterialIdList), "evidence_material_id_list", "官方原文拼写（复核举证材料）");

        // copyright_verification 官方类型列为 string（取值见 WxaVodCopyrightVerifications 的字符串常量），
        // 而同页 copyright_role 为 number ⇒ 同子表内两字段类型口径不同，各自照录。
        typeof(WxaVodCopyright).GetProperty(nameof(WxaVodCopyright.CopyrightVerification))!.PropertyType
            .Should().Be(typeof(string), "官方 copyright_verification 类型列为 string（与 WxaVodCopyrightVerifications 常量集同为字符串口径）");
        typeof(WxaVodCopyright).GetProperty(nameof(WxaVodCopyright.CopyrightRole))!.PropertyType
            .Should().Be(typeof(long?), "官方 copyright_role 类型列为 number（与同子表的 verification 口径不同，不得统一）");

        // 被授权侧 drama_id 在授权写端点是数组，在查询端点是单值 number ⇒ 同名不同型，各自照录。
        typeof(WxaVodAuthorizeDramaRequest).GetProperty(nameof(WxaVodAuthorizeDramaRequest.DramaId))!.PropertyType
            .Should().Be(typeof(List<long>), "authorizedrama 官方 drama_id 为 numarray（一次授权一批剧目）");
        typeof(WxaVodGetAuthorizeObjectsRequest).GetProperty(nameof(WxaVodGetAuthorizeObjectsRequest.DramaId))!.PropertyType
            .Should().Be(typeof(long?), "getauthorizeobjects 官方 drama_id 为单值 number（与写侧数组口径不同）");

        // 授权族：官方参数表与官方示例自相矛盾的改名裁决（authorized → authorized_appid）。
        AssertJsonProperty<WxaVodAuthorizeDramaRequest>(nameof(WxaVodAuthorizeDramaRequest.AuthorizedAppid), "authorized_appid",
            "官方参数表列作 authorized，同页请求示例为 authorized_appid ⇒ 采信示例与域内一致性（本域其余被授权方字段名一律 authorized_appid）");
        typeof(WxaVodAuthorizeDramaRequest).GetProperty("Authorized", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("不得按官方参数表把字段名做回 authorized（改名裁决见接口与 DTO 注释，DV4 锁定）");
        AssertJsonProperty<WxaVodAuthorizeDramaRequest>(nameof(WxaVodAuthorizeDramaRequest.AuthzExpireTime), "authz_expire_time", "官方原文拼写（不传或 0 表示永久）");
        AssertJsonProperty<WxaVodAuthorizeCopyrightRequest>(nameof(WxaVodAuthorizeCopyrightRequest.ExpireTime), "expire_time", "官方原文拼写（版权侧到期键名为 expire_time，与剧目侧 authz_expire_time 不同源）");
        AssertJsonProperty<WxaVodAuthorizeCopyrightRequest>(nameof(WxaVodAuthorizeCopyrightRequest.AuthorizedSubjectCertNo), "authorized_subject_cert_no", "官方原文拼写（主体证件号）");
        AssertJsonProperty<WxaVodAuthorizedObject>(nameof(WxaVodAuthorizedObject.AuthorizerAppid), "authorizer_appid", "官方原文拼写（被授权方视角页里「授权方」的键名是 authorizer_appid）");
        AssertJsonProperty<WxaVodAuthorizeObject>(nameof(WxaVodAuthorizeObject.AuthorizedAppid), "authorized_appid", "官方原文拼写（授权方视角页里「被授权方」的键名是 authorized_appid）");
        AssertJsonProperty<WxaVodCopyrightAuthorization>(nameof(WxaVodCopyrightAuthorization.AuthorizerAppid), "authorizer_appid", "官方原文拼写（同一子表两视角键名并存）");
        AssertJsonProperty<WxaVodGetCdnLogsResponse>(nameof(WxaVodGetCdnLogsResponse.DomesticCdnLogs), "domestic_cdn_logs", "官方原文拼写（仅国内日志键）");
        AssertJsonProperty<WxaVodPackageInfo>(nameof(WxaVodPackageInfo.All), "all", "官方原文拼写（资源包总量字段名为 all）");

        // data_interval 官方参数表类型列写 string、示例给数字 ⇒ 采信示例按 number 建模（请求与应答同口径）。
        typeof(WxaVodGetCdnUsageDataRequest).GetProperty(nameof(WxaVodGetCdnUsageDataRequest.DataInterval))!.PropertyType
            .Should().Be(typeof(long?), "官方 getcdnusagedata 请求表 data_interval 类型列误写 string、示例为数字 ⇒ 按示例取 number");
        typeof(WxaVodGetCdnUsageDataResponse).GetProperty(nameof(WxaVodGetCdnUsageDataResponse.DataInterval))!.PropertyType
            .Should().Be(typeof(long?), "应答侧 data_interval 同口径（回显实际聚合粒度）");

        // 共用 DTO 裁决 1：getmedia 与 deletemedia 的官方请求体同为单字段 media_id ⇒ 同一类型。
        typeof(IWxaVodMediaService).GetMethod(nameof(IWxaVodMediaService.GetMediaAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaVodMediaIdRequest));
        typeof(IWxaVodMediaService).GetMethod(nameof(IWxaVodMediaService.DeleteMediaAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaVodMediaIdRequest),
            "getmedia 与 deletemedia 官方请求体同为单字段 media_id ⇒ 共用 WxaVodMediaIdRequest");

        // 共用 DTO 裁决 2：单文件上传与分片确认上传的应答同为 {errcode, errmsg, media_id} ⇒ 同一类型
        //（单文件上传请求侧无 DTO，走 [MultipartForm] IFormContent）。
        ResultTypeOf(typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.SingleFileUploadAsync))
            .Should().Be(typeof(WxaVodMediaIdResponse));
        ResultTypeOf(typeof(IWxaVodUploadService), nameof(IWxaVodUploadService.CommitUploadAsync))
            .Should().Be(typeof(WxaVodMediaIdResponse),
            "两支上传通路的应答逐字段相同（media_id）⇒ 共用 WxaVodMediaIdResponse，按通路拆两个同形类只会误导差异");

        // 两支 multipart 端点不得存在请求 DTO（官方为表单字段，不是 JSON 对象）。
        typeof(WxaVodMediaInfo).Assembly.GetTypes().Should().NotContain(static t =>
                t.Namespace == DtoNamespace && (t.Name == "WxaVodSingleFileUploadRequest" || t.Name == "WxaVodUploadPartRequest"),
            "singlefileupload / uploadpart 官方为 multipart 表单 ⇒ 不得凭空建 JSON 请求 DTO");

        // 共用 DTO 裁决 3：actor_list 的「对象包装 + actor 数组」形态在四处出现（提审请求体、改基础信息请求体、
        // 剧目详情条目对象、而详情条目又被 getdrama 与 listdramas 两页共用）⇒ 同一 WxaVodActorList。
        AssertPropertyType<WxaVodAuditDramaRequest>(nameof(WxaVodAuditDramaRequest.ActorList), typeof(WxaVodActorList));
        AssertPropertyType<WxaVodModifyDramaBasicInfoRequest>(nameof(WxaVodModifyDramaBasicInfoRequest.ActorList), typeof(WxaVodActorList));
        AssertPropertyType<WxaVodDramaInfo>(nameof(WxaVodDramaInfo.ActorList), typeof(WxaVodActorList),
            "剧目详情条目对象（getdrama 与 listdramas 共用）的 actor_list 与写侧同为包装形态");

        // 共用 DTO 裁决 4：copyright 子表在提审与改基础信息两页字段名集完全相同 ⇒ 同一 WxaVodCopyright。
        //（必填面差异属语义：提审侧要求更多字段必填，不改变字段名集，故仍共用。）
        AssertPropertyType<WxaVodAuditDramaRequest>(nameof(WxaVodAuditDramaRequest.Copyright), typeof(WxaVodCopyright));
        AssertPropertyType<WxaVodModifyDramaBasicInfoRequest>(nameof(WxaVodModifyDramaBasicInfoRequest.Copyright), typeof(WxaVodCopyright),
            "auditdrama 与 modifydramabasicinfo 的 copyright 子表字段名集一致 ⇒ 共用 WxaVodCopyright");

        // 共用 DTO 裁决 5：replace_media_list 元素类型在 auditdrama 与 submitreplacedramamedias 两页同为
        // {old, new} ⇒ 共用 WxaVodReplaceMediaItem。
        AssertPropertyType<WxaVodAuditDramaRequest>(nameof(WxaVodAuditDramaRequest.ReplaceMediaList), typeof(List<WxaVodReplaceMediaItem>));
        AssertPropertyType<WxaVodSubmitReplaceDramaMediasRequest>(nameof(WxaVodSubmitReplaceDramaMediasRequest.ReplaceMediaList),
            typeof(List<WxaVodReplaceMediaItem>), "两页 replace_media_list 元素字段名集（old/new）相同 ⇒ 共用元素类型");

        // 不共用裁决：replacedramamedia 官方请求体是平铺的 old_media_id + new_media_id（单对替换），
        // 与 submitreplacedramamedias 的数组形态字段名集不同 ⇒ 两类型不得合并。
        AssertJsonProperty<WxaVodReplaceDramaMediaRequest>(nameof(WxaVodReplaceDramaMediaRequest.OldMediaId), "old_media_id", "官方原文拼写（平铺单对替换）");
        AssertJsonProperty<WxaVodReplaceDramaMediaRequest>(nameof(WxaVodReplaceDramaMediaRequest.NewMediaId), "new_media_id", "官方原文拼写");
        typeof(WxaVodReplaceDramaMediaRequest).GetProperty("ReplaceMediaList", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("replacedramamedia 官方请求体无 replace_media_list（数组形态只属于 submitreplacedramamedias）");

        // 共用 DTO 裁决 6：authorizedrama / deauthorizedrama / authorizecopyright / deauthorizecopyright
        // 四页应答逐字段相同（errcode + errmsg + result[]）⇒ 四端点同一返回类型。
        var sharedResultEndpoints = new (Type Interface, string Method)[]
        {
            (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.AuthorizeDramaAsync)),
            (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.DeauthorizeDramaAsync)),
            (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.AuthorizeCopyrightAsync)),
            (typeof(IWxaVodAuthorizationService), nameof(IWxaVodAuthorizationService.DeauthorizeCopyrightAsync)),
        };
        foreach (var (iface, method) in sharedResultEndpoints)
        {
            ResultTypeOf(iface, method).Should().Be(typeof(WxaVodAuthorizeResultResponse),
                $"{method} 的官方应答与其余三支逐字段相同 ⇒ 共用 WxaVodAuthorizeResultResponse（拆四个同形包装会误导「四者有差异」）");
        }

        // 共用 DTO 裁决 7：版权授权两页查询（authorizationlist / authorizedlist）元素字段名集相同 ⇒ 同一 WxaVodCopyrightAuthorization。
        AssertPropertyType<WxaVodGetCopyrightAuthorizationListResponse>(nameof(WxaVodGetCopyrightAuthorizationListResponse.List),
            typeof(List<WxaVodCopyrightAuthorization>));
        AssertPropertyType<WxaVodGetCopyrightAuthorizedListResponse>(nameof(WxaVodGetCopyrightAuthorizedListResponse.List),
            typeof(List<WxaVodCopyrightAuthorization>), "两页 list 元素字段名集一致 ⇒ 共用 WxaVodCopyrightAuthorization");

        // 历史/边界锁定：官方未给出的字段不得凭空补进请求体。
        typeof(WxaVodDeauthorizeCopyrightRequest).GetProperty("ExpireTime", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("解除版权授权官方无 expire_time（解除即生效，无到期时间语义）");
        typeof(WxaVodModifyDramaBasicInfoRequest).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("modifydramabasicinfo 官方参数表无 name（剧目正名不可改，可改的是备用剧名 alternate_name）");
        AssertJsonProperty<WxaVodModifyDramaBasicInfoRequest>(nameof(WxaVodModifyDramaBasicInfoRequest.AlternateName), "alternate_name", "官方原文拼写（备用剧名可改）");

        // 提审应答官方只有 drama_id 一支业务字段 ⇒ 不得凭空补审核状态等字段。
        typeof(WxaVodAuditDramaResponse).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, "auditdrama 官方返回体除 errcode/errmsg 外只有 drama_id");
    }

    /// <summary>契约守卫 DV5：DTO 全量登记进 AOT JSON 上下文；命名空间清单锁死（常量类恰 21）；官方枚举取值锁定。</summary>
    [Fact]
    public void VodDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaVodMediaInfo).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("短剧媒资域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 69 DTO = 上传 11 + 媒资 9 + 剧目 19（+1 setplayerdramarecmdswitch）+ CDN 9 + 授权 21
        //（按接口端点的请求体 / 应答 / 嵌套对象逐页官方表建模；子目录计数与文件计数由生成脚本自证）。
        dtoTypes.Should().HaveCount(69,
            "短剧媒资域 32 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫"
            + "（upload 11 + media 9 + drama 19 + cdn 9 + authorization 21 = 69）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Vod");
        }

        var contextType = assembly.GetType(DtoNamespace + ".VodJsonContext");
        contextType.Should().NotBeNull(
            "VodJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"Vod\")]，"
                + "必须登记进 VodJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：除已登记 DTO、生成上下文与二十一支官方枚举常量类外不得出现任何新类型。
        var constClasses = new[]
        {
            nameof(WxaVodMediaTypes), nameof(WxaVodCoverTypes), nameof(WxaVodResourceTypes),
            nameof(WxaVodTaskTypes), nameof(WxaVodTaskStatuses),
            nameof(WxaVodMediaAuditStatuses),
            nameof(WxaVodDramaTypes), nameof(WxaVodDramaStatuses), nameof(WxaVodDramaAuditStatuses),
            nameof(WxaVodDramaAuditTypes), nameof(WxaVodQualificationTypes), nameof(WxaVodExpedited),
            nameof(WxaVodContentDeclared), nameof(WxaVodCopyrightRoles), nameof(WxaVodCopyrightVerifications),
            nameof(WxaVodApplyForCopyrightProtection),
            nameof(WxaVodCdnQueryTypes), nameof(WxaVodCdnDataIntervals), nameof(WxaVodPackageQueryTypes),
            nameof(WxaVodPackageStatuses),
            nameof(WxaVodAuthorizationTypes),
        };
        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType && !constClasses.Contains(t.Name))
            .ToList();

        unregistered.Should().BeEmpty(
            "短剧媒资域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文、亦非已登记常量类的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        constClasses.Should().HaveCount(21, "本域官方枚举常量类为 21 支");
        constClasses.Select(n => assembly.GetType(DtoNamespace + "." + n))
            .Should().OnlyContain(t => t != null,
            "二十一支官方枚举常量类必须存在（上传媒体/封面/资源/任务 + 媒资与剧目审核 + 剧目备案与版权子表 + CDN 与资源包 + 授权主体类型）");

        // 官方枚举常量类必须是 static class + const 字段（不得写成 enum：跨 TFM 线格式与序列化口径不一）。
        foreach (var name in constClasses)
        {
            var constType = assembly.GetType(DtoNamespace + "." + name)!;
            constType.IsAbstract.Should().BeTrue($"{name} 须为 static class（全仓常量类统一形态）");
            constType.GetFields().Where(static f => f.IsLiteral).Should().NotBeEmpty($"{name} 必须含 const 字段");
        }

        // 应答对象全量继承 WxaResponse（判错走单点判定），且每个应答类型都真的被端点用作返回类型。
        var responseDtos = domainTypes
            .Where(static t => t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();

        responseDtos.Should().HaveCount(21,
            "本域官方应答对象为 21（upload 5 + media 3 + drama 4 + cdn 3 + authorization 6）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "短剧媒资域响应 DTO 必须继承 WxaResponse（errcode 判错与全线同源）");

        var endpointReturnTypes = Routes
            .Select(static r => ResultTypeOf(r.Interface, r.Method))
            .Where(static t => t.Namespace == DtoNamespace)
            .Distinct()
            .ToList();

        endpointReturnTypes.Should().BeEquivalentTo(responseDtos,
            "本域应答类型必须与端点返回类型一一对应（既不得留下无人返回的孤儿应答，也不得返回命名空间外的自建应答）");

        Routes.Count(static r => ResultTypeOf(r.Interface, r.Method) == typeof(WxaResponse))
            .Should().Be(7,
            "七支官方返回体只有 errcode/errmsg 的端点（deletemedia / submitreplacedramamedias / replacedramamedia / "
            + "modifydramabasicinfo / authorizeapp / deauthorizeapp / setplayerdramarecmdswitch）直接用 WxaResponse，不建空壳应答");

        // 官方枚举取值锁定（逐页照官方取值表；官方未公布的枚举不得凭空建常量类）。
        AssertConstValues(typeof(WxaVodResourceTypes), ("Video", 1), ("Image", 2));
        AssertConstValues(typeof(WxaVodTaskTypes), ("PullUpload", 1));
        AssertConstValues(typeof(WxaVodTaskStatuses), ("Waiting", 1), ("Processing", 2), ("Finished", 3), ("Failed", 4));
        AssertConstValues(typeof(WxaVodMediaAuditStatuses),
            ("Invalid", 0), ("Auditing", 1), ("Rejected", 2), ("Approved", 3), ("RejectedRefill", 4));
        AssertConstValues(typeof(WxaVodDramaStatuses),
            ("Normal", 0), ("Auditing", 1), ("AuditFailed", 2), ("Offline", 3));
        AssertConstValues(typeof(WxaVodDramaAuditStatuses),
            ("Invalid", 0), ("Auditing", 1), ("FinalFailed", 2), ("Approved", 3), ("RejectedRefill", 4));
        AssertConstValues(typeof(WxaVodDramaAuditTypes),
            ("FirstAudit", 0), ("ReAudit", 1), ("ReplaceMediaAudit", 2), ("ModifyBasicInfoAudit", 3));
        AssertConstValues(typeof(WxaVodDramaTypes), ("Comic", 1), ("RealPerson", 2), ("DigitalHuman", 3));
        AssertConstValues(typeof(WxaVodQualificationTypes), ("Licensed", 1), ("UnderCostLimit", 2));
        AssertConstValues(typeof(WxaVodExpedited), ("Normal", 0), ("Urgent", 1));
        AssertConstValues(typeof(WxaVodContentDeclared), ("NotDeclared", 0), ("ContainsAi", 1));
        AssertConstValues(typeof(WxaVodCopyrightRoles), ("Producer", 1), ("Licensee", 2));
        AssertConstValues(typeof(WxaVodApplyForCopyrightProtection), ("NotApply", 0), ("Apply", 1));
        AssertConstValues(typeof(WxaVodCopyrightVerifications), ("ByMaterial", "1"), ("ByAuthorization", "2"));
        AssertConstValues(typeof(WxaVodCdnQueryTypes), ("All", 0), ("ShortDramaPlayer", 1), ("GeneralPlayback", 2));
        AssertConstValues(typeof(WxaVodCdnDataIntervals), ("FiveMinutes", 5), ("Hourly", 60), ("Daily", 1440));
        AssertConstValues(typeof(WxaVodPackageQueryTypes), ("General", 0), ("Directed", 1));
        AssertConstValues(typeof(WxaVodPackageStatuses),
            ("NoFilter", 0), ("Valid", 1), ("Invalid", 2), ("Expired", 3));
        AssertConstValues(typeof(WxaVodAuthorizationTypes), ("Subject", 1), ("MiniProgram", 2));
        AssertConstValues(typeof(WxaVodMediaTypes),
            ("MP4", "MP4"), ("TS", "TS"), ("MOV", "MOV"), ("MXF", "MXF"), ("MPG", "MPG"),
            ("FLV", "FLV"), ("WMV", "WMV"), ("AVI", "AVI"), ("M4V", "M4V"), ("F4V", "F4V"),
            ("MPEG", "MPEG"), ("F3GP", "3GP"), ("ASF", "ASF"), ("MKV", "MKV"));
        AssertConstValues(typeof(WxaVodCoverTypes),
            ("JPG", "JPG"), ("JPEG", "JPEG"), ("PNG", "PNG"), ("BMP", "BMP"), ("TIFF", "TIFF"),
            ("AI", "AI"), ("CDR", "CDR"), ("EPS", "EPS"), ("TIF", "TIF"));

        // 反直觉点锁定：媒资审核状态与剧目审核状态的取值集同为 0~4，但 2 的语义不同
        //（媒资侧 Rejected＝审核不通过，剧目侧 FinalFailed＝审核不通过且不可再提）⇒ 两套常量类分立，不得合并。
        typeof(WxaVodMediaAuditStatuses).GetField(nameof(WxaVodMediaAuditStatuses.Rejected))!
            .GetRawConstantValue().Should().Be(2, "媒资侧 2 官方语义为「审核不通过」");
        typeof(WxaVodDramaAuditStatuses).GetField(nameof(WxaVodDramaAuditStatuses.FinalFailed))!
            .GetRawConstantValue().Should().Be(2, "剧目侧 2 官方语义为「最终审核不通过」——与媒资侧同名取值语义不同源");
        typeof(WxaVodMediaAuditStatuses).GetFields().Where(static f => f.IsLiteral)
            .Should().NotContain(static f => f.Name == "FinalFailed",
            "两套审核状态不得合并成一支常量类（官方两页对取值 2 的描述不同）");
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    /// <summary>取端点方法的应答类型（<c>Task&lt;T&gt;</c> 解包为 <c>T</c>），守卫比对的是应答对象本身而非 Task 壳。</summary>
    private static Type ResultTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        method.Should().NotBeNull($"{interfaceType.Name}.{methodName} 不存在（契约面漂移）");

        var returnType = method!.ReturnType;
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : returnType;
    }

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void AssertPropertyType<T>(string propertyName, Type expectedType, string because = "")
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType,
            $"{typeof(T).Name}.{propertyName} 的类型必须是 {expectedType.Name}（共用/元素类型裁决{(because.Length > 0 ? "：" + because : string.Empty)}）");
    }

    private static void AssertConstValues(Type constType, params (string Name, object Value)[] expected)
    {
        var actual = constType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .ToDictionary(static f => f.Name, static f => f.GetRawConstantValue()!);

        actual.Should().BeEquivalentTo(expected.ToDictionary(static e => e.Name, static e => e.Value),
            because: $"{constType.Name} 的官方取值集漂移须先核对官方枚举表再同批调整本守卫");
    }
}
