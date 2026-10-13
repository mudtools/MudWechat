// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「虚拟支付 · 代币操作」域 SDK（4 端点：扣减代币 / 代币支付退款 / 代币赠送 / 查询代币余额）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成口径（2026-10-13 依官方 VirtualPayment 分区逐页取证）</b>：官方把 37 个 <c>/xpay/*</c> 端点编成
/// 代币、订单与账单、道具、会员订阅、广告金与提现、支付投诉六个章节，本线按章节分六支接口、
/// 共用注册组 <c>XPay</c> 与 DTO 命名空间 <c>Mud.Wechat.MiniProgram.DataModels.XPay</c>（守卫 XP1~XP6）。
/// </para>
/// <para>
/// <b>全域统一形态</b>：官方 37 端点<b>全部是 POST</b>（无 GET），<b>全部不支持云调用</b>；
/// 除「商家解约」一页<b>完全没有第三方调用段</b>外，其余 36 端点均声明「支持第三方平台代商家调用」
/// 且权限集 id 为 <b>157</b>，<c>access_token</c> 位可用 <c>authorizer_access_token</c> 代调用。
/// 适用范围段全域统一为「本接口暂未明确可调用账号类型，或在业务中根据调用传参自行确定是否可调用」
/// ⇒ <b>该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数</b>。
/// </para>
/// <para>
/// <b>令牌与签名注入（MP-X2 / MP-X3 同口径 + 本域特有）</b>：令牌复用 <c>MpTokenTypes.AccessToken</c> +
/// Query 注入 <c>access_token</c>；本域在其之外<b>还强制 Query 支付签名</b>，官方调用示例逐字为
/// <c>?access_token=ACCESS_TOKEN&amp;pay_sig=PAY_SIG</c>（双签名端点为 <c>&amp;signature=SIGNATURE&amp;pay_sig=PAY_SIG</c>）
/// ⇒ <c>pay_sig</c> / <c>signature</c> 是<b>方法级显式参数</b>而非 <c>[Token]</c> 注入，签名算法与
/// <c>session_key</c> 相关，属宿主职责，SDK 不代签。
/// </para>
/// <para>
/// <b><c>pay_sig</c> 的凭据处置（AGENTS §8「二选一」）</b>：该参数名不在组件
/// <c>SensitiveUrlRedactor</c> 静态词表内（词表有 <c>sig</c>/<c>sign</c>/<c>signature</c>，是<b>精确匹配</b>语义、
/// 覆盖不到 <c>pay_sig</c>），而组件登记门面是公开 API ⇒ 本模块注册期经
/// <c>SensitiveUrlKeys.Register("pay_sig")</c> 登记为进程级强制掩码键，形态与企微线群机器人
/// <c>key</c>（守卫 WEB3）、广告线 <c>user_token</c>（守卫 ADS-B5）一致，由守卫 XP6 锁定「登记表 ⊆ 词表补集」。
/// </para>
/// <para>
/// <b>敏感值处置（全域）</b>：代币余额、商户可提现余额与提现金额、账单/订单下载链接、<c>cos_url</c>（官方明示
/// 30 分钟有效）、<c>sign</c>、<c>pay_sig</c>、<c>signature</c>、投诉人手机号与 openid、订单 <c>token</c>
/// ⇒ <b>严禁</b>写进日志、遥测或异常消息（与 <c>permanent_code</c> 同级处置）。
/// </para>
/// <para>
/// <b>错误码面三分（逐页取证，不得合并口径）</b>：官方 <b>28</b> 端点共享同一张 <c>26849_0xx</c> 域内错误码表
/// （另有 <c>-1</c> 系统错误）；会员订阅的两支<b>各有独立码表</b> —— 预通知扣款是 <c>674690001</c> /
/// <c>690000000</c> / <c>690000001</c>，发起订阅扣款是米大师侧 <c>-15001 ~ -15027</c> 加 <c>1001</c>；
/// 余 <b>7</b> 端点（商家解约、通知已发货完成、下载 iOS 月结账单、查询下载订单任务、查询签约关系、商户被管控原因查询、
/// 下载支付订单）官方明写「此接口没有特殊错误码」⇒ 只有通用错误码，不得臆造域内码。
/// </para>
/// <para>
/// <b><c>env</c> 有三类官方语义，不可当成同一个「环境开关」</b>：<b>17</b> 端点的 <c>env</c> 原文标注
/// 「仅作为签名校验，查询的结果都是正式环境的」或「用于基类签名校验」（含本域绑定/充值/查询广告金与全部投诉端点、
/// 账单任务两支）⇒ 传沙箱值不改变查询结果；<b>13</b> 端点是真正的「0 正式 / 1 沙箱」数据面语义；
/// 发起订阅扣款虽标 0/1 却官方明写<b>仅支持 0</b>（错误码 <c>-15011</c>）；余 <b>6</b> 端点
/// 参数表里<b>根本没有 <c>env</c> 支</b>（商家解约、查询签约关系、预通知扣款、商户被管控原因查询、
/// 下载小程序账单、下载 iOS 月结账单）⇒ <b>不得</b>给它们造出环境入参。逐端点归类由守卫 XP3 锁定。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "XPay", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaXPayCurrencyService
{
    /// <summary>
    /// 扣减代币（<c>POST /xpay/currency_pay</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_currency_pay.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaXPayCurrencyPayRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="signature">用户态签名（官方 Query 参数 <c>signature</c>，必填，由宿主以 <c>session_key</c> 计算）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>扣减后余额与本次消耗的赠送部分，见 <see cref="WxaXPayCurrencyPayResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体，Query 同时带 <c>access_token</c> / <c>signature</c> / <c>pay_sig</c>
    /// 三支（本域仅 <c>currency_pay</c>、<c>cancel_currency_pay</c>、<c>query_user_balance</c> 三支是双签名形态）。
    /// </para>
    /// <para>
    /// <b>幂等判定</b>：<c>268490004</c> 官方原文表示「之前的操作已经成功」⇒ 该码须按成功处理，
    /// 换单号重试会造成重复扣减。
    /// </para>
    /// <para><b>本端点不产生支付单营收</b>：代币是用户已充值余额的扣减，退款语义在 <see cref="CancelCurrencyPayAsync"/>。</para>
    /// </remarks>
    [Post("/xpay/currency_pay")]
    Task<WxaXPayCurrencyPayResponse> CurrencyPayAsync(
        [Body] WxaXPayCurrencyPayRequest request,
        [Query("pay_sig")] string paySig,
        [Query("signature")] string signature,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代币支付退款（<c>POST /xpay/cancel_currency_pay</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_cancel_currency_pay.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>pay_order_id</c> 是原支付单号、<c>order_id</c> 是本次退款单号），见 <see cref="WxaXPayCancelCurrencyPayRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="signature">用户态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>退款订单号，见 <see cref="WxaXPayCancelCurrencyPayResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 双签名 Query（<c>signature</c> + <c>pay_sig</c>），六支请求字段官方全部标注必填。</para>
    /// <para>
    /// <b>与订单退款的分工</b>：本端点退的是<b>代币扣减</b>，真金白银的支付单退款走
    /// <c>IWxaXPayOrderService.RefundOrderAsync</c>；官方对已走后者退款的单回 <c>268490005</c>
    /// （「订单已经通过cancel_currency_pay接口退款，不支持再退款」）⇒ 两条退款线互斥，不可双退。
    /// </para>
    /// <para><b><c>268490006</c>（金额不足）</b>是余额校验失败而非系统错误 ⇒ 重试前须先核对 <c>amount</c> 与原单金额。</para>
    /// </remarks>
    [Post("/xpay/cancel_currency_pay")]
    Task<WxaXPayCancelCurrencyPayResponse> CancelCurrencyPayAsync(
        [Body] WxaXPayCancelCurrencyPayRequest request,
        [Query("pay_sig")] string paySig,
        [Query("signature")] string signature,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代币赠送（<c>POST /xpay/present_currency</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_present_currency.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaXPayPresentCurrencyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>赠送后余额与累计赠送额，见 <see cref="WxaXPayPresentCurrencyResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query <b>只有 <c>access_token</c></b>（官方调用示例
    /// <c>?access_token=ACCESS_TOKEN</c>，本域九支无签名端点之一）⇒ <b>不得</b>为本端点索要 <c>pay_sig</c>，
    /// 守卫 XP3 锁定「有签名 / 无签名」两族清单。
    /// </para>
    /// <para><b>赠送走无价账户</b>：<c>present_balance</c> 与有价余额分账，官方扣减顺序是先赠送后有价（见
    /// <see cref="WxaXPayCurrencyPayResponse"/>）⇒ 赠送不可计入营收口径。</para>
    /// <para><b><c>268490004</c> 同样表示「已赠送成功」</b>（官方原文把赠送列入该码适用范围）。</para>
    /// </remarks>
    [Post("/xpay/present_currency")]
    Task<WxaXPayPresentCurrencyResponse> PresentCurrencyAsync(
        [Body] WxaXPayPresentCurrencyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询代币余额（<c>POST /xpay/query_user_balance</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_user_balance.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>env</c> / <c>user_ip</c> 三支全部必填），见 <see cref="WxaXPayQueryUserBalanceRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="signature">用户态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>有价/赠送两账户余额与四项累计量，见 <see cref="WxaXPayQueryUserBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 双签名 Query。官方注意事项给出的签名拼接示例正是本页
    /// （<c>https://api.weixin.qq.com/xpay/query_user_balance?access_token=xxxx&amp;pay_sig=xxx</c>）
    /// ⇒ <c>pay_sig</c> 落 Query 是官方明示而非推断。
    /// </para>
    /// <para><b>本端点是「用户态」余额</b>（按 <c>openid</c> 定位）；商户侧可提现余额走
    /// <c>IWxaXPayFundsService.QueryBizBalanceAsync</c>，两者单位与币种口径都不同，不可互替。</para>
    /// <para><b>敏感值</b>：响应全部字段是用户资产数据 ⇒ 不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/xpay/query_user_balance")]
    Task<WxaXPayQueryUserBalanceResponse> QueryUserBalanceAsync(
        [Body] WxaXPayQueryUserBalanceRequest request,
        [Query("pay_sig")] string paySig,
        [Query("signature")] string signature,
        CancellationToken cancellationToken = default);
}
