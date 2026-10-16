// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PayrollCard;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「代发薪（PayrollCard 服务商面）」域 SDK（6 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中<b>6 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：1 / 6 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 6 支里<b>3 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>六支官方全部只印服务商页</b>（各页均标「支持商户：【普通服务商】」）⇒ 无直连面镜像、无双页归并问题。 <b>本域没有 <c>sp_mchid</c></b>：请求侧只有 <c>sub_mchid</c>（服务商身份由签名主体承载）， 而<b>应答</b>侧的服务商商户号字段名是 <c>mchid</c>；AppID 命名亦两套并存——令牌申领、授权关系与认证单三支用 <c>appid</c>（服务商）+ <c>sub_appid</c>，发起批次支用 <c>sp_appid</c> + <c>sub_appid</c> ⇒ 按「服务商面必是 sp_*」归一即错。</para>
/// <para><b>路由分四段、生命周期是两段式</b>：<c>/payroll-card/tokens</c>（申领授权令牌）→ <c>/payroll-card/authentications/pre-order-with-auth</c>（凭核身单号预下单取令牌）→ <c>/payroll-card/transfer-batches</c>（发起批次），外加 <c>/relations/{openid}</c> 与认证单两支查询。 「申领令牌」与「凭令牌发薪」不可折叠成一支，<c>/payroll-card/authentications</c> 的三支也各按定位键分列。</para>
/// <para><b>密文与必填口径逐页钉死，不得跨页套用</b>：<c>user_name</c> 与 <c>id_card_number</c> 在令牌支和预下单支 官方都标「按照APIV3标准加密该字段」且类型 <c>string(1024)</c>；但 <c>employment_type</c> 在预下单支是<b>必填</b>、 在令牌支<b>选填</b>（官方原文「未授权必填，授权过非必填」）⇒ 两个请求类型各建、口径照页不得归一。 本域证件号叫 <c>id_card_number</c>，进件域是 <c>identification_no</c> ⇒ 跨域「对齐字段名」即造出官方不存在的报文。</para>
/// <para><b>令牌申领应答与预下单应答不同形 ⇒ 各建一型</b>：前者 5 键（<c>openid</c> / <c>mchid</c> / <c>sub_mchid</c> / <c>token</c> / <c>expires_in</c>），后者多 <c>authenticate_number</c>（商家核身单号，即认证单查询支的定位键）⇒ 6 键。 按「字段高度重合」合并即造出官方不存在的契约。</para>
/// <para><b>认证单两支各建一应答</b>：按 <c>authenticate_number</c> 单查的应答与列表支 <c>data[]</c> 元素 官方字段表<b>逐字同形</b>（13 键同序），但本线口径是<b>共用只发生在应答根类型</b>、子树按「端点 + 路径段」 机械拼装独立成类型 ⇒ 列表元素类型不与单查应答共用；列表支另带 <c>total_count</c> / <c>offset</c> / <c>limit</c> 包装。</para>
/// <para><b>分页三键在「应答侧」必填、在「query 侧」选填</b>：列表支的 <c>total_count</c> / <c>offset</c> / <c>limit</c> 是官方<b>应答表</b>必填三键，而其请求 query 的 <c>offset</c> / <c>limit</c> 官方标<b>选填</b>，单查支根本没有这两键 ⇒ 同名字段分属两侧、两套口径不得互套，也不得把 query 侧做成必填。</para>
/// <para><b>本域唯一印端点级 QPS 的接口</b>：<c>POST /v3/payroll-card/transfer-batches</c> 官方原文 「单个服务商（发起批量转账接口）50QPS，超过限制会报 <c>FREQUENCY_LIMITED</c>」⇒ 已照页写入该方法 remarks； 其余五支官方未印数字，不得凭第三方 SDK 补数。</para>
/// <para><b>清单里两条 <c>/payroll-card/wesure/*</c>（insurance、token-validations）判非契约、零声明零建模</b>： 该路由族在 PayrollCard 六支现行页正文里<b>零命中</b>，且表内为 token-validations 引的官方页 4012461785 实为 <c>GET /v3/payroll-card/authentications/{authenticate_number}</c>（张冠李戴）⇒ 与 P0-9 微企付 「官方出处缺失」同一证据面，由零声明守卫断言其在 <c>Src/</c> 全源码零声明。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PayrollCard", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPayrollCardService
{
    /// <summary>
    /// 获取代发薪用户授权令牌。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461589"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">获取代发薪用户授权令牌请求体，字段见 <see cref="PayrollCardTokenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取代发薪用户授权令牌，见 <see cref="PayrollCardTokenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payroll-card/tokens</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="PayrollCardTokenResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_MISSING：参数缺失 / 400 PARAM_ILLEGAL：参数值不合法 / 401 MCH_TICKET_ILLEGAL：服务商票据校验失败 / 400 AUTH_ERROR：校验身份或者产品权限失败 / 403 GEN_TOKEN_ERROR：生成token失败 / 403 MERCHANT_INFO_ERROR：获取商户信息失败 / 403 AUTH_INFO_ERROR：获取授权信息失败。</para>
    /// </remarks>
    [Post("/v3/payroll-card/tokens")]
    Task<PayrollCardTokenResponse> CreatePayrollCardTokenAsync(
        [Body] PayrollCardTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户代发薪授权与开通状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461728"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 微信用户OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【特约商户号】 特约商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="appid">【服务商AppID】 当输入服务商AppID时会校验其与服务商商户号绑定关系。 服务商AppID和特约商户AppID至少输入一个。（官方 query <c>appid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="subAppid">【特约商户AppID】 当输入特约商户AppID时会校验其与特约商户号关系。 特约商户AppID和服务商AppID至少输入一个。（官方 query <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询用户代发薪授权与开通状态，见 <see cref="PayrollCardRelationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payroll-card/relations/{openid}</c>；path <c>openid</c>，query <c>sub_mchid</c> / <c>appid</c> / <c>sub_appid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_MISSING：参数缺失 / 400 PARAM_ILLEGAL：参数值不合法 / 401 MCH_TICKET_ILLEGAL：服务商票据校验失败 / 400 AUTH_ERROR：校验身份或者产品权限失败 / 403 MERCHANT_INFO_ERROR：获取商户信息失败 / 403 AUTH_INFO_ERROR：获取授权信息失败。</para>
    /// </remarks>
    [Get("/v3/payroll-card/relations/{openid}")]
    Task<PayrollCardRelationResponse> QueryPayrollCardRelationAsync(
        [Path] string openid,
        [Query("sub_mchid")] string subMchid,
        [Query("appid")] string? appid = null,
        [Query("sub_appid")] string? subAppid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 凭授权单号预下单取令牌。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461761"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">凭授权单号预下单取令牌请求体，字段见 <see cref="PayrollCardAuthenticationPreOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>凭授权单号预下单取令牌，见 <see cref="PayrollCardAuthenticationPreOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payroll-card/authentications/pre-order-with-auth</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="PayrollCardAuthenticationPreOrderResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：商家核身单号已使用，请调整后再试 / 400 INVALID_REQUEST：该商户当日核身下单已超限额 / 400 INVALID_REQUEST：校验商户身份失败 / 400 INVALID_REQUEST：校验商户身份或者产品权限失败 / 400 INVALID_REQUEST：下单用工类型与授权用工类型不匹配 / 400 INVALID_REQUEST：核身下单过于频繁 / 400 PARAM_ERROR：参数值不正确，必填参数为空。</para>
    /// </remarks>
    [Post("/v3/payroll-card/authentications/pre-order-with-auth")]
    Task<PayrollCardAuthenticationPreOrderResponse> CreatePayrollCardAuthenticationPreOrderAsync(
        [Body] PayrollCardAuthenticationPreOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询代发薪授权单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461785"/>（【普通服务商】）。
    /// </summary>
    /// <param name="authenticateNumber">【商家核身单号】 商户系统内部的商家核身单号，要求此参数只能由数字、大小写字母组成，在服务商内部唯一（官方 path <c>authenticate_number</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【特约商户号】 特约商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询代发薪授权单，见 <see cref="PayrollCardAuthenticationInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payroll-card/authentications/{authenticate_number}</c>；path <c>authenticate_number</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Get("/v3/payroll-card/authentications/{authenticateNumber}")]
    Task<PayrollCardAuthenticationInfoResponse> QueryPayrollCardAuthenticationAsync(
        [Path] string authenticateNumber,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按日期分页查询代发薪授权单列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461820"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【用户OpenID】 微信用户OpenID（官方 query <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【特约商户号】 特约商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="authenticateDate">【核身日期】 核身日期，一次只能查询一天，最久可查询90天内的记录，格式为“yyyy-MM-DD”（官方 query <c>authenticate_date</c>，<b>必填</b>，类型 <c>string(10)</c>）。</param>
    /// <param name="appid">【服务商AppID】 当输入服务商AppID时会校验其与服务商商户号绑定关系。 服务商AppID和特约商户AppID至少输入一个。（官方 query <c>appid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="subAppid">【特约商户AppID】 当输入特约商户AppID时会校验其与特约商户号关系。 特约商户AppID和服务商AppID至少输入一个。（官方 query <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="authenticateState">【核身状态】 核身状态，列表查询仅提供成功状态的核身记录查询，故此字段固定默认值即可（官方 query <c>authenticate_state</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="offset">【本次查询偏移量】 非负整数，表示该次请求资源的起始位置，从0开始计数。调用方选填，默认为0。offset为10，limit为10时，查询第10-19条数据（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【本次请求最大查询条数】 非0非负的整数，该次请求可返回的最大资源条数，默认值为10，最大支持10条。（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>按日期分页查询代发薪授权单列表，见 <see cref="PayrollCardAuthenticationListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payroll-card/authentications</c>；query <c>openid</c> / <c>sub_mchid</c> / <c>authenticate_date</c> / <c>appid</c> / <c>sub_appid</c> / <c>authenticate_state</c> / <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>openid</c> 与 <c>sub_mchid</c> 与 <c>authenticate_date</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Get("/v3/payroll-card/authentications")]
    Task<PayrollCardAuthenticationListResponse> QueryPayrollCardAuthenticationsAsync(
        [Query("openid")] string openid,
        [Query("sub_mchid")] string subMchid,
        [Query("authenticate_date")] string authenticateDate,
        [Query("appid")] string? appid = null,
        [Query("sub_appid")] string? subAppid = null,
        [Query("authenticate_state")] string? authenticateState = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起代发薪批量转账。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461824"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">发起代发薪批量转账请求体，字段见 <see cref="PayrollCardTransferBatchCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发起代发薪批量转账，见 <see cref="PayrollCardTransferBatchCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payroll-card/transfer-batches</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：单个服务商（发起批量转账接口）50QPS，如果超过频率限制，会报错FREQUENCY\_LIMITED，请降低频率请求。。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="PayrollCardTransferBatchCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户信息不合法 / 403 ACCOUNT_ERROR：商户账户付款受限 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 APPID_MCHID_NOT_MATCH：商户号和AppID没有绑定关系 / 403 NOT_ENOUGH：资金不足 / 429 QUOTA_EXCEED：超出商户单日转账额度 / 429 FREQUENCY_LIMITED：频率超限 / 403 NO_AUTH：未开通产品权限。</para>
    /// </remarks>
    [Post("/v3/payroll-card/transfer-batches")]
    Task<PayrollCardTransferBatchCreateResponse> CreatePayrollCardTransferBatchAsync(
        [Body] PayrollCardTransferBatchCreateRequest request,
        CancellationToken cancellationToken = default);
}
