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
/// WechatContractGuards 的 partial 分文件（F2 守卫拆分，审查报告 v1）：断言方法逐字搬移、
/// 类名与测试 FQN 不变 —— 本文件承载 OfficialRoutes_ShouldHaveSingleDeclaringInterface。
/// </summary>
public partial class WechatContractGuards
{
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
}
