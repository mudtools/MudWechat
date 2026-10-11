// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Shop;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「店铺管理」域契约守卫（CS 系列：Shop 域 4 端点路由 + 1 GET / 3 POST 动词分布 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>企业微信参数传播面（官方契约，措辞锁定）</b>：H5 链接 / 二维码 / 口令三端点均支持传入
/// <c>wecom_corp_id</c>/<c>wecom_user_id</c>（经「获取关联账号企微id」获取），带参下单后订单
/// 成交来源展示该企业。
/// </para>
/// <para>
/// <b>路由前缀</b>：本域 4 端点全部走 <c>/channels/ec/basics/*</c> 主干前缀。
/// </para>
/// </remarks>
public class ChannelsShopContractGuards
{
    private const string ShopRegistryGroupName = "Shop";

    /// <summary>Shop 域官方路由表（4 端点：info/get 1 GET + h5url/qrcode/taglink 3 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ShopRoutes =
    {
        (typeof(IChannelsShopService), nameof(IChannelsShopService.GetShopInfoAsync), typeof(GetAttribute), "/channels/ec/basics/info/get"),
        (typeof(IChannelsShopService), nameof(IChannelsShopService.GetShopH5UrlAsync), typeof(PostAttribute), "/channels/ec/basics/shop/h5url/get"),
        (typeof(IChannelsShopService), nameof(IChannelsShopService.GetShopQrcodeAsync), typeof(PostAttribute), "/channels/ec/basics/shop/qrcode/get"),
        (typeof(IChannelsShopService), nameof(IChannelsShopService.GetShopTagLinkAsync), typeof(PostAttribute), "/channels/ec/basics/shop/taglink/get"),
    };

    /// <summary>契约守卫 CS1：Shop 域 4 端点路由必须与官方契约一致。</summary>
    [Fact]
    public void ShopEndpoints_ShouldMatchOfficialRoutes()
    {
        ShopRoutes.Should().HaveCount(4, "店铺信息 1 + H5 链接 1 + 二维码 1 + 口令 1 = 4 端点");
        ShopRoutes.Select(r => r.Route).Distinct().Should().HaveCount(4);

        ShopRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/basics/", StringComparison.Ordinal))
            .Should().Be(4, "本域 4 端点全部走 /channels/ec/basics/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in ShopRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CS2：动词如实（1 GET + 3 POST）；空请求体端点不得带 [Body]。</summary>
    [Fact]
    public void ShopEndpoints_ShouldHonorOfficialVerbAndBodyRules()
    {
        // 动词分布：官方页如实——info/get 为 GET，其余 3 端点 POST。
        ShopRoutes.Count(static r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(1);
        ShopRoutes.Count(static r => r.HttpAttribute == typeof(PostAttribute)).Should().Be(3);

        // info/get 官方参数表仅 access_token 一个 URL 参数（空请求体）——不得人为造请求体 DTO。
        var getInfo = typeof(IChannelsShopService).GetMethod(nameof(IChannelsShopService.GetShopInfoAsync))!;
        getInfo.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("GetShopInfoAsync 官方规定空请求体——不得声明 [Body]");

        // 其余 3 端点官方为 POST JSON 请求体——必须带 [Body]。
        foreach (var bodyEndpoints in new[]
                 {
                     nameof(IChannelsShopService.GetShopH5UrlAsync),
                     nameof(IChannelsShopService.GetShopQrcodeAsync),
                     nameof(IChannelsShopService.GetShopTagLinkAsync),
                 })
        {
            var target = typeof(IChannelsShopService).GetMethod(bodyEndpoints)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().NotBeEmpty($"{bodyEndpoints} 官方契约为 POST JSON 请求体——必须声明 [Body]");
        }
    }

    /// <summary>契约守卫 CS3：令牌绑定与注册形态。</summary>
    [Fact]
    public void ShopInterface_ShouldBindChannelsToken_AndRegisterUnderShopGroup()
    {
        var token = typeof(IChannelsShopService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Shop 域 4 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsShopService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(ShopRegistryGroupName, "必须挂 Shop 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Shop");
        typeof(ChannelsServiceBuilder).GetMethod("AddShopApi").Should().NotBeNull("必须提供 AddShopApi 链式注册方法");
    }

    /// <summary>契约守卫 CS4：Shop 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void ShopDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = ShopJsonContext.Default;

        var domainTypes = typeof(ChannelsShopBasicInfoResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Shop"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(7,
            "Shop 域 4 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（店铺信息 2 + 共用企微参数请求 1 + 二维码请求 1 + H5 链接响应 1 + 二维码响应 1 + 口令响应 1 = 7）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Shop 域命名空间，必须登记进 ShopJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(ShopRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Shop");
        }

        // 常量类断言计数且未标序列化特性（标了会进上下文并使上方计数失真）。
        var constantTypes = domainTypes.Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null).ToList();
        constantTypes.Should().HaveCount(3,
            "Shop 域常量类 = 店铺状态 + 店铺类型 + 二维码类型（枚举常量类不标 [HttpJsonSerializable]）");
    }

    /// <summary>契约守卫 CS5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + h5url/taglink 共用裁决）。</summary>
    [Fact]
    public void ShopDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 店铺基本信息。
        AssertJsonProperty<ChannelsShopBasicInfoResponse>("info", "店铺信息");
        AssertJsonProperty<ChannelsShopBasicInfo>("nickname", "店铺名称");
        AssertJsonProperty<ChannelsShopBasicInfo>("headimg_url", "店铺头像 URL");
        AssertJsonProperty<ChannelsShopBasicInfo>("subject_type", "店铺类型");
        AssertJsonProperty<ChannelsShopBasicInfo>("status", "店铺状态");
        AssertJsonProperty<ChannelsShopBasicInfo>("username", "店铺原始 ID");
        AssertJsonProperty<ChannelsShopBasicInfo>("is_local_life", "是否本地生活小店");
        AssertJsonProperty<ChannelsShopBasicInfo>("open_timestamp", "开店时间戳");

        // 企业微信参数（H5 链接 / 口令共用请求）。
        AssertJsonProperty<ChannelsShopWecomLinkRequest>("wecom_corp_id", "企业微信 id");
        AssertJsonProperty<ChannelsShopWecomLinkRequest>("wecom_user_id", "企业微信成员 id");

        // 店铺二维码（独立请求：多必填 qrcode_type）。
        AssertJsonProperty<ChannelsShopQrcodeRequest>("wecom_corp_id", "企业微信 id");
        AssertJsonProperty<ChannelsShopQrcodeRequest>("qrcode_type", "二维码类型（必填）");

        // 传播物料响应。
        AssertJsonProperty<ChannelsShopH5UrlResponse>("shop_h5url", "店铺 H5 链接");
        AssertJsonProperty<ChannelsShopQrcodeResponse>("shop_qrcode", "店铺二维码链接");
        AssertJsonProperty<ChannelsShopTagLinkResponse>("shop_taglink", "店铺微信口令");

        // 共用 DTO 裁决：h5url/get 与 taglink/get 官方请求体字段集一致（wecom_corp_id + wecom_user_id，均选填）
        // ⇒ 共用 ChannelsShopWecomLinkRequest，语义演化方向一致（传播物料 + 企微归因）。
        typeof(IChannelsShopService).GetMethod(nameof(IChannelsShopService.GetShopH5UrlAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsShopWecomLinkRequest));
        typeof(IChannelsShopService).GetMethod(nameof(IChannelsShopService.GetShopTagLinkAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsShopWecomLinkRequest));

        // 共用裁决（负向）：qrcode/get 官方请求体多必填 qrcode_type ⇒ 独立 ChannelsShopQrcodeRequest，不得并入共用 DTO。
        typeof(IChannelsShopService).GetMethod(nameof(IChannelsShopService.GetShopQrcodeAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsShopQrcodeRequest));
        typeof(ChannelsShopQrcodeRequest).GetProperty(nameof(ChannelsShopQrcodeRequest.QrcodeType))!
            .PropertyType.Should().Be(typeof(int), "qrcode_type 官方必填——非可空");
    }

    /// <summary>契约守卫 CS6：枚举常量字面量锁定（官方 status / subject_type / qrcode_type）。</summary>
    [Fact]
    public void ShopEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsShopStatuses.Opening.Should().Be("opening");
        ChannelsShopStatuses.OpenFinished.Should().Be("open_finished");
        ChannelsShopStatuses.Closing.Should().Be("closing");
        ChannelsShopStatuses.CloseFinished.Should().Be("close_finished");

        ChannelsShopSubjectTypes.Enterprise.Should().Be("企业");
        ChannelsShopSubjectTypes.IndividualBusiness.Should().Be("个体工商户");

        ChannelsShopQrcodeTypes.Qrcode.Should().Be(1);
        ChannelsShopQrcodeTypes.StandardMaterial.Should().Be(2);
        ChannelsShopQrcodeTypes.GiftMaterial.Should().Be(3);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
