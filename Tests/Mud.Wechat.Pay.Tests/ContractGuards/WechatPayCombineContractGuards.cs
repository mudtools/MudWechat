// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.DataModels.Combine;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P2 合单支付域契约守卫（<b>4 端点</b>：JSAPI 下单 + Native 下单 + 关单 + 查询）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（2026-10-09 逐页核验，均为<b>合作伙伴</b>文档中心 —— 本域按<b>服务商面</b>建模）</b>：
/// JSAPI/小程序合单下单 <c>…/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml</c>
/// （<b>POST</b> <c>/v3/combine-transactions/jsapi</c>，更新 2025.01.16）、
/// Native 合单下单 <c>…/doc/v3/partner/4012758240</c>
/// （<b>POST</b> <c>/v3/combine-transactions/native</c>，更新 2025.01.16）、
/// 合单关闭订单 <c>…/doc/v3/partner/4012709095</c>
/// （<b>POST</b> <c>/v3/combine-transactions/out-trade-no/{combine_out_trade_no}/close</c>，更新 2024.10.24）、
/// 合单查询订单 <c>…/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml</c>。
/// </para>
/// <para>
/// <b>⚠️「合单支付仅服务商可用」是<b>错的</b>（本轮修正）</b>：官方文档中合单支付<b>同时存在两个面</b> ——
/// 服务商面各页标注<b>【普通服务商】</b>且支持 <b>1–50 笔</b>；普通商户面亦有对应页
/// （Native 合单下单 <c>…/wiki/doc/apiv3/apis/chapter5_1_5.shtml</c>、
/// App 合单下单 <c>…/wiki/doc/apiv3/open/pay/chapter2_9_3.shtml</c>），
/// 均标注<b>【普通商户】</b>且原文「普通商户模式<b>只支持 2–10 笔</b>订单进行合单支付」，
/// 其子单字段表<b>无</b> <c>sub_mchid</c> / <c>sub_appid</c>。
/// <b>路由两面共用</b>（同为 <c>/v3/combine-transactions/…</c>），故本域 DTO 以服务商面为准；
/// 普通商户接入时勿填 <c>sub_mchid</c> / <c>sub_appid</c>。
/// </para>
/// <para>
/// <b>核验中值得留档的官方原文</b>：① 服务商模式支持 <b>1–50 笔</b>订单合单（普通商户 <b>2–10 笔</b>）；
/// ② <c>sub_appid</c> <b>仅允许一笔</b>商品单填写；
/// ③ <c>combine_payer_info</c> 的 <c>openid</c>/<c>sub_openid</c> <b>二选一必填</b>，
/// 且传 <c>sub_openid</c> 则 <c>sub_appid</c> 必填（<b>Native 下单无此字段</b>）； 
/// ④ 关单<b>不支持关闭部分子单</b>，且主单/子单信息<b>必须与下单时完全一致</b>；
/// ⑤ 关单<b>无应答包体</b>（204）；
/// ⑥ Native 应答<b>只有 <c>code_url</c></b>（有效期 2 小时），<b>没有</b> <c>prepay_id</c>。
/// </para>
/// </remarks>
public class WechatPayCombineContractGuards
{
    private const string RegistryGroupName = "CombineTransactions";
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Combine";

    /// <summary>CB1：2 端点路由与方法照官方原文；关单为无应答体（204）。</summary>
    [Fact]
    public void CombineEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayCombineService.CreateJsapiOrderAsync), "/v3/combine-transactions/jsapi");
        AssertRoute<PostAttribute>(
            nameof(IWechatPayCombineService.CloseOrderAsync),
            "/v3/combine-transactions/out-trade-no/{combineOutTradeNo}/close");

        // 关单：官方 204 无应答体 ⇒ 必须无返回值。
        FindMethod(nameof(IWechatPayCombineService.CloseOrderAsync))
            .ReturnType.Should().Be(typeof(Task), "官方 204 No Content ⇒ 无应答体，返回类型必须是 Task");
        FindMethod(nameof(IWechatPayCombineService.CreateJsapiOrderAsync))
            .ReturnType.Should().NotBe(typeof(Task), "下单有应答体（prepay_id）");

        // 关单：combine_out_trade_no 走 path（占位符名 = C# 参数名，生成器 HTTPCLIENT013 强制）。
        FindMethod(nameof(IWechatPayCombineService.CloseOrderAsync))
            .GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "combineOutTradeNo" });

        // 查询：只有一种查询方式（按合单商户订单号），且**无任何 query 参数**。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayCombineService.QueryOrderAsync),
            "/v3/combine-transactions/out-trade-no/{combineOutTradeNo}");
        FindMethod(nameof(IWechatPayCombineService.QueryOrderAsync))
            .GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方合单查询页无 query 参数（仅按合单商户订单号查）");

        // Native 下单：独立路由，且应答体是 code_url 而非 prepay_id。
        AssertRoute<PostAttribute>(
            nameof(IWechatPayCombineService.CreateNativeOrderAsync), "/v3/combine-transactions/native");
        FindMethod(nameof(IWechatPayCombineService.CreateNativeOrderAsync))
            .ReturnType.Should().Be(typeof(Task<CombineNativePrepayResponse>),
                "Native 应答只有 code_url（与服务商模式下单的 prepay_id 是两种应答）");
    }

    /// <summary>CB2：官方字段名锁定（含 <c>combine_*</c> 前缀与 <c>sub_orders</c> 子字段）。</summary>
    [Fact]
    public void CombineDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<CombinePrepayRequest>(nameof(CombinePrepayRequest.CombineAppId), "combine_appid");
        JsonNameShouldBe<CombinePrepayRequest>(nameof(CombinePrepayRequest.CombineMchId), "combine_mchid");
        JsonNameShouldBe<CombinePrepayRequest>(nameof(CombinePrepayRequest.CombineOutTradeNo), "combine_out_trade_no");
        JsonNameShouldBe<CombinePrepayRequest>(nameof(CombinePrepayRequest.CombinePayerInfo), "combine_payer_info");
        JsonNameShouldBe<CombinePrepayRequest>(nameof(CombinePrepayRequest.SubOrders), "sub_orders");

        JsonNameShouldBe<CombinePrepaySubOrder>(nameof(CombinePrepaySubOrder.OutTradeNo), "out_trade_no");
        JsonNameShouldBe<CombinePrepaySubOrder>(nameof(CombinePrepaySubOrder.SubMchId), "sub_mchid");
        JsonNameShouldBe<CombinePrepaySubOrder>(nameof(CombinePrepaySubOrder.SettleInfo), "settle_info");
        JsonNameShouldBe<CombineSubOrderAmount>(nameof(CombineSubOrderAmount.TotalAmount), "total_amount");
        JsonNameShouldBe<CombineSettleInfo>(nameof(CombineSettleInfo.ProfitSharing), "profit_sharing");
        JsonNameShouldBe<CombinePayerInfo>(nameof(CombinePayerInfo.SubOpenId), "sub_openid");
        JsonNameShouldBe<CombineSceneInfo>(nameof(CombineSceneInfo.PayerClientIp), "payer_client_ip");
        JsonNameShouldBe<CombinePrepayResponse>(nameof(CombinePrepayResponse.PrepayId), "prepay_id");

        JsonNameShouldBe<CombineCloseSubOrder>(nameof(CombineCloseSubOrder.MchId), "mchid");
        JsonNameShouldBe<CombineCloseSubOrder>(nameof(CombineCloseSubOrder.SubAppId), "sub_appid");

        JsonNameShouldBe<CombineQueryResponse>(nameof(CombineQueryResponse.CombineOutTradeNo), "combine_out_trade_no");
        JsonNameShouldBe<CombineQuerySubOrder>(nameof(CombineQuerySubOrder.TradeState), "trade_state");
        JsonNameShouldBe<CombineQuerySubOrder>(nameof(CombineQuerySubOrder.SubOpenId), "sub_openid");
        JsonNameShouldBe<CombineQuerySubOrderAmount>(nameof(CombineQuerySubOrderAmount.PayerAmount), "payer_amount");
        JsonNameShouldBe<CombineQuerySubOrderAmount>(nameof(CombineQuerySubOrderAmount.SettlementRate), "settlement_rate");

        JsonNameShouldBe<CombineNativePrepayRequest>(nameof(CombineNativePrepayRequest.CombineOutTradeNo), "combine_out_trade_no");
        JsonNameShouldBe<CombineNativePrepayRequest>(nameof(CombineNativePrepayRequest.CombineMchId), "combine_mchid");
        JsonNameShouldBe<CombineNativePrepayRequest>(nameof(CombineNativePrepayRequest.SubOrders), "sub_orders");
        JsonNameShouldBe<CombineNativePrepayResponse>(nameof(CombineNativePrepayResponse.CodeUrl), "code_url");
    }

    /// <summary>
    /// CB6：<b>「合单退款」刻意不建模</b> —— 官方明示<b>不存在</b>按合单总单号退款的接口。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方原文</b>（《订单退款》开发指引
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013080623"/>，更新 2026.06.10）：
    /// 「对于合单支付的订单，<b>无法通过合单支付总单号 <c>combine_out_trade_no</c> 退款，
    /// 只能根据单个子单进行退款</b>」；退款时 <c>transaction_id</c> 填
    /// <c>sub_orders.transaction_id</c>、<c>out_trade_no</c> 填 <c>sub_orders.out_trade_no</c>。
    /// </para>
    /// <para>
    /// <b>为何本守卫不是「漏实现」而是「显式裁决」</b>：设计方案把「合单退款」与其余合单端点并列，
    /// 极易被当作一个待补接口；而社区二手资料里流传的 <c>/v3/combine-transactions/refunds</c>
    /// <b>在官方不存在</b>（本轮抓取官方「合单支付 → API列表 → 退款申请」链接，
    /// 实际指向的是<b>普通单笔退款</b> <c>POST /v3/refund/domestic/refunds</c>）。
    /// 故按本仓纪律固化：合单域<b>不得</b>出现任何退款路由，退款一律逐子单走退款域。
    /// </para>
    /// </remarks>
    [Fact]
    public void CombineRefundEndpoint_ShouldStayUnmodeled()
    {
        var routes = typeof(IWechatPayCombineService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri)
            .ToList();

        routes.Where(static r => r is not null && r.Contains("refund", StringComparison.OrdinalIgnoreCase))
            .Should().BeEmpty(
                "官方明示合单订单无法按总单号退款、只能按子单退款 ⇒ 合单域不得有任何退款路由；" +
                "若将来官方新增独立合单退款接口，须先核验字段契约再同批解除本裁决");

        routes.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");

        // 退款的**正确机制**：逐子单调用退款域的普通退款申请（填子单 transaction_id / out_trade_no）。
        typeof(IWechatPayRefundService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri)
            .Should().Contain("/v3/refund/domestic/refunds",
                "合单退款的正解是逐子单走退款域接口；若该路由被改名，本裁决的落点须一并更新");
    }

    /// <summary>
    /// CB5：<b>三个子单形态互不相同</b>，且查询的 <c>combine_payer_info</c> / <c>scene_info</c> 也不得与下单共用。
    /// </summary>
    /// <remarks>
    /// 本域已出现 <b>三</b> 套商品单字段表（下单 / 关单 / 查询）与两套支付者与场景字段表 —— 都按官方表分建。
    /// 合并任一方向都会给出「某接口永不返回的字段」，调用方据此写出的分支永不命中。
    /// </remarks>
    [Fact]
    public void QueryShapes_ShouldStayStructurallySeparate()
    {
        // 查询子单有交易结果与实付金额；下单/关单子单没有。
        typeof(CombineQuerySubOrder).GetProperty("TradeState").Should().NotBeNull();
        typeof(CombineQuerySubOrderAmount).GetProperty("PayerAmount").Should().NotBeNull();
        typeof(CombineSubOrderAmount).GetProperty("PayerAmount").Should().BeNull(
            "下单页的金额表只有标价金额与币种");
        typeof(CombineCloseSubOrder).GetProperty("TradeState").Should().BeNull(
            "关单页的子单只有身份字段");

        // 下单的支付者信息有 sub_openid；查询的没有。
        typeof(CombinePayerInfo).GetProperty("SubOpenId").Should().NotBeNull();
        typeof(CombineQueryPayerInfo).GetProperty("SubOpenId").Should().BeNull(
            "官方查询页的 combine_payer_info 只有 openid");

        // 下单的场景信息有 payer_client_ip；查询的没有。
        typeof(CombineSceneInfo).GetProperty("PayerClientIp").Should().NotBeNull();
        typeof(CombineQuerySceneInfo).GetProperty("PayerClientIp").Should().BeNull(
            "官方查询页的 scene_info 只有 device_id");

        // Native 与服务商 JSAPI：子单与场景字段表**逐项一致** ⇒ 复用同类型（两页对照核验）。
        typeof(CombineNativePrepayRequest).GetProperty(nameof(CombineNativePrepayRequest.SubOrders))!
            .PropertyType.Should().Be(typeof(List<CombinePrepaySubOrder>),
                "官方两页的 sub_orders 字段表逐项一致（含 sub_mchid 必填 / sub_appid 选填）⇒ 共用类型");
        typeof(CombineNativePrepayRequest).GetProperty(nameof(CombineNativePrepayRequest.SceneInfo))!
            .PropertyType.Should().Be(typeof(CombineSceneInfo),
                "官方两页的 scene_info 字段表一致 ⇒ 共用类型");

        // 但顶层**不共用**：Native 无 combine_payer_info（官方该页顶层字段表没有它）。
        typeof(CombineNativePrepayRequest).GetProperty("CombinePayerInfo").Should().BeNull(
            "Native 下单不需要支付者标识 ⇒ 复用 JSAPI 请求 DTO 会让调用方传官方未定义的字段");

        // 应答也**不共用**：Native 只有 code_url，没有 prepay_id。
        typeof(CombineNativePrepayResponse).GetProperty(nameof(CombineNativePrepayResponse.CodeUrl)).Should().NotBeNull();
        typeof(CombineNativePrepayResponse).GetProperty("PrepayId").Should().BeNull(
            "prepay_id 是 JSAPI 调起支付的参数，Native 靠 code_url 生成二维码");
    }

    /// <summary>
    /// CB3：<b>下单与关单的子单类型必须分开</b>（官方两页子单字段表不同）。
    /// </summary>
    /// <remarks>
    /// 关单页的子单<b>无</b> <c>attach</c> / <c>amount</c> / <c>description</c> / <c>settle_info</c> /
    /// <c>goods_tag</c>，且 <c>sub_mchid</c> 由下单页的<b>必填</b>变为<b>选填</b>；
    /// 合并成一个类型会让两处各自带上对方才有的字段（调用方据此序列化会多传或漏传）。
    /// </remarks>
    [Fact]
    public void SubOrderShapes_ShouldStayStructurallySeparate()
    {
        typeof(CombinePrepaySubOrder).GetProperty("Amount").Should().NotBeNull();
        typeof(CombinePrepaySubOrder).GetProperty("Attach").Should().NotBeNull();
        typeof(CombinePrepaySubOrder).GetProperty("GoodsTag").Should().NotBeNull();

        typeof(CombineCloseSubOrder).GetProperty("Amount").Should().BeNull(
            "官方关单页的子单字段表没有 amount");
        typeof(CombineCloseSubOrder).GetProperty("Attach").Should().BeNull(
            "官方关单页的子单字段表没有 attach");
        typeof(CombineCloseSubOrder).GetProperty("GoodsTag").Should().BeNull(
            "官方关单页的子单字段表没有 goods_tag");

        // 两类型共有的是身份字段（关单须与下单「完全一致」的那几项）。
        foreach (var shared in new[] { "MchId", "OutTradeNo", "SubMchId", "SubAppId" })
        {
            typeof(CombineCloseSubOrder).GetProperty(shared).Should().NotBeNull();
            typeof(CombinePrepaySubOrder).GetProperty(shared).Should().NotBeNull();
        }
    }

    /// <summary>CB4：判错面 + AOT 上下文登记（分组名一致）。</summary>
    [Fact]
    public void CombineDataModels_ShouldBeRegisteredInJsonContext()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(CombinePrepayResponse))
            .Should().BeTrue("下单应答必须承载官方 code/message（判错面）");

        var domainTypes = typeof(CombinePrepayRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(16,
            "合单域 DTO：下单族 6（请求/场景/子单/金额/结算/支付者） + 下单应答 1 + 关单族 2（请求/子单）" +
            " + 查询族 5（应答/支付者/场景/子单/金额） + Native 族 2（请求/应答）");

        foreach (var type in domainTypes)
        {
            CombineTransactionsJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 CombineTransactionsJsonContext（Native AOT 无元数据会静默失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayCombineService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayCombineService.{methodName} 必须存在");

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }
}
