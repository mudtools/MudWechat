// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.EducationSchoolPay;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「EducationSchoolPay」契约守卫 —— 本批 <b>8 端点</b>（注册组 <c>EducationSchoolPay</c>，接口 <c>IWechatPayEducationSchoolPayService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.EducationSchoolPay</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 8 支中<b>8 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 8 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 1 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 8 支里<b>1 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>本域八支官方全部只印服务商页</b>（各页均标「支持商户：【普通服务商】」）⇒ 无商户面镜像、无双页归并问题， 一支一页一方法；<b>不得</b>按教育代扣域的双页形态补建商户面方法。</para>
/// <para><b>预签约支的密文字段恰四枚</b>：<c>identity.real_name</c> / <c>identity.id_card_number</c> / <c>bank_card.bank_card_no</c> / <c>bank_card.phone</c> 官方逐枚标「按照APIV3标准加密该字段」且类型一律 <c>string(512)</c> ⇒ 以 <c>string</c> 承载、SDK <b>不代为加解密</b>；同支其余字段（<c>appid</c> / <c>openid</c> / <c>plan_id</c> / <c>school_id</c> 等）官方<b>无</b>加密要求，不得整体按密文处理。</para>
/// <para><b>解除协议支 204 无包体</b> ⇒ 返回裸 <see cref="Task"/>；缴费下单（POST transactions）官方应答表与 两支查单（按 <c>transaction_id</c> / 按 <c>out_trade_no</c>）<b>逐字段同形且必填口径一致</b> ⇒ 三支共用同一响应类型， 这是本域唯一「写侧与读侧共用应答」的一对。</para>
/// <para><b>下单请求里没有 <c>openid</c></b>：扣款对象由 <c>contract_id</c> + <c>user_id</c> 定位（签约协议 + 学生账户）， 而<b>应答</b>侧的 <c>payer</c> 只有 <c>openid</c> / <c>sub_openid</c> ⇒ 请求与应答两侧的定位口径不同，不得按「下单也要传 openid」补字段。</para>
/// <para><b>「受理 ≠ 扣款成功」</b>：共用应答里的 <c>trade_state</c> 可为中间态，官方错误码原文即 「商户订单号重复，请查询订单确定交易状态。确定交易失败后换订单号重试」⇒ 结果由查单支判定，<b>不得</b>据 HTTP 2xx 判定扣款成功。</para>
/// <para><b>欠费状态支应答五键官方全必填</b>（<c>appid</c> / <c>openid</c> / <c>state</c> / <c>debt_count</c> / <c>update_time</c>）；其 <c>state</c> 是「用户欠款风控状态」（<c>NORMAL</c> / <c>FORBIDDEN</c>），与订单支的 <c>trade_state</c> <b>同名不同义 ⇒ 不得互套</b>。本线 DTO 惯例是属性一律可空承载、必填口径只由文档注释与守卫断言表达， 故「全必填」不等于「做成非空类型」。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>1 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayEducationSchoolPayContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.EducationSchoolPay";

    private static readonly Type[] Services = { typeof(IWechatPayEducationSchoolPayService) };

    /// <summary>ESP-B1：拓扑 —— 注册组 EducationSchoolPay、本批 8 方法（接口实测 8）、8 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void EducationSchoolPayTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayEducationSchoolPayService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("EducationSchoolPay", "组名即源生成注册入口名（改组名即改 AddEducationSchoolPayWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(8, "EducationSchoolPay 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(8, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" }, "本域写支全 POST（解除协议是 POST 动词段 /terminate 而非 DELETE）⇒ 不得按「解除一律 DELETE」补动词");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/eduschoolpay/ 前缀内");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayVehicleService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayVehicleService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>ESP-B2：本批 8 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void EducationSchoolPayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 校园缴费预签约｜官方页 v3/partner/4012468952｜身份 【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayContractPresignAsync)).Should().Be("/v3/eduschoolpay/contracts/presign");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayContractPresignAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayContractPresignAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayContractPresignAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayContractPresignAsync)).Should().Be(typeof(EducationSchoolPayContractPresignRequest));
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayContractPresignAsync)).Should().Be(typeof(EducationSchoolPayContractPresignResponse));

        // 查询校园缴费签约｜官方页 v3/partner/4012468959｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayContractAsync)).Should().Be("/v3/eduschoolpay/contracts/{contractId}");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayContractAsync)).Should().BeEquivalentTo(new[] { "contractId" });
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayContractAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayContractAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayContractAsync)).Should().BeNull("查询校园缴费签约官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayContractAsync)).Should().Be(typeof(EducationSchoolPayContractInfoResponse));

        // 解除校园缴费签约｜官方页 v3/partner/4012469025｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayEducationSchoolPayService.TerminateEducationSchoolPayContractAsync)).Should().Be("/v3/eduschoolpay/contracts/{contractId}/terminate");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.TerminateEducationSchoolPayContractAsync)).Should().BeEquivalentTo(new[] { "contractId" });
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.TerminateEducationSchoolPayContractAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.TerminateEducationSchoolPayContractAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.TerminateEducationSchoolPayContractAsync)).Should().BeNull("解除校园缴费签约官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.TerminateEducationSchoolPayContractAsync)).Should().Be(typeof(Task), "解除校园缴费签约官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 按用户查询校园缴费签约列表｜官方页 v3/partner/4012468993｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserContractsAsync)).Should().Be("/v3/eduschoolpay/users/{openid}/contracts");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserContractsAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserContractsAsync)).Should().BeEquivalentTo(new[] { "plan_id", "contract_status", "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserContractsAsync)).Should().BeEquivalentTo(new[] { "plan_id", "contract_status" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserContractsAsync)).Should().BeNull("按用户查询校园缴费签约列表官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserContractsAsync)).Should().Be(typeof(EducationSchoolPayUserContractListResponse));

        // 校园缴费订单查询（微信订单号）｜官方页 v3/partner/4012469063｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByIdAsync)).Should().Be("/v3/eduschoolpay/transactions/id/{transactionId}");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByIdAsync)).Should().BeEquivalentTo(new[] { "transactionId" });
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByIdAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByIdAsync)).Should().BeNull("校园缴费订单查询（微信订单号）官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByIdAsync)).Should().Be(typeof(EducationSchoolPayTransactionQueryResponse));

        // 校园缴费下单｜官方页 v3/partner/4012469030｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayTransactionAsync)).Should().Be("/v3/eduschoolpay/transactions");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayTransactionAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayTransactionAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayTransactionAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayTransactionAsync)).Should().Be(typeof(EducationSchoolPayTransactionCreateRequest));
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.CreateEducationSchoolPayTransactionAsync)).Should().Be(typeof(EducationSchoolPayTransactionQueryResponse));

        // 校园缴费订单查询（商户订单号）｜官方页 v3/partner/4012469097｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByOutTradeNoAsync)).Should().Be("/v3/eduschoolpay/transactions/out-trade-no/{outTradeNo}");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[] { "outTradeNo" });
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByOutTradeNoAsync)).Should().BeNull("校园缴费订单查询（商户订单号）官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayTransactionByOutTradeNoAsync)).Should().Be(typeof(EducationSchoolPayTransactionQueryResponse));

        // 查询用户欠费状态｜官方页 v3/partner/4012469161｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserDebtStateAsync)).Should().Be("/v3/eduschoolpay/users/{openid}/debt-state");
        PathKeysOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserDebtStateAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserDebtStateAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserDebtStateAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserDebtStateAsync)).Should().BeNull("查询用户欠费状态官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationSchoolPayService.QueryEducationSchoolPayUserDebtStateAsync)).Should().Be(typeof(EducationSchoolPayUserDebtStateResponse));
    }

    /// <summary>ESP-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void EducationSchoolPayDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<EducationSchoolPayContractPresignRequest>().Should().BeEquivalentTo(NAMES["EducationSchoolPayContractPresignRequest"], "EducationSchoolPayContractPresignRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationSchoolPayTransactionCreateRequest>().Should().BeEquivalentTo(NAMES["EducationSchoolPayTransactionCreateRequest"], "EducationSchoolPayTransactionCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationSchoolPayContractPresignResponse>().Should().BeEquivalentTo(NAMES["EducationSchoolPayContractPresignResponse"], "EducationSchoolPayContractPresignResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationSchoolPayContractInfoResponse>().Should().BeEquivalentTo(NAMES["EducationSchoolPayContractInfoResponse"], "EducationSchoolPayContractInfoResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationSchoolPayUserContractListResponse>().Should().BeEquivalentTo(NAMES["EducationSchoolPayUserContractListResponse"], "EducationSchoolPayUserContractListResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationSchoolPayTransactionQueryResponse>().Should().BeEquivalentTo(NAMES["EducationSchoolPayTransactionQueryResponse"], "EducationSchoolPayTransactionQueryResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationSchoolPayUserDebtStateResponse>().Should().BeEquivalentTo(NAMES["EducationSchoolPayUserDebtStateResponse"], "EducationSchoolPayUserDebtStateResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>ESP-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void EducationSchoolPayDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationSchoolPayContractPresignResponse)).Should().BeTrue("EducationSchoolPayContractPresignResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationSchoolPayContractInfoResponse)).Should().BeTrue("EducationSchoolPayContractInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationSchoolPayUserContractListResponse)).Should().BeTrue("EducationSchoolPayUserContractListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationSchoolPayTransactionQueryResponse)).Should().BeTrue("EducationSchoolPayTransactionQueryResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationSchoolPayUserDebtStateResponse)).Should().BeTrue("EducationSchoolPayUserDebtStateResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationSchoolPayContractPresignRequest)).Should().BeFalse("EducationSchoolPayContractPresignRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationSchoolPayTransactionCreateRequest)).Should().BeFalse("EducationSchoolPayTransactionCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>ESP-B5：本批 DTO 全量形态 —— 18 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void EducationSchoolPayAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(18, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            EducationSchoolPayJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 EducationSchoolPayJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("EducationSchoolPay", type.Name + " 的 SerializerClassName 必须为 EducationSchoolPay");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    type.Name + " 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in batchDtos.SelectMany(JsonNamesOf))
            name.Should().NotContain(".", name + " 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        // 名册与 B3 的期望值同源：每个根型的全部名单都必须出现在 NAMES 表里（防「B3 漏列某型」）。
        foreach (var type in batchDtos.Where(t => NAMES.ContainsKey(t.Name)))
            DeclaredJsonNamesOf(type).Should().BeEquivalentTo(NAMES[type.Name], type.Name + " 的名单与 B3 期望值必须同源");

        // 时间字段一律 string（rfc3339），不得用 DateTime / DateTimeOffset 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in batchDtos)
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
            }
    }

    /// <summary>ESP-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void EducationSchoolPayRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("EducationSchoolPay", "PayModule 须有 EducationSchoolPay 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddEducationSchoolPayWebApiHttpClient()", "注册委托必须指向源生成方法（组名 EducationSchoolPay 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("EducationSchoolPayJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["EducationSchoolPayContractPresignRequest"] = new[] { "appid", "openid", "plan_id", "user_id", "school_id", "out_contract_code", "contract_mode", "downgrade_default_contract", "identity", "bank_card" },
        ["EducationSchoolPayTransactionCreateRequest"] = new[] { "appid", "sub_mchid", "sub_appid", "description", "attach", "out_trade_no", "goods_tag", "contract_id", "user_id", "amount", "scene_info", "device_info", "settle_info" },
        ["EducationSchoolPayContractPresignResponse"] = new[] { "presign_token" },
        ["EducationSchoolPayContractInfoResponse"] = new[] { "contract_id", "mchid", "appid", "openid", "plan_id", "contract_status", "create_time", "out_contract_code" },
        ["EducationSchoolPayUserContractListResponse"] = new[] { "data", "offset", "limit", "total_count", "links" },
        ["EducationSchoolPayTransactionQueryResponse"] = new[] { "mchid", "appid", "sub_mchid", "sub_appid", "out_trade_no", "transaction_id", "trade_type", "trade_state", "trade_state_desc", "bank_type", "attach", "success_time", "payer", "amount", "device_info", "promotion_detail" },
        ["EducationSchoolPayUserDebtStateResponse"] = new[] { "appid", "openid", "state", "debt_count", "update_time" },
    };

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

    private static string[] DeclaredJsonNamesOf(Type type) => JsonNamesOf(type);

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
