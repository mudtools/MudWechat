// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BrandApplyment;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌入驻申请」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域四支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 RATELIMIT_EXCEEDED</c> /
/// <c>FREQUENCY_LIMITED</c> ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>本域零 query、零 <c>brand_id</c></b>：申请作用域全在包体内（品牌入驻面尚无品牌 ID 可填），两支查询按
/// <c>applyment_id</c> / <c>business_code</c> 分列 ⇒ 分页与品牌作用域口径都无从套用。</para>
/// <para><b>两套申请编号</b>：提交时商户侧自带 <c>business_code</c>，受理成功后微信分配 <c>applyment_id</c> ⇒ 状态查询有「按申请单号」与「按业务申请编号」<b>两支</b>，官方两页应答表<b>逐字段同形</b> ⇒ 共用一个响应类型。</para>
/// <para><b>提交与撤销共用应答</b>：<c>POST /brand/applyments</c> 与 <c>POST /brand/applyments/cancel-applyment</c> 的官方应答表同为 <c>applyment_id</c> / <c>business_code</c>，故共用 <see cref="BrandApplymentAcceptResponse"/>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BrandApplyment", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBrandApplymentService
{
    /// <summary>
    /// 品牌入驻申请提交。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌入驻申请提交请求体，字段见 <see cref="BrandApplymentSubmitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌入驻申请提交，见 <see cref="BrandApplymentAcceptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/applyments</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandApplymentAcceptResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持提交申请，请检查是否重入 / 400 PARAM_ERROR 申请资料校验失败 / 403 NO_AUTH 商户权限异常 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/applyments")]
    Task<BrandApplymentAcceptResponse> SubmitBrandApplymentAsync(
        [Body] BrandApplymentSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌入驻申请撤销。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016257700"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">品牌入驻申请撤销请求体，字段见 <see cref="BrandApplymentCancelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌入驻申请撤销，见 <see cref="BrandApplymentAcceptResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/applyments/cancel-applyment</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandApplymentAcceptResponse"/>（官方应答表逐字段建模，与 <c>POST /brand/applyments</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持撤销，请检查是否重入 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/applyments/cancel-applyment")]
    Task<BrandApplymentAcceptResponse> CancelBrandApplymentAsync(
        [Body] BrandApplymentCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌入驻申请状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016257685"/>（【普通服务商】）。
    /// </summary>
    /// <param name="applymentId">【微信支付申请单号】 1、微信支付分配的申请单号，在调用 提交入驻申请 时返回； 2、数字，不超过 16 个字符。（官方 path <c>applyment_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌入驻申请状态，见 <see cref="BrandApplymentStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/applyments/applyment-id/{applyment_id}</c>；path <c>applyment_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Get("/v3/brand/applyments/applyment-id/{applymentId}")]
    Task<BrandApplymentStateResponse> QueryBrandApplymentByApplymentIdAsync(
        [Path] string applymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌入驻申请状态（按业务申请编号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016257694"/>（【普通服务商】）。
    /// </summary>
    /// <param name="businessCode">【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号； 3、每个编号对应一个申请单； 4、填写调用 提交入驻申请 时传递的业务申请编号。（官方 path <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌入驻申请状态（按业务申请编号），见 <see cref="BrandApplymentStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/applyments/business-code/{business_code}</c>；path <c>business_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Get("/v3/brand/applyments/business-code/{businessCode}")]
    Task<BrandApplymentStateResponse> QueryBrandApplymentByBusinessCodeAsync(
        [Path] string businessCode,
        CancellationToken cancellationToken = default);
}
