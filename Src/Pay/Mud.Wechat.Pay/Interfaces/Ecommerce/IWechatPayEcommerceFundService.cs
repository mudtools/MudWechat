// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 二级商户资金」域 SDK（<b>5 端点</b>：实时余额 + 日终余额 + 预约提现 + 两支提现单查询）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐页逐字段核验）：实时余额
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476690"/>、日终余额
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476693"/>、预约提现
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476652"/>、按 <c>withdraw_id</c> 查
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476665"/>、按 <c>out_request_no</c> 查
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476656"/>。五页「支持商户」一律是【平台商户】。</para>
/// <para><b>本域的身份是「平台商户（电商平台自身）」，不是服务商</b>：签名主体是电商平台的 <c>sp_mchid</c>，
/// 二级商户只以 <c>sub_mchid</c> 出现在 path/body ⇒ 与
/// <see cref="IWechatPayMerchantFundService"/>（【普通服务商】面、<c>account_type</c> 直接进 path 的两支余额）
/// 是<b>同族不同身份的两套契约</b>，路由前缀（<c>/v3/ecommerce/fund</c> 对 <c>/v3/merchant/fund</c>）与
/// 应答字段表都不同，不得互相套用 DTO。</para>
/// <para><b>两支余额共用一个应答类型、两支查询共用一个应答类型</b>（逐字段比对结论见
/// <see cref="EcommerceBalanceResponse"/> 与 <see cref="EcommerceWithdrawQueryResponse"/> 的类级注释）；
/// 差异全在请求侧：日终支多一个<b>必填</b> query <c>date</c>，两支查询的定位键一个在 path、<c>sub_mchid</c>
/// 恒定是<b>必填</b> query。</para>
/// <para><b>「受理 ≠ 到账」是本域最核心的陷阱</b>：预约提现的成功应答只有三字段、<b>没有状态</b> ⇒
/// 拿到 <see cref="EcommerceWithdrawApplyResponse"/> 不能记账，必须落库 <c>withdraw_id</c>
/// （或 <c>out_request_no</c>）后经查询支读 <c>status</c>（六值，见 <see cref="EcommerceWithdrawStatuses"/>）。
/// 官方另注明<b>「<c>SUCCESS</c> 不等于银行入账成功」</b>——部分情况会在约 7 个工作日内退票成 <c>REFUND</c>
/// ⇒ 资金结论只能取查询支的终态，且 <c>SUCCESS</c> 之后仍有一段会翻转的窗口。</para>
/// <para><b>「不得换单重试」是官方明文的资金安全约束</b>：<c>SYSTEM_ERROR</c>、回包解析报错、新增未知错误码
/// 三种情形官方一律要求<b>用原单（所有请求参数保持不变）重试</b>；而「相同 <c>out_request_no</c> + <c>sub_mchid</c>」
/// 只提供 <b>6 天</b>防重 ⇒ 换单等于对同一笔提现重复出款，超过 6 天换单更没有任何防重保护。</para>
/// <para><b>频率上限</b>：预约提现 <b>40 次/秒</b>（平台服务商维度），两支查询各 <b>100 次/秒</b>；
/// 两支余额页页内无端点级频率数字。超限回 429 <c>FREQUENCY_LIMITED</c>。</para>
/// <para><b>业务限制</b>：单笔预约提现金额上限「不能超过 8 亿」（单位分 ⇒ 以 <c>long</c> 承载）；
/// <c>out_request_no</c> 限 32 且<b>仅字母数字</b>；查询支只可查 <b>90 日内</b>发起的提现单；
/// 日终余额只可查 <b>90 天内</b>、且当日数据<b>次日 10:00 后</b>才生成；提现发起后<b>次日</b>才实际出款。</para>
/// <para><b>零 <c>[Token]</c>、零加密字段</b>：五页的 Header 参数表都只有 <c>Authorization</c> 与
/// <c>Accept</c>，未把 <c>Wechatpay-Serial</c> 列为参数 ⇒ 本域无需密文（守卫 EC-F5 锁定该核对结论）。
/// <c>account_number</c> 在查询应答里<b>只有后四位</b>（string(4)），仅够展示与人工核对，不是账号指纹，
/// 也不得据此写日志之外的资金判断。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>，备域名 <c>api2</c> 需宿主自行处理）；商户作用域经
/// <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，签名主体即当前作用域的平台商户号。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommerceFundService
{
    /// <summary>
    /// 「查询二级商户账户实时余额」：按二级商户号取可用与不可用金额（单位<b>分</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476690"/>。
    /// </summary>
    /// <param name="subMchId">二级商户号（官方 path <c>sub_mchid</c>，必填 string(32)）：由微信支付生成并下发。</param>
    /// <param name="accountType">账户类型（官方 query <c>account_type</c>，<b>选填</b>）：取值见 <see cref="EcommerceAccountTypes"/>，缺省语义见 Remarks。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>二级商户余额，见 <see cref="EcommerceBalanceResponse"/>（与日终支<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/fund/balance/{sub_mchid}</c>；<c>account_type</c> 是<b>选填 query</b>。</para>
    /// <para><b><c>account_type</c> 的取值口径按页分列</b>：本页请求侧列四值（<c>BASIC</c>/<c>FEES</c>/
    /// <c>OPERATION</c>/<c>DEPOSIT</c>），而<b>提现支只接受三值</b>（无 <c>DEPOSIT</c>）⇒ 保证金查得到、提不出。</para>
    /// <para><b>应答的 <c>pending_amount</c> 与 <c>account_type</c> 官方标选填</b> ⇒ 字段缺省不等于 0 / 空串。</para>
    /// </remarks>
    [Get("/v3/ecommerce/fund/balance/{subMchId}")]
    Task<EcommerceBalanceResponse> QueryBalanceAsync(
        [Path] string subMchId,
        [Query("account_type")] string? accountType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询二级商户账户日终余额」：取指定日期当天 24 点的账面余额，与实时余额<b>同一应答类型</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476693"/>。
    /// </summary>
    /// <param name="subMchId">二级商户号（官方 path <c>sub_mchid</c>，必填 string(32)）。</param>
    /// <param name="date">日期（官方 query <c>date</c>，<b>必填</b> string(10)）：格式 <c>YYYY-MM-DD</c>，只可查 90 天内。</param>
    /// <param name="accountType">账户类型（官方 query <c>account_type</c>，<b>选填</b>）：本页应答侧只列 <c>BASIC</c>/<c>DEPOSIT</c> 两值。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与实时余额<b>同一应答类型</b>，见 <see cref="EcommerceBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/fund/enddaybalance/{sub_mchid}?date=YYYY-MM-DD</c>。</para>
    /// <para><b><c>date</c> 是必填 query</b>：漏传即 400 <c>PARAM_ERROR</c>。<b>当日日终余额次日才生成</b>，
    /// 官方建议上午 10 点后查询 ⇒ 查当天拿不到数。</para>
    /// <para><b>日终余额是结算后的账面数</b>，与实时余额的差属时间口径差，不是资金异常。</para>
    /// </remarks>
    [Get("/v3/ecommerce/fund/enddaybalance/{subMchId}")]
    Task<EcommerceBalanceResponse> QueryDayEndBalanceAsync(
        [Path] string subMchId,
        [Query("date")] string date,
        [Query("account_type")] string? accountType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「申请二级商户预约提现」：代二级商户发起账户预约提现（<b>受理应答没有状态</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476652"/>。
    /// </summary>
    /// <param name="request">提现请求，见 <see cref="EcommerceWithdrawRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>sub_mchid</c> / <c>withdraw_id</c> / <c>out_request_no</c> 三字段，见 <see cref="EcommerceWithdrawApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/fund/withdraw</c>；限频 <b>40 次/秒</b>。</para>
    /// <para><b>本支是「次日发起、当日到账」的预约单</b>，不是即时出款：提交后系统<b>次日</b>才发起提现。</para>
    /// <para><b>失败重试口径</b>：<c>SYSTEM_ERROR</c> 与回包解析异常<b>不得换单</b>，须以原单（全部参数不变）重试；
    /// 防重窗口只有 <b>6 天</b>（按 <c>out_request_no</c> + <c>sub_mchid</c>）⇒ 换单重试等于重复出款。</para>
    /// <para><b>金额上限 8 亿（分）</b>⇒ 越出 int32；<c>account_type</c> 三值（无 <c>DEPOSIT</c>）；
    /// <c>notify_url</c> 必须 HTTPS 且不携带参数。未配回调时须自行轮询查询支闭环。</para>
    /// </remarks>
    [Post("/v3/ecommerce/fund/withdraw")]
    Task<EcommerceWithdrawApplyResponse> ApplyWithdrawAsync(
        [Body] EcommerceWithdrawRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按微信支付提现单号查询预约提现状态」：读提现单终态（六值）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476665"/>。
    /// </summary>
    /// <param name="withdrawId">微信支付预约提现单号（官方 path <c>withdraw_id</c>，必填 string(128)）。</param>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>提现单详情，见 <see cref="EcommerceWithdrawQueryResponse"/>（与按单号查<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/fund/withdraw/{withdraw_id}?sub_mchid=…</c>；限频 <b>100 次/秒</b>。</para>
    /// <para><b><c>sub_mchid</c> 是必填 query</b>：官方把它放在 Query 参数表而非 Path ⇒ 漏传即参数错误。</para>
    /// <para><b>只可查 90 日内</b>发起的提现单（以微信侧创建时间为准）。<c>INIT</c> 态须用<b>原单</b>重试发起，
    /// <c>CREATE_SUCCESS</c> 态表示已受理等待银行结果 ⇒ 两者都不是失败，不得据此判负。</para>
    /// </remarks>
    [Get("/v3/ecommerce/fund/withdraw/{withdrawId}")]
    Task<EcommerceWithdrawQueryResponse> QueryWithdrawByWithdrawIdAsync(
        [Path] string withdrawId,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按商户预约提现单号查询预约提现状态」：与上一支<b>同一应答类型</b>，只是定位键换成商户侧单号。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476656"/>。
    /// </summary>
    /// <param name="outRequestNo">商户预约提现单号（官方 path <c>out_request_no</c>，必填 string(32)，仅字母数字）。</param>
    /// <param name="subMchId">二级商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按 <c>withdraw_id</c> 查<b>同一应答类型</b>，见 <see cref="EcommerceWithdrawQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/fund/withdraw/out-request-no/{out_request_no}?sub_mchid=…</c>；
    /// 限频 <b>100 次/秒</b>。官方明说两支查询「返回结果一致」⇒ 本支是宿主只有自有单号时的等价入口。</para>
    /// <para><b><c>SUCCESS</c> 之后仍可能翻转为 <c>REFUND</c></b>（银行约 7 个工作日内处理退票，资金自动退回原账户）
    /// ⇒ 财务对账要以「超过退票窗口后的状态」为准，不能把 <c>SUCCESS</c> 当最终态立即结算。</para>
    /// </remarks>
    [Get("/v3/ecommerce/fund/withdraw/out-request-no/{outRequestNo}")]
    Task<EcommerceWithdrawQueryResponse> QueryWithdrawByOutRequestNoAsync(
        [Path] string outRequestNo,
        [Query("sub_mchid")] string subMchId,
        CancellationToken cancellationToken = default);
}
