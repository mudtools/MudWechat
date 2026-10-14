// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PartnerTransfer;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「服务商批量转账到零钱」域 SDK（<b>5 端点</b>：发起批量转账 1 + 批次查询 2 + 明细查询 2）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（合作伙伴文档中心 ·「批量转账到零钱」，2026-10-14 逐页核验，
/// 各页「支持商户」栏恒为【普通服务商】）：
/// 发起批量转账 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465738"/>、
/// 微信支付批次单号查询批次单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465772"/>、
/// 微信支付明细单号查询明细单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465781"/>、
/// 商家批次单号查询批次单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465790"/>、
/// 商家明细单号查询明细单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465795"/>。</para>
/// <para><b>本域与 <see cref="Mud.Wechat.Pay.Extensions.PayModule.Transfer"/>（直连商家转账）是两套契约</b>：
/// 本域走 <c>/v3/partner-transfer/batches</c>、签名主体为<b>服务商商户号</b>、
/// 报文用 <c>sub_mchid</c> / <c>sub_appid</c> / <c>sp_appid</c>；直连面（<c>IWechatPayTransferService</c>）
/// 现行落位是<b>新版商家转账</b> <c>/v3/fund-app/mch-transfer/transfer-bills</c>（<b>按笔发起、无批次</b>）
/// ⇒ 两域路由与 DTO 均不互通，路由交叠由守卫 PTF-B2 与 <c>PAY-B1</c> 同时锁定（本域亦零 <c>[Token]</c>）。</para>
/// <para><b>批次单号一经发起不可更换</b>（官方硬约束）：批量转账<b>一旦发起不允许撤销</b>、
/// 未查到明确批次结果前<b>不得换 <c>out_batch_no</c> 重提</b>、报错时<b>不要更换单号重试</b>
/// —— 换单重提会造成重复出资。SDK 侧不生成也不改写 <c>out_batch_no</c>，由宿主全权持有。</para>
/// <para><b>异步受理 + 只可查近 30 天</b>：发起接口只回 <c>batch_id</c> 与受理时间，
/// 转账结果须经查单轮询；批次/明细单只支持查询最近 30 天，更早的须登录商户平台。
/// 「批次还在受理中」与「批次不存在」在官方语义上是<b>同一现象</b>（查不到 ≠ 失败），
/// 官方明令此时须隔几分钟重查或<b>用同一 <c>out_batch_no</c> 再次发起</b>，
/// 误判为受理失败的责任由商户自担。</para>
/// <para><b>计数与状态字段名是本域特有的坑</b>：本域用 <c>total_num</c> / <c>success_num</c> / <c>fail_num</c>
/// 与 <c>batch_status</c> / <c>detail_status</c>；<see cref="Mud.Wechat.Pay.Extensions.PayModule.BankTransfer"/> 同族能力
/// 用的是 <c>total_count</c> / <c>success_count</c> / <c>fail_count</c> 与 <c>batch_state</c> / <c>detail_state</c>
/// ⇒ 跨域复制字段名会静默丢字段（守卫 PTF-B3 逐域锁定 snake_case）。</para>
/// <para><b>明细应答的姓名字段是 <c>username</c>，明细请求的是 <c>user_name</c></b>：
/// 两处不同名，且值均为 RSA 密文（本 SDK 不解密）。</para>
/// <para><b>本域批次上限一千笔</b>（银行面三千）；金额单位一律<b>分</b>。</para>
/// <para><b>限频</b>：发起批量转账与两支查单各 <b>50QPS</b>（官方按接口分别标注），超频报
/// <c>FREQUENCY_LIMITED</c>（HTTP 429）⇒ 须降频<b>原单</b>重试。</para>
/// <para><b>业务错误码</b>：<c>PARAM_ERROR</c> / <c>INVALID_REQUEST</c> / <c>SIGN_ERROR</c> /
/// <c>APPID_MCHID_NOT_MATCH</c> / <c>NO_AUTH</c> / <c>NOT_ENOUGH</c> / <c>ACCOUNT_ERROR</c> /
/// <c>QUOTA_EXCEED</c> / <c>FREQUENCY_LIMITED</c> / <c>SYSTEM_ERROR</c>，
/// 原样透传至 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
/// <para><b>本章其余侧栏组不纳入本域</b>：转账电子回单五支（路由实测为 <c>/v3/transfer/bill-receipt</c>、
/// <c>/v3/transfer-detail/electronic-receipts</c>，<b>无 <c>partner-</c> 段</b>、与银行面
/// <c>/v3/bank-batch-transfer/receipt/</c> 不同族）、余额查询 / 提现 / 帐单 / 来账识别
/// ⇒ 属后续增量，本域<b>零端点</b>（守卫 PTF-B2 带正对照）。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；服务商作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c>
/// 选择，<c>sub_mchid</c> 是<b>报文参数</b>而非商户作用域。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PartnerTransfer", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPartnerTransferService
{
    /// <summary>
    /// 服务商「发起批量转账」：一次提交一个转账批次（含多笔明细），返回微信支付批次单号。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465738"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="PartnerTransferBatchCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>out_batch_no</c> / <c>batch_id</c> / <c>create_time</c> 的受理应答，见 <see cref="PartnerTransferBatchCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/partner-transfer/batches</c>；无 path / query 参数；限频 50QPS。</para>
    /// <para><b>敏感字段须先加密</b>：明细 <c>user_name</c> / <c>user_id_card</c> 传 RSA 密文，
    /// 请求头 <c>Wechatpay-Serial</c> 随之必填；未传入姓名时不得臆造明文。</para>
    /// <para><b>受理 ≠ 转账成功</b>：本应答不含状态，须以返回的 <c>batch_id</c> 轮询查单端点。</para>
    /// <para><b>报错后不得换单</b>：<c>SYSTEM_ERROR</c> / <c>FREQUENCY_LIMITED</c> 一律<b>原单</b>重试
    /// （官方原文：「请勿更换商家转账批次单号，请使用相同参数再次调用 API。否则可能造成资金损失」）。</para>
    /// </remarks>
    [Post("/v3/partner-transfer/batches")]
    Task<PartnerTransferBatchCreateResponse> CreateBatchAsync(
        PartnerTransferBatchCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商「微信支付批次单号查询批次单」：按微信批次号查批次状态与（可选）明细列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465772"/>。
    /// </summary>
    /// <param name="batchId">微信支付批次单号（官方 path <c>batch_id</c>，必填 string(64)）。</param>
    /// <param name="needQueryDetail">是否查询转账明细单（官方 query <c>need_query_detail</c>，<b>必填</b> boolean）。</param>
    /// <param name="offset">明细起始位置（官方 query <c>offset</c>，选填 integer，默认 0）。</param>
    /// <param name="limit">明细最大条数（官方 query <c>limit</c>，选填 integer，20~100，默认 20）。</param>
    /// <param name="detailStatus">明细状态过滤（官方 query <c>detail_status</c>，选填，4 值）：见 <see cref="PartnerTransferDetailStateFilters"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次单及其明细概要，见 <see cref="PartnerTransferBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/partner-transfer/batches/batch-id/{batch_id}</c>。</para>
    /// <para><b><c>need_query_detail</c> 官方为必填 query</b>（不是「不传即不查」的选填）
    /// ⇒ 声明式签名必须显式给出布尔值，缺省即 400 <c>PARAM_ERROR</c>。</para>
    /// <para><b>过滤参数名是 <c>detail_status</c></b>：银行面同名过滤叫 <c>detail_state</c>，值集也少一档
    /// （本域含 <c>WAIT_PAY</c>）。</para>
    /// <para><b>明细列表只在批次 <c>FINISHED</c> 时返回</b>：批次处理中须改用两支明细查询端点。</para>
    /// </remarks>
    [Get("/v3/partner-transfer/batches/batch-id/{batchId}")]
    Task<PartnerTransferBatchQueryResponse> QueryBatchByBatchIdAsync(
        [Path] string batchId,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_status")] string? detailStatus = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商「微信支付明细单号查询明细单」：按微信明细号查单笔转账明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465781"/>。
    /// </summary>
    /// <param name="batchId">微信支付批次单号（官方 path <c>batch_id</c>，必填 string(64)）。</param>
    /// <param name="detailId">微信支付明细单号（官方 path <c>detail_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>单笔明细，见 <see cref="PartnerTransferDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/partner-transfer/batches/batch-id/{batch_id}/details/detail-id/{detail_id}</c>；无 query 参数。</para>
    /// <para><b>明细查询无「是否查明细」开关</b>：与批次查询端点形态不同，本支直接回明细全字段。</para>
    /// <para><b>本应答没有 <c>sub_mchid</c></b>（批次应答有）：只有 <c>sp_mchid</c> + 选填 <c>appid</c>。</para>
    /// <para><b>时间字段是 <c>initiate_time</c></b> 而非 <c>create_time</c>，且 <c>username</c> 为密文。</para>
    /// <para><b>查不到 ≠ 失败</b>：官方说明可能是「转账还在处理中」或「批次未开始处理」，
    /// 须隔几分钟重查且<b>不换单</b>。</para>
    /// </remarks>
    [Get("/v3/partner-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}")]
    Task<PartnerTransferDetailQueryResponse> QueryDetailByDetailIdAsync(
        [Path] string batchId,
        [Path] string detailId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商「商家批次单号查询批次单」：与 <see cref="QueryBatchByBatchIdAsync"/> 同表，以商户自有批次号定位。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465790"/>。
    /// </summary>
    /// <param name="outBatchNo">商家批次单号（官方 path <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="needQueryDetail">是否查询转账明细单（官方 query <c>need_query_detail</c>，<b>必填</b> boolean）。</param>
    /// <param name="offset">明细起始位置（官方 query <c>offset</c>，选填 integer，默认 0）。</param>
    /// <param name="limit">明细最大条数（官方 query <c>limit</c>，选填 integer，20~100，默认 20）。</param>
    /// <param name="detailStatus">明细状态过滤（官方 query <c>detail_status</c>，选填，4 值）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与微信单号支<b>同一应答形态</b>，见 <see cref="PartnerTransferBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/partner-transfer/batches/out-batch-no/{out_batch_no}</c>。</para>
    /// <para><b>两支共用应答类型</b>：官方两页字段表逐字段一致 ⇒ 按「表相同则共用」归并
    /// （守卫 PTF-B6 断言两方法返回同一类型）。</para>
    /// <para><b>本支是「受理中 / 不存在」歧义的处置入口</b>：官方允许「用同一 <c>out_batch_no</c> 再次发起」，
    /// 故按商家单号查单比按微信单号更可靠（受理失败时 <c>batch_id</c> 可能从未落地）。</para>
    /// </remarks>
    [Get("/v3/partner-transfer/batches/out-batch-no/{outBatchNo}")]
    Task<PartnerTransferBatchQueryResponse> QueryBatchByOutBatchNoAsync(
        [Path] string outBatchNo,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_status")] string? detailStatus = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商「商家明细单号查询明细单」：与 <see cref="QueryDetailByDetailIdAsync"/> 同表，以商户自有明细号定位。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465795"/>。
    /// </summary>
    /// <param name="outBatchNo">商家批次单号（官方 path <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="outDetailNo">商家明细单号（官方 path <c>out_detail_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与微信单号支<b>同一应答形态</b>，见 <see cref="PartnerTransferDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/partner-transfer/batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}</c>。</para>
    /// <para><b>两层 path 定位</b>：本支须同时给批次号与明细号（官方路由形态），单给明细号无法定位。</para>
    /// </remarks>
    [Get("/v3/partner-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}")]
    Task<PartnerTransferDetailQueryResponse> QueryDetailByOutDetailNoAsync(
        [Path] string outBatchNo,
        [Path] string outDetailNo,
        CancellationToken cancellationToken = default);
}
