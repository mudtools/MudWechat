// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Warehouse;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「区域仓库」域契约守卫（设计方案 v1 P2 / CW 系列：Warehouse 域 11 端点路由 +
/// 双前缀并存（warehouse/* 10 + basics/addresscode/get 1）+ 全 POST + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>双前缀并存（照抄官方）</b>：官方将「获取地址行政编码」归入区域仓库 API 族但路由落在
/// <c>/channels/ec/basics/addresscode/get</c>（basics 历史前缀），禁止「对齐规范」改写为
/// <c>/warehouse/*</c>。
/// </para>
/// <para>
/// <b>下单匹配口径（守卫措辞锁定）</b>：官方「更新区域仓库存数量」注意事项——下单匹配原则
/// <b>按照地址从右往左进行匹配</b>（先筛库存、再逐级匹配、通配符 0 匹配任意地址、精准优先于通配，
/// 最后取优先级最高的区域仓）；未设置优先级时默认按仓库创建时间（越早创建优先级越高）。
/// </para>
/// </remarks>
public class ChannelsWarehouseContractGuards
{
    private const string WarehouseRegistryGroupName = "Warehouse";

    /// <summary>Warehouse 域官方路由表（11 端点：/channels/ec/warehouse/* 10 + /channels/ec/basics/* 1，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] WarehouseRoutes =
    {
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.CreateWarehouseAsync), typeof(PostAttribute), "/channels/ec/warehouse/create"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.GetWarehouseListAsync), typeof(PostAttribute), "/channels/ec/warehouse/list/get"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.GetWarehouseAsync), typeof(PostAttribute), "/channels/ec/warehouse/get"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.UpdateWarehouseDetailAsync), typeof(PostAttribute), "/channels/ec/warehouse/detail/update"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.AddCoverLocationsAsync), typeof(PostAttribute), "/channels/ec/warehouse/coverlocations/add"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.DeleteCoverLocationsAsync), typeof(PostAttribute), "/channels/ec/warehouse/coverlocations/del"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.GetAddressPrioritySortAsync), typeof(PostAttribute), "/channels/ec/warehouse/address/prioritysort/get"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.SetAddressPrioritySortAsync), typeof(PostAttribute), "/channels/ec/warehouse/address/prioritysort/set"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.UpdateWarehouseStockAsync), typeof(PostAttribute), "/channels/ec/warehouse/stock/update"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.GetWarehouseStockAsync), typeof(PostAttribute), "/channels/ec/warehouse/stock/get"),
        (typeof(IChannelsWarehouseService), nameof(IChannelsWarehouseService.GetAddressCodeAsync), typeof(PostAttribute), "/channels/ec/basics/addresscode/get"),
    };

    /// <summary>契约守卫 CW1：Warehouse 域 11 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void WarehouseEndpoints_ShouldMatchOfficialRoutes()
    {
        // 11 端点 = 仓库生命周期 4（创建/列表/详情/修改）+ 覆盖区域 2 + 地址优先级 2 + 库存 2 + 地址行政编码 1。
        WarehouseRoutes.Should().HaveCount(11, "仓库生命周期 4 + 覆盖区域 2 + 地址优先级 2 + 库存 2 + 地址行政编码 1 = 11 端点");
        WarehouseRoutes.Select(r => r.Route).Distinct().Should().HaveCount(11);

        WarehouseRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/warehouse/", StringComparison.Ordinal))
            .Should().Be(10, "10 端点走 /channels/ec/warehouse/* 主干前缀");
        WarehouseRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/basics/", StringComparison.Ordinal))
            .Should().Be(1, "获取地址行政编码官方归入仓库 API 族但路由为 /channels/ec/basics/addresscode/get（历史前缀，照抄原文禁止改写）");

        foreach (var (iface, method, httpAttribute, route) in WarehouseRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CW2：Warehouse 域 11 端点全部为 POST；全部端点均须携带 [Body]（官方参数表均有请求体，addresscode 的 addr_code 为选填入参）。</summary>
    [Fact]
    public void WarehouseEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in WarehouseRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（区域仓库无 GET 端点）");

            var bodyParams = target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>()).ToList();
            bodyParams.Should().HaveCount(1,
                $"{method} 官方为 POST 且参数表含请求体（addresscode 的 addr_code 亦为请求体选填入参），必须声明且仅声明一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 CW3：令牌绑定与注册形态。</summary>
    [Fact]
    public void WarehouseInterface_ShouldBindChannelsToken_AndRegisterUnderWarehouseGroup()
    {
        var token = typeof(IChannelsWarehouseService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Warehouse 域 11 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsWarehouseService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(WarehouseRegistryGroupName, "必须挂 Warehouse 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Warehouse");
        typeof(ChannelsServiceBuilder).GetMethod("AddWarehouseApi").Should().NotBeNull("必须提供 AddWarehouseApi 链式注册方法");
    }

    /// <summary>契约守卫 CW4：Warehouse 域 DTO 全量登记进 AOT JSON 上下文（含常量类形态检查）。</summary>
    [Fact]
    public void WarehouseDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = WarehouseJsonContext.Default;

        // domainTypes 不过滤 abstract（静态常量类编译为 abstract + sealed，此处必须保留），
        // abstract 的排除只在 dtoTypes 子查询进行——否则常量类断言恒 0（前序批次踩坑）。
        var domainTypes = typeof(ChannelsCreateWarehouseRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Warehouse"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        var constTypes = domainTypes
            .Where(t => t.IsAbstract && t.IsSealed)
            .ToList();

        dtoTypes.Should().HaveCount(21,
            "Warehouse 域 11 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（请求 10 + 响应 5 + data 嵌套 4 + 覆盖区域元素 1 + 地址元素 1 = 21）");

        constTypes.Should().HaveCount(2, "Warehouse 域常量类 = 库存修改类型 + 地址级别 = 2");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Warehouse 域命名空间，必须登记进 WarehouseJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(WarehouseRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Warehouse");
        }

        foreach (var type in constTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>().Should().BeNull(
                $"{type.Name} 为常量类（不参与 JSON 序列化），不得标 [HttpJsonSerializable]");
        }
    }

    /// <summary>契约守卫 CW5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void WarehouseDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 创建区域仓库。
        AssertJsonProperty<ChannelsCreateWarehouseRequest>("out_warehouse_id", "外部仓库 ID");
        AssertJsonProperty<ChannelsCreateWarehouseRequest>("name", "仓库名称");
        AssertJsonProperty<ChannelsCreateWarehouseRequest>("intro", "仓库介绍");
        AssertJsonProperty<ChannelsCreateWarehouseRequest>("cover_locations", "覆盖区域");

        // 查询区域仓库列表。
        AssertJsonProperty<ChannelsGetWarehouseListRequest>("page_size", "每页数量");
        AssertJsonProperty<ChannelsGetWarehouseListRequest>("next_key", "翻页上下文");
        AssertJsonProperty<ChannelsGetWarehouseListResponse>("data", "列表数据");
        AssertJsonProperty<ChannelsWarehouseListData>("out_warehouse_ids", "外部仓库 ID 列表");
        AssertJsonProperty<ChannelsWarehouseListData>("next_key", "翻页上下文（空即最后一页）");

        // 获取区域仓库。
        AssertJsonProperty<ChannelsGetWarehouseRequest>("out_warehouse_id", "外部仓库 ID");
        AssertJsonProperty<ChannelsWarehouseInfo>("out_warehouse_id", "外部仓库 ID");
        AssertJsonProperty<ChannelsWarehouseInfo>("intro", "仓库介绍");
        AssertJsonProperty<ChannelsWarehouseInfo>("cover_locations", "覆盖区域");

        // 修改区域仓库详情。
        AssertJsonProperty<ChannelsUpdateWarehouseDetailRequest>("out_warehouse_id", "外部仓库 ID");
        AssertJsonProperty<ChannelsUpdateWarehouseDetailRequest>("name", "仓库名称");

        // 批量增加 / 删除覆盖区域（共用请求体）。
        AssertJsonProperty<ChannelsWarehouseCoverLocationsRequest>("out_warehouse_id", "外部仓库 ID");
        AssertJsonProperty<ChannelsWarehouseCoverLocationsRequest>("cover_locations", "覆盖区域");

        // 地址优先级。
        AssertJsonProperty<ChannelsGetAddressPrioritySortRequest>("address_id1", "省份地址编码");
        AssertJsonProperty<ChannelsSetAddressPrioritySortRequest>("address_id1", "省份地址编码");
        AssertJsonProperty<ChannelsSetAddressPrioritySortRequest>("priority_sort", "优先级列表（高到低）");
        AssertJsonProperty<ChannelsWarehousePrioritySortData>("priority_sort", "优先级列表（高到低）");

        // 库存（官方 product_id/sku_id 为 string，与联盟域 number 型 product_id 分域照录）。
        AssertJsonProperty<ChannelsUpdateWarehouseStockRequest>("product_id", "商品 ID");
        AssertJsonProperty<ChannelsUpdateWarehouseStockRequest>("sku_id", "skuID");
        AssertJsonProperty<ChannelsUpdateWarehouseStockRequest>("diff_type", "修改类型");
        AssertJsonProperty<ChannelsUpdateWarehouseStockRequest>("num", "库存值");
        AssertJsonProperty<ChannelsGetWarehouseStockRequest>("out_warehouse_id", "外部仓库 ID");
        AssertJsonProperty<ChannelsWarehouseStockData>("num", "库存数量");
        typeof(ChannelsUpdateWarehouseStockRequest).GetProperty(nameof(ChannelsUpdateWarehouseStockRequest.ProductId))!
            .PropertyType.Should().Be(typeof(string),
                "官方 stock/update 参数表 product_id 类型为 string（示例 \"10000000666721\"），照抄官方");

        // 地址行政编码（addrs_msg / next_level_addrs 位于响应顶层，无 data 包裹）。
        AssertJsonProperty<ChannelsGetAddressCodeRequest>("addr_code", "地址行政编码（0 取全国省级）");
        AssertJsonProperty<ChannelsGetAddressCodeResponse>("addrs_msg", "本行政编码地址信息");
        AssertJsonProperty<ChannelsGetAddressCodeResponse>("next_level_addrs", "下一级所有地址信息");
        AssertJsonProperty<ChannelsAddress>("name", "地址名称");
        AssertJsonProperty<ChannelsAddress>("code", "地址行政编码");
        AssertJsonProperty<ChannelsAddress>("level", "地址级别");

        // 覆盖区域元素（address_id1~4 照抄官方）。
        AssertJsonProperty<ChannelsWarehouseLocation>("address_id1", "省份地址编码");
        AssertJsonProperty<ChannelsWarehouseLocation>("address_id2", "市地址编码");
        AssertJsonProperty<ChannelsWarehouseLocation>("address_id3", "区地址编码");
        AssertJsonProperty<ChannelsWarehouseLocation>("address_id4", "街道地址编码");

        // 共用 DTO 裁决 1：coverlocations/add 与 coverlocations/del 请求体字段集一致 ⇒ 共用 ChannelsWarehouseCoverLocationsRequest。
        typeof(IChannelsWarehouseService).GetMethod(nameof(IChannelsWarehouseService.AddCoverLocationsAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsWarehouseCoverLocationsRequest));
        typeof(IChannelsWarehouseService).GetMethod(nameof(IChannelsWarehouseService.DeleteCoverLocationsAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsWarehouseCoverLocationsRequest));

        // 共用 DTO 裁决 2：create / get / add / del 四端点覆盖区域元素同构 ⇒ 共用 ChannelsWarehouseLocation。
        typeof(ChannelsCreateWarehouseRequest).GetProperty(nameof(ChannelsCreateWarehouseRequest.CoverLocations))!
            .PropertyType.Should().Be(typeof(List<ChannelsWarehouseLocation>));
        typeof(ChannelsWarehouseInfo).GetProperty(nameof(ChannelsWarehouseInfo.CoverLocations))!
            .PropertyType.Should().Be(typeof(List<ChannelsWarehouseLocation>));
        typeof(ChannelsWarehouseCoverLocationsRequest).GetProperty(nameof(ChannelsWarehouseCoverLocationsRequest.CoverLocations))!
            .PropertyType.Should().Be(typeof(List<ChannelsWarehouseLocation>));

        // 共用 DTO 裁决 3：addrs_msg 与 next_level_addrs 元素同构（name/code/level）⇒ 共用 ChannelsAddress。
        typeof(ChannelsGetAddressCodeResponse).GetProperty(nameof(ChannelsGetAddressCodeResponse.AddrsMsg))!
            .PropertyType.Should().Be(typeof(ChannelsAddress));
        typeof(ChannelsGetAddressCodeResponse).GetProperty(nameof(ChannelsGetAddressCodeResponse.NextLevelAddrs))!
            .PropertyType.Should().Be(typeof(List<ChannelsAddress>));

        // 分建裁决 1：优先级 get 请求（无 priority_sort）与 set 请求（有 priority_sort）字段集不同 ⇒ 分建。
        typeof(ChannelsGetAddressPrioritySortRequest).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "priority_sort")
            .Should().BeFalse("prioritysort/get 请求无 priority_sort 入参，与 set 请求分建");
        typeof(ChannelsSetAddressPrioritySortRequest).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "priority_sort")
            .Should().BeTrue("prioritysort/set 请求含 priority_sort 入参");

        // 分建裁决 2：stock/update 请求（多 diff_type/num）与 stock/get 请求字段集不同 ⇒ 分建。
        typeof(ChannelsGetWarehouseStockRequest).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "diff_type")
            .Should().BeFalse("stock/get 请求无 diff_type 入参，与 stock/update 请求分建");
    }

    /// <summary>契约守卫 CW6：枚举常量字面量锁定（库存修改类型 + 地址级别）。</summary>
    [Fact]
    public void WarehouseEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsWarehouseStockDiffTypes.Increase.Should().Be(1, "官方原文「1: 增加」");
        ChannelsWarehouseStockDiffTypes.Decrease.Should().Be(2, "官方原文「2:减少」");
        ChannelsWarehouseStockDiffTypes.Set.Should().Be(3, "官方原文「3:设置」");

        ChannelsAddressLevels.Province.Should().Be(1, "官方原文「1-省级」");
        ChannelsAddressLevels.City.Should().Be(2, "官方原文「2-市级」");
        ChannelsAddressLevels.County.Should().Be(3, "官方原文「3-区县级」");
        ChannelsAddressLevels.Street.Should().Be(4, "官方原文「4-街道」");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
