// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.ProfitSharing;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P2 分账域契约守卫（<b>首批 3 端点</b>：添加接收方 / 请求分账 / 查询分账结果）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（2026-10-09 逐页核验）</b>：请求分账
/// <c>pay.weixin.qq.com/doc/v3/merchant/4012524936</c>（<b>POST</b> <c>/v3/profitsharing/orders</c>）、
/// 添加分账接收方 <c>…/4012528995</c>（<b>POST</b> <c>/v3/profitsharing/receivers/add</c>）、
/// 查询分账结果 <c>…/wiki/doc/apiv3/apis/chapter8_1_2.shtml</c>
/// （<b>GET</b> <c>/v3/profitsharing/orders/{out_order_no}?transaction_id=…</c>）。
/// 三页均标注支持商户：【普通商户】，页面更新时间 2025.09.29。
/// </para>
/// <para>
/// <b>核验中值得留档的官方原文</b>：
/// ① 请求分账的 <c>appid</c> 是<b>选填</b>，<b>仅当</b>接收方含 <c>PERSONAL_OPENID</c> 时必填；
/// ② <c>unfreeze_unsplit=true</c> 表示剩余未分资金解冻回发起方且<b>解冻后不可再次分账</b>；
/// ③ <c>state=FINISHED</c> <b>仅</b>代表分账动账执行完毕，各接收方成败须看 <c>receivers[].result</c>；
/// ④ 同一 <c>out_order_no</c> 多次请求<b>等同一次</b>（天然幂等键）；
/// ⑤ 请求分账为<b>异步受理</b>，受理成功不等于分账到账。
/// </para>
/// </remarks>
public class WechatPayProfitSharingContractGuards
{
    private const string RegistryGroupName = "ProfitSharing";

    /// <summary>PS-B1：3 端点的方法与路由照官方原文；GET 端点的 path/query 位置不可互换。</summary>
    [Fact]
    public void ProfitSharingEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayProfitSharingService.AddReceiverAsync), "/v3/profitsharing/receivers/add");
        AssertRoute<PostAttribute>(nameof(IWechatPayProfitSharingService.CreateOrderAsync), "/v3/profitsharing/orders");
        AssertRoute<GetAttribute>(nameof(IWechatPayProfitSharingService.QueryOrderAsync), "/v3/profitsharing/orders/{outOrderNo}");

        // 回退单是**独立资源族** —— 断言锁死「不是 /orders/{out_order_no}/return」这一先验误判。
        AssertRoute<PostAttribute>(nameof(IWechatPayProfitSharingService.CreateReturnOrderAsync), "/v3/profitsharing/return-orders");
        AssertRoute<GetAttribute>(nameof(IWechatPayProfitSharingService.QueryReturnOrderAsync), "/v3/profitsharing/return-orders/{outReturnNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayProfitSharingService.UnfreezeAsync), "/v3/profitsharing/orders/unfreeze");
        AssertRoute<GetAttribute>(nameof(IWechatPayProfitSharingService.QueryUnsplitAmountAsync), "/v3/profitsharing/transactions/{transactionId}/amounts");
        AssertRoute<GetAttribute>(nameof(IWechatPayProfitSharingService.GetBillAsync), "/v3/profitsharing/bills");
        AssertRoute<PostAttribute>(nameof(IWechatPayProfitSharingService.DeleteReceiverAsync), "/v3/profitsharing/receivers/delete");

        // 查询分账结果：out_order_no 走 path、transaction_id 走 query（官方两参数均必填）。
        // 占位符名用 C# 参数名（生成器 HTTPCLIENT013 强制占位符 ↔ [Path] 参数名对应），不影响线上报文。
        var query = FindMethod(nameof(IWechatPayProfitSharingService.QueryOrderAsync));
        query.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "outOrderNo" }, "官方 path 参数 out_order_no 必须走路径替换而非查询串");
        query.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "transaction_id" });
    }

    /// <summary>PS-B2：分账域 DTO 官方字段名锁定（照官方原文，<b>不得「纠正」</b>）。</summary>
    [Fact]
    public void ProfitSharingDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<ProfitSharingAddReceiverRequest>(nameof(ProfitSharingAddReceiverRequest.RelationType), "relation_type");
        JsonNameShouldBe<ProfitSharingAddReceiverRequest>(nameof(ProfitSharingAddReceiverRequest.CustomRelation), "custom_relation");
        JsonNameShouldBe<ProfitSharingReceiver>(nameof(ProfitSharingReceiver.Account), "account");

        JsonNameShouldBe<ProfitSharingOrderRequest>(nameof(ProfitSharingOrderRequest.TransactionId), "transaction_id");
        JsonNameShouldBe<ProfitSharingOrderRequest>(nameof(ProfitSharingOrderRequest.OutOrderNo), "out_order_no");
        JsonNameShouldBe<ProfitSharingOrderRequest>(nameof(ProfitSharingOrderRequest.UnfreezeUnsplit), "unfreeze_unsplit");

        JsonNameShouldBe<ProfitSharingOrderResponse>(nameof(ProfitSharingOrderResponse.OrderId), "order_id");
        JsonNameShouldBe<ProfitSharingOrderResponse>(nameof(ProfitSharingOrderResponse.State), "state");

        JsonNameShouldBe<ProfitSharingReceiverResult>(nameof(ProfitSharingReceiverResult.FailReason), "fail_reason");
        JsonNameShouldBe<ProfitSharingReceiverResult>(nameof(ProfitSharingReceiverResult.DetailId), "detail_id");
        JsonNameShouldBe<ProfitSharingReceiverResult>(nameof(ProfitSharingReceiverResult.CreateTime), "create_time");

        JsonNameShouldBe<ProfitSharingReturnOrderRequest>(nameof(ProfitSharingReturnOrderRequest.OutReturnNo), "out_return_no");
        JsonNameShouldBe<ProfitSharingReturnOrderRequest>(nameof(ProfitSharingReturnOrderRequest.ReturnMchId), "return_mchid");
        JsonNameShouldBe<ProfitSharingReturnOrderResponse>(nameof(ProfitSharingReturnOrderResponse.ReturnId), "return_id");
        JsonNameShouldBe<ProfitSharingReturnOrderResponse>(nameof(ProfitSharingReturnOrderResponse.FailReason), "fail_reason");
        JsonNameShouldBe<ProfitSharingUnfreezeRequest>(nameof(ProfitSharingUnfreezeRequest.OutOrderNo), "out_order_no");
        JsonNameShouldBe<ProfitSharingAmountsResponse>(nameof(ProfitSharingAmountsResponse.UnsplitAmount), "unsplit_amount");

        JsonNameShouldBe<ProfitSharingDeleteReceiverRequest>(nameof(ProfitSharingDeleteReceiverRequest.Account), "account");
        JsonNameShouldBe<ProfitSharingDeleteReceiverResponse>(nameof(ProfitSharingDeleteReceiverResponse.Type), "type");
    }

    /// <summary>PS-B3：判错面 —— 分账单应答须继承 <see cref="WechatPayResponse"/>（承载官方 <c>code</c>/<c>message</c>）。</summary>
    /// <remarks>
    /// 本域接口带 <c>[AllowAnyStatusCode]</c>（官方 4xx 错误体 <c>{code,message}</c> 不被组件拦成 <c>ApiException</c>），
    /// 错误体必须有落点：漏继承的应答在失败场景下会「字段全空但调用方以为成功」。
    /// </remarks>
    [Fact]
    public void ProfitSharingOrderResponse_ShouldDeriveFromWechatPayResponse()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(ProfitSharingOrderResponse))
            .Should().BeTrue("分账单应答必须能承载官方 code/message，否则 PARAM_ERROR 等错误静默变成空对象");
    }

    /// <summary>PS-B4：官方枚举取值逐项锁定（含「仅两值 / 十值 / 三态 / 终态语义」这些易被顺手扩展的点）。</summary>
    [Fact]
    public void ProfitSharingEnumConstants_ShouldMatchOfficialValues()
    {
        ProfitSharingReceiverTypes.MerchantId.Should().Be("MERCHANT_ID");
        ProfitSharingReceiverTypes.PersonalOpenId.Should().Be("PERSONAL_OPENID");

        // 官方 relation_type 共 10 值 —— 数量与取值同时锁定（防漏抄 / 防自造值）。
        var relationTypes = typeof(ProfitSharingRelationTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
        relationTypes.Should().BeEquivalentTo(new[]
        {
            "STORE", "STAFF", "STORE_OWNER", "PARTNER", "HEADQUARTER",
            "BRAND", "DISTRIBUTOR", "USER", "SUPPLIER", "CUSTOM",
        });
        relationTypes.Should().HaveCount(10, "官方 relation_type 枚举为 10 值");

        ProfitSharingOrderStates.Processing.Should().Be("PROCESSING");
        ProfitSharingOrderStates.Finished.Should().Be("FINISHED");

        ProfitSharingReceiverResults.Pending.Should().Be("PENDING");
        ProfitSharingReceiverResults.Success.Should().Be("SUCCESS");
        ProfitSharingReceiverResults.Closed.Should().Be("CLOSED");

        ProfitSharingFailReasons.AccountAbnormal.Should().Be("ACCOUNT_ABNORMAL");
        ProfitSharingFailReasons.ReceiverRealNameNotVerified.Should().Be("RECEIVER_REAL_NAME_NOT_VERIFIED");
        ProfitSharingFailReasons.PayerAccountAbnormal.Should().Be("PAYER_ACCOUNT_ABNORMAL");
        ProfitSharingFailReasons.InvalidRequest.Should().Be("INVALID_REQUEST");

        // 回退单是**另一套枚举**：终态为 SUCCESS/FAILED（分账单是 FINISHED），失败原因 5 值（分账单 8 值）。
        ProfitSharingReturnResults.Processing.Should().Be("PROCESSING");
        ProfitSharingReturnResults.Success.Should().Be("SUCCESS");
        ProfitSharingReturnResults.Failed.Should().Be("FAILED");
        ProfitSharingOrderStates.Finished.Should().NotBe(ProfitSharingReturnResults.Success,
            "两套结果枚举不得混用（分账单 FINISHED ≠ 回退单 SUCCESS）");

        var returnFailReasons = typeof(ProfitSharingReturnFailReasons)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
        returnFailReasons.Should().BeEquivalentTo(new[]
        {
            "ACCOUNT_ABNORMAL", "BALANCE_NOT_ENOUGH", "TIME_OUT_CLOSED",
            "PAYER_ACCOUNT_ABNORMAL", "INVALID_REQUEST",
        });
        returnFailReasons.Should().HaveCount(5, "官方回退失败原因为 5 值（与分账单的 8 值不是同一张表）");
    }

    /// <summary>PS-B5：分账域 DTO 全量登记进 AOT 上下文且分组名一致。</summary>
    [Fact]
    public void ProfitSharingDataModels_ShouldBeRegisteredInJsonContext()
    {
        var domainTypes = typeof(ProfitSharingOrderResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == "Mud.Wechat.Pay.DataModels.ProfitSharing")
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(12,
            "分账域 DTO：分账单族 6（接收方请求/接收方/分账请求/请求接收方/分账单应答/接收方结果）" +
            " + 回退单族 2（请求/应答） + 解冻请求 1 + 待分金额应答 1 + 删除接收方 2（请求/应答）");

        foreach (var type in domainTypes)
        {
            ProfitSharingJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 ProfitSharingJsonContext（否则 Native AOT 下无元数据）");
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
        => typeof(IWechatPayProfitSharingService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayProfitSharingService.{methodName} 必须存在");

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }
}
