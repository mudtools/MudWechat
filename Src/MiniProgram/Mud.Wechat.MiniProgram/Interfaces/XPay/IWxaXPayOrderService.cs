// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「虚拟支付 · 订单与账单」域 SDK（7 端点：查询订单 / 启动退款任务 / 通知发货完成 /
/// 创建账单下载任务 / 查询下载任务 / 下载小程序账单 / 下载 iOS 月结账单）。
/// </summary>
/// <remarks>
/// <para>
/// 域统一形态（POST only、不支持云调用、权限集 157、<c>pay_sig</c> 落 Query 的凭据处置、敏感值口径与
/// 全域错误码表）见 <see cref="IWxaXPayCurrencyService"/>，本文件只写本族特有事实。
/// </para>
/// <para>
/// <b>账单下载是三条不同通路，不可互相替代</b>：日粒度区间账单走 <see cref="DownloadBillAsync"/>（同步取链接）、
/// 按订单类型筛选的明细走「创建任务 + 轮询」两支（<see cref="StartDownloadOrderAsync"/> +
/// <see cref="QueryDownloadOrderAsync"/>）、iOS 月结走 <see cref="DownloadIosSettlementBillAsync"/>（月粒度）。
/// </para>
/// <para>
/// <b>本族是「先查后动」的强依赖关系</b>：官方 <c>268490016</c> 明写退款 <c>left_fee</c> 不符时
/// 「请通过query_order接口查询确认」⇒ <see cref="RefundOrderAsync"/> 的入参必须来自
/// <see cref="QueryOrderAsync"/> 的实时结果，不得用本地缓存金额。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "XPay", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaXPayOrderService
{
    /// <summary>
    /// 查询创建的订单（<c>POST /xpay/query_order</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>env</c> 必填，<c>order_id</c> 与 <c>wx_order_id</c> 二选一），见 <see cref="WxaXPayQueryOrderRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情袋（23 字段，见 <see cref="WxaXPayOrderInfo"/>），见 <see cref="WxaXPayQueryOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + <c>pay_sig</c> Query；本页也是本域字段面最宽的订单模型出处。</para>
    /// <para>
    /// <b>退款进度的官方口径是 <c>status</c> 的 5 / 7 / 8 三值</b>（<see cref="WxaXPayOrderStatuses"/>）：
    /// 5 只代表商户侧扣款完成，用户侧收款完成才是 8 ⇒ 把 5 当「退款完成」会早于用户实际到账。
    /// </para>
    /// <para><b>结算相关三支（<c>sett_time</c> / <c>sett_state</c> / 两支服务费）互相约束</b>：服务费仅
    /// <c>sett_state = 2</c> 时有值 ⇒ 分账核算须先判结算状态。</para>
    /// <para><b>敏感值</b>：响应内 <c>token</c> 是米大师下单凭证 ⇒ 不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/xpay/query_order")]
    Task<WxaXPayQueryOrderResponse> QueryOrderAsync(
        [Body] WxaXPayQueryOrderRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 启动订单退款任务（<c>POST /xpay/refund_order</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_refund_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>left_fee</c> 须等于查询到的剩余可退金额），见 <see cref="WxaXPayRefundOrderRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="signature">
    /// 用户态签名（官方 Query 参数 <c>signature</c>）。
    /// <b>官方本页自相矛盾</b>：「注意事项」原文写「使用用户态签名与支付签名」，而「查询参数」表只列
    /// <c>access_token</c> 与 <c>pay_sig</c> 两支（调用示例亦然）⇒ 本 SDK 以<b>参数表为准</b>把 <c>pay_sig</c> 定为必填、
    /// 以<b>注意事项为准</b>额外开放可选的 <c>signature</c>（默认不发送），宿主若被官方判 <c>268490003</c>（签名错误）
    /// 时应补传该支。该取舍由守卫 XP3 显式登记，<b>不得</b>当成「SDK 漏参」。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退款单与关联支付单的双侧单号，见 <see cref="WxaXPayRefundOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c> Query；「启动任务」四字是语义核心 —— 本端点<b>同步只回单号</b>，
    /// 到账结果须回 <see cref="QueryOrderAsync"/> 看 <c>status</c> 8。
    /// </para>
    /// <para>
    /// <b>三个退款约束（官方错误码反推）</b>：已核销单禁退 <c>268490013</c>、金额不足 <c>268490006</c>、
    /// <c>left_fee</c> 与实际不符 <c>268490016</c> ⇒ 退款前必须查单；进行中 <c>268490014</c> 官方原文
    /// 「稍后可以使用相同参数重试」⇒ <b>必须</b>原参重试，换 <c>refund_order_id</c> 重发会重复退款。
    /// </para>
    /// <para><b>退款原因与来源两支是官方必填的审计字段</b>（<c>refund_reason</c> / <c>req_from</c>）⇒ 不得填空串糊过去。</para>
    /// </remarks>
    [Post("/xpay/refund_order")]
    Task<WxaXPayRefundOrderResponse> RefundOrderAsync(
        [Body] WxaXPayRefundOrderRequest request,
        [Query("pay_sig")] string paySig,
        [Query("signature")] string? signature = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通知已发货完成（<c>POST /xpay/notify_provide_goods</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_notify_provide_goods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> 与 <c>wx_order_id</c> 二选一），见 <see cref="WxaXPayNotifyProvideGoodsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c>（页面原文返回体「无」）⇒ 直接返回 <see cref="WxaResponse"/>，不造 DTO。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query 只有 <c>access_token</c> ⇒ 本端点是本域「无签名 + 无返回体」双例外端点。
    /// </para>
    /// <para>
    /// <b>虚拟商品的「发货」是合规动作而非物流动作</b>：订单状态机里 2（待发货）→ 3（发货中）→ 4（已发货）
    /// 由本端点驱动（<see cref="WxaXPayOrderStatuses"/>），未通知发货的订单官方侧不结算 ⇒
    /// 交付完成后漏调本端点会直接压住资金。
    /// </para>
    /// <para>官方明写本页「此接口没有特殊错误码」⇒ 只有通用错误码。</para>
    /// </remarks>
    [Post("/xpay/notify_provide_goods")]
    Task<WxaResponse> NotifyProvideGoodsAsync(
        [Body] WxaXPayNotifyProvideGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载支付订单（创建账单下载任务，<c>POST /xpay/start_download_order</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_download_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（区间不超 31 天；<c>is_provided</c> 与 <c>refund_status</c> 按 <c>order_type</c> 条件必填），见 <see cref="WxaXPayStartDownloadOrderRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务 id（须再走 <see cref="QueryDownloadOrderAsync"/> 取链接），见 <see cref="WxaXPayStartDownloadOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；本端点是<b>异步任务创建</b>，官方明写本页无特殊错误码 ⇒
    /// 失败只能通过轮询任务状态发现，不能假定同步返回即成功。
    /// </para>
    /// <para>
    /// <b>四支筛选参数的适用面官方写在说明列</b>（无枚举表，故不建常量类）：
    /// <c>order_type</c> 1 代币交易 / 2 道具直购 / 3 会员订阅 / 4 退款；<c>refund_status</c> 仅 <c>order_type = 4</c>
    /// 有效（0 全部 / 2 已退款 / 4 退款中 / 5 退款失败，默认 0）；<c>is_provided</c> 在 2/3 时必传（默认 true）；
    /// <c>pay_channel</c> 1 普通虚拟支付 / 2 苹果 IAP 且<b>必填</b>。
    /// </para>
    /// <para><b>官方类型列把 <c>is_provided</c> 标为 <c>boolean</c></b> ⇒ 线格式落 <c>bool?</c>，与短剧域同类处置一致。</para>
    /// </remarks>
    [Post("/xpay/start_download_order")]
    Task<WxaXPayStartDownloadOrderResponse> StartDownloadOrderAsync(
        [Body] WxaXPayStartDownloadOrderRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询下载订单任务（<c>POST /xpay/query_download_order</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_download_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>task_id</c> / <c>env</c> 必填），见 <see cref="WxaXPayQueryDownloadOrderRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务状态 + 下载链接 + 失效时刻，见 <see cref="WxaXPayQueryDownloadOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + <c>pay_sig</c>；官方明写本页无特殊错误码，失败形态是任务状态未就绪而非报错。</para>
    /// <para>
    /// <b><c>download_url</c> 必须即时消费</b>：官方给 <c>expire_at</c> 字段本身就说明链接有时效 ⇒
    /// 不得持久化后复用，也<b>不得</b>写进日志、遥测或异常消息（与广告金订单下载链接同处置）。
    /// </para>
    /// </remarks>
    [Post("/xpay/query_download_order")]
    Task<WxaXPayQueryDownloadOrderResponse> QueryDownloadOrderAsync(
        [Body] WxaXPayQueryDownloadOrderRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载小程序账单（<c>POST /xpay/download_bill</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_bill.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>begin_ds</c> / <c>end_ds</c>，形如 <c>20230801</c>），见 <see cref="WxaXPayDownloadBillRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账单下载链接（与广告金商户订单下载共用应答形态），见 <see cref="WxaXPayDownloadUrlResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，请求只有日期区间两支 ⇒ 与 <see cref="StartDownloadOrderAsync"/>
    /// 的区别是<b>本端点同步回链接、不提供类型筛选</b>。
    /// </para>
    /// <para><b><c>begin_ds</c>/<c>end_ds</c> 官方类型是 <c>number</c></b>（日期写作数字）⇒ 线格式照官方落 <c>long?</c>，
    /// 与投诉侧 <c>begin_date</c> 的 <c>YYYY-MM-DD</c> 字符串不同形。</para>
    /// </remarks>
    [Post("/xpay/download_bill")]
    Task<WxaXPayDownloadUrlResponse> DownloadBillAsync(
        [Body] WxaXPayDownloadBillRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载虚拟支付 iOS 月结账单（<c>POST /xpay/download_ios_settlement_bill</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_ios_settlement_bill.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>start_month</c> / <c>end_month</c>，形如 <c>YYYYMM</c>），见 <see cref="WxaXPayDownloadIosSettlementBillRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐月账单条目（<c>month</c> + <c>bill_url</c>），见 <see cref="WxaXPayDownloadIosSettlementBillResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，一次请求返回<b>区间内逐月列表</b>（不是单条）⇒
    /// 与 <see cref="DownloadBillAsync"/> 的单链接形态不同。
    /// </para>
    /// <para><b>本页官方明写无特殊错误码</b>，且 iOS 支只走苹果 IAP 通道（与订单模型 <c>order_type</c> 7/8 对应）
    /// ⇒ 用安卓流水去对月结账单必然对不平。</para>
    /// <para><b>失效时长官方未给数字</b>（原文「及时使用，一定时间后失效」）⇒ 不得套用 <c>cos_url</c> 的 30 分钟口径。</para>
    /// </remarks>
    [Post("/xpay/download_ios_settlement_bill")]
    Task<WxaXPayDownloadIosSettlementBillResponse> DownloadIosSettlementBillAsync(
        [Body] WxaXPayDownloadIosSettlementBillRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);
}
