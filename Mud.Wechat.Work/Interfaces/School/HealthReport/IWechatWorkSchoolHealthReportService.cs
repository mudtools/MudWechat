// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School.HealthReport;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「家校沟通」模块健康上报域公共 SDK。
/// <para>
/// 官方仅向企业自建应用开放本域 4 个端点（使用统计、任务 ID 列表、任务详情、用户填写答案），
/// 第三方应用与服务商代开发均标注「暂不支持」；4 个端点全部收敛声明于本接口，
/// 继承链上仅声明自建子接口 <see cref="IWechatWorkInternalSchoolHealthReportService"/>
/// （形态对齐家校管理配置域仅自建 + 第三方的「官方开放面收敛」模式）。
/// </para>
/// <para>
/// 家校应用域其它功能族：<see cref="IWechatWorkSchoolService"/>（基础域）、
/// <see cref="IWechatWorkSchoolSettingService"/>（家校管理配置域）、
/// <see cref="IWechatWorkSchoolUserService"/>（学生与家长管理域）、
/// <see cref="IWechatWorkSchoolDepartmentService"/>（部门管理域）、
/// <see cref="IWechatWorkSchoolAuthService"/>（网页授权登录域）、
/// <see cref="IWechatWorkSchoolLivingService"/>（上课直播域）、
/// <see cref="IWechatWorkSchoolClassPayService"/>（班级收款域）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>）。官方权限口径：自建应用须配置到
/// 「健康上报 - 可调用接口的应用」中；第三方应用暂不支持；服务商代开发暂不支持。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolHealthReportService
{
    /// <summary>
    /// 获取健康上报使用统计
    /// <para>获取企业某天的健康上报应用使用统计。</para>
    /// <para>官方业务限制：date 仅支持获取最近 30 天内的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="HealthReportGetStatRequest"/>，date 为具体某天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>使用统计（pv 应用使用次数、uv 应用使用成员数）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93676"/></para>
    /// <para>官方权限：自建应用须配置到「健康上报 - 可调用接口的应用」中；第三方应用暂不支持；服务商代开发暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/health/get_health_report_stat")]
    Task<HealthReportGetStatResponse> GetHealthReportStatAsync(
        [Body] HealthReportGetStatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取健康上报任务 ID 列表
    /// <para>获取企业当前正在运行的上报任务 ID 列表。</para>
    /// <para>官方业务限制：limit 取值范围 1 ~ 100（默认 100）；
    /// 以响应 ending 字段判断是否拉完（0 还需继续拉取、1 已拉完）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="HealthReportGetJobIdsRequest"/>，offset/limit 分页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务 ID 列表（jobids）与分页结束标记（ending）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93677"/></para>
    /// <para>官方权限：自建应用须配置到「健康上报 - 可调用接口的应用」中；第三方应用暂不支持；服务商代开发暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/health/get_report_jobids")]
    Task<HealthReportGetJobIdsResponse> GetReportJobIdsAsync(
        [Body] HealthReportGetJobIdsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取健康上报任务详情
    /// <para>获取指定健康上报任务的详情（任务名、适用范围、汇报对象、问题模板等）。</para>
    /// <para>官方业务限制：date 仅支持获取最近 14 天数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="HealthReportGetJobInfoRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务详情（job_info，含问题模板 question_templates）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93678"/></para>
    /// <para>官方权限：自建应用须配置到「健康上报 - 可调用接口的应用」中；第三方应用暂不支持；服务商代开发暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/health/get_report_job_info")]
    Task<HealthReportGetJobInfoResponse> GetReportJobInfoAsync(
        [Body] HealthReportGetJobInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户填写答案
    /// <para>获取指定健康上报任务、指定某天的用户填写答案（按问题类型仅返回对应字段）。</para>
    /// <para>官方业务限制：date 仅支持获取最近 14 天数据；limit 最大值为 100。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="HealthReportGetAnswerRequest"/>，jobid + date + offset/limit 分页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>答案列表（answers，id_type 区分企业成员与家长/学生两类答卷人）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93679"/></para>
    /// <para>官方权限：自建应用须配置到「健康上报 - 可调用接口的应用」中；第三方应用暂不支持；服务商代开发暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/health/get_report_answer")]
    Task<HealthReportGetAnswerResponse> GetReportAnswerAsync(
        [Body] HealthReportGetAnswerRequest request,
        CancellationToken cancellationToken = default);
}
