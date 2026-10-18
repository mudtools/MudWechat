// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Transfer;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商家转账」域 SDK（APIv3 商家转账，<b>首批 1 端点</b>：发起转账）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>
/// （发起转账，2026-10-09 逐字段核验；更新时间 2025.03.21）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>官方「商家转账」接口清单（共 8 项，照录自该页导航）</b>：① 发起转账 ✔本批已实现；
/// ② APP 调起用户确认收款（客户端 SDK）；③ JSAPI 调起用户确认收款（客户端 SDK）；
/// ④ 撤销转账；⑤ 商户单号查询转账单；⑥ 微信单号查询转账单；⑦ 商家转账回调通知（归回调包）；
/// ⑧ 获取电子回单（商户单号查询 / 微信单号查询，两条路由）。
/// </para>
/// <para>
/// <b>🔴 本域第一原则（官方原文）</b>：发起转账遇到错误码时<b>不得换单重试</b> ——
/// 必须先经『商户单号/微信单号查询转账单』确认原单结果，<b>明确为失败后</b>才可换号重试，
/// 否则有<b>重复转账的资金风险</b>。故本域<b>必须</b>具备两个查询端点才谈生产可用性。
/// </para>
/// <para>
/// <b>✅ 本域现已 19 端点</b>，分两代产品面：
/// <b>新版商家转账 10</b>（发起 · 两查询 · 撤销 · 两回单查询 · 两回单申请 · 免确认授权转账 · 授权后转账）、
/// <b>旧版批量转账 9</b>（发起批次 · 按批次单号/商户批次单号查批次 · 按明细单号/商户明细单号查明细 ·
/// 申请/查询批次回单 · 申请/查询明细回单）。
/// </para>
/// <para>
/// <b>⚠️ 两代面的状态值表不同</b>：旧版批次 <c>batch_status</c> 只有 4 值
/// （<see cref="TransferBatchStatuses"/>），新版转账单 <c>state</c> 有 8 值
/// （<see cref="TransferBillStates"/>）—— 不可互相套用、也不可把旧版批次号当新版转账单号查询。
/// </para>
/// <para>
/// <b>✅ 值表已补齐（原「值域未核验」留档已关闭）</b>：<c>state</c>（8 值，含终态与「可原单重试」语义）
/// → <see cref="TransferBillStates"/>；<c>user_recv_style.type</c>（2 值）→
/// <see cref="TransferRecvStyleTypes"/>；<c>user_recv_perception</c>（14 个<b>中文</b>取值）→
/// <see cref="TransferUserRecvPerceptions"/>；<c>transfer_scene_report_infos[].info_type</c>
/// （13 个中文取值）→ <see cref="TransferSceneReportInfoTypes"/>。
/// </para>
/// <para>
/// <b>⚠️ <c>transfer_scene_id</c> 是唯一例外，且这是官方事实</b>：官方<b>无</b>集中值表
/// （《产品介绍》原文只指向「商户平台 - 产品中心 - 商家转账 - 产品设置」查看），
/// 数值由各<b>场景页</b>分页给出 ⇒ <see cref="TransferSceneIds"/> 只收录已逐字确证的
/// 现金营销 <c>1000</c>，其余<b>不臆造</b>；<b>不要凭推断补路由或补取值</b>。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Transfer", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayTransferService
{
    /// <summary>
    /// 发起转账。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>。
    /// </summary>
    /// <param name="request">发起转账请求体，见 <see cref="TransferBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账单（含 <c>state</c> 与调起用户确认收款的 <c>package_info</c>），见 <see cref="TransferBillResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-bills</c>；无 path / query 参数。</para>
    /// <para>
    /// <b>🔴 调用方必须遵守（官方原文）</b>：遇到错误码时<b>不要换单重试</b> ——
    /// 先查清原单结果，明确失败后才换 <c>out_bill_no</c> 重试，否则有<b>重复转账资金风险</b>。
    /// </para>
    /// <para>
    /// <b>异步语义</b>：应答中的 <c>package_info</c> 只是<b>调起用户确认收款</b>的凭据，
    /// <b>不等于</b>转账成功；终态须经查询端点或回调通知确认（取值见 <see cref="TransferBillStates"/>，
    /// 其中只有 <c>SUCCESS</c> / <c>FAIL</c> / <c>CANCELLED</c> 是终态）。
    /// </para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/transfer-bills")]
    Task<TransferBillResponse> CreateTransferBillAsync(
        [Body] TransferBillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号查询转账单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716437"/>。
    /// </summary>
    /// <param name="outBillNo">商户转账单号（官方 path <c>out_bill_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账单详情（含 <c>state</c> 与 <c>fail_reason</c>），见 <see cref="TransferBillQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}</c>；
    /// 仅 path 参数，无 query / body。</para>
    /// <para>
    /// <b>🔴 本端点是发起转账红线的配套</b>：遇错后<b>先查单</b>；
    /// <c>state</c> 为 <c>ACCEPTED</c> / <c>PROCESSING</c> 时官方要求<b>原单重试</b>（<b>不要</b>换单），
    /// 只有 <c>FAIL</c> 终态才允许重新生成单据 —— 判定表见 <see cref="TransferBillStates"/>。
    /// </para>
    /// <para><b>时限</b>：官方产品介绍注明当前 API <b>仅支持查询 30 天内</b>的转账单。</para>
    /// </remarks>
    [Get("/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}")]
    Task<TransferBillQueryResponse> QueryByOutBillNoAsync(
        [Path] string outBillNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信单号查询转账单。官方文档：<see href="https://pay.weixin.qq.com/docs/merchant/apis/mch-trans/transfer-bill/get-transfer-bill-by-no.html"/>。
    /// </summary>
    /// <param name="transferBillNo">微信转账单号（官方 path <c>transfer_bill_no</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账单详情，见 <see cref="TransferBillQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/mch-transfer/transfer-bills/transfer-bill-no/{transfer_bill_no}</c>；
    /// 仅 path 参数，无 query / body。</para>
    /// <para>
    /// <b>返回类型为何与「商户单号查询转账单」共用</b>：官方两页的应答字段表<b>逐项完全一致</b>
    /// （12 个平铺字段，含 <c>state</c> 的 8 个取值亦相同），只是<b>入参维度</b>不同（微信单号 vs 商户单号）。
    /// 同一份事实两处类型只会让字段漂移 ⇒ 复用同一 DTO（与支付分「按协议号/按 openid 查授权记录」
    /// 共用应答同款判断：<b>表相同则共用，表不同则分建</b>）。
    /// </para>
    /// <para><b>时限</b>：官方产品介绍注明当前 API <b>仅支持查询 30 天内</b>的转账单。</para>
    /// </remarks>
    [Get("/v3/fund-app/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}")]
    Task<TransferBillQueryResponse> QueryByTransferBillNoAsync(
        [Path] string transferBillNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号查询电子回单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716436"/>。
    /// </summary>
    /// <param name="outBillNo">商户转账单号（官方 path <c>out_bill_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单申请单（含状态与（已完成时）摘要与下载地址），见 <see cref="TransferElecsignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/mch-transfer/elecsign/out-bill-no/{out_bill_no}</c>；
    /// 仅 path 参数。</para>
    /// <para><b>两段式</b>：回单是<b>异步生成</b>的 —— 首次调用常返回 <c>state = GENERATING</c>，
    /// <b>必须轮询</b>到 <c>FINISHED</c> 才有 <c>hash_value</c> / <c>download_url</c>。</para>
    /// <para><b>时限</b>：<c>download_url</c> <b>有效期仅 10 分钟</b>，过期须重新调用本接口；官方要求原样使用、勿拼接。</para>
    /// <para><b>完整性</b>：下载后须用 <c>hash_type</c>/<c>hash_value</c> 比对文件摘要（官方要求），勿只凭下载成功采信。</para>
    /// </remarks>
    [Get("/v3/fund-app/mch-transfer/elecsign/out-bill-no/{outBillNo}")]
    Task<TransferElecsignResponse> QueryElecsignByOutBillNoAsync(
        [Path] string outBillNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信单号查询电子回单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716455"/>。
    /// </summary>
    /// <param name="transferBillNo">微信转账单号（官方 path <c>transfer_bill_no</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单申请单，见 <see cref="TransferElecsignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/mch-transfer/elecsign/transfer-bill-no/{transfer_bill_no}</c>；
    /// 仅 path 参数（更新时间 2025.03.21）。</para>
    /// <para>
    /// <b>返回类型为何与「商户单号查询电子回单」共用</b>：官方两页的应答字段表<b>逐项完全一致</b>
    /// （<c>state</c> / <c>create_time</c> / <c>update_time</c> / <c>hash_type</c> / <c>hash_value</c> /
    /// <c>download_url</c>），只是<b>入参维度</b>不同（微信单号 vs 商户单号）⇒ 复用同一 DTO
    /// （与「两个查询转账单」共用应答同款判断：<b>表相同则共用，表不同则分建</b>）。
    /// </para>
    /// <para><b>两段式与时限同商户单号版</b>：须轮询到 <c>state = FINISHED</c>；<c>download_url</c> 有效期 10 分钟。</para>
    /// </remarks>
    [Get("/v3/fund-app/mch-transfer/elecsign/transfer-bill-no/{transferBillNo}")]
    Task<TransferElecsignResponse> QueryElecsignByTransferBillNoAsync(
        [Path] string transferBillNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销转账。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716458"/>。
    /// </summary>
    /// <param name="outBillNo">商户转账单号（官方 path <c>out_bill_no</c>，必填 string(32)，仅数字与大小写字母）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销后的单据标识与状态（<c>CANCELING</c> / <c>CANCELLED</c>），见 <see cref="TransferRevokeResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}/cancel</c>；
    /// path 必带 <c>out_bill_no</c>；<b>无请求体</b>（官方字段表未定义任何 body 字段）。</para>
    /// <para>
    /// <b>⚠️ 可撤销时机</b>：官方原文「商户通过转账接口发起付款后，在<b>用户确认收款之前</b>可以通过该接口撤销付款」
    /// —— 已确认收款或已完成的单据不能撤销。
    /// </para>
    /// <para>
    /// <b>异步语义（官方原文）</b>：返回成功<b>仅表示撤销请求已受理</b>，系统会异步处理退款等操作，
    /// <b>以最终查询单据返回状态为准</b> ⇒ 拿到 <c>CANCELING</c> 时<b>必须</b>继续经
    /// <see cref="QueryByOutBillNoAsync"/> 查到 <c>CANCELLED</c> 终态才算撤销完成。
    /// </para>
    /// <para>
    /// <b>路由对照（防串用）</b>：本接口是<b>普通商户</b>面的
    /// <c>/v3/fund-app/mch-transfer/transfer-bills/…</c>；<b>服务商</b>侧另有
    /// <c>/v3/fund-app/mch-transfer/<b>partner</b>/transfer-bills/…</c> 的对应文档，二者<b>不可互换</b>。
    /// </para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel")]
    Task<TransferRevokeResponse> RevokeTransferAsync(
        [Path] string outBillNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户单号申请电子回单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716452"/>。
    /// </summary>
    /// <param name="request">申请请求体，见 <see cref="TransferElecsignApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单申请单（含状态与创建时间），见 <see cref="TransferElecsignApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/elecsign/out-bill-no</c>；无 path / query 参数。</para>
    /// <para><b>两段式</b>：回单是<b>异步生成</b>的 —— 首次调用常返回 <c>state = GENERATING</c>，<b>必须轮询</b>到 <c>FINISHED</c> 才有下载地址。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/elecsign/out-bill-no")]
    Task<TransferElecsignApplyResponse> ApplyElecsignByOutBillNoAsync(
        [Body] TransferElecsignApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信单号申请电子回单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716456"/>。
    /// </summary>
    /// <param name="request">申请请求体，见 <see cref="TransferElecsignApplyByWxNoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单申请单，见 <see cref="TransferElecsignApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/elecsign/transfer-bill-no</c>；无 path / query 参数。</para>
    /// <para><b>返回类型为何与「商户单号申请电子回单」共用</b>：官方两页的应答字段表<b>逐项完全一致</b>（<c>state</c> / <c>create_time</c>）⇒ 复用同一 DTO。</para>
    /// <para><b>两段式同商户单号版</b>：须轮询到 <c>state = FINISHED</c>。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/elecsign/transfer-bill-no")]
    Task<TransferElecsignApplyResponse> ApplyElecsignByTransferBillNoAsync(
        [Body] TransferElecsignApplyByWxNoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起转账并完成免确认收款授权。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399293"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="TransferPreTransferWithAuthorizationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账单 + 授权包信息，见 <see cref="TransferPreTransferWithAuthorizationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-bills/pre-transfer-with-authorization</c>；无 path / query 参数。</para>
    /// <para><b>支持商户：【普通商户】</b>。频率限制：官方页<b>未印</b>端点级数字。</para>
    /// <para><b>异步语义</b>：应答中的 <c>package_info</c> 只是调起用户确认收款的凭据，<b>不等于</b>转账成功；终态须经查询端点确认。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/transfer-bills/pre-transfer-with-authorization")]
    Task<TransferPreTransferWithAuthorizationResponse> CreateTransferBillWithAuthorizationAsync(
        [Body] TransferPreTransferWithAuthorizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户授权后转账。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399371"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="TransferAuthorizedTransferRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账单结果，见 <see cref="TransferAuthorizedTransferResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-bills/transfer</c>；无 path / query 参数。</para>
    /// <para><b>支持商户：【普通商户】</b>。频率限制：官方页<b>未印</b>端点级数字。</para>
    /// <para><b>前置条件</b>：必须先经免确认收款授权接口获取 <c>authorization_id</c> 或 <c>out_authorization_no</c>（二选一必填）。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/transfer-bills/transfer")]
    Task<TransferAuthorizedTransferResponse> TransferByAuthorizationAsync(
        [Body] TransferAuthorizedTransferRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·发起批量转账。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458841"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="request">发起批次请求体，见 <see cref="TransferBatchCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次标识与初始状态（<c>batch_status</c>），见 <see cref="TransferBatchCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/transfer/batches</c>；无 path / query 参数。</para>
    /// <para><b>旧版产品面</b>：与新版 <c>/v3/fund-app/mch-transfer/transfer-bills</c> 不是同一接口；
    /// 批次状态只有 <c>ACCEPTED/PROCESSING/FINISHED/CLOSED</c> 4 值（<see cref="TransferBatchStatuses"/>）。</para>
    /// <para><b>应答不是终态</b>：须用批次查询端点轮询到 <c>FINISHED</c>/<c>CLOSED</c>，
    /// 并逐笔查询明细状态后才能做资金结论。</para>
    /// </remarks>
    [Post("/v3/transfer/batches")]
    Task<TransferBatchCreateResponse> CreateBatchTransferAsync(
        [Body] TransferBatchCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·按微信批次单号查询批次。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458868"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="batchId">微信批次单号（官方 path <c>batch_id</c>，必填 string(64)）。</param>
    /// <param name="needQueryDetail">是否返回明细（官方 query <c>need_query_detail</c>，<b>必填</b> bool）。</param>
    /// <param name="offset">分页偏移（官方 query <c>offset</c>，选填）：<paramref name="needQueryDetail"/> 为 <c>true</c> 时生效。</param>
    /// <param name="limit">分页大小（官方 query <c>limit</c>，选填）。</param>
    /// <param name="detailStatus">明细状态过滤（官方 query <c>detail_status</c>，选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次快照 + 明细索引列表，见 <see cref="TransferBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/transfer/batches/batch-id/{batch_id}?need_query_detail=…</c>。</para>
    /// <para><b><c>need_query_detail</c> 是必填 query</b>：传 <c>false</c> 时 <c>transfer_detail_list</c> 为空，
    /// 不能据此判定「批次无明细」。</para>
    /// </remarks>
    [Get("/v3/transfer/batches/batch-id/{batchId}")]
    Task<TransferBatchQueryResponse> QueryBatchByBatchIdAsync(
        [Path] string batchId,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_status")] string? detailStatus = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·按商户批次单号查询批次。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458877"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="outBatchNo">商户批次单号（官方 path <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="needQueryDetail">是否返回明细（官方 query <c>need_query_detail</c>，<b>必填</b> bool）。</param>
    /// <param name="offset">分页偏移（官方 query <c>offset</c>，选填）。</param>
    /// <param name="limit">分页大小（官方 query <c>limit</c>，选填）。</param>
    /// <param name="detailStatus">明细状态过滤（官方 query <c>detail_status</c>，选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次快照 + 明细索引列表，见 <see cref="TransferBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/transfer/batches/out-batch-no/{out_batch_no}?need_query_detail=…</c>；
    /// 应答表与按微信批次单号查询<b>逐项一致</b> ⇒ 共用同一响应类型。
    /// </remarks>
    [Get("/v3/transfer/batches/out-batch-no/{outBatchNo}")]
    Task<TransferBatchQueryResponse> QueryBatchByOutBatchNoAsync(
        [Path] string outBatchNo,
        [Query("need_query_detail")] bool needQueryDetail,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("detail_status")] string? detailStatus = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·按微信明细单号查询转账明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458886"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="batchId">微信批次单号（官方 path <c>batch_id</c>，必填 string(64)）。</param>
    /// <param name="detailId">微信明细单号（官方 path <c>detail_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>平铺明细表（含金额、收款人、失败原因），见 <see cref="TransferBatchDetailResponse"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/transfer/batches/batch-id/{batch_id}/details/detail-id/{detail_id}</c>；
    /// 两段 path 都必须带，缺一不可。
    /// </remarks>
    [Get("/v3/transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}")]
    Task<TransferBatchDetailResponse> QueryBatchDetailByDetailIdAsync(
        [Path] string batchId,
        [Path] string detailId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·按商户明细单号查询转账明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458902"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="outBatchNo">商户批次单号（官方 path <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="outDetailNo">商户明细单号（官方 path <c>out_detail_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>平铺明细表，见 <see cref="TransferBatchDetailResponse"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/transfer/batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}</c>；
    /// 应答表与按微信明细单号查询<b>逐项一致</b> ⇒ 共用同一响应类型。
    /// </remarks>
    [Get("/v3/transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}")]
    Task<TransferBatchDetailResponse> QueryBatchDetailByOutDetailNoAsync(
        [Path] string outBatchNo,
        [Path] string outDetailNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·申请批次转账电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458946"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="request">申请请求体（仅 <c>out_batch_no</c>），见 <see cref="TransferBillReceiptRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单申请单，见 <see cref="TransferBillReceiptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/transfer/bill-receipt</c>；无 path / query 参数。</para>
    /// <para><b>两段式</b>：申请后通常为 <c>ACCEPTED</c>，须轮询查询端点直到 <c>FINISHED</c>
    /// 才具备 <c>hash_value</c> / <c>download_url</c>。</para>
    /// </remarks>
    [Post("/v3/transfer/bill-receipt")]
    Task<TransferBillReceiptResponse> ApplyBillReceiptAsync(
        [Body] TransferBillReceiptRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·查询批次转账电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458950"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="outBatchNo">商户批次单号（官方 path <c>out_batch_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回单申请单，见 <see cref="TransferBillReceiptResponse"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/transfer/bill-receipt/{out_batch_no}</c>；
    /// 应答表与申请端点<b>逐项一致</b> ⇒ 共用同一响应类型。
    /// 下载地址不得自行拼接，且须用 <c>hash_value</c> 校验文件摘要。
    /// </remarks>
    [Get("/v3/transfer/bill-receipt/{outBatchNo}")]
    Task<TransferBillReceiptResponse> QueryBillReceiptAsync(
        [Path] string outBatchNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·申请转账明细电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458967"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="request">申请请求体，见 <see cref="TransferElectronicReceiptRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>明细回单申请单，见 <see cref="TransferElectronicReceiptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/transfer-detail/electronic-receipts</c>；无 path / query 参数。</para>
    /// <para><b><c>accept_type</c> 三值</b>见 <see cref="TransferReceiptAcceptTypes"/>；
    /// 不同取值的 <c>out_batch_no</c> 必填性不同，须按官方页面逐场景核对。</para>
    /// </remarks>
    [Post("/v3/transfer-detail/electronic-receipts")]
    Task<TransferElectronicReceiptResponse> ApplyElectronicReceiptAsync(
        [Body] TransferElectronicReceiptRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 旧版·查询转账明细电子回单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458972"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="acceptType">回单类型（官方 query <c>accept_type</c>，必填 string）：取值见 <see cref="TransferReceiptAcceptTypes"/>。</param>
    /// <param name="outBatchNo">商户批次单号（官方 query <c>out_batch_no</c>，条件必填 string(32)）。</param>
    /// <param name="outDetailNo">商户明细单号（官方 query <c>out_detail_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>明细回单申请单，见 <see cref="TransferElectronicReceiptResponse"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/transfer-detail/electronic-receipts?accept_type=…&amp;out_detail_no=…</c>；
    /// 应答表与申请端点<b>逐项一致</b> ⇒ 共用同一响应类型。
    /// </remarks>
    [Get("/v3/transfer-detail/electronic-receipts")]
    Task<TransferElectronicReceiptResponse> QueryElectronicReceiptAsync(
        [Query("accept_type")] string acceptType,
        [Query("out_batch_no")] string? outBatchNo,
        [Query("out_detail_no")] string outDetailNo,
        CancellationToken cancellationToken = default);
}
