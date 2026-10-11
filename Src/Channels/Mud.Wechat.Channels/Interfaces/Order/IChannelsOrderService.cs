// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「订单管理」域 SDK（27 端点：订单增查 / 搜索 / 改价 / 改地址 /
/// 改备注 / 物流变更 / 发货协商 / 生鲜质检 / 礼物单 / 发货前换款 / 盲盒拆盒 / 敏感信息解密 /
/// 虚拟号与真实号 / 商家私密号实名认证 / 用户预约发货）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由形态（设计方案 v1 §2.2）</b>：本域 27 端点 = <c>/channels/ec/order/*</c> 24 条 +
/// <c>/channels/ec/merchant/privatenumber/*</c> 3 条（商家私密号实名认证），全部 POST。
/// </para>
/// <para>
/// <b>收货信息脱敏语义</b>：平台订单的收货信息（昵称 / 电话 / 详细地址）默认隐藏，需经
/// <c>order/sensitiveinfo/decode</c> 解密；虚拟号由官方分配（<c>virtualnumber/*</c>），
/// 实名认证后才可拨打（<c>merchant/privatenumber/*</c>），真实号申请需走审核
/// （<c>realnumber/apply</c> + <c>realnumberviewaudit/get</c>）。
/// </para>
/// <para>
/// <b>代改址协商</b>：<c>order/address/update</c> 提交后进入「协商」流程，需买家同意
/// （<c>addressmodify/accept</c>）后才可修改成功；<c>order/price/update</c> 可改商品价与运费。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Order", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsOrderService
{
    /// <summary>
    /// 获取订单详情（<c>order/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单结构（<c>order</c>，含下单人 / 商品列表 / 支付 / 价格 / 配送 / 优惠券 / 结算等）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getorder"/></para>
    /// <para>官方业务限制：订单收货信息为脱敏数据，详细收货信息需调 <c>order/sensitiveinfo/decode</c> 解密。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/get")]
    Task<ChannelsGetOrderResponse> GetOrderAsync(
        [Body] ChannelsGetOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单列表（<c>order/list/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>create_time_range</c> / <c>update_time_range</c> 至少填一个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单号列表与翻页上下文（<c>order_id_list</c>/<c>next_key</c>/<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getorderlist"/></para>
    /// <para>官方业务限制：时间范围至少填一个；单次时间跨度不可超过 7 天；<c>page_size</c> 不超过 100。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/list/get")]
    Task<ChannelsOrderIdListResponse> GetOrderListAsync(
        [Body] ChannelsGetOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订单搜索（<c>order/search</c>）。
    /// </summary>
    /// <param name="request">搜索请求（<c>search_condition</c> 至少设置一个字段，<c>page_size</c>/<c>next_key</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单号列表与翻页上下文（<c>order_id_list</c>/<c>next_key</c>/<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_searchorder"/></para>
    /// <para>
    /// 官方业务限制：①搜索条件下的参数必须至少设置一个字段，否则接口报错；②收件人电话字段
    /// （<c>tel_number</c>）已废弃，请使用后四位 <c>tel_number_last4</c>；③<b>正在售后</b>定义为
    /// "处于非终止态"（USER_CANCELD / MERCHANT_REFUND_RETRY_FAIL / MERCHANT_FAIL /
    /// MERCHANT_REFUND_SUCCESS / MERCHANT_RETURN_SUCCESS 之外的售后单状态）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/search")]
    Task<ChannelsSearchOrderResponse> SearchOrderAsync(
        [Body] ChannelsSearchOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改订单价格（<c>order/price/update</c>）。
    /// </summary>
    /// <param name="request">改价请求（<c>order_id</c>/<c>change_order_infos</c>/<c>change_express</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changeorderprice"/></para>
    /// <para>官方业务限制：修改后的运费由 <c>express_fee</c> 指定（change_express 为 true 时必填，不填默认为 0，单位为分）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/price/update")]
    Task<ChannelsResponse> ChangeOrderPriceAsync(
        [Body] ChannelsChangeOrderPriceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改订单地址（<c>order/address/update</c>）。
    /// </summary>
    /// <param name="request">改地址请求（<c>order_id</c>/<c>user_address</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changeorderaddress"/></para>
    /// <para>
    /// 官方业务限制：①提交修改后将进入「协商」流程，需买家同意后方可修改成功（详见「代改址协商」指南）；
    /// ②普通订单（deliver_method=0）必填 <c>user_name</c>/<c>province_name</c>/<c>city_name</c>/
    /// <c>detail_info</c>/<c>tel_number</c>；虚拟商品订单（deliver_method=1）必填 <c>virtual_order_tel_number</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/address/update")]
    Task<ChannelsResponse> ChangeOrderAddressAsync(
        [Body] ChannelsChangeOrderAddressRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同意用户修改收货地址申请（<c>order/addressmodify/accept</c>）。
    /// </summary>
    /// <param name="request">同意请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_acceptorderaddressmodifyapply"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/addressmodify/accept")]
    Task<ChannelsResponse> AcceptAddressModifyAsync(
        [Body] ChannelsAcceptAddressModifyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝买家的订单修改收货地址申请（<c>order/addressmodify/reject</c>）。
    /// </summary>
    /// <param name="request">拒绝请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_rejectorderaddressmodifyapply"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/addressmodify/reject")]
    Task<ChannelsResponse> RejectAddressModifyAsync(
        [Body] ChannelsRejectAddressModifyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改订单备注（<c>order/merchantnotes/update</c>）。
    /// </summary>
    /// <param name="request">备注请求（<c>order_id</c>/<c>merchant_notes</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changemerchantnotes"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/merchantnotes/update")]
    Task<ChannelsResponse> UpdateMerchantNotesAsync(
        [Body] ChannelsUpdateMerchantNotesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 礼物订单新增备注信息（<c>order/presentnote/add</c>）。
    /// </summary>
    /// <param name="request">备注请求（<c>present_order_id</c>/<c>notes</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_presentnote"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/presentnote/add")]
    Task<ChannelsResponse> AddPresentNoteAsync(
        [Body] ChannelsAddPresentNoteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取礼物单的子单列表（<c>order/presentsuborder/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>present_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>礼物单对应的子单订单号列表（<c>order_ids</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getpresentsuborder"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/presentsuborder/get")]
    Task<ChannelsGetPresentSubOrderResponse> GetPresentSubOrderAsync(
        [Body] ChannelsGetPresentSubOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有待发货前更换 sku 待处理请求（<c>order/preshipmentchangesku/get</c>）。
    /// </summary>
    /// <param name="request">分页请求（<c>page</c>/<c>page_size</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>等待商家处理的换款请求订单 id 列表（<c>order_ids</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getpreshipmentchangeskuwaithandlelist"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/preshipmentchangesku/get")]
    Task<ChannelsGetPreShipmentChangeSkuResponse> GetPreShipmentChangeSkuListAsync(
        [Body] ChannelsGetPreShipmentChangeSkuRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同意待发货前更换 sku 请求（<c>order/preshipmentchangesku/approve</c>）。
    /// </summary>
    /// <param name="request">同意请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_approvepreshipmentchangesku"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/preshipmentchangesku/approve")]
    Task<ChannelsResponse> ApprovePreShipmentChangeSkuAsync(
        [Body] ChannelsApprovePreShipmentChangeSkuRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝待发货前更换 sku 请求（<c>order/preshipmentchangesku/reject</c>）。
    /// </summary>
    /// <param name="request">拒绝请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_rejectpreshipmentchangesku"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/preshipmentchangesku/reject")]
    Task<ChannelsResponse> RejectPreShipmentChangeSkuAsync(
        [Body] ChannelsRejectPreShipmentChangeSkuRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置潮玩订单盲盒款式（<c>order/blindboxitem/set</c>）。
    /// </summary>
    /// <param name="request">回传请求（<c>order_id</c>/<c>item_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置失败的盲盒款式列表（<c>fail_item_list</c>，空即全部成功）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_setblindboxitem"/></para>
    /// <para>官方业务限制：商家将潮玩订单拆盒结果回传给小店订单，与 <c>product_infos[].order_product_item_list[].item_unique_id</c> 一一对应。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/blindboxitem/set")]
    Task<ChannelsSetBlindBoxItemResponse> SetBlindBoxItemAsync(
        [Body] ChannelsSetBlindBoxItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改物流信息（<c>order/deliveryinfo/update</c>）。
    /// </summary>
    /// <param name="request">修改请求（<c>order_id</c> 必填；<c>delivery_list</c> / <c>change_infos</c> 二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changedeliveryinfo"/></para>
    /// <para>
    /// 官方业务限制：①整单物流信息（<c>delivery_list</c>）不支持拆单；②更新包裹物流信息（<c>change_infos</c>）
    /// 支持拆单，<c>old</c>/<c>new</c> 各含原 / 新快递信息；③发货方式目前仅支持 1（自寄快递）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/deliveryinfo/update")]
    Task<ChannelsResponse> UpdateDeliveryInfoAsync(
        [Body] ChannelsUpdateDeliveryInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交发货协商申请（<c>order/deliverynegotiation/submit</c>）。
    /// </summary>
    /// <param name="request">协商请求（<c>order_id</c>/<c>predict_delivery_time</c>/<c>reason</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_submitdeliverynegotiation"/></para>
    /// <para>官方业务限制：发货超时订单可提交发货协商，与买家协商新的预计发货时间；处理结果经 <c>deliverynegotiation/result/get</c> 查询。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/deliverynegotiation/submit")]
    Task<ChannelsResponse> SubmitDeliveryNegotiationAsync(
        [Body] ChannelsSubmitDeliveryNegotiationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取发货协商结果（<c>order/deliverynegotiation/result/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>协商结果列表（<c>negotiation_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_querydeliverynegotiation"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/deliverynegotiation/result/get")]
    Task<ChannelsGetDeliveryNegotiationResultResponse> GetDeliveryNegotiationResultAsync(
        [Body] ChannelsGetDeliveryNegotiationResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传生鲜质检信息（<c>order/freshinspect/submit</c>）。
    /// </summary>
    /// <param name="request">上传请求（<c>order_id</c>/<c>audit_items</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_submitfreshinspectinfo"/></para>
    /// <para>
    /// 官方业务限制：给生鲜类质检订单上传商品打包信息；审核项名称见
    /// <see cref="ChannelsFreshInspectItemNames"/>（快递单图片 / 包装箱全景视频 / 开箱全景视频 / 单个细节全景视频）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/freshinspect/submit")]
    Task<ChannelsResponse> SubmitFreshInspectAsync(
        [Body] ChannelsSubmitFreshInspectRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户预约发货列表（<c>order/userbooking/list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>time_range_order_create</c> / <c>time_range_booking_predict</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预约发货记录列表与翻页信息（<c>record_list</c>/<c>next_page</c>/<c>total</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_pullshopuserbookinglist"/></para>
    /// <para>
    /// 官方业务限制：①两个时间范围均必填且间隔不超过 7 天；②预约类型 <c>booking_type</c>：1=指定时间发货，2=暂不发货；
    /// ③分页 <c>limit</c> 默认为 20、不能超过 100。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/userbooking/list")]
    Task<ChannelsGetUserBookingListResponse> GetUserBookingListAsync(
        [Body] ChannelsGetUserBookingListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解密订单中的详细收货信息（<c>order/sensitiveinfo/decode</c>）。
    /// </summary>
    /// <param name="request">解密请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>收货信息与虚拟号信息（<c>address_info</c>/<c>virtual_number_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_decodesensitiveinfo"/></para>
    /// <para>官方业务限制：为保护用户隐私（收货人昵称、电话号码、详细收货地址），平台订单的收货信息进行了部分隐藏。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/sensitiveinfo/decode")]
    Task<ChannelsDecodeSensitiveInfoResponse> DecodeSensitiveInfoAsync(
        [Body] ChannelsDecodeSensitiveInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请查看订单真实号码（<c>order/realnumber/apply</c>）。
    /// </summary>
    /// <param name="request">申请请求（<c>order_id</c>/<c>apply_type</c>/<c>apply_reason</c>/<c>pic_media_ids</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_applyrealnumber"/></para>
    /// <para>官方业务限制：订单使用虚拟号且虚拟号不满足需求时提交申请查看真实号；申请原因图片 1-5 张；审核状态经 <c>realnumberviewaudit/get</c> 查询。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/realnumber/apply")]
    Task<ChannelsResponse> ApplyRealNumberAsync(
        [Body] ChannelsApplyRealNumberRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查看订单真实号审核状态（<c>order/realnumberviewaudit/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请时间 / 审核状态 / 申请原因 / 申请 id。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getrealnumberviewaudit"/></para>
    /// <para>官方业务限制：审核状态 <c>audit_state</c>：1 审核中、2 审核拒绝、3 审核通过（见 <see cref="ChannelsRealNumberAuditStates"/>）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/realnumberviewaudit/get")]
    Task<ChannelsGetRealNumberViewAuditResponse> GetRealNumberViewAuditAsync(
        [Body] ChannelsGetRealNumberViewAuditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订单再次申请虚拟号（<c>order/virtualnumber/applyagain</c>）。
    /// </summary>
    /// <param name="request">申请请求（<c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>虚拟号 / 分机号 / 剩余申请次数。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_applyvirtualnumberagain"/></para>
    /// <para>官方业务限制：订单虚拟号过期之后，可再次申请一次虚拟号。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/virtualnumber/applyagain")]
    Task<ChannelsApplyVirtualNumberAgainResponse> ApplyVirtualNumberAgainAsync(
        [Body] ChannelsApplyVirtualNumberAgainRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订单虚拟号延期（<c>order/virtualnumber/delay</c>）。
    /// </summary>
    /// <param name="request">延期请求（<c>order_id</c>/<c>has_delay_times</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>过期时间与可延期次数（<c>expiration</c>/<c>available_extend_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_delayvirtualnumber"/></para>
    /// <para>官方业务限制：订单虚拟号过期前 7 天可通过该接口延期；<c>has_delay_times</c> 为当前已延期次数（可通过 获取订单详情 拿到，不填默认为 0）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/virtualnumber/delay")]
    Task<ChannelsDelayVirtualNumberResponse> DelayVirtualNumberAsync(
        [Body] ChannelsDelayVirtualNumberRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加待认证的手机号（<c>order/merchant/privatenumber/addphone</c>）。
    /// </summary>
    /// <param name="request">认证请求（<c>mobile</c>/<c>verify_code</c>/<c>wxusername</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>认证链接与认证状态（<c>qrcode_url</c>/<c>status</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_privatenumberaddphone"/></para>
    /// <para>
    /// 官方业务限制：【虚拟号拨打实名认证】添加待认证的手机号并返回运营商实名认证的页面地址；
    /// <c>verify_code</c> 经 <c>privatenumber/sendverifycode</c> 获取；认证状态：1=认证成功、2=认证失败、3=运营商审核中。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/privatenumber/addphone")]
    Task<ChannelsPrivatenumberAddPhoneResponse> AddPrivatenumberPhoneAsync(
        [Body] ChannelsPrivatenumberAddPhoneRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前手机号的认证状态（<c>order/merchant/privatenumber/getphone</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>mobile</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>认证状态与审核失败原因（<c>status</c>/<c>fail_reason</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_privatenumbergetshopphone"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/privatenumber/getphone")]
    Task<ChannelsPrivatenumberGetPhoneResponse> GetPrivatenumberPhoneAsync(
        [Body] ChannelsPrivatenumberGetPhoneRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取短信验证码（<c>order/merchant/privatenumber/sendverifycode</c>）。
    /// </summary>
    /// <param name="request">发送请求（<c>mobile</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_privatenumbersendverifycode"/></para>
    /// <para>官方业务限制：【虚拟号拨打实名认证】第一步，获取手机号验证码。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/privatenumber/sendverifycode")]
    Task<ChannelsResponse> SendPrivatenumberVerifyCodeAsync(
        [Body] ChannelsPrivatenumberSendVerifyCodeRequest request,
        CancellationToken cancellationToken = default);
}