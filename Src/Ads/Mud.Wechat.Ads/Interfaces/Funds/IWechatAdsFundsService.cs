// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.Funds;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「资金账户」域 SDK（<c>funds</c> / <c>fund_transfer</c> /
/// <c>fund_statements_detailed</c> / <c>realtime_cost</c> 四支资源族共 4 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-17）：查询客户保证金信息
/// <see href="https://developers.e.qq.com/v3.0/docs/api/funds/get"/>、发起转账
/// <see href="https://developers.e.qq.com/v3.0/docs/api/fund_transfer/add"/>、
/// 查询账户资金流水明细
/// <see href="https://developers.e.qq.com/v3.0/docs/api/fund_statements_detailed/get"/>、
/// 获取账号实时消耗 <see href="https://developers.e.qq.com/v3.0/docs/api/realtime_cost/get"/>。</para>
/// <para><b>所属权限</b>（四页「所属权限」均为官方原文 <c>account_management</c>）。
/// 权限不足由应答 <c>code</c> 表达，SDK 不做本地权限预判。</para>
/// <para><b>无 <c>[Token]</c>、无 <c>user_token</c></b>（守卫 ADS-B1 / ADS-B5）：全局参数
/// <c>access_token</c> + <c>timestamp</c> + <c>nonce</c> 由 <c>AdsAuthorizationHandler</c> 成组注入；
/// 四页均<b>没有</b>「特定请求参数」表 ⇒ 一个 <c>user_token</c> 都不带。</para>
/// <para><b>四支资源族合并为一个模块</b>（口径同 <c>AdsModule.Reports</c>）：四族权限同一
/// （<c>account_management</c>）、信封与分页形态同构、链路语义一体（查资金 → 转账 → 拉流水 → 查实时消耗）。
/// <c>realtime_cost/get</c> 官方在文档树中置于「账户管理 - 资金账户」章节内、且「所属权限」与本页三面同为
/// <c>account_management</c> ⇒ 落本域而非 <c>Reports</c> 域（Reports 三面是报表权限族，权限面不同）。</para>
/// <para><b><c>date_range</c> 只能以 JSON 字符串承载</b>：官方 curl 把 <c>date_range</c> 写成
/// <b>单个</b>键值对、值为 JSON 对象字面量 ⇒ 参数类型为 <see cref="string"/>，构造入口唯一
/// <see cref="AdsQueryJson.DateRange"/>（形态差异的完整推导见 <c>AdsQueryJson</c> remarks）。</para>
/// <para><b>应答信封</b>：<c>{code, message, message_cn, data}</c>，<c>code == 0</c> 为成功；
/// 全部端点带 <c>[AllowAnyStatusCode]</c>，业务失败一律落到信封后由 <c>WechatAdsException.ThrowIfFailed</c> 判定。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Funds", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsFundsService
{
    /// <summary>
    /// 查询客户资金信息。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/funds/get"/>。
    /// </summary>
    /// <param name="accountId">推广帐号 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>资金账户列表（<c>data.list[]</c>，无分页元信息），见 <see cref="AdsFundsGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/funds/get</c>。请求侧只有 <c>account_id</c> 一支业务参数。</para>
    /// <para><b>金额单位均为分</b>；<c>realtime_cost</c> 是今日实时消耗、<c>bill_deposit_amount</c>
    /// 是锁定金额；<c>effect_funds[]</c> 按有效期逐条展开（<c>effect_date</c> / <c>expire_date</c>
    /// 为 <c>YYYY-MM-DD</c> 字符串）。</para>
    /// </remarks>
    [Get("/v3.0/funds/get")]
    Task<AdsFundsGetResponse> GetFundsAsync(
        [Query("account_id")] long accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起转账。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/fund_transfer/add"/>。
    /// </summary>
    /// <param name="request">请求体（必填 <c>account_id</c> / <c>fund_type</c> / <c>amount</c> /
    /// <c>transfer_type</c>；区间与搭配约束见 <see cref="AdsFundTransferAddRequest"/> 逐属性 XML）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账结果（含幂等标记 <c>is_repeated</c>、预取金额 <c>recommend_amount</c> 与
    /// 预校验 <c>check_result</c>），见 <see cref="AdsFundTransferAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/fund_transfer/add</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>金额硬约束</b>（原文）：<c>amount</c> 单位为分，最小 5,000、最大 2,000,000,000；
    /// 实际可转金额还取决于账户余额与冻结（SDK 不可见）⇒ 不做本地拦截，越界由应答 <c>code</c> 表达。</para>
    /// <para><b>转账类型 × 财务自理标签（六支取值的官方原文搭配，勿弱化）</b>：
    /// <c>AGENCY_TO_ADVERTISER</c>：一代服务商→子客账户或二代→二代子客（二代子客账户须「支持转入转出或仅转入」）；
    /// <c>ADVERTISER_TO_AGENCY</c>：子客→一代/二代服务商（二代子客须「支持转入转出」）；
    /// <c>AGENCY_TO_SUB_ADVERTISER</c>：一代→二代子客（须「禁止转入转出」）；
    /// <c>SUB_ADVERTISER_TO_AGENCY</c>：二代子客→一代（须「禁止转入转出或仅转入」）；
    /// <c>AGENCY_TO_SUB_AGENCY</c> / <c>SUB_AGENCY_TO_AGENCY</c>：一代↔二代（二代须「支持转入转出或仅转入」）。</para>
    /// <para><b>幂等与「只查不转」三兄弟</b>：<c>external_bill_no</c> 同一客户下唯一且<b>不支持大写</b>，
    /// 重复提交返回 <c>is_repeated == true</c>；<c>pre_fetch_amount = 1</c> 只取 <c>recommend_amount</c>、
    /// <c>pre_check = 1</c> 只回 <c>check_result</c>，两者都<b>不进行实际转账</b>；
    /// <c>transfer_try_best</c> / <c>high_frequency_transfer</c> 只对特定 <c>transfer_type</c> 生效（见 DTO XML）。
    /// 预校验明细里的错误描述官方拼写为 <c>messageCn</c>（camelCase，与信封 <c>message_cn</c> 不同构），
    /// DTO 已按原文照录。</para>
    /// </remarks>
    [Post("/v3.0/fund_transfer/add")]
    Task<AdsFundTransferAddResponse> AddTransferAsync(
        AdsFundTransferAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询账户资金流水明细。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/fund_statements_detailed/get"/>。
    /// </summary>
    /// <param name="accountId">推广帐号 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>；含代理商和账户 id）。</param>
    /// <param name="fundType">资金账户类型（Query <c>fund_type</c>，官方 <c>enum</c>、<b>必填</b>）。
    /// 本页可选值 <b>36</b> 支（2026-10-17 逐字核验）：<c>{ FUND_TYPE_CASH, FUND_TYPE_GIFT,
    /// FUND_TYPE_SHARED, FUND_TYPE_BANK, FUND_TYPE_CREDIT_ROLL, FUND_TYPE_CREDIT_TEMPORARY,
    /// FUND_TYPE_COMPENSATE_VIRTUAL, FUND_TYPE_INTERNAL_QUOTA, FUND_TYPE_TEST_VIRTUAL, FUND_TYPE_TCC_GIFT,
    /// FUND_TYPE_UNSUPPORTED, FUND_TYPE_CONTRACT_CREDIT, FUND_TYPE_SPECIAL_GIFT,
    /// FUND_TYPE_MP_GAME_DEVELOPER_WORKING_FUND, FUND_TYPE_MP_GAME_DEVELOPER_GIFT,
    /// FUND_TYPE_FLOW_SOURCE_AD_FUND, FUND_TYPE_ANDROID_ORIENTED_GIFT, FUND_TYPE_LOCATION_PROMOTION_REWARDS,
    /// FUND_TYPE_GIFT_RESTRICT, FUND_TYPE_DEBT_FIXED, FUND_TYPE_DEBT_TEMP, FUND_TYPE_SHORT_PLAY_GIFT,
    /// FUND_TYPE_GAME_CHARGE_INDIRECT_REWARDS_GIFT, FUND_TYPE_GAME_CHARGE_DIRECT_REWARDS_GIFT,
    /// FUND_TYPE_SHORT_PLAY_PUBLISHER_GIFT, FUND_TYPE_MP_GAME_SPECIAL_REWARDS_GIFT, FUND_TYPE_MINIGAME_CREDIT,
    /// FUND_TYPE_MP_GAME_PC_TARGET_GIFT, FUND_TYPE_INNER_RESOURCE_EXCHANGE, FUND_TYPE_MP_NEW_GAME_SUBSIDY,
    /// FUND_TYPE_INTEGRATED_MARKETING, FUND_TYPE_CASH_COST, FUND_TYPE_IAA_SHARED_INCOME_COST,
    /// FUND_TYPE_IAP_SHARED_INCOME_COST, FUND_TYPE_UTILITY_PUBLISHER_GIFT,
    /// FUND_TYPE_MUTUAL_SELECTION_CREDIT_ROLL }</c>。
    /// <b>比 <c>fund_transfer/add</c> 页的 9 支多 27 支</b> ⇒ 用转账页集合调本页会被官方拒绝，反之亦然。</param>
    /// <param name="dateRange">日期范围（Query <c>date_range</c>，官方 <c>struct</c>、<b>必填</b>）。值为
    /// <b>JSON 对象字符串</b>，用 <see cref="AdsQueryJson.DateRange"/> 构造；<c>start_date</c> /
    /// <c>end_date</c> 均 <c>YYYY-MM-DD</c>、10 字节；官方原文「支持两年内的数据查询」。</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>，最小 1、最大 99999，默认 1）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>，最小 1、最大 100，默认 10）。</param>
    /// <param name="primaryKey">续拉游标（Query <c>primary_key</c>，官方原文「上一页最后一条记录的
    /// primary_key」；首页不填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>流水明细列表（<c>data.list[]</c> + <c>page_info</c>），见
    /// <see cref="AdsFundStatementsDetailedGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/fund_statements_detailed/get</c>。</para>
    /// <para><b><c>page</c> 上限 99999 与 <c>primary_key</c> 并存</b>：本页既给普通分页又给「上一页最后一条
    /// 主键」续拉参数，官方未说明两者如何配套 ⇒ SDK 不预判、原样透传，以应答为准。</para>
    /// </remarks>
    [Get("/v3.0/fund_statements_detailed/get")]
    Task<AdsFundStatementsDetailedGetResponse> GetFundStatementsDetailedAsync(
        [Query("account_id")] long accountId,
        [Query("fund_type")] string fundType,
        [Query("date_range")] string dateRange,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("primary_key")] string? primaryKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取账号实时消耗。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/realtime_cost/get"/>。
    /// </summary>
    /// <param name="accountId">推广帐号 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>；
    /// 原文「有操作权限的帐号 id，不支持代理商 id」）。</param>
    /// <param name="level">实时消耗的类型级别（Query <c>level</c>，官方 <c>string</c>、<b>必填</b>；
    /// 可选值 <c>{ ADVERTISER, ADGROUP, ADTOTAL }</c>，2026-10-17 逐字核验）。
    /// <b>本页三值不带 <c>REPORT_LEVEL_</c> 前缀</b>，与 <c>daily_reports/get</c> 的 <c>level</c>
    /// 是两套拼写 ⇒ 不得互相套用（守卫 ADS-B2 逐页集合口径）。</param>
    /// <param name="date">查询日期（Query <c>date</c>，官方 <c>string</c>、<c>YYYY-MM-DD</c>、10 字节；
    /// 官方原文「只支持今天的数据查询，当 level=ADTOTAL 时不需要传该值」⇒ 虽标 <c>*</c>
    /// 仍建为可空，条件必填由应答表达）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>，数组最小 1、最大 2；
    /// 原文「当 level=ADGROUP 或 ADTOTAL 时，该字段必填；若此字段不传，或传空则视为无限制条件」）。
    /// 值为 <b>JSON 数组字符串</b>，构造入口唯一 <see cref="AdsQueryJson.Filtering"/>；
    /// 官方限定 <c>field</c> 仅可选 <c>{ adgroup_id }</c>、<c>operator</c> 仅可选 <c>{ EQUALS }</c>，
    /// <c>values</c> 为 <c>string[]</c>。）</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>，最小 1、最大 99999，默认 1）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>，最小 1、最大 100，默认 10）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>实时消耗列表（<c>data.list[]</c> + <c>page_info</c>），见
    /// <see cref="AdsRealtimeCostGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/realtime_cost/get</c>。</para>
    /// <para><b>条件必填面不在 SDK 拦截</b>：<c>date</c> 与 <c>filtering</c> 的必填性都取决于 <c>level</c>
    /// 取值（ADTOTAL 免传 date；ADGROUP/ADTOTAL 时 filtering 必填），SDK 侧看不到 <c>level</c> 与账户
    /// 形态的组合结果 ⇒ 全部按「不传即无限制/由官方判」透传。</para>
    /// <para><c>level=ADVERTISER</c> 时条目 <c>adgroup_id</c> 官方返回 <c>0</c> 且原文标注「无意义」，
    /// 不得据此判「数据缺失」。</para>
    /// </remarks>
    [Get("/v3.0/realtime_cost/get")]
    Task<AdsRealtimeCostGetResponse> GetRealtimeCostAsync(
        [Query("account_id")] long accountId,
        [Query("level")] string level,
        [Query("date")] string? date = null,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        CancellationToken cancellationToken = default);
}
