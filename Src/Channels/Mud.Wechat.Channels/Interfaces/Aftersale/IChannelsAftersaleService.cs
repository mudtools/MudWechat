// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「售后管理」域 SDK（27 端点：售后单 16 / 纠纷单 4 / 保障单 6 +
/// 全量售后原因 / 拒绝原因）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由形态（设计方案 v1 §2.2）</b>：本域 27 端点全部走 <c>/channels/ec/aftersale/*</c>，全部 POST。
/// 与 SHIPINHAO 本地生活重叠的 <c>aftersale/getaftersaleorder</c> 只计 1。
/// </para>
/// <para>
/// <b>售后单（16）</b>：获取 / 列表 / 同意 / 拒绝 / 同意换货发货 / 拒绝换货发货 / 补寄发货 / 更新补寄物流 /
/// 商家协商 / 商家代发起 / 代用户退差价 / 可换 SKU 列表 / 兑换虚拟号 / 极速换货收货确认 / 上传退款凭证 / 售后原因 / 拒绝原因。
/// </para>
/// <para>
/// <b>纠纷单（4）</b>：获取纠纷单 / 补充留言 / 举证 / 工单同步（<c>syncworkorder</c>）。
/// </para>
/// <para>
/// <b>保障单（6）</b>：获取保障单 / 保障单列表 / 保障单同意 / 协商 / 举证 / 拒绝（假一赔三 / 坏损包退）。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Aftersale", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsAftersaleService
{
    /// <summary>
    /// 获取售后单（<c>aftersale/getaftersaleorder</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>after_sale_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>售后单结构（<c>after_sale_order</c>，含商品 / 退款 / 退货 / 换货 / 协商等）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_getaftersaleorder"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/getaftersaleorder")]
    Task<ChannelsGetAftersaleOrderResponse> GetAftersaleOrderAsync(
        [Body] ChannelsGetAftersaleOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取售后单列表（<c>aftersale/getaftersalelist</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>begin_create_time</c>+<c>end_create_time</c> 与 <c>begin_update_time</c>+<c>end_update_time</c> 两组至少一组）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>售后单号列表与翻页上下文（<c>after_sale_order_id_list</c>/<c>next_key</c>/<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_getaftersalelist"/></para>
    /// <para>官方业务限制：<c>end_create_time</c> 减 <c>begin_create_time</c> 不得大于 24 小时（更新范围同理）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/getaftersalelist")]
    Task<ChannelsGetAftersaleListResponse> GetAftersaleListAsync(
        [Body] ChannelsGetAftersaleListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同意售后（<c>aftersale/acceptapply</c>）。
    /// </summary>
    /// <param name="request">同意请求（<c>after_sale_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_acceptaftersaleapply"/></para>
    /// <para>官方业务限制：同意退货时传 <c>address_id</c>，否则后续处理缺地址请求将报错；<c>accept_type</c> 1 用于退货/换货、2 用于仅退款/收到货。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/acceptapply")]
    Task<ChannelsResponse> AcceptAftersaleAsync(
        [Body] ChannelsAcceptAftersaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝售后（<c>aftersale/rejectapply</c>）。
    /// </summary>
    /// <param name="request">拒绝请求（<c>after_sale_order_id</c>/<c>reject_reason_type</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_rejectaftersaleapply"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/rejectapply")]
    Task<ChannelsResponse> RejectAftersaleAsync(
        [Body] ChannelsRejectAftersaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同意换货发货（<c>aftersale/acceptexchangereship</c>）。
    /// </summary>
    /// <param name="request">发货请求（<c>after_sale_order_id</c>/<c>waybill_id</c>/<c>delivery_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_acceptexchangereship"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/acceptexchangereship")]
    Task<ChannelsResponse> AcceptExchangeReshipAsync(
        [Body] ChannelsAftersaleReshipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝换货发货（<c>aftersale/rejectexchangereship</c>）。
    /// </summary>
    /// <param name="request">拒绝请求（<c>after_sale_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_rejectexchangereship"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/rejectexchangereship")]
    Task<ChannelsResponse> RejectExchangeReshipAsync(
        [Body] ChannelsRejectExchangeReshipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 补寄发货（<c>aftersale/merchantreship</c>）。
    /// </summary>
    /// <param name="request">发货请求（<c>after_sale_order_id</c>/<c>waybill_id</c>/<c>delivery_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_merchantreship"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/merchantreship")]
    Task<ChannelsResponse> MerchantReshipAsync(
        [Body] ChannelsAftersaleReshipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新补寄物流（<c>aftersale/merchantupdatereshipexpress</c>）。
    /// </summary>
    /// <param name="request">更新请求（<c>after_sale_order_id</c>/<c>waybill_id</c>/<c>delivery_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_merchantupdatereshipexpress"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/merchantupdatereshipexpress")]
    Task<ChannelsResponse> MerchantUpdateReshipExpressAsync(
        [Body] ChannelsAftersaleReshipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家协商（<c>aftersale/merchantupdateaftersale</c>）。
    /// </summary>
    /// <param name="request">协商请求（<c>after_sale_order_id</c>/<c>type</c>/<c>merchant_update_desc</c>/<c>update_reason_type</c>/<c>merchant_update_type</c>/<c>media_ids</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_merchantupdateaftersale"/></para>
    /// <para>官方业务限制：协商类型 <c>merchant_update_type</c> 1 已协商一致、2 邀请核实补充凭证、3 修改买家售后申请；换货场景必填 <c>new_sku_id</c> 与 <c>product_cnt</c>（必须填 1）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/merchantupdateaftersale")]
    Task<ChannelsResponse> MerchantUpdateAftersaleAsync(
        [Body] ChannelsMerchantUpdateAftersaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家代发起售后（<c>aftersale/genaftersaleorder</c>）。
    /// </summary>
    /// <param name="request">发起请求（<c>request_id</c>/<c>order_id</c>/<c>product_id</c>/<c>sku_id</c>/<c>count</c>/<c>reason</c>/<c>type</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>售后 ID（<c>aftersale_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_genaftersaleorder"/></para>
    /// <para>官方业务限制：<c>request_id</c> 失败可用相同 ID 重试避免重复发起；换货场景仅支持 count=1 且必填 <c>exchange_sku_info.new_sku_id</c>；非换货场景 <c>amount</c> 必填。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/genaftersaleorder")]
    Task<ChannelsGenAftersaleOrderResponse> GenAftersaleOrderAsync(
        [Body] ChannelsGenAftersaleOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代用户发起退差价（<c>aftersale/refundpricediff</c>）。
    /// </summary>
    /// <param name="request">发起请求（<c>request_id</c>/<c>order_id</c>/<c>product_id</c>/<c>sku_id</c>/<c>amount</c>/<c>reason</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>售后 ID（<c>aftersale_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_refundpricediff"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/refundpricediff")]
    Task<ChannelsRefundPriceDiffResponse> RefundPriceDiffAsync(
        [Body] ChannelsRefundPriceDiffRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可换 SKU 列表（<c>aftersale/getexchangeableskulist</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c>/<c>product_id</c>/<c>sku_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>可换 SKU ID 列表（<c>sku_id_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_getexchangeableskulist"/></para>
    /// <para>官方业务限制：商家代发起换货时，从返回列表选一个填到 <c>genaftersaleorder.exchange_sku_info.new_sku_id</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/getexchangeableskulist")]
    Task<ChannelsGetExchangeableSkuListResponse> GetExchangeableSkuListAsync(
        [Body] ChannelsGetExchangeableSkuListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 售后单兑换虚拟号（<c>aftersale/applyvirtualtelnum</c>）。
    /// </summary>
    /// <param name="request">兑换请求（<c>after_sale_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>虚拟号号码与有效期（<c>virtual_tel_number</c>/<c>virtual_tel_expire_time</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_applyvirtualtelnum"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/applyvirtualtelnum")]
    Task<ChannelsApplyVirtualTelNumResponse> ApplyVirtualTelNumAsync(
        [Body] ChannelsApplyVirtualTelNumRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 极速换货收货确认（<c>aftersale/handlefastexchangereceipt</c>）。
    /// </summary>
    /// <param name="request">确认请求（<c>after_sale_order_id</c>/<c>act</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_handlefastexchangereceipt"/></para>
    /// <para>官方业务限制：<c>act</c> 1 同意、2 拒绝；拒绝时填 <c>reject_reason_type</c>（选择 reject_scene 为 7 的场景）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/handlefastexchangereceipt")]
    Task<ChannelsResponse> HandleFastExchangeReceiptAsync(
        [Body] ChannelsHandleFastExchangeReceiptRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传退款凭证（<c>aftersale/uploadrefundcertificate</c>）。
    /// </summary>
    /// <param name="request">上传请求（<c>after_sale_order_id</c>/<c>refund_certificates</c>/<c>desc</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_uploadrefundcertificate"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/uploadrefundcertificate")]
    Task<ChannelsResponse> UploadRefundCertificateAsync(
        [Body] ChannelsUploadRefundCertificateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取全量售后原因（<c>aftersale/reason/get</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>售后原因列表（<c>reason_list</c>，后续新增原因不再有字面含义，请参考 reason_text）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_getaftersalereason"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/reason/get")]
    Task<ChannelsGetAftersaleReasonResponse> GetAftersaleReasonAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取拒绝售后原因（<c>aftersale/rejectreason/get</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>售后拒绝原因列表（<c>reason_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/api_getaftersalerejectreason"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/rejectreason/get")]
    Task<ChannelsGetAftersaleRejectReasonResponse> GetAftersaleRejectReasonAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取纠纷单详情（<c>aftersale/getcomplaintorder</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>complaint_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>纠纷历史 / 状态 / 虚拟号（<c>history</c>/<c>status</c>/<c>virtual_tel_num_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/complaint/api_getcomplaintorder"/></para>
    /// <para>官方业务限制：纠纷单状态枚举见 <see cref="ChannelsComplaintStatuses"/>（100 待商家处理 … 400 待商家上传工单）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/getcomplaintorder")]
    Task<ChannelsGetComplaintOrderResponse> GetComplaintOrderAsync(
        [Body] ChannelsGetComplaintOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 纠纷单补充留言（<c>aftersale/addcomplaintmaterial</c>）。
    /// </summary>
    /// <param name="request">留言请求（<c>complaint_id</c>/<c>content</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/complaint/api_addcomplaintmaterial"/></para>
    /// <para>官方业务限制：留言内容最多 500 字；所有留言总图片数最多 20 张。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/addcomplaintmaterial")]
    Task<ChannelsResponse> AddComplaintMaterialAsync(
        [Body] ChannelsAddComplaintMaterialRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 纠纷单举证（<c>aftersale/addcomplaintproof</c>）。
    /// </summary>
    /// <param name="request">举证请求（<c>complaint_id</c>/<c>content</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/complaint/api_addcomplaintproof"/></para>
    /// <para>官方业务限制：举证文字内容最多 500 字。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/addcomplaintproof")]
    Task<ChannelsResponse> AddComplaintProofAsync(
        [Body] ChannelsAddComplaintProofRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 工单同步（<c>aftersale/syncworkorder</c>）。
    /// </summary>
    /// <param name="request">同步请求（<c>complaint_id</c>/<c>work_order_info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/complaint/api_syncworkorder"/></para>
    /// <para>官方业务限制：工单状态 1 创建、2 处理、3 完结；<c>result_type</c> 0 无结果、1 同意退款、2 拒绝退款；<c>version</c> 每次全量同步递增。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/syncworkorder")]
    Task<ChannelsResponse> SyncWorkOrderAsync(
        [Body] ChannelsSyncWorkOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取保障单详情（<c>aftersale/getguaranteeorder</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>guarantee_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>保障单结构（<c>guarantee_order</c>，含假一赔三 / 坏损包退详情）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/guarantee/api_getguaranteeorder"/></para>
    /// <para>官方业务限制：保障单类型 1 假一赔三、2 坏损包退；状态枚举见 <see cref="ChannelsGuaranteeStatuses"/>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/getguaranteeorder")]
    Task<ChannelsGetGuaranteeOrderResponse> GetGuaranteeOrderAsync(
        [Body] ChannelsGetGuaranteeOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取保障单列表（<c>aftersale/searchguaranteeorder</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>limit</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>保障单详情列表与总数（<c>guarantee_order_list</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/guarantee/api_searchguaranteeorder"/></para>
    /// <para>官方业务限制：<c>type</c> 0 全部、1 假一赔三、2 坏损包退；<c>status_list</c> 用保障单状态枚举过滤；<c>offset</c> 默认从 0 开始。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/searchguaranteeorder")]
    Task<ChannelsSearchGuaranteeOrderResponse> SearchGuaranteeOrderAsync(
        [Body] ChannelsSearchGuaranteeOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保障单同意（<c>aftersale/merchantacceptguarantee</c>）。
    /// </summary>
    /// <param name="request">同意请求（<c>guarantee_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/guarantee/api_merchantacceptguarantee"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/merchantacceptguarantee")]
    Task<ChannelsResponse> MerchantAcceptGuaranteeAsync(
        [Body] ChannelsMerchantAcceptGuaranteeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保障单协商（<c>aftersale/merchantmodifyguarantee</c>）。
    /// </summary>
    /// <param name="request">协商请求（<c>guarantee_order_id</c>/<c>bad_level</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/guarantee/api_merchantmodifyguarantee"/></para>
    /// <para>官方业务限制：商家修改坏损比例可填 10/30/50/80/100。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/merchantmodifyguarantee")]
    Task<ChannelsResponse> MerchantModifyGuaranteeAsync(
        [Body] ChannelsMerchantModifyGuaranteeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保障单举证（<c>aftersale/merchantproofguarantee</c>）。
    /// </summary>
    /// <param name="request">举证请求（<c>guarantee_order_id</c>/<c>content</c>/<c>pic_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/guarantee/api_merchantproofguarantee"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/merchantproofguarantee")]
    Task<ChannelsResponse> MerchantProofGuaranteeAsync(
        [Body] ChannelsMerchantProofGuaranteeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保障单拒绝（<c>aftersale/merchantrefuseguarantee</c>）。
    /// </summary>
    /// <param name="request">拒绝请求（<c>guarantee_order_id</c>/<c>reason</c>/<c>pic_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-aftersale/guarantee/api_merchantrefuseguarantee"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/aftersale/merchantrefuseguarantee")]
    Task<ChannelsResponse> MerchantRefuseGuaranteeAsync(
        [Body] ChannelsMerchantRefuseGuaranteeRequest request,
        CancellationToken cancellationToken = default);
}
