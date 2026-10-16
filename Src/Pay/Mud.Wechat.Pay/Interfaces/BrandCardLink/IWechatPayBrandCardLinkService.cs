// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BrandCardLink;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌交易连接名片」域 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域五支官方页<b>全部未印</b>端点级 QPS 数字 ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>本域分页是 <c>page_index</c>/<c>page_size</c>（官方均选填）</b>，与会员卡两支、门店列表的
/// <c>offset</c>/<c>limit</c> <b>不同形</b>；且本域列表支的 <c>brand_id</c> 官方<b>必填</b>，
/// <c>payment_scene</c> 选填 ⇒ 必填口径逐页照抄，不得跨域互套。</para>
/// <para><b><c>unbind-card-link</c> 是「场景解除」不是删除规则</b>：官方语义为关闭该名片在交易连接场景的挂载， 与 <c>cancel-applyment</c>（撤销未生效的申请）分属两支 ⇒ 照页不得合并。</para>
/// <para><b>申请与状态</b>：<c>POST /brand/card/card-links</c> 受理返回 <c>business_code</c> 等 6 行， 状态查询支（按 <c>business_code</c>）返回 8 行 ⇒ 两支应答<b>不同形</b>，各建一型。</para>
/// <para><b>列表支分页是 <c>page_index</c>/<c>page_size</c></b>（本域唯一使用该形态的列表，官方均选填）， 与本域其余列表支的 <c>offset</c>/<c>limit</c> 形态<b>不得互套</b>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BrandCardLink", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBrandCardLinkService
{
    /// <summary>
    /// 交易连接名片规则列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366785"/>（【普通服务商】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌。（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="paymentScene">【交易场景类型】 交易场景类型是指微信支付提供的帮助商家在不同场景中快速收款的工具（如小程序支付、付款码支付等）。 不同类型的交易场景下，建立交易连接名片规则依据的账号类型不同： 1、当交易场景类型填 MINI_PROGRAM、 APP 时， appid 字段必填； 2、当交易场景类型填 PAYMENT_SCORE 时， service_id 字段必填； 3、当交易场景类型填 PAYMENT_CODE 时， card_link_mchid 字段必填； 请选择交易场景类型，不填默认查询所有场景已生效的账号信息。。可选取值：`MINI_PROGRAM` 小程序支付：用户在品牌的小程序中选择商品下单…（官方 query <c>payment_scene</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="pageIndex">【查询页码】 查询页码。从1开始，不传默认返回首页数据。（官方 query <c>page_index</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="pageSize">【查询页大小】 查询页大小。单次拉取的数据条数上限，不填默认为20，最大值50。（官方 query <c>page_size</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>交易连接名片规则列表，见 <see cref="BrandCardLinkListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/card/card-links</c>；query <c>brand_id</c> / <c>payment_scene</c> / <c>page_index</c> / <c>page_size</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Get("/v3/brand/card/card-links")]
    Task<BrandCardLinkListResponse> QueryBrandCardLinksAsync(
        [Query("brand_id")] string brandId,
        [Query("payment_scene")] string? paymentScene = null,
        [Query("page_index")] int? pageIndex = null,
        [Query("page_size")] int? pageSize = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 交易连接名片规则申请。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016333302"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">交易连接名片规则申请请求体，字段见 <see cref="BrandCardLinkApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>交易连接名片规则申请，见 <see cref="BrandCardLinkApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/card/card-links</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandCardLinkApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 连接关系已被绑定或在配置流程中 / 403 NO_AUTH 商户权限异常 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/card/card-links")]
    Task<BrandCardLinkApplyResponse> AddBrandCardLinkApplyAsync(
        [Body] BrandCardLinkApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 交易连接名片规则申请状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366816"/>（【普通服务商】）。
    /// </summary>
    /// <param name="businessCode">【品牌业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的请求流水号，用于标识每一个交易连接名片关联关系配置，需确保同一品牌下不同账号ID配置的流水号唯一； 3、填写调用 配置交易连接名片 时传递的业务申请编号。（官方 path <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="brandId">【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌。（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>交易连接名片规则申请状态，见 <see cref="BrandCardLinkStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/card/card-links/business-code/{business_code}</c>；path <c>business_code</c>，query <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 品牌业务申请编号不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Get("/v3/brand/card/card-links/business-code/{businessCode}")]
    Task<BrandCardLinkStateResponse> QueryBrandCardLinkApplyStateAsync(
        [Path] string businessCode,
        [Query("brand_id")] string brandId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 交易连接名片配置申请撤销。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366797"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">交易连接名片配置申请撤销请求体，字段见 <see cref="BrandCardLinkCancelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>交易连接名片配置申请撤销，见 <see cref="BrandCardLinkCancelResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/card/card-links/cancel-applyment</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandCardLinkCancelResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前配置申请状态不支持撤销 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 品牌业务申请编号不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/card/card-links/cancel-applyment")]
    Task<BrandCardLinkCancelResponse> CancelBrandCardLinkApplyAsync(
        [Body] BrandCardLinkCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 交易连接名片场景解除。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366804"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">交易连接名片场景解除请求体，字段见 <see cref="BrandCardLinkUnbindRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>交易连接名片场景解除，见 <see cref="BrandCardLinkUnbindResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/card/card-links/unbind-card-link</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandCardLinkUnbindResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 交易连接名片配置不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/card/card-links/unbind-card-link")]
    Task<BrandCardLinkUnbindResponse> UnbindBrandCardLinkAsync(
        [Body] BrandCardLinkUnbindRequest request,
        CancellationToken cancellationToken = default);
}
