// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台服务商解决方案 · 平台商户付款（商户到商户）」域 SDK（3 端点）。
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
/// <para><b>本域是「付款给商户」</b>（<c>receiver_detail.receiver.mch_info</c>），与赔付 / 理赔 / 保证金三族共用 <c>/v3/platsolution/ecommerce/mch-transfer/*</c> 前缀但应答字段集各异 ⇒ 类型不得跨族复用。</para>
/// <para><b>两支查询应答逐字段同形</b>（19 行）⇒ 共用 <see cref="MchTransferQueryResponse"/>； 按 <c>bill_id</c> 支官方<b>同时</b>给出 <c>sub_mchid</c> 与 <c>business_type</c> 两个选填 query， 按 <c>out_bill_no</c> 支只给 <c>business_type</c> ⇒ 照页逐支声明，不得合并成一套参数。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionEcommerceMchTransfer", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionEcommerceMchTransferService
{
    /// <summary>
    /// 请求商户付款。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504198"/>（【平台商户】，官方页更新于 2025.03.21）。
    /// </summary>
    /// <param name="request">请求商户付款请求体，字段见 <see cref="MchTransferCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>请求商户付款，见 <see cref="MchTransferCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/ecommerce/mch-transfer/transfer-bills</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>应答</b>见 <see cref="MchTransferCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/ecommerce/mch-transfer/transfer-bills")]
    Task<MchTransferCreateResponse> CreateMchTransferAsync(
        [Body] MchTransferCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号查询商户付款记录。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504199"/>（【平台商户】，官方页更新于 2025.03.21）。
    /// </summary>
    /// <param name="outBillNo">【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="businessType">【业务类型】 业务类型。可选取值：`DEPOSIT_COMPENSATION` 保证金赔付 / `PLATFORM_AFTER_SALES_COMPENSATION` 平台售后赔付 / `INSURANCE_CLAIM` 保险理赔 / `DEPOSIT_AFTER_SALES_COMPENSATION` 保证金售后赔付（用户）（官方 query <c>business_type</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户单号查询商户付款记录，见 <see cref="MchTransferQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}</c>；path <c>out_bill_no</c>，query <c>business_type</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}")]
    Task<MchTransferQueryResponse> QueryMchTransferByOutBillNoAsync(
        [Path] string outBillNo,
        [Query("business_type")] string? businessType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信支付单号查询商户付款记录。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504200"/>（【平台商户】，官方页更新于 2025.03.21）。
    /// </summary>
    /// <param name="billId">【微信支付单号】 微信支付单号（官方 path <c>bill_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="businessType">【业务类型】 业务类型。可选取值：`DEPOSIT_COMPENSATION` 保证金赔付 / `PLATFORM_AFTER_SALES_COMPENSATION` 平台售后赔付 / `INSURANCE_CLAIM` 保险理赔 / `DEPOSIT_AFTER_SALES_COMPENSATION` 保证金售后赔付（用户）（官方 query <c>business_type</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付单号查询商户付款记录，见 <see cref="MchTransferQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/ecommerce/mch-transfer/transfer-bills/bill-id/{bill_id}</c>；path <c>bill_id</c>，query <c>sub_mchid</c> / <c>business_type</c>。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率限制为100次/s</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/ecommerce/mch-transfer/transfer-bills/bill-id/{billId}")]
    Task<MchTransferQueryResponse> QueryMchTransferByBillIdAsync(
        [Path] string billId,
        [Query("sub_mchid")] string? subMchid = null,
        [Query("business_type")] string? businessType = null,
        CancellationToken cancellationToken = default);
}
