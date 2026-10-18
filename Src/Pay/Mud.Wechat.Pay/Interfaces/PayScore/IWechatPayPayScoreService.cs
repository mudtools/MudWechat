// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Bill;
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
/// <b>✅ 本域现已 23 端点</b>，按功能族分组：
/// <b>服务订单 7</b>（创建 · 查询 · 取消 · 完结 · 修改金额 · 发起催收扣款 · 同步状态）、
/// <b>授权面 6</b>（商户预授权 · 查询授权记录（协议号）· 查询授权记录（OPENID）· 评估用户风险等级 ·
/// 解除授权（协议号）· 解除授权（OPENID）），
/// <b>消息提醒 2</b>（下发待处理消息 · 下发预扣费消息）、
/// <b>支付分计划 3</b>（创建 · 查询 · 终止）、
/// <b>签约计划 4</b>（创建用户签约计划 · 查询用户签约计划 · 解除用户签约计划 ·
/// 基于签约计划创建服务订单）、<b>商户账单 1</b>。
/// </para>
/// <para>
/// <b>⚠️ 唯一刻意不建模的是「创单结单合并」</b>
/// （<c>POST /v3/payscore/serviceorder/direct-complete</c>）：官方页明示
/// 「特别提醒：创单结单合并接口<b>暂未对外开放</b>」⇒ 按「未开放不建模」纪律暂缺
/// （守卫 <c>PY-B9</c> 固化该裁决，恢复前须先确认开通权限与真实路径）。
/// </para>
/// <para>
/// <b>回调面</b>：已覆盖确认订单 / 支付成功 / 授权成功 / 解除授权四类
/// （<c>PAYSCORE.USER_CONFIRM</c> / <c>USER_PAID</c> / <c>USER_OPEN_SERVICE</c> / <c>USER_CLOSE_SERVICE</c>，
/// 均<b>大写</b>前缀），归回调包。
/// <b>⚠️ 留档一处未核验</b>：「订单状态变更通知」是否存在<b>本轮未单独核实</b> ⇒
/// 勿按其它产品线的习惯臆造事件名。
/// </para>
/// <para>
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

    /// <summary>
    /// 商户预授权。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_2.shtml"/>。
    /// </summary>
    /// <param name="request">预授权请求体，见 <see cref="PayScorePermissionsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预授权令牌（唯一字段），见 <see cref="PayScorePermissionsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/permissions</c>；无 path / query 参数；
    /// body 必填 <c>service_id</c> / <c>appid</c> / <c>authorization_code</c>，<c>notify_url</c> 选填。</para>
    /// <para><b>产品线归属</b>：官方把本端点归入「微信支付分（<b>免确认模式</b>）」—— 免确认模式才有预授权流程。</para>
    /// </remarks>
    [Post("/v3/payscore/permissions")]
    Task<PayScorePermissionsResponse> PreAuthorizeAsync(
        [Body] PayScorePermissionsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户授权记录（授权协议号）。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_3.shtml"/>。
    /// </summary>
    /// <param name="authorizationCode">授权协议号（官方 path <c>authorization_code</c>，必填 string(32)）。</param>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权记录（含授权状态与授权/取消时间），见 <see cref="PayScoreAuthorizationRecordResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/permissions/authorization-code/{authorization_code}?service_id=…</c>；
    /// path 必带 <c>authorization_code</c>、query 必带 <c>service_id</c>。</para>
    /// <para><b>未完成项（诚实记录）</b>：<c>authorization_state</c> 的<b>值域</b>本轮未取得（核验只覆盖字段表）
    /// ⇒ SDK 不臆造枚举常量；使用方须以官方值表判定，后续增量将补该值表并加守卫。</para>
    /// </remarks>
    [Get("/v3/payscore/permissions/authorization-code/{authorizationCode}")]
    Task<PayScoreAuthorizationRecordResponse> QueryAuthorizationRecordAsync(
        [Path] string authorizationCode,
        [Query("service_id")] string serviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步订单状态。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_20.shtml"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">同步请求体，见 <see cref="PayScoreSyncOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>同步后的订单，见 <see cref="PayScoreSyncOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder/{out_order_no}/sync</c>；
    /// path 必带 <c>out_order_no</c>；body 必填 <c>appid</c> / <c>service_id</c> / <c>type</c> / <c>detail</c>
    /// （<c>detail.paid_time</c> 必填）。</para>
    /// <para><b><c>type</c> 取值</b>：官方只给 <c>Order_Paid</c>（收款场景固定值，<b>混合大小写</b>），
    /// 见 <see cref="PayScoreSyncOrderTypes"/>。</para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder/{outOrderNo}/sync")]
    Task<PayScoreSyncOrderResponse> SyncServiceOrderAsync(
        [Path] string outOrderNo,
        [Body] PayScoreSyncOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除用户授权（授权协议号）。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_4.shtml"/>。
    /// </summary>
    /// <param name="authorizationCode">授权协议号（官方 path <c>authorization_code</c>，必填 string(32)）。</param>
    /// <param name="request">解除请求体（<c>service_id</c> + <c>reason</c> 均必填），见 <see cref="PayScoreTerminateAuthorizationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/permissions/authorization-code/{authorization_code}/terminate</c>；
    /// path 必带 <c>authorization_code</c>，<b>无 query</b>；body 必填 <c>service_id</c> / <c>reason</c>。</para>
    /// <para>
    /// <b>返回类型为何是 <c>Task</c></b>：官方明确「<b>无应答包体</b>」，成功状态码 <b>204 No Content</b>
    /// ⇒ 与交易域关单（同为 204）同款处置：声明为无返回值，避免调用方去解一个不存在的包体。
    /// </para>
    /// </remarks>
    [Post("/v3/payscore/permissions/authorization-code/{authorizationCode}/terminate")]
    Task TerminateAuthorizationAsync(
        [Path] string authorizationCode,
        [Body] PayScoreTerminateAuthorizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户授权记录（OPENID）。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_5.shtml"/>。
    /// </summary>
    /// <param name="openId">用户标识（官方 path <c>openid</c>，必填 string(128)）。</param>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，必填 string(32)）。</param>
    /// <param name="appId">公众账号 ID（官方 query <c>appid</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权记录，见 <see cref="PayScoreAuthorizationRecordResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/permissions/openid/{openid}?service_id=…&amp;appid=…</c>；
    /// path 必带 <c>openid</c>，query 必填 <c>service_id</c> 与 <c>appid</c>（<b>两个都是必填</b>）。</para>
    /// <para>
    /// <b>返回类型为何与「授权协议号」版共用</b>：官方两页的应答字段表<b>逐项完全一致</b>
    /// （8 个平铺字段，含 <c>authorization_state</c>），只是<b>入参维度</b>不同（按协议号 vs 按 openid）。
    /// 同一份事实两处类型只会让字段漂移 —— 故复用同一 DTO（与「请求分账/查询分账结果」共用应答同款判断：
    /// <b>表相同则共用，表不同则分建</b>）。
    /// </para>
    /// </remarks>
    [Get("/v3/payscore/permissions/openid/{openId}")]
    Task<PayScoreAuthorizationRecordResponse> QueryAuthorizationRecordByOpenIdAsync(
        [Path] string openId,
        [Query("service_id")] string serviceId,
        [Query("appid")] string appId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取支付分商户账单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_29.shtml"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="billDate">账单日期（Query <c>bill_date</c>，必填 string(10)，格式 <c>YYYYMMDDHH</c>，仅支持三个月内）。</param>
    /// <param name="serviceId">支付分服务 ID（Query <c>service_id</c>，必填 string(32)）。</param>
    /// <param name="tarType">压缩类型（Query <c>tar_type</c>，必填 string）：仅 <c>GZIP</c>。</param>
    /// <param name="encryptionAlgorithm">加密算法（Query <c>encryption_algorithm</c>，必填 string）：固定 <c>AEAD_AES_256_GCM</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns><b>分片列表</b>（<c>download_bill_count</c> + <c>download_bill_list[]</c>），见 <see cref="BillDownloadInfoListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/merchant-bill</c>；必带 <c>Accept: application/json</c>。</para>
    /// <para><b>⚠️ 账单日期是小时粒度</b>：<c>bill_date</c> 格式为 <c>YYYYMMDDHH</c>（10 位，含小时），
    /// 与本线账单域的 <c>yyyy-MM-DD</c> 不同形，勿照搬。</para>
    /// <para><b>四个 query 全部必填</b>（<c>tar_type</c> 与 <c>encryption_algorithm</c> 也不例外）——
    /// 这是官方参数表的原文口径，与「压缩 / 算法通常选填」的直觉相反。</para>
    /// <para><b>应答是加密的分片列表</b>：每个 <c>download_url</c> 5 分钟内有效且只可下载一次；
    /// 分片须按 <c>bill_sequence</c> 升序、用 <c>encrypt_key</c> + <c>nonce</c> 逐片解密后合并，
    /// 并用 <c>hash_value</c>（SHA1）校验完整性。<c>encrypt_key</c> / <c>nonce</c> 不得写入日志、遥测或异常消息。</para>
    /// <para><b>应答 DTO 与账单域共用</b>：官方本页与「服务商查询子商户资金账单」的应答字段表逐项一致
    /// ⇒ 复用 <see cref="BillDownloadInfoListResponse"/>（同表共用、表不同才分建）。</para>
    /// </remarks>
    [Get("/v3/payscore/merchant-bill")]
    Task<BillDownloadInfoListResponse> GetMerchantBillAsync(
        [Query("bill_date")] string billDate,
        [Query("service_id")] string serviceId,
        [Query("tar_type")] string tarType,
        [Query("encryption_algorithm")] string encryptionAlgorithm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 评估用户风险等级。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012856611"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="request">评估请求体，见 <see cref="PayScoreRiskAssessRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权记录 + 用户风险分层，见 <see cref="PayScoreRiskAssessResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/permissions/assess-user-risk-level</c>；无 path / query 参数。</para>
    /// <para><b>定位用户的两个入参至少传一个</b>：<c>authorization_code</c> 或 <c>openid</c>
    /// （官方两者均标选填，但都缺失时无法评估）。</para>
    /// <para><b>风控结果跨版本不可比</b>：<c>user_risk_level</c> 须与同一 <c>risk_level_version</c> 下的服务额度配合使用。</para>
    /// </remarks>
    [Post("/v3/payscore/permissions/assess-user-risk-level")]
    Task<PayScoreRiskAssessResponse> AssessUserRiskLevelAsync(
        [Body] PayScoreRiskAssessRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除用户授权（OPENID）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012647413"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="openId">用户标识（官方 path <c>openid</c>，必填 string(128)）。</param>
    /// <param name="request">解除请求体（<c>service_id</c> / <c>appid</c> / <c>reason</c> 均必填），见 <see cref="PayScoreTerminateByOpenIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/permissions/openid/{openid}/terminate</c>；<b>无应答包体</b>（204）⇒ 返回 <see cref="Task"/>。</para>
    /// <para><b>与「按协议号解除」不同</b>：本支请求体<b>必填 <c>appid</c></b>，而协议号版没有 <c>appid</c> 字段。</para>
    /// </remarks>
    [Post("/v3/payscore/permissions/openid/{openId}/terminate")]
    Task TerminateAuthorizationByOpenIdAsync(
        [Path] string openId,
        [Body] PayScoreTerminateByOpenIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建支付分计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467232"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="request">创计划请求体，见 <see cref="PayScoreCreatePlanRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建后的计划（与查询/终止同一张计划表），见 <see cref="PayScorePlanResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/plan/payscore-plans</c>；无 path / query 参数。</para>
    /// <para><b>四条一致性校验</b>：扣费次数 = 明细条数；明细实付依序不递增；每条原价 ≥ 实付；
    /// 总原价/总实付 = 明细之和且总实付 ≠ 0。任一条不满足即 400。</para>
    /// <para><b>明细序号由微信侧生成</b>：不能自报 <c>plan_detail_no</c>，后续使用须按从 1 开始的顺序。</para>
    /// </remarks>
    [Post("/v3/payscore/plan/payscore-plans")]
    Task<PayScorePlanResponse> CreatePlanAsync(
        [Body] PayScoreCreatePlanRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支付分计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012470804"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="merchantPlanNo">商户侧计划号（官方 path <c>merchant_plan_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>计划详情，见 <see cref="PayScorePlanResponse"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/payscore/plan/payscore-plans/merchant-plan-no/{merchant_plan_no}</c>；
    /// 定位键是<b>商户侧计划号</b>而非 <c>plan_id</c>，且无 query 参数。
    /// </remarks>
    [Get("/v3/payscore/plan/payscore-plans/merchant-plan-no/{merchantPlanNo}")]
    Task<PayScorePlanResponse> QueryPlanAsync(
        [Path] string merchantPlanNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止支付分计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012470851"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="merchantPlanNo">商户侧计划号（官方 path <c>merchant_plan_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>终止后的计划（<c>plan_state = STOP</c>），见 <see cref="PayScorePlanResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/plan/payscore-plans/merchant-plan-no/{merchant_plan_no}/stop</c>；
    /// <b>无请求体</b>、无 query ⇒ 方法只有 path 参数（刻意不造空请求体 DTO）。</para>
    /// <para><b>终态语义</b>：<c>STOP</c> 只表示停止新签约，<b>已签约</b>的用户仍可继续履约。</para>
    /// </remarks>
    [Post("/v3/payscore/plan/payscore-plans/merchant-plan-no/{merchantPlanNo}/stop")]
    Task<PayScorePlanResponse> StopPlanAsync(
        [Path] string merchantPlanNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下发服务费用待处理消息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013394606"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">消息请求体，见 <see cref="PayScoreMessageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder/{out_order_no}/send-pendingmessage</c>；
    /// <b>无应答包体</b>（204）⇒ 返回 <see cref="Task"/>。</para>
    /// <para><b><c>prepaid_time</c> 是 8 位 <c>YYYYMMDD</c></b> 且须 ≥ 调接口时间 + 7 天 —— 与 <c>time_range</c> 的 14 位格式不同。</para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder/{outOrderNo}/send-pendingmessage")]
    Task SendPendingMessageAsync(
        [Path] string outOrderNo,
        [Body] PayScoreMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下发服务预扣费消息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013394615"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">消息请求体，见 <see cref="PayScoreMessageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/serviceorder/{out_order_no}/send-prepaidmessage</c>；
    /// <b>无应答包体</b>（204）⇒ 返回 <see cref="Task"/>。</para>
    /// <para><b>与待处理消息共用请求类型</b>：官方两页 path + body 参数表逐项一致 ⇒ 同一 DTO。</para>
    /// </remarks>
    [Post("/v3/payscore/serviceorder/{outOrderNo}/send-prepaidmessage")]
    Task SendPrepaidMessageAsync(
        [Path] string outOrderNo,
        [Body] PayScoreMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 基于签约计划创建支付分服务订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489351"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="request">创单请求体，见 <see cref="PayScoreSignPlanServiceOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单表子集（金额字段由签约计划详情派生），见 <see cref="PayScoreSignPlanServiceOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/sign-plan/serviceorder</c>；无 path / query 参数。</para>
    /// <para><b><c>out_trade_no</c> = 创建签约计划时的 <c>merchant_plan_detail_no</c></b>：传新单号会被拒；
    /// 且不允许跳明细序号、同一时刻只允许一个在途订单。</para>
    /// </remarks>
    [Post("/v3/payscore/sign-plan/serviceorder")]
    Task<PayScoreSignPlanServiceOrderResponse> CreateSignPlanServiceOrderAsync(
        [Body] PayScoreSignPlanServiceOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建用户签约计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489420"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="request">签约请求体，见 <see cref="PayScoreCreateUserSignPlanRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>包装形态 <c>{sign_plan, package}</c>，见 <see cref="PayScoreCreateUserSignPlanResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/sign-plan/user-sign-plans</c>；无 path / query 参数。</para>
    /// <para><b>本支是签约计划族唯一的包装应答</b>：查询与解除都把签约计划字段平铺在应答根。</para>
    /// </remarks>
    [Post("/v3/payscore/sign-plan/user-sign-plans")]
    Task<PayScoreCreateUserSignPlanResponse> CreateUserSignPlanAsync(
        [Body] PayScoreCreateUserSignPlanRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户签约计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012470990"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="merchantSignPlanNo">商户侧签约计划号（官方 path <c>merchant_sign_plan_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>平铺的签约计划表，见 <see cref="PayScoreSignPlan"/>。</returns>
    /// <remarks>
    /// <b>官方契约</b>：<b>GET</b> <c>/v3/payscore/sign-plan/user-sign-plans/merchant-sign-plan-no/{merchant_sign_plan_no}</c>；
    /// 定位键是商户侧签约计划号而非 <c>sign_plan_id</c>。
    /// </remarks>
    [Get("/v3/payscore/sign-plan/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}")]
    Task<PayScoreSignPlan> QueryUserSignPlanAsync(
        [Path] string merchantSignPlanNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除用户签约计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471050"/>
    /// （2026-10-18 逐字段核验；支持商户：【普通商户】）。
    /// </summary>
    /// <param name="merchantSignPlanNo">商户侧签约计划号（官方 path <c>merchant_sign_plan_no</c>，必填 string(32)）。</param>
    /// <param name="request">解除请求体（必填 <c>stop_reason</c>），见 <see cref="PayScoreStopUserSignPlanRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>解除后的签约计划（平铺表），见 <see cref="PayScoreSignPlan"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/sign-plan/user-sign-plans/merchant-sign-plan-no/{merchant_sign_plan_no}/stop</c>；
    /// 本页应答 <b>200 + 签约计划表</b>（不是 204，也不是空包体）。</para>
    /// <para><b>与「终止支付分计划」的差异</b>：本端点必填 <c>stop_reason</c>；计划终止无请求体。</para>
    /// </remarks>
    [Post("/v3/payscore/sign-plan/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}/stop")]
    Task<PayScoreSignPlan> StopUserSignPlanAsync(
        [Path] string merchantSignPlanNo,
        [Body] PayScoreStopUserSignPlanRequest request,
        CancellationToken cancellationToken = default);
}
