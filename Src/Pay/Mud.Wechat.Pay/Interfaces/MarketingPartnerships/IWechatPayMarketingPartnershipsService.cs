// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingPartnerships;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商户合作关系」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对<b>完全同形</b>
/// ⇒ <b>一面一方法 + 双页文档引用</b>，只有字段表存在差异的端点才按「同路由多方法」分建。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>分页口径不统一</b>：本域既有 <c>offset</c>/<c>limit</c>（整型页码）也有 <c>page_size</c> + <c>offset</c> 形态，
/// 一律<b>照页</b>，不得互相套用；应答的 <c>total_count</c> 官方多为选填（<c>offset=0</c> 或结果为空时才回）。</para>
/// <para><b>PATCH 与 PUT</b>：批次/活动的改预算与改状态走 <b>PATCH</b>，终止走 <b>POST .../terminate</b>
/// ⇒ 「写操作动词」在本域不统一，照页不得合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingPartnerships", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingPartnershipsService
{
    /// <summary>
    /// 建立品牌委托营销合作关系。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380498"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012381469"/>（普通服务商）。
    /// </summary>
    /// <param name="request">建立品牌委托营销合作关系请求体，字段见 <see cref="PartnershipPartnershipBuildRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>建立品牌委托营销合作关系，见 <see cref="PartnershipPartnershipBuildResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partnerships/build</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PartnershipPartnershipBuildResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH appid与mchid不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/partnerships/build")]
    Task<PartnershipPartnershipBuildResponse> BuildPartnershipAsync(
        [Body] PartnershipPartnershipBuildRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询品牌委托营销合作关系列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012380536"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012381479"/>（普通服务商）。
    /// </summary>
    /// <param name="authorizedData">【被授权数据】 被授权数据（官方 query <c>authorized_data</c>，<b>必填</b>，类型 <c>object</c>，官方线格式为 URL 编码 JSON ⇒ 以 <c>string</c> 承载）。</param>
    /// <param name="limit">【分页大小】 分页大小，最大50。 不传默认为20（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="offset">【分页页码】 分页页码，页码从0开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="partner">【合作方信息】 合作方信息（官方 query <c>partner</c>，选填，类型 <c>object</c>，官方线格式为 URL 编码 JSON ⇒ 以 <c>string</c> 承载）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询品牌委托营销合作关系列表，见 <see cref="PartnershipPartnershipListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partnerships</c>；query <c>authorized_data</c> / <c>limit</c> / <c>offset</c> / <c>partner</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>authorized_data</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH appid与mchid不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST 委托营销合作关系不存在 / 400 MCH_NOT_EXISTS 商户号不存在 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/partnerships")]
    Task<PartnershipPartnershipListResponse> QueryPartnershipsAsync(
        [Query("authorized_data")] string authorizedData,
        [Query("limit")] int? limit = null,
        [Query("offset")] int? offset = null,
        [Query("partner")] string? partner = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止合作关系（直连商户）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017164460"/>（【直连商户】）。
    /// </summary>
    /// <param name="request">终止合作关系请求体，字段见 <see cref="PartnershipTerminateRequest"/>。</param>
    /// <param name="idempotencyKey">【业务请求幂等值】 请求包头 Idempotency-Key：传递；商户侧需保持唯一性，可包含英文字母、数字、｜、_、*、- 等内容（官方 header <c>Idempotency-Key</c>，<b>必填</b>，类型 <c>string[1,48]</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>终止合作关系，见 <see cref="PartnershipTerminateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partnerships/terminate</c>；body 7 键（含嵌套 <c>partner</c>/<c>authorized_data</c>），resp 1 键（<c>terminate_time</c>）。</para>
    /// <para><b>支持商户</b>【直连商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Idempotency-Key</c></b>：官方请求头表把该头列为<b>必填</b>，商户侧需保持唯一性。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/partnerships/terminate")]
    Task<PartnershipTerminateResponse> TerminatePartnershipAsync(
        [Body] PartnershipTerminateRequest request,
        [Header("Idempotency-Key")] string idempotencyKey,
        CancellationToken cancellationToken = default);
}
