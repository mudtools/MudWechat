// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.Vip;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「会员服务」域契约守卫（MPV 系列：Vip 域 6 端点路由 + 令牌绑定 + 空请求体端点规则 +
/// DTO 登记计数与 JSON 上下文登记 + 字段抽样与共用 DTO 裁决）。
/// </summary>
/// <remarks>
/// <para>
/// <b>归属裁决（小店线设计方案 §4.1 结论 A）</b>：本域 6 端点（<c>/wxa/vip/*</c>）虽出现在小店文档
/// 目录，但官方原文明确「小程序 API 请使用小程序身份，即小程序的 access_token 调用」，
/// 适用范围「小程序」账号类型，支持第三方平台 <c>authorizer_access_token</c> 代调用
/// （权限集 id 185；2026-10-12 逐页核验）⇒ 归小程序线，复用 <c>MpTokenTypes.AccessToken</c>
/// （MP-X2 / MP-X3 同口径）。小店线侧由 CH-V1 锁定「Channels 程序集不得声明 <c>/wxa/vip/*</c>
/// 路由」，两守卫合起来即该路由族的<b>归属唯一性</b>闸门——任何一侧回潮即红。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、端点聚合计数（MP-X5 / ContractBaseline）
/// 与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；本文件只锁 Vip 域内的精确契约。
/// </para>
/// </remarks>
public class WxaVipContractGuards
{
    private const string VipRegistryGroupName = "Vip";

    private const string VipDtoNamespace = "Mud.Wechat.MiniProgram.DataModels.Vip";

    /// <summary>Vip 域官方路由表（6 端点：/wxa/vip/*，全部 POST；官方接口英文名 v3_wxa_*）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] VipRoutes =
    {
        (typeof(IWxaVipService), nameof(IWxaVipService.AddUserInfoAsync), typeof(PostAttribute), "/wxa/vip/user/info/add"),
        (typeof(IWxaVipService), nameof(IWxaVipService.UpdateUserInfoAsync), typeof(PostAttribute), "/wxa/vip/user/info/update"),
        (typeof(IWxaVipService), nameof(IWxaVipService.GetUserInfoAsync), typeof(PostAttribute), "/wxa/vip/user/info/get"),
        (typeof(IWxaVipService), nameof(IWxaVipService.GetUserListAsync), typeof(PostAttribute), "/wxa/vip/user/list/get"),
        (typeof(IWxaVipService), nameof(IWxaVipService.DeleteUserInfoAsync), typeof(PostAttribute), "/wxa/vip/user/info/delete"),
        (typeof(IWxaVipService), nameof(IWxaVipService.GetShopListAsync), typeof(PostAttribute), "/wxa/vip/shop/list/get"),
    };

    /// <summary>契约守卫 MPV1：Vip 域 6 端点路由必须与官方契约一致（全部 POST，前缀 /wxa/vip/）。</summary>
    [Fact]
    public void VipEndpoints_ShouldMatchOfficialRoutes()
    {
        // 6 端点 = 会员信息增 1 + 改 1 + 查 1 + 删 1 + 会员列表 1 + 已关联小店列表 1。
        VipRoutes.Should().HaveCount(6, "小程序会员服务（小程序相关）开放面为 6 端点（官方文档 2026-10-12 逐页核验）");
        VipRoutes.Select(static r => r.Route).Distinct().Should().HaveCount(6, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in VipRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文，禁止改写）");
            route.StartsWith("/wxa/vip/", StringComparison.Ordinal).Should().BeTrue(
                $"{route} 必须走 /wxa/vip/ 前缀（小店线 CH-V1 的镜像约束：该前缀属小程序线）");
        }
    }

    /// <summary>
    /// 契约守卫 MPV2：空请求体端点规则 —— <c>shop/list/get</c> 官方请求体为空，不得携带 [Body]；
    /// 其余 5 端点恰有一个 [Body] 请求体参数。
    /// </summary>
    [Fact]
    public void VipEndpoints_ShouldRespectEmptyBodyRules()
    {
        var shopList = typeof(IWxaVipService).GetMethod(nameof(IWxaVipService.GetShopListAsync))!;
        shopList.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("shop/list/get 官方 Request Payload 为空（请求示例 {}）——不得人为造请求体 DTO");
        shopList.GetCustomAttribute<PostAttribute>().Should().NotBeNull("空请求体端点官方契约为 POST");

        foreach (var (iface, method, _, _) in VipRoutes)
        {
            if (method == nameof(IWxaVipService.GetShopListAsync))
            {
                continue;
            }

            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            var bodyParameters = target.GetParameters()
                .Where(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                .ToList();

            bodyParameters.Should().HaveCount(1,
                $"{iface.Name}.{method} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 MPV3：令牌绑定与注册形态 —— 复用公众号 <c>MpTokenTypes.AccessToken</c>（MP-X2/X3 同口径）。</summary>
    /// <remarks>
    /// 官方原文「小程序 API 请使用小程序身份，即小程序的 access_token 调用」⇒
    /// 不得改绑小店 <c>ChannelsTokenTypes.AccessToken</c>，也不得新增令牌类型
    /// （新增会让 <c>MpTokenManagerRegistry</c> 单槽 Resolve 返回 null，errcode 自愈静默失效）。
    /// </remarks>
    [Fact]
    public void VipInterface_ShouldBindMpAccessToken_AndRegisterUnderVipGroup()
    {
        var token = typeof(IWxaVipService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Vip 域端点消费小程序 access_token（官方契约：请使用小程序身份调用）");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
            "Vip 域必须复用公众号/小程序同域令牌 Wechat.Mp.AccessToken（不得改绑小店令牌或新增类型）");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3）");
        token.Name.Should().Be("access_token");

        var api = typeof(IWxaVipService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(VipRegistryGroupName, "必须挂 Vip 注册组");
        api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");
    }

    /// <summary>契约守卫 MPV4：Vip 域 DTO 全量登记进 AOT JSON 上下文；命名空间清单锁死（无常量类）。</summary>
    [Fact]
    public void VipDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaVipAddUserRequest).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == VipDtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("Vip 域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 11 DTO = 请求 4（add + update + 共用 openid + 列表分页）+ 响应 3（单查 + 会员列表 + 小店列表）
        //          + 嵌套 4（请求侧会员信息 + 单查 info + 列表条目 + 小店条目）。
        dtoTypes.Should().HaveCount(11,
            "Vip 域 6 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（add 请求 1 + 请求侧会员信息 1 + update 请求 1 + 共用 openid 请求 1 + 单查响应 1 +" +
            " 单查 info 1 + 列表请求 1 + 列表响应 1 + 列表条目 1 + 小店列表响应 1 + 小店条目 1 = 11）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(VipRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Vip");
        }

        // 生成上下文按反射定位（而非直接类型引用）：上下文尚未生成时守卫给出可执行的修复指引，
        // 而不是让测试工程编译失败——登记本身是「同批完成」契约的一部分，缺失必须红。
        var contextType = assembly.GetType(VipDtoNamespace + ".VipJsonContext");
        contextType.Should().NotBeNull(
            "VipJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"Vip\")]，必须登记进 VipJsonContext" +
                "（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：除已登记 DTO 与生成上下文外不得出现任何新类型
        // （Vip 域 6 端点参数表无数值枚举 ⇒ 常量类为 0；后续新增须同批更新本守卫）。
        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType)
            .ToList();

        unregistered.Should().BeEmpty(
            "Vip 域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文的新类型：" +
            string.Join(", ", unregistered.Select(static t => t.Name)) +
            " —— 常量类或新 DTO 须同批更新本守卫");
    }

    /// <summary>契约守卫 MPV5：官方字段名抽样 + 类型口径 + 共用 DTO 裁决 + 历史字段禁建模。</summary>
    [Fact]
    public void VipDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // add 请求（session_id 有效期 10 分钟、由小店传递给小程序）。
        AssertJsonProperty<WxaVipAddUserRequest>(nameof(WxaVipAddUserRequest.SessionId), "session_id", "官方原文拼写");
        AssertJsonProperty<WxaVipAddUserRequest>(nameof(WxaVipAddUserRequest.WxaOpenId), "wxa_openid", "官方原文拼写");
        AssertJsonProperty<WxaVipAddUserRequest>(nameof(WxaVipAddUserRequest.Info), "info", "官方原文拼写");

        // 请求侧会员信息（add / update 参数表完全一致 ⇒ 共用）。
        AssertJsonProperty<WxaVipMemberInfo>(nameof(WxaVipMemberInfo.Identity), "identity", "官方原文拼写");
        AssertJsonProperty<WxaVipMemberInfo>(nameof(WxaVipMemberInfo.OuterUserId), "outer_userid", "官方参数表拼写（非示例中的 out_userid 笔误）");

        // update 请求与共用 openid 请求。
        AssertJsonProperty<WxaVipUpdateUserRequest>(nameof(WxaVipUpdateUserRequest.WxaOpenId), "wxa_openid", "官方原文拼写");
        AssertJsonProperty<WxaVipUpdateUserRequest>(nameof(WxaVipUpdateUserRequest.Info), "info", "官方原文拼写");
        AssertJsonProperty<WxaVipOpenidRequest>(nameof(WxaVipOpenidRequest.WxaOpenId), "wxa_openid", "官方原文拼写");

        // 单查响应。
        AssertJsonProperty<WxaVipUserInfoResponse>(nameof(WxaVipUserInfoResponse.Info), "info", "官方原文拼写");
        AssertJsonProperty<WxaVipUserInfo>(nameof(WxaVipUserInfo.Identity), "identity", "官方原文拼写");
        AssertJsonProperty<WxaVipUserInfo>(nameof(WxaVipUserInfo.OuterUserId), "outer_userid", "官方原文拼写");
        AssertJsonProperty<WxaVipUserInfo>(nameof(WxaVipUserInfo.RelatedShopAppIds), "related_shop_appid", "官方原文拼写");

        // 列表请求 / 列表响应（元素字段集与单查差异照录：多 wxa_openid）。
        AssertJsonProperty<WxaVipGetUserListRequest>(nameof(WxaVipGetUserListRequest.Offset), "offset", "官方原文拼写");
        AssertJsonProperty<WxaVipGetUserListRequest>(nameof(WxaVipGetUserListRequest.Limit), "limit", "官方原文拼写");
        AssertJsonProperty<WxaVipGetUserListResponse>(nameof(WxaVipGetUserListResponse.List), "list", "官方原文拼写");
        AssertJsonProperty<WxaVipUserListItem>(nameof(WxaVipUserListItem.Identity), "identity", "官方原文拼写");
        AssertJsonProperty<WxaVipUserListItem>(nameof(WxaVipUserListItem.OuterUserId), "outer_userid", "官方参数表拼写");
        AssertJsonProperty<WxaVipUserListItem>(nameof(WxaVipUserListItem.WxaOpenId), "wxa_openid", "官方原文拼写（仅列表元素携带）");
        AssertJsonProperty<WxaVipUserListItem>(nameof(WxaVipUserListItem.RelatedShopAppIds), "related_shop_appid", "官方原文拼写");

        // 小店列表响应。
        AssertJsonProperty<WxaVipGetShopListResponse>(nameof(WxaVipGetShopListResponse.List), "list", "官方原文拼写");
        AssertJsonProperty<WxaVipShopListItem>(nameof(WxaVipShopListItem.ShopAppId), "shop_appid", "官方原文拼写");

        // 类型口径锁定：官方参数表 offset / limit 为 number、related_shop_appid 为 array。
        typeof(WxaVipGetUserListRequest).GetProperty(nameof(WxaVipGetUserListRequest.Offset))!.PropertyType
            .Should().Be(typeof(long?), "官方参数表 offset / limit 类型为 number");
        typeof(WxaVipGetUserListRequest).GetProperty(nameof(WxaVipGetUserListRequest.Limit))!.PropertyType
            .Should().Be(typeof(long?), "官方参数表 offset / limit 类型为 number");
        typeof(WxaVipUserInfo).GetProperty(nameof(WxaVipUserInfo.RelatedShopAppIds))!.PropertyType
            .Should().Be(typeof(List<string>), "官方参数表 related_shop_appid 类型为 array（string 元素）");

        // 共用 DTO 裁决：user/info/get 与 user/info/delete 请求体字段集完全一致 ⇒ 共用 WxaVipOpenidRequest
        //（对齐小店线 Channels Vip 域「共用 openid 请求」先例）。
        typeof(IWxaVipService).GetMethod(nameof(IWxaVipService.GetUserInfoAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaVipOpenidRequest));
        typeof(IWxaVipService).GetMethod(nameof(IWxaVipService.DeleteUserInfoAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaVipOpenidRequest));

        // 共用 DTO 裁决：add 与 update 的 info 字段同为 WxaVipMemberInfo（参数表完全一致）。
        typeof(WxaVipAddUserRequest).GetProperty(nameof(WxaVipAddUserRequest.Info))!.PropertyType
            .Should().Be(typeof(WxaVipMemberInfo));
        typeof(WxaVipUpdateUserRequest).GetProperty(nameof(WxaVipUpdateUserRequest.Info))!.PropertyType
            .Should().Be(typeof(WxaVipMemberInfo));

        // 字段集边界锁定（防「顺手统一」）：单查响应 info 官方不含 wxa_openid（仅列表元素携带）。
        typeof(WxaVipUserInfo).GetProperty(nameof(WxaVipUserListItem.WxaOpenId), BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("官方 user/info/get 参数表 info 不含 wxa_openid——字段集照官方精确表达");

        // 历史字段禁建模（官方变更日志）：receive_register_coupon 已删除，不得回潮。
        typeof(WxaVipMemberInfo).GetProperty("ReceiveRegisterCoupon", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
            .Should().BeNull("官方变更日志已删除 receive_register_coupon，禁止建模历史字段");
    }

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }
}
