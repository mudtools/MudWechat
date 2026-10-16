// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Papay;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「Papay」契约守卫 —— 本批 <b>21 端点</b>（注册组 <c>Papay</c> + <c>PartnerPapay</c>，接口 <c>IWechatPayPapayService</c> + <c>IWechatPayPartnerPapayService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.Papay</c>）。
/// </summary>
/// <remarks>
/// <para><b>预扣费通知是扣款前置闸且幂等码属正常态</b>：两支 notify（直连 <c>/v3/papay/contracts/&#42;</c>、 服务商 <c>/v3/partner-papay/contracts/&#42;</c>）官方应答节原文「无参数返回」+ 正常示例「无数据(Http状态码为204)」 ⇒ 返回裸 <see cref="Task"/>，而 <c>RESOURCE_ALREADY_EXISTS</c>（已成功发送通知，无需重复调用）是<b>重入的正常态</b>。</para>
/// <para><b>服务商面官方只印一支</b>：委托代扣 3.0 的 <c>PartnerPapay</c> 组只有预扣费通知 ⇒ 签约 / 查询 / 扣款 各支<b>无服务商页</b>，不得按直连面镜像补齐（照页判据：缺页即缺端点，由 PPY-B1 的 21 方法数与 PPY-B2 的逐支路由名册锁定）。</para>
/// <para><b>请求类型不因同语义而跨身份共用</b>：服务商 notify 包体为 <c>sp_mchid</c> / <c>sp_appid</c> / <c>sub_mchid</c> / <c>sub_appid</c>（多子商户两级），直连页为 <c>mchid</c> / <c>appid</c> ⇒ <b>不同名即不同型</b>， 两支各建请求类型（先例：Ecommerce 进件 / 退款按身份分建，不做超集）。</para>
/// <para><b>两套协议定位键皆为 path 段</b>：签约查询与解除用 <c>plan_id</c> + <c>out_contract_code</c>， 预约扣费 / 续期 / 周期查询用 <c>contract_id</c> ⇒ 官方<b>无</b>按 <c>contract_id</c> 查通用协议的支， 两套键不得互换（守卫 PPY-EP 逐支锁定 PathKeys）。</para>
/// <para><b>共用只发生在应答侧</b>：签约查询 / 解除、预约扣费查询 / 受理、保险协议查询 / 解除、保险周期查询 / 受理 四对官方应答表<b>逐字段同形且必填口径一致</b> ⇒ 各共用一型；四端与保险三端预签约的<b>请求表字段集互异</b> （13 / 15 / 13 / 13 行、37 / 35 / 35 行）⇒ 请求侧逐端建型，只应答同形者共用（JSAPI 与 H5 皆单行 <c>redirect_url</c>）。</para>
/// <para><b>「受理」不等于「扣款成功」</b>：扣款与预约两支只返回受理结果，扣费结果由异步通知下发 ⇒ 应答类型只承载受理字段，不得据 HTTP 200 判定业务成功。</para>
/// <para><b>本域 21 支官方页全部未印端点级 QPS</b>，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数（逐支「未印」结论即取证结果）。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayPapayContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Papay";

    private static readonly Type[] Services = { typeof(IWechatPayPapayService), typeof(IWechatPayPartnerPapayService) };

    /// <summary>PPY-B1：拓扑 —— 注册组 Papay / PartnerPapay、本批 21 方法（接口实测 21）、21 条 (动词, 路由)；动词集 Post / Get。</summary>
    [Fact]
    public void PapayTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayPapayService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("Papay", "组名即源生成注册入口名（改组名即改 AddPapayWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        var svc1 = typeof(IWechatPayPartnerPapayService);
        ApiProp(svc1, "RegistryGroupName").Should().Be("PartnerPapay", "组名即源生成注册入口名（改组名即改 AddPartnerPapayWebApiHttpClient）");
        ApiProp(svc1, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc1.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc1.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc1.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(21, "Papay 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(21, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[]{ "PostAttribute", "GetAttribute" }, "动词集照官方页面逐支登记：本域写动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由为 APIv3 形态（官方文档树存在 v2 版式页，但请求 URL 仍印 /v3 前缀）");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayTransactionsService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>PPY-B2：本批 21 支端点的路由、path 键、query 名册、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void PapayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 预扣费通知｜官方页 v2/merchant/4011987402｜身份 【直连商户】
        RouteOf(nameof(IWechatPayPapayService.SendPapayDeductNotifyAsync)).Should().Be("/v3/papay/contracts/{contractId}/notify");
        PathKeysOf(nameof(IWechatPayPapayService.SendPapayDeductNotifyAsync)).Should().BeEquivalentTo(new[]{ "contractId" });
        QueryNamesOf(nameof(IWechatPayPapayService.SendPapayDeductNotifyAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.SendPapayDeductNotifyAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.SendPapayDeductNotifyAsync)).Should().Be(typeof(PapayContractNotifyRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.SendPapayDeductNotifyAsync)).Should().Be(typeof(Task), "预扣费通知官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 预扣费通知（服务商）｜官方页 v2/partner/4011988373｜身份 【服务商】
        RouteOf(nameof(IWechatPayPartnerPapayService.SendPartnerPapayDeductNotifyAsync)).Should().Be("/v3/partner-papay/contracts/{contractId}/notify");
        PathKeysOf(nameof(IWechatPayPartnerPapayService.SendPartnerPapayDeductNotifyAsync)).Should().BeEquivalentTo(new[]{ "contractId" });
        QueryNamesOf(nameof(IWechatPayPartnerPapayService.SendPartnerPapayDeductNotifyAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPartnerPapayService.SendPartnerPapayDeductNotifyAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPartnerPapayService.SendPartnerPapayDeductNotifyAsync)).Should().Be(typeof(PartnerPapayContractNotifyRequest));
        ReturnTypeOf(nameof(IWechatPayPartnerPapayService.SendPartnerPapayDeductNotifyAsync)).Should().Be(typeof(Task), "预扣费通知（服务商）官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 代扣签约协议查询｜官方页 v3/merchant/4012489245｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.QueryPapayContractAsync)).Should().Be("/v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}");
        PathKeysOf(nameof(IWechatPayPapayService.QueryPapayContractAsync)).Should().BeEquivalentTo(new[]{ "planId", "outContractCode" });
        QueryNamesOf(nameof(IWechatPayPapayService.QueryPapayContractAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.QueryPapayContractAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.QueryPapayContractAsync)).Should().BeNull("代扣签约协议查询官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPapayService.QueryPapayContractAsync)).Should().Be(typeof(PapayContractInfoResponse));

        // 代扣签约协议解除｜官方页 v3/merchant/4012489295｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.TerminatePapayContractAsync)).Should().Be("/v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate");
        PathKeysOf(nameof(IWechatPayPapayService.TerminatePapayContractAsync)).Should().BeEquivalentTo(new[]{ "planId", "outContractCode" });
        QueryNamesOf(nameof(IWechatPayPapayService.TerminatePapayContractAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.TerminatePapayContractAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.TerminatePapayContractAsync)).Should().Be(typeof(PapayContractTerminateRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.TerminatePapayContractAsync)).Should().Be(typeof(PapayContractInfoResponse));

        // 预约扣费结果｜官方页 v3/merchant/4012466997｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.QueryPapayDeductScheduleAsync)).Should().Be("/v3/papay/pay/schedules/contract-id/{contractId}");
        PathKeysOf(nameof(IWechatPayPapayService.QueryPapayDeductScheduleAsync)).Should().BeEquivalentTo(new[]{ "contractId" });
        QueryNamesOf(nameof(IWechatPayPapayService.QueryPapayDeductScheduleAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.QueryPapayDeductScheduleAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.QueryPapayDeductScheduleAsync)).Should().BeNull("预约扣费结果官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPapayService.QueryPapayDeductScheduleAsync)).Should().Be(typeof(PapayDeductScheduleInfoResponse));

        // 预约扣费受理｜官方页 v3/merchant/4012467036｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayDeductScheduleAsync)).Should().Be("/v3/papay/pay/schedules/contract-id/{contractId}/schedule");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayDeductScheduleAsync)).Should().BeEquivalentTo(new[]{ "contractId" });
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayDeductScheduleAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayDeductScheduleAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayDeductScheduleAsync)).Should().Be(typeof(PapayDeductScheduleCreateRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayDeductScheduleAsync)).Should().Be(typeof(PapayDeductScheduleInfoResponse));

        // 代扣受理扣款｜官方页 v3/merchant/4013388429｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.ApplyPapayHarmonyDeductAsync)).Should().Be("/v3/papay/pay/harmony/transactions/apply");
        PathKeysOf(nameof(IWechatPayPapayService.ApplyPapayHarmonyDeductAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.ApplyPapayHarmonyDeductAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.ApplyPapayHarmonyDeductAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.ApplyPapayHarmonyDeductAsync)).Should().Be(typeof(PapayHarmonyTransactionApplyRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.ApplyPapayHarmonyDeductAsync)).Should().Be(typeof(PapayHarmonyTransactionApplyResponse));

        // 预约扣费APP预签约｜官方页 v3/merchant/4012524934｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignAppAsync)).Should().Be("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/app");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignAppAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignAppAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignAppAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignAppAsync)).Should().Be(typeof(PapayScheduledPreEntrustSignAppRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignAppAsync)).Should().Be(typeof(PapayScheduledPreEntrustSignAppResponse));

        // 预约扣费H5预签约｜官方页 v3/merchant/4012489208｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignH5Async)).Should().Be("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignH5Async)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignH5Async)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignH5Async)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignH5Async)).Should().Be(typeof(PapayScheduledPreEntrustSignH5Request));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignH5Async)).Should().Be(typeof(PapayScheduledPreEntrustSignH5Response));

        // 预约扣费JSAPI预签约｜官方页 v3/merchant/4012525133｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignJsapiAsync)).Should().Be("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/jsapi");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignJsapiAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignJsapiAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignJsapiAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignJsapiAsync)).Should().Be(typeof(PapayScheduledPreEntrustSignJsapiRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignJsapiAsync)).Should().Be(typeof(PapayScheduledPreEntrustSignH5Response));

        // 预约扣费小程序预签约｜官方页 v3/merchant/4012525209｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignMiniProgramAsync)).Should().Be("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/mini-program");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignMiniProgramAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignMiniProgramAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignMiniProgramAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignMiniProgramAsync)).Should().Be(typeof(PapayScheduledPreEntrustSignMiniProgramRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayScheduledPreEntrustSignMiniProgramAsync)).Should().Be(typeof(PapayScheduledPreEntrustSignMiniProgramResponse));

        // 保险自动续费H5预签约｜官方页 v3/merchant/4012529050｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignH5Async)).Should().Be("/v3/papay/insurance-sign/contracts/pre-entrust-sign/h5");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignH5Async)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignH5Async)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignH5Async)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignH5Async)).Should().Be(typeof(PapayInsurancePreEntrustSignH5Request));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignH5Async)).Should().Be(typeof(PapayInsurancePreEntrustSignH5Response));

        // 保险自动续费JSAPI预签约｜官方页 v3/merchant/4012528600｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignJsapiAsync)).Should().Be("/v3/papay/insurance-sign/contracts/pre-entrust-sign/jsapi");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignJsapiAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignJsapiAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignJsapiAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignJsapiAsync)).Should().Be(typeof(PapayInsurancePreEntrustSignJsapiRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignJsapiAsync)).Should().Be(typeof(PapayInsurancePreEntrustSignH5Response));

        // 保险自动续费小程序预签约｜官方页 v3/merchant/4012528950｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignMiniProgramAsync)).Should().Be("/v3/papay/insurance-sign/contracts/pre-entrust-sign/mini-program");
        PathKeysOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignMiniProgramAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignMiniProgramAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignMiniProgramAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignMiniProgramAsync)).Should().Be(typeof(PapayInsurancePreEntrustSignMiniProgramRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.CreatePapayInsurancePreEntrustSignMiniProgramAsync)).Should().Be(typeof(PapayInsurancePreEntrustSignMiniProgramResponse));

        // 保险自动续费协议｜官方页 v3/merchant/4012472641｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.QueryPapayInsuranceContractAsync)).Should().Be("/v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}");
        PathKeysOf(nameof(IWechatPayPapayService.QueryPapayInsuranceContractAsync)).Should().BeEquivalentTo(new[]{ "planId", "outContractCode" });
        QueryNamesOf(nameof(IWechatPayPapayService.QueryPapayInsuranceContractAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.QueryPapayInsuranceContractAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.QueryPapayInsuranceContractAsync)).Should().BeNull("保险自动续费协议官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPapayService.QueryPapayInsuranceContractAsync)).Should().Be(typeof(PapayInsuranceContractInfoResponse));

        // 保险自动续费协议解除｜官方页 v3/merchant/4012472767｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.TerminatePapayInsuranceContractAsync)).Should().Be("/v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate");
        PathKeysOf(nameof(IWechatPayPapayService.TerminatePapayInsuranceContractAsync)).Should().BeEquivalentTo(new[]{ "planId", "outContractCode" });
        QueryNamesOf(nameof(IWechatPayPapayService.TerminatePapayInsuranceContractAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.TerminatePapayInsuranceContractAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.TerminatePapayInsuranceContractAsync)).Should().Be(typeof(PapayInsuranceContractTerminateRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.TerminatePapayInsuranceContractAsync)).Should().Be(typeof(PapayInsuranceContractInfoResponse));

        // 续期自动续费协议受理｜官方页 v3/merchant/4012489588｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.RenewPapayInsuranceContractAsync)).Should().Be("/v3/papay/insurance-sign/contracts/plan-id/{planId}/contract-id/{contractId}/renew");
        PathKeysOf(nameof(IWechatPayPapayService.RenewPapayInsuranceContractAsync)).Should().BeEquivalentTo(new[]{ "planId", "contractId" });
        QueryNamesOf(nameof(IWechatPayPapayService.RenewPapayInsuranceContractAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.RenewPapayInsuranceContractAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.RenewPapayInsuranceContractAsync)).Should().Be(typeof(PapayInsuranceContractRenewRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.RenewPapayInsuranceContractAsync)).Should().Be(typeof(Task), "续期自动续费协议受理官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 保险自动续费预约｜官方页 v3/merchant/4012471229｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodAsync)).Should().Be("/v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}");
        PathKeysOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodAsync)).Should().BeEquivalentTo(new[]{ "contractId", "policyPeriodId" });
        QueryNamesOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodAsync)).Should().BeNull("保险自动续费预约官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodAsync)).Should().Be(typeof(PapayInsurancePolicyPeriodResponse));

        // 保险自动续费预约受理｜官方页 v3/merchant/4012471270｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.SchedulePapayInsurancePolicyPeriodAsync)).Should().Be("/v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}/schedule");
        PathKeysOf(nameof(IWechatPayPapayService.SchedulePapayInsurancePolicyPeriodAsync)).Should().BeEquivalentTo(new[]{ "contractId", "policyPeriodId" });
        QueryNamesOf(nameof(IWechatPayPapayService.SchedulePapayInsurancePolicyPeriodAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.SchedulePapayInsurancePolicyPeriodAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.SchedulePapayInsurancePolicyPeriodAsync)).Should().Be(typeof(PapayInsurancePolicyPeriodScheduleRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.SchedulePapayInsurancePolicyPeriodAsync)).Should().Be(typeof(PapayInsurancePolicyPeriodResponse));

        // 保险扣费周期列表｜官方页 v3/merchant/4012489530｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodsAsync)).Should().Be("/v3/papay/insurance-sign/policy_periods/plan-id/{planId}/out-contract-code/{outContractCode}");
        PathKeysOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodsAsync)).Should().BeEquivalentTo(new[]{ "planId", "outContractCode" });
        QueryNamesOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodsAsync)).Should().BeEquivalentTo(new[]{ "limit", "offset" });
        RequiredQueryOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodsAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodsAsync)).Should().BeNull("保险扣费周期列表官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayPapayService.QueryPapayInsurancePolicyPeriodsAsync)).Should().Be(typeof(PapayInsurancePolicyPeriodListResponse));

        // 保险自动续费扣款受理｜官方页 v3/merchant/4012471158｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPapayService.ApplyPapayInsuranceDeductAsync)).Should().Be("/v3/papay/insurance-pay/transactions/apply");
        PathKeysOf(nameof(IWechatPayPapayService.ApplyPapayInsuranceDeductAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayPapayService.ApplyPapayInsuranceDeductAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayPapayService.ApplyPapayInsuranceDeductAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPapayService.ApplyPapayInsuranceDeductAsync)).Should().Be(typeof(PapayInsuranceTransactionApplyRequest));
        ReturnTypeOf(nameof(IWechatPayPapayService.ApplyPapayInsuranceDeductAsync)).Should().Be(typeof(PapayInsuranceTransactionApplyResponse));
    }

    /// <summary>PPY-B3：官方字段表顶层名册（请求体与应答，逐字照页；嵌套子树另立类型）。</summary>
    [Fact]
    public void PapayDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<PapayContractNotifyRequest>().Should().BeEquivalentTo(new[]{ "mchid", "appid", "deduct_duration", "estimated_amount" }, "预扣费通知请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PartnerPapayContractNotifyRequest>().Should().BeEquivalentTo(new[]{ "sp_mchid", "sp_appid", "sub_mchid", "sub_appid", "deduct_duration", "estimated_amount" }, "预扣费通知（服务商）请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayContractTerminateRequest>().Should().BeEquivalentTo(new[]{ "contract_termination_remark" }, "代扣签约协议解除请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayDeductScheduleCreateRequest>().Should().BeEquivalentTo(new[]{ "appid", "schedule_amount" }, "预约扣费受理请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayHarmonyTransactionApplyRequest>().Should().BeEquivalentTo(new[]{ "appid", "out_trade_no", "description", "transaction_notify_url", "contract_id", "amount", "goods_tag", "attach", "harmony_account_id", "mcc" }, "代扣受理扣款请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayScheduledPreEntrustSignAppRequest>().Should().BeEquivalentTo(new[]{ "appid", "openid", "plan_id", "out_contract_code", "contract_display_account", "contract_notify_url", "out_user_code", "deduct_schedule" }, "预约扣费APP预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayScheduledPreEntrustSignH5Request>().Should().BeEquivalentTo(new[]{ "appid", "openid", "plan_id", "out_contract_code", "contract_display_account", "contract_notify_url", "out_user_code", "deduct_schedule", "jump_control" }, "预约扣费H5预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayScheduledPreEntrustSignJsapiRequest>().Should().BeEquivalentTo(new[]{ "appid", "openid", "plan_id", "out_contract_code", "contract_display_account", "contract_notify_url", "out_user_code", "deduct_schedule" }, "预约扣费JSAPI预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayScheduledPreEntrustSignMiniProgramRequest>().Should().BeEquivalentTo(new[]{ "appid", "openid", "plan_id", "out_contract_code", "contract_display_account", "contract_notify_url", "out_user_code", "deduct_schedule" }, "预约扣费小程序预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePreEntrustSignH5Request>().Should().BeEquivalentTo(new[]{ "appid", "openid", "plan_id", "out_contract_code", "insured_display_name", "contract_notify_url", "policy_start_date", "policy_end_date", "policy_periods", "amount", "out_trade_no", "description", "transaction_notify_url", "out_user_code", "goods_tag", "attach", "can_auto_insure", "can_auto_reinsure", "real_identity", "jump_control", "combined_deduct_period_count" }, "保险自动续费H5预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePreEntrustSignJsapiRequest>().Should().BeEquivalentTo(new[]{ "appid", "openid", "plan_id", "out_contract_code", "insured_display_name", "contract_notify_url", "policy_start_date", "policy_end_date", "policy_periods", "amount", "out_trade_no", "description", "transaction_notify_url", "out_user_code", "goods_tag", "attach", "can_auto_insure", "can_auto_reinsure", "real_identity", "combined_deduct_period_count" }, "保险自动续费JSAPI预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePreEntrustSignMiniProgramRequest>().Should().BeEquivalentTo(new[]{ "appid", "openid", "plan_id", "out_contract_code", "insured_display_name", "contract_notify_url", "policy_start_date", "policy_end_date", "policy_periods", "amount", "out_trade_no", "description", "transaction_notify_url", "out_user_code", "goods_tag", "attach", "can_auto_insure", "can_auto_reinsure", "real_identity", "combined_deduct_period_count" }, "保险自动续费小程序预签约请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsuranceContractTerminateRequest>().Should().BeEquivalentTo(new[]{ "contract_termination_remark" }, "保险自动续费协议解除请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsuranceContractRenewRequest>().Should().BeEquivalentTo(new[]{ "appid", "policy_periods" }, "续期自动续费协议受理请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePolicyPeriodScheduleRequest>().Should().BeEquivalentTo(new[]{ "appid", "scheduled_amount" }, "保险自动续费预约受理请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsuranceTransactionApplyRequest>().Should().BeEquivalentTo(new[]{ "appid", "out_trade_no", "description", "transaction_notify_url", "contract_id", "policy_period_id", "amount", "goods_tag", "attach" }, "保险自动续费扣款受理请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayContractInfoResponse>().Should().BeEquivalentTo(new[]{ "mchid", "contract_id", "appid", "plan_id", "out_contract_code", "contract_display_account", "contract_state", "contract_signed_time", "contract_expired_time", "openid", "contract_terminate_info", "out_user_code", "deduct_schedule" }, "代扣签约协议查询应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayDeductScheduleInfoResponse>().Should().BeEquivalentTo(new[]{ "schedule_state", "deduct_start_date", "deduct_end_date", "scheduled_amount", "deduct_amount", "deduct_date" }, "预约扣费结果应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayHarmonyTransactionApplyResponse>().Should().BeEquivalentTo(new[]{ "out_trade_no", "amount" }, "代扣受理扣款应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayScheduledPreEntrustSignAppResponse>().Should().BeEquivalentTo(new[]{ "pre_entrustweb_id", "miniprogram_username", "miniprogram_path" }, "预约扣费APP预签约应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayScheduledPreEntrustSignH5Response>().Should().BeEquivalentTo(new[]{ "redirect_url" }, "预约扣费H5预签约应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayScheduledPreEntrustSignMiniProgramResponse>().Should().BeEquivalentTo(new[]{ "pre_entrustweb_id", "redirect_appid", "redirect_path" }, "预约扣费小程序预签约应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePreEntrustSignH5Response>().Should().BeEquivalentTo(new[]{ "redirect_url" }, "保险自动续费H5预签约应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePreEntrustSignMiniProgramResponse>().Should().BeEquivalentTo(new[]{ "pre_entrustweb_id", "redirect_appid", "redirect_path" }, "保险自动续费小程序预签约应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsuranceContractInfoResponse>().Should().BeEquivalentTo(new[]{ "mchid", "contract_id", "appid", "plan_id", "out_contract_code", "insured_display_name", "contract_state", "contract_signed_time", "contract_expired_time", "contract_terminate_info", "openid", "out_user_code" }, "保险自动续费协议应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePolicyPeriodResponse>().Should().BeEquivalentTo(new[]{ "policy_period_id", "policy_period_state", "deduct_start_date", "deduct_end_date", "scheduled_amount", "deduct_amount", "deduct_date" }, "保险自动续费预约应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsurancePolicyPeriodListResponse>().Should().BeEquivalentTo(new[]{ "data", "limit", "offset", "total_count" }, "保险扣费周期列表应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<PapayInsuranceTransactionApplyResponse>().Should().BeEquivalentTo(new[]{ "out_trade_no", "amount" }, "保险自动续费扣款受理应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");
    }

    /// <summary>PPY-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void PapayDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayContractInfoResponse)).Should().BeTrue("PapayContractInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayDeductScheduleInfoResponse)).Should().BeTrue("PapayDeductScheduleInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayHarmonyTransactionApplyResponse)).Should().BeTrue("PapayHarmonyTransactionApplyResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayScheduledPreEntrustSignAppResponse)).Should().BeTrue("PapayScheduledPreEntrustSignAppResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayScheduledPreEntrustSignH5Response)).Should().BeTrue("PapayScheduledPreEntrustSignH5Response 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayScheduledPreEntrustSignMiniProgramResponse)).Should().BeTrue("PapayScheduledPreEntrustSignMiniProgramResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePreEntrustSignH5Response)).Should().BeTrue("PapayInsurancePreEntrustSignH5Response 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePreEntrustSignMiniProgramResponse)).Should().BeTrue("PapayInsurancePreEntrustSignMiniProgramResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsuranceContractInfoResponse)).Should().BeTrue("PapayInsuranceContractInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePolicyPeriodResponse)).Should().BeTrue("PapayInsurancePolicyPeriodResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePolicyPeriodListResponse)).Should().BeTrue("PapayInsurancePolicyPeriodListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsuranceTransactionApplyResponse)).Should().BeTrue("PapayInsuranceTransactionApplyResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayContractNotifyRequest)).Should().BeFalse("PapayContractNotifyRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PartnerPapayContractNotifyRequest)).Should().BeFalse("PartnerPapayContractNotifyRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayContractTerminateRequest)).Should().BeFalse("PapayContractTerminateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayDeductScheduleCreateRequest)).Should().BeFalse("PapayDeductScheduleCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayHarmonyTransactionApplyRequest)).Should().BeFalse("PapayHarmonyTransactionApplyRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayScheduledPreEntrustSignAppRequest)).Should().BeFalse("PapayScheduledPreEntrustSignAppRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayScheduledPreEntrustSignH5Request)).Should().BeFalse("PapayScheduledPreEntrustSignH5Request 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayScheduledPreEntrustSignJsapiRequest)).Should().BeFalse("PapayScheduledPreEntrustSignJsapiRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayScheduledPreEntrustSignMiniProgramRequest)).Should().BeFalse("PapayScheduledPreEntrustSignMiniProgramRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePreEntrustSignH5Request)).Should().BeFalse("PapayInsurancePreEntrustSignH5Request 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePreEntrustSignJsapiRequest)).Should().BeFalse("PapayInsurancePreEntrustSignJsapiRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePreEntrustSignMiniProgramRequest)).Should().BeFalse("PapayInsurancePreEntrustSignMiniProgramRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsuranceContractTerminateRequest)).Should().BeFalse("PapayInsuranceContractTerminateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsuranceContractRenewRequest)).Should().BeFalse("PapayInsuranceContractRenewRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsurancePolicyPeriodScheduleRequest)).Should().BeFalse("PapayInsurancePolicyPeriodScheduleRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PapayInsuranceTransactionApplyRequest)).Should().BeFalse("PapayInsuranceTransactionApplyRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>PPY-B5：本批 DTO 全量形态 —— 82 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void PapayAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(82, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            PapayJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 PapayJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("Papay", type.Name + " 的 SerializerClassName 必须为 Papay");
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

    /// <summary>PPY-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void PapayRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("Papay", "PayModule 须有 Papay 成员（漏挂即 AddModules 静默不注册）");
        modules.Should().Contain("PartnerPapay", "PayModule 须有 PartnerPapay 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPapayWebApiHttpClient()", "注册委托必须指向源生成方法（组名 Papay 唯一）");
        builderSource.Should().Contain("AddPartnerPapayWebApiHttpClient()", "注册委托必须指向源生成方法（组名 PartnerPapay 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PapayJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
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
