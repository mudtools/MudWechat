// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Papay;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「委托代扣（PAPay 3.0 直连商户面）」域 SDK（20 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 20 支中<b>20 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 20 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 20 支里<b>3 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>预扣费通知是扣款的前置闸</b>：<c>POST /v3/papay/contracts/{contractId}/notify</c> 成功后才进入扣费等待期 （官方原文「具有委托代扣扣费权限，且<b>另行开通了预扣费通知权限</b>的直连商户」）；正常应答为 <b>204 无内容</b> ⇒ 本方法返回裸 <see cref="Task"/>，而业务码 <c>RESOURCE_ALREADY_EXISTS</c>（已成功发送通知，无需重复调用） 是<b>幂等重入的正常态</b>，不得当失败处理。</para>
/// <para><b>两套协议定位键不得互换</b>：签约查询与解除走 <c>plan_id</c> + <c>out_contract_code</c>（<b>两支都是 path 段</b>）， 预约扣费与续期走 <c>contract_id</c>（签约成功后微信分配）⇒ 「商户侧协议号」与「微信协议号」分属两套， SDK 不做本地映射，由宿主自行持有。</para>
/// <para><b>三条产品线共存于一路由树</b>：<c>/papay/sign/*</c>（通用代扣签约）、<c>/papay/scheduled-deduct-sign/*</c> （预约扣费签约）、<c>/papay/insurance-sign/*</c> + <c>/papay/insurance-pay/*</c>（保险自动续费） ⇒ 应答与请求字段集互异，<b>不得跨产品线套用类型</b>。</para>
/// <para><b>四端预签约各建一型</b>：<c>.../pre-entrust-sign/{app,h5,jsapi,mini-program}</c> 与 <c>.../insurance-sign/contracts/pre-entrust-sign/{h5,jsapi,mini-program}</c> 的官方<b>请求表字段集不同</b> （APP 支 13 行、H5 支 15 行、JSAPI/小程序支 13 行；保险面 H5 支 37 行、JSAPI 与小程序支各 35 行）⇒ 请求侧<b>逐端建型</b>； 只有<b>应答同形</b>者共用（JSAPI 与 H5 同为 1 行 <c>pre_entrust_sign_id</c> ⇒ 共用；APP 与小程序各回 3 行、形态亦不同 ⇒ 单建）。</para>
/// <para><b>「受理成功」不等于「扣款成功」</b>：<c>/papay/pay/harmony/transactions/apply</c>、 <c>/papay/insurance-pay/transactions/apply</c> 与预约扣费两支官方均<b>只返回受理结果</b>， 扣费结果由「委托代扣支付结果通知」异步下发（亦可查订单）⇒ 不得据 HTTP 200 判定扣款成功。</para>
/// <para><b>续期支无应答包体</b>：<c>POST .../contract-id/{contract_id}/renew</c> 官方应答节原文为「无应答包体」 ⇒ 返回裸 <see cref="Task"/>；其重试口径官方明确要求<b>先收「签解约通知」或调扣费周期列表确认续期结果</b>后再重试 （<c>500 SYSTEM_ERROR</c> 的解决方案原文），盲目重试会产生重复周期。</para>
/// <para><b>扣费周期列表支的分页</b>：<c>GET /v3/papay/insurance-sign/policy_periods/.../out-contract-code/{out_contract_code}</c> 是本域唯一带 query 的支，<c>limit</c> / <c>offset</c> 官方均<b>选填</b>（与本仓营销面多支「必填分页」口径不同，照页不套）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Papay", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPapayService
{
    /// <summary>
    /// 预扣费通知。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987402"/>（【直连商户】）。
    /// </summary>
    /// <param name="contractId">签约成功后，微信返回的委托代扣协议id。 示例值：123124412412423431（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</param>
    /// <param name="request">预扣费通知请求体，字段见 <see cref="PapayContractNotifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/contracts/{contract_id}/notify</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【直连商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：SYSTEMERROR：接口返回错误 / CONTRACT_NOT_EXIST：签约协议不存在 / PARAM_ERROR：参数错误 / APPID_MCHID_NOT_MATCH：appid和mchid不匹配 / INVALID_REQUEST：无效请求 / RULELIMIT：该请求存在风险 / RESOURCE_ALREADY_EXISTS：资源已经存在。</para>
    /// </remarks>
    [Post("/v3/papay/contracts/{contractId}/notify")]
    Task SendPapayDeductNotifyAsync(
        [Path] string contractId,
        [Body] PapayContractNotifyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代扣签约协议查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489245"/>（【普通商户】）。
    /// </summary>
    /// <param name="planId">【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="outContractCode">【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>代扣签约协议查询，见 <see cref="PapayContractInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/papay/sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c>；path <c>plan_id</c> / <c>out_contract_code</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Get("/v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}")]
    Task<PapayContractInfoResponse> QueryPapayContractAsync(
        [Path] int planId,
        [Path] string outContractCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代扣签约协议解除。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489295"/>（【普通商户】）。
    /// </summary>
    /// <param name="planId">【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="outContractCode">【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">代扣签约协议解除请求体，字段见 <see cref="PapayContractTerminateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>代扣签约协议解除，见 <see cref="PapayContractInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}/terminate</c>；path <c>plan_id</c> / <c>out_contract_code</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayContractInfoResponse"/>（官方应答表逐字段建模，与 <c>GET /papay/sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate")]
    Task<PapayContractInfoResponse> TerminatePapayContractAsync(
        [Path] int planId,
        [Path] string outContractCode,
        [Body] PapayContractTerminateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预约扣费结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466997"/>（【普通商户】）。
    /// </summary>
    /// <param name="contractId">【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID。（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预约扣费结果，见 <see cref="PapayDeductScheduleInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/papay/pay/schedules/contract-id/{contract_id}</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Get("/v3/papay/pay/schedules/contract-id/{contractId}")]
    Task<PapayDeductScheduleInfoResponse> QueryPapayDeductScheduleAsync(
        [Path] string contractId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预约扣费受理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467036"/>（【普通商户】）。
    /// </summary>
    /// <param name="contractId">【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID。（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">预约扣费受理请求体，字段见 <see cref="PapayDeductScheduleCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预约扣费受理，见 <see cref="PapayDeductScheduleInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/pay/schedules/contract-id/{contract_id}/schedule</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayDeductScheduleInfoResponse"/>（官方应答表逐字段建模，与 <c>GET /papay/pay/schedules/contract-id/{contract_id}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/pay/schedules/contract-id/{contractId}/schedule")]
    Task<PapayDeductScheduleInfoResponse> CreatePapayDeductScheduleAsync(
        [Path] string contractId,
        [Body] PapayDeductScheduleCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代扣受理扣款。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013388429"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">代扣受理扣款请求体，字段见 <see cref="PapayHarmonyTransactionApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>代扣受理扣款，见 <see cref="PapayHarmonyTransactionApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/pay/harmony/transactions/apply</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayHarmonyTransactionApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// </remarks>
    [Post("/v3/papay/pay/harmony/transactions/apply")]
    Task<PapayHarmonyTransactionApplyResponse> ApplyPapayHarmonyDeductAsync(
        [Body] PapayHarmonyTransactionApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预约扣费APP预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524934"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">预约扣费APP预签约请求体，字段见 <see cref="PapayScheduledPreEntrustSignAppRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预约扣费APP预签约，见 <see cref="PapayScheduledPreEntrustSignAppResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/app</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayScheduledPreEntrustSignAppResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/app")]
    Task<PapayScheduledPreEntrustSignAppResponse> CreatePapayScheduledPreEntrustSignAppAsync(
        [Body] PapayScheduledPreEntrustSignAppRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预约扣费H5预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489208"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">预约扣费H5预签约请求体，字段见 <see cref="PapayScheduledPreEntrustSignH5Request"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预约扣费H5预签约，见 <see cref="PapayScheduledPreEntrustSignH5Response"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayScheduledPreEntrustSignH5Response"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5")]
    Task<PapayScheduledPreEntrustSignH5Response> CreatePapayScheduledPreEntrustSignH5Async(
        [Body] PapayScheduledPreEntrustSignH5Request request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预约扣费JSAPI预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525133"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">预约扣费JSAPI预签约请求体，字段见 <see cref="PapayScheduledPreEntrustSignJsapiRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预约扣费JSAPI预签约，见 <see cref="PapayScheduledPreEntrustSignH5Response"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/jsapi</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayScheduledPreEntrustSignH5Response"/>（官方应答表逐字段建模，与 <c>POST /papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/jsapi")]
    Task<PapayScheduledPreEntrustSignH5Response> CreatePapayScheduledPreEntrustSignJsapiAsync(
        [Body] PapayScheduledPreEntrustSignJsapiRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预约扣费小程序预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525209"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">预约扣费小程序预签约请求体，字段见 <see cref="PapayScheduledPreEntrustSignMiniProgramRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预约扣费小程序预签约，见 <see cref="PapayScheduledPreEntrustSignMiniProgramResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/mini-program</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayScheduledPreEntrustSignMiniProgramResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/mini-program")]
    Task<PapayScheduledPreEntrustSignMiniProgramResponse> CreatePapayScheduledPreEntrustSignMiniProgramAsync(
        [Body] PapayScheduledPreEntrustSignMiniProgramRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费H5预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">保险自动续费H5预签约请求体，字段见 <see cref="PapayInsurancePreEntrustSignH5Request"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费H5预签约，见 <see cref="PapayInsurancePreEntrustSignH5Response"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/insurance-sign/contracts/pre-entrust-sign/h5</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="PapayInsurancePreEntrustSignH5Response"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/insurance-sign/contracts/pre-entrust-sign/h5")]
    Task<PapayInsurancePreEntrustSignH5Response> CreatePapayInsurancePreEntrustSignH5Async(
        [Body] PapayInsurancePreEntrustSignH5Request request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费JSAPI预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">保险自动续费JSAPI预签约请求体，字段见 <see cref="PapayInsurancePreEntrustSignJsapiRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费JSAPI预签约，见 <see cref="PapayInsurancePreEntrustSignH5Response"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/insurance-sign/contracts/pre-entrust-sign/jsapi</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="PapayInsurancePreEntrustSignH5Response"/>（官方应答表逐字段建模，与 <c>POST /papay/insurance-sign/contracts/pre-entrust-sign/h5</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/insurance-sign/contracts/pre-entrust-sign/jsapi")]
    Task<PapayInsurancePreEntrustSignH5Response> CreatePapayInsurancePreEntrustSignJsapiAsync(
        [Body] PapayInsurancePreEntrustSignJsapiRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费小程序预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">保险自动续费小程序预签约请求体，字段见 <see cref="PapayInsurancePreEntrustSignMiniProgramRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费小程序预签约，见 <see cref="PapayInsurancePreEntrustSignMiniProgramResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/insurance-sign/contracts/pre-entrust-sign/mini-program</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="PapayInsurancePreEntrustSignMiniProgramResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/insurance-sign/contracts/pre-entrust-sign/mini-program")]
    Task<PapayInsurancePreEntrustSignMiniProgramResponse> CreatePapayInsurancePreEntrustSignMiniProgramAsync(
        [Body] PapayInsurancePreEntrustSignMiniProgramRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费协议。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012472641"/>（【普通商户】）。
    /// </summary>
    /// <param name="planId">【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="outContractCode">【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费协议，见 <see cref="PapayInsuranceContractInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/papay/insurance-sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c>；path <c>plan_id</c> / <c>out_contract_code</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Get("/v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}")]
    Task<PapayInsuranceContractInfoResponse> QueryPapayInsuranceContractAsync(
        [Path] int planId,
        [Path] string outContractCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费协议解除。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012472767"/>（【普通商户】）。
    /// </summary>
    /// <param name="planId">【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="outContractCode">【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">保险自动续费协议解除请求体，字段见 <see cref="PapayInsuranceContractTerminateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费协议解除，见 <see cref="PapayInsuranceContractInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/insurance-sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}/terminate</c>；path <c>plan_id</c> / <c>out_contract_code</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayInsuranceContractInfoResponse"/>（官方应答表逐字段建模，与 <c>GET /papay/insurance-sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate")]
    Task<PapayInsuranceContractInfoResponse> TerminatePapayInsuranceContractAsync(
        [Path] int planId,
        [Path] string outContractCode,
        [Body] PapayInsuranceContractTerminateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 续期自动续费协议受理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489588"/>（【普通商户】）。
    /// </summary>
    /// <param name="planId">【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="contractId">【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">续期自动续费协议受理请求体，字段见 <see cref="PapayInsuranceContractRenewRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/insurance-sign/contracts/plan-id/{plan_id}/contract-id/{contract_id}/renew</c>；path <c>plan_id</c> / <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 400 INVALID_REQUEST：协议处于挽留期，不支持进行除解约外其他操作 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/insurance-sign/contracts/plan-id/{planId}/contract-id/{contractId}/renew")]
    Task RenewPapayInsuranceContractAsync(
        [Path] int planId,
        [Path] string contractId,
        [Body] PapayInsuranceContractRenewRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费预约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471229"/>（【普通商户】）。
    /// </summary>
    /// <param name="contractId">【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID。（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="policyPeriodId">【保单的扣费周期编号】 需要查询预约状态的保单的扣费周期编号。由商户在进行委托代扣签约时传入。需要大于0的正整数，且符合相关的业务规则。（官方 path <c>policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费预约，见 <see cref="PapayInsurancePolicyPeriodResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/papay/insurance-pay/policy-periods/contract-id/{contract_id}/policy-period-id/{policy_period_id}</c>；path <c>contract_id</c> / <c>policy_period_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Get("/v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}")]
    Task<PapayInsurancePolicyPeriodResponse> QueryPapayInsurancePolicyPeriodAsync(
        [Path] string contractId,
        [Path] int policyPeriodId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费预约受理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471270"/>（【普通商户】）。
    /// </summary>
    /// <param name="contractId">【委托代扣协议ID】签约成功后，微信返回的委托代扣协议ID。（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="policyPeriodId">【保单的扣费周期编号】进行预约的扣费周期编号，由商户在进行委托代扣签约时传入。需要大于0的正整数，且符合相关的业务规则。（官方 path <c>policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="request">保险自动续费预约受理请求体，字段见 <see cref="PapayInsurancePolicyPeriodScheduleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费预约受理，见 <see cref="PapayInsurancePolicyPeriodResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/insurance-pay/policy-periods/contract-id/{contract_id}/policy-period-id/{policy_period_id}/schedule</c>；path <c>contract_id</c> / <c>policy_period_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayInsurancePolicyPeriodResponse"/>（官方应答表逐字段建模，与 <c>GET /papay/insurance-pay/policy-periods/contract-id/{contract_id}/policy-period-id/{policy_period_id}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 400 INVALID_REQUEST：协议处于挽留期，不支持进行除解约外其他操作 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}/schedule")]
    Task<PapayInsurancePolicyPeriodResponse> SchedulePapayInsurancePolicyPeriodAsync(
        [Path] string contractId,
        [Path] int policyPeriodId,
        [Body] PapayInsurancePolicyPeriodScheduleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险扣费周期列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489530"/>（【普通商户】）。
    /// </summary>
    /// <param name="planId">【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="outContractCode">【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="limit">【分页大小】 设置该次请求返回的最大扣费周期条数，范围[1,120]，默认值为20（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="offset">【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回。默认值为0（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险扣费周期列表，见 <see cref="PapayInsurancePolicyPeriodListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/papay/insurance-sign/policy_periods/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c>；path <c>plan_id</c> / <c>out_contract_code</c>，query <c>limit</c> / <c>offset</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Get("/v3/papay/insurance-sign/policy_periods/plan-id/{planId}/out-contract-code/{outContractCode}")]
    Task<PapayInsurancePolicyPeriodListResponse> QueryPapayInsurancePolicyPeriodsAsync(
        [Path] int planId,
        [Path] string outContractCode,
        [Query("limit")] int? limit = null,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保险自动续费扣款受理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471158"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">保险自动续费扣款受理请求体，字段见 <see cref="PapayInsuranceTransactionApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>保险自动续费扣款受理，见 <see cref="PapayInsuranceTransactionApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/papay/insurance-pay/transactions/apply</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PapayInsuranceTransactionApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：无效请求 / 400 INVALID_REQUEST：协议处于挽留期，不支持进行除解约外其他操作 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Post("/v3/papay/insurance-pay/transactions/apply")]
    Task<PapayInsuranceTransactionApplyResponse> ApplyPapayInsuranceDeductAsync(
        [Body] PapayInsuranceTransactionApplyRequest request,
        CancellationToken cancellationToken = default);
}
