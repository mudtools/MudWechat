// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Adgroups;
using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「营销单元」域 SDK（<c>adgroups</c>，8 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-10）：查询 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/get"/>、
/// 创建 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/add"/>、
/// 更新 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update"/>、
/// 删除 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/delete"/>、
/// 批量日预算 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_daily_budget"/>、
/// 批量状态 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_configured_status"/>、
/// 批量出价 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_bid_amount"/>、
/// 批量投放日期与时段 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_datetime"/>。
/// 请求域 <c>https://api.e.qq.com/v3.0/</c>，路由按官方「请求地址」原文写成 <c>/v3.0/{resource}/{action}</c>。</para>
/// <para><b>层级事实（本域存在的理由）</b>：v3.0 的资源层级是
/// <c>advertiser</c>（客户账号）→ <c>adgroups</c>（营销单元）→ <c>dynamic_creatives</c>（创意）→
/// <c>components</c>（创意组件）。官方 v3.0 清单里<b>不存在</b> <c>campaigns/*</c>（推广计划层已在 v3.0 被移除）
/// 与 <c>ads/*</c>（广告实例不再作为独立资源族暴露）⇒ 本 SDK <b>不建模</b>那两层，
/// 任何「按计划层级组织投放」的尝试都属于对齐已下线旧 API（守卫 ADS-B2 的零路由断言负责拦住回潮）。</para>
/// <para><b>权限</b>：本页 8 个端点官方「所属权限」均为 <c>ads_management</c>，仅 <c>get</c> 额外带
/// <c>ads_insights</c>。权限不足由应答 <c>code</c> 表达，SDK 不做本地权限预判。</para>
/// <para><b>无 <c>[Token]</c></b>（守卫 ADS-B1）：全局参数 <c>access_token</c> + <c>timestamp</c> + <c>nonce</c>
/// 由 <c>AdsAuthorizationHandler</c> 成组注入，方法签名不出现这三者（同 <see cref="IWechatAdsAdvertiserService"/>）。</para>
/// <para><b><c>user_token</c> 只出现在 POST 面</b>：官方 4 支写端点与 4 支批量端点的「特定请求参数」表都列
/// <c>user_token</c>（实名认证令牌，官方原文「注意不是放在 header 中」⇒ 必须是 Query），
/// 而 <c>get</c> 页<b>没有</b>该表 ⇒ 本接口只有非 GET 的 7 个方法带该参数（守卫 ADS-B2 逐方法锁定）。</para>
/// <para><b>批量族的判错是两层的</b>：外层信封 <c>code == 0</c> 只表示请求被受理，逐条成败看
/// <c>data.list[i].code</c>，部分失败时外层仍为 <c>0</c> ⇒ 四个批量方法<b>不</b>做单层抛出判定，
/// 调用方必须逐条判定（见 <see cref="AdsAdgroupBatchData"/>）。</para>
/// <para><b>串行约束不在本域但在邻域</b>：官方对创意族写「同一个营销单元(adgroup)下，创意的新建、更新和删除
/// 操作必须串行执行」—— 该约束以 <c>adgroup_id</c> 为粒度，属 <c>dynamic_creatives</c> 域文档面。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Adgroups", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsAdgroupService
{
    /// <summary>
    /// 查询营销单元。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/get"/>。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方 <c>integer</c>、<b>必填</b>；原文「不支持代理商 id」）。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>）。值为 <b>JSON 数组字符串</b>，
    /// 用 <see cref="AdsQueryJson.Filtering"/> 构造。
    /// <b>本页官方限制</b>：数组长度 1–255，「若获取联盟营销位信息此字段必填」；
    /// <c>field</c> 可选值 <c>{ adgroup_id, adgroup_name, created_time, last_modified_time,
    /// material_package_id, configured_status, joint_budget_rule_id, auto_derived_creative_enabled,
    /// rta_target_id }</c>；<c>operator</c> 逐字段不同（<c>adgroup_id</c> 为 <c>{ EQUALS, IN }</c>、
    /// <c>adgroup_name</c> 为 <c>{ EQUALS, CONTAINS }</c>、时间两支为 <c>{ EQUALS, LESS_EQUALS, LESS,
    /// GREATER_EQUALS, GREATER }</c>、<c>configured_status</c> 与 <c>auto_derived_creative_enabled</c> 仅
    /// <c>{ EQUALS }</c>）；<c>values</c> 在 <c>adgroup_id</c> + <c>IN</c> 下长度 1–100，
    /// 时间类字段为 10 字节，<c>configured_status</c> 取值 <c>{ AD_STATUS_NORMAL, AD_STATUS_SUSPEND }</c>。
    /// <b>官方本页自相矛盾（照录）</b>：<c>values</c> 规则里引用了 <c>field</c> 可选值集合中不存在的
    /// <c>smart_delivery_platform</c>，而可选值里的 <c>rta_target_id</c> 没有任何 operator/values 规则 ⇒
    /// SDK 不做本地校验。</param>
    /// <param name="page">页码（Query <c>page</c>，官方 <c>integer</c>，最小 1、最大 100，默认 1）。
    /// 注意本页上限是 <b>100</b>，而 <c>advertiser/get</c> 的 <c>page</c> 上限是 1000 ⇒ 两域翻页策略不同。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>，最小 1、最大 100，默认 10）。</param>
    /// <param name="isDeleted">是否查询已删除的营销单元（Query <c>is_deleted</c>，官方 <c>boolean</c>，
    /// 可选值 <c>{ true, false }</c>）。</param>
    /// <param name="fields">指定返回的字段列表（Query <c>fields</c>，官方 <c>string[]</c>）。值为 <b>JSON 数组字符串</b>，
    /// 用 <see cref="AdsQueryJson.Fields"/> 构造；官方限制：数组 1–1024，每项 1–64 字节。
    /// 官方<b>未在本页枚举</b>合法取值 ⇒ 事实取值集即 <see cref="AdsAdgroupInfo"/> 的字段名，
    /// 未请求的字段在应答里为 <c>null</c> 属正常形态。</param>
    /// <param name="paginationMode">分页方式（Query <c>pagination_mode</c>，官方 <c>enum</c>，
    /// 可选值 <c>{ PAGINATION_MODE_NORMAL, PAGINATION_MODE_CURSOR }</c>，默认普通翻页）。</param>
    /// <param name="cursor">游标值（Query <c>cursor</c>，官方 <c>string</c>，0–10 字节；第一次拉取无需填写）。
    /// 官方原文：「如果你希望获取全量数据，推荐采用游标分页的方式；但是每次查询时，除了游标分页参数外，
    /// 其余参数需要保持一致」「游标分页模式下未保存初次数据快照 ⇒ 数据变化可能导致结果不一致；
    /// 返回结果中游标有效期为 24 小时」。<b>回填值是应答的 <c>next_cursor</c>，请求与应答两侧不同名</b>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>营销单元列表 + 两种分页元信息，见 <see cref="AdsAdgroupGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/adgroups/get</c>，本页<b>无</b> <c>Content-Type</c> 行、
    /// <b>无</b>「特定请求参数」表（即不带 <c>user_token</c>）。</para>
    /// <para><b>应答无示例可供对照（官方缺陷）</b>：本页应答示例恰为 <c>{"code":0,"message":"","message_cn":""}</c>，
    /// 整个 <c>data</c> 缺席，而应答字段表有近 80 个顶层字段 ⇒ 应答形态的唯一来源是字段表，
    /// 建模细节与三处「平面阅读会读错层级」的坑见 <see cref="AdsAdgroupInfo"/> remarks。</para>
    /// </remarks>
    [Get("/v3.0/adgroups/get")]
    Task<AdsAdgroupGetResponse> GetAsync(
        [Query("account_id")] long accountId,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("page_size")] long? pageSize = null,
        [Query("is_deleted")] bool? isDeleted = null,
        [Query("fields")] string? fields = null,
        [Query("pagination_mode")] string? paginationMode = null,
        [Query("cursor")] string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建营销单元。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/add"/>。
    /// </summary>
    /// <param name="request">请求体（官方 162 行参数表，顶层必填集为 <c>account_id</c> / <c>adgroup_name</c> /
    /// <c>marketing_goal</c> / <c>marketing_carrier_type</c> / <c>begin_date</c> / <c>end_date</c> /
    /// <c>time_series</c>），见 <see cref="AdsAdgroupAddRequest"/>。</param>
    /// <param name="requestId">请求唯一 id（请求头 <c>X-Request-Id</c>，选填）。官方原文：「资源请求的唯一 id…
    /// 用于保证接口重试的幂等性…即使重复请求，API 侧永远不会新建一个全新的投放资源…如果重复传入相同的请求参数
    /// 和 X-Request-Id，则会返回该 X-Request-Id 对应的唯一的投放资源」⇒ <b>这是本域唯一有官方幂等面的写端点</b>
    /// （<c>update</c> / <c>delete</c> 与四支批量端点的页面都<b>没有</b>该请求头表），
    /// 网络重试必须复用同一值，每次重试新生成值等于放弃幂等保护。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；官方「特定请求参数」表列出，
    /// 调用受限接口时必传）。属凭据 ⇒ 不得进日志 / 遥测 / 异常消息，登记面见守卫 ADS-B5。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方 <c>data</c> 只回新建的 <c>adgroup_id</c>，见 <see cref="AdsAdgroupAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/adgroups/add</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>本域条件必填面极重，SDK 一律不做本地拦截</b>：官方对大量「可选结构内的子字段」标 <c>*</c>
    /// （如 <c>targeting.geo_location.location_types</c>、<c>user_action_sets[].type/id</c>、
    /// <c>deep_conversion_*_spec.goal</c>、<c>prospect_retargeting.enabled</c>），
    /// 其真实语义是「父结构在场时子字段必填」；本地按 <c>*</c> 拦截会把「整个父结构不传」这一合法形态一起挡掉。
    /// 另有一条跨结构二选一：官方原文「当推广产品类型是以下类型的时候，必须使用该字段，不允许使用
    /// <c>marketing_asset_id</c>」（指 <c>marketing_asset_outer_spec</c>）与「当营销形态为动态商品营销时，
    /// <c>mpa_spec</c> 该字段必填」⇒ 判定依赖推广产品类型，SDK 侧不可见。</para>
    /// <para><b>ADX 程序化投放的禁用面</b>：官方对 <c>scene_spec</c>、<c>daily_budget</c>、
    /// <c>deep_conversion_spec</c>、<c>configured_status</c> 等十余个字段重复标注「ADX 程序化投放不可填写提交」，
    /// 完整清单见 <see cref="AdsAdgroupAddRequest"/> remarks。</para>
    /// </remarks>
    [Post("/v3.0/adgroups/add")]
    Task<AdsAdgroupAddResponse> AddAsync(
        AdsAdgroupAddRequest request,
        [Header("X-Request-Id")] string? requestId = null,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新营销单元。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update"/>。
    /// </summary>
    /// <param name="request">请求体（官方 123 行参数表，顶层仅 <c>account_id</c> 与 <c>adgroup_id</c> 必填），
    /// 见 <see cref="AdsAdgroupUpdateRequest"/>。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；语义同
    /// <see cref="AddAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方 <c>data</c> 回 <c>adgroup_id</c>，见 <see cref="AdsAdgroupUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/adgroups/update</c>，<c>Content-Type: application/json</c>。
    /// 本页<b>没有</b> <c>X-Request-Id</c> 请求头表（幂等面只在 <c>add</c>）⇒ 更新的重试不做幂等保证。</para>
    /// <para><b>官方不开放的字段集是事实而非遗漏</b>：<c>marketing_goal</c> / <c>marketing_carrier_type</c> /
    /// <c>site_set</c> / <c>automatic_site_enabled</c> / <c>mpa_spec</c> / <c>dca_spec</c> /
    /// <c>marketing_asset*</c> / <c>bid_scene</c> / <c>smart_bid_type</c> 等约二十三支在 <c>add</c> 存在、
    /// 在 <c>update</c> 不存在 ⇒ 不得为「对齐 add」补上（见 <see cref="AdsAdgroupUpdateRequest"/> remarks）。</para>
    /// </remarks>
    [Post("/v3.0/adgroups/update")]
    Task<AdsAdgroupUpdateResponse> UpdateAsync(
        AdsAdgroupUpdateRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除营销单元。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/delete"/>。
    /// </summary>
    /// <param name="request">请求体（官方只有 <c>account_id</c> 与 <c>adgroup_id</c> 两个必填字段，
    /// <b>不存在</b>批量删除形态），见 <see cref="AdsAdgroupDeleteRequest"/>。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；语义同 <see cref="AddAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方 <c>data</c> 回被删除的 <c>adgroup_id</c>，见 <see cref="AdsAdgroupDeleteResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/adgroups/delete</c>，<c>Content-Type: application/json</c>。
    /// 批量改字段走 <see cref="UpdateDailyBudgetAsync"/> 等四支批量端点；批量删除官方<b>未提供</b>。</para>
    /// <para><b>删除语义官方未写</b>：本页无使用说明章节，没有一句说明删除可否恢复；
    /// 但 <see cref="GetAsync"/> 提供 <c>is_deleted</c> 请求参数 ⇒ 官方为标记删除。
    /// 该推断只作提示，不构成 SDK 承诺（见 <see cref="AdsAdgroupDeleteRequest"/> remarks）。</para>
    /// </remarks>
    [Post("/v3.0/adgroups/delete")]
    Task<AdsAdgroupDeleteResponse> DeleteAsync(
        AdsAdgroupDeleteRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量修改营销单元日预算。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_daily_budget"/>。
    /// </summary>
    /// <param name="request">请求体（<c>account_id</c> + <c>update_daily_budget_spec</c> 数组，
    /// 元素内 <c>adgroup_id</c> / <c>daily_budget</c> 均必填），见 <see cref="AdsAdgroupUpdateDailyBudgetRequest"/>。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；语义同 <see cref="AddAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>逐条结果（<c>data.list[]</c> 每条自带 <c>code</c>）+ 失败 id 集合（<c>data.fail_id_list</c>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/adgroups/update_daily_budget</c>，<c>application/json</c>。</para>
    /// <para><b>本页官方未标数组最大长度</b>（同族另三支标 100、账户级同名端点也标 100）⇒ 视为文档遗漏、
    /// SDK 不做本地长度拦截；<c>daily_budget</c> 的四条数值约束（区间、幅度、与当日消耗及冻结金的关系）
    /// 见 <see cref="AdsAdgroupUpdateDailyBudgetSpec"/>，本地一律不校验。</para>
    /// <para><b>判错两层都做</b>：外层 <c>code == 0</c> 不代表每条成功；官方原文「返回结果的顺序和
    /// <c>update_daily_budget_spec</c> 中的参数顺序是一致的」⇒ 可按索引配对，<c>adgroup_id</c> 不允许重复。</para>
    /// </remarks>
    [Post("/v3.0/adgroups/update_daily_budget")]
    Task<AdsAdgroupUpdateDailyBudgetResponse> UpdateDailyBudgetAsync(
        AdsAdgroupUpdateDailyBudgetRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量修改营销单元投放状态。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_configured_status"/>。
    /// </summary>
    /// <param name="request">请求体（<c>account_id</c> + <c>update_configured_status_spec</c>，数组最大长度 100），
    /// 见 <see cref="AdsAdgroupUpdateConfiguredStatusRequest"/>。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；语义同 <see cref="AddAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>逐条结果 + 失败 id 集合，见 <see cref="AdsAdgroupBatchData"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/adgroups/update_configured_status</c>，<c>application/json</c>。
    /// 本页是四支批量端点里<b>内部一致性最好</b>的一支（官方对 <c>adgroup_id</c> 的描述仍是父结构文案，
    /// 但数组上限、枚举值、无重复约束齐全）。</para>
    /// <para><b>与单支更新的关系</b>：<c>configured_status</c> 也可经 <see cref="UpdateAsync"/> 逐单元修改，
    /// 本端点的差异只在「一次最多 100 支且逐条返回结果」。</para>
    /// </remarks>
    [Post("/v3.0/adgroups/update_configured_status")]
    Task<AdsAdgroupUpdateConfiguredStatusResponse> UpdateConfiguredStatusAsync(
        AdsAdgroupUpdateConfiguredStatusRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量修改营销单元出价。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_bid_amount"/>。
    /// </summary>
    /// <param name="request">请求体（<c>account_id</c> + <c>update_bid_amount_spec</c>，数组最大长度 100），
    /// 见 <see cref="AdsAdgroupUpdateBidAmountRequest"/>。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；语义同 <see cref="AddAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>逐条结果 + 失败 id 集合，见 <see cref="AdsAdgroupBatchData"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/adgroups/update_bid_amount</c>，<c>application/json</c>。</para>
    /// <para><b>出价合法性依赖出价方式与优化目标</b>：官方只在本页给出「单位为分、ADX 程序化投放默认填写 200」，
    /// 并把合法区间指向「出价规则」章节（该章节按 <c>bid_mode</c> / <c>optimization_goal</c> 组合而不同）⇒
    /// SDK 侧不做任何范围校验，越界由应答 <c>code</c> 表达。</para>
    /// </remarks>
    [Post("/v3.0/adgroups/update_bid_amount")]
    Task<AdsAdgroupUpdateBidAmountResponse> UpdateBidAmountAsync(
        AdsAdgroupUpdateBidAmountRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量修改营销单元投放日期与时段。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/update_datetime"/>。
    /// </summary>
    /// <param name="request">请求体（<c>account_id</c> + <c>update_datetime_spec</c>，数组最大长度 100），
    /// 见 <see cref="AdsAdgroupUpdateDatetimeRequest"/>。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；语义同 <see cref="AddAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>逐条结果 + 失败 id 集合，见 <see cref="AdsAdgroupBatchData"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/adgroups/update_datetime</c>，<c>application/json</c>。</para>
    /// <para><b>每条至少给出一个日期/时段字段</b>（官方使用说明原文），三字段本身全标选填 ⇒ 条件必填，
    /// SDK 不预判。微信流量另有「更新后的结束时间至少是当前时间 6 小时之后」等约束，
    /// 详见 <see cref="AdsAdgroupUpdateDatetimeSpec"/>。</para>
    /// <para><b>本页官方自相矛盾两处（照录）</b>：使用说明把数组名写成 <c>update_date_spec</c>
    /// （请求表为 <c>update_datetime_spec</c>，报文以请求表为准）；请求示例体省略了标必填的 spec 数组。</para>
    /// </remarks>
    [Post("/v3.0/adgroups/update_datetime")]
    Task<AdsAdgroupUpdateDatetimeResponse> UpdateDatetimeAsync(
        AdsAdgroupUpdateDatetimeRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);
}
