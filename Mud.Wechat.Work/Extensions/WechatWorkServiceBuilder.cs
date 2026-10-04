// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
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
            [WechatModule.Mail] = new WechatModuleRegistrar(
                WechatModule.Mail,
                s => s.AddMailWebApiHttpClient()),
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

    /// <summary>注册政民沟通业务接口（配置网格结构域与配置事件类别域为自建/代开发公共面；获取网格列表官方仅自建开放，不设代开发子接口；第三方应用官方暂不支持）。</summary>
    public WechatWorkServiceBuilder AddGovApi() => AddModule(WechatModule.Gov);

    /// <summary>注册数据与智能专区业务接口（基础接口域为三类应用公共面 + 差异端点子接口：获取授权信息官方不支持自建、文档存档授权信息官方仅第三方；应用调用专区程序域为三类应用公共面 + 空标记子接口）。</summary>
    public WechatWorkServiceBuilder AddDataZoneApi() => AddModule(WechatModule.DataZone);

    /// <summary>注册邮件业务接口（发送邮件族与获取接收的邮件族为三类应用公共面收敛父接口 + 空标记子接口，普通/日程/会议三端点共用 compose_send 路由；管理应用邮箱账号族官方仅自建开放，不设第三方/代开发子接口）。</summary>
    public WechatWorkServiceBuilder AddMailApi() => AddModule(WechatModule.Mail);

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

        // errcode 令牌失效判定器（Mud.HttpUtils v2.0.9）：TokenRecoveryOptions 编程式注入。
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
