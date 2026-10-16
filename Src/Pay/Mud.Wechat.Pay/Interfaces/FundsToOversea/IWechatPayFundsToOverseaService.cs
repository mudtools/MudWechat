// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.FundsToOversea;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「资金出境（平台商户面）」域 SDK（6 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中<b>6 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 6 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>六支官方全部只印服务商页，且身份是【平台商户】</b>（不是【普通服务商】）⇒ 本域是收付通平台面能力， <b>不得</b>按服务商直连面补建商户页，也不得与其它「/v3/partner 树」域混认身份。</para>
/// <para><b>「受理 ≠ 出境成功」是官方原文</b>：「接口请求成功仅代表受理成功，如需知晓业务执行情况请<b>通过查询接口获取</b>」⇒ 下单支与退回支都不得据 HTTP 200 判定业务成功；共用应答里的 <c>result</c> / <c>state</c> 与 <c>fail_reason</c> 才是判据。</para>
/// <para><b>账单下载链接三条硬时效</b>：官方原文「下载地址的有效时间为 <b>30s</b>」、账单「出账日期为 T+1」（当天下载到的是前一天 购付汇<b>且出境成功</b>的单）、「一般在<b>上午 10 点</b>后可以下载到前一天的账单」，并要求 「将实际账单文件的哈希值和接口返回的哈希值进行<b>比对</b>以确认数据完整性」⇒ <c>hash_type</c> / <c>hash_value</c> 不是可选项； 另注「账单中涉及金额的字段单位为<b>元</b>」，与本线绝大多数「分」口径相反。</para>
/// <para><b>两处写读共用应答，各按官方应答表逐字段同形判定</b>：出境单 POST 与按 <c>out_order_id</c> 查询的应答表 <b>15 行同形且必填口径一致</b> ⇒ 共用 <see cref="FundsToOverseaOrderResponse"/>；退回单两支同为 <b>11 行同形</b> ⇒ 共用 <see cref="FundsToOverseaReturnOrderResponse"/>。账单下载支与在途可出境额度支各建一型，<b>不得</b>并入上述两型。</para>
/// <para><b>查询支的三个键不得省</b>：按 <c>out_order_id</c> 查出境单的官方必填 <b>query</b> 是 <c>sub_mchid</c> + <c>transaction_id</c>，而 path 段是商户出境单号 ⇒ 一支同时带「商户单号 + 微信单号 + 特约商户号」三种定位键， 照页不得只留 path 段。退回单查询支同理把 <c>sub_mchid</c> 列为必填 query 而 path 段是 <c>out_return_no</c>。</para>
/// <para><b>频率限制</b>：六支官方页<b>未印</b>端点级数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c>（下单支原文「资金出境限频」）； 六支请求头表均<b>未列</b> <c>Wechatpay-Serial</c> ⇒ 本域无涉密必填头。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "FundsToOversea", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayFundsToOverseaService
{
    /// <summary>
    /// 获取购付汇账单文件下载链接。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476132"/>（【平台商户】）。
    /// </summary>
    /// <param name="billDate">【账单日期】格式YYYY-MM-DD（官方 query <c>bill_date</c>，<b>必填</b>，类型 <c>string(10)</c>）。</param>
    /// <param name="subMchid">【子商户号】若商户是直连商户： &lt;ul&gt;&lt;li&gt;无需填写该字段&lt;/li&gt;&lt;/ul&gt; 若商户是服务商： &lt;ul&gt;&lt;li&gt;不填则默认返回服务商下的账单数据&lt;/li&gt;&lt;li&gt;如需下载某个子商户下的账单数据，则该字段必填&lt;/li&gt;&lt;/ul&gt;（官方 query <c>sub_mchid</c>，选填，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取购付汇账单文件下载链接，见 <see cref="FundsToOverseaBillDownloadUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/funds-to-oversea/bill-download-url</c>；query <c>bill_date</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>bill_date</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH：商户无权限申请资金出境。</para>
    /// </remarks>
    [Get("/v3/funds-to-oversea/bill-download-url")]
    Task<FundsToOverseaBillDownloadUrlResponse> GetFundsToOverseaBillDownloadUrlAsync(
        [Query("bill_date")] string billDate,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请资金出境。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/>（【平台商户】）。
    /// </summary>
    /// <param name="request">申请资金出境请求体，字段见 <see cref="FundsToOverseaOrderCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请资金出境，见 <see cref="FundsToOverseaOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/funds-to-oversea/orders</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="FundsToOverseaOrderResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH：商户无权限申请资金出境 / 404 NOT_FOUND：请求的资源不存在 / 429 FREQUENCY_LIMITED：资金出境限频。</para>
    /// </remarks>
    [Post("/v3/funds-to-oversea/orders")]
    Task<FundsToOverseaOrderResponse> CreateFundsToOverseaOrderAsync(
        [Body] FundsToOverseaOrderCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询出境结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476127"/>（【平台商户】）。
    /// </summary>
    /// <param name="outOrderId">【商户出境单号】本次需要查询的商户出境单号，由商户在发起资金出境请求时生成。（官方 path <c>out_order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】申请资金出境的二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="transactionId">【微信支付订单号】微信支付返回的支付订单号（官方 query <c>transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询出境结果，见 <see cref="FundsToOverseaOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/funds-to-oversea/orders/{out_order_id}</c>；path <c>out_order_id</c>，query <c>sub_mchid</c> / <c>transaction_id</c>。</para>
    /// <para><b>支持商户</b>【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 与 <c>transaction_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH：商户无权限申请资金出境。</para>
    /// </remarks>
    [Get("/v3/funds-to-oversea/orders/{outOrderId}")]
    Task<FundsToOverseaOrderResponse> QueryFundsToOverseaOrderAsync(
        [Path] string outOrderId,
        [Query("sub_mchid")] string subMchid,
        [Query("transaction_id")] string transactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请资金出境退回。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013735039"/>（【平台商户】）。
    /// </summary>
    /// <param name="request">申请资金出境退回请求体，字段见 <see cref="FundsToOverseaReturnOrderCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请资金出境退回，见 <see cref="FundsToOverseaReturnOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/funds-to-oversea/return/return-orders</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="FundsToOverseaReturnOrderResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：403 NO_AUTH：商户无权限申请资金出境退回 / 429 FREQUENCY_LIMITED：频率限制。</para>
    /// </remarks>
    [Post("/v3/funds-to-oversea/return/return-orders")]
    Task<FundsToOverseaReturnOrderResponse> CreateFundsToOverseaReturnOrderAsync(
        [Body] FundsToOverseaReturnOrderCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询出境退回结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013735052"/>（【平台商户】）。
    /// </summary>
    /// <param name="outReturnNo">【商户出境退回单号】 商户在发起资金出境退回请求时生成，要求在同一个商户号下唯一。只能由数字，大小写字母_-组成（官方 path <c>out_return_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 申请资金出境退回的二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询出境退回结果，见 <see cref="FundsToOverseaReturnOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/funds-to-oversea/return/return-orders/out-return-no/{out_return_no}</c>；path <c>out_return_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：404 NOT_FOUND：资金出境退回单不存在 / 429 FREQUENCY_LIMITED：频率限制。</para>
    /// </remarks>
    [Get("/v3/funds-to-oversea/return/return-orders/out-return-no/{outReturnNo}")]
    Task<FundsToOverseaReturnOrderResponse> QueryFundsToOverseaReturnOrderAsync(
        [Path] string outReturnNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询订单剩余可出境余额。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476109"/>（【平台商户】）。
    /// </summary>
    /// <param name="transactionId">【微信支付订单号】微信支付返回的支付订单号（官方 path <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】查询剩余可出境金额的二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询订单剩余可出境余额，见 <see cref="FundsToOverseaTransactionAvailableAbroadAmountResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/funds-to-oversea/transactions/{transaction_id}/available_abroad_amounts</c>；path <c>transaction_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【平台商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 403 NO_AUTH：商户无权限申请资金出境 / 429 FREQUENCY_LIMITED：频率限制 / 500 SYSTEM_ERROR：接口返回错误。</para>
    /// </remarks>
    [Get("/v3/funds-to-oversea/transactions/{transactionId}/available_abroad_amounts")]
    Task<FundsToOverseaTransactionAvailableAbroadAmountResponse> QueryFundsToOverseaAvailableAbroadAmountAsync(
        [Path] string transactionId,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);
}
