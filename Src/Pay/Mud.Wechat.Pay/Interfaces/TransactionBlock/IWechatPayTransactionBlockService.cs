// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.TransactionBlock;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「交易拦截记录与申诉（服务商面）」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>4 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 4 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>四支官方全部只印服务商页</b>（「支持商户：【普通服务商】」）⇒ 无商户面镜像；且业务码原文 「此接口仅对部分合作伙伴开放，当前商户号未开放」⇒ 本域是<b>白名单开放面</b>，未开通即 400，属预期而非 SDK 缺陷。</para>
/// <para><b>定位键四支四形，照页不得统一</b>：拦截记录单查把 <c>block_record_id</c> 放 <b>path</b>、<c>sub_mchid</c> 放<b>必填 query</b>； 列表把 <c>sub_mchid</c> 放 <b>path</b>、<c>limit</c> <b>必填</b>而 <c>offset</c> 选填；申诉单查以 <c>block_submission_id</c> 为 path 且<b>无 query</b>；申诉提交的 <c>sub_mchid</c> 在 path、请求体内<b>不含</b>该键。</para>
/// <para><b>申诉两支应答逐字段同形 ⇒ 共用一型</b>：按 <c>block_submission_id</c> 查询与提交申诉的官方应答表均为 <c>block_submission_id</c> / <c>submit_time</c> / <c>review_result</c> / <c>review_time</c> / <c>review_reject_reason</c> 五行同形 ⇒ 共用同一响应类型；拦截记录两支各建（单查 6 键、列表 <c>data[]</c> 元素 + <c>offset</c> / <c>limit</c> / <c>total_count</c> 包装）， 元素与单查字段虽同形，但本线口径是<b>共用只发生在应答根类型</b>。</para>
/// <para><b>两个巨型字段以 <c>string</c> 承载，官方上限 4000000 字符</b>：应答侧 <c>requested_item_info</c>、 请求侧 <c>submit_data</c> 官方类型均写 <c>string(4000000)</c>（约 4MB）⇒ SDK 不解析其内部结构、不做长度校验， 但宿主须知请求与应答的实际体积上限（勿整段进日志）。</para>
/// <para><b>同商户的申诉单是「一商户一在审」</b>：提交支业务码原文「该商户下已经存在<b>审核中</b>的申诉单，不允许提交申诉」⇒ 提交前应先查记录/申诉状态；<c>merchant_submit_id</c>（商家申诉单号，<c>string(128)</c> 必填）是商户侧幂等键， SDK 不做本地去重。</para>
/// <para><b>频率限制</b>：四支官方页<b>全部未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "TransactionBlock", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayTransactionBlockService
{
    /// <summary>
    /// 查询子商户交易拦截记录详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940435"/>（【普通服务商】）。
    /// </summary>
    /// <param name="blockRecordId">【交易拦截记录ID】 交易拦截记录的主键，唯一定义此资源的标识（官方 path <c>block_record_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询子商户交易拦截记录详情，见 <see cref="TransactionBlockRecordInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/transaction-block/transaction-block-records/{block_record_id}</c>；path <c>block_record_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：此接口仅对部分合作伙伴开放，当前商户号未开放。</para>
    /// </remarks>
    [Get("/v3/transaction-block/transaction-block-records/{blockRecordId}")]
    Task<TransactionBlockRecordInfoResponse> QueryTransactionBlockRecordAsync(
        [Path] string blockRecordId,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询子商户名下的交易拦截记录。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940432"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchid">【子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 path <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="limit">【最大资源条数】 该次请求可返回的最大资源条数，不超过10（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="offset">【请求资源起始位置】 该次请求资源的起始位置（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分页查询子商户名下的交易拦截记录，见 <see cref="TransactionBlockRecordListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/transaction-block/transaction-block-records/sub-mchid/{sub_mchid}</c>；path <c>sub_mchid</c>，query <c>limit</c> / <c>offset</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>limit</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：此接口仅对部分合作伙伴开放，当前商户号未开放。</para>
    /// </remarks>
    [Get("/v3/transaction-block/transaction-block-records/sub-mchid/{subMchid}")]
    Task<TransactionBlockRecordListResponse> QueryTransactionBlockRecordsAsync(
        [Path] string subMchid,
        [Query("limit")] int limit,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询交易拦截申诉详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940449"/>（【普通服务商】）。
    /// </summary>
    /// <param name="blockSubmissionId">【交易拦截申诉ID】 交易拦截申诉记录的主键，唯一定义此资源的标识，该参数可从“发起交易拦截申诉”接口的返回参数中获取（官方 path <c>block_submission_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询交易拦截申诉详情，见 <see cref="TransactionBlockSubmissionInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/transaction-block/transaction-block-submissions/{block_submission_id}</c>；path <c>block_submission_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：此接口仅对部分合作伙伴开放，当前商户号未开放。</para>
    /// </remarks>
    [Get("/v3/transaction-block/transaction-block-submissions/{blockSubmissionId}")]
    Task<TransactionBlockSubmissionInfoResponse> QueryTransactionBlockSubmissionAsync(
        [Path] string blockSubmissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起交易拦截申诉。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940446"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchid">【子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 path <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">发起交易拦截申诉请求体，字段见 <see cref="TransactionBlockSubmissionCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发起交易拦截申诉，见 <see cref="TransactionBlockSubmissionInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/transaction-block/transaction-block-submissions/sub-mchid/{sub_mchid}</c>；path <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="TransactionBlockSubmissionInfoResponse"/>（官方应答表逐字段建模，与 <c>GET /transaction-block/transaction-block-submissions/{block_submission_id}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：此接口仅对部分合作伙伴开放，当前商户号未开放 / 400 INVALID_REQUEST：该商户下已经存在审核中的申诉单，不允许提交申诉。。</para>
    /// </remarks>
    [Post("/v3/transaction-block/transaction-block-submissions/sub-mchid/{subMchid}")]
    Task<TransactionBlockSubmissionInfoResponse> CreateTransactionBlockSubmissionAsync(
        [Path] string subMchid,
        [Body] TransactionBlockSubmissionCreateRequest request,
        CancellationToken cancellationToken = default);
}
