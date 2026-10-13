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
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 微信客服模块（Kf 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （客服账号管理域 + 接待人员管理域 + 会话分配与消息收发域 + 客户基础信息域 +
/// 统计管理域 + 机器人管理域（仅自建） + 微信客服组件域（仅第三方） + 「升级服务」配置域）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：客服账号管理 / 接待人员管理 / 会话分配与消息收发 / 客户基础信息 / 统计管理 / 「升级服务」配置六域为
/// 三类应用公共面收敛父接口（自建 / 第三方 / 代开发子接口均为零差异端点空标记）；
/// 机器人管理域官方仅自建开放（零端点父接口 + 仅自建子接口承载 8 端点）；
/// 微信客服组件域官方仅微信客服组件应用（套件形态）消费（零端点父接口 + 仅第三方子接口承载 3 端点，
/// 其中 2 条与客服账号管理域共用路由）；
/// 「其他基础信息获取」域已<b>整体撤回</b>（官方页面仅剩概述，见 <see cref="WithdrawnCorpQualificationRoutes"/> 与 KF1i）。
/// </para>
/// </remarks>
public class WechatKfContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
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
        // 添加客服账号（自建/第三方 94662、代开发 96404）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.AddAccountAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/add"),
        // 获取客服账号列表（自建/第三方 94661、代开发 96415）。
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
    /// 会话分配与消息收发域官方路由表（父接口 5 条路由 / 15 个端点方法：
    /// service_state 2 条 + sync_msg 1 条 + send_msg 10 个 msgtype 方法 + send_msg_on_event 2 个 msgtype 方法；
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
        // 同步消息与事件（自建 94670、第三方 94699、代开发 96426）。
        (typeof(IWechatWorkKfSessionService),
            nameof(IWechatWorkKfSessionService.SyncMessageAsync),
            typeof(PostAttribute), "/cgi-bin/kf/sync_msg"),
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
    /// 的官方页面已不含端点规范，该域整体撤回，见 <see cref="WithdrawnCorpQualificationRoutes"/> 与 KF1i）。
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
    /// 微信客服组件域官方路由表（仅第三方/组件应用消费；<b>单一所有者</b>：客服账号列表
    /// <c>kf/account/list</c> 与客服账号链接 <c>kf/add_contact_way</c>虽官方以组件文档
    /// （99368/99400）分文档承载，但与客服账号管理域为<b>同路由、同 DTO</b>，故只在客服账号管理域
    /// 父接口声明一次（见 <see cref="AccountRoutes"/>），本表仅保留组件域独有路由）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ComponentRoutes =
    {
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
    /// 「其他基础信息获取」域的非契约路由清单（官方文档面已撤回该端点，SDK 须零声明）。
    /// <para>
    /// 核验留档（2026-10-13）：官方两棵文档树的「微信客服 / 其他基础信息获取 / 获取企业状态信息」页面
    /// 正文均<b>只剩概述</b>——「微信客服可接待的客户数，和企业是否已完成主体验证相关……
    /// 开发者可获取状态后，在应用等地方提示企业去完成主体验证」，其「获取企业验证状态」小节的唯一指引是
    /// 「可通过<b>获取企业授权信息</b>接口获取」（服务商侧 <c>service/get_corp_auth_info</c>，本仓已承载），
    /// 页面<b>不含请求方式 / 请求地址 / 参数表 / 返回结果</b>。
    /// </para>
    /// <para>
    /// 判据是官方文档面而非网关可达性（AGENTS.md 定夺三）：第三方 SDK 与旧版文档里的
    /// <c>kf/get_corp_qualification</c> + <c>wechat_channels_binding</c> 属已下架页面遗留，
    /// 不得据此在本仓声明端点，故本域整体撤回（零端点父接口与子接口一并移除），并转由 KF1i 零声明守卫锁定。
    /// </para>
    /// </summary>
    private static readonly string[] WithdrawnCorpQualificationRoutes =
    {
        "/cgi-bin/kf/get_corp_qualification",
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
    /// <c>kf/sync_msg</c>（接收消息和事件）为三类应用公共面单一路由单方法，
    /// 入参 <c>token</c> 是事件通知下发的同步令牌，与 Query 注入的 <c>access_token</c> 无关，
    /// 须以请求体字段承载而非 <c>[Token]</c> 声明。
    /// </summary>
    [Fact]
    public void KfSessionEndpoints_ShouldMatchOfficialRoutes()
    {
        SessionRoutes.Should().HaveCount(15,
            "会话分配与消息收发域为 5 条路由 / 15 个端点方法（service_state 2 + sync_msg 1 + send_msg 10 + send_msg_on_event 2）");

        var distinctRoutes = SessionRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(5, "发送类路由按 msgtype 一方法一请求体收敛，实际路由仅 5 条");

        foreach (var (iface, method, httpAttribute, route) in SessionRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // sync_msg 的同步令牌为请求体字段（token），不得被 [Token] 的 access_token 注入替换。
        var syncBodyParam = typeof(IWechatWorkKfSessionService)
            .GetMethod(nameof(IWechatWorkKfSessionService.SyncMessageAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
            .GetParameters().Single(p => p.GetCustomAttribute<BodyAttribute>() != null);
        syncBodyParam.ParameterType.Should().Be<SyncKfMessageRequest>(
            "sync_msg 必须以 SyncKfMessageRequest 承载官方请求体（token/cursor/limit/voice_format/open_kfid）");
    }

    /// <summary>
    /// 契约守卫 KF1d：客户基础信息域全部端点路由必须与官方契约一致
    /// （batchget 为官方原文拼写，勿改 batch_get）。
    /// </summary>
    [Fact]
    public void KfCustomerEndpoints_ShouldMatchOfficialRoutes()
    {
        CustomerRoutes.Should().HaveCount(1,
            "客户基础信息域仅 1 个公共端点（「获取企业状态信息」官方页面已无端点规范，该域整体撤回，见 KF1i）");

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
    /// 契约守卫 KF1g：微信客服组件域<b>独有</b>端点路由必须与官方契约一致；
    /// <c>kf/account/list</c> 与 <c>kf/add_contact_way</c> 与客服账号管理域为同路由同 DTO，
    /// 已按单一所有者收敛为账号域声明，本守卫反向锁定组件域<b>不得</b>重复声明（KF1g2）。
    /// </summary>
    [Fact]
    public void KfComponentEndpoints_ShouldMatchOfficialRoutes()
    {
        ComponentRoutes.Should().HaveCount(1,
            "组件域仅保留 1 个独有端点（同路由端点归客服账号管理域单一所有者）");

        foreach (var (iface, method, httpAttribute, route) in ComponentRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 KF1g2（反重复守卫）：客服账号列表与客服账号链接两条路由在 SDK 内只允许
    /// 客服账号管理域一处声明——官方虽另以组件文档（99368/99400）分文档承载，但二者
    /// <b>同路由且共用同一批 DTO</b>，重复声明只会产生两份同形契约面。
    /// </summary>
    [Fact]
    public void KfComponentEndpoints_ShouldNotRedundantlyDeclareAccountRoutes()
    {
        var componentChild = typeof(IWechatWorkThirdPartyKfComponentService);
        var accountRoutes = new HashSet<string>(
            AccountRoutes.Select(r => r.Route), StringComparer.Ordinal);

        foreach (var route in accountRoutes)
        {
            var redeclared = AccountRoutes.First(r => r.Route == route);
            componentChild.GetMethod(redeclared.Method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeNull($"{route} 归客服账号管理域单一所有者声明，组件域不得重复声明");
        }
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
    /// 契约守卫 KF1i：「其他基础信息获取」域已<b>整体撤回</b>（官方页面只剩概述，正文指向
    /// 「获取企业授权信息接口」，无请求方式/地址/参数表）——本域不得残留任何接口类型或响应 DTO，
    /// 路由 <c>kf/get_corp_qualification</c> 的零声明由 RD1 非契约路由守卫锁定。
    /// 正对照：客服账号管理域与批次客户信息域接口必须仍可解析（否则本守卫空跑恒真）。
    /// </summary>
    [Fact]
    public void KfCorpQualificationDomain_ShouldStayWithdrawn()
    {
        WithdrawnCorpQualificationRoutes.Should().HaveCount(1,
            "「其他基础信息获取」域撤回清单本身即契约：增删须先复核官方页面正文（追踪号 P2-QY-KF-CORPQUALIFICATION-11）");

        var workAssembly = typeof(IWechatWorkKfAccountService).Assembly;
        var dataModelsAssembly = typeof(SyncKfMessageRequest).Assembly;

        var leftoverInterfaces = workAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.Contains("KfCorpQualification", StringComparison.Ordinal))
            .Select(t => t.Name)
            .ToList();
        leftoverInterfaces.Should().BeEmpty(
            "「获取企业状态信息」官方页面已不含端点规范（追踪号 P2-QY-KF-CORPQUALIFICATION-11），" +
            "本域零端点父接口与第三方子接口必须一并撤回，不得以「网关仍可达」为由加回");

        dataModelsAssembly
            .GetType("Mud.Wechat.Work.DataModels.Kf.GetKfCorpQualificationResponse", throwOnError: false)
            .Should().BeNull("无端点即无响应契约，wechat_channels_binding 属已下架页面遗留字段");

        // 正对照：同模块的在架接口仍可解析，证明上面的枚举不是空扫描。
        workAssembly.GetType("Mud.Wechat.Work.IWechatWorkThirdPartyKfCustomerService", throwOnError: false)
            .Should().NotBeNull("客户基础信息域（batchget 95159）仍在架，须作为本守卫的活体对照");
        workAssembly.GetType("Mud.Wechat.Work.IWechatWorkThirdPartyKfAccountService", throwOnError: false)
            .Should().NotBeNull("客服账号管理域仍在架，须作为本守卫的活体对照");
    }

    /// <summary>
    /// 契约守卫 KF1j：<c>kf/sync_msg</c> 响应消息体的官方判别字段名与按 msgtype 的载体字段名必须逐字保留
    /// （msg_list / has_more / next_cursor 与 msgmenu / business_card / channels_shop_product /
    /// channels_shop_order / merged_msg，勿写成 menu / card / 驼峰）。
    /// </summary>
    [Fact]
    public void KfSyncMessageOfficialFieldNames_ShouldBePreservedVerbatim()
    {
        var envelopeNames = new (PropertyInfo Property, string JsonName)[]
        {
            (typeof(SyncKfMessageResponse).GetProperty(nameof(SyncKfMessageResponse.MessageList))!, "msg_list"),
            (typeof(SyncKfMessageResponse).GetProperty(nameof(SyncKfMessageResponse.HasMore))!, "has_more"),
            (typeof(SyncKfMessageResponse).GetProperty(nameof(SyncKfMessageResponse.NextCursor))!, "next_cursor"),
        };

        foreach (var (property, jsonName) in envelopeNames)
        {
            property.Should().NotBeNull($"{nameof(SyncKfMessageResponse)}.{jsonName} 对应属性必须存在");
            property.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonName,
                $"同步消息响应字段名照官方原文 {jsonName}");
        }

        // has_more 官方线格式为数字 0/1（本仓 Kf 知识族分页响应亦以 int? 承载），改成 bool? 即在反序列化期失败。
        typeof(SyncKfMessageResponse).GetProperty(nameof(SyncKfMessageResponse.HasMore))!
            .PropertyType.Should().Be(typeof(int?),
                "同步消息响应的 has_more 官方以数字承载布尔语义，须以 int? 原样承载");

        var carrierNames = new (PropertyInfo Property, string JsonName)[]
        {
            (typeof(KfSyncMessage).GetProperty(nameof(KfSyncMessage.Menu))!, "msgmenu"),
            (typeof(KfSyncMessage).GetProperty(nameof(KfSyncMessage.BusinessCard))!, "business_card"),
            (typeof(KfSyncMessage).GetProperty(nameof(KfSyncMessage.ChannelsShopProduct))!, "channels_shop_product"),
            (typeof(KfSyncMessage).GetProperty(nameof(KfSyncMessage.ChannelsShopOrder))!, "channels_shop_order"),
            (typeof(KfSyncMessage).GetProperty(nameof(KfSyncMessage.MergedMessage))!, "merged_msg"),
            (typeof(KfSyncMessage).GetProperty(nameof(KfSyncMessage.OpenKfId))!, "open_kfid"),
            (typeof(KfSyncMessage).GetProperty(nameof(KfSyncMessage.SendTime))!, "send_time"),
        };

        foreach (var (property, jsonName) in carrierNames)
        {
            property.Should().NotBeNull($"{nameof(KfSyncMessage)}.{jsonName} 对应属性必须存在");
            property.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonName,
                $"消息体载体字段名照官方原文 {jsonName}");
        }
    }

    /// <summary>
    /// 契约守卫 KF1k：<c>kf/sync_msg</c> 请求体的必填/选填形态必须与官方 94670 参数表一致
    /// （追踪号 P2-QY-B8-KF-REQUIREDFACE-10）。历史缺陷：<c>open_kfid</c> 曾被写成「官方选填」、
    /// <c>token</c> 曾被写成「官方必填」——参数表口径恰好相反，故本守卫双向锁定。
    /// </summary>
    [Fact]
    public void KfSyncMessageRequestRequiredFace_ShouldMatchOfficialParameterTable()
    {
        var required = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(SyncKfMessageRequest), nameof(SyncKfMessageRequest.OpenKfId), "open_kfid"),
        };

        var optional = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(SyncKfMessageRequest), nameof(SyncKfMessageRequest.Token), "token"),
            (typeof(SyncKfMessageRequest), nameof(SyncKfMessageRequest.Cursor), "cursor"),
            (typeof(SyncKfMessageRequest), nameof(SyncKfMessageRequest.Limit), "limit"),
            (typeof(SyncKfMessageRequest), nameof(SyncKfMessageRequest.VoiceFormat), "voice_format"),
        };

        foreach (var (owner, property, jsonName) in required)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            IsNullableMember(prop!).Should().BeFalse(
                $"{jsonName} 官方参数表标为必填，不得声明为可空（可空即允许请求体省略必填参数 ⇒ 官方回参数错误）");
        }

        foreach (var (owner, property, jsonName) in optional)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            IsNullableMember(prop!).Should().BeTrue(
                $"{jsonName} 官方参数表标为选填，收敛为非可空会强制每个请求都带该参数（首次拉取无法省略 cursor）");
        }
    }

    /// <summary>
    /// 契约守卫 KF2：六类应用公共面域的接口层级与生成器注册形态——公共端点收敛于 IsAbstract 父接口，    /// 自建 / 第三方 / 代开发子接口均为零差异端点空标记（能力漂移守卫）。
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
    /// 均为零端点父接口 + 仅一个子接口承载端点，
    /// 且继承链上不得出现其它应用类型子接口（形态对齐获客助手组件 / 已服务的外部联系人守卫）。
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
    /// 契约守卫 KF4：微信客服模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 107 个契约面类型）。
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
            // 会话分配与消息收发域 · 同步消息与事件（信封 + 消息体 + 各 msgtype 载体）。
            typeof(SyncKfMessageRequest), typeof(SyncKfMessageResponse), typeof(KfSyncMessage),
            typeof(KfSyncTextMsgBody), typeof(KfSyncLinkMsgBody), typeof(KfSyncBusinessCardMsgBody),
            typeof(KfSyncEventMsgBody), typeof(KfSyncEventWechatChannels), typeof(KfSyncChannelsMsgBody),
            typeof(KfSyncChannelsShopProductMsgBody), typeof(KfSyncChannelsShopOrderMsgBody),
            typeof(KfSyncMergedMsgBody), typeof(KfSyncMergedMsgItem),
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

    /// <summary>
    /// 属性是否可空：值类型的 <see cref="Nullable{T}"/>，或引用类型的可空标注
    /// （<c>PropertyType</c> 不含引用可空性信息，须走 Nullable Context API）。
    /// </summary>
    private static bool IsNullableMember(PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) != null)
        {
            return true;
        }

        return new NullabilityInfoContext().Create(property).WriteState == NullabilityState.Nullable;
    }
}
