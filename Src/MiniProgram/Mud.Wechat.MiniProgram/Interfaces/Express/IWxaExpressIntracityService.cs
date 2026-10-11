// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「物流助手 · 同城配送」域 SDK（13 端点）—— <b>带令牌</b>通道。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 物流 → 同城配送，2026-10-11 逐页核验）：
/// 申请开通 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_apply"/>、
/// 创建门店 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore"/>、
/// 查询门店 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_querystore"/>、
/// 更新门店 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_updatestore"/>、
/// 运费充值 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storecharge"/>、
/// 运费退款 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storerefund"/>、
/// 查询流水 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryflow"/>、
/// 查询余额 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_balancequery"/>、
/// 询价 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder"/>、
/// 下单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>、
/// 查询配送单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryorder"/>、
/// 取消配送单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_cancelorder"/>、
/// 模拟回调 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_mocknotify"/>。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// <b>业务警示</b>：本域全部端点为 <b>POST</b> ＋ 请求体 JSON；<c>mocknotify</c> 仅供<b>测试环境</b>调用；
/// 同城配送使用前须先开通并创建门店（<c>wx_store_id</c> 是后续运力订单的定位键）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Express", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaExpressIntracityService
{
    /// <summary>
    /// 申请开通同城配送（<c>POST /cgi-bin/express/intracity/apply</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_apply"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示受理成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，<b>无请求体</b>；Query 携带 <c>access_token</c>。</para>
    /// <para><b>业务语义（官方原文）</b>：使用同城配送能力前须先申请开通；本接口<b>无请求体</b>，
    /// 申请结果经<b>审核商户事件</b>（<c>review_merchant</c>）异步通知，后续方可创建门店并下单。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/apply")]
    Task<WxaResponse> ApplyIntracityAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建门店（<c>POST /cgi-bin/express/intracity/createstore</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore"/>。
    /// </summary>
    /// <param name="request">创建门店请求体（<c>out_store_id</c>/<c>store_name</c>/<c>address_info</c> 必填），见 <see cref="WxaExpressCreateStoreRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门店编号（<c>out_store_id</c>/<c>wx_store_id</c>/<c>appid</c>），见 <see cref="WxaExpressCreateStoreResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <c>out_store_id</c> 自定义门店编号需唯一；创建后系统生成全局唯一 <c>wx_store_id</c>，
    /// 后续创建运力订单需要该门店编号。<c>order_pattern</c>：1 价格优先，2 运力优先（默认价格优先）；
    /// <c>order_pattern=2</c> 时 <c>service_trans_prefer</c> 必填。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/createstore")]
    Task<WxaExpressCreateStoreResponse> CreateStoreAsync(
        [Body] WxaExpressCreateStoreRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询门店（<c>POST /cgi-bin/express/intracity/querystore</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_querystore"/>。
    /// </summary>
    /// <param name="request">查询门店请求体（<c>out_store_id</c> 与 <c>wx_store_id</c> 二选一），见 <see cref="WxaExpressQueryStoreRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门店列表（<c>store_list</c>/<c>total</c>），见 <see cref="WxaExpressQueryStoreResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/intracity/querystore")]
    Task<WxaExpressQueryStoreResponse> QueryStoreAsync(
        [Body] WxaExpressQueryStoreRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新门店（<c>POST /cgi-bin/express/intracity/updatestore</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_updatestore"/>。
    /// </summary>
    /// <param name="request">更新门店请求体（<c>keys</c>/<c>content</c> 必填），见 <see cref="WxaExpressUpdateStoreRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新后门店编号，见 <see cref="WxaExpressUpdateStoreResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>keys</c> 为门店定位（<c>wx_store_id</c> 与 <c>out_store_id</c> 二选一），<c>content</c> 为待更新内容（均可选，只更新传入字段）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/updatestore")]
    Task<WxaExpressUpdateStoreResponse> UpdateStoreAsync(
        [Body] WxaExpressUpdateStoreRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 门店运费充值（<c>POST /cgi-bin/express/intracity/storecharge</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storecharge"/>。
    /// </summary>
    /// <param name="request">充值请求体（<c>wx_store_id</c>/<c>service_trans_id</c>/<c>amount</c> 必填），见 <see cref="WxaExpressStoreChargeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支付链接（<c>payurl</c>），见 <see cref="WxaExpressStoreChargeResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>充值金额 <c>amount</c> 单位：分；充值成功后返回支付链接 <c>payurl</c> 拉起微信支付完成充值。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/storecharge")]
    Task<WxaExpressStoreChargeResponse> StoreChargeAsync(
        [Body] WxaExpressStoreChargeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 门店运费退款（<c>POST /cgi-bin/express/intracity/storerefund</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storerefund"/>。
    /// </summary>
    /// <param name="request">退款请求体（<c>wx_store_id</c>/<c>service_trans_id</c> 必填），见 <see cref="WxaExpressStoreRefundRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退款结果（<c>refund_amount</c>），见 <see cref="WxaExpressStoreRefundResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/intracity/storerefund")]
    Task<WxaExpressStoreRefundResponse> StoreRefundAsync(
        [Body] WxaExpressStoreRefundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询门店运费流水（<c>POST /cgi-bin/express/intracity/queryflow</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryflow"/>。
    /// </summary>
    /// <param name="request">流水查询请求体（<c>wx_store_id</c>/<c>flow_type</c> 必填），见 <see cref="WxaExpressQueryFlowRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水列表（<c>flow_list</c>/<c>total</c>），见 <see cref="WxaExpressQueryFlowResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>flow_type</c>：1 充值，2 扣款，3 退款，4 扣款退回。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/queryflow")]
    Task<WxaExpressQueryFlowResponse> QueryFlowAsync(
        [Body] WxaExpressQueryFlowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询门店余额（<c>POST /cgi-bin/express/intracity/balancequery</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_balancequery"/>。
    /// </summary>
    /// <param name="request">余额查询请求体（<c>wx_store_id</c>/<c>service_trans_id</c> 必填），见 <see cref="WxaExpressBalanceQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>余额明细（<c>all_balance</c>/<c>balance_detail</c>），见 <see cref="WxaExpressBalanceQueryResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/intracity/balancequery")]
    Task<WxaExpressBalanceQueryResponse> BalanceQueryAsync(
        [Body] WxaExpressBalanceQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同城配送询价（<c>POST /cgi-bin/express/intracity/preaddorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder"/>。
    /// </summary>
    /// <param name="request">询价请求体（<c>wx_store_id</c>/<c>user_*</c>/<c>cargo</c> 必填），见 <see cref="WxaExpressIntracityOrderBaseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预估费用与服务信息（<c>service_trans_id</c>/<c>distance</c>/<c>est_fee</c>），见 <see cref="WxaExpressPreAddOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>调用后返回预估费用与服务信息，商家可据此展示给用户；<c>service_trans_id</c> 在下单时须传回。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/preaddorder")]
    Task<WxaExpressPreAddOrderResponse> PreAddOrderAsync(
        [Body] WxaExpressIntracityOrderBaseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同城配送下单（<c>POST /cgi-bin/express/intracity/addorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>。
    /// </summary>
    /// <param name="request">下单请求体（<c>wx_store_id</c>/<c>store_order_id</c>/<c>user_*</c>/<c>cargo</c> 必填），见 <see cref="WxaExpressAddIntracityOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配送单信息（<c>wx_order_id</c>/<c>fee</c>/<c>distance</c>），见 <see cref="WxaExpressAddIntracityOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>store_order_id</c> 用于幂等（重复下单返回同一配送单）；回调 <c>callback_url</c> 配置后配送状态变化会异步通知。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/addorder")]
    Task<WxaExpressAddIntracityOrderResponse> AddIntracityOrderAsync(
        [Body] WxaExpressAddIntracityOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询配送单（<c>POST /cgi-bin/express/intracity/queryorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryorder"/>。
    /// </summary>
    /// <param name="request">查询请求体（<c>wx_store_id</c> 必填，<c>store_order_id</c> 与 <c>wx_order_id</c> 二选一），见 <see cref="WxaExpressQueryIntracityOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配送单全量明细，见 <see cref="WxaExpressQueryIntracityOrderResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/intracity/queryorder")]
    Task<WxaExpressQueryIntracityOrderResponse> QueryIntracityOrderAsync(
        [Body] WxaExpressQueryIntracityOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消配送单（<c>POST /cgi-bin/express/intracity/cancelorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_cancelorder"/>。
    /// </summary>
    /// <param name="request">取消请求体（<c>cancel_reason_id</c> 必填），见 <see cref="WxaExpressCancelIntracityOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>取消结果（<c>order_status</c>/<c>deductfee</c>），见 <see cref="WxaExpressCancelIntracityOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>cancel_reason_id</c>：1001 用户取消，1002 门店取消，1003 骑手取消，1004 其它；骑手已接单后取消可能产生扣款（<c>deductfee</c>）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/cancelorder")]
    Task<WxaExpressCancelIntracityOrderResponse> CancelIntracityOrderAsync(
        [Body] WxaExpressCancelIntracityOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 模拟配送状态变更回调（<c>POST /cgi-bin/express/intracity/mocknotify</c>，<b>仅测试环境</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_mocknotify"/>。
    /// </summary>
    /// <param name="request">模拟回调请求体（<c>order_status</c> 必填），见 <see cref="WxaExpressMockNotifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>仅测试环境可用：模拟配送单状态变化，触发配置的 <c>callback_url</c> 回调，用于商家侧联调。</para>
    /// </remarks>
    [Post("/cgi-bin/express/intracity/mocknotify")]
    Task<WxaResponse> MockNotifyAsync(
        [Body] WxaExpressMockNotifyRequest request,
        CancellationToken cancellationToken = default);
}
