// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Services.Authorization;

namespace Mud.Wechat.Work.Extensions;

/// <summary>
/// 企业微信模块注册器（对齐 <c>FeishuServiceBuilder</c>）：模块字典 + <c>Add{Module}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// 每个模块的注册委托指向源生成器按 [HttpClientApi] 自动产出的
/// <c>Add{RegistryGroupName}WebApiHttpClient()</c> 扩展。
/// </remarks>
public class WechatWorkServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration? _hostConfiguration;
    private readonly WechatServiceConfiguration _configuration = new();
    private readonly Dictionary<WechatModule, IWechatModuleRegistrar> _registrars;

    /// <summary>创建模块注册器（由 <see cref="WechatWorkServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="hostConfiguration">宿主配置（可选）；用于绑定 WechatAuthorization 配置节。</param>
    internal WechatWorkServiceBuilder(IServiceCollection services, IConfiguration? hostConfiguration = null)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _hostConfiguration = hostConfiguration;
        _registrars = InitializeRegistrars();
    }

    private Dictionary<WechatModule, IWechatModuleRegistrar> InitializeRegistrars()
        => new()
        {
            // AddAuthenticationWebApiHttpClient() / AddContactWebApiHttpClient() /
            // AddExternalContactWebApiHttpClient() / AddCorpGroupWebApiHttpClient()
            // 由 Mud.HttpUtils.Generator 按主包 [HttpClientApi] 自动产出。
            [WechatModule.Authentication] = new WechatModuleRegistrar(
                WechatModule.Authentication,
                s => s.AddAuthenticationWebApiHttpClient().AddWechatAuthorizationServices(_hostConfiguration)),
            [WechatModule.Contact] = new WechatModuleRegistrar(
                WechatModule.Contact,
                s => s.AddContactWebApiHttpClient()),
            [WechatModule.ExternalContact] = new WechatModuleRegistrar(
                WechatModule.ExternalContact,
                s => s.AddExternalContactWebApiHttpClient()),
            [WechatModule.CorpGroup] = new WechatModuleRegistrar(
                WechatModule.CorpGroup,
                s => s.AddCorpGroupWebApiHttpClient()),
            [WechatModule.Security] = new WechatModuleRegistrar(
                WechatModule.Security,
                s => s.AddSecurityWebApiHttpClient()),
            [WechatModule.Message] = new WechatModuleRegistrar(
                WechatModule.Message,
                s => s.AddMessageWebApiHttpClient()),
            [WechatModule.AccountId] = new WechatModuleRegistrar(
                WechatModule.AccountId,
                s => s.AddAccountIdWebApiHttpClient()),
            [WechatModule.Kf] = new WechatModuleRegistrar(
                WechatModule.Kf,
                s => s.AddKfWebApiHttpClient()),
            [WechatModule.Identity] = new WechatModuleRegistrar(
                WechatModule.Identity,
                s => s.AddIdentityWebApiHttpClient()),
            [WechatModule.Pay] = new WechatModuleRegistrar(
                WechatModule.Pay,
                s => s.AddPayWebApiHttpClient()),
            [WechatModule.MsgAudit] = new WechatModuleRegistrar(
                WechatModule.MsgAudit,
                s => s.AddMsgAuditWebApiHttpClient()),
            [WechatModule.School] = new WechatModuleRegistrar(
                WechatModule.School,
                s => s.AddSchoolWebApiHttpClient()),
            [WechatModule.Media] = new WechatModuleRegistrar(
                WechatModule.Media,
                s => s.AddMediaWebApiHttpClient()),
            [WechatModule.Invoice] = new WechatModuleRegistrar(
                WechatModule.Invoice,
                s => s.AddInvoiceWebApiHttpClient()),
            [WechatModule.Gov] = new WechatModuleRegistrar(
                WechatModule.Gov,
                s => s.AddGovWebApiHttpClient()),
            [WechatModule.DataZone] = new WechatModuleRegistrar(
                WechatModule.DataZone,
                s => s.AddDataZoneWebApiHttpClient()),
            [WechatModule.Approval] = new WechatModuleRegistrar(
                WechatModule.Approval,
                s => s.AddApprovalWebApiHttpClient()),
            [WechatModule.Mail] = new WechatModuleRegistrar(
                WechatModule.Mail,
                s => s.AddMailWebApiHttpClient()),
            [WechatModule.Emergency] = new WechatModuleRegistrar(
                WechatModule.Emergency,
                s => s.AddEmergencyWebApiHttpClient()),
            [WechatModule.Wedoc] = new WechatModuleRegistrar(
                WechatModule.Wedoc,
                s => s.AddWedocWebApiHttpClient()),
            [WechatModule.Checkin] = new WechatModuleRegistrar(
                WechatModule.Checkin,
                s => s.AddCheckinWebApiHttpClient()),
            [WechatModule.Schedule] = new WechatModuleRegistrar(
                WechatModule.Schedule,
                s => s.AddScheduleWebApiHttpClient()),
            [WechatModule.Meeting] = new WechatModuleRegistrar(
                WechatModule.Meeting,
                s => s.AddMeetingWebApiHttpClient()),
            [WechatModule.Wedrive] = new WechatModuleRegistrar(
                WechatModule.Wedrive,
                s => s.AddWedriveWebApiHttpClient()),
            [WechatModule.Living] = new WechatModuleRegistrar(
                WechatModule.Living,
                s => s.AddLivingWebApiHttpClient()),
            [WechatModule.Agent] = new WechatModuleRegistrar(
                WechatModule.Agent,
                s => s.AddAgentWebApiHttpClient()),
            [WechatModule.JsSdk] = new WechatModuleRegistrar(
                WechatModule.JsSdk,
                s => s.AddJsSdkWebApiHttpClient()),
            [WechatModule.Basic] = new WechatModuleRegistrar(
                WechatModule.Basic,
                s => s.AddBasicWebApiHttpClient()),
        };

    /// <summary>注册授权流业务接口与授权编排服务（get_pre_auth_code / set_session_info / get_permanent_code / get_auth_info / get_customized_auth_url + 编排）。</summary>
    public WechatWorkServiceBuilder AddAuthenticationApi() => AddModule(WechatModule.Authentication);

    /// <summary>注册通讯录业务接口（成员/部门/标签/通讯录查看权限/异步导入/异步导出六域）。</summary>
    public WechatWorkServiceBuilder AddContactApi() => AddModule(WechatModule.Contact);

    /// <summary>注册客户联系业务接口（企业服务人员管理域 + 客户管理域 + 客户标签管理域 + 在职继承域：公共面 + 第三方/代开发能力差异端点）。</summary>
    public WechatWorkServiceBuilder AddExternalContactApi() => AddModule(WechatModule.ExternalContact);

    /// <summary>注册上下游业务接口（基础接口 + 关联客户信息 + 上下游通讯录管理；自建/代开发两类应用）。</summary>
    public WechatWorkServiceBuilder AddCorpGroupApi() => AddModule(WechatModule.CorpGroup);

    /// <summary>注册安全管理业务接口（文件防泄漏 / 设备管理 / 截屏录屏管理 / 域名 IP 信息 / 高级功能账号管理 / 操作日志，官方仅向自建应用开放）。</summary>
    public WechatWorkServiceBuilder AddSecurityApi() => AddModule(WechatModule.Security);

    /// <summary>注册消息推送业务接口（发送应用消息 / 群聊会话 / 家校学校通知 / 智能表格自动化创建的群聊；template_msg 仅第三方差异端点，群聊会话与智能表格群聊官方仅自建开放，家校学校通知为三类应用公共面）。</summary>
    public WechatWorkServiceBuilder AddMessageApi() => AddModule(WechatModule.Message);

    /// <summary>注册账号ID业务接口（ID 转换 / tmp_external_userid 转换 / 自建应用对接 / corpid 转换 / ID 迁移完成状态 / 智能机器人 userid 转换 / 群 ID 升级，跨 access/provider/suite 三种令牌路由键七接口族）。</summary>
    public WechatWorkServiceBuilder AddAccountIdApi() => AddModule(WechatModule.AccountId);

    /// <summary>注册微信客服业务接口（客服账号管理域 + 接待人员管理域：三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    public WechatWorkServiceBuilder AddKfApi() => AddModule(WechatModule.Kf);

    /// <summary>注册身份验证业务接口（网页授权登录/企业微信Web登录身份获取域 + 二次验证域；第三方身份获取族独立套件令牌）。</summary>
    public WechatWorkServiceBuilder AddIdentityApi() => AddModule(WechatModule.Identity);

    /// <summary>注册企业支付业务接口（对外收款记录域为三类应用公共面；收款商户号管理域、资金流水域、创建对外收款账户域、普通支付域、退款域与交易账单域官方仅自建开放）。</summary>
    public WechatWorkServiceBuilder AddPayApi() => AddModule(WechatModule.Pay);

    /// <summary>注册会话内容存档业务接口（开启成员列表域、机器人信息域、会话同意情况域与内部群信息域，官方仅自建开放；access_token 须由会话内容存档应用 secret 获取）。</summary>
    public WechatWorkServiceBuilder AddMsgAuditApi() => AddModule(WechatModule.MsgAudit);

    /// <summary>注册家校沟通业务接口（家校沟通基础域为三类应用公共面；家校管理配置域官方仅自建与第三方开放，不设代开发子接口）。</summary>
    public WechatWorkServiceBuilder AddSchoolApi() => AddModule(WechatModule.School);

    /// <summary>注册素材管理业务接口（上传临时素材 / 获取临时素材 / 上传图片 / 获取高清语音素材 / 异步上传临时素材为三类应用公共面；服务商上传临时素材官方仅第三方开放，走 provider_access_token 独立成族）。</summary>
    public WechatWorkServiceBuilder AddMediaApi() => AddModule(WechatModule.Media);

    /// <summary>注册电子发票业务接口（查询电子发票 / 更新发票状态 / 批量更新发票状态 / 批量查询电子发票，四端点为三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    public WechatWorkServiceBuilder AddInvoiceApi() => AddModule(WechatModule.Invoice);

    /// <summary>注册政民沟通业务接口（配置网格结构域与配置事件类别域为自建/代开发公共面；获取网格列表、巡查上报族与居民上报族官方仅自建开放，不设代开发子接口；第三方应用官方暂不支持）。</summary>
    public WechatWorkServiceBuilder AddGovApi() => AddModule(WechatModule.Gov);

    /// <summary>注册数据与智能专区业务接口（基础接口域为三类应用公共面 + 差异端点子接口：获取授权信息官方不支持自建、文档存档授权信息官方仅第三方；应用调用专区程序域为三类应用公共面 + 空标记子接口）。</summary>
    public WechatWorkServiceBuilder AddDataZoneApi() => AddModule(WechatModule.DataZone);

    /// <summary>注册审批业务接口（审批申请数据域为三类应用公共面 + 差异端点子接口：获取审批数据（旧）官方仅自建；审批模板域为三类应用公共面 + 差异端点子接口：创建/更新模板自建与代开发开放、复制/更新模板到企业官方仅第三方；假期管理域与审批流程引擎域为三类应用公共面 + 空标记子接口）。</summary>
    public WechatWorkServiceBuilder AddApprovalApi() => AddModule(WechatModule.Approval);

    /// <summary>注册邮件业务接口（应用邮箱侧：发送邮件族与获取接收的邮件族为三类应用公共面收敛父接口 + 空标记子接口，普通/日程/会议三端点共用 compose_send 路由；管理端侧：管理邮件群组/管理公共邮箱/高级功能账号/成员邮箱操作/其他邮件客户端登录设置五族官方仅自建开放，不设第三方/代开发子接口）。</summary>
    public WechatWorkServiceBuilder AddMailApi() => AddModule(WechatModule.Mail);

/// <summary>注册紧急通知业务接口（发起语音电话 + 获取接听状态官方仅自建应用开放，零端点父接口 + 仅自建子接口承载端点；不设第三方/代开发子接口）。</summary>
    public WechatWorkServiceBuilder AddEmergencyApi() => AddModule(WechatModule.Emergency);

    /// <summary>注册文档业务接口（管理文档族 + 管理文档内容族 + 管理表格内容族 + 管理智能表格内容族 + 管理智能文档内容族：三类应用公共面收敛父接口 + 空标记子接口；编辑文档内容与编辑表格内容为批量更新形态，单次操作数量官方分别限制 30 与 5；管理智能表格内容族为 20 个端点，子表/视图/字段/记录/编组各 4 个；管理智能文档内容族为 17 个端点，发布与可见范围 3 / 页面 4 / 内容块 4 / 导出 2 / 数据表 4）。</summary>
    public WechatWorkServiceBuilder AddWedocApi() => AddModule(WechatModule.Wedoc);

    /// <summary>注册打卡业务接口（打卡规则族：获取员工打卡规则三类公共，获取企业所有打卡规则与管理打卡规则 4 写端点自建/代开发开放、第三方暂不支持；打卡记录族 + 打卡报表族：获取打卡记录/日报/月报三类开放但第三方文档页为旧字段结构，同路由不同构由子接口分形态承载，补卡/添加打卡记录/录入人脸官方仅自建；打卡排班族 + 设备打卡数据族：三类应用公共面收敛父接口 + 空标记子接口，设备打卡数据路由挂 /cgi-bin/hardware/ 域）。</summary>
    public WechatWorkServiceBuilder AddCheckinApi() => AddModule(WechatModule.Checkin);

    /// <summary>注册日程业务接口（管理日历族：创建/更新/获取/删除日历 4 端点为三类应用公共面收敛父接口 + 空标记子接口；创建日历路由官方即 calendar/add，更新操作为覆盖式而非增量式）。</summary>
    public WechatWorkServiceBuilder AddScheduleApi() => AddModule(WechatModule.Schedule);

    /// <summary>注册会议业务接口（预约会议基础管理族：创建/修改/取消/获取成员会议 ID 列表 4 端点为三类应用公共面收敛父接口，获取会议详情为自建/第三方差异端点、代开发零端点空标记；会议统计管理族：获取会议发起记录官方仅自建开放，零端点父接口 + 仅自建子接口承载）。</summary>
    public WechatWorkServiceBuilder AddMeetingApi() => AddModule(WechatModule.Meeting);

    /// <summary>注册微盘业务接口（管理空间族 4 端点 + 管理空间权限族 5 端点 + 管理文件族 11 端点 + 管理文件权限族 6 端点 + 版本和容量管理族 2 端点，均为三类应用公共面收敛父接口 + 空标记子接口；高级功能账号管理族 3 端点官方仅自建开放，零端点父接口 + 仅自建子接口承载、路由挂 vip/ 段；官方获取空间信息存在 space_info 旧版与 new_space_info 新版两条路由分挂两个分组，文件分块上传与版本容量均为一页多路由）。</summary>
    public WechatWorkServiceBuilder AddWedriveApi() => AddModule(WechatModule.Wedrive);

    /// <summary>注册直播业务接口（创建/修改/取消预约直播 + 删除直播回放 + 获取微信观看直播凭证 + 获取成员直播 ID 列表 + 获取直播详情 + 获取直播观看明细 + 获取跳转小程序商城的直播观众信息，9 端点为三类应用公共面收敛父接口 + 空标记子接口；获取直播详情官方即 GET，获取成员直播 ID 列表与删除直播回放两条路由与家校沟通·上课直播域共用）。</summary>
    public WechatWorkServiceBuilder AddLivingApi() => AddModule(WechatModule.Living);

    /// <summary>注册应用管理业务接口（获取应用族：agent/get + agent/list 两端点三类应用公共面收敛父接口 + 空标记子接口，设置应用官方仅企业可调用——第三方以及代开发自建应用不可调用、落自建差异端点；工作台自定义展示族 5 端点三类应用公共面收敛父接口 + 空标记子接口；自定义菜单族 3 端点官方仅自建应用开放，零端点父接口 + 仅自建子接口承载、agentid 走 Query；自建应用迁移成代开发应用族官方仅代开发章节提供但消费待迁移自建应用自身 access_token，落 Internal 归属域子接口、suite_access_token 为官方包体参数经请求体显式传入）。</summary>
    public WechatWorkServiceBuilder AddAgentApi() => AddModule(WechatModule.Agent);

    /// <summary>注册 JS-SDK 业务接口（获取企业 jsapi_ticket + 获取应用 jsapi_ticket 两端点三类应用公共面收敛父接口 + 空标记子接口，官方三份文档逐字一致；ticket/get 端点以固定 Query 值 type=agent_config 携带官方鉴权类型；jsapi_ticket 频率限制一小时内一个企业最多 400 次、单个应用不超过 100 次，官方要求后台缓存）。</summary>
    public WechatWorkServiceBuilder AddJsSdkApi() => AddModule(WechatModule.JsSdk);

    /// <summary>注册基础接口业务接口（获取企业微信接口IP段 + 获取企业微信回调IP段 2 端点为自建/代开发公共面收敛父接口 + 空标记子接口，官方权限说明均为「无限定」，两端点官方即 GET、无请求体；官方建议每天定时拉取 IP 段并更新防火墙设置；官方第三方应用开发文档树无「基础接口」分组，不设第三方子接口）。</summary>
    public WechatWorkServiceBuilder AddBasicApi() => AddModule(WechatModule.Basic);

    /// <summary>注册全部模块。</summary>
    public WechatWorkServiceBuilder AddAllApis()
    {
        foreach (var module in _registrars.Keys)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>按枚举注册模块。</summary>
    public WechatWorkServiceBuilder AddModules(params WechatModule[] modules)
    {
        if (modules == null)
        {
            throw new ArgumentNullException(nameof(modules));
        }

        foreach (var module in modules)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>注册自定义模块注册器（扩展点）。</summary>
    public WechatWorkServiceBuilder RegisterModule(IWechatModuleRegistrar registrar)
    {
        if (registrar == null)
        {
            throw new ArgumentNullException(nameof(registrar));
        }

        if (!_registrars.ContainsKey(registrar.Module))
        {
            _registrars.Add(registrar.Module, registrar);
            registrar.Register(_services);
            _configuration.TryAdd(registrar.Module);
        }

        return this;
    }

    private WechatWorkServiceBuilder AddModule(WechatModule module)
    {
        if (_registrars.TryGetValue(module, out var registrar) && _configuration.TryAdd(module))
        {
            registrar.Register(_services);
        }

        return this;
    }

    /// <summary>
    /// 完成注册：校验必需服务、接入 errcode 令牌失效判定器并合并 AOT JsonContext。
    /// </summary>
    public IServiceCollection Build()
    {
        if (!_configuration.HasAnyService())
        {
            throw new InvalidOperationException(
                "至少需要添加一个服务，请使用相应的 Add 方法。" +
                "示例：services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());");
        }

        if (!_services.Any(s => s.ServiceType == typeof(IWechatAppManager)))
        {
            throw new InvalidOperationException(
                "未注册 IWechatAppManager。请在 AddWechatWorkServices 之前调用 AddWechatApp 注册企业微信应用配置。" +
                "示例：services.AddWechatApp(configuration, \"WechatApps\").AddWechatWorkServices(builder => builder.AddAuthenticationApi());");
        }

        // errcode 令牌失效判定器（Mud.HttpUtils v3.0.1）：TokenRecoveryOptions 编程式注入。
        _services.AddWechatTokenInvalidationDetector();

#if NET8_0_OR_GREATER
        WechatJsonResolverExtensions.ConfigureDataModelsResolver(_services);
#endif

        return _services;
    }
}

/// <summary>已注册模块集合（去重；对齐 <c>FeishuServiceConfiguration</c>）。</summary>
internal sealed class WechatServiceConfiguration
{
    private readonly HashSet<WechatModule> _modules = new();

    public bool TryAdd(WechatModule module) => _modules.Add(module);

    public bool HasAnyService() => _modules.Count > 0;
}
