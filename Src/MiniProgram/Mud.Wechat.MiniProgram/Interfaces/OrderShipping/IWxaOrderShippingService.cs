// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「发货信息与订单 · 发货录入与报备」域 SDK（4 端点：发货信息录入 / 发货信息合单录入 /
/// 确认收货提醒 / 特殊发货报备；官方 order_shipping 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域是「资金解冻」的前置动作</b>（官方原文「用户交易后，默认资金将会进入冻结状态，开发者在发货后，
/// 需要在小程序平台录入相关发货信息」）：未录入即未发货，用户的确认收货与平台的结算都不会推进。
/// 前置开通状态由 <see cref="IWxaOrderQueryService.IsTradeManagedAsync"/> 查询。
/// </para>
/// <para>
/// <b>一次机会语义（本域最重要的约束）</b>：官方对录入两支逐字写明「以下情况将视为重新发货，
/// 每笔支付单仅有一次重新发货机会 —— (1) 对已完成发货的支付单再次调用该 API；
/// (2) 使用该 API 修改发货模式或物流模式」⇒ <b>本域端点一律不是幂等的</b>，
/// 网络超时后的盲目重试会直接消耗掉这次机会。判据回读 <c>order.shipping.finish_shipping_count</c>
/// （见 <see cref="IWxaOrderQueryService.GetOrderAsync"/>）：0 未发货、1 完成、2 已重新发货并已用尽机会。
/// </para>
/// <para>
/// <b>「同一请求体的字段名在上传侧与查询侧不同」</b>：上传侧订单定位对象用 <c>mchid</c> / <c>out_trade_no</c>，
/// 查询侧用 <c>merchant_id</c> / <c>merchant_trade_no</c>；上传侧物流单商品描述为 <c>item_desc</c>，
/// 查询侧为 <c>goods_desc</c>。两套形态由守卫 OS4 分别锁定，<b>不得</b>为「统一命名」而合并 DTO。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>；四端点官方均支持第三方平台以 <c>authorizer_access_token</c> 代调用，权限集 142。
/// </para>
/// <para>
/// <b>敏感值</b>：<c>payer.openid</c>、<c>contact.consignor_contact</c> / <c>receiver_contact</c>
/// （官方要求<b>掩码</b>传输、最后 4 位数字不得打掩码）禁止写入日志、遥测或异常消息。
/// </para>
/// <para><b>调用限制</b>：四端点官方正文均未公布端点级频率数字（注意事项段只给业务约束），不得凭第三方 SDK 注释或记忆补数。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OrderShipping", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaOrderShippingService
{
    /// <summary>
    /// 发货信息录入（<c>POST /wxa/sec/order/upload_shipping_info</c>，云调用 <c>wxa.sec.order.uploadShippingInfo</c>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_key</c> / <c>logistics_type</c> / <c>delivery_mode</c> / <c>shipping_list</c> / <c>upload_time</c> / <c>payer</c> 必填，<c>is_all_delivered</c> 条件必填），见 <see cref="WxaOrderShippingUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方返回体表除通用两字段外无业务字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；<c>shipping_list</c> 多重性 <b>[1, 15]</b>。</para>
    /// <para>
    /// <b><c>upload_time</c> 是 RFC 3339 字符串</b>（官方类型 <c>string</c>，示例 <c>2022-12-15T13:29:35.120+08:00</c>），
    /// 且「如果要更新物流信息，上传时间必须比之前的请求更新」⇒ 传过去的时间戳串会被拒。
    /// </para>
    /// <para>
    /// <b>条件必填面由业务态决定，SDK 不校验</b>：<c>is_all_delivered</c> 在分拆发货（<c>delivery_mode=2</c>）时必填；
    /// <c>tracking_no</c> / <c>express_company</c> 在物流快递（<c>logistics_type=1</c>）时必填；
    /// <c>contact</c> 在顺丰时必填且收寄件人二选一。分拆发货<b>仅支持</b>物流快递。
    /// </para>
    /// <para><b>物流公司编码</b>须取「获取运力 id 列表 <c>get_delivery_list</c>」（本仓已落位于 <see cref="IWxaExpressDeliveryService"/>），不得自行硬编码快递名。</para>
    /// <para><b>兜底通道</b>（官方原文）：未对接本接口时可在小程序后台「发货信息录入页」手工录入 ⇒ 两条通路写入同一份数据，宿主不得双写。</para>
    /// </remarks>
    [Post("/wxa/sec/order/upload_shipping_info")]
    Task<WxaResponse> UploadShippingInfoAsync(
        [Body] WxaOrderShippingUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发货信息合单录入（<c>POST /wxa/sec/order/upload_combined_shipping_info</c>，云调用 <c>wxa.sec.order.uploadCombinedShippingInfo</c>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadcombinedshippinginfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_key</c> / <c>upload_time</c> / <c>payer</c> 必填，物流信息只挂在 <c>sub_orders</c> 上），见 <see cref="WxaOrderCombinedShippingUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；注意事项与「发货信息录入」逐字同族（一次重新发货机会、
    /// 分拆仅支持物流快递、最多 15 个包裹）。</para>
    /// <para>
    /// <b>顶层没有 <c>logistics_type</c> / <c>delivery_mode</c> / <c>shipping_list</c></b>：合单是把物流信息
    /// 按子单分摊，模式与物流类型都是<b>子单级</b>属性 ⇒ 与 <see cref="UploadShippingInfoAsync"/> 的请求体
    /// 结构不同，守卫 OS4 锁定该形状差异。
    /// </para>
    /// <para><b>主单与子单的 <c>order_number_type</c> 必须一致</b>（官方原文）：混用「商户侧单号」与「微信支付单号」两种形式即为参数错误。</para>
    /// </remarks>
    [Post("/wxa/sec/order/upload_combined_shipping_info")]
    Task<WxaResponse> UploadCombinedShippingInfoAsync(
        [Body] WxaOrderCombinedShippingUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 确认收货提醒（<c>POST /wxa/sec/order/notify_confirm_receive</c>，云调用 <c>wxa.sec.order.notifyConfirmReceive</c>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_notifyconfirmreceive.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>received_time</c> 必填，四支订单定位键按「微信单号」或「商户号+商户单号」二选一），见 <see cref="WxaOrderConfirmReceiveRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；用途是「提醒用户及时确认收货，以提高资金结算效率」。</para>
    /// <para><b>每个订单仅可调用一次</b>（官方正文原话）⇒ 与录入两支同为非幂等端点，重试须宿主自行判重。</para>
    /// <para>
    /// <b>两条硬前置</b>（官方注意事项）：① 只有物流类型为<b>物流快递</b>时才能提醒（虚拟商品 / 自提 / 同城配送
    /// 调用即失败）；② <c>received_time</c> 由商户传入且<b>必须在发货时间之后</b>，该值会直接显示在推给用户的提醒消息里。
    /// </para>
    /// <para><b>签收时间真实性</b>：官方要求「已从快递物流服务方获知用户确实签收」才可调 ⇒ 伪造签收时间属业务违规，SDK 不做校验也不背书。</para>
    /// </remarks>
    [Post("/wxa/sec/order/notify_confirm_receive")]
    Task<WxaResponse> NotifyConfirmReceiveAsync(
        [Body] WxaOrderConfirmReceiveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 特殊发货报备（<c>POST /wxa/sec/order/opspecialorder</c>，<b>不支持云调用</b>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_opspecialorder.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> + <c>type</c> 必填，<c>delay_to</c> 在预售类型下必填），见 <see cref="WxaOrderSpecialReportRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；适用「预售商品订单和测试订单」两类，<b>发货时效超过 15 天的订单必须报备为预售</b>。</para>
    /// <para>
    /// <b>定位键形态与同域其它端点不同</b>：<c>order_id</c> 是<b>单个字符串</b>（微信支付单号<b>或</b>商户单号均可），
    /// 没有 <c>order_number_type</c> 判别位，也不接 <c>order_key</c> 对象 ⇒ 不得复用 <see cref="WxaOrderKey"/>。
    /// </para>
    /// <para><b>预售时间窗</b>（官方注意事项）：<c>delay_to</c> 必须在订单支付时间<b>之后 15 天以上、330 天以内</b>；报备后用户收到预计发货时间通知。</para>
    /// <para><b>测试订单不触发退款</b>（官方原文）：报备后无需发货，如需退回资金须前往商户号后台操作退款或按正常发货流程处理。</para>
    /// <para><b>本端点错误码表只有 3 支</b>（<c>268546000</c> ~ <c>268546002</c>，官方正文即如此，<b>不含通用码 <c>-1</c></b>）⇒ 判错以本表为准。</para>
    /// </remarks>
    [Post("/wxa/sec/order/opspecialorder")]
    Task<WxaResponse> ReportSpecialOrderAsync(
        [Body] WxaOrderSpecialReportRequest request,
        CancellationToken cancellationToken = default);
}
