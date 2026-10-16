// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.TaxiInvoice;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「出租车电子发票」域 SDK（9 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 9 支中<b>9 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 9 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 9 支里<b>1 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>本域十支官方全部只印服务商页</b>（各页均标「支持商户：【普通服务商】」）⇒ 无商户面镜像、无双页归并问题， 一支一页一方法。</para>
/// <para><b>两支为 204 无应答包体</b>：签到签退与插卡官方应答节原文「无应答包体」+ 正常示例 <b>204 No Content</b> ⇒ 返回裸 <see cref="Task"/>，不建响应类型；插卡的业务结果须先调「上传出租车电子发票文件」取文件 ID（有效期 3 天）。</para>
/// <para><b>上传发票文件一支刻意不在本接口内</b>（非遗漏）：官方为 <c>multipart/form-data</c>（表单 <c>file</c> + <c>meta</c>）， <c>meta.digest</c> 要求<b>国密 SM3</b> 十六进制摘要且 SM3 不在 .NET BCL 内 ⇒ 声明式 JSON 端点无法承载， 由手写通道 <c>WechatPayTaxiInvoiceFileService</c> 承载。注意本页官方字段名是 <b><c>digest_algorithm</c></b>（拼写正常）， 与电子发票域「<c>digest_alogrithm</c> 少一个 r」<b>不是同一契约</b>，不得互相「纠正」。</para>
/// <para><b><c>region_id</c> 是本域的定位基座</b>：十支里九支携带该字段，且「查司机 / 查出租车 / 查公司」三支把它放在 <b>必填 query</b>（不是请求体）⇒ 单靠 <c>driver_license</c> / <c>plate_number</c> / <c>company_mchid</c> 不足以定位，照页不得省。</para>
/// <para><b>「更新」支的请求与应答不共用</b>：<c>update-driver</c> 与 <c>update-taxi</c> 两侧字段名集相近但必填口径不同 （请求侧 <c>company_name</c> / <c>mchid</c> 必填、应答侧 <c>company_name</c> / <c>mchid</c> 亦必填而 <c>driver_category</c> 等口径各异） ⇒ 请求与应答各建一型，不做超集合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "TaxiInvoice", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayTaxiInvoiceService
{
    /// <summary>
    /// 出租车司机签到签退。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461368"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">出租车司机签到签退请求体，字段见 <see cref="TaxiInvoiceAttendancePunchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/taxi-invoice/attendance/punch</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 429 RATE_LIMITED：频率限制。</para>
    /// </remarks>
    [Post("/v3/taxi-invoice/attendance/punch")]
    Task PunchTaxiInvoiceAttendanceAsync(
        [Body] TaxiInvoiceAttendancePunchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将出租车电子发票插入用户卡包。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461432"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">将出租车电子发票插入用户卡包请求体，字段见 <see cref="TaxiInvoiceCardInsertRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/taxi-invoice/cards</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：请检查参数格式是否符合要求 / 403 NO_AUTH：出租车公司不属于服务商 / 403 NO_AUTH：服务商无权限 / 404 NOT_FOUND：所查询的出租车不存在 / 404 NOT_FOUND：所查询的出租车司机不存在 / 404 NOT_FOUND：未找到指定的出租车经营主体信息。</para>
    /// </remarks>
    [Post("/v3/taxi-invoice/cards")]
    Task InsertTaxiInvoiceCardAsync(
        [Body] TaxiInvoiceCardInsertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新司机信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460171"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">更新司机信息请求体，字段见 <see cref="TaxiInvoiceDriverUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>更新司机信息，见 <see cref="TaxiInvoiceDriverUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/taxi-invoice/driver/update-driver</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>应答</b>见 <see cref="TaxiInvoiceDriverUpdateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：请检查参数格式是否符合要求 / 400 INVALID_REQUEST：身份信息不一致或不匹配 / 403 NO_AUTH：出租车公司不属于服务商 / 403 NO_AUTH：服务商无权限 / 404 NOT_FOUND：所查询的出租车司机不存在 / 404 NOT_FOUND：未找到指定的出租车经营主体信息。</para>
    /// </remarks>
    [Post("/v3/taxi-invoice/driver/update-driver")]
    Task<TaxiInvoiceDriverUpdateResponse> UpdateTaxiInvoiceDriverAsync(
        [Body] TaxiInvoiceDriverUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取司机信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461309"/>（【普通服务商】）。
    /// </summary>
    /// <param name="driverLicense">【营运资格证号】 营运资格证号（官方 path <c>driver_license</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="regionId">【行政区划代码】 行政区划代码（官方 query <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取司机信息，见 <see cref="TaxiInvoiceDriverInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/taxi-invoice/drivers/{driver_license}</c>；path <c>driver_license</c>，query <c>region_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>region_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：请检查参数格式是否符合要求 / 403 NO_AUTH：出租车公司不属于服务商 / 403 NO_AUTH：服务商无权限 / 404 NOT_FOUND：所查询的出租车不存在 / 404 NOT_FOUND：未找到指定的出租车经营主体信息。</para>
    /// </remarks>
    [Get("/v3/taxi-invoice/drivers/{driverLicense}")]
    Task<TaxiInvoiceDriverInfoResponse> QueryTaxiInvoiceDriverAsync(
        [Path] string driverLicense,
        [Query("region_id")] int regionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新出租车信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460167"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">更新出租车信息请求体，字段见 <see cref="TaxiInvoiceTaxiUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>更新出租车信息，见 <see cref="TaxiInvoiceTaxiUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/taxi-invoice/taxi/update-taxi</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="TaxiInvoiceTaxiUpdateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：请检查参数格式是否符合要求 / 403 NO_AUTH：出租车公司不属于服务商 / 403 NO_AUTH：服务商无权限 / 404 NOT_FOUND：所查询的出租车司机不存在 / 404 NOT_FOUND：未找到指定的出租车经营主体信息。</para>
    /// </remarks>
    [Post("/v3/taxi-invoice/taxi/update-taxi")]
    Task<TaxiInvoiceTaxiUpdateResponse> UpdateTaxiInvoiceTaxiAsync(
        [Body] TaxiInvoiceTaxiUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取出租车信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460169"/>（【普通服务商】）。
    /// </summary>
    /// <param name="plateNumber">【车牌号】 车牌号（官方 path <c>plate_number</c>，<b>必填</b>，类型 <c>string(16)</c>）。</param>
    /// <param name="regionId">【行政区划代码】 行政区划代码（官方 query <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取出租车信息，见 <see cref="TaxiInvoiceTaxiInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/taxi-invoice/taxies/{plate_number}</c>；path <c>plate_number</c>，query <c>region_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>region_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：请检查参数格式是否符合要求 / 403 NO_AUTH：出租车公司不属于服务商 / 403 NO_AUTH：服务商无权限 / 404 NOT_FOUND：所查询的出租车不存在 / 404 NOT_FOUND：未找到指定的出租车经营主体信息。</para>
    /// </remarks>
    [Get("/v3/taxi-invoice/taxies/{plateNumber}")]
    Task<TaxiInvoiceTaxiInfoResponse> QueryTaxiInvoiceTaxiAsync(
        [Path] string plateNumber,
        [Query("region_id")] int regionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取出租车公司商户信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460161"/>（【普通服务商】）。
    /// </summary>
    /// <param name="companyMchid">【公司商户号】 公司商户号（官方 path <c>company_mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="regionId">【行政区划代码】 行政区划代码（官方 query <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取出租车公司商户信息，见 <see cref="TaxiInvoiceCompanyInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/taxi-invoice/taxi-companies/{company_mchid}</c>；path <c>company_mchid</c>，query <c>region_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>region_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：请检查参数格式是否符合要求 / 403 NO_AUTH：出租车公司不属于服务商 / 403 NO_AUTH：服务商无权限 / 404 NOT_FOUND：未找到指定的出租车经营主体信息。</para>
    /// </remarks>
    [Get("/v3/taxi-invoice/taxi-companies/{companyMchid}")]
    Task<TaxiInvoiceCompanyInfoResponse> QueryTaxiInvoiceTaxiCompanyAsync(
        [Path] string companyMchid,
        [Query("region_id")] int regionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增出租车公司。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460154"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">新增出租车公司请求体，字段见 <see cref="TaxiInvoiceCompanyCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>新增出租车公司，见 <see cref="TaxiInvoiceCompanyCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/taxi-invoice/taxi-company/create-taxi-company</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="TaxiInvoiceCompanyCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR：请检查参数格式是否符合要求 / 429 RATE_LIMITED：频率限制。</para>
    /// </remarks>
    [Post("/v3/taxi-invoice/taxi-company/create-taxi-company")]
    Task<TaxiInvoiceCompanyCreateResponse> CreateTaxiInvoiceTaxiCompanyAsync(
        [Body] TaxiInvoiceCompanyCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 凭凭证查询乘客行程单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460180"/>（【普通服务商】）。
    /// </summary>
    /// <param name="token">【获取乘客行程单详情的凭证】 获取乘客行程单详情的凭证（官方 query <c>token</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</param>
    /// <param name="openid">【公众号或小程序的用户的微信OpenID】 公众号或小程序的用户的微信OpenID（官方 query <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="appid">【公众号或小程序的AppID】 公众号或小程序的AppID（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="authMode">【授权模式】 授权模式。可选取值：`ORDER_AUTH` 协议授权的基础上，还需对每笔订单进行授权 / `PROTOCOL_AUTH` 用户同意协议，授权查询（官方 query <c>auth_mode</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="regionId">【行政区划代码】 行政区划代码（官方 query <c>region_id</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>凭凭证查询乘客行程单，见 <see cref="TaxiInvoiceUserOrderFindResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/taxi-invoice/user-taxi-order/find-by-token</c>；query <c>token</c> / <c>openid</c> / <c>appid</c> / <c>auth_mode</c> / <c>region_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>token</c> 与 <c>openid</c> 与 <c>appid</c> 与 <c>auth_mode</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NO_AUTH：用户未授权 / 403 NO_AUTH：订单未授权 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 429 RATE_LIMITED：频率限制。</para>
    /// </remarks>
    [Get("/v3/taxi-invoice/user-taxi-order/find-by-token")]
    Task<TaxiInvoiceUserOrderFindResponse> QueryTaxiInvoiceUserTaxiOrderAsync(
        [Query("token")] string token,
        [Query("openid")] string openid,
        [Query("appid")] string appid,
        [Query("auth_mode")] string authMode,
        [Query("region_id")] int? regionId = null,
        CancellationToken cancellationToken = default);
}
