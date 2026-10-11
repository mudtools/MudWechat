// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「即时配送」域 SDK（15 端点：小程序方 14 + 运力方 1）—— <b>带令牌</b>通道。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 即时配送，2026-10-12 逐页核验）：
/// 已支持配送公司列表 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getallimmedelivery"/>、
/// 申请开通 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_opendelivery"/>、
/// 绑定账号 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_bindlocalaccount"/>、
/// 已绑定账号 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getbindaccount"/>、
/// 预下单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_preaddorder"/>、
/// 下单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addlocalorder"/>、
/// 重新下单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_reorder"/>、
/// 拉取配送单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getlocalorder"/>、
/// 取消 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_cancellocalorder"/>、
/// 预取消 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_precancelorder"/>、
/// 异常件退回确认 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_abnormalconfirm"/>、
/// 添加小费 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addtips"/>、
/// 沙盒模拟状态 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_mockupdateorder"/>、
/// 测试环境模拟状态 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_realmockupdateorder"/>、
/// 运力方更新状态 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-provider/api_updateorder"/>。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 官方声明本族<b>支持第三方平台代商家调用</b>（权限集 id <b>51、71</b>；<c>update_order</c> 官方明示<b>不支持</b>）；
/// 本线按 P3-R2 裁决<b>不</b>引入开放平台令牌链，故仅承载自建形态的单级 <c>access_token</c>。
/// </para>
/// <para>
/// <b>本域与「物流助手 / 微信物流服务」的差异（勿混用）</b>：本域路由前缀为 <c>/cgi-bin/express/local/*</c>，
/// 面向<b>配送公司（即时配送运力）</b>；返回体用 <c>resultcode</c> / <c>resultmsg</c> 而非 <c>errcode</c> / <c>errmsg</c>
/// （见 <see cref="WxaExpressLocalResponse"/>）。
/// </para>
/// <para>
/// <b>敏感字段（MP-X7 同口径）</b>：<c>delivery_sign</c>（appSecret 加密校验串）、<c>phone</c> / <c>rider_phone</c>
/// <b>不得</b>入日志 / 遥测 / 异常消息；SDK 只在 DTO 上承载，不做任何回显。
/// </para>
/// <para>
/// <b>官方接入前置</b>：小程序须完成微信认证 + 开通事件推送（安全模式 / JSON），并在配送公司注册账号后
/// 于小程序后台完成授权绑定。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Express", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaExpressLocalService
{
    /// <summary>
    /// 获取已支持的配送公司列表（<c>POST /cgi-bin/express/local/business/delivery/getall</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getallimmedelivery"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配送公司列表（<c>list</c>：<c>delivery_id</c> / <c>delivery_name</c>），见 <see cref="WxaExpressLocalGetAllDeliveryResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b>，<b>无请求体</b>（官方「请求体 Request Payload」原文为「无」）；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/local/business/delivery/getall")]
    Task<WxaExpressLocalGetAllDeliveryResponse> GetAllDeliveryAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请开通即时配送（<c>POST /cgi-bin/express/local/business/open</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_opendelivery"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运力侧结果（<c>resultcode</c> / <c>resultmsg</c>），见 <see cref="WxaExpressLocalResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，<b>无请求体</b>；Query 携带 <c>access_token</c>。</para>
    /// <para><b>官方约束</b>：只能由<b>第三方服务商</b>调用，为代开发的小程序发起开通；调用成功后小程序管理员收到模板消息完成开通。适用范围为「小程序（仅认证）」。</para>
    /// </remarks>
    [Post("/cgi-bin/express/local/business/open")]
    Task<WxaExpressLocalResponse> OpenDeliveryAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起绑定配送公司账号请求（<c>POST /cgi-bin/express/local/business/shop/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_bindlocalaccount"/>。
    /// </summary>
    /// <param name="request">绑定请求体（<c>delivery_id</c> 必填），见 <see cref="WxaExpressLocalBindAccountRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运力侧结果，见 <see cref="WxaExpressLocalResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。官方约束：只能由第三方服务商调用。</remarks>
    [Post("/cgi-bin/express/local/business/shop/add")]
    Task<WxaExpressLocalResponse> BindAccountAsync(
        [Body] WxaExpressLocalBindAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取已绑定账号（<c>POST /cgi-bin/express/local/business/shop/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getbindaccount"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已绑定的商家签约账号列表（<c>shop_list</c>），见 <see cref="WxaExpressLocalGetBindAccountResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b>，<b>无请求体</b>；Query 携带 <c>access_token</c>。<c>audit_result</c>：<c>0</c> 通过 / <c>1</c> 审核中 / <c>2</c> 不通过。</remarks>
    [Post("/cgi-bin/express/local/business/shop/get")]
    Task<WxaExpressLocalGetBindAccountResponse> GetBindAccountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预下配送单（<c>POST /cgi-bin/express/local/business/order/pre_add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_preaddorder"/>。
    /// </summary>
    /// <param name="request">预下单请求体（与下单同构；<c>delivery_token</c> 由本接口产出、下单时回传），见 <see cref="WxaExpressLocalAddOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运费预估与 <c>delivery_token</c>，见 <see cref="WxaExpressLocalPreAddOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>运力差异（官方原文）</b>：达达返回费用 / 距离 / 预计接单时间与 <c>delivery_token</c>（有效期 3 分钟）；
    /// 顺丰同城、闪送返回费用 / 距离 / 预计接单时间但<b>不支持</b> <c>delivery_token</c>；
    /// 美团配送返回 <c>0</c> 仅表示校验通过。也可不调用本接口直接下单。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/express/local/business/order/pre_add")]
    Task<WxaExpressLocalPreAddOrderResponse> PreAddOrderAsync(
        [Body] WxaExpressLocalAddOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加配送单（<c>POST /cgi-bin/express/local/business/order/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addlocalorder"/>。
    /// </summary>
    /// <param name="request">下单请求体，见 <see cref="WxaExpressLocalAddOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配送单与运费明细，见 <see cref="WxaExpressLocalAddOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>字段适用性</b>：<c>wxa_appid</c>（<see cref="WxaExpressLocalShop.WxaAppId"/>）<b>仅本端点生效</b>；<c>delivery_token</c> 取 <see cref="PreAddOrderAsync"/> 的返回。</para>
    /// </remarks>
    [Post("/cgi-bin/express/local/business/order/add")]
    Task<WxaExpressLocalAddOrderResponse> AddOrderAsync(
        [Body] WxaExpressLocalAddOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重新下单（<c>POST /cgi-bin/express/local/business/order/readd</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_reorder"/>。
    /// </summary>
    /// <param name="request">重新下单请求体（与 <see cref="AddOrderAsync"/> 同构），见 <see cref="WxaExpressLocalAddOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配送单与运费明细，见 <see cref="WxaExpressLocalAddOrderResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/local/business/order/readd")]
    Task<WxaExpressLocalAddOrderResponse> ReAddOrderAsync(
        [Body] WxaExpressLocalAddOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取配送单信息（<c>POST /cgi-bin/express/local/business/order/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getlocalorder"/>。
    /// </summary>
    /// <param name="request">查询请求体（<c>shopid</c> / <c>shop_order_id</c> / <c>shop_no</c> / <c>delivery_sign</c> 必填），见 <see cref="WxaExpressLocalGetOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配送单状态与骑手信息，见 <see cref="WxaExpressLocalGetOrderResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。<c>order_status</c> 枚举见 <see cref="WxaExpressLocalOrderStatus"/>。</remarks>
    [Post("/cgi-bin/express/local/business/order/get")]
    Task<WxaExpressLocalGetOrderResponse> GetOrderAsync(
        [Body] WxaExpressLocalGetOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消配送单（<c>POST /cgi-bin/express/local/business/order/cancel</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_cancellocalorder"/>。
    /// </summary>
    /// <param name="request">取消请求体（<c>cancel_reason_id</c> / <c>cancel_reason</c> 必填），见 <see cref="WxaExpressLocalCancelOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>扣除的违约金与说明，见 <see cref="WxaExpressLocalCancelOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>各家取消规则（官方原文）</b>：顺丰同城急送 / 闪送 / 美团配送——配送完成前任意节点可取消；达达——骑手取货之前可取消。可先用 <see cref="PreCancelOrderAsync"/> 预检违约金。</para>
    /// </remarks>
    [Post("/cgi-bin/express/local/business/order/cancel")]
    Task<WxaExpressLocalCancelOrderResponse> CancelOrderAsync(
        [Body] WxaExpressLocalCancelOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预取消配送单（<c>POST /cgi-bin/express/local/business/order/precancel</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_precancelorder"/>。
    /// </summary>
    /// <param name="request">预取消请求体（<c>cancel_reason_id</c> / <c>cancel_reason</c> 可选），见 <see cref="WxaExpressLocalCancelOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预计扣除的违约金与说明，见 <see cref="WxaExpressLocalCancelOrderResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。官方场景：正式取消前先查询该订单是否可取消及需扣费用。</remarks>
    [Post("/cgi-bin/express/local/business/order/precancel")]
    Task<WxaExpressLocalCancelOrderResponse> PreCancelOrderAsync(
        [Body] WxaExpressLocalCancelOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 异常件退回商家确认（<c>POST /cgi-bin/express/local/business/order/confirm_return</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_abnormalconfirm"/>。
    /// </summary>
    /// <param name="request">确认请求体（<c>shopid</c> / <c>shop_order_id</c> / <c>waybill_id</c> / <c>delivery_sign</c> / <c>shop_no</c> 必填），见 <see cref="WxaExpressLocalConfirmReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运力侧结果，见 <see cref="WxaExpressLocalResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。官方场景：配送异常、骑手把货物退还商家，商家收货后调用本接口确认。</remarks>
    [Post("/cgi-bin/express/local/business/order/confirm_return")]
    Task<WxaExpressLocalResponse> ConfirmReturnAsync(
        [Body] WxaExpressLocalConfirmReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加小费（<c>POST /cgi-bin/express/local/business/order/addtips</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addtips"/>。
    /// </summary>
    /// <param name="request">加小费请求体（<c>tips</c> / <c>remark</c> 必填），见 <see cref="WxaExpressLocalAddTipsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运力侧结果，见 <see cref="WxaExpressLocalResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>官方约束</b>：仅可对<b>待接单状态</b>订单加小费；小费以<b>最新一次</b>为准 ⇒ 下次金额必须<b>大于</b>上一次；
    /// 美团配送不支持；闪送须按固定档位（2/3/5/10/15/20/50/100）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/express/local/business/order/addtips")]
    Task<WxaExpressLocalResponse> AddTipsAsync(
        [Body] WxaExpressLocalAddTipsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 沙盒环境模拟配送公司更新配送单状态（<c>POST /cgi-bin/express/local/business/test_update_order</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_mockupdateorder"/>。
    /// </summary>
    /// <param name="request">模拟请求体（<c>shopid</c> 必须为 <c>test_shop_id</c>；<b>不传</b> <c>delivery_sign</c>），见 <see cref="WxaExpressLocalMockUpdateOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运力侧结果，见 <see cref="WxaExpressLocalResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。<b>官方原文</b>：只用于沙盒环境，订单<b>不会</b>真实流转到运力方。</remarks>
    [Post("/cgi-bin/express/local/business/test_update_order")]
    Task<WxaExpressLocalResponse> MockUpdateOrderAsync(
        [Body] WxaExpressLocalMockUpdateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试环境模拟更新配送单状态（<c>POST /cgi-bin/express/local/business/realmock_update_order</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_realmockupdateorder"/>。
    /// </summary>
    /// <param name="request">模拟请求体（<c>delivery_sign</c> <b>必填</b>），见 <see cref="WxaExpressLocalMockUpdateOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运力侧结果，见 <see cref="WxaExpressLocalResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>官方约束</b>：只能用于测试，请求会转发到<b>运力测试环境</b>，目前仅支持顺丰同城与达达（须用官方提供的测试账号）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/local/business/realmock_update_order")]
    Task<WxaExpressLocalResponse> RealMockUpdateOrderAsync(
        [Body] WxaExpressLocalMockUpdateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 运力方更新配送单状态（<c>POST /cgi-bin/express/local/delivery/update_order</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-provider/api_updateorder"/>。
    /// </summary>
    /// <param name="request">状态更新请求体（<c>wx_token</c> / <c>order_status</c> / <c>waybill_id</c> / <c>action_time</c> / <c>shopid</c> / <c>shop_order_id</c> / <c>wxa_path</c> 必填），见 <see cref="WxaExpressLocalUpdateOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运力侧结果，见 <see cref="WxaExpressLocalResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；本端点<b>不支持</b>第三方平台代调用。</para>
    /// <para>
    /// <b><c>wx_token</c> 语义（勿填错）</b>：官方原文为「下单事件中推送的 token 字段」，
    /// 与物流助手散单寄件的 <c>token</c> 同源，<b>不是</b>应用级令牌。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/express/local/delivery/update_order")]
    Task<WxaExpressLocalResponse> UpdateOrderStatusAsync(
        [Body] WxaExpressLocalUpdateOrderRequest request,
        CancellationToken cancellationToken = default);
}
