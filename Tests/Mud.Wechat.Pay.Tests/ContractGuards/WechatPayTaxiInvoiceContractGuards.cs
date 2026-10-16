// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.TaxiInvoice;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「TaxiInvoice」契约守卫 —— 本批 <b>9 端点</b>（注册组 <c>TaxiInvoice</c>，接口 <c>IWechatPayTaxiInvoiceService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.TaxiInvoice</c>）。
/// </summary>
/// <remarks>
/// <para><b>本域十支官方全部只印服务商页</b>（各页均标「支持商户：【普通服务商】」）⇒ 无商户面镜像、 无双页归并问题，一支一页一方法；<b>不得</b>按电子发票域或 Papay 面镜像补齐商户支。</para>
/// <para><b>两支 204 无应答包体 ⇒ 裸 <see cref="Task"/> 不建响应类型</b>：签到签退与插卡官方应答节原文 「无应答包体」+ 正常示例 <b>204 No Content</b>；仍按方法计入端点数（B1 的 9 方法数）。</para>
/// <para><b>第十支「上传出租车电子发票文件」刻意不在声明面</b>（非遗漏）：官方为 <c>multipart/form-data</c> （表单 <c>file</c> + <c>meta</c>）且 <c>meta.digest</c> 要求<b>国密 SM3</b> 十六进制摘要（SM3 不在 .NET BCL 内） ⇒ 由手写通道 <c>IWechatPayTaxiInvoiceFileService</c> 承载、<b>不计入</b>端点基线； B1 断言该路由在声明式名册中<b>零出现</b>且通道类型可解析。</para>
/// <para><b><c>digest_algorithm</c> 与电子发票域不是同一契约</b>：本页官方字段名拼写<b>正常</b>、取值 <c>DIGEST_ALGORITHM_SM3</c>，而 <c>/v3/new-tax-control-fapiao/.../upload-fapiao-file</c> 印的是 <c>digest_alogrithm</c>（少一个 r）、取值 <c>SM3</c> ⇒ 两支各照各页，<b>不得互相「纠正」</b>； 两个通道的常量名册由各自守卫逐字锁定。</para>
/// <para><b><c>region_id</c> 是本域的定位基座</b>：十支里九支携带该字段，且「查司机 / 查出租车 / 查公司」三支把它放在 <b>必填 query</b>（不是请求体）⇒ 单靠 <c>driver_license</c> / <c>plate_number</c> / <c>company_mchid</c> 不足以定位（B2 的 <c>RequiredQueryOf</c> 逐支锁定）；「凭凭证查询乘客行程单」的 <c>region_id</c> 官方为<b>选填</b>， 不得一并收紧。</para>
/// <para><b>「更新」支的请求与应答分建</b>：<c>update-driver</c> 与 <c>update-taxi</c> 两侧字段名集相近但必填口径不同 ⇒ 请求与应答各建一型，不做超集合并（B4 的基类分流断言据此才有意义）。</para>
/// <para><b>本域 9 支（含未入声明面的上传支）官方页全部未印端点级 QPS</b>，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayTaxiInvoiceContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.TaxiInvoice";

    private static readonly Type[] Services = { typeof(IWechatPayTaxiInvoiceService) };

    /// <summary>TXI-B1：拓扑 —— 注册组 TaxiInvoice、本批 9 方法（接口实测 9）、9 条 (动词, 路由)；动词集 Post / Get。</summary>
    [Fact]
    public void TaxiInvoiceTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayTaxiInvoiceService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("TaxiInvoice", "组名即源生成注册入口名（改组名即改 AddTaxiInvoiceWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(9, "TaxiInvoice 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(9, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[]{ "PostAttribute", "GetAttribute" }, "动词集照官方页面逐支登记：本域写动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由为 APIv3 形态（官方文档树存在 v2 版式页，但请求 URL 仍印 /v3 前缀）");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayTransactionsService)).Should().HaveCountGreaterThan(0);

        // /v3/taxi-invoice/cards/upload-file 属手工通道 ⇒ 在声明式名册中零出现，但通道本体与 DI 接线必须存在（失踪即静默少一支）。
        Routes().Should().NotContain("/v3/taxi-invoice/cards/upload-file", "官方请求体是 multipart/form-data（file + meta，meta.digest 为国密 SM3）⇒ 声明式 [HttpClientApi] 无法承载，由手写通道承载且不计入端点基线");
        var channelAssembly = typeof(IWechatPayTaxiInvoiceService).Assembly;
        var manualChannel = channelAssembly.GetType("Mud.Wechat.Pay.TaxiInvoice.IWechatPayTaxiInvoiceFileService", throwOnError: false);
        manualChannel.Should().NotBeNull("IWechatPayTaxiInvoiceFileService 必须存在（手工通道接口的公开面）");
        manualChannel!.IsInterface.Should().BeTrue();
        var manualImpl = channelAssembly.GetType("Mud.Wechat.Pay.TaxiInvoice.WechatPayTaxiInvoiceFileService", throwOnError: false);
        manualImpl.Should().NotBeNull("WechatPayTaxiInvoiceFileService 必须存在");
        manualChannel.IsAssignableFrom(manualImpl).Should().BeTrue("WechatPayTaxiInvoiceFileService 必须实现 IWechatPayTaxiInvoiceFileService");
        File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"))
            .Should().Contain("TryAddSingleton<IWechatPayTaxiInvoiceFileService, WechatPayTaxiInvoiceFileService>()",
                "手工通道须随本域模块注册一并挂 DI（TryAdd：宿主预注册者胜出）");
    }

    /// <summary>TXI-B2：本批 9 支端点的路由、path 键、query 名册、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void TaxiInvoiceEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 出租车司机签到签退｜官方页 v3/partner/4012461368｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.PunchTaxiInvoiceAttendanceAsync)).Should().Be("/v3/taxi-invoice/attendance/punch");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.PunchTaxiInvoiceAttendanceAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.PunchTaxiInvoiceAttendanceAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.PunchTaxiInvoiceAttendanceAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.PunchTaxiInvoiceAttendanceAsync)).Should().Be(typeof(TaxiInvoiceAttendancePunchRequest));
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.PunchTaxiInvoiceAttendanceAsync)).Should().Be(typeof(Task), "出租车司机签到签退官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 将出租车电子发票插入用户卡包｜官方页 v3/partner/4012461432｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.InsertTaxiInvoiceCardAsync)).Should().Be("/v3/taxi-invoice/cards");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.InsertTaxiInvoiceCardAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.InsertTaxiInvoiceCardAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.InsertTaxiInvoiceCardAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.InsertTaxiInvoiceCardAsync)).Should().Be(typeof(TaxiInvoiceCardInsertRequest));
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.InsertTaxiInvoiceCardAsync)).Should().Be(typeof(Task), "将出租车电子发票插入用户卡包官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 更新司机信息｜官方页 v3/partner/4012460171｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceDriverAsync)).Should().Be("/v3/taxi-invoice/driver/update-driver");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceDriverAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceDriverAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceDriverAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceDriverAsync)).Should().Be(typeof(TaxiInvoiceDriverUpdateRequest));
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceDriverAsync)).Should().Be(typeof(TaxiInvoiceDriverUpdateResponse));

        // 获取司机信息｜官方页 v3/partner/4012461309｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceDriverAsync)).Should().Be("/v3/taxi-invoice/drivers/{driverLicense}");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceDriverAsync)).Should().BeEquivalentTo(new[]{ "driverLicense" });
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceDriverAsync)).Should().BeEquivalentTo(new[]{ "region_id" });
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceDriverAsync)).Should().BeEquivalentTo(new[]{ "region_id" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceDriverAsync)).Should().BeNull("获取司机信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceDriverAsync)).Should().Be(typeof(TaxiInvoiceDriverInfoResponse));

        // 更新出租车信息｜官方页 v3/partner/4012460167｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceTaxiAsync)).Should().Be("/v3/taxi-invoice/taxi/update-taxi");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceTaxiAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceTaxiAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceTaxiAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceTaxiAsync)).Should().Be(typeof(TaxiInvoiceTaxiUpdateRequest));
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.UpdateTaxiInvoiceTaxiAsync)).Should().Be(typeof(TaxiInvoiceTaxiUpdateResponse));

        // 获取出租车信息｜官方页 v3/partner/4012460169｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiAsync)).Should().Be("/v3/taxi-invoice/taxies/{plateNumber}");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiAsync)).Should().BeEquivalentTo(new[]{ "plateNumber" });
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiAsync)).Should().BeEquivalentTo(new[]{ "region_id" });
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiAsync)).Should().BeEquivalentTo(new[]{ "region_id" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiAsync)).Should().BeNull("获取出租车信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiAsync)).Should().Be(typeof(TaxiInvoiceTaxiInfoResponse));

        // 获取出租车公司商户信息｜官方页 v3/partner/4012460161｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiCompanyAsync)).Should().Be("/v3/taxi-invoice/taxi-companies/{companyMchid}");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiCompanyAsync)).Should().BeEquivalentTo(new[]{ "companyMchid" });
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiCompanyAsync)).Should().BeEquivalentTo(new[]{ "region_id" });
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiCompanyAsync)).Should().BeEquivalentTo(new[]{ "region_id" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiCompanyAsync)).Should().BeNull("获取出租车公司商户信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceTaxiCompanyAsync)).Should().Be(typeof(TaxiInvoiceCompanyInfoResponse));

        // 新增出租车公司｜官方页 v3/partner/4012460154｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.CreateTaxiInvoiceTaxiCompanyAsync)).Should().Be("/v3/taxi-invoice/taxi-company/create-taxi-company");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.CreateTaxiInvoiceTaxiCompanyAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.CreateTaxiInvoiceTaxiCompanyAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.CreateTaxiInvoiceTaxiCompanyAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.CreateTaxiInvoiceTaxiCompanyAsync)).Should().Be(typeof(TaxiInvoiceCompanyCreateRequest));
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.CreateTaxiInvoiceTaxiCompanyAsync)).Should().Be(typeof(TaxiInvoiceCompanyCreateResponse));

        // 凭凭证查询乘客行程单｜官方页 v3/partner/4012460180｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceUserTaxiOrderAsync)).Should().Be("/v3/taxi-invoice/user-taxi-order/find-by-token");
        PathKeysOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceUserTaxiOrderAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceUserTaxiOrderAsync)).Should().BeEquivalentTo(new[]{ "token", "openid", "appid", "region_id", "auth_mode" });
        RequiredQueryOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceUserTaxiOrderAsync)).Should().BeEquivalentTo(new[]{ "token", "openid", "appid", "auth_mode" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceUserTaxiOrderAsync)).Should().BeNull("凭凭证查询乘客行程单官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayTaxiInvoiceService.QueryTaxiInvoiceUserTaxiOrderAsync)).Should().Be(typeof(TaxiInvoiceUserOrderFindResponse));
    }

    /// <summary>TXI-B3：官方字段表顶层名册（请求体与应答，逐字照页；嵌套子树另立类型）。</summary>
    [Fact]
    public void TaxiInvoiceDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<TaxiInvoiceAttendancePunchRequest>().Should().BeEquivalentTo(new[]{ "plate_number", "driver_license", "event_time", "event_type", "region_id" }, "出租车司机签到签退请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceCardInsertRequest>().Should().BeEquivalentTo(new[]{ "company_mchid", "token", "appid", "openid", "region_id", "fapiao_card_information" }, "将出租车电子发票插入用户卡包请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceDriverUpdateRequest>().Should().BeEquivalentTo(new[]{ "driver_name", "id_card_number", "company_name", "mchid", "driver_license", "driver_category", "driver_status", "driver_photo", "region_id" }, "更新司机信息请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceTaxiUpdateRequest>().Should().BeEquivalentTo(new[]{ "plate_number", "company_name", "mchid", "main_license", "deputy_license", "region_id", "invoice_flag", "taxi_state" }, "更新出租车信息请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceCompanyCreateRequest>().Should().BeEquivalentTo(new[]{ "company_name", "tax_id", "address", "phone", "bank_name", "bank_account", "invoice_mode", "legal_person", "short_name", "region_id", "enterprise_type" }, "新增出租车公司请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceDriverUpdateResponse>().Should().BeEquivalentTo(new[]{ "driver_name", "id_card_number", "company_name", "mchid", "driver_license", "driver_category", "driver_status", "driver_photo", "region_id" }, "更新司机信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceDriverInfoResponse>().Should().BeEquivalentTo(new[]{ "driver_name", "company_name", "company_mchid", "driver_license", "driver_category", "driver_photo", "region_id", "driver_state" }, "获取司机信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceTaxiUpdateResponse>().Should().BeEquivalentTo(new[]{ "plate_number", "company_name", "mchid", "main_license", "deputy_license", "region_id", "taxi_state" }, "更新出租车信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceTaxiInfoResponse>().Should().BeEquivalentTo(new[]{ "plate_number", "company_name", "company_mchid", "main_license", "deputy_license", "region_id", "support_invoice", "taxi_state" }, "获取出租车信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceCompanyInfoResponse>().Should().BeEquivalentTo(new[]{ "company_mchid", "enterprise_type", "company_name", "short_name", "tax_id", "address", "phone", "bank_name", "bank_account", "legal_person", "invoice_mode", "region_id" }, "获取出租车公司商户信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceCompanyCreateResponse>().Should().BeEquivalentTo(new[]{ "mchid", "company_name" }, "新增出租车公司应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<TaxiInvoiceUserOrderFindResponse>().Should().BeEquivalentTo(new[]{ "plate_number", "driver_license", "up_time", "region_id", "source", "pay_time", "pay_amount" }, "凭凭证查询乘客行程单应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");
    }

    /// <summary>TXI-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void TaxiInvoiceDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceDriverUpdateResponse)).Should().BeTrue("TaxiInvoiceDriverUpdateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceDriverInfoResponse)).Should().BeTrue("TaxiInvoiceDriverInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceTaxiUpdateResponse)).Should().BeTrue("TaxiInvoiceTaxiUpdateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceTaxiInfoResponse)).Should().BeTrue("TaxiInvoiceTaxiInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceCompanyInfoResponse)).Should().BeTrue("TaxiInvoiceCompanyInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceCompanyCreateResponse)).Should().BeTrue("TaxiInvoiceCompanyCreateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceUserOrderFindResponse)).Should().BeTrue("TaxiInvoiceUserOrderFindResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceAttendancePunchRequest)).Should().BeFalse("TaxiInvoiceAttendancePunchRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceCardInsertRequest)).Should().BeFalse("TaxiInvoiceCardInsertRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceDriverUpdateRequest)).Should().BeFalse("TaxiInvoiceDriverUpdateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceTaxiUpdateRequest)).Should().BeFalse("TaxiInvoiceTaxiUpdateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TaxiInvoiceCompanyCreateRequest)).Should().BeFalse("TaxiInvoiceCompanyCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>TXI-B5：本批 DTO 全量形态 —— 24 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void TaxiInvoiceAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(24, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            TaxiInvoiceJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 TaxiInvoiceJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("TaxiInvoice", type.Name + " 的 SerializerClassName 必须为 TaxiInvoice");
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

    /// <summary>TXI-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void TaxiInvoiceRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("TaxiInvoice", "PayModule 须有 TaxiInvoice 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddTaxiInvoiceWebApiHttpClient()", "注册委托必须指向源生成方法（组名 TaxiInvoice 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("TaxiInvoiceJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
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
