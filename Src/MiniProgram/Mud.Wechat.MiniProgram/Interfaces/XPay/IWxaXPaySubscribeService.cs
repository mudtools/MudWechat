// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「虚拟支付 · 会员订阅」域 SDK（4 端点：预通知扣款 / 发起订阅扣款 / 查询签约关系 / 商家解约）。
/// </summary>
/// <remarks>
/// <para>
/// 域统一形态（POST only、不支持云调用、权限集 157、<c>pay_sig</c> 落 Query 的凭据处置、敏感值口径与
/// 全域错误码表三分）见 <see cref="IWxaXPayCurrencyService"/>，本文件只写本族特有事实。
/// </para>
/// <para>
/// <b>本族是全域唯一「两端点自带独立错误码表」的章节</b>：<see cref="SendSubscribePrePaymentAsync"/> 回
/// <c>674690001</c> / <c>690000000</c> / <c>690000001</c>，<see cref="SubmitSubscribePayOrderAsync"/> 回米大师侧
/// <c>-15001 ~ -15027</c> 与 <c>1001</c> —— 二者都<b>不是</b>本域共享的 <c>26849_0xx</c> 表 ⇒ 判定器不得按域表前缀过滤。
/// </para>
/// <para>
/// <b>三步时序由官方错误码强制</b>：签约关系（<see cref="QuerySubscribeContractAsync"/>）→ 预通知
/// （<see cref="SendSubscribePrePaymentAsync"/>）→ 扣款（<see cref="SubmitSubscribePayOrderAsync"/>）。
/// 官方 <c>690000000</c>「user not subscribed」拦未签约、<c>-15025</c>「不存在预通知单/重复下单」拦未预通知 ⇒
/// 顺序错不是参数错，重试同一请求不会成功。
/// </para>
/// <para>
/// <b>本族的定位键是协议号 <c>out_contract_code</c> + 订阅制道具 <c>product_id</c> 的组合</b>（三支端点都要求这两支），
/// 且官方对道具单列「需为订阅制道具」⇒ 用直购道具 id 调用本族必然失败。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "XPay", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaXPaySubscribeService
{
    /// <summary>
    /// 预通知扣款（<c>POST /xpay/send_subscribe_pre_payment</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_send_subscribe_pre_payment.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>deduct_price</c> / <c>product_id</c> / <c>out_contract_code</c> 四支全必填），见 <see cref="WxaXPaySendSubscribePrePaymentRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>，不造 DTO。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；<b>本端点参数表没有 <c>env</c> 支</b>（本族四端点里唯一有
    /// <c>deduct_price</c> 下限的一支：官方原文「金额/分，属于 [100，道具价格]」⇒ 低于 1 元的预通知被
    /// <c>674690001</c> 拒绝）。
    /// </para>
    /// <para>
    /// <b>预通知不等于扣款</b>：本调用只向用户推送扣款提醒，实际资金动作走 <see cref="SubmitSubscribePayOrderAsync"/>；
    /// 官方 <c>690000001</c>「not allowed to trigger pre payment at this time」说明提醒有<b>时间窗约束</b>，
    /// 不是随时可发。
    /// </para>
    /// <para><b><c>690000000</c>（未签约）是本页最常见的失败形态</b> ⇒ 扣款前应先以
    /// <see cref="QuerySubscribeContractAsync"/> 确认 <c>SIGNED</c>。</para>
    /// </remarks>
    [Post("/xpay/send_subscribe_pre_payment")]
    Task<WxaResponse> SendSubscribePrePaymentAsync(
        [Body] WxaXPaySendSubscribePrePaymentRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起订阅扣款（<c>POST /xpay/submit_subscribe_pay_order</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_submit_subscribe_pay_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（九支除 <c>env</c> 外全必填），见 <see cref="WxaXPaySubmitSubscribePayOrderRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>；扣款结果须回 <c>IWxaXPayOrderService.QueryOrderAsync</c> 查单。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；本端点是本域<b>请求面最宽</b>的一支（9 字段），且
    /// <c>offer_id</c> 是米大师侧应用 id（公众号支付基础配置里的 offerid）、<c>currency_type</c> 官方目前只接受
    /// <c>CNY</c>（<c>-15004</c>）。
    /// </para>
    /// <para>
    /// <b><c>env</c> 在本端点上只能填 0</b>（官方原文「环境配置, 0 正式环境, 仅支持正式环境」，错误码
    /// <c>-15011</c> 明示沙箱值被拒）⇒ 与本页参数表「否（选填）」的标注并不冲突，但<b>不可</b>据此认为沙箱可用。
    /// </para>
    /// <para>
    /// <b>错误码表与全域不同形</b>：本端点回米大师侧负数码 <c>-15001 ~ -15027</c> 与 <c>1001</c>，
    /// 其中 <c>-15002</c>（单号重复）官方原文「请换新单号重试」与 <see cref="SendSubscribePrePaymentAsync"/>
    /// 侧 <c>-15025</c>（不存在预通知单/重复下单）共同说明<b>单号不可复用</b>；而 <c>-15012</c>
    /// （调用米大师失败导致关单）同样要求换新单号 ⇒ 幂等重放必须原单号，这两类必须换单号，
    /// <b>宿主须按码值分流而非一律重试</b>。
    /// </para>
    /// <para>
    /// <b>官方自注唯一性校验不可强依赖</b>（原文「不建议业务强依赖平台对这里的唯一性校验」）⇒ 幂等责任在调用方单号体系。
    /// </para>
    /// <para><b>发布生效延迟</b>：<c>-15014</c> 原文「道具/代币发布未生效，禁止下单，大概 10 分钟后生效」⇒
    /// 刚发布的道具立即扣款会失败，这是官方正文里本域唯一的<b>延迟数字</b>，照录不做推断。</para>
    /// </remarks>
    [Post("/xpay/submit_subscribe_pay_order")]
    Task<WxaResponse> SubmitSubscribePayOrderAsync(
        [Body] WxaXPaySubmitSubscribePayOrderRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询签约关系（<c>POST /xpay/query_subscribe_contract</c>，不支持云调用，权限集 157、<b>官方明写无特殊错误码</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_subscribe_contract.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>product_id</c> / <c>out_contract_code</c> 三支全必填），见 <see cref="WxaXPayQuerySubscribeContractRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>签约状态字符串（三取值官方写在说明列），见 <see cref="WxaXPayQuerySubscribeContractResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；本端点<b>参数表没有 <c>env</c> 支</b>且官方明写「此接口没有特殊错误码」
    /// ⇒ 失败只可能是通用码（令牌/签名），签约关系不存在时返回的是状态值而不是错误。
    /// </para>
    /// <para>
    /// <b>三状态语义直接决定能否再扣款</b>：<c>SIGNED</c> 生效中、<c>TERMINATED</c> 已解约且<b>该协议号到达终态</b>
    /// （官方原文「该协议无法再次进行签约；可更换协议号再发起签约」）、<c>UNBINDUSER</c> 从未签约 ⇒
    /// 把 <c>TERMINATED</c> 当「可恢复」是错误用法，须换新 <c>out_contract_code</c>。
    /// </para>
    /// <para><b>官方无枚举表</b>（三值写在字段说明列）⇒ 按 XP5 口径不建常量类，取值照录进 DTO 注释。</para>
    /// </remarks>
    [Post("/xpay/query_subscribe_contract")]
    Task<WxaXPayQuerySubscribeContractResponse> QuerySubscribeContractAsync(
        [Body] WxaXPayQuerySubscribeContractRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家解约（<c>POST /xpay/cancel_subscribe_contract</c>，不支持云调用，<b>官方页面完全没有第三方调用段</b>、
    /// 因而<b>无权限集 id</b>，且官方明写无特殊错误码）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_cancel_subscribe_contract.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>termination_reason</c> / <c>product_id</c> / <c>out_contract_code</c> 四支全必填），见 <see cref="WxaXPayCancelSubscribeContractRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>本端点是全域 37 端点里唯一「无第三方调用段」的一支</b>：官方页面既没有「支持第三方平台代商家调用」小节，
    /// 也就<b>没有权限集 id</b> ⇒ 本 SDK 不宣称代商家调用可用，代调用方若需该能力须先取得官方口径。
    /// 该空缺是官方正文口径，<b>不得</b>凭第三方 SDK 注释推断成「同 157」。
    /// </para>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，请求体<b>没有 <c>env</c> 支</b>。
    /// </para>
    /// <para>
    /// <b><c>termination_reason</c> 是官方必填的审计字段</b> ⇒ 不得填空串；解约动作对用户的直接后果是后续扣款
    /// 被 <see cref="QuerySubscribeContractAsync"/> 读成 <c>TERMINATED</c>，且该协议号<b>不可复用</b>。
    /// </para>
    /// </remarks>
    [Post("/xpay/cancel_subscribe_contract")]
    Task<WxaResponse> CancelSubscribeContractAsync(
        [Body] WxaXPayCancelSubscribeContractRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);
}
