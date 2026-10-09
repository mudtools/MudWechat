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
using Mud.Wechat.Pay.DataModels.Fapiao;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P2 电子发票域契约守卫（<b>首批 2 端点</b>：开具 + 查询）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（2026-10-09 逐页核验，均为<b>普通商户</b>文档中心）</b>：开具电子发票
/// <c>…/doc/v3/merchant/4012538301</c>（<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications</c>，
/// 更新 2025.09.26）、查询电子发票 <c>…/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html</c>
/// （<b>GET</b> <c>…/fapiao-applications/{fapiao_apply_id}?fapiao_id=…</c>）。
/// </para>
/// <para>
/// <b>✅ 本守卫同时锁定一条「方案记录更正」</b>：设计方案 §2.5 记「普通商户文档中心<b>无</b>独立入口」，
/// 实测<b>不成立</b> —— 普通商户侧有完整文档；服务商侧的「开具通用行业电子发票」走
/// <c>/fapiao-applications/<b>issue-general</b></c>（<b>不同路由</b>），本域<b>不得</b>混用该路由。
/// </para>
/// </remarks>
public class WechatPayFapiaoContractGuards
{
    private const string RegistryGroupName = "NewTaxControlFapiao";
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Fapiao";

    /// <summary>
    /// FP1：5 端点路由与方法照官方原文；三个动作型端点（开具 / 冲红 / 插卡）均为 202 无应答体。
    /// </summary>
    /// <remarks>
    /// <b>另有两个非 JSON 端点不在本接口内</b>（裁决，非遗漏）：
    /// <b>上传发票文件</b>是 <c>multipart/form-data</c>（见 <c>WechatPayFapiaoFileService</c> 手写通道，
    /// 表单元信息 <c>meta</c> 走 SM3 摘要，而 SM3 不在 BCL 内 ⇒ 摘要由调用方传入）；
    /// <b>下载发票文件</b>的官方文件域名是 <c>pay.wechatpay.cn</c>，<b>不在</b>进程级白名单内
    /// （<c>WechatApiHosts.AllowedBaseUrlDomains</c> 被 MP-X8 锁定为两条新线零改动）⇒
    /// 放开该主机＝进程级安全面变更，须由宿主显式决策；SDK 当前只返回 <c>download_url</c>。
    /// </remarks>
    [Fact]
    public void FapiaoEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(
            nameof(IWechatPayFapiaoService.IssueFapiaoAsync),
            "/v3/new-tax-control-fapiao/fapiao-applications");
        AssertRoute<GetAttribute>(
            nameof(IWechatPayFapiaoService.QueryFapiaoApplicationsAsync),
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}");

        // 开具：官方 202 且无应答字段 ⇒ 必须无返回值（不得为它造一个应答 DTO 去解空包体）。
        FindMethod(nameof(IWechatPayFapiaoService.IssueFapiaoAsync))
            .ReturnType.Should().Be(typeof(Task), "官方 202 Accepted 且无应答包体 ⇒ 返回类型必须是 Task");
        FindMethod(nameof(IWechatPayFapiaoService.QueryFapiaoApplicationsAsync))
            .ReturnType.Should().NotBe(typeof(Task), "查询有应答体（含嵌套发票树）");

        // 查询：fapiao_apply_id 走 path、fapiao_id 走 query（官方前者必填、后者选填）。
        var query = FindMethod(nameof(IWechatPayFapiaoService.QueryFapiaoApplicationsAsync));
        query.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "fapiaoApplyId" });
        query.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "fapiao_id" });

        // ⚠️ 不得混用服务商侧的 issue-general 路由。
        var routes = typeof(IWechatPayFapiaoService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri)
            .ToList();
        routes.Should().NotContain("/v3/new-tax-control-fapiao/fapiao-applications/issue-general",
            "issue-general 是**服务商**侧『开具通用行业电子发票』的路由，与普通商户侧不是同一接口");

        // 冲红：动作式路径 /reverse（不是对 {fapiao_apply_id} 发 DELETE）；202 无应答体。
        AssertRoute<PostAttribute>(
            nameof(IWechatPayFapiaoService.ReverseFapiaoAsync),
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/reverse");
        FindMethod(nameof(IWechatPayFapiaoService.ReverseFapiaoAsync))
            .ReturnType.Should().Be(typeof(Task), "官方 202 Accepted 且无应答包体 ⇒ 必须无返回值");

        // 获取下载信息：GET 到 /fapiao-files（子资源名不是 /files 之类的直觉名）。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayFapiaoService.GetFapiaoFilesAsync),
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/fapiao-files");
        FindMethod(nameof(IWechatPayFapiaoService.GetFapiaoFilesAsync))
            .ReturnType.Should().Be(typeof(Task<FapiaoFilesResponse>), "本端点返回下载地址列表（非无包体）");

        // 插入卡包：动作式路径 /insert-cards；202 无应答体。
        AssertRoute<PostAttribute>(
            nameof(IWechatPayFapiaoService.InsertFapiaoCardsAsync),
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/insert-cards");
        FindMethod(nameof(IWechatPayFapiaoService.InsertFapiaoCardsAsync))
            .ReturnType.Should().Be(typeof(Task), "官方 202 Accepted 且无应答包体 ⇒ 必须无返回值");

        // 三个路径参数都叫 fapiao_apply_id（占位符名 = C# 参数名，生成器 HTTPCLIENT013 强制）。
        foreach (var name in new[]
                 {
                     nameof(IWechatPayFapiaoService.ReverseFapiaoAsync),
                     nameof(IWechatPayFapiaoService.GetFapiaoFilesAsync),
                     nameof(IWechatPayFapiaoService.InsertFapiaoCardsAsync),
                 })
        {
            FindMethod(name)
                .GetParameters()
                .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
                .Select(static p => p.Name)
                .Should().BeEquivalentTo(new[] { "fapiaoApplyId" });
        }
    }

    /// <summary>FP2：官方字段名锁定（含受票方 / 明细项两套表）。</summary>
    [Fact]
    public void FapiaoDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<FapiaoIssueRequest>(nameof(FapiaoIssueRequest.FapiaoApplyId), "fapiao_apply_id");
        JsonNameShouldBe<FapiaoIssueRequest>(nameof(FapiaoIssueRequest.BuyerInformation), "buyer_information");
        JsonNameShouldBe<FapiaoIssueRequest>(nameof(FapiaoIssueRequest.FapiaoInformation), "fapiao_information");

        JsonNameShouldBe<FapiaoBuyerInformation>(nameof(FapiaoBuyerInformation.TaxpayerId), "taxpayer_id");
        JsonNameShouldBe<FapiaoBuyerInformation>(nameof(FapiaoBuyerInformation.BankAccount), "bank_account");

        JsonNameShouldBe<FapiaoIssueInformation>(nameof(FapiaoIssueInformation.NeedList), "need_list");
        JsonNameShouldBe<FapiaoIssueItem>(nameof(FapiaoIssueItem.TaxCode), "tax_code");
        JsonNameShouldBe<FapiaoIssueItem>(nameof(FapiaoIssueItem.TaxPreferMark), "tax_prefer_mark");

        JsonNameShouldBe<FapiaoQueryResponse>(nameof(FapiaoQueryResponse.TotalCount), "total_count");
        JsonNameShouldBe<FapiaoQueryInformation>(nameof(FapiaoQueryInformation.BlueFapiao), "blue_fapiao");
        JsonNameShouldBe<FapiaoQueryInformation>(nameof(FapiaoQueryInformation.SellerInformation), "seller_information");
        JsonNameShouldBe<FapiaoBlueRedInformation>(nameof(FapiaoBlueRedInformation.CheckCode), "check_code");
        JsonNameShouldBe<FapiaoCardInformation>(nameof(FapiaoCardInformation.CardOpenId), "card_openid");
        JsonNameShouldBe<FapiaoSellerInformation>(nameof(FapiaoSellerInformation.BankName), "bank_name");
        JsonNameShouldBe<FapiaoExtraInformation>(nameof(FapiaoExtraInformation.Drawer), "drawer");
        JsonNameShouldBe<FapiaoQueryItem>(nameof(FapiaoQueryItem.UnitPrice), "unit_price");

        JsonNameShouldBe<FapiaoReverseRequest>(nameof(FapiaoReverseRequest.ReverseReason), "reverse_reason");
        JsonNameShouldBe<FapiaoReverseInformation>(nameof(FapiaoReverseInformation.FapiaoId), "fapiao_id");
        JsonNameShouldBe<FapiaoFilesResponse>(nameof(FapiaoFilesResponse.FapiaoDownloadInfoList), "fapiao_download_info_list");
        JsonNameShouldBe<FapiaoDownloadInfo>(nameof(FapiaoDownloadInfo.DownloadUrl), "download_url");
        JsonNameShouldBe<FapiaoInsertCardsRequest>(nameof(FapiaoInsertCardsRequest.FapiaoCardInformation), "fapiao_card_information");
        JsonNameShouldBe<FapiaoInsertCardInformation>(nameof(FapiaoInsertCardInformation.FapiaoMediaId), "fapiao_media_id");
        JsonNameShouldBe<FapiaoInsertCardInformation>(nameof(FapiaoInsertCardInformation.CheckCode), "check_code");

        JsonNameShouldBe<FapiaoUploadFileResponse>(nameof(FapiaoUploadFileResponse.FapiaoMediaId), "fapiao_media_id");

        // ⚠️ 官方拼写照录：digest_alogrithm（少一个 r）。若被「纠正」为 digest_algorithm，
        // 官方会当作未知字段 ⇒ 摘要校验失败（且错误发生在 multipart 内部，极难定位）。
        // 该字段属 multipart 表单段（非 DTO），故此处锁通道上的字段名常量。
        WechatPayFapiaoFileService.DigestAlgorithmFieldName.Should().Be("digest_alogrithm");
        WechatPayFapiaoFileService.DigestAlgorithmFieldName.Should().NotBe("digest_algorithm",
            "官方拼写少一个 r —— 擅自纠正会导致摘要校验失败");
        WechatPayFapiaoFileService.FileTypeFieldName.Should().Be("file_type");
        WechatPayFapiaoFileService.DigestFieldName.Should().Be("digest");
        WechatPayFapiaoFileService.MediaIdFieldName.Should().Be("fapiao_media_id");
        WechatPayFapiaoFileService.FileFieldName.Should().Be("file");
        WechatPayFapiaoFileService.MetaFieldName.Should().Be("meta");
    }

    /// <summary>FP3：<b>开具与查询的明细项类型必须分开</b>（官方两表不同）。</summary>
    [Fact]
    public void IssueAndQueryItemShapes_ShouldStaySeparate()
    {
        typeof(FapiaoQueryItem).GetProperty("UnitPrice").Should().NotBeNull("查询版明细有单价");
        typeof(FapiaoQueryItem).GetProperty("TaxAmount").Should().NotBeNull("查询版明细有税额");
        typeof(FapiaoIssueItem).GetProperty("UnitPrice").Should().BeNull("官方开具请求的明细表没有 unit_price");
        typeof(FapiaoIssueItem).GetProperty("TaxAmount").Should().BeNull("官方开具请求的明细表没有 tax_amount");
        typeof(FapiaoIssueItem).GetProperty("GoodsCategory").Should().NotBeNull("开具版明细有商品分类");
        typeof(FapiaoQueryItem).GetProperty("GoodsCategory").Should().BeNull("官方查询应答的明细表没有 goods_category");

        // 受票方两张表一致 ⇒ 共用同一 DTO（开具请求与查询应答都用 FapiaoBuyerInformation）。
        typeof(FapiaoQueryInformation).GetProperty("BuyerInformation")!.PropertyType
            .Should().Be<FapiaoBuyerInformation>("官方受票方字段表一致 ⇒ 共用同一 DTO");

        // ⚠️ 两个「card_information」**必须分开**：请求侧（插卡要素）与应答侧（卡券状态）字段表无关。
        typeof(FapiaoInsertCardInformation).GetProperty("FapiaoMediaId").Should().NotBeNull();
        typeof(FapiaoInsertCardInformation).GetProperty("SellerInformation").Should().NotBeNull();
        typeof(FapiaoInsertCardInformation).GetProperty("CardStatus").Should().BeNull(
            "card_status 是**应答**侧卡券状态字段（FapiaoCardInformation），不在插卡请求里");
        typeof(FapiaoCardInformation).GetProperty("FapiaoMediaId").Should().BeNull(
            "fapiao_media_id 是**请求**侧插卡要素，不在应答侧卡券信息里");

        // 插卡复用的三处子结构（官方字段表逐项一致 ⇒ 共用同类型，勿另建同形类）。
        typeof(FapiaoInsertCardsRequest).GetProperty("BuyerInformation")!.PropertyType
            .Should().Be<FapiaoBuyerInformation>();
        typeof(FapiaoInsertCardInformation).GetProperty("SellerInformation")!.PropertyType
            .Should().Be<FapiaoSellerInformation>();
        typeof(FapiaoInsertCardInformation).GetProperty("ExtraInformation")!.PropertyType
            .Should().Be<FapiaoExtraInformation>();
        typeof(FapiaoInsertCardInformation).GetProperty("Items")!.PropertyType
            .Should().Be<List<FapiaoQueryItem>>("插卡明细与查询应答明细字段表一致 ⇒ 共用 FapiaoQueryItem");

        // 冲红目标发票是**三**字段，与蓝红票信息（五字段）不是同一张表 ⇒ 不得复用。
        typeof(FapiaoReverseInformation).GetProperty("FapiaoId").Should().NotBeNull(
            "冲红必须能定位到具体发票 ⇒ 必须含 fapiao_id（蓝红票信息没有它）");
        typeof(FapiaoReverseInformation).GetProperty("CheckCode").Should().BeNull(
            "check_code 属蓝红票信息表，冲红页的字段表没有它");
    }

    /// <summary>FP4：判错面 + AOT 上下文登记（分组名一致）。</summary>
    [Fact]
    public void FapiaoDataModels_ShouldBeRegisteredInJsonContext()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoQueryResponse))
            .Should().BeTrue("查询应答必须承载官方 code/message（判错面）");

        var domainTypes = typeof(FapiaoIssueRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(18,
            "电子发票域 DTO：开具族 4（请求/受票方/发票信息/明细） + 查询族 7（应答/发票信息/蓝红票/卡包/销售方/附加/明细）" +
            " + 冲红族 2（请求/目标发票） + 下载族 2（应答/下载信息） + 插卡族 2（请求/卡券要素）");

        foreach (var type in domainTypes)
        {
            NewTaxControlFapiaoJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 NewTaxControlFapiaoJsonContext（Native AOT 无元数据会静默失败）");
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
        => typeof(IWechatPayFapiaoService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayFapiaoService.{methodName} 必须存在");

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }
}
