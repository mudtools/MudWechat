// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「非税支付」域 SDK
/// （缴费查询 / 下单 / 退单 / 车辆场景 / 票据卡券——官方「非税支付」分组的 JSON 端点；
/// <c>downloadbill</c> 成功响应为 CSV 文本，走 <c>IMpNontaxBillDownloadService</c> 独立下载通道，不在本接口）。
/// </summary>
/// <remarks>
/// <para>
/// <b>业务准入</b>：非税支付需与财政局签约开通专项服务（<c>service_id</c>）；
/// 全部端点适用范围为「公众号 / 服务号」，且金额单位一律为<b>分</b>。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信公众号官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；URL 遥测已由组件 <c>SensitiveUrlRedactor</c> 与 <c>MpException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Nontax", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IMpNontaxService
{
    /// <summary>
    /// 查询欠费。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.1"/>
    /// （官方接口英文名 <c>queryfee</c>）。
    /// </summary>
    /// <param name="request">查询请求（通知书号 + 执收单位 + 地区，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应缴金额与收费明细。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/queryfee")]
    Task<MpNontaxQueryFeeResponse> QueryFeeAsync(
        [Body] MpNontaxQueryFeeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 统一下单。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.2"/>
    /// （官方接口英文名 <c>unifiedorder</c>）。
    /// </summary>
    /// <param name="request">下单请求（明细 / 金额 / 场景必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>非税订单号 + 支付跳转地址（H5）或小程序调起信息（小程序）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；同一通知书重复下单须先查 <see cref="GetOrderListAsync"/> 防止重复缴费。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/unifiedorder")]
    Task<MpNontaxUnifiedOrderResponse> UnifiedOrderAsync(
        [Body] MpNontaxUnifiedOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询订单。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.3"/>
    /// （官方接口英文名 <c>getorder</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单全量信息（状态 / 支付 / 退款 / 通知历史）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/getorder")]
    Task<MpNontaxGetOrderResponse> GetOrderAsync(
        [Body] MpNontaxGetOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 退款。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.4"/>
    /// （官方接口英文名 <c>refund</c>）。
    /// </summary>
    /// <param name="request">退款请求（<c>order_id</c> + <c>reason</c> 必填；部分退款另填 <c>refund_fee</c>/<c>refund_out_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信退款单号。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。<b>覆盖删除语义</b>：退款为资金逆向操作，原支付订单进入退款流程不可撤回。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/refund")]
    Task<MpNontaxRefundResponse> RefundAsync(
        [Body] MpNontaxRefundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询订单列表。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.9"/>
    /// （官方接口英文名 <c>getorderlist</c>；<c>order_no</c> 与 <c>payment_notice_no</c> 二选一）。
    /// </summary>
    /// <param name="request">查询请求（执收单位 + 地区必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>非税订单号列表与已支付订单号。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/getorderlist")]
    Task<MpNontaxGetOrderListResponse> GetOrderListAsync(
        [Body] MpNontaxGetOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信支付（被扫）。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.9"/>
    /// （官方接口英文名 <c>micropay</c>；商户扫用户付款码完成缴费）。
    /// </summary>
    /// <param name="request">支付请求（<c>auth_code</c> 扫码授权码必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>非税订单号。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/micropay")]
    Task<MpNontaxMicroPayResponse> MicroPayAsync(
        [Body] MpNontaxMicroPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通知不一致订单。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.6"/>
    /// （官方接口英文名 <c>notifyinconsistentorder</c>；缴费状态与财政侧不一致时上报）。
    /// </summary>
    /// <param name="request">通知请求（<c>order_id</c> 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/notifyinconsistentorder")]
    Task<MpResponse> NotifyInconsistentOrderAsync(
        [Body] MpNontaxNotifyInconsistentOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 模拟通知（联调工具）。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.7"/>
    /// （官方接口英文名 <c>mocknotification</c>）。
    /// </summary>
    /// <param name="request">模拟请求（回调地址必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；仅联调环境使用（官方提供的通知回调验证工具面）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/mocknotification")]
    Task<MpResponse> MockNotificationAsync(
        [Body] MpNontaxMockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 模拟查询欠费（联调工具）。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#6.1.8"/>
    /// （官方接口英文名 <c>mockqueryfee</c>）。
    /// </summary>
    /// <param name="request">模拟请求（回调地址必填；与模拟通知同形）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/mockqueryfee")]
    Task<MpResponse> MockQueryFeeAsync(
        [Body] MpNontaxMockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实名信息。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/API_document.html#9.3"/>
    /// （官方接口英文名 <c>getrealname</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>wx_realname_token</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>加密后的实名数据与算法标识（<b>解密由调用方执行</b>，SDK 不持密钥）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；响应字段均为加密后数据，密钥由调用方与财政局约定。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/getrealname")]
    Task<MpNontaxGetRealNameResponse> GetRealNameAsync(
        [Body] MpNontaxGetRealNameRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询车辆状态。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/Non-tax_driver_platform.html#2.1"/>
    /// （官方接口英文名 <c>vehicle querystate</c>；<c>plate_number</c> 与 <c>openid</c> 二选一）。
    /// </summary>
    /// <param name="request">查询请求（地区 + 交易场景必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户状态与授权信息。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/vehicle/querystate")]
    Task<MpNontaxVehicleQueryStateResponse> QueryVehicleStateAsync(
        [Body] MpNontaxVehicleQueryStateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 车辆入场通知。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/Non-tax_driver_platform.html#2.2"/>
    /// （官方接口英文名 <c>vehicle entrancenotify</c>）。
    /// </summary>
    /// <param name="request">入场通知请求（<c>scene_info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户状态。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/vehicle/entrancenotify")]
    Task<MpNontaxVehicleEntranceNotifyResponse> VehicleEntranceNotifyAsync(
        [Body] MpNontaxVehicleEntranceNotifyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 车辆支付申请。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Non_tax_pay/Non-tax_driver_platform.html#2.3"/>
    /// （官方接口英文名 <c>vehicle payapply</c>）。
    /// </summary>
    /// <param name="request">支付申请请求（<c>scene_info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>非税订单号。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/vehicle/payapply")]
    Task<MpNontaxVehiclePayApplyResponse> VehiclePayApplyAsync(
        [Body] MpNontaxVehiclePayApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建票据卡券。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Invoice/Nontax_Bill/API_list.html#3.2"/>
    /// （官方接口英文名 <c>createbillcard</c>）。
    /// </summary>
    /// <param name="request">创建请求（<c>invoice_info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>票据卡券 ID（<c>card_id</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；卡券面创建后经 <see cref="InsertBillAsync"/> 插入用户卡包。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/createbillcard")]
    Task<MpNontaxCreateBillCardResponse> CreateBillCardAsync(
        [Body] MpNontaxCreateBillCardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 插入票据。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Invoice/Nontax_Bill/API_list.html#3.5"/>
    /// （官方接口英文名 <c>insertbill</c>）。
    /// </summary>
    /// <param name="request">插卡请求（<c>order_id</c> + <c>card_id</c> + <c>card_ext</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>卡券 Code 与用户 OpenId。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/insertbill")]
    Task<MpNontaxInsertBillResponse> InsertBillAsync(
        [Body] MpNontaxInsertBillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取票据授权链接。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Invoice/Nontax_Bill/API_list.html#2.2"/>
    /// （官方接口英文名 <c>getbillauthurl</c>）。
    /// </summary>
    /// <param name="request">授权请求（<c>ticket</c> 为官方 <c>api_ticket</c>，由票据基座承载）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权链接与有效期（秒）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/nontax/getbillauthurl")]
    Task<MpNontaxGetBillAuthUrlResponse> GetBillAuthUrlAsync(
        [Body] MpNontaxGetBillAuthUrlRequest request,
        CancellationToken cancellationToken = default);
}
