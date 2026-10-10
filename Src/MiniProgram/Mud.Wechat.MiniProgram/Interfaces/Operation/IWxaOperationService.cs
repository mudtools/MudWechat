// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「运维中心」域 SDK（9 端点；另 1 端点 <c>getfeedbackmedia</c> 为图片二进制流，走
/// <see cref="IWxaFeedbackMediaService"/> 独立通道，随本模块一并注册）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 运维中心，2026-10-10 依据官方清单核验）：
/// <c>operation/api_*.html</c> 系列（域名配置 / 性能数据 / 访问来源 / 客户端版本 / 实时日志 / 用户反馈 / JS 错误 / 分阶段发布）。
/// </para>
/// <para>
/// <b>应答形态</b>：日志统计类（性能 / 来源 / 客户端版本 / JS 错误）的 <c>data</c> 字段随官方版本演进、
/// 逐端点互不一致，SDK 以<b>原始键值对透传</b>（<see cref="DataModels.Operation.WxaLogDataListResponse"/> 等），
/// 不猜测字段名（详情见各 DTO）。实时日志与用户反馈的字段稳定、按官方语义逐字段建模。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。所有端点官方均为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Operation", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaOperationService
{
    /// <summary>
    /// 查询域名配置。官方文档：<c>operation/api_getdomaininfo.html</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>request / ws / upload / download 四类合法域名，见 <see cref="DataModels.Operation.WxaDevInfoResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/getwxadevinfo</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/getwxadevinfo")]
    Task<DataModels.Operation.WxaDevInfoResponse> GetWxaDevInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取性能数据。官方文档：<c>operation/api_getperformance.html</c>。
    /// </summary>
    /// <param name="request">日期与模块（<c>date</c> + <c>module</c> 必填），见 <see cref="DataModels.Operation.WxaLogDateModuleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>性能数据项列表（原始键值对透传），见 <see cref="DataModels.Operation.WxaLogDataListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxaapi/log/get_performance</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>module</c> 取值：<c>all</c> / <c>js</c> / <c>network</c> / <c>run</c>。</para>
    /// </remarks>
    [Post("/wxaapi/log/get_performance")]
    Task<DataModels.Operation.WxaLogDataListResponse> GetPerformanceAsync(
        [Body] DataModels.Operation.WxaLogDateModuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取访问来源。官方文档：<c>operation/api_getscenelist.html</c>。
    /// </summary>
    /// <param name="request">日期与模块（<c>date</c> + <c>module</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>访问来源数据项列表（原始键值对透传），见 <see cref="DataModels.Operation.WxaLogDataListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxaapi/log/get_scene</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxaapi/log/get_scene")]
    Task<DataModels.Operation.WxaLogDataListResponse> GetSceneListAsync(
        [Body] DataModels.Operation.WxaLogDateModuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户端版本。官方文档：<c>operation/api_getversionlist.html</c>。
    /// </summary>
    /// <param name="request">日期与模块（<c>date</c> + <c>module</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户端版本数据项列表（原始键值对透传），见 <see cref="DataModels.Operation.WxaLogDataListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxaapi/log/get_client_version</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxaapi/log/get_client_version")]
    Task<DataModels.Operation.WxaLogDataListResponse> GetClientVersionAsync(
        [Body] DataModels.Operation.WxaLogDateModuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询实时日志。官方文档：<c>operation/api_realtimelogsearch.html</c>。
    /// </summary>
    /// <param name="request">时间窗与分页（<c>date</c> / <c>begintime</c> / <c>endtime</c> / <c>start</c> / <c>limit</c> 必填），见 <see cref="DataModels.Operation.WxaUserLogSearchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日志条数与原始日志行数组，见 <see cref="DataModels.Operation.WxaUserLogSearchResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxaapi/userlog/userlog_search</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxaapi/userlog/userlog_search")]
    Task<DataModels.Operation.WxaUserLogSearchResponse> SearchUserLogAsync(
        [Body] DataModels.Operation.WxaUserLogSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户反馈列表。官方文档：<c>operation/api_getfeedback.html</c>。
    /// </summary>
    /// <param name="request">分页与类型过滤，见 <see cref="DataModels.Operation.WxaFeedbackListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>反馈总数与列表，见 <see cref="DataModels.Operation.WxaFeedbackListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxaapi/feedback/list</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxaapi/feedback/list")]
    Task<DataModels.Operation.WxaFeedbackListResponse> GetFeedbackListAsync(
        [Body] DataModels.Operation.WxaFeedbackListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 JS 错误详情。官方文档：<c>operation/api_getjserrdetail.html</c>。
    /// </summary>
    /// <param name="request">日期与错误关键字（<c>date</c> + <c>errmsg_key</c> 必填），见 <see cref="DataModels.Operation.WxaJsErrDetailRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>错误详情数据（原始键值对透传），见 <see cref="DataModels.Operation.WxaJsErrDataResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxaapi/log/jserr_detail</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxaapi/log/jserr_detail")]
    Task<DataModels.Operation.WxaJsErrDataResponse> GetJsErrDetailAsync(
        [Body] DataModels.Operation.WxaJsErrDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询错误列表。官方文档：<c>operation/api_getjserrlist.html</c>。
    /// </summary>
    /// <param name="request">日期（<c>date</c> 必填）+ 可选错误关键字，见 <see cref="DataModels.Operation.WxaJsErrListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>错误列表数据（原始键值对透传），见 <see cref="DataModels.Operation.WxaJsErrDataResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxaapi/log/jserr_list</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxaapi/log/jserr_list")]
    Task<DataModels.Operation.WxaJsErrDataResponse> GetJsErrListAsync(
        [Body] DataModels.Operation.WxaJsErrListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分阶段发布详情。官方文档：<c>operation/api_getgrayreleaseplan.html</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分阶段发布计划（<c>gray_release_plan</c>，原始键值对透传），见 <see cref="DataModels.Operation.WxaGrayReleasePlanResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/getgrayreleaseplan</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/getgrayreleaseplan")]
    Task<DataModels.Operation.WxaGrayReleasePlanResponse> GetGrayReleasePlanAsync(
        CancellationToken cancellationToken = default);
}