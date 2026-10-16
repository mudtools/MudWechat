// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.SmartGuide;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「智慧导购（服务人员注册与分配）」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>0 支官方只印一页</b>、<b>4 支印多页</b>（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 4 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 4 支里<b>3 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>四支各印商户页 + 服务商页 ⇒ 并集建模</b>：服务商页统一多一个<b>选填</b> <c>sub_mchid</c>（<c>string(64)</c>）， 其余字段与商户页同形 ⇒ 一支一方法，身份差异写在参数注释内，不拆方法也不复制 DTO。</para>
/// <para><b>注册与查询共用同一 URL、靠动词区分</b>：官方注意原文「服务人员注册API和查询API请求URL相同，区别主要是在body和query， 请注意区分」（<c>GET /v3/smartguide/guides</c> 查询、<c>POST /v3/smartguide/guides</c> 注册）⇒ 本域两支路由字符串相同， 守卫按「动词 + 路由」计数，<b>不得</b>合并成一支或改动路由写法。</para>
/// <para><b>涉密面是逐页结论</b>：查询、注册、更新三支的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b> （<c>mobile</c> / <c>qr_code</c> / <c>avatar</c> 等按 APIv3 加密上送），而<b>分配支未列</b>该头 ⇒ 本接口内同时存在「须带序列号」与「不带」两类，密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>，声明式签名不承载该头。</para>
/// <para><b>两支是 204 无应答包体</b>：<c>PATCH .../guides/{guide_id}</c>（更新）与 <c>POST .../guides/{guide_id}/assign</c>（分配） ⇒ 返回裸 <see cref="Task"/>；查询与注册各建应答（列表包装 <c>data[]</c> + <c>total_count</c> / <c>limit</c> / <c>offset</c>， 注册应答单键 <c>guide_id</c>）。</para>
/// <para><b>两套身份键互斥、单次查询上限 10 条</b>：官方错误码原文「个人微信商家不应填写企业微信ID」「企业微信商家不应填写工号」 ⇒ <c>userid</c>（企微员工 ID）与 <c>work_id</c>（工号）属两种商家形态，SDK 两者皆可空承载、由宿主择一； <c>store_id</c> 是<b>必填 query</b>，且「最大资源条数大于10」即 <c>limit</c> 官方上限 10。</para>
/// <para><b>分配须在支付完成之前</b>：官方原文「调用服务人员分配接口需在完成支付之前，若分配服务人员晚于支付完成， 则将<b>无法在支付凭证上出现服务人员名片入口</b>」⇒ <c>out_trade_no</c> 指向的订单必须尚未支付； 同域码「服务人员已注册」属注册支、「所有可更新的参数都没有填」「个人微信商家不能改二维码」属更新支，<b>不得</b>互挂。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "SmartGuide", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPaySmartGuideService
{
    /// <summary>
    /// 服务人员查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535123"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688558"/>（【普通服务商】）。
    /// </summary>
    /// <param name="storeId">【门店ID】 门店在微信支付商户平台的唯一标识（官方 query <c>store_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="userid">【企业微信的员工ID】 员工在商户企业微信通讯录使用的唯一标识，企业微信商家可传入（官方 query <c>userid</c>，选填，类型 <c>string(64)</c>）。</param>
    /// <param name="mobile">【手机号码】服务人员通过小程序注册时填写的手机号码，该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方 query <c>mobile</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="workId">【工号】 服务人员通过小程序注册时填写的工号，个人微信商家可传入（官方 query <c>work_id</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="limit">【最大资源条数】 该次请求可返回的最大资源条数，不大于10（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="offset">【请求资源起始位置】 该次请求资源的起始位置，默认值为0（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="subMchid">【子商户ID】 导购所属商户的商户ID（官方 query <c>sub_mchid</c>，选填，类型 <c>string(64)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务人员查询，见 <see cref="SmartGuideListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/smartguide/guides</c>；query <c>store_id</c> / <c>userid</c> / <c>mobile</c> / <c>work_id</c> / <c>limit</c> / <c>offset</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>store_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：子商户未授权 / 400 INVALID_REQUEST：子商户产品权限未开通 / 400 INVALID_REQUEST：子商户产品配置未完成 / 400 MCH_NOT_EXISTS：子商户不存在 / 400 MCH_NOT_EXISTS：商户号格式错误 / 400 MCH_NOT_EXISTS：子商户号格式错误 / 400 MCH_NOT_EXISTS：子商户号为空 / 400 PARAM_ERROR：必填参数为空 / 400 PARAM_ERROR：门店ID格式错误 / 400 PARAM_ERROR：userid格式错误 / 400 PARAM_ERROR：手机号码格式错误 / 400 PARAM_ERROR：门店ID为空 / 400 PARAM_ERROR：个人微信商家不应填写企业微信ID / 400 PARAM_ERROR：企业微信商家不应填写工号 / 400 PARAM_ERROR：最大资源条数大于10 / 500 SYSTEM_ERROR：系统错误 / 500 SYSTEM_ERROR：扩展父子商户票据错误 / 500 SYSTEM_ERROR：调用云函数错误。</para>
    /// </remarks>
    [Get("/v3/smartguide/guides")]
    Task<SmartGuideListResponse> QuerySmartGuidesAsync(
        [Query("store_id")] int storeId,
        [Query("userid")] string? userid = null,
        [Query("mobile")] string? mobile = null,
        [Query("work_id")] string? workId = null,
        [Query("limit")] int? limit = null,
        [Query("offset")] int? offset = null,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务人员注册。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535138"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688564"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">服务人员注册请求体，字段见 <see cref="SmartGuideRegisterRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>服务人员注册，见 <see cref="SmartGuideRegisterResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/smartguide/guides</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>应答</b>见 <see cref="SmartGuideRegisterResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 MCH_NOT_EXISTS：商户号格式错误 / 400 PARAM_ERROR：必填参数为空 / 400 PARAM_ERROR：corpid格式错误 / 400 PARAM_ERROR：门店ID格式错误 / 400 PARAM_ERROR：userid格式错误 / 400 PARAM_ERROR：服务人员姓名格式错误 / 400 PARAM_ERROR：服务人员姓名过长 / 400 PARAM_ERROR：手机号码格式错误 / 400 PARAM_ERROR：二维码链接格式错误 / 400 PARAM_ERROR：头像URL不是合法链接 / 400 PARAM_ERROR：非企业微信提供的头像URL / 400 PARAM_ERROR：corpid为空 / 400 PARAM_ERROR：门店ID为空 / 400 PARAM_ERROR：userid为空 / 400 PARAM_ERROR：服务人员姓名为空 / 400 PARAM_ERROR：手机号码为空 / 400 PARAM_ERROR：二维码链接为空 / 400 PARAM_ERROR：头像URL为空 / 400 PARAM_ERROR：服务人员已注册 / 400 PARAM_ERROR：群二维码URL格式错误 / 500 SYSTEM_ERROR：系统错误 / 500 SYSTEM_ERROR：系统添加GUIDE失败 / 500 SYSTEM_ERROR：扩展父子商户票据错误 / 500 SYSTEM_ERROR：调用云函数错误 / 400 INVALID_REQUEST：子商户未授权 / 400 INVALID_REQUEST：子商户产品权限未开通 / 400 INVALID_REQUEST：子商户产品配置未完成 / 400 MCH_NOT_EXISTS：子商户不存在 / 400 MCH_NOT_EXISTS：子商户号格式错误 / 400 MCH_NOT_EXISTS：子商户号为空。</para>
    /// </remarks>
    [Post("/v3/smartguide/guides")]
    Task<SmartGuideRegisterResponse> RegisterSmartGuideAsync(
        [Body] SmartGuideRegisterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务人员更新。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535160"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688570"/>（【普通服务商】）。
    /// </summary>
    /// <param name="guideId">【服务人员ID】 服务人员在支付即服务系统中的唯一标识（官方 path <c>guide_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="request">服务人员更新请求体，字段见 <see cref="SmartGuideUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/smartguide/guides/{guide_id}</c>；path <c>guide_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方请求头表把该头列为<b>必填</b>（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名<b>不</b>承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径，由传输层按商户配置注入）；密文以 <c>string</c> 承载，SDK <b>不代为加解密</b>。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：400 MCH_NOT_EXISTS：商户号格式错误 / 400 PARAM_ERROR：必填参数为空 / 400 PARAM_ERROR：服务人员ID为空 / 400 PARAM_ERROR：服务人员ID格式错误 / 400 PARAM_ERROR：员工姓名格式错误 / 400 PARAM_ERROR：员工姓名过长 / 400 PARAM_ERROR：手机号码格式错误 / 400 PARAM_ERROR：二维码串格式错误 / 400 PARAM_ERROR：头像URL不是合法链接 / 400 PARAM_ERROR：非企业微信提供的头像URL / 400 PARAM_ERROR：个人微信商家不能改二维码 / 400 PARAM_ERROR：所有可更新的参数都没有填 / 400 PARAM_ERROR：群二维码URL格式错误 / 500 SYSTEM_ERROR：系统错误 / 500 SYSTEM_ERROR：扩展父子商户票据错误 / 500 SYSTEM_ERROR：调用云函数错误 / 400 INVALID_REQUEST：子商户未授权 / 400 INVALID_REQUEST：子商户产品权限未开通 / 400 INVALID_REQUEST：子商户产品配置未完成 / 400 MCH_NOT_EXISTS：子商户不存在 / 400 MCH_NOT_EXISTS：子商户号格式错误 / 400 MCH_NOT_EXISTS：子商户号为空。</para>
    /// </remarks>
    [Patch("/v3/smartguide/guides/{guideId}")]
    Task UpdateSmartGuideAsync(
        [Path] string guideId,
        [Body] SmartGuideUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务人员分配。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535161"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474145"/>（【普通服务商】）。
    /// </summary>
    /// <param name="guideId">【服务人员ID】 导购在导购系统中的唯一标识（官方 path <c>guide_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="request">服务人员分配请求体，字段见 <see cref="SmartGuideAssignRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/smartguide/guides/{guide_id}/assign</c>；path <c>guide_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：400 MCH_NOT_EXISTS：商户号格式错误 / 400 PARAM_ERROR：必填参数为空 / 400 PARAM_ERROR：服务人员ID格式错误 / 400 PARAM_ERROR：商户订单号格式错误 / 400 PARAM_ERROR：商户订单号过长 / 400 PARAM_ERROR：服务人员ID为空 / 400 PARAM_ERROR：商户订单号为空 / 500 SYSTEM_ERROR：系统错误 / 500 SYSTEM_ERROR：扩展父子商户票据错误 / 500 SYSTEM_ERROR：调用云函数错误 / 400 INVALID_REQUEST：子商户未授权 / 400 INVALID_REQUEST：子商户产品权限未开通 / 400 MCH_NOT_EXISTS：子商户号格式错误 / 400 MCH_NOT_EXISTS：子商户号为空。</para>
    /// </remarks>
    [Post("/v3/smartguide/guides/{guideId}/assign")]
    Task AssignSmartGuideAsync(
        [Path] string guideId,
        [Body] SmartGuideAssignRequest request,
        CancellationToken cancellationToken = default);
}
