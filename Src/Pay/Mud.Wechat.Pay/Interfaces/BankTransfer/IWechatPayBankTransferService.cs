// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BankTransfer;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「从业机构（银行）批量转账到零钱 + 电子回单」域 SDK（<b>9 端点</b>：转账 5 + 电子回单 4）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（合作伙伴文档中心 ·「开发文档(银行/机构)」树，2026-10-14 逐页核验，
/// 各页「支持商户」栏恒为【从业机构（银行）】）：发起批量转账
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465889"/>、微信支付批次单号查询批次单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465891"/>、微信支付明细单号查询明细单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465897"/>、商家批次单号查询批次单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465903"/>、商家明细单号查询明细单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465915"/>；受理汇总电子回单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470746"/>、查询汇总电子回单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470761"/>、受理明细电子回单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470770"/>、查询明细电子回单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470775"/>。</para>
/// <para><b>两套路由前缀同域共存</b>：转账族走 <c>/v3/bank-transfer/batches</c>，电子回单族走
/// <c>/v3/bank-batch-transfer/receipt/</c> ⇒ 同名不成立（守卫 BT-B1 逐端点比对 (方法, 路由)）。
/// <see cref="Mud.Wechat.Pay.Extensions.PayModule.PartnerTransfer"/>（服务商面）则是 <c>/v3/partner-transfer/batches</c> 单前缀。</para>
/// <para><b>本域商户标识是六层，而非服务商面的两层</b>：包体必填 <c>platform_mchid</c> /
/// <c>specialized_payment_mchid</c> / <c>sponsor_mchid</c> / <c>user_authorized_mchid</c> /
/// <c>user_authorized_appid</c>，应答另加 <c>bank_sp_mchid</c>。签名主体是<b>从业机构商户号</b>
/// ⇒ 宿主以从业机构商户配置进入作用域，五标识全为<b>报文参数</b>而非作用域。</para>
/// <para><b>字段名与服务商面同源不同形</b>（照抄必静默丢字段，守卫 BT-B3/BT-B4 逐域锁定）：
/// 本域 <c>total_count</c> / <c>success_count</c> / <c>fail_count</c> 对服务商面
/// <c>total_num</c> / <c>success_num</c> / <c>fail_num</c>；本域 <c>batch_state</c>（4 值）/
/// <c>detail_state</c>（4 值）对服务商面 <c>batch_status</c>（5 值）/ <c>detail_status</c>（6 值）；
/// 本域明细过滤 query 为 <c>detail_state</c>（3 值）对服务商面 <c>detail_status</c>（4 值）。
/// 本域<b>没有</b> <c>WAIT_PAY</c> / <c>INIT</c> 档（出资方直接扣款，无付款确认环节）。</para>
/// <para><b>批次上限三千笔</b>（服务商面一千）；金额单位一律<b>分</b>。</para>
/// <para><b>电子回单是「同一路径、POST 与 GET 各自成端点」</b>：受理走 POST、查受理结果走 GET，
/// 路径完全相同、定位键同名（汇总支 <c>out_batch_no</c>、明细支 <c>+out_detail_no</c>）
/// ⇒ <b>端点计数须按 (方法, 路由) 二元组</b>，否则 9 支被数成 7 支（守卫 BT-B1）。</para>
/// <para><b>回单生成为异步</b>：受理成功只回 <c>receipt_state = ACCEPTED</c>，
/// <c>hash_type</c> / <c>hash_value</c> / <c>download_url</c> 官方注明仅在 <c>FINISHED</c> 时出现
/// ⇒ 受理后立刻读 URL 必为 <c>null</c>，须按状态轮询。<b><c>download_url</c> 不进生成式接口</b>
/// （动态路由 + 非 JSON，同账单下载通道形态，守卫 BT-B2 零声明断言带正对照）。</para>
/// <para><b>姓名密文两域同名不同必填</b>：本域明细 <c>user_name</c> <b>必填</b>（应答字段名为
/// <c>username</c>，亦必填）、<c>user_id_card</c> 选填；服务商面两者皆选填。
/// 敏感字段须 RSA 加密后传入（本 SDK 不做加解密，由 <c>Wechatpay-Serial</c> 承载证书序列号）。</para>
/// <para><b>限频</b>：发起批量转账 50QPS（官方标注）；超频报 <c>FREQUENCY_LIMITED</c>（HTTP 429）
/// ⇒ 降频后<b>原单</b>重试，不得更换 <c>out_batch_no</c>。</para>
/// <para><b>业务错误码</b>：<c>PARAM_ERROR</c> / <c>INVALID_REQUEST</c> / <c>SIGN_ERROR</c> /
/// <c>NO_AUTH</c>（官方两行不同文案，分别对应「权限未开通」与「出资方授权关系不符」）/
/// <c>NOT_ENOUGH</c> / <c>ACCOUNT_ERROR</c> / <c>QUOTA_EXCEED</c> / <c>FREQUENCY_LIMITED</c> /
/// <c>SYSTEM_ERROR</c>，原样透传至 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
/// <para><b>本章其余侧栏组不纳入本域</b>：来账识别、余额查询、余额提现、资金账单
/// （及服务商面的转账电子回单五支）⇒ 属后续增量，本域<b>零端点</b>（守卫 BT-B2 带正对照）。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse"/> 的
/// <c>Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BankTransfer", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBankTransferService
{
    /// <summary>
    /// 从业机构「发起批量转账」：一次提交一个转账批次（含多笔明细），返回微信批次号。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465889"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="BankTransferBatchCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>out_batch_no</c> / <c>batch_id</c> / <c>create_time</c> 的受理应答，见 <see cref="BankTransferBatchCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/bank-transfer/batches</c>；无 path / query 参数；限频 50QPS。</para>
    /// <para><b>五个商户 / 应用标识全为必填包体字段</b>（平台商户号、专项支付商户号、出资商户号、
    /// 被授权商户号与其 appid）：本域不提供任何默认值，错配即 <c>NO_AUTH</c>。
    /// 官方在权限说明里明确「<c>sponsor_mchid</c> 须与 <c>user_authorized_mchid</c> 建立授权关系」。</para>
    /// <para><b><c>transfer_detail_list</c> 官方标为选填</b>（与服务商面的必填相反）：
    /// 允许先建空批次、后续逐笔追加 ⇒ 传空列表不报 <c>PARAM_ERROR</c>，勿按服务商面经验校验。</para>
    /// <para><b><c>transfer_scene</c> / <c>transfer_purpose</c> 本面必填</b>（服务商面选填），
    /// 且 <c>PAYROLL_CARD_TRANSFER</c> 场景下明细 <c>user_name</c> 密文必填。</para>
    /// <para><b>受理 ≠ 转账成功</b>：应答不含状态，须以 <c>batch_id</c> 或 <c>out_batch_no</c> 轮询查单。</para>
    /// </remarks>
    [Post("/v3/bank-transfer/batches")]
    Task<BankTransferBatchCreateResponse> CreateBatchAsync(
        BankTransferBatchCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「微信支付批次单号查询批次单」：按微信批次号查批次状态与（可选）明细列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465891"/>。
    /// </summary>
    /// <param name="batchId">微信支付批次单号（官方 path <c>batch_id</c>，必填 string(64)）。</param>
    /// <param name="needQueryDetail">是否查询转账明细单（官方 query <c>need_query_detail</c>，<b>必填</b> boolean）。</param>
    /// <param name="offset">明细起始位置（官方 query <c>offset</c>，选填 integer，默认 0）。</param>
    /// <param name="limit">明细最大条数（官方 query <c>limit</c>，选填 integer，20~100，默认 20）。</param>
    /// <param name="detailState">明细状态过滤（官方 query <c>detail_state</c>，选填，3 值）：见 <see cref="BankTransferDetailStateFilters"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次单及其明细概要，见 <see cref="BankTransferBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/bank-transfer/batches/batch-id/{batch_id}</c>。</para>
    /// <para><b>过滤参数名是 <c>detail_state</c></b>（不是服务商面的 <c>detail_status</c>），
    /// 值集为 ALL / SUCCESS / FAIL <b>三档</b>（服务商面四档、含 <c>WAIT_PAY</c>）。</para>
    /// <para><b><c>need_query_detail</c> 官方为必填 query</b>：缺省即 400 <c>PARAM_ERROR</c>。</para>
    /// <para><b>明细列表只在批次 <c>FINISHED</c> 时返回</b>：处理中须改用两支明细查询端点。</para>
    /// </remarks>
    [Get("/v3/bank-transfer/batches/batch-id/{batchId}")]
    Task<BankTransferBatchQueryResponse> QueryBatchByBatchIdAsync(
        [Path] string batchId,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_state")] string? detailState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「商家批次单号查询批次单」：与 <see cref="QueryBatchByBatchIdAsync"/> 同表，以银行自有批次号定位。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465903"/>。
    /// </summary>
    /// <param name="outBatchNo">银行批次单号（官方 path <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="needQueryDetail">是否查询转账明细单（官方 query <c>need_query_detail</c>，<b>必填</b> boolean）。</param>
    /// <param name="offset">明细起始位置（官方 query <c>offset</c>，选填 integer，默认 0）。</param>
    /// <param name="limit">明细最大条数（官方 query <c>limit</c>，选填 integer，20~100，默认 20）。</param>
    /// <param name="detailState">明细状态过滤（官方 query <c>detail_state</c>，选填，3 值）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与微信单号支<b>同一应答形态</b>，见 <see cref="BankTransferBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/bank-transfer/batches/out-batch-no/{out_batch_no}</c>。</para>
    /// <para><b>两支共用应答类型</b>：官方两页字段表逐字段一致 ⇒ 按「表相同则共用」归并
    /// （守卫 BT-B6 断言两方法返回同一类型）。</para>
    /// </remarks>
    [Get("/v3/bank-transfer/batches/out-batch-no/{outBatchNo}")]
    Task<BankTransferBatchQueryResponse> QueryBatchByOutBatchNoAsync(
        [Path] string outBatchNo,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_state")] string? detailState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「微信支付明细单号查询明细单」：按微信明细号查单笔转账明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465897"/>。
    /// </summary>
    /// <param name="batchId">微信支付批次单号（官方 path <c>batch_id</c>，必填 string(64)）。</param>
    /// <param name="detailId">微信支付明细单号（官方 path <c>detail_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>单笔明细，见 <see cref="BankTransferDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/bank-transfer/batches/batch-id/{batch_id}/details/detail-id/{detail_id}</c>；无 query 参数。</para>
    /// <para><b>明细应答的 <c>username</c> 必填</b>（服务商面选填），且本支字段名是 <c>username</c>、
    /// 请求侧是 <c>user_name</c> —— 两处不同名。</para>
    /// <para><b><c>account_type</c> 本面必填、<c>initiate_time</c> 本面选填</b>：
    /// 与服务商面正好相反（那里 <c>account_type</c> 选填、<c>initiate_time</c> 必填）。</para>
    /// <para><b>官方本页未附 <c>account_type</c> 取值表</b> ⇒ 本域按
    /// <see cref="Mud.Wechat.Pay.Extensions.PayModule.PartnerTransfer"/>
    /// 的 WECHATPAY_ACCOUNT / BANK_ACCOUNT 语义使用，但<b>不得</b>据此做穷尽校验（守卫 BT-B4 只锁
    /// 有官方取值表的集合）。</para>
    /// <para><b>查不到 ≠ 失败</b>：官方说明可能是批次尚未开始处理，须隔几分钟重查且<b>不换单</b>。</para>
    /// </remarks>
    [Get("/v3/bank-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}")]
    Task<BankTransferDetailQueryResponse> QueryDetailByDetailIdAsync(
        [Path] string batchId,
        [Path] string detailId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「商家明细单号查询明细单」：与 <see cref="QueryDetailByDetailIdAsync"/> 同表，以银行自有明细号定位。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465915"/>。
    /// </summary>
    /// <param name="outBatchNo">银行批次单号（官方 path <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="outDetailNo">银行明细单号（官方 path <c>out_detail_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与微信单号支<b>同一应答形态</b>，见 <see cref="BankTransferDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/bank-transfer/batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}</c>。</para>
    /// <para><b>两层 path 定位</b>：须同时给批次号与明细号，单给明细号无法定位。</para>
    /// </remarks>
    [Get("/v3/bank-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}")]
    Task<BankTransferDetailQueryResponse> QueryDetailByOutDetailNoAsync(
        [Path] string outBatchNo,
        [Path] string outDetailNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「受理汇总电子回单」：提交批次汇总回单的申请（异步生成文件）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012470746"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="BankTransferSummaryReceiptRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单受理结果，见 <see cref="BankTransferSummaryReceiptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/bank-batch-transfer/receipt/summary-receipts</c>；
    /// 注意前缀是 <c>bank-batch-transfer</c>，与转账族的 <c>bank-transfer</c> <b>不同</b>。</para>
    /// <para><b>与 <see cref="QuerySummaryReceiptAsync"/> 同一路径</b>：本支 POST 发起、后者 GET 查询，
    /// 共用同一应答类型 ⇒ 计数按 (方法, 路由)（守卫 BT-B1）。</para>
    /// <para><b>仅已完成批次可受理</b>：官方要求 <c>out_batch_no</c> 指向<b>已完成</b>的批次，
    /// 处理中申请会失败。</para>
    /// </remarks>
    [Post("/v3/bank-batch-transfer/receipt/summary-receipts")]
    Task<BankTransferSummaryReceiptResponse> AcceptSummaryReceiptAsync(
        BankTransferSummaryReceiptRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「查询汇总电子回单受理结果」：与 <see cref="AcceptSummaryReceiptAsync"/> 同路径，按商家批次号定位。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012470761"/>。
    /// </summary>
    /// <param name="outBatchNo">银行批次单号（官方 query <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与受理支<b>同一应答形态</b>，见 <see cref="BankTransferSummaryReceiptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/bank-batch-transfer/receipt/summary-receipts?out_batch_no={out_batch_no}</c>。</para>
    /// <para><b>定位键从包体换成 query，参数名不变</b>（<c>out_batch_no</c>）。</para>
    /// <para><b><c>download_url</c> 只在 <c>receipt_state = FINISHED</c> 时出现</b>：
    /// 取到 URL 后的文件下载<b>不在本域</b>（动态路由 + 非 JSON，守卫 BT-B2）。</para>
    /// </remarks>
    [Get("/v3/bank-batch-transfer/receipt/summary-receipts")]
    Task<BankTransferSummaryReceiptResponse> QuerySummaryReceiptAsync(
        [Query("out_batch_no")] string outBatchNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「受理明细电子回单」：提交单笔转账明细的回单申请（异步生成文件）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012470770"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="BankTransferDetailReceiptRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单受理结果，见 <see cref="BankTransferDetailReceiptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/bank-batch-transfer/receipt/detail-receipts</c>。</para>
    /// <para><b>比汇总支多一个定位键</b>：<c>out_batch_no</c> + <c>out_detail_no</c> 皆必填。</para>
    /// <para><b>明细应答与汇总应答不共用类型</b>：本型多 <c>out_detail_no</c>、
    /// 但<b>没有</b> <c>create_time</c> / <c>update_time</c>（「表相同则共用，表不同则分建」，守卫 BT-B6）。</para>
    /// </remarks>
    [Post("/v3/bank-batch-transfer/receipt/detail-receipts")]
    Task<BankTransferDetailReceiptResponse> AcceptDetailReceiptAsync(
        BankTransferDetailReceiptRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构「查询明细电子回单受理结果」：与 <see cref="AcceptDetailReceiptAsync"/> 同路径，按商家批次号 + 明细号定位。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012470775"/>。
    /// </summary>
    /// <param name="outBatchNo">银行批次单号（官方 query <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="outDetailNo">银行明细单号（官方 query <c>out_detail_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与受理支<b>同一应答形态</b>，见 <see cref="BankTransferDetailReceiptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/bank-batch-transfer/receipt/detail-receipts?out_batch_no=…&amp;out_detail_no=…</c>。</para>
    /// <para><b>两支皆必填 query</b>：缺任一即 <c>PARAM_ERROR</c>（官方未提供默认值）。</para>
    /// </remarks>
    [Get("/v3/bank-batch-transfer/receipt/detail-receipts")]
    Task<BankTransferDetailReceiptResponse> QueryDetailReceiptAsync(
        [Query("out_batch_no")] string outBatchNo,
        [Query("out_detail_no")] string outDetailNo,
        CancellationToken cancellationToken = default);
}
