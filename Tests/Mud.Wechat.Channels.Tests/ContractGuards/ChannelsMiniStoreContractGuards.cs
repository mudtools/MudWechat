// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.MiniStore;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「连接小程序」域契约守卫（CMS 系列：MiniStore 域 10 端点三组前缀并存 + upload 双 Query 形态 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量 + 与 Order 域的边界措辞）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三组前缀并存（照抄官方原文）</b>：<c>/channels/ec/open/*</c>（连接小程序基础 2 端点）、
/// <c>/channels/ec/b2c/*</c>（礼物活动查询 2 端点）、<c>/channels/ec/order/*</c>（礼物单全链路 6 端点）。
/// </para>
/// <para>
/// <b>与 Order 域边界</b>：礼物单端点虽挂在 <c>/channels/ec/order/*</c> 下，但仅「合作商家为小程序发放礼物」
/// 族的 6 端点落本域；小店订单主链路（下单 / 查单 / 发货 / presentnote / presentsuborder 等）归
/// <c>IChannelsOrderService</c>，两域路由零交叠。
/// </para>
/// </remarks>
public class ChannelsMiniStoreContractGuards
{
    private const string MiniStoreRegistryGroupName = "MiniStore";

    /// <summary>MiniStore 域官方路由表（10 端点：/channels/ec/open/* 2 + /channels/ec/b2c/* 2 + /channels/ec/order/* 6，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MiniStoreRoutes =
    {
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.GetDownloadUrlAsync), typeof(PostAttribute), "/channels/ec/open/get_download_url"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.UploadMaterialAsync), typeof(PostAttribute), "/channels/ec/open/upload"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.GetPresentActivityListAsync), typeof(PostAttribute), "/channels/ec/b2c/activity/list/promoter/get"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.GetPresentActivityAsync), typeof(PostAttribute), "/channels/ec/b2c/activity/info/promoter/get"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.SetPresentReceiverAsync), typeof(PostAttribute), "/channels/ec/order/presentorder/receiver/set"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.CreatePresentOrderAsync), typeof(PostAttribute), "/channels/ec/order/presentorder/create"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.GetPresentListAsync), typeof(PostAttribute), "/channels/ec/order/presentlist/get"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.GetReceiverOrderListAsync), typeof(PostAttribute), "/channels/ec/order/receiverorderlist/get"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.GetPresentAsync), typeof(PostAttribute), "/channels/ec/order/present/get"),
        (typeof(IChannelsMiniStoreService), nameof(IChannelsMiniStoreService.SetPresentWishMessageAsync), typeof(PostAttribute), "/channels/ec/order/presentorder/wishmessage/set"),
    };

    /// <summary>契约守卫 CMS1：MiniStore 域 10 端点路由必须与官方契约一致（全部 POST，三组前缀并存）。</summary>
    [Fact]
    public void MiniStoreEndpoints_ShouldMatchOfficialRoutes()
    {
        // 10 端点 = 连接小程序基础 2 + 礼物活动查询 2 + 礼物单全链路 6（礼物单端点亦为 6：
        // 指定收礼者 / 创建发送 / 列表 / 收礼者订单列表 / 详情 / 祝福语）。
        MiniStoreRoutes.Should().HaveCount(10, "连接小程序域 = 基础 2 + 合作商家礼物族 8");
        MiniStoreRoutes.Select(r => r.Route).Distinct().Should().HaveCount(10);

        MiniStoreRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/open/", StringComparison.Ordinal))
            .Should().Be(2, "2 端点走 /channels/ec/open/* 连接小程序基础前缀");
        MiniStoreRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/b2c/", StringComparison.Ordinal))
            .Should().Be(2, "2 端点走 /channels/ec/b2c/* 礼物活动前缀");
        MiniStoreRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/order/", StringComparison.Ordinal))
            .Should().Be(6, "6 端点走 /channels/ec/order/* 礼物单前缀（与合作商家礼物族一一对应）");

        foreach (var (iface, method, httpAttribute, route) in MiniStoreRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CMS2：MiniStore 域 10 端点全部为 POST；upload 走 [Query] upload_type + [Body] JSON 形态；其余 [Body] 规则。</summary>
    [Fact]
    public void MiniStoreEndpoints_ShouldAllBePost_WithUploadQueryRules()
    {
        foreach (var (iface, method, _, _) in MiniStoreRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        // upload：官方 upload_type 恒为 Query 参数（两种上传形态皆然），SDK 按 JSON 形态（upload_type=1）建模。
        var upload = typeof(IChannelsMiniStoreService).GetMethod(nameof(IChannelsMiniStoreService.UploadMaterialAsync))!;
        var uploadTypeParam = upload.GetParameters().Should().Contain(
                static p => p.GetCustomAttribute<QueryAttribute>() != null
                            && p.GetCustomAttribute<QueryAttribute>()!.Name == "upload_type",
                "upload 官方契约将 upload_type 放在 Query String（URL 参数），非请求体")
            .Subject;
        uploadTypeParam.ParameterType.Should().Be(typeof(int));
        upload.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().ContainSingle("upload 的 URL 上传形态（upload_type=1）走 JSON 请求体 {openid, file_url}");

        // 官方无请求体参数的端点在 MiniStore 域不存在（10 端点全部有 JSON 请求体），
        // 反向锁定：10 端点均恰有一个 [Body] 参数（upload 含 Query + Body 双形态参数）。
        foreach (var (iface, method, _, _) in MiniStoreRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().ContainSingle($"{iface.Name}.{method} 官方请求体均为 JSON，须恰有一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 CMS3：令牌绑定与注册形态。</summary>
    [Fact]
    public void MiniStoreInterface_ShouldBindChannelsToken_AndRegisterUnderMiniStoreGroup()
    {
        var token = typeof(IChannelsMiniStoreService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("MiniStore 域端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsMiniStoreService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(MiniStoreRegistryGroupName, "必须挂 MiniStore 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("MiniStore");
        typeof(ChannelsServiceBuilder).GetMethod("AddMiniStoreApi").Should().NotBeNull("必须提供 AddMiniStoreApi 链式注册方法");
    }

    /// <summary>契约守卫 CMS4：MiniStore 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void MiniStoreDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = MiniStoreJsonContext.Default;

        var domainTypes = typeof(ChannelsMiniStoreGetDownloadUrlRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.MiniStore"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(34,
            "MiniStore 域 10 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（基础 4：下载链接请求/响应 + 上传请求/响应；活动 11：列表请求/条目/响应 + 详情请求/响应 +" +
            " activity/info/basic_info/shop_info/prize_info/product_info；礼物单 19：指定收礼者请求/子单 +" +
            " 创建请求/响应 + 时间范围（共用）+ 列表请求/单 id 引用/响应 + 收礼者列表请求/条目/响应 +" +
            " 详情请求/响应/present/detail/price_info/pay_info/子单详情 + 祝福语请求 = 34）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 MiniStore 域命名空间，必须登记进 MiniStoreJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(MiniStoreRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 MiniStore");
        }

        // 常量类（静态常量类不参与序列化）：MiniStore 域 4 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(4,
            "MiniStore 域常量类 = 上传方式 + 活动状态 + 活动类型 + 礼物单子单状态");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CMS5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void MiniStoreDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 连接小程序基础。
        AssertJsonProperty<ChannelsMiniStoreGetDownloadUrlRequest>("openid", "下单用户 openid");
        AssertJsonProperty<ChannelsMiniStoreGetDownloadUrlRequest>("order_id", "用户下单的订单号");
        AssertJsonProperty<ChannelsMiniStoreGetDownloadUrlResponse>("url", "文件下载链接（官方变更：download_url 已修正为 url）");
        AssertJsonProperty<ChannelsMiniStoreGetDownloadUrlResponse>("expire_time", "链接到期时间");
        AssertJsonProperty<ChannelsMiniStoreUploadMaterialRequest>("openid", "上传资料的用户 openid");
        AssertJsonProperty<ChannelsMiniStoreUploadMaterialRequest>("file_url", "上传文件的 url");
        AssertJsonProperty<ChannelsMiniStoreUploadMaterialResponse>("media_id", "文件 media_id");

        // 礼物活动。
        AssertJsonProperty<ChannelsMiniStoreGetActivityListRequest>("offset", "偏移量");
        AssertJsonProperty<ChannelsMiniStoreGetActivityListRequest>("limit", "单次请求个数");
        AssertJsonProperty<ChannelsMiniStoreGetActivityListRequest>("shop_appid", "小店 appid");
        AssertJsonProperty<ChannelsMiniStoreGetActivityListRequest>("status", "活动状态");
        AssertJsonProperty<ChannelsMiniStoreGetActivityListRequest>("type", "类型");
        AssertJsonProperty<ChannelsMiniStoreActivityItem>("shop_nickname", "小店昵称");
        AssertJsonProperty<ChannelsMiniStoreGetActivityListResponse>("total_num", "总数");
        AssertJsonProperty<ChannelsMiniStoreGetActivityResponse>("activity", "活动信息");
        AssertJsonProperty<ChannelsMiniStoreActivity>("activity_id", "活动 id");
        AssertJsonProperty<ChannelsMiniStoreActivityInfo>("basic_info", "活动基础信息");
        AssertJsonProperty<ChannelsMiniStoreActivityInfo>("prize_info", "活动礼物商品信息");
        AssertJsonProperty<ChannelsMiniStoreActivityBasicInfo>("activity_start_time", "活动开始时间");
        AssertJsonProperty<ChannelsMiniStoreShopInfo>("nickname", "小店昵称");
        AssertJsonProperty<ChannelsMiniStoreActivityPrizeInfo>("product_info_list", "商品信息列表");
        AssertJsonProperty<ChannelsMiniStoreActivityPrizeInfo>("present_order_id", "礼物订单号（自购类型才有）");
        AssertJsonProperty<ChannelsMiniStoreActivityProductInfo>("can_use_stock_num", "活动 sku 剩余库存（小店授权类型才有）");

        // 礼物单全链路。
        AssertJsonProperty<ChannelsMiniStoreSetPresentReceiverRequest>("present_order_id", "礼物单 ID");
        AssertJsonProperty<ChannelsMiniStoreSetPresentReceiverRequest>("sub_order_list", "礼物单子单列表");
        AssertJsonProperty<ChannelsMiniStorePresentSubOrder>("order_id", "礼物单子单订单 ID");
        AssertJsonProperty<ChannelsMiniStoreCreatePresentOrderRequest>("wishmessage", "礼物祝福语（官方字段名无下划线，照抄）");
        AssertJsonProperty<ChannelsMiniStoreCreatePresentOrderRequest>("idempotent_id", "幂等 id");
        AssertJsonProperty<ChannelsMiniStoreCreatePresentOrderResponse>("present_order_id", "礼物单 id");
        AssertJsonProperty<ChannelsMiniStoreCreatePresentOrderResponse>("business_type", "外部 app 跳转业务类型");
        AssertJsonProperty<ChannelsMiniStoreCreatePresentOrderResponse>("query", "外部 app 跳转参数");
        AssertJsonProperty<ChannelsMiniStoreTimeRange>("start_time", "开始时间");
        AssertJsonProperty<ChannelsMiniStoreTimeRange>("end_time", "结束时间");
        AssertJsonProperty<ChannelsMiniStoreGetPresentListRequest>("create_time_range", "礼物单创建时间范围");
        AssertJsonProperty<ChannelsMiniStoreGetPresentListRequest>("update_time_range", "礼物单更新时间范围");
        AssertJsonProperty<ChannelsMiniStoreGetPresentListRequest>("next_key", "分页参数");
        AssertJsonProperty<ChannelsMiniStorePresentOrderRef>("present_order_id", "礼物单 ID");
        AssertJsonProperty<ChannelsMiniStoreGetReceiverOrderListRequest>("openid", "收礼者 openid");
        AssertJsonProperty<ChannelsMiniStoreReceiverOrderItem>("order_id", "订单 ID");
        AssertJsonProperty<ChannelsMiniStoreGetPresentResponse>("present", "礼物单结构");
        AssertJsonProperty<ChannelsMiniStorePresent>("present_detail", "礼物单详情");
        AssertJsonProperty<ChannelsMiniStorePresent>("is_b2c_free_present", "是否无需付款（boolean，0/1 描述照录）");
        AssertJsonProperty<ChannelsMiniStorePresentDetail>("price_info", "价格信息");
        AssertJsonProperty<ChannelsMiniStorePresentDetail>("pay_info", "支付信息");
        AssertJsonProperty<ChannelsMiniStorePresentDetail>("wishmessage", "祝福语（官方字段名无下划线，照抄）");
        AssertJsonProperty<ChannelsMiniStorePresentPriceInfo>("order_price", "礼物单价格（分）");
        AssertJsonProperty<ChannelsMiniStorePresentPayInfo>("pay_time", "礼物单支付时间");
        AssertJsonProperty<ChannelsMiniStorePresentSubOrderDetail>("status", "子单订单状态");
        AssertJsonProperty<ChannelsMiniStoreSetPresentWishMessageRequest>("wish_message", "祝福语信息（官方字段名带下划线，照抄）");

        // 共用 DTO 裁决：presentlist/get 与 receiverorderlist/get 的 create_time_range / update_time_range
        // 字段集完全一致（start_time / end_time，跨度均 ≤ 7 天）⇒ 4 处共用 ChannelsMiniStoreTimeRange。
        typeof(ChannelsMiniStoreGetPresentListRequest).GetProperty(nameof(ChannelsMiniStoreGetPresentListRequest.CreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsMiniStoreTimeRange));
        typeof(ChannelsMiniStoreGetPresentListRequest).GetProperty(nameof(ChannelsMiniStoreGetPresentListRequest.UpdateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsMiniStoreTimeRange));
        typeof(ChannelsMiniStoreGetReceiverOrderListRequest).GetProperty(nameof(ChannelsMiniStoreGetReceiverOrderListRequest.CreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsMiniStoreTimeRange));
        typeof(ChannelsMiniStoreGetReceiverOrderListRequest).GetProperty(nameof(ChannelsMiniStoreGetReceiverOrderListRequest.UpdateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsMiniStoreTimeRange));

        // 分建裁决：指定收礼者的子单（order_id + openid）与礼物单详情的子单（多 status）字段集不一致 ⇒ 分建。
        typeof(ChannelsMiniStorePresent).GetProperty(nameof(ChannelsMiniStorePresent.SubOrderList))!
            .PropertyType.Should().Be(typeof(List<ChannelsMiniStorePresentSubOrderDetail>));
    }

    /// <summary>契约守卫 CMS6：枚举常量字面量锁定。</summary>
    [Fact]
    public void MiniStoreEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsMiniStoreUploadTypes.Binary.Should().Be(0);
        ChannelsMiniStoreUploadTypes.Url.Should().Be(1);

        ChannelsMiniStoreActivityStatuses.NotStarted.Should().Be(1);
        ChannelsMiniStoreActivityStatuses.InProgress.Should().Be(2);
        ChannelsMiniStoreActivityStatuses.Drawn.Should().Be(4);
        ChannelsMiniStoreActivityStatuses.Invalid.Should().Be(5);

        ChannelsMiniStoreActivityTypes.ShopAuthorized.Should().Be(0);
        ChannelsMiniStoreActivityTypes.SelfPurchase.Should().Be(1);
        ChannelsMiniStoreActivityTypes.All.Should().Be(2);

        ChannelsMiniStorePresentSubOrderStatuses.PendingAccept.Should().Be(12);
        ChannelsMiniStorePresentSubOrderStatuses.PendingDelivery.Should().Be(20);
        ChannelsMiniStorePresentSubOrderStatuses.PartialDelivery.Should().Be(21);
        ChannelsMiniStorePresentSubOrderStatuses.PendingReceipt.Should().Be(30);
        ChannelsMiniStorePresentSubOrderStatuses.Completed.Should().Be(100);
        ChannelsMiniStorePresentSubOrderStatuses.Expired.Should().Be(180);
        ChannelsMiniStorePresentSubOrderStatuses.CancelledAfterAftersale.Should().Be(200);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
