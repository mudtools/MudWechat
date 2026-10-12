// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.DataZone;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 数据与智能专区模块（DataZone 模块）契约守卫：路由表、接口层级、开放面差异与令牌绑定锁定
/// （基础接口域：三类应用公共面收敛父接口 + 差异端点子接口——「获取数据与智能专区授权信息」官方不支持自建、
/// 「获取数据与智能专区文档存档授权信息」官方仅第三方；应用调用专区程序域：三类应用公共面收敛父接口 + 空标记子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：基础接口域 12 个公共端点（设置公钥 / 获取会话存档授权成员列表 / 设置专区接收回调事件 /
/// 会话组件敏感信息隐藏设置×2 / 日志打印级别×2 / 上传临时文件到专区 / 获取文件内容存档授权成员列表 /
/// 专区调试模式×3——开启·关闭·查询状态，官方文档落位于「数据与智能专区 → 专区调试模式」子节，
/// 三类应用均需「数据与智能专区权限」）
/// 收敛声明于 <see cref="IWechatWorkDataZoneService"/> 父接口（IsAbstract）；
/// 第三方子接口额外声明 2 个差异端点、代开发子接口额外声明 1 个差异端点、自建子接口为空标记。
/// 应用调用专区程序域 3 个端点（同步调用 / 创建异步任务 / 查询任务结果）收敛声明于
/// <see cref="IWechatWorkDataZoneProgramService"/> 父接口，三个子接口均为空标记。
/// </para>
/// <para>
/// <b>「数据与智能功能族」（/cgi-bin/data/get_conversation_records、/cgi-bin/data/get_message_statistics）
/// 已按证伪回退删除（定夺三 3c，追踪号 DZ-DATA-INTEL-VERIFY-01）</b>：官方三棵文档树
/// （自建 / 第三方 / 代开发）的「数据与智能专区」章节均只有基础接口 / 应用调用专区程序 /
/// 专区程序调用 SDK / 专区程序接收事件通知四组，无任何 HTTP「数据与智能功能族」分组；
/// 官方「获取会话记录」实为专区程序调用 SDK 的 sync_msg 专区面（非 HTTP 端点），
/// 「获取消息统计」官方无任何文档页；两端点路由在官方网关 qyapi.weixin.qq.com 实测 404
/// （同前缀负对照同签名、正对照均 200）。证据链与原方案字段面（对照基准 Senparc，其官方链接即同源讹传）
/// 详见 .docs 定夺三 §6 核实记录。
/// </para>
/// <para>
/// <b>「会话存档数据面」19 条 <c>/cgi-bin/chatdata/*</c> 路由同样零端点（DZ5，追踪号
/// DZ-CHATDATA-SDK-FACE-VERIFY-02，2026-10-12）</b>：官方现行文档把这批能力整体落在「数据与智能专区 →
/// 专区程序调用 SDK」分组（概述页 99864「企业微信在专区中以 SDK 的方式提供了一套涉及会话记录、会话搜索、
/// 关键词命中记录、模型分析等接口，专区中的程序可以按需调用」；99829「请求方法通过 SDK 调用……请求接口名：
/// create_rule」；99866 §3.1「专区程序通过 sdk 调用企微后台」），该组页面上<b>既无请求地址也无 HTTP 方法</b>。
/// 曾被第三方 SDK 用作 HTTP 路径的官方页（97240 / 99495 / 99530 / 99527 / 99620 / 97237 / 97266 / 99496 /
/// 97374）已从官方文档目录（4479 节点）移除，故应用类型开放面、频率上限、字段必填性<b>无官方来源可核</b>；
/// 网关侧实测仍是「注册残留」形态（200 + 结构化 errcode，同前缀负对照 404），可达性不等于契约。
/// 专区安全模型（仅脱敏结果 / 消息 ID 可出专区）与「应用侧 HTTP 拉取原始会话内容」架构上互斥，
/// 因此本域<b>不落地</b>这 19 条端点，由 DZ5 断言其在 <c>Src/</c> 内零声明（含 <c>specapi</c> 两条，
/// 官方 2024-06-06 已宣布下线）。证据链见 .docs 专区会话存档数据面路由核验留档。
/// </para>
/// </remarks>
public class WechatDataZoneContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string DataZoneParentImplementationClassName = "WechatWorkDataZoneService";

    private const string DataZoneProgramParentImplementationClassName = "WechatWorkDataZoneProgramService";

    private const string DataZoneRegistryGroupName = "DataZone";

    /// <summary>
    /// 基础接口域公共面官方路由表（父接口 9 条端点；除上传临时文件到专区以 <c>type</c> 走 Query 外均为 JSON 包体）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] DataZoneRoutes =
    {
        // 设置公钥（自建 99961、第三方 99845、代开发 100016）：设置公钥之后消息才开始存档。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetPublicKeyAsync), "/cgi-bin/chatdata/set_public_key"),
        // 获取会话存档授权成员列表（自建 99962、第三方 99846、代开发 100017）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetChatArchiveAuthUserListAsync), "/cgi-bin/chatdata/get_auth_user_list"),
        // 设置专区接收回调事件（自建 99963、第三方 99850、代开发 100018）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetReceiveCallbackAsync), "/cgi-bin/chatdata/set_receive_callback"),
        // 会话组件敏感信息隐藏设置（自建 100139、第三方 100055、代开发 100054）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetHideSensitiveInfoConfigAsync), "/cgi-bin/chatdata/set_hide_sensitiveinfo_config"),
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetHideSensitiveInfoConfigAsync), "/cgi-bin/chatdata/get_hide_sensitiveinfo_config"),
        // 设置/获取日志打印级别（自建 100108、第三方 100106、代开发 100109）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.SetLogLevelAsync), "/cgi-bin/chatdata/set_log_level"),
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetLogLevelAsync), "/cgi-bin/chatdata/get_log_level"),
        // 上传临时文件到专区（自建 100174、第三方 100140、代开发 100175）：type 走 Query、文件走 multipart。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.UploadMediaAsync), "/cgi-bin/chatdata/upload_media"),
        // 获取文件内容存档授权成员列表（自建 101873、第三方 101681、代开发 101882）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.GetDocArchiveAuthUserListAsync), "/cgi-bin/docdata/get_auth_user_list"),
        // 专区调试模式（自建 100087/100088/100113、第三方 100083/100084/100112、代开发 100114；
        // 三类应用均需「数据与智能专区权限」；代开发侧开启/关闭文档页未发布正文，见接口 XML 注释）。
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.OpenDebugModeAsync), "/cgi-bin/chatdata/open_debug_mode"),
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.CloseDebugModeAsync), "/cgi-bin/chatdata/close_debug_mode"),
        (typeof(IWechatWorkDataZoneService),
            nameof(IWechatWorkDataZoneService.CheckDebugModeAsync), "/cgi-bin/chatdata/check_debug_mode"),
    };

    /// <summary>
    /// 基础接口域差异端点官方路由表：第三方 2 条（获取授权信息 + 文档存档授权信息）、代开发 1 条（获取授权信息）；
    /// 自建官方不支持这两端点，不得出现在自建子接口。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] DataZoneDiffRoutes =
    {
        // 获取数据与智能专区授权信息（第三方 100244、代开发 100237；官方标注自建应用不支持）。
        (typeof(IWechatWorkThirdPartyDataZoneService),
            nameof(IWechatWorkThirdPartyDataZoneService.GetCorpAuthInfoAsync), "/cgi-bin/chatdata/get_corp_auth_info"),
        (typeof(IWechatWorkProviderDataZoneService),
            nameof(IWechatWorkProviderDataZoneService.GetCorpAuthInfoAsync), "/cgi-bin/chatdata/get_corp_auth_info"),
        // 获取数据与智能专区文档存档授权信息（第三方 101488；官方标注自建/代开发均不支持）。
        (typeof(IWechatWorkThirdPartyDataZoneService),
            nameof(IWechatWorkThirdPartyDataZoneService.GetDocArchiveAuthInfoAsync), "/cgi-bin/docdata/get_auth_info"),
    };

    /// <summary>
    /// 应用调用专区程序域官方路由表（父接口 3 条端点：同步调用 / 创建异步任务 / 查询任务结果）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] DataZoneProgramRoutes =
    {
        // 应用同步调用专区程序（自建 99965、第三方 99811、代开发 100020）。
        (typeof(IWechatWorkDataZoneProgramService),
            nameof(IWechatWorkDataZoneProgramService.SyncCallProgramAsync), "/cgi-bin/chatdata/sync_call_program"),
        // 创建专区程序调用任务（自建 99966、第三方 99812、代开发 100021）。
        (typeof(IWechatWorkDataZoneProgramService),
            nameof(IWechatWorkDataZoneProgramService.CreateAsyncProgramTaskAsync), "/cgi-bin/chatdata/async_program_task"),
        // 获取专区程序任务结果（自建 99966、第三方 99812、代开发 100021）。
        (typeof(IWechatWorkDataZoneProgramService),
            nameof(IWechatWorkDataZoneProgramService.GetAsyncProgramResultAsync), "/cgi-bin/chatdata/async_program_result"),
    };

    /// <summary>
    /// 「专区程序调用 SDK」数据面 17 条 + 官方已下线 <c>specapi</c> 2 条路由清单——
    /// 本域<b>零声明</b>断言的对象（见类级 remarks 段与 DZ5）。
    /// </summary>
    private static readonly string[] DataZoneSdkFaceRoutes =
    {
        // 获取会话记录（第三方 SDK 侧路径形态，官方现仅以 SDK 接口名 sync_msg 承载）。
        "/cgi-bin/chatdata/sync_msg",
        // 获取群组会话记录。
        "/cgi-bin/chatdata/groupchat/get",
        // 获取会话存档成员同意状态（单聊 / 群聊）。
        "/cgi-bin/chatdata/getagreestatus/single",
        "/cgi-bin/chatdata/getagreestatus/room",
        // 添加 / 提交 / 查询模型分析任务。
        "/cgi-bin/chatdata/analyze_task_add",
        "/cgi-bin/chatdata/analyze_task_submit",
        "/cgi-bin/chatdata/analyze_task_result",
        // 创建 / 查询会话记录导出任务。
        "/cgi-bin/chatdata/export/create_job",
        "/cgi-bin/chatdata/export/get_job_status",
        // 关键词规则管理五支 + 命中消息拉取。
        "/cgi-bin/chatdata/keyword/create_rule",
        "/cgi-bin/chatdata/keyword/update_rule",
        "/cgi-bin/chatdata/keyword/delete_rule",
        "/cgi-bin/chatdata/keyword/get_rule_list",
        "/cgi-bin/chatdata/keyword/get_rule_detail",
        "/cgi-bin/chatdata/keyword/get_hit_msg_list",
        // 会话搜索 / 消息搜索。
        "/cgi-bin/chatdata/search_chat",
        "/cgi-bin/chatdata/search_msg",
        // 专区程序算法调用（官方 2024-06-06 宣布下线，网关仍可达）。
        "/cgi-bin/chatdata/specapi/create_job",
        "/cgi-bin/chatdata/specapi/get_job_result",
    };

    /// <summary>
    /// 契约守卫 DZ1：数据与智能专区基础接口域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 全部端点官方即 POST（含分页查询类，勿改 GET）。
    /// </summary>
    [Fact]
    public void DataZoneEndpoints_ShouldMatchOfficialRoutes()
    {
        DataZoneRoutes.Should().HaveCount(12,
            "基础接口域 12 个端点为三类应用公共面，全部收敛父接口");
        DataZoneRoutes.Select(r => r.Route).Distinct().Should().HaveCount(12, "基础接口域各端点路由互不重复");

        AssertRoutes(DataZoneRoutes);

        // 上传临时文件到专区：type 为官方必填 Query 参数（目前仅支持 file），文件走 multipart/form-data。
        var upload = typeof(IWechatWorkDataZoneService).GetMethod(
            nameof(IWechatWorkDataZoneService.UploadMediaAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        upload.Should().NotBeNull();
        var typeParam = upload!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "type");
        typeParam.Should().NotBeNull("upload_media 的 type 必须以 [Query(\"type\")] 显式承载");
        upload.GetParameters().Should().Contain(p =>
            p.GetCustomAttribute<MultipartFormAttribute>() != null,
            "upload_media 为 multipart/form-data 上传端点（文件标识名 media）");
    }

    /// <summary>
    /// 契约守卫 DZ1b：基础接口域差异端点路由必须与官方契约一致，且开放面与官方标注一致——
    /// 「获取数据与智能专区授权信息」官方不支持自建应用；「获取数据与智能专区文档存档授权信息」官方仅第三方应用开放。
    /// </summary>
    [Fact]
    public void DataZoneDiffEndpoints_ShouldMatchOfficialOpenSurface()
    {
        DataZoneDiffRoutes.Should().HaveCount(3, "差异端点：第三方 2 条 + 代开发 1 条");
        AssertRoutes(DataZoneDiffRoutes);

        // 差异端点不得漂移进官方不支持的应用类型子接口（nameof 取承载接口上的方法名，断言目标为不得声明的子接口）。
        typeof(IWechatWorkInternalDataZoneService)
            .GetMethod(nameof(IWechatWorkThirdPartyDataZoneService.GetCorpAuthInfoAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取数据与智能专区授权信息官方标注自建应用不支持");
        typeof(IWechatWorkInternalDataZoneService)
            .GetMethod(nameof(IWechatWorkThirdPartyDataZoneService.GetDocArchiveAuthInfoAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取数据与智能专区文档存档授权信息官方标注自建应用不支持");
        typeof(IWechatWorkProviderDataZoneService)
            .GetMethod(nameof(IWechatWorkThirdPartyDataZoneService.GetDocArchiveAuthInfoAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeNull("获取数据与智能专区文档存档授权信息官方标注代开发应用不支持");
    }

    /// <summary>
    /// 契约守卫 DZ1c：应用调用专区程序域全部端点路由必须与官方契约一致。
    /// </summary>
    [Fact]
    public void DataZoneProgramEndpoints_ShouldMatchOfficialRoutes()
    {
        DataZoneProgramRoutes.Should().HaveCount(3, "应用调用专区程序域 3 个端点为三类应用公共面");
        DataZoneProgramRoutes.Select(r => r.Route).Distinct().Should().HaveCount(3, "各端点路由互不重复");

        AssertRoutes(DataZoneProgramRoutes);
    }

    /// <summary>
    /// 契约守卫 DZ2：数据与智能专区接口层级与生成器注册形态——公共端点收敛于 IsAbstract 父接口，
    /// 自建子接口为空标记；第三方/代开发子接口仅承载官方标注的差异端点（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void DataZoneInterfaceHierarchy_ShouldConvergeOnAbstractParentWithDataZoneRegistry()
    {
        // —— 基础接口域 ——
        var baseChildren = new[]
        {
            typeof(IWechatWorkInternalDataZoneService),
            typeof(IWechatWorkThirdPartyDataZoneService),
            typeof(IWechatWorkProviderDataZoneService),
        };

        foreach (var child in baseChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkDataZoneService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkDataZoneService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkDataZoneService),
            DataZoneParentImplementationClassName,
            expectedDeclaredMethods: null,
            new[] { (typeof(IWechatWorkInternalDataZoneService), 0),
                    (typeof(IWechatWorkThirdPartyDataZoneService), 2),
                    (typeof(IWechatWorkProviderDataZoneService), 1) });

        // —— 应用调用专区程序域 ——
        var programChildren = new[]
        {
            typeof(IWechatWorkInternalDataZoneProgramService),
            typeof(IWechatWorkThirdPartyDataZoneProgramService),
            typeof(IWechatWorkProviderDataZoneProgramService),
        };

        foreach (var child in programChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkDataZoneProgramService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkDataZoneProgramService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkDataZoneProgramService),
            DataZoneProgramParentImplementationClassName,
            expectedDeclaredMethods: 3,
            new[] { (typeof(IWechatWorkInternalDataZoneProgramService), 0),
                    (typeof(IWechatWorkThirdPartyDataZoneProgramService), 0),
                    (typeof(IWechatWorkProviderDataZoneProgramService), 0) });
    }

    /// <summary>
    /// 契约守卫 DZ3：令牌绑定——数据与智能专区全部 8 个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；第三方/代开发消费授权企业级令牌，scope = authCorpId）。
    /// </summary>
    [Fact]
    public void DataZoneTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkDataZoneService),
            typeof(IWechatWorkInternalDataZoneService),
            typeof(IWechatWorkThirdPartyDataZoneService),
            typeof(IWechatWorkProviderDataZoneService),
            typeof(IWechatWorkDataZoneProgramService),
            typeof(IWechatWorkInternalDataZoneProgramService),
            typeof(IWechatWorkThirdPartyDataZoneProgramService),
            typeof(IWechatWorkProviderDataZoneProgramService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 DZ4：数据与智能专区模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 28 个契约面类型）。
    /// </summary>
    [Fact]
    public void DataZoneDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = DataZoneJsonContext.Default;

        var requiredTypes = new Type[]
        {
            // 设置公钥。
            typeof(SetDataZonePublicKeyRequest),
            // 获取会话存档授权成员列表。
            typeof(GetChatArchiveAuthUserListRequest), typeof(GetChatArchiveAuthUserListResponse),
            typeof(ChatArchiveAuthUser),
            // 设置专区接收回调事件。
            typeof(SetDataZoneReceiveCallbackRequest),
            // 会话组件敏感信息隐藏设置。
            typeof(SetDataZoneHideSensitiveInfoConfigRequest), typeof(GetDataZoneHideSensitiveInfoConfigRequest),
            typeof(GetDataZoneHideSensitiveInfoConfigResponse), typeof(DataZoneSensitiveInfoHideConfig),
            // 设置/获取日志打印级别。
            typeof(SetDataZoneLogLevelRequest), typeof(GetDataZoneLogLevelRequest), typeof(GetDataZoneLogLevelResponse),
            // 上传临时文件到专区。
            typeof(UploadDataZoneMediaResponse),
            // 获取文件内容存档授权成员列表。
            typeof(GetDocArchiveAuthUserListRequest), typeof(GetDocArchiveAuthUserListResponse),
            typeof(DocArchiveAuthUser),
            // 应用同步调用专区程序。
            typeof(SyncCallDataZoneProgramRequest), typeof(SyncCallDataZoneProgramResponse),
            // 应用异步调用专区程序。
            typeof(CreateDataZoneAsyncProgramTaskRequest), typeof(CreateDataZoneAsyncProgramTaskResponse),
            typeof(GetDataZoneAsyncProgramResultRequest), typeof(GetDataZoneAsyncProgramResultResponse),
            // 获取数据与智能专区授权信息 / 文档存档授权信息。
            typeof(GetDataZoneCorpAuthInfoResponse), typeof(DataZoneCorpAuthEdition), typeof(DataZoneAuthScope),
            typeof(GetDocArchiveAuthInfoResponse),
            // 专区调试模式（开启 / 关闭 / 查询状态）。
            typeof(OpenDataZoneDebugModeRequest), typeof(CloseDataZoneDebugModeRequest),
            typeof(CheckDataZoneDebugModeRequest), typeof(CheckDataZoneDebugModeResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是数据与智能专区模块契约面类型，必须登记进 DataZoneJsonContext（AOT 源生成）");
        }
    }

    /// <summary>
    /// 契约守卫 DZ5：<b>「专区程序调用 SDK」数据面 17 条 + 官方已下线 <c>specapi</c> 2 条路由，
    /// 在 <c>Src/</c> 全部源码内零声明</b>（防「网关还能打通」重新诱导加回）。
    /// <para>
    /// 判据是官方文档面而非网关可达性：这批能力的现行官方落位是「数据与智能专区 → 专区程序调用 SDK」分组
    /// （99864 / 99829 / 99866 三页原文见类级 remarks 段），页面上无请求地址与 HTTP 方法；
    /// 曾承载其 HTTP 路径的官方页（97240 等 9 页）已从官方文档目录移除，应用类型开放面与频率上限无源可核。
    /// 零凭据探针实测这批路由在 qyapi 仍返回结构化 errcode（同前缀负对照 404），属<b>注册残留</b>，
    /// 且专区安全模型（仅脱敏结果 / 消息 ID 可出专区）与「应用侧 HTTP 拉取原始会话内容」架构上互斥
    /// ——与定夺三 3c（DZ-DATA-INTEL-VERIFY-01）对 <c>/cgi-bin/data/*</c> 的证伪同源。
    /// </para>
    /// </summary>
    [Fact]
    public void DataZoneSdkFaceEndpoints_ShouldHaveZeroDeclaredRoutes_WhenScannedAcrossSrc()
    {
        DataZoneSdkFaceRoutes.Should().HaveCount(19,
            "数据面 17 条 + specapi 2 条：清单本身即契约，增删须先复核官方文档面");
        DataZoneSdkFaceRoutes.Distinct().Should().HaveCount(19, "清单内路由不得重复");

        var routeRegex = new Regex(@"\[(?:Get|Post|Put|Delete|Patch)\(""([^""]+)""\)\]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var forbidden = new HashSet<string>(DataZoneSdkFaceRoutes, StringComparer.Ordinal);

        var declared = new HashSet<string>(StringComparer.Ordinal);
        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(GetSolutionRoot(), "Src"), "*.cs",
                     SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal)
                || normalized.Contains("/Generated/", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (System.Text.RegularExpressions.Match match in routeRegex.Matches(File.ReadAllText(file)))
            {
                var route = match.Groups[1].Value;
                declared.Add(route);
                if (forbidden.Contains(route))
                {
                    offenders.Add($"{normalized}: {route}");
                }
            }
        }

        // 扫描非空跑正对照：本域已落位的 15 条专区路由必须被同一趟扫描捕获
        // （否则正则/定位漂移会让「零声明」断言静默恒真）。
        var landedRoutes = DataZoneRoutes.Select(r => r.Route)
            .Concat(DataZoneDiffRoutes.Select(r => r.Route))
            .Concat(DataZoneProgramRoutes.Select(r => r.Route))
            .ToArray();
        landedRoutes.Should().OnlyContain(route => declared.Contains(route),
            "DZ5 的扫描必须能捕获本域已声明的专区路由（正对照，防扫描空跑）");

        offenders.Should().BeEmpty(
            "「专区程序调用 SDK」数据面与已下线 specapi 共 19 条路由不是应用侧 HTTP 契约，" +
            "本仓不得在任何产品线接口上声明（追踪号 DZ-CHATDATA-SDK-FACE-VERIFY-02）");
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, string Route)[] routes)
    {
        foreach (var (iface, method, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<HttpMethodAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 HTTP 方法特性");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 接口层级断言：父接口 IsAbstract 且不进注册组；子接口挂 DataZone 注册组、继承父实现类；
    /// 各子接口的自身声明端点数必须与官方开放面一致。
    /// </summary>
    private static void AssertFamilyHierarchy(
        Type parent,
        string parentImplementationClassName,
        int? expectedDeclaredMethods,
        (Type Child, int DeclaredMethods)[] children)
    {
        if (expectedDeclaredMethods.HasValue)
        {
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(expectedDeclaredMethods.Value, $"{parent.Name} 公共端点数漂移");
        }

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var (child, declaredMethods) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(DataZoneRegistryGroupName,
                $"{child.Name} 必须挂 {DataZoneRegistryGroupName} 注册组" +
                $"（DataZone 模块共用 Add{DataZoneRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(declaredMethods,
                    $"{child.Name} 自身声明端点数必须与官方开放面一致（差异端点漂移须先核对官方文档再同批调整守卫）");
        }
    }
}
