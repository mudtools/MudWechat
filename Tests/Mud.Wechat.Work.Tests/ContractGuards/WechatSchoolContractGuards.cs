// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.School;
using Mud.Wechat.Work.DataModels.School.ClassPay;
using Mud.Wechat.Work.DataModels.School.HealthReport;
using Mud.Wechat.Work.DataModels.School.Living;
// LivingJsonContext 由 mud-jsonctx 落在多命名空间分组的字母序首个目录（DataModels.Living，见
// GenerateJsonContext.ps1 头注）。本文件已 using School.Living（承载上课直播域 DTO，其中 LivingInfo
// 与直播域 DataModels.Living.LivingInfo 同名），故以别名引用上下文类而非整命名空间导入，避免类型歧义。
using LivingJsonContext = Mud.Wechat.Work.DataModels.Living.LivingJsonContext;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 家校沟通模块（School 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （家校沟通基础域：三类应用公共面收敛父接口 + 空标记子接口；
/// 家校管理配置域：官方仅自建与第三方开放，不设代开发子接口；
/// 学生与家长管理域：16 个端点为三类应用公共面，收敛父接口 + 空标记子接口；
/// 部门管理域：5 个端点为三类应用公共面，收敛父接口 + 空标记子接口；
/// 网页授权登录域：自建/代开发公共面收敛父接口，第三方为独立路由且走 suite_access_token
/// 令牌路由键，独立成接口、不继承公共父接口；
/// 健康上报域：官方仅自建开放（第三方/代开发暂不支持），不设第三方与代开发子接口；
/// 上课直播域：7 个端点为三类应用公共面，收敛父接口 + 空标记子接口；
/// 班级收款域：官方仅自建与第三方开放（代开发无服务端查询接口），不设代开发子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：家校沟通基础域与学生与家长管理域对齐企业支付域对外收款记录族（三类公共面收敛）；
/// 家校管理配置域对齐通讯录异步导入域（仅自建 + 第三方子接口，官方未向代开发开放）；
/// 网页授权登录域对齐上下游域（差异面独立成族）与账号ID域群 ID 升级族
/// （suite_access_token 一接口族一令牌路由键，第三方接口不继承 access_token 父接口）。
/// </para>
/// <para>
/// 既有覆盖注记：发送「学校通知」（<c>externalcontact/message/send</c>）由消息推送域
/// 家校学校通知族承载（三类应用公共面）；获取外部联系人详情（<c>externalcontact/get</c>，
/// 家校版文档 91670）由客户联系域客户管理族承载，其响应中的家校字段
/// （is_subscribe / subscriber_info）已补入 <c>ExternalContactInfo</c>——两路由均不在本模块重复声明。
/// </para>
/// </remarks>
public class WechatSchoolContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string SchoolParentImplementationClassName = "WechatWorkSchoolService";

    private const string SchoolSettingParentImplementationClassName = "WechatWorkSchoolSettingService";

    private const string SchoolUserParentImplementationClassName = "WechatWorkSchoolUserService";

    private const string SchoolDepartmentParentImplementationClassName = "WechatWorkSchoolDepartmentService";

    private const string SchoolAuthParentImplementationClassName = "WechatWorkSchoolAuthService";

    private const string SchoolHealthReportParentImplementationClassName = "WechatWorkSchoolHealthReportService";

    private const string SchoolLivingParentImplementationClassName = "WechatWorkSchoolLivingService";

    private const string SchoolClassPayParentImplementationClassName = "WechatWorkSchoolClassPayService";

    private const string SchoolRegistryGroupName = "School";

    /// <summary>
    /// 家校沟通基础域官方路由表（三类应用公共面，7 条端点全部收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolRoutes =
    {
        // 获取「学校通知」二维码（自建 92320、第三方 92197、代开发 96719；官方即 GET）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetSubscribeQrCodeAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/get_subscribe_qr_code"),
        // 设置关注「学校通知」的模式（自建 92318、第三方 92290）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.SetSubscribeModeAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/set_subscribe_mode"),
        // 获取关注「学校通知」的模式（自建 92318、第三方 92290；官方即 GET）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetSubscribeModeAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/get_subscribe_mode"),
        // 获取「班级群创建方式」（92430；官方即 GET）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetChatCreateModeAsync),
            typeof(GetAttribute), "/cgi-bin/school/get_chat_create_mode"),
        // 设置「班级群创建方式」（92430）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.SetChatCreateModeAsync),
            typeof(PostAttribute), "/cgi-bin/school/set_chat_create_mode"),
        // 外部联系人 openid 转换（自建 92323、第三方 92292、代开发 96721）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.ConvertToOpenIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/convert_to_openid"),
        // 获取可使用的家长范围（自建 94895、第三方 94960、代开发 96725；官方即 GET，agentid 走 Query）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetAllowScopeAsync),
            typeof(GetAttribute), "/cgi-bin/school/agent/get_allow_scope"),
    };

    /// <summary>
    /// 家校管理配置域官方路由表（官方仅自建与第三方开放，3 条端点全部收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolSettingRoutes =
    {
        // 设置「老师可查看班级」的模式（92652）。
        (typeof(IWechatWorkSchoolSettingService),
            nameof(IWechatWorkSchoolSettingService.SetTeacherViewModeAsync),
            typeof(PostAttribute), "/cgi-bin/school/set_teacher_view_mode"),
        // 获取「老师可查看班级」的模式（92652；官方即 GET）。
        (typeof(IWechatWorkSchoolSettingService),
            nameof(IWechatWorkSchoolSettingService.GetTeacherViewModeAsync),
            typeof(GetAttribute), "/cgi-bin/school/get_teacher_view_mode"),
        // 手机号转外部联系人 ID（92506）。
        (typeof(IWechatWorkSchoolSettingService),
            nameof(IWechatWorkSchoolSettingService.BatchToExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/batch_to_external_userid"),
    };

    /// <summary>
    /// 学生与家长管理域官方路由表（三类应用公共面，16 条端点全部收敛父接口；
    /// 单个删除学生/家长官方即 GET、批量删除官方即 POST，勿「顺手统一」）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolUserRoutes =
    {
        // 创建学生（自建 92325、第三方 92035、代开发 100145）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.CreateStudentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/create_student"),
        // 删除学生（自建 92326、第三方 92039、代开发 100146；官方即 GET，userid 走 Query）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.DeleteStudentAsync),
            typeof(GetAttribute), "/cgi-bin/school/user/delete_student"),
        // 更新学生（自建 92327、第三方 92041、代开发 100147）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.UpdateStudentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/update_student"),
        // 批量创建学生（自建 92328、第三方 92037、代开发 100148）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.BatchCreateStudentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/batch_create_student"),
        // 批量删除学生（自建 92329、第三方 92040、代开发 100149；官方批量删除即 POST）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.BatchDeleteStudentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/batch_delete_student"),
        // 批量更新学生（自建 92330、第三方 92042、代开发 100150）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.BatchUpdateStudentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/batch_update_student"),
        // 创建家长（自建 92331、第三方 92077、代开发 100151）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.CreateParentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/create_parent"),
        // 删除家长（自建 92332、第三方 92079、代开发 100152；官方即 GET，userid 走 Query）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.DeleteParentAsync),
            typeof(GetAttribute), "/cgi-bin/school/user/delete_parent"),
        // 更新家长（自建 92333、第三方 92081、代开发 100153）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.UpdateParentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/update_parent"),
        // 批量创建家长（自建 92334、第三方 92078、代开发 100154）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.BatchCreateParentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/batch_create_parent"),
        // 批量删除家长（自建 92335、第三方 92080、代开发 100155；官方批量删除即 POST）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.BatchDeleteParentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/batch_delete_parent"),
        // 批量更新家长（自建 92336、第三方 92082、代开发 100156）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.BatchUpdateParentAsync),
            typeof(PostAttribute), "/cgi-bin/school/user/batch_update_parent"),
        // 读取学生或家长（自建 92337、第三方 92038、代开发 96738；官方即 GET，userid 走 Query）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.GetSchoolUserAsync),
            typeof(GetAttribute), "/cgi-bin/school/user/get"),
        // 获取部门学生详情（自建 92338、第三方 92043、代开发 96739；官方即 GET，
        // department_id 走 Query，官方无 cursor/limit 分页）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.ListDepartmentStudentsAsync),
            typeof(GetAttribute), "/cgi-bin/school/user/list"),
        // 获取部门家长详情（自建 92446、第三方 92627、代开发 96741；官方即 GET，
        // department_id 走 Query，官方无 cursor/limit 分页）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.ListDepartmentParentsAsync),
            typeof(GetAttribute), "/cgi-bin/school/user/list_parent"),
        // 设置家校通讯录自动同步模式（自建 92345、第三方 92083、代开发 100157；设置后不可逆）。
        (typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.SetArchSyncModeAsync),
            typeof(PostAttribute), "/cgi-bin/school/set_arch_sync_mode"),
    };

    /// <summary>
    /// 部门管理域官方路由表（三类应用公共面，5 条端点全部收敛父接口；
    /// 删除/列表官方即 GET 且 id 走 Query，删除接口官方参数表将 id 标注为「否」属官方原文，照抄不纠正）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolDepartmentRoutes =
    {
        // 创建部门（自建 92340、第三方 92296、代开发 100158）。
        (typeof(IWechatWorkSchoolDepartmentService),
            nameof(IWechatWorkSchoolDepartmentService.CreateDepartmentAsync),
            typeof(PostAttribute), "/cgi-bin/school/department/create"),
        // 更新部门（自建 92341、第三方 92297、代开发 100159）。
        (typeof(IWechatWorkSchoolDepartmentService),
            nameof(IWechatWorkSchoolDepartmentService.UpdateDepartmentAsync),
            typeof(PostAttribute), "/cgi-bin/school/department/update"),
        // 删除部门（自建 92342、第三方 92298、代开发 100160；官方即 GET，id 走 Query）。
        (typeof(IWechatWorkSchoolDepartmentService),
            nameof(IWechatWorkSchoolDepartmentService.DeleteDepartmentAsync),
            typeof(GetAttribute), "/cgi-bin/school/department/delete"),
        // 获取部门列表（自建 92343、第三方 92299、代开发 96745；官方即 GET，
        // id 走 Query 可选，不填默认全量组织架构）。
        (typeof(IWechatWorkSchoolDepartmentService),
            nameof(IWechatWorkSchoolDepartmentService.ListDepartmentsAsync),
            typeof(GetAttribute), "/cgi-bin/school/department/list"),
        // 修改自动升年级的配置（自建 92949、第三方 92950、代开发 100161）。
        (typeof(IWechatWorkSchoolDepartmentService),
            nameof(IWechatWorkSchoolDepartmentService.SetUpgradeInfoAsync),
            typeof(PostAttribute), "/cgi-bin/school/set_upgrade_info"),
    };

    /// <summary>
    /// 网页授权登录域官方路由表（4 条：自建/代开发公共面 2 条收敛父接口；
    /// 第三方为独立路由 2 条、以 suite_access_token 鉴权，独立声明于第三方接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolAuthRoutes =
    {
        // 获取访问用户身份（自建 91707、代开发 96712；官方即 GET，code 走 Query）。
        (typeof(IWechatWorkSchoolAuthService),
            nameof(IWechatWorkSchoolAuthService.GetUserInfoAsync),
            typeof(GetAttribute), "/cgi-bin/auth/getuserinfo"),
        // 获取家校访问用户身份（自建 95791、代开发 96715；官方即 GET，code 走 Query）。
        (typeof(IWechatWorkSchoolAuthService),
            nameof(IWechatWorkSchoolAuthService.GetSchoolUserInfoAsync),
            typeof(GetAttribute), "/cgi-bin/school/getuserinfo"),
        // 获取访问用户身份·第三方（91711；官方即 GET，code 走 Query，suite_access_token 鉴权）。
        (typeof(IWechatWorkThirdPartySchoolAuthService),
            nameof(IWechatWorkThirdPartySchoolAuthService.GetUserInfoAsync),
            typeof(GetAttribute), "/cgi-bin/service/getuserinfo3rd"),
        // 获取家校访问用户身份·第三方（95790；官方即 GET，code 走 Query，suite_access_token 鉴权）。
        (typeof(IWechatWorkThirdPartySchoolAuthService),
            nameof(IWechatWorkThirdPartySchoolAuthService.GetSchoolUserInfoAsync),
            typeof(GetAttribute), "/cgi-bin/service/school/getuserinfo3rd"),
    };

    /// <summary>
    /// 健康上报域官方路由表（官方仅自建应用开放，第三方/代开发「暂不支持」，4 条端点全部收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolHealthReportRoutes =
    {
        // 获取健康上报使用统计（93676；官方即 POST）。
        (typeof(IWechatWorkSchoolHealthReportService),
            nameof(IWechatWorkSchoolHealthReportService.GetHealthReportStatAsync),
            typeof(PostAttribute), "/cgi-bin/health/get_health_report_stat"),
        // 获取健康上报任务 ID 列表（93677；官方即 POST）。
        (typeof(IWechatWorkSchoolHealthReportService),
            nameof(IWechatWorkSchoolHealthReportService.GetReportJobIdsAsync),
            typeof(PostAttribute), "/cgi-bin/health/get_report_jobids"),
        // 获取健康上报任务详情（93678；官方即 POST）。
        (typeof(IWechatWorkSchoolHealthReportService),
            nameof(IWechatWorkSchoolHealthReportService.GetReportJobInfoAsync),
            typeof(PostAttribute), "/cgi-bin/health/get_report_job_info"),
        // 获取用户填写答案（93679；官方即 POST）。
        (typeof(IWechatWorkSchoolHealthReportService),
            nameof(IWechatWorkSchoolHealthReportService.GetReportAnswerAsync),
            typeof(PostAttribute), "/cgi-bin/health/get_report_answer"),
    };

    /// <summary>
    /// 上课直播域官方路由表（三类应用公共面，7 条端点全部收敛父接口；
    /// 获取老师直播 ID 列表与删除直播回放官方路由位于 /cgi-bin/living/ 段、
    /// 其余 5 条位于 /cgi-bin/school/living/ 段，照抄不纠正；
    /// 获取直播详情官方即 GET（livingid 走 Query），其余官方即 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolLivingRoutes =
    {
        // 获取老师直播 ID 列表（自建 93739、第三方 93856、代开发 97127）。
        (typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.GetUserAllLivingIdAsync),
            typeof(PostAttribute), "/cgi-bin/living/get_user_all_livingid"),
        // 获取直播详情（自建 93740、第三方 93857、代开发 97128；官方即 GET，livingid 走 Query）。
        (typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.GetLivingInfoAsync),
            typeof(GetAttribute), "/cgi-bin/school/living/get_living_info"),
        // 获取观看直播统计（自建 93741、第三方 93858、代开发 97129）。
        (typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.GetWatchStatAsync),
            typeof(PostAttribute), "/cgi-bin/school/living/get_watch_stat"),
        // 获取未观看直播统计（自建 93742、第三方 93859、代开发 97130）。
        (typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.GetUnwatchStatAsync),
            typeof(PostAttribute), "/cgi-bin/school/living/get_unwatch_stat"),
        // 删除直播回放（自建 93743、第三方 93860、代开发 97131）。
        (typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.DeleteReplayDataAsync),
            typeof(PostAttribute), "/cgi-bin/living/delete_replay_data"),
        // 获取观看直播统计 V2（自建 95793、第三方 95799、代开发 97132）。
        (typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.GetWatchStatV2Async),
            typeof(PostAttribute), "/cgi-bin/school/living/get_watch_stat_v2"),
        // 获取未观看直播统计 V2（自建 95795、第三方 95800、代开发 97133）。
        (typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.GetUnwatchStatV2Async),
            typeof(PostAttribute), "/cgi-bin/school/living/get_unwatch_stat_v2"),
    };

    /// <summary>
    /// 班级收款域官方路由表（官方仅自建与第三方开放，服务商代开发无服务端查询接口，
    /// 2 条端点全部收敛父接口；发起班级收款为 JS-SDK / 小程序客户端能力，不属于本域）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolClassPayRoutes =
    {
        // 获取学生付款结果（自建 94470、第三方 94553）。
        (typeof(IWechatWorkSchoolClassPayService),
            nameof(IWechatWorkSchoolClassPayService.GetPaymentResultAsync),
            typeof(PostAttribute), "/cgi-bin/school/get_payment_result"),
        // 获取订单详情（自建 94471、第三方 94554）。
        (typeof(IWechatWorkSchoolClassPayService),
            nameof(IWechatWorkSchoolClassPayService.GetTradeAsync),
            typeof(PostAttribute), "/cgi-bin/school/get_trade"),
    };

    /// <summary>
    /// 契约守卫 SCH1a：家校沟通基础域全部端点路由必须与官方契约一致
    /// （7 个端点为三类应用公共面，全部收敛父接口；二维码/关注模式/班级群创建方式/家长范围
    /// 官方即 GET，勿改成 POST）。
    /// </summary>
    [Fact]
    public void SchoolEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolRoutes.Should().HaveCount(7,
            "家校沟通基础域 7 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = SchoolRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(7, "家校沟通基础域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolRoutes)
        {
            // DeclaredOnly：公共端点必须落在父接口自身声明，不得下沉到子接口重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 SCH1b：家校管理配置域全部端点路由必须与官方契约一致
    /// （官方仅自建与第三方开放；获取老师可查看班级模式官方即 GET）。
    /// </summary>
    [Fact]
    public void SchoolSettingEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolSettingRoutes.Should().HaveCount(3,
            "家校管理配置域 3 个端点官方仅自建与第三方应用开放，全部声明于父接口");

        var distinctRoutes = SchoolSettingRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "家校管理配置域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolSettingRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // get_allow_scope 的 agentid 必须以 [Query("agentid")] 显式 Query 参数传递（官方契约）。
        var methodInfo = typeof(IWechatWorkSchoolService).GetMethod(
            nameof(IWechatWorkSchoolService.GetAllowScopeAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        methodInfo.Should().NotBeNull();
        methodInfo!.GetParameters()
            .Any(p => p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "agentid")
            .Should().BeTrue("get_allow_scope 的 agentid 为官方 Query 参数，必须以 [Query(\"agentid\")] 标注");
    }

    /// <summary>
    /// 契约守卫 SCH1c：学生与家长管理域全部端点路由必须与官方契约一致
    /// （16 个端点为三类应用公共面；单个删除学生/家长官方即 GET（userid 走 Query）、
    /// 批量删除官方即 POST（useridlist 请求体），勿「顺手统一」；
    /// 部门学生/家长详情官方即 GET 且无 cursor/limit 分页参数）。
    /// </summary>
    [Fact]
    public void SchoolUserEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolUserRoutes.Should().HaveCount(16,
            "学生与家长管理域 16 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = SchoolUserRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(16, "学生与家长管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolUserRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // Query 参数契约：删除学生/删除家长/读取学生或家长的 userid、部门学生/家长详情的
        // department_id 均为官方 Query 参数，必须以 [Query] 显式标注。
        AssertHasQueryParameter(typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.DeleteStudentAsync), "userid");
        AssertHasQueryParameter(typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.DeleteParentAsync), "userid");
        AssertHasQueryParameter(typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.GetSchoolUserAsync), "userid");
        AssertHasQueryParameter(typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.ListDepartmentStudentsAsync), "department_id");
        AssertHasQueryParameter(typeof(IWechatWorkSchoolUserService),
            nameof(IWechatWorkSchoolUserService.ListDepartmentParentsAsync), "department_id");
    }

    /// <summary>
    /// 契约守卫 SCH1e：部门管理域全部端点路由必须与官方契约一致
    /// （5 个端点为三类应用公共面；删除/列表官方即 GET（id 走 Query）、
    /// 创建/更新/升年级配置官方即 POST，勿「顺手统一」）。
    /// </summary>
    [Fact]
    public void SchoolDepartmentEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolDepartmentRoutes.Should().HaveCount(5,
            "部门管理域 5 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = SchoolDepartmentRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(5, "部门管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolDepartmentRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // Query 参数契约：删除部门与获取部门列表的 id 均为官方 Query 参数，必须以 [Query("id")] 标注。
        AssertHasQueryParameter(typeof(IWechatWorkSchoolDepartmentService),
            nameof(IWechatWorkSchoolDepartmentService.DeleteDepartmentAsync), "id");
        AssertHasQueryParameter(typeof(IWechatWorkSchoolDepartmentService),
            nameof(IWechatWorkSchoolDepartmentService.ListDepartmentsAsync), "id");
    }

    /// <summary>
    /// 契约守卫 SCH1d：网页授权登录域全部端点路由必须与官方契约一致
    /// （自建/代开发公共面 2 条收敛父接口；第三方为独立路由 2 条；
    /// 4 个端点官方即 GET 且 code 均走 Query）。
    /// </summary>
    [Fact]
    public void SchoolAuthEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolAuthRoutes.Should().HaveCount(4,
            "网页授权登录域 4 个端点 = 自建/代开发公共面 2 条（父接口）+ 第三方独立路由 2 条");

        var distinctRoutes = SchoolAuthRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "网页授权登录域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolAuthRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");

            AssertHasQueryParameter(iface, method, "code");
        }
    }

    /// <summary>
    /// 契约守卫 SCH1f：健康上报域全部端点路由必须与官方契约一致
    /// （官方仅自建应用开放，第三方/代开发「暂不支持」；4 个端点官方即 POST，勿「顺手统一」为 GET）。
    /// </summary>
    [Fact]
    public void SchoolHealthReportEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolHealthReportRoutes.Should().HaveCount(4,
            "健康上报域 4 个端点官方仅自建应用开放，全部收敛父接口");

        var distinctRoutes = SchoolHealthReportRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "健康上报域各端点路由互不重复");
        distinctRoutes.Should().OnlyContain(r => r.StartsWith("/cgi-bin/health/", StringComparison.Ordinal),
            "健康上报域全部路由位于 /cgi-bin/health/ 段");

        foreach (var (iface, method, httpAttribute, route) in SchoolHealthReportRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 SCH1g：上课直播域全部端点路由必须与官方契约一致
    /// （7 个端点为三类应用公共面；获取直播详情官方即 GET（livingid 走 Query）、其余官方即 POST；
    /// 获取老师直播 ID 列表与删除直播回放官方路由位于 /cgi-bin/living/ 段，勿「顺手归位」到 school 段）。
    /// </summary>
    [Fact]
    public void SchoolLivingEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolLivingRoutes.Should().HaveCount(7,
            "上课直播域 7 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = SchoolLivingRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(7, "上课直播域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolLivingRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 路由段契约：2 条端点位于 /cgi-bin/living/ 段（官方原文），5 条位于 /cgi-bin/school/living/ 段。
        SchoolLivingRoutes.Count(r => r.Route.StartsWith("/cgi-bin/living/", StringComparison.Ordinal))
            .Should().Be(2, "获取老师直播 ID 列表与删除直播回放的官方路由位于 /cgi-bin/living/ 段");
        SchoolLivingRoutes.Count(r => r.Route.StartsWith("/cgi-bin/school/living/", StringComparison.Ordinal))
            .Should().Be(5, "其余 5 条上课直播端点的官方路由位于 /cgi-bin/school/living/ 段");

        // Query 参数契约：获取直播详情的 livingid 为官方 Query 参数，必须以 [Query("livingid")] 标注。
        AssertHasQueryParameter(typeof(IWechatWorkSchoolLivingService),
            nameof(IWechatWorkSchoolLivingService.GetLivingInfoAsync), "livingid");
    }

    /// <summary>
    /// 契约守卫 SCH1h：班级收款域全部端点路由必须与官方契约一致
    /// （官方仅自建与第三方开放，服务商代开发无服务端查询接口；2 个端点官方即 POST）。
    /// </summary>
    [Fact]
    public void SchoolClassPayEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolClassPayRoutes.Should().HaveCount(2,
            "班级收款域 2 个端点官方仅自建与第三方应用开放，全部收敛父接口");

        var distinctRoutes = SchoolClassPayRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "班级收款域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolClassPayRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 断言指定接口方法的参数中存在以 <c>[Query(name)]</c> 标注的官方 Query 参数。
    /// </summary>
    private static void AssertHasQueryParameter(Type iface, string methodName, string queryName)
    {
        var methodInfo = iface.GetMethod(methodName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        methodInfo.Should().NotBeNull($"{iface.Name}.{methodName} 必须存在");
        methodInfo!.GetParameters()
            .Any(p => p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == queryName)
            .Should().BeTrue($"{iface.Name}.{methodName} 的 {queryName} 为官方 Query 参数，必须以 [Query(\"{queryName}\")] 标注");
    }

    /// <summary>
    /// 契约守卫 SCH2：接口层级与生成器注册形态——
    /// 家校沟通基础域与学生与家长管理域：三类应用公共面收敛父接口（IsAbstract），
    /// 自建/第三方/代开发子接口均为零差异端点空标记；
    /// 家校管理配置域：父接口承载端点（IsAbstract），继承链上恰好只有自建与第三方子接口
    /// （官方未向代开发开放，能力漂移守卫）；
    /// 健康上报域：父接口承载端点（IsAbstract），继承链上恰好只有自建子接口
    /// （官方对第三方/代开发标注「暂不支持」，能力漂移守卫）；
    /// 上课直播域：三类应用公共面收敛父接口（IsAbstract），子接口均为零差异端点空标记；
    /// 班级收款域：父接口承载端点（IsAbstract），继承链上恰好只有自建与第三方子接口
    /// （官方代开发文档「班级收款」仅目录页、无服务端查询 API，能力漂移守卫）；
    /// 网页授权登录域：父接口承载自建/代开发公共面（IsAbstract），继承链上恰好只有自建与代开发
    /// 空标记子接口；第三方接口为独立路由 + suite_access_token，<b>不继承</b>公共父接口、
    /// 独立声明 2 个端点（一接口族一令牌路由键）。
    /// </summary>
    [Fact]
    public void SchoolInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        var families = new[]
        {
            (typeof(IWechatWorkSchoolService),
                new[] { typeof(IWechatWorkInternalSchoolService), typeof(IWechatWorkThirdPartySchoolService), typeof(IWechatWorkProviderSchoolService) },
                SchoolParentImplementationClassName,
                "家校沟通基础域 7 个端点为三类应用公共面，继承链上不得出现其它子接口"),
            (typeof(IWechatWorkSchoolSettingService),
                new[] { typeof(IWechatWorkInternalSchoolSettingService), typeof(IWechatWorkThirdPartySchoolSettingService) },
                SchoolSettingParentImplementationClassName,
                "家校管理配置域官方仅向自建与第三方应用开放，继承链上不得出现代开发等其它子接口"),
            (typeof(IWechatWorkSchoolUserService),
                new[] { typeof(IWechatWorkInternalSchoolUserService), typeof(IWechatWorkThirdPartySchoolUserService), typeof(IWechatWorkProviderSchoolUserService) },
                SchoolUserParentImplementationClassName,
                "学生与家长管理域 16 个端点为三类应用公共面，继承链上不得出现其它子接口"),
            (typeof(IWechatWorkSchoolDepartmentService),
                new[] { typeof(IWechatWorkInternalSchoolDepartmentService), typeof(IWechatWorkThirdPartySchoolDepartmentService), typeof(IWechatWorkProviderSchoolDepartmentService) },
                SchoolDepartmentParentImplementationClassName,
                "部门管理域 5 个端点为三类应用公共面，继承链上不得出现其它子接口"),
            (typeof(IWechatWorkSchoolHealthReportService),
                new[] { typeof(IWechatWorkInternalSchoolHealthReportService) },
                SchoolHealthReportParentImplementationClassName,
                "健康上报域官方仅向自建应用开放（第三方/代开发暂不支持），继承链上不得出现其它子接口"),
            (typeof(IWechatWorkSchoolLivingService),
                new[] { typeof(IWechatWorkInternalSchoolLivingService), typeof(IWechatWorkThirdPartySchoolLivingService), typeof(IWechatWorkProviderSchoolLivingService) },
                SchoolLivingParentImplementationClassName,
                "上课直播域 7 个端点为三类应用公共面，继承链上不得出现其它子接口"),
            (typeof(IWechatWorkSchoolClassPayService),
                new[] { typeof(IWechatWorkInternalSchoolClassPayService), typeof(IWechatWorkThirdPartySchoolClassPayService) },
                SchoolClassPayParentImplementationClassName,
                "班级收款域官方仅向自建与第三方应用开放（代开发无服务端查询接口），继承链上不得出现其它子接口"),
        };

        foreach (var (parent, children, implementationClassName, because) in families)
        {
            var assignable = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
                .ToList();

            assignable.Should().BeEquivalentTo(children, because);

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeEmpty("公共端点全部声明于父接口");

            foreach (var child in children)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(SchoolRegistryGroupName,
                    $"{child.Name} 必须挂 {SchoolRegistryGroupName} 注册组" +
                    $"（School 模块共用 Add{SchoolRegistryGroupName}WebApiHttpClient()）");
                childApi.InheritedFrom.Should().Be(implementationClassName,
                    $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

                // 三类应用开放面完全一致（或自建+第三方一致）：任何子接口出现差异端点均为能力漂移。
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
            }
        }

        // ── 网页授权登录域：自建/代开发公共面收敛父接口，第三方独立成接口 ──
        var authParent = typeof(IWechatWorkSchoolAuthService);
        var authInternal = typeof(IWechatWorkInternalSchoolAuthService);
        var authProvider = typeof(IWechatWorkProviderSchoolAuthService);
        var authThirdParty = typeof(IWechatWorkThirdPartySchoolAuthService);

        authInternal.Should().BeAssignableTo(authParent,
            $"{authInternal.Name} 必须继承网页授权登录域公共父接口 {authParent.Name}");
        authProvider.Should().BeAssignableTo(authParent,
            $"{authProvider.Name} 必须继承网页授权登录域公共父接口 {authParent.Name}");

        // 第三方不得继承公共父接口：父接口以 access_token 鉴权，第三方官方契约是
        // suite_access_token + /cgi-bin/service/ 独立路由，继承即令牌路由键漂移。
        authParent.IsAssignableFrom(authThirdParty).Should().BeFalse(
            $"{authThirdParty.Name} 不得继承 {authParent.Name}：官方第三方为独立路由且以 suite_access_token 鉴权");

        authParent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != authParent && authParent.IsAssignableFrom(t))
            .Select(t => t.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList()
            .Should().BeEquivalentTo(
                new[] { nameof(IWechatWorkInternalSchoolAuthService), nameof(IWechatWorkProviderSchoolAuthService) },
                "网页授权登录域公共父接口（access_token 面）的子接口恰为自建与代开发两个应用类型子接口");

        var authParentApi = authParent.GetCustomAttribute<HttpClientApiAttribute>();
        authParentApi.Should().NotBeNull("网页授权登录域公共父接口必须声明 [HttpClientApi]");
        authParentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        authParentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        authParent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, "自建/代开发公共面 2 个端点必须声明于网页授权登录域公共父接口");

        foreach (var child in new[] { authInternal, authProvider })
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(SchoolRegistryGroupName,
                $"{child.Name} 必须挂 {SchoolRegistryGroupName} 注册组");
            childApi.InheritedFrom.Should().Be(SchoolAuthParentImplementationClassName,
                $"{child.Name} 必须继承网页授权登录域公共父接口生成实现类");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：自建/代开发公共面全部声明于父接口");
        }

        var authThirdPartyApi = authThirdParty.GetCustomAttribute<HttpClientApiAttribute>();
        authThirdPartyApi.Should().NotBeNull($"{authThirdParty.Name} 必须声明 [HttpClientApi]");
        authThirdPartyApi!.RegistryGroupName.Should().Be(SchoolRegistryGroupName,
            $"{authThirdParty.Name} 必须挂 {SchoolRegistryGroupName} 注册组");
        authThirdPartyApi.InheritedFrom.Should().BeNullOrEmpty(
            $"{authThirdParty.Name} 不继承公共父接口（令牌路由键不同），独立生成实现类");
        authThirdParty.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, "第三方独立路由的 2 个端点必须声明于第三方接口自身");

        // 类型化面：自建/代开发为官方开放的 2 条公共面，第三方为官方开放的 2 条独立路由。
        CollectInterfaceEndpoints(authInternal).Should().HaveCount(2,
            "自建应用类型化面为官方开放的 2 条网页授权端点");
        CollectInterfaceEndpoints(authProvider).Should().HaveCount(2,
            "代开发应用类型化面为官方开放的 2 条网页授权端点");
        CollectInterfaceEndpoints(authThirdParty).Should().HaveCount(2,
            "第三方应用类型化面为官方开放的 2 条独立路由网页授权端点");
    }

    /// <summary>
    /// 契约守卫 SCH3：令牌绑定——School 模块学生与家长管理域、上课直播域、班级收款域、
    /// 网页授权登录域自建/代开发公共面统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）；
    /// 网页授权登录域第三方接口消费 SuiteAccessToken 路由键并以 Query 注入（官方契约 suite_access_token）。
    /// </summary>
    [Fact]
    public void SchoolTokenBinding_ShouldMatchOfficialTokenRouteKeys()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkSchoolService),
            typeof(IWechatWorkInternalSchoolService),
            typeof(IWechatWorkThirdPartySchoolService),
            typeof(IWechatWorkProviderSchoolService),
            typeof(IWechatWorkSchoolSettingService),
            typeof(IWechatWorkInternalSchoolSettingService),
            typeof(IWechatWorkThirdPartySchoolSettingService),
            typeof(IWechatWorkSchoolUserService),
            typeof(IWechatWorkInternalSchoolUserService),
            typeof(IWechatWorkThirdPartySchoolUserService),
            typeof(IWechatWorkProviderSchoolUserService),
            typeof(IWechatWorkSchoolDepartmentService),
            typeof(IWechatWorkInternalSchoolDepartmentService),
            typeof(IWechatWorkThirdPartySchoolDepartmentService),
            typeof(IWechatWorkProviderSchoolDepartmentService),
            typeof(IWechatWorkSchoolAuthService),
            typeof(IWechatWorkInternalSchoolAuthService),
            typeof(IWechatWorkProviderSchoolAuthService),
            typeof(IWechatWorkSchoolHealthReportService),
            typeof(IWechatWorkInternalSchoolHealthReportService),
            typeof(IWechatWorkSchoolLivingService),
            typeof(IWechatWorkInternalSchoolLivingService),
            typeof(IWechatWorkThirdPartySchoolLivingService),
            typeof(IWechatWorkProviderSchoolLivingService),
            typeof(IWechatWorkSchoolClassPayService),
            typeof(IWechatWorkInternalSchoolClassPayService),
            typeof(IWechatWorkThirdPartySchoolClassPayService),
        };

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }

        // 第三方网页授权：suite_access_token 为套件级凭证、无企业 scope，独立令牌路由键。
        var thirdPartyAuth = typeof(IWechatWorkThirdPartySchoolAuthService);
        var thirdPartyToken = thirdPartyAuth.GetCustomAttribute<TokenAttribute>();
        thirdPartyToken.Should().NotBeNull($"{thirdPartyAuth.Name} 必须声明 [Token]");
        thirdPartyToken!.TokenType.Should().Be(WechatTokenTypes.SuiteAccessToken,
            $"{thirdPartyAuth.Name} 令牌路由键必须为 SuiteAccessToken（官方以 suite_access_token 鉴权）");
        thirdPartyToken.InjectionMode.Should().Be(TokenInjectionMode.Query,
            $"{thirdPartyAuth.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
        thirdPartyToken.Name.Should().Be("suite_access_token",
            $"{thirdPartyAuth.Name} Query 注入参数名必须为官方契约的 suite_access_token");
    }

    /// <summary>
    /// 契约守卫 SCH4：家校沟通模块的请求/响应 DTO 必须登记进 AOT JSON 上下文
    /// （家校沟通既有域 55 型落 SchoolJsonContext；健康上报域 15 型落 HealthReportJsonContext；
    /// 上课直播域 25 型落 LivingJsonContext；班级收款域 5 型落 ClassPayJsonContext，
    /// 共 100 个契约面类型）。
    /// </summary>
    [Fact]
    public void SchoolDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var schoolContext = SchoolJsonContext.Default;

        var schoolTypes = new[]
        {
            // 「学校通知」二维码 + 关注模式。
            typeof(GetSchoolSubscribeQrCodeResponse),
            typeof(SetSchoolSubscribeModeRequest), typeof(GetSchoolSubscribeModeResponse),
            // 班级群创建方式。
            typeof(GetSchoolChatCreateModeResponse), typeof(SetSchoolChatCreateModeRequest),
            // 外部联系人 openid 转换。
            typeof(SchoolConvertToOpenIdRequest), typeof(SchoolConvertToOpenIdResponse),
            // 可使用的家长范围。
            typeof(GetSchoolAllowScopeResponse), typeof(SchoolAllowScope),
            typeof(SchoolAllowScopeStudents), typeof(SchoolAllowScopeDepartments),
            // 老师可查看班级模式。
            typeof(SetSchoolTeacherViewModeRequest), typeof(GetSchoolTeacherViewModeResponse),
            // 手机号转外部联系人 ID。
            typeof(SchoolBatchToExternalUserIdRequest), typeof(SchoolBatchToExternalUserIdResponse),
            typeof(SchoolMobileConvertSuccessItem), typeof(SchoolMobileConvertFailItem),
            // 学生与家长管理：单个增删改请求。
            typeof(SchoolCreateStudentRequest), typeof(SchoolUpdateStudentRequest),
            typeof(SchoolCreateParentRequest), typeof(SchoolUpdateParentRequest),
            typeof(SchoolParentChildItem), typeof(SchoolSetArchSyncModeRequest),
            // 学生与家长管理：批量增删改请求。
            typeof(SchoolBatchCreateStudentRequest), typeof(SchoolBatchUpdateStudentRequest),
            typeof(SchoolBatchDeleteStudentRequest),
            typeof(SchoolBatchCreateParentRequest), typeof(SchoolBatchUpdateParentRequest),
            typeof(SchoolBatchDeleteParentRequest),
            // 学生与家长管理：批量结果。
            typeof(SchoolStudentBatchResultItem), typeof(SchoolParentBatchResultItem),
            typeof(SchoolBatchStudentResultResponse), typeof(SchoolBatchParentResultResponse),
            // 学生与家长管理：读取学生或家长 / 部门学生与家长详情。
            typeof(GetSchoolUserResponse), typeof(SchoolStudentInfo), typeof(SchoolParentInfo),
            typeof(SchoolChildInfo),
            typeof(SchoolDepartmentStudentsResponse), typeof(SchoolDepartmentParentsResponse),
            // 网页授权登录：自建/代开发公共面。
            typeof(SchoolAuthUserInfoResponse), typeof(SchoolAuthSchoolUserInfoResponse),
            // 网页授权登录：第三方。
            typeof(SchoolAuthThirdPartyUserInfoResponse), typeof(SchoolAuthThirdPartyParentItem),
            typeof(SchoolAuthThirdPartyStudentItem), typeof(SchoolAuthThirdPartySchoolUserInfoResponse),
            // 部门管理：请求。
            typeof(SchoolCreateDepartmentRequest), typeof(SchoolDepartmentAdminItem),
            typeof(SchoolUpdateDepartmentRequest), typeof(SchoolUpdateDepartmentAdminItem),
            typeof(SchoolSetUpgradeInfoRequest),
            // 部门管理：响应。
            typeof(SchoolCreateDepartmentResponse), typeof(SchoolDepartmentListResponse),
            typeof(SchoolDepartmentInfo), typeof(SchoolDepartmentAdminInfo),
            typeof(SchoolSetUpgradeInfoResponse),
        };

        schoolTypes.Should().HaveCount(55, "家校沟通既有域契约面共 55 型");
        schoolTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in schoolTypes)
        {
            schoolContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是家校沟通模块契约面类型，必须登记进 SchoolJsonContext（AOT 源生成）");
        }

        // ── 健康上报域：官方仅自建开放，15 型落 HealthReportJsonContext ──
        var healthReportContext = HealthReportJsonContext.Default;

        var healthReportTypes = new[]
        {
            // 使用统计。
            typeof(HealthReportGetStatRequest), typeof(HealthReportGetStatResponse),
            // 任务 ID 列表。
            typeof(HealthReportGetJobIdsRequest), typeof(HealthReportGetJobIdsResponse),
            // 任务详情。
            typeof(HealthReportGetJobInfoRequest), typeof(HealthReportGetJobInfoResponse),
            typeof(HealthReportJobInfo), typeof(HealthReportApplyRange), typeof(HealthReportReportTo),
            typeof(HealthReportQuestionTemplate), typeof(HealthReportQuestionOption),
            // 用户填写答案。
            typeof(HealthReportGetAnswerRequest), typeof(HealthReportGetAnswerResponse),
            typeof(HealthReportAnswer), typeof(HealthReportAnswerValue),
        };

        healthReportTypes.Should().HaveCount(15, "健康上报域契约面共 15 型");
        healthReportTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in healthReportTypes)
        {
            healthReportContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是健康上报域契约面类型，必须登记进 HealthReportJsonContext（AOT 源生成）");
        }

        // ── 上课直播域：三类应用公共面，25 型落 LivingJsonContext ──
        var livingContext = LivingJsonContext.Default;

        var livingTypes = new[]
        {
            // 获取老师直播 ID 列表。
            typeof(LivingGetUserAllLivingIdRequest), typeof(LivingGetUserAllLivingIdResponse),
            // 获取直播详情。
            typeof(LivingGetLivingInfoResponse), typeof(LivingInfo), typeof(LivingRange),
            // 观看/未观看直播统计（V1，next_key 分页）。
            typeof(LivingGetWatchStatRequest), typeof(LivingGetWatchStatResponse),
            typeof(LivingWatchStatInfoes), typeof(LivingWatchStudent), typeof(LivingVisitor),
            typeof(LivingGetUnwatchStatRequest), typeof(LivingGetUnwatchStatResponse),
            typeof(LivingUnwatchStatInfo), typeof(LivingUnwatchStudent),
            // 删除直播回放。
            typeof(LivingDeleteReplayDataRequest),
            // 观看/未观看直播统计 V2（next_cursor 分页，较 V1 新增家长列表）。
            typeof(LivingGetWatchStatV2Request), typeof(LivingGetWatchStatV2Response),
            typeof(LivingWatchStatInfoV2), typeof(LivingWatchStudentV2), typeof(LivingWatchParent),
            typeof(LivingGetUnwatchStatV2Request), typeof(LivingGetUnwatchStatV2Response),
            typeof(LivingUnwatchStatInfoV2), typeof(LivingUnwatchStudentV2), typeof(LivingUnwatchParent),
        };

        livingTypes.Should().HaveCount(25, "上课直播域契约面共 25 型");
        livingTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in livingTypes)
        {
            livingContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是上课直播域契约面类型，必须登记进 LivingJsonContext（AOT 源生成）");
        }

        // ── 班级收款域：官方仅自建与第三方开放，5 型落 ClassPayJsonContext ──
        var classPayContext = ClassPayJsonContext.Default;

        var classPayTypes = new[]
        {
            typeof(ClassPayGetPaymentResultRequest), typeof(ClassPayGetPaymentResultResponse),
            typeof(ClassPayPaymentResultItem),
            typeof(ClassPayGetTradeRequest), typeof(ClassPayGetTradeResponse),
        };

        classPayTypes.Should().HaveCount(5, "班级收款域契约面共 5 型");
        classPayTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in classPayTypes)
        {
            classPayContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是班级收款域契约面类型，必须登记进 ClassPayJsonContext（AOT 源生成）");
        }
    }

    /// <summary>
    /// 沿接口继承链收集端点方法声明。
    /// <para>接口反射<b>不</b>返回继承成员（<c>Type.GetMethods()</c> 对接口仅返回自身声明），
    /// 故类型化端点面必须逐级上溯接口继承链后统计。</para>
    /// </summary>
    private static List<MethodInfo> CollectInterfaceEndpoints(Type iface)
    {
        var methods = new List<MethodInfo>();
        for (var current = iface; current is not null && current != typeof(object); current = current.GetInterfaces().FirstOrDefault())
        {
            methods.AddRange(current.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        }

        return methods;
    }
}
