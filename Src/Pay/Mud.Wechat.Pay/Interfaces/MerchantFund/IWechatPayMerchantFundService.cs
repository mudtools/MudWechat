// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MerchantFund;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商户账户资金管理」域 SDK（<b>4 端点</b>：查询账户实时余额 + 查询账户日终余额 + 商户号维度收入记录 + 服务商维度收入记录）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-14 逐字段核验）：实时余额
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012721477"/>、日终余额
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012721446"/>。</para>
/// <para><b>本域是「前缀裁决收窄」的落点，不是新增面</b>：方案 §2.7 三把资金面的裁决守卫
/// 从「零 <c>/v3/merchant/fund</c> 路由」<b>收窄为「零 <c>/v3/merchant/fund/withdraw</c> 路由」</b>，
/// 理由是一条可机械检验的线——逐页取「支持商户」一栏，只有含【普通商户】/【普通服务商】/【渠道商】/
/// 【从业机构】/【清算机构】之一的页面才落进本包。<b>这两支余额页的「支持商户」正是【普通服务商】</b>
/// （同路由另有【平台商户】页 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012476700"/>，
/// 身份不同不影响路由与字段表），而提现族十一页全部只标【平台商户】⇒ 提现族保持零端点
/// （守卫 MF-B2 以刚落地的两支余额作<b>正对照</b>，防止正则漂移导致断言静默恒真）。</para>
/// <para><b>两支都是 <c>GET</c>、都无包体、应答表逐字段相同</b> ⇒ 共用
/// <see cref="MerchantFundBalanceResponse"/>（守卫 MF-B6）；差异只在日终支多一个<b>必填 query</b>
/// 参数 <c>date</c>（string(10)、<c>YYYY-MM-DD</c>）。</para>
/// <para><b><c>account_type</c> 落在 path 段</b>（不是 query、不是包体），取值见
/// <see cref="MerchantFundAccountTypes"/>（<c>BASIC</c> / <c>OPERATION</c> / <c>FEES</c> 三值）。
/// <b>账户未开通是 400 <c>INVALID_REQUEST</c></b>（官方文案「输入账户类型未开通」）而不是 404
/// ⇒ 按「路由不存在」重试不会收敛。</para>
/// <para><b>金额单位是分、以 <c>long</c> 承载</b>：官方类型是 integer，而账户余额量级可越出 int32
/// （约 21.47 亿分 = 2147 万元）⇒ 用 <c>int</c> 会在头部商户静默溢出（守卫 MF-B3）。</para>
/// <para><b>「余额」不是「可提现金额」</b>：可提头寸还要扣在途提现与冻结，而提现族属【平台商户】面
/// （本包零端点）⇒ 不得由本域应答推算可提金额，也不得据余额变动做资金对账结论（对账走
/// <see cref="IWechatPayBillService"/> 的资金账单）。</para>
/// <para><b>两支页内无端点级频率数字</b>（两轮复核一致）。<b>业务错误码</b>：400 <c>PARAM_ERROR</c> /
/// 400 <c>INVALID_REQUEST</c> / 403 <c>NO_AUTH</c> / 401 <c>SIGN_ERROR</c> / 500 <c>SYSTEM_ERROR</c>，
/// 原样透传至 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
/// <para><b>本章不含提现面</b>：申请提现 / 查询提现（两支）/ 撤销提现 / 提现改号 / 余额账户提现四支、
/// 预约提现与提现异常文件、注销后提现——共 13 行的现行页面「支持商户」<b>只标【平台商户】</b>，
/// 且依赖平台收付通的账户体系与「提现结果通知」回调族（只补 HTTP 支会给出一个回调无主、状态无法闭环的
/// 半成品）⇒ 由守卫 MF-B2 与 <c>WechatPayCapitalRulingContractGuards</c> C1 同批锁定，
/// 解除条件是正式引入「平台商户」形态（配置 + 分槽 + 回调族）后同批落地。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择；
/// 本域<b>零 <c>[Token]</c></b>（守卫 PAY-B1）、<b>无加密字段</b>（两页均未把 <c>Wechatpay-Serial</c> 列入参数表）。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantFund", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantFundService
{
    /// <summary>
    /// 「查询账户实时余额」：按账户类型查当前可用与不可用金额（单位<b>分</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012721477"/>。
    /// </summary>
    /// <param name="accountType">账户类型（官方 path <c>account_type</c>，必填 string）：取值见 <see cref="MerchantFundAccountTypes"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>可用与不可用金额，见 <see cref="MerchantFundBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/merchant/fund/balance/{account_type}</c>；无 query 参数、无包体。</para>
    /// <para><b>支持商户【普通服务商】</b>：同路由在【平台商户】面另有页面，本包按服务商面承载。</para>
    /// <para><b><c>account_type</c> 未开通回 400 <c>INVALID_REQUEST</c></b>（「输入账户类型未开通」）⇒
    /// 不是 404、也不是空应答；宿主不要把「没这个账户」当瞬时故障重试。</para>
    /// </remarks>
    [Get("/v3/merchant/fund/balance/{accountType}")]
    Task<MerchantFundBalanceResponse> QueryBalanceAsync(
        [Path] string accountType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询账户日终余额」：与实时余额<b>同一应答类型</b>，额外按 <c>date</c>（<c>YYYY-MM-DD</c>）取历史某日。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012721446"/>。
    /// </summary>
    /// <param name="accountType">账户类型（官方 path <c>account_type</c>，必填 string）：取值见 <see cref="MerchantFundAccountTypes"/>。</param>
    /// <param name="date">日期（官方 query <c>date</c>，<b>必填</b> string(10)）：格式 <c>YYYY-MM-DD</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与实时余额<b>同一应答类型</b>，见 <see cref="MerchantFundBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/merchant/fund/dayendbalance/{account_type}?date=YYYY-MM-DD</c>。</para>
    /// <para><b><c>date</c> 是必填 query</b>：本域唯一带 query 的一支 ⇒ 不传即 400 <c>PARAM_ERROR</c>；
    /// 官方示例形态是 <c>/dayendbalance/BASIC?date=2019-08-17</c>（账户类型直接进路径）。</para>
    /// <para><b>日终余额是结算后的账面数</b>，与实时余额的差属时间口径差，不是资金异常。</para>
    /// </remarks>
    [Get("/v3/merchant/fund/dayendbalance/{accountType}")]
    Task<MerchantFundBalanceResponse> QueryDayEndBalanceAsync(
        [Path] string accountType,
        [Query("date")] string date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询商户账户资金收入记录」（<b>商户号维度</b>）：按账户类型与日期分页拉取入账记录。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012718647"/>。
    /// </summary>
    /// <param name="accountType">账户类型（官方 query <c>account_type</c>，必填 string(32)）：取值见 <see cref="MerchantFundAccountTypes"/>。</param>
    /// <param name="date">入账日期（官方 query <c>date</c>，<b>必填</b> string(10)）：格式 <c>YYYY-MM-DD</c>，按成功时间归日。</param>
    /// <param name="limit">本页条数（官方 query <c>limit</c>，<b>必填</b> integer）：官方页未印上限，按服务端回 400 处理越界。</param>
    /// <param name="offset">偏移量（官方 query <c>offset</c>，选填 integer）：首页传 0 或不传。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>收入记录分页结果，见 <see cref="MerchantFundIncomeRecordQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/merchantfund/merchant/income-records</c>；
    /// query 参数 <c>account_type</c> / <c>date</c> / <c>offset</c> / <c>limit</c>，无包体。</para>
    /// <para><b>支持商户【普通服务商】</b>：与余额面同属商户账户资金管理域，客户端的商户作用域同上。</para>
    /// <para><b>应答含银行账号/户名</b>（<c>bank_account_number</c> / <c>bank_account_name</c>）⇒
    /// 属个人金融信息，宿主<b>不得</b>写入日志、遥测或异常消息。</para>
    /// <para><b>分页语义</b>：<c>total_count</c> 是筛选条件下总数，<c>data</c> 只含本页；
    /// <c>links.next</c> 非空才可继续翻页，勿以「本页条数 = limit」自行推断。</para>
    /// </remarks>
    [Get("/v3/merchantfund/merchant/income-records")]
    Task<MerchantFundIncomeRecordQueryResponse> QueryMerchantIncomeRecordsAsync(
        [Query("account_type")] string accountType,
        [Query("date")] string date,
        [Query("limit")] int limit,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询服务商账户资金收入记录」（<b>子商户维度</b>）：与商户号维度同表，仅 <c>data[].sub_mchid</c> 差异。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012718620"/>。
    /// </summary>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="accountType">账户类型（官方 query <c>account_type</c>，必填 string(32)）：取值见 <see cref="MerchantFundAccountTypes"/>。</param>
    /// <param name="date">入账日期（官方 query <c>date</c>，<b>必填</b> string(10)）：格式 <c>YYYY-MM-DD</c>。</param>
    /// <param name="limit">本页条数（官方 query <c>limit</c>，<b>必填</b> integer）。</param>
    /// <param name="offset">偏移量（官方 query <c>offset</c>，选填 integer）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>子商户维度的收入记录分页结果，见 <see cref="PartnerIncomeRecordQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/merchantfund/partner/income-records</c>；
    /// query 参数 <c>sub_mchid</c> / <c>account_type</c> / <c>date</c> / <c>offset</c> / <c>limit</c>，无包体。</para>
    /// <para><b>返回类型为何与商户号维度分开</b>：两页应答<b>唯一差异</b>是 <c>data[]</c> 首字段
    /// （本页 <c>sub_mchid</c> vs 商户页 <c>mchid</c>）⇒ 按「表不同则分建」独立建模，防字段串用。</para>
    /// <para><b>应答含银行账号/户名</b> ⇒ 属个人金融信息，不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Get("/v3/merchantfund/partner/income-records")]
    Task<PartnerIncomeRecordQueryResponse> QueryPartnerIncomeRecordsAsync(
        [Query("sub_mchid")] string subMchId,
        [Query("account_type")] string accountType,
        [Query("date")] string date,
        [Query("limit")] int limit,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);
}
