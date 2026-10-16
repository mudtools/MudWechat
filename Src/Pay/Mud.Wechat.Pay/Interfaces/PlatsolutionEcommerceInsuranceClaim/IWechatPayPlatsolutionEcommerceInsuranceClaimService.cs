// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceClaim;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 · 保险理赔出款」域 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b> ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>「平台服务商」身份口径</b>：本族官方各页「支持商户」栏为【平台商户】（保险理赔功能开通族另含【普通服务商】），
/// 请求体统一以 <c>sub_mchid</c> / <c>sponsor_mchid</c> 指向被操作的二级商户 ⇒ 出资方与被赔付方是两回事，字段不得互相顶替。</para>
/// <para><b>与 <see cref="IWechatPayPlatsolutionEcommerceCompensateService"/> 同路由族不同契约</b>： 本域 <c>/insurance-claim-bills</c> 的应答比赔付域多 <c>business_type</c> 等字段、撤销支<b>官方有请求体</b> （赔付域撤销无体）⇒ 两域 DTO <b>不得互套</b>。</para>
/// <para><b>两支查询应答逐字段同形</b>（19 行）⇒ 共用 <see cref="InsuranceClaimQueryResponse"/>； 两支查询的 <c>sub_mchid</c> 官方为<b>选填 query</b> ⇒ 给可空缺省。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerceInsuranceClaim", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceInsuranceClaimService
{
    /// <summary>
    /// 请求保险理赔。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504184"/>（【平台商户】，官方页更新于 2025.07.25）。
    /// </summary>
    /// <param name="request">请求保险理赔请求体，字段见 <see cref="InsuranceClaimCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>请求保险理赔，见 <see cref="InsuranceClaimCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>应答</b>见 <see cref="InsuranceClaimCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills")]
    Task<InsuranceClaimCreateResponse> CreateInsuranceClaimAsync(
        [Body] InsuranceClaimCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求保险理赔预下单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504185"/>（【平台商户】，官方页更新于 2025.07.25）。
    /// </summary>
    /// <param name="request">请求保险理赔预下单请求体，字段见 <see cref="InsuranceClaimPreTransferRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>请求保险理赔预下单，见 <see cref="InsuranceClaimPreTransferResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/pre-transfer</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>应答</b>见 <see cref="InsuranceClaimPreTransferResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/pre-transfer")]
    Task<InsuranceClaimPreTransferResponse> PreTransferInsuranceClaimAsync(
        [Body] InsuranceClaimPreTransferRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求撤销保险理赔。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504186"/>（【平台商户】，官方页更新于 2025.07.25）。
    /// </summary>
    /// <param name="outBillNo">【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">请求撤销保险理赔请求体，字段见 <see cref="InsuranceClaimCancelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>请求撤销保险理赔，见 <see cref="InsuranceClaimCancelResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{out_bill_no}/cancel</c>；path <c>out_bill_no</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="InsuranceClaimCancelResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{outBillNo}/cancel")]
    Task<InsuranceClaimCancelResponse> CancelInsuranceClaimAsync(
        [Path] string outBillNo,
        [Body] InsuranceClaimCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号查询保险理赔结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504187"/>（【平台商户】，官方页更新于 2025.07.25）。
    /// </summary>
    /// <param name="outBillNo">【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户单号查询保险理赔结果，见 <see cref="InsuranceClaimQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{out_bill_no}</c>；path <c>out_bill_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{outBillNo}")]
    Task<InsuranceClaimQueryResponse> QueryInsuranceClaimByOutBillNoAsync(
        [Path] string outBillNo,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信支付单号查询保险理赔结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504188"/>（【平台商户】，官方页更新于 2025.07.25）。
    /// </summary>
    /// <param name="billId">【微信支付单号】 微信支付单号（官方 path <c>bill_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付单号查询保险理赔结果，见 <see cref="InsuranceClaimQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/bill-id/{bill_id}</c>；path <c>bill_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/bill-id/{billId}")]
    Task<InsuranceClaimQueryResponse> QueryInsuranceClaimByBillIdAsync(
        [Path] string billId,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);
}
