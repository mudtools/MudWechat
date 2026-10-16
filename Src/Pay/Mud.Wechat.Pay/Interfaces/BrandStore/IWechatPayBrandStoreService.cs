// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BrandStore;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌门店」域 SDK（9 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域九支官方页<b>全部未印</b>端点级 QPS 数字 ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>列表支的 <c>offset</c>/<c>limit</c> 官方选填</b>，与会员卡两支的<b>必填</b>相反 ⇒ 必填口径逐页照抄，
/// 不得按「列表一律必填分页」归一；过滤项 <c>store_state</c> 选填，<c>brand_id</c> 官方<b>必填</b>。</para>
/// <para><b>四种「变更」彼此独立</b>：<c>PATCH</c> 改门店信息、<c>DELETE</c> 删除门店记录、 <c>POST .../close</c> 与 <c>POST .../resume</c> 只改<b>营业状态</b>（应答同为 3 行、含 <c>store_state</c> 枚举）、 <c>POST .../bindrecipient</c> 与 <c>.../unbindrecipient</c> 改<b>收款商户号绑定</b> ⇒ 五支语义不可折叠。</para>
/// <para><b>门店创建 / 查询 / 更新三支应答同形</b>（21 行签名一致）⇒ 共用 <see cref="BrandStoreResponse"/>； 请求侧创建（13 行）与更新的<b>必填口径不同</b> ⇒ 仍各建一型。</para>
/// <para><b>绑定与解绑应答不同形</b>：绑定支 5 行、解绑支只 1 行（<c>failed_reason</c>）⇒ 分建； <b>删除支官方未印发应表</b> ⇒ 返回裸 <see cref="Task"/>，以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BrandStore", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBrandStoreService
{
    /// <summary>
    /// 品牌门店创建。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌门店创建请求体，字段见 <see cref="BrandStoreCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌门店创建，见 <see cref="BrandStoreResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/store/brandstores</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandStoreResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 400 ALREADY_EXISTS 品牌门店已存在 / 400 PARAM_ERROR 门店经度不合法 / 400 PARAM_ERROR 门店纬度不合法 / 400 PARAM_ERROR 经营时间不合法 / 400 PARAM_ERROR 分店名称不合法 / 400 PARAM_ERROR 门店地址编码不合法 / 400 PARAM_ERROR 服务电话不合法。</para>
    /// </remarks>
    [Post("/v3/brand/partner/store/brandstores")]
    Task<BrandStoreResponse> CreateBrandStoreAsync(
        [Body] BrandStoreCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌门店列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756674"/>（【普通服务商】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID。（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="storeState">【门店状态】 若不填，则查询所有门店。。可选取值：`OPEN` 门店生效中。门店审核通过创建成功后即为生效中，后续更新门店信息只会影响审核状态，不会改变门店状态，如门店暂停营业可调用“暂停门店营业 API”，如门店关闭可调用“删除品牌门店 API”。门店营业时间不影响门店状态。 / `CREATING` 门店创建中。在创建门店后，门店资料正在审核。审核详情请查看审核状态。 / `CLOSED` 门店停业中。不可用于微信支付生态中的其他业务，可删除该门店，删除后将无法恢复，请谨慎操作。（官方 query <c>store_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="offset">【分页起始位置】 从第几条开始查询，默认从第0条开始查询。（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页条数】 一次查询的最大门店条数，默认值为20，取值范围\[1,200\]。（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌门店列表，见 <see cref="BrandStoreListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/partner/store/brandstores</c>；query <c>brand_id</c> / <c>store_state</c> / <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限。</para>
    /// </remarks>
    [Get("/v3/brand/partner/store/brandstores")]
    Task<BrandStoreListResponse> QueryBrandStoresAsync(
        [Query("brand_id")] string brandId,
        [Query("store_state")] string? storeState = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌门店信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783153"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID。（官方 path <c>store_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID。（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌门店信息，见 <see cref="BrandStoreResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/partner/store/brandstores/{store_id}</c>；path <c>store_id</c>，query <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 404 NOT_FOUND 品牌门店不存在。</para>
    /// </remarks>
    [Get("/v3/brand/partner/store/brandstores/{storeId}")]
    Task<BrandStoreResponse> QueryBrandStoreAsync(
        [Path] string storeId,
        [Query("brand_id")] string brandId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌门店更新。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783158"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID。（官方 path <c>store_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">品牌门店更新请求体，字段见 <see cref="BrandStoreUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌门店更新，见 <see cref="BrandStoreResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/brand/partner/store/brandstores/{store_id}</c>；path <c>store_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandStoreResponse"/>（官方应答表逐字段建模，与 <c>POST /brand/partner/store/brandstores</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 404 NOT_FOUND 品牌门店不存在 / 400 PARAM_ERROR 门店状态暂不支持修改 / 400 PARAM_ERROR 门店经度不合法 / 400 PARAM_ERROR 门店纬度不合法 / 400 PARAM_ERROR 经营时间不合法 / 400 PARAM_ERROR 分店名称不合法 / 400 PARAM_ERROR 门店地址编码不合法 / 400 PARAM_ERROR 服务电话不合法。</para>
    /// </remarks>
    [Patch("/v3/brand/partner/store/brandstores/{storeId}")]
    Task<BrandStoreResponse> UpdateBrandStoreAsync(
        [Path] string storeId,
        [Body] BrandStoreUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌门店删除。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783113"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID。（官方 path <c>store_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">品牌门店删除请求体，字段见 <see cref="BrandStoreDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段 ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>DELETE</b> <c>/v3/brand/partner/store/brandstores/{store_id}</c>；path <c>store_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 404 NOT_FOUND 品牌门店不存在。</para>
    /// </remarks>
    [Delete("/v3/brand/partner/store/brandstores/{storeId}")]
    Task DeleteBrandStoreAsync(
        [Path] string storeId,
        [Body] BrandStoreDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 门店收款商户号绑定。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783177"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID。（官方 path <c>store_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">门店收款商户号绑定请求体，字段见 <see cref="BrandStoreBindRecipientRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>门店收款商户号绑定，见 <see cref="BrandStoreBindRecipientResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/store/brandstores/{store_id}/bindrecipient</c>；path <c>store_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandStoreBindRecipientResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 404 NOT_FOUND 品牌门店不存在。</para>
    /// </remarks>
    [Post("/v3/brand/partner/store/brandstores/{storeId}/bindrecipient")]
    Task<BrandStoreBindRecipientResponse> BindBrandStoreRecipientAsync(
        [Path] string storeId,
        [Body] BrandStoreBindRecipientRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 门店收款商户号解绑。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783188"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID。（官方 path <c>store_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">门店收款商户号解绑请求体，字段见 <see cref="BrandStoreUnbindRecipientRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>门店收款商户号解绑，见 <see cref="BrandStoreUnbindRecipientResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/store/brandstores/{store_id}/unbindrecipient</c>；path <c>store_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandStoreUnbindRecipientResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 404 NOT_FOUND 品牌门店不存在。</para>
    /// </remarks>
    [Post("/v3/brand/partner/store/brandstores/{storeId}/unbindrecipient")]
    Task<BrandStoreUnbindRecipientResponse> UnbindBrandStoreRecipientAsync(
        [Path] string storeId,
        [Body] BrandStoreUnbindRecipientRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 暂停门店营业。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756671"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID。（官方 path <c>store_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">暂停门店营业请求体，字段见 <see cref="BrandStoreCloseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>暂停门店营业，见 <see cref="BrandStoreStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/store/brandstores/{store_id}/close</c>；path <c>store_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandStoreStateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 404 NOT_FOUND 品牌门店不存在。</para>
    /// </remarks>
    [Post("/v3/brand/partner/store/brandstores/{storeId}/close")]
    Task<BrandStoreStateResponse> CloseBrandStoreAsync(
        [Path] string storeId,
        [Body] BrandStoreCloseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复门店营业。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756672"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID。（官方 path <c>store_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">恢复门店营业请求体，字段见 <see cref="BrandStoreResumeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>恢复门店营业，见 <see cref="BrandStoreStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/store/brandstores/{store_id}/resume</c>；path <c>store_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandStoreStateResponse"/>（官方应答表逐字段建模，与 <c>POST /brand/partner/store/brandstores/{store_id}/close</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 403 NO_AUTH 无操作权限 / 404 NOT_FOUND 品牌门店不存在。</para>
    /// </remarks>
    [Post("/v3/brand/partner/store/brandstores/{storeId}/resume")]
    Task<BrandStoreStateResponse> ResumeBrandStoreAsync(
        [Path] string storeId,
        [Body] BrandStoreResumeRequest request,
        CancellationToken cancellationToken = default);
}
