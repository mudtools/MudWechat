// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BankPayScore;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「银行/从业机构支付分」域 SDK（<b>11 端点</b>：授权 3 + 服务订单 6 + 扣款信息 2）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（合作伙伴文档中心 · 开发文档(银行/机构) · 「从业机构-微信支付分（免确认模式）」，
/// 2026-10-14 逐页核验，各页「支持商户」栏恒为【从业机构（支付机构）】）：
/// 从业机构预授权 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691489"/>、
/// 查询用户授权记录（授权协议号）<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461004"/>、
/// 解除用户授权关系（授权协议号）<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461012"/>、
/// 创建支付分订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691326"/>、
/// 查询支付分订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691343"/>、
/// 取消支付分订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012458146"/>、
/// 完结支付分订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012460791"/>、
/// 修改支付分订单金额 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691363"/>、
/// 同步支付分订单状态 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691378"/>、
/// 登记扣款信息 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012458414"/>、
/// 查询扣款信息 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012458665"/>。</para>
/// <para><b>本域是「从业机构（支付机构）」面，不是服务商面</b>：官方章节挂在
/// 「开发文档(银行/机构)」树下，签名主体是<b>从业机构商户号</b>；渠道商与子商户只出现在报文里
/// —— 守卫 <c>PAY-B1</c> fail-closed 断言全域无 <c>[Token]</c>。</para>
/// <para><b>比服务商支付分域（P0-4）多一层 <c>channel_id</c></b>（守卫 BS-B1 逐端点锁定）：
/// 从业机构 → <b>渠道商</b> → 子商户三级，<c>channel_id</c>（渠道商商户号）在<b>十一端点的请求侧处处必填</b>
/// （四支 GET 与「查询授权记录」走 query，其余走 body），而服务商支付分域完全没有该字段。
/// <b>应答侧必填性会变</b>：创建/查询订单应答为必填，授权记录应答降为<b>选填</b> ⇒ 不能按请求形态推应答。</para>
/// <para><b>单一路由前缀</b>（与 P0-4 的三前缀形态相反）：全域 <c>/v3/payscore/acquiringbank/…</c>，
/// 无 <c>/v3/payscore/plan/…</c>、无 <c>/v3/payscore/sign-plan/…</c> —— 本域<b>没有</b>支付分计划与用户签约计划族
/// （守卫 BS-B2 以零声明断言钉住，并须落位相邻路由作正对照）。</para>
/// <para><b>本域只有免确认模式</b>：创单 <c>need_user_confirm</c> 官方<b>必填且固定 <c>false</c></b>，
/// 故 <c>state_description</c> 值集只有 <c>USER_CONFIRM</c>/<c>MCH_COMPLETE</c> 两值，
/// <b>不存在</b> P0-4「基于签约计划创建服务订单」页的 <c>MERCHANT_COMPLETE</c> 拼写（守卫 BS-B4 锁定）。</para>
/// <para><b>官方无「商户收款」「曝光」端点</b>：SKIT 清单的 <c>POST /pay/acquiring-bank/transactions/exposure</c>
/// 出处页（4014326541）现已跳至 50x（公益 404），现行侧栏 API 列表 16 项里也没有该能力；
/// 侧栏的「拉起支付分小程序授权页」「拉起支付分小程序订单详情页」是<b>端上形态</b>、
/// 「开启/解除授权服务回调通知」「商户预下单通知」「支付成功回调通知」是<b>回调面</b>
/// ⇒ 三类均<b>零端点</b>，不得按服务商面或直连面路由补进来（守卫 BS-B2）。</para>
/// <para><b>两套时间格式并存</b>（守卫 BS-B3 锁定）：<c>time_range</c> 用紧凑格式
/// （<c>yyyyMMddHHmmss</c>/<c>yyyyMMdd</c>，且 <c>start_time</c> 另有 <c>OnAccept</c> 取值）、
/// <c>collection.details[].paid_time</c> 与同步的 <c>detail.paid_time</c> 亦为紧凑格式，
/// 而 <c>complete_time</c>、授权记录的 <c>cancel_authorization_time</c>/<c>authorization_success_time</c> 走 rfc3339
/// （官方原文把该标准写作「Rfc399」，是其自身笔误）⇒ DTO 时间字段一律保持 <c>string</c>，不经
/// <see cref="DateTimeOffset"/> 往返。</para>
/// <para><b>本域业务错误码不建专有常量类</b>（守卫 BS-B5 记录裁决）：官方各页业务码全为通用名
/// （<c>INVALID_ORDER_STATE</c>/<c>INVALID_REQUEST</c>/<c>ORDER_CANCELED</c>/<c>ORDER_DONE</c>/
/// <c>ORDER_NOT_EXIST</c>/<c>ALREADY_EXIST</c>/<c>NO_AUTH</c>/<c>RULE_LIMIT</c>/<c>FREQUENCY_LIMITED</c>/
/// <c>SYSTEM_ERROR</c>），且「无授权资格」一支<b>本域十一页统一写 <c>NO_AUTH</c>（有下划线）</b> ——
/// 与 P0-4 服务商域「计划族写 <c>NOAUTH</c>」的形态相反；<c>ALREADY_EXIST</c> 只在登记扣款信息页出现。
/// 错误面统一由 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 原样透传与既有 <c>WechatPayErrorCodes</c> 承载。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>），不复用默认实例，否则与企微线撞 BaseUrl。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>/<see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，
/// 由 <c>WechatPayException.ThrowIfFailed</c> 判错；否则组件抛 <c>ApiException</c> 把官方业务码丢掉。</para>
/// <para><b>不做运行时多态</b>：每端点一个方法 + 各自请求 DTO；五支 <b>204 无包体</b>端点
/// （解除授权、取消、完结、修改金额、同步状态）返回裸 <see cref="Task"/>（守卫 BS-B1 计数锁定），
/// 不为其伪造响应类型。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BankPayScore", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBankPayScoreService
{
    /// <summary>
    /// 从业机构支付分「商户预授权」：以商户侧生成的授权协议号换取 <c>apply_permissions_token</c>，
    /// 供拉起支付分小程序授权页。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691489"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="BankPayScoreApplyPermissionsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>apply_permissions_token</c> 的应答，见 <see cref="BankPayScoreApplyPermissionsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/acquiringbank/permissions</c>；无 path / query 参数。</para>
    /// <para><b><c>channel_id</c> 是本域比服务商面多出的必填项</b>：渠道商须在服务商平台完成入驻，
    /// <c>sub_mchid</c> 由从业机构或渠道商用「开户意愿确认接口」生成。</para>
    /// <para><b><c>authorization_code</c> 是商户侧生成的授权单号</b>：32 字符内、同一商户号下唯一，
    /// 官方明令多个用户授权不得复用同一协议号。</para>
    /// <para><b>应答 token 有时效</b>（官方注明 1 小时），过期须重新调用本接口，不得缓存复用。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/permissions")]
    Task<BankPayScoreApplyPermissionsResponse> PreAuthorizeAsync(
        BankPayScoreApplyPermissionsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构支付分「查询用户授权记录（授权协议号）」。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461004"/>。
    /// </summary>
    /// <param name="authorizationCode">授权协议号（官方 path <c>authorization_code</c>，必填 string(32)）。</param>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，必填）。</param>
    /// <param name="channelId">渠道商商户号（官方 query <c>channel_id</c>，必填）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权记录，见 <see cref="BankPayScoreAuthorizationRecordResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/payscore/acquiringbank/permissions/authorization-code/{authorization_code}</c>，
    /// 其余三个参数<b>全部走 query</b>且<b>全必填</b>（官方 curl 原文
    /// <c>…?service_id=…&amp;channel_id=…&amp;sub_mchid=…</c>）。</para>
    /// <para><b>本域只开放协议号一支</b>：官方侧栏无「按 OpenID 查授权记录」页（服务商域有两支）
    /// ⇒ 本域恰 3 支授权端点（守卫 BS-B1 锁定）。</para>
    /// <para><b>应答里的 <c>channel_id</c> 是选填</b>（请求 query 必填）：本域请求与应答的必填性不镜像，勿按直觉推。</para>
    /// <para><b><c>authorization_state</c> 本页列 3 值</b>（含 <see cref="BankPayScoreAuthorizationStates.UnbindUser"/>
    /// =「仅完成预授权」）⇒ 预授权成功但用户未确认时状态是 <c>UNBINDUSER</c>，不得据此判定已获授权。</para>
    /// <para><b><c>user_label</c> 官方值集为 A/B/C 三值</b>，且「仅授权成功时返回、并且不保证一定返回」；
    /// 官方正文的比较说明自身被截断 ⇒ 不承诺排序语义。</para>
    /// </remarks>
    [Get("/v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}")]
    Task<BankPayScoreAuthorizationRecordResponse> QueryAuthorizationRecordAsync(
        [Path] string authorizationCode,
        [Query("service_id")] string serviceId,
        [Query("channel_id")] string channelId,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构支付分「解除用户授权关系（授权协议号）」。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461012"/>。
    /// </summary>
    /// <param name="authorizationCode">授权协议号（官方 path <c>authorization_code</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（<c>service_id</c> + <c>channel_id</c> + <c>sub_mchid</c> + <c>reason</c>），见 <see cref="BankPayScoreTerminatePermissionsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b>
    /// <c>/v3/payscore/acquiringbank/permissions/authorization-code/{authorization_code}/terminate</c>；
    /// 成功 <c>204</c> ⇒ 本方法返回裸 <see cref="Task"/>。</para>
    /// <para><b>影响面</b>：解除授权后该用户在本服务下不可再免确认创单，应答侧
    /// <c>authorization_state</c> 转为 <see cref="BankPayScoreAuthorizationStates.Unavailable"/>
    /// 并由 <c>cancel_authorization_time</c> 记录时点。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}/terminate")]
    Task TerminateAuthorizationAsync(
        [Path] string authorizationCode,
        BankPayScoreTerminatePermissionsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构创建支付分订单（免确认模式），返回订单与 <c>order_id</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691326"/>。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="BankPayScoreCreateOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单表 + <c>package</c>（本域可忽略），见 <see cref="BankPayScoreCreateOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/acquiringbank/serviceorder</c>；无 path / query 参数。</para>
    /// <para><b><c>need_user_confirm</c> 必填且固定 <c>false</c></b>：本域只开放免确认模式；
    /// <c>risk_fund</c> 与 <c>time_range</c> 在本域<b>必填</b>（服务商域同为必填，但本域 <c>notify_url</c> 亦必填）。</para>
    /// <para><b><c>openid</c> / <c>sub_openid</c> 二选一必传</b>，传 <c>sub_openid</c> 则 <c>sub_appid</c> 必填。</para>
    /// <para><b>红线</b>：<c>out_order_no</c> <b>不可</b>用作申请退款接口的 <c>out_trade_no</c>（官方原文）——
    /// 退款须用查询应答 <c>collection.details[].transaction_id</c>；<c>out_order_no</c> 是服务订单号，
    /// 而 <c>out_trade_no</c> 属于扣款单（见 <see cref="RegisterDeductInfoAsync"/>）。</para>
    /// <para><b>业务限制</b>：<c>appid</c> 须与从业机构商户号绑定、<c>sub_appid</c> 须与 <c>sub_mchid</c> 绑定；
    /// 用户达周期内创单上限时官方返回 <c>403 RULE_LIMIT</c>（须用户主动在支付分「服务管理页」去限制或次日再下单）。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/serviceorder")]
    Task<BankPayScoreCreateOrderResponse> CreateServiceOrderAsync(
        BankPayScoreCreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构查询支付分订单（含收款信息与代金券优惠明细）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691343"/>。
    /// </summary>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="channelId">渠道商商户号（官方 query <c>channel_id</c>，必填）。</param>
    /// <param name="outOrderNo">商户服务订单号（官方 query <c>out_order_no</c>，<b>选填</b>）。</param>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，<b>选填</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单全集，见 <see cref="BankPayScoreQueryOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/acquiringbank/serviceorder</c>，四参数全走 query；
    /// 官方必填组合是 <c>sub_mchid</c> + <c>channel_id</c>，而 <c>service_id</c> 与 <c>out_order_no</c>
    /// <b>在本请求侧都是选填</b>（应答侧 <c>service_id</c> 为必填）—— 与服务商域
    /// 「<c>sub_mchid</c> + <c>out_order_no</c> 必填」不同形（守卫 BS-B1 逐端点锁定）。</para>
    /// <para><b>与创建应答形态不同</b>：本应答<b>无</b> <c>package</c>、有 <c>collection</c>/
    /// <c>promotion_detail</c>/<c>total_amount</c>/<c>openid</c>/<c>sub_openid</c> ⇒ 分建两类型；
    /// <b>本域亦无 <c>need_collection</c></b>（服务商域查询应答有该字段且固定返回 <c>true</c>）。</para>
    /// <para><b>状态机</b>：官方原文要求<b>参考「支付分订单状态流转图」</b>处理，
    /// <c>state</c> 须结合 <c>collection.state</c> 与 <c>state_description</c> 判断；
    /// <c>collection</c> 仅在调完结后返回，完结金额为 0 时不返回。</para>
    /// <para><b>退款口径</b>：收款明细里 <c>paid_type=NEWTON</c> 才返回 <c>transaction_id</c>，
    /// 申请退款用该单号；<c>paid_type=MCH</c>（用户走其它渠道、商户调同步接口补登记）无该单号。</para>
    /// </remarks>
    [Get("/v3/payscore/acquiringbank/serviceorder")]
    Task<BankPayScoreQueryOrderResponse> QueryServiceOrderAsync(
        [Query("sub_mchid")] string subMchId,
        [Query("channel_id")] string channelId,
        [Query("out_order_no")] string? outOrderNo = null,
        [Query("service_id")] string? serviceId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构取消支付分订单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458146"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体（<c>service_id</c> 选填 / <c>sub_mchid</c> 必填 / <c>channel_id</c> 必填 / <c>reason</c> 必填），见 <see cref="BankPayScoreCancelOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/acquiringbank/serviceorder/{out_order_no}/cancel</c>；
    /// 成功 <c>204</c>。</para>
    /// <para><b><c>service_id</c> 在本页是选填</b>（完结/修改/同步页必填）—— 四页字段表各不同，照录本页。</para>
    /// <para><b><c>reason</c> 用户可见</b>：官方注明取消订单的消息通知会展示该字段，长度上限 50。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/cancel")]
    Task CancelServiceOrderAsync(
        [Path] string outOrderNo,
        BankPayScoreCancelOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构完结支付分订单（用户已实际接受服务，进入待收款状态）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460791"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 <b>string(64)</b>）。</param>
    /// <param name="request">请求体，字段见 <see cref="BankPayScoreCompleteOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/acquiringbank/serviceorder/{out_order_no}/complete</c>；
    /// 成功 <c>204</c>。</para>
    /// <para><b>⚠️ 本端点 path 参数官方宽度写 <c>string(64)</c></b>，而同族取消/修改/同步三页写
    /// <c>string(32)</c> —— 官方自身不一致，照录各自页（守卫 BS-B6 锁定），
    /// 商户侧单号生成仍以创单页的 32 为约束。</para>
    /// <para><b>金额必须自洽</b>：<c>total_amount</c> = <c>post_payments.amount</c> 总和 −
    /// <c>post_discounts.amount</c> 总和，且 ≤「服务 ID 风险金额上限」（须与微信支付运营/BD 确认）。</para>
    /// <para><b><c>complete_time</c> 是本域唯一的 rfc3339 请求字段</b>：官方约束落在
    /// <c>[t−24h, t+60s]</c>（<c>t</c> 为调用完结接口的当前时间），越界即 <c>PARAM_ERROR</c>。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/complete")]
    Task CompleteServiceOrderAsync(
        [Path] string outOrderNo,
        BankPayScoreCompleteOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构修改支付分订单金额（完结后仅可下调收款金额）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691363"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体，字段见 <see cref="BankPayScoreModifyOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/acquiringbank/serviceorder/{out_order_no}/modify</c>；
    /// 成功 <c>204</c>。官方为「修改金额」单设一页，SKIT 清单该行误挂取消页 4012458146，此处按官方侧栏修正。</para>
    /// <para><b>可调用窗口</b>：仅 <c>state=DOING</c> 且 <c>state_description=MCH_COMPLETE</c>，
    /// 且只能<b>下调</b>收款金额。</para>
    /// <para><b>扣款单互斥</b>：存在登记生效的扣款单（<see cref="RegisterDeductInfoAsync"/>）时须等其 24 小时失效、
    /// 或先调清算机构（银联/网联）关单，才能改金额。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/modify")]
    Task ModifyServiceOrderAsync(
        [Path] string outOrderNo,
        BankPayScoreModifyOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构同步支付分订单状态（用户已走<b>其它渠道</b>付款时补登记）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691378"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体，字段见 <see cref="BankPayScoreSyncOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回值 —— 官方成功应答为 <b>204 No Content</b>（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/payscore/acquiringbank/serviceorder/{out_order_no}/sync</c>；
    /// 成功 <c>204</c>。</para>
    /// <para><b><c>type</c> 官方单值</b>：<see cref="BankPayScoreSyncTypes.OrderPaid"/>（混合大小写，非全大写蛇形），
    /// 该 <c>type</c> 下 <c>detail</c> 必填。</para>
    /// <para><b><c>detail.paid_time</c> 有硬性区间</b>：官方要求
    /// 「商户调用完结接口时间 &lt; <c>paid_time</c> ≤ 商户调用同步订单接口时间 + 60 秒」，
    /// 支持 <c>yyyyMMddHHmmss</c> 与 <c>yyyyMMdd</c> 两种紧凑格式；微信支付分据该时间更新用户侧守约/负面记录
    /// ⇒ <b>必须如实填写</b>，造假将直接引发客诉。</para>
    /// <para><b>与完结同样的窗口与扣款单约束</b>：仅 <c>DOING</c> + <c>MCH_COMPLETE</c> 可调，
    /// 存在生效扣款单时须先失效或关单。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/sync")]
    Task SyncServiceOrderAsync(
        [Path] string outOrderNo,
        BankPayScoreSyncOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构登记扣款信息（清算机构委托代扣的前置登记），返回回显的 <c>description</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458414"/>。
    /// </summary>
    /// <param name="outOrderNo">商户服务订单号（官方 path <c>out_order_no</c>，必填 string(32)）。</param>
    /// <param name="request">请求体，字段见 <see cref="BankPayScoreRegisterDeductRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>description</c> 的应答，见 <see cref="BankPayScoreRegisterDeductResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b>
    /// <c>/v3/payscore/acquiringbank/serviceorder/{out_order_no}/registerdeductinfo</c>；成功 <c>200</c>。</para>
    /// <para><b>一致性是本端点的存在理由</b>：登记的 <c>out_trade_no</c> 与 <c>description</c> 必须与后续调用
    /// 清算机构（银联/网联）委托代扣接口时传的商户订单号、商品描述<b>逐字一致</b>，否则代扣失败。</para>
    /// <para><b>时效 24 小时</b>：生效期间该订单<b>不得</b>修改金额或同步状态，
    /// 须等扣款单失效或先走清算机构关单。</para>
    /// <para><b><c>ALREADY_EXIST</c> 只在本端点出现</b>：重复登记同一扣款单时官方返回该码，
    /// 属可重试判错面（守卫 BS-B5 记录，不建专有码常量类）。</para>
    /// </remarks>
    [Post("/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/registerdeductinfo")]
    Task<BankPayScoreRegisterDeductResponse> RegisterDeductInfoAsync(
        [Path] string outOrderNo,
        BankPayScoreRegisterDeductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从业机构查询扣款信息（回读已登记的 <c>description</c> 与 <c>out_trade_no</c>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458665"/>。
    /// </summary>
    /// <param name="serviceId">服务 ID（官方 query <c>service_id</c>，必填）。</param>
    /// <param name="appId">从业机构 AppID（官方 query <c>appid</c>，必填）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，必填）。</param>
    /// <param name="channelId">渠道商商户号（官方 query <c>channel_id</c>，必填）。</param>
    /// <param name="outOrderNo">商户服务订单号（官方 query <c>out_order_no</c>，必填）。</param>
    /// <param name="subAppId">子商户 AppID（官方 query <c>sub_appid</c>，<b>选填</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>扣款信息，见 <see cref="BankPayScoreDeductionQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/payscore/acquiringbank/serviceorder/deduction</c>，
    /// 六个参数<b>全部走 query</b>（无 path 参数）。</para>
    /// <para><b>⚠️ 路由形态陷阱</b>：本端点与 <see cref="QueryServiceOrderAsync"/> 同为
    /// <c>GET /v3/payscore/acquiringbank/serviceorder…</c> 前缀，本端点是<b>固定段 <c>/deduction</c></b>
    /// 而非 <c>/{out_order_no}/deduction</c> —— 按服务订单族的直觉拼 path 会得 <c>ORDER_NOT_EXIST</c>
    /// （守卫 BS-B2 以两支相邻路由并存锁定）。</para>
    /// <para><b>应答两字段均必填</b>：查询即回读登记值，用于代扣前自检一致性。</para>
    /// </remarks>
    [Get("/v3/payscore/acquiringbank/serviceorder/deduction")]
    Task<BankPayScoreDeductionQueryResponse> QueryDeductionInfoAsync(
        [Query("service_id")] string serviceId,
        [Query("appid")] string appId,
        [Query("sub_mchid")] string subMchId,
        [Query("channel_id")] string channelId,
        [Query("out_order_no")] string outOrderNo,
        [Query("sub_appid")] string? subAppId = null,
        CancellationToken cancellationToken = default);
}
