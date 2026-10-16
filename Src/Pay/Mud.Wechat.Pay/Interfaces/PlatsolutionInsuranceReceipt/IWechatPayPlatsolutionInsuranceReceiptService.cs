// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReceipt;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台商户保险理赔商家转账 · 电子回单（批次汇总 + 明细）」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b> ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>「平台商户」身份口径</b>：本域 13 页「支持商户」栏<b>恒为【平台商户】</b>（无【普通服务商】分身页），
/// 请求体与查询 query 统一以 <c>sub_mchid</c>（二级商户号，出资方）定位 ⇒ 平台商户身份由调用侧签名承载，本域<b>零</b> <c>sp_mchid</c> 请求参数。</para>
/// <para><b>四支四型、两两不同形</b>：汇总申请 3 行、汇总查询 7 行、明细申请 4 行、明细查询 9 行 ⇒ 申请与查询不同形（查询支才返回 <c>hash_type</c> / <c>hash_value</c> / <c>download_url</c>）， 汇总与明细亦不同形（明细支多 <c>out_detail_no</c> / <c>detail_id</c>）⇒ 四支各建一型，<b>不得</b>因「字段名大量重合」合并。</para>
/// <para><b>同名 <c>sub_mchid</c> 两支落位不同</b>：申请支是<b>请求体唯一字段</b>（定位键 <c>out_batch_no</c> 在 path）， 查询支是<b>必填 query</b> 且零请求体 ⇒ 申请与查询不得共用一个请求 DTO。</para>
/// <para><b>只支持查询最近 2 年内</b>的转账批次单 / 明细单（官方前置条件）；回单文件生成<b>结束后</b>查询支才带下载地址 ⇒ 拿到 <c>receipt_state</c> 非终态时须继续轮询而非报错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionInsuranceReceipt", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionInsuranceReceiptService
{
    /// <summary>
    /// 受理商家转账批次汇总电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504226"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="outBatchNo">【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">受理商家转账批次汇总电子回单请求体，字段见 <see cref="InsuranceSummaryReceiptApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理商家转账批次汇总电子回单，见 <see cref="InsuranceSummaryReceiptApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{out_batch_no}/apply</c>；path <c>out_batch_no</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="InsuranceSummaryReceiptApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{outBatchNo}/apply")]
    Task<InsuranceSummaryReceiptApplyResponse> ApplyInsuranceSummaryReceiptAsync(
        [Path] string outBatchNo,
        [Body] InsuranceSummaryReceiptApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商家转账批次汇总电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504227"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="outBatchNo">【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商家转账批次汇总电子回单，见 <see cref="InsuranceSummaryReceiptQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{out_batch_no}</c>；path <c>out_batch_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{outBatchNo}")]
    Task<InsuranceSummaryReceiptQueryResponse> QueryInsuranceSummaryReceiptAsync(
        [Path] string outBatchNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 受理商家转账明细电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504228"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="outBatchNo">【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="outDetailNo">【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">受理商家转账明细电子回单请求体，字段见 <see cref="InsuranceDetailReceiptApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理商家转账明细电子回单，见 <see cref="InsuranceDetailReceiptApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{out_batch_no}/out-detail-no/{out_detail_no}/apply</c>；path <c>out_batch_no</c> / <c>out_detail_no</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="InsuranceDetailReceiptApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{outBatchNo}/out-detail-no/{outDetailNo}/apply")]
    Task<InsuranceDetailReceiptApplyResponse> ApplyInsuranceDetailReceiptAsync(
        [Path] string outBatchNo,
        [Path] string outDetailNo,
        [Body] InsuranceDetailReceiptApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商家转账明细电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504229"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="outBatchNo">【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="outDetailNo">【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商家转账明细电子回单，见 <see cref="InsuranceDetailReceiptQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{out_batch_no}/out-detail-no/{out_detail_no}</c>；path <c>out_batch_no</c> / <c>out_detail_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{outBatchNo}/out-detail-no/{outDetailNo}")]
    Task<InsuranceDetailReceiptQueryResponse> QueryInsuranceDetailReceiptAsync(
        [Path] string outBatchNo,
        [Path] string outDetailNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);
}
