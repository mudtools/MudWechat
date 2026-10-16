// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.EducationPapay;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「教育委托代扣（EducationPAPay）」域 SDK（8 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 8 支中<b>1 支官方只印一页</b>、<b>7 支印多页</b>（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 8 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 3 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>七支印双页、一支只印商户页</b>：本域 <c>/edu-papay/*</c> 八支里七支同时印【普通商户】与【普通服务商】两页 ⇒ 按<b>并集</b>建模、一面一方法，身份差异（服务商页多 <c>sub_mchid</c> / <c>sub_appid</c>）写进参数注释而不拆方法； 但 <c>DELETE /v3/edu-papay/contracts/{contract_id}</c>（解除协议）<b>官方只印商户页</b> ⇒ 该支无服务商镜像， <b>不得</b>按「同域其余支都有双页」补齐，也不得把服务商页文档链接错挂到本支（处置表原引的服务商 docId 实为 <c>GET /v3/edu-papay/user/{openid}/contracts</c> 的服务商页，已逐对核正）。</para>
/// <para><b>三支官方成功应答是 204 无包体</b>：解除协议（DELETE）、下单受理（POST transactions）、 发送预扣费通知（POST user-notifications）⇒ 一律返回裸 <see cref="Task"/>，不建响应类型； 其中「下单」是<b>受理</b>不是扣款成功，扣款结果只由异步通知与查单支判定。</para>
/// <para><b>两支查单应答逐字段同形 ⇒ 共用一型</b>：<c>/transactions/id/{transaction_id}</c> 与 <c>/transactions/out-trade-no/{out_trade_no}</c> 官方应答表 33 行同形且必填口径一致 ⇒ 共用一个响应类型。 但「按 <c>contract_id</c> 查协议」与「按 <c>openid</c> 查协议列表」两支<b>各建一应答</b>： 列表支的 <c>data[]</c> 元素与单查应答字段同形，本线口径是<b>共用只发生在应答根类型</b>、 子树按「端点 + 路径段」机械拼装独立成类型 ⇒ 不得为「看起来一样」把单查应答当成列表元素类型。</para>
/// <para><b>预签约支只回 <c>presign_token</c></b>：官方两页应答表单行且<b>必填</b> ⇒ 该 token 是调起签约小程序/公众号页面的入参， 不等于签约成功；签约结果须回查协议支。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "EducationPapay", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEducationPapayService
{
    /// <summary>
    /// 解除教育代扣签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466882"/>（【普通商户】）。
    /// </summary>
    /// <param name="contractId">【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>DELETE</b> <c>/v3/edu-papay/contracts/{contract_id}</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Delete("/v3/edu-papay/contracts/{contractId}")]
    Task TerminateEducationPapayContractAsync(
        [Path] string contractId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按微信协议号查询教育代扣签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012522951"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465367"/>（【普通服务商】）。
    /// </summary>
    /// <param name="contractId">【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="appid">【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="subAppid">【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，不用填写（官方 query <c>sub_appid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>按微信协议号查询教育代扣签约，见 <see cref="EducationPapayContractInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/edu-papay/contracts/id/{contract_id}</c>；path <c>contract_id</c>，query <c>appid</c> / <c>sub_mchid</c> / <c>sub_appid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>appid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Get("/v3/edu-papay/contracts/id/{contractId}")]
    Task<EducationPapayContractInfoResponse> QueryEducationPapayContractByIdAsync(
        [Path] string contractId,
        [Query("appid")] string appid,
        [Query("sub_mchid")] string? subMchid = null,
        [Query("sub_appid")] string? subAppid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按用户查询教育代扣签约列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523265"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012489548"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【服务商APPID下的用户标识】 用户在服务商appid下的唯一标识（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="appid">【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="planId">【签约模版号】 签约模版号，签约模版唯一标识（官方 query <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="contractStatus">【签约状态】 ADD-签约（默认）; DELETE-解约（官方 query <c>contract_status</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="offset">【分页页码】 该次请求资源的起始位置，从0开始计数。调用方选填，默认为0（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 该次请求可返回的最大资源条数。调用方选填，默认值为20（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="subAppid">【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，不用填写（官方 query <c>sub_appid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="subOpenid">【子商户APPID下的用户标识】 用户在子商户appid下的唯一标识（官方 query <c>sub_openid</c>，选填，类型 <c>string(128)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>按用户查询教育代扣签约列表，见 <see cref="EducationPapayUserContractListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/edu-papay/user/{openid}/contracts</c>；path <c>openid</c>，query <c>appid</c> / <c>plan_id</c> / <c>contract_status</c> / <c>offset</c> / <c>limit</c> / <c>sub_mchid</c> / <c>sub_appid</c> / <c>sub_openid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>appid</c> 与 <c>plan_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Get("/v3/edu-papay/user/{openid}/contracts")]
    Task<EducationPapayUserContractListResponse> QueryEducationPapayUserContractsAsync(
        [Path] string openid,
        [Query("appid")] string appid,
        [Query("plan_id")] string planId,
        [Query("contract_status")] string? contractStatus = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("sub_mchid")] string? subMchid = null,
        [Query("sub_appid")] string? subAppid = null,
        [Query("sub_openid")] string? subOpenid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 教育代扣预签约。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012522560"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465360"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">教育代扣预签约请求体，字段见 <see cref="EducationPapayContractPresignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>教育代扣预签约，见 <see cref="EducationPapayContractPresignResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/edu-papay/contracts/presign</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>应答</b>见 <see cref="EducationPapayContractPresignResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Post("/v3/edu-papay/contracts/presign")]
    Task<EducationPapayContractPresignResponse> CreateEducationPapayContractPresignAsync(
        [Body] EducationPapayContractPresignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 教育代扣下单（受理）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524731"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465548"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">教育代扣下单（受理）请求体，字段见 <see cref="EducationPapayTransactionCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/edu-papay/transactions</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Post("/v3/edu-papay/transactions")]
    Task CreateEducationPapayTransactionAsync(
        [Body] EducationPapayTransactionCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 教育代扣订单查询（微信订单号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523468"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465567"/>（【普通服务商】）。
    /// </summary>
    /// <param name="transactionId">【微信支付订单号】 微信支付订单号（官方 path <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【子商户号】 子商户号（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>教育代扣订单查询（微信订单号），见 <see cref="EducationPapayTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/edu-papay/transactions/id/{transaction_id}</c>；path <c>transaction_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Get("/v3/edu-papay/transactions/id/{transactionId}")]
    Task<EducationPapayTransactionQueryResponse> QueryEducationPapayTransactionByIdAsync(
        [Path] string transactionId,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 教育代扣订单查询（商户订单号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523591"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465574"/>（【普通服务商】）。
    /// </summary>
    /// <param name="outTradeNo">【商户订单号】 商户订单号（官方 path <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>教育代扣订单查询（商户订单号），见 <see cref="EducationPapayTransactionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/edu-papay/transactions/out-trade-no/{out_trade_no}</c>；path <c>out_trade_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Get("/v3/edu-papay/transactions/out-trade-no/{outTradeNo}")]
    Task<EducationPapayTransactionQueryResponse> QueryEducationPapayTransactionByOutTradeNoAsync(
        [Path] string outTradeNo,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送扣费前用户通知。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524814"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012855408"/>（【普通服务商】）。
    /// </summary>
    /// <param name="contractId">【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方 path <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">发送扣费前用户通知请求体，字段见 <see cref="EducationPapayUserNotificationSendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/edu-papay/user-notifications/{contract_id}/send</c>；path <c>contract_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。</para>
    /// </remarks>
    [Post("/v3/edu-papay/user-notifications/{contractId}/send")]
    Task SendEducationPapayUserNotificationAsync(
        [Path] string contractId,
        [Body] EducationPapayUserNotificationSendRequest request,
        CancellationToken cancellationToken = default);
}
