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
using Mud.Wechat.Pay.DataModels.Transfer;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P2 商家转账域契约守卫（<b>首批 1 端点</b>：发起转账）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（2026-10-09 核验）</b>：发起转账
/// <c>pay.weixin.qq.com/doc/v3/merchant/4012716434</c>
/// （<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-bills</c>，更新 2025.03.21，支持商户：普通商户）。
/// </para>
/// <para>
/// <b>🔴 官方资金安全原文（本域第一原则，须长期留档）</b>：「如果发起转账接口遇到新的错误码，
/// 请务必<b>不要换单重试</b>，需通过『商户单号查询转账单』或『微信单号查询转账单』API 接口查询订单结果，
/// <b>当查询原订单结果明确为「失败」时，再更换商户订单号进行重试。否则会有重复转账的资金风险。</b>」
/// </para>
/// <para>
/// <b>本批刻意的范围限制</b>：仅落「发起转账」。两个<b>查询端点尚未实现</b> ⇒ 按官方要求
/// 「遇错必须先查原单」的流程<b>尚不可执行</b>，故本域<b>暂不具备生产可用性</b>。
/// 下一增量必须优先补：商户单号查询转账单 / 微信单号查询转账单 / 撤销转账 / 获取电子回单
/// （其中「商户单号查询电子回单」路由已核：<c>GET /v3/fund-app/mch-transfer/elecsign/out-bill-no/{out_bill_no}</c>）。
/// </para>
/// <para>
/// <b>⚠️ 值域未核验</b>：<c>state</c> / <c>transfer_scene_id</c> / <c>user_recv_perception</c> /
/// <c>user_recv_style.type</c> 的取值表本轮<b>未</b>取得 ⇒ 守卫<b>不</b>断言任何常量
/// （臆造取值比留空字符串危险得多）。
/// </para>
/// </remarks>
public class WechatPayTransferContractGuards
{
    private const string RegistryGroupName = "Transfer";
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Transfer";

    /// <summary>TR1：端点路由与方法照官方原文；无 path / query 参数。</summary>
    [Fact]
    public void TransferEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(
            nameof(IWechatPayTransferService.CreateTransferBillAsync),
            "/v3/fund-app/mch-transfer/transfer-bills");

        var method = FindMethod(nameof(IWechatPayTransferService.CreateTransferBillAsync));
        method.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方本页无 query 参数");
        method.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Should().BeEmpty("官方本页无 path 参数");

        // 商户单号查询转账单：仅 path 参数，无 query / body。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayTransferService.QueryByOutBillNoAsync),
            "/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}");
        var query = FindMethod(nameof(IWechatPayTransferService.QueryByOutBillNoAsync));
        query.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "outBillNo" });
        query.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方查询页无 query 参数");
        query.ReturnType.Should().NotBe(typeof(Task), "查询有应答体（12 字段）");

        // 微信单号查询转账单：同样仅 path 参数；且与商户单号版**共用同一应答 DTO**（官方两页字段表逐项一致）。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayTransferService.QueryByTransferBillNoAsync),
            "/v3/fund-app/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}");
        var byTransferNo = FindMethod(nameof(IWechatPayTransferService.QueryByTransferBillNoAsync));
        byTransferNo.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "transferBillNo" });
        byTransferNo.ReturnType.Should().Be(typeof(Task<TransferBillQueryResponse>),
            "官方两页应答字段表逐项一致 ⇒ 复用同一 DTO（防两处字段各自漂移）");

        // 商户单号查询电子回单：仅 path 参数。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayTransferService.QueryElecsignByOutBillNoAsync),
            "/v3/fund-app/mch-transfer/elecsign/out-bill-no/{outBillNo}");
        FindMethod(nameof(IWechatPayTransferService.QueryElecsignByOutBillNoAsync))
            .GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "outBillNo" });

        // 微信单号查询电子回单：同样仅 path 参数；且与商户单号版**共用同一应答 DTO**（官方两页字段表逐项一致）。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayTransferService.QueryElecsignByTransferBillNoAsync),
            "/v3/fund-app/mch-transfer/elecsign/transfer-bill-no/{transferBillNo}");
        FindMethod(nameof(IWechatPayTransferService.QueryElecsignByTransferBillNoAsync))
            .ReturnType.Should().Be(typeof(Task<TransferElecsignResponse>),
                "官方两页回单应答字段表逐项一致 ⇒ 复用同一 DTO（防两处字段各自漂移）");

        // 撤销转账：**普通商户**面路由；官方本接口**无请求体** ⇒ 方法不得声明 [Body] 参数
        // （发一个官方没定义的 {} 会被判 PARAM_ERROR）。
        AssertRoute<PostAttribute>(
            nameof(IWechatPayTransferService.RevokeTransferAsync),
            "/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel");
        var revoke = FindMethod(nameof(IWechatPayTransferService.RevokeTransferAsync));
        revoke.GetParameters().Should().NotContain(
            static p => p.GetCustomAttribute<BodyAttribute>() != null,
            "官方《撤销转账》字段表无任何 body 字段 ⇒ 不得造空请求体");
        revoke.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "outBillNo" });
        revoke.ReturnType.Should().NotBe(typeof(Task), "撤销有应答体（4 字段），不是 204 无包体");
    }

    /// <summary>
    /// TR5：撤销转账走 <b>普通商户面</b>路由，<b>不得</b>混入服务商侧的 <c>/partner/</c> 变体。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>已核事实（2026-10-09）</b>：partner 侧《撤销转账》
    /// （<c>pay.weixin.qq.com/doc/v3/partner/4015469118</c>，更新 2025.07.29）给的请求路径是
    /// <c>POST /v3/fund-app/mch-transfer/<b>partner</b>/transfer-bills/out-bill-no/{out_bill_no}/cancel</c>
    /// —— <b>含 <c>/partner/</c> 段</b>；请求体仅 <c>sub_mchid</c>（必填），应答为转账单 13 字段
    /// （比查询多一个 <c>sub_mchid</c>）。
    /// </para>
    /// <para>
    /// <b>本轮已核到普通商户面的对应页并完成实现</b>（<c>pay.weixin.qq.com/doc/v3/merchant/4012716458</c>，
    /// 更新 2025.03.18，标注【普通商户】）：路由为
    /// <c>POST /v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}/cancel</c>
    /// —— <b>无 <c>/partner/</c> 段、且无请求体</b>，应答为 4 字段（<c>out_bill_no</c> /
    /// <c>transfer_bill_no</c> / <c>state</c> / <c>update_time</c>）。
    /// </para>
    /// <para>
    /// <b>本守卫的裁决已随之更新</b>：上轮「merchant 路由未核验 ⇒ 暂不建模」的处置在当时是正确的
    /// （当时只核到 partner 页），拿到 merchant 官方页后<b>同批解除</b>并改为<b>正向</b>断言。
    /// <b>但 partner 路由仍严禁进入本接口</b>：两套面在官方是不同文档、不同必填性
    /// （partner 侧要 <c>sub_mchid</c> 与 13 字段应答），去掉 <c>/partner/</c> 段照搬会静默错。
    /// </para>
    /// <para>
    /// <b>⚠️ 守卫与事实的同步纪律</b>：本用例是「裁决随核验结果变更」的示范 ——
    /// 事实变化时，守卫必须在<b>同一批</b>改动里改写，否则守卫会从「防错」退化为「锁错」。
    /// </para>
    /// </remarks>
    [Fact]
    public void RevokeTransfer_ShouldUseMerchantRoute_NotPartnerRoute()
    {
        var routes = typeof(IWechatPayTransferService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri)
            .ToList();

        routes.Should().Contain(
            "/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel",
            "普通商户面撤销路由已由官方页 4012716458 核验 ⇒ 必须实现");
        routes.Should().NotContain(
            "/v3/fund-app/mch-transfer/partner/transfer-bills/out-bill-no/{outBillNo}/cancel",
            "partner 专属路由不得进入普通商户面接口（官方两套文档、必填性不同）");
        routes.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>TR6：电子回单状态与摘要类型取值锁定（与转账单状态是两套枚举）。</summary>
    [Fact]
    public void ElecsignConstants_ShouldMatchOfficialValues()
    {
        var elecsignStates = typeof(TransferElecsignStates)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
        elecsignStates.Should().BeEquivalentTo(new[] { "GENERATING", "FINISHED", "FAILED" });
        elecsignStates.Should().HaveCount(3, "官方电子回单申请单状态为 3 值");

        // 两套 state 枚举不得混用：转账单有 ACCEPTED，电子回单没有。
        TransferElecsignStates.Finished.Should().NotBe(TransferBillStates.Success,
            "电子回单的 FINISHED 与转账单的 SUCCESS 是两套语义");

        var hashTypes = typeof(TransferElecsignHashTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
        hashTypes.Should().BeEquivalentTo(new[] { "SHA256", "SM3" });
    }

    /// <summary>TR2：官方字段名锁定。</summary>
    [Fact]
    public void TransferDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<TransferBillRequest>(nameof(TransferBillRequest.OutBillNo), "out_bill_no");
        JsonNameShouldBe<TransferBillRequest>(nameof(TransferBillRequest.TransferSceneId), "transfer_scene_id");
        JsonNameShouldBe<TransferBillRequest>(nameof(TransferBillRequest.TransferAmount), "transfer_amount");
        JsonNameShouldBe<TransferBillRequest>(nameof(TransferBillRequest.TransferRemark), "transfer_remark");
        JsonNameShouldBe<TransferBillRequest>(nameof(TransferBillRequest.UserRecvPerception), "user_recv_perception");
        JsonNameShouldBe<TransferBillRequest>(nameof(TransferBillRequest.TransferSceneReportInfos), "transfer_scene_report_infos");
        JsonNameShouldBe<TransferBillRequest>(nameof(TransferBillRequest.UserRecvStyle), "user_recv_style");

        JsonNameShouldBe<TransferSceneReportInfo>(nameof(TransferSceneReportInfo.InfoContent), "info_content");
        JsonNameShouldBe<TransferUserRecvStyle>(nameof(TransferUserRecvStyle.Type), "type");

        JsonNameShouldBe<TransferBillResponse>(nameof(TransferBillResponse.TransferBillNo), "transfer_bill_no");
        JsonNameShouldBe<TransferBillResponse>(nameof(TransferBillResponse.CreateTime), "create_time");
        JsonNameShouldBe<TransferBillResponse>(nameof(TransferBillResponse.PackageInfo), "package_info");

        JsonNameShouldBe<TransferBillQueryResponse>(nameof(TransferBillQueryResponse.OutBillNo), "out_bill_no");
        JsonNameShouldBe<TransferBillQueryResponse>(nameof(TransferBillQueryResponse.FailReason), "fail_reason");
        JsonNameShouldBe<TransferBillQueryResponse>(nameof(TransferBillQueryResponse.UpdateTime), "update_time");

        JsonNameShouldBe<TransferElecsignResponse>(nameof(TransferElecsignResponse.HashType), "hash_type");
        JsonNameShouldBe<TransferElecsignResponse>(nameof(TransferElecsignResponse.HashValue), "hash_value");
        JsonNameShouldBe<TransferElecsignResponse>(nameof(TransferElecsignResponse.DownloadUrl), "download_url");

        JsonNameShouldBe<TransferRevokeResponse>(nameof(TransferRevokeResponse.OutBillNo), "out_bill_no");
        JsonNameShouldBe<TransferRevokeResponse>(nameof(TransferRevokeResponse.TransferBillNo), "transfer_bill_no");
        JsonNameShouldBe<TransferRevokeResponse>(nameof(TransferRevokeResponse.UpdateTime), "update_time");

        // ⚠️ 商户号字段名陷阱：本域（商家转账）官方用 mch_id（**带下划线**），
        // 而支付线其它域一律 mchid（无下划线）⇒ 两侧都钉死，禁止互相「纠正」。
        JsonNameShouldBe<TransferBillQueryResponse>(nameof(TransferBillQueryResponse.MchId), "mch_id");
        JsonNameShouldBe<CloseOrderRequest>(nameof(CloseOrderRequest.MchId), "mchid");
    }

    /// <summary>TR4：<c>state</c> 取值逐项锁定（含「可原单重试」的非终态与三个终态）。</summary>
    /// <remarks>
    /// 本表是「发起转账遇错不得换单重试」红线的<b>判定依据</b>：<c>ACCEPTED</c>/<c>PROCESSING</c>
    /// 属可原单重试的非终态，<c>FAIL</c> 才是允许重新生成单据的终态。取值错一个字符即判定失效。
    /// </remarks>
    [Fact]
    public void TransferBillStates_ShouldMatchOfficialValues()
    {
        var states = typeof(TransferBillStates)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

        states.Should().BeEquivalentTo(new[]
        {
            "ACCEPTED", "PROCESSING", "WAIT_USER_CONFIRM", "TRANSFERING",
            "SUCCESS", "FAIL", "CANCELING", "CANCELLED",
        });
        states.Should().HaveCount(8, "官方 state 为 8 值");

        // 可原单重试的两个非终态必须存在且拼写正确（本红线的判定入口）。
        TransferBillStates.Accepted.Should().Be("ACCEPTED");
        TransferBillStates.Processing.Should().Be("PROCESSING");
        // 唯一允许「重新生成单据」的终态。
        TransferBillStates.Fail.Should().Be("FAIL");
    }

    /// <summary>TR3：判错面 + AOT 上下文登记（分组名一致）。</summary>
    [Fact]
    public void TransferDataModels_ShouldBeRegisteredInJsonContext()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TransferBillResponse))
            .Should().BeTrue("发起转账应答必须承载官方 code/message（判错面）");

        var domainTypes = typeof(TransferBillRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(7,
            "商家转账域 DTO：发起族 4（请求/场景报备/收款样式/应答） + 查询应答 1 + 电子回单应答 1 + 撤销应答 1");

        foreach (var type in domainTypes)
        {
            TransferJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 TransferJsonContext（Native AOT 无元数据会静默失败）");
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
        => typeof(IWechatPayTransferService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayTransferService.{methodName} 必须存在");

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }
}
