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
/// ⑧ 获取电子回单（已核一条路由：商户单号查询电子回单
/// <c>GET /v3/fund-app/mch-transfer/elecsign/out-bill-no/{out_bill_no}</c>）。
/// </para>
/// <para>
/// <b>🔴 本域第一原则（官方原文）</b>：发起转账遇到错误码时<b>不得换单重试</b> ——
/// 必须先经『商户单号/微信单号查询转账单』确认原单结果，<b>明确为失败后</b>才可换号重试，
/// 否则有<b>重复转账的资金风险</b>。故本域<b>必须先补齐两个查询端点</b>才具备生产可用性；
/// 本批先落发起端点是**刻意的最小步**（见下条待办）。
/// </para>
/// <para>
/// <b>本批未覆盖</b>：撤销转账 · 商户单号查询转账单 · 微信单号查询转账单 · 获取电子回单。
/// 并且：各枚举字段（<c>state</c> / <c>transfer_scene_id</c> / <c>user_recv_perception</c> /
/// <c>user_recv_style.type</c>）的<b>值域本轮未核验</b> ⇒ SDK 不臆造常量。
/// <b>不要凭推断补路由或补取值</b>。
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
    /// <b>不等于</b>转账成功；终态须经查询端点或回调通知确认（<c>state</c> 值域本轮未核验）。
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
}
