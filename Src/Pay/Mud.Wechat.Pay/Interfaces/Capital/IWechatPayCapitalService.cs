// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Capital;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「银行组件（省市 / 银行 / 支行 / 卡号开户行查询）」域 SDK（6 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中<b>6 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>六支官方全部只印服务商页</b>（「支持商户：【普通服务商】」，路由属 <c>/v3/partner</c> 文档树的「银行组件」章节）⇒ 无商户面镜像，<b>不得</b>按其余行业域的双页形态补建。</para>
/// <para><b>频率限制是逐支印出的</b>：六支官方正文均写「API按照接口-商户级别限频，单商户单接口最大请求频率不超过<b>300TPS</b>」⇒ 数字照页入各方法 remarks，本域不属于「官方未印 QPS」那一类。</para>
/// <para><b>本域是纯查询字典族 ⇒ 官方要求前端缓存</b>：省份 / 城市 / 银行 / 支行四支官方正文均写「如需检索，请在前端缓存所有XX数据并自行完成检索功能」 「业务查询到的数据为后端缓存快照，业务方需要<b>定期淘汰并更新数据</b>」⇒ 不得把本域当实时业务接口高频调用；SDK <b>不做</b>任何本地缓存。</para>
/// <para><b>翻页形态两支两支不同</b>：省份 / 城市两支官方<b>无请求参数</b>（返回全量 + <c>total_count</c>）；银行两支与支行支是「<c>limit</c> <b>必填</b> + <c>offset</c> 选填」⇒ <c>limit</c> 做成必传参数，不给默认值。</para>
/// <para><b>共用只发生在应答根类型</b>：对公与对私两支的官方应答表逐字段同形 ⇒ 共用 <see cref="CapitalBankPageResponse"/>； 支行支与卡号开户行支各建一型（前者根层另印四个银行定位键，后者无 <c>count</c> / <c>offset</c> / <c>links</c> 骨架），见守卫 CAP-B4。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 6 支里<b>1 支</b>（获取对私银行卡号开户银行）的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>且 <c>account_number</c> 官方注明<b>需加密上送</b>，其余五支只列 <c>Authorization</c> / <c>Accept</c> ⇒ 不得按「同域一律涉密」补齐。声明式签名一律不承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Capital", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayCapitalService
{
    /// <summary>
    /// 查询省份列表（不包含中国港澳台地区）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698641"/>（【普通服务商】）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询省份列表，见 <see cref="CapitalProvinceListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/capital/capitallhh/areas/provinces</c>；<b>无路径参数、无查询参数</b>（官方请求头只列 <c>Authorization</c> / <c>Accept</c>）。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：API按照接口-商户级别限频，单商户单接口最大请求频率不超过300TPS。</para>
    /// <para><b>官方检索口径</b>：原文「如需检索，请在前端缓存所有省份数据并自行完成检索功能」⇒ 本支返回<b>全量</b>省份，无分页参数。</para>
    /// <para><b>业务错误码</b>：404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。</para>
    /// </remarks>
    [Get("/v3/capital/capitallhh/areas/provinces")]
    Task<CapitalProvinceListResponse> QueryCapitalProvincesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据省份编码查询该省下的城市列表（不包含中国港澳台地区城市）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697470"/>（【普通服务商】）。
    /// </summary>
    /// <param name="provinceCode">【省份编码】 省份编码，唯一标识一个省份（官方 path <c>province_code</c>，<b>必填</b>，类型 <c>integer</c>），取值来自「查询省份列表」支。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询城市列表，见 <see cref="CapitalCityListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/capital/capitallhh/areas/provinces/{province_code}/cities</c>；path <c>province_code</c>，<b>无查询参数</b>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：API按照接口-商户级别限频，单商户单接口最大请求频率不超过300TPS。</para>
    /// <para><b>官方用途口径</b>：原文「通过本接口根据省份编码获取省份下的城市列表信息，不包含中国港澳台地区城市信息，可用于支行数据过滤查询」 ⇒ 本支是「查询支行列表」的前置定位支。</para>
    /// <para><b>业务错误码</b>：404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。</para>
    /// </remarks>
    [Get("/v3/capital/capitallhh/areas/provinces/{provinceCode}/cities")]
    Task<CapitalCityListResponse> QueryCapitalCitiesAsync(
        [Path] long provinceCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支持对公业务的银行列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697626"/>（【普通服务商】）。
    /// </summary>
    /// <param name="limit">【最大资源条数】 该次请求可返回的最大资源条数（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="offset">【请求资源起始位置】 该次请求资源的起始位置，从 0 开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询支持对公业务的银行列表，见 <see cref="CapitalBankPageResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/capital/capitallhh/banks/corporate-banking</c>；query <c>offset</c>（选填）/ <c>limit</c>（<b>必填</b>）。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：API按照接口-商户级别限频，单商户单接口最大请求频率不超过300TPS。</para>
    /// <para><b>与「查询支持个人业务的银行列表」应答同形</b>：两支官方应答表逐字段一致 ⇒ 共用 <see cref="CapitalBankPageResponse"/>（守卫 CAP-B4），<b>但两支是不同业务开放面</b>（对公账户 vs 对私银行卡），不得并成一支方法。</para>
    /// <para><b>官方数据时效口径</b>：原文「业务查询到的数据为后端缓存快照，业务方需要定期淘汰并更新数据」⇒ 结果不可当实时数据使用。</para>
    /// <para><b>业务错误码</b>：404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。</para>
    /// </remarks>
    [Get("/v3/capital/capitallhh/banks/corporate-banking")]
    Task<CapitalBankPageResponse> QueryCapitalCorporateBanksAsync(
        [Query("limit")] int limit,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支持个人业务的银行列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697583"/>（【普通服务商】）。
    /// </summary>
    /// <param name="limit">【最大资源条数】 该次请求可返回的最大资源条数（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="offset">【请求资源起始位置】 该次请求资源的起始位置，从 0 开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询支持个人业务的银行列表，见 <see cref="CapitalBankPageResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/capital/capitallhh/banks/personal-banking</c>；query <c>offset</c>（选填）/ <c>limit</c>（<b>必填</b>）。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：API按照接口-商户级别限频，单商户单接口最大请求频率不超过300TPS。</para>
    /// <para><b>官方路由是 <c>personal-banking</c> 而非 <c>private-banking</c></b>：正文标题「查询支持<b>个人</b>业务的银行列表」⇒ 照页不得按「对公 / 对私」的直觉改成 <c>private-banking</c>。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：银行卡非储蓄卡 / 查询结果超限 / 404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。</para>
    /// </remarks>
    [Get("/v3/capital/capitallhh/banks/personal-banking")]
    Task<CapitalBankPageResponse> QueryCapitalPersonalBanksAsync(
        [Query("limit")] int limit,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据银行别名编码与城市编码查询支行列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697673"/>（【普通服务商】）。
    /// </summary>
    /// <param name="bankAliasCode">【银行别名编码】 银行别名的编码，与 bank_alias 一一对应（官方 path <c>bank_alias_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cityCode">【城市编码】 城市编码，唯一标识一个城市（官方 query <c>city_code</c>，<b>必填</b>，类型 <c>integer</c>），取值来自「查询城市列表」支。</param>
    /// <param name="limit">【最大资源条数】 该次请求可返回的最大资源条数（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="offset">【请求资源起始位置】 该次请求资源的起始位置，从 0 开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询支行列表，见 <see cref="CapitalBranchPageResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/capital/capitallhh/banks/{bank_alias_code}/branches</c>；path <c>bank_alias_code</c>，query <c>city_code</c>（<b>必填</b>）/ <c>offset</c>（选填）/ <c>limit</c>（<b>必填</b>）。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：API按照接口-商户级别限频，单商户单接口最大请求频率不超过300TPS。</para>
    /// <para><b>只对「需要填写支行的银行别名」开放</b>：官方原文「本接口可以用于根据银行别名编码（<b>仅支持需要填写支行的银行别名编码</b>）和城市编码过滤查询支行列表数据」 ⇒ 是否需填支行由银行列表支的 <c>need_bank_branch</c> 判别，不满足的编码调用即失败。</para>
    /// <para><b>应答根层另印四个银行定位键</b>（<c>account_bank</c> / <c>account_bank_code</c> / <c>bank_alias</c> / <c>bank_alias_code</c>）⇒ 照页建模进 <see cref="CapitalBranchPageResponse"/>，不得判为官方笔误删除。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：银行别名编码非数字格式 / 404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。</para>
    /// </remarks>
    [Get("/v3/capital/capitallhh/banks/{bankAliasCode}/branches")]
    Task<CapitalBranchPageResponse> QueryCapitalBranchesAsync(
        [Path] string bankAliasCode,
        [Query("city_code")] int cityCode,
        [Query("limit")] int limit,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据对私银行卡号查询开户银行信息（仅支持部分银行的对私银行卡）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697541"/>（【普通服务商】）。
    /// </summary>
    /// <param name="accountNumber">【银行卡号】 对私银行卡号，官方类型为 <c>string(512)</c>（官方 query <c>account_number</c>，<b>必填</b>）：<b>须以微信支付公钥或平台证书公钥加密后上送</b>，本参数承载<b>密文</b>，SDK 不代为加解密。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取对私银行卡号开户银行，见 <see cref="CapitalBankByAccountResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/capital/capitallhh/banks/search-banks-by-bank-account</c>；query <c>account_number</c>（<b>必填</b>，<c>string(512)</c> 密文）。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：API按照接口-商户级别限频，单商户单接口最大请求频率不超过300TPS。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>，且 <c>account_number</c> 官方注明需加密上送 ⇒ 本域唯一涉密支（其余五支只列 <c>Authorization</c> / <c>Accept</c>）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>本支 Query 凭据之外的敏感参数在 URL 上</b>：<c>account_number</c> 是密文仍属敏感串 ⇒ 宿主日志与错误上报须按脱敏口径处理请求 URI，不得原样落日志。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数格式错误 / 400 INVALID_REQUEST：无效请求 / 404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。</para>
    /// </remarks>
    [Get("/v3/capital/capitallhh/banks/search-banks-by-bank-account")]
    Task<CapitalBankByAccountResponse> SearchCapitalBanksByBankAccountAsync(
        [Query("account_number")] string accountNumber,
        CancellationToken cancellationToken = default);
}
