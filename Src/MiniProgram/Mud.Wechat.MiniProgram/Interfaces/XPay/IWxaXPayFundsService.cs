// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「虚拟支付 · 广告金与提现」域 SDK（10 端点：绑定/查询广告金充值账户、充值广告金、
/// 查充值记录、查发放记录、查回收记录、下载广告金对应商户订单、查商家可提现余额、创建提现单、查询提现单）。
/// </summary>
/// <remarks>
/// <para>
/// 域统一形态（POST only、不支持云调用、权限集 157、<c>pay_sig</c> 落 Query 的凭据处置、敏感值口径、
/// 错误码表三分与 <c>env</c> 的三类语义）见 <see cref="IWxaXPayCurrencyService"/>，本文件只写本族特有事实。
/// </para>
/// <para>
/// <b>本族是「签名面」分裂最严重的一章</b>：10 端点里只有 <see cref="QueryBizBalanceAsync"/>、
/// <see cref="CreateWithdrawOrderAsync"/>、<see cref="QueryWithdrawOrderAsync"/> 三支要求 <c>pay_sig</c>，
/// 其余 7 支（广告金侧全部）Query 只有 <c>access_token</c> ⇒ 该分界是官方逐页正文的差异，
/// <b>不是</b> SDK 漏参，由守卫 XP2 逐端点锁定。
/// </para>
/// <para>
/// <b>官方本域 37 页均无端点级频率数字</b>：只有错误码 <c>268490015</c>「频率限制」与订阅侧 <c>-15020</c>
/// 「操作过快，请稍候再试」证明确有限频存在，但<b>没有任何一页给出次/分钟量级</b>
/// ⇒ 注释与守卫一律写「官方正文未给端点级频率数字」，<b>不得</b>凭第三方 SDK 注释或记忆补数。
/// </para>
/// <para>
/// <b>资金方向的三步链路是官方必填面强制的</b>：绑定账户（<see cref="BindTransferAccountAsync"/>）→
/// 用发放记录的结算周期定位并充值（<see cref="QueryAdverFundsAsync"/> → <see cref="CreateFundsBillAsync"/>）→
/// 余额与提现（<see cref="QueryBizBalanceAsync"/> → <see cref="CreateWithdrawOrderAsync"/>）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "XPay", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaXPayFundsService
{
    /// <summary>
    /// 绑定广告金充值账户（<c>POST /xpay/bind_transfer_accout</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_bind_transfer_accout.html"/>。
    /// </summary>
    /// <param name="request">请求体（三支官方全部非必填），见 <see cref="WxaXPayBindTransferAccountRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>，绑定结果须走 <see cref="QueryTransferAccountAsync"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>路由拼写官方原文是 <c>bind_transfer_accout</c>（缺字母 n）</b> ⇒ 路由照官方原文，
    /// 方法名用正确拼写 <see cref="BindTransferAccountAsync"/>；该拼写由守卫 XP1 逐字锁定，<b>不得</b>「顺手修正」路由。
    /// </para>
    /// <para>
    /// 官方契约：本端点 Query 只有 <c>access_token</c>，且三支字段<b>全部标注非必填</b>（本域唯一）⇒
    /// 「至少定位一个账户」是正文语义、表格不承载，SDK 不本地校验。
    /// </para>
    /// <para>
    /// <b>本页最常见的三类失败都有专属码</b>：<c>268490018</c> 行业 id 不匹配、<c>268490019</c> 账户已绑定其他
    /// appid、<c>268490020</c> 主体名称错误 ⇒ 绑定不是幂等操作，重复绑定同账户会撞 19。
    /// </para>
    /// </remarks>
    [Post("/xpay/bind_transfer_accout")]
    Task<WxaResponse> BindTransferAccountAsync(
        [Body] WxaXPayBindTransferAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询广告金充值账户（<c>POST /xpay/query_transfer_account</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_transfer_account.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>env</c> 一支必填，且其官方语义是「仅作签名校验」），见 <see cref="WxaXPayQueryTransferAccountRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账户列表（<c>acct_list</c>），见 <see cref="WxaXPayQueryTransferAccountResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query 只有 <c>access_token</c>；请求单字段 ⇒ <b>本查询无分页、无筛选</b>，
    /// 返回的是当前 appid 下的账户全集。
    /// </para>
    /// <para>
    /// <b><c>state</c>（审核 0 待审 / 1 通过 / 2 驳回）与 <c>bind_result</c>（绑定 1 成功 / 2 失败）是两段独立状态，
    /// 且后者从 1 开始编号</b> ⇒ 把 0 读成「绑定失败」会误判未审核的中间态。
    /// </para>
    /// <para><b>本端点是充值的前置</b>：<c>create_funds_bill</c> 的 <c>transfer_account_uid</c> /
    /// <c>transfer_account_name</c> / <c>transfer_account_agency_id</c> 三支必填值都来自本页 ⇒ 不得凭本地记录拼。</para>
    /// </remarks>
    [Post("/xpay/query_transfer_account")]
    Task<WxaXPayQueryTransferAccountResponse> QueryTransferAccountAsync(
        [Body] WxaXPayQueryTransferAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 充值广告金（<c>POST /xpay/create_funds_bill</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_create_funds_bill.html"/>。
    /// </summary>
    /// <param name="request">请求体（十支全部必填，<c>request_id</c> 是幂等键），见 <see cref="WxaXPayCreateFundsBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>充值单 ID（<c>bill_id</c>），见 <see cref="WxaXPayCreateFundsBillResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query 只有 <c>access_token</c>；<b>十支全部必填是本域请求面最严的一页</b>。
    /// </para>
    /// <para>
    /// <b><c>request_id</c> 是官方幂等键</b>（原文「相同 id 的不同请求视为重复请求」，上限 1024 字符）⇒
    /// 重复请求返回 <c>268490004</c>，语义是「此前已成功」而非失败，<b>不得</b>据此换 id 重放（会重复充值）。
    /// </para>
    /// <para>
    /// <b>充值定位的是发放记录而不是余额</b>：<c>settle_begin</c>/<c>settle_end</c> 必须是某条发放记录的结算周期
    /// （由 <see cref="QueryAdverFundsAsync"/> 取得），且 <c>fund_type</c> 与 <c>authorize_advertise</c>
    /// 两支官方要求显式赋值 ⇒ 金额不足撞 <c>268490023</c>、零金额撞 <c>268490024</c>、账户未进件撞 <c>268490021</c>。
    /// </para>
    /// <para><b>敏感值</b>：充值金额与账户信息 ⇒ 不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/xpay/create_funds_bill")]
    Task<WxaXPayCreateFundsBillResponse> CreateFundsBillAsync(
        [Body] WxaXPayCreateFundsBillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询广告金充值记录（<c>POST /xpay/query_funds_bill</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_funds_bill.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page</c> / <c>page_size</c> / <c>filter</c> / <c>env</c> 四支全必填），见 <see cref="WxaXPayQueryFundsBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐充值单 + 总页数，见 <see cref="WxaXPayQueryFundsBillResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query 只有 <c>access_token</c>；<b>分页与筛选官方全部必填</b>（与本族
    /// <see cref="QueryAdverFundsAsync"/> 的「四支全非必填」相反），且 <c>filter</c> 内时间窗两支亦必填 ⇒
    /// 不传时间窗无法查询。
    /// </para>
    /// <para>
    /// <b><c>env</c> 在此只影响签名校验</b>（官方原文「仅作为签名校验，查询的结果都是正式环境的」）⇒
    /// 传 1 不会读到沙箱记录。
    /// </para>
    /// <para><b>结果里的 <c>request_id</c> 原样回显充值时传入值</b> ⇒ 幂等核对应以它而不是 <c>bill_id</c> 为键。</para>
    /// </remarks>
    [Post("/xpay/query_funds_bill")]
    Task<WxaXPayQueryFundsBillResponse> QueryFundsBillAsync(
        [Body] WxaXPayQueryFundsBillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询广告金发放记录（<c>POST /xpay/query_adver_funds</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_adver_funds.html"/>。
    /// </summary>
    /// <param name="request">请求体（四支官方全部非必填），见 <see cref="WxaXPayQueryAdverFundsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐发放记录 + 总页数（<c>adver_funds_list</c> / <c>total_page</c>），见 <see cref="WxaXPayQueryAdverFundsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query 只有 <c>access_token</c>；本端点是本域「全字段选填」的查询端点 ⇒
    /// 可无条件全量翻页。
    /// </para>
    /// <para>
    /// <b><c>fund_type</c> 决定后续可用性</b>（0 通用赠送 / 1 广告激励 / 2 定向激励）：<see cref="DownloadAdverfundsOrderAsync"/>
    /// 官方原文<b>仅支持通用赠送（0）</b> ⇒ 用 1/2 的 <c>fund_id</c> 去下载订单必然拿不到结果。
    /// </para>
    /// <para><b><c>remain_amount</c> 与 <c>expire_time</c> 须同时读</b>：广告金有过期时刻，只看余额会在过期日突然失血。</para>
    /// </remarks>
    [Post("/xpay/query_adver_funds")]
    Task<WxaXPayQueryAdverFundsResponse> QueryAdverFundsAsync(
        [Body] WxaXPayQueryAdverFundsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询广告金回收记录（<c>POST /xpay/query_recover_bill</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_recover_bill.html"/>。
    /// </summary>
    /// <param name="request">请求体（四支全必填，<c>filter.bill_id</c> 亦必填），见 <see cref="WxaXPayQueryRecoverBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐回收单 + 总页数，见 <see cref="WxaXPayQueryRecoverBillResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query 只有 <c>access_token</c>。
    /// </para>
    /// <para>
    /// <b>本端点 <c>filter.bill_id</c> 官方标注必填</b>（充值记录侧为选填）⇒ 两支筛选形状不同，
    /// SDK 已分立为 <see cref="WxaXPayRecoverBillFilter"/> 与 <see cref="WxaXPayFundsBillFilter"/>，
    /// <b>不得</b>共用（守卫 XP5 锁定）。
    /// </para>
    /// <para>
    /// <b>回收是官方主动动作，不是商户可选项</b>：结果里的 <c>refund_order_list</c>（对应的退款订单 id 列表）说明
    /// 回收由退款触发 ⇒ 营收预测须把本查询纳入对账，只看充值记录会高估可用广告金。
    /// </para>
    /// <para><b>同名不同形</b>：本表与充值记录侧的子表官方都叫 <c>bill_list</c> 而字段集几乎不交 ⇒ 判据以类型为准，不以键名为准。</para>
    /// </remarks>
    [Post("/xpay/query_recover_bill")]
    Task<WxaXPayQueryRecoverBillResponse> QueryRecoverBillAsync(
        [Body] WxaXPayQueryRecoverBillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载广告金对应的商户订单信息（<c>POST /xpay/download_adverfunds_order</c>，不支持云调用，权限集 157、<b>无签名 Query 参数</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_adverfunds_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>fund_id</c> / <c>env</c> 必填），见 <see cref="WxaXPayDownloadAdverfundsOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>下载链接（与「下载小程序账单」共用应答形态），见 <see cref="WxaXPayDownloadUrlResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，Query 只有 <c>access_token</c>（本族广告金侧全部不要求签名）。
    /// </para>
    /// <para>
    /// <b>本端点是「生成 + 轮询」两段式而非同步取链接</b>：官方注意事项原文「第一次调用触发生成下载url，
    /// 可以间隔轮训来获取最终生成的下载url」⇒ 首次调用拿到空链接是正常形态，
    /// 须轮询到链接非空而非据首次结果报错。该空缺没有官方延迟数字，<b>不得</b>臆造轮询间隔上限。
    /// </para>
    /// <para><b>官方明写「当前仅支持通用赠送广告金对应订单下载」</b>（<c>fund_type = 0</c>）⇒ 见 <see cref="QueryAdverFundsAsync"/>。</para>
    /// <para><b>敏感值</b>：链接本身是订单明细入口 ⇒ 不得进日志、遥测或异常消息，也不得持久化复用。</para>
    /// </remarks>
    [Post("/xpay/download_adverfunds_order")]
    Task<WxaXPayDownloadUrlResponse> DownloadAdverfundsOrderAsync(
        [Body] WxaXPayDownloadAdverfundsOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商家账户可提现余额（<c>POST /xpay/query_biz_balance</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_biz_balance.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>env</c> 一支必填），见 <see cref="WxaXPayQueryBizBalanceRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>余额袋（<c>balance_available</c>：金额 + 币种），见 <see cref="WxaXPayQueryBizBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c> —— <b>本端点是广告金侧与提现侧的分界</b>：从这一支起本族三支都要求签名，
    /// 因为它读的是可动资金。
    /// </para>
    /// <para>
    /// <b>金额单位是「元」且类型为 <c>string</c></b>（官方原文「单位元」）⇒ 与本域订单侧「分为单位的数字」相反，
    /// 换算须固定精度，禁止用浮点累加。
    /// </para>
    /// <para><b>敏感值</b>：商户账户余额 ⇒ 不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/xpay/query_biz_balance")]
    Task<WxaXPayQueryBizBalanceResponse> QueryBizBalanceAsync(
        [Body] WxaXPayQueryBizBalanceRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建提现单（<c>POST /xpay/create_withdraw_order</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_create_withdraw_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>withdraw_no</c> / <c>withdraw_amount</c> / <c>env</c>），见 <see cref="WxaXPayCreateWithdrawOrderRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商户侧与微信侧双单号，见 <see cref="WxaXPayCreateWithdrawOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；<b>「创建」只表示受理</b>，到账结果须走 <see cref="QueryWithdrawOrderAsync"/>。
    /// </para>
    /// <para>
    /// <b>本页官方自相矛盾已留档</b>：<c>withdraw_amount</c> 参数表标必填，说明列原文却写「允许不传，
    /// 不传的情况下表示全额提现」⇒ 本 SDK 取<b>参数表</b>形态（属性可空即允许省略），
    /// 该取舍由守卫 XP3 登记，<b>不得</b>当成「SDK 漏参」。
    /// </para>
    /// <para>
    /// <b>单号格式是硬约束</b>：长度 [8,32]、仅字母/数字/下划线/短横线（与订阅扣款单号同一规则）。
    /// SDK 不本地校验（照官方错误码 <c>268490002</c>）。
    /// </para>
    /// <para><b>敏感值</b>：提现金额与双单号 ⇒ 不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/xpay/create_withdraw_order")]
    Task<WxaXPayCreateWithdrawOrderResponse> CreateWithdrawOrderAsync(
        [Body] WxaXPayCreateWithdrawOrderRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询提现单（<c>POST /xpay/query_withdraw_order</c>，不支持云调用，权限集 157）。
    /// 文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_withdraw_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>withdraw_no</c> / <c>env</c> 必填），见 <see cref="WxaXPayQueryWithdrawOrderRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现单全貌（状态、金额、微信侧单号、成功时刻、失败原因），见 <see cref="WxaXPayQueryWithdrawOrderResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，以商户侧 <c>withdraw_no</c> 定位（不支持按微信侧单号反查）。
    /// </para>
    /// <para>
    /// <b>状态三值从 1 开始</b>（1 创建成功提现中 / 2 提现成功 / 3 提现失败，官方写在说明列、无枚举表）⇒
    /// <c>fail_reason</c> 仅在 3 时有意义，成功时刻 <c>withdraw_success_timestamp</c> 仅在 2 时有值。
    /// </para>
    /// <para>
    /// <b>三支金额/时间字段官方类型是 <c>string</c></b>（<c>withdraw_amount</c> 元、两支时间戳）⇒
    /// 线格式照官方落字符串，<b>不得</b>「顺手」改成数字或时间类型（守卫 XP5 锁定）。
    /// </para>
    /// </remarks>
    [Post("/xpay/query_withdraw_order")]
    Task<WxaXPayQueryWithdrawOrderResponse> QueryWithdrawOrderAsync(
        [Body] WxaXPayQueryWithdrawOrderRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);
}
