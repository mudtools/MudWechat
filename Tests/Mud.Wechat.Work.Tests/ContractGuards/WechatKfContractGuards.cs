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
using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 微信客服模块（Kf 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （客服账号管理域 + 接待人员管理域 + 会话分配与消息收发域 + 客户基础信息域 +
/// 统计管理域 + 机器人管理域（仅自建） + 微信客服组件域（仅第三方））。
/// </summary>
/// <remarks>
/// <para>
/// 形态：客服账号管理 / 接待人员管理 / 会话分配与消息收发 / 客户基础信息 / 统计管理五域为
/// 三类应用公共面收敛父接口（自建 / 第三方 / 代开发子接口均为零差异端点空标记）；
/// 机器人管理域官方仅自建开放（零端点父接口 + 仅自建子接口承载 8 端点）；
/// 微信客服组件域官方仅微信客服组件应用（套件形态）消费（零端点父接口 + 仅第三方子接口承载 3 端点，
/// 其中 2 条与客服账号管理域共用路由）。
/// </para>
/// </remarks>
public class WechatKfContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string AccountParentImplementationClassName = "WechatWorkKfAccountService";

    private const string ServicerParentImplementationClassName = "WechatWorkKfServicerService";

    private const string SessionParentImplementationClassName = "WechatWorkKfSessionService";

    private const string CustomerParentImplementationClassName = "WechatWorkKfCustomerService";

    private const string StatisticsParentImplementationClassName = "WechatWorkKfStatisticsService";

    private const string UpgradeParentImplementationClassName = "WechatWorkKfUpgradeService";

    private const string KfRegistryGroupName = "Kf";

    /// <summary>
    /// 客服账号管理域官方路由表（父接口 5 条公共端点，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AccountRoutes =
    {
        // 客服账号管理族（自建/第三方 94661、代开发 96404）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.AddAccountAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/add"),
        // 获取客服账号列表（自建/第三方 94662、代开发 96415）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.GetAccountListAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/list"),
        // 删除客服账号（自建/第三方 94663、代开发 96405）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.DeleteAccountAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/del"),
        // 修改客服账号（自建/第三方 94664、代开发 96406）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.UpdateAccountAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/update"),
        // 获取客服账号链接（自建/第三方 94665、代开发 96416）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.GetAccountContactWayAsync),
            typeof(PostAttribute), "/cgi-bin/kf/add_contact_way"),
    };

    /// <summary>
    /// 接待人员管理域官方路由表（父接口 3 条公共端点；servicer/list 为 GET，其余 2 条为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ServicerRoutes =
    {
        // 添加接待人员（自建/第三方 94646、代开发 96418）。
        (typeof(IWechatWorkKfServicerService),
            nameof(IWechatWorkKfServicerService.AddServicerAsync),
            typeof(PostAttribute), "/cgi-bin/kf/servicer/add"),
        // 删除接待人员（自建/第三方 94647、代开发 96419）。
        (typeof(IWechatWorkKfServicerService),
            nameof(IWechatWorkKfServicerService.DeleteServicerAsync),
            typeof(PostAttribute), "/cgi-bin/kf/servicer/del"),
        // 获取接待人员列表（自建/第三方 94645、代开发 96420），open_kfid 走 Query。
        (typeof(IWechatWorkKfServicerService),
            nameof(IWechatWorkKfServicerService.GetServicerListAsync),
            typeof(GetAttribute), "/cgi-bin/kf/servicer/list"),
    };

    /// <summary>
    /// 会话分配与消息收发域官方路由表（父接口 4 条路由 / 14 个端点方法：
    /// service_state 2 条 + send_msg 10 个 msgtype 方法 + send_msg_on_event 2 个 msgtype 方法；
    /// 发送类路由按 msgtype 一方法一请求体收敛，形态对齐 message/send 先例）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SessionRoutes =
    {
        // 会话状态族（自建/第三方 94669、代开发 96425）。
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.GetServiceStateAsync),
            typeof(PostAttribute), "/cgi-bin/kf/service_state/get"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.TransitServiceStateAsync),
            typeof(PostAttribute), "/cgi-bin/kf/service_state/trans"),
        // 发送消息族（自建/第三方 94677、代开发 96427），10 种 msgtype 同路由多方法。
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendTextMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendImageMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendVoiceMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendVideoMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendFileMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendLinkMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendMiniProgramMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendMenuMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendLocationMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendCaLinkMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg"),
        // 事件响应消息族（自建/第三方 95122/94910、代开发 96428），2 种 msgtype 同路由多方法。
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendTextMsgOnEventAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg_on_event"),
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SendMenuMsgOnEventAsync),
            typeof(PostAttribute), "/cgi-bin/kf/send_msg_on_event"),
    };

    /// <summary>
    /// 客户基础信息域官方路由表（父接口 1 条公共端点；第三方「获取企业状态信息」95153
    /// 为概述页无独立 API，不入接口面）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] CustomerRoutes =
    {
        // 批量获取客户基础信息（自建 95159、第三方 95149、代开发 96429）。
        (typeof(IWechatWorkKfCustomerService),
            nameof(IWechatWorkKfCustomerService.BatchGetCustomerInfoAsync),
            typeof(PostAttribute), "/cgi-bin/kf/customer/batchget"),
    };

    /// <summary>
    /// 统计管理域官方路由表（父接口 2 条公共端点，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] StatisticsRoutes =
    {
        // 企业汇总数据（自建 95489、第三方 95492、代开发 96432）。
        (typeof(IWechatWorkKfStatisticsService),
            nameof(IWechatWorkKfStatisticsService.GetCorpStatisticAsync),
            typeof(PostAttribute), "/cgi-bin/kf/get_corp_statistic"),
        // 接待人员明细数据（自建 95490、第三方 95493、代开发 96433）。
        (typeof(IWechatWorkKfStatisticsService),
            nameof(IWechatWorkKfStatisticsService.GetServicerStatisticAsync),
            typeof(PostAttribute), "/cgi-bin/kf/get_servicer_statistic"),
    };

    /// <summary>
    /// 机器人管理域官方路由表（仅自建开放，8 条端点全部声明于自建子接口，全部为 POST；
    /// 官方文档目录为「机器人管理」，路由在 /cgi-bin/kf/knowledge/ 下）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] KnowledgeRoutes =
    {
        // 知识库分组管理族（95971）。
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.AddKnowledgeGroupAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/add_group"),
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.DeleteKnowledgeGroupAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/del_group"),
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.UpdateKnowledgeGroupAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/mod_group"),
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.GetKnowledgeGroupListAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/list_group"),
        // 知识库问答管理族（95972）。
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.AddKnowledgeIntentAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/add_intent"),
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.DeleteKnowledgeIntentAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/del_intent"),
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.UpdateKnowledgeIntentAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/mod_intent"),
        (typeof(IWechatWorkInternalKfKnowledgeService),
            nameof(IWechatWorkInternalKfKnowledgeService.GetKnowledgeIntentListAsync),
            typeof(PostAttribute), "/cgi-bin/kf/knowledge/list_intent"),
    };

    /// <summary>
    /// 微信客服组件域官方路由表（仅第三方/组件应用消费，3 条端点全部声明于第三方子接口；
    /// 前两条与客服账号管理域共用路由，组件语义为仅可见企业已授权的客服账号）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ComponentRoutes =
    {
        // 获取客服账号列表·组件版（99368，与 94662/96415 客服账号管理域共用路由）。
        (typeof(IWechatWorkThirdPartyKfComponentService),
            nameof(IWechatWorkThirdPartyKfComponentService.GetAccountListAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/list"),
        // 获取客服账号链接·组件版（99400，与 94665/96416 客服账号管理域共用路由）。
        (typeof(IWechatWorkThirdPartyKfComponentService),
            nameof(IWechatWorkThirdPartyKfComponentService.GetAccountContactWayAsync),
            typeof(PostAttribute), "/cgi-bin/kf/add_contact_way"),
        // 获取客服数据统计·组件版（99367，企业级口径、无 open_kfid 入参，为组件域独有路由）。
        (typeof(IWechatWorkThirdPartyKfComponentService),
            nameof(IWechatWorkThirdPartyKfComponentService.GetStatisticAsync),
            typeof(PostAttribute), "/cgi-bin/kf/get_statistic"),
    };

    /// <summary>
    /// 「升级服务」配置域官方路由表（父接口 3 条公共端点；get_upgrade_service_config 为 GET，
    /// 其余 2 条为 POST；路由在 kf/customer/ 下，与客户基础信息域 batchget 同前缀）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] UpgradeRoutes =
    {
        // 获取配置的专员与客户群（自建 94674、第三方 94702、代开发 96422）。
        (typeof(IWechatWorkKfUpgradeService),
            nameof(IWechatWorkKfUpgradeService.GetUpgradeServiceConfigAsync),
            typeof(GetAttribute), "/cgi-bin/kf/customer/get_upgrade_service_config"),
        // 为客户升级为专员或客户群服务（自建 94674、第三方 94702、代开发 96422）。
        (typeof(IWechatWorkKfUpgradeService),
            nameof(IWechatWorkKfUpgradeService.UpgradeServiceAsync),
            typeof(PostAttribute), "/cgi-bin/kf/customer/upgrade_service"),
        // 为客户取消推荐（自建 94674、第三方 94702、代开发 96422）。
        (typeof(IWechatWorkKfUpgradeService),
            nameof(IWechatWorkKfUpgradeService.CancelUpgradeServiceAsync),
            typeof(PostAttribute), "/cgi-bin/kf/customer/cancel_upgrade_service"),
    };

    /// <summary>
    /// 契约守卫 KF1a：客服账号管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意 <c>kf/account/list</c>（列表）官方即 POST（勿改 GET）；
    /// <c>kf/add_contact_way</c>（获取客服账号链接）直接挂在 <c>/cgi-bin/kf/</c> 根下，
    /// 不在 <c>account/</c> 子路径，与其余端点形状不同。
    /// </summary>
    [Fact]
    public void KfAccountEndpoints_ShouldMatchOfficialRoutes()
    {
        AccountRoutes.Should().HaveCount(5,
            "客服账号管理域 5 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = AccountRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(5, "客服账号管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in AccountRoutes)
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
    /// 契约守卫 KF1b：接待人员管理域全部端点路由必须与官方契约一致；
    /// <c>kf/servicer/list</c> 官方即 GET（open_kfid 走 Query），勿改成 POST。
    /// </summary>
    [Fact]
    public void KfServicerEndpoints_ShouldMatchOfficialRoutes()
    {
        ServicerRoutes.Should().HaveCount(3,
            "接待人员管理域 3 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = ServicerRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "接待人员管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in ServicerRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // servicer/list 的 open_kfid 为官方必填 Query 参数，须以 [Query("open_kfid")] 显式声明（非可空）。
        var listMethod = typeof(IWechatWorkKfServicerService).GetMethod(
            nameof(IWechatWorkKfServicerService.GetServicerListAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        listMethod.Should().NotBeNull();
        var queryParam = listMethod!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "open_kfid");
        queryParam.Should().NotBeNull("servicer/list 的 open_kfid 必须以 [Query(\"open_kfid\")] 显式承载");
        queryParam!.ParameterType.Should().Be<string>("open_kfid 为官方必填参数，不得声明为可选");
    }

    /// <summary>
    /// 契约守卫 KF1c：会话分配与消息收发域全部端点路由必须与官方契约一致。
    /// <c>kf/send_msg</c> 官方开放 10 种 msgtype、<c>kf/send_msg_on_event</c> 开放 2 种 msgtype，
    /// 与 message/send 先例一致按「同路由多方法、每 msgtype 一请求体」收敛；
    /// 官方未在本批开放「读取消息」（sync_msg），不得擅自添加。
    /// </summary>
    [Fact]
    public void KfSessionEndpoints_ShouldMatchOfficialRoutes()
    {
        SessionRoutes.Should().HaveCount(14,
            "会话分配与消息收发域为 4 条路由 / 14 个端点方法（service_state 2 + send_msg 10 + send_msg_on_event 2）");

        var distinctRoutes = SessionRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "发送类路由按 msgtype 一方法一请求体收敛，实际路由仅 4 条");

        foreach (var (iface, method, httpAttribute, route) in SessionRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 KF1d：客户基础信息域全部端点路由必须与官方契约一致
    /// （batchget 为官方原文拼写，勿改 batch_get）。
    /// </summary>
    [Fact]
    public void KfCustomerEndpoints_ShouldMatchOfficialRoutes()
    {
        CustomerRoutes.Should().HaveCount(1,
            "客户基础信息域仅 1 个端点（第三方 95153「获取企业状态信息」为概述页无独立 API，不入接口面）");

        foreach (var (iface, method, httpAttribute, route) in CustomerRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 KF1e：统计管理域全部端点路由必须与官方契约一致（get_corp_statistic 企业汇总 +
    /// get_servicer_statistic 接待人员明细，路由不含 statistic_list 子路径）。
    /// </summary>
    [Fact]
    public void KfStatisticsEndpoints_ShouldMatchOfficialRoutes()
    {
        StatisticsRoutes.Should().HaveCount(2,
            "统计管理域 2 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = StatisticsRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "统计管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in StatisticsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 KF1f：机器人管理域全部端点路由必须与官方契约一致（官方文档目录为「机器人管理」，
    /// 路由在 <c>kf/knowledge/</c> 下；分组族 mod_group、问答族 mod_intent 勿写成 update）。
    /// </summary>
    [Fact]
    public void KfKnowledgeEndpoints_ShouldMatchOfficialRoutes()
    {
        KnowledgeRoutes.Should().HaveCount(8,
            "机器人管理域 8 个端点（分组 4 + 问答 4）官方仅自建应用开放，全部声明于自建子接口");

        var distinctRoutes = KnowledgeRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(8, "机器人管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in KnowledgeRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 KF1g：微信客服组件域全部端点路由必须与官方契约一致；
    /// 组件版 <c>kf/account/list</c> 与 <c>kf/add_contact_way</c> 与客服账号管理域共用路由
    /// （形态对齐获客助手组件先例），<c>kf/get_statistic</c> 为组件域独有路由且无 open_kfid 入参。
    /// </summary>
    [Fact]
    public void KfComponentEndpoints_ShouldMatchOfficialRoutes()
    {
        ComponentRoutes.Should().HaveCount(3,
            "微信客服组件域 3 个端点官方仅由微信客服组件应用（套件形态）消费，全部声明于第三方子接口");

        foreach (var (iface, method, httpAttribute, route) in ComponentRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 组件域与客服账号管理域共用路由的契约锚点：若账号域路由漂移而组件域未同步，此处立即打红。
        ComponentRoutes[0].Route.Should().Be("/cgi-bin/kf/account/list",
            "组件版获取客服账号列表必须与客服账号管理域（KF1a）共用同一路由");
        ComponentRoutes[1].Route.Should().Be("/cgi-bin/kf/add_contact_way",
            "组件版获取客服账号链接必须与客服账号管理域（KF1a）共用同一路由");
    }

    /// <summary>
    /// 契约守卫 KF1h：「升级服务」配置域全部端点路由必须与官方契约一致
    /// （get_upgrade_service_config 为 GET 且无请求参数，勿改成 POST；
    /// 专员范围部门列表字段官方参数表作 department_list、官方 JSON 示例与自建文档作
    /// department_id_list，SDK 以官方 JSON 示例为准——对齐 cusor/cursor 处置先例）。
    /// </summary>
    [Fact]
    public void KfUpgradeEndpoints_ShouldMatchOfficialRoutes()
    {
        UpgradeRoutes.Should().HaveCount(3,
            "「升级服务」配置域 3 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = UpgradeRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "「升级服务」配置域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in UpgradeRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 升级服务部门列表字段名以官方 JSON 示例为准（department_id_list），防止按参数表漂移为 department_list。
        var memberRange = typeof(KfUpgradeMemberRange).GetProperty(nameof(KfUpgradeMemberRange.DepartmentIdList));
        memberRange.Should().NotBeNull("KfUpgradeMemberRange.DepartmentIdList 必须存在");
        memberRange!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("department_id_list",
            "官方参数表 department_list 与官方 JSON 示例 department_id_list 冲突，以官方 JSON 示例为准（cusor/cursor 先例）");
    }

    /// <summary>
    /// 契约守卫 KF2：六类应用公共面域的接口层级与生成器注册形态——公共端点收敛于 IsAbstract 父接口，
    /// 自建 / 第三方 / 代开发子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void KfInterfaceHierarchy_ShouldConvergeOnAbstractParentsWithKfRegistry()
    {
        var families = new[]
        {
            (AccountParentImplementationClassName, typeof(IWechatWorkKfAccountService), new[]
            {
                typeof(IWechatWorkInternalKfAccountService),
                typeof(IWechatWorkThirdPartyKfAccountService),
                typeof(IWechatWorkProviderKfAccountService),
            }),
            (ServicerParentImplementationClassName, typeof(IWechatWorkKfServicerService), new[]
            {
                typeof(IWechatWorkInternalKfServicerService),
                typeof(IWechatWorkThirdPartyKfServicerService),
                typeof(IWechatWorkProviderKfServicerService),
            }),
            (SessionParentImplementationClassName, typeof(IWechatWorkKfSessionService), new[]
            {
                typeof(IWechatWorkInternalKfSessionService),
                typeof(IWechatWorkThirdPartyKfSessionService),
                typeof(IWechatWorkProviderKfSessionService),
            }),
            (CustomerParentImplementationClassName, typeof(IWechatWorkKfCustomerService), new[]
            {
                typeof(IWechatWorkInternalKfCustomerService),
                typeof(IWechatWorkThirdPartyKfCustomerService),
                typeof(IWechatWorkProviderKfCustomerService),
            }),
            (StatisticsParentImplementationClassName, typeof(IWechatWorkKfStatisticsService), new[]
            {
                typeof(IWechatWorkInternalKfStatisticsService),
                typeof(IWechatWorkThirdPartyKfStatisticsService),
                typeof(IWechatWorkProviderKfStatisticsService),
            }),
            (UpgradeParentImplementationClassName, typeof(IWechatWorkKfUpgradeService), new[]
            {
                typeof(IWechatWorkInternalKfUpgradeService),
                typeof(IWechatWorkThirdPartyKfUpgradeService),
                typeof(IWechatWorkProviderKfUpgradeService),
            }),
        };

        foreach (var (parentImplementationClassName, parent, children) in families)
        {
            foreach (var child in children)
            {
                child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");
            }

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

            foreach (var child in children)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(KfRegistryGroupName,
                    $"{child.Name} 必须挂 {KfRegistryGroupName} 注册组" +
                    $"（Kf 模块共用 Add{KfRegistryGroupName}WebApiHttpClient()）");
                childApi.InheritedFrom.Should().Be(parentImplementationClassName,
                    $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

                // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 KF1/KF2。
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
            }
        }
    }

    /// <summary>
    /// 契约守卫 KF2b：仅单类应用开放域的接口层级——机器人管理域（仅自建）与微信客服组件域（仅第三方）
    /// 均为零端点父接口 + 仅一个子接口承载端点，且继承链上不得出现其它应用类型子接口
    /// （形态对齐获客助手组件 / 已服务的外部联系人守卫）。
    /// </summary>
    [Fact]
    public void KfSingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var singleChildFamilies = new[]
        {
            (typeof(IWechatWorkKfKnowledgeService),
                new[] { typeof(IWechatWorkInternalKfKnowledgeService) },
                "机器人管理域官方仅向自建应用开放（第三方/代开发暂不支持）"),
            (typeof(IWechatWorkKfComponentService),
                new[] { typeof(IWechatWorkThirdPartyKfComponentService) },
                "微信客服组件域官方仅向微信客服组件应用（套件形态，第三方文档目录）开放"),
        };

        foreach (var (parent, expectedChildren, because) in singleChildFamilies)
        {
            var assignable = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
                .ToList();

            assignable.Should().BeEquivalentTo(expectedChildren,
                $"继承链上不得出现自建/第三方/代开发之外的子接口：{because}");

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty("零端点父接口：端点全部声明于唯一的子接口");

            foreach (var child in expectedChildren)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(KfRegistryGroupName,
                    $"{child.Name} 必须挂 {KfRegistryGroupName} 注册组");
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().NotBeEmpty($"{child.Name} 为唯一端点承载接口：{because}");
            }
        }
    }

    /// <summary>
    /// 契约守卫 KF3：令牌绑定——Kf 模块全部 28 个接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void KfTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkKfAccountService),
            typeof(IWechatWorkInternalKfAccountService),
            typeof(IWechatWorkThirdPartyKfAccountService),
            typeof(IWechatWorkProviderKfAccountService),
            typeof(IWechatWorkKfServicerService),
            typeof(IWechatWorkInternalKfServicerService),
            typeof(IWechatWorkThirdPartyKfServicerService),
            typeof(IWechatWorkProviderKfServicerService),
            typeof(IWechatWorkKfSessionService),
            typeof(IWechatWorkInternalKfSessionService),
            typeof(IWechatWorkThirdPartyKfSessionService),
            typeof(IWechatWorkProviderKfSessionService),
            typeof(IWechatWorkKfCustomerService),
            typeof(IWechatWorkInternalKfCustomerService),
            typeof(IWechatWorkThirdPartyKfCustomerService),
            typeof(IWechatWorkProviderKfCustomerService),
            typeof(IWechatWorkKfStatisticsService),
            typeof(IWechatWorkInternalKfStatisticsService),
            typeof(IWechatWorkThirdPartyKfStatisticsService),
            typeof(IWechatWorkProviderKfStatisticsService),
            typeof(IWechatWorkKfUpgradeService),
            typeof(IWechatWorkInternalKfUpgradeService),
            typeof(IWechatWorkThirdPartyKfUpgradeService),
            typeof(IWechatWorkProviderKfUpgradeService),
            typeof(IWechatWorkKfKnowledgeService),
            typeof(IWechatWorkInternalKfKnowledgeService),
            typeof(IWechatWorkKfComponentService),
            typeof(IWechatWorkThirdPartyKfComponentService),
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
    /// 契约守卫 KF4：微信客服模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 94 个契约面类型）。
    /// </summary>
    [Fact]
    public void KfDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = KfJsonContext.Default;

        var requiredTypes = new[]
        {
            // 客服账号管理域。
            typeof(AddKfAccountRequest), typeof(GetKfAccountListRequest), typeof(DeleteKfAccountRequest),
            typeof(UpdateKfAccountRequest), typeof(GetKfAccountContactWayRequest),
            typeof(AddKfAccountResponse), typeof(GetKfAccountListResponse), typeof(GetKfAccountContactWayResponse),
            typeof(KfAccount),
            // 接待人员管理域。
            typeof(AddKfServicerRequest), typeof(DeleteKfServicerRequest),
            typeof(AddKfServicerResponse), typeof(DeleteKfServicerResponse), typeof(GetKfServicerListResponse),
            typeof(KfServicer), typeof(KfServicerOperateResult),
            // 会话分配与消息收发域（信封基类 + 各 msgtype 请求体 + 消息体模型）。
            typeof(GetKfServiceStateRequest), typeof(TransitKfServiceStateRequest),
            typeof(GetKfServiceStateResponse), typeof(TransitKfServiceStateResponse),
            typeof(SendKfMsgRequest), typeof(SendKfTextMsgRequest), typeof(SendKfImageMsgRequest),
            typeof(SendKfVoiceMsgRequest), typeof(SendKfVideoMsgRequest), typeof(SendKfFileMsgRequest),
            typeof(SendKfLinkMsgRequest), typeof(SendKfMiniProgramMsgRequest), typeof(SendKfMenuMsgRequest),
            typeof(SendKfLocationMsgRequest), typeof(SendKfCaLinkMsgRequest),
            typeof(SendKfEventMsgRequest), typeof(SendKfEventTextMsgRequest), typeof(SendKfEventMenuMsgRequest),
            typeof(SendKfMsgResponse),
            typeof(KfTextMsgBody), typeof(KfMediaMsgBody), typeof(KfLinkMsgBody), typeof(KfMiniProgramMsgBody),
            typeof(KfMenuMsgBody), typeof(KfMenuMsgListItem), typeof(KfMenuClickItem), typeof(KfMenuViewItem),
            typeof(KfMenuMiniProgramItem), typeof(KfMenuTextItem),
            typeof(KfLocationMsgBody), typeof(KfCaLinkMsgBody),
            // 客户基础信息域。
            typeof(BatchGetKfCustomerInfoRequest), typeof(BatchGetKfCustomerInfoResponse),
            typeof(KfCustomerInfo), typeof(KfEnterSessionContext), typeof(KfWechatChannelsInfo),
            // 统计管理域。
            typeof(GetKfCorpStatisticRequest), typeof(GetKfCorpStatisticResponse),
            typeof(KfCorpStatisticItem), typeof(KfCorpStatisticData),
            typeof(GetKfServicerStatisticRequest), typeof(GetKfServicerStatisticResponse),
            typeof(KfServicerStatisticItem), typeof(KfServicerStatisticData),
            // 机器人管理域（知识库分组 + 问答）。
            typeof(AddKfKnowledgeGroupRequest), typeof(DeleteKfKnowledgeGroupRequest),
            typeof(UpdateKfKnowledgeGroupRequest), typeof(GetKfKnowledgeGroupListRequest),
            typeof(AddKfKnowledgeGroupResponse), typeof(GetKfKnowledgeGroupListResponse), typeof(KfKnowledgeGroup),
            typeof(AddKfKnowledgeIntentRequest), typeof(DeleteKfKnowledgeIntentRequest),
            typeof(UpdateKfKnowledgeIntentRequest), typeof(GetKfKnowledgeIntentListRequest),
            typeof(AddKfKnowledgeIntentResponse), typeof(GetKfKnowledgeIntentListResponse), typeof(KfKnowledgeIntent),
            typeof(KfKnowledgeText), typeof(KfKnowledgeQuestion), typeof(KfKnowledgeSimilarQuestions),
            typeof(KfKnowledgeAnswer), typeof(KfKnowledgeAttachment),
            typeof(KfKnowledgeImageAttachment), typeof(KfKnowledgeVideoAttachment),
            typeof(KfKnowledgeLinkAttachment), typeof(KfKnowledgeMiniProgramAttachment),
            // 微信客服组件域。
            typeof(GetKfComponentStatisticRequest), typeof(GetKfComponentStatisticResponse),
            typeof(KfComponentStatisticItem), typeof(KfComponentStatisticData),
            // 「升级服务」配置域。
            typeof(GetKfUpgradeServiceConfigResponse), typeof(KfUpgradeMemberRange), typeof(KfUpgradeGroupchatRange),
            typeof(UpgradeKfServiceRequest), typeof(KfUpgradeMember), typeof(KfUpgradeGroupchat),
            typeof(CancelKfUpgradeServiceRequest),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是微信客服模块契约面类型，必须登记进 KfJsonContext（AOT 源生成）");
        }
    }
}
