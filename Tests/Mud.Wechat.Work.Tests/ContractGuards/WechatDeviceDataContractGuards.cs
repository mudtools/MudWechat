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
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.DeviceData;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 智能设备数据模块（DeviceData 模块）契约守卫：路由表、接口层级、令牌绑定、上下文登记与字段名 / 线格式锁定。
/// <para>
/// 官方「智慧硬件服务端」文档树（分类 96107 → 接口调用）只在<b>第三方应用开发</b>根下存在页面
/// （数据获取族 96097 / 96027 / 96028 / 96029 + 门禁规则管理族 96030 / 96031 / 96221 / 96227），
/// 企业自建应用与服务商代开发两棵树均无对应页面 ⇒ 「零端点父接口 + 唯一第三方子接口」，
/// 继承链上不得出现自建 / 代开发子接口。
/// </para>
/// <para>
/// 与「智能硬件开发」根树的 <c>/cgi-bin/openhw/*</c> 分属官方两棵章节树（openhw 凭据链为
/// <c>model_token</c> / <c>device_token</c>，不属三类应用令牌面 ⇒ 本域零承载）。
/// </para>
/// </summary>
public class WechatDeviceDataContractGuards
{
    private const string DeviceDataRegistryGroupName = "DeviceData";

    /// <summary>
    /// 智能设备数据域官方路由表（8 端点，官方页 96097/96027/96028/96029/96030/96031/96221/96227，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.GetDeviceAuthInfoAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/get_auth_info"),
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.GetDeviceCheckinDataAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/get_checkin_data"),
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.GetDeviceTemperatureDataAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/get_temperature_data"),
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.GetDeviceAccessControlDataAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/get_accesscontrol_data"),
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.GetDeviceAccessControlRuleAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/get_accesscontrol_rule"),
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.AddDeviceAccessControlRuleAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/add_accesscontrol_rule"),
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.ModifyDeviceAccessControlRuleAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/mod_accesscontrol_rule"),
        (typeof(IWechatWorkThirdPartyDeviceDataService),
            nameof(IWechatWorkThirdPartyDeviceDataService.DeleteDeviceAccessControlRuleAsync),
            typeof(PostAttribute), "/cgi-bin/devicedata/del_accesscontrol_rule"),
    };

    /// <summary>
    /// 契约守卫 DD1：路由与官方契约逐条一致，且三条数据面的请求参数名逐字段照抄官方原文
    /// （begin_time / end_time 带下划线，与汇报域的 starttime / endtime 不同口径）。
    /// </summary>
    [Fact]
    public void DeviceDataRoutesAndRequestShape_ShouldMatchOfficialContract()
    {
        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须声明于第三方子接口");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 三条数据面请求字段面同构（agentid / user_type / begin_time / end_time / data_filter_type /
        // device_sn_list / open_userid_list / limit / cursor），逐端点各成类型而非共用，
        // 因为 data_filter_type 的取值语义属各端点参数表。
        static string[] OfficialNames(Type request) => request
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name)
            .ToArray();

        var expected = new[] { "agentid", "user_type", "begin_time", "end_time", "data_filter_type", "device_sn_list", "open_userid_list", "limit", "cursor" };
        var dataRequests = new[]
        {
            typeof(DeviceDataCheckinDataRequest),
            typeof(DeviceDataTemperatureDataRequest),
            typeof(DeviceDataAccessControlDataRequest),
        };
        foreach (var request in dataRequests)
        {
            OfficialNames(request).Should().BeEquivalentTo(expected, $"{request.Name} 的官方参数名不得漂移");
        }

        // 授权设备信息请求只有 agentid（官方参数表标注为可选）。
        OfficialNames(typeof(DeviceDataAuthInfoRequest))
            .Should().BeEquivalentTo(new[] { "agentid" }, "get_auth_info 官方请求面只有 agentid");
    }

    /// <summary>
    /// 契约守卫 DD2：仅第三方开放域的接口层级——「智慧硬件服务端」文档树官方只在第三方根下有页面，
    /// 本域为「零端点父接口 + 唯一第三方子接口承载 8 端点」，继承链上不得出现自建 / 代开发子接口。
    /// </summary>
    [Fact]
    public void DeviceDataSingleAppTypeFamily_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var parent = typeof(IWechatWorkDeviceDataService);
        var child = typeof(IWechatWorkThirdPartyDeviceDataService);
        const string reason = "智能设备数据族官方仅第三方开放（自建与代开发文档树无对应页面）";

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
        assignable.Should().BeEquivalentTo(new[] { child }, $"继承链上不得出现第三方之外的子接口：{reason}");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("零端点父接口：端点全部声明于唯一的第三方子接口");

        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(DeviceDataRegistryGroupName,
            $"{child.Name} 必须挂 {DeviceDataRegistryGroupName} 注册组");
        childApi.InheritedFrom.Should().Be(parent.Name.Substring(1),
            "生成器契约：子接口必须以 InheritedFrom 指向父接口实现类名");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(8, "本域官方开放面恰为 8 端点（数据获取 4 + 门禁规则管理 4）");
    }

    /// <summary>
    /// 契约守卫 DD3：令牌绑定——本域两个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；第三方子接口必须声明企业级凭据归属域，父接口不声明归属域）。
    /// </summary>
    [Fact]
    public void DeviceDataTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var owningChild = typeof(IWechatWorkThirdPartyDeviceDataService);
        var abstractParent = typeof(IWechatWorkDeviceDataService);

        foreach (var iface in new[] { owningChild, abstractParent })
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }

        owningChild.GetCustomAttribute<TokenAttribute>()!.TokenManagerKey
            .Should().Be(WechatTokenManagerKeys.CorpAccessToken,
                $"{owningChild.Name} 官方仅第三方开放，必须声明授权企业级凭据归属域（错配由咽喉点 fail-fast）");

        // 反射陷阱：父接口未显式书写 TokenManagerKey 时读到的是特性构造参数默认值
        // （TokenTypes.AccessToken），故只能断言「不是任何真实归属域键」。
        var key = abstractParent.GetCustomAttribute<TokenAttribute>()!.TokenManagerKey;
        key.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
            $"{abstractParent.Name} 公共父接口不得声明凭据归属域（调用方须按应用类型选择子接口）");
        key.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
            $"{abstractParent.Name} 公共父接口不得声明凭据归属域（调用方须按应用类型选择子接口）");
    }

    /// <summary>
    /// 契约守卫 DD4：智能设备数据模块的请求 / 响应 DTO 必须全量登记进 AOT JSON 上下文
    /// （两族 27 个契约面类型落 DeviceDataJsonContext）。
    /// </summary>
    [Fact]
    public void DeviceDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = DeviceDataJsonContext.Default;

        var requiredTypes = new[]
        {
            // 数据获取族（96097 / 96027 / 96028 / 96029）。
            typeof(DeviceDataAuthInfoRequest), typeof(DeviceDataAuthInfoResponse),
            typeof(DeviceDataAuthDeviceList), typeof(DeviceDataAuthDevice),
            typeof(DeviceDataCheckinDataRequest), typeof(DeviceDataCheckinDataResponse),
            typeof(DeviceDataCheckinRecordList), typeof(DeviceDataCheckinRecord),
            typeof(DeviceDataTemperatureDataRequest), typeof(DeviceDataTemperatureDataResponse),
            typeof(DeviceDataTemperatureRecordList), typeof(DeviceDataTemperatureRecord),
            typeof(DeviceDataAccessControlDataRequest), typeof(DeviceDataAccessControlDataResponse),
            typeof(DeviceDataAccessControlRecordList), typeof(DeviceDataAccessControlRecord),
            // 门禁规则管理族（96030 / 96031 / 96221 / 96227）。
            typeof(DeviceDataAccessControlRuleGetRequest), typeof(DeviceDataAccessControlRuleGetResponse),
            typeof(DeviceDataAccessControlRuleList), typeof(DeviceDataAccessControlRule),
            typeof(DeviceDataPassRule), typeof(DeviceDataPassRuleEffectUser),
            typeof(DeviceDataAccessControlRuleAddRequest), typeof(DeviceDataAccessControlRuleAddResponse),
            typeof(DeviceDataAccessControlRuleModRequest), typeof(DeviceDataAccessControlRuleModResponse),
            typeof(DeviceDataAccessControlRuleDelRequest),
        };

        requiredTypes.Should().HaveCount(27, "本域契约面类型全集（删除 / 新增须同批调整本守卫）");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull($"{type.Name} 必须登记进 DeviceDataJsonContext（AOT 无反射序列化）");
        }
    }

    /// <summary>
    /// 契约守卫 DD5：官方字段名逐字锁定——本域集中了三处「同名不同形 / 同义不同名」的官方契约陷阱，
    /// 任何「顺手统一字段名」的改动都会破坏与官方报文的一致性。
    /// </summary>
    [Fact]
    public void DeviceDataFieldNames_ShouldMatchOfficialWireContract()
    {
        var assertions = new (Type Owner, string Property, string JsonName, string Because)[]
        {
            // 设备定位键：查询规则侧单值 device_sn，数据面与写侧 device_sn_list。
            (typeof(DeviceDataAccessControlRuleGetRequest), nameof(DeviceDataAccessControlRuleGetRequest.DeviceSerialNumber), "device_sn",
                "get_accesscontrol_rule 以单值 device_sn 定位，不是 device_sn_list"),
            (typeof(DeviceDataAccessControlRuleAddRequest), nameof(DeviceDataAccessControlRuleAddRequest.DeviceSerialNumberList), "device_sn_list",
                "add_accesscontrol_rule 绑定多台设备，官方为 device_sn_list"),

            // 规则名：请求侧 rule_name，查询响应侧 name（同一语义两个字段名）。
            (typeof(DeviceDataAccessControlRuleAddRequest), nameof(DeviceDataAccessControlRuleAddRequest.RuleName), "rule_name",
                "添加侧规则名官方为 rule_name"),
            (typeof(DeviceDataAccessControlRuleModRequest), nameof(DeviceDataAccessControlRuleModRequest.RuleName), "rule_name",
                "修改侧规则名官方为 rule_name"),
            (typeof(DeviceDataAccessControlRule), nameof(DeviceDataAccessControlRule.RuleName), "name",
                "查询响应侧规则名官方为 name（不是 rule_name）"),

            // pass_rule 单名双形：查询响应侧是容器（内层 items），写侧是单对象（内层 rule_list）。
            (typeof(DeviceDataAccessControlRuleGetResponse), nameof(DeviceDataAccessControlRuleGetResponse.PassRuleList), "pass_rule",
                "查询响应侧 pass_rule 为「容器 + items」形态"),
            (typeof(DeviceDataAccessControlRuleAddRequest), nameof(DeviceDataAccessControlRuleAddRequest.PassRule), "pass_rule",
                "添加请求侧 pass_rule 为单对象形态"),
            (typeof(DeviceDataAccessControlRuleList), nameof(DeviceDataAccessControlRuleList.ItemList), "items",
                "规则容器的内层元素名官方为 items"),
            (typeof(DeviceDataAuthDeviceList), nameof(DeviceDataAuthDeviceList.ItemList), "item",
                "授权设备容器的内层元素名官方为 item（单数），与数据面容器的 items 不同"),

            // 三条数据面响应容器名与条目时间字段名。
            (typeof(DeviceDataCheckinDataResponse), nameof(DeviceDataCheckinDataResponse.CheckinDataList), "checkindata",
                "打卡容器名官方连写 checkindata（无下划线）"),
            (typeof(DeviceDataTemperatureDataResponse), nameof(DeviceDataTemperatureDataResponse.TemperatureDataList), "temperature_data",
                "温度容器名官方为 temperature_data"),
            (typeof(DeviceDataAccessControlDataResponse), nameof(DeviceDataAccessControlDataResponse.AccessControlDataList), "accesscontrol_data",
                "门禁容器名官方为 accesscontrol_data"),
            (typeof(DeviceDataCheckinRecord), nameof(DeviceDataCheckinRecord.Timestamp), "checkin_time",
                "打卡条目时间字段官方为 checkin_time"),
            (typeof(DeviceDataTemperatureRecord), nameof(DeviceDataTemperatureRecord.Timestamp), "timestamp",
                "温度条目时间字段官方为 timestamp（与打卡侧不同名）"),
            (typeof(DeviceDataAccessControlRecord), nameof(DeviceDataAccessControlRecord.Timestamp), "timestamp",
                "门禁条目时间字段官方为 timestamp"),

            // 非法成员回传键名。
            (typeof(DeviceDataAccessControlRuleAddResponse), nameof(DeviceDataAccessControlRuleAddResponse.InvalidOpenUserIdList), "invalid_list",
                "add 侧非法成员官方键为 invalid_list"),
            (typeof(DeviceDataAccessControlRuleModResponse), nameof(DeviceDataAccessControlRuleModResponse.InvalidOpenUserIdList), "invalid_list",
                "mod 侧非法成员官方键为 invalid_list"),

            // 生效成员子结构。
            (typeof(DeviceDataPassRule), nameof(DeviceDataPassRule.EffectUserList), "effect_open_userid_list",
                "放行规则生效成员官方键为 effect_open_userid_list"),
            (typeof(DeviceDataPassRuleEffectUser), nameof(DeviceDataPassRuleEffectUser.OpenUserId), "open_userid",
                "本域成员标识全线为 open_userid（不是 userid）"),
        };

        foreach (var (owner, property, jsonName, because) in assertions)
        {
            var info = owner.GetProperty(property)!;
            info.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonName,
                $"{owner.Name}.{property} 官方字段名必须为 {jsonName}：{because}");
        }
    }

    /// <summary>
    /// 契约守卫 DD6：线格式锁定——时间戳一律 long、枚举与分页一律 int、温度值官方为字符串；
    /// 规则 ID 与设备序列号官方为字符串（不得「优化」成数字）。
    /// </summary>
    [Fact]
    public void DeviceDataNumericAndTextWireTypes_ShouldNotDrift()
    {
        var wireTypes = new (Type Owner, string Property, Type Expected, string Because)[]
        {
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.BeginTimestamp), typeof(long), "begin_time 官方为秒级时间戳"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.EndTimestamp), typeof(long), "end_time 官方为秒级时间戳"),
            (typeof(DeviceDataTemperatureDataRequest), nameof(DeviceDataTemperatureDataRequest.BeginTimestamp), typeof(long), "begin_time 官方为秒级时间戳"),
            (typeof(DeviceDataAccessControlDataRequest), nameof(DeviceDataAccessControlDataRequest.EndTimestamp), typeof(long), "end_time 官方为秒级时间戳"),
            (typeof(DeviceDataCheckinRecord), nameof(DeviceDataCheckinRecord.Timestamp), typeof(long), "checkin_time 官方为秒级时间戳"),
            (typeof(DeviceDataAccessControlRule), nameof(DeviceDataAccessControlRule.EffectTimestamp), typeof(long), "effect_time 官方为秒级时间戳"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.UserType), typeof(int), "user_type 官方为数字枚举"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.DataFilterType), typeof(int), "data_filter_type 官方为数字枚举"),
            (typeof(DeviceDataTemperatureRecord), nameof(DeviceDataTemperatureRecord.Status), typeof(int), "status 官方为数字枚举"),
            (typeof(DeviceDataAccessControlRecord), nameof(DeviceDataAccessControlRecord.PassType), typeof(int), "pass_type 官方为数字枚举"),
            (typeof(DeviceDataAccessControlRecord), nameof(DeviceDataAccessControlRecord.PassMethod), typeof(int), "pass_method 官方为数字枚举"),
            (typeof(DeviceDataAuthDevice), nameof(DeviceDataAuthDevice.DeviceType), typeof(int), "device_type 官方为数字枚举"),
            (typeof(DeviceDataPassRuleEffectUser), nameof(DeviceDataPassRuleEffectUser.UserType), typeof(int?), "user_type 在本子结构可缺省，须为 int?"),
            (typeof(DeviceDataTemperatureRecord), nameof(DeviceDataTemperatureRecord.TemperatureString), typeof(string), "temperature 官方以字符串承载而非数字"),
            (typeof(DeviceDataAccessControlRule), nameof(DeviceDataAccessControlRule.RuleId), typeof(string), "rule_id 官方为字符串而非数字"),
            (typeof(DeviceDataAccessControlRuleDelRequest), nameof(DeviceDataAccessControlRuleDelRequest.RuleId), typeof(string), "rule_id 官方为字符串而非数字"),
            (typeof(DeviceDataAuthDevice), nameof(DeviceDataAuthDevice.DeviceSerialNumber), typeof(string), "device_sn 官方为字符串"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.Cursor), typeof(string), "cursor 官方为字符串翻页标记（本仓声明为 string?）"),
            (typeof(DeviceDataCheckinDataResponse), nameof(DeviceDataCheckinDataResponse.NextCursor), typeof(string), "next_cursor 官方为字符串翻页标记（本仓声明为 string?）"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.Limit), typeof(int?), "limit 官方为数字分页参数"),
        };

        foreach (var (owner, property, expected, because) in wireTypes)
        {
            var info = owner.GetProperty(property)!;
            info.PropertyType.Should().Be(expected, $"{owner.Name}.{property} 线格式必须为 {expected}：{because}");
        }

        // 删除门禁规则端点官方成功响应无业务字段 ⇒ 直接返回 WechatWorkResponse，不建响应 DTO。
        typeof(IWechatWorkThirdPartyDeviceDataService)
            .GetMethod(nameof(IWechatWorkThirdPartyDeviceDataService.DeleteDeviceAccessControlRuleAsync))!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "del_accesscontrol_rule 官方响应仅 errcode / errmsg，不得凭空造响应 DTO");
    }

    /// <summary>
    /// 契约守卫 DD7：请求侧「必填」以类型系统表达（B8 官方正文复核结论，2026-10-13，追踪号 P2-QY-B8-DEVICEDATA-REQUIRED-13）。
    /// <para>
    /// 96027 / 96028 / 96029 三页参数表的「必须」列一致：<c>user_type</c> / <c>begin_time</c> / <c>end_time</c> /
    /// <c>data_filter_type</c> 为<b>必须=是</b>，<c>agentid</c> 为<b>必须=否</b>（三页均标「否」，但描述为
    /// 「第三方套件应用必填，第三方单应用可不填」⇒ 该条件必填由调用方按应用形态决定，SDK 侧不得替调用方收紧）；
    /// <c>device_sn_list</c> 与 <c>open_userid_list</c> 官方标「否」而以 <c>data_filter_type</c> 判别实际必填项，
    /// 故两者都保持可空；<c>cursor</c> / <c>limit</c> 为选填。
    /// 96029 的门禁规则管理四页则按端点各给必填：get 的 <c>device_sn</c>、add 的 <c>rule_name</c> + <c>device_sn_list</c>、
    /// mod 的 <c>rule_id</c> + <c>rule_name</c> + <c>device_sn_list</c>、del 的 <c>rule_id</c>。
    /// </para>
    /// </summary>
    [Fact]
    public void DeviceDataOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable()
    {
        var required = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.UserType), "user_type"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.BeginTimestamp), "begin_time"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.EndTimestamp), "end_time"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.DataFilterType), "data_filter_type"),
            (typeof(DeviceDataTemperatureDataRequest), nameof(DeviceDataTemperatureDataRequest.UserType), "user_type"),
            (typeof(DeviceDataTemperatureDataRequest), nameof(DeviceDataTemperatureDataRequest.DataFilterType), "data_filter_type"),
            (typeof(DeviceDataAccessControlDataRequest), nameof(DeviceDataAccessControlDataRequest.UserType), "user_type"),
            (typeof(DeviceDataAccessControlDataRequest), nameof(DeviceDataAccessControlDataRequest.DataFilterType), "data_filter_type"),
            (typeof(DeviceDataAccessControlRuleGetRequest), nameof(DeviceDataAccessControlRuleGetRequest.DeviceSerialNumber), "device_sn"),
            (typeof(DeviceDataAccessControlRuleAddRequest), nameof(DeviceDataAccessControlRuleAddRequest.RuleName), "rule_name"),
            (typeof(DeviceDataAccessControlRuleAddRequest), nameof(DeviceDataAccessControlRuleAddRequest.DeviceSerialNumberList), "device_sn_list"),
            (typeof(DeviceDataAccessControlRuleModRequest), nameof(DeviceDataAccessControlRuleModRequest.RuleId), "rule_id"),
            (typeof(DeviceDataAccessControlRuleModRequest), nameof(DeviceDataAccessControlRuleModRequest.RuleName), "rule_name"),
            (typeof(DeviceDataAccessControlRuleModRequest), nameof(DeviceDataAccessControlRuleModRequest.DeviceSerialNumberList), "device_sn_list"),
            (typeof(DeviceDataAccessControlRuleDelRequest), nameof(DeviceDataAccessControlRuleDelRequest.RuleId), "rule_id"),
            // 96031 参数表把 pass_rule 本身标「否」，但其两个子字段 rule_list 与 effect_open_userid_list 标「是」，
            // 且子结构内的 open_userid 亦标「是」⇒ 一旦传对象就必须带齐这两列
            (typeof(DeviceDataPassRule), nameof(DeviceDataPassRule.RuleList), "pass_rule.rule_list"),
            (typeof(DeviceDataPassRule), nameof(DeviceDataPassRule.EffectUserList), "pass_rule.effect_open_userid_list"),
            (typeof(DeviceDataPassRuleEffectUser), nameof(DeviceDataPassRuleEffectUser.OpenUserId), "open_userid"),
        };

        foreach (var (owner, property, jsonName) in required)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            IsNullableMember(prop!).Should().BeFalse(
                $"{jsonName} 官方参数表标为必填，不得声明为可空（可空即允许请求体省略必填参数）");
        }

        var optional = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.AgentId), "agentid"),
            (typeof(DeviceDataTemperatureDataRequest), nameof(DeviceDataTemperatureDataRequest.AgentId), "agentid"),
            (typeof(DeviceDataAccessControlDataRequest), nameof(DeviceDataAccessControlDataRequest.AgentId), "agentid"),
            (typeof(DeviceDataAuthInfoRequest), nameof(DeviceDataAuthInfoRequest.AgentId), "agentid"),
            // 二选一筛选：官方按 data_filter_type 判别实际必填项，参数表两列均标「否」⇒ 两侧都不得收紧
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.DeviceSerialNumberList), "device_sn_list"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.OpenUserIdList), "open_userid_list"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.Cursor), "cursor"),
            (typeof(DeviceDataCheckinDataRequest), nameof(DeviceDataCheckinDataRequest.Limit), "limit"),
            (typeof(DeviceDataAccessControlRuleAddRequest), nameof(DeviceDataAccessControlRuleAddRequest.PassRule), "pass_rule"),
        };

        foreach (var (owner, property, jsonName) in optional)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            IsNullableMember(prop!).Should().BeTrue(
                $"{jsonName} 官方参数表标为「必须=否」，须保持可空以便请求体省略（收敛为非可空会替调用方决定 agentid 形态或筛选方式）");
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
