// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.EducationSchoolPay;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「教育学费缴纳（EducationSchoolPay）」域 SDK（8 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 8 支中<b>8 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 8 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 1 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 8 支里<b>1 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>本域八支官方全部只印服务商页</b>（各页均标「支持商户：【普通服务商】」）⇒ 无商户面镜像、无双页归并问题， 一支一页一方法；<b>不得</b>按教育代扣域的双页形态补建商户面方法。</para>
/// <para><b>预签约支的密文字段恰四枚</b>：<c>identity.real_name</c> / <c>identity.id_card_number</c> / <c>bank_card.bank_card_no</c> / <c>bank_card.phone</c> 官方逐枚标「按照APIV3标准加密该字段」且类型一律 <c>string(512)</c> ⇒ 以 <c>string</c> 承载、SDK <b>不代为加解密</b>；同支其余字段（<c>appid</c> / <c>openid</c> / <c>plan_id</c> / <c>school_id</c> 等）官方<b>无</b>加密要求，不得整体按密文处理。</para>
/// <para><b>解除协议支 204 无包体</b> ⇒ 返回裸 <see cref="Task"/>；缴费下单（POST transactions）官方应答表与 两支查单（按 <c>transaction_id</c> / 按 <c>out_trade_no</c>）<b>逐字段同形且必填口径一致</b> ⇒ 三支共用同一响应类型， 这是本域唯一「写侧与读侧共用应答」的一对。</para>
/// <para><b>下单请求里没有 <c>openid</c></b>：扣款对象由 <c>contract_id</c> + <c>user_id</c> 定位（签约协议 + 学生账户）， 而<b>应答</b>侧的 <c>payer</c> 只有 <c>openid</c> / <c>sub_openid</c> ⇒ 请求与应答两侧的定位口径不同，不得按「下单也要传 openid」补字段。</para>
/// <para><b>「受理 ≠ 扣款成功」</b>：共用应答里的 <c>trade_state</c> 可为中间态，官方错误码原文即 「商户订单号重复，请查询订单确定交易状态。确定交易失败后换订单号重试」⇒ 结果由查单支判定，<b>不得</b>据 HTTP 2xx 判定扣款成功。</para>
/// <para><b>欠费状态支应答五键官方全必填</b>（<c>appid</c> / <c>openid</c> / <c>state</c> / <c>debt_count</c> / <c>update_time</c>）；其 <c>state</c> 是「用户欠款风控状态」（<c>NORMAL</c> / <c>FORBIDDEN</c>），与订单支的 <c>trade_state</c> <b>同名不同义 ⇒ 不得互套</b>。本线 DTO 惯例是属性一律可空承载、必填口径只由文档注释与守卫断言表达， 故「全必填」不等于「做成非空类型」。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "EducationSchoolPay", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEducationSchoolPayService
{
    /// <summary>
    /// 校园缴费预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468952"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">校园缴费预签约请求体，字段见 <see cref="EducationSchoolPayContractPresignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>校园缴费预签约，见 <see cref="EducationSchoolPayContractPresignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/eduschoolpay/contracts/presign</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="EducationSchoolPayContractPresignResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 ALREADY_EXISTS：已存在使用该out_contract_no的合约 / 400 INVALID_REQUEST：不支持的商户 / 400 INVALID_REQUEST：商户号和公众号没有绑定关系 / 400 INVALID_REQUEST：用户信息有误，请检查OpenID / 400 INVALID_REQUEST：签约模板非法，请检查plan_id / 400 INVALID_REQUEST：签约模板ID或学校ID有误，请检查plan_id与school_id / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Post("/v3/eduschoolpay/contracts/presign")]
    Task<EducationSchoolPayContractPresignResponse> CreateEducationSchoolPayContractPresignAsync(
        [Body] EducationSchoolPayContractPresignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询校园缴费签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468959"/>（【普通服务商】）。
    /// </summary>
    /// <param name="contractId">【签约协议号】 微信用户在校园轻松付下的合约的唯一标识，用户在校园轻松付小程序完成签约后，由微信支付生成。（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询校园缴费签约，见 <see cref="EducationSchoolPayContractInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/eduschoolpay/contracts/{contract_id}</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误，请检查contract_id / 404 NOT_FOUND：未找到对应合约 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Get("/v3/eduschoolpay/contracts/{contractId}")]
    Task<EducationSchoolPayContractInfoResponse> QueryEducationSchoolPayContractAsync(
        [Path] string contractId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除校园缴费签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469025"/>（【普通服务商】）。
    /// </summary>
    /// <param name="contractId">【签约协议号】 微信用户在校园轻松付下的合约的唯一标识，用户在校园轻松付小程序完成签约后，由微信支付生成。（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/eduschoolpay/contracts/{contract_id}/terminate</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误，请检查contract_id / 404 NOT_FOUND：未找到对应合约 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Post("/v3/eduschoolpay/contracts/{contractId}/terminate")]
    Task TerminateEducationSchoolPayContractAsync(
        [Path] string contractId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按用户查询校园缴费签约列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468993"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【商户AppID下的用户标识】 OpenID是由微信生成的，代表某个微信用户在AppID下的唯一标识。相同微信用户，AppID不同，则生成的OpenID不同。 请注意：调用该接口传入的OpenID应当与签约appid对应，如果传入的OpenID不合法，或与签约AppID不对应，则可能出现：1、查询结果为空或者报错；2、查询返回的OpenID与请求传入的OpenID不一致。（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="planId">【签约模板号】 签约模板的唯一标识。商户接入校园轻松付并申请签约模板后由微信支付生成。（官方 query <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="contractStatus">【签约状态】 希望查询的签约协议的状态。枚举值：ADD-已签约; DELETE-已解约; ALL-同时查询已签约和已解约（官方 query <c>contract_status</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="offset">【分页页码】 该次请求资源的起始位置，从0开始计数，请保持为分页大小 limit 的整数倍。调用方选填，默认为0。（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 该次请求可返回的最大资源条数。调用方选填，默认值为20，最小为1，最大为40。（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>按用户查询校园缴费签约列表，见 <see cref="EducationSchoolPayUserContractListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/eduschoolpay/users/{openid}/contracts</c>；path <c>openid</c>，query <c>plan_id</c> / <c>contract_status</c> / <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>plan_id</c> 与 <c>contract_status</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：用户信息有误，请检查OpenID / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Get("/v3/eduschoolpay/users/{openid}/contracts")]
    Task<EducationSchoolPayUserContractListResponse> QueryEducationSchoolPayUserContractsAsync(
        [Path] string openid,
        [Query("plan_id")] string planId,
        [Query("contract_status")] string contractStatus,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 校园缴费订单查询（微信订单号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469063"/>（【普通服务商】）。
    /// </summary>
    /// <param name="transactionId">【微信支付订单号】 微信支付订单号（官方 path <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系。如果是服务商模式接入则可选；如果是直连商户模式接入则不用填写。（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>校园缴费订单查询（微信订单号），见 <see cref="EducationSchoolPayTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/eduschoolpay/transactions/id/{transaction_id}</c>；path <c>transaction_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：非法商户，请确认参数与订单创建商户一致 / 400 PARAM_ERROR：参数错误，请检查transaction_id / 404 NOT_FOUND：订单不存在 / 429 RATELIMIT_EXCEEDED：达到调用速率限制。</para>
    /// </remarks>
    [Get("/v3/eduschoolpay/transactions/id/{transactionId}")]
    Task<EducationSchoolPayTransactionQueryResponse> QueryEducationSchoolPayTransactionByIdAsync(
        [Path] string transactionId,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 校园缴费下单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469030"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">校园缴费下单请求体，字段见 <see cref="EducationSchoolPayTransactionCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>校园缴费下单，见 <see cref="EducationSchoolPayTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/eduschoolpay/transactions</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="EducationSchoolPayTransactionQueryResponse"/>（官方应答表逐字段建模，与 <c>GET /eduschoolpay/transactions/id/{transaction_id}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 ALREADY_EXISTS：out_trade_no冲突 / 400 INVALID_REQUEST：非法商户，服务商模式下请确认子商户正确 / 400 INVALID_REQUEST：商户号和公众号没有绑定关系 / 400 INVALID_REQUEST：商户号、子商户号以及子公众号没有绑定关系 / 400 INVALID_REQUEST：订单状态异常 / 400 INVALID_REQUEST：模板与学校不匹配 / 400 INVALID_REQUEST：学生账户与合约不匹配 / 400 INVALID_REQUEST：签约无效或已解除 / 400 INVALID_REQUEST：商户没有大额扣费场景权限 / 400 INVALID_REQUEST：单日扣款总金额超过限额 / 400 INVALID_REQUEST：商户订单号重复，请查询订单确定交易状态。确定交易失败后换订单号重试 / 400 INVALID_REQUEST：交易存在风险，不能发起扣款 / 403 NO_AUTH：用户禁用当前商户扣款权限 / 403 NOT_ENOUGH：用户欠款过多，暂停交易 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：用户数据异常，请稍后重试 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Post("/v3/eduschoolpay/transactions")]
    Task<EducationSchoolPayTransactionQueryResponse> CreateEducationSchoolPayTransactionAsync(
        [Body] EducationSchoolPayTransactionCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 校园缴费订单查询（商户订单号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469097"/>（【普通服务商】）。
    /// </summary>
    /// <param name="outTradeNo">【商户订单号】 商户在调用「扣款接口」时自行生成的订单号，要求64个字符内，只能是数字、大小写字母和 _-，且在同一个商户号下唯一。（官方 path <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系。如果是服务商模式接入则可选；如果是直连商户模式接入则不用填写。（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>校园缴费订单查询（商户订单号），见 <see cref="EducationSchoolPayTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/eduschoolpay/transactions/out-trade-no/{out_trade_no}</c>；path <c>out_trade_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：非法商户，请确认参数与订单创建商户一致 / 400 PARAM_ERROR：参数错误，请检查out_trade_no / 404 NOT_FOUND：订单不存在 / 429 RATELIMIT_EXCEEDED：达到调用速率限制。</para>
    /// </remarks>
    [Get("/v3/eduschoolpay/transactions/out-trade-no/{outTradeNo}")]
    Task<EducationSchoolPayTransactionQueryResponse> QueryEducationSchoolPayTransactionByOutTradeNoAsync(
        [Path] string outTradeNo,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户欠费状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469161"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【商户AppID下的用户标识】 OpenID是由微信生成的，代表某个微信用户在AppID下的唯一标识。相同微信用户，AppID不同，则生成的OpenID不同。请注意：调用该接口传入的OpenID应当与商户在校园轻松付登记的AppID对应。如果传入的OpenID不合法或者与登记的AppID不对应，则可能出现：1、查询报错；2、查询返回的OpenID与请求上传的OpenID不一致。（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询用户欠费状态，见 <see cref="EducationSchoolPayUserDebtStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/eduschoolpay/users/{openid}/debt-state</c>；path <c>openid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：用户信息有误，请检查OpenID / 400 INVALID_REQUEST：商户未登记AppID / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。</para>
    /// </remarks>
    [Get("/v3/eduschoolpay/users/{openid}/debt-state")]
    Task<EducationSchoolPayUserDebtStateResponse> QueryEducationSchoolPayUserDebtStateAsync(
        [Path] string openid,
        CancellationToken cancellationToken = default);
}
