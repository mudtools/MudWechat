// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceBatch;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台商户保险理赔商家转账 · 商家转账批次与明细（保险场景）」域 SDK（5 端点）。
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
/// <para><b>五支建四型</b>：批次两支查询（按 <c>out_batch_no</c> / 按 <c>batch_id</c>）应答<b>逐字段同形</b>（24 行， 含 <c>transfer_batch</c> 对象与 <c>data</c> 明细数组 + <c>offset</c> / <c>limit</c>）⇒ 共用 <see cref="InsuranceBatchQueryResponse"/>； 明细两支查询同形（17 行）⇒ 共用 <see cref="InsuranceBatchDetailQueryResponse"/>；受理支单建一型。</para>
/// <para><b>受理成功 ≠ 转账成功（官方原文）</b>：受理应答只回批次号与 <c>batch_state</c>，须走查询支取付款状态； 幂等标识 <c>out_batch_no</c>、有效性 5 年 ⇒ 不得把受理应答的 <c>batch_state</c> 当终态判定。</para>
/// <para><b>分页 query 只印在批次查询两支</b>：<c>need_query_detail</c>（必填）/ <c>offset</c> / <c>limit</c> / <c>detail_state</c>； 明细两支<b>零分页</b>且仅 <c>sub_mchid</c> 必填 query ⇒ 不得把批次查询的参数表套到明细查询上。</para>
/// <para><b>与 <see cref="IWechatPayTransferService"/> / <see cref="IWechatPayTransferToQqWalletService"/> 不同契约</b>： 本域是<b>【平台商户】保险场景</b>的 <c>/v3/platsolution/insurance/mch-transfer/*</c>，应答多 <c>sp_mchid</c> / <c>sp_appid</c> / <c>sub_appid</c> 三层标识、字段名用 <c>batch_id</c>（不是 <c>transfer_bill_no</c>）⇒ DTO 不得与商家转账各域互相套用。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionInsuranceBatch", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionInsuranceBatchService
{
    /// <summary>
    /// 受理商家转账。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504220"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="request">受理商家转账请求体，字段见 <see cref="InsuranceBatchApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理商家转账，见 <see cref="InsuranceBatchApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/insurance/mch-transfer/batches/apply</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="InsuranceBatchApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/insurance/mch-transfer/batches/apply")]
    Task<InsuranceBatchApplyResponse> ApplyInsuranceBatchAsync(
        [Body] InsuranceBatchApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户转账批次单号查询批次单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504222"/>（【平台商户】，官方页更新于 2025.03.11）。
    /// </summary>
    /// <param name="outBatchNo">【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="needQueryDetail">【是否查询转账明细单】 true-是；false-否，默认否。商户可选择是否查询指定状态的转账明细单，当转账批次单状态为“FINISHED”（已完成）时，才会返回满足条件的转账明细单（官方 query <c>need_query_detail</c>，<b>必填</b>，类型 <c>boolean</c>）。</param>
    /// <param name="offset">【请求资源起始位置】 该次请求资源（转账明细单）的起始位置，从0开始，默认值为0（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【最大资源条数】 该次请求可返回的最大资源（转账明细单）条数，最小20条，最大100条，不传则默认20条。不足20条按实际条数返回（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="detailState">【明细状态】 查询指定状态的转账明细单。。可选取值：`ALL` 全部。查询批次下所有明细单 / `WAIT_PAY` 待二级商户员工（转账验密人）确认, 符合免确认条件时, 系统会自动扭转为转账中 / `SUCCESS` 转账成功 / `FAIL` 需要确认失败原因后，再决定是否重新发起对该笔明细单的转账（并非整个转账批次单） / `BANK_REFUND` 银行返回转账成功后，拒绝入账，资金会返回商户账户。（官方 query <c>detail_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户转账批次单号查询批次单，见 <see cref="InsuranceBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{out_batch_no}</c>；path <c>out_batch_no</c>，query <c>sub_mchid</c> / <c>need_query_detail</c> / <c>offset</c> / <c>limit</c> / <c>detail_state</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 与 <c>need_query_detail</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}")]
    Task<InsuranceBatchQueryResponse> QueryInsuranceBatchByOutBatchNoAsync(
        [Path] string outBatchNo,
        [Query("sub_mchid")] string subMchid,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_state")] string? detailState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信支付转账批次单号查询批次单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504221"/>（【平台商户】，官方页更新于 2025.03.11）。
    /// </summary>
    /// <param name="batchId">【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方 path <c>batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="needQueryDetail">【是否查询转账明细单】 true-是；false-否，默认否。商户可选择是否查询指定状态的转账明细单，当转账批次单状态为“FINISHED”（已完成）时，才会返回满足条件的转账明细单（官方 query <c>need_query_detail</c>，<b>必填</b>，类型 <c>boolean</c>）。</param>
    /// <param name="offset">【请求资源起始位置】 该次请求资源的起始位置。返回的明细是按照设置的明细条数进行分页展示的，一次查询可能无法返回所有明细，我们使用该参数标识查询开始位置，默认值为0（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【最大资源条数】 该次请求可返回的最大明细条数，最小20条，最大100条，不传则默认20条。不足20条按实际条数返回（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="detailState">【明细状态】 查询指定状态的转账明细单。。可选取值：`ALL` 全部。查询批次下所有明细单 / `WAIT_PAY` 待二级商户员工（转账验密人）确认, 符合免确认条件时, 系统会自动扭转为转账中 / `SUCCESS` 转账成功 / `FAIL` 需要确认失败原因后，再决定是否重新发起对该笔明细单的转账（并非整个转账批次单） / `BANK_REFUND` 银行返回转账成功后，拒绝入账，资金会返回商户账户。（官方 query <c>detail_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付转账批次单号查询批次单，见 <see cref="InsuranceBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batch_id}</c>；path <c>batch_id</c>，query <c>sub_mchid</c> / <c>need_query_detail</c> / <c>offset</c> / <c>limit</c> / <c>detail_state</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 与 <c>need_query_detail</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}")]
    Task<InsuranceBatchQueryResponse> QueryInsuranceBatchByBatchIdAsync(
        [Path] string batchId,
        [Query("sub_mchid")] string subMchid,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_state")] string? detailState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户明细单号查询明细单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504224"/>（【平台商户】，官方页更新于 2025.03.11）。
    /// </summary>
    /// <param name="outBatchNo">【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="outDetailNo">【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户明细单号查询明细单，见 <see cref="InsuranceBatchDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}</c>；path <c>out_batch_no</c> / <c>out_detail_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}")]
    Task<InsuranceBatchDetailQueryResponse> QueryInsuranceBatchDetailByOutDetailNoAsync(
        [Path] string outBatchNo,
        [Path] string outDetailNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信支付转账明细单号查询明细单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504223"/>（【平台商户】，官方页更新于 2025.03.11）。
    /// </summary>
    /// <param name="batchId">【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方 path <c>batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="detailId">【商家转账明细单号】 微信商家转账系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path <c>detail_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付转账明细单号查询明细单，见 <see cref="InsuranceBatchDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batch_id}/details/detail-id/{detail_id}</c>；path <c>batch_id</c> / <c>detail_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}")]
    Task<InsuranceBatchDetailQueryResponse> QueryInsuranceBatchDetailByDetailIdAsync(
        [Path] string batchId,
        [Path] string detailId,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);
}
