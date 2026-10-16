// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.RateActivity;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「优惠费率活动报名（从业机构面）」域 SDK（3 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 3 支中<b>3 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 3 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>三支官方全部只印服务商页，身份是【从业机构（银行）】+【从业机构（支付机构）】双标签</b> （正文「支持商户：【从业机构（银行）】 【从业机构（支付机构）】」）⇒ 两类从业机构共用契约， <b>不得</b>按银行/支付机构拆接口或补商户页。</para>
/// <para><b>报名与修改的应答同形 ⇒ 共用一型</b>：POST <c>/rate-activity/applications</c> 与 PATCH <c>/rate-activity/applications/id/{application_id}</c> 官方应答表均为单键 <c>application_id</c> <c>string(32)</c> 必填 ⇒ 共用 <see cref="RateActivityApplicationResponse"/>；GET 查状态支应答为 <c>application_state</c> / <c>sub_mchid</c> / <c>reject_parameter</c> / <c>reject_reason</c> / <c>pass_time</c> 五键 ⇒ <b>单建一型</b>。</para>
/// <para><b><c>sub_mchid</c> 两侧类型不同</b>：请求侧是 <c>string(32)</c>，<b>应答</b>侧官方印 <c>integer</c> ⇒ 「同名即同型」的直觉在本域会造成反序列化失败，请求与应答类型分别照页。</para>
/// <para><b>修改支的请求体全为选填</b>：<c>activity_detail_modification</c> 与 <c>additional_information</c> 官方均标选填， 「至少要给一个」由业务码 <c>INVALID_REQUEST</c>（参数符合格式但不符合业务规则）兜 ⇒ SDK <b>不做</b>空体拦截， 照页保持两支皆可空。</para>
/// <para><b>重复报名是业务码不是 HTTP 冲突</b>：官方码原文「<c>ALREADY_EXISTS</c>：商户提交过报名申请」（HTTP 400）⇒ 重试前先查状态支；<c>NO_AUTH</c>「商户信息不合法」与前置「已完成<b>开户意愿确认</b>的商户」同页印发。</para>
/// <para><b>频率限制</b>：三支官方页<b>全部未印</b>端点级数字，只在错误码表给 <c>429 FREQUENCY_LIMITED</c> ⇒ 不得补数。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "RateActivity", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayRateActivityService
{
    /// <summary>
    /// 报名费率优惠活动。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698774"/>（【从业机构（银行）】【从业机构（支付机构）】）。
    /// </summary>
    /// <param name="request">报名费率优惠活动请求体，字段见 <see cref="RateActivityApplicationCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>报名费率优惠活动，见 <see cref="RateActivityApplicationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/rate-activity/applications</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【从业机构（银行）】【从业机构（支付机构）】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RateActivityApplicationResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 ALREADY_EXISTS：商户提交过报名申请 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户信息不合法 / 404 NOT_FOUND：申请单不存在 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Post("/v3/rate-activity/applications")]
    Task<RateActivityApplicationResponse> CreateRateActivityApplicationAsync(
        [Body] RateActivityApplicationCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改单据信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698885"/>（【从业机构（银行）】【从业机构（支付机构）】）。
    /// </summary>
    /// <param name="applicationId">【申请单编号】微信支付分配的申请单号（官方 path <c>application_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">修改单据信息请求体，字段见 <see cref="RateActivityApplicationUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改单据信息，见 <see cref="RateActivityApplicationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/rate-activity/applications/id/{application_id}</c>；path <c>application_id</c>。</para>
    /// <para><b>支持商户</b>【从业机构（银行）】【从业机构（支付机构）】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="RateActivityApplicationResponse"/>（官方应答表逐字段建模，与 <c>POST /rate-activity/applications</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 ALREADY_EXISTS：商户提交过报名申请 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户信息不合法 / 404 NOT_FOUND：申请单不存在 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Patch("/v3/rate-activity/applications/id/{applicationId}")]
    Task<RateActivityApplicationResponse> UpdateRateActivityApplicationAsync(
        [Path] string applicationId,
        [Body] RateActivityApplicationUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据申请单ID查询申请单结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471722"/>（【从业机构（银行）】【从业机构（支付机构）】）。
    /// </summary>
    /// <param name="applicationId">【申请单编号】微信支付分配的申请单号.（官方 path <c>application_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>根据申请单ID查询申请单结果，见 <see cref="RateActivityApplicationStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/rate-activity/applications/id/{application_id}</c>；path <c>application_id</c>。</para>
    /// <para><b>支持商户</b>【从业机构（银行）】【从业机构（支付机构）】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 ALREADY_EXISTS：商户提交过报名申请 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户信息不合法 / 404 NOT_FOUND：申请单不存在 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Get("/v3/rate-activity/applications/id/{applicationId}")]
    Task<RateActivityApplicationStateResponse> QueryRateActivityApplicationStateAsync(
        [Path] string applicationId,
        CancellationToken cancellationToken = default);
}
