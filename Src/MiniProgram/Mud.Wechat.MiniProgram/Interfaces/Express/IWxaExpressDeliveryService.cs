// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「物流助手 · 服务商侧 / 物流消息 / 退货组件」域 SDK（26 端点）—— <b>带令牌</b>通道。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（2026-10-11 逐页核验）：
/// 面单联系人 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact"/>、
/// 面单预览 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_previewtemplate"/>、
/// 订单审核 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatebusiness"/>、
/// 轨迹同步 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatepath"/>、
/// 无忧退 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_addreturnid"/>（及 get/unbind）、
/// 物流消息 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_followwaybill"/>（及 query/update/get_delivery_list）、
/// 物流查询插件 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_tracewaybill"/>（及 query_trace/update_waybill_goods）、
/// 退货组件 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_addreturn"/>（及 get/unbind）、
/// 散单寄件 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderstatus"/>（及 fee/cancel/refund/evaluate_feedback）。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 四个「开通类」端点（<c>open_openmsg</c> / <c>open_query_plugin</c> / <c>open_return</c> / <c>get_delivery_list</c>）
/// 请求体为<b>空对象</b>（无字段）。
/// </para>
/// <para>
/// <b>业务警示</b>：本域大多面向<b>微信物流服务商（服务商代调用）</b>；散单寄件端点（<c>single_waybill/*</c>）
/// 以<b>商户侧下单事件中推送的 Token</b> 作为调用凭据（非模糊查询令牌）；<c>get_bill</c> 对账文件端点
/// 返回<b>文件内容（非 JSON）</b>，走独立通道 <see cref="IWxaExpressBillService"/>（不在本接口）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Express", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaExpressDeliveryService
{
    /// <summary>
    /// 获取面单联系人信息（<c>POST /cgi-bin/express/delivery/contact/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact"/>。
    /// </summary>
    /// <param name="request">联系人查询请求体（<c>token</c>/<c>waybill_id</c> 必填），见 <see cref="WxaExpressGetContactRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>面单发收件人（<c>sender</c>/<c>receiver</c>），见 <see cref="WxaExpressGetContactResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>本组接口面向<b>微信物流服务商</b>：<c>token</c> 为商户侧下单事件中推送的 Token 字段（非模糊查询令牌）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/contact/get")]
    Task<WxaExpressGetContactResponse> GetContactAsync(
        [Body] WxaExpressGetContactRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预览面单模板（<c>POST /cgi-bin/express/delivery/template/preview</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_previewtemplate"/>。
    /// </summary>
    /// <param name="request">面单预览请求体（<c>waybill_id</c>/<c>waybill_template</c>/<c>waybill_data</c>/<c>custom</c> 必填），见 <see cref="WxaExpressPreviewTemplateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>渲染后的面单 HTML（<c>rendered_waybill_template</c>），见 <see cref="WxaExpressPreviewTemplateResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>服务商自定义面单模板时调用，用于预览渲染效果；<c>waybill_template</c> 为需 Base64 编码的 HTML 模板内容。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/template/preview")]
    Task<WxaExpressPreviewTemplateResponse> PreviewTemplateAsync(
        [Body] WxaExpressPreviewTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商更新订单审核结果（<c>POST /cgi-bin/express/delivery/service/business/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatebusiness"/>。
    /// </summary>
    /// <param name="request">审核结果回传请求体（<c>shop_app_id</c>/<c>biz_id</c>/<c>result_code</c> 必填），见 <see cref="WxaExpressUpdateBusinessRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>服务商承接商户绑定时异步审核（账号绑定为异步流程），审核完成后调用本接口把结果回传微信侧。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/service/business/update")]
    Task<WxaResponse> UpdateBusinessAsync(
        [Body] WxaExpressUpdateBusinessRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商更新运单轨迹（<c>POST /cgi-bin/express/delivery/path/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatepath"/>。
    /// </summary>
    /// <param name="request">轨迹同步请求体（<c>token</c>/<c>waybill_id</c>/轨迹字段 必填），见 <see cref="WxaExpressUpdatePathRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>服务商（或运力）侧订单状态变化时调用，将轨迹实时同步给微信物流助手；<c>action_type</c> 枚举值同附录 action_type 定义。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/path/update")]
    Task<WxaResponse> UpdatePathAsync(
        [Body] WxaExpressUpdatePathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建退货 ID（无忧退货运费险，<c>POST /cgi-bin/express/delivery/no_worry_return/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_addreturnid"/>。
    /// </summary>
    /// <param name="request">创建退货请求体（<c>shop_order_id</c>/<c>biz_addr</c>/<c>openid</c>/<c>order_path</c>/<c>goods_list</c>/<c>order_price</c>/<c>wx_pay_id</c> 必填），见 <see cref="WxaExpressAddReturnIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退货 ID（<c>return_id</c>），见 <see cref="WxaExpressAddReturnIdResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <c>shop_order_id</c> 与退货 ID 一一对应；“一个订单需要多次退货”可在商家内部用 1 个退货订单号映射多个 <c>shop_order_id</c>。
    /// <c>wx_pay_id</c> 为<b>已投保的微信支付单号</b>（必填）：无该字段返回 9300569，未投保即 9300570。
    /// 创建退货 ID 时平台自动下发模板消息给用户提醒退货。
    /// </para>
    /// <para>官方错误码：<c>40097</c>（order_id 或退货地址或 wx_pay_id 为空）、<c>9300522</c>（shop_order_id 已存在）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/no_worry_return/add")]
    Task<WxaExpressAddReturnIdResponse> AddNoWorryReturnAsync(
        [Body] WxaExpressAddReturnIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询退货 ID 状态（无忧退货运费险，<c>POST /cgi-bin/express/delivery/no_worry_return/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_getreturnid"/>。
    /// </summary>
    /// <param name="request">退货状态查询请求体（<c>return_id</c> 必填），见 <see cref="WxaExpressGetReturnIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退货状态（<c>status</c>/<c>waybill_id</c>/<c>order_status</c> 等），见 <see cref="WxaExpressReturnStatusResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/delivery/no_worry_return/get")]
    Task<WxaExpressReturnStatusResponse> GetNoWorryReturnAsync(
        [Body] WxaExpressGetReturnIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解绑退货 ID（无忧退货运费险，<c>POST /cgi-bin/express/delivery/no_worry_return/unbind</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_unbindreturnid"/>。
    /// </summary>
    /// <param name="request">解绑退货请求体（<c>return_id</c> 必填），见 <see cref="WxaExpressUnbindReturnIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>与用户达成协商「无需退货」时解除商家退货单与退货 ID 的绑定；仅当用户自主填写运单号时才支持解绑。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/no_worry_return/unbind")]
    Task<WxaResponse> UnbindNoWorryReturnAsync(
        [Body] WxaExpressUnbindReturnIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户绑定运单（物流消息，<c>POST /cgi-bin/express/delivery/open_msg/follow_waybill</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_followwaybill"/>。
    /// </summary>
    /// <param name="request">绑定运单请求体（<c>openid</c>/<c>waybill_id</c>/<c>goods_info</c> 必填），见 <see cref="WxaExpressFollowWaybillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运单 Token（<c>waybill_token</c>），见 <see cref="WxaExpressFollowWaybillResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>频率上限（官方原文）</b>：同一 appid 最多调用 <b>10 万次/日</b>；同一用户最多被调用 <b>50 次/日</b>；
    /// 同一用户最多同时绑定 <b>100 个</b>运单号。绑定后向该用户下发物流状态通知。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/open_msg/follow_waybill")]
    Task<WxaExpressFollowWaybillResponse> FollowWaybillAsync(
        [Body] WxaExpressFollowWaybillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询运单轨迹（物流消息，<c>POST /cgi-bin/express/delivery/open_msg/query_follow_trace</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_queryfollowtrace"/>。
    /// </summary>
    /// <param name="request">轨迹查询请求体（<c>waybill_token</c>/<c>openid</c> 必填），见 <see cref="WxaExpressQueryFollowTraceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运单轨迹明细（<c>waybill_info</c>/<c>shop_info</c>/<c>delivery_info</c>），见 <see cref="WxaExpressTraceResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；凭 <see cref="WxaExpressFollowWaybillResponse.WaybillToken"/> 查询。</remarks>
    [Post("/cgi-bin/express/delivery/open_msg/query_follow_trace")]
    Task<WxaExpressTraceResponse> QueryFollowTraceAsync(
        [Body] WxaExpressQueryFollowTraceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新运单商品信息（物流消息，<c>POST /cgi-bin/express/delivery/open_msg/update_follow_waybill_goods</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_updatefollowwaybillgood"/>。
    /// </summary>
    /// <param name="request">商品更新请求体（<c>waybill_token</c>/<c>openid</c>/<c>goods_info</c> 必填），见 <see cref="WxaExpressUpdateFollowWaybillGoodsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/delivery/open_msg/update_follow_waybill_goods")]
    Task<WxaResponse> UpdateFollowWaybillGoodsAsync(
        [Body] WxaExpressUpdateFollowWaybillGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开通物流消息（<c>POST /cgi-bin/express/delivery/open_msg/open_openmsg</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/logistics-service/applyMsgPlugin.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 空请求体；Query 携带 <c>access_token</c>。</para>
    /// <para>开通后用户可绑定运单并接收物流状态通知；须具备相应接口权限集（45）。错误码 <c>933000</c>~<c>933004</c>。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/open_msg/open_openmsg")]
    Task<WxaResponse> OpenOpenMessageAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开通物流查询插件（<c>POST /cgi-bin/express/delivery/open_msg/open_query_plugin</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/logistics-service/applyQueryPlugin.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 空请求体；Query 携带 <c>access_token</c>。</para>
    /// <para>开通后用户可凭运单号 + 手机号查询物流轨迹（<c>trace_waybill</c> / <c>query_trace</c>）。须具备相应接口权限集（45）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/open_msg/open_query_plugin")]
    Task<WxaResponse> OpenQueryPluginAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询运单信息（物流查询插件，<c>POST /cgi-bin/express/delivery/open_msg/trace_waybill</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_tracewaybill"/>。
    /// </summary>
    /// <param name="request">运单查询请求体（<c>openid</c>/<c>waybill_id</c>/<c>goods_info</c>/<c>delivery_id</c> 必填），见 <see cref="WxaExpressTraceWaybillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运单 Token（<c>waybill_token</c>），见 <see cref="WxaExpressTraceWaybillResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>delivery_id</c> 必填（无单号无法匹配快递公司时可用 single-delivery 场景的运单绑定先转正）；返回 <c>waybill_token</c> 供后续查询。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/open_msg/trace_waybill")]
    Task<WxaExpressTraceWaybillResponse> TraceWaybillAsync(
        [Body] WxaExpressTraceWaybillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询运单轨迹（物流查询插件，<c>POST /cgi-bin/express/delivery/open_msg/query_trace</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_querytrace"/>。
    /// </summary>
    /// <param name="request">轨迹查询请求体（<c>waybill_token</c>/<c>openid</c> 必填），见 <see cref="WxaExpressQueryTraceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运单轨迹明细，见 <see cref="WxaExpressTraceResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>频率上限（官方原文）</b>：一个 appid 最多调用 <b>10 万次/日</b>；一个用户最多调用 <b>100 次/日</b>。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/open_msg/query_trace")]
    Task<WxaExpressTraceResponse> QueryTraceAsync(
        [Body] WxaExpressQueryTraceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新运单商品信息（物流查询插件，<c>POST /cgi-bin/express/delivery/open_msg/update_waybill_goods</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_updatewaybillgoods"/>。
    /// </summary>
    /// <param name="request">商品更新请求体（<c>waybill_token</c>/<c>openid</c>/<c>goods_info</c> 必填），见 <see cref="WxaExpressUpdateWaybillGoodsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/delivery/open_msg/update_waybill_goods")]
    Task<WxaResponse> UpdateWaybillGoodsAsync(
        [Body] WxaExpressUpdateWaybillGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取快递公司列表（物流消息 / 查询插件共用，<c>POST /cgi-bin/express/delivery/open_msg/get_delivery_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_getdeliverylist"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支持物流消息 / 查询能力的快递公司列表（<c>delivery_list</c>/<c>count</c>），见 <see cref="WxaExpressGetDeliveryListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 空请求体；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/delivery/open_msg/get_delivery_list")]
    Task<WxaExpressGetDeliveryListResponse> GetDeliveryListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开通退货组件（<c>POST /cgi-bin/express/delivery/return/open_return</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/logistics-service/applyReturnPlugin.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 空请求体；Query 携带 <c>access_token</c>。</para>
    /// <para>开通后可调用退货组件相关接口（<c>return/add</c>/<c>get</c>/<c>unbind</c>）。调用额度 <b>500 次/天</b>；须具备接口权限集（45）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/return/open_return")]
    Task<WxaResponse> OpenReturnAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建退货 ID（退货组件，<c>POST /cgi-bin/express/delivery/return/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_addreturn"/>。
    /// </summary>
    /// <param name="request">创建退货请求体（<c>shop_order_id</c>/<c>biz_addr</c>/<c>openid</c>/<c>order_path</c>/<c>goods_list</c>/<c>order_price</c> 必填），见 <see cref="WxaExpressAddReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退货 ID（<c>return_id</c>），见 <see cref="WxaExpressAddReturnResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// 与无忧退货运费险不同：<b>本族无 <c>wx_pay_id</c></b>（不要求保单）；<c>shop_order_id</c> 与退货 ID 一一对应，
    /// “一个订单需要多次退货”时可在商家内部 1 个退货订单号映射多个 <c>shop_order_id</c>。
    /// </para>
    /// <para>官方错误码：<c>40097</c>（order_id 或退货地址为空）、<c>9300522</c>（shop_order_id 已存在）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/return/add")]
    Task<WxaExpressAddReturnResponse> AddReturnAsync(
        [Body] WxaExpressAddReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询退货 ID 状态（退货组件，<c>POST /cgi-bin/express/delivery/return/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_getreturn"/>。
    /// </summary>
    /// <param name="request">退货状态查询请求体（<c>return_id</c> 必填），见 <see cref="WxaExpressGetReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退货状态，见 <see cref="WxaExpressReturnStatusResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/delivery/return/get")]
    Task<WxaExpressReturnStatusResponse> GetReturnAsync(
        [Body] WxaExpressGetReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解绑退货 ID（退货组件，<c>POST /cgi-bin/express/delivery/return/unbind</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_unbindreturn"/>。
    /// </summary>
    /// <param name="request">解绑退货请求体（<c>return_id</c> 必填），见 <see cref="WxaExpressUnbindReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>商家同意退货后与用户达成协商「无需退货」时解除绑定；仅当用户自主填写运单号（自行寄回）时才支持解绑。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/return/unbind")]
    Task<WxaResponse> UnbindReturnAsync(
        [Body] WxaExpressUnbindReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 运力更新订单状态（散单寄件，<c>POST /cgi-bin/express/delivery/single_waybill/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderstatus"/>。
    /// </summary>
    /// <param name="request">订单状态更新请求体（<c>token</c>/<c>action_time</c>/<c>action_type</c>/<c>action_msg</c>/取派件员信息 必填），见 <see cref="WxaExpressSingleWaybillUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>action_type</c> 枚举见附录（<c>90001</c> 网点接单 / <c>90002</c> 分配业务员 / <c>100001</c> 揽件成功 …）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/single_waybill/update")]
    Task<WxaResponse> UpdateSingleWaybillAsync(
        [Body] WxaExpressSingleWaybillUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 运力更新订单费用（散单寄件，<c>POST /cgi-bin/express/delivery/single_waybill/fee</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderfee"/>。
    /// </summary>
    /// <param name="request">费用更新请求体（<c>token</c>/<c>waybill_id</c>/<c>need_pay</c>/费用字段 必填），见 <see cref="WxaExpressSingleWaybillFeeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>所有费用字段单位均为<b>分</b>；<c>need_pay</c>：0 表示无需支付，1 表示需要支付。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/single_waybill/fee")]
    Task<WxaResponse> UpdateSingleWaybillFeeAsync(
        [Body] WxaExpressSingleWaybillFeeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 运力取消订单（散单寄件，<c>POST /cgi-bin/express/delivery/single_waybill/cancel_order</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterdeliverycancel"/>。
    /// </summary>
    /// <param name="request">取消订单请求体（<c>token</c>/<c>reason</c> 必填），见 <see cref="WxaExpressSingleWaybillCancelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/delivery/single_waybill/cancel_order")]
    Task<WxaResponse> CancelSingleWaybillAsync(
        [Body] WxaExpressSingleWaybillCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 运力对订单退款（散单寄件，<c>POST /cgi-bin/express/delivery/single_waybill/refund_order</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterdeliveryrefund"/>。
    /// </summary>
    /// <param name="request">退款请求体（<c>token</c>/<c>fee</c> 必填，<c>fee</c> 为退款金额单位分），见 <see cref="WxaExpressSingleWaybillRefundRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/express/delivery/single_waybill/refund_order")]
    Task<WxaResponse> RefundSingleWaybillAsync(
        [Body] WxaExpressSingleWaybillRefundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 运力回传用户评价反馈（散单寄件，<c>POST /cgi-bin/express/delivery/single_waybill/evaluate_feedback</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterevaluatefeedback"/>。
    /// </summary>
    /// <param name="request">评价反馈请求体（<c>token</c>/<c>waybill_id</c>/<c>result</c> 必填），见 <see cref="WxaExpressSingleWaybillEvaluateFeedbackRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>result</c>：0 已处理，1 不处理；<c>desc</c> 为处理结果描述（可选）。</para>
    /// </remarks>
    [Post("/cgi-bin/express/delivery/single_waybill/evaluate_feedback")]
    Task<WxaResponse> EvaluateSingleWaybillFeedbackAsync(
        [Body] WxaExpressSingleWaybillEvaluateFeedbackRequest request,
        CancellationToken cancellationToken = default);
}
