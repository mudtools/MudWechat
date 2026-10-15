// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingFavor;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「代金券」域 SDK（APIv3 营销·代金券，<b>15 端点</b>：批次生命周期 6 + 券读写 3 + 批次明细 4 + 回调地址 2）。
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
/// <b>✅ 本域现已 15 端点</b>（原「尚未覆盖：激活 / 暂停 / 重启 / 批次详情」与 P1-3 的「列表 / 明细 / 回调地址」留档均已关闭）：
/// 创建批次 + 激活批次 + 暂停批次 + 重启批次 + 查询批次详情 + 条件查询批次列表 +
/// 查询券详情 + 条件查询用户券列表 + 发放代金券 +
/// 查询可用单品 / 可用商户 / 下载批次退款明细 / 核销明细 + 设置 / 查询回调通知 url
/// ⇒ 「创建 → 激活 → 发放 → 暂停 / 重启」主流程与对账明细下载可完整执行。
/// </para>
/// <para>
/// <b>本域仍未覆盖</b>：营销专用图片上传（归 <c>MarketingMedia</c> 域，multipart 手工通道）
/// 与代金券相关<b>通知接收</b>（归回调包）。<b>不要凭推断补路由</b>。
/// </para>
/// <para>
/// <b>双面同表 ⇒ 一面一方法</b>：本域新增 8 支同时有【普通商户】与【普通服务商】两页，
/// 逐页字段表指纹比对<b>完全同形</b> ⇒ 只声明一个方法、注释引用两页（与
/// <see cref="IWechatPayMerchantRiskManageService"/> 的「表不同则分建」判据同源）。
/// </para>
/// <para>
/// <b>✅ 值表已补齐（原「值域未核验」留档已关闭）</b>：由官方「查询批次详情」页逐字取得 ——
/// <c>status</c>（<b>全小写</b>，含官方拼写 <c>stoped</c>）、<c>coupon_type</c>、<c>stock_type</c>、
/// <c>trade_type</c>、<c>business_type</c> 与地域级别，见 <c>MarketingFavorConstants.cs</c>。
/// <b>⚠️ 注意两条风格陷阱</b>：① 本域 <c>status</c> 是<b>全小写</b>（与支付分/发票的
/// <c>event_type</c> <b>全大写</b>风格相反）；② 官方拼写 <c>stoped</c>（少一个 p）<b>必须照录</b>。
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

    /// <summary>
    /// 设置代金券回调通知 url。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012464198"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012464176"/>（普通服务商）。
    /// </summary>
    /// <param name="request">设置代金券回调通知 url请求体，字段见 <see cref="CouponCallbackSetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>设置代金券回调通知 url，见 <see cref="CouponCallbackSetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/favor/callbacks</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率： 不区分来源 1000/min 单ip 5/min。</para>
    /// <para><b>应答</b>见 <see cref="CouponCallbackSetResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/favor/callbacks")]
    Task<CouponCallbackSetResponse> SetCouponCallbackAsync(
        [Body] CouponCallbackSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询代金券回调通知 url。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012464070"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012464155"/>（普通服务商）。
    /// </summary>
    /// <param name="mchid">【商户号】 微信支付商户号（官方 query <c>mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询代金券回调通知 url，见 <see cref="CouponCallbackQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/callbacks</c>；query <c>mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率： 2000QPS。</para>
    /// <para><b><c>mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/callbacks")]
    Task<CouponCallbackQueryResponse> QueryCouponCallbackAsync(
        [Query("mchid")] string mchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 条件查询代金券批次列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460489"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460518"/>（普通服务商）。
    /// </summary>
    /// <param name="offset">【分页页码】页码从0开始，默认第0页（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】分页大小，最大10（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="stockCreatorMchid">【创建批次的商户号】批次创建方商户号。 校验规则：接口传入的批次号需由stock_creator_mchid所创建。（官方 query <c>stock_creator_mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="createStartTime">【起始时间】 起始创建时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 校验规则：get请求，参数在 url中，需要进行 url 编码传递（官方 query <c>create_start_time</c>，选填，类型 <c>string(64)</c>）。</param>
    /// <param name="createEndTime">【终止时间】 终止创建时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 校验规则：get请求，参数在 url中，需要进行 url 编码传递（官方 query <c>create_end_time</c>，选填，类型 <c>string(664)</c>）。</param>
    /// <param name="status">【批次状态】 批次状态，枚举值： unactivated：未激活 audit：审核中 running：运行中 stoped：已停止 paused：暂停发放（官方 query <c>status</c>，选填，类型 <c>string(12)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>条件查询代金券批次列表，见 <see cref="CouponStockListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/stocks</c>；query <c>offset</c> / <c>limit</c> / <c>stock_creator_mchid</c> / <c>create_start_time</c> / <c>create_end_time</c> / <c>status</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率： 不区分来源 1000/s 单ip 500/s；接口频率：不区分来源 1000/s 单ip 500/s。</para>
    /// <para><b><c>offset</c> 与 <c>limit</c> 与 <c>stock_creator_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/stocks")]
    Task<CouponStockListResponse> QueryCouponStocksAsync(
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("stock_creator_mchid")] string stockCreatorMchid,
        [Query("create_start_time")] string? createStartTime = null,
        [Query("create_end_time")] string? createEndTime = null,
        [Query("status")] string? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询代金券可用单品。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463442"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463475"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】 批次号 校验规则：必须为代金券（全场券或单品券）批次号，不支持立减与折扣。（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="offset">【分页页码】 分页页码，最大500（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，最大100（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="stockCreatorMchid">【创建批次的商户号】 批次创建方商户号。 校验规则：接口传入的批次号需由stock_creator_mchid所创建（官方 query <c>stock_creator_mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询代金券可用单品，见 <see cref="CouponStockItemsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/stocks/{stock_id}/items</c>；path <c>stock_id</c>，query <c>offset</c> / <c>limit</c> / <c>stock_creator_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min，单ip 200/min；接口频率：不区分来源 1000/min，单ip 200/min。</para>
    /// <para><b><c>offset</c> 与 <c>limit</c> 与 <c>stock_creator_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/stocks/{stockId}/items")]
    Task<CouponStockItemsResponse> QueryCouponStockItemsAsync(
        [Path] string stockId,
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("stock_creator_mchid")] string stockCreatorMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询代金券可用商户。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463358"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463409"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】 微信为每个批次分配的唯一id 校验规则：必须为代金券（全场券或单品券）批次号，不支持立减与折扣（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="offset">【分页页码】 分页页码，最大1000（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，最大50（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="stockCreatorMchid">【创建批次的商户号】 批次创建方商户号。 校验规则：接口传入的批次号需由stock_creator_mchid所创建。（官方 query <c>stock_creator_mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询代金券可用商户，见 <see cref="CouponStockMerchantsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/stocks/{stock_id}/merchants</c>；path <c>stock_id</c>，query <c>offset</c> / <c>limit</c> / <c>stock_creator_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min，单ip 200/min；接口频率：不区分来源 1000/min，单ip 200/min。</para>
    /// <para><b><c>offset</c> 与 <c>limit</c> 与 <c>stock_creator_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/stocks/{stockId}/merchants")]
    Task<CouponStockMerchantsResponse> QueryCouponStockMerchantsAsync(
        [Path] string stockId,
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("stock_creator_mchid")] string stockCreatorMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载批次退款明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463523"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463548"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】 微信为每个代金券批次分配的唯一id。 校验规则： 1、支持全场券;单品券;立减券;折扣券 2、需要是调用创建方创建的批次号（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>下载批次退款明细，见 <see cref="CouponStockRefundFlowResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/stocks/{stock_id}/refund-flow</c>；path <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min ，单ip 5/min；接口频率：不区分来源 1000/min ，单ip 5/min。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/stocks/{stockId}/refund-flow")]
    Task<CouponStockRefundFlowResponse> QueryCouponStockRefundFlowAsync(
        [Path] string stockId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载批次核销明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012463585"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463698"/>（普通服务商）。
    /// </summary>
    /// <param name="stockId">【批次号】 微信为每个代金券批次分配的唯一id。 校验规则： 1、支持全场券;单品券;立减券;折扣券 2、需要是调用创建方创建的批次号（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>下载批次核销明细，见 <see cref="CouponStockUseFlowResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/stocks/{stock_id}/use-flow</c>；path <c>stock_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min， 单ip 5/min；接口频率：不区分来源 1000/min， 单ip 5/min。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/stocks/{stockId}/use-flow")]
    Task<CouponStockUseFlowResponse> QueryCouponStockUseFlowAsync(
        [Path] string stockId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 条件查询用户代金券列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534690"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012494237"/>（普通服务商）。
    /// </summary>
    /// <param name="openid">【用户标识】 用户在商户appid下的唯一标识。 校验规则：该openid需要与接口传入中的appid有对应关系。（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="appid">【公众账号ID】 微信为发券方商户分配的公众账号ID，接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的）或APP的appid（在open.weixin.qq.com申请的）。 校验规则： 1、该appid需要与接口传入中的openid有对应关系； 2、该appid需要与调用接口的商户号（即请求头中的商户号）有绑定关系，若未绑定，可参考该指引完成绑定（ 商家商户号与AppID账号关联管理）（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="stockId">【批次号】 批次号，是否指定批次号查询，填写available_mchid，该字段不生效。（官方 query <c>stock_id</c>，选填，类型 <c>string(20)</c>）。</param>
    /// <param name="status">【券或消费金状态】 代金券或消费金状态： 选填creator_mchid时， SENDED：返回可用 USED：返回可用+已实扣 选填available_mchid时，该字段不生效，仅返回 可用 状态的券或消费金。（官方 query <c>status</c>，选填，类型 <c>string(6)</c>）。</param>
    /// <param name="businessType">【业务类型】 若选择MULTIUSE，则仅返回查询用户拥有的消费金列表 枚举值： MULTIUSE：消费金。可选取值：`MULTIUSE` 消费金类型（官方 query <c>business_type</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="creatorMchid">【创建批次的商户号】 批次创建方商户号。请求参数传创建商户号返回除过期以外所有状态的用户券。creator_mchid与available_mchid二选一，如果同时存在，优先使用creator_mchid。（官方 query <c>creator_mchid</c>，选填，类型 <c>string(20)</c>）。</param>
    /// <param name="availableMchid">【可用商户号】 可用商户号请求参数传可用商户号只能返回可用的代金券，实扣的与过期的无法返回。creator_mchid与available_mchid二选一，如果同时存在，优先使用creator_mchid。（官方 query <c>available_mchid</c>，选填，类型 <c>string(20)</c>）。</param>
    /// <param name="offset">【分页页码】 分页页码，默认0，填写available_mchid，该字段不生效（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，默认20，填写available_mchid，该字段不生效（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>条件查询用户代金券列表，见 <see cref="CouponUserCouponListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/favor/users/{openid}/coupons</c>；path <c>openid</c>，query <c>appid</c> / <c>stock_id</c> / <c>status</c> / <c>business_type</c> / <c>creator_mchid</c> / <c>available_mchid</c> / <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【渠道商】　【清算机构】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 2000/s 单ip 500/s。</para>
    /// <para><b><c>appid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。</para>
    /// </remarks>
    [Get("/v3/marketing/favor/users/{openid}/coupons")]
    Task<CouponUserCouponListResponse> QueryCouponUserCouponsAsync(
        [Path] string openid,
        [Query("appid")] string appid,
        [Query("stock_id")] string? stockId = null,
        [Query("status")] string? status = null,
        [Query("business_type")] string? businessType = null,
        [Query("creator_mchid")] string? creatorMchid = null,
        [Query("available_mchid")] string? availableMchid = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);
}
