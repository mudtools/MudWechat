// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingFavor;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「代金券」域 SDK（APIv3 营销·代金券，<b>P2 表内最后一项</b>，首批 2 端点：创建批次 + 查询券详情）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（普通商户文档中心，2026-10-09 核验；产品归属「营销产品 &gt; 代金券」，
/// 官方<b>未</b>声明下线）：
/// 创建代金券批次 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534633"/>（更新 2024.10.31）、
/// 查询代金券详情 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012486942"/>（更新 2024.09.19）。
/// 两页均标注<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>官方批次生命周期（页面导航照录）</b>：创建 → <b>激活</b>（<c>POST /v3/marketing/favor/stocks/{stock_id}/start</c>）
/// → 发放 → 暂停（<c>…/pause</c>）/ 重启（<c>…/restart</c>）；另有条件查询批次列表、查询批次详情、
/// 查询代金券可用商户 / 可用单品、下载批次退款明细 / 核销明细、图片上传（营销专用）。
/// ⚠️ <b>创建成功 ≠ 可发放</b>：须先激活。
/// </para>
/// <para>
/// <b>本域尚未覆盖</b>：激活 / 暂停 / 重启批次、批次列表与详情、可用商户与单品、两个明细下载、图片上传，
/// 以及代金券相关通知（归回调包）。<b>不要凭推断补路由</b>。
/// </para>
/// <para>
/// <b>值域未核验（诚实记录）</b>：<c>stock_type</c> / <c>status</c> / <c>coupon_type</c> / <c>business_type</c>
/// 的取值表本轮<b>未</b>取得 ⇒ SDK 不臆造常量。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingFavor", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingFavorService
{
    /// <summary>
    /// 创建代金券批次。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534633"/>。
    /// </summary>
    /// <param name="request">创建批次请求体，见 <see cref="CouponStockCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次号与创建时间（<b>仅 2 字段</b>），见 <see cref="CouponStockCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/favor/coupon-stocks</c>；无 path / query 参数。</para>
    /// <para><b>跨字段约束（官方原文）</b>：<c>coupon_use_rule.fixed_normal_coupon</c> 在
    /// <c>stock_type = NORMAL</c> 时<b>必填</b>；<c>coupon_use_rule</c> 内多数子字段选填，
    /// 但 <c>available_merchants</c> 必填。</para>
    /// <para><b>后续动作</b>：创建后须调「激活代金券批次」该批次才可发放（本批未实现该端点）。</para>
    /// </remarks>
    [Post("/v3/marketing/favor/coupon-stocks")]
    Task<CouponStockCreateResponse> CreateCouponStockAsync(
        [Body] CouponStockCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询代金券详情。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012486942"/>。
    /// </summary>
    /// <param name="openId">用户 openid（官方 path <c>openid</c>，必填 string(128)）。</param>
    /// <param name="couponId">代金券或消费金 id（官方 path <c>coupon_id</c>，必填 string(20)）。</param>
    /// <param name="appId">公众账号 ID（官方 query <c>appid</c>，必填 string(128)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>代金券详情（含状态与各类券型信息），见 <see cref="CouponQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/users/{openid}/coupons/{coupon_id}?appid=…</c>；
    /// 两个 path 参数与 <c>appid</c> query <b>均必填</b>。</para>
    /// <para><b>调用方范围</b>：官方注明「支持批次创建商户号与批次发放商户调用」。</para>
    /// <para><b>幂等</b>：官方注明「接口支持幂等重入」；频率限制 1000/s、单 IP 500/s。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/users/{openId}/coupons/{couponId}")]
    Task<CouponQueryResponse> QueryCouponAsync(
        [Path] string openId,
        [Path] string couponId,
        [Query("appid")] string appId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 激活代金券批次。官方文档：<see href="https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/start-stock.html"/>。
    /// </summary>
    /// <param name="stockId">批次号（官方 path <c>stock_id</c>，必填 string(20)）。</param>
    /// <param name="request">操作请求体（仅 <c>stock_creator_mchid</c>），见 <see cref="CouponStockOperationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>激活时间与批次号，见 <see cref="CouponStockStartResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/favor/stocks/{stock_id}/start</c>（更新时间 2024.09.19）。</para>
    /// <para><b>官方原文</b>：免充值券激活<b>不扣款</b>，预充值券激活会锁定对应金额至营销资金池；接口支持<b>幂等</b>重入。</para>
    /// </remarks>
    [Post("/v3/marketing/favor/stocks/{stockId}/start")]
    Task<CouponStockStartResponse> StartCouponStockAsync(
        [Path] string stockId,
        [Body] CouponStockOperationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 暂停代金券批次。官方文档：<see href="https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/pause-stock.html"/>。
    /// </summary>
    /// <param name="stockId">批次号（官方 path <c>stock_id</c>，必填 string(20)）。</param>
    /// <param name="request">操作请求体，见 <see cref="CouponStockOperationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>暂停时间与批次号，见 <see cref="CouponStockPauseResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/favor/stocks/{stock_id}/pause</c>。</para>
    /// <para><b>官方原文</b>：暂停后该批次<b>暂停发放</b>，用户<b>无法通过任何渠道</b>再领取该批次的券；
    /// 接口支持幂等重入。<b>注意</b>「暂停」不同于「终止」—— 暂停可经
    /// <see cref="RestartCouponStockAsync"/> 恢复发放，而已 <c>stop_time</c> 终止的批次不可恢复。</para>
    /// </remarks>
    [Post("/v3/marketing/favor/stocks/{stockId}/pause")]
    Task<CouponStockPauseResponse> PauseCouponStockAsync(
        [Path] string stockId,
        [Body] CouponStockOperationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重启代金券批次。官方文档：<see href="https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/restart-stock.html"/>。
    /// </summary>
    /// <param name="stockId">批次号（官方 path <c>stock_id</c>，必填 string(20)）。</param>
    /// <param name="request">操作请求体，见 <see cref="CouponStockOperationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>重启时间与批次号，见 <see cref="CouponStockRestartResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/favor/stocks/{stock_id}/restart</c>。</para>
    /// <para><b>官方原文</b>：通过此接口可重启指定代金券批次；重启后该批次<b>可以再次发放</b>；接口支持幂等重入。</para>
    /// </remarks>
    [Post("/v3/marketing/favor/stocks/{stockId}/restart")]
    Task<CouponStockRestartResponse> RestartCouponStockAsync(
        [Path] string stockId,
        [Body] CouponStockOperationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询批次详情。官方文档：<see href="https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/query-stock.html"/>。
    /// </summary>
    /// <param name="stockId">批次号（官方 path <c>stock_id</c>，必填 string(20)）。</param>
    /// <param name="stockCreatorMchId">创建批次的商户号（官方 query <c>stock_creator_mchid</c>，必填 string(20)）：接口传入的批次号须由该商户号创建。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>批次详情（含状态与使用规则），见 <see cref="CouponStockQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/stocks/{stock_id}?stock_creator_mchid=…</c>；
    /// path 与 query <b>两者均必填</b>（更新时间 2025.03.25）。</para>
    /// <para>
    /// <b>路径为何没有 <c>coupon-</c> 前缀</b>（与创建批次的 <c>/coupon-stocks</c> 不同）：
    /// 创建是 <c>POST /v3/marketing/favor/coupon-stocks</c>，而批次管理的四个动作都在
    /// <c>/v3/marketing/favor/stocks/…</c> 下 —— <b>两族路径前缀不一致</b>，官方原文如此，勿「统一」。
    /// </para>
    /// <para><b>用途</b>：发放前先查 <c>status</c>（须为 <c>running</c>）；官方发券接口的「非法的批次状态」错误即源于此。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/stocks/{stockId}")]
    Task<CouponStockQueryResponse> QueryCouponStockAsync(
        [Path] string stockId,
        [Query("stock_creator_mchid")] string stockCreatorMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发放指定批次的代金券。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463767"/>。
    /// </summary>
    /// <param name="openId">用户 openid（官方 path <c>openid</c>，必填 string(128)）：须与请求体的 <c>appid</c> 对应。</param>
    /// <param name="request">发放请求体，见 <see cref="CouponIssueRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>代金券 id，见 <see cref="CouponIssueResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/favor/users/{openid}/coupons</c>；<b>无 query 参数</b>。</para>
    /// <para>
    /// <b>前置</b>：批次须为<b>已激活且运营中</b>（<c>status = running</c>）—— 否则官方报
    /// <c>param_error</c>「非法的批次状态」。
    /// </para>
    /// <para>
    /// <b>幂等</b>：由请求体的 <c>out_request_no</c> 承担（商户侧凭据号）⇒ 网络重试时
    /// <b>必须</b>沿用同一个 <c>out_request_no</c>，否则可能重复发券。
    /// </para>
    /// </remarks>
    [Post("/v3/marketing/favor/users/{openId}/coupons")]
    Task<CouponIssueResponse> IssueCouponAsync(
        [Path] string openId,
        [Body] CouponIssueRequest request,
        CancellationToken cancellationToken = default);
}
