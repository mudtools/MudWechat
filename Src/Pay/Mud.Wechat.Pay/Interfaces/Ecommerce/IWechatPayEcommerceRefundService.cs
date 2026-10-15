// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 退款」域 SDK（<b>9 端点</b>：申请退款两支 + 查单四支 + 垫付回补两支 + 异常退款一支）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐页逐字段核验）：申请退款（二级商户）
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>、按 <c>refund_id</c> 查单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/>、按 <c>out_refund_no</c> 查单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476911"/>、查询垫付回补
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476916"/>、发起垫付回补
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476927"/>、申请退款（个人收款方）
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649457"/>、个人收款方按 <c>refund_id</c> 查单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650253"/>、个人收款方按 <c>out_refund_no</c> 查单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650314"/>、异常退款
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649274"/>。</para>
/// <para><b>同一路由有两份契约，本域共三处</b>：<c>POST /v3/ecommerce/refunds/apply</c>、
/// <c>GET /v3/ecommerce/refunds/id/{refund_id}</c>、<c>GET /v3/ecommerce/refunds/out-refund-no/{out_refund_no}</c>
/// 三支各由「二级商户身份（按 <c>sub_mchid</c> 定位）」与「商品单个人收款方身份（按 <c>individual_auth_id</c> 定位）」
/// 两页分别成文。两族的<b>请求表与应答表均不重合</b>（二级商户查单 26 行 vs 个人收款方 21 行，差集恰是
/// <c>amount.from</c> 子表、<c>amount.advance</c>、<c>refund_account</c> 五项）⇒ 本接口按身份分建方法、
/// SDK 不提供「一个方法两种语义」的合并入口（守卫 EC-R1）。</para>
/// <para><b>「受理 ≠ 结果」是本域的主线</b>：官方原文「申请退款接口的返回仅代表业务的受理情况，具体退款是否成功，
/// 需要通过退款查询接口获取结果」，且两支受理应答<b>都没有 <c>status</c> 字段</b> ⇒ 拿到
/// <see cref="EcommerceRefundApplyResponse"/> 只表示退款单已创建，终态必须走 <see cref="QueryRefundByRefundIdAsync"/>
/// 一类查单方法读 <see cref="EcommerceRefundStatuses"/>。</para>
/// <para><b>365 天与 50 次是本域两条硬时限</b>：交易时间超过一年（按支付成功时间 <c>+365</c> 天）无法提交退款；
/// 每个支付订单的部分退款次数不超过 <b>50</b> 次。两者超限后没有任何接口可补救。</para>
/// <para><b>「一笔退款失败后重新提交，请不要更换退款单号，请使用原商户退款单号」</b>（官方原文，两支同文）⇒
/// SDK 不做任何单号改写或自动重试；同一 <c>out_refund_no</c> 多次请求官方只退一笔。</para>
/// <para><b>频率上限</b>（官方逐页给出）：申请退款两支均「<b>150 QPS</b>，且<b>单笔订单 1 QPM</b>」
/// —— 单笔订单每分钟只能申请一次是本线少见的单粒度限频，批量重试脚本必须先按订单排队；
/// 异常退款支 150 QPS；查单与回补各页只列通用 429 <c>FREQUENCY_LIMITED</c>。</para>
/// <para><b><c>amount.from</c> 与 <c>funds_account</c> 互斥</b>（官方原文「该功能优先级高于……且二者不可同时使用」）：
/// 出资账户指定多账户退款时须满足「基本账户可用 + 不可用出资金额之和 = 退款金额」「账户类型不得重复」
/// 「订单属于分账订单且分账处于待分账或分账中」等条件 ⇒ 条件校验交给官方判定，SDK 侧不加本地闸门。</para>
/// <para><b>垫付退款是需先开通的能力</b>：官方原文「垫付退款需要向微信支付申请开通权限，开通权限时需要指定一个
/// 垫付出款账户」，且「请联系微信支付客服」。回补方向与退款<b>相反</b>（资金回到垫付方）⇒
/// <see cref="EcommerceReturnAdvanceResponse"/> 的 <c>payer_*</c> 是出款方、<c>payee_*</c> 是入账方，
/// 两对字段名是本域最易读反的地方。</para>
/// <para><b>回补的 90 天重试窗口不可跳过</b>：官方原文「垫付回补结果为处理中，微信支付会在 90 天内定期重试，
/// 重试结果需要查单确认」；若 90 天后出款方账户仍余额不足，回补结果置为<b>回补失败</b>且
/// 「系统不再重试，商户也无法重试，后续不能再发起此退款单的回补」⇒ 处理期间须保证出款账户余额充足。</para>
/// <para><b><c>ABNORMAL</c> 的处置路径按身份分叉</b>：二级商户支的异常单官方指引是「登录商户平台交易中心人工处理」，
/// 而个人收款方支才有机器可调的 <see cref="ApplyAbnormalRefundAsync"/>（异常退款 API）⇒ 对个人收款方之外的单据
/// 调本方法属误用。异常退款官方限定<b>只支持退款至用户银行账户</b>（<c>type</c> 唯一取值
/// <see cref="EcommerceAbnormalRefundTypes"/>.<c>USER_BANK_CARD</c>），且仅支持招行、交通、农行、建行、工商、
/// 中行、平安、浦发、中信、光大、民生、兴业、广发、邮储、宁波银行这 15 家银行的<b>借记卡</b>。</para>
/// <para><b>本域加密面恰一支</b>：只有 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649274"/> 把
/// <c>Wechatpay-Serial</c> 列为参数表行，密文入参是 <c>bank_account</c> 与 <c>real_name</c>（均 string(1024)）⇒
/// 其余八支不得支配该头（守卫 EC-R6）。两个字段属收款人银行账号与真实姓名，
/// <b>绝不写入日志、遥测或异常消息</b>。</para>
/// <para><b>与直连退款域不共用契约</b>：<see cref="IWechatPayRefundService"/> 走 <c>/v3/refund/domestic/refunds/*</c>
/// 且按 <c>sub_mchid</c> 定位；该域已承载<b>普通服务商</b>面的异常退款
/// <c>POST /v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund</c>
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4013080627"/>）⇒ 与本域 <see cref="ApplyAbnormalRefundAsync"/>
/// 是同名不同路由的两支，不可互相替代。</para>
/// <para><b>退款到账时延</b>：官方原文「用零钱支付的退款 20 分钟内到账，银行卡支付的退款 3 个工作日后重新查询退款状态」
/// ⇒ 查单节奏应按支付方式区分，银行卡单过早轮询只会读到 <c>PROCESSING</c>。</para>
/// <para><b>客户端与凭据</b>：走支付专用命名客户端 <c>wechat-pay</c>，签名主体是当前作用域商户号；
/// 本域<b>零 <c>[Token]</c></b>。<c>[AllowAnyStatusCode]</c> 使 APIv3 的 4xx/5xx 错误体原样落到
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommerceRefundService
{
    /// <summary>
    /// 「申请退款 · 二级商户身份」：按 <c>sub_mchid</c> 定位收款方发起退款（<b>受理语义，不代表退款成功</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>。
    /// </summary>
    /// <param name="request">退款请求，见 <see cref="EcommerceRefundApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款受理单（<b>无 <c>status</c></b>），见 <see cref="EcommerceRefundApplyResponse"/>；终态须查单。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/refunds/apply</c>；限频 150 QPS 且<b>单笔订单 1 QPM</b>。</para>
    /// <para><b><c>sub_mchid</c> 官方标选填、注「二级商户收款必填」</b>（条件必填）；<c>transaction_id</c>
    /// 与 <c>out_trade_no</c> 均选填 ⇒ 原交易定位由宿主二选一保证。</para>
    /// <para><b>个人收款方请改用 <see cref="ApplyIndividualRefundAsync"/></b>：本方法报文不含
    /// <c>individual_auth_id</c>，混用即造出官方不认的载荷。</para>
    /// </remarks>
    [Post("/v3/ecommerce/refunds/apply")]
    Task<EcommerceRefundApplyResponse> ApplyRefundAsync(
        [Body] EcommerceRefundApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查单 · 二级商户身份，按微信退款单号」：读退款终态与资金去向。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/>。
    /// </summary>
    /// <param name="refundId">微信支付退款单号（官方 path <c>refund_id</c>，必填 string(32)）。</param>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款单全量，见 <see cref="EcommerceRefundQueryResponse"/>（与 <see cref="QueryRefundByOutRefundNoAsync"/><b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/refunds/id/{refund_id}?sub_mchid=</c>；
    /// <c>sub_mchid</c> 是<b>必填 query</b>，漏传即 400 <c>PARAM_ERROR</c>。</para>
    /// <para><b>查单比受理多出七个字段</b>：<c>status</c>、<c>transaction_id</c>、<c>out_trade_no</c>、
    /// <c>channel</c>、<c>user_received_account</c>、<c>success_time</c>、<c>funds_account</c> ⇒
    /// 退款终态与资金去向只能从本类方法取。</para>
    /// <para><b>时延口径</b>：零钱退款 20 分钟内到账，银行卡退款 3 个工作日后再查。</para>
    /// </remarks>
    [Get("/v3/ecommerce/refunds/id/{refundId}")]
    Task<EcommerceRefundQueryResponse> QueryRefundByRefundIdAsync(
        [Path] string refundId,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查单 · 二级商户身份，按商户退款单号」：与 <see cref="QueryRefundByRefundIdAsync"/> 同一应答类型，只是定位键不同。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476911"/>。
    /// </summary>
    /// <param name="outRefundNo">商户退款单号（官方 path <c>out_refund_no</c>，必填 string(64)）。</param>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按 <c>refund_id</c> 查单<b>共用同一应答类型</b>，见 <see cref="EcommerceRefundQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/refunds/out-refund-no/{out_refund_no}?sub_mchid=</c>。</para>
    /// <para><b>两页应答表逐行相同</b>（2026-10-15 比对，26 行）⇒ 共用一个类型；本域不含「退款列表」面，
    /// 官方无按商户号枚举退款单的端点。</para>
    /// </remarks>
    [Get("/v3/ecommerce/refunds/out-refund-no/{outRefundNo}")]
    Task<EcommerceRefundQueryResponse> QueryRefundByOutRefundNoAsync(
        [Path] string outRefundNo,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「发起垫付退款回补」：把电商平台垫付的退款资金从二级商户回补回垫付账户（<b>方向与退款相反</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476927"/>。
    /// </summary>
    /// <param name="refundId">微信退款单号（官方 path <c>refund_id</c>，必填 string(32)）：<b>必须是垫付退款的微信退款单</b>。</param>
    /// <param name="request">回补请求，见 <see cref="EcommerceReturnAdvanceRequest"/>（官方入参只有 <c>sub_mchid</c> 一行）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回补结果，见 <see cref="EcommerceReturnAdvanceResponse"/>（与 <see cref="QueryReturnAdvanceAsync"/><b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/refunds/{refund_id}/return-advance</c>；
    /// <c>sub_mchid</c> 走 <b>body</b>（查询支走 query）⇒ 同一凭据两种传法，照页实现。</para>
    /// <para><b>非垫付单回补会被拒</b>：官方注明回补对象必须是垫付退款的微信退款单。</para>
    /// <para><b>能力需先开通</b>（联系微信支付客服）；结果为处理中时官方在 <b>90 天</b>内定期重试，
    /// 90 天后仍余额不足即置回补失败且<b>不可再发起</b>。</para>
    /// </remarks>
    [Post("/v3/ecommerce/refunds/{refundId}/return-advance")]
    Task<EcommerceReturnAdvanceResponse> ReturnAdvanceAsync(
        [Path] string refundId,
        [Body] EcommerceReturnAdvanceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询垫付退款回补结果」：回补是异步动作，重试结果需查单确认。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476916"/>。
    /// </summary>
    /// <param name="refundId">微信退款单号（官方 path <c>refund_id</c>，必填 string(32)）。</param>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与发起回补<b>共用同一应答类型</b>，见 <see cref="EcommerceReturnAdvanceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/refunds/{refund_id}/return-advance?sub_mchid=</c>。</para>
    /// <para><b><c>result</c> 为处理中时商户可以重试</b>（取值见 <see cref="EcommerceReturnAdvanceResults"/>）；
    /// 判读方向注意 <c>payer_*</c> 是出款方、<c>payee_*</c> 是入账方。</para>
    /// </remarks>
    [Get("/v3/ecommerce/refunds/{refundId}/return-advance")]
    Task<EcommerceReturnAdvanceResponse> QueryReturnAdvanceAsync(
        [Path] string refundId,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「申请退款 · 商品单个人收款方身份」：按 <c>individual_auth_id</c> 定位收款方发起退款。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649457"/>。
    /// </summary>
    /// <param name="request">退款请求，见 <see cref="EcommerceIndividualRefundApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款受理单（<b>无 <c>status</c></b>，14 行），见 <see cref="EcommerceIndividualRefundApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/refunds/apply</c>（与二级商户身份支<b>同路由不同契约</b>）；
    /// 限频 150 QPS 且单笔订单 1 QPM。</para>
    /// <para><b>本页 <c>funds_account</c> 必填且官方只列 <c>UNSETTLED</c></b>（未结算资金）；二级商户支该字段选填
    /// 且另有 <c>AVAILABLE</c>/<c>PREPAID</c> ⇒ 同名字段的必填性与取值集同时分叉。</para>
    /// <para><b><c>transaction_id</c> 在本页是必填</b>（二级商户支选填）；本页无 <c>sub_mchid</c>、
    /// <c>sub_appid</c>、<c>refund_account</c> 与 <c>amount.from</c> 四项 ⇒ 传了即造出官方不认的载荷。</para>
    /// <para><b>结果通知口径不同</b>：本页官方写「需要通过退款查询接口<b>或退款结果通知</b>获取结果」。</para>
    /// </remarks>
    [Post("/v3/ecommerce/refunds/apply")]
    Task<EcommerceIndividualRefundApplyResponse> ApplyIndividualRefundAsync(
        [Body] EcommerceIndividualRefundApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查单 · 个人收款方身份，按微信退款单号」。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012650253"/>。
    /// </summary>
    /// <param name="refundId">微信支付退款单号（官方 path <c>refund_id</c>，必填 string(32)）。</param>
    /// <param name="individualAuthId">受理授权 ID（官方 query <c>individual_auth_id</c>，<b>必填</b> string(32)）：身份定位键，取代二级商户支的 <c>sub_mchid</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>退款单全量（21 行），见 <see cref="EcommerceIndividualRefundQueryResponse"/>（与 <see cref="QueryIndividualRefundByOutRefundNoAsync"/>、<see cref="ApplyAbnormalRefundAsync"/><b>三支共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/refunds/id/{refund_id}?individual_auth_id=</c>。</para>
    /// <para><b>与二级商户支查单应答的差集恰五项</b>：<c>amount.from</c>（含子表两行）、<c>amount.advance</c>、
    /// <c>refund_account</c> ⇒ 个人收款方<b>无垫付退款概念</b>，故不建「一个超集类型」。</para>
    /// <para><b>本身份支的 <c>ABNORMAL</c> 指向异常退款 API</b>（<see cref="ApplyAbnormalRefundAsync"/>），
    /// 与二级商户支「商户平台人工处理」的处置路径不同。</para>
    /// </remarks>
    [Get("/v3/ecommerce/refunds/id/{refundId}")]
    Task<EcommerceIndividualRefundQueryResponse> QueryIndividualRefundByRefundIdAsync(
        [Path] string refundId,
        [Query("individual_auth_id")] string individualAuthId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查单 · 个人收款方身份，按商户退款单号」：与 <see cref="QueryIndividualRefundByRefundIdAsync"/> 同一应答类型，只是定位键不同。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012650314"/>。
    /// </summary>
    /// <param name="outRefundNo">商户退款单号（官方 path <c>out_refund_no</c>，必填 string(64)）。</param>
    /// <param name="individualAuthId">受理授权 ID（官方 query <c>individual_auth_id</c>，<b>必填</b> string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按 <c>refund_id</c> 查单<b>共用同一应答类型</b>，见 <see cref="EcommerceIndividualRefundQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/refunds/out-refund-no/{out_refund_no}?individual_auth_id=</c>。</para>
    /// <para><b>三页应答表逐行相同</b>（2026-10-15 比对，21 行，含对 <c>ABNORMAL</c> 的处置指引文本亦一致）⇒ 共用一个类型。</para>
    /// </remarks>
    [Get("/v3/ecommerce/refunds/out-refund-no/{outRefundNo}")]
    Task<EcommerceIndividualRefundQueryResponse> QueryIndividualRefundByOutRefundNoAsync(
        [Path] string outRefundNo,
        [Query("individual_auth_id")] string individualAuthId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「申请异常退款」：对查单/通知确认为 <c>ABNORMAL</c>（原路退银行卡失败）的个人收款方退款单，改退至用户银行账户。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649274"/>。
    /// </summary>
    /// <param name="refundId">微信支付退款单号（官方 path <c>refund_id</c>，必填 string(32)）：待处理的异常退款单。</param>
    /// <param name="request">异常退款请求，见 <see cref="EcommerceAbnormalRefundRequest"/>（<c>bank_account</c>、<c>real_name</c> 为<b>密文</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>处理后的退款单，见 <see cref="EcommerceIndividualRefundQueryResponse"/>（与个人收款方查单两支<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/refunds/{refund_id}/apply-abnormal-refund</c>；限频 150 QPS。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：本方法是<b>本域唯一</b>需要该头的端点，
    /// <c>bank_account</c> 与 <c>real_name</c> 须以微信支付公钥（推荐）或平台证书公钥加密；
    /// 两个字段属收款人银行账号与真实姓名，禁止进日志、遥测或异常消息。</para>
    /// <para><b>前置状态是 <c>ABNORMAL</c></b>：官方原文「查询退款或退款结果通知确认状态为退款异常，可调用此接口」⇒
    /// 对 <c>PROCESSING</c>/<c>SUCCESS</c> 单调用属误用。</para>
    /// <para><b>只支持退款至用户银行卡</b>（<c>type</c> 唯一取值 <c>USER_BANK_CARD</c>），且仅支持 15 家银行的<b>借记卡</b>；
    /// <c>bank_type</c> 官方标选填、退款至用户时<b>必填</b>（条件必填）。</para>
    /// <para><b>与服务商面的同名端点不是同一契约</b>：<c>/v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund</c>
    /// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4013080627"/>）属普通服务商，已由
    /// <see cref="IWechatPayRefundService"/> 承载 ⇒ 路由不同、字段不同，不可互相替代。</para>
    /// </remarks>
    [Post("/v3/ecommerce/refunds/{refundId}/apply-abnormal-refund")]
    Task<EcommerceIndividualRefundQueryResponse> ApplyAbnormalRefundAsync(
        [Path] string refundId,
        [Body] EcommerceAbnormalRefundRequest request,
        CancellationToken cancellationToken = default);
}
