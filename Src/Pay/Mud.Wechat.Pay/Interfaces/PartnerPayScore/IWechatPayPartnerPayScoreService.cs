// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PartnerPayScore;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「服务商支付分」域 SDK（<b>23 端点</b>：授权 7 + 服务订单 9 + 签约计划 7）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（服务商文档中心 · 微信支付分（免确认模式），2026-10-14 逐页核验）：
/// 商户预授权 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012382030"/>、
/// 开启场景服务权限 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012778802"/>、
/// 评估用户分层 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012783585"/>、
/// 查询授权记录（协议号）<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692687"/>、
/// 查询授权记录（OpenID）<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692798"/>、
/// 解除授权（协议号）<see href="https://pay.weixin.qq.com/doc/v3/partner/4012382049"/>、
/// 解除授权（OpenID）<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461154"/>、
/// 创建订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692136"/>、
/// 查询订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692256"/>、
/// 取消订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012458758"/>、
/// 完结订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692408"/>、
/// 修改订单金额 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692360"/>、
/// 下发服务费用待处理消息 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013394914"/>、
/// 下发服务预扣费消息 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013394929"/>、
/// 同步服务订单状态 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012458816"/>、
/// 查询建议先享金额 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013394486"/>、
/// 创建支付分计划 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012463290"/>、
/// 查询支付分计划 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012463318"/>、
/// 终止支付分计划 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012463322"/>、
/// 基于签约计划创建服务订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012463469"/>、
/// 创建用户签约计划 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012463521"/>、
/// 查询用户签约计划 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012463526"/>、
/// 解除用户签约计划 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012463534"/>。</para>
/// <para><b>⚠️ 本域 AppID/商户号形态与服务商基础支付族相反</b>（守卫 PS-B3 锁定）：
/// 本域用 <c>appid</c>（服务商）+ <c>sub_appid</c>（子商户），应答里服务商商户号叫 <c>mchid</c> ——
/// <b>全域不存在 <c>sp_appid</c>/<c>sp_mchid</c></b>。跨域抄 <c>PartnerJsapiPrepayRequest</c> 会静默错参。</para>
/// <para><b>三族路由前缀并存</b>（守卫 PS-B2 锁定）：授权与服务订单族在 <c>/v3/payscore/partner/…</c>、
/// 支付分计划在 <c>/v3/payscore/plan/partner/…</c>、用户签约在 <c>/v3/payscore/sign-plan/partner/…</c> ——
/// 同一业务域内三套前缀，<b>不能</b>按基础支付族的 <c>/v3/pay/partner/</c> 直觉拼接。</para>
/// <para><b><c>sub_mchid</c> 的两处位置</b>（守卫 PS-B1 逐端点锁定）：多数在 body；
/// 六个 GET（授权记录查 ×2、查询订单、查询建议先享金额、查询计划、查询签约计划）与「评估用户分层」
/// 在 <b>query</b>（官方 curl 原文 <c>…?sub_mchid=</c>）。签名方法参数须逐端点核对，不得按一个模板套全族。</para>
/// <para><b>官方无「商户收款」端点</b>：SKIT 清单里的 <c>POST /v3/payscore/partner/serviceorder/{out_order_no}/pay</c>
/// 出处是<b>已下架旧 wiki 页</b>（现 302 跳到无关页），现行服务商支付分 API 列表侧栏 12 项不含该能力 ⇒
/// 本域<b>零端点</b>；亦<b>不得</b>按直连面 <c>POST /v3/payscore/serviceorder/{out_order_no}/pay</c> 补进来
/// （守卫 PS-B2 以零声明断言钉住，并须落位相邻路由作正对照）。</para>
/// <para><b>两套时间格式并存</b>（守卫 PS-B3 锁定）：<c>time_range</c> 与 <c>collection.details[].paid_time</c>
/// 用紧凑格式（<c>yyyyMMddHHmmss</c>/<c>yyyyMMdd</c>）、<c>prepaid_time</c> 只有 8 位，
/// 而 <c>complete_time</c>/<c>stop_time</c>/<c>plan_over_time</c>/<c>cancel_sign_time</c> 及签约明细的三个时间走 rfc3339。
/// 故 DTO 里时间字段一律保持 <c>string</c>，不经 <see cref="DateTimeOffset"/> 往返。</para>
/// <para><b>本域业务错误码不建专有常量类</b>（守卫 PS-B5 记录裁决）：官方各页业务码全为通用名
/// （<c>PARAM_ERROR</c>/<c>INVALID_REQUEST</c>/<c>INVALID_ORDER_STATE</c>/<c>ORDER_NOT_EXIST</c>/<c>NOAUTH</c>或<c>NO_AUTH</c>/<c>RULE_LIMIT</c>/<c>FREQUENCY_LIMITED</c>/<c>SYSTEM_ERROR</c>），
/// 且「无授权资格」一支<b>同域两套拼写并存</b>（2026-10-14 逐页核验）：授权族五页与服务额度页写
/// <c>NO_AUTH</c>（有下划线），计划族与用户签约族 7 页写 <c>NOAUTH</c>（无下划线），而服务订单族多数页面只有公共四码 ⇒ 任何「统一拼写」的常量表都会在一半端点上失真，
/// 错误面统一由 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 原样透传与既有 <c>WechatPayErrorCodes</c> 承载。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 <c>PAY-B1</c> fail-closed）：APIv3 全程以商户 API 证书做
/// RSA-SHA256 请求签名，签名主体是<b>服务商商户号</b>，子商户号只出现在报文里 —— 服务商<b>不得</b>用子商户证书调本域。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>），不复用默认实例，否则与企微线撞 BaseUrl。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>/<see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，
/// 由 <c>WechatPayException.ThrowIfFailed</c> 判错；否则组件抛 <c>ApiException</c> 把官方业务码丢掉。</para>
/// <para><b>不做运行时多态</b>：每端点一个方法 + 各自请求 DTO；八支 <b>204 无包体</b>端点返回裸
/// <see cref="Task"/>（守卫 PS-B1 计数锁定），不为其伪造响应类型。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PartnerPayScore", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPartnerPayScoreService
{
    /// <summary>
    /// 服务商支付分「商户预授权」：以商户侧生成的授权协议号换取 <c>apply_permissions_token</c>，
    /// 供拉起支付分小程序授权页。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012382030"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScorePermissionsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>apply_permissions_token</c> 的应答，见 <see cref="PartnerPayScorePermissionsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/permissions</c>；无 path / query 参数。</para>
    /// <para><b><c>authorization_code</c> 是商户侧生成的授权单号</b>：32 字符内、仅数字大小写字母与 <c>_-|*</c>、
    /// 同一<b>服务商商户号</b>下唯一，官方明令「多个用户授权不能重复使用同一授权协议号」。</para>
    /// <para><b><c>token</c> 时效 1 小时</b>（官方原文），过期须重新调用本接口，不得缓存复用。</para>
    /// <para><b><c>notify_url</c> 在本页是选填且 string(255)</b>：与创单页 <c>notify_url</c>（必填、256）不同，照录各自页。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/permissions")]
    Task<PartnerPayScorePermissionsResponse> PreAuthorizeAsync(
        PartnerPayScorePermissionsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商支付分「开启场景服务权限」（交易场景预授权），换取场景级
    /// <c>transaction_permissions_token</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012778802"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreScenePermissionsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回显 <c>scene</c> 与 <c>transaction_permissions_token</c>，见 <see cref="PartnerPayScoreScenePermissionsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/permissions/apply-for-scene</c>。</para>
    /// <para><b><c>scene</c> 当前官方只开放 <see cref="PartnerPayScoreScenes.Transaction"/></b>，
    /// 且该场景要求填 <c>transaction_scene_info</c>（内含 <c>mchid</c> + <c>sub_mchid</c>）⇒ 事实上必填。</para>
    /// <para><b>与 <see cref="PreAuthorizeAsync"/> 的差异</b>：本端点应答是<b>场景令牌</b>而非
    /// <c>apply_permissions_token</c>，两者用途不同，不可互换传给拉起授权页的参数。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/permissions/apply-for-scene")]
    Task<PartnerPayScoreScenePermissionsResponse> PreAuthorizeForSceneAsync(
        PartnerPayScoreScenePermissionsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商支付分「评估用户分层」（先用后付风控评估）：返回用户分层与建议先享额度。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012783585"/>。
    /// </summary>
    /// <param name="subMchId">子商户号（官方 <b>query</b> <c>sub_mchid</c>，必填 string(32)）。</param>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreRiskAssessRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权记录字段的全集再加风险分层四项，见 <see cref="PartnerPayScoreRiskAssessResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/permissions/assess-user-risk-level?sub_mchid=…</c>；
    /// <b>⚠️ 本端点最易漏的一处</b>：<c>sub_mchid</c> <b>不在请求体里</b>，官方 curl 原文把它放在 query ⇒
    /// 放进 body 会得到 <c>PARAM_ERROR</c>。</para>
    /// <para><b><c>authorization_code</c> / <c>openid</c> / <c>sub_openid</c> 三选一</b>（官方原文），
    /// 比创单族的「双 openid 二选一」多一个入口。</para>
    /// <para><b>商品信息是硬约束</b>：<c>shopping_info.goods</c> 一旦上送则各商品
    /// <c>name</c>/<c>picture</c>/<c>amount</c>/<c>count</c> 全必填，且官方注明
    /// 「所有商品单价 × 数量之和<b>不能超过服务风险金</b>」；<c>category_id</c> 最多 3 个且仅支持整数型品类 ID。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/permissions/assess-user-risk-level")]
    Task<PartnerPayScoreRiskAssessResponse> AssessUserRiskLevelAsync(
        [Query("sub_mchid")] string subMchId,
        PartnerPayScoreRiskAssessRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商支付分「查询用户授权记录（授权协议号）」。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692687"/>。
    /// </summary>
    /// <param name="authorizationCode">授权协议号（官方 path <c>authorization_code</c>，必填 string(32)）。</param>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，必填）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权记录，见 <see cref="PartnerPayScoreAuthorizationRecordResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/partner/permissions/authorization-code/{authorization_code}</c>，
    /// <c>service_id</c> 与 <c>sub_mchid</c> 走 <b>query</b>。</para>
    /// <para><b><c>authorization_state</c> 本页列 3 值</b>：含 <see cref="PartnerPayScoreAuthorizationStates.UnbindUser"/>，
    /// 而 OpenID 页只列 2 值 —— 同一字段名两套值集（守卫 PS-B4 锁定）。</para>
    /// </remarks>
    [Get("/v3/payscore/partner/permissions/authorization-code/{authorizationCode}")]
    Task<PartnerPayScoreAuthorizationRecordResponse> QueryAuthorizationRecordAsync(
        [Path] string authorizationCode,
        [Query("service_id")] string serviceId,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商支付分「查询用户授权记录（OpenID）」。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692798"/>。
    /// </summary>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，必填）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="appId">服务商 AppID（官方 query <c>appid</c>，必填）。</param>
    /// <param name="openId">用户在服务商 AppID 下的标识（官方 query <c>openid</c>，选填）。</param>
    /// <param name="subAppId">子商户 AppID（官方 query <c>sub_appid</c>，选填）。</param>
    /// <param name="subOpenId">用户在子商户 AppID 下的标识（官方 query <c>sub_openid</c>，选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权记录（与协议号页<b>共用</b>同一类型），见 <see cref="PartnerPayScoreAuthorizationRecordResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/partner/permissions/search</c>，六个参数<b>全部走 query</b>。</para>
    /// <para><b>与直连域的根本差异</b>：直连侧按 OpenID 查授权是
    /// <c>GET /v3/payscore/serviceorder/permissions/openid/{openid}</c>（path 形态）⇒
    /// 本域是 <c>/permissions/search</c> + query，<b>不得</b>照直连改路由。</para>
    /// <para><b>本端点无 body</b>：全部入参在 query，故签名不接请求 DTO。</para>
    /// </remarks>
    [Get("/v3/payscore/partner/permissions/search")]
    Task<PartnerPayScoreAuthorizationRecordResponse> QueryAuthorizationRecordByOpenIdAsync(
        [Query("service_id")] string serviceId,
        [Query("sub_mchid")] string subMchId,
        [Query("appid")] string appId,
        [Query("openid")] string? openId = null,
        [Query("sub_appid")] string? subAppId = null,
        [Query("sub_openid")] string? subOpenId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商支付分「解除用户授权关系（授权协议号）」。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012382049"/>。
    /// </summary>
    /// <param name="authorizationCode">授权协议号（官方 path <c>authorization_code</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（<c>service_id</c> + <c>sub_mchid</c> + <c>reason</c>），见 <see cref="PartnerPayScoreTerminateByCodeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/permissions/authorization-code/{authorization_code}/terminate</c>；
    /// 成功 <c>204</c> ⇒ 本方法返回裸 <see cref="Task"/>。</para>
    /// <para><b>影响面</b>：解除授权后该用户在本服务下不可再免确认创单；官方另有
    /// 「用户解除服务授权时连带取消服务下已签约协议」（<see cref="PartnerPayScoreCancelSignTypes.RevokeService"/>）
    /// ⇒ 解除授权须评估对进行中签约计划的影响。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/permissions/authorization-code/{authorizationCode}/terminate")]
    Task TerminateAuthorizationByCodeAsync(
        [Path] string authorizationCode,
        PartnerPayScoreTerminateByCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商支付分「解除用户授权关系（OpenID）」。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461154"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreTerminateByOpenIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/permissions/terminate</c>（<b>无 path 参数</b>，
    /// 用户标识在 body 里）；成功 <c>204</c>。</para>
    /// <para><b>与协议号页的字段差异</b>：本请求体多 <c>appid</c>(必) / <c>sub_appid</c>(选) /
    /// <c>openid</c> / <c>sub_openid</c>（二选一），而协议号页只有三字段 ⇒ 两个请求类型<b>不共用</b>。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/permissions/terminate")]
    Task TerminateAuthorizationByOpenIdAsync(
        PartnerPayScoreTerminateByOpenIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商创建支付分订单（免确认模式），返回订单与 <c>order_id</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692136"/>。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="PartnerPayScoreCreateOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单表 + <c>package</c>（本域可忽略），见 <see cref="PartnerPayScoreCreateOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/serviceorder</c>；无 path / query 参数。</para>
    /// <para><b>幂等</b>：官方原文「支持原参重入，相同参数重复调用可以返回成功」⇒ 可安全重试；
    /// 重入成功不等于业务只发生一次，落库仍以 <c>out_order_no</c> 为幂等键。</para>
    /// <para><b><c>need_user_confirm</c> 固定 <c>false</c></b>；<c>risk_fund</c> 与 <c>time_range</c> <b>必填</b>，
    /// 且 <c>risk_fund.name</c> 官方要求固定 <see cref="PartnerPayScoreRiskFundNames.EstimateOrderCost"/>。</para>
    /// <para><b>红线</b>：<c>out_order_no</c> <b>不可</b>用作申请退款接口的 <c>out_trade_no</c>（官方原文）——
    /// 退款须用查询应答 <c>collection.details[].transaction_id</c>。</para>
    /// <para><b>业务限制</b>：<c>appid</c>/<c>sub_appid</c> 须与对应商户号绑定；
    /// 用户达周期内创单上限时官方返回 <c>403 RULE_LIMIT</c>（须用户主动在支付分「服务管理页」去限制或次日再下单）。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/serviceorder")]
    Task<PartnerPayScoreCreateOrderResponse> CreateServiceOrderAsync(
        PartnerPayScoreCreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商查询支付分订单（含收款信息与代金券优惠明细）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692256"/>。
    /// </summary>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="outOrderNo">商户服务订单号（官方 query <c>out_order_no</c>，必填）。</param>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，<b>选填</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单全集，见 <see cref="PartnerPayScoreQueryOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/partner/serviceorder</c>，三参数全走 query；
    /// 官方必填组合是 <c>sub_mchid</c> + <c>out_order_no</c>，而 <c>service_id</c> 在请求侧为<b>选填</b>
    /// （应答侧为必填）。</para>
    /// <para><b>与创建应答形态不同</b>：本应答<b>无</b> <c>package</c>、有 <c>collection</c>/
    /// <c>promotion_detail</c>/<c>total_amount</c>/<c>openid</c>/<c>sub_openid</c> ⇒ 分建两类型。</para>
    /// <para><b>状态机</b>：官方原文要求<b>参考「支付分订单状态流转图」</b>处理，
    /// <c>state</c> 须结合 <c>collection.state</c> 与 <c>state_description</c> 判断；
    /// <c>collection</c> 仅在调完结后返回，完结金额为 0 时不返回。</para>
    /// </remarks>
    [Get("/v3/payscore/partner/serviceorder")]
    Task<PartnerPayScoreQueryOrderResponse> QueryServiceOrderAsync(
        [Query("sub_mchid")] string subMchId,
        [Query("out_order_no")] string outOrderNo,
        [Query("service_id")] string? serviceId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商取消支付分订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458758"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（<c>service_id</c> 选填 / <c>sub_mchid</c> 必填 / <c>reason</c> 必填），见 <see cref="PartnerPayScoreCancelOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/serviceorder/{out_order_no}/cancel</c>；成功 <c>204</c>。</para>
    /// <para><b><c>service_id</c> 在本页是选填</b>（创单/完结页必填）—— 三页字段表各不同，照录本页。</para>
    /// <para><b><c>reason</c> 用户可见</b>：官方注明取消订单的消息通知会展示该字段，长度上限 50。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/serviceorder/{outOrderNo}/cancel")]
    Task CancelServiceOrderAsync(
        [Path] string outOrderNo,
        PartnerPayScoreCancelOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商完结支付分订单（确认最终收款金额）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692408"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreCompleteOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/serviceorder/{out_order_no}/complete</c>；成功 <c>204</c>
    /// —— <b>与直连域完结接口返回订单体不同</b>，本域完结后须另调查单确认状态。</para>
    /// <para><b>金额校验式</b>：<c>total_amount</c> = <c>post_payments.amount</c> 之和 − <c>post_discounts.amount</c> 之和，
    /// 且 ≤ 服务 ID 风险金额上限（官方注明该上限须与 BD 确认）。</para>
    /// <para><b>时间约束</b>：<c>complete_time</c>（rfc3339）须晚于或等于 <c>end_time</c>、且早于或等于调接口当前时间；
    /// 本域 <c>time_range</c> 走紧凑格式，两处格式不同勿统一。</para>
    /// <para><b>分账标记</b>：<c>profit_sharing=true</c> 时收款资金<b>冻结</b>待请求分账，
    /// 未分账且距支付完成超 30 天自动解冻 —— 属分账域编排，本方法只负责上送标记。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/serviceorder/{outOrderNo}/complete")]
    Task CompleteServiceOrderAsync(
        [Path] string outOrderNo,
        PartnerPayScoreCompleteOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商修改支付分订单金额。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692360"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreModifyOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/serviceorder/{out_order_no}/modify</c>；成功 <c>204</c>。</para>
    /// <para><b>与完结页的字段差异</b>：本页 <c>service_id</c> <b>选填</b>、新增必填 <c>reason</c>、
    /// 无 <c>time_range</c>/<c>location</c>/<c>profit_sharing</c>/<c>complete_time</c>/<c>goods_tag</c> ⇒ 两请求类型不共用。</para>
    /// <para><b>金额上限与校验式同完结页</b>（官方同页原文），且修改后的总额仍受服务 ID 风险金额上限约束。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/serviceorder/{outOrderNo}/modify")]
    Task ModifyServiceOrderAsync(
        [Path] string outOrderNo,
        PartnerPayScoreModifyOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商「下发服务费用待处理消息」：向用户推送待处理服务费用提醒。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013394914"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（与 <see cref="SendPrepaidMessageAsync"/> <b>共用</b>），见 <see cref="PartnerPayScoreMessageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/serviceorder/{out_order_no}/send-pendingmessage</c>；成功 <c>204</c>。</para>
    /// <para><b>两支消息端点请求表逐项一致</b> ⇒ 共用 <see cref="PartnerPayScoreMessageRequest"/>（守卫 PS-B6 锁定），
    /// 差异只在路由与消息语义。</para>
    /// <para><b><c>prepaid_time</c> 只有 8 位（<c>YYYYMMDD</c>）且须 ≥ 调接口时间 + 7 天</b>：
    /// 官方原文示例「2023-12-01 调用可传 20231208」——按本域第三种时间形态处理，勿写 rfc3339。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/serviceorder/{outOrderNo}/send-pendingmessage")]
    Task SendPendingMessageAsync(
        [Path] string outOrderNo,
        PartnerPayScoreMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商「下发服务预扣费消息」：向用户推送预扣费提醒。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013394929"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（与 <see cref="SendPendingMessageAsync"/> <b>共用</b>），见 <see cref="PartnerPayScoreMessageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/serviceorder/{out_order_no}/send-prepaidmessage</c>；成功 <c>204</c>。</para>
    /// <para><b>两支消息端点各自独立</b>：官方把它们列为 API 列表的两项（待处理 / 预扣费），
    /// 请求表一致但<b>不可</b>合并成一个方法带「消息类型参数」运行期分派（不做运行时多态）。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/serviceorder/{outOrderNo}/send-prepaidmessage")]
    Task SendPrepaidMessageAsync(
        [Path] string outOrderNo,
        PartnerPayScoreMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商同步服务订单状态：把「用户已在商户自有渠道付款」这一事实同步给支付分。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458816"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreSyncOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/partner/serviceorder/{out_order_no}/sync</c>；成功 <c>204</c>。</para>
    /// <para><b><c>type</c> 官方固定值</b>：<see cref="PartnerPayScoreSyncTypes.OrderPaid"/>（<c>Order_Paid</c>，
    /// 大小写照原文），且该值存在时 <c>detail</c> 必填。</para>
    /// <para><b>60 秒窗口</b>：官方要求「调完结接口时间 &lt; <c>paid_time</c> ≤ 调同步接口时间 + 60 秒」⇒
    /// 同步必须在完结之后、且实付时间如实填写。</para>
    /// <para><b>副作用</b>：微信支付分据本接口更新用户侧<b>守约记录 / 负面记录</b>，
    /// 虚报实付时间会造成客诉；成功后查单的 <c>collection.details[].paid_type</c> 返回
    /// <see cref="PartnerPayScoreCollectionPaidTypes.Merchant"/>。</para>
    /// </remarks>
    [Post("/v3/payscore/partner/serviceorder/{outOrderNo}/sync")]
    Task SyncServiceOrderAsync(
        [Path] string outOrderNo,
        PartnerPayScoreSyncOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商查询「用户分层对应建议先享金额」字典。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013394486"/>。
    /// </summary>
    /// <param name="serviceId">服务 ID（官方 path <c>service_id</c>，必填 string(32)）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="appId">服务商 AppID（官方 query <c>appid</c>，必填）。</param>
    /// <param name="riskLevelVersion">分层版本（官方 query <c>risk_level_version</c>，必填整型）。</param>
    /// <param name="subAppId">子商户 AppID（官方 query <c>sub_appid</c>，选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分层 → 建议先享金额列表，见 <see cref="PartnerPayScoreServiceQuotaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/partner/servicequotas/service-id/{service_id}</c>：
    /// 本域<b>唯一 path + query 混合</b>的 GET，且 <c>servicequotas</c> 段不属于
    /// <c>serviceorder</c> 资源族。</para>
    /// <para><b><c>risk_level_version</c> 必填整型</b>：该版本来自
    /// <see cref="AssessUserRiskLevelAsync"/> 应答的 <c>risk_level_version</c>，须成对使用；
    /// 分层与金额都是<b>整型</b>，不按字符串枚举处理。</para>
    /// <para><b>本端点是字典式查询</b>：返回的是「该服务 + 该版本下」的分层额度表，与具体用户无关 ⇒
    /// 不落库为订单数据，宿主可缓存。</para>
    /// </remarks>
    [Get("/v3/payscore/partner/servicequotas/service-id/{serviceId}")]
    Task<PartnerPayScoreServiceQuotaResponse> GetServiceQuotasAsync(
        [Path] string serviceId,
        [Query("sub_mchid")] string subMchId,
        [Query("appid")] string appId,
        [Query("risk_level_version")] int riskLevelVersion,
        [Query("sub_appid")] string? subAppId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商创建支付分计划（多次扣费的计划模板）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463290"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreCreatePlanRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>计划表（查询 / 终止计划<b>共用</b>同一应答类型），见 <see cref="PartnerPayScorePlanResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/plan/partner/payscore-plans</c>；
    /// 注意前缀是 <c>/v3/payscore/plan/partner/</c>，<b>不在</b>服务订单族的 <c>/v3/payscore/partner/</c> 下。</para>
    /// <para><b>官方四条一致性校验</b>（400 直接拒）：<c>deduction_quantity</c> 须等于明细条数；
    /// 明细实付金额须<b>依序不递增</b>；每条 <c>original_price</c> ≥ <c>actual_price</c>；
    /// 两个总额须与明细对应金额之和一致且总实付不得为 0 ⇒ 上送前应在宿主侧自查。</para>
    /// <para><b>明细序号由微信侧生成</b>：官方按列表顺序从 1 编号，商户不得自报 <c>plan_detail_no</c>，
    /// 且使用时须<b>依序</b>。</para>
    /// <para><b>后续定位用 <c>merchant_plan_no</c></b>：查询与终止计划的 path 都用商户侧计划号，而非 <c>plan_id</c>。</para>
    /// </remarks>
    [Post("/v3/payscore/plan/partner/payscore-plans")]
    Task<PartnerPayScorePlanResponse> CreatePayScorePlanAsync(
        PartnerPayScoreCreatePlanRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商按商户侧计划号查询支付分计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463318"/>。
    /// </summary>
    /// <param name="merchantPlanNo">商户侧计划号（官方 path <c>merchant_plan_no</c>，必填 string(32)）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>计划表，见 <see cref="PartnerPayScorePlanResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchant_plan_no}?sub_mchid=…</c>。</para>
    /// <para><b>path 段是 <c>merchant-plan-no</c> 的字面量前缀</b>：照官方原文，不得简写成 <c>plan-no</c>。</para>
    /// <para><b>应答表与创建 / 终止两页逐项一致</b> ⇒ 三支端点共用 <see cref="PartnerPayScorePlanResponse"/>（守卫 PS-B6）。</para>
    /// </remarks>
    [Get("/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}")]
    Task<PartnerPayScorePlanResponse> QueryPayScorePlanAsync(
        [Path] string merchantPlanNo,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商终止支付分计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463322"/>。
    /// </summary>
    /// <param name="merchantPlanNo">商户侧计划号（官方 path <c>merchant_plan_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（<b>仅 <c>sub_mchid</c> 一个字段</b>），见 <see cref="PartnerPayScoreStopPlanRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>终止后的计划表（含 <c>stop_mchid</c>/<c>stop_time</c>），见 <see cref="PartnerPayScorePlanResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b>
    /// <c>/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchant_plan_no}/stop</c>；
    /// 本端点<b>有应答体</b>（200，计划表），与服务订单族的八支 204 端点形态相反。</para>
    /// <para><b>请求体没有终止原因</b>：官方本页 body 只有 <c>sub_mchid</c>；
    /// 带 <c>stop_reason</c> 的是「解除用户签约计划」⇒ 勿据那一页补字段。</para>
    /// <para><b>终止语义</b>：计划转 <see cref="PartnerPayScorePlanStates.Stop"/> 后不可新签约，
    /// 但<b>已签约的计划仍可继续完成服务</b>（官方原文）。</para>
    /// </remarks>
    [Post("/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}/stop")]
    Task<PartnerPayScorePlanResponse> StopPayScorePlanAsync(
        [Path] string merchantPlanNo,
        PartnerPayScoreStopPlanRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商「基于签约计划创建支付分服务订单」（计划履约的一次扣费订单）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463469"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreSignPlanServiceOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单表子集（金额派生自签约计划详情），见 <see cref="PartnerPayScoreSignPlanServiceOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/sign-plan/partner/serviceorder</c>；
    /// 前缀是 <c>/v3/payscore/sign-plan/partner/</c>（第三套前缀），且路由段是 <c>serviceorder</c> 单数
    /// 而<b>不带</b> <c>/{out_order_no}</c> —— 订单号由 body 的 <c>out_trade_no</c> 提供。</para>
    /// <para><b>请求字段名是 <c>out_trade_no</c>、应答字段名回落成 <c>out_order_no</c></b>：官方两表拼写不同，照录。</para>
    /// <para><b><c>out_trade_no</c> 必须等于该明细在创建签约计划时上送的 <c>merchant_plan_detail_no</c></b>（官方原文）。</para>
    /// <para><b>履约串行约束</b>：同时只能有一个签约计划存在服务中的订单，且不允许跳序号 ——
    /// 创建下一条明细的订单前须先取消上一订单，或完结并等支付成功。</para>
    /// <para><b>不重复上送金额</b>：本请求无 <c>post_payments</c>/<c>risk_fund</c>/<c>total_amount</c>，
    /// 全部金额由订单对应的签约计划详情派生；<b>本域无「计划收款 / 商户收款」端点</b>（设计方案 §2.4 零端点裁决）。</para>
    /// </remarks>
    [Post("/v3/payscore/sign-plan/partner/serviceorder")]
    Task<PartnerPayScoreSignPlanServiceOrderResponse> CreateSignPlanServiceOrderAsync(
        PartnerPayScoreSignPlanServiceOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商创建用户签约计划，返回签约计划与拉起签约页所需的 <c>package</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463521"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="PartnerPayScoreCreateUserSignPlanRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns><b>包装形态</b> <c>{sign_plan, package}</c>，见 <see cref="PartnerPayScoreCreateUserSignPlanResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/sign-plan/partner/user-sign-plans</c>（复数资源）。</para>
    /// <para><b>本支应答是三支签约端点中唯一的包装形态</b>：查询与解除把签约计划字段<b>平铺</b>在应答根
    /// （返回 <see cref="PartnerPayScoreSignPlan"/>）⇒ 两形态分建，勿合并。</para>
    /// <para><b><c>package</c> 必填且须 URLEncode 后使用</b>：官方注明它是「跳转微信侧确认签约小程序的数据」——
    /// 与服务订单族那个可忽略的 <c>package</c> 语义相反，不得丢弃。</para>
    /// <para><b><c>sign_plan_detail</c> 是「明细序号 + 商户侧单号」配对列表</b>（必填）：
    /// 该 <c>merchant_plan_detail_no</c> 后续即该明细服务订单的 <c>out_trade_no</c>，两处必须一致。</para>
    /// <para><b><c>notify_url</c> 本页上限 string(1024)</b>（签约成功事件通知），比服务订单族的 256 宽得多，照录本页。</para>
    /// </remarks>
    [Post("/v3/payscore/sign-plan/partner/user-sign-plans")]
    Task<PartnerPayScoreCreateUserSignPlanResponse> CreateUserSignPlanAsync(
        PartnerPayScoreCreateUserSignPlanRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商按商户侧签约计划号查询用户签约计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463526"/>。
    /// </summary>
    /// <param name="merchantSignPlanNo">商户侧签约计划号（官方 path <c>merchant_sign_plan_no</c>，必填 string(32)）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填：须是该签约计划的<b>创建方</b>子商户）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>签约计划（平铺形态），见 <see cref="PartnerPayScoreSignPlan"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchant_sign_plan_no}?sub_mchid=…</c>。</para>
    /// <para><b>应答表与「解除用户签约计划」页逐项一致</b> ⇒ 两支共用 <see cref="PartnerPayScoreSignPlan"/>；
    /// 该类型同时承载创建应答里的嵌套 <c>sign_plan</c>（守卫 PS-B6 锁定三处共用）。</para>
    /// <para><b>读侧判据</b>：<c>sign_state</c>（五值）+ <c>going_detail_no</c>（<c>0</c> 表示尚未使用任何明细）
    /// + <c>signed_detail_list[].plan_detail_state</c>（四值）三层共同决定能否继续为该签约计划创建下一笔订单。</para>
    /// <para><b>金额字段拼写</b>：本表用 <c>total_origin_price</c>，计划表用 <c>total_original_price</c> —— 官方两套拼写，勿统一。</para>
    /// </remarks>
    [Get("/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}")]
    Task<PartnerPayScoreSignPlan> QueryUserSignPlanAsync(
        [Path] string merchantSignPlanNo,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商解除用户签约计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012463534"/>。
    /// </summary>
    /// <param name="merchantSignPlanNo">商户侧签约计划号（官方 path <c>merchant_sign_plan_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（<c>sub_mchid</c> + <c>stop_reason</c>），见 <see cref="PartnerPayScoreStopUserSignPlanRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>解除后的签约计划（平铺形态），见 <see cref="PartnerPayScoreSignPlan"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b>
    /// <c>/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchant_sign_plan_no}/stop</c>；
    /// <b>本支有 200 应答体</b>（与计划族的 stop 同形态）。</para>
    /// <para><b>与「终止支付分计划」的两处差异</b>：本请求体多必填 <c>stop_reason</c>(128)，
    /// 且 <c>sub_mchid</c> 须是<b>该签约计划的创建方</b>子商户 ⇒ 两请求类型不共用。</para>
    /// <para><b>解除后应答</b>：官方在 <c>cancel_sign_type</c> 返回
    /// <see cref="PartnerPayScoreCancelSignTypes.Merchant"/>、<c>sign_state</c> 转
    /// <see cref="PartnerPayScoreSignStates.SignPlanCancel"/>，并以 <c>cancel_sign_time</c>/<c>cancel_reason</c> 留痕。</para>
    /// </remarks>
    [Post("/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}/stop")]
    Task<PartnerPayScoreSignPlan> StopUserSignPlanAsync(
        [Path] string merchantSignPlanNo,
        PartnerPayScoreStopUserSignPlanRequest request,
        CancellationToken cancellationToken = default);
}
