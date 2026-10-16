// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.EducationPapay;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「EducationPapay」契约守卫 —— 本批 <b>8 端点</b>（注册组 <c>EducationPapay</c>，接口 <c>IWechatPayEducationPapayService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.EducationPapay</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 8 支中<b>1 支官方只印一页</b>、<b>7 支印多页</b>（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 8 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 3 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>七支印双页、一支只印商户页</b>：本域 <c>/edu-papay/*</c> 八支里七支同时印【普通商户】与【普通服务商】两页 ⇒ 按<b>并集</b>建模、一面一方法，身份差异（服务商页多 <c>sub_mchid</c> / <c>sub_appid</c>）写进参数注释而不拆方法； 但 <c>DELETE /v3/edu-papay/contracts/{contract_id}</c>（解除协议）<b>官方只印商户页</b> ⇒ 该支无服务商镜像， <b>不得</b>按「同域其余支都有双页」补齐，也不得把服务商页文档链接错挂到本支（处置表原引的服务商 docId 实为 <c>GET /v3/edu-papay/user/{openid}/contracts</c> 的服务商页，已逐对核正）。</para>
/// <para><b>三支官方成功应答是 204 无包体</b>：解除协议（DELETE）、下单受理（POST transactions）、 发送预扣费通知（POST user-notifications）⇒ 一律返回裸 <see cref="Task"/>，不建响应类型； 其中「下单」是<b>受理</b>不是扣款成功，扣款结果只由异步通知与查单支判定。</para>
/// <para><b>两支查单应答逐字段同形 ⇒ 共用一型</b>：<c>/transactions/id/{transaction_id}</c> 与 <c>/transactions/out-trade-no/{out_trade_no}</c> 官方应答表 33 行同形且必填口径一致 ⇒ 共用一个响应类型。 但「按 <c>contract_id</c> 查协议」与「按 <c>openid</c> 查协议列表」两支<b>各建一应答</b>： 列表支的 <c>data[]</c> 元素与单查应答字段同形，本线口径是<b>共用只发生在应答根类型</b>、 子树按「端点 + 路径段」机械拼装独立成类型 ⇒ 不得为「看起来一样」把单查应答当成列表元素类型。</para>
/// <para><b>预签约支只回 <c>presign_token</c></b>：官方两页应答表单行且<b>必填</b> ⇒ 该 token 是调起签约小程序/公众号页面的入参， 不等于签约成功；签约结果须回查协议支。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>3 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayEducationPapayContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.EducationPapay";

    private static readonly Type[] Services = { typeof(IWechatPayEducationPapayService) };

    /// <summary>EDP-B1：拓扑 —— 注册组 EducationPapay、本批 8 方法（接口实测 8）、8 条 (动词, 路由)；动词集 Delete / Get / Post。</summary>
    [Fact]
    public void EducationPapayTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayEducationPapayService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("EducationPapay", "组名即源生成注册入口名（改组名即改 AddEducationPapayWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(8, "EducationPapay 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(8, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "DeleteAttribute", "GetAttribute", "PostAttribute" }, "本域写动词只有 POST 与 DELETE（解除协议是 DELETE、余下写支是 POST），读支全 GET ⇒ 动词集合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/edu-papay/ 前缀内，与直连 PAPay 的 /v3/papay/ 零交集");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayPapayService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayPapayService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>EDP-B2：本批 8 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void EducationPapayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 解除教育代扣签约｜官方页 v3/merchant/4012466882｜身份 【普通商户】
        RouteOf(nameof(IWechatPayEducationPapayService.TerminateEducationPapayContractAsync)).Should().Be("/v3/edu-papay/contracts/{contractId}");
        PathKeysOf(nameof(IWechatPayEducationPapayService.TerminateEducationPapayContractAsync)).Should().BeEquivalentTo(new[] { "contractId" });
        QueryNamesOf(nameof(IWechatPayEducationPapayService.TerminateEducationPapayContractAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.TerminateEducationPapayContractAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.TerminateEducationPapayContractAsync)).Should().BeNull("解除教育代扣签约官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.TerminateEducationPapayContractAsync)).Should().Be(typeof(Task), "解除教育代扣签约官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 按微信协议号查询教育代扣签约｜官方页 v3/merchant/4012522951 + v3/partner/4012465367｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayContractByIdAsync)).Should().Be("/v3/edu-papay/contracts/id/{contractId}");
        PathKeysOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayContractByIdAsync)).Should().BeEquivalentTo(new[] { "contractId" });
        QueryNamesOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayContractByIdAsync)).Should().BeEquivalentTo(new[] { "appid", "sub_mchid", "sub_appid" });
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayContractByIdAsync)).Should().BeEquivalentTo(new[] { "appid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayContractByIdAsync)).Should().BeNull("按微信协议号查询教育代扣签约官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayContractByIdAsync)).Should().Be(typeof(EducationPapayContractInfoResponse));

        // 按用户查询教育代扣签约列表｜官方页 v3/merchant/4012523265 + v3/partner/4012489548｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayUserContractsAsync)).Should().Be("/v3/edu-papay/user/{openid}/contracts");
        PathKeysOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayUserContractsAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayUserContractsAsync)).Should().BeEquivalentTo(new[] { "appid", "plan_id", "contract_status", "offset", "limit", "sub_mchid", "sub_appid", "sub_openid" });
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayUserContractsAsync)).Should().BeEquivalentTo(new[] { "appid", "plan_id" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayUserContractsAsync)).Should().BeNull("按用户查询教育代扣签约列表官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayUserContractsAsync)).Should().Be(typeof(EducationPapayUserContractListResponse));

        // 教育代扣预签约｜官方页 v3/merchant/4012522560 + v3/partner/4012465360｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayContractPresignAsync)).Should().Be("/v3/edu-papay/contracts/presign");
        PathKeysOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayContractPresignAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayContractPresignAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayContractPresignAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayContractPresignAsync)).Should().Be(typeof(EducationPapayContractPresignRequest));
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayContractPresignAsync)).Should().Be(typeof(EducationPapayContractPresignResponse));

        // 教育代扣下单（受理）｜官方页 v3/merchant/4012524731 + v3/partner/4012465548｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayTransactionAsync)).Should().Be("/v3/edu-papay/transactions");
        PathKeysOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayTransactionAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayTransactionAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayTransactionAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayTransactionAsync)).Should().Be(typeof(EducationPapayTransactionCreateRequest));
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.CreateEducationPapayTransactionAsync)).Should().Be(typeof(Task), "教育代扣下单（受理）官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 教育代扣订单查询（微信订单号）｜官方页 v3/merchant/4012523468 + v3/partner/4012465567｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByIdAsync)).Should().Be("/v3/edu-papay/transactions/id/{transactionId}");
        PathKeysOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByIdAsync)).Should().BeEquivalentTo(new[] { "transactionId" });
        QueryNamesOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByIdAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByIdAsync)).Should().BeNull("教育代扣订单查询（微信订单号）官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByIdAsync)).Should().Be(typeof(EducationPapayTransactionQueryResponse));

        // 教育代扣订单查询（商户订单号）｜官方页 v3/merchant/4012523591 + v3/partner/4012465574｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByOutTradeNoAsync)).Should().Be("/v3/edu-papay/transactions/out-trade-no/{outTradeNo}");
        PathKeysOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[] { "outTradeNo" });
        QueryNamesOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByOutTradeNoAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByOutTradeNoAsync)).Should().BeNull("教育代扣订单查询（商户订单号）官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.QueryEducationPapayTransactionByOutTradeNoAsync)).Should().Be(typeof(EducationPapayTransactionQueryResponse));

        // 发送扣费前用户通知｜官方页 v3/merchant/4012524814 + v3/partner/4012855408｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayEducationPapayService.SendEducationPapayUserNotificationAsync)).Should().Be("/v3/edu-papay/user-notifications/{contractId}/send");
        PathKeysOf(nameof(IWechatPayEducationPapayService.SendEducationPapayUserNotificationAsync)).Should().BeEquivalentTo(new[] { "contractId" });
        QueryNamesOf(nameof(IWechatPayEducationPapayService.SendEducationPapayUserNotificationAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayEducationPapayService.SendEducationPapayUserNotificationAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayEducationPapayService.SendEducationPapayUserNotificationAsync)).Should().Be(typeof(EducationPapayUserNotificationSendRequest));
        ReturnTypeOf(nameof(IWechatPayEducationPapayService.SendEducationPapayUserNotificationAsync)).Should().Be(typeof(Task), "发送扣费前用户通知官方成功应答是 204 无包体 ⇒ 裸 Task");
    }

    /// <summary>EDP-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void EducationPapayDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<EducationPapayContractPresignRequest>().Should().BeEquivalentTo(NAMES["EducationPapayContractPresignRequest"], "EducationPapayContractPresignRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationPapayTransactionCreateRequest>().Should().BeEquivalentTo(NAMES["EducationPapayTransactionCreateRequest"], "EducationPapayTransactionCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationPapayUserNotificationSendRequest>().Should().BeEquivalentTo(NAMES["EducationPapayUserNotificationSendRequest"], "EducationPapayUserNotificationSendRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationPapayContractInfoResponse>().Should().BeEquivalentTo(NAMES["EducationPapayContractInfoResponse"], "EducationPapayContractInfoResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationPapayUserContractListResponse>().Should().BeEquivalentTo(NAMES["EducationPapayUserContractListResponse"], "EducationPapayUserContractListResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationPapayContractPresignResponse>().Should().BeEquivalentTo(NAMES["EducationPapayContractPresignResponse"], "EducationPapayContractPresignResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<EducationPapayTransactionQueryResponse>().Should().BeEquivalentTo(NAMES["EducationPapayTransactionQueryResponse"], "EducationPapayTransactionQueryResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>EDP-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void EducationPapayDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationPapayContractInfoResponse)).Should().BeTrue("EducationPapayContractInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationPapayUserContractListResponse)).Should().BeTrue("EducationPapayUserContractListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationPapayContractPresignResponse)).Should().BeTrue("EducationPapayContractPresignResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationPapayTransactionQueryResponse)).Should().BeTrue("EducationPapayTransactionQueryResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationPapayContractPresignRequest)).Should().BeFalse("EducationPapayContractPresignRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationPapayTransactionCreateRequest)).Should().BeFalse("EducationPapayTransactionCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(EducationPapayUserNotificationSendRequest)).Should().BeFalse("EducationPapayUserNotificationSendRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>EDP-B5：本批 DTO 全量形态 —— 16 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void EducationPapayAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(16, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            EducationPapayJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 EducationPapayJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("EducationPapay", type.Name + " 的 SerializerClassName 必须为 EducationPapay");
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

    /// <summary>EDP-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void EducationPapayRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("EducationPapay", "PayModule 须有 EducationPapay 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddEducationPapayWebApiHttpClient()", "注册委托必须指向源生成方法（组名 EducationPapay 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("EducationPapayJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["EducationPapayContractPresignRequest"] = new[] { "appid", "openid", "plan_id", "user_id", "period_start_date", "trade_scene", "sub_mchid", "sub_appid", "sub_openid" },
        ["EducationPapayTransactionCreateRequest"] = new[] { "appid", "body", "attach", "out_trade_no", "goods_tag", "notify_url", "contract_id", "trade_scene", "amount", "device_information", "sub_mchid", "sub_appid" },
        ["EducationPapayUserNotificationSendRequest"] = new[] { "appid", "sub_mchid", "sub_appid" },
        ["EducationPapayContractInfoResponse"] = new[] { "sp_mchid", "appid", "openid", "plan_id", "contract_information", "sub_mchid", "sub_appid", "sub_openid" },
        ["EducationPapayUserContractListResponse"] = new[] { "data", "total_count", "offset", "limit" },
        ["EducationPapayContractPresignResponse"] = new[] { "presign_token" },
        ["EducationPapayTransactionQueryResponse"] = new[] { "sp_mchid", "appid", "out_trade_no", "transaction_id", "attach", "bank_type", "success_time", "trade_state", "trade_state_description", "payer", "amount", "device_information", "promotion_detail", "sub_mchid", "sub_appid" },
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
