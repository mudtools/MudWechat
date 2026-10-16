// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BusinessCircle;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商圈会员（支付即积分）」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>0 支官方只印一页</b>、<b>4 支印多页</b>（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：1 / 4 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>无应答体支的返回形态</b>：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>四支各印商户页 + 服务商页 ⇒ 并集建模</b>：停车状态同步与积分同步两支两页请求表<b>逐字同形</b>； 「查授权状态」支服务商页另印<b>选填</b> <c>sub_mchid</c>（商户页根本没有该键）⇒ 一支一方法、身份差异写在参数注释里， <b>不得</b>按身份拆成两支方法或复制成两个 DTO。</para>
/// <para><b>两支是 204 无应答包体</b>：停车状态同步（POST parkings）与积分同步（POST points/notify）官方正常示例 <b>204 No Content</b> ⇒ 返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>积分同步支是「场内退款通知」的硬前置</b>：官方接入顺序原文「商圈会员积分同步（<b>只有接入该接口，才会获取到退款信息</b>）」， 且退款监控窗口是「对已同步过积分的会员消费，监控 <b>30 天内</b>的退款情况」⇒ 未回传积分就没有退款回调； 同支业务码「该订单积分已回传」「该订单已超过回传时效」也印证回传是一次性、有时效的。</para>
/// <para><b>本域唯一印端点级数字的是授权状态查询支</b>：官方原文「接口频率：150QPS」+「前置条件：商圈商户/服务商已开通支付即积分产品功能」； 其余三支官方<b>未印</b>数字 ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>正文有加密提示、请求头表却没列 <c>Wechatpay-Serial</c></b>：四支正文均写「商户上送敏感信息时需要使用微信支付公钥加密（推荐）」， 但四支的官方请求头表只有 <c>Authorization</c> / <c>Accept</c>（+ 写侧的 <c>Content-Type</c>）⇒ 按「涉密口径取请求头表」的判据， 本域判为<b>无涉密必填头</b>，与智慧导购、多次扣费发券支的逐页结论<b>不同</b>，不得据正文提示补头。</para>
/// <para><b>定位键三套并存、长度随页变</b>：<c>brandid</c> 官方类型是 <c>integer</c>（不是字符串），<c>sub_mchid</c> 在停车支为 <c>string(32)</c>、在积分与授权两支为 <c>string(64)</c>，<c>openid</c> 为 <c>string(64)</c> ⇒ 照页保留长度差异， 「服务商面一律 sub_mchid(32)」式归一会造出官方不存在的报文。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BusinessCircle", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBusinessCircleService
{
    /// <summary>
    /// 商圈会员停车状态同步。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535502"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474127"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">商圈会员停车状态同步请求体，字段见 <see cref="BusinessCircleParkingSyncRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/businesscircle/parkings</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：业务必填字段brand_id为空 / 400 PARAM_ERROR：业务必填字段AppID为空 / 400 PARAM_ERROR：业务必填字段OpenID为空 / 400 PARAM_ERROR：业务必填字段plate_number为空 / 400 PARAM_ERROR：业务必填字段state为空 / 400 PARAM_ERROR：业务必填字段time为空 / 400 PARAM_ERROR：plate_number格式有误 / 400 PARAM_ERROR：time格式有误 / 400 PARAM_ERROR：time超出取值范围 / 400 PARAM_ERROR：OpenID有误 / 400 PARAM_ERROR：AppID有误 / 403 NO_AUTH：mch_id与brand_id关系不匹配 / 403 NO_AUTH：OpenID与AppID不匹配 / 403 NO_AUTH：brandid与AppID不匹配 / 403 NO_AUTH：brandid无对应的唯一商圈mainid / 500 SYSTEM_ERROR：系统必填字段source为空 / 500 SYSTEM_ERROR：系统必填字段machine_key为空 / 500 SYSTEM_ERROR：系统必填字段mch_id为空 / 500 SYSTEM_ERROR：保存数据失败 / 500 SYSTEM_ERROR：brandid无对应的唯一商圈mainid / 400 PARAM_ERROR：sub_mch_code有误 / 403 NO_AUTH：父子关系不匹配 / 403 NO_AUTH：sub_mch_code与brand_id不匹配。</para>
    /// </remarks>
    [Post("/v3/businesscircle/parkings")]
    Task SyncBusinessCircleParkingAsync(
        [Body] BusinessCircleParkingSyncRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商圈会员积分同步。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534698"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474133"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">商圈会员积分同步请求体，字段见 <see cref="BusinessCirclePointsSyncRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/businesscircle/points/notify</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：该订单未提交积分 / 400 INVALID_REQUEST：该订单积分已回传 / 400 INVALID_REQUEST：该订单已超过回传时效 / 400 MCH_NOT_EXISTS：商圈商户号错误 / 400 PARAM_ERROR：未获得积分时无备注信息 / 400 PARAM_ERROR：OpenID参数错误。</para>
    /// </remarks>
    [Post("/v3/businesscircle/points/notify")]
    Task SyncBusinessCirclePointsAsync(
        [Body] BusinessCirclePointsSyncRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商圈会员积分服务授权查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534848"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474135"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【顾客OpenID】 顾客授权时使用的小程序上的OpenID（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="appid">【小程序AppID】 顾客授权积分时使用的小程序的AppID（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="subMchid">【商圈商户ID】 当以服务商模式管理商圈积分能力时，则要带上商圈商户ID，否则留空（官方 query <c>sub_mchid</c>，选填，类型 <c>string(64)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商圈会员积分服务授权查询，见 <see cref="BusinessCircleUserAuthorizationInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/businesscircle/user-authorizations/{openid}</c>；path <c>openid</c>，query <c>appid</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：- 接口频率：150QPS。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>appid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：商圈暂未开通支付即积分功能 / 400 PARAM_ERROR：请求参数中AppID非商圈支付即积分配置的AppID / 400 PARAM_ERROR：请求参数中AppID和OpenID不匹配 / 400 PARAM_ERROR：请求参数中AppID无效 / 400 PARAM_ERROR：请求参数中OpenID无效 / 400 INVALID_REQUEST：当前请求服务商非商圈支付即积分服务商。</para>
    /// </remarks>
    [Get("/v3/businesscircle/user-authorizations/{openid}")]
    Task<BusinessCircleUserAuthorizationInfoResponse> QueryBusinessCircleUserAuthorizationAsync(
        [Path] string openid,
        [Query("appid")] string appid,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商圈会员待积分状态查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534994"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474129"/>（【普通服务商】）。
    /// </summary>
    /// <param name="openid">【用户标识】 用户在商户对应AppID下的唯一标识（官方 path <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="brandid">【品牌ID】 调用方商户号对应的品牌brandid，调用方商户号需为此品牌brandid的品牌主商户号或品牌服务商商户号（官方 query <c>brandid</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="appid">【应用ID】 支持服务号、小程序等类型的AppID，需已与brandid完成下单AppID绑定（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商圈会员待积分状态查询，见 <see cref="BusinessCirclePointsCommitStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/businesscircle/users/{openid}/points/commit_status</c>；path <c>openid</c>，query <c>brandid</c> / <c>appid</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>brandid</c> 与 <c>appid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：传入的OpenID未对当前商圈授权会员快速积分 / 400 PARAM_ERROR：参数有误：应用ID（AppID）为空 / 400 PARAM_ERROR：参数有误：品牌ID（brandid）为空 / 400 PARAM_ERROR：参数有误：用户标识（OpenID）为空 / 400 PARAM_ERROR：参数有误：用户标识（OpenID）有误 / 403 NO_AUTH：填写的用户OpenID与AppID不匹配，请传入正确的AppID下的用户OpenID / 403 NO_AUTH：无权限，AppID未与brandid绑定 / 403 NO_AUTH：无权限，brandid下商圈不唯一 / 403 NO_AUTH：该接口仅支持接入商圈会员的商圈调用 / 400 PARAM_ERROR：参数有误：子商户号（sub_mchid）有误 / 403 NO_AUTH：无权限，传入的sub_mchid非brandid的品牌主商户号 / 403 NO_AUTH：无权限，mchid与submchid无父子关系。</para>
    /// </remarks>
    [Get("/v3/businesscircle/users/{openid}/points/commit_status")]
    Task<BusinessCirclePointsCommitStateResponse> QueryBusinessCirclePointsCommitStateAsync(
        [Path] string openid,
        [Query("brandid")] int brandid,
        [Query("appid")] string appid,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);
}
