// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Vehicle;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「Vehicle」契约守卫 —— 本批 <b>10 端点</b>（注册组 <c>Vehicle</c>，接口 <c>IWechatPayVehicleService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.Vehicle</c>）。
/// </summary>
/// <remarks>
/// <para><b>一路由树两族、开放面不同</b>：<c>/v3/vehicle/etc/*</c>（ETC 六支）官方<b>只印服务商页</b>【普通服务商】 ⇒ 请求以 <c>sp_mchid</c> + <c>sub_mchid</c> 双层定位；<c>/v3/vehicle/parking/*</c> 与 <c>/v3/vehicle/transactions/parking</c> 四支各印<b>商户页 + 服务商页</b> ⇒ 字段取<b>并集</b>、 必填只取<b>交集</b>（仅某页印出的字段在 DTO 注释标「仅服务商页印发」）， 一支路由一方法，<b>不得</b>按身份拆两套方法或类型；ETC 六支<b>不得</b>反向补齐商户页。</para>
/// <para><b>共用只发生在应答侧且须含必填口径一致</b>：ETC 两支查单（<c>transactions/id/&#42;</c> 与 <c>transactions/out-trade-no/&#42;</c>）应答表逐字段同形 ⇒ 共用 <c>VehicleEtcTransactionResponse</c>； 签约两支（按 <c>contract_id</c> 与按 <c>openid</c> 查）应答同为 9 行 ⇒ 共用； 但<b>高速扣款下单的应答与查单不同形</b>（<c>device_info</c> 下单侧必填、<c>trade_type</c> 16 vs 32、 <c>success_time</c> 32 vs 25）⇒ 单建一型；停车「查单」与「下单」字段名集相同而 <c>amount.currency</c> 必填口径不同（查询页选填、下单页必填）⇒ <b>分建两型</b>，不得据「看起来一样」合并。</para>
/// <para><b>两支是「受理」不是「扣款成功」</b>：<c>POST /v3/vehicle/transactions/parking</c> 官方原文 「微信支付进行异步扣款，支付完成后，会将订单支付结果发送给商户」，<c>POST /v3/vehicle/etc/preopen</c> 只回 <c>preopen_id</c> ⇒ 结果由异步通知与查询支判定，<b>不得</b>据 HTTP 2xx 判定扣款或开通成功。</para>
/// <para><b>ETC 按用户查绑定的定位键在 query 而 path 段只是 <c>openid</c></b>：<c>sub_mchid</c> / <c>plan_id</c> / <c>plate_number</c> 三支官方必填 query（B2 的 <c>RequiredQueryOf</c> 逐支锁定）⇒ 只给 <c>openid</c> 不足以定位。</para>
/// <para><b>本域 10 支官方页全部未印端点级 QPS</b>，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数（逐支「未印」结论即取证结果）。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayVehicleContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Vehicle";

    private static readonly Type[] Services = { typeof(IWechatPayVehicleService) };

    /// <summary>VEH-B1：拓扑 —— 注册组 Vehicle、本批 10 方法（接口实测 10）、10 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void VehicleTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayVehicleService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("Vehicle", "组名即源生成注册入口名（改组名即改 AddVehicleWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(10, "Vehicle 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(10, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[]{ "GetAttribute", "PostAttribute" }, "动词集照官方页面逐支登记：本域写动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由为 APIv3 形态（官方文档树存在 v2 版式页，但请求 URL 仍印 /v3 前缀）");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayTransactionsService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>VEH-B2：本批 10 支端点的路由、path 键、query 名册、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void VehicleEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 查询ETC签约状态｜官方页 v3/partner/4012715667｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.QueryVehicleEtcContractAsync)).Should().Be("/v3/vehicle/etc/contracts/{contractId}");
        PathKeysOf(nameof(IWechatPayVehicleService.QueryVehicleEtcContractAsync)).Should().BeEquivalentTo(new[]{ "contractId" });
        QueryNamesOf(nameof(IWechatPayVehicleService.QueryVehicleEtcContractAsync)).Should().BeEquivalentTo(new[]{ "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayVehicleService.QueryVehicleEtcContractAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcContractAsync)).Should().BeNull("查询ETC签约状态官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcContractAsync)).Should().Be(typeof(VehicleEtcContractResponse));

        // 按用户与车牌查询ETC绑定信息｜官方页 v3/partner/4012715623｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.QueryVehicleEtcUserContractAsync)).Should().Be("/v3/vehicle/etc/users/{openid}/contracts");
        PathKeysOf(nameof(IWechatPayVehicleService.QueryVehicleEtcUserContractAsync)).Should().BeEquivalentTo(new[]{ "openid" });
        QueryNamesOf(nameof(IWechatPayVehicleService.QueryVehicleEtcUserContractAsync)).Should().BeEquivalentTo(new[]{ "sub_mchid", "plan_id", "plate_number" });
        RequiredQueryOf(nameof(IWechatPayVehicleService.QueryVehicleEtcUserContractAsync)).Should().BeEquivalentTo(new[]{ "plan_id", "plate_number" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcUserContractAsync)).Should().BeNull("按用户与车牌查询ETC绑定信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcUserContractAsync)).Should().Be(typeof(VehicleEtcContractResponse));

        // ETC自动扣费预开通｜官方页 v3/partner/4012715215｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.CreateVehicleEtcPreopenAsync)).Should().Be("/v3/vehicle/etc/preopen");
        PathKeysOf(nameof(IWechatPayVehicleService.CreateVehicleEtcPreopenAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayVehicleService.CreateVehicleEtcPreopenAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayVehicleService.CreateVehicleEtcPreopenAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayVehicleService.CreateVehicleEtcPreopenAsync)).Should().Be(typeof(VehicleEtcPreopenRequest));
        ReturnTypeOf(nameof(IWechatPayVehicleService.CreateVehicleEtcPreopenAsync)).Should().Be(typeof(VehicleEtcPreopenResponse));

        // ETC高速场景订单扣款下单｜官方页 v3/partner/4012715331｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.CreateVehicleEtcHighwayTransactionAsync)).Should().Be("/v3/vehicle/etc/transactions/highway");
        PathKeysOf(nameof(IWechatPayVehicleService.CreateVehicleEtcHighwayTransactionAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayVehicleService.CreateVehicleEtcHighwayTransactionAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayVehicleService.CreateVehicleEtcHighwayTransactionAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayVehicleService.CreateVehicleEtcHighwayTransactionAsync)).Should().Be(typeof(VehicleEtcHighwayTransactionRequest));
        ReturnTypeOf(nameof(IWechatPayVehicleService.CreateVehicleEtcHighwayTransactionAsync)).Should().Be(typeof(VehicleEtcHighwayTransactionResponse));

        // ETC订单查询（微信订单号）｜官方页 v3/partner/4012715719｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByIdAsync)).Should().Be("/v3/vehicle/etc/transactions/id/{transactionId}");
        PathKeysOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByIdAsync)).Should().BeEquivalentTo(new[]{ "transactionId" });
        QueryNamesOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByIdAsync)).Should().BeEquivalentTo(new[]{ "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByIdAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByIdAsync)).Should().BeNull("ETC订单查询（微信订单号）官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByIdAsync)).Should().Be(typeof(VehicleEtcTransactionResponse));

        // ETC订单查询（商户订单号）｜官方页 v3/partner/4012715845｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByOutTradeNoAsync)).Should().Be("/v3/vehicle/etc/transactions/out-trade-no/{outTradeNo}");
        PathKeysOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[]{ "outTradeNo" });
        QueryNamesOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[]{ "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByOutTradeNoAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByOutTradeNoAsync)).Should().BeNull("ETC订单查询（商户订单号）官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayVehicleService.QueryVehicleEtcTransactionByOutTradeNoAsync)).Should().Be(typeof(VehicleEtcTransactionResponse));

        // 创建停车入场信息｜官方页 v3/merchant/4012533937 + v3/partner/4012533994｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.CreateVehicleParkingEntryAsync)).Should().Be("/v3/vehicle/parking/parkings");
        PathKeysOf(nameof(IWechatPayVehicleService.CreateVehicleParkingEntryAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayVehicleService.CreateVehicleParkingEntryAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayVehicleService.CreateVehicleParkingEntryAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayVehicleService.CreateVehicleParkingEntryAsync)).Should().Be(typeof(VehicleParkingEntryRequest));
        ReturnTypeOf(nameof(IWechatPayVehicleService.CreateVehicleParkingEntryAsync)).Should().Be(typeof(VehicleParkingEntryResponse));

        // 查询车牌服务开通信息｜官方页 v3/merchant/4012534043 + v3/partner/4012534183｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.QueryVehicleParkingServiceAsync)).Should().Be("/v3/vehicle/parking/services/find");
        PathKeysOf(nameof(IWechatPayVehicleService.QueryVehicleParkingServiceAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayVehicleService.QueryVehicleParkingServiceAsync)).Should().BeEquivalentTo(new[]{ "appid", "plate_number", "openid", "plate_color", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayVehicleService.QueryVehicleParkingServiceAsync)).Should().BeEquivalentTo(new[]{ "appid", "plate_number", "openid", "plate_color", "sub_mchid" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayVehicleService.QueryVehicleParkingServiceAsync)).Should().BeNull("查询车牌服务开通信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayVehicleService.QueryVehicleParkingServiceAsync)).Should().Be(typeof(VehicleParkingServiceResponse));

        // 停车订单查询（商户订单号）｜官方页 v3/merchant/4012534308 + v3/partner/4012534441｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.QueryVehicleParkingTransactionByOutTradeNoAsync)).Should().Be("/v3/vehicle/transactions/out-trade-no/{outTradeNo}");
        PathKeysOf(nameof(IWechatPayVehicleService.QueryVehicleParkingTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[]{ "outTradeNo" });
        QueryNamesOf(nameof(IWechatPayVehicleService.QueryVehicleParkingTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[]{ "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayVehicleService.QueryVehicleParkingTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[]{ "sub_mchid" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayVehicleService.QueryVehicleParkingTransactionByOutTradeNoAsync)).Should().BeNull("停车订单查询（商户订单号）官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayVehicleService.QueryVehicleParkingTransactionByOutTradeNoAsync)).Should().Be(typeof(VehicleParkingTransactionQueryResponse));

        // 停车扣费受理下单｜官方页 v3/merchant/4012534352 + v3/partner/4012534427｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayVehicleService.CreateVehicleParkingTransactionAsync)).Should().Be("/v3/vehicle/transactions/parking");
        PathKeysOf(nameof(IWechatPayVehicleService.CreateVehicleParkingTransactionAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayVehicleService.CreateVehicleParkingTransactionAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayVehicleService.CreateVehicleParkingTransactionAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayVehicleService.CreateVehicleParkingTransactionAsync)).Should().Be(typeof(VehicleParkingTransactionRequest));
        ReturnTypeOf(nameof(IWechatPayVehicleService.CreateVehicleParkingTransactionAsync)).Should().Be(typeof(VehicleParkingTransactionResponse));
    }

    /// <summary>VEH-B3：官方字段表顶层名册（请求体与应答，逐字照页；嵌套子树另立类型）。</summary>
    [Fact]
    public void VehicleDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<VehicleEtcPreopenRequest>().Should().BeEquivalentTo(new[]{ "sp_appid", "sub_appid", "sub_mchid", "openid", "plan_id", "etc_device_id", "plate_number", "identify", "bank_card_no" }, "ETC自动扣费预开通请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleEtcHighwayTransactionRequest>().Should().BeEquivalentTo(new[]{ "out_trade_no", "sub_mchid", "appid", "sub_appid", "contract_id", "description", "attach", "goods_tag", "highway_info", "amount", "device_info" }, "ETC高速场景订单扣款下单请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleParkingEntryRequest>().Should().BeEquivalentTo(new[]{ "out_parking_no", "plate_number", "plate_color", "notify_url", "start_time", "parking_name", "free_duration", "sub_mchid" }, "创建停车入场信息请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleParkingTransactionRequest>().Should().BeEquivalentTo(new[]{ "appid", "description", "attach", "out_trade_no", "trade_scene", "goods_tag", "notify_url", "profit_sharing", "amount", "parking_info", "sub_appid", "sub_mchid" }, "停车扣费受理下单请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleEtcContractResponse>().Should().BeEquivalentTo(new[]{ "appid", "sp_mchid", "sp_openid", "sub_appid", "sub_mchid", "contract_id", "bind_state", "plate_number", "sub_openid" }, "查询ETC签约状态应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleEtcPreopenResponse>().Should().BeEquivalentTo(new[]{ "preopen_id" }, "ETC自动扣费预开通应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleEtcHighwayTransactionResponse>().Should().BeEquivalentTo(new[]{ "mchid", "sub_mchid", "appid", "sub_appid", "out_trade_no", "transaction_id", "attach", "trade_type", "bank_type", "success_time", "trade_state", "trade_state_desc", "payer", "amount", "device_info", "promotion_detail" }, "ETC高速场景订单扣款下单应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleEtcTransactionResponse>().Should().BeEquivalentTo(new[]{ "mchid", "sub_mchid", "appid", "sub_appid", "out_trade_no", "transaction_id", "attach", "trade_type", "bank_type", "success_time", "trade_state", "trade_state_desc", "payer", "amount", "device_info", "promotion_detail" }, "ETC订单查询（微信订单号）应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleParkingEntryResponse>().Should().BeEquivalentTo(new[]{ "id", "out_parking_no", "plate_number", "plate_color", "start_time", "parking_name", "free_duration", "state", "block_reason" }, "创建停车入场信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleParkingServiceResponse>().Should().BeEquivalentTo(new[]{ "plate_number", "plate_color", "service_open_time", "openid", "service_state" }, "查询车牌服务开通信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleParkingTransactionQueryResponse>().Should().BeEquivalentTo(new[]{ "appid", "sp_mchid", "description", "create_time", "out_trade_no", "transaction_id", "trade_state", "trade_state_description", "success_time", "bank_type", "user_repaid", "attach", "trade_scene", "parking_info", "payer", "amount", "promotion_detail", "sub_appid", "sub_mchid" }, "停车订单查询（商户订单号）应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<VehicleParkingTransactionResponse>().Should().BeEquivalentTo(new[]{ "appid", "sp_mchid", "description", "create_time", "out_trade_no", "transaction_id", "trade_state", "trade_state_description", "success_time", "bank_type", "user_repaid", "attach", "trade_scene", "parking_info", "payer", "amount", "promotion_detail", "sub_appid", "sub_mchid" }, "停车扣费受理下单应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");
    }

    /// <summary>VEH-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void VehicleDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleEtcContractResponse)).Should().BeTrue("VehicleEtcContractResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleEtcPreopenResponse)).Should().BeTrue("VehicleEtcPreopenResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleEtcHighwayTransactionResponse)).Should().BeTrue("VehicleEtcHighwayTransactionResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleEtcTransactionResponse)).Should().BeTrue("VehicleEtcTransactionResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleParkingEntryResponse)).Should().BeTrue("VehicleParkingEntryResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleParkingServiceResponse)).Should().BeTrue("VehicleParkingServiceResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleParkingTransactionQueryResponse)).Should().BeTrue("VehicleParkingTransactionQueryResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleParkingTransactionResponse)).Should().BeTrue("VehicleParkingTransactionResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleEtcPreopenRequest)).Should().BeFalse("VehicleEtcPreopenRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleEtcHighwayTransactionRequest)).Should().BeFalse("VehicleEtcHighwayTransactionRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleParkingEntryRequest)).Should().BeFalse("VehicleParkingEntryRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(VehicleParkingTransactionRequest)).Should().BeFalse("VehicleParkingTransactionRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>VEH-B5：本批 DTO 全量形态 —— 34 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void VehicleAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(34, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            VehicleJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 VehicleJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("Vehicle", type.Name + " 的 SerializerClassName 必须为 Vehicle");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    type.Name + " 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in batchDtos.SelectMany(JsonNamesOf))
            name.Should().NotContain(".", name + " 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        // 时间字段一律 string（rfc3339），不得用 DateTime / DateTimeOffset 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in batchDtos)
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
            }
    }

    /// <summary>VEH-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void VehicleRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("Vehicle", "PayModule 须有 Vehicle 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddVehicleWebApiHttpClient()", "注册委托必须指向源生成方法（组名 Vehicle 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("VehicleJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- helpers -------------------------------------------------------------

    private static object? ApiProp(Type service, string name)
    {
        var api = service.GetCustomAttributes(false).Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        return api.GetType().GetProperty(name)!.GetValue(api);
    }

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException(methodName + " 必须存在于本域接口");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull(methodName + " 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static Type? BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Select(p => (p: p, attr: p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "BodyAttribute")))
            .Where(x => x.attr != null)
            .Select(x => x.p.ParameterType)
            .FirstOrDefault();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Services.SelectMany(static s => s.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .ToArray();

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant() + " " + a.RequestUri))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(RepoRootDir, projectName + ".csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                               && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string RepoRootDir
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
