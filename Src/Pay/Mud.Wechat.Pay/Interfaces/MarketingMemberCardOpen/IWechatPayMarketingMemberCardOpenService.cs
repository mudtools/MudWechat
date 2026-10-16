// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingMemberCardOpen;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「旧版会员卡（membercard-open）」域 SDK（25 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径；本域 25 支官方<b>全部只印一页</b>（merchant 中心【普通商户】）⇒ 一支一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>两套并存契约</b>：品牌会员卡 <c>/v3/brand/partner/card-member/*</c>（已落位 15 端点）与本域是<b>并存非替代</b>关系，
/// 不得以「已有替代品」为由并回零声明名册（守卫 MCO-B6 锁定）。</para>
/// <para><b>动词不统一</b>：修改模板走 <b>PATCH</b>，删除模板走 <b>DELETE</b>，激活走 <b>POST</b> ⇒ 照页不得合并。</para>
/// <para><b>四支 204 无应答包体</b>：删除模板 / 升级模板 / 作废卡code / 激活卡code 返回裸 <see cref="Task"/>，不建响应类型。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingMemberCardOpen", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingMemberCardOpenService
{
    /// <summary>
    /// 创建会员卡模板。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545584"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">创建会员卡模板请求体，字段见 <see cref="MemberCardTemplateCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建会员卡模板，见 <see cref="MemberCardTemplateCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards</c>；body 38 键（含嵌套 brand / date_information / balance_information / user_information_form / additional_statement / favor…），resp 55 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards")]
    Task<MemberCardTemplateCreateResponse> AddMemberCardTemplateAsync(
        [Body] MemberCardTemplateCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询会员卡模板列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547108"/>（【普通商户】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取）（官方 query <c>brand_id</c>，选填，类型 <c>string(16)</c>）。</param>
    /// <param name="appid">【公众号APPID】 公众号APPID，用于筛选该公众号下创建的会员卡（官方 query <c>appid</c>，选填，类型 <c>string(18)</c>）。</param>
    /// <param name="offset">【分页起始位置】 分页起始位置，从 0 开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，最大50（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询会员卡模板列表，见 <see cref="MemberCardTemplateListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-open/cards</c>；query <c>brand_id</c>/<c>appid</c>/<c>offset</c>/<c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-open/cards")]
    Task<MemberCardTemplateListResponse> QueryMemberCardTemplatesAsync(
        [Query("brand_id")] string? brandId = null,
        [Query("appid")] string? appid = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询会员卡模板信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547650"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询会员卡模板信息，见 <see cref="MemberCardTemplateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-open/cards/{card_id}</c>；path <c>card_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-open/cards/{cardId}")]
    Task<MemberCardTemplateResponse> QueryMemberCardTemplateAsync(
        [Path] string cardId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改会员卡模板信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549567"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">修改会员卡模板信息请求体，字段见 <see cref="MemberCardTemplateUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/membercard-open/cards/{card_id}</c>；path <c>card_id</c>，body 34 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Patch("/v3/marketing/membercard-open/cards/{cardId}")]
    Task UpdateMemberCardTemplateAsync(
        [Path] string cardId,
        [Body] MemberCardTemplateUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除会员卡模板。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549682"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>DELETE</b> <c>/v3/marketing/membercard-open/cards/{card_id}</c>；path <c>card_id</c>；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 <see cref="Task"/>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Delete("/v3/marketing/membercard-open/cards/{cardId}")]
    Task DeleteMemberCardTemplateAsync(
        [Path] string cardId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 老会员卡模板升级。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545673"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">老会员卡模板升级请求体，字段见 <see cref="MemberCardTemplateUpgradeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/upgrade</c>；path <c>card_id</c>，body 2 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 <see cref="Task"/>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/upgrade")]
    Task UpgradeMemberCardTemplateAsync(
        [Path] string cardId,
        [Body] MemberCardTemplateUpgradeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会员卡卡code预存。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012545728"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">会员卡卡code预存请求体，字段见 <see cref="MemberCardCodeDepositRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员卡卡code预存，见 <see cref="MemberCardCodeDepositResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/deposit</c>；path <c>card_id</c>，body 1 键（<c>code_list</c> 数组），resp 3 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/codes/deposit")]
    Task<MemberCardCodeDepositResponse> DepositMemberCardCodesAsync(
        [Path] string cardId,
        [Body] MemberCardCodeDepositRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户会员卡信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012547768"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询用户会员卡信息，见 <see cref="MemberCardUserCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}</c>；path <c>card_id</c>/<c>code</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}")]
    Task<MemberCardUserCardResponse> QueryMemberCardUserCardAsync(
        [Path] string cardId,
        [Path] string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改用户会员卡信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549292"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">修改用户会员卡信息请求体，字段见 <see cref="MemberCardUserCardUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}</c>；path <c>card_id</c>/<c>code</c>，body 11 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Patch("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}")]
    Task UpdateMemberCardUserCardAsync(
        [Path] string cardId,
        [Path] string code,
        [Body] MemberCardUserCardUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 作废用户会员卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549677"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">作废用户会员卡请求体，字段见 <see cref="MemberCardCodeUnavailableRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/unavailable</c>；path <c>card_id</c>/<c>code</c>，body 1 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 <see cref="Task"/>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/unavailable")]
    Task InvalidateMemberCardCodeAsync(
        [Path] string cardId,
        [Path] string code,
        [Body] MemberCardCodeUnavailableRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 确认用户入会。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4024293873"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">确认用户入会请求体，字段见 <see cref="MemberCardCodeConfirmRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>确认用户入会，见 <see cref="MemberCardCodeConfirmResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/confirm</c>；path <c>card_id</c>/<c>code</c>，body 2 键，resp 2 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/confirm")]
    Task<MemberCardCodeConfirmResponse> ConfirmMemberCardCodeAsync(
        [Path] string cardId,
        [Path] string code,
        [Body] MemberCardCodeConfirmRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据OpenID导入用户会员卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549739"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">根据OpenID导入用户会员卡请求体，字段见 <see cref="MemberCardUserImportRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>根据OpenID导入用户会员卡，见 <see cref="MemberCardUserImportResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/users/import</c>；path <c>card_id</c>，body 3 键，resp 1 键（<c>code</c>）。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/users/import")]
    Task<MemberCardUserImportResponse> ImportMemberCardUsersAsync(
        [Path] string cardId,
        [Body] MemberCardUserImportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据手机号导入用户会员卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550535"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">根据手机号导入用户会员卡请求体，字段见 <see cref="MemberCardPhoneImportRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>根据手机号导入用户会员卡，见 <see cref="MemberCardPhoneImportResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/phone-membercard/import</c>；path <c>card_id</c>，body 3 键，resp 1 键（<c>code</c>）。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/phone-membercard/import")]
    Task<MemberCardPhoneImportResponse> ImportMemberCardPhoneUsersAsync(
        [Path] string cardId,
        [Body] MemberCardPhoneImportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建会员卡二维码。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550605"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">创建会员卡二维码请求体，字段见 <see cref="MemberCardQrcodeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建会员卡二维码，见 <see cref="MemberCardQrcodeResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/qrcode</c>；path <c>card_id</c>，body 5 键，resp 1 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/qrcode")]
    Task<MemberCardQrcodeResponse> CreateMemberCardQrcodeAsync(
        [Path] string cardId,
        [Body] MemberCardQrcodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解密会员卡code。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550742"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="encryptCode">【加密后的code】 加密后的会员卡code（官方 query <c>encrypt_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>解密会员卡code，见 <see cref="MemberCardDecryptCodeResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-open/cards/{card_id}/decrypt-code</c>；path <c>card_id</c>，query <c>encrypt_code</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-open/cards/{cardId}/decrypt-code")]
    Task<MemberCardDecryptCodeResponse> DecryptMemberCardCodeAsync(
        [Path] string cardId,
        [Query("encrypt_code")] string encryptCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询会员卡激活信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550779"/>（【普通商户】）。
    /// </summary>
    /// <param name="activateTicket">【激活票据】 用户在开卡组件中完成信息填写后生成的激活票据（官方 path <c>activate_ticket</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询会员卡激活信息，见 <see cref="MemberCardActivateInformationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-open/activate-informations/{activate_ticket}</c>；path <c>activate_ticket</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-open/activate-informations/{activateTicket}")]
    Task<MemberCardActivateInformationResponse> QueryMemberCardActivateInformationAsync(
        [Path] string activateTicket,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 激活会员卡（跳转型激活）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550809"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">激活会员卡请求体，字段见 <see cref="MemberCardActivateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/activate</c>；path <c>card_id</c>/<c>code</c>，body 1 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 <see cref="Task"/>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/activate")]
    Task ActivateMemberCardCodeAsync(
        [Path] string cardId,
        [Path] string code,
        [Body] MemberCardActivateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户授权令牌。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550867"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">获取用户授权令牌请求体，字段见 <see cref="MemberCardPermissionTokenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取用户授权令牌，见 <see cref="MemberCardPermissionTokenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/permission-tokens</c>；path <c>card_id</c>，body 6 键，resp 1 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/permission-tokens")]
    Task<MemberCardPermissionTokenResponse> CreateMemberCardPermissionTokenAsync(
        [Path] string cardId,
        [Body] MemberCardPermissionTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置用户会员卡权益。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551213"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">设置用户会员卡权益请求体，字段见 <see cref="MemberCardUserCodeRightsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/rights</c>；path <c>card_id</c>/<c>code</c>，body 13 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Patch("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/rights")]
    Task UpdateMemberCardUserCodeRightsAsync(
        [Path] string cardId,
        [Path] string code,
        [Body] MemberCardUserCodeRightsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置会员卡模板权益项。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551219"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">设置会员卡模板权益项请求体，字段见 <see cref="MemberCardTemplateRightsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/membercard-open/cards/{card_id}/rights</c>；path <c>card_id</c>，body 31 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Patch("/v3/marketing/membercard-open/cards/{cardId}/rights")]
    Task UpdateMemberCardTemplateRightsAsync(
        [Path] string cardId,
        [Body] MemberCardTemplateRightsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交会员卡积分。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551350"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">提交会员卡积分请求体，字段见 <see cref="MemberCardBonusCommitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/bonus/commit</c>；path <c>card_id</c>/<c>code</c>，body 3 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 <see cref="Task"/>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/commit")]
    Task CommitMemberCardBonusAsync(
        [Path] string cardId,
        [Path] string code,
        [Body] MemberCardBonusCommitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预下单会员卡积分。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012595808"/>（【普通商户】）。
    /// </summary>
    /// <param name="cardId">【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="code">【会员卡code】 会员在card_id下的唯一标识（官方 path <c>code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">预下单会员卡积分请求体，字段见 <see cref="MemberCardBonusPrepareRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/bonus/prepare</c>；path <c>card_id</c>/<c>code</c>，body 3 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 <see cref="Task"/>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Post("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/prepare")]
    Task PrepareMemberCardBonusAsync(
        [Path] string cardId,
        [Path] string code,
        [Body] MemberCardBonusPrepareRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置会员卡事件通知URL。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012552251"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">设置会员卡事件通知URL请求体，字段见 <see cref="MemberCardCallbackRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>设置会员卡事件通知URL，见 <see cref="MemberCardCallbackResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/membercard-open/callback</c>；body 1 键（<c>notify_url</c>），resp 2 键。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Patch("/v3/marketing/membercard-open/callback")]
    Task<MemberCardCallbackResponse> UpdateMemberCardCallbackAsync(
        [Body] MemberCardCallbackRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询会员卡事件通知URL。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012552275"/>（【普通商户】）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询会员卡事件通知URL，见 <see cref="MemberCardCallbackResponse"/>（复用 PATCH 回调应答类型）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-open/callback</c>；resp 2 键（<c>notify_url</c>/<c>mchid</c>）。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-open/callback")]
    Task<MemberCardCallbackResponse> QueryMemberCardCallbackAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户在品牌下所有会员卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549255"/>（【普通商户】）。
    /// </summary>
    /// <param name="openid">【用户openid】 用户在公众号下的openid（官方 query <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="appid">【公众号APPID】 公众号APPID，用于筛选该公众号下领取的会员卡（官方 query <c>appid</c>，选填，类型 <c>string(18)</c>）。</param>
    /// <param name="offset">【分页起始位置】 分页起始位置，从 0 开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，最大50（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询用户在品牌下所有会员卡，见 <see cref="MemberCardBrandUserCardsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/membercard-open/user/cards</c>；query <c>openid</c>/<c>appid</c>/<c>offset</c>/<c>limit</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。</para>
    /// </remarks>
    [Get("/v3/marketing/membercard-open/user/cards")]
    Task<MemberCardBrandUserCardsResponse> QueryMemberCardsByUserAsync(
        [Query("openid")] string openid,
        [Query("appid")] string? appid = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        CancellationToken cancellationToken = default);
}
