// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingPayGiftActivity;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「支付有礼活动」域 SDK（8 端点）。
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
[HttpClientApi(RegistryGroupName = "MarketingPayGiftActivity", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingPayGiftActivityService
{
    /// <summary>
    /// 创建支付有礼活动。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487898"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012492900"/>（普通服务商）。
    /// </summary>
    /// <param name="request">创建支付有礼活动请求体，字段见 <see cref="PayGiftActivityCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建支付有礼活动，见 <see cref="PayGiftActivityCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/paygiftactivity/unique-threshold-activity</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PayGiftActivityCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误 / 400 APPID_MCHID_NOT_MATCH AppID与mchid不匹配。</para>
    /// </remarks>
    [Post("/v3/marketing/paygiftactivity/unique-threshold-activity")]
    Task<PayGiftActivityCreateResponse> CreatePayGiftActivityAsync(
        [Body] PayGiftActivityCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支付有礼活动列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489126"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012493214"/>（普通服务商）。
    /// </summary>
    /// <param name="offset">【分页页码】 分页页码（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="activityName">【活动名称】 活动名称，支持模糊搜索（官方 query <c>activity_name</c>，选填，类型 <c>string(10)</c>）。</param>
    /// <param name="activityStatus">【活动状态】 活动状态。可选取值：`ACT_STATUS_UNKNOWN` 状态未知 / `CREATE_ACT_STATUS` 已创建 / `ONGOING_ACT_STATUS` 运行中 / `TERMINATE_ACT_STATUS` 已终止 / `STOP_ACT_STATUS` 已暂停 / `OVER_TIME_ACT_STATUS` 已过期 / `CREATE_ACT_FAILED` 创建活动失败（官方 query <c>activity_status</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="awardType">【奖品类型】 奖品类型，暂时只支持商家券。可选取值：`BUSIFAVOR` 商家券（官方 query <c>award_type</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询支付有礼活动列表，见 <see cref="PayGiftActivityListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/paygiftactivity/activities</c>；query <c>offset</c> / <c>limit</c> / <c>activity_name</c> / <c>activity_status</c> / <c>award_type</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>offset</c> 与 <c>limit</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/paygiftactivity/activities")]
    Task<PayGiftActivityListResponse> QueryPayGiftActivitiesAsync(
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("activity_name")] string? activityName = null,
        [Query("activity_status")] string? activityStatus = null,
        [Query("award_type")] string? awardType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支付有礼活动详情。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012487971"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012492967"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【活动Id】 活动Id（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询支付有礼活动详情，见 <see cref="PayGiftActivityQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/paygiftactivity/activities/{activity_id}</c>；path <c>activity_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/paygiftactivity/activities/{activityId}")]
    Task<PayGiftActivityQueryResponse> QueryPayGiftActivityAsync(
        [Path] string activityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止支付有礼活动。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466523"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466633"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【活动Id】 活动Id（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>终止支付有礼活动，见 <see cref="PayGiftActivityTerminateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/paygiftactivity/activities/{activity_id}/terminate</c>；path <c>activity_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Post("/v3/marketing/paygiftactivity/activities/{activityId}/terminate")]
    Task<PayGiftActivityTerminateResponse> TerminatePayGiftActivityAsync(
        [Path] string activityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询活动商品。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466448"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466492"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【活动Id】 活动Id（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="offset">【分页页码】 分页页码（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询活动商品，见 <see cref="PayGiftActivityGoodsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/paygiftactivity/activities/{activity_id}/goods</c>；path <c>activity_id</c>，query <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/paygiftactivity/activities/{activityId}/goods")]
    Task<PayGiftActivityGoodsResponse> QueryPayGiftActivityGoodsAsync(
        [Path] string activityId,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询活动商户列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466149"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466191"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【活动Id】 活动Id（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="offset">【分页页码】 分页页码（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 限制分页最大数据条目（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询活动商户列表，见 <see cref="PayGiftActivityMerchantsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/paygiftactivity/activities/{activity_id}/merchants</c>；path <c>activity_id</c>，query <c>offset</c> / <c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/paygiftactivity/activities/{activityId}/merchants")]
    Task<PayGiftActivityMerchantsResponse> QueryPayGiftActivityMerchantsAsync(
        [Path] string activityId,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加活动商户。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466671"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466735"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【活动Id】 活动Id（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="request">添加活动商户请求体，字段见 <see cref="PayGiftMerchantAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>添加活动商户，见 <see cref="PayGiftMerchantAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/paygiftactivity/activities/{activity_id}/merchants/add</c>；path <c>activity_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PayGiftMerchantAddResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST 查询活动详情结果为空，请确认活动ID是否正确 / 400 INVALID_REQUEST 发券商户号校验失败，请核实是否满足同品牌等规则 / 400 INVALID_REQUEST 发券商户号非奖品的同品牌商户号 / 400 INVALID_REQUEST 券归属商户或发券商户非同品牌 / 400 INVALID_REQUEST 创建方商户与发券商户非同品牌 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 PARAM_ERROR 创建活动参数错误，请联系技术支持 / 400 PARAM_ERROR 活动曝光商户的同品牌商户中需要至少存在一个与创建方商户具备父子关系 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 403 RULE_LIMIT 上一次修改还未完成，请稍后重试 / 403 RULE_LIMIT 活动不在运行中，不允许再进行修改 / 403 RULE_LIMIT 所有的曝光商户号均校验失败，请再次检查 / 403 RULE_LIMIT 并发修改，请稍后再试 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Post("/v3/marketing/paygiftactivity/activities/{activityId}/merchants/add")]
    Task<PayGiftMerchantAddResponse> AddPayGiftActivityMerchantsAsync(
        [Path] string activityId,
        [Body] PayGiftMerchantAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除活动商户。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466787"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466827"/>（普通服务商）。
    /// </summary>
    /// <param name="activityId">【活动Id】 活动Id（官方 path <c>activity_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</param>
    /// <param name="request">删除活动商户请求体，字段见 <see cref="PayGiftMerchantDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>删除活动商户，见 <see cref="PayGiftMerchantDeleteResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/paygiftactivity/activities/{activity_id}/merchants/delete</c>；path <c>activity_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="PayGiftMerchantDeleteResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST 查询活动详情结果为空，请确认活动ID是否正确 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 PARAM_ERROR 创建活动参数错误，请联系技术支持 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 403 RULE_LIMIT 上一次修改还未完成，请稍后重试 / 403 RULE_LIMIT 活动不在运行中，不允许再进行修改 / 403 RULE_LIMIT 并发修改，请稍后再试 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Post("/v3/marketing/paygiftactivity/activities/{activityId}/merchants/delete")]
    Task<PayGiftMerchantDeleteResponse> DeletePayGiftActivityMerchantsAsync(
        [Path] string activityId,
        [Body] PayGiftMerchantDeleteRequest request,
        CancellationToken cancellationToken = default);
}
