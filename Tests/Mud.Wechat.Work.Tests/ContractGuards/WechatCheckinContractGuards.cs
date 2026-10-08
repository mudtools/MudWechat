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
using Mud.Wechat.Work.DataModels.Checkin;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 打卡模块（Checkin 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 五功能族形态：
/// 打卡规则族（获取员工打卡规则三类公共收敛父接口；获取企业所有打卡规则 + 管理打卡规则 4 写端点为自建/代开发差异端点，第三方暂不支持零端点空标记）；
/// 打卡记录族（获取打卡记录数据三类开放但第三方文档页为旧字段结构——同路由不同构无法收敛进父接口，父接口零端点，三分支子接口各自承载；补卡/添加打卡记录/录入人脸官方仅自建）；
/// 打卡报表族（日报/月报三类开放但第三方文档页为旧字段结构，父接口零端点，三分支子接口各自承载）；
/// 打卡排班族（获取/设置排班三类公共收敛父接口 + 空标记子接口）；
/// 设备打卡数据族（获取设备打卡数据三类公共收敛父接口 + 空标记子接口，路由挂 /cgi-bin/hardware/ 域）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：打卡域 15 条路由官方全部即 POST（含仅查询语义的 getcheckinoption /
/// getcorpcheckinoption / getcheckinschedulist / get_hardware_checkin_data）；获取企业所有打卡规则请求体为空 JSON 对象；
/// 获取打卡记录数据 / 日报 / 月报三种应用类型同路由但第三方文档页为旧字段结构
/// （94205/94206/94207：baseinfo、agency_name、location_title_lat/lng、excepion_days_cnt 等），
/// 与自建/代开发页（90262/93374/93387、96497-96499）的 base_info/summary_info/ot_info 现代结构不同构；
/// 排班响应 schedule.scheduleList 为 camelCase（同页其余字段 snake_case）；yearmonth 为 uint32 数字而非字符串；
/// 员工打卡规则返回示例含字面量点号键 "group.checkin_method_type"；规则请求侧加班配置为 ot_info_v2、
/// 响应侧为旧 ot_info（官方错误表明确 ot_info 是旧字段不建议使用），两结构不同构；
/// 字段名照抄官方原文（lastest_time、acctivity_name、excepion_days_cnt 等）。
/// </para>
/// </remarks>
public class WechatCheckinContractGuards
{
    private const string CheckinRegistryGroupName = "Checkin";

    // ------------------------------------------------------------------
    // 路由表：15 条官方路由，全部 POST（勿「顺手统一」为 GET）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        CheckinRuleRoutes =
        {
            // 获取员工打卡规则（自建 90263 / 第三方 94204 / 代开发 99443；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkCheckinRuleService),
                nameof(IWechatWorkCheckinRuleService.GetCheckinOptionAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckinoption"),
            // 获取企业所有打卡规则（自建 93384 / 代开发 99444；官方即 POST；请求体为空 JSON 对象）。
            (typeof(IWechatWorkInternalCheckinRuleService),
                nameof(IWechatWorkInternalCheckinRuleService.GetCorpCheckinOptionAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcorpcheckinoption"),
            // 管理打卡规则 4 写端点（自建 98041 / 代开发 98767；官方即 POST）。
            (typeof(IWechatWorkInternalCheckinRuleService),
                nameof(IWechatWorkInternalCheckinRuleService.AddCheckinOptionAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/add_checkin_option"),
            (typeof(IWechatWorkInternalCheckinRuleService),
                nameof(IWechatWorkInternalCheckinRuleService.UpdateCheckinOptionAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/update_checkin_option"),
            (typeof(IWechatWorkInternalCheckinRuleService),
                nameof(IWechatWorkInternalCheckinRuleService.ClearCheckinOptionArrayFieldAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/clear_checkin_option_array_field"),
            (typeof(IWechatWorkInternalCheckinRuleService),
                nameof(IWechatWorkInternalCheckinRuleService.DelCheckinOptionAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/del_checkin_option"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        CheckinRecordRoutes =
        {
            // 获取打卡记录数据（自建 90262 / 代开发 96497 为现代结构；第三方 94205 为旧字段结构——同路由三分支分形态承载）。
            (typeof(IWechatWorkInternalCheckinRecordService),
                nameof(IWechatWorkInternalCheckinRecordService.GetCheckinDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckindata"),
            (typeof(IWechatWorkProviderCheckinRecordService),
                nameof(IWechatWorkProviderCheckinRecordService.GetCheckinDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckindata"),
            (typeof(IWechatWorkThirdPartyCheckinRecordService),
                nameof(IWechatWorkThirdPartyCheckinRecordService.GetCheckinDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckindata"),
            // 为打卡人员补卡（自建 95803；代开发/第三方暂不支持；官方即 POST）。
            (typeof(IWechatWorkInternalCheckinRecordService),
                nameof(IWechatWorkInternalCheckinRecordService.PunchCorrectionAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/punch_correction"),
            // 添加打卡记录（自建 99647；代开发/第三方暂不支持）。
            (typeof(IWechatWorkInternalCheckinRecordService),
                nameof(IWechatWorkInternalCheckinRecordService.AddCheckinRecordAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/add_checkin_record"),
            // 录入打卡人员人脸信息（自建 93378；代开发/第三方暂不支持）。
            (typeof(IWechatWorkInternalCheckinRecordService),
                nameof(IWechatWorkInternalCheckinRecordService.AddCheckinUserFaceAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/addcheckinuserface"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        CheckinReportRoutes =
        {
            // 获取打卡日报数据（自建 93374 / 代开发 96498 现代结构；第三方 94206 旧字段结构；官方即 POST）。
            (typeof(IWechatWorkInternalCheckinReportService),
                nameof(IWechatWorkInternalCheckinReportService.GetCheckinDayDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckin_daydata"),
            (typeof(IWechatWorkProviderCheckinReportService),
                nameof(IWechatWorkProviderCheckinReportService.GetCheckinDayDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckin_daydata"),
            (typeof(IWechatWorkThirdPartyCheckinReportService),
                nameof(IWechatWorkThirdPartyCheckinReportService.GetCheckinDayDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckin_daydata"),
            // 获取打卡月报数据（自建 93387 / 代开发 96499 现代结构；第三方 94207 旧字段结构）。
            (typeof(IWechatWorkInternalCheckinReportService),
                nameof(IWechatWorkInternalCheckinReportService.GetCheckinMonthDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckin_monthdata"),
            (typeof(IWechatWorkProviderCheckinReportService),
                nameof(IWechatWorkProviderCheckinReportService.GetCheckinMonthDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckin_monthdata"),
            (typeof(IWechatWorkThirdPartyCheckinReportService),
                nameof(IWechatWorkThirdPartyCheckinReportService.GetCheckinMonthDataAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckin_monthdata"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        CheckinScheduleRoutes =
        {
            // 获取打卡人员排班信息（自建 93380 / 第三方 94208 / 代开发 96500；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkCheckinScheduleService),
                nameof(IWechatWorkCheckinScheduleService.GetCheckinScheduleListAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/getcheckinschedulist"),
            // 为打卡人员排班（自建 93385 / 第三方 94209 / 代开发 96501）。
            (typeof(IWechatWorkCheckinScheduleService),
                nameof(IWechatWorkCheckinScheduleService.SetCheckinScheduleListAsync),
                typeof(PostAttribute), "/cgi-bin/checkin/setcheckinschedulist"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        CheckinDeviceRoutes =
        {
            // 获取设备打卡数据（自建 94126 / 第三方 95176 / 代开发 96504；三类公共收敛父接口；
            // 官方即 POST；路由挂 /cgi-bin/hardware/ 域而非 /cgi-bin/checkin/ 域，勿「顺手归位」）。
            (typeof(IWechatWorkCheckinDeviceService),
                nameof(IWechatWorkCheckinDeviceService.GetHardwareCheckinDataAsync),
                typeof(PostAttribute), "/cgi-bin/hardware/get_hardware_checkin_data"),
        };

    // ------------------------------------------------------------------
    // CK1：全部端点路由与官方契约一致（15 条官方路由去重后 12 条 + 同路由三分支 3 条）。
    // ------------------------------------------------------------------

    [Fact]
    public void CheckinEndpoints_ShouldMatchOfficialRoutes()
    {
        // 打卡规则族：父接口 1 公共端点 + 自建 5 差异端点（代开发同构同路由，不重复断言路由值）。
        CheckinRuleRoutes.Should().HaveCount(6, "打卡规则族 = 获取员工打卡规则（公共）+ 获取企业所有打卡规则 + 管理打卡规则 4 写端点");
        CheckinRuleRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/checkin/", StringComparison.Ordinal), "打卡规则族路由位于 /cgi-bin/checkin/ 段");

        // 打卡记录族：获取打卡记录数据同路由三分支 + 补卡 / 添加打卡记录 / 录入人脸。
        CheckinRecordRoutes.Should().HaveCount(6, "打卡记录族 = 获取打卡记录数据（同路由三分支）+ 补卡 + 添加打卡记录 + 录入人脸");
        CheckinRecordRoutes.Count(r => r.Interface == typeof(IWechatWorkInternalCheckinRecordService))
            .Should().Be(4, "自建子接口承载获取打卡记录数据（现代结构）+ 补卡 + 添加打卡记录 + 录入人脸");
        CheckinRecordRoutes.Count(r => r.Interface == typeof(IWechatWorkProviderCheckinRecordService))
            .Should().Be(1, "代开发子接口仅承载获取打卡记录数据（现代结构）");
        CheckinRecordRoutes.Count(r => r.Interface == typeof(IWechatWorkThirdPartyCheckinRecordService))
            .Should().Be(1, "第三方子接口仅承载获取打卡记录数据（旧字段结构）");
        CheckinRecordRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/checkin/", StringComparison.Ordinal), "打卡记录族路由位于 /cgi-bin/checkin/ 段");

        // 打卡报表族：日报 + 月报各三分支。
        CheckinReportRoutes.Should().HaveCount(6, "打卡报表族 = 日报三分支 + 月报三分支");
        CheckinReportRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/checkin/", StringComparison.Ordinal), "打卡报表族路由位于 /cgi-bin/checkin/ 段");

        // 打卡排班族：获取排班 + 设置排班。
        CheckinScheduleRoutes.Should().HaveCount(2, "打卡排班族 = 获取排班信息 + 为人员排班");

        // 设备打卡数据族：路由挂 hardware 域（官方原文如此，勿「顺手归位」到 checkin 域）。
        CheckinDeviceRoutes.Should().HaveCount(1, "设备打卡数据族 = 获取设备打卡数据单端点");
        CheckinDeviceRoutes.Single().Route.Should().StartWith(
            "/cgi-bin/hardware/", "获取设备打卡数据官方路由挂 /cgi-bin/hardware/ 域（get_hardware_checkin_data）");

        AssertRoutes(CheckinRuleRoutes);
        AssertRoutes(CheckinRecordRoutes);
        AssertRoutes(CheckinReportRoutes);
        AssertRoutes(CheckinScheduleRoutes);
        AssertRoutes(CheckinDeviceRoutes);

        // 全部官方路由去重清单锁定（同路由三分支去重后 12 条）。
        var allRoutes = CheckinRuleRoutes.Concat(CheckinRecordRoutes).Concat(CheckinReportRoutes)
            .Concat(CheckinScheduleRoutes).Concat(CheckinDeviceRoutes)
            .Select(r => r.Route).Distinct().ToList();
        allRoutes.Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/checkin/getcheckinoption",
            "/cgi-bin/checkin/getcorpcheckinoption",
            "/cgi-bin/checkin/add_checkin_option",
            "/cgi-bin/checkin/update_checkin_option",
            "/cgi-bin/checkin/clear_checkin_option_array_field",
            "/cgi-bin/checkin/del_checkin_option",
            "/cgi-bin/checkin/getcheckindata",
            "/cgi-bin/checkin/punch_correction",
            "/cgi-bin/checkin/add_checkin_record",
            "/cgi-bin/checkin/addcheckinuserface",
            "/cgi-bin/checkin/getcheckin_daydata",
            "/cgi-bin/checkin/getcheckin_monthdata",
            "/cgi-bin/checkin/getcheckinschedulist",
            "/cgi-bin/checkin/setcheckinschedulist",
            "/cgi-bin/hardware/get_hardware_checkin_data",
        }, "打卡域全部官方路由须与官方文档一一对应（15 条路由 / 12 条去重后含 hardware 域 1 条）");
        allRoutes.Should().HaveCount(15, "打卡域共 15 条官方路由");
    }

    // ------------------------------------------------------------------
    // CK2：接口层级与生成器注册形态（五族继承链、父/子端点数与开放面收敛）。
    // ------------------------------------------------------------------

    [Fact]
    public void CheckinInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 打卡规则族：公共父（1 端点）+ 自建/代开发各 5 差异端点 + 第三方零端点空标记（官方对第三方标注暂不支持）。
        AssertFamily(
            parent: typeof(IWechatWorkCheckinRuleService),
            parentImplementation: "WechatWorkCheckinRuleService",
            parentDeclaredEndpointCount: 1,
            new[]
            {
                (typeof(IWechatWorkInternalCheckinRuleService), 5),
                (typeof(IWechatWorkProviderCheckinRuleService), 5),
                (typeof(IWechatWorkThirdPartyCheckinRuleService), 0),
            });

        // 打卡记录族：父接口零端点（获取打卡记录数据同路由不同构无法收敛），三分支各自承载。
        AssertFamily(
            parent: typeof(IWechatWorkCheckinRecordService),
            parentImplementation: "WechatWorkCheckinRecordService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalCheckinRecordService), 4),
                (typeof(IWechatWorkProviderCheckinRecordService), 1),
                (typeof(IWechatWorkThirdPartyCheckinRecordService), 1),
            });

        // 打卡报表族：父接口零端点（日报/月报同路由不同构无法收敛），三分支各自承载。
        AssertFamily(
            parent: typeof(IWechatWorkCheckinReportService),
            parentImplementation: "WechatWorkCheckinReportService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalCheckinReportService), 2),
                (typeof(IWechatWorkProviderCheckinReportService), 2),
                (typeof(IWechatWorkThirdPartyCheckinReportService), 2),
            });

        // 打卡排班族：公共父 2 端点 + 三个空标记子接口。
        AssertFamily(
            parent: typeof(IWechatWorkCheckinScheduleService),
            parentImplementation: "WechatWorkCheckinScheduleService",
            parentDeclaredEndpointCount: 2,
            new[]
            {
                (typeof(IWechatWorkInternalCheckinScheduleService), 0),
                (typeof(IWechatWorkProviderCheckinScheduleService), 0),
                (typeof(IWechatWorkThirdPartyCheckinScheduleService), 0),
            });

        // 设备打卡数据族：公共父 1 端点 + 三个空标记子接口。
        AssertFamily(
            parent: typeof(IWechatWorkCheckinDeviceService),
            parentImplementation: "WechatWorkCheckinDeviceService",
            parentDeclaredEndpointCount: 1,
            new[]
            {
                (typeof(IWechatWorkInternalCheckinDeviceService), 0),
                (typeof(IWechatWorkProviderCheckinDeviceService), 0),
                (typeof(IWechatWorkThirdPartyCheckinDeviceService), 0),
            });
    }

    /// <summary>族断言：父接口 IsAbstract + 指定端点数，子接口集合不漂移 + 指定端点数 + 注册组/继承契约。</summary>
    private static void AssertFamily(
        Type parent,
        string parentImplementation,
        int parentDeclaredEndpointCount,
        (Type Interface, int DeclaredEndpointCount)[] children)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(parentDeclaredEndpointCount, $"{parent.Name} 承载官方开放面收敛的端点数");

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
        assignable.Should().BeEquivalentTo(children.Select(c => c.Interface),
            $"{parent.Name} 继承链子接口集合不得漂移（官方未开放的应用类型不得补子接口）");

        foreach (var (child, endpointCount) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(CheckinRegistryGroupName,
                $"{child.Name} 必须挂 {CheckinRegistryGroupName} 注册组（Checkin 模块共用 Add{CheckinRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 承载官方开放面差异端点数");
        }
    }

    // ------------------------------------------------------------------
    // CK3：令牌绑定——打卡域 20 个接口统一 AccessToken 路由键 + Query 注入。
    // ------------------------------------------------------------------

    [Fact]
    public void CheckinTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkCheckinRuleService),
            typeof(IWechatWorkInternalCheckinRuleService),
            typeof(IWechatWorkProviderCheckinRuleService),
            typeof(IWechatWorkThirdPartyCheckinRuleService),
            typeof(IWechatWorkCheckinRecordService),
            typeof(IWechatWorkInternalCheckinRecordService),
            typeof(IWechatWorkProviderCheckinRecordService),
            typeof(IWechatWorkThirdPartyCheckinRecordService),
            typeof(IWechatWorkCheckinReportService),
            typeof(IWechatWorkInternalCheckinReportService),
            typeof(IWechatWorkProviderCheckinReportService),
            typeof(IWechatWorkThirdPartyCheckinReportService),
            typeof(IWechatWorkCheckinScheduleService),
            typeof(IWechatWorkInternalCheckinScheduleService),
            typeof(IWechatWorkProviderCheckinScheduleService),
            typeof(IWechatWorkThirdPartyCheckinScheduleService),
            typeof(IWechatWorkCheckinDeviceService),
            typeof(IWechatWorkInternalCheckinDeviceService),
            typeof(IWechatWorkProviderCheckinDeviceService),
            typeof(IWechatWorkThirdPartyCheckinDeviceService),
        };

        accessTokenInterfaces.Should().HaveCount(20, "打卡域五族 × 4 接口（公共父 + 三应用类型子接口）");

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    // ------------------------------------------------------------------
    // CK4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Checkin）。
    // ------------------------------------------------------------------

    [Fact]
    public void CheckinDataModels_ShouldBeRegisteredInJsonContext()
    {
        var checkinContext = CheckinJsonContext.Default;

        var domainTypes = typeof(CheckinRecord).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Checkin"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 CheckinJsonContext 且 SerializerClassName 统一为 Checkin
        //（生成物 CheckinJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(104,
            "打卡模块契约面类型数漂移须先核对官方文档再同批调整本守卫（规则族 48 + 记录族 9 + 报表族 35 + 排班族 9 + 设备族 3）");

        foreach (var type in domainTypes)
        {
            checkinContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于打卡域命名空间，必须登记进 CheckinJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Checkin",
                $"{type.Name} 的 SerializerClassName 必须为打卡域段 Checkin");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            // 打卡规则族。
            typeof(GetCheckinOptionRequest), typeof(GetCheckinOptionResponse),
            typeof(GetCorpCheckinOptionRequest), typeof(GetCorpCheckinOptionResponse),
            typeof(AddCheckinOptionRequest), typeof(UpdateCheckinOptionRequest),
            typeof(ClearCheckinOptionArrayFieldRequest), typeof(DelCheckinOptionRequest),
            typeof(CheckinOptionInfo), typeof(CheckinGroup), typeof(CheckinCorpGroup),
            typeof(CheckinRuleGroup), typeof(CheckinOtInfoV2), typeof(CheckinOtInfo),
            // 打卡记录族。
            typeof(GetCheckinDataRequest), typeof(GetCheckinDataResponse), typeof(CheckinRecord),
            typeof(GetThirdPartyCheckinDataResponse), typeof(ThirdPartyCheckinRecord),
            typeof(PunchCorrectionRequest), typeof(AddCheckinRecordRequest), typeof(AddCheckinUserFaceRequest),
            // 打卡报表族。
            typeof(GetCheckinDayDataRequest), typeof(GetCheckinDayDataResponse), typeof(CheckinDayData),
            typeof(GetCheckinMonthDataRequest), typeof(GetCheckinMonthDataResponse), typeof(CheckinMonthData),
            typeof(GetThirdPartyCheckinDayDataResponse), typeof(GetThirdPartyCheckinMonthDataResponse),
            // 打卡排班族。
            typeof(GetCheckinScheduleListRequest), typeof(GetCheckinScheduleListResponse),
            typeof(SetCheckinScheduleListRequest),
            // 设备打卡数据族。
            typeof(GetHardwareCheckinDataRequest), typeof(GetHardwareCheckinDataResponse),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于打卡域命名空间");
    }

    // ------------------------------------------------------------------
    // CK5：官方契约陷阱锁定——同路由不同构分形态、字段形态与拼写照抄。
    // ------------------------------------------------------------------

    [Fact]
    public void CheckinDataModels_ShouldLockOfficialContractTraps()
    {
        // 获取打卡记录数据：自建/代开发现代结构与第三方旧字段结构必须分形态承载（同路由不同构）。
        typeof(GetCheckinDataResponse).GetProperty(nameof(GetCheckinDataResponse.Checkindata))!
            .PropertyType.Should().Be(typeof(List<CheckinRecord>),
                "自建/代开发文档页打卡记录为现代字段结构（lat/lng/deviceid/sch_checkin_time）");
        typeof(GetThirdPartyCheckinDataResponse).GetProperty(nameof(GetThirdPartyCheckinDataResponse.Checkindata))!
            .PropertyType.Should().Be(typeof(List<ThirdPartyCheckinRecord>),
                "第三方文档页打卡记录为旧字段结构（agency_name/location_title_lat/location_title_lng）");
        JsonNameShouldBe(typeof(CheckinRecord), nameof(CheckinRecord.SchCheckinTime), "sch_checkin_time");
        JsonNameShouldBe(typeof(ThirdPartyCheckinRecord), nameof(ThirdPartyCheckinRecord.AgencyName), "agency_name");
        JsonNameShouldBe(typeof(ThirdPartyCheckinRecord), nameof(ThirdPartyCheckinRecord.LocationTitleLat), "location_title_lat");
        JsonNameShouldBe(typeof(ThirdPartyCheckinRecord), nameof(ThirdPartyCheckinRecord.ScheduleCheckinTime), "schedule_checkin_time");

        // 打卡类型形态差异：现代结构为中文字符串、第三方旧结构参数表标注字符串但示例为整数（以示例为准）。
        typeof(CheckinRecord).GetProperty(nameof(CheckinRecord.CheckinType))!
            .PropertyType.Should().Be(typeof(string), "自建/代开发文档页 checkin_type 为中文字符串（如「上班打卡」）");
        typeof(ThirdPartyCheckinRecord).GetProperty(nameof(ThirdPartyCheckinRecord.CheckinType))!
            .PropertyType.Should().Be(typeof(int?), "第三方文档页 checkin_type 参数表标注字符串但返回示例为整数，以示例的整数形态承载");

        // 打卡报表同路由不同构：日报/月报顶层字段 base_info（现代）vs baseinfo（第三方旧结构）。
        JsonNameShouldBe(typeof(CheckinDayData), nameof(CheckinDayData.BaseInfo), "base_info");
        JsonNameShouldBe(typeof(ThirdPartyCheckinDayData), nameof(ThirdPartyCheckinDayData.Baseinfo), "baseinfo");
        JsonNameShouldBe(typeof(CheckinMonthData), nameof(CheckinMonthData.BaseInfo), "base_info");
        JsonNameShouldBe(typeof(ThirdPartyCheckinMonthData), nameof(ThirdPartyCheckinMonthData.Baseinfo), "baseinfo");
        typeof(CheckinDayData).GetProperty(nameof(CheckinDayData.OtInfo))!.PropertyType
            .Should().Be(typeof(CheckinDayOtInfo), "日报加班信息 ot_info 与月报 overwork_info 字段命名不对称，两结构不可共用");
        typeof(CheckinMonthData).GetProperty(nameof(CheckinMonthData.OverworkInfo))!.PropertyType
            .Should().Be(typeof(CheckinMonthOverworkInfo), "月报加班信息 overwork_info 字段单复数不对称，照抄勿改");
        JsonNameShouldBe(typeof(CheckinMonthOverworkInfo), nameof(CheckinMonthOverworkInfo.WorkdayOverSec), "workday_over_sec");
        JsonNameShouldBe(typeof(CheckinMonthOverworkInfo), nameof(CheckinMonthOverworkInfo.WorkdaysOverAsMoney), "workdays_over_as_money");
        JsonNameShouldBe(typeof(CheckinDaySummaryInfo), nameof(CheckinDaySummaryInfo.LastestTime), "lastest_time");

        // 规则族：请求侧 ot_info_v2 与响应侧旧 ot_info 不同构（官方错误表明确 ot_info 是旧字段不建议使用）。
        JsonNameShouldBe(typeof(CheckinRuleGroup), nameof(CheckinRuleGroup.OtInfoV2), "ot_info_v2");
        typeof(CheckinRuleGroup).GetProperty(nameof(CheckinRuleGroup.OtInfoV2))!.PropertyType
            .Should().Be(typeof(CheckinOtInfoV2), "创建/修改打卡规则请求侧加班配置为 ot_info_v2");
        JsonNameShouldBe(typeof(CheckinCorpGroup), nameof(CheckinCorpGroup.OtInfo), "ot_info");
        typeof(CheckinCorpGroup).GetProperty(nameof(CheckinCorpGroup.OtInfo))!.PropertyType
            .Should().Be(typeof(CheckinOtInfo), "获取企业所有打卡规则响应侧加班信息为旧 ot_info 结构");
        JsonNameShouldBe(typeof(CheckinOtInfoV2), nameof(CheckinOtInfoV2.Workdayconf), "workdayconf");
        JsonNameShouldBe(typeof(CheckinOtInfo), nameof(CheckinOtInfo.Otapplyinfo), "otapplyinfo");

        // 规则响应 checkindate.checkintime：官方参数表标注 uint32，返回示例为对象数组（以示例为准）。
        typeof(CheckinGroupCheckindate).GetProperty(nameof(CheckinGroupCheckindate.Checkintime))!
            .PropertyType.Should().Be(typeof(List<CheckinCheckintime>),
                "获取企业所有打卡规则 checkindate.checkintime 参数表类型标注 uint32 系官方笔误，返回示例为对象数组");

        // range.userid：官方参数表标注 string，返回示例为字符串数组（以示例为准）。
        typeof(CheckinRange).GetProperty(nameof(CheckinRange.Userid))!
            .PropertyType.Should().Be(typeof(List<string>), "range.userid 返回示例为字符串数组形态");

        // 获取企业所有打卡规则请求体为空 JSON 对象（零属性 DTO 承载 {}）。
        typeof(GetCorpCheckinOptionRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Should().BeEmpty("获取企业所有打卡规则官方请求体为空 JSON 对象（仅 Query 上携带 access_token）");

        // 排班：schedule.scheduleList 为 camelCase 字段名（同页其余字段 snake_case，照抄勿改）；yearmonth 为 uint32 数字。
        JsonNameShouldBe(typeof(CheckinPersonalSchedule), nameof(CheckinPersonalSchedule.ScheduleList), "scheduleList");
        JsonNameShouldBe(typeof(CheckinScheduleListInfo), nameof(CheckinScheduleListInfo.Schedule), "schedule");
        typeof(CheckinScheduleListInfo).GetProperty(nameof(CheckinScheduleListInfo.Yearmonth))!
            .PropertyType.Should().Be(typeof(int?), "yearmonth 官方类型 uint32 数字（如 202011），非字符串");
        JsonNameShouldBe(typeof(CheckinScheduleTimeSection), nameof(CheckinScheduleTimeSection.Id), "id");

        // 员工打卡规则返回示例的字面量点号键陷阱：checkin_method_type 按嵌套形态建模。
        JsonNameShouldBe(typeof(CheckinGroup), nameof(CheckinGroup.CheckinMethodType), "checkin_method_type");

        // 为打卡人员补卡：schedule_checkin_time 为相对当天 0 点的偏移秒数（与 Unix 时间戳形态并存）。
        typeof(PunchCorrectionRequest).GetProperty(nameof(PunchCorrectionRequest.ScheduleCheckinTime))!
            .PropertyType.Should().Be(typeof(int?), "补卡 schedule_checkin_time 为相对当天 0 点的偏移秒数（如 32400 = 9:00）");
        JsonNameShouldBe(typeof(PunchCorrectionRequest), nameof(PunchCorrectionRequest.ScheduleDateTime), "schedule_date_time");

        // 录入人脸：userface 为 base64 字符串（非 media_id）；官方「必须」列标注为否。
        JsonNameShouldBe(typeof(AddCheckinUserFaceRequest), nameof(AddCheckinUserFaceRequest.Userface), "userface");

        // 设备打卡：路由挂 hardware 域，记录仅 4 字段。
        JsonNameShouldBe(typeof(HardwareCheckinRecord), nameof(HardwareCheckinRecord.DeviceSn), "device_sn");
        JsonNameShouldBe(typeof(HardwareCheckinRecord), nameof(HardwareCheckinRecord.DeviceName), "device_name");

        // 第三方月报官方拼写照抄（勿「顺手修正」）。
        JsonNameShouldBe(typeof(ThirdPartyCheckinMonthBaseInfo), nameof(ThirdPartyCheckinMonthBaseInfo.AcctivityName), "acctivity_name");
        JsonNameShouldBe(typeof(ThirdPartyCheckinOverwork), nameof(ThirdPartyCheckinOverwork.ExcepionDaysCnt), "excepion_days_cnt");
        typeof(ThirdPartyCheckinMonthBaseInfo).GetProperty(nameof(ThirdPartyCheckinMonthBaseInfo.Overwork))!
            .PropertyType.Should().Be(typeof(List<ThirdPartyCheckinOverwork>),
                "第三方月报 overwork 参数表描述为单对象、返回示例为数组，以示例的数组形态承载");
        typeof(ThirdPartyCheckinMonthBaseInfo).GetProperty(nameof(ThirdPartyCheckinMonthBaseInfo.OvTime))!
            .PropertyType.Should().Be(typeof(List<ThirdPartyCheckinOverwork>),
                "第三方月报 ov_time 参数表描述为单对象、返回示例为数组（审批口径与打卡口径同构）");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, Type HttpAttribute, string Route)[] routes)
    {
        foreach (var (iface, method, httpAttribute, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致（拼写差异属官方契约）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
