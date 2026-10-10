// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「数据分析」域 SDK（11 端点：访问趋势 3 + 访问留存 3 + 用户画像 1 + 访问分布 1 + 访问页面 1 + 概况 1 + 性能 1）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：小程序服务端 API → 数据分析（2026-10-09 逐页核验）：
/// <c>data-analysis/visit-trend/api_getdailyvisittrend.html</c>、<c>visit-retain/api_getdailyretain.html</c>、
/// <c>others/api_getuserportrait.html</c>、<c>others/api_getvisitdistribution.html</c>、
/// <c>others/api_getvisitpage.html</c>（weekly / monthly 为同级页面，路径仅周期词不同）、
/// <c>others/api_getdailysummary.html</c>、<c>others/api_getperformancedata.html</c>。
/// </para>
/// <para>
/// <b>域级约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>日期跨度与上界</b>：趋势 / 留存 / 分布 / 页面四族<b>限定查询 1 天</b>（weekly / monthly 为对应周期），
/// 用户画像为「相差 <b>0 / 6 / 29</b> 天」（即最近 1 / 7 / 30 天）；全部<b>最大值为昨日</b>；
/// 格式 <c>yyyymmdd</c>。越界由官方错误码表达，SDK <b>不做本地校验</b>
/// （与公众号线 <c>DataCube</c> 同款纪律）。</item>
/// <item><b>访问页面只返回 top 200</b>（官方原文：按 <c>page_visit_pv</c> 排序）。</item>
/// <item><b>应答信封不成一形</b>（逐页核验的<b>事实</b>，不是设计选择）：趋势 / 分布 / 页面为
/// <c>{ref_date, list:[…]}</c>（趋势页 <b>无</b>顶层 <c>ref_date</c>、仅 <c>list</c>），
/// 留存为 <c>{ref_date, visit_uv_new:[…], visit_uv:[…]}</c>，画像为
/// <c>{ref_date, visit_uv_new:{…}, visit_uv:{…}}</c> ⇒ <b>逐端点各自 DTO</b>，不做统一信封抽象。</item>
/// <item><b>支持第三方平台代调用</b>（权限集 <b>18</b>，可用 <c>authorizer_access_token</c>）——本线只覆盖自建形态。</item>
/// <item>适用范围：<b>小程序 ✔ / 小游戏 ✔</b>。</item>
/// </list>
/// </remarks>
[HttpClientApi(RegistryGroupName = "DataAnalysis", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaDataAnalysisService
{
    /// <summary>获取用户访问小程序数据<b>日</b>趋势。官方文档：<c>visit-trend/api_getdailyvisittrend.html</c>。</summary>
    /// <param name="request">日期区间（<c>begin_date</c> / <c>end_date</c>），见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日趋势列表（<c>list</c>），见 <see cref="WxaVisitTrendResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/datacube/getweanalysisappiddailyvisittrend</c> ＋ 请求体
    /// <c>{"begin_date":…,"end_date":…}</c>；Query 携带 <c>access_token</c>。</para>
    /// <para><b>跨度</b>：<c>begin_date</c> 与 <c>end_date</c> <b>限定查询 1 天</b>（两者相同），最大值为昨日。</para>
    /// <para>官方错误码：<c>-1</c>（system error）/ <c>40001</c>（令牌无效，走自愈）。</para>
    /// </remarks>
    [Post("/datacube/getweanalysisappiddailyvisittrend")]
    Task<WxaVisitTrendResponse> GetDailyVisitTrendAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取用户访问小程序数据<b>周</b>趋势。官方文档：<c>visit-trend/api_getweeklyvisittrend.html</c>。</summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>周趋势列表（<c>list</c>）。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/datacube/getweanalysisappidweeklyvisittrend</c>；跨度按官方周维度（最大值为昨日所在周）。</remarks>
    [Post("/datacube/getweanalysisappidweeklyvisittrend")]
    Task<WxaVisitTrendResponse> GetWeeklyVisitTrendAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取用户访问小程序数据<b>月</b>趋势。官方文档：<c>visit-trend/api_getmonthlyvisittrend.html</c>。</summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>月趋势列表（<c>list</c>）。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/datacube/getweanalysisappidmonthlyvisittrend</c>；跨度按官方月维度。</remarks>
    [Post("/datacube/getweanalysisappidmonthlyvisittrend")]
    Task<WxaVisitTrendResponse> GetMonthlyVisitTrendAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取用户访问小程序<b>日</b>留存。官方文档：<c>visit-retain/api_getdailyretain.html</c>。</summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>留存数据（<c>ref_date</c> + <c>visit_uv_new</c> / <c>visit_uv</c> 两个数组），见 <see cref="WxaRetainResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/datacube/getweanalysisappiddailyretaininfo</c>；跨度<b>限定 1 天</b>、最大值为昨日。</para>
    /// <para><b><c>key</c> 语义（官方原文）</b>：<c>0</c> 表示当天、<c>1</c> 表示 1 天后……取值集合为
    /// <c>0,1,2,3,4,5,6,7,14,30</c>；<c>key = 0</c> 时为当日新增 / 活跃用户数，<c>key &gt; 0</c> 时为留存用户数。
    /// <b>默认只返回 <c>key = 0</c></b>，其余需按官方口径取值。</para>
    /// </remarks>
    [Post("/datacube/getweanalysisappiddailyretaininfo")]
    Task<WxaRetainResponse> GetDailyRetainAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取用户访问小程序<b>周</b>留存。官方文档：<c>visit-retain/api_getweeklyretain.html</c>。</summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>周留存数据。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/datacube/getweanalysisappidweeklyretaininfo</c>；<c>key</c> 语义同上。</remarks>
    [Post("/datacube/getweanalysisappidweeklyretaininfo")]
    Task<WxaRetainResponse> GetWeeklyRetainAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取用户访问小程序<b>月</b>留存。官方文档：<c>visit-retain/api_getmonthlyretain.html</c>。</summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>月留存数据。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/datacube/getweanalysisappidmonthlyretaininfo</c>；<c>key</c> 语义同上。</remarks>
    [Post("/datacube/getweanalysisappidmonthlyretaininfo")]
    Task<WxaRetainResponse> GetMonthlyRetainAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序用户画像分布。官方文档：<c>others/api_getuserportrait.html</c>。
    /// </summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新用户 / 活跃用户画像（各含省 / 市 / 性别 / 平台 / 终端 / 年龄六维），见 <see cref="WxaUserPortraitResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/datacube/getweanalysisappiduserportrait</c>。</para>
    /// <para><b>跨度（本域唯一非 1 天的端点，官方原文）</b>：<c>begin_date</c> 与 <c>end_date</c> 相差天数
    /// <b>限定为 0 / 6 / 29</b>（对应最近 <b>1 / 7 / 30</b> 天），最大值为昨日。</para>
    /// <para><b><c>ref_date</c> 为区间串</b>（如 <c>"20170611-20170617"</c>），非单日。</para>
    /// <para><b>官方文档不一致（照录）</b>：字段表对六类数组统一列出 <c>id</c>/<c>name</c>/<c>value</c>，
    /// 但返回示例中 <c>devices</c> 元素<b>仅有</c> <c>name</c>/<c>value</c> ⇒ 本线把 <see cref="WxaPortraitItem.Id"/> 建模为可空，
    /// <b>不以示例覆盖字段表</b>，也不做本地补齐。</para>
    /// </remarks>
    [Post("/datacube/getweanalysisappiduserportrait")]
    Task<WxaUserPortraitResponse> GetUserPortraitAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户小程序访问分布数据。官方文档：<c>others/api_getvisitdistribution.html</c>。
    /// </summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>三维分布（来源 / 时长 / 深度），见 <see cref="WxaVisitDistributionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/datacube/getweanalysisappidvisitdistribution</c>；跨度<b>限定 1 天</b>、最大值为昨日。</para>
    /// <para><b><c>list[].index</c> 三个枚举值（官方原文）</b>：
    /// <c>access_source_session_cnt</c>（访问来源分布）/
    /// <c>access_staytime_info</c>（访问时长分布）/
    /// <c>access_depth_info</c>（访问深度分布）；<c>key</c> 为各 <c>index</c> 下的场景 id。</para>
    /// </remarks>
    [Post("/datacube/getweanalysisappidvisitdistribution")]
    Task<WxaVisitDistributionResponse> GetVisitDistributionAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取访问页面数据。官方文档：<c>others/api_getvisitpage.html</c>。
    /// </summary>
    /// <param name="request">日期区间，见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>页面访问明细（<c>ref_date</c> + <c>list</c>），见 <see cref="WxaVisitPageResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/datacube/getweanalysisappidvisitpage</c>；跨度<b>限定 1 天</b>、最大值为昨日。</para>
    /// <para><b>只返回 top 200</b>（官方原文：按 <c>page_visit_pv</c> 排序）——超出的页面<b>不会</b>出现在应答里，
    /// 勿把「列表不足」当作全量。</para>
    /// </remarks>
    [Post("/datacube/getweanalysisappidvisitpage")]
    Task<WxaVisitPageResponse> GetVisitPageAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户访问小程序数据概况。官方文档：<c>data-analysis/others/api_getdailysummary.html</c>。
    /// </summary>
    /// <param name="request">日期区间（<c>begin_date</c> / <c>end_date</c>），见 <see cref="WxaDateRangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>概况列表（<c>list</c>：累计用户数 / 转发次数 / 转发人数），见 <see cref="WxaDailySummaryResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/datacube/getweanalysisappiddailysummarytrend</c>；跨度<b>限定 1 天</b>、最大值为昨日。</para>
    /// <para><b>与「日趋势」差异（勿混淆）</b>：趋势返回打开 / 访问 / 停留等<b>过程指标</b>；概况返回
    /// <c>visit_total</c>（累计用户数）与 <c>share_pv</c> / <c>share_uv</c>（转发次数 / 人数）等<b>结果指标</b>。</para>
    /// </remarks>
    [Post("/datacube/getweanalysisappiddailysummarytrend")]
    Task<WxaDailySummaryResponse> GetDailySummaryAsync(
        [Body] WxaDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序性能数据（启动 / 运行性能）。官方文档：<c>data-analysis/others/api_getperformancedata.html</c>。
    /// </summary>
    /// <param name="request">时间与模块，见 <see cref="WxaPerformanceBootRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>性能指标数组（<c>data</c>，<c>{key,value}</c>），见 <see cref="WxaPerformanceBootResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/performance/boot</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>时间粒度（官方原文）</b>：<c>time</c> 为开始时间，格式 <c>yyyymmddhh</c>，<b>精确到小时</b>；
    /// <c>module</c> = <c>1</c> 启动性能 / <c>2</c> 运行性能。</para>
    /// </remarks>
    [Post("/wxa/business/performance/boot")]
    Task<WxaPerformanceBootResponse> GetPerformanceBootDataAsync(
        [Body] WxaPerformanceBootRequest request,
        CancellationToken cancellationToken = default);
}
