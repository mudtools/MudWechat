// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Vip;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「会员营销」域契约守卫（CV 系列：Vip 域 5 端点路由 + 令牌绑定 + DTO 字段与共用裁决 +
/// JSON 上下文登记 + 枚举常量 + 积分类型口径照录）。
/// </summary>
/// <remarks>
/// <para>
/// <b>积分类型口径（官方矛盾照录锁定）</b>：单点查询（score/get）的积分为 string，流水中的积分变动值为
/// number——两处类型不一致为官方原文，SDK 照录建模，守卫锁定该形态防止「顺手统一」。
/// </para>
/// <para>
/// <b>令牌归属</b>：5 端点令牌均为小店 access_token（含 v3/wxa/info/get——官方明确「小店 API 请使用
/// 小店身份」，不得改用小程序 access_token）。
/// </para>
/// </remarks>
public class ChannelsVipContractGuards
{
    private const string VipRegistryGroupName = "Vip";

    /// <summary>Vip 域官方路由表（5 端点：/channels/ec/vip/*，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] VipRoutes =
    {
        (typeof(IChannelsVipService), nameof(IChannelsVipService.GetUserScoreAsync), typeof(PostAttribute), "/channels/ec/vip/user/score/get"),
        (typeof(IChannelsVipService), nameof(IChannelsVipService.GetUserInfoAsync), typeof(PostAttribute), "/channels/ec/vip/user/info/get"),
        (typeof(IChannelsVipService), nameof(IChannelsVipService.GetUserListAsync), typeof(PostAttribute), "/channels/ec/vip/user/list/get"),
        (typeof(IChannelsVipService), nameof(IChannelsVipService.GetUserScoreFlowRecordAsync), typeof(PostAttribute), "/channels/ec/vip/user/score/flowrecord/get"),
        (typeof(IChannelsVipService), nameof(IChannelsVipService.GetWxaInfoAsync), typeof(PostAttribute), "/channels/ec/vip/v3/wxa/info/get"),
    };

    /// <summary>契约守卫 CV1：Vip 域 5 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void VipEndpoints_ShouldMatchOfficialRoutes()
    {
        // 5 端点 = 用户积分 1 + 用户信息 1 + 用户列表 1 + 积分流水 1 + 关联小程序信息 1（v3 前缀）。
        VipRoutes.Should().HaveCount(5, "会员营销域开放 user/* 4 端点 + v3/wxa/info/get 1 端点");
        VipRoutes.Select(r => r.Route).Distinct().Should().HaveCount(5);

        VipRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/vip/user/", StringComparison.Ordinal))
            .Should().Be(4, "4 端点走 /channels/ec/vip/user/* 会员用户前缀");
        VipRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/vip/v3/", StringComparison.Ordinal))
            .Should().Be(1, "1 端点（v3/wxa/info/get）走 /channels/ec/vip/v3/* 小程序会员服务前缀");

        foreach (var (iface, method, httpAttribute, route) in VipRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CV2：Vip 域 5 端点全部为 POST；空请求体端点（v3/wxa/info/get）不得携带 [Body] 参数。</summary>
    [Fact]
    public void VipEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in VipRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        // v3/wxa/info/get 官方 Request Payload 为空，不得人为造请求体 DTO。
        var target0 = typeof(IChannelsVipService).GetMethod(nameof(IChannelsVipService.GetWxaInfoAsync))!;
        target0.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("v3/wxa/info/get 官方无请求体参数——不得声明 [Body]");
    }

    /// <summary>契约守卫 CV3：令牌绑定与注册形态。</summary>
    [Fact]
    public void VipInterface_ShouldBindChannelsToken_AndRegisterUnderVipGroup()
    {
        var token = typeof(IChannelsVipService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Vip 域端点消费小店 access_token（官方契约——v3/wxa/info/get 亦为小店身份）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsVipService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(VipRegistryGroupName, "必须挂 Vip 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Vip");
        typeof(ChannelsServiceBuilder).GetMethod("AddVipApi").Should().NotBeNull("必须提供 AddVipApi 链式注册方法");
    }

    /// <summary>契约守卫 CV4：Vip 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void VipDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = VipJsonContext.Default;

        var domainTypes = typeof(ChannelsVipOpenidRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Vip"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(15,
            "Vip 域 5 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（共用 openid 请求 1 + 积分响应 1 + 积分信息 1 + 用户信息响应 1 + 用户信息 1 + 等级信息 1 +" +
            " 列表请求 1 + 列表响应 1 + 流水请求 1 + 流水响应 1 + 流水记录 1 + 小程序信息响应 1 + wxa info 1 +" +
            " wxa paths 1 + display_info 1 = 15）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Vip 域命名空间，必须登记进 VipJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(VipRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Vip");
        }

        // 常量类（静态常量类不参与序列化）：Vip 域 3 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(3,
            "Vip 域常量类 = 积分流水来源 + 关联小程序上线状态 + 下单页权益展示开关");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CV5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void VipDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 用户积分（score 官方类型为 string——照录，禁止「顺手统一」为 number）。
        AssertJsonProperty<ChannelsVipOpenidRequest>("openid", "用户 openid");
        AssertJsonProperty<ChannelsVipGetUserScoreResponse>("info", "用户积分信息");
        AssertJsonProperty<ChannelsVipUserScoreInfo>("score", "用户当前积分余额（官方类型 string）");

        // 用户信息 / 用户列表。
        AssertJsonProperty<ChannelsVipGetUserInfoResponse>("info", "用户信息");
        AssertJsonProperty<ChannelsVipUserInfo>("openid", "用户 openid");
        AssertJsonProperty<ChannelsVipUserInfo>("unionid", "用户 unionid");
        AssertJsonProperty<ChannelsVipUserInfo>("user_grade_info", "用户等级信息");
        AssertJsonProperty<ChannelsVipUserGradeInfo>("grade", "会员等级");
        AssertJsonProperty<ChannelsVipUserGradeInfo>("experience_value", "当前等级经验值");
        AssertJsonProperty<ChannelsVipGetUserListRequest>("page_num", "当前页");
        AssertJsonProperty<ChannelsVipGetUserListRequest>("page_size", "每页数量");
        AssertJsonProperty<ChannelsVipGetUserListResponse>("list", "用户信息列表");
        AssertJsonProperty<ChannelsVipGetUserListResponse>("total_num", "会员用户总数");

        // 积分流水。
        AssertJsonProperty<ChannelsVipGetUserScoreFlowRecordRequest>("openid", "用户 openid");
        AssertJsonProperty<ChannelsVipGetUserScoreFlowRecordResponse>("list", "积分流水列表");
        AssertJsonProperty<ChannelsVipScoreFlowRecord>("score", "积分变动值（number，与单点查询的 string 照录不一致）");
        AssertJsonProperty<ChannelsVipScoreFlowRecord>("source", "流水来源");
        AssertJsonProperty<ChannelsVipScoreFlowRecord>("remark", "流水备注说明");

        // 关联小程序信息。
        AssertJsonProperty<ChannelsVipGetWxaInfoResponse>("wxa_appid", "关联的小程序 appid");
        AssertJsonProperty<ChannelsVipGetWxaInfoResponse>("state", "上线状态");
        AssertJsonProperty<ChannelsVipWxaInfo>("paths", "小程序会员信息");
        AssertJsonProperty<ChannelsVipWxaInfo>("whitelist_openids", "小程序测试白名单用户 openid 列表");
        AssertJsonProperty<ChannelsVipWxaInfo>("display_info", "下单页权益");
        AssertJsonProperty<ChannelsVipWxaPaths>("register_path", "小程序会员注册页面路径");
        AssertJsonProperty<ChannelsVipWxaPaths>("rights_path", "小程序会员权益页面路径");
        AssertJsonProperty<ChannelsVipWxaDisplayInfo>("order_page_display", "下单页权益展示开关");

        // 共用 DTO 裁决：score/get 与 info/get 请求体字段集完全一致（单个 openid）⇒ 共用 ChannelsVipOpenidRequest。
        typeof(IChannelsVipService).GetMethod(nameof(IChannelsVipService.GetUserScoreAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsVipOpenidRequest));
        typeof(IChannelsVipService).GetMethod(nameof(IChannelsVipService.GetUserInfoAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsVipOpenidRequest));

        // 共用 DTO 裁决：info/get 的 info 与 list/get 的 list 元素字段集完全一致 ⇒ 共用 ChannelsVipUserInfo。
        typeof(ChannelsVipGetUserInfoResponse).GetProperty(nameof(ChannelsVipGetUserInfoResponse.Info))!
            .PropertyType.Should().Be(typeof(ChannelsVipUserInfo));
        typeof(ChannelsVipGetUserListResponse).GetProperty(nameof(ChannelsVipGetUserListResponse.List))!
            .PropertyType.Should().Be(typeof(List<ChannelsVipUserInfo>));

        // 类型口径锁定：单点积分为 string、流水积分变动值为 number（官方原文照录）。
        typeof(ChannelsVipUserScoreInfo).GetProperty(nameof(ChannelsVipUserScoreInfo.Score))!
            .PropertyType.Should().Be(typeof(string), "官方 score/get 参数表 score 类型为 string");
        typeof(ChannelsVipScoreFlowRecord).GetProperty(nameof(ChannelsVipScoreFlowRecord.Score))!
            .PropertyType.Should().Be(typeof(int?), "官方 flowrecord/get 参数表 score 类型为 number");
    }

    /// <summary>契约守卫 CV6：枚举常量字面量锁定。</summary>
    [Fact]
    public void VipEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsVipScoreFlowSources.OrderEarn.Should().Be(2);
        ChannelsVipScoreFlowSources.OrderDeduct.Should().Be(5);

        ChannelsVipWxaStates.Online.Should().Be(1);
        ChannelsVipWxaStates.Offline.Should().Be(2);
        ChannelsVipWxaStates.PreOnlineTesting.Should().Be(3);

        ChannelsVipOrderPageDisplaySwitches.On.Should().Be(1);
        ChannelsVipOrderPageDisplaySwitches.Off.Should().Be(2);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
