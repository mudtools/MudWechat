// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopOrder;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · 合单支付与资金操作」域 SDK（6 端点：创建支付单 / 关闭支付单 /
/// 支付单详情 / 支付单列表 / 请求分账 / 请求退款；官方 <c>wxafunds/API/order</c> 分区，路由一律 <c>/shop/pay/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 本域与小店线 <c>/channels/ec/*</c> 的微信支付链无关。
/// </para>
/// <para>
/// <b>与「订单本体」域的分工</b>：<c>/shop/order/*</c>（<see cref="IWxaShopOrderService"/>）管理交易组件订单状态；
/// 本域 <c>/shop/pay/*</c> 管理<b>合单支付单</b>（一分多子单）与分账 / 退款。两套单号不可互换：
/// 本域的 <c>trade_no</c> 是商家交易单号、<c>combine_trade_no</c> 是合单总单号，
/// 与订单域的 <c>out_order_id</c> / <c>order_id</c> 是不同键。
/// </para>
/// <para>
/// <b>幂等键（官方原文）</b>：<c>createorder</c> 的 <c>combine_trade_no</c> 与子单 <c>trade_no</c> 长度 6～32、
/// 「同一 <c>combine_trade_no</c> 视为同一请求，不允许修改子单等参数」「同一 <c>trade_no</c> 不允许修改价格等参数」；
/// <c>profitsharingorder</c> 的 <c>profit_sharing_no</c>「同一分账单号多次请求等同一次」；
/// <c>refundorder</c> 的 <c>refund_no</c>「同一退款单号多次请求只退一笔」⇒ 分账与退款两支是
/// <b>幂等安全</b>的（同号重放无副作用），创建单是<b>同号只能同参</b>。
/// </para>
/// <para>
/// <b>金额单位一律分</b>：<c>amount</c> / <c>total_amount</c> / <c>refund_amount</c> / <c>pay_amount</c> 全部为分，
/// 官方逐行标注「单位为分」。
/// </para>
/// <para>
/// <b>时间口径</b>：本域时间一律<b>秒级时间戳</b>（<c>expire_time</c> / <c>create_time</c> / <c>update_time</c> /
/// <c>pay_time</c> / <c>finish_time</c>），与订单域 <c>yyyy-MM-dd HH:mm:ss</c> 字符串口径<b>不同</b>，不得混用；
/// <c>getorderlist</c> 的起止时间<b>间隔不得超过 1 天</b>。
/// </para>
/// <para>
/// <b>不可逆与资金风险</b>：<c>closeorder</c> 关闭后不可重开；<c>profitsharingorder</c> 与 <c>refundorder</c>
/// 直接动账且官方无撤销端点 ⇒ 虽然同号幂等，仍<b>不得</b>放进自动重试管道（重试应复用同一幂等号，而非新号）。
/// </para>
/// <para>
/// <b>官方端点级频率数字空缺</b>：6 页正文均无「次/分钟」类数字，也未给权限集编号与云调用说明 ⇒ 注释留档，
/// 不得凭第三方 SDK 或记忆补数；<c>page_size</c> 上限 100 与「时间跨度 ≤ 1 天」是规模限制，与频率分列。
/// </para>
/// <para>
/// <b>敏感面（AGENTS §8）</b>：<c>openid</c>、<c>mchid</c>、<c>transaction_id</c>、<c>refund_id</c>、
/// <c>profit_sharing_id</c>、<c>payment_params.package</c> / <c>paySign</c> 与全部金额属个人金融信息或支付凭据
/// ⇒ <b>一律不得写入日志、遥测或异常消息</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopOrder", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopPayService
{
    /// <summary>
    /// 创建支付单（<c>POST /shop/pay/createorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/order/create_order.html"/>。
    /// </summary>
    /// <param name="request">合单支付请求（总单号 + 子单列表），见 <see cref="WxaShopPayCreateOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>payment_params</c>（拉起收银台参数），见 <see cref="WxaShopPaymentParamsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>openid</c> / <c>combine_trade_no</c> / <c>sub_orders</c> 必填；
    /// 子单 <c>mchid</c> / <c>description</c> / <c>amount</c> / <c>trade_no</c> 必填；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>幂等约束</b>：<c>combine_trade_no</c> 与子单 <c>trade_no</c> 长度 6～32、字符集
    /// <c>数字 / 大小写字母 / _-|*@</c>；同一 <c>combine_trade_no</c> 视为同一请求且<b>不允许修改子单等参数</b>
    /// ⇒ 重试必须原样重放，新参数须换新号。
    /// </para>
    /// <para>
    /// <b><c>expire_time</c> 为秒级时间戳</b>（选填）；返回的 <c>payment_params.package</c> 与 <c>paySign</c>
    /// 是敏感支付凭据，不得进日志。
    /// </para>
    /// </remarks>
    [Post("/shop/pay/createorder")]
    Task<WxaShopPaymentParamsResponse> CreateOrderAsync(
        [Body] WxaShopPayCreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭支付单（<c>POST /shop/pay/closeorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/order/close_order.html"/>。
    /// </summary>
    /// <param name="request">总单号 + 子单列表，见 <see cref="WxaShopPayCloseOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>openid</c> / <c>combine_trade_no</c> / <c>sub_orders</c> 必填，
    /// 子单 <c>mchid</c> / <c>trade_no</c> 必填；本域官方无端点级频率数字。
    /// </para>
    /// <para><b>状态不可逆</b>：关闭后官方无重开端点 ⇒ 非幂等、不得自动重试。</para>
    /// </remarks>
    [Post("/shop/pay/closeorder")]
    Task<WxaResponse> CloseOrderAsync(
        [Body] WxaShopPayCloseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支付单详情（<c>POST /shop/pay/getorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/order/get_order_detail.html"/>。
    /// </summary>
    /// <param name="request">商家交易单号，见 <see cref="WxaShopPayGetOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支付单详情（含退款与分账记录），见 <see cref="WxaShopPayGetOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体 <c>{"trade_no":"…"}</c>（<c>trade_no</c> 必填）；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>应答是 <c>order</c> 包装</b>：<c>{errcode, errmsg, order:{…}}</c>；详情含
    /// <c>refund_list[]</c> 与 <c>profit_sharing_list[]</c> 两个子列表，以及
    /// <c>close_status</c>（0 正常 / 1 已关闭）、<c>profit_sharing_delay</c>（账期天数）、
    /// <c>profit_sharing_frozen</c>（1 冻结分账 / 0 正常）。
    /// </para>
    /// <para><b>只读</b>：可安全重试。</para>
    /// </remarks>
    [Post("/shop/pay/getorder")]
    Task<WxaShopPayGetOrderResponse> GetOrderAsync(
        [Body] WxaShopPayGetOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支付单列表（<c>POST /shop/pay/getorderlist</c>，游标式分页）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/order/get_order_list.html"/>。
    /// </summary>
    /// <param name="request">时间窗 + 分页游标，见 <see cref="WxaShopPayGetOrderListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>trade_no_list</c> + <c>has_more</c> + <c>last_index</c>，见 <see cref="WxaShopPayGetOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>begin_create_time</c> / <c>end_create_time</c> / <c>page_size</c>
    /// 必填，起止<b>间隔不得超过 1 天</b>，<c>page_size</c> 上限 <b>100</b>；<c>mchid</c> 选填（不填默认全量）；
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>游标分页</b>：官方用 <c>last_index</c> 而非页码——首页不传，后续把回包 <c>last_index</c> 原样回传，
    /// <c>has_more=false</c> 即结束；回包只给 <c>trade_no_list</c>，详情须逐单调用 <see cref="GetOrderAsync"/>。
    /// </para>
    /// <para>
    /// <b>官方工作单映射更正</b>：该路由的官方页是 <c>get_order_list.html</c>（不是 <c>get_order_detail.html</c>），
    /// 守卫 SHPO-P 锁定本 URL。
    /// </para>
    /// <para><b>只读</b>：可安全重试。</para>
    /// </remarks>
    [Post("/shop/pay/getorderlist")]
    Task<WxaShopPayGetOrderListResponse> GetOrderListAsync(
        [Body] WxaShopPayGetOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求分账（<c>POST /shop/pay/profitsharingorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/order/profitsharing_order.html"/>。
    /// </summary>
    /// <param name="request">分账请求，见 <see cref="WxaShopPayProfitSharingOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>openid</c> / <c>mchid</c> / <c>trade_no</c> / <c>transaction_id</c> /
    /// <c>profit_sharing_no</c> 五支全必填；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>幂等安全</b>：官方原文「<c>profit_sharing_no</c> 小程序系统内唯一，同一分账单号多次请求等同一次」
    /// ⇒ 重试必须复用同一分账单号；错误 <c>9720004</c> 表示订单待分账余额为 0，<c>-1</c> 为系统异常。
    /// </para>
    /// <para>
    /// <b>动账风险</b>：成功即产生分账事实且官方无撤销端点 ⇒ 不得放进自动重试管道（业务层重试须同号）。
    /// </para>
    /// </remarks>
    [Post("/shop/pay/profitsharingorder")]
    Task<WxaResponse> ProfitSharingOrderAsync(
        [Body] WxaShopPayProfitSharingOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求退款（<c>POST /shop/pay/refundorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/order/refunds_order.html"/>。
    /// </summary>
    /// <param name="request">退款请求，见 <see cref="WxaShopPayRefundOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>openid</c> / <c>mchid</c> / <c>trade_no</c> / <c>transaction_id</c> /
    /// <c>refund_no</c> / <c>total_amount</c> / <c>refund_amount</c> 七支全必填；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>幂等安全</b>：官方原文「<c>refund_no</c> 小程序系统内部唯一，同一退款单号多次请求只退一笔」，
    /// 字符集 <c>数字 / 大小写字母 / _-|*@</c>、长度 6～32 ⇒ 重试必须复用同一退款单号。
    /// </para>
    /// <para>
    /// <b>金额口径</b>：<c>total_amount</c> 是订单总金额、<c>refund_amount</c> 是本次退款金额，均为分；
    /// 退款为动账操作且官方无撤销端点 ⇒ 不得放进自动重试管道（业务层重试须同号）。
    /// </para>
    /// </remarks>
    [Post("/shop/pay/refundorder")]
    Task<WxaResponse> RefundOrderAsync(
        [Body] WxaShopPayRefundOrderRequest request,
        CancellationToken cancellationToken = default);
}
