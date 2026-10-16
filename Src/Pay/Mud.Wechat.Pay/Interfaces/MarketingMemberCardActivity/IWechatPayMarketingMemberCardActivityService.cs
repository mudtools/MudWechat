// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingMemberCardActivity;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「会员有礼活动（membercard-activity）」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径；本域 4 支官方<b>全部只印一页</b>（merchant 中心【普通商户】）⇒ 一支一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>终止活动走 POST .../terminate</b>：写操作动词在本域不统一，照页不得合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingMemberCardActivity", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingMemberCardActivityService
{
    /// <summary>
    /// 创建会员有礼活动。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553030"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">创建会员有礼活动请求体，字段见 <see cref="MemberCardActivityCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建会员有礼活动，见 <see cref="MemberCardActivityCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-activity/activities</c>；body 43 键，resp 47 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-activity/activities")]
    Task<MemberCardActivityCreateResponse> AddMemberCardActivityAsync(
        [Body] MemberCardActivityCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询会员有礼活动列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553261"/>（【普通商户】）。
    /// </summary>
    /// <param name="limit">【分页大小】 分页大小，最大50（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="offset">【分页起始位置】 分页起始位置，从 0 开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="activityType">【活动类型】 活动类型，枚举值见文档（官方 query <c>activity_type</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cardId">【会员卡ID】 会员卡ID，用于筛选该会员卡下的活动（官方 query <c>card_id</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="activityStatus">【活动状态】 活动状态，枚举值见文档（官方 query <c>activity_status</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询会员有礼活动列表，见 <see cref="MemberCardActivityListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-activity/activities</c>；query <c>limit</c>/<c>offset</c>/<c>activity_type</c>/<c>card_id</c>/<c>activity_status</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-activity/activities")]
    Task<MemberCardActivityListResponse> QueryMemberCardActivitiesAsync(
        [Query("limit")] int? limit = null,
        [Query("offset")] int? offset = null,
        [Query("activity_type")] string? activityType = null,
        [Query("card_id")] string? cardId = null,
        [Query("activity_status")] string? activityStatus = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询会员有礼活动详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553365"/>（【普通商户】）。
    /// </summary>
    /// <param name="activityId">【活动ID】 创建活动后获得的活动ID（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询会员有礼活动详情，见 <see cref="MemberCardActivityDetailResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-activity/activities/{activity_id}</c>；path <c>activity_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-activity/activities/{activityId}")]
    Task<MemberCardActivityDetailResponse> QueryMemberCardActivityAsync(
        [Path] string activityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止会员有礼活动。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553381"/>（【普通商户】）。
    /// </summary>
    /// <param name="activityId">【活动ID】 创建活动后获得的活动ID（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>终止会员有礼活动，见 <see cref="MemberCardActivityTerminateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-activity/activities/{activity_id}/terminate</c>；path <c>activity_id</c>，resp 2 键（<c>activity_id</c>/<c>terminate_time</c>）。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-activity/activities/{activityId}/terminate")]
    Task<MemberCardActivityTerminateResponse> TerminateMemberCardActivityAsync(
        [Path] string activityId,
        CancellationToken cancellationToken = default);
}
