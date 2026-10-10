// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.DataCube;

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「数据统计」域 SDK（21 端点单域承载——全部 POST /datacube/*、请求体同构
/// <see cref="MpDateRangeRequest"/>，N3 裁决：不拆 4 个域）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 → 数据统计（<b>服务号域 /doc/service/api/wedata/</b> 前缀——
/// subscription 订阅号域 404，2026-10-07 逐页核验 21 页全部在服务号域命中；用户/图文/消息/接口四组）。
/// 适用范围：全部「公众号 / 服务号 —— 仅认证」。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>跨度上限措辞不一（照官方各页原文，勿互相套用）</b>：各端点上限见
/// <see cref="MpDateRangeRequest"/> remarks——用户 7 天 / 消息日数据「差值小于7天」/ 周·月·分时「必须为同一天」/
/// 分布 15 天 / 接口概要 30 天 / 接口分时 1 天 / 新图文 3 端点 1 天 / getbizsummary 30 天 /
/// 旧图文 6 端点仅「结束日期(最大值为昨日)」。越界由官方 61501 表达。</item>
/// <item><b>停止维护声明（官方原文）</b>：旧图文 6 端点（getarticlesummary / getarticletotal /
/// getuserread / getuserreadhour / getusershare / getusersharehour）页面均声明「本接口已停止维护」，
/// 推荐替换为 getarticleread / getarticleshare / getbizsummary / getarticletotaldetail——
/// SDK 照常建模（官方仍在响应）并在各方法 remarks 标注。</item>
/// <item><b>数据起始与就绪</b>：多端点声明数据始于 2014-12-01（新图文族始于 2025-11-01）；
/// 「建议每日 8 点后查询前一天数据」；getarticletotal「最多统计发表日后 7 天数据」、
/// getarticletotaldetail「每篇文章仅统计发表日期起 30 天内」。</item>
/// <item><b>共用响应 DTO（N4 裁决，由守卫双向锁定）</b>：消息族 4 端点元素完全同构共用
/// <see cref="MpUpstreamMsgItem"/>；分布族 3 端点共用 <see cref="MpUpstreamMsgDistItem"/>；
/// 日/分时对（getuserread·hour / getusershare·hour / getinterfacesummary·hour）为超集共用
/// （ref_hour 仅分时端点返回）。</item>
/// <item><b>官方文档缺口（照录）</b>：getupstreammsghour 的 ref_hour 仅在示例、字段表未列；
/// getinterfacesummaryhour 请求体表写「无」但示例含 begin_date/end_date（SDK 按同构请求建模）；
/// getarticletotal / getarticletotaldetail 的响应字段行不全（按示例建模）。</item>
/// </list>
/// <para><b>令牌路由</b>：全部端点 Query 注入 <c>access_token</c>（MUD005 已知接受风险）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "DataCube", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpDataCubeService
{
    // ---------------------------------------------------------------- 用户数据（wedata/user，跨度 7 天）

    /// <summary>
    /// 获取用户增减数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/user/api_getusersummary.html"/>
    /// （官方接口英文名 <c>getusersummary</c>；跨度「最大跨度7天」）。
    /// </summary>
    /// <param name="request">日期区间（<see cref="MpDateRangeRequest"/>，跨度 ≤ 7 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按渠道维度的用户增减列表。</returns>
    /// <remarks>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>61500</c>（date format error，日期格式错误）/
    /// <c>61501</c>（date range error，日期跨度超过限制）/ <c>61503</c>（data not ready，指定日期数据尚未生成）。
    /// 第三方平台代调用权限集 id = 2。
    /// </remarks>
    [Post("/datacube/getusersummary")]
    Task<MpUserSummaryResponse> GetUserSummaryAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取累计用户数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/user/api_getusercumulate.html"/>
    /// （官方接口英文名 <c>getusercumulate</c>；跨度「最大跨度7天」）。
    /// </summary>
    /// <param name="request">日期区间（≤ 7 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>累计用户量列表。</returns>
    /// <remarks>官方错误码与 getusersummary 同表；数据存储始于 2014-12-01；建议每日 8 点后获取前一天数据。</remarks>
    [Post("/datacube/getusercumulate")]
    Task<MpUserCumulateResponse> GetUserCumulateAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    // ---------------------------------------------------------------- 图文数据·旧（wedata/news，官方声明已停止维护）

    /// <summary>
    /// 获取图文群发每日数据（<b>官方声明已停止维护</b>，推荐 getarticleread 等新端点）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getarticlesummary.html"/>
    /// （官方接口英文名 <c>getarticlesummary</c>）。
    /// </summary>
    /// <param name="request">日期区间（参数表仅「结束日期(最大值为昨日)」，无天数表述）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按 msgid 粒度的当日阅读/分享/收藏数据。</returns>
    /// <remarks>官方错误码：<c>-1</c> / <c>40001</c> / <c>61500</c>（date range error——官方两处描述措辞不一，照录）。
    /// 阅读量总和 ≥ 3 的图文才会被统计。</remarks>
    [Post("/datacube/getarticlesummary")]
    Task<MpArticleSummaryResponse> GetArticleSummaryAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取图文群发总数据（<b>官方声明已停止维护</b>；「最多统计发表日后7天数据」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getarticletotal.html"/>
    /// （官方接口英文名 <c>getarticletotal</c>）。
    /// </summary>
    /// <param name="request">日期区间。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按群发日期的逐日累计数据（<c>details</c> 嵌套；数值为「到该日为止的总量」而非当日量）。</returns>
    /// <remarks>
    /// 官方文档缺陷（照录）：响应字段表未列 ref_date/msgid/title/details 行，仅出现在返回示例——按示例建模；
    /// feed_share 三组字段后缀为反直觉的 <c>_cnt</c>（其余为 <c>_count</c>），见 <see cref="MpArticleTotalStat"/>。
    /// </remarks>
    [Post("/datacube/getarticletotal")]
    Task<MpArticleTotalResponse> GetArticleTotalAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取图文阅读概况数据（<b>官方声明已停止维护</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getuserread.html"/>
    /// （官方接口英文名 <c>getuserread</c>；参数表仅「结束日期(最大值为昨日)」）。
    /// </summary>
    /// <param name="request">日期区间。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按来源渠道（user_source）的阅读数据。</returns>
    [Post("/datacube/getuserread")]
    Task<MpUserReadResponse> GetUserReadAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取图文阅读分时数据（<b>官方声明已停止维护</b>）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getuserreadhour.html"/>
    /// （官方接口英文名 <c>getuserreadhour</c>；与日数据同构 + <c>ref_hour</c>）。
    /// </summary>
    /// <param name="request">日期区间。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分时阅读数据（元素含 ref_hour）。</returns>
    [Post("/datacube/getuserreadhour")]
    Task<MpUserReadResponse> GetUserReadHourAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取图文转发概况数据（<b>官方声明已停止维护</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getusershare.html"/>
    /// （官方接口英文名 <c>getusershare</c>）。
    /// </summary>
    /// <param name="request">日期区间。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按分享场景（share_scene）的转发数据。</returns>
    [Post("/datacube/getusershare")]
    Task<MpUserShareResponse> GetUserShareAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取图文转发分时数据（<b>官方声明已停止维护</b>）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getusersharehour.html"/>
    /// （官方接口英文名 <c>getusersharehour</c>；与日数据同构 + <c>ref_hour</c>）。
    /// </summary>
    /// <param name="request">日期区间。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分时转发数据。</returns>
    [Post("/datacube/getusersharehour")]
    Task<MpUserShareResponse> GetUserShareHourAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    // ---------------------------------------------------------------- 图文数据·新（wedata/news，未声明停维）

    /// <summary>
    /// 获取发表内容每日阅读数据（新接口）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getarticleread.html"/>
    /// （官方接口英文名 <c>getarticleread</c>；「日期范围仅支持查询1天。」）。
    /// </summary>
    /// <param name="request">日期区间（仅 1 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按 msgid 的阅读数据（detail 含来源分布；顶层 <c>is_delay</c> 标记延迟）。</returns>
    /// <remarks>
    /// 官方注意事项：数据始于 2025-11-01；「如果当天没有文章的指标发生变化，返回的数据为空」；建议次日 8 点后请求前 1 天数据。
    /// </remarks>
    [Post("/datacube/getarticleread")]
    Task<MpDataArticleReadResponse> GetArticleReadAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取发表内容每日分享数据（新接口）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getarticleshare.html"/>
    /// （官方接口英文名 <c>getarticleshare</c>；「日期范围仅支持查询1天。」）。
    /// </summary>
    /// <param name="request">日期区间（仅 1 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按 msgid 的分享数据（detail 仅 share_user）。</returns>
    [Post("/datacube/getarticleshare")]
    Task<MpDataArticleShareResponse> GetArticleShareAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取发表内容概况总数据（账号级聚合）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getbizsummary.html"/>
    /// （官方接口英文名 <c>getbizsummary</c>；「查询日期范围的长度最长支持查询30天。」）。
    /// </summary>
    /// <param name="request">日期区间（≤ 30 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账号级阅读/分享/赞/留言/收藏/跳转原文/发布篇数聚合（非按 msgid 拆分）。</returns>
    [Post("/datacube/getbizsummary")]
    Task<MpDataBizSummaryResponse> GetBizSummaryAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取发表内容发表详细数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/news/api_getarticletotaldetail.html"/>
    /// （官方接口英文名 <c>getarticletotaldetail</c>；「日期范围仅支持查询1天。」）。
    /// </summary>
    /// <param name="request">日期区间（仅 1 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发表内容详细数据（<c>detail_list</c> 逐日累计；每篇仅统计发表起 30 天内；含赞赏金额/送达率/完成率/跳出分布）。</returns>
    /// <remarks>官方文档缺陷（照录）：响应字段表未列 ref_date/msgid/title/content_url 行，按示例建模（2026-03-03 变更日志补充标题与链接）。</remarks>
    [Post("/datacube/getarticletotaldetail")]
    Task<MpDataArticleTotalDetailResponse> GetArticleTotalDetailAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    // ---------------------------------------------------------------- 消息数据（wedata/mess）

    /// <summary>
    /// 获取消息发送概况数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/mess/api_getupstreammsg.html"/>
    /// （官方接口英文名 <c>getupstreammsg</c>；「与end_date差值小于7天」——官方原文措辞）。
    /// </summary>
    /// <param name="request">日期区间（与 end_date 差值小于 7 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按消息类型（msg_type）的发送概况。</returns>
    /// <remarks>官方错误码：<c>-1</c> / <c>40001</c> / <c>61500</c> / <c>61501</c> / <c>61503</c>（未完成数据统计处理）。</remarks>
    [Post("/datacube/getupstreammsg")]
    Task<MpUpstreamMsgResponse> GetUpstreamMsgAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息发送周数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/mess/api_getupstreammsgweek.html"/>
    /// （官方接口英文名 <c>getupstreammsgweek</c>；「结束日期(必须为同一天)」，数据标注在当周第一天）。
    /// </summary>
    /// <param name="request">日期区间（传周期首日——begin/end 同一天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按消息类型的周周期数据（元素与日数据同构 ⇒ 共用 DTO）。</returns>
    [Post("/datacube/getupstreammsgweek")]
    Task<MpUpstreamMsgResponse> GetUpstreamMsgWeekAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息发送月数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/mess/api_getupstreammsgmonth.html"/>
    /// （官方接口英文名 <c>getupstreammsgmonth</c>；「结束日期(必须为同一天)」，数据标注在当月第一天）。
    /// </summary>
    /// <param name="request">日期区间（传周期首日——begin/end 同一天；官方请求示例与说明相悖，矛盾照录）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按消息类型的月周期数据。</returns>
    [Post("/datacube/getupstreammsgmonth")]
    Task<MpUpstreamMsgResponse> GetUpstreamMsgMonthAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息发送分时数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/mess/api_getupstreammsghour.html"/>
    /// （官方接口英文名 <c>getupstreammsghour</c>；「结束日期(必须为同一天)」）。
    /// </summary>
    /// <param name="request">日期区间（begin/end 同一天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分时发送数据（元素与日数据同构 ⇒ 共用 DTO；官方示例含 ref_hour 但字段表未列——缺口照录，未建模）。</returns>
    [Post("/datacube/getupstreammsghour")]
    Task<MpUpstreamMsgResponse> GetUpstreamMsgHourAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息发送分布数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/mess/api_getupstreammsgdist.html"/>
    /// （官方接口英文名 <c>getupstreammsgdist</c>；「跨度不超过15天」）。
    /// </summary>
    /// <param name="request">日期区间（≤ 15 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按消息量分布区间（count_interval）的用户数分布。</returns>
    [Post("/datacube/getupstreammsgdist")]
    Task<MpUpstreamMsgDistResponse> GetUpstreamMsgDistAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息发送分布周数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/mess/api_getupstreammsgdistweek.html"/>
    /// （官方接口英文名 <c>getupstreammsgdistweek</c>；「跨度不超过15天」——分布族与发送族周数据措辞不一致，照录）。
    /// </summary>
    /// <param name="request">日期区间（≤ 15 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按分布区间的周数据。</returns>
    [Post("/datacube/getupstreammsgdistweek")]
    Task<MpUpstreamMsgDistResponse> GetUpstreamMsgDistWeekAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息发送分布月数据。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/mess/api_getupstreammsgdistmonth.html"/>
    /// （官方接口英文名 <c>getupstreammsgdistmonth</c>；「跨度不超过15天」）。
    /// </summary>
    /// <param name="request">日期区间（≤ 15 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按分布区间的月数据。</returns>
    [Post("/datacube/getupstreammsgdistmonth")]
    Task<MpUpstreamMsgDistResponse> GetUpstreamMsgDistMonthAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    // ---------------------------------------------------------------- 接口数据（wedata/api）

    /// <summary>
    /// 获取被动回复概要数据（官方页面现名；即接口分析数据）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/api/api_getinterfacesummary.html"/>
    /// （官方接口英文名 <c>getinterfacesummary</c>；「结束日期(最大时间跨度30天)」）。
    /// </summary>
    /// <param name="request">日期区间（≤ 30 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>被动回复次数与耗时概要（callback_count / fail_count / total_time_cost / max_time_cost）。</returns>
    /// <remarks>官方错误码：<c>-1</c> / <c>40001</c> / <c>61500</c> / <c>61501</c>（begin_date 和 end_date 差值超过最大跨度）/ <c>61503</c>（数据尚未生成）。第三方平台代调用权限集 id = 3。</remarks>
    [Post("/datacube/getinterfacesummary")]
    Task<MpInterfaceSummaryResponse> GetInterfaceSummaryAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取被动回复分布数据（分时）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/wedata/api/api_getinterfacesummaryhour.html"/>
    /// （官方接口英文名 <c>getinterfacesummaryhour</c>；「最大时间跨度为1天」（注意事项）——
    /// 请求体表写「无」但示例含 begin_date/end_date，矛盾照录，SDK 按同构请求建模）。
    /// </summary>
    /// <param name="request">日期区间（≤ 1 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分时被动回复数据（元素多 ref_hour——与日数据超集共用 DTO）。</returns>
    [Post("/datacube/getinterfacesummaryhour")]
    Task<MpInterfaceSummaryResponse> GetInterfaceSummaryHourAsync(
        [Body] MpDateRangeRequest request,
        CancellationToken cancellationToken = default);
}
