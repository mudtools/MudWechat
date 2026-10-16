// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BrandProfitSharing;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌分账」域 SDK（9 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域九支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c>
/// （分账频率过高 / 查询频率过高两种措辞）⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>本域零 <c>brand_id</c>、零列表支</b>：query 全部是单据定位键（<c>sub_mchid</c> + <c>transaction_id</c> +
/// <c>out_order_no</c>，回退单再加 <c>out_return_no</c> / <c>order_id</c>）⇒ 品牌作用域走签名主体与 <c>sub_mchid</c>，
/// 不存在可补的分页参数。</para>
/// <para><b>与 <see cref="IWechatPayProfitSharingService"/> 是同族不同路由</b>：本域全部走 <c>/v3/brand/profitsharing/*</c>， 请求/应答多 <c>brand_mchid</c>（品牌主商户号）与 <c>sub_mchid</c>（收款方）两级商户口径 ⇒ DTO 与接收方类型<b>不得</b>与普通服务商分账互相套用。</para>
/// <para><b>分账与回退各两支</b>：发起 <c>POST /orders</c>（15 行应答）与结果查询 <c>GET /orders</c>（16 行， 多 <c>finish_amount</c> / <c>finish_description</c>，仅查询分账完结结果时存在）⇒ <b>不同形</b>、分建； 回退 <c>POST /returnorders</c> 与回退查询 <c>GET /returnorders</c> 应答<b>同形</b>（10 行）⇒ 共用。</para>
/// <para><b>结果查询支的三个 query 全必填</b>：<c>sub_mchid</c> / <c>transaction_id</c> / <c>out_order_no</c>； 回退查询支只 <c>sub_mchid</c> 与 <c>out_return_no</c> 必填 ⇒ 两支定位口径不同，照页不得互套。</para>
/// <para><b>接收方添加与删除共用应答</b>：<c>receivers/add</c> 与 <c>receivers/delete</c> 官方应答表同形（3 行）⇒ 共用 <see cref="BrandProfitSharingReceiverResponse"/>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BrandProfitSharing", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBrandProfitSharingService
{
    /// <summary>
    /// 品牌最大分账比例。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467022"/>（【普通服务商】）。
    /// </summary>
    /// <param name="brandMchid">【品牌商户号】 微信支付分配的商户号，分账出资的品牌商户号（官方 path <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌最大分账比例，见 <see cref="BrandProfitSharingMaxRatioResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/profitsharing/brand-configs/{brand_mchid}</c>；path <c>brand_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMITED 商户发起查询的频率过高 / 403 NO_AUTH 服务商未开通分账功能。</para>
    /// </remarks>
    [Get("/v3/brand/profitsharing/brand-configs/{brandMchid}")]
    Task<BrandProfitSharingMaxRatioResponse> QueryBrandProfitSharingMaxRatioAsync(
        [Path] string brandMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌分账解冻剩余资金。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467016"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌分账解冻剩余资金请求体，字段见 <see cref="BrandProfitSharingFinishRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌分账解冻剩余资金，见 <see cref="BrandProfitSharingFinishResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/profitsharing/finish-order</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandProfitSharingFinishResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 403 NOT_ENOUGH 分账金额为0 / 429 FREQUENCY_LIMITED 对同笔订单分账频率过高。</para>
    /// </remarks>
    [Post("/v3/brand/profitsharing/finish-order")]
    Task<BrandProfitSharingFinishResponse> FinishBrandProfitSharingOrderAsync(
        [Body] BrandProfitSharingFinishRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌分账请求。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692975"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌分账请求请求体，字段见 <see cref="BrandProfitSharingOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌分账请求，见 <see cref="BrandProfitSharingOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/profitsharing/orders</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandProfitSharingOrderResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 403 RULE_LIMIT 分账金额超出最大分账比例 / 403 NOT_ENOUGH 分账金额不足 / 429 FREQUENCY_LIMITED 对同笔订单分账频率过高。</para>
    /// </remarks>
    [Post("/v3/brand/profitsharing/orders")]
    Task<BrandProfitSharingOrderResponse> ApplyBrandProfitSharingOrderAsync(
        [Body] BrandProfitSharingOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌分账结果查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467002"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchid">【特约商户号】 订单收款方商户号，填写微信支付分配的商户号，可以是品牌主商户号，也可以是门店商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="transactionId">【微信订单号】 微信支付订单号（官方 query <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="outOrderNo">【商户分账单号】 请求分账时的商户分账单号（官方 query <c>out_order_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌分账结果查询，见 <see cref="BrandProfitSharingOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/profitsharing/orders</c>；query <c>sub_mchid</c> / <c>transaction_id</c> / <c>out_order_no</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 与 <c>transaction_id</c> 与 <c>out_order_no</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 RESOURCE_NOT_EXISTS 记录不存在 / 429 FREQUENCY_LIMITED 商户发起分账查询的频率过高。</para>
    /// </remarks>
    [Get("/v3/brand/profitsharing/orders")]
    Task<BrandProfitSharingOrderListResponse> QueryBrandProfitSharingOrdersAsync(
        [Query("sub_mchid")] string subMchid,
        [Query("transaction_id")] string transactionId,
        [Query("out_order_no")] string outOrderNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订单剩余待分金额。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467021"/>（【普通服务商】）。
    /// </summary>
    /// <param name="transactionId">【微信订单号】 微信支付订单号（官方 path <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单剩余待分金额，见 <see cref="BrandProfitSharingAmountsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/profitsharing/orders/{transaction_id}/amounts</c>；path <c>transaction_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 RESOURCE_NOT_EXISTS 记录不存在 / 429 FREQUENCY_LIMITED 商户发起查询的频率过高。</para>
    /// </remarks>
    [Get("/v3/brand/profitsharing/orders/{transactionId}/amounts")]
    Task<BrandProfitSharingAmountsResponse> QueryBrandProfitSharingAmountsAsync(
        [Path] string transactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌分账接收方添加。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467100"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌分账接收方添加请求体，字段见 <see cref="BrandProfitSharingReceiverAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌分账接收方添加，见 <see cref="BrandProfitSharingReceiverResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/profitsharing/receivers/add</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandProfitSharingReceiverResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 429 FREQUENCY_LIMITED 添加接收方频率过高。</para>
    /// </remarks>
    [Post("/v3/brand/profitsharing/receivers/add")]
    Task<BrandProfitSharingReceiverResponse> AddBrandProfitSharingReceiverAsync(
        [Body] BrandProfitSharingReceiverAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌分账接收方删除。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467103"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌分账接收方删除请求体，字段见 <see cref="BrandProfitSharingReceiverDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌分账接收方删除，见 <see cref="BrandProfitSharingReceiverResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/profitsharing/receivers/delete</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandProfitSharingReceiverResponse"/>（官方应答表逐字段建模，与 <c>POST /brand/profitsharing/receivers/add</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 429 FREQUENCY_LIMITED 删除接收方频率过高。</para>
    /// </remarks>
    [Post("/v3/brand/profitsharing/receivers/delete")]
    Task<BrandProfitSharingReceiverResponse> DeleteBrandProfitSharingReceiverAsync(
        [Body] BrandProfitSharingReceiverDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌分账回退请求。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467097"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌分账回退请求请求体，字段见 <see cref="BrandProfitSharingReturnOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌分账回退请求，见 <see cref="BrandProfitSharingReturnOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/profitsharing/returnorders</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandProfitSharingReturnOrderResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 回退方未开通分账回退功能 / 403 NOT_ENOUGH 剩余可回退的金额不足 / 429 FREQUENCY_LIMITED 商户发起分账回退的频率过高。</para>
    /// </remarks>
    [Post("/v3/brand/profitsharing/returnorders")]
    Task<BrandProfitSharingReturnOrderResponse> ApplyBrandProfitSharingReturnOrderAsync(
        [Body] BrandProfitSharingReturnOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌分账回退结果查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467011"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchid">【特约商户号】 微信支付分配的特约商户号，分账回退的接收商户，对应原分账出资的分账方商户（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="outReturnNo">【商户回退单号】 调用 请求分账回退 接口时传入的商户回退单号（官方 query <c>out_return_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="orderId">【微信分账单号】 原发起分账请求时，微信返回的微信分账单号，与商户分账单号一一对应。 微信分账单号与商户分账单号二选一填写（官方 query <c>order_id</c>，选填，类型 <c>string(64)</c>）。</param>
    /// <param name="outOrderNo">【商户分账单号】 原发起分账请求时使用的商户系统内部的分账单号。微信分账单号与商户分账单号二选一填写（官方 query <c>out_order_no</c>，选填，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌分账回退结果查询，见 <see cref="BrandProfitSharingReturnOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/profitsharing/returnorders</c>；query <c>sub_mchid</c> / <c>out_return_no</c> / <c>order_id</c> / <c>out_order_no</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 与 <c>out_return_no</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 RESOURCE_NOT_EXISTS 记录不存在 / 429 FREQUENCY_LIMITED 商户发起分账回退查询的频率过高。</para>
    /// </remarks>
    [Get("/v3/brand/profitsharing/returnorders")]
    Task<BrandProfitSharingReturnOrderResponse> QueryBrandProfitSharingReturnOrderAsync(
        [Query("sub_mchid")] string subMchid,
        [Query("out_return_no")] string outReturnNo,
        [Query("order_id")] string? orderId = null,
        [Query("out_order_no")] string? outOrderNo = null,
        CancellationToken cancellationToken = default);
}
