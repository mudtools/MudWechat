// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「物流助手 · 商家寄件」域 SDK（12 端点）—— <b>带令牌</b>通道。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 物流 → 商家寄件，2026-10-11 逐页核验）：
/// 下单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_addorder"/>、
/// 查询 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getorder"/>、
/// 批量查询 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_batchgetorder"/>、
/// 取消 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_cancelorder"/>、
/// 轨迹 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getpath"/>、
/// 快递公司列表 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getdelivery"/>、
/// 快递账号列表 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getaccount"/>、
/// 绑定账号 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_bindaccount"/>、
/// 打印员列表 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getprinter"/>、
/// 绑定打印员 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_updateprinter"/>、
/// 面单余额 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getquota"/>、
/// 模拟轨迹 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_testupdateorder"/>。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// <b>GET 端点（delivery/getall、account/getall、printer/getall）</b>无请求体。
/// </para>
/// <para>
/// <b>业务警示</b>：下单须已绑定快递账号且开通下单权限；取消只能取消<b>待揽件</b>运单；
/// <c>test_update_order</c> 仅供<b>测试环境</b>（生产不可调用）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Express", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaExpressBusinessService
{
    /// <summary>
    /// 商家寄件下单（<c>POST /cgi-bin/express/business/order/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_addorder"/>。
    /// </summary>
    /// <param name="request">下单请求体（<c>order_id</c>/<c>openid</c>/<c>delivery_id</c>/<c>biz_id</c>/<c>add_source</c>/发收件人/包裹/商品/保价/服务必填），见 <see cref="WxaExpressAddOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运单号与面单数据（<c>order_id</c>/<c>waybill_id</c>/<c>waybill_data</c>），见 <see cref="WxaExpressAddOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>前置与频率（官方原文）</b>：调用前须已绑定快递账号且开通下单权限；官方建议单商户日下单量超
    /// 10 万时联系快递公司申请提额。<c>add_source=2</c>（App/AppH5 订单）时不发送物流服务通知；
    /// <c>expect_time</c> 顺丰必传，0 表示已事先约定取件时间；选择预期揽件时间后<b>不要自己打单</b>。
    /// 平台型小程序 <c>tagid</c> 须与入驻方账号一一对应。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>40097</c>（参数不完整）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/business/order/add")]
    Task<WxaExpressAddOrderResponse> AddOrderAsync(
        [Body] WxaExpressAddOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询运单信息（<c>POST /cgi-bin/express/business/order/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getorder"/>。
    /// </summary>
    /// <param name="request">查询请求体（<c>order_id</c>/<c>openid</c>/<c>delivery_id</c> 必填），见 <see cref="WxaExpressGetOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运单与面单信息（含 <c>print_html</c>），见 <see cref="WxaExpressGetOrderResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。下单成功后会收到发货通知事件（推送 <c>waybill_id</c>），凭 <c>order_id</c> 查询。</remarks>
    [Post("/cgi-bin/express/business/order/get")]
    Task<WxaExpressGetOrderResponse> GetOrderAsync(
        [Body] WxaExpressGetOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量查询运单信息（<c>POST /cgi-bin/express/business/order/batchget</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_batchgetorder"/>。
    /// </summary>
    /// <param name="request">批量查询请求体（<c>order_list</c> 必填，单次最多 50 单），见 <see cref="WxaExpressBatchGetOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐单运单信息（含逐单 <c>errcode</c>/<c>errmsg</c> 信封），见 <see cref="WxaExpressBatchGetOrderResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；单次最多 50 单。</remarks>
    [Post("/cgi-bin/express/business/order/batchget")]
    Task<WxaExpressBatchGetOrderResponse> BatchGetOrderAsync(
        [Body] WxaExpressBatchGetOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消运单（<c>POST /cgi-bin/express/business/order/cancel</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_cancelorder"/>。
    /// </summary>
    /// <param name="request">取消请求体（<c>order_id</c>/<c>openid</c>/<c>delivery_id</c> 必填），见 <see cref="WxaExpressCancelOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>快递侧取消结果，见 <see cref="WxaExpressCancelOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>业务限制（官方原文）</b>：只能取消<b>待揽件</b>运单，揽件成功或运输中的运单无法取消。</para>
    /// </remarks>
    [Post("/cgi-bin/express/business/order/cancel")]
    Task<WxaExpressCancelOrderResponse> CancelOrderAsync(
        [Body] WxaExpressCancelOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询物流轨迹（<c>POST /cgi-bin/express/business/path/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getpath"/>。
    /// </summary>
    /// <param name="request">轨迹查询请求体（<c>openid</c>/<c>delivery_id</c>/<c>waybill_id</c> 必填），见 <see cref="WxaExpressGetPathRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>轨迹列表（<c>path_item_list</c>），见 <see cref="WxaExpressGetPathResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；<c>action_type</c> 枚举值同附录轨迹类型。</remarks>
    [Post("/cgi-bin/express/business/path/get")]
    Task<WxaExpressGetPathResponse> GetPathAsync(
        [Body] WxaExpressGetPathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取快递公司列表（<c>GET /cgi-bin/express/business/delivery/getall</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getdelivery"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>快递公司列表（<c>data</c>/<c>count</c>），见 <see cref="WxaExpressGetAllDeliveryResponse"/>。</returns>
    /// <remarks>官方契约：<b>GET</b>，无请求体；Query 携带 <c>access_token</c>；<c>can_use_cash</c> 表示是否支持现金支付、<c>can_get_quota</c> 表示是否支持电子面单。</remarks>
    [Get("/cgi-bin/express/business/delivery/getall")]
    Task<WxaExpressGetAllDeliveryResponse> GetAllDeliveryAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取快递账号列表（<c>GET /cgi-bin/express/business/account/getall</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getaccount"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>快递账号列表（<c>list</c>/<c>count</c>），见 <see cref="WxaExpressGetAllAccountResponse"/>。</returns>
    /// <remarks>官方契约：<b>GET</b>，无请求体；Query 携带 <c>access_token</c>。</remarks>
    [Get("/cgi-bin/express/business/account/getall")]
    Task<WxaExpressGetAllAccountResponse> GetAllAccountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 绑定 / 解绑快递账号（<c>POST /cgi-bin/express/business/account/bind</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_bindaccount"/>。
    /// </summary>
    /// <param name="request">绑定 / 解绑请求体（<c>type</c>/<c>biz_id</c>/<c>delivery_id</c> 必填），见 <see cref="WxaExpressBindAccountRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示受理成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>异步绑定（官方原文）</b>：<c>type=bind</c> 绑定（须传入快递公司签约的 <c>password</c>）、<c>type=unbind</c> 解绑；绑定为<b>异步</b>流程，返回结果仅表示受理，最终状态经回调/查询确认。</para>
    /// </remarks>
    [Post("/cgi-bin/express/business/account/bind")]
    Task<WxaResponse> BindAccountAsync(
        [Body] WxaExpressBindAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取打印员列表（<c>GET /cgi-bin/express/business/printer/getall</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getprinter"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>打印员 OpenId 列表（<c>openid</c>/<c>tagid_list</c>/<c>count</c>），见 <see cref="WxaExpressGetPrinterResponse"/>。</returns>
    /// <remarks>官方契约：<b>GET</b>，无请求体；Query 携带 <c>access_token</c>。</remarks>
    [Get("/cgi-bin/express/business/printer/getall")]
    Task<WxaExpressGetPrinterResponse> GetAllPrinterAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 绑定 / 解绑打印员（<c>POST /cgi-bin/express/business/printer/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_updateprinter"/>。
    /// </summary>
    /// <param name="request">绑定 / 解绑请求体（<c>update_type</c>/<c>openid</c> 必填），见 <see cref="WxaExpressUpdatePrinterRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>业务语义（官方原文）</b>：<c>update_type=bind</c> 绑定 / <c>unbind</c> 解绑；绑定后商家端下单可指定打印员（快递员）接收面单打印通知。</para>
    /// </remarks>
    [Post("/cgi-bin/express/business/printer/update")]
    Task<WxaResponse> UpdatePrinterAsync(
        [Body] WxaExpressUpdatePrinterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查看电子面单余额（<c>POST /cgi-bin/express/business/quota/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getquota"/>。
    /// </summary>
    /// <param name="request">余额查询请求体（<c>delivery_id</c>/<c>biz_id</c> 必填），见 <see cref="WxaExpressGetQuotaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>电子面单余额（<c>quota_num</c>），见 <see cref="WxaExpressGetQuotaResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；<c>quota_num</c> 为 -1 表示不限制。</remarks>
    [Post("/cgi-bin/express/business/quota/get")]
    Task<WxaExpressGetQuotaResponse> GetQuotaAsync(
        [Body] WxaExpressGetQuotaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 模拟快递公司更新订单状态（<c>POST /cgi-bin/express/business/test_update_order</c>，<b>仅测试环境</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_testupdateorder"/>。
    /// </summary>
    /// <param name="request">模拟轨迹请求体（<c>biz_id</c>/<c>order_id</c>/<c>delivery_id</c>/<c>waybill_id</c>/轨迹字段 必填），见 <see cref="WxaExpressTestUpdateOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>测试专用（官方原文）</b>：日常测试时调用本接口模拟快递公司推送轨迹，观察商户侧回调联调；生产环境不可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/express/business/test_update_order")]
    Task<WxaResponse> TestUpdateOrderAsync(
        [Body] WxaExpressTestUpdateOrderRequest request,
        CancellationToken cancellationToken = default);
}
