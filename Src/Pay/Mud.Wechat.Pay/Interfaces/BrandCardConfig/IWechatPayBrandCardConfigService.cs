// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BrandCardConfig;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌商家名片配置」域 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 7 个官方页树<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 RATELIMIT_EXCEEDED</c> /
/// <c>FREQUENCY_LIMITED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>分页口径不统一（照页，不得互套）</b>：本域列表支存在<b>两种</b>整型分页形态
/// —— <c>offset</c>/<c>limit</c>（会员卡模板列表、用户会员卡列表、品牌门店列表）与
/// <c>page_index</c>/<c>page_size</c>（交易连接名片规则列表）；且同为 <c>offset</c>/<c>limit</c>，
/// 会员卡两支官方<b>必填</b>、门店列表支官方<b>选填</b> ⇒ 必填口径也须照页。应答总数字段官方为选填。</para>
/// <para><b><c>brand_id</c> 是本域的「商户作用域参数」</b>：除品牌入驻面（<c>/brand/applyments/*</c>）与免密契约面外，
/// 本域其余每支都把 <c>brand_id</c> 印为<b>必填 query</b> ⇒ 它不是可选过滤条件，缺省即 400；
/// 长度上限官方<b>不统一</b>（名片配置/连接/门店面为 <c>string(20)</c>，会员卡面为 <c>string(32)</c>），照页不做本地归一。</para>
/// <para><b>「申请 → 发布」两段式</b>：<c>POST /brand/card/card-configs</c> 只是<b>提交配置申请</b>（审核态）， 生效须再调 <c>POST /brand/card/card-configs/publish</c>（其请求体另有 <c>publish_type</c> 与条件字段 <c>scheduled_publish_time</c>）；撤销走 <c>.../cancel-applyment</c> ⇒ 三者语义不同，不得合并。</para>
/// <para><b>定位键是「二选一」而非「必填」</b>：状态查询支与预览支都用 <c>business_code</c>（<c>string(128)</c>） 与 <c>applyment_id</c>（<c>string(25)</c>）定位，官方把两者<b>都印为选填</b>并写明「二选一作为请求参数」 ⇒ 声明式签名给可空缺省，至少提供一个由宿主与官方共同保证，SDK 不做本地校验。</para>
/// <para><b>预览支与发布支应答同为 5 行但字段名不同</b>（预览回 <c>card_preview_url</c> / <c>url_expired_time</c>， 发布回 <c>publish_type</c> / <c>scheduled_publish_time</c>）⇒ 各建一型，不得因行数相同而共用。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BrandCardConfig", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBrandCardConfigService
{
    /// <summary>
    /// 商家名片配置申请状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475174"/>（【普通服务商】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符。（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="businessCode">【业务申请编号】 1、只能由数字、字母或下划线组成，和微信支付申请单号二选一作为请求参数； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单； 3、填写调用 提交商家名片配置申请 时传递的业务申请编号。（官方 query <c>business_code</c>，选填，类型 <c>string(128)</c>）。</param>
    /// <param name="applymentId">【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成，和业务申请编号二选一作为请求参数。 2、填写调用 提交商家名片配置申请 时返回的微信支付申请单号。（官方 query <c>applyment_id</c>，选填，类型 <c>string(25)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家名片配置申请状态，见 <see cref="BrandCardConfigQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/card/card-configs</c>；query <c>brand_id</c> / <c>business_code</c> / <c>applyment_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Get("/v3/brand/card/card-configs")]
    Task<BrandCardConfigQueryResponse> QueryBrandCardConfigAsync(
        [Query("brand_id")] string brandId,
        [Query("business_code")] string? businessCode = null,
        [Query("applyment_id")] string? applymentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家名片配置申请提交。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016468440"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">商家名片配置申请提交请求体，字段见 <see cref="BrandCardConfigApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家名片配置申请提交，见 <see cref="BrandCardConfigApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/card/card-configs</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandCardConfigApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 存在在途商家名片配置申请单 / 403 NO_AUTH 商户权限异常 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/card/card-configs")]
    Task<BrandCardConfigApplyResponse> SubmitBrandCardConfigApplyAsync(
        [Body] BrandCardConfigApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家名片配置申请撤销。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475172"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">商家名片配置申请撤销请求体，字段见 <see cref="BrandCardConfigCancelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家名片配置申请撤销，见 <see cref="BrandCardConfigCancelResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/card/card-configs/cancel-applyment</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandCardConfigCancelResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持撤销，请检查是否重入 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/card/card-configs/cancel-applyment")]
    Task<BrandCardConfigCancelResponse> CancelBrandCardConfigApplyAsync(
        [Body] BrandCardConfigCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家名片预览二维码。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016641998"/>（【普通服务商】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符。（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="businessCode">【业务申请编号】 1、只能由数字、字母或下划线组成，和微信支付申请单号二选一作为请求参数； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单； 3、填写调用 提交商家名片配置申请 时传递的业务申请编号。（官方 query <c>business_code</c>，选填，类型 <c>string(128)</c>）。</param>
    /// <param name="applymentId">【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成，和业务申请编号二选一作为请求参数。 2、填写调用 提交商家名片配置申请 时返回的微信支付申请单号。（官方 query <c>applyment_id</c>，选填，类型 <c>string(25)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家名片预览二维码，见 <see cref="BrandCardConfigPreviewResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/card/card-configs/preview-url</c>；query <c>brand_id</c> / <c>business_code</c> / <c>applyment_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持获取预览链接 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Get("/v3/brand/card/card-configs/preview-url")]
    Task<BrandCardConfigPreviewResponse> QueryBrandCardConfigPreviewUrlAsync(
        [Query("brand_id")] string brandId,
        [Query("business_code")] string? businessCode = null,
        [Query("applyment_id")] string? applymentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家名片配置发布。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016475176"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">商家名片配置发布请求体，字段见 <see cref="BrandCardConfigPublishRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家名片配置发布，见 <see cref="BrandCardConfigPublishResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/card/card-configs/publish</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandCardConfigPublishResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持提交发布申请，请检查是否重入 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 业务申请编号或微信支付申请单号不存在 / 429 RATELIMIT_EXCEEDED 频率限制。</para>
    /// </remarks>
    [Post("/v3/brand/card/card-configs/publish")]
    Task<BrandCardConfigPublishResponse> PublishBrandCardConfigAsync(
        [Body] BrandCardConfigPublishRequest request,
        CancellationToken cancellationToken = default);
}
