// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「发货信息与订单 · 查询」域 SDK（4 端点：查询订单发货状态 / 查询订单列表 /
/// 查询是否已开通发货信息管理服务 / 查询是否已完成交易结算管理确认；官方 order_shipping 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两支订单查询与两支开通状态查询同族的理由</b>：前两者给「单笔/多笔支付单的发货事实」，
/// 后两者给「本账号能否使用本域的准入条件」⇒ 官方把它们都放在 order_shipping 目录内，
/// 且后两支是接入本域前的<b>前置探测</b>（未开通即无法调用 <see cref="IWxaOrderShippingService"/> 的录入端点）。
/// </para>
/// <para>
/// <b>定位键名与上传侧不同</b>：本域用 <c>merchant_id</c> / <c>merchant_trade_no</c>（+ <c>sub_merchant_id</c>），
/// 上传侧 <see cref="WxaOrderKey"/> 用 <c>mchid</c> / <c>out_trade_no</c> ⇒ 由守卫 OS4 分别锁定，不得合并。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>。<b>第三方开放面在本接口内即不统一</b>：<see cref="GetOrderAsync"/> 与
/// 两支开通状态查询支持 <c>authorizer_access_token</c>（权限集 142，后两支为 18、142），
/// <see cref="GetOrderListAsync"/> 官方明写「本接口不支持第三方平台调用」。
/// </para>
/// <para>
/// <b>敏感值</b>：<c>openid</c>（支付者）、<c>shipping.shipping_list[].contact</c> 的收寄件人联系方式
/// 禁止写入日志、遥测或异常消息；<c>paid_amount</c> 是金额（单位分），同样按敏感值处置。
/// </para>
/// <para><b>调用限制</b>：四端点官方正文均未公布端点级频率数字；<see cref="GetOrderListAsync"/> 的 <c>page_size</c> 默认 100 属规模限制，不得当频率解读。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OrderShipping", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaOrderQueryService
{
    /// <summary>
    /// 查询订单发货状态（<c>POST /wxa/sec/order/get_order</c>，云调用 <c>wxa.sec.order.getOrder</c>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html"/>。
    /// </summary>
    /// <param name="request">请求体（四支定位键官方全标选填，实为「微信单号」或「商户号+商户单号」二选一），见 <see cref="WxaOrderQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支付单信息（<c>order</c>，含发货信息与物流单列表），见 <see cref="WxaOrderGetResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；定位方式与微信支付侧一致 —— 交易单号形式或商户号 + 商户单号形式，二选一。</para>
    /// <para><b>定位键写错即 <c>10060001</c>「支付单不存在」</b>（官方错误码说明原文点名「检查 transaction_id 字段或 mchid、out_trade_no 字段是否有误」）
    /// —— 注意官方这段<b>说明文字用的是上传侧的键名 <c>mchid</c> / <c>out_trade_no</c></b>，而本端点请求体实际是
    /// <c>merchant_id</c> / <c>merchant_trade_no</c>，属官方页面自身的名实不一致，不得据此改本端点字段面。</para>
    /// <para><b><c>order_state</c> 有 6 档</b>（1 待发货 / 2 已发货 / 3 确认收货 / 4 交易完成 / 5 已退款 / 6 资金待结算，见 <see cref="WxaOrderStateValues"/>）：与虚拟支付域的订单状态是两套取值，不得跨域复用。</para>
    /// <para><b><c>finish_shipping_count</c> 是「重新发货机会」计数器</b>（官方：未完成 0、完成 1、重新发货并完成后 2）⇒ 值为 2 表示本单已用掉唯一一次重新发货机会。</para>
    /// <para><b><c>description</c> 由官方截断</b>（超 120 字截断加省略号），而 <c>shipping.goods_desc</c> 是后台录入页的原值 ⇒ 两者可能不等长，别互相校验。</para>
    /// </remarks>
    [Post("/wxa/sec/order/get_order")]
    Task<WxaOrderGetResponse> GetOrderAsync(
        [Body] WxaOrderQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询订单列表（<c>POST /wxa/sec/order/get_order_list</c>，云调用 <c>wxa.sec.order.getOrderList</c>，<b>不支持第三方平台调用</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorderlist.html"/>。
    /// </summary>
    /// <param name="request">请求体（五个参数全填选填：时间范围 / 订单状态 / openid / 游标 / 页长），见 <see cref="WxaOrderListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支付单列表 + 游标（<c>last_index</c> / <c>has_more</c>），见 <see cref="WxaOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；可按支付时间、支付者 openid 或订单状态检索。</para>
    /// <para>
    /// <b>游标翻页，不是 offset</b>：第一页<b>不传</b> <c>last_index</c>；返回体 <c>has_more</c> 为 <c>true</c> 时
    /// 把<b>本次</b>返回的 <c>last_index</c> 原样带上取下一页 ⇒ 不得自行构造、跨批复用或缓存后回灌。
    /// </para>
    /// <para><b>时间窗默认值不对称</b>：<c>begin_time</c> 缺省为 0，<c>end_time</c> 缺省为 32 位无符号整型最大值且<b>含</b>该时刻 ⇒ 全不传即全量扫描，须自行收窄。</para>
    /// <para><b>本端点不支持第三方代调用</b>（官方原文），与同域其它查询端点不同 ⇒ 服务商场景请改走 <see cref="GetOrderAsync"/> 逐单查。</para>
    /// <para><b>返回体表未列 <c>errmsg</c></b>（只有 <c>errcode</c> 与业务三字段）⇒ 判错须以 <c>errcode</c> 为准，不得依赖 <c>errmsg</c> 非空。</para>
    /// </remarks>
    [Post("/wxa/sec/order/get_order_list")]
    Task<WxaOrderListResponse> GetOrderListAsync(
        [Body] WxaOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询小程序是否已开通发货信息管理服务（<c>POST /wxa/sec/order/is_trade_managed</c>，云调用 <c>wxa.sec.order.isTradeManaged</c>，权限集 18、142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanaged.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>appid</c> 一字段且<b>必填</b>，与 <see cref="IsTradeManagementConfirmationCompletedAsync"/> 共用），见 <see cref="WxaOrderTradeManagedQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>is_trade_managed</c>：是否已开通小程序发货信息管理服务，见 <see cref="WxaOrderTradeManagedResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；「已开通的小程序，可接入发货信息管理服务 API 进行发货管理」⇒ 本端点是调用录入端点的前置探测。</para>
    /// <para>
    /// <b><c>appid</c> 必填且非服务商只能查本账号</b>（官方原文）：与同域订单查询「按单号定位」的形态不同，
    /// 本端点<b>不接</b>任何订单参数，只做账号级状态查询。
    /// </para>
    /// <para><b>服务商前置</b>（官方注意事项）：须被授权 <b>18 或 142</b> 权限集之一才能查询 —— 与 <see cref="GetOrderAsync"/> 只需 142 不同。</para>
    /// <para><b>错误码面为通用码族</b>（<c>-1</c> / <c>40013</c> / <c>40097</c> / <c>44990</c> / <c>61003</c> / <c>61004</c> / <c>61011</c>，共 7 支）：与本域订单查询端的 <c>1006xxxx</c> / <c>2684851xx</c> 族完全不同，判错不得跨端点套表。</para>
    /// </remarks>
    [Post("/wxa/sec/order/is_trade_managed")]
    Task<WxaOrderTradeManagedResponse> IsTradeManagedAsync(
        [Body] WxaOrderTradeManagedQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询小程序是否已完成交易结算管理确认（<c>POST /wxa/sec/order/is_trade_management_confirmation_completed</c>，<b>不支持云调用</b>，权限集 18、142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanagementconfirmationcompleted.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>appid</c> 一字段且必填，与 <see cref="IsTradeManagedAsync"/> 共用同一类型），见 <see cref="WxaOrderTradeManagedQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>completed</c>：是否已完成交易结算管理确认，见 <see cref="WxaOrderTradeConfirmationResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；语义是「已对小程序关联的<b>所有</b>商户号都完成订单管理授权或解绑」。</para>
    /// <para>
    /// <b>响应键名不跟随路由</b>：本端点返回 <c>completed</c>，而 <see cref="IsTradeManagedAsync"/> 返回
    /// <c>is_trade_managed</c> ⇒ 两支请求体同形、响应键名各异（守卫 OS4 锁定两个键名，不得对齐）。
    /// </para>
    /// <para><b>业务后果</b>（官方原文）：已完成订单管理授权的商户号，其产生的<b>所有</b>订单均须通过发货信息管理服务发货 ⇒ 该状态为 true 后，未录入发货的订单不会结算。</para>
    /// <para><b>本端点不支持云调用</b>，而其同请求体的 <see cref="IsTradeManagedAsync"/> 支持（<c>wxa.sec.order.isTradeManaged</c>）⇒ 同一对镜像端点在云调用面上也不统一。</para>
    /// </remarks>
    [Post("/wxa/sec/order/is_trade_management_confirmation_completed")]
    Task<WxaOrderTradeConfirmationResponse> IsTradeManagementConfirmationCompletedAsync(
        [Body] WxaOrderTradeManagedQueryRequest request,
        CancellationToken cancellationToken = default);
}
