// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 · 保证金赔付（售后 / 运费 / 保费）」域 SDK（7 端点）。
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
/// <para><b>一个注册组三笔子族</b>：售后赔付（下单 + 两路查询）、运费赔付（下单 + 商户单号查询）、保费赔付（下单 + 商户单号查询）⇒ 七支建六个响应类型：售后族两路查询<b>逐字段同形</b> ⇒ 共用一型，其余各建一型。<b>三笔子族的下单应答并非「互不同形」</b>：运费与保费两支字段集相同（官方表格仅行序不同）、售后支少一个 <c>jump_wxa_option</c> ⇒ 本域选择逐子族独立建型是隔离演进的设计决定，不是形态强制。</para>
/// <para><b>售后族两支查询应答逐字段同形</b>（21 行）⇒ 共用 <see cref="DepositAfterSalesQueryResponse"/>。</para>
/// <para><b>保费族的定位键是裸 path 段</b>：<c>.../deposit-insurance-premiums-compensation-bills/{out_bill_no}</c> （官方<b>没有</b> <c>out-bill-no/</c> 前缀，与售后/运费两支不同）⇒ <b>照官方原文，不得「顺手对齐」</b>。</para>
/// <para><b>保费查询支 <c>sub_mchid</c> 官方必填</b>，而售后/运费查询支同一参数<b>选填</b> ⇒ 同一族内必填口径也不统一，照页不得互套。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerceDeposit", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceDepositService
{
    /// <summary>
    /// 请求保证金售后赔付。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504194"/>（【平台商户】，官方页更新于 2025.03.21）。
    /// </summary>
    /// <param name="request">请求保证金售后赔付请求体，字段见 <see cref="DepositAfterSalesCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>请求保证金售后赔付，见 <see cref="DepositAfterSalesCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>应答</b>见 <see cref="DepositAfterSalesCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NOT_ENOUGH 资金不足 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills")]
    Task<DepositAfterSalesCreateResponse> CreateDepositAfterSalesCompensationAsync(
        [Body] DepositAfterSalesCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号查询保证金售后赔付结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504195"/>（【平台商户】，官方页更新于 2025.03.21）。
    /// </summary>
    /// <param name="outBillNo">【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户单号查询保证金售后赔付结果，见 <see cref="DepositAfterSalesQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/out-bill-no/{out_bill_no}</c>；path <c>out_bill_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 记录不存在 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/out-bill-no/{outBillNo}")]
    Task<DepositAfterSalesQueryResponse> QueryDepositAfterSalesCompensationByOutBillNoAsync(
        [Path] string outBillNo,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信支付单号查询保证金售后赔付结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504196"/>（【平台商户】，官方页更新于 2025.03.21）。
    /// </summary>
    /// <param name="billId">【微信支付单号】 微信支付单号（官方 path <c>bill_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付单号查询保证金售后赔付结果，见 <see cref="DepositAfterSalesQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/bill-id/{bill_id}</c>；path <c>bill_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 记录不存在 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/bill-id/{billId}")]
    Task<DepositAfterSalesQueryResponse> QueryDepositAfterSalesCompensationByBillIdAsync(
        [Path] string billId,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求保证金运费赔付。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504202"/>（【平台商户】，官方页更新于 2025.03.11）。
    /// </summary>
    /// <param name="request">请求保证金运费赔付请求体，字段见 <see cref="DepositShippingCostCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>请求保证金运费赔付，见 <see cref="DepositShippingCostCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>应答</b>见 <see cref="DepositShippingCostCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NOT_ENOUGH 资金不足 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills")]
    Task<DepositShippingCostCreateResponse> CreateDepositShippingCostCompensationAsync(
        [Body] DepositShippingCostCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号查询保证金运费赔付结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504203"/>（【平台商户】，官方页更新于 2025.03.11）。
    /// </summary>
    /// <param name="outBillNo">【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户单号查询保证金运费赔付结果，见 <see cref="DepositShippingCostQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills/out-bill-no/{out_bill_no}</c>；path <c>out_bill_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 记录不存在 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills/out-bill-no/{outBillNo}")]
    Task<DepositShippingCostQueryResponse> QueryDepositShippingCostCompensationByOutBillNoAsync(
        [Path] string outBillNo,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求保证金保费赔付。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010905"/>（【平台商户】，官方页更新于 2025.03.25）。
    /// </summary>
    /// <param name="request">请求保证金保费赔付请求体，字段见 <see cref="DepositInsurancePremiumsCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>请求保证金保费赔付，见 <see cref="DepositInsurancePremiumsCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>应答</b>见 <see cref="DepositInsurancePremiumsCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NOT_ENOUGH 资金不足 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills")]
    Task<DepositInsurancePremiumsCreateResponse> CreateDepositInsurancePremiumsCompensationAsync(
        [Body] DepositInsurancePremiumsCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号查询保证金保费赔付结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014010920"/>（【平台商户】，官方页更新于 2025.03.25）。
    /// </summary>
    /// <param name="outBillNo">【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户单号查询保证金保费赔付结果，见 <see cref="DepositInsurancePremiumsQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills/{out_bill_no}</c>；path <c>out_bill_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 记录不存在 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills/{outBillNo}")]
    Task<DepositInsurancePremiumsQueryResponse> QueryDepositInsurancePremiumsCompensationByOutBillNoAsync(
        [Path] string outBillNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);
}
