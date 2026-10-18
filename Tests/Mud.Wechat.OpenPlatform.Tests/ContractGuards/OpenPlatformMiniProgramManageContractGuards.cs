// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.HttpUtils;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线「代小程序管理」（MiniProgramManage）域 30-1 子批（<c>/wxa/*</c> 三支接口共 22 端点：
/// 代码管理 15 + 名称与搜索 6 + 类目 1）契约守卫（MPG 系列，2026-10-17 逐页取证）。
/// </summary>
/// <remarks>
/// <para><b>本守卫锁定的裁决</b>：① 逐接口（方法, 动词, 路由）表——本族查询类端点动词不统一
/// （<c>get_auditstatus</c>/<c>fetchdatasetting</c> 为 POST、<c>get_latest_auditstatus</c>/<c>get_page</c>/<c>queryquota</c>
/// /<c>undocodeaudit</c>/<c>revertgrayrelease</c>/<c>revertcoderelease</c>/<c>getwxasearchstatus</c>/<c>get_category</c> 为 GET），逐页照抄官方；
/// ② <b>令牌面统一</b>——21 端点全部消费 <c>authorizer_access_token</c>（代授权方小程序调用），Query 参数名恒 <c>access_token</c>、
/// 三支接口共用注册组 <c>MiniProgramManage</c>（一次 <c>AddMiniProgramManageWebApiHttpClient()</c> 覆盖三支）；
/// ③ <b>审核单号字段名差异</b>——代码审核用 <c>auditid</c>（无下划线，<see cref="OpenPlatformMpAuditIdRequest"/>）、
/// 名称审核用 <c>audit_id</c>（带下划线，<see cref="OpenPlatformMpQueryNicknameRequest"/>），照官方原文不「对齐」；
/// ④ <c>revertcoderelease</c> 单路由以 Query <c>action</c> 分派（<c>get_history_version</c> / <c>set_specific_version</c> + 可选 <c>app_version</c>），计为 1 端点；
/// ⑤ <b>特殊形态面</b>——<c>uploadmedia</c>（表单仅 <c>media</c>）与 <c>upload_icp_media</c>（表单四支字段）为 <c>multipart/form-data</c>、
/// 零 JSON 请求 DTO（MPG11），应答键名 <c>mediaid</c> 与 <c>media_id</c> 各自照官方不「对齐」（MPG12）；
/// 本族第三支特殊形态 <c>get_qrcode</c>（成功应答为 JPEG 二进制流）不走声明式管线，落 <b>手写通道契约守卫</b>（QRC 系列）。</para>
/// <para>聚合面（接口总数 / 端点总数 / 路由前缀独占 / DTO 上下文登记）由 <see cref="OpenPlatformContractGuards"/>（OP-B 系列）持有，本守卫逐条锁定本域明细。</para>
/// </remarks>
public class OpenPlatformMiniProgramManageContractGuards
{
    private static readonly Assembly MainAssembly = typeof(IWechatOpenPlatformMpCodeService).Assembly;

    private static readonly Assembly DataModelsAssembly = typeof(OpenPlatformResponse).Assembly;

    /// <summary>代码管理 <see cref="IWechatOpenPlatformMpCodeService"/> 的 16 条（方法名, 动词, 路由）期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] CodeEndpoints =
    {
        ("CommitCodeAsync", "POST", "/wxa/commit"),
        ("SubmitAuditAsync", "POST", "/wxa/submit_audit"),
        ("GetAuditStatusAsync", "POST", "/wxa/get_auditstatus"),
        ("GetLatestAuditStatusAsync", "GET", "/wxa/get_latest_auditstatus"),
        ("SpeedUpAuditAsync", "POST", "/wxa/speedupaudit"),
        ("UndoCodeAuditAsync", "GET", "/wxa/undocodeaudit"),
        ("QueryAuditQuotaAsync", "GET", "/wxa/queryquota"),
        ("ReleaseCodeAsync", "POST", "/wxa/release"),
        ("GrayReleaseAsync", "POST", "/wxa/grayrelease"),
        ("RevertGrayReleaseAsync", "GET", "/wxa/revertgrayrelease"),
        ("RevertCodeReleaseAsync", "GET", "/wxa/revertcoderelease"),
        ("GetPageAsync", "GET", "/wxa/get_page"),
        ("GetVersionInfoAsync", "POST", "/wxa/getversioninfo"),
        ("GetVisitStatusAsync", "POST", "/wxa/getvisitstatus"),
        ("ChangeVisitStatusAsync", "POST", "/wxa/change_visitstatus"),
        ("UploadMediaAsync", "POST", "/wxa/uploadmedia"),
    };

    /// <summary>名称与搜索 <see cref="IWechatOpenPlatformMpBasicInfoService"/> 的 6 条（方法名, 动词, 路由）期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] BasicInfoEndpoints =
    {
        ("SetNicknameAsync", "POST", "/wxa/setnickname"),
        ("QueryNicknameStatusAsync", "POST", "/wxa/api_wxa_querynickname"),
        ("ChangeWxaSearchStatusAsync", "POST", "/wxa/changewxasearchstatus"),
        ("GetWxaSearchStatusAsync", "GET", "/wxa/getwxasearchstatus"),
        ("FetchDataSettingAsync", "POST", "/wxa/fetchdatasetting"),
        ("CheckWxVerifyNicknameAsync", "POST", "/cgi-bin/wxverify/checkwxverifynickname"),
    };

    /// <summary>类目 <see cref="IWechatOpenPlatformMpCategoryService"/> 的 1 条期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] CategoryEndpoints =
    {
        ("GetCategoryAsync", "GET", "/wxa/get_category"),
    };

    /// <summary>域名与 DNS 预解析 <see cref="IWechatOpenPlatformMpDomainService"/> 的 9 条（方法名, 动词, 路由）期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] DomainEndpoints =
    {
        ("GetEffectiveServerDomainAsync", "POST", "/wxa/get_effective_domain"),
        ("GetEffectiveWebViewDomainAsync", "POST", "/wxa/get_effective_webviewdomain"),
        ("GetPrefetchDomainAsync", "GET", "/wxa/get_prefetchdnsdomain"),
        ("GetWebViewDomainConfirmFileAsync", "POST", "/wxa/get_webviewdomain_confirmfile"),
        ("ModifyServerDomainAsync", "POST", "/wxa/modify_domain"),
        ("ModifyServerDomainDirectlyAsync", "POST", "/wxa/modify_domain_directly"),
        ("SetPrefetchDomainAsync", "POST", "/wxa/set_prefetchdnsdomain"),
        ("SetWebViewDomainAsync", "POST", "/wxa/setwebviewdomain"),
        ("SetWebViewDomainDirectlyAsync", "POST", "/wxa/setwebviewdomain_directly"),
    };

    /// <summary>体验成员 <see cref="IWechatOpenPlatformMpMemberService"/> 的 3 条期望表（含 <c>memberauth</c> 官方为 POST）。</summary>
    private static readonly (string Method, string Verb, string Route)[] MemberEndpoints =
    {
        ("BindTesterAsync", "POST", "/wxa/bind_tester"),
        ("UnbindTesterAsync", "POST", "/wxa/unbind_tester"),
        ("GetTesterAsync", "POST", "/wxa/memberauth"),
    };

    /// <summary>违规记录与申诉 <see cref="IWechatOpenPlatformMpRecordService"/> 的 2 条期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] RecordEndpoints =
    {
        ("GetIllegalRecordsAsync", "POST", "/wxa/getillegalrecords"),
        ("GetAppealRecordsAsync", "POST", "/wxa/getappealrecords"),
    };

    /// <summary>被展示组件（订阅）<see cref="IWechatOpenPlatformMpSubscribeService"/> 的 3 条期望表（两支查询为 GET、设置为 POST）。</summary>
    private static readonly (string Method, string Verb, string Route)[] SubscribeEndpoints =
    {
        ("GetShowItemAsync", "GET", "/wxa/getshowwxaitem"),
        ("GetLinkForShowAsync", "GET", "/wxa/getwxamplinkforshow"),
        ("UpdateShowItemAsync", "POST", "/wxa/updateshowwxaitem"),
    };

    /// <summary>ICP 备案 JSON 面 <see cref="IWechatOpenPlatformMpIcpService"/> 的 13 条期望表（<c>query_icp_verifytask</c> 官方为 POST，其余 <c>query_*</c>/<c>get_*</c> 为 GET，<c>upload_icp_media</c> 为 multipart POST）。</summary>
    private static readonly (string Method, string Verb, string Route)[] IcpEndpoints =
    {
        ("ApplyIcpFilingAsync", "POST", "/wxa/icp/apply_icp_filing"),
        ("CancelApplyIcpFilingAsync", "POST", "/wxa/icp/cancel_apply_icp_filing"),
        ("CancelIcpFilingAsync", "POST", "/wxa/icp/cancel_icp_filing"),
        ("CreateIcpVerifyTaskAsync", "POST", "/wxa/icp/create_icp_verifytask"),
        ("QueryIcpVerifyTaskAsync", "POST", "/wxa/icp/query_icp_verifytask"),
        ("GetIcpEntranceInfoAsync", "GET", "/wxa/icp/get_icp_entrance_info"),
        ("GetOnlineIcpOrderAsync", "GET", "/wxa/icp/get_online_icp_order"),
        ("QueryIcpCertificateTypesAsync", "GET", "/wxa/icp/query_icp_certificate_types"),
        ("QueryIcpNrlxTypesAsync", "GET", "/wxa/icp/query_icp_nrlx_types"),
        ("QueryIcpSubjectTypesAsync", "GET", "/wxa/icp/query_icp_subject_types"),
        ("QueryIcpServiceContentTypesAsync", "GET", "/wxa/icp/query_icp_service_content_types"),
        ("QueryIcpDistrictCodeAsync", "GET", "/wxa/icp/query_icp_district_code"),
        ("UploadIcpMediaAsync", "POST", "/wxa/icp/upload_icp_media"),
    };

    /// <summary>隐私接口 <see cref="IWechatOpenPlatformMpPrivacyService"/> 的 3 条期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] PrivacyEndpoints =
    {
        ("ApplyPrivacyInterfaceAsync", "POST", "/wxa/security/apply_privacy_interface"),
        ("GetPrivacyInterfaceAsync", "GET", "/wxa/security/get_privacy_interface"),
        ("GetCodePrivacyInfoAsync", "GET", "/wxa/security/get_code_privacy_info"),
    };

    /// <summary>订单路径查询 <see cref="IWechatOpenPlatformMpOrderPathService"/> 的 1 条期望表（<c>getorderpathinfo</c> 官方为 POST）。</summary>
    private static readonly (string Method, string Verb, string Route)[] OrderPathEndpoints =
    {
        ("GetOrderPathInfoAsync", "POST", "/wxa/security/getorderpathinfo"),
    };

    /// <summary>试用小程序核验 <see cref="IWechatOpenPlatformMpBetaVerifyService"/> 的 2 条期望表（转正 / 改名，均 POST、权限集 81、<c>authorizer_access_token</c>）。</summary>
    private static readonly (string Method, string Verb, string Route)[] BetaVerifyEndpoints =
    {
        ("VerifyBetaWeappAsync", "POST", "/wxa/verifybetaweapp"),
        ("SetBetaWeappNicknameAsync", "POST", "/wxa/setbetaweappnickname"),
    };

    /// <summary>
    /// MPG1：代码管理 16 端点（方法名, 动词, 路由）逐条锁定——查询类动词不统一即官方原文，误改动词即请求落错分支。
    /// </summary>
    [Fact]
    public void CodeServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpCodeService), CodeEndpoints, 16);

    /// <summary>
    /// MPG2：名称与搜索 5 端点（方法名, 动词, 路由）逐条锁定。
    /// </summary>
    [Fact]
    public void BasicInfoServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpBasicInfoService), BasicInfoEndpoints, 6);

    /// <summary>
    /// MPG3：类目 1 端点（GET <c>/wxa/get_category</c>）锁定，与第三方平台自身类目域（<c>/cgi-bin/wxopen/*</c>）不同路由不同令牌。
    /// </summary>
    [Fact]
    public void CategoryServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpCategoryService), CategoryEndpoints, 1);

    /// <summary>
    /// MPG3b：域名与 DNS 预解析 9 端点逐条锁定——<c>get_effective_*</c>/<c>get_webviewdomain_confirmfile</c> 官方为 POST、仅 <c>get_prefetchdnsdomain</c> 为 GET（本族动词最易被「望文生义」改错）。
    /// </summary>
    [Fact]
    public void DomainServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpDomainService), DomainEndpoints, 9);

    /// <summary>
    /// MPG3c：体验成员 3 端点逐条锁定（<c>memberauth</c> 官方为 POST，非 GET）。
    /// </summary>
    [Fact]
    public void MemberServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpMemberService), MemberEndpoints, 3);

    /// <summary>
    /// MPG3d：违规记录与申诉 2 端点逐条锁定（两支均 POST）。
    /// </summary>
    [Fact]
    public void RecordServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpRecordService), RecordEndpoints, 2);

    /// <summary>
    /// MPG3e：被展示组件（订阅）3 端点逐条锁定——两支查询 GET、设置 POST。
    /// </summary>
    [Fact]
    public void SubscribeServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpSubscribeService), SubscribeEndpoints, 3);

    /// <summary>
    /// MPG3f：ICP 备案面 13 端点逐条锁定——<c>query_icp_verifytask</c> 名义查询实为 POST（带请求体），其余 <c>query_*</c>/<c>get_*</c> 字典查询为 GET，
    /// <c>upload_icp_media</c> 为 multipart POST，动词混用即请求落错分支。
    /// </summary>
    [Fact]
    public void IcpServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpIcpService), IcpEndpoints, 13);

    /// <summary>
    /// MPG3g：隐私接口 3 端点逐条锁定（设置为 POST、两支查询为 GET）。
    /// </summary>
    [Fact]
    public void PrivacyServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpPrivacyService), PrivacyEndpoints, 3);

    /// <summary>
    /// MPG3h：订单路径查询 <c>getorderpathinfo</c> 官方为 POST（"get" 命名陷阱）；其 <b>component 面姊妹</b> <c>applysetorderpathinfo</c>
    /// 令牌归属不同，独立到 <c>MiniProgramTemplate</c> 注册组（守卫 TPL 系列锁定），不入本授权方组。
    /// </summary>
    [Fact]
    public void OrderPathServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpOrderPathService), OrderPathEndpoints, 1);

    /// <summary>
    /// MPG3i：试用小程序核验 2 端点逐条锁定——<c>verifybetaweapp</c>/<c>setbetaweappnickname</c> 均 POST；
    /// 官方文档树虽在 <c>register-management/fast-regist-beta</c>，令牌经逐页取证为 <c>authorizer_access_token</c>（权限集 81）⇒ 归本授权方组。
    /// 同族 2.0 旧版两支 <c>/wxa/getmpadminauth</c>、<c>/wxa/mpverifybetaweapp</c> 判<b>非契约</b>（旧页正文仅剩「优化后重新上线」、不在现行目录树内），
    /// 不入本表、亦不得声明，由 <c>OpenPlatformNonContractRouteZeroDeclarationContractGuards</c>（OP-NC1/OP-NC2）锁定零声明。
    /// </summary>
    [Fact]
    public void BetaVerifyServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpBetaVerifyService), BetaVerifyEndpoints, 2);

    /// <summary>
    /// MPG11：30-5 两支 multipart 端点形态锁定——<c>uploadmedia</c>（表单仅 <c>media</c>）与
    /// <c>upload_icp_media</c>（表单 <c>type</c>/<c>certificate_type</c>/<c>icp_order_field</c>/<c>media</c> 四支，由宿主拼装）
    /// 必须走 <c>[MultipartForm] IFormContent</c> 通路且<b>不得</b>带 JSON 请求体 DTO；
    /// 官方 <c>Content-Type</c> 为 multipart，改用 <c>[Body]</c> DTO 即二进制字段静默丢失。
    /// </summary>
    [Fact]
    public void MultipartUploadEndpoints_ShouldUseFormContent_WhenDeclared()
    {
        foreach (var (interfaceType, methodName, route) in new[]
                 {
                     (typeof(IWechatOpenPlatformMpCodeService), "UploadMediaAsync", "/wxa/uploadmedia"),
                     (typeof(IWechatOpenPlatformMpIcpService), "UploadIcpMediaAsync", "/wxa/icp/upload_icp_media"),
                 })
        {
            var m = interfaceType.GetMethod(methodName)
                ?? throw new InvalidOperationException($"{methodName} 不存在（契约面漂移）");

            m.GetCustomAttributes<HttpMethodAttribute>(false).Single().RequestUri
                .Should().Be(route, $"{methodName} 的路由照官方正文");

            var form = m.GetParameters().Single(static p => p.GetCustomAttribute<MultipartFormAttribute>(false) != null);
            form.ParameterType.Should().Be(typeof(IFormContent),
                $"{methodName} 官方为 multipart/form-data，须走 [MultipartForm] IFormContent 通路（表单字段名由宿主按官方原文拼装）");

            m.GetParameters().Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>(false) != null,
                $"{methodName} 是 multipart 端点——声明 [Body] JSON 请求 DTO 即二进制字段丢失的静默错误");
        }
    }

    /// <summary>
    /// MPG12：30-5 两支上传端点的应答键名照官方原文——<c>mediaid</c>（代码侧<b>无</b>下划线）与
    /// <c>media_id</c>（备案侧<b>带</b>下划线）同源不同形，不得「对齐」；
    /// 备案侧时间戳取官方<b>参数表</b>的 <c>created_at</c>（同页示例写 <c>create_at</c>，属官方示例笔误）。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformMpUploadMediaResponse), "Type", "type")]
    [InlineData(typeof(OpenPlatformMpUploadMediaResponse), "MediaId", "mediaid")]
    [InlineData(typeof(OpenPlatformMpUploadIcpMediaResponse), "Type", "type")]
    [InlineData(typeof(OpenPlatformMpUploadIcpMediaResponse), "MediaId", "media_id")]
    [InlineData(typeof(OpenPlatformMpUploadIcpMediaResponse), "CreatedAt", "created_at")]
    public void UploadResponseKeys_ShouldBePreservedPerOfficialTable(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文，改名即字段静默丢失");
    }

    /// <summary>
    /// MPG4：令牌面统一——十一支接口全部 <c>authorizer_access_token</c> + Query 参数名 <c>access_token</c> + 注册组 <c>MiniProgramManage</c>。
    /// </summary>
    [Fact]
    public void TokenSemantics_ShouldBeAuthorizerSharedGroup_WhenInterfacesDeclared()
    {
        foreach (var type in new[]
                 {
                     typeof(IWechatOpenPlatformMpCodeService),
                     typeof(IWechatOpenPlatformMpBasicInfoService),
                     typeof(IWechatOpenPlatformMpCategoryService),
                     typeof(IWechatOpenPlatformMpDomainService),
                     typeof(IWechatOpenPlatformMpMemberService),
                     typeof(IWechatOpenPlatformMpRecordService),
                     typeof(IWechatOpenPlatformMpSubscribeService),
                     typeof(IWechatOpenPlatformMpIcpService),
                     typeof(IWechatOpenPlatformMpPrivacyService),
                     typeof(IWechatOpenPlatformMpOrderPathService),
                     typeof(IWechatOpenPlatformMpBetaVerifyService),
                 })
        {
            var token = type.GetCustomAttributes<TokenAttribute>(false).Single();
            token.TokenType.Should().Be(OpenPlatformTokenTypes.AuthorizerAccessToken,
                $"{type.Name} 代授权方小程序调用，取平台自身 component 令牌即令牌串号");
            token.Name.Should().Be("access_token",
                "官方页面 query 参数名为 access_token（值才是 authorizer_access_token）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query);

            type.GetCustomAttributes<HttpClientApiAttribute>(false).Single()
                .RegistryGroupName.Should().Be("MiniProgramManage",
                    "三支接口共用一个注册组 ⇒ 一次 AddMiniProgramManageWebApiHttpClient() 覆盖全部接口");
        }
    }

    /// <summary>
    /// MPG5：三处一致（模块枚举 + builder 委托 + JSON resolver 合并登记）——注册组 <c>MiniProgramManage</c>。
    /// </summary>
    [Fact]
    public void RegistrationSurface_ShouldBeWired_WhenModuleAdded()
    {
        Enum.GetValues<OpenPlatformModule>().Should().Contain(OpenPlatformModule.MiniProgramManage,
            "模块枚举缺值 ⇒ AddModules 无法寻址本域");

        var repoRoot = FindRepoRoot();
        var builderSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformServiceBuilder.cs"));
        builderSource.Should().Contain("[OpenPlatformModule.MiniProgramManage] = static s =>",
            "注册入口必须存在（模块键 → 委托）");
        builderSource.Should().Contain("s.AddMiniProgramManageWebApiHttpClient();",
            "注册入口必须委托到生成器按注册组产出的 AddMiniProgramManageWebApiHttpClient()");
        builderSource.Should().Contain(
            "s.AddSingleton<IOpenPlatformMpTrialQrCodeService, OpenPlatformMpTrialQrCodeService>();",
            "体验版二维码是**非声明式**图片流通道，生成器不会产出它 ⇒ 必须随 MiniProgramManage 模块手工装配，"
            + "否则宿主 AddModules(MiniProgramManage) 后解析不到该服务");

        foreach (var module in Enum.GetValues<OpenPlatformModule>())
        {
            builderSource.Should().Contain($"[OpenPlatformModule.{module}] =",
                $"模块 {module} 无注册委托 ⇒ AddAllApis() 漏注册");
        }

        var resolverSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MiniProgramManageJsonContext.Default",
            "域上下文未并进合并 resolver ⇒ 运行期退回反射序列化，AOT 下直接失败");
    }

    /// <summary>
    /// MPG6：本域 106 个 DTO 全量登记进 <see cref="MiniProgramManageJsonContext"/>，且 <c>SerializerClassName</c> 恒为域段 <c>MiniProgramManage</c>。
    /// </summary>
    [Fact]
    public void DataModels_ShouldBeRegisteredInDomainJsonContext_WhenSurfaceEnumerated()
    {
        var domainTypes = DomainDtoTypes();

        domainTypes.Should().HaveCount(106,
            "本域 DTO 数漂移须先核对官方参数表，再同批重跑生成脚本（30-1 落 31 + 30-2 落 30 + 30-3 落 39 + 30-4 试用核验 4 + 30-5 上传应答 2）");

        var context = (JsonSerializerContext)typeof(MiniProgramManageJsonContext)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 MiniProgramManage 命名空间，必须登记进 MiniProgramManageJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("MiniProgramManage",
                $"{type.Name} 的 SerializerClassName 必须为域段 MiniProgramManage");
        }
    }

    /// <summary>
    /// MPG7：审核单号字段名差异照官方——代码审核 <c>auditid</c>（无下划线）、名称审核 <c>audit_id</c>（带下划线），不「对齐」。
    /// </summary>
    [Fact]
    public void AuditIdFieldNames_ShouldDifferPerOfficialSpelling()
    {
        typeof(OpenPlatformMpAuditIdRequest).GetProperty("AuditId")!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("auditid",
                "代码审核单号官方为无下划线 auditid，改成 audit_id 即字段静默丢失");
        typeof(OpenPlatformMpQueryNicknameRequest).GetProperty("AuditId")!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("audit_id",
                "名称审核单号官方为带下划线 audit_id，与代码侧不同源，不「顺手对齐」");
    }

    /// <summary>
    /// MPG8：<c>revertcoderelease</c> 单路由以 Query <c>action</c> 分派 + 可选 <c>app_version</c>（null 即省略），计为 1 端点。
    /// </summary>
    [Fact]
    public void RevertCodeRelease_ShouldDispatchByOptionalQuery_WhenSingleRoute()
    {
        var m = typeof(IWechatOpenPlatformMpCodeService).GetMethod("RevertCodeReleaseAsync")!;
        m.GetCustomAttributes<HttpMethodAttribute>(false).Single().RequestUri
            .Should().Be("/wxa/revertcoderelease",
                "官方单路由 action 分派（get_history_version / set_specific_version），不得拆成两支路由");

        var action = m.GetParameters()
            .Single(static p => p.GetCustomAttributes<QueryAttribute>(false).Any(static q => q.Name == "action"));
        action.ParameterType.Should().Be(typeof(string), "action 为必填分派参数");

        var appVersion = m.GetParameters()
            .Single(static p => p.GetCustomAttributes<QueryAttribute>(false).Any(static q => q.Name == "app_version"));
        appVersion.ParameterType.Should().Be(typeof(long?),
            "app_version 仅 set_specific_version 分支需要 ⇒ 可空，null 时生成器省略该 query");
    }

    /// <summary>
    /// MPG9：官方字段名/拼写照抄不修正（选取本域易漂移键逐条钉死）。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformMpCommitRequest), "TemplateId", "template_id")]
    [InlineData(typeof(OpenPlatformMpCommitRequest), "ExtJson", "ext_json")]
    [InlineData(typeof(OpenPlatformMpCommitRequest), "UserVersion", "user_version")]
    [InlineData(typeof(OpenPlatformMpSubmitAuditRequest), "ItemList", "item_list")]
    [InlineData(typeof(OpenPlatformMpAuditItem), "FirstClass", "first_class")]
    [InlineData(typeof(OpenPlatformMpAuditItem), "SecondId", "second_id")]
    [InlineData(typeof(OpenPlatformMpSubmitAuditResponse), "AuditId", "auditid")]
    [InlineData(typeof(OpenPlatformMpLatestAuditStatusResponse), "SubmitAuditTime", "submit_audit_time")]
    [InlineData(typeof(OpenPlatformMpQueryQuotaResponse), "SpeedupRest", "speedup_rest")]
    [InlineData(typeof(OpenPlatformMpSetNicknameRequest), "NamingOtherStuff1", "naming_other_stuff_1")]
    [InlineData(typeof(OpenPlatformMpFetchDataSettingRequest), "IsPreFetchOpen", "is_pre_fetch_open")]
    [InlineData(typeof(OpenPlatformMpFetchDataSettingRequest), "PeriodFunctionName", "period_function_name")]
    [InlineData(typeof(OpenPlatformMpCategoryItem), "ThirdClass", "third_class")]
    [InlineData(typeof(OpenPlatformMpCategoryListResponse), "CategoryList", "category_list")]
    [InlineData(typeof(OpenPlatformMpQueryNicknameResponse), "AuditStat", "audit_stat")]
    public void OfficialJsonKeyNames_ShouldBePreserved(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文，改名即字段静默丢失");
    }

    /// <summary>
    /// MPG10：30-2 各族官方字段名/拼写照抄不修正（选取易漂移键逐条钉死）。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformMpModifyDomainRequest), "Action", "action")]
    [InlineData(typeof(OpenPlatformMpModifyDomainRequest), "WsRequestDomain", "wsrequestdomain")]
    [InlineData(typeof(OpenPlatformMpModifyDomainRequest), "TcpDomain", "tcpdomain")]
    [InlineData(typeof(OpenPlatformMpModifyDomainResponse), "NoIcpDomain", "no_icp_domain")]
    [InlineData(typeof(OpenPlatformMpModifyDomainResponse), "InvalidWsRequestDomain", "invalid_wsrequestdomain")]
    [InlineData(typeof(OpenPlatformMpWebViewDomainRequest), "WebViewDomain", "webviewdomain")]
    [InlineData(typeof(OpenPlatformMpSetPrefetchDomainRequest), "PrefetchDnsDomain", "prefetch_dns_domain")]
    [InlineData(typeof(OpenPlatformMpPrefetchDomainItem), "Url", "url")]
    [InlineData(typeof(OpenPlatformMpWebviewConfirmFileResponse), "FileName", "file_name")]
    [InlineData(typeof(OpenPlatformMpBindTesterRequest), "WechatId", "wechatid")]
    [InlineData(typeof(OpenPlatformMpBindTesterResponse), "UserStr", "userstr")]
    [InlineData(typeof(OpenPlatformMpMemberAuthRequest), "Action", "action")]
    [InlineData(typeof(OpenPlatformMpAppealRecordsRequest), "IllegalRecordId", "illegal_record_id")]
    [InlineData(typeof(OpenPlatformMpAppealApplyMaterial), "ProofMaterialIds", "proof_material_ids")]
    [InlineData(typeof(OpenPlatformMpIllegalRecord), "IllegalRecordId", "illegal_record_id")]
    [InlineData(typeof(OpenPlatformMpIllegalRecord), "CaseUrl", "case_url")]
    [InlineData(typeof(OpenPlatformMpShowItemFlagRequest), "WxaSubscribeBizFlag", "wxa_subscribe_biz_flag")]
    [InlineData(typeof(OpenPlatformMpLinkForShowResponse), "BizInfoList", "biz_info_list")]
    [InlineData(typeof(OpenPlatformMpShowBizInfo), "HeadImg", "headimg")]
    public void Domain2OfficialJsonKeyNames_ShouldBePreserved(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文，改名即字段静默丢失");
    }

    /// <summary>
    /// MPG11：共用请求 DTO 与 body 内 action 分派照官方——<c>modify_domain</c>/<c>modify_domain_directly</c> 同体、<c>setwebviewdomain</c>/<c>setwebviewdomain_directly</c> 同体、<c>memberauth</c> 的 <c>action</c> 走请求体而非 query。
    /// </summary>
    [Fact]
    public void SharedRequestsAndBodyAction_ShouldStayAsOfficial()
    {
        BodyParamType(typeof(IWechatOpenPlatformMpDomainService), "ModifyServerDomainAsync")
            .Should().Be(typeof(OpenPlatformMpModifyDomainRequest));
        BodyParamType(typeof(IWechatOpenPlatformMpDomainService), "ModifyServerDomainDirectlyAsync")
            .Should().Be(typeof(OpenPlatformMpModifyDomainRequest),
                "两支请求体逐字相同（action + 六类域名数组），共用一支 DTO");
        BodyParamType(typeof(IWechatOpenPlatformMpDomainService), "SetWebViewDomainAsync")
            .Should().Be(typeof(OpenPlatformMpWebViewDomainRequest));
        BodyParamType(typeof(IWechatOpenPlatformMpDomainService), "SetWebViewDomainDirectlyAsync")
            .Should().Be(typeof(OpenPlatformMpWebViewDomainRequest));

        // memberauth 的 action 在 JSON body（官方页面 body 字段），不得误做成 [Query]（清单的 ?action= 是文档简写）。
        var getTester = typeof(IWechatOpenPlatformMpMemberService).GetMethod("GetTesterAsync")!;
        getTester.GetParameters().Should().NotContain(static p =>
                p.GetCustomAttributes<QueryAttribute>(false).Any(static q => q.Name == "action"),
            "memberauth 的 action 属请求体字段，非 query 分派");
        BodyParamType(typeof(IWechatOpenPlatformMpMemberService), "GetTesterAsync")
            .Should().Be(typeof(OpenPlatformMpMemberAuthRequest));
    }

    private static Type BodyParamType(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName)
            ?? throw new InvalidOperationException($"{methodName} 不存在");
        var body = method.GetParameters()
            .FirstOrDefault(static p => p.GetCustomAttributes<BodyAttribute>(false).Any());
        body.Should().NotBeNull($"{methodName} 官方有 JSON 请求体，须带 [Body] 参数");
        return body!.ParameterType;
    }

    private static void AssertEndpoints(Type interfaceType, (string Method, string Verb, string Route)[] expected, int count)
    {
        var methods = MethodsOf(interfaceType).ToArray();
        methods.Should().HaveCount(count, $"{interfaceType.Name} 恰 {count} 端点——少一条即缺口，多一条即漂移");

        foreach (var (methodName, verb, route) in expected)
        {
            var m = interfaceType.GetMethod(methodName)
                ?? throw new InvalidOperationException($"{methodName} 不存在（契约面漂移）");

            var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
            attr.HttpMethod.Method.Should().Be(verb, $"{methodName} 官方动词为 {verb}（本族查询类动词不统一，逐页照抄）");
            attr.RequestUri.Should().Be(route, $"{methodName} 的路由照官方正文");
        }
    }

    private static IEnumerable<MethodInfo> MethodsOf(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == interfaceType
                && m.GetCustomAttributes<HttpMethodAttribute>(false).Any());

    /// <summary>本域 DTO 类型集（上下文自身按 <see cref="JsonSerializerContext"/> 剔除）。</summary>
    private static Type[] DomainDtoTypes()
        => DataModelsAssembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsNested
                               && t.Namespace == "Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage")
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToArray();

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
