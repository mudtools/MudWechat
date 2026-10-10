// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.Reports;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「报表」域 SDK（<c>daily_reports</c> / <c>hourly_reports</c> /
/// <c>async_reports</c> 三支资源族，4 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-10）：日报表 <see href="https://developers.e.qq.com/v3.0/docs/api/daily_reports/get"/>、
/// 小时报表 <see href="https://developers.e.qq.com/v3.0/docs/api/hourly_reports/get"/>、
/// 创建异步报表任务 <see href="https://developers.e.qq.com/v3.0/docs/api/async_reports/add"/>、
/// 查询异步报表任务 <see href="https://developers.e.qq.com/v3.0/docs/api/async_reports/get"/>。
/// 请求域 <c>https://api.e.qq.com/v3.0/</c>，路由按官方「请求地址」原文写成 <c>/v3.0/{resource}/{action}</c>。</para>
/// <para><b>本域一次跨三支资源族</b>（守卫 ADS-B2 的 <c>ReportsRoutes</c> 逐条点名）：官方把
/// <c>daily_reports</c> / <c>hourly_reports</c> / <c>async_reports</c> 分成三族、各有 <c>*</c> 动作，
/// 但它们的权限（全部 <c>ads_insights</c>）、信封与分页形态同构，且异步链路的语义是一体的
/// （add 拿 <c>task_id</c> → get 读 <c>status</c> 与 <c>file_id</c>）⇒ 收在一支接口、一个注册组
/// <c>Reports</c>，而不是为凑「一族一接口」拆出三个空模块。</para>
/// <para><b>方法全是官方原文形态</b>：<b>四支里三支 GET、一支 POST</b>。本仓「多数查询类接口官方即 POST」
/// 的预期在报表族<b>不成立</b>（官方留档 §7.8 已把这条列为族级结论）⇒ 照录，不「顺手对齐」成 POST。</para>
/// <para><b>权限</b>：本域四端点官方「所属权限」均为 <c>ads_insights</c>（与 <c>adgroups</c> 域的
/// <c>ads_management</c> 不同一批次授权面）。权限不足由应答 <c>code</c> 表达，SDK 不做本地权限预判。</para>
/// <para><b>无 <c>[Token]</c>、无 <c>user_token</c></b>（守卫 ADS-B1 / ADS-B5）：全局参数
/// <c>access_token</c> + <c>timestamp</c> + <c>nonce</c> 由 <c>AdsAuthorizationHandler</c> 成组注入；
/// 本域四页<b>都没有</b>「特定请求参数」表 ⇒ 一个 <c>user_token</c> 都不带（GET 面本就无该表，
/// <c>async_reports/add</c> 页也没有）。</para>
/// <para><b>复合 Query 参数只能以 JSON 字符串承载</b>：官方 curl 把 <c>date_range</c> 写成
/// <b>单个</b>键值对、值为 JSON 对象字面量（<c>-d 'date_range={"start_date": "2024-01-01", …}'</c>），
/// <c>filtering</c> / <c>fields</c> / <c>group_by</c> / <c>order_by</c> 为 JSON 数组字面量 ⇒
/// 这五支参数类型为 <see cref="string"/>，构造入口唯一 <see cref="AdsQueryJson"/>。</para>
/// <para><b>两条只写在「使用说明」、SDK 不本地拦截的上限</b>：① 同步两页 <c>page * page_size &lt;= 20000</c>，
/// 超限须改用 <see cref="AddAsyncReportAsync"/>；② <c>async_reports/add</c> 官方原文
/// 「每账号（account_id）限制最多 5 分钟创建 1 个异步报表任务，多种级别异步报表任务算多个」——
/// 这是 v3.0 已核验页面里<b>唯一</b>一处按账号计的<b>创建频率</b>上限。SDK 不做进程内节流：
/// 多实例部署下进程节流既挡不住跨实例并发、又会在单实例误伤合法调用，节奏归调用方
/// （分布式节流属宿主侧能力，不在 SDK 内造第二套时钟）。</para>
/// <para><b>金额单位是分</b>：本域全部金额类指标字段（<c>cost</c> 等）官方单位为<b>分</b>，
/// 与 <c>advertiser</c> / <c>adgroups</c> 的预算、出价字段同一口径；SDK 不做任何单位换算。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Reports", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsReportService
{
    /// <summary>
    /// 查询日报表。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/daily_reports/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>；原文「有操作权限的帐号 id，
    /// 不支持代理商 id」）。<b>官方本页未加必填星号，但按描述实际必填</b>（留档 §9 矛盾登记）⇒
    /// 本签名取<b>非可空</b>，理由取描述而非取星号。</param>
    /// <param name="level">查询层级（Query <c>level</c>，官方 <c>enum</c>、<b>必填</b>）。
    /// 本页（daily_reports/get）官方可选值 <b>17</b> 支：<c>{ REPORT_LEVEL_ADVERTISER,
    /// REPORT_LEVEL_ADGROUP, REPORT_LEVEL_DYNAMIC_CREATIVE, REPORT_LEVEL_COMPONENT, REPORT_LEVEL_CHANNEL,
    /// REPORT_LEVEL_BIDWORD, REPORT_LEVEL_QUERYWORD, REPORT_LEVEL_MATERIAL_IMAGE,
    /// REPORT_LEVEL_MATERIAL_VIDEO, REPORT_LEVEL_MARKETING_ASSET, REPORT_LEVEL_PRODUCT_CATALOG,
    /// REPORT_LEVEL_PROJECT, REPORT_LEVEL_PROJECT_CREATIVE, REPORT_LEVEL_VIDEO_HIGHLIGHT,
    /// REPORT_LEVEL_PRODUCT_CREATIVE_TEMPLATE, REPORT_LEVEL_WECHAT_SHOP_PRODUCT, REPORT_LEVEL_PLAYLET }</c>。
    /// 官方原文：「查询业务单元报表时 level 只支持组件层级」。
    /// <b>本集合与 hourly_reports/get（8 支）、async_reports/add（21 支）互不相同，是官方页面之间的真实差异</b> ⇒
    /// 不得合并成一支公共枚举（守卫 ADS-B2 逐页锁定三支集合）。以 <see cref="string"/> 承载。</param>
    /// <param name="dateRange">日期区间（Query <c>date_range</c>，官方 <c>struct</c>、<b>必填</b>）。值为
    /// <b>JSON 对象字符串</b>，用 <see cref="AdsQueryJson.DateRange"/> 构造；官方原文「最早支持查询 1 年内
    /// （365 天）的数据」，<c>start_date</c> / <c>end_date</c> 均为 <c>YYYY-MM-DD</c>、10 字节。</param>
    /// <param name="fields">返回字段列表（Query <c>fields</c>，官方 <c>string[]</c>、<b>必填</b>）。值为
    /// <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.Fields"/> 构造；数组 1–1024、每项 1–64 字节。
    /// <b>应答行的键集由本参数决定</b>（见 <see cref="AdsReportListData"/>）⇒ 未请求的指标不会出现在应答里。
    /// 取分版位数据须同时在 <c>fields</c> 与 <c>group_by</c> 里带 <c>site_set</c>。</param>
    /// <param name="groupBy">分组维度（Query <c>group_by</c>，官方 <c>string[]</c>、<b>必填</b>）。值为
    /// <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.GroupBy"/> 构造；数组 1–10、每项 ≤255 字节，
    /// 例 <c>["date"]</c>。<b>本页元素上限 255 字节、hourly 页是 64 字节</b> ⇒ 两支不共用常量。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>，数组 1–40）。值为
    /// <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.Filtering"/> 构造。
    /// <b>本页 <c>field</c> 可选值</b>：<c>{ adgroup_id, dynamic_creative_id, component_id, component_type,
    /// bidword_id, channel_id, image_id, video_id, marketing_target_type, marketing_asset_id,
    /// smart_delivery_platform, md5, product_catalog_id, product_series_id, product_outer_id,
    /// creative_template_group_id }</c>；<c>operator</c> 逐 <c>field</c> 不同（普遍为 <c>{ EQUALS, IN }</c>，
    /// <c>smart_delivery_platform</c> 另允许 <c>NOT_EQUALS</c>）；<c>values</c> 数组 1–100、每项 ≤64 字节。
    /// 官方原文「若获取联盟营销位信息此字段必填」⇒ 条件必填，SDK 不预判。</param>
    /// <param name="orderBy">排序条件（Query <c>order_by</c>，官方 <c>struct[]</c>，数组 1–2）。值为
    /// <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.OrderBy"/> 构造；<c>sort_type</c> 可选值
    /// <c>{ ASCENDING, DESCENDING }</c>。</param>
    /// <param name="timeLine">数据口径（Query <c>time_line</c>，官方 <c>enum</c>，可选值
    /// <c>{ REQUEST_TIME, REPORTING_TIME, ACTIVE_TIME }</c>）。</param>
    /// <param name="page">页码（Query <c>page</c>，官方 <c>integer</c>，1–99999，默认 1）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>，1–2000，默认 10）。
    /// <b>与 <paramref name="page"/> 的乘积官方上限 20000</b>，超限须改用 <see cref="AddAsyncReportAsync"/>。</param>
    /// <param name="organizationId">业务单元 id（Query <c>organization_id</c>，官方 <c>integer</c>，0–9999999999）。
    /// <b>本页有该参数、hourly_reports/get 页没有</b> ⇒ 两支签名不同，勿「对齐」补字段。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>报表行列表（键集随 <c>fields</c> 而变的透传形态）+ <c>page_info</c>，
    /// 见 <see cref="AdsDailyReportResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/daily_reports/get</c>，curl 用 <c>-G -d</c> ⇒ 参数进 Query；
    /// 本页<b>无</b>「特定请求参数」表（不带 <c>user_token</c>）。</para>
    /// <para><b>官方本页四处自相矛盾（照录，SDK 一律不校验）</b>：① <c>account_id</c> 未加星却实际必填；
    /// ② <c>end_date</c> 的约束引用参数表里<b>不存在</b>的 <c>begin_date</c>；③ <c>group_by</c> / <c>fields</c>
    /// 标必填但官方示例两支都没给；④ <c>level</c> 集合缺 <c>async_reports/add</c> 上出现的
    /// <c>REPORT_LEVEL_AGE</c> / <c>GENDER</c> / … 七支（该页另删三支）。</para>
    /// </remarks>
    [Get("/v3.0/daily_reports/get")]
    Task<AdsDailyReportResponse> GetDailyAsync(
        [Query("account_id")] long accountId,
        [Query("level")] string level,
        [Query("date_range")] string dateRange,
        [Query("fields")] string fields,
        [Query("group_by")] string groupBy,
        [Query("filtering")] string? filtering = null,
        [Query("order_by")] string? orderBy = null,
        [Query("time_line")] string? timeLine = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("organization_id")] long? organizationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询小时报表。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/hourly_reports/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>；
    /// 原文「不支持代理商 id」—— 本页<b>有</b>星号，与 daily 页的未加星形成对照）。</param>
    /// <param name="level">查询层级（Query <c>level</c>，官方 <c>enum</c>、<b>必填</b>）。
    /// 本页（hourly_reports/get）官方可选值 <b>8</b> 支：<c>{ REPORT_LEVEL_ADVERTISER,
    /// REPORT_LEVEL_ADGROUP, REPORT_LEVEL_DYNAMIC_CREATIVE, REPORT_LEVEL_CHANNEL, REPORT_LEVEL_BIDWORD,
    /// REPORT_LEVEL_PROJECT, REPORT_LEVEL_PROJECT_CREATIVE, REPORT_LEVEL_VIDEO_HIGHLIGHT }</c>。
    /// <b>比 daily 少 9 支</b>（无组件 / 素材 / 营销资产 / 商品 / 版位等层级）⇒ 用 daily 的集合调本页
    /// 会被官方拒绝。以 <see cref="string"/> 承载。</param>
    /// <param name="dateRange">日期区间（Query <c>date_range</c>，官方 <c>struct</c>、<b>必填</b>），用
    /// <see cref="AdsQueryJson.DateRange"/> 构造。
    /// <b>本页两条约束与 daily 页完全不同</b>：官方原文「最多支持查询 90 天内的数据查询，支持的最长查询跨度
    /// 为 1 天」，且 <c>start_date</c> 与 <c>end_date</c> <b>必须相等</b>（<c>daily</c> 允许 365 天、任意跨度）。
    /// <b>官方自己的示例就写成 <c>start_date=2024-01-02</c> / <c>end_date=2024-01-01</c>，违反该「等于」约束</b>
    /// （照录进留档 §9）⇒ SDK 不校验。</param>
    /// <param name="fields">返回字段列表（Query <c>fields</c>，官方 <c>string[]</c>、<b>必填</b>，数组 1–1024、
    /// 每项 1–64 字节），用 <see cref="AdsQueryJson.Fields"/> 构造。本页示例行含约 150 支指标
    /// （<c>hour</c>、<c>cost</c>、<c>view_count</c>、<c>valid_click_count</c>、<c>ctr</c>、<c>cpc</c>、
    /// <c>thousand_display_price</c> 等），实际返回键集仍由本参数决定。</param>
    /// <param name="groupBy">分组维度（Query <c>group_by</c>，官方 <c>string[]</c>、<b>必填</b>，数组 1–10、
    /// <b>每项 ≤64 字节</b>（daily 页为 255）），用 <see cref="AdsQueryJson.GroupBy"/> 构造。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>，数组 1–40），用
    /// <see cref="AdsQueryJson.Filtering"/> 构造。<b>本页 <c>field</c> 只有 8 支</b>：
    /// <c>{ adgroup_id, dynamic_creative_id, component_id, bidword_id, channel_id, component_type,
    /// image_id, video_id }</c>（daily 页的 16 支子集）；<c>operator</c> 为 <c>{ EQUALS, IN }</c>；
    /// <c>values</c> 数组 1–100、每项 ≤64 字节。</param>
    /// <param name="orderBy">排序条件（Query <c>order_by</c>，官方 <c>struct[]</c>，数组 1–2），用
    /// <see cref="AdsQueryJson.OrderBy"/> 构造；<c>sort_type</c> 可选值 <c>{ ASCENDING, DESCENDING }</c>。</param>
    /// <param name="timeLine">数据口径（Query <c>time_line</c>，官方 <c>enum</c>，可选值
    /// <c>{ REQUEST_TIME, REPORTING_TIME, ACTIVE_TIME }</c>）。</param>
    /// <param name="page">页码（Query <c>page</c>，官方 <c>integer</c>，<b>1–100</b>，默认 1）。
    /// <b>本页上限 100、daily 页是 99999</b>；且与「<c>page * page_size &lt;= 20000</c>」只在
    /// <c>page_size = 200</c> 附近自洽（官方两约束并列即矛盾，照录）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>，1–2000，默认 10）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>报表行列表 + <c>page_info</c>，见 <see cref="AdsHourlyReportResponse"/>（载荷与 daily 共用
    /// <see cref="AdsReportListData"/>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/hourly_reports/get</c>；本页<b>没有</b> <c>organization_id</c>
    /// 参数（daily 页有）⇒ 本签名<b>不</b>带该参数，「与 daily 保持一致而补一个」会发送官方未定义的参数。</para>
    /// <para><b>官方本页三处自相矛盾（照录）</b>：① <c>end_date</c> 又引用不存在的 <c>begin_date</c>；
    /// ② 示例的 <c>start_date</c> 晚于 <c>end_date</c>；③ 示例应答 <c>page_info.total_number = 1</c> 而
    /// <c>list</c> 有 2 项。</para>
    /// </remarks>
    [Get("/v3.0/hourly_reports/get")]
    Task<AdsHourlyReportResponse> GetHourlyAsync(
        [Query("account_id")] long accountId,
        [Query("level")] string level,
        [Query("date_range")] string dateRange,
        [Query("fields")] string fields,
        [Query("group_by")] string groupBy,
        [Query("filtering")] string? filtering = null,
        [Query("order_by")] string? orderBy = null,
        [Query("time_line")] string? timeLine = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建异步报表任务。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/async_reports/add"/>。
    /// </summary>
    /// <param name="request">请求体（官方 <c>account_id</c> / <c>task_name</c> / <c>report_fields</c> /
    /// <c>level</c> / <c>granularity</c> / <c>date</c> 六支必填），见 <see cref="AdsAsyncReportAddRequest"/>。
    /// 本页（async_reports/add）官方可选值 <b>21</b> 支：<c>{ REPORT_LEVEL_ADVERTISER,
    /// REPORT_LEVEL_ADGROUP, REPORT_LEVEL_DYNAMIC_CREATIVE, REPORT_LEVEL_COMPONENT, REPORT_LEVEL_CHANNEL,
    /// REPORT_LEVEL_BIDWORD, REPORT_LEVEL_QUERYWORD, REPORT_LEVEL_MATERIAL_IMAGE,
    /// REPORT_LEVEL_MATERIAL_VIDEO, REPORT_LEVEL_MARKETING_ASSET, REPORT_LEVEL_PRODUCT_CATALOG,
    /// REPORT_LEVEL_PROJECT, REPORT_LEVEL_PROJECT_CREATIVE, REPORT_LEVEL_PRODUCT_CREATIVE_TEMPLATE,
    /// REPORT_LEVEL_AGE, REPORT_LEVEL_GENDER, REPORT_LEVEL_REGION, REPORT_LEVEL_CITY,
    /// REPORT_LEVEL_LANDING_PAGE, REPORT_LEVEL_AD_UNION, REPORT_LEVEL_OS }</c> —— 即在 daily 的集合上
    /// <b>增</b>人口属性与落地页等七支、<b>删</b> <c>VIDEO_HIGHLIGHT</c> / <c>WECHAT_SHOP_PRODUCT</c> /
    /// <c>PLAYLET</c> 三支 ⇒ 与两支同步查询页均不同（守卫 ADS-B2 逐页锁定三支集合）。
    /// 本页 <c>filtering</c> 数组上限 <b>5</b>（同步页为 40）、
    /// <c>group_by</c> 上限 <b>5</b> 且为<b>选填</b>（同步页必填）、<c>operator</c> 集合更宽
    /// （<c>{ EQUALS, CONTAINS, LESS_EQUALS, LESS, GREATER_EQUALS, GREATER, IN, NOT_EQUALS }</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方 <c>data</c> 只回 <c>task_id</c>，见 <see cref="AdsAsyncReportAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/async_reports/add</c>，<c>Content-Type: application/json</c>；
    /// 本页<b>无</b>「特定请求参数」表（不带 <c>user_token</c>）、<b>无</b>幂等请求头表（不带 <c>X-Request-Id</c>）
    /// ⇒ 重试创建任务会真的再建一个任务，且官方按账号限频，重试前先确认前一次是否成功。</para>
    /// <para><b>本域唯一的创建频率上限（原文）</b>：「每账号（account_id）限制最多 5 分钟创建 1 个异步报表任务，
    /// 多种级别异步报表任务算多个」。SDK 不做进程内节流，理由见接口 remarks。</para>
    /// <para><b>本端点没有 <c>date_range</c> / <c>page</c> / <c>page_size</c></b>：只有<b>单支</b> <c>date</c>
    /// 配 <c>granularity</c>（<c>HOURLY</c> → 最近 30 天、<c>DAILY</c> → 最近 365 天），字段清单那支叫
    /// <c>report_fields</c> 而非 <c>fields</c>。官方示例还把必填的 <c>report_fields</c> 整个省略（照录）。</para>
    /// </remarks>
    [Post("/v3.0/async_reports/add")]
    Task<AdsAsyncReportAddResponse> AddAsyncReportAsync(
        AdsAsyncReportAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询异步报表任务。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/async_reports/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>，本页<b>标选填</b>，
    /// 描述「拥有操作权限的账户 ID」）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>，数组 1–5）。值为
    /// <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.Filtering"/> 构造。
    /// <b>本页 <c>field</c> 只有两支</b>：<c>{ task_id, task_name }</c>；<c>operator</c> 可选值
    /// <c>{ EQUALS, CONTAINS, LESS_EQUALS, LESS, GREATER_EQUALS, GREATER, IN, NOT_EQUALS }</c>；
    /// <c>values</c> 数组 1–100、每项 1–64 字节。
    /// <b><c>status</c> 不在可过滤集合里</b> ⇒ 「轮询到完成」的正确写法是按 <c>task_id</c> 过滤后
    /// 读回元素的 <see cref="AdsAsyncReportTaskInfo.Status"/>，不能把 <c>TASK_STATUS_COMPLETED</c> 当过滤条件传。</param>
    /// <param name="page">页码（Query <c>page</c>，官方 <c>integer</c>，1–99999，默认 1）。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>，<b>1–100</b>，默认 10）。
    /// <b>本页上限 100</b>，与同步两页的 2000 不同。</param>
    /// <param name="organizationId">业务单元 id（Query <c>organization_id</c>，官方 <c>integer</c>，0–9999999999）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>任务列表（<c>task_id</c> / <c>task_name</c> / <c>status</c> / <c>created_time</c> /
    /// <c>result.file_info_list[]{file_id, md5}</c>）+ <c>page_info</c>，见 <see cref="AdsAsyncReportGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/async_reports/get</c>；本页<b>没有</b> <c>level</c> /
    /// <c>granularity</c> / <c>date</c>（那三支只在 <see cref="AddAsyncReportAsync"/> 上）。</para>
    /// <para><b>两条官方时限（原文）</b>：「异步任务获取接口支持 7 日内任务记录的查询，单生成任务文件下载
    /// 有效期为 24 小时」⇒ 任务记录只在 7 日窗口内可查，拿到 <c>file_id</c> 后必须在 24 小时内完成下载，
    /// 否则只能重建任务。</para>
    /// <para><b>本域链路到此处为止，下载不在本接口</b>：<c>file_id</c> + <c>task_id</c> 要交给
    /// <c>async_report_files/get</c>，而那页的请求地址<b>逐字是另一台主机</b>
    /// <c>https://dl.e.qq.com/v3.0/async_report_files/get</c>（官方 curl 同页逐字给出）。本仓广告线目前
    /// 只有一条指向 <c>api.e.qq.com</c> 的业务客户端 ⇒ 该端点<b>尚未落地</b>（需先决定第二条基址的客户端形态），
    /// 已核验事实与阻塞原因登记在 <c>.docs</c> 留档 §7.4 与方案文档。</para>
    /// </remarks>
    [Get("/v3.0/async_reports/get")]
    Task<AdsAsyncReportGetResponse> GetAsyncReportAsync(
        [Query("account_id")] long? accountId,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("organization_id")] long? organizationId = null,
        CancellationToken cancellationToken = default);
}
