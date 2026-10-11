// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 契约守卫测试（对齐 Feishu TokenMultiAppContractGuards 模式，详细设计 §14/M6）。
/// </summary>
public class WechatContractGuards
{
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

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(GetSolutionRoot(), $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    /// <summary>
    /// 契约守卫 G1：全仓库 Mud.HttpUtils 单一版本（防混版 TypeLoadException，
    /// 对齐 Feishu「契约守卫锁定全仓库单一版本」）。
    /// </summary>
    [Fact]
    public void MudHttpUtils_PackageReference_ShouldBeSingleVersion()
    {
        var root = GetSolutionRoot();
        var csprojFiles = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                        // .codeartsdoer 为本机工具临时目录（gitignored，含评测 harness 生成的临时 csproj），
                        // 契约面只约束仓库源码树 —— 与 verify-build.ps1 步骤 2 的排除口径一致，防本机假红。
                        && !f.Contains($"{Path.DirectorySeparatorChar}.codeartsdoer{Path.DirectorySeparatorChar}"))
            .ToList();

        csprojFiles.Should().NotBeEmpty();

        var versions = new HashSet<string>();
        var pattern = new System.Text.RegularExpressions.Regex(
            @"<PackageReference\s+Include=""(Mud\.HttpUtils(?:\.\w+)*)""\s+Version=""([^""]+)""",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        foreach (var file in csprojFiles)
        {
            var content = File.ReadAllText(file);
            foreach (System.Text.RegularExpressions.Match match in pattern.Matches(content))
            {
                versions.Add(match.Groups[2].Value);
            }
        }

        versions.Should().HaveCount(1, $"Mud.HttpUtils 全仓库必须锁定单一版本，实际：{string.Join(", ", versions)}");
    }

    /// <summary>
    /// 契约守卫 G2：配置 DTO 禁用 required（ConfigurationBinder 以 new T() 构造 → CS9035，
    /// 对齐 Feishu AOT-3）。
    /// </summary>
    [Fact]
    public void ConfigDtos_ShouldNotUseRequired()
    {
        var configTypes = new[] { typeof(WechatAppConfig) };
        foreach (var type in configTypes)
        {
            var requiredProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "RequiredMemberAttribute"))
                .ToList();

            requiredProps.Should().BeEmpty($"{type.Name} 不得声明 required 成员（配置绑定源生成器 CS9035）");
        }
    }

    /// <summary>
    /// 契约守卫 G3：WechatTokenTypes 常量采用 "Wechat." 前缀命名空间，
    /// 与通用 TokenTypes（"AccessToken"）隔离（详细设计 §4.2 / 关键设计决策）。
    /// </summary>
    [Fact]
    public void WechatTokenTypes_ShouldUseWechatPrefixedNamespace()
    {
        WechatTokenTypes.AccessToken.Should().Be("Wechat.AccessToken");
        WechatTokenTypes.ProviderAccessToken.Should().Be("Wechat.ProviderAccessToken");
        WechatTokenTypes.SuiteAccessToken.Should().Be("Wechat.SuiteAccessToken");

        // 与组件通用常量明确区隔。
        WechatTokenTypes.AccessToken.Should().NotBe(Mud.HttpUtils.TokenTypes.AccessToken);
    }

    /// <summary>
    /// 契约守卫 G4：令牌失效码集合与判定器集合一致（{40014,42001,42007,42009,42011}，
    /// 与《Mud.HttpUtils-企业微信SDK需要的改动.md》§3.1 保持一致）。
    /// </summary>
    [Fact]
    public void WechatErrorCodes_ShouldAlignWithDetectorCollection()
    {
        var expected = new[] { 40014, 42001, 42007, 42009, 42011 };
        var actual = new[]
        {
            WechatErrorCodes.InvalidAccessToken,
            WechatErrorCodes.ExpiredAccessToken,
            WechatErrorCodes.RelatedAccessTokenInvalid,
            WechatErrorCodes.InvalidSuiteAccessToken,
            WechatErrorCodes.InvalidProviderAccessToken,
        };

        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// 契约守卫 G5：Query 令牌注入仅允许既有官方契约接口（MUD005 已知接受风险面收敛）。
    /// </summary>
    [Fact]
    public void QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces()
    {
        var mainAssembly = typeof(WechatWorkServiceCollectionExtensions).Assembly;
        var interfaces = mainAssembly.GetTypes().Where(t => t.IsInterface).ToList();

        var queryInjectionInterfaces = interfaces
            .Where(i => i.GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>() is { } attr
                        && attr.InjectionMode == Mud.HttpUtils.TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();

        queryInjectionInterfaces.Should().BeEquivalentTo(
            new[]
            {
                nameof(IWechatWorkProviderAuthenticationService),
                // 成员管理域（Contact 模块）：官方契约 access_token 一律走 Query（MUD005 同源已知接受风险），
                // 公共父接口 + 三个应用类型子接口（自建/第三方/代开发）均声明同一 [Token]。
                nameof(IWechatWorkUsersService),
                nameof(IWechatWorkInternalUsersService),
                nameof(IWechatWorkThirdPartyUsersService),
                nameof(IWechatWorkProviderUsersService),
                // 部门管理域（Contact 模块）：同上，令牌路由键与注入方式与成员管理域完全一致。
                nameof(IWechatWorkDepartmentsService),
                nameof(IWechatWorkInternalDepartmentsService),
                nameof(IWechatWorkThirdPartyDepartmentsService),
                nameof(IWechatWorkProviderDepartmentsService),
                // 标签管理域（Contact 模块）：同上，7 个端点全部为三类应用公共面，父接口 + 三个空标记子接口。
                nameof(IWechatWorkTagsService),
                nameof(IWechatWorkInternalTagsService),
                nameof(IWechatWorkThirdPartyTagsService),
                nameof(IWechatWorkProviderTagsService),
                // 通讯录查看权限管理域（Contact 模块）：官方仅向自建应用开放，父接口零端点 + 仅自建子接口承载端点。
                nameof(IWechatWorkContactRulesService),
                nameof(IWechatWorkInternalContactRulesService),
                // 异步导入接口域（Contact 模块）：4 个端点为自建/第三方公共面，父接口 + 自建/第三方空标记子接口。
                nameof(IWechatWorkBatchService),
                nameof(IWechatWorkInternalBatchService),
                nameof(IWechatWorkThirdPartyBatchService),
                // 异步导出接口域（Contact 模块）：同上，5 个端点为三类应用公共面，父接口 + 三个空标记子接口。
                nameof(IWechatWorkExportService),
                nameof(IWechatWorkInternalExportService),
                nameof(IWechatWorkThirdPartyExportService),
                nameof(IWechatWorkProviderExportService),
                // 客户联系·企业服务人员管理域（ExternalContact 模块）：get_follow_user_list 为三类应用公共面
                // （父接口 + 三个应用类型子接口），第三方/代开发子接口各持 1 条差异端点。
                nameof(IWechatWorkExternalContactFollowUserService),
                nameof(IWechatWorkInternalExternalContactFollowUserService),
                nameof(IWechatWorkThirdPartyExternalContactFollowUserService),
                nameof(IWechatWorkProviderExternalContactFollowUserService),
                // 客户联系·客户管理域（ExternalContact 模块）：10 个端点为三类应用公共面
                // （父接口 + 三个应用类型子接口），第三方子接口另持 3 条身份转换差异端点。
                nameof(IWechatWorkExternalContactCustomerService),
                nameof(IWechatWorkInternalExternalContactCustomerService),
                nameof(IWechatWorkThirdPartyExternalContactCustomerService),
                nameof(IWechatWorkProviderExternalContactCustomerService),
                // 客户联系·客户标签管理域（ExternalContact 模块）：9 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactTagService),
                nameof(IWechatWorkInternalExternalContactTagService),
                nameof(IWechatWorkThirdPartyExternalContactTagService),
                nameof(IWechatWorkProviderExternalContactTagService),
                // 客户联系·在职继承域（ExternalContact 模块）：3 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactJobInheritanceService),
                nameof(IWechatWorkInternalExternalContactJobInheritanceService),
                nameof(IWechatWorkThirdPartyExternalContactJobInheritanceService),
                nameof(IWechatWorkProviderExternalContactJobInheritanceService),
                // 客户联系·离职继承域（ExternalContact 模块）：4 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactResignedInheritanceService),
                nameof(IWechatWorkInternalExternalContactResignedInheritanceService),
                nameof(IWechatWorkThirdPartyExternalContactResignedInheritanceService),
                nameof(IWechatWorkProviderExternalContactResignedInheritanceService),
                // 客户联系·客户群管理域（ExternalContact 模块）：3 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactGroupChatService),
                nameof(IWechatWorkInternalExternalContactGroupChatService),
                nameof(IWechatWorkThirdPartyExternalContactGroupChatService),
                nameof(IWechatWorkProviderExternalContactGroupChatService),
                // 客户联系·「联系我」与客户入群方式域（ExternalContact 模块）：10 个端点为三类应用公共面
                //（联系我管理 92228/95724/96348 + 客户群加入群聊管理 92229/99546/99547；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactContactWayService),
                nameof(IWechatWorkInternalExternalContactContactWayService),
                nameof(IWechatWorkThirdPartyExternalContactContactWayService),
                nameof(IWechatWorkProviderExternalContactContactWayService),
                // 客户联系·客户朋友圈域（ExternalContact 模块）：14 个端点为三类应用公共面
                //（发表 95094/95095/96351 与 97612/97616/97615、数据 93333/93443/96352、
                // 规则组 94890/99541/99545；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactMomentService),
                nameof(IWechatWorkInternalExternalContactMomentService),
                nameof(IWechatWorkThirdPartyExternalContactMomentService),
                nameof(IWechatWorkProviderExternalContactMomentService),
                // 客户联系·获客助手域（ExternalContact 模块）：9 个端点为三类应用公共面
                //（链接管理 97297/97394/97398、客户列表 97298/97395/97399、额度统计 97375/97396/97400、
                // 收消息详情 100130/100134/100133；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactCustomerAcquisitionService),
                nameof(IWechatWorkInternalExternalContactCustomerAcquisitionService),
                nameof(IWechatWorkThirdPartyExternalContactCustomerAcquisitionService),
                nameof(IWechatWorkProviderExternalContactCustomerAcquisitionService),
                // 客户联系·消息推送（群发）域（ExternalContact 模块）：11 个端点为三类应用公共面
                //（企业群发 92135/92698/96366、97610/97613/97618、97611/97614/97619，群发记录 93338/93439/96355，
                // 欢迎语 92137/92599/96356，入群欢迎语素材 92366/93438/96357；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactGroupMsgService),
                nameof(IWechatWorkInternalExternalContactGroupMsgService),
                nameof(IWechatWorkThirdPartyExternalContactGroupMsgService),
                nameof(IWechatWorkProviderExternalContactGroupMsgService),
                // 客户联系·统计管理域（ExternalContact 模块）：3 个端点为三类应用公共面
                //（联系客户统计 92132/92275/96359、群聊统计 92133/93476/96358；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactStatisticsService),
                nameof(IWechatWorkInternalExternalContactStatisticsService),
                nameof(IWechatWorkThirdPartyExternalContactStatisticsService),
                nameof(IWechatWorkProviderExternalContactStatisticsService),
                // 客户联系·商品图册域（ExternalContact 模块）：5 个端点为三类应用公共面
                //（95096/95131/96345；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactProductAlbumService),
                nameof(IWechatWorkInternalExternalContactProductAlbumService),
                nameof(IWechatWorkThirdPartyExternalContactProductAlbumService),
                nameof(IWechatWorkProviderExternalContactProductAlbumService),
                // 客户联系·聊天敏感词域（ExternalContact 模块）：5 个端点为三类应用公共面
                //（95097/95130/96346，其中 get_intercept_rule_list 为 GET；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactInterceptRuleService),
                nameof(IWechatWorkInternalExternalContactInterceptRuleService),
                nameof(IWechatWorkThirdPartyExternalContactInterceptRuleService),
                nameof(IWechatWorkProviderExternalContactInterceptRuleService),
                // 客户联系·上传附件资源域（ExternalContact 模块）：1 个 multipart 端点为三类应用公共面
                //（95098/95178/96347，路由在 /cgi-bin/media/ 下；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactAttachmentService),
                nameof(IWechatWorkInternalExternalContactAttachmentService),
                nameof(IWechatWorkThirdPartyExternalContactAttachmentService),
                nameof(IWechatWorkProviderExternalContactAttachmentService),
                // 客户联系·获取已服务的外部联系人域（ExternalContact 模块）：1 个端点官方仅自建开放
                //（99434，第三方/代开发暂不支持；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkExternalContactServedContactService),
                nameof(IWechatWorkInternalExternalContactServedContactService),
                // 客户联系·获客助手组件域（ExternalContact 模块）：6 个端点官方仅第三方应用开放
                //（组件授权信息 99610、链接管理 99484、使用详情 99483、代支付 key 99603、收消息数据 100135；
                // 零端点父接口 + 仅第三方子接口）。
                nameof(IWechatWorkExternalContactAcquisitionComponentService),
                nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService),
                // 客户联系·获客助手组件域·代支付流水接口族（ExternalContact 模块）：1 个端点官方仅第三方应用开放
                //（99602，suite_access_token 鉴权、路由在 /cgi-bin/service/ 下，独立令牌路由键成族；
                // 零端点父接口 + 仅第三方子接口）。
                nameof(IWechatWorkExternalContactAcquisitionComponentBillService),
                nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService),
                // 上下游域（CorpGroup 模块）：公共面 1 端点在父接口 + 官方无第三方文档的 5 端点在自建代开发公共父接口
                //（第三方仅获取应用共享信息 95324，其子接口不继承自建代开发公共父接口）。
                nameof(IWechatWorkCorpGroupService),
                nameof(IWechatWorkCorpGroupInternalProviderService),
                nameof(IWechatWorkInternalCorpGroupService),
                nameof(IWechatWorkThirdPartyCorpGroupService),
                nameof(IWechatWorkProviderCorpGroupService),
                // 上下游通讯录管理域（CorpGroup 模块）：公共读取面在父接口，写入端点仅自建子接口，代开发空标记。
                nameof(IWechatWorkCorpGroupContactsService),
                nameof(IWechatWorkInternalCorpGroupContactsService),
                nameof(IWechatWorkProviderCorpGroupContactsService),
                // 上下游规则域（CorpGroup 模块）：官方仅向自建开放，父接口零端点 + 仅自建子接口承载端点。
                nameof(IWechatWorkCorpGroupRulesService),
                nameof(IWechatWorkInternalCorpGroupRulesService),
                // 安全管理域（Security 模块）：官方仅向自建开放，三接口族均为父接口零端点 + 仅自建子接口承载端点
                //（文件防泄漏 98079 / 设备管理 98920 / 截屏录屏 100128 / 域名 IP 100079 / 高级功能账号 99503、99505、99506 / 操作日志 100178、100179）。
                nameof(IWechatWorkSecurityService),
                nameof(IWechatWorkInternalSecurityService),
                nameof(IWechatWorkSecurityVipService),
                nameof(IWechatWorkInternalSecurityVipService),
                nameof(IWechatWorkSecurityOperLogService),
                nameof(IWechatWorkInternalSecurityOperLogService),
                // 消息推送域（Message 模块）：发送应用消息族为三类应用公共面（发送应用消息 90236/90372/96458、
                // 更新模版卡片 94888/94945/96459、撤回 94867/94947/96460；template_msg 仅第三方子接口差异端点 94515）；
                // 群聊会话族、家校学校通知族与智能表格自动化创建的群聊族官方仅自建开放（父接口零端点 + 仅自建子接口承载端点；
                // 群聊会话 90245/98913/98914/90248，学校通知 91609，智能表格群聊 100989/101028/101029）。
                nameof(IWechatWorkMessageService),
                nameof(IWechatWorkInternalMessageService),
                nameof(IWechatWorkThirdPartyMessageService),
                nameof(IWechatWorkProviderMessageService),
                nameof(IWechatWorkAppChatService),
                nameof(IWechatWorkInternalAppChatService),
                // 消息推送·家校学校通知族（Message 模块）：8 个端点为三类应用公共面
                //（发送「学校通知」8 种 msgtype，自建 91609、第三方 92291、代开发 96720/96723；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkSchoolMessageService),
                nameof(IWechatWorkInternalSchoolMessageService),
                nameof(IWechatWorkThirdPartySchoolMessageService),
                nameof(IWechatWorkProviderSchoolMessageService),
                nameof(IWechatWorkSmartSheetGroupChatService),
                nameof(IWechatWorkInternalSmartSheetGroupChatService),
                // 账号ID域（AccountId 模块）：七接口族跨三种令牌路由键——ID 转换族 / tmp_external_userid 转换族 /
                // 自建应用对接族走企业级 access_token；corpid 转换族 / ID 迁移完成状态族 / 智能机器人 userid 转换族
                // 走 provider_access_token；群 ID 升级（新授权企业）族走 suite_access_token。
                nameof(IWechatWorkAccountIdService),
                nameof(IWechatWorkThirdPartyAccountIdService),
                nameof(IWechatWorkProviderAccountIdService),
                nameof(IWechatWorkAccountIdTmpExternalUserIdService),
                nameof(IWechatWorkInternalAccountIdTmpExternalUserIdService),
                nameof(IWechatWorkThirdPartyAccountIdTmpExternalUserIdService),
                nameof(IWechatWorkProviderAccountIdTmpExternalUserIdService),
                nameof(IWechatWorkAccountIdInteropService),
                nameof(IWechatWorkInternalAccountIdInteropService),
                nameof(IWechatWorkAccountIdCorpidService),
                nameof(IWechatWorkThirdPartyAccountIdCorpidService),
                nameof(IWechatWorkProviderAccountIdCorpidService),
                nameof(IWechatWorkAccountIdMigrationService),
                nameof(IWechatWorkThirdPartyAccountIdMigrationService),
                nameof(IWechatWorkProviderAccountIdMigrationService),
                nameof(IWechatWorkAccountIdBotService),
                nameof(IWechatWorkThirdPartyAccountIdBotService),
                nameof(IWechatWorkProviderAccountIdBotService),
                nameof(IWechatWorkAccountIdChatIdUpgradeService),
                nameof(IWechatWorkProviderAccountIdChatIdUpgradeService),
                // 微信客服·客服账号管理域（Kf 模块）：5 个端点为三类应用公共面
                //（添加 94662/96404、列表 94661/96415（官方即 POST）、删除 94663/96405、
                // 修改 94664/96406、获取客服账号链接 94665/96416；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkKfAccountService),
                nameof(IWechatWorkInternalKfAccountService),
                nameof(IWechatWorkThirdPartyKfAccountService),
                nameof(IWechatWorkProviderKfAccountService),
                // 微信客服·接待人员管理域（Kf 模块）：3 个端点为三类应用公共面
                //（添加 94646/96418、删除 94647/96419、列表 94645/96420（GET，open_kfid 走 Query）；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkKfServicerService),
                nameof(IWechatWorkInternalKfServicerService),
                nameof(IWechatWorkThirdPartyKfServicerService),
                nameof(IWechatWorkProviderKfServicerService),
                // 身份验证域（Identity 模块）：网页授权/Web 登录身份获取族为自建+代开发公共面
                //（getuserinfo 91023/96442/98176/98177 GET（code 走 Query）、getuserdetail 95833/96443 POST；
                // 父接口 + 自建/代开发空标记子接口，第三方应用官方走独立路由与套件令牌，不设第三方子接口）；
                // 第三方套件级身份获取族走 suite_access_token（getuserinfo3rd 91121/98179、getuserdetail3rd 91122；
                // 零端点父接口 + 仅第三方子接口承载）；二次验证族官方仅「通讯录同步」或自建应用开放
                //（get_tfa_info 99499、tfa_succ 99500；零端点父接口 + 仅自建子接口承载；
                // authsucc 99521 已由通讯录成员域承载，不重复开放）。
                nameof(IWechatWorkIdentityService),
                nameof(IWechatWorkInternalIdentityService),
                nameof(IWechatWorkProviderIdentityService),
                nameof(IWechatWorkIdentitySuiteService),
                nameof(IWechatWorkThirdPartyIdentitySuiteService),
                nameof(IWechatWorkIdentityTfaService),
                nameof(IWechatWorkInternalIdentityTfaService),
                // 身份验证·小程序登录族（Identity 模块）：code2Session 自建/代开发 1 端点公共面收敛父接口
                //（自建 91507、代开发 96959，GET，js_code 与固定 grant_type 走 Query；父接口 + 两个空标记子接口）；
                // 第三方为独立路由（service/miniprogram/jscode2session 92423）且走 suite_access_token
                // 令牌路由键，零端点套件父接口 + 仅第三方子接口承载（响应多 open_userid 字段）。
                nameof(IWechatWorkIdentityMiniProgramService),
                nameof(IWechatWorkInternalIdentityMiniProgramService),
                nameof(IWechatWorkProviderIdentityMiniProgramService),
                nameof(IWechatWorkIdentityMiniProgramSuiteService),
                nameof(IWechatWorkThirdPartyIdentityMiniProgramSuiteService),
                // 微信客服·会话分配与消息收发域（Kf 模块）：4 条路由 / 14 个端点方法为三类应用公共面
                //（会话状态 94669/94698/96425、发送消息 94677/94700/96427（10 种 msgtype 同路由多方法）、
                // 事件响应消息 95122/94910/96428（2 种 msgtype 同路由多方法）；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkKfSessionService),
                nameof(IWechatWorkInternalKfSessionService),
                nameof(IWechatWorkThirdPartyKfSessionService),
                nameof(IWechatWorkProviderKfSessionService),
                // 微信客服·客户基础信息域（Kf 模块）：1 个端点为三类应用公共面
                //（batchget 95159/95149/96429；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkKfCustomerService),
                nameof(IWechatWorkInternalKfCustomerService),
                nameof(IWechatWorkThirdPartyKfCustomerService),
                nameof(IWechatWorkProviderKfCustomerService),
                // 微信客服·「升级服务」配置域（Kf 模块）：3 个端点为三类应用公共面
                //（get_upgrade_service_config（GET）/ upgrade_service / cancel_upgrade_service 94674/94702/96422；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkKfUpgradeService),
                nameof(IWechatWorkInternalKfUpgradeService),
                nameof(IWechatWorkThirdPartyKfUpgradeService),
                nameof(IWechatWorkProviderKfUpgradeService),
                // 微信客服·统计管理域（Kf 模块）：2 个端点为三类应用公共面
                //（企业汇总 95489/95492/96432、接待人员明细 95490/95493/96433；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkKfStatisticsService),
                nameof(IWechatWorkInternalKfStatisticsService),
                nameof(IWechatWorkThirdPartyKfStatisticsService),
                nameof(IWechatWorkProviderKfStatisticsService),
                // 微信客服·机器人管理域（Kf 模块）：8 个端点官方仅自建应用开放
                //（知识库分组 95971、知识库问答 95972，路由在 kf/knowledge/ 下；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkKfKnowledgeService),
                nameof(IWechatWorkInternalKfKnowledgeService),
                // 微信客服·微信客服组件域（Kf 模块）：3 个端点官方仅由微信客服组件应用（套件形态）消费
                //（99368/99400/99367，前两条与客服账号管理域共用路由；零端点父接口 + 仅第三方子接口）。
                nameof(IWechatWorkKfComponentService),
                nameof(IWechatWorkThirdPartyKfComponentService),
                // 企业支付·对外收款记录域（Pay 模块）：2 个端点为三类应用公共面
                //（获取对外收款记录 93667/93727/96701、获取收款项目的商户单号 95944/95936/96702；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkPayBillService),
                nameof(IWechatWorkInternalPayBillService),
                nameof(IWechatWorkThirdPartyPayBillService),
                nameof(IWechatWorkProviderPayBillService),
                // 企业支付·收款商户号管理域（Pay 模块）：2 个端点官方仅自建应用开放
                //（查询商户号详情 + 设置商户号使用范围 93666；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkPayMerchantService),
                nameof(IWechatWorkInternalPayMerchantService),
                // 企业支付·资金流水域（Pay 模块）：1 个端点官方仅自建应用开放
                //（获取资金流水 98100；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkPayFundFlowService),
                nameof(IWechatWorkInternalPayFundFlowService),
                // 企业支付·创建对外收款账户域（Pay 模块）：3 个端点官方仅自建应用开放
                //（提交申请单 98973、查询申请单状态 98974、提交图片 98972；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkPayMchApplyService),
                nameof(IWechatWorkInternalPayMchApplyService),
                // 企业支付·普通支付域（Pay 模块）：4 个端点官方仅自建应用开放
                //（小程序下单 97322、查询订单 97323、关闭订单 97324、获取支付签名 98130；
                // 零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkPayOrderService),
                nameof(IWechatWorkInternalPayOrderService),
                // 企业支付·退款域（Pay 模块）：2 个端点官方仅自建应用开放
                //（申请退款 97333、查询退款 97352；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkPayRefundService),
                nameof(IWechatWorkInternalPayRefundService),
                // 企业支付·交易账单域（Pay 模块）：1 个端点官方仅自建应用开放
                //（交易账单申请 98115；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkPayTradeBillService),
                nameof(IWechatWorkInternalPayTradeBillService),
                // 会话内容存档·开启成员列表域（MsgAudit 模块）：1 个端点官方仅自建应用开放
                //（获取开启成员列表 91614；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkMsgAuditPermitUserService),
                nameof(IWechatWorkInternalMsgAuditPermitUserService),
                // 会话内容存档·机器人信息域（MsgAudit 模块）：1 个端点官方仅自建应用开放
                //（获取机器人信息 91774「获取会话内容」页内唯一 HTTP API，官方即 GET；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkMsgAuditRobotService),
                nameof(IWechatWorkInternalMsgAuditRobotService),
                // 会话内容存档·会话同意情况域（MsgAudit 模块）：2 个端点官方仅自建应用开放
                //（单聊同意 91782 ≤2500 次/分钟、群聊同意 91782 ≤1500 次/分钟；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkMsgAuditAgreeService),
                nameof(IWechatWorkInternalMsgAuditAgreeService),
                // 会话内容存档·内部群信息域（MsgAudit 模块）：1 个端点官方仅自建应用开放
                //（获取内部群信息 92951 ≤2000 次/分钟；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkMsgAuditGroupChatService),
                nameof(IWechatWorkInternalMsgAuditGroupChatService),
                // 家校沟通·家校沟通基础域（School 模块）：7 个端点为三类应用公共面
                //（「学校通知」二维码 92320/92197/96719、关注模式 92318/92290、班级群创建方式 92430、
                // 外部联系人 openid 转换 92323/92292/96721、可使用的家长范围 94895/94960/96725；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkSchoolService),
                nameof(IWechatWorkInternalSchoolService),
                nameof(IWechatWorkThirdPartySchoolService),
                nameof(IWechatWorkProviderSchoolService),
                // 家校沟通·家校管理配置域（School 模块）：3 个端点官方仅自建与第三方应用开放
                //（老师可查看班级模式 92652、手机号转外部联系人 ID 92506；零端点父接口 +
                // 自建/第三方子接口，官方未向代开发开放，不设代开发子接口）。
                nameof(IWechatWorkSchoolSettingService),
                nameof(IWechatWorkInternalSchoolSettingService),
                nameof(IWechatWorkThirdPartySchoolSettingService),
                // 家校沟通·学生与家长管理域（School 模块）：16 个端点为三类应用公共面
                //（学生增删改 92325~92327/92035/92039/92041/100145~100147、批量增删改学生 92328~92330/92037/92040/92042/100148~100150、
                // 家长增删改 92331~92333/92077/92079/92081/100151~100153、批量增删改家长 92334~92336/92078/92080/92082/100154~100156、
                // 读取学生或家长 92337/92038/96738、部门学生详情 92338/92043/96739、部门家长详情 92446/92627/96741、
                // 家校通讯录自动同步模式 92345/92083/100157；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkSchoolUserService),
                nameof(IWechatWorkInternalSchoolUserService),
                nameof(IWechatWorkThirdPartySchoolUserService),
                nameof(IWechatWorkProviderSchoolUserService),
                // 家校沟通·部门管理域（School 模块）：5 个端点为三类应用公共面
                //（创建部门 92340/92296/100158、更新部门 92341/92297/100159、删除部门 92342/92298/100160（GET，id 走 Query）、
                // 获取部门列表 92343/92299/96745（GET，id 走 Query 可选）、修改自动升年级配置 92949/92950/100161；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkSchoolDepartmentService),
                nameof(IWechatWorkInternalSchoolDepartmentService),
                nameof(IWechatWorkThirdPartySchoolDepartmentService),
                nameof(IWechatWorkProviderSchoolDepartmentService),
                // 家校沟通·网页授权登录域（School 模块）：自建/代开发 2 端点公共面收敛父接口
                //（获取访问用户身份 91707/96712、获取家校访问用户身份 95791/96715）；
                // 第三方为独立路由（getuserinfo3rd 91711、school/getuserinfo3rd 95790）且走 suite_access_token
                // 令牌路由键，独立成接口不继承公共父接口。
                nameof(IWechatWorkSchoolAuthService),
                nameof(IWechatWorkInternalSchoolAuthService),
                nameof(IWechatWorkProviderSchoolAuthService),
                nameof(IWechatWorkThirdPartySchoolAuthService),
                // 家校沟通·健康上报域（School 模块）：4 个端点官方仅自建应用开放
                //（使用统计 93676、任务 ID 列表 93677、任务详情 93678、用户填写答案 93679；
                // 第三方/代开发官方「暂不支持」，父接口 + 仅自建空标记子接口）。
                nameof(IWechatWorkSchoolHealthReportService),
                nameof(IWechatWorkInternalSchoolHealthReportService),
                // 家校沟通·上课直播域（School 模块）：7 个端点为三类应用公共面
                //（老师直播 ID 列表 93739/93856/97127、直播详情 93740/93857/97128（GET，livingid 走 Query）、
                // 观看/未观看统计 93741/93858/97129、93742/93859/97130、删除回放 93743/93860/97131、
                // 观看/未观看统计 V2 95793/95799/97132、95795/95800/97133；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkSchoolLivingService),
                nameof(IWechatWorkInternalSchoolLivingService),
                nameof(IWechatWorkThirdPartySchoolLivingService),
                nameof(IWechatWorkProviderSchoolLivingService),
                // 家校沟通·班级收款域（School 模块）：2 个端点官方仅自建与第三方应用开放
                //（学生付款结果 94470/94553、订单详情 94471/94554；代开发无服务端查询接口，
                // 父接口 + 自建/第三方空标记子接口）。
                nameof(IWechatWorkSchoolClassPayService),
                nameof(IWechatWorkInternalSchoolClassPayService),
                nameof(IWechatWorkThirdPartySchoolClassPayService),
                // 素材管理域（Media 模块）：公共面 6 端点为三类应用公共面收敛父接口 + 空标记子接口
                //（上传临时素材 90253/90389/96484、获取临时素材 90256/90390/96486、上传图片 90254/90392/96485、
                // 获取高清语音素材 90255/90391/96487、异步上传临时素材 96219/97126/96488）；
                // 服务商上传临时素材（99310，路由在 /cgi-bin/service/media/ 下）官方仅第三方应用开放、
                // 走 provider_access_token 鉴权，独立令牌路由键成族（零端点父接口 + 仅第三方子接口）。
                nameof(IWechatWorkMediaService),
                nameof(IWechatWorkInternalMediaService),
                nameof(IWechatWorkThirdPartyMediaService),
                nameof(IWechatWorkProviderMediaService),
                nameof(IWechatWorkServiceMediaService),
                nameof(IWechatWorkThirdPartyServiceMediaService),
                // 电子发票域（Invoice 模块）：4 个端点（查询电子发票 90284/90420/99451、更新发票状态 90285/90421/99452、
                // 批量更新发票状态 90286/90422/99453、批量查询电子发票 90287/90423/99454）为三类应用公共面
                // 收敛父接口 + 空标记子接口，均走 access_token。
                nameof(IWechatWorkInvoiceService),
                nameof(IWechatWorkInternalInvoiceService),
                nameof(IWechatWorkThirdPartyInvoiceService),
                nameof(IWechatWorkProviderInvoiceService),
                // 政民沟通·配置网格结构域（Gov 模块）：4 个端点为自建/代开发公共面
                //（添加网格 94478/97136、编辑网格 94479/97137、删除网格 94480/97138、
                // 获取用户负责及参与的网格列表 94482/97140；第三方应用官方「暂不支持」，
                // 父接口 + 自建/代开发空标记子接口）。
                nameof(IWechatWorkGovGridService),
                nameof(IWechatWorkInternalGovGridService),
                nameof(IWechatWorkProviderGovGridService),
                // 政民沟通·获取网格列表（Gov 模块）：1 个端点官方仅自建应用开放
                //（94481；代开发/第三方官方「暂不支持」，零端点父接口 + 仅自建空标记子接口）。
                nameof(IWechatWorkGovGridListService),
                nameof(IWechatWorkInternalGovGridListService),
                // 政民沟通·配置事件类别域（Gov 模块）：4 个端点为自建/代开发公共面
                //（添加事件类别 94536/97141、修改事件类别 94537/97142、删除事件类别 94538/97143、
                // 获取事件类别列表 94540/99548；第三方应用官方「暂不支持」，
                // 父接口 + 自建/代开发空标记子接口）。
                nameof(IWechatWorkGovEventCategoryService),
                nameof(IWechatWorkInternalGovEventCategoryService),
                nameof(IWechatWorkProviderGovEventCategoryService),
                // 政民沟通·巡查上报族（Gov 模块）：6 个端点官方仅自建应用开放
                //（网格及负责人 93531、单位统计 93532、个人统计 93533、分类统计 93534、
                // 事件列表 93536、事件详情 93535；代开发/第三方官方「暂不支持」，
                // 零端点父接口 + 唯一自建子接口承载端点）。
                nameof(IWechatWorkGovPatrolService),
                nameof(IWechatWorkInternalGovPatrolService),
                // 政民沟通·居民上报族（Gov 模块）：6 个端点官方仅自建应用开放
                //（网格及负责人 93514、单位统计 93515、个人统计 93516、分类统计 93517、
                // 事件列表 93518、事件详情 93519；代开发/第三方官方「暂不支持」，
                // 零端点父接口 + 唯一自建子接口承载端点）。
                nameof(IWechatWorkGovResidentService),
                nameof(IWechatWorkInternalGovResidentService),
                // 数据与智能专区·基础接口域（DataZone 模块）：9 个端点为三类应用公共面
                //（设置公钥 99961/99845/100016、获取会话存档授权成员列表 99962/99846/100017、
                // 设置专区接收回调事件 99963/99850/100018、会话组件敏感信息隐藏设置 100139/100055/100054、
                // 设置/获取日志打印级别 100108/100106/100109、上传临时文件到专区 100174/100140/100175、
                // 获取文件内容存档授权成员列表 101873/101681/101882；
                // 父接口 + 自建空标记子接口；第三方/代开发子接口各持差异端点）。
                nameof(IWechatWorkDataZoneService),
                nameof(IWechatWorkInternalDataZoneService),
                nameof(IWechatWorkThirdPartyDataZoneService),
                nameof(IWechatWorkProviderDataZoneService),
                // 数据与智能专区·应用调用专区程序域（DataZone 模块）：3 个端点为三类应用公共面
                //（同步调用 99965/99811/100020、创建异步任务 + 查询结果 99966/99812/100021；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkDataZoneProgramService),
                nameof(IWechatWorkInternalDataZoneProgramService),
                nameof(IWechatWorkThirdPartyDataZoneProgramService),
                nameof(IWechatWorkProviderDataZoneProgramService),
                // 审批（Approval 模块）：审批申请数据域 3 个公共端点（91853/92632/96507、91816/94603/96509、
                // 91983/92634/96510）+ 自建差异端点获取审批数据（旧）91530；审批模板域获取模板详情
                //（91982/92631/96506）+ 创建/更新模板（97437/97438 自建、97439/97440 代开发，官方对第三方标注暂不支持）
                // + 复制/更新模板到企业（92630 官方仅第三方）；假期管理域 3 个公共端点
                //（93375~93377、94211~94213、96512~96514）；审批流程引擎域查询审批单当前状态（90269/93798/97114）。
                // 四族父接口 + 应用类型子接口共 16 个。
                nameof(IWechatWorkApprovalService),
                nameof(IWechatWorkInternalApprovalService),
                nameof(IWechatWorkProviderApprovalService),
                nameof(IWechatWorkThirdPartyApprovalService),
                nameof(IWechatWorkApprovalTemplateService),
                nameof(IWechatWorkInternalApprovalTemplateService),
                nameof(IWechatWorkProviderApprovalTemplateService),
                nameof(IWechatWorkThirdPartyApprovalTemplateService),
                nameof(IWechatWorkVacationService),
                nameof(IWechatWorkInternalVacationService),
                nameof(IWechatWorkProviderVacationService),
                nameof(IWechatWorkThirdPartyVacationService),
                nameof(IWechatWorkApprovalEngineService),
                nameof(IWechatWorkInternalApprovalEngineService),
                nameof(IWechatWorkProviderApprovalEngineService),
                nameof(IWechatWorkThirdPartyApprovalEngineService),
                // 邮件·发送邮件族（Mail 模块）：3 个端点为三类应用公共面
                //（发送普通邮件 97445/97515/97504、发送日程邮件 97854/97867/97865、发送会议邮件 97855/97868/97866；
                // 三端点共用 compose_send 路由；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkMailSendService),
                nameof(IWechatWorkInternalMailSendService),
                nameof(IWechatWorkThirdPartyMailSendService),
                nameof(IWechatWorkProviderMailSendService),
                // 邮件·获取接收的邮件族（Mail 模块）：2 个端点为三类应用公共面
                //（获取收件箱邮件列表 97369/97516/97505、获取邮件内容 97979/97983/97982；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkMailReceiveService),
                nameof(IWechatWorkInternalMailReceiveService),
                nameof(IWechatWorkThirdPartyMailReceiveService),
                nameof(IWechatWorkProviderMailReceiveService),
                // 邮件·管理应用邮箱账号族（Mail 模块）：2 个端点官方仅自建应用开放
                //（更新应用邮箱账号 97373、查询应用邮箱账号 97991；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkMailAccountService),
                nameof(IWechatWorkInternalMailAccountService),
                // 邮件·管理端五族（Mail 模块）：官方均仅自建应用开放
                //（管理邮件群组 95510/97995/97996/97997/97998、管理公共邮箱 95511/98000/98001/98002/98003/100183/100184、
                // 高级功能账号 99316/99317/99318、成员邮箱操作 95512/95514、其他邮件客户端登录设置 95513/98008；
                // 各族零端点父接口 + 仅自建子接口承载）。
                nameof(IWechatWorkMailGroupService),
                nameof(IWechatWorkInternalMailGroupService),
                nameof(IWechatWorkMailPublicMailService),
                nameof(IWechatWorkInternalMailPublicMailService),
                nameof(IWechatWorkMailVipService),
                nameof(IWechatWorkInternalMailVipService),
                nameof(IWechatWorkMailUserService),
                nameof(IWechatWorkInternalMailUserService),
                nameof(IWechatWorkMailUserOptionService),
                nameof(IWechatWorkInternalMailUserOptionService),
                // 紧急通知（Emergency 模块）：2 个端点官方仅自建应用开放
                //（发起语音电话 91627、获取接听状态 91628；代开发文档页 97115/97116 与自建页逐字一致
                // 但权限表对代开发/第三方均标注「暂不支持」；零端点父接口 + 仅自建子接口承载）。
                nameof(IWechatWorkEmergencyService),
                nameof(IWechatWorkInternalEmergencyService),
                // 文档·管理文档族（Wedoc 模块）：5 个端点为三类应用公共面
                //（新建文档 97460/97464/97470、重命名文档 97736/97745/97740、删除文档 97735/97746/97742、
                // 获取文档基础信息 97734/97747/97743、分享文档 97733/97748/97744；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkWedocService),
                nameof(IWechatWorkInternalWedocService),
                nameof(IWechatWorkThirdPartyWedocService),
                nameof(IWechatWorkProviderWedocService),
                // 文档·管理文档内容族（Wedoc 模块）：2 个端点为三类应用公共面
                //（编辑文档内容 97626/98027/98034、获取文档数据 101161/101188/101170；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkWedocDocumentService),
                nameof(IWechatWorkInternalWedocDocumentService),
                nameof(IWechatWorkThirdPartyWedocDocumentService),
                nameof(IWechatWorkProviderWedocDocumentService),
                // 文档·管理表格内容族（Wedoc 模块）：3 个端点为三类应用公共面
                //（编辑表格内容 101168/101190/101169、获取表格数据 97711/98031/98038、
                // 获取表格行列信息 97661/98030/98037；父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkWedocSpreadsheetService),
                nameof(IWechatWorkInternalWedocSpreadsheetService),
                nameof(IWechatWorkThirdPartyWedocSpreadsheetService),
                nameof(IWechatWorkProviderWedocSpreadsheetService),
                // 文档·管理智能表格内容族（Wedoc 模块）：20 个端点为三类应用公共面
                //（子表：添加 99896/100196/100214、删除 99899/100197/100215、更新 99898/100198/100216、查询 101154/101182/101164；
                //  视图：添加 99900/100199/100217、删除 99901/100200/100218、更新 99902/100201/100219、查询 101155/101183/101165；
                //  字段：添加 99904/100202/100220、删除 99905/100203/100221、更新 99906/100204/100222、查询 101157/100223/101166；
                //  记录：添加 99907/101184/100224、删除 99908/100206/100225、更新 99909/100207/100226、查询 101158/101185/101167；
                //  编组：添加 101100/101178/101174、删除 101102/101179/101175、更新 101101/101180/101176、获取 101103/101181/101177；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkWedocSmartSheetService),
                nameof(IWechatWorkInternalWedocSmartSheetService),
                nameof(IWechatWorkThirdPartyWedocSmartSheetService),
                nameof(IWechatWorkProviderWedocSmartSheetService),
                // 文档·管理智能文档内容族（Wedoc 模块）：17 个端点为三类应用公共面
                //（发布与可见范围：发布 101616/101633/101650、取消发布 101617/101634/101651、修改可查看范围 101618/101635/101652；
                //  页面：添加 101620/101637/101654、更新 101621/101638/101655、删除 101622/101639/101656、获取页面结构 101619/101636/101653；
                //  内容块：添加 101623/101640/101657、更新 101624/101641/101658、删除 101625/101642/101659、获取列表 101626/101643/101660；
                //  导出：提交任务与查询结果 101627/101644/101661（同一文档页两路由）；
                //  数据表：获取数据源 101628/101645/101662、添加 101629/101646/101663、更新 101631/101648/101665、删除 101630/101647/101664；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkWedocSmartDocService),
                nameof(IWechatWorkInternalWedocSmartDocService),
                nameof(IWechatWorkThirdPartyWedocSmartDocService),
                nameof(IWechatWorkProviderWedocSmartDocService),
                // 文档·设置文档权限族（Wedoc 模块）：9 个端点为三类应用公共面
                //（文档权限：获取权限信息 97461/97471、修改加入规则 97778/97792、修改成员与权限 97781/97795、修改安全设置 97782/97797；
                //  智能表格内容权限（同一文档页 5 路由）99935/100193：查询子表权限、更新子表权限、
                //  新增指定成员额外权限、更新指定成员额外权限、删除指定成员额外权限；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkWedocDocPermissionService),
                nameof(IWechatWorkInternalWedocDocPermissionService),
                nameof(IWechatWorkThirdPartyWedocDocPermissionService),
                nameof(IWechatWorkProviderWedocDocPermissionService),
                // 文档·管理收集表族（Wedoc 模块）：5 个端点为三类应用公共面
                //（创建 97462/97472、编辑 97816/97820、获取信息 97817/97821、统计查询 97818/97822、读取答案 97819/97823；
                // 父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkWedocFormService),
                nameof(IWechatWorkInternalWedocFormService),
                nameof(IWechatWorkThirdPartyWedocFormService),
                nameof(IWechatWorkProviderWedocFormService),
                // 打卡·打卡规则族（Checkin 模块）：获取员工打卡规则三类公共收敛父接口；
                // 获取企业所有打卡规则 + 管理打卡规则 4 写端点为自建/代开发差异端点，第三方暂不支持零端点空标记。
                nameof(IWechatWorkCheckinRuleService),
                nameof(IWechatWorkInternalCheckinRuleService),
                nameof(IWechatWorkProviderCheckinRuleService),
                nameof(IWechatWorkThirdPartyCheckinRuleService),
                // 打卡·打卡记录族（Checkin 模块）：获取打卡记录数据三类开放但第三方文档页为旧字段结构（同路由不同构，
                // 父接口零端点、三分支子接口各自承载）；补卡/添加打卡记录/录入人脸官方仅自建。
                nameof(IWechatWorkCheckinRecordService),
                nameof(IWechatWorkInternalCheckinRecordService),
                nameof(IWechatWorkProviderCheckinRecordService),
                nameof(IWechatWorkThirdPartyCheckinRecordService),
                // 打卡·打卡报表族（Checkin 模块）：日报/月报三类开放但第三方文档页为旧字段结构（同路由不同构，
                // 父接口零端点、三分支子接口各自承载）。
                nameof(IWechatWorkCheckinReportService),
                nameof(IWechatWorkInternalCheckinReportService),
                nameof(IWechatWorkProviderCheckinReportService),
                nameof(IWechatWorkThirdPartyCheckinReportService),
                // 打卡·打卡排班族（Checkin 模块）：获取/设置排班三类应用公共面收敛父接口 + 空标记子接口。
                nameof(IWechatWorkCheckinScheduleService),
                nameof(IWechatWorkInternalCheckinScheduleService),
                nameof(IWechatWorkProviderCheckinScheduleService),
                nameof(IWechatWorkThirdPartyCheckinScheduleService),
                // 打卡·设备打卡数据族（Checkin 模块）：获取设备打卡数据三类应用公共面收敛父接口 + 空标记子接口
                //（路由挂 /cgi-bin/hardware/ 域）。
                nameof(IWechatWorkCheckinDeviceService),
                nameof(IWechatWorkInternalCheckinDeviceService),
                nameof(IWechatWorkProviderCheckinDeviceService),
                nameof(IWechatWorkThirdPartyCheckinDeviceService),
                // 日程·管理日历族（Schedule 模块）：创建/更新/获取/删除日历 4 端点为三类应用公共面收敛父接口 +
                // 空标记子接口（创建日历 93647/93702/96823、更新日历 97716/97783/97758、获取日历详情 97717/97784/97759、
                // 删除日历 97718/97785/97760；创建日历路由官方即 calendar/add）。
                nameof(IWechatWorkScheduleCalendarService),
                nameof(IWechatWorkInternalScheduleCalendarService),
                nameof(IWechatWorkProviderScheduleCalendarService),
                nameof(IWechatWorkThirdPartyScheduleCalendarService),
                // 日程·管理日程族（Schedule 模块）：创建/更新日程、新增/删除日程参与者、获取日历下的日程列表、
                // 获取日程详情、取消日程 7 端点为三类应用公共面收敛父接口 + 空标记子接口
                //（创建日程 93648/93703/96824、更新日程 97720/97787/97761、新增日程参与者 97721/97789/97763、
                // 删除日程参与者 97722/97794/97764、获取日历下的日程列表 97723/97796/97765、
                // 获取日程详情 97724/97798/97766、取消日程 97725/97799/97767）。
                nameof(IWechatWorkScheduleService),
                nameof(IWechatWorkInternalScheduleService),
                nameof(IWechatWorkProviderScheduleService),
                nameof(IWechatWorkThirdPartyScheduleService),
                // 日程·待办族（Schedule 模块）：获取待办详情、更新待办状态 2 端点官方仅向企业自建应用开放
                //（第三方应用开发与服务商代开发均无对应 API），零端点父接口 + 仅自建子接口承载端点
                //（获取待办详情 101524、更新待办状态 101534；路由挂 /cgi-bin/todo/ 段）。
                nameof(IWechatWorkScheduleTodoService),
                nameof(IWechatWorkInternalScheduleTodoService),
                // 会议·预约会议基础管理族（Meeting 模块）：创建/修改/取消/获取成员会议 ID 列表 4 端点为三类应用公共面收敛父接口
                //（创建 99104/93706/97454、修改 99047/93710/97455、取消 99048/93709/97456、获取成员会议 ID 列表 99050/93707/97457；
                // 获取会议详情 99049/93708 为自建/第三方差异端点，代开发零端点空标记）。
                nameof(IWechatWorkMeetingService),
                nameof(IWechatWorkInternalMeetingService),
                nameof(IWechatWorkProviderMeetingService),
                nameof(IWechatWorkThirdPartyMeetingService),
                // 会议·会议统计管理族（Meeting 模块）：获取会议发起记录 99651 官方仅自建应用开放
                //（第三方/代开发章节未提供会议统计管理文档页；零端点父接口 + 仅自建子接口承载）。
                nameof(IWechatWorkMeetingStatisticsService),
                nameof(IWechatWorkInternalMeetingStatisticsService),
                // 会议·预约会议高级管理族（Meeting 模块）：19 端点官方仅自建应用开放
                //（第三方应用开发与服务商代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；
                // 创建/修改/取消预约会议、获取会议详情、获取成员会议 ID 列表 5 个文档页与基础管理族同路由，不重复建端点）。
                nameof(IWechatWorkMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService),
                // 会议·会中控制管理族（Meeting 模块）：17 端点官方仅自建应用开放
                //（第三方/代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；
                // 会控 9 端点挂 realcontrol 段、会议投票 8 端点挂 poll 段）。
                nameof(IWechatWorkMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService),
                // 会议·网络研讨会管理族（Meeting 模块）：14 端点官方仅自建应用开放
                //（第三方/代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；全部挂 webinar 段）。
                nameof(IWechatWorkMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService),
                // 会议·电话入会（PSTN）管理族（Meeting 模块）：3 端点官方仅自建应用开放
                //（第三方/代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；全部挂 phone 段）。
                nameof(IWechatWorkMeetingPstnService),
                nameof(IWechatWorkInternalMeetingPstnService),
                // 会议·Rooms 会议室管理族（Meeting 模块）：12 端点官方仅自建应用开放
                //（第三方/代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；全部挂 rooms 段，
                // 其中获取 Rooms 会议室资源为无请求体 POST）。
                nameof(IWechatWorkMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService),
                // 会议·会议室连接器（MRA）管理族（Meeting 模块）：4 端点官方仅自建应用开放
                //（第三方/代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；全部挂 mra 段）。
                nameof(IWechatWorkMeetingMraService),
                nameof(IWechatWorkInternalMeetingMraService),
                // 会议·会议布局和背景管理族（Meeting 模块）：15 端点官方仅自建应用开放
                //（第三方应用开发与服务商代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；
                // 基础布局与背景挂 layout 段、高级布局挂 advanced_layout 段；获取布局模板列表为本域唯一 GET 端点）。
                nameof(IWechatWorkMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService),
                // 会议·录制管理族（Meeting 模块）：10 端点官方仅自建应用开放
                //（第三方/代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；
                // 挂 record 段、录制转写挂 record/transcript 子段）。
                nameof(IWechatWorkMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService),
                // 会议·高级功能账号管理族（Meeting 模块）：3 个官方文档页承载 5 端点官方仅自建应用开放
                //（分配 99508 / 取消 99509 各含「提交任务」与「查询结果」两端点，查询账号列表 99510；
                // 第三方/代开发章节均无对应 API；零端点父接口 + 仅自建子接口承载；全部挂 vip 段）。
                nameof(IWechatWorkMeetingPremiumAccountService),
                nameof(IWechatWorkInternalMeetingPremiumAccountService),
                // 微盘·管理空间族（Wedrive 模块）：新建/重命名/解散空间 + 获取空间信息 4 端点为三类应用公共面收敛父接口 + 空标记子接口
                //（新建空间 93655/95857/96845、重命名空间 97856/97872/97862、解散空间 97857/97873/97863、
                // 获取空间信息（旧版 space_info）97858/97874/97864）。
                nameof(IWechatWorkWedriveSpaceService),
                nameof(IWechatWorkInternalWedriveSpaceService),
                nameof(IWechatWorkProviderWedriveSpaceService),
                nameof(IWechatWorkThirdPartyWedriveSpaceService),
                // 微盘·管理空间权限族（Wedrive 模块）：添加/移除成员部门 + 安全设置 + 获取邀请链接 + 获取空间信息（新版）5 端点
                // 为三类应用公共面收敛父接口 + 空标记子接口
                //（添加成员/部门 93656/95858/96846、移除成员/部门 97875/97947/97910、安全设置 97876/97948/97911、
                // 获取邀请链接 97877/97949/97912、获取空间信息（新版 new_space_info）97878/97950/97913）。
                nameof(IWechatWorkWedriveSpaceAclService),
                nameof(IWechatWorkInternalWedriveSpaceAclService),
                nameof(IWechatWorkProviderWedriveSpaceAclService),
                nameof(IWechatWorkThirdPartyWedriveSpaceAclService),
                // 微盘·管理文件族（Wedrive 模块）：9 页文档承载 11 条路由，为三类应用公共面收敛父接口 + 空标记子接口
                //（获取文件列表 93657/95859/96847、上传文件 97880/97951/97914、文件分块上传一页三路由
                // file_upload_init/file_upload_part/file_upload_finish 98004/98005/98007、下载文件 97881/97953/97915、
                // 新建文件夹/文档 97882/97954/97916、重命名文件 97883/97955/97917、移动文件 97884/97956/97918、
                // 删除文件 97885/97957/97919、获取文件信息 97886/97958/97920）。
                nameof(IWechatWorkWedriveFileService),
                nameof(IWechatWorkInternalWedriveFileService),
                nameof(IWechatWorkProviderWedriveFileService),
                nameof(IWechatWorkThirdPartyWedriveFileService),
                // 微盘·管理文件权限族（Wedrive 模块）：6 端点为三类应用公共面收敛父接口 + 空标记子接口
                //（新增成员 93658/95860/96848、删除成员 97888/97959/97922、分享设置 97889/97960/97923、
                // 获取分享链接 97890/97961/97924、获取文件权限信息 97891/97962/97925、修改文件安全设置 97892/97965/97926）。
                nameof(IWechatWorkWedriveFileAclService),
                nameof(IWechatWorkInternalWedriveFileAclService),
                nameof(IWechatWorkProviderWedriveFileAclService),
                nameof(IWechatWorkThirdPartyWedriveFileAclService),
                // 微盘·版本和容量管理族（Wedrive 模块）：官方单文档页承载 2 条路由，三类应用公共面收敛父接口 + 空标记子接口
                //（获取盘专业版信息 + 获取盘容量信息 95856/95861/96849；空请求体无请求 DTO）。
                nameof(IWechatWorkWedriveCapacityService),
                nameof(IWechatWorkInternalWedriveCapacityService),
                nameof(IWechatWorkProviderWedriveCapacityService),
                nameof(IWechatWorkThirdPartyWedriveCapacityService),
                // 微盘·高级功能账号管理族（Wedrive 模块）：3 端点官方仅自建应用开放（代开发/第三方标注「暂不支持」），
                // 零端点父接口 + 仅自建子接口承载；路由挂 /cgi-bin/wedrive/vip/ 段
                //（分配 99512、取消 99513、获取列表 99514）。
                nameof(IWechatWorkWedriveVipService),
                nameof(IWechatWorkInternalWedriveVipService),
                // 直播·直播管理域（Living 模块）：9 个端点为三类应用公共面收敛父接口 + 空标记子接口
                //（创建预约直播 93637/93717/96837、修改预约直播 93640/93720/96839、取消预约直播 93638/93718/96838、
                // 删除直播回放 93874/93719/96841、获取微信观看直播凭证 93641/93721/96840、
                // 获取成员直播 ID 列表 93634/93714/96834（官方即 POST）、获取直播详情 93635/93715/96835（官方即 GET，livingid 走 Query）、
                // 获取直播观看明细 93636/93716/96836、获取跳转小程序商城的直播观众信息 94442/94578/96843）。
                nameof(IWechatWorkLivingService),
                nameof(IWechatWorkInternalLivingService),
                nameof(IWechatWorkProviderLivingService),
                nameof(IWechatWorkThirdPartyLivingService),
                // 应用管理·获取应用族（Agent 模块）：agent/get + agent/list 两端点为三类应用公共面收敛父接口
                //（自建 90227 / 第三方 90363 / 代开发 96448，官方即 GET、agentid 走 Query）；
                // 设置应用 90228 官方仅企业可调用——第三方以及代开发自建应用不可调用，落自建差异端点。
                nameof(IWechatWorkAgentService),
                nameof(IWechatWorkInternalAgentService),
                nameof(IWechatWorkProviderAgentService),
                nameof(IWechatWorkThirdPartyAgentService),
                // 应用管理·工作台自定义展示族（Agent 模块）：设置/获取模版 + 设置/批量设置/获取用户数据 5 端点
                // 为三类应用公共面收敛父接口 + 空标记子接口
                //（自建 92535 / 第三方 94620 / 代开发 96454，官方全部即 POST）。
                nameof(IWechatWorkAgentWorkbenchService),
                nameof(IWechatWorkInternalAgentWorkbenchService),
                nameof(IWechatWorkProviderAgentWorkbenchService),
                nameof(IWechatWorkThirdPartyAgentWorkbenchService),
                // 应用管理·自定义菜单族（Agent 模块）：创建/获取/删除菜单 3 端点官方权限均为
                //「仅企业可调用；第三方不可调用」（代开发章节无对应 API），零端点父接口 + 仅自建子接口承载
                //（创建 90231、获取 90232、删除 90233；官方即 POST/GET/GET，agentid 走 Query）。
                nameof(IWechatWorkAgentMenuService),
                nameof(IWechatWorkInternalAgentMenuService),
                // 应用管理·自建应用迁移成代开发应用族（Agent 模块）：官方仅服务商代开发章节提供（99617），
                // 但消费待迁移自建应用自身 access_token（URL 参数，调用上下文须为自建应用），
                // 故零端点父接口 + 仅自建子接口承载；suite_access_token 为官方包体参数，不经 Query 注入。
                nameof(IWechatWorkAgentMigrationService),
                nameof(IWechatWorkInternalAgentMigrationService),
                // JS-SDK 域（JsSdk 模块）：获取企业 jsapi_ticket + 获取应用 jsapi_ticket 2 端点为三类应用公共面
                //（自建 90506 / 第三方 90539 / 代开发 96909，官方三份文档逐字一致；父接口 + 三个应用类型空标记子接口；
                // ticket/get 端点以方法级固定 Query type=agent_config 携带官方鉴权类型）。
                nameof(IWechatWorkJsSdkService),
                nameof(IWechatWorkInternalJsSdkService),
                nameof(IWechatWorkThirdPartyJsSdkService),
                nameof(IWechatWorkProviderJsSdkService),
                // 基础接口域（Basic 模块）：2 个端点为自建/代开发公共面收敛父接口 + 空标记子接口
                //（获取企业微信接口IP段 92520/97073、获取企业微信回调IP段 92521/98988，官方即 GET、无请求体；
                //  官方第三方应用开发文档树无「基础接口」分组，不设第三方子接口）。
                nameof(IWechatWorkBasicService),
                nameof(IWechatWorkInternalBasicService),
                nameof(IWechatWorkProviderBasicService),
                // 推广二维码域（PromotionQrCode 模块）：企业注册族 2 端点为第三方应用专有、走 provider_access_token
                //（获取注册码 90581、查询注册状态 90582；零端点父接口 + 唯一第三方子接口承载）。
                // 注意：同域「通讯录迁移」族（设置授权应用可见范围 90583 / 设置通讯录同步完成 90584）以显式
                // [Query("access_token")] 传令牌、不带 [Token]，故不进入本白名单（同 get_customized_auth_url 例外形态）。
                nameof(IWechatWorkServicePromotionQrCodeService),
                nameof(IWechatWorkThirdPartyServicePromotionQrCodeService),
                // 收银台域（PayTool 模块，官方仅第三方应用开发开放）：三族两令牌、各为「零端点父接口 + 唯一第三方子接口承载」。
                // 收款工具族 4 端点（创建收款订单 98045 / 取消收款订单 98046 / 获取收款订单列表 98053 / 获取收款订单详情 98054）
                // 与发票管理族 2 端点（获取发票列表 99436 / 标记开票状态 99437）走 provider_access_token；
                // 应用版本付费族 3 端点（获取订单列表 91910 / 获取订单详情 91909 / 延长试用期 91913）走 suite_access_token。
                nameof(IWechatWorkPayToolOrderService),
                nameof(IWechatWorkThirdPartyPayToolOrderService),
                nameof(IWechatWorkPayToolInvoiceService),
                nameof(IWechatWorkThirdPartyPayToolInvoiceService),
                nameof(IWechatWorkPayToolVersionSuiteService),
                nameof(IWechatWorkThirdPartyPayToolVersionSuiteService),
                // 接口调用许可域（License 模块，官方在第三方应用开发与服务商代开发两棵文档树开放、共享同一端点页）：
                // 四族 25 端点统一走 provider_access_token，各为「零端点父接口 + 唯一第三方子接口承载」。
                // 订单管理族 13 端点（97182/97183/97184/97185/97186/97187/98887/98888/99420）、
                // 账号管理族 9 端点（97188/97189/97190/97191/97192/97193）、
                // 应用管理族 1 端点（97194）、自动激活设置族 2 端点（97199/97200）。
                nameof(IWechatWorkLicenseOrderService),
                nameof(IWechatWorkThirdPartyLicenseOrderService),
                nameof(IWechatWorkLicenseAccountService),
                nameof(IWechatWorkThirdPartyLicenseAccountService),
                nameof(IWechatWorkLicenseAppService),
                nameof(IWechatWorkThirdPartyLicenseAppService),
                nameof(IWechatWorkLicenseAutoActiveService),
                nameof(IWechatWorkThirdPartyLicenseAutoActiveService),
                // 服务商登录授权域（Authentication 模块）：获取登录用户信息 91154 走服务商主体级
                // provider_access_token（Query 注入；单接口直接注册，无父/子接口分层）。
                nameof(IWechatWorkProviderLoginService),
                // 人事助手·花名册域（Hr 模块）：3 个端点官方仅自建应用开放
                //（字段配置 99131、花名册信息 99132、更新花名册 99133；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkHrService),
                nameof(IWechatWorkInternalHrService),
                // 公费电话域（Dial 模块）：1 个端点官方仅自建应用开放
                //（拨打记录 93662；零端点父接口 + 仅自建子接口）。
                nameof(IWechatWorkDialService),
                nameof(IWechatWorkInternalDialService),
            },
            "企业微信官方契约强制 Query 注入（MUD005 已知接受风险），新增 Query 注入接口须评估后扩展本守卫");
    }

    /// <summary>
    /// 契约守卫 G6（R14）：授权流端点路由表（新增 3 条，与 §4.5 接口声明一致），
    /// 并显式断言新增接口以 <c>[Query]</c> 显式传令牌、不携带 <c>[Token]</c>，
    /// 即 G5（Query 令牌注入白名单）<b>未放宽</b>。
    /// </summary>
    [Fact]
    public void AuthorizationEndpoints_ShouldMatchOfficialRoutes()
    {
        var expected = new (Type Interface, string Method, string Route)[]
        {
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetPermanentCodeV2Async),
                "/cgi-bin/service/v2/get_permanent_code"),
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetAuthInfoV2Async),
                "/cgi-bin/service/v2/get_auth_info"),
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetAppQrcodeAsync),
                "/cgi-bin/service/get_app_qrcode"),
            (typeof(IWechatWorkProviderAuthenticationUrl),
                nameof(IWechatWorkProviderAuthenticationUrl.GetCustomizedAuthUrlAsync),
                "/cgi-bin/service/get_customized_auth_url"),
        };

        foreach (var (iface, method, route) in expected)
        {
            var target = iface.GetMethod(method);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var post = target!.GetCustomAttribute<Mud.HttpUtils.Attributes.PostAttribute>();
            post.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [Post] 路由");
            post!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // R14：get_customized_auth_url 显式传 provider_access_token，不得进入 [Token] Query 注入白名单。
        typeof(IWechatWorkProviderAuthenticationUrl)
            .GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>()
            .Should().BeNull("G5 白名单仅 IWechatWorkProviderAuthenticationService，新增接口不得放宽");

        var providerTokenParam = typeof(IWechatWorkProviderAuthenticationUrl)
            .GetMethod(nameof(IWechatWorkProviderAuthenticationUrl.GetCustomizedAuthUrlAsync))!
            .GetParameters()[0];
        providerTokenParam.GetCustomAttribute<Mud.HttpUtils.Attributes.QueryAttribute>()!
            .Name.Should().Be("provider_access_token", "服务商令牌必须以显式 Query 参数传入");
    }

    /// <summary>
    /// 契约守卫 G7（P0-4）：Query 承载凭据的参数名必须已被组件脱敏词表覆盖，或在豁免清单中显式登记；
    /// 且豁免清单**自动过期**（组件词表一旦覆盖该键，豁免必须移除）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何不是「未覆盖即失败」</b>：组件（<c>Mud.HttpUtils</c>，独立仓库、NuGet 单一版本锁定）的词表补齐
    /// 属跨仓交付（C-01，已在组件 2.0.10 源码落地，并随 <b>3.0.0</b> 被本仓库消费），
    /// 跨仓期间无法在同一提交内使其转绿；若写成硬失败，则与「门禁必须全绿」的硬约束冲突。
    /// 故本守卫的职责是<b>可审计 + 自过期</b>：任何新增的 Query 凭据参数都必须做出「已覆盖 / 豁免（附追踪号）」决策。
    /// </para>
    /// <para>
    /// <b>自过期机制</b>：豁免项若已被组件词表覆盖，本守卫立即失败并要求清理 —— 升级
    /// <c>Mud.HttpUtils</c> 到含 C-01 的版本后，豁免清单**不能**被静默遗留（否则未来真实缺口会被掩盖）。
    /// </para>
    /// <para>
    /// <c>SensitiveUrlRedactor</c> 为组件 internal 类型，SDK 无法编译期引用 → 反射读取；
    /// 测试工程单 TFM net8.0 且不参与 AOT strict 冒烟（verify-build 步骤 2 排除 Tests），反射可接受。
    /// </para>
    /// </remarks>
    [Fact]
    public void QueryCredentialParams_ShouldBeRedactionRegisteredOrExplicitlyExempted()
    {
        // 豁免清单：每条 MUST 带追踪号与理由。
        // 【已清空】Mud.HttpUtils 3.0.0（含 C-01 词表补齐）已消费 ⇒ corpsecret / suite_access_token /
        // provider_access_token 均被组件词表覆盖，按本守卫的「自过期」机制清空（并由下方 stale 断言防止回归）。
        var exemptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var vocabulary = ReadComponentSensitiveVocabulary();
        vocabulary.Should().NotBeEmpty("未能读取组件脱敏词表（组件版本或字段名变更，请同步本守卫）");

        var stale = exemptions.Keys.Where(vocabulary.Contains).OrderBy(p => p, StringComparer.Ordinal).ToList();
        stale.Should().BeEmpty(
            "以下豁免项已被组件脱敏词表覆盖（组件 ≥ 2.0.10 含 C-01）⇒ 豁免已过期，请从本守卫的豁免清单中删除：" +
            string.Join(", ", stale));

        var uncovered = EnumerateCredentialQueryParamNames()
            .Where(p => !vocabulary.Contains(p) && !exemptions.ContainsKey(p))
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToList();

        uncovered.Should().BeEmpty(
            "以下 Query 参数承载凭据、既未被组件脱敏词表覆盖、也未登记豁免：会随 ApiException.RequestUri / " +
            "遥测 URL 明文外泄。请二选一：补齐组件词表（C-01）或在本守卫豁免清单登记（须附追踪号）：" +
            string.Join(", ", uncovered));
    }

    /// <summary>反射读取组件脱敏词表（单一事实源：<c>SensitiveUrlRedactor.SensitiveFieldNames</c>）。</summary>
    private static List<string> ReadComponentSensitiveVocabulary()
    {
        var abstractions = typeof(Mud.HttpUtils.ApiException).Assembly;
        var redactor = abstractions.GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（G7 依赖其词表）");

        var field = redactor!.GetField("SensitiveFieldNames",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull();

        var value = field!.GetValue(null) as System.Collections.IEnumerable;
        value.Should().NotBeNull();

        return value!.Cast<string>().ToList();
    }

    /// <summary>
    /// 枚举「Query 承载凭据」的参数名：扫描 SDK 全部接口，收集
    /// ① <c>[Query("x")]</c> 参数名；② <c>[Token(..., InjectionMode = Query, Name = "x")]</c> 的 Name；
    /// 再以「名称含 token / secret」过滤为凭据面。
    /// </summary>
    private static List<string> EnumerateCredentialQueryParamNames()
    {
        var assemblies = new[]
        {
            typeof(WechatWorkServiceCollectionExtensions).Assembly,
            typeof(Mud.Wechat.Work.Abstractions.WechatTokenTypes).Assembly,
        };

        var names = new List<string>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsInterface)
                {
                    continue;
                }

                foreach (var method in type.GetMethods())
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        if (parameter.GetCustomAttribute<Mud.HttpUtils.Attributes.QueryAttribute>() is { } query
                            && !string.IsNullOrEmpty(query.Name))
                        {
                            names.Add(query.Name!);
                        }
                    }
                }

                if (type.GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>() is { } token
                    && token.InjectionMode == Mud.HttpUtils.TokenInjectionMode.Query
                    && !string.IsNullOrEmpty(token.Name))
                {
                    names.Add(token.Name!);
                }
            }
        }

        return names
            .Where(n => n.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0
                        || n.IndexOf("secret", StringComparison.OrdinalIgnoreCase) >= 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// 契约守卫 G10（单一所有者不变量）：同一条官方路由在<b>跨接口</b>层面只允许存在<b>一处</b>声明，
    /// 合法重复仅两类，均须被本守卫显式豁免：
    /// <list type="bullet">
    /// <item><b>L1 类型化重载</b>：同一接口内多个方法共用一条路由（如 <c>/cgi-bin/message/send</c> 的
    /// text / image / markdown 等 msgtype 重载）——报文结构族相同、仅请求体形态不同。</item>
    /// <item><b>L2 令牌归属域多态</b>：同一接口族的「应用类型子接口」各声明一次，父接口不声明——
    /// 这是声明式令牌路由机制（<c>InternalAccessToken</c> 与 <c>CorpAccessToken</c> 须各绑定一个
    /// HttpClient），不是能力冗余。</item>
    /// </list>
    /// 其余任何跨接口重复（同路由被两个不同域/服务族各声明一次）即为架构缺陷：会产出两份平行 DTO 家族、
    /// 两套 JSON 上下文登记与两份升级维护面。
    /// </summary>
    [Fact]
    public void OfficialRoutes_ShouldHaveSingleDeclaringInterface()
    {
        // L2 白名单：接口族名 -> 允许重复声明的路由。这些路由由「应用类型子接口」各声明一次，
        // 承载的是令牌归属域多态（自建 vs 授权企业级），不得视为冗余。
        var tokenPolymorphicRoutes = new HashSet<string>(StringComparer.Ordinal)
        {
            // 审批模板管理（代开发与自建各一，父接口零端点）。
            "/cgi-bin/oa/approval/create_template",
            "/cgi-bin/oa/approval/update_template",
            "/cgi-bin/oa/approval/get_template_detail",
            "/cgi-bin/oa/approval/get_template_list",
            // 打卡记录与统计（自建 / 代开发 / 第三方各一；第三方响应为独立可空超集）。
            "/cgi-bin/checkin/getcheckindata",
            "/cgi-bin/checkin/getcheckin_daydata",
            "/cgi-bin/checkin/getcheckin_monthdata",
            "/cgi-bin/checkin/getcorpcheckinoption",
            "/cgi-bin/checkin/add_checkin_option",
            "/cgi-bin/checkin/update_checkin_option",
            "/cgi-bin/checkin/clear_checkin_option_array_field",
            "/cgi-bin/checkin/del_checkin_option",
            // 通讯录成员与部门（自建与第三方令牌归属域不同）。
            "/cgi-bin/user/create",
            "/cgi-bin/user/update",
            "/cgi-bin/user/delete",
            "/cgi-bin/user/batchdelete",
            "/cgi-bin/batch/invite",
            "/cgi-bin/department/create",
            "/cgi-bin/department/update",
            "/cgi-bin/department/delete",
            // 会议详情（自建与第三方令牌归属域不同）。
            "/cgi-bin/meeting/get_info",
            // 数据与智能专区授权信息（第三方与代开发均消费 CorpAccessToken，但官方开放面按应用类型
            // 分别表述——自建官方不支持，无法上提至公共父接口，故保留两处声明）。
            "/cgi-bin/chatdata/get_corp_auth_info",
            // 发送应用消息·模板消息（msgtype = template_msg）：父接口承载 11 种msgtype 类型化重载，
            // 官方仅向第三方应用开放 template_msg（文档 94515，自建/代开发无对应文档），
            // 故该重载声明在第三方子接口而非父接口——属 L1 类型化重载 + L2 开放面差异的叠加。
            "/cgi-bin/message/send",
        };

        var mainAssembly = typeof(WechatWorkServiceCollectionExtensions).Assembly;
        var declarations = mainAssembly.GetTypes()
            .Where(t => t.IsInterface)
            .SelectMany(i => i
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => (Interface: i, Method: m)))
            .SelectMany(x => x.Method.GetCustomAttributes()
                .OfType<Mud.HttpUtils.Attributes.HttpMethodAttribute>()
                .Where(a => !string.IsNullOrEmpty(a.RequestUri))
                .Select(a => (x.Interface, x.Method, Route: a.RequestUri!)))
            .ToList();

        var crossInterfaceDuplicates = declarations
            .GroupBy(d => d.Route, StringComparer.Ordinal)
            .Where(g => g.Select(d => d.Interface).Distinct().Count() > 1)
            .Where(g => !tokenPolymorphicRoutes.Contains(g.Key))
            .Select(g => (Route: g.Key, Interfaces: g.Select(d => d.Interface.Name).Distinct().OrderBy(n => n).ToList()))
            .OrderBy(x => x.Route, StringComparer.Ordinal)
            .ToList();

        crossInterfaceDuplicates.Should().BeEmpty(
            "以下官方路由被多个接口族重复声明，违反「单一所有者」不变量：" +
            Environment.NewLine +
            string.Join(Environment.NewLine,
                crossInterfaceDuplicates.Select(d => $"  {d.Route} → {string.Join(", ", d.Interfaces)}")) +
            Environment.NewLine +
            "处置：保留业务域归属方一处声明，删除其余重复声明；若确属令牌归属域多态（L2），" +
            "请显式登记到本守卫的 tokenPolymorphicRoutes 白名单并附理由。");
    }

    /// <summary>
    /// 契约守卫 G9（P0-3）：<c>cancel_auth</c> 的清理范围必须收敛到 SuiteId 命中集，
    /// 不得再次引入「未命中即回退全部应用」的越权删除（行为用例见
    /// <c>WechatCallbackAuthorizationDispatchTests</c>）。
    /// </summary>
    [Fact]
    public void CancelAuthCleanup_ShouldBeScopedToMatchedAppKeys()
    {
        var handlerPath = SourcePath(
            "Mud.Wechat.Work.Callback", "WechatCallbackHandler.cs");
        File.Exists(handlerPath).Should().BeTrue($"未找到回调处理器源码：{handlerPath}");

        var source = File.ReadAllText(handlerPath);

        source.Should().NotContain("ResolveAppKeys(",
            "G9：cancel_auth/change_auth 不得再经「未命中即回退全部应用」的 ResolveAppKeys 兜底");
        source.Should().Contain("已跳过授权清理以避免误删其它套件授权",
            "G9：未命中归属应用时必须走「只告警不删库」分支");
        source.Should().Contain("MatchAppKeysBySuiteId",
            "G9：清理范围必须恒为 SuiteId 命中集");
    }

    /// <summary>
    /// 契约守卫 G8-A（P0-1）：<c>IAppContextHolder</c> 必须与 <c>IWechatAppContextSwitcher</c> 同实例，
    /// 否则声明式（<c>[Token]</c>）客户端读到的环境上下文恒为 null（多套件静默回退默认应用令牌）。
    /// </summary>
    [Fact]
    public void AppContextHolder_ShouldBeSameInstanceAsSwitcher_InRegistrationSource()
    {
        var extensionsPath = SourcePath(
            "Mud.Wechat.Work.Abstractions", "Extensions", "WechatWorkMultiAppExtensions.cs");
        File.Exists(extensionsPath).Should().BeTrue();

        var source = File.ReadAllText(extensionsPath);

        var switcherIndex = source.IndexOf("TryAddSingleton<IWechatAppContextSwitcher", StringComparison.Ordinal);
        var loopIndex = source.IndexOf("services.AddMudHttpClient(", StringComparison.Ordinal);

        switcherIndex.Should().BeGreaterThan(0, "必须注册 IWechatAppContextSwitcher");
        loopIndex.Should().BeGreaterThan(0);
        switcherIndex.Should().BeLessThan(loopIndex,
            "G8-A：切换器必须在 AddMudHttpClient（其内部 TryAdd IAppContextHolder）之前注册，否则 TryAdd 失效");

        source.Should().Contain("TryAddSingleton<IAppContextHolder>(sp => sp.GetRequiredService<IWechatAppContextSwitcher>())",
            "G8-A：IAppContextHolder 必须委托到同一个切换器实例");
    }
}
