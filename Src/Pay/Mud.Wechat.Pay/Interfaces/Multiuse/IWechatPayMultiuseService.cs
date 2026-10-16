// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Multiuse;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「多次扣费券/消费金（发券与账单流水）」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>4 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：4 / 4 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 4 支里<b>1 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>四支官方全部只印商户页</b>（「支持商户：【普通商户】」）⇒ 无服务商镜像；发放对象由 path 段 <c>openid</c> 承载， 请求体里没有 <c>sub_mchid</c> 相关键。</para>
/// <para><b>三支流水下载应答同形 ⇒ 共用一型</b>：发放 / 核销 / 退款明细（<c>send-flow</c> / <c>use-flow</c> / <c>refund-flow</c>） 官方应答表均为单键 <c>url</c> <c>string(2048)</c> 必填、query 均为 <c>begin_date</c> / <c>end_date</c> ⇒ 共用 <see cref="MultiuseFlowUrlResponse"/>；发券支应答单键 <c>coupon_id</c> 另建一型。</para>
/// <para><b>本域是行业方案里唯一逐支印限速与幂等口径的族</b>：四支官方正文均写「接口频率：不区分来源 <b>1000/min</b>」 +「幂等规则：接口<b>支持幂等重入</b>」⇒ 已逐支照抄；流水下载链接的有效期原文为 <b>30s</b>，拿到 <c>url</c> 须立即下载。</para>
/// <para><b>发券支是本域唯一涉密支</b>：官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>，且 <c>user_name</c> / <c>id_card_number</c> 类型为 <c>string(2048)</c> 密文 ⇒ 以 <c>string</c> 承载、SDK <b>不代为加解密</b>； 同域其余三支<b>未列</b>该头（GET 流水无需上送密文），涉密是<b>逐页</b>结论。</para>
/// <para><b>发券四条业务约束照页</b>：「商户<b>默认只允许发放本商户号</b>创建的消费金」「消费金<b>不涉及跨商户发券</b>」 「若发放消费金，则领取用户<b>必须为微信实名用户</b>，否则将发放失败」「该接口只能使用<b>创建活动方</b>进行调用」⇒ 幂等键是 <c>out_request_no</c>，业务码 <c>USER_NOT_EXIST</c>（实名信息错误）与 <c>NOT_ENOUGH</c>（预算/余额/单天限额）不可重试。</para>
/// <para><b>官方页引言跨页抄用 ⇒ 语义以标题 + 路由为准</b>：发放明细（<c>send-flow</c>）页首引言印的是 「可获取到某批次的<b>退款</b>明细数据」（与退款支同文）⇒ 不得据引言把三支并成一支。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Multiuse", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMultiuseService
{
    /// <summary>
    /// 下载批次发放明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012539756"/>（【普通商户】）。
    /// </summary>
    /// <param name="stockId">【批次号】微信为每个代金券批次分配的唯一ID（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="beginDate">【账单开始日期】 账单开始日期，最长下载31天区间的（官方 query <c>begin_date</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="endDate">【账单结束日期】 账单结束日期，最长下载31天区间的（官方 query <c>end_date</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>下载批次发放明细，见 <see cref="MultiuseFlowUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/multiuse/stocks/{stock_id}/send-flow</c>；path <c>stock_id</c>，query <c>begin_date</c> / <c>end_date</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：非法商户号 / 400 PARAM_ERROR：指定范围内不存在账单 / 429 FREQUENCY_LIMITED：调用频率过高。</para>
    /// </remarks>
    [Get("/v3/multiuse/stocks/{stockId}/send-flow")]
    Task<MultiuseFlowUrlResponse> QueryMultiuseSendFlowAsync(
        [Path] string stockId,
        [Query("begin_date")] string? beginDate = null,
        [Query("end_date")] string? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载批次核销明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012539758"/>（【普通商户】）。
    /// </summary>
    /// <param name="stockId">【批次号】成功创建政府消费卡活动后，微信为每个批次分配的唯一ID（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="beginDate">【账单开始时间】 账单开始日期，最长下载31天区间的（官方 query <c>begin_date</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="endDate">【账单结束时间】 账单结束时间，最长下载31天区间的（官方 query <c>end_date</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>下载批次核销明细，见 <see cref="MultiuseFlowUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/multiuse/stocks/{stock_id}/use-flow</c>；path <c>stock_id</c>，query <c>begin_date</c> / <c>end_date</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：非法商户号 / 400 PARAM_ERROR：指定范围内不存在账单 / 429 FREQUENCY_LIMITED：调用频率过高。</para>
    /// </remarks>
    [Get("/v3/multiuse/stocks/{stockId}/use-flow")]
    Task<MultiuseFlowUrlResponse> QueryMultiuseUseFlowAsync(
        [Path] string stockId,
        [Query("begin_date")] string? beginDate = null,
        [Query("end_date")] string? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载批次退款明细。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012539762"/>（【普通商户】）。
    /// </summary>
    /// <param name="stockId">【批次号】微信为每个代金券批次分配的唯一ID（官方 path <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="beginDate">【账单开始时间】 账单开始日期，最长下载31天区间的（官方 query <c>begin_date</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="endDate">【账单结束时间】 账单结束时间，最长下载31天区间的（官方 query <c>end_date</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>下载批次退款明细，见 <see cref="MultiuseFlowUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/multiuse/stocks/{stock_id}/refund-flow</c>；path <c>stock_id</c>，query <c>begin_date</c> / <c>end_date</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：非法商户号 / 400 PARAM_ERROR：指定范围内不存在账单 / 429 FREQUENCY_LIMITED：调用频率过高。</para>
    /// </remarks>
    [Get("/v3/multiuse/stocks/{stockId}/refund-flow")]
    Task<MultiuseFlowUrlResponse> QueryMultiuseRefundFlowAsync(
        [Path] string stockId,
        [Query("begin_date")] string? beginDate = null,
        [Query("end_date")] string? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发放指定批次的消费金。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012539766"/>（【普通商户】）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 OpenID信息，用户在AppID下的唯一标识。（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="request">发放指定批次的消费金请求体，字段见 <see cref="MultiuseCouponCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发放指定批次的消费金，见 <see cref="MultiuseCouponCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/multiuse/users/{openid}/coupons</c>；path <c>openid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：接口频率：不区分来源 1000/min。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="MultiuseCouponCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH：商户号与appid不匹配 / 400 INVALID_REQUEST：非法商户号 / 400 INVALID_REQUEST：openid与appid不匹配 / 400 INVALID_REQUEST：活动已结束或未激活 / 400 INVALID_REQUEST：批次信息获取失败，请确认参数是否有误 / 400 PARAM_ERROR：非法的批次状态 / 403 MCH_NOT_EXISTS：商户号不合法 / 403 NOT_ENOUGH：批次预算不足 / 403 NOT_ENOUGH：发券超过单天限额 / 403 NOT_ENOUGH：账户余额不足，请充值 / 403 NOT_ENOUGH：批次预算耗尽 / 403 REQUEST_BLOCKED：商户无权发券 / 403 REQUEST_BLOCKED：批次不支持跨商户发券 / 403 REQUEST_BLOCKED：用户被限领拦截 / 403 REQUEST_BLOCKED：仅在广告场景下发放批次 / 403 RULE_LIMIT：用户已达最大领券次数 / 403 RULE_LIMIT：被自然人规则拦截 / 403 USER_ACCOUNT_ABNORMAL：用户非法 / 404 RESOURCE_NOT_EXISTS：批次不存在 / 404 USER_NOT_EXIST：用户实名信息错误，请检查输入 / 404 USER_NOT_EXIST：用户信息错误，请检查输入 / 404 USER_NOT_EXIST：openid不匹配，请检查输入 / 404 USER_NOT_EXIST：用户信息解密错误，请检查输入 / 429 FREQUENCY_LIMITED：调用频率过高。</para>
    /// </remarks>
    [Post("/v3/multiuse/users/{openid}/coupons")]
    Task<MultiuseCouponCreateResponse> CreateMultiuseCouponAsync(
        [Path] string openid,
        [Body] MultiuseCouponCreateRequest request,
        CancellationToken cancellationToken = default);
}
