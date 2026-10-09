// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PayScore;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「支付分」域 SDK（APIv3 支付分，<b>首批 3 端点</b>：创建 / 查询 / 取消服务订单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（普通商户文档中心，2026-10-09 逐页核验）：
/// 创建支付分订单 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587900"/>、
/// 查询支付分订单 <see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_15.shtml"/>、
/// 取消支付分订单 <see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_16.shtml"/>。
/// 三页均标注支持商户：<b>普通商户</b>。
/// </para>
/// <para>
/// <b>本域尚未覆盖的端点</b>（官方「支付分 → API 列表」中确有此页，本批未实现）：
/// 商户预授权（<c>POST /v3/payscore/permissions</c>）、查询用户授权记录（授权协议号 /
/// <c>GET /v3/payscore/permissions/authorization-code/{authorization_code}</c>）、
/// 创单结单合并（<c>POST /v3/payscore/serviceorder/direct-complete</c>）、
/// 完结服务订单、修改订单金额、订单收款、同步订单、查询用户授权状态、解除授权、
/// 以及确认订单 / 支付成功 / 授权变更 / 订单状态变更等回调。
/// <b>不要凭推断补路由</b>——须先按接口名核验其页面。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PayScore", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPayScoreService
{
    /// <summary>
    /// 创建支付分订单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587900"/>。
    /// </summary>
    /// <param name="request">创单请求体，见 <see cref="PayScoreServiceOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务订单（创单成功 <c>state = CREATED</c>，含拉起确认订单页的 <c>package</c>），见 <see cref="PayScoreServiceOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder</c>；无 path / query 参数。</para>
    /// <para><b>幂等</b>：官方原文「该接口<b>支持原参重入</b>，相同参数重复调用可以返回成功」——
    /// 但商户侧仍须以 <c>out_order_no</c> 做业务幂等（重入成功≠业务只发生一次）。</para>
    /// <para><b>红线</b>：<c>out_order_no</c> <b>不可</b>用作申请退款接口的 <c>out_trade_no</c>（官方原文）。</para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder")]
    Task<PayScoreServiceOrderResponse> CreateServiceOrderAsync(
        [Body] PayScoreServiceOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支付分订单。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_15.shtml"/>。
    /// </summary>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，必填 string(32)）。</param>
    /// <param name="appId">公众账号 ID（官方 query <c>appid</c>，必填 string(32)）。</param>
    /// <param name="outOrderNo">商户服务订单号（官方 query <c>out_order_no</c>，选填）：与 <paramref name="queryId"/> <b>二选一</b>。</param>
    /// <param name="queryId">微信侧查询凭据（官方 query <c>query_id</c>，选填 string(512)）：与 <paramref name="outOrderNo"/> <b>二选一</b>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务订单（含收款与优惠信息），见 <see cref="PayScoreServiceOrderQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/serviceorder?out_order_no=…&amp;service_id=…&amp;appid=…&amp;query_id=…</c>；
    /// 必填 <c>service_id</c> + <c>appid</c>；<c>out_order_no</c> 与 <c>query_id</c>
    /// <b>二者必填其一，不允许都填或都不填</b>（官方原文明示的互斥约束，本签名以可选参数表达，
    /// 互斥校验须由服务端返回的 <c>PARAM_ERROR</c> 或调用方前置校验承担）。</para>
    /// <para><b>状态机</b>：官方要求<b>参考「支付分订单状态流转图」</b>处理业务逻辑，不得自行推断流转。</para>
    /// </remarks>
    [Get("/v3/payscore/serviceorder")]
    Task<PayScoreServiceOrderQueryResponse> QueryServiceOrderAsync(
        [Query("service_id")] string serviceId,
        [Query("appid")] string appId,
        [Query("out_order_no")] string? outOrderNo = null,
        [Query("query_id")] string? queryId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消支付分订单。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_16.shtml"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">取消请求体，见 <see cref="PayScoreCancelOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>被取消的订单标识（仅 5 字段），见 <see cref="PayScoreCancelOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder/{out_order_no}/cancel</c>；
    /// path 必带 <c>out_order_no</c>，body 含 <c>appid</c>(必) / <c>service_id</c>(<b>选填</b>) / <c>reason</c>(必)。</para>
    /// <para><b>可取消状态</b>：<c>CREATED</c> 与 <c>DOING</c>（含已完结但收款状态为待支付 <c>USER_PAYING</c>）。</para>
    /// <para><b>appid 一致性</b>：官方两处强调「完结订单和取消订单需与创单传入的 appid 保持一致」。</para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder/{outOrderNo}/cancel")]
    Task<PayScoreCancelOrderResponse> CancelServiceOrderAsync(
        [Path] string outOrderNo,
        [Body] PayScoreCancelOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 完结支付分订单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587955"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(64)）。</param>
    /// <param name="request">完结请求体，见 <see cref="PayScoreCompleteOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>完结后的订单（<c>need_collection</c> 固定为 <c>true</c>），见 <see cref="PayScoreCompleteOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder/{out_order_no}/complete</c>；
    /// path 必带 <c>out_order_no</c>；body 的 <c>post_payments</c> 与 <c>total_amount</c> 均为<b>必填</b>。</para>
    /// <para>
    /// <b>与创单的必填性差异（官方原样）</b>：<c>post_payments</c> 在创单里是<b>选填</b>、在完结里是<b>必填</b>；
    /// <c>total_amount</c> 只在本接口出现（创单不含最终金额）。
    /// </para>
    /// <para>
    /// <b>金额上限</b>：官方注明 <c>total_amount</c> <b>受服务 ID 风险金额上限影响</b> ——
    /// 超限会被风控拒绝，不是本地校验能替的。
    /// </para>
    /// <para>
    /// <b>后续动作</b>：完结只代表金额已确认并进入待收款（<c>need_collection = true</c>），
    /// <b>不等于</b>已收到款；收款侧须另用「发起催收扣款」（<c>POST /v3/payscore/serviceorder/{out_order_no}/pay</c>）
    /// 或等用户自动扣款，并以查询接口/回调确认终态。
    /// </para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder/{outOrderNo}/complete")]
    Task<PayScoreCompleteOrderResponse> CompleteServiceOrderAsync(
        [Path] string outOrderNo,
        [Body] PayScoreCompleteOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改订单金额。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_17.shtml"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">修改请求体，见 <see cref="PayScoreModifyOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改后的订单（含收款信息），见 <see cref="PayScoreModifyOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder/{out_order_no}/modify</c>；
    /// path 必带 <c>out_order_no</c>；body 的 <c>post_payments</c> / <c>total_amount</c> / <c>reason</c> 均<b>必填</b>。</para>
    /// <para><b>必填性差异</b>：<c>post_payments</c> 在此为必填（创单选填）；<c>post_discounts[].name</c> 在此为选填（创单必填）。</para>
    /// <para><b>结构差异（勿按查询应答解析）</b>：本接口应答的 <c>collection</c> <b>内嵌</b>
    /// <c>promotion_detail</c> 与 <c>goods_detail</c>，而查询应答把它们放在顶层。</para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder/{outOrderNo}/modify")]
    Task<PayScoreModifyOrderResponse> ModifyServiceOrderAsync(
        [Path] string outOrderNo,
        [Body] PayScoreModifyOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起催收扣款。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_19.shtml"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">扣款请求体（仅 <c>appid</c> + <c>service_id</c>），见 <see cref="PayScoreCollectRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>被催收的订单标识（仅 4 字段），见 <see cref="PayScoreCollectResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder/{out_order_no}/pay</c>；
    /// path 必带 <c>out_order_no</c>；body 仅 <c>appid</c> / <c>service_id</c>（两者均必填）。</para>
    /// <para>
    /// <b>⚠️ 语义</b>：官方接口名是「<b>发起催收扣款</b>」—— 对已完结且待收款（<c>USER_PAYING</c>）
    /// 的订单<b>主动发起扣款</b>；<b>不等于</b>入账、也不等于查询。扣款结果须经查询接口或支付成功回调确认。
    /// </para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder/{outOrderNo}/pay")]
    Task<PayScoreCollectResponse> CollectServiceOrderAsync(
        [Path] string outOrderNo,
        [Body] PayScoreCollectRequest request,
        CancellationToken cancellationToken = default);
}
