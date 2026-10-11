// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「微信物流服务 · 无忧退货」域 SDK（11 端点）—— <b>带令牌</b>通道。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 微信物流服务 → 无忧退货，2026-10-12 逐页核验）：
/// 开通 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_open"/>、
/// 查询开通状态 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_query_open"/>、
/// 发货时投保 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createorder"/>、
/// 无忧退理赔 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_claim"/>、
/// 申请充值订单号 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createchargeid"/>、
/// 申请支付 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_applypay"/>、
/// 拉取充值订单信息 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getpayorderlist"/>、
/// 充值保费退款 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_refund"/>、
/// 拉取理赔摘要 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getsummary"/>、
/// 拉取保单信息 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getorderlist"/>、
/// 设置保费告警余额 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_update_notify_funds"/>。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 官方声明本族<b>支持第三方平台代商家调用</b>（权限集 id <b>139</b>，可用 <c>authorizer_access_token</c>）；
/// 本线按 P3-R2 裁决<b>不</b>引入开放平台令牌链，故仅承载自建形态的单级 <c>access_token</c>。
/// </para>
/// <para>
/// <b>形态差异（勿「顺手对齐」）</b>：<c>open</c> 与 <c>query_open</c> 官方为 <b>GET 且无请求体</b>，
/// <c>refund</c> 官方为 <b>POST 且无请求体</b>（参数表原文「无」），其余 8 端点为 POST ＋ 请求体 JSON。
/// </para>
/// <para>
/// <b>业务顺序（官方原文推断的调用链）</b>：先 <see cref="OpenInsuranceFreightAsync"/> 开通 →
/// <see cref="CreateInsuranceOrderAsync"/> 发货时投保 → 收到退货后 <see cref="ClaimAsync"/> 理赔；
/// 保费侧：<see cref="CreateChargeIdAsync"/> → <see cref="ApplyPayAsync"/>（拿充值链接付款）。
/// <b>官方特别提示</b>：充值后退款<b>只能退回原充值账号</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Express", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaExpressInsuranceFreightService
{
    /// <summary>
    /// 开通无忧退货（<c>GET /wxa/business/insurance_freight/open</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_open"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示开通成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>，<b>无请求体</b>；Query 携带 <c>access_token</c>。</para>
    /// <para>开通结果可用 <see cref="QueryInsuranceFreightOpenAsync"/> 复核（<c>is_open</c>）。</para>
    /// </remarks>
    [Get("/wxa/business/insurance_freight/open")]
    Task<WxaResponse> OpenInsuranceFreightAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询无忧退货开通状态（<c>GET /wxa/business/insurance_freight/query_open</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_query_open"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>开通状态（<c>is_open</c>：<c>0</c> 否，<c>1</c> 是），见 <see cref="WxaExpressQueryInsuranceOpenResponse"/>。</returns>
    /// <remarks>官方契约：<b>GET</b>，<b>无请求体</b>；Query 携带 <c>access_token</c>。</remarks>
    [Get("/wxa/business/insurance_freight/query_open")]
    Task<WxaExpressQueryInsuranceOpenResponse> QueryInsuranceFreightOpenAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发货时投保（<c>POST /wxa/business/insurance_freight/createorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createorder"/>。
    /// </summary>
    /// <param name="request">投保请求体（<c>openid</c>/<c>order_no</c>/<c>pay_time</c>/<c>pay_amount</c>/<c>delivery_no</c>/<c>delivery_place</c>/<c>receipt_place</c>/<c>product_info</c> 必填），见 <see cref="WxaExpressCreateInsuranceOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>保单信息（<c>policy_no</c>/<c>insurance_end_date</c>/<c>estimate_amount</c>/<c>premium</c>），见 <see cref="WxaExpressCreateInsuranceOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>官方约束</b>：<c>openid</c> 必须和理赔 <c>openid</c> 一致；一个微信支付单号<b>只能投保一次</b>；
    /// <c>pay_time</c> 与微信支付时间误差须在 3 天内。常见错误码 <c>41009</c>（missing openid）。
    /// </para>
    /// </remarks>
    [Post("/wxa/business/insurance_freight/createorder")]
    Task<WxaExpressCreateInsuranceOrderResponse> CreateInsuranceOrderAsync(
        [Body] WxaExpressCreateInsuranceOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 无忧退理赔（<c>POST /wxa/business/insurance_freight/claim</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_claim"/>。
    /// </summary>
    /// <param name="request">理赔请求体（<c>openid</c>/<c>order_no</c>/<c>refund_delivery_no</c>/<c>refund_company</c> 必填），见 <see cref="WxaExpressClaimRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>理赔结果（<c>report_no</c>/<c>is_home_pick_up</c>），见 <see cref="WxaExpressClaimResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>官方原文</b>：<b>收到用户退货后</b>再触发；<c>openid</c> / <c>order_no</c> 须与投保时<b>保持一致</b>，<c>refund_delivery_no</c> 唯一。</para>
    /// </remarks>
    [Post("/wxa/business/insurance_freight/claim")]
    Task<WxaExpressClaimResponse> ClaimAsync(
        [Body] WxaExpressClaimRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请充值订单号（<c>POST /wxa/business/insurance_freight/createchargeid</c>，支持自定义金额）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createchargeid"/>。
    /// </summary>
    /// <param name="request">充值申请请求体（<c>quota</c> 必填，单位：分），见 <see cref="WxaExpressCreateChargeIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>充值订单号（<c>order_id</c>），见 <see cref="WxaExpressCreateChargeIdResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。<c>order_id</c> 为 64 位整数，JS 等语言须按 string 传递以防精度丢失。</remarks>
    [Post("/wxa/business/insurance_freight/createchargeid")]
    Task<WxaExpressCreateChargeIdResponse> CreateChargeIdAsync(
        [Body] WxaExpressCreateChargeIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请支付（<c>POST /wxa/business/insurance_freight/applypay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_applypay"/>。
    /// </summary>
    /// <param name="request">支付申请请求体（<c>order_id</c> 必填），见 <see cref="WxaExpressApplyPayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>充值链接（<c>pay_url</c>），见 <see cref="WxaExpressApplyPayResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>官方特别提示</b>：充值后退款<b>只能退回原充值账号</b>。</para>
    /// </remarks>
    [Post("/wxa/business/insurance_freight/applypay")]
    Task<WxaExpressApplyPayResponse> ApplyPayAsync(
        [Body] WxaExpressApplyPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取充值订单信息（<c>POST /wxa/business/insurance_freight/getpayorderlist</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getpayorderlist"/>。
    /// </summary>
    /// <param name="request">查询请求体（<c>status_list</c> 必填，<c>offset</c>/<c>limit</c> 可选），见 <see cref="WxaExpressGetPayOrderListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>充值订单列表（<c>total</c>/<c>list</c>），见 <see cref="WxaExpressGetPayOrderListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。订单状态与退款状态枚举见 <see cref="WxaExpressPayOrderItem"/>。</remarks>
    [Post("/wxa/business/insurance_freight/getpayorderlist")]
    Task<WxaExpressGetPayOrderListResponse> GetPayOrderListAsync(
        [Body] WxaExpressGetPayOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 充值保费退款（<c>POST /wxa/business/insurance_freight/refund</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_refund"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示退款受理成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，<b>无请求体</b>（官方「请求体 Request Payload」原文为「无」）；Query 携带 <c>access_token</c>。</para>
    /// <para><b>官方特别提示</b>：充值后退款<b>只能退回原充值账号</b>。</para>
    /// </remarks>
    [Post("/wxa/business/insurance_freight/refund")]
    Task<WxaResponse> RefundPremiumAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取理赔摘要（<c>POST /wxa/business/insurance_freight/getsummary</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getsummary"/>。
    /// </summary>
    /// <param name="request">摘要查询请求体（<c>begin_time</c>/<c>end_time</c> 必填），见 <see cref="WxaExpressGetSummaryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>当前保费、投保单量、理赔单量、账号余额等，见 <see cref="WxaExpressGetSummaryResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。官方错误码 <c>-202</c> 为内部错误（可稍后重试）。</remarks>
    [Post("/wxa/business/insurance_freight/getsummary")]
    Task<WxaExpressGetSummaryResponse> GetSummaryAsync(
        [Body] WxaExpressGetSummaryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取保单信息（<c>POST /wxa/business/insurance_freight/getorderlist</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getorderlist"/>。
    /// </summary>
    /// <param name="request">保单查询请求体（<b>全字段可选</b>；<c>limit</c> 默认 100、最大 100），见 <see cref="WxaExpressGetOrderListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>保单列表（<c>total</c>/<c>list</c>），见 <see cref="WxaExpressGetOrderListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。保单状态与排序方式枚举见 <see cref="WxaExpressInsuranceOrderItem"/> / <see cref="WxaExpressGetOrderListRequest.SortDirection"/>。</remarks>
    [Post("/wxa/business/insurance_freight/getorderlist")]
    Task<WxaExpressGetOrderListResponse> GetOrderListAsync(
        [Body] WxaExpressGetOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置保费告警余额（<c>POST /wxa/business/insurance_freight/update_notify_funds</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_update_notify_funds"/>。
    /// </summary>
    /// <param name="request">告警余额请求体（<c>notify_funds</c> 必填，单位：分；置 <c>0</c> 表示不通知），见 <see cref="WxaExpressUpdateNotifyFundsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示设置成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>官方原文</b>：余额为设定值时通知小程序管理员（余额无改动情况下 24h 内通知一次），通知消息经<b>微信公众平台</b>下发。</para>
    /// </remarks>
    [Post("/wxa/business/insurance_freight/update_notify_funds")]
    Task<WxaResponse> UpdateNotifyFundsAsync(
        [Body] WxaExpressUpdateNotifyFundsRequest request,
        CancellationToken cancellationToken = default);
}
