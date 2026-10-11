// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Favorite;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「收藏管理」域契约守卫（CFA 系列：Favorite 域 1 端点路由 + 空请求体规则 +
/// 令牌绑定 + DTO 字段 + JSON 上下文登记；官方无枚举值 ⇒ 无 CFA6）。
/// </summary>
/// <remarks>
/// <para>
/// <b>数据时效措辞锁定（官方契约，勿当实时数）</b>：官方原文「此接口使用离线数据进行统计，
/// 结果可能存在延迟」——接口 XML 文档必须保留该警示，禁止弱化为「实时统计」。
/// </para>
/// <para>
/// <b>适用范围</b>：官方仅放行「微信小店」账号类型（权限集 id 129）——比多数小店端点（129/131/192）
/// 更窄，接入侧不得按其他域的权限集口径放行服务商代调用。
/// </para>
/// </remarks>
public class ChannelsFavoriteContractGuards
{
    private const string FavoriteRegistryGroupName = "Favorite";

    /// <summary>Favorite 域官方路由表（1 端点：POST + 空请求体）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] FavoriteRoutes =
    {
        (typeof(IChannelsFavoriteService), nameof(IChannelsFavoriteService.GetFavoritesCountAsync), typeof(PostAttribute), "/channels/ec/favorites/count/get"),
    };

    /// <summary>契约守卫 CFA1：Favorite 域 1 端点路由必须与官方契约一致。</summary>
    [Fact]
    public void FavoriteEndpoints_ShouldMatchOfficialRoutes()
    {
        FavoriteRoutes.Should().HaveCount(1, "收藏管理域仅店铺收藏人数统计 1 端点");
        FavoriteRoutes.Select(r => r.Route).Distinct().Should().HaveCount(1);

        FavoriteRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/favorites/", StringComparison.Ordinal))
            .Should().Be(1, "本域端点走 /channels/ec/favorites/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in FavoriteRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CFA2：官方契约 POST + 空请求体——不得人为造请求体 DTO。</summary>
    [Fact]
    public void FavoriteEndpoints_ShouldBePost_WithEmptyBody()
    {
        foreach (var (iface, method, _, _) in FavoriteRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        var endpoint = typeof(IChannelsFavoriteService).GetMethod(nameof(IChannelsFavoriteService.GetFavoritesCountAsync))!;
        endpoint.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("GetFavoritesCountAsync 官方参数表仅 access_token 一个 URL 参数（空请求体）——不得声明 [Body]");
    }

    /// <summary>契约守卫 CFA3：令牌绑定与注册形态。</summary>
    [Fact]
    public void FavoriteInterface_ShouldBindChannelsToken_AndRegisterUnderFavoriteGroup()
    {
        var token = typeof(IChannelsFavoriteService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Favorite 域端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsFavoriteService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(FavoriteRegistryGroupName, "必须挂 Favorite 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Favorite");
        typeof(ChannelsServiceBuilder).GetMethod("AddFavoriteApi").Should().NotBeNull("必须提供 AddFavoriteApi 链式注册方法");
    }

    /// <summary>契约守卫 CFA4：Favorite 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void FavoriteDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = FavoriteJsonContext.Default;

        var domainTypes = typeof(ChannelsFavoritesCountResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Favorite"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t)
                        && !t.IsAbstract)
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(1,
            "Favorite 域 1 端点（空请求体、响应扁平无嵌套）仅 1 个响应 DTO；" +
            "总数漂移须先核对官方文档再同批调整本守卫");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Favorite 域命名空间，必须登记进 FavoriteJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(FavoriteRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Favorite");
        }

        // 常量类断言计数且未标序列化特性：官方响应无枚举值 ⇒ 本域无常量类。
        var constantTypes = domainTypes.Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null).ToList();
        constantTypes.Should().BeEmpty("Favorite 域官方无枚举值——不得私造常量类");
    }

    /// <summary>契约守卫 CFA5：官方字段名锁定（5 个渠道收藏统计字段逐字照抄官方原文 favor_uv_acc_*）。</summary>
    [Fact]
    public void FavoriteDataModels_ShouldLockOfficialFields()
    {
        AssertJsonProperty<ChannelsFavoritesCountResponse>("favor_uv_acc_shop_homepage", "店铺首页收藏用户数");
        AssertJsonProperty<ChannelsFavoritesCountResponse>("favor_uv_acc_order_detail", "订单详情页收藏用户数");
        AssertJsonProperty<ChannelsFavoritesCountResponse>("favor_uv_acc_product_detail", "商品详情页收藏用户数");
        AssertJsonProperty<ChannelsFavoritesCountResponse>("favor_uv_acc_other_scene", "其他场景收藏用户数");
        AssertJsonProperty<ChannelsFavoritesCountResponse>("favor_uv_acc_all", "所有收藏用户数");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
