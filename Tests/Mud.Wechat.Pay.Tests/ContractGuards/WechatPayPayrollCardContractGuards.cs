// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.PayrollCard;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「PayrollCard」契约守卫 —— 本批 <b>6 端点</b>（注册组 <c>PayrollCard</c>，接口 <c>IWechatPayPayrollCardService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.PayrollCard</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中<b>6 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：1 / 6 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 6 支里<b>3 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>六支官方全部只印服务商页</b>（各页均标「支持商户：【普通服务商】」）⇒ 无直连面镜像、无双页归并问题。 <b>本域没有 <c>sp_mchid</c></b>：请求侧只有 <c>sub_mchid</c>（服务商身份由签名主体承载）， 而<b>应答</b>侧的服务商商户号字段名是 <c>mchid</c>；AppID 命名亦两套并存——令牌申领、授权关系与认证单三支用 <c>appid</c>（服务商）+ <c>sub_appid</c>，发起批次支用 <c>sp_appid</c> + <c>sub_appid</c> ⇒ 按「服务商面必是 sp_*」归一即错。</para>
/// <para><b>路由分四段、生命周期是两段式</b>：<c>/payroll-card/tokens</c>（申领授权令牌）→ <c>/payroll-card/authentications/pre-order-with-auth</c>（凭核身单号预下单取令牌）→ <c>/payroll-card/transfer-batches</c>（发起批次），外加 <c>/relations/{openid}</c> 与认证单两支查询。 「申领令牌」与「凭令牌发薪」不可折叠成一支，<c>/payroll-card/authentications</c> 的三支也各按定位键分列。</para>
/// <para><b>密文与必填口径逐页钉死，不得跨页套用</b>：<c>user_name</c> 与 <c>id_card_number</c> 在令牌支和预下单支 官方都标「按照APIV3标准加密该字段」且类型 <c>string(1024)</c>；但 <c>employment_type</c> 在预下单支是<b>必填</b>、 在令牌支<b>选填</b>（官方原文「未授权必填，授权过非必填」）⇒ 两个请求类型各建、口径照页不得归一。 本域证件号叫 <c>id_card_number</c>，进件域是 <c>identification_no</c> ⇒ 跨域「对齐字段名」即造出官方不存在的报文。</para>
/// <para><b>令牌申领应答与预下单应答不同形 ⇒ 各建一型</b>：前者 5 键（<c>openid</c> / <c>mchid</c> / <c>sub_mchid</c> / <c>token</c> / <c>expires_in</c>），后者多 <c>authenticate_number</c>（商家核身单号，即认证单查询支的定位键）⇒ 6 键。 按「字段高度重合」合并即造出官方不存在的契约。</para>
/// <para><b>认证单两支各建一应答</b>：按 <c>authenticate_number</c> 单查的应答与列表支 <c>data[]</c> 元素 官方字段表<b>逐字同形</b>（13 键同序），但本线口径是<b>共用只发生在应答根类型</b>、子树按「端点 + 路径段」 机械拼装独立成类型 ⇒ 列表元素类型不与单查应答共用；列表支另带 <c>total_count</c> / <c>offset</c> / <c>limit</c> 包装。</para>
/// <para><b>分页三键在「应答侧」必填、在「query 侧」选填</b>：列表支的 <c>total_count</c> / <c>offset</c> / <c>limit</c> 是官方<b>应答表</b>必填三键，而其请求 query 的 <c>offset</c> / <c>limit</c> 官方标<b>选填</b>，单查支根本没有这两键 ⇒ 同名字段分属两侧、两套口径不得互套，也不得把 query 侧做成必填。</para>
/// <para><b>本域唯一印端点级 QPS 的接口</b>：<c>POST /v3/payroll-card/transfer-batches</c> 官方原文 「单个服务商（发起批量转账接口）50QPS，超过限制会报 <c>FREQUENCY_LIMITED</c>」⇒ 已照页写入该方法 remarks； 其余五支官方未印数字，不得凭第三方 SDK 补数。</para>
/// <para><b>清单里两条 <c>/payroll-card/wesure/*</c>（insurance、token-validations）判非契约、零声明零建模</b>： 该路由族在 PayrollCard 六支现行页正文里<b>零命中</b>，且表内为 token-validations 引的官方页 4012461785 实为 <c>GET /v3/payroll-card/authentications/{authenticate_number}</c>（张冠李戴）⇒ 与 P0-9 微企付 「官方出处缺失」同一证据面，由零声明守卫断言其在 <c>Src/</c> 全源码零声明。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayPayrollCardContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PayrollCard";

    private static readonly Type[] Services = { typeof(IWechatPayPayrollCardService) };

    /// <summary>PRC-B1：拓扑 —— 注册组 PayrollCard、本批 6 方法（接口实测 6）、6 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void PayrollCardTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayPayrollCardService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("PayrollCard", "组名即源生成注册入口名（改组名即改 AddPayrollCardWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(6, "PayrollCard 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(6, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" }, "本域写支全 POST、读支全 GET，无 PATCH/DELETE ⇒ 发起批次与预下单都是 POST 到集合/动词段路由");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/payroll-card/ 前缀内，与 /v3/fund-app/mch-transfer/ 商家转账族零交集");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayPartnerTransferService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayPartnerTransferService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>PRC-B2：本批 6 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void PayrollCardEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 获取代发薪用户授权令牌｜官方页 v3/partner/4012461589｜身份 【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTokenAsync)).Should().Be("/v3/payroll-card/tokens");
        PathKeysOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTokenAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTokenAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTokenAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTokenAsync)).Should().Be(typeof(PayrollCardTokenRequest));
        ReturnTypeOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTokenAsync)).Should().Be(typeof(PayrollCardTokenResponse));

        // 查询用户代发薪授权与开通状态｜官方页 v3/partner/4012461728｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardRelationAsync)).Should().Be("/v3/payroll-card/relations/{openid}");
        PathKeysOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardRelationAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardRelationAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "appid", "sub_appid" });
        RequiredQueryOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardRelationAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardRelationAsync)).Should().BeNull("查询用户代发薪授权与开通状态官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardRelationAsync)).Should().Be(typeof(PayrollCardRelationResponse));

        // 凭授权单号预下单取令牌｜官方页 v3/partner/4012461761｜身份 【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardAuthenticationPreOrderAsync)).Should().Be("/v3/payroll-card/authentications/pre-order-with-auth");
        PathKeysOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardAuthenticationPreOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardAuthenticationPreOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardAuthenticationPreOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardAuthenticationPreOrderAsync)).Should().Be(typeof(PayrollCardAuthenticationPreOrderRequest));
        ReturnTypeOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardAuthenticationPreOrderAsync)).Should().Be(typeof(PayrollCardAuthenticationPreOrderResponse));

        // 查询代发薪授权单｜官方页 v3/partner/4012461785｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationAsync)).Should().Be("/v3/payroll-card/authentications/{authenticateNumber}");
        PathKeysOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationAsync)).Should().BeEquivalentTo(new[] { "authenticateNumber" });
        QueryNamesOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationAsync)).Should().BeNull("查询代发薪授权单官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationAsync)).Should().Be(typeof(PayrollCardAuthenticationInfoResponse));

        // 按日期分页查询代发薪授权单列表｜官方页 v3/partner/4012461820｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationsAsync)).Should().Be("/v3/payroll-card/authentications");
        PathKeysOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationsAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationsAsync)).Should().BeEquivalentTo(new[] { "openid", "sub_mchid", "authenticate_date", "appid", "sub_appid", "authenticate_state", "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationsAsync)).Should().BeEquivalentTo(new[] { "openid", "sub_mchid", "authenticate_date" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationsAsync)).Should().BeNull("按日期分页查询代发薪授权单列表官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPayrollCardService.QueryPayrollCardAuthenticationsAsync)).Should().Be(typeof(PayrollCardAuthenticationListResponse));

        // 发起代发薪批量转账｜官方页 v3/partner/4012461824｜身份 【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTransferBatchAsync)).Should().Be("/v3/payroll-card/transfer-batches");
        PathKeysOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTransferBatchAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTransferBatchAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTransferBatchAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTransferBatchAsync)).Should().Be(typeof(PayrollCardTransferBatchCreateRequest));
        ReturnTypeOf(nameof(IWechatPayPayrollCardService.CreatePayrollCardTransferBatchAsync)).Should().Be(typeof(PayrollCardTransferBatchCreateResponse));
    }

    /// <summary>PRC-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void PayrollCardDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<PayrollCardTokenRequest>().Should().BeEquivalentTo(NAMES["PayrollCardTokenRequest"], "PayrollCardTokenRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardAuthenticationPreOrderRequest>().Should().BeEquivalentTo(NAMES["PayrollCardAuthenticationPreOrderRequest"], "PayrollCardAuthenticationPreOrderRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardTransferBatchCreateRequest>().Should().BeEquivalentTo(NAMES["PayrollCardTransferBatchCreateRequest"], "PayrollCardTransferBatchCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardTokenResponse>().Should().BeEquivalentTo(NAMES["PayrollCardTokenResponse"], "PayrollCardTokenResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardRelationResponse>().Should().BeEquivalentTo(NAMES["PayrollCardRelationResponse"], "PayrollCardRelationResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardAuthenticationPreOrderResponse>().Should().BeEquivalentTo(NAMES["PayrollCardAuthenticationPreOrderResponse"], "PayrollCardAuthenticationPreOrderResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardAuthenticationInfoResponse>().Should().BeEquivalentTo(NAMES["PayrollCardAuthenticationInfoResponse"], "PayrollCardAuthenticationInfoResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardAuthenticationListResponse>().Should().BeEquivalentTo(NAMES["PayrollCardAuthenticationListResponse"], "PayrollCardAuthenticationListResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayrollCardTransferBatchCreateResponse>().Should().BeEquivalentTo(NAMES["PayrollCardTransferBatchCreateResponse"], "PayrollCardTransferBatchCreateResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>PRC-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void PayrollCardDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardTokenResponse)).Should().BeTrue("PayrollCardTokenResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardRelationResponse)).Should().BeTrue("PayrollCardRelationResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardAuthenticationPreOrderResponse)).Should().BeTrue("PayrollCardAuthenticationPreOrderResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardAuthenticationInfoResponse)).Should().BeTrue("PayrollCardAuthenticationInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardAuthenticationListResponse)).Should().BeTrue("PayrollCardAuthenticationListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardTransferBatchCreateResponse)).Should().BeTrue("PayrollCardTransferBatchCreateResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardTokenRequest)).Should().BeFalse("PayrollCardTokenRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardAuthenticationPreOrderRequest)).Should().BeFalse("PayrollCardAuthenticationPreOrderRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayrollCardTransferBatchCreateRequest)).Should().BeFalse("PayrollCardTransferBatchCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>PRC-B5：本批 DTO 全量形态 —— 11 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void PayrollCardAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(11, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            PayrollCardJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 PayrollCardJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("PayrollCard", type.Name + " 的 SerializerClassName 必须为 PayrollCard");
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

    /// <summary>PRC-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void PayrollCardRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("PayrollCard", "PayModule 须有 PayrollCard 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPayrollCardWebApiHttpClient()", "注册委托必须指向源生成方法（组名 PayrollCard 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PayrollCardJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["PayrollCardTokenRequest"] = new[] { "openid", "appid", "sub_mchid", "sub_appid", "user_name", "id_card_number", "employment_type" },
        ["PayrollCardAuthenticationPreOrderRequest"] = new[] { "openid", "appid", "sub_mchid", "sub_appid", "authenticate_number", "project_name", "employer_name", "user_name", "id_card_number", "employment_type", "authenticate_type" },
        ["PayrollCardTransferBatchCreateRequest"] = new[] { "sub_mchid", "sub_appid", "authorization_type", "out_batch_no", "batch_name", "batch_remark", "total_amount", "total_num", "transfer_detail_list", "sp_appid", "employment_type", "employment_scene" },
        ["PayrollCardTokenResponse"] = new[] { "openid", "mchid", "sub_mchid", "token", "expires_in" },
        ["PayrollCardRelationResponse"] = new[] { "openid", "mchid", "sub_mchid", "authorize_state", "authorize_time", "deauthorize_time", "register_state", "register_time", "close_time" },
        ["PayrollCardAuthenticationPreOrderResponse"] = new[] { "authenticate_number", "openid", "mchid", "sub_mchid", "token", "expires_in" },
        ["PayrollCardAuthenticationInfoResponse"] = new[] { "mchid", "sub_mchid", "openid", "authenticate_scene", "authenticate_source", "project_name", "employer_name", "authenticate_state", "authenticate_time", "authenticate_number", "authenticate_failed_reason", "authenticate_type" },
        ["PayrollCardAuthenticationListResponse"] = new[] { "data", "total_count", "offset", "limit" },
        ["PayrollCardTransferBatchCreateResponse"] = new[] { "out_batch_no", "batch_id", "create_time" },
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
