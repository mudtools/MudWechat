// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School.Living;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块上课直播域公共 SDK。
/// <para>
/// 官方对三类应用开放完全一致的 7 个端点（老师直播 ID 列表、直播详情、观看/未观看直播统计
/// 及其 V2 版本、删除直播回放），全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建见 <see cref="IWechatWorkInternalSchoolLivingService"/>，
/// 第三方见 <see cref="IWechatWorkThirdPartySchoolLivingService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderSchoolLivingService"/>。
/// </para>
/// <para>
/// 注意官方路由差异：获取老师直播 ID 列表与删除直播回放位于
/// <c>/cgi-bin/living/</c> 段下，其余 5 个端点位于 <c>/cgi-bin/school/living/</c> 段下，照抄不纠正。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：获取老师直播 ID 列表与删除直播回放——自建应用须配置到「上课直播/直播 - 可调用接口的应用」中，
/// 第三方 / 代开发应用须具有「上课直播/直播」（删除直播回放为「直播」）权限；
/// 其余端点——自建应用须配置到「上课直播 - 可调用接口的应用」和「家长可以使用的应用」中，
/// 第三方 / 代开发应用须具有「直播」和「家校沟通」权限。
/// 全域仅能获取本应用创建的直播。自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口
/// （存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolLivingService
{
    /// <summary>
    /// 获取老师直播 ID 列表（官方即 POST，路由位于 /cgi-bin/living/ 段）
    /// <para>通过此接口可以获取指定老师的所有直播 ID。</para>
    /// <para>官方业务限制：只能获取本应用创建的直播；
    /// limit 默认值和最大值都为 100；以 next_cursor 分页，返回空字符串表示已是最后一页。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="LivingGetUserAllLivingIdRequest"/>，userid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播 ID 列表（livingid_list）与分页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93739"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93856"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97127"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/get_user_all_livingid")]
    Task<LivingGetUserAllLivingIdResponse> GetUserAllLivingIdAsync(
        [Body] LivingGetUserAllLivingIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播详情（官方即 GET，livingid 走 Query）
    /// <para>获取直播详情（主题、开播时间、时长、主播、直播范围、观看/评论数、回放开关等）。</para>
    /// <para>官方业务限制：只能获取本应用创建的直播；
    /// push_stream_url 仅直播类型为活动直播且状态是待开播时返回。</para>
    /// </summary>
    /// <param name="livingId">直播 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播详情（living_info）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93740"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93857"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97128"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/living/get_living_info")]
    Task<LivingGetLivingInfoResponse> GetLivingInfoAsync(
        [Query("livingid")] string livingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取观看直播统计
    /// <para>通过该接口可获取所有观看直播的人员统计（学生与游客）。</para>
    /// <para>官方业务限制：只能获取本应用创建的直播；
    /// 以响应 ending 字段判断是否拉完（0 还需继续拉取、1 已拉完），next_key 分页；
    /// 响应统计容器字段官方原文为 stat_infoes（照抄不纠正）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="LivingGetWatchStatRequest"/>，livingid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>观看统计（stat_infoes.students / stat_infoes.visitors）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93741"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93858"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97129"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/living/get_watch_stat")]
    Task<LivingGetWatchStatResponse> GetWatchStatAsync(
        [Body] LivingGetWatchStatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取未观看直播统计
    /// <para>获取未观看直播的学生统计（仅统计家长已关注「学校通知」的学生）。</para>
    /// <para>官方业务限制：只能获取本应用创建的直播；
    /// 以响应 ending 字段判断是否拉完（0 还需继续拉取、1 已拉完），next_key 分页。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="LivingGetUnwatchStatRequest"/>，livingid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>未观看统计（stat_info.students）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93742"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93859"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97130"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/living/get_unwatch_stat")]
    Task<LivingGetUnwatchStatResponse> GetUnwatchStatAsync(
        [Body] LivingGetUnwatchStatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除直播回放（官方即 POST，路由位于 /cgi-bin/living/ 段）
    /// <para>删除指定直播的回放数据。</para>
    /// <para>官方业务限制：仅允许取消当前应用创建的直播。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="LivingDeleteReplayDataRequest"/>，livingid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93743"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93860"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97131"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/delete_replay_data")]
    Task<WechatWorkResponse> DeleteReplayDataAsync(
        [Body] LivingDeleteReplayDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取观看直播统计 V2
    /// <para>获取所有观看直播的人员统计（学生、家长与游客；较 V1 新增家长观看统计）。</para>
    /// <para>官方业务限制：只能获取本应用创建的直播；
    /// 以响应 has_more 字段判断是否拉完（1 还需继续拉取、0 已拉完，与 V1 的 ending 语义相反），next_cursor 分页。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="LivingGetWatchStatV2Request"/>，livingid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>观看统计 V2（stat_info.students / stat_info.parents / stat_info.visitors）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95793"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95799"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97132"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/living/get_watch_stat_v2")]
    Task<LivingGetWatchStatV2Response> GetWatchStatV2Async(
        [Body] LivingGetWatchStatV2Request request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取未观看直播统计 V2
    /// <para>获取未观看直播的学生与家长统计
    /// （学生和学生的家长必须已经关注「学校通知」才会纳入统计；较 V1 新增家长未观看统计）。</para>
    /// <para>官方业务限制：只能获取本应用创建的直播；
    /// 以响应 has_more 字段判断是否拉完（1 还需继续拉取、0 已拉完，与 V1 的 ending 语义相反），next_cursor 分页。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="LivingGetUnwatchStatV2Request"/>，livingid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>未观看统计 V2（stat_info.students / stat_info.parents）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95795"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95800"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97133"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/living/get_unwatch_stat_v2")]
    Task<LivingGetUnwatchStatV2Response> GetUnwatchStatV2Async(
        [Body] LivingGetUnwatchStatV2Request request,
        CancellationToken cancellationToken = default);
}
